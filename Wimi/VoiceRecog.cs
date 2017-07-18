using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using Windows.ApplicationModel.Core;
using Windows.Media.SpeechRecognition;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Wimi
{
    public partial class MainPage : Page
    {
        private MainPage rootPage;
        private CoreDispatcher dispatcher; //UI쓰레드 화면 업데이트를 위해 필요.
        private SpeechRecognizer speechRecognizer;
        private DispatcherTimer timer;

        private bool isListening;
        private bool iscalled = false;

        private int cntTime = 0;
        //제약조건
        private SpeechRecognitionListConstraint helloConstraint;
        private SpeechRecognitionListConstraint noticeConstraint;
        private SpeechRecognitionListConstraint ShowWeatherConstraint;
        private SpeechRecognitionListConstraint TellWeatherConstraint;


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
            timer.Tick += Timer_Tick;
            bool permissionGained = await AudioCapturePermissions.RequestMicrophonePermission();
            if (permissionGained)
            {
                if (speechRecognizer != null)
                {
                    // cleanup prior to re-initializing this scenario.
                    speechRecognizer.ContinuousRecognitionSession.Completed -= ContinuousRecognitionSession_Completed;
                    speechRecognizer.ContinuousRecognitionSession.ResultGenerated -= ContinuousRecognitionSession_ResultGenerated;
                    speechRecognizer.StateChanged -= SpeechRecognizer_StateChanged;

                    speechRecognizer.Dispose();
                    speechRecognizer = null;
                }

                speechRecognizer = new SpeechRecognizer();
                
#if true //timeout 안되도록 1시간정도로 설정
                //speechRecognizer.Timeouts.EndSilenceTimeout = new TimeSpan(1, 0, 0);
                //speechRecognizer.Timeouts.InitialSilenceTimeout = new TimeSpan(1, 0, 0);
                //speechRecognizer.Timeouts.BabbleTimeout = new TimeSpan(1, 0, 0);
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

                Recognize();

                SetVoice("음성인식이 시작되었습니다.");
                resultTextBlock.Text = string.Format("음성인식이 시작되었습니다.");
            }
            else
            {
                resultTextBlock.Text = "Permission to access capture resources was not given by the user, reset the application setting in Settings->Privacy->Microphone.";
            }


        }

        private void Timer_Tick(object sender, object e)
        {
            cntTime++;
            if(cntTime == 10)
            {
                iscalled = false;
                cntTime = 0;
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
            if (iscalled == false)
            {
                if (args.Result.Confidence == SpeechRecognitionConfidence.Medium ||
                    args.Result.Confidence == SpeechRecognitionConfidence.High ||
                    args.Result.Confidence == SpeechRecognitionConfidence.Low)
                {
                    await dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                    {
                        resultTextBlock.Text = string.Format("Heard: '{0}', (Tag: '{1}', Confidence: {2})", args.Result.Text, tag, args.Result.Confidence.ToString());
                        if (!string.IsNullOrEmpty(tag))
                        {
                            if (tag == "hello")
                            {
                                //SetVoice("왜 불러?");
                                iscalled = true;
                                if (!timer.IsEnabled)
                                {
                                    timer.Start();
                                }
                            }
                        }

                    });
                }
            }
            else if (iscalled == true)
            {
                await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.N‌​ormal, () => { timer.Stop(); });
                
                iscalled = false;
                cntTime = 0;
                if (args.Result.Confidence == SpeechRecognitionConfidence.Medium ||
                    args.Result.Confidence == SpeechRecognitionConfidence.High ||
                    args.Result.Confidence == SpeechRecognitionConfidence.Low)
                {
                    await dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                    {
                        resultTextBlock.Text = string.Format("Heard: '{0}', (Tag: '{1}', Confidence: {2})", args.Result.Text, tag, args.Result.Confidence.ToString());
                        if (!string.IsNullOrEmpty(tag))
                        {
                            if (tag == "hello")
                            {
                                SetVoice("왜 또 불러?");
                                iscalled = true;
                            }
                            else if (tag == "sleep")
                            {
                                SetVoice("가서 자세요");
                                iscalled = false;
                            }
                            /*else if(tag == "ShowWeather")
                            {
                                SetVoice("오늘의 날씨입니다.");
                                iscalled = false;
                            }/**/
                            else if(tag == "TellWeather")
                            {
                                TellmeWeatherAsync();
                                iscalled = false;
                            }
                        }

                    });
                }
                //태그보니까 신뢰도 low라도 꽤 잘알아들어서 low도 위에 규칙에 포함함
                /*else if (args.Result.Confidence == SpeechRecognitionConfidence.Low)
                {
                    await dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                    {
                        //TODO: 인식이 잘 안되었어도(low) 동작은 하게 해봄
                        resultTextBlock.Text = string.Format("인식률이 낮습니다. (Heard: '{0}', Tag: {1}, Confidence: {2})", args.Result.Text, tag, args.Result.Confidence.ToString());
                        if (!string.IsNullOrEmpty(tag))
                        {
                            if (tag == "hello")
                            {
                                SetVoice("왜 불러?");
                            }
                            else
                            {
                            }
                        }
                        else
                        {
                            SetVoice("다시 말해주세요.");
                            resultTextBlock.Text = string.Format("음성인식이 실패하였습니다.");
                        }
                    });
                }/**/
                //rejected된 인식은 그냥 폐기하는게 정신건강에 이로울듯
                /**/else if (args.Result.Confidence == SpeechRecognitionConfidence.Rejected)
                {
                    await dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                    {
                        //SetVoice("다시 말해주세요."); //Please say it again. //Tell me again. //What did you say? //Say what? //다른건 발음이 이상하게 나옴 ㅋㅋ
                    resultTextBlock.Text = string.Format("음성인식이 실패하였습니다.");
                    });
                }/**/
                else
                {
                    Debug.WriteLine("ContinuousRecognitionSession_ResultGenerated?????");
                }
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
        }

        private async void SpeechRecognizer_StateChanged(SpeechRecognizer sender, SpeechRecognizerStateChangedEventArgs args)
        {
            await dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                Debug.WriteLine("SpeechRecognizer_StateChanged, state = {0}", args.State);
            });
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
            { "wimi", "Hello" }, "hello");
            noticeConstraint = new SpeechRecognitionListConstraint(new List<string>()
            { "I am tired" ,"i'm too tired"}, "sleep");
            ShowWeatherConstraint = new SpeechRecognitionListConstraint(new List<string>()
            { "show me forecast", "show me Weather", "show me weather forecast","nalssi boyeojwo"}, "ShowWeather");
            TellWeatherConstraint = new SpeechRecognitionListConstraint(new List<string>()
            { "Tell me forecast", "Tell me Weather", "Tell me weather forecast","nalssi allyeojwo","today Weather"}, "TellWeather");



            speechRecognizer.Constraints.Add(helloConstraint);
            speechRecognizer.Constraints.Add(noticeConstraint);
            speechRecognizer.Constraints.Add(ShowWeatherConstraint);
            speechRecognizer.Constraints.Add(TellWeatherConstraint);
        }

        public void RemoveConstraints()
        {
            speechRecognizer.Constraints.Remove(helloConstraint);
            speechRecognizer.Constraints.Remove(noticeConstraint);
            speechRecognizer.Constraints.Remove(ShowWeatherConstraint);
            speechRecognizer.Constraints.Remove(TellWeatherConstraint);
        }
    }
}
