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
        bool colorLoopAtr = false;
        string ip = string.Empty;
        string key = string.Empty;
        ILocalHueClient client = null;
        /// <summary>
        /// Hue를 사용할수있게 세팅 과 불끄기
        /// </summary>
        public async Task<bool> Init()
        {
            if (client == null)
            {
                ip = await GetIP();
                if (string.IsNullOrEmpty(ip))
                    return false;

                client = new LocalHueClient(ip);
                key = await GetKey(client);
                client.Initialize(key);
                var command = new LightCommand();
                command.TurnOff();
                await client.SendCommandAsync(command);
            }
            return true;
        }

        /// <summary>
        /// Emotion.____형태의 매개변수를 받음,
        /// emotion.none은 불끄기,
        /// 반환형 bool타입의 true
        /// </summary>
        /// <param name="emotion"></param>

        public async Task<bool> HueLightWithEmotion(DSEmotion.Emotion emotion)
        {
            if(client == null)
            {
                return false;
            }
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
                    command.TurnOn().SetColor(new RGBColor("00FFF7"));
                    break;
                case Emotion.disgust:
                    command.TurnOn().SetColor(new RGBColor("0009FF"));
                    break;
                case Emotion.fear:
                    command.TurnOn().SetColor(new RGBColor("000000"));
                    break;
                case Emotion.happiness:
                    command.TurnOn().SetColor(new RGBColor("FFFC00"));
                    break;
                case Emotion.neutral:
                    command.TurnOn().SetColor(new RGBColor("FAFF8D"));
                    break;
                case Emotion.sadness:
                    command.TurnOn().SetColor(new RGBColor("11FF00"));
                    break;
                case Emotion.surprise:
                    command.TurnOn().SetColor(new RGBColor("FF00F3"));
                    break;
            }
            await client.SendCommandAsync(command);
            return true;
        }

        /// <summary>
        /// 반환형 bool타입의 true
        /// </summary>
        public async Task<bool> HueLightOn()
        {
            if (client == null)
            {
                return false;
            }
            var command = new LightCommand();
            command.On = true;
            await client.SendCommandAsync(command);
            return true;
        }

        /// <summary>
        /// 반환형 bool타입의 true
        /// </summary>
        public async Task<bool> HueLightOff()
        {
            if (client == null)
            {
                return false;
            }
            var command = new LightCommand();
            command.On = false;
            await client.SendCommandAsync(command);
            return true;
        }

        /// <summary>
        /// -1 OR 인자X == 루프모드 변환 ex)첫번쨰는 루프 진입, 두번째는 루프 빠져나오기
        /// 0 == 루프 끄기 AND 루프를 빠져나옴(원래 색으로),
        /// 1 == 루프(무한),
        /// 반환형 bool타입의 true
        /// </summary>
        /// <param name="property"></param>
        public async Task<bool> HueEffect(int property = -1)
        {
            if (client == null)
            {
                return false;
            }
            var command = new LightCommand();
            command.On = true;
            switch (property)
            {
                case -1: ////////////////////////////// NeedTest
                    if(colorLoopAtr == false)
                        command.Effect = Effect.ColorLoop;
                    else
                        command.Effect = Effect.None;
                    break;
                case 0:
                    command.Effect = Effect.None;
                    break;
                case 1:
                    command.Effect = Effect.ColorLoop;
                    break;
            }
            await client.SendCommandAsync(command);
            return true;
        }

        /// <summary>
        /// 0 == 알람 끄기 OR 없음, 
        /// 1 == 1번 깜박, 
        /// 2 == 15번 깜박,
        /// 반환형 bool타입의 true
        /// </summary>
        /// <param name="cnt"></param>
        public async Task<bool> HueAlert(int cnt)
        {
            if (client == null)
            {
                return false;
            }
            var command = new LightCommand();
            command.On = true;
            switch (cnt)
            {
                case 0:
                    command.Alert = Alert.None;
                    break;
                case 1:
                    command.Alert = Alert.Once;
                    break;
                case 2:
                    command.Alert = Alert.Multiple;
                    break;
            }
            await client.SendCommandAsync(command);
            return true;
        }

        public async Task<bool> SetColor(string color) ///컴잇 및 이 주석 삭제
        {
            if (client == null)
            {
                return false;
            }
            var colorTable = new Dictionary<string, string>()
            {
                { "red", "DF0101" }, { "brown", "61210B" }, { "yellow", "D7DF01" }, { "green", "0B610B" },
                { "blue", "0404B4" }, { "purple", "5F04B4" }, { "pink", "FF0080" }, { "white", "F2F2F2" }
            };

            var command = new LightCommand();
            command.On = true;
            bool isExists = colorTable.ContainsKey(color);
            if (isExists)
            {
                command.TurnOn().SetColor(new RGBColor(colorTable[color]));
                await client.SendCommandAsync(command);
                return true;
            }
            else
            {
                return false;
            }
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
                using (StreamReader readFile = new StreamReader(new IsolatedStorageFileStream(fileName, FileMode.Open, FileAccess.Read, filePath)))
                {
                    ip = readFile.ReadLine();
                }
                return ip;
            }
            else
            {
                IBridgeLocator locator = new HttpBridgeLocator();
                ///bridgeIPSs == 검색된  브릿지들
                ///브릿지를 찾지않고 함수를 빠져나올시 HueLight주석후 확인
                IEnumerable<LocatedBridge> bridgeIPs = await locator.LocateBridgesAsync(TimeSpan.FromSeconds(5));
                ///검색된 브릿지들중 첫번째 선택
                LocatedBridge bridge = bridgeIPs.FirstOrDefault();
                if (bridge == null)
                {
                    return string.Empty;
                }

                ip = bridge.IpAddress;

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
            if (filePath.FileExists(fileName))
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