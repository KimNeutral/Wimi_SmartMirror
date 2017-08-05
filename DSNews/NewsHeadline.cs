using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;

namespace DSNews
{
    public  class NewsHeadline
    {
        //String Politics_Url = "http://www.iheadlinenews.co.kr/rss/clickTop.xml"; //cur not used

        public async Task<List<News>> GetHeadlineNewsAsync(int count = 5)
        {
           
            List<News> result = new List<News>(); 
            XmlDocument xmld = new XmlDocument();
            try
            {
                //xml URL로드
                //xmld.Load(url);
                var httpClient = new HttpClient();
                HttpResponseMessage response = await httpClient.GetAsync(new Uri("http://rss.donga.com/total.xml"));
                var jsonStream = await response.Content.ReadAsStreamAsync();
                xmld.Load(jsonStream);
            }
            catch (NullReferenceException e)
            {
                Debug.WriteLine(e.Message);
                return new List<News>();

            }
            catch (WebException e)
            {
                Debug.WriteLine(e.Message);
                return new List<News>();
            }
            //title값들, 헤드라인의 제목들만 받음.
            XmlNodeList titleList = xmld.GetElementsByTagName("title");
            XmlNodeList linkList = xmld.GetElementsByTagName("link");
            XmlNodeList descList = xmld.GetElementsByTagName("description");
            XmlNodeList pubDateList = xmld.GetElementsByTagName("pubDate");

            //result += "--뉴스 헤드라인--\n";
            for (int i = 2; i < titleList.Count; i ++)
            {
                if (i - 1 > count)
                {
                    break;
                }
                News news = new News();
                news.title = titleList[i].InnerText;
                news.link = linkList[i].InnerText;
                news.descrption = descList[i].InnerText;
                news.pubDate = pubDateList[i].InnerText;
                result.Add(news);
            }

            return result;
        }

        public async Task<List<News>> GetDaumNewsAsync(int maxCount = 6)
        {
            List<News> result = new List<News>();

            var httpClient = new HttpClient();
            string response = await httpClient.GetStringAsync(new Uri("http://media.daum.net/syndication/today_sisa.rss"));

            XDocument x = XDocument.Parse(response);
            XElement header = x.Element("rss").Element("channel");

            string title = header.Element("title").Value;

            var NewsInfo = from news in x.Descendants("item")
                              select new
                              {
                                  title = news.Element("title").Value,
                                  image = news.Element("enclosure").Attribute("url").Value
                              };


            int count = 0;
            foreach (var unit in NewsInfo)
            {
                count++;
                if (count > maxCount)
                {
                    break;
                }

                News news = new News();
                news.title = unit.title;
                news.image = unit.image;

                result.Add(news);
            }

            return result;
        }
    }

    public class News
    {
        public string title { get; set; }
        public string link { get; set; }
        public string descrption { get; set; }
        public string pubDate { get; set; }
        public string image { get; set; }
    }
}
