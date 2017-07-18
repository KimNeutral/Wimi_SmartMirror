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

        public async Task<List<string>> getHeadlineAsync()
        {
           
            List<String> result = new List<string>(); 
            int n = 0;
            XmlDocument xmld = new XmlDocument();
            try
            {
                //xml URL로드
                //xmld.Load(url);
                var httpClient = new HttpClient();
                HttpResponseMessage response = await httpClient.GetAsync(new Uri("http://www.iheadlinenews.co.kr/rss/clickTop.xml"));
                var jsonStream = await response.Content.ReadAsStreamAsync();

            }
            catch (NullReferenceException e)
            {
                result.Add("참조 페이지가 잘못되었습니다..\n");
                return result;

            }
            catch (WebException e)
            {
                result.Add("인터넷이 연결되지 않아, 뉴스를 표시할 수 없습니다..\n");
                return result;
            }
            //title값들, 헤드라인의 제목들만 받음.
            XmlNodeList titleList = xmld.GetElementsByTagName("title");
            //result += "--뉴스 헤드라인--\n";
            foreach (XmlNode s in titleList)
            {
                result.Add(s.InnerText);
            }

            return result;
        }
    }
}
