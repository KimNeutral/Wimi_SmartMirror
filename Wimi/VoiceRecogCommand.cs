using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Media.SpeechRecognition;
using Windows.UI.Xaml.Controls;


namespace Wimi
{
    public partial class MainPage : Page
    {
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

#if false
        #region 조명
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
        #endregion
#endif

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

#if false
            #region
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
            { "Change color White","turn on White"}, "WhiteColor");
            #endregion
#endif

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
#if false
            #region 조명
            speechRecognizer.Constraints.Add(TurnOnLightConstraint);
            speechRecognizer.Constraints.Add(TurnOffLightConstraint);
            speechRecognizer.Constraints.Add(ChangeLightModeOn);
            speechRecognizer.Constraints.Add(ChangeLightModeOff);
            speechRecognizer.Constraints.Add(RedColorLightConstraint);
            speechRecognizer.Constraints.Add(YellowColorLightConstraint);
            speechRecognizer.Constraints.Add(BrownColorLightConstraint);
            speechRecognizer.Constraints.Add(GreenColorLightConstraint);
            speechRecognizer.Constraints.Add(BlueColorLightConstraint);
            speechRecognizer.Constraints.Add(PinkColorLightConstraint);
            speechRecognizer.Constraints.Add(PurpleColorLightConstraint);
            speechRecognizer.Constraints.Add(WhiteColorLightConstraint);
            #endregion
#endif
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
#if false
            #region 조명
            speechRecognizer.Constraints.Remove(TurnOnLightConstraint);
            speechRecognizer.Constraints.Remove(TurnOffLightConstraint);
            speechRecognizer.Constraints.Remove(ChangeLightModeOn);
            speechRecognizer.Constraints.Remove(ChangeLightModeOff);
            speechRecognizer.Constraints.Remove(RedColorLightConstraint);
            speechRecognizer.Constraints.Remove(YellowColorLightConstraint);
            speechRecognizer.Constraints.Remove(BrownColorLightConstraint);
            speechRecognizer.Constraints.Remove(GreenColorLightConstraint);
            speechRecognizer.Constraints.Remove(BlueColorLightConstraint);
            speechRecognizer.Constraints.Remove(PinkColorLightConstraint);
            speechRecognizer.Constraints.Remove(PurpleColorLightConstraint);
            speechRecognizer.Constraints.Remove(WhiteColorLightConstraint);
            #endregion
#endif
        }

    }
}
