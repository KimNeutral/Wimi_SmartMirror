using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.Resources.Core;
using Windows.Media.SpeechSynthesis;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;

namespace Wimi
{
    public partial class MainPage : Page
    {
        DispatcherTimer TTSDispatcherTimer;

        private SpeechSynthesizer synthesizer;
        private ResourceContext speechContext;
        private ResourceMap speechResourceMap;
        private MediaElement media;
        public Boolean TTSflag;


        private void TTSDispatcherTimer_Tick(object sender, object e)
        {
            TTSflag = false;
            TTSDispatcherTimer.Stop();
        }

        private void initSynthesizer()
        {
            TTSDispatcherTimer = new DispatcherTimer();
            TTSDispatcherTimer.Tick += TTSDispatcherTimer_Tick;
            TTSDispatcherTimer.Interval = new TimeSpan(0, 0, 3);

            media = new MediaElement();
            media.Volume = 1;
            synthesizer = new SpeechSynthesizer();

            speechContext = ResourceContext.GetForCurrentView();
            speechContext.Languages = new string[] { SpeechSynthesizer.DefaultVoice.Language };

            speechResourceMap = ResourceManager.Current.MainResourceMap.GetSubtree("LocalizationTTSResources");

            var voices = SpeechSynthesizer.AllVoices;

            // Get the currently selected voice.
            VoiceInformation currentVoice = synthesizer.Voice;

            foreach (VoiceInformation voice in voices.OrderBy(p => p.Language))
            {
                synthesizer.Voice = voice;
            }
        }

        public async void SetVoice(string str)
        {
            //return; //잡음 문제로 일단 아래코드를 처리하지 않는다.

            if (media.CurrentState.Equals(MediaElementState.Playing))
            {
                media.Stop();
            }
            else
            {
                string text = str;
                if (!String.IsNullOrEmpty(text))
                {
                    try
                    {
                        // Create a stream from the text. This will be played using a media element.
                        SpeechSynthesisStream synthesisStream = await synthesizer.SynthesizeTextToStreamAsync(text);

                        // Set the source and start playing the synthesized audio stream.
                        media.AutoPlay = true;
                        media.SetSource(synthesisStream, synthesisStream.ContentType);

                        TTSDispatcherTimer.Start();
                        TTSflag = true;
                        media.Play();
                    }
                    catch (System.IO.FileNotFoundException)
                    {
                    }
                    catch (Exception)
                    {
                        media.AutoPlay = false;
                    }
                }
            }
        }
    }
}
