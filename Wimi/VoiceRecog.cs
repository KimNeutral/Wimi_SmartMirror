using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using Windows.ApplicationModel.Core;
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
        private DispatcherTimer timer;

        private bool isListening;

        //제약조건
        private SpeechRecognitionListConstraint helloConstraint;
        private SpeechRecognitionListConstraint noticeConstraint;
        private SpeechRecognitionListConstraint ShowWeatherConstraint;
        private SpeechRecognitionListConstraint TellWeatherConstraint;
        private SpeechRecognitionListConstraint TestConstraint;
        private SpeechRecognitionListConstraint PlayMusicConstraint;
        private SpeechRecognitionListConstraint StopMusicConstraint;
        private SpeechRecognitionListConstraint PauseMusicConstraint;
        //조-명
        /**/
        private SpeechRecognitionListConstraint TurnOnLightConstraint;
        private SpeechRecognitionListConstraint TurnOffLightConstraint;
        private SpeechRecognitionListConstraint ChangeLightModeOn;
        private SpeechRecognitionListConstraint ChangeLightModeOff;

        private SpeechRecognitionListConstraint RedColorLightConstraint;
        private SpeechRecognitionListConstraint OrangeColorLightConstraint;
        private SpeechRecognitionListConstraint YellowColorLightConstraint;
        private SpeechRecognitionListConstraint GreenColorLightConstraint;
        private SpeechRecognitionListConstraint BlueColorLightConstraint;
        private SpeechRecognitionListConstraint PinkColorLightConstraint;
        private SpeechRecognitionListConstraint PurpleColorLightConstraint;
        private SpeechRecognitionListConstraint WhiteColorLightConstraint;/**/

        public async void Recognize()
        {
            if (isListening == false)
            {
                if (speechRecognizer.State == SpeechRecognizerState.Idle)
                {
                    try
                    {
                        await speechRecognizer.ContinuousRecognitionSession.StartAsync();
                        isListening = true;
                    }
                    catch (Exception ex)
                    {
                        string str = ex.Message;
                    }
                }
            }
            else
            {
                isListening = false;

                if (speechRecognizer.State != SpeechRecognizerState.Idle)
                {
                    await speechRecognizer.ContinuousRecognitionSession.CancelAsync();
                }
            }

        }

        private async Task InitializeRecognizer()
        {
            isListening = false;
            timer = new DispatcherTimer();
            rootPage = this;
            dispatcher = CoreWindow.GetForCurrentThread().Dispatcher;
            timer.Interval = new TimeSpan(0, 0, 1);

            bool permissionGained = await AudioCapturePermissions.RequestMicrophonePermission();
            if (permissionGained)
            {
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

#if true //timeout 안되도록 1시간정도로 설정
                //speechRecognizer.Timeouts.EndSilenceTimeout = new TimeSpan(1, 0, 0);
                //speechRecognizer.Timeouts.InitialSilenceTimeout = new TimeSpan(1, 0, 0);
                //speechRecognizer.Timeouts.BabbleTimeout = new TimeSpan(1, 0, 0);
                //speechRecognizer.ContinuousRecognitionSession.AutoStopSilenceTimeout = TimeSpan.MaxValue; //new TimeSpan(1, 0, 0);
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

                //SetVoice("음성인식이 시작되었습니다.");
                resultTextBlock.Text = string.Format("음성인식이 시작되었습니다.");
                //tbVoiceRecogReady.Foreground = new SolidColorBrush(Colors.LightGreen);
                tbVoiceRecogReady.Text = "\xE1D6"; //E1D6 //EC71
            }
            else
            {
                resultTextBlock.Text = "Permission to access capture resources was not given by the user, reset the application setting in Settings->Privacy->Microphone.";
            }
        }

        private async void SpeechRecognizer_RecognitionQualityDegrading(SpeechRecognizer sender, SpeechRecognitionQualityDegradingEventArgs args)
        {
            // Create an instance of a speech synthesis engine (voice).
            var speechSynthesizer = new Windows.Media.SpeechSynthesis.SpeechSynthesizer();
            Debug.WriteLine("SpeechRecognizer_RecognitionQualityDegrading, Result = {0}", args.Problem.ToString());
            // If input speech is too quiet, prompt the user to speak louder.
            if (args.Problem == Windows.Media.SpeechRecognition.SpeechRecognitionAudioProblem.TooQuiet)
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
                    this.media.SetSource(stream, stream.ContentType);
                });
            }
        }

        private async void ContinuousRecognitionSession_ResultGenerated(SpeechContinuousRecognitionSession sender, SpeechContinuousRecognitionResultGeneratedEventArgs args)
        {
            Debug.WriteLine("ContinuousRecognitionSession_ResultGenerated, Result = {0}", args.Result.Confidence);

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
                await dispatcher.RunAsync(CoreDispatcherPriority.Normal, async () =>
                {
                    resultTextBlock.Text = string.Format("Heard: '{0}', (Tag: '{1}', Confidence: {2})", args.Result.Text, tag, args.Result.Confidence.ToString());
                    if (!string.IsNullOrEmpty(tag))
                    {
                        switch (tag)
                        {
                            case "Hello":
                                SetVoice("왜 불러?");
                                break;
                            case "Sleep":
                                SetVoice("가서 자세요");
                                break;
                            case "ShowWeather":
                                break;
                            case "TellWeather":
                                TellmeWeatherAsync();
                                break;
                            case "PlayMusic":
                                await PlayMusic();
                                break;
                            case "PauseMusic":
                                PauseMusic();
                                break;
                            case "StopMusic":
                                StopMusic();
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
                            case "OrangeColor":
                                HueAtrBool = await HueControl.SetColor("orange");
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

                        }
                        /*if (tag == "Hello")
                        {
                            SetVoice("왜 불러?");
                        }
                        else if (tag == "Sleep")
                        {
                            SetVoice("가서 자세요");
                        }
                        else if (tag == "ShowWeather")
                        {
                            //SetVoice("오늘의 날씨입니다.");
                        }
                        else if (tag == "TellWeather")
                        {
                            TellmeWeatherAsync();
                        }
                        else if (tag == "PlayMusic")
                        {
                            await PlayMusic();

                        }
                        else if (tag == "PauseMusic")
                        {
                            PauseMusic();
                        }
                        else if (tag == "StopMusic")
                        {
                            StopMusic();
                        }/**/
                    }

                });
            }
            else if (args.Result.Confidence == SpeechRecognitionConfidence.Rejected)
            {
                await dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                {
                    //SetVoice("다시 말해주세요."); //Please say it again. //Tell me again. //What did you say? //Say what? //다른건 발음이 이상하게 나옴 ㅋㅋ
                    resultTextBlock.Text = string.Format("음성인식이 실패하였습니다.");
                });
            }
            else
            {
                Debug.WriteLine("ContinuousRecognitionSession_ResultGenerated?????");
            }
        }


        private async void ContinuousRecognitionSession_Completed(SpeechContinuousRecognitionSession sender, SpeechContinuousRecognitionCompletedEventArgs args)
        {
            Debug.WriteLine("ContinuousRecognitionSession_Completed, Status = {0}", args.Status);
            if (args.Status != SpeechRecognitionResultStatus.Success)
            {
                await dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                {
                    isListening = false;
                });
            }
            if (args.Status == SpeechRecognitionResultStatus.PauseLimitExceeded)
            {
                await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                {
                    Debug.WriteLine("*********PauseLimitExceeded*********");

                    Recognize();
                });
            }
        }

        private async void SpeechRecognizer_StateChanged(SpeechRecognizer sender, SpeechRecognizerStateChangedEventArgs args)
        {
            await dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                Debug.WriteLine("SpeechRecognizer_StateChanged, state = {0}", args.State);
            });
            if (args.State.Equals(SpeechRecognitionResultStatus.PauseLimitExceeded))
            {
                Debug.WriteLine("*********PauseLimitExceeded*********");

                Recognize();
            }
        }


        private async void CleanSpeechRecognizer()
        {
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

        public void AddConstraints()
        {
            //{"들을 내용1", "내용2"},"태그이름");
            helloConstraint = new SpeechRecognitionListConstraint(new List<string>()
            { "wimi", "Hello" }, "Hello");
            noticeConstraint = new SpeechRecognitionListConstraint(new List<string>()
            { "I am tired" ,"i'm too tired"}, "Sleep");
            ShowWeatherConstraint = new SpeechRecognitionListConstraint(new List<string>()
            { "show me forecast", "show me Weather", "show me weather forecast" }, "ShowWeather");
            TellWeatherConstraint = new SpeechRecognitionListConstraint(new List<string>()
            { "Tell me forecast", "Tell me Weather", "Tell me weather forecast","today Weather"}, "TellWeather");
            TestConstraint = new SpeechRecognitionListConstraint(new List<string>()
            { "VoiceTest"}, "Test");
            PlayMusicConstraint = new SpeechRecognitionListConstraint(new List<string>()
            { "Play Music"}, "PlayMusic");
            StopMusicConstraint = new SpeechRecognitionListConstraint(new List<string>()
            { "Stop Music"}, "StopMusic");
            PauseMusicConstraint = new SpeechRecognitionListConstraint(new List<string>()
            { "Pause Music"}, "PauseMusic");
            //조명명령어 추가 파이팅 ㅎ
            /**/
            TurnOnLightConstraint = new SpeechRecognitionListConstraint(new List<string>()
            { "turn On the Light"}, "TurnOn");
            TurnOffLightConstraint = new SpeechRecognitionListConstraint(new List<string>()
            { "turn On the Light"}, "TurnOff");
            ChangeLightModeOn = new SpeechRecognitionListConstraint(new List<string>()
            { "Change Light Mode On","Loop colors start"}, "LightModeOn");
            ChangeLightModeOff = new SpeechRecognitionListConstraint(new List<string>()
            { "Change Light Mode Off","Loop colors stop"}, "LightModeOff");
            RedColorLightConstraint = new SpeechRecognitionListConstraint(new List<string>()
            { "Change color Red","turn on Red Light","Red Light"}, "RedColor");
            YellowColorLightConstraint = new SpeechRecognitionListConstraint(new List<string>()
            { "Change color Orange","turn on Orange Light","Orange Light"}, "OrangeColor");
            OrangeColorLightConstraint = new SpeechRecognitionListConstraint(new List<string>()
            { "Change color Yellow","turn on Yellow Light","Yellow Light"}, "YellowColor");
            GreenColorLightConstraint = new SpeechRecognitionListConstraint(new List<string>()
            { "Change color Green","turn on Green Light","Green Light"}, "GreenColor");
            BlueColorLightConstraint = new SpeechRecognitionListConstraint(new List<string>()
            { "Change color Blue","turn on Blue Light","Blue Light"}, "BlueColor");
            PinkColorLightConstraint = new SpeechRecognitionListConstraint(new List<string>()
            { "Change color Pink","turn on Pink Light","Pink Light"}, "PinkColor");
            PurpleColorLightConstraint = new SpeechRecognitionListConstraint(new List<string>()
            { "Change color purple","turn on purple Light","purple Light"}, "PurpleColor");
            WhiteColorLightConstraint = new SpeechRecognitionListConstraint(new List<string>()
            { "Change color white","turn on white Light","white Light"}, "WhiteColor");/**/

            speechRecognizer.Constraints.Add(helloConstraint);
            speechRecognizer.Constraints.Add(noticeConstraint);
            speechRecognizer.Constraints.Add(ShowWeatherConstraint);
            speechRecognizer.Constraints.Add(TellWeatherConstraint);
            speechRecognizer.Constraints.Add(TestConstraint);
            speechRecognizer.Constraints.Add(PlayMusicConstraint);
            speechRecognizer.Constraints.Add(StopMusicConstraint);
            speechRecognizer.Constraints.Add(PauseMusicConstraint);
            speechRecognizer.Constraints.Add(TurnOnLightConstraint);
            speechRecognizer.Constraints.Add(TurnOffLightConstraint);
            speechRecognizer.Constraints.Add(ChangeLightModeOn);
            speechRecognizer.Constraints.Add(ChangeLightModeOff);
            speechRecognizer.Constraints.Add(RedColorLightConstraint);
            speechRecognizer.Constraints.Add(YellowColorLightConstraint);
            speechRecognizer.Constraints.Add(OrangeColorLightConstraint);
            speechRecognizer.Constraints.Add(GreenColorLightConstraint);
            speechRecognizer.Constraints.Add(BlueColorLightConstraint);
            speechRecognizer.Constraints.Add(PinkColorLightConstraint);
            speechRecognizer.Constraints.Add(PurpleColorLightConstraint);
            speechRecognizer.Constraints.Add(WhiteColorLightConstraint);

        }

        public void RemoveConstraints()
        {
            speechRecognizer.Constraints.Remove(helloConstraint);
            speechRecognizer.Constraints.Remove(noticeConstraint);
            speechRecognizer.Constraints.Remove(ShowWeatherConstraint);
            speechRecognizer.Constraints.Remove(TellWeatherConstraint);
            speechRecognizer.Constraints.Remove(TestConstraint);
            speechRecognizer.Constraints.Remove(PlayMusicConstraint);
            speechRecognizer.Constraints.Remove(StopMusicConstraint);
            speechRecognizer.Constraints.Remove(PauseMusicConstraint);
            speechRecognizer.Constraints.Remove(TurnOnLightConstraint);
            speechRecognizer.Constraints.Remove(TurnOffLightConstraint);
            speechRecognizer.Constraints.Remove(ChangeLightModeOn);
            speechRecognizer.Constraints.Remove(ChangeLightModeOff);
            speechRecognizer.Constraints.Remove(RedColorLightConstraint);
            speechRecognizer.Constraints.Remove(YellowColorLightConstraint);
            speechRecognizer.Constraints.Remove(OrangeColorLightConstraint);
            speechRecognizer.Constraints.Remove(GreenColorLightConstraint);
            speechRecognizer.Constraints.Remove(BlueColorLightConstraint);
            speechRecognizer.Constraints.Remove(PinkColorLightConstraint);
            speechRecognizer.Constraints.Remove(PurpleColorLightConstraint);
            speechRecognizer.Constraints.Remove(WhiteColorLightConstraint);
        }
    }
}
