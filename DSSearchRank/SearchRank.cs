using HtmlAgilityPack;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DSSearchRank
{
    public class SearchRank
    {
        List<RankUnit> lstRankUnit = new List<RankUnit>();
        public SearchRank()
        {

        }
        public async Task<List<RankUnit>> getSearchRank()
        {
            try
            {
                HtmlWeb web = new HtmlWeb();
                HtmlDocument doc = await web.LoadFromWebAsync("http://www.naver.com");
                HtmlNode docNodes = doc.DocumentNode;

                var rank = docNodes.Descendants().Where
                    (x => (x.Name == "a" && x.Attributes["class"] != null && x.Attributes["class"].Value.Contains("ah_a"))).ToList();
                int count = rank.Count - 1;
                int n = 0;

                foreach (var item in rank)
                {
                    string Nnum = string.Empty;
                    string Ncontent = string.Empty;
                    var list = rank[n].Descendants("span").ToList();
                    foreach (HtmlNode node in list)
                    {
                        string attributeValue = node.GetAttributeValue("class", "");
                        if (attributeValue == "ah_r")
                        {
                            Nnum = node.InnerText;
                        }
                        else if (attributeValue == "ah_k")
                        {
                            Ncontent = node.InnerText;
                        }
                    }
                    lstRankUnit.Add(new RankUnit()
                    {
                        num = Nnum,
                        content = Ncontent
                    });
                    n++;
                }
            }catch(HtmlWebException e)
            {
                return new List<RankUnit>();
            }
            return lstRankUnit;
        }
        
    }
}
