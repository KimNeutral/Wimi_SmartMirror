using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;

namespace DSWeather
{
    public partial class Weather
    {
        /// <summary>
        /// 발표시간(yyyyMMddHHmm)
        /// </summary>
        public string tm { get; set; }

        /// <summary>
        /// 발표순서
        /// </summary>
        public int ts { get; set; }

        /// <summary>
        /// X좌표
        /// </summary>
        public int x { get; set; }

        /// <summary>
        /// Y좌표
        /// </summary>
        public int y { get; set; }

        public async Task<List<ForecastInfo>> GetForecastInfoByCountAsync(int count = 5)
        {
            List<ForecastInfo> lForcastInfo = new List<ForecastInfo>();
            string response = await httpClient.GetStringAsync(new Uri("http://www.kma.go.kr/wid/queryDFS.jsp?gridx=86&gridy=86"));

            XDocument x = XDocument.Parse(response);
            XElement header = x.Element("wid").Element("header");

            this.tm = header.Element("tm").Value;

            DateTime dateTimeTm = new DateTime(int.Parse(this.tm.Substring(0, 4), CultureInfo.InvariantCulture),
                                               int.Parse(this.tm.Substring(4, 2), CultureInfo.InvariantCulture),
                                               int.Parse(this.tm.Substring(6, 2), CultureInfo.InvariantCulture));

            string str = string.Empty;
            str = header.Element("ts").Value;
            if (!string.IsNullOrEmpty(str))
            {
                this.ts = int.Parse(str, CultureInfo.InvariantCulture);
            }

            str = header.Element("x").Value;
            if (!string.IsNullOrEmpty(str))
            {
                this.x = int.Parse(str, CultureInfo.InvariantCulture);
            }

            str = header.Element("y").Value;
            if (!string.IsNullOrEmpty(str))
            {
                this.y = int.Parse(str, CultureInfo.InvariantCulture);
            }

            var weatherInfo = from weather in x.Descendants("data")
                              select new
                              {
                                  hour = weather.Element("hour").Value,
                                  day = weather.Element("day").Value,
                                  temp = weather.Element("temp").Value,
                                  tmx = weather.Element("tmx").Value,
                                  tmn = weather.Element("tmn").Value,
                                  sky = weather.Element("sky").Value,
                                  pty = weather.Element("pty").Value,
                                  wfKor = weather.Element("wfKor").Value,
                                  wfEn = weather.Element("wfEn").Value,
                                  pop = weather.Element("pop").Value,
                                  r12 = weather.Element("r12").Value,
                                  s12 = weather.Element("s12").Value,
                                  ws = weather.Element("ws").Value,
                                  wd = weather.Element("wd").Value,
                                  wdKor = weather.Element("wdKor").Value,
                                  wdEn = weather.Element("wdEn").Value,
                                  reh = weather.Element("reh").Value
                              };

            lForcastInfo.Clear();


            foreach (var weather in weatherInfo)
            {
                ForecastInfo unit = new ForecastInfo();

                if (!string.IsNullOrEmpty(weather.hour))
                {
                    unit.hour = int.Parse(weather.hour, CultureInfo.InvariantCulture);
                }

                if (!string.IsNullOrEmpty(weather.day))
                {
                    unit.day = int.Parse(weather.day, CultureInfo.InvariantCulture);

                    if (dateTimeTm.ToString("yyyyMMdd").Equals(DateTime.Now.ToString("yyyyMMdd")) == false)
                    {
                        unit.day--;
                    }
                }

                if (!string.IsNullOrEmpty(weather.temp))
                {
                    unit.temp = float.Parse(weather.temp, CultureInfo.InvariantCulture);
                }

                if (!string.IsNullOrEmpty(weather.tmx))
                {
                    unit.tmx = float.Parse(weather.tmx, CultureInfo.InvariantCulture);
                }

                if (!string.IsNullOrEmpty(weather.tmn))
                {
                    unit.tmn = float.Parse(weather.tmn, CultureInfo.InvariantCulture);
                }

                if (!string.IsNullOrEmpty(weather.sky))
                {
                    unit.sky = int.Parse(weather.sky, CultureInfo.InvariantCulture);
                }

                if (!string.IsNullOrEmpty(weather.pty))
                {
                    unit.pty = int.Parse(weather.pty, CultureInfo.InvariantCulture);
                }

                unit.wfKor = weather.wfKor;
                unit.wfEn = weather.wfEn;

                if (!string.IsNullOrEmpty(weather.pop))
                {
                    unit.pop = int.Parse(weather.pop, CultureInfo.InvariantCulture);
                }

                if (!string.IsNullOrEmpty(weather.r12))
                {
                    unit.r12 = float.Parse(weather.r12, CultureInfo.InvariantCulture);
                }

                if (!string.IsNullOrEmpty(weather.s12))
                {
                    unit.s12 = float.Parse(weather.s12, CultureInfo.InvariantCulture);
                }

                if (!string.IsNullOrEmpty(weather.ws))
                {
                    unit.ws = float.Parse(weather.ws, CultureInfo.InvariantCulture);
                }

                if (!string.IsNullOrEmpty(weather.wd))
                {
                    unit.wd = int.Parse(weather.wd, CultureInfo.InvariantCulture);
                }

                unit.wdKor = weather.wdKor;
                unit.wdEn = weather.wdEn;

                if (!string.IsNullOrEmpty(weather.reh))
                {
                    unit.reh = int.Parse(weather.reh, CultureInfo.InvariantCulture);
                }

                lForcastInfo.Add(unit);
            }
#if false
            XmlDocument docX = new XmlDocument(); // XmlDocument 생성
            try
            {
                //docX.Load("http://www.kma.go.kr/wid/queryDFS.jsp?gridx=86&gridy=86"); // url로 xml 파일 로드
                var httpClient = new HttpClient();
                HttpResponseMessage response = await httpClient.GetAsync(new Uri("http://www.kma.go.kr/wid/queryDFS.jsp?gridx=86&gridy=86"));
                var jsonStream = await response.Content.ReadAsStreamAsync();
                docX.Load(jsonStream);
            }
            catch
            {
                Console.WriteLine("Error");
                return null;
            }

            XmlNodeList hourList = docX.GetElementsByTagName("hour"); // 태그 이름으로 노드 리스트 저장
            XmlNodeList tempList = docX.GetElementsByTagName("temp");
            XmlNodeList weatherList = docX.GetElementsByTagName("wfKor");
            XmlNodeList rehList = docX.GetElementsByTagName("reh"); //습도
            XmlNodeList popList = docX.GetElementsByTagName("pop"); //강수확률
            XmlNodeList wsList = docX.GetElementsByTagName("ws"); //풍속

            for (int i = 0; i < count; i++)
            {
                ForecastInfo info = new ForecastInfo();
                info.hour = int.Parse(hourList[i].InnerText);
                if(info.hour > 12)
                {
                    info.strHour = info.hour - 12 + "PM";
                }
                else
                {
                    info.strHour = info.hour + "AM";
                }
                
                info.temp = float.Parse(tempList[i].InnerText);
                info.wfKor = weatherList[i].InnerText;
                info.statSymbol = GetSymbol(info.wfKor);
                lForcastInfo.Add(info);
            }

#endif

            return lForcastInfo;
        }

        //public async Task<List<>>
    }

    public class ForecastInfo
    {
        /// <summary>
        /// 예보 시각 (03 ~ 24)
        /// </summary>
        private int _hour;
        public int hour
        {
            get
            {
                return _hour;
            }
            set
            {
                _hour = value;
                if (value > 12)
                {
                    strHour = value - 12 + "PM";
                }
                else
                {
                    strHour = value + "AM";
                }
            }
        }

        public string strHour { get; set; }

        /// <summary>
        /// 오늘, 내일, 모레(0, 1 ,2)
        /// </summary>
        public int day { get; set; }

        /// <summary>
        /// 현재기온
        /// </summary>
        public float temp { get; set; }

        /// <summary>
        /// 예상 최고기온 (-999 는 예보 없음)
        /// </summary>
        public float tmx { get; set; }

        /// <summary>
        /// 예상 최저기온 (-999 는 예보 없음)
        /// </summary>
        public float tmn { get; set; }

        public string iconPath { get; set; }

        /// <summary>
        /// 하늘 상태 (날씨)
        /// </summary>
        public int sky { get; set; }

        /// <summary>
        /// 강수 상태 (0:없음,1:비,2:비,눈,3:눈)
        /// </summary>
        public int pty { get; set; }

        /// <summary>
        /// 한글 날씨
        /// </summary>
        private string _wfKor;
        public string wfKor
        {
            get
            {
                return _wfKor;
            }
            set
            {
                _wfKor = value;
                statSymbol = GetSymbol(value);
            }
        }

        /// <summary>
        /// 영문 날씨
        /// </summary>
        public string wfEn { get; set; }

        /// <summary>
        /// 강수 확률
        /// </summary>
        public int pop { get; set; }

        /// <summary>
        /// 강수량 (12시간) mm
        /// </summary>
        public float r12 { get; set; }

        /// <summary>
        /// 적설량 (12시간) cm
        /// </summary>
        public float s12 { get; set; }

        /// <summary>
        /// 풍속 (m/s)
        /// </summary>
        public float ws { get; set; }

        /// <summary>
        /// 풍향 0~7 (북, 북동, 동, 남동, 남, 남서, 서, 서북)
        /// </summary>
        public int wd { get; set; }

        /// <summary>
        /// 한글 풍향
        /// </summary>
        public string wdKor { get; set; }

        /// <summary>
        /// 영문 풍향
        /// </summary>
        public string wdEn { get; set; }

        /// <summary>
        /// 습도
        /// </summary>
        public int reh { get; set; }

        public string statSymbol { get; set; }

    
        public ForecastInfo() { }

        public string GetSymbol(string stat)
        {
            string sky = "";
            switch (stat)
            {
                case "맑음":
                    sky = "\uE284";
                    //맑음
                    break;
                case "구름 조금":
                    sky = "\uE286";
                    //구름 조금
                    break;
                case "구름 많음":
                    sky = "\uE285";
                    //구름 많음
                    break;
                case "비":
                    sky = "\uE288";
                    //비
                    break;
                case "눈":
                    sky = "\uE28A";
                    //눈
                    break;
                case "낙뢰":
                    sky = "\uE289";
                    //낙뢰
                    break;
            }
            return sky;
        }
    }
}


#if false
public class ForecastInfo
{
    public int hour { get; set; }
    public string hourStr { get; set; }
    public double temperture { get; set; }
    public string stat { get; set; }
    public string statSymbol { get; set; }

    public ForecastInfo() { }
    public ForecastInfo(int hour, double temperture, string stat)
    {
        this.hour = hour;
        this.temperture = temperture;
        this.stat = stat;
    }
}
#endif
