using Q42.HueApi;
using Q42.HueApi.Interfaces;
using Q42.HueApi.Models.Bridge;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.IsolatedStorage;
using System.Linq;
using System.Threading.Tasks;
using DSEmotion;
using Q42.HueApi.ColorConverters;
using Q42.HueApi.ColorConverters.HSB;

namespace DSHue
{
    public class Hue
    {
        string ip = string.Empty;
        string key = string.Empty;
        ILocalHueClient client = null;

        public async void Init()
        {
            if(client == null)
            {
                ip = await GetIP();
                client = new LocalHueClient(ip);
                key = await GetKey(client);
                client.Initialize(key);
            }
        }

        public async void HueLight(Emotion emotion)
        {
            var command = new LightCommand();
            command.On = true;

            switch (emotion)
            {
                case Emotion.anger:
                    command.TurnOn().SetColor(new RGBColor("FF00A0"));
                    break;
                case Emotion.contempt:
                    command.TurnOn().SetColor(new RGBColor("FF00A1"));
                    break;
                case Emotion.disgust:
                    command.TurnOn().SetColor(new RGBColor("FF00A2"));
                    break;
                case Emotion.fear:
                    command.TurnOn().SetColor(new RGBColor("FF00A3"));
                    break;
                case Emotion.happiness:
                    command.TurnOn().SetColor(new RGBColor("FF00A4"));
                    break;
                case Emotion.neutral:
                    command.TurnOn().SetColor(new RGBColor("FF00A5"));
                    break;
                case Emotion.sadness:
                    command.TurnOn().SetColor(new RGBColor("FF00A6"));
                    break;
                case Emotion.surprise:
                    command.TurnOn().SetColor(new RGBColor("FF00A7"));
                    break;

            }
            client.SendCommandAsync(command);
        }
        
        public async Task<string> GetIP()
        {
            string fileName = "HueBridgeIP";
            string ip = string.Empty;

            IsolatedStorageFile filePath = IsolatedStorageFile.GetUserStoreForApplication();
            if (filePath.FileExists(fileName))
            {
                IBridgeLocator locator = new HttpBridgeLocator();
                ///bridgeIPSs == 검색된  브릿지들
                IEnumerable<LocatedBridge> bridgeIPs = await locator.LocateBridgesAsync(TimeSpan.FromSeconds(5));
                ///검색된 브릿지들중 첫번째 선택f
                LocatedBridge bridge = bridgeIPs.FirstOrDefault();
                if (bridge == null)
                {
                    return ip;
                }
                ///IP생성
                ip = await client.RegisterAsync("DS_UWP", "HUE_Bridge");
                ///ip저장
                using (StreamWriter writeFile = new StreamWriter(new IsolatedStorageFileStream(fileName, FileMode.Create, FileAccess.Write, filePath)))
                {
                    ///키 값 쓰기
                    writeFile.WriteLine(ip);
                }
                return ip;
            }
            else
            {
                using (StreamReader readFile = new StreamReader(new IsolatedStorageFileStream(fileName, FileMode.Open, FileAccess.Read, filePath)))
                {
                    ip = readFile.ReadLine();
                }
                return ip;
            }

            
        }

        public async Task<string> GetKey(ILocalHueClient client)
        {
            ///key를 저장할 파일명
            string fileName = "HueBridgeKey";
            string key = string.Empty;

            ///저장 경로를 실행파일(?)의 경로
            IsolatedStorageFile filePath = IsolatedStorageFile.GetUserStoreForApplication();
            if(filePath.FileExists(fileName))
            {
                key = await client.RegisterAsync("DS_UWP", "Hue_Bridge");
                using (StreamWriter writeFile = new StreamWriter(new IsolatedStorageFileStream(fileName, FileMode.Create, FileAccess.Write, filePath)))
                {
                    ///키 값 쓰기
                    writeFile.WriteLine(key);
                }
            }
            else
            {
                using (StreamReader readFile = new StreamReader(new IsolatedStorageFileStream(fileName, FileMode.Open, FileAccess.Read, filePath)))
                {
                    key = readFile.ReadLine();
                }
            }

            return key;
        }
    }
}