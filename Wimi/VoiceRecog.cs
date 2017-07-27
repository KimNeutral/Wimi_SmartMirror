using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using Windows.ApplicationModel.Core;
using Windows.Media.Capture;
using Windows.Media.SpeechRecognition;
using Windows.UI;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;

namespace Wimi
{
    public partial class MainPage : Page
    {

        private MainPage rootPage;
        private CoreDispatcher dispatcher; //UI쓰레드 화면 업데이트를 위해 필요.
        private SpeechRecognizer speechRecognizer;
        private DispatcherTimer RecogCheckTimer; //디버깅용
        MediaCapture capture;

        private bool isListening;

        //제약조건
        private SpeechRecognitionListConstraint helloConstraint;
        private SpeechRecognitionListConstraint TellWeatherConstraint;
        private SpeechRecognitionListConstraint TestConstraint;
        private SpeechRecognitionListConstraint PlayRandomMusicConstraint;
        private SpeechRecognitionListConstraint StopMusicConstraint;
        private SpeechRecognitionListConstraint PauseMusicConstraint;
        private SpeechRecognitionListConstraint PlayMusicConstraint;
        private SpeechRecognitionListConstraint ShowNewsConstraint;
        private SpeechRecognitionListConstraint ShowBusConstraint;
        private SpeechRecognitionListConstraint FullScreenConstraint;
        //조명
        /*
        private SpeechRecognitionListConstraint TurnOnLightConstraint;
        private SpeechRecognitionListConstraint TurnOffLightConstraint;
        private SpeechRecognitionListConstraint ChangeLightModeOn;
        private SpeechRecognitionListConstraint ChangeLightModeOff;

        private SpeechRecognitionListConstraint RedColorLightConstraint;
        private SpeechRecognitionListConstraint BrownColorLightConstraint;
        private SpeechRecognitionListConstraint YellowColorLightConstraint;
        private SpeechRecognitionListConstraint GreenColorLightConstraint;
        private SpeechRecognitionListConstraint BlueColorLightConstraint;
        private SpeechRecognitionListConstraint PinkColorLightConstraint;
        private SpeechRecognitionListConstraint PurpleColorLightConstraint;
        private SpeechRecognitionListConstraint WhiteColorLightConstraint;
        */

        public async void Recognize()
        {
            if (isListening == false)
            {
                if (speechRecognizer.State == SpeechRecognizerState.Idle)
                {
                    try
                    {
                        await speechRecognizer.ContinuousRecognitionSession.StartAsync(); //SpeechContinuousRecognitionMode.PauseOnRecognition
                        isListening = true;
                        Debug.WriteLine("ContinuousRecognitionSession StartAsync");
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine("ContinuousRecognitionSession StartAsync " + ex.Message);
                    }
                }
            }
            else
            {
                isListening = false;

                if (speechRecognizer.State != SpeechRecognizerState.Idle)
                {
                    await speechRecognizer.ContinuousRecognitionSession.CancelAsync();
                    Debug.WriteLine("ContinuousRecognitionSession CancelAsync " + speechRecognizer.State.ToString());
                }
            }

        }

        private async Task InitializeRecognizer()
        {
            isListening = false;
            RecogCheckTimer = new DispatcherTimer();
            rootPage = this;
            dispatcher = CoreWindow.GetForCurrentThread().Dispatcher;

            RecogCheckTimer.Interval = new TimeSpan(0, 0, 10);
            RecogCheckTimer.Tick += RecogCheckTimer_Tick;
            RecogCheckTimer.Start();
#if false
            bool permissionGained = await AudioCapturePermissions.RequestMicrophonePermission();
            if (permissionGained)
#else
            capture = await AudioCapturePermissions.RequestMicrophonePermission();
            if(capture != null)
#endif
            {
                capture.Failed += Capture_Failed;
                capture.RecordLimitationExceeded += Capture_RecordLimitationExceeded;
                if (speechRecognizer != null)
                {
                    // cleanup prior to re-initializing this scenario.
                    speechRecognizer.ContinuousRecognitionSession.Completed -= ContinuousRecognitionSession_Completed;
                    speechRecognizer.ContinuousRecognitionSession.ResultGenerated -= ContinuousRecognitionSession_ResultGenerated;
                    speechRecognizer.StateChanged -= SpeechRecognizer_StateChanged;
                    speechRecognizer.RecognitionQualityDegrading -= SpeechRecognizer_RecognitionQualityDegrading;

                    speechRecognizer.Dispose();
                    speechRecognizer = null;
                }

                speechRecognizer = new SpeechRecognizer();

#if true //이 설정들은 어떻게 동작하는지 실제 체크해봐야 함
                //https://blogs.windows.com/buildingapps/2016/05/16/using-speech-in-your-uwp-apps-its-good-to-talk/#1oeBioSBAzPIK3bJ.97
                //speechRecognizer.Timeouts.InitialSilenceTimeout = TimeSpan.FromSeconds(6.0);
                //speechRecognizer.Timeouts.BabbleTimeout = TimeSpan.FromSeconds(5.0);
                //speechRecognizer.Timeouts.EndSilenceTimeout = TimeSpan.FromSeconds(1.0);
                speechRecognizer.ContinuousRecognitionSession.AutoStopSilenceTimeout = TimeSpan.MaxValue; //new TimeSpan(1, 0, 0);
#endif
                speechRecognizer.StateChanged += SpeechRecognizer_StateChanged;

                AddConstraints();

                SpeechRecognitionCompilationResult result = await speechRecognizer.CompileConstraintsAsync();

                if (result.Status != SpeechRecognitionResultStatus.Success)
                {
                    //resultTextBlock.Text = "Unable to compile grammar.";
                }

                speechRecognizer.ContinuousRecognitionSession.Completed += ContinuousRecognitionSession_Completed;
                speechRecognizer.ContinuousRecognitionSession.ResultGenerated += ContinuousRecognitionSession_ResultGenerated;
                speechRecognizer.RecognitionQualityDegrading += SpeechRecognizer_RecognitionQualityDegrading;

                Recognize();

                resultTextBlock.Text = string.Format("음성인식이 시작되었습니다.");
                tbRecog.Text = "Listening...";
            }
            else
            {
                resultTextBlock.Text = "Permission to access capture resources was not given by the user, reset the application setting in Settings->Privacy->Microphone.";
            }

            return;
        }

        private void Capture_RecordLimitationExceeded(MediaCapture sender)
        {
            Debug.WriteLine("마이크 캡쳐 RecordLimitationExceeded");
        }

        private void Capture_Failed(MediaCapture sender, MediaCaptureFailedEventArgs errorEventArgs)
        {
            Debug.WriteLine("마이크 캡쳐 Failed");
        }

        private void RecogCheckTimer_Tick(object sender, object e)
        {
            //Debug.WriteLine("상태체크: " + speechRecognizer.State.ToString());
        }

        private async void ContinuousRecognitionSession_ResultGenerated(SpeechContinuousRecognitionSession sender, SpeechContinuousRecognitionResultGeneratedEventArgs args)
        {
            try
            {
                string tag = "unknown";
                if (args.Result.Constraint != null)
                {
                    tag = args.Result.Constraint.Tag;
                }
                //Debug.WriteLine(iscalled);

                if (args.Result.Confidence == SpeechRecognitionConfidence.Medium ||
                    args.Result.Confidence == SpeechRecognitionConfidence.High ||
                    args.Result.Confidence == SpeechRecognitionConfidence.Low)
                {
                    Debug.WriteLine(string.Format("********************************************** 인식: '{0}', (태그: '{1}', 정확도: {2})", args.Result.Text, tag, args.Result.Confidence.ToString()));

                    await dispatcher.RunAsync(CoreDispatcherPriority.Normal, async () =>
                    {
                         //Confidence
                        resultTextBlock.Text = string.Format("Heard: {0}, Tag: {1}, Confidence: {2}", args.Result.Text, tag, args.Result.Confidence.ToString());
                        if (!string.IsNullOrEmpty(tag))
                        {
                            switch (tag)
                            {
                                case "Hello":
                                    ClearLeftPanel();
                                    await DetectCalledByWimi();
                                    break;
                                case "TellWeather":
                                    ShowForecast();
                                    TellmeWeatherAsync();
                                    break;
                                //case "PlayRandomMusic":
                                //    await PlayRandomMusic();
                                //    break;
                                case "PauseMusic":
                                        PauseMusic();
                                        break;
                                case "StopMusic":
                                    StopMusic();
                                    break;
                                case "PlayMusic":
                                    await PlayRandomMusic();
                                    break;
                                case "FullScreen":
                                    SetFullScreen();
                                    break;
                                case "ShowNews":
                                    ShowNews();
                                    break;
                                case "ShowBus":
                                    ShowBus();
                                    break;
                                case "LightModeOn":
                                    HueAtrBool = await HueControl.HueEffect(1);
                                    break;
                                case "LightModeOff":
                                    HueAtrBool = await HueControl.HueEffect(0);
                                    break;
                                case "TurnOn":
                                    HueAtrBool = await HueControl.HueLightOn();
                                    break;
                                case "TurnOff":
                                    HueAtrBool = await HueControl.HueLightOff();
                                    break;
                                case "RedColor":
                                    HueAtrBool = await HueControl.SetColor("red");
                                    break;
                                case "BrownColor":
                                    HueAtrBool = await HueControl.SetColor("brown");
                                    break;
                                case "YellowColor":
                                    HueAtrBool = await HueControl.SetColor("yellow");
                                    break;
                                case "GreenColor":
                                    HueAtrBool = await HueControl.SetColor("green");
                                    break;
                                case "BlueColor":
                                    HueAtrBool = await HueControl.SetColor("blue");
                                    break;
                                case "PurpleColor":
                                    HueAtrBool = await HueControl.SetColor("purple");
                                    break;
                                case "PinkColor":
                                    HueAtrBool = await HueControl.SetColor("pink");
                                    break;
                                case "WhiteColor":
                                    HueAtrBool = await HueControl.SetColor("white");
                                    break;
                                default:
                                    {
                                        resultTextBlock.Text = "등록된 명령어가 아닙니다";
                                        break;
                                    }
                            }
                        }

                    });
                }
                else if (args.Result.Confidence == SpeechRecognitionConfidence.Rejected)
                {
                    await dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                    {
                    //SetVoice("다시 말해주세요."); //Please say it again. //Tell me again. //What did you say? //Say what? //다른건 발음이 이상하게 나옴 ㅋㅋ
                    resultTextBlock.Text = string.Format("음성인식이 실패하였습니다.");
                        Debug.WriteLine("ContinuousRecognitionSession_ResultGenerated - Rejected");
                    });
                }
                else
                {
                    resultTextBlock.Text = string.Format("No Confidence.");
                    Debug.WriteLine("ContinuousRecognitionSession_ResultGenerated - {0}, 아무조건도 안걸림", args.Result.Confidence);
                }
            }
            catch(Exception ex)
            {
                Debug.WriteLine("ContinuousRecognitionSession_ResultGenerated :" + ex.Message);
            }
        }

        private async void ContinuousRecognitionSession_Completed(SpeechContinuousRecognitionSession sender, SpeechContinuousRecognitionCompletedEventArgs args)
        {
            Debug.WriteLine("ContinuousRecognitionSession_Completed, Status = {0}", args.Status);
            if (args.Status != SpeechRecognitionResultStatus.Success)
            {
                await dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                {
                    resultTextBlock.Text = string.Format("ContinuousRecognitionSession_Completed, status = {0}", args.Status.ToString());
                    if (args.Status.Equals(SpeechRecognitionResultStatus.PauseLimitExceeded))
                    {
                        //do nothing...
                    }
                    isListening = false;
                    Recognize();
                });
            }
            else if(args.Status == SpeechRecognitionResultStatus.Success)
            {
                isListening = false;
                Recognize();//chris: 음성인식이 끝난후 음성세션이 완료되게 임시방편으로 처리하였으므로 다시 시작되도록 한다.
            }
        }

        private async void SpeechRecognizer_StateChanged(SpeechRecognizer sender, SpeechRecognizerStateChangedEventArgs args)
        {
            Debug.WriteLine("SpeechRecognizer state = {0}", args.State);
            try
            {
                await dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                {
                    tbVoiceRecogState.Text = args.State.ToString();

                    switch (args.State)
                    {
                        case SpeechRecognizerState.Idle:
                            {
                                break;
                            }
                        case SpeechRecognizerState.Capturing:
                            {
                                tbMicSymbol.Foreground = new SolidColorBrush(Colors.DeepPink);
                                break;
                            }
                        case SpeechRecognizerState.Processing:
                            {
                                break;
                            }
                        case SpeechRecognizerState.SoundStarted:
                            {
                                tbMicSymbol.Foreground = new SolidColorBrush(Colors.DeepPink);
                                resultTextBlock.Text = string.Empty;
                                break;
                            }
                        case SpeechRecognizerState.SoundEnded:
                            {
                                tbMicSymbol.Foreground = new SolidColorBrush(Colors.Gray);
                                Recognize(); //chris: 음성인식이 끝난후 다시 시작되도록, 임시방편
                                break;
                            }
                        case SpeechRecognizerState.SpeechDetected:
                            {
                                break;
                            }
                        case SpeechRecognizerState.Paused:
                            {
                                break;
                            }
                        default:
                            {
                                break;
                            }
                    }
                });
            }
            catch(Exception ex)
            {
                Debug.WriteLine("SpeechRecognizer_StateChanged :" + ex.Message);
            }
            
        }

        private async void SpeechRecognizer_RecognitionQualityDegrading(SpeechRecognizer sender, SpeechRecognitionQualityDegradingEventArgs args)
        {
            // Create an instance of a speech synthesis engine (voice).
            //await dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            //{

            //});
            var speechSynthesizer = new Windows.Media.SpeechSynthesis.SpeechSynthesizer();
            Debug.WriteLine("SpeechRecognizer_RecognitionQualityDegrading, Result = {0}", args.Problem.ToString());
            // If input speech is too quiet, prompt the user to speak louder.
            if (args.Problem != Windows.Media.SpeechRecognition.SpeechRecognitionAudioProblem.None) //TooQuiet
            {
                // Generate the audio stream from plain text.
                Windows.Media.SpeechSynthesis.SpeechSynthesisStream stream;
                try
                {
                    stream = await speechSynthesizer.SynthesizeTextToStreamAsync("Try speaking louder");
                    stream.Seek(0);
                }
                catch (Exception)
                {
                    stream = null;
                }

                // Send the stream to the MediaElement declared in XAML.
                await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.High, () =>
                {
                    resultTextBlock.Text = "RecognitionQualityDegrading: " + args.Problem.ToString();
                    this.media.SetSource(stream, stream.ContentType);
                });
            }
        }

        private async void CleanSpeechRecognizer()
        {
            RecogCheckTimer.Stop();

            if (speechRecognizer != null)
            {
                if (isListening)
                {
                    await speechRecognizer.ContinuousRecognitionSession.CancelAsync();
                    isListening = false;
                }

                speechRecognizer.ContinuousRecognitionSession.Completed -= ContinuousRecognitionSession_Completed;
                speechRecognizer.ContinuousRecognitionSession.ResultGenerated -= ContinuousRecognitionSession_ResultGenerated;
                speechRecognizer.StateChanged -= SpeechRecognizer_StateChanged;

                speechRecognizer.Dispose();
                speechRecognizer = null;
            }
        }

        private void tbMicSymbol_DoubleTapped(object sender, Windows.UI.Xaml.Input.DoubleTappedRoutedEventArgs e)
        {
            //chris: 강제로 다시 시작되도록
            Debug.WriteLine("Forced SpeechRecognizer Start");
            isListening = false;
            Recognize();
        }

        public void AddConstraints()
        {
            //{"들을 내용1", "내용2"},"태그이름");
            helloConstraint = new SpeechRecognitionListConstraint(new List<string>()
            { "wimi" }, "Hello");
            TellWeatherConstraint = new SpeechRecognitionListConstraint(new List<string>()
            { "Tell me forecast", "Tell me Weather", "Tell me weather forecast","today Weather"}, "TellWeather");
            TestConstraint = new SpeechRecognitionListConstraint(new List<string>()
            { "VoiceTest"}, "Test");
            PlayRandomMusicConstraint = new SpeechRecognitionListConstraint(new List<string>()
            { "Play Random Music"}, "PlayRandomMusic");
            StopMusicConstraint = new SpeechRecognitionListConstraint(new List<string>()
            { "Stop Music"}, "StopMusic");
            PauseMusicConstraint = new SpeechRecognitionListConstraint(new List<string>()
            { "Pause Music"}, "PauseMusic");
            PlayMusicConstraint = new SpeechRecognitionListConstraint(new List<string>()
            {"Play Music"}, "PlayMusic");
            ShowNewsConstraint = new SpeechRecognitionListConstraint(new List<string>()
            {"Show Todays News"}, "ShowNews");
            ShowBusConstraint = new SpeechRecognitionListConstraint(new List<string>()
            {"Where is Bus"}, "ShowBus");
            FullScreenConstraint = new SpeechRecognitionListConstraint(new List<string>()
            {"Set FullScreen"}, "FullScreen");
            //조명명령어 추가 파이팅 ㅎ
            /*
            TurnOnLightConstraint = new SpeechRecognitionListConstraint(new List<string>()
            { "turn On the Light"}, "TurnOn");
            TurnOffLightConstraint = new SpeechRecognitionListConstraint(new List<string>()
            { "turn Off the Light"}, "TurnOff");
            ChangeLightModeOn = new SpeechRecognitionListConstraint(new List<string>()
            { "Change Light Mode On","Loop colors start"}, "LightModeOn");
            ChangeLightModeOff = new SpeechRecognitionListConstraint(new List<string>()
            { "Change Light Mode Off","Loop colors stop"}, "LightModeOff");
            RedColorLightConstraint = new SpeechRecognitionListConstraint(new List<string>()
            { "Change color Red","turn on Red"}, "RedColor");
            BrownColorLightConstraint = new SpeechRecognitionListConstraint(new List<string>()
            { "Change color Brown","turn on Brown"}, "BrownColor");
            YellowColorLightConstraint = new SpeechRecognitionListConstraint(new List<string>()
            { "Change color Yellow","turn on Yellow"}, "YellowColor");
            GreenColorLightConstraint = new SpeechRecognitionListConstraint(new List<string>()
            { "Change color Green","turn on Green"}, "GreenColor");
            BlueColorLightConstraint = new SpeechRecognitionListConstraint(new List<string>()
            { "Change color Blue","turn on Blue"}, "BlueColor");
            PinkColorLightConstraint = new SpeechRecognitionListConstraint(new List<string>()
            { "Change color Pink","turn on Pink"}, "PinkColor");
            PurpleColorLightConstraint = new SpeechRecognitionListConstraint(new List<string>()
            { "Change color Purple","turn on Purple"}, "PurpleColor");
            WhiteColorLightConstraint = new SpeechRecognitionListConstraint(new List<string>()
            { "Change color White","turn on White"}, "WhiteColor");*/

            speechRecognizer.Constraints.Add(helloConstraint);
            speechRecognizer.Constraints.Add(TellWeatherConstraint);
            speechRecognizer.Constraints.Add(TestConstraint);
            speechRecognizer.Constraints.Add(PlayRandomMusicConstraint);
            speechRecognizer.Constraints.Add(StopMusicConstraint);
            speechRecognizer.Constraints.Add(PauseMusicConstraint);
            speechRecognizer.Constraints.Add(PlayMusicConstraint);
            speechRecognizer.Constraints.Add(FullScreenConstraint);
            speechRecognizer.Constraints.Add(ShowNewsConstraint);
            speechRecognizer.Constraints.Add(ShowBusConstraint);

            //speechRecognizer.Constraints.Add(TurnOnLightConstraint);
            //speechRecognizer.Constraints.Add(TurnOffLightConstraint);
            //speechRecognizer.Constraints.Add(ChangeLightModeOn);
            //speechRecognizer.Constraints.Add(ChangeLightModeOff);
            //speechRecognizer.Constraints.Add(RedColorLightConstraint);
            //speechRecognizer.Constraints.Add(YellowColorLightConstraint);
            //speechRecognizer.Constraints.Add(BrownColorLightConstraint);
            //speechRecognizer.Constraints.Add(GreenColorLightConstraint);
            //speechRecognizer.Constraints.Add(BlueColorLightConstraint);
            //speechRecognizer.Constraints.Add(PinkColorLightConstraint);
            //speechRecognizer.Constraints.Add(PurpleColorLightConstraint);
            //speechRecognizer.Constraints.Add(WhiteColorLightConstraint);

        }

        public void RemoveConstraints()
        {
            speechRecognizer.Constraints.Remove(helloConstraint);

            speechRecognizer.Constraints.Remove(TellWeatherConstraint);
            speechRecognizer.Constraints.Remove(TestConstraint);
            speechRecognizer.Constraints.Remove(PlayRandomMusicConstraint);
            speechRecognizer.Constraints.Remove(StopMusicConstraint);
            speechRecognizer.Constraints.Remove(PauseMusicConstraint);
            speechRecognizer.Constraints.Remove(PlayMusicConstraint);
            speechRecognizer.Constraints.Remove(FullScreenConstraint);
            speechRecognizer.Constraints.Remove(ShowNewsConstraint);
            speechRecognizer.Constraints.Remove(ShowNewsConstraint);

            //speechRecognizer.Constraints.Remove(TurnOnLightConstraint);
            //speechRecognizer.Constraints.Remove(TurnOffLightConstraint);
            //speechRecognizer.Constraints.Remove(ChangeLightModeOn);
            //speechRecognizer.Constraints.Remove(ChangeLightModeOff);
            //speechRecognizer.Constraints.Remove(RedColorLightConstraint);
            //speechRecognizer.Constraints.Remove(YellowColorLightConstraint);
            //speechRecognizer.Constraints.Remove(BrownColorLightConstraint);
            //speechRecognizer.Constraints.Remove(GreenColorLightConstraint);
            //speechRecognizer.Constraints.Remove(BlueColorLightConstraint);
            //speechRecognizer.Constraints.Remove(PinkColorLightConstraint);
            //speechRecognizer.Constraints.Remove(PurpleColorLightConstraint);
            //speechRecognizer.Constraints.Remove(WhiteColorLightConstraint);
        }
    }
}
