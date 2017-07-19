using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace DSNews
{
    public  class NewsHeadline
    {
        String Politics_Url = "http://www.iheadlinenews.co.kr/rss/clickTop.xml";

        public async Task<List<News>> GetHeadlineAsync()
        {
           
            List<News> result = new List<News>(); 
            int n = 0;
            XmlDocument xmld = new XmlDocument();
            try
            {
                //xml URL로드
                //xmld.Load(url);
                var httpClient = new HttpClient();
                HttpResponseMessage response = await httpClient.GetAsync(new Uri("http://www.iheadlinenews.co.kr/rss/clickTop.xml"));
                var jsonStream = await response.Content.ReadAsStreamAsync();
                xmld.Load(jsonStream);
            }
            catch (NullReferenceException e)
            {
                return new List<News>();

            }
            catch (WebException e)
            {
                return new List<News>();
            }
            //title값들, 헤드라인의 제목들만 받음.
            XmlNodeList titleList = xmld.GetElementsByTagName("title");
            XmlNodeList linkList = xmld.GetElementsByTagName("link");
            XmlNodeList descList = xmld.GetElementsByTagName("description");
            XmlNodeList authorList = xmld.GetElementsByTagName("author");
            XmlNodeList pubDateList = xmld.GetElementsByTagName("pubDate");

            //result += "--뉴스 헤드라인--\n";
            for (int i = 1; i < titleList.Count; i ++)
            {
                News news = new News();
                news.title = titleList[i].InnerText;
                news.link = linkList[i].InnerText;
                news.descrption = descList[i].InnerText;
                news.author = authorList[i-1].InnerText;
                news.pubDate = pubDateList[i-1].InnerText;
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
        public string author { get; set; }
        public string pubDate { get; set; }
    }
}
