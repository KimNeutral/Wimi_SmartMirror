using Q42.HueApi;
using Q42.HueApi.Interfaces;
using Q42.HueApi.Models.Bridge;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.IsolatedStorage;
using System.Linq;
using System.Text;
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
        /// <summary>
        /// Hue를 사용할수있게 세팅 및 불끄기
        /// </summary>
        public async void Init()
        {
            if(client == null)
            {
                ip = await GetIP();
                client = new LocalHueClient(ip);
                key = await GetKey(client);
                client.Initialize(key);
                var command = new LightCommand();
                command.TurnOff();
                client.SendCommandAsync(command);
            }
        }

        /// <summary>
        /// Emotion.____형태의 매개변수 전달
        /// emotion.none은 불끄기
        /// </summary>
        /// <param name="emotion"></param>
        
        public async void HueLight(DSEmotion.Emotion emotion)
        {
            var command = new LightCommand();
            command.On = true;
            switch (emotion)
            {
                case Emotion.none:
                    command.TurnOff();
                    break;
                case Emotion.anger:
                    command.TurnOn().SetColor(new RGBColor("FF0000"));
                    break;
                case Emotion.contempt:
                    command.TurnOn().SetColor(new RGBColor("FF00BF"));
                    break;
                case Emotion.disgust:
                    command.TurnOn().SetColor(new RGBColor("013ADF"));
                    break;
                case Emotion.fear:
                    command.TurnOn().SetColor(new RGBColor("A901DB"));
                    break;
                case Emotion.happiness:
                    command.TurnOn().SetColor(new RGBColor("00FF00"));
                    break;
                case Emotion.neutral:
                    command.TurnOn().SetColor(new RGBColor("01DF01"));
                    break;
                case Emotion.sadness:
                    command.TurnOn().SetColor(new RGBColor("B40431"));
                    break;
                case Emotion.surprise:
                    command.TurnOn().SetColor(new RGBColor("FF0080"));
                    break;
            }
            //command.On = true;
            client.SendCommandAsync(command);
        }
        /// <summary>
        /// IP주소를 반환 해줌
        /// </summary>
        /// <returns>string</returns>
        public async Task<string> GetIP()
        {
            string fileName = "HueBridgeIP.txt";
            string ip = string.Empty;

            IsolatedStorageFile filePath = IsolatedStorageFile.GetUserStoreForApplication();
            if (filePath.FileExists(fileName))
            {
                using(StreamReader readFile = new StreamReader(new IsolatedStorageFileStream(fileName, FileMode.Open, FileAccess.Read, filePath)))
                {
                    ip = readFile.ReadLine();
                }
                return ip;
            }
            else
            {
                IBridgeLocator locator = new HttpBridgeLocator();
                ///bridgeIPSs == 검색된  브릿지들
                IEnumerable<LocatedBridge> bridgeIPs = await locator.LocateBridgesAsync(TimeSpan.FromSeconds(5));
                ///검색된 브릿지들중 첫번째 선택
                LocatedBridge bridge = bridgeIPs.FirstOrDefault();
                ip = bridge.IpAddress;
                if (bridge == null)
                {
                    return ip;
                }
                ///IP생성
                ///ip저장
                using (StreamWriter writeFile = new StreamWriter(new IsolatedStorageFileStream(fileName, FileMode.Create, FileAccess.Write, filePath)))
                {
                    ///키 값 쓰기
                    writeFile.WriteLine(ip);
                }
                return ip;

            }


        }
        /// <summary>
        /// Key를 반환
        /// </summary>
        /// <param name="client"></param>
        /// <returns></returns>
        public async Task<string> GetKey(ILocalHueClient client)
        {
            ///key를 저장할 파일명
            string fileName = "HueBridgeKey.txt";
            string key = string.Empty;

            ///저장 경로를 실행파일(?)의 경로
            IsolatedStorageFile filePath = IsolatedStorageFile.GetUserStoreForApplication();
            if(filePath.FileExists(fileName))
            {
                using (StreamReader readFile = new StreamReader(new IsolatedStorageFileStream(fileName, FileMode.Open, FileAccess.Read, filePath)))
                {
                    key = readFile.ReadLine();
                }
            }
            else
            {
                key = await client.RegisterAsync("DS_UWP", "Hue_Bridge");
                using (StreamWriter writeFile = new StreamWriter(new IsolatedStorageFileStream(fileName, FileMode.Create, FileAccess.Write, filePath)))
                {
                    ///키 값 쓰기
                    writeFile.WriteLine(key);
                }
            }

            return key;
        }
    }
}