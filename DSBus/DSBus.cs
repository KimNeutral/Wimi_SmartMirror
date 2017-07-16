using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using HtmlAgilityPack;
using System.Diagnostics;

namespace DSBus
{
    public class Bus
    {
        public List<BusInfo> BusListInfo = null;

        public void Init()
        {
            if(BusListInfo == null)
                BusListInfo = new List<BusInfo>();
            else
            {
                BusListInfo = null;
                BusListInfo = new List<BusInfo>();
            }
        } 

        /// <summary>
        /// 대구시 버스 정보 읽어오기
        /// </summary>
        /// <returns>true일경우 버스정보 존재 false일경우 버스정보 없음</returns>
        public async Task<bool> LoadBusInfo()
        {
            Init();
            string url = @"http://m.businfo.go.kr/bp/m/realTime.do?act=arrInfo&bsId=7111041200&bsNm=%B1%B8%C1%F6%B8%E9%BB%E7%B9%AB%BC%D2%B0%C7%B3%CA";
            HttpClient http = new HttpClient();
            byte[] b = await http.GetByteArrayAsync(url);
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            Encoding encode = Encoding.GetEncoding("windows-1254");
            System.Text.Encoding euckr = System.Text.Encoding.GetEncoding(51949);
            string str = euckr.GetString(b);
            HtmlDocument document = new HtmlDocument();
            document.LoadHtml(str);

            HtmlNode docNodes = document.DocumentNode;
            List<HtmlNode> toftitle = docNodes.Descendants().Where
                (x => (x.Name == "div" && x.Attributes["class"] != null && x.Attributes["class"].Value.Contains("dp"))).ToList();

            int count = toftitle.Count - 1;
            var li = toftitle[count].Descendants("li").ToList();

            foreach (var item in li)
            {
                string num = string.Empty;
                string st = string.Empty;
                string pos = string.Empty;
                string nt = string.Empty;

                var span = item.Descendants("span").ToList();

                foreach (HtmlNode node in span)
                {
                    string attributeValue = node.GetAttributeValue("class", "");
                    if (attributeValue == "route_no")
                    {
                        num = node.InnerText;
                    }
                    else if (attributeValue == "arr_state")
                    {
                        st = node.InnerText;
                    }
                    else if (attributeValue == "cur_pos")
                    {
                        pos = node.InnerText;
                    }
                    else if (attributeValue == "cur_pos nsbus")
                    {
                        pos = node.InnerText;
                    }
                    else if (attributeValue == "route_note")
                    {
                        nt = node.InnerText;
                    }
                }

                BusListInfo.Add(new BusInfo()
                {
                    number = num,
                    state = st,
                    position = pos,
                    note = nt
                });
            }
            if (BusListInfo.Count == 0)
                return false;
            else
                return true;
        }

    }

    public class BusInfo
    {
        public string number { get; set; }
        public string state { get; set; }
        public string position { get; set; }
        public string note { get; set; }
    }
}
