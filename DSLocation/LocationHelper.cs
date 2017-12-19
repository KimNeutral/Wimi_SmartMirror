using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace DSLocation
{
    public class LocationHelper
    {
        string uri;
        
        public LocationHelper()
        {
            uri = @"http://dgswn.us-east-2.elasticbeanstalk.com/";
        }

        public async Task<Users> getLocationInfosAsync()
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(uri);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                try
                {
                    HttpResponseMessage response = await client.GetAsync("targets");
                    var responseContent = await response.Content.ReadAsStringAsync();
                    var locationInfos = JsonConvert.DeserializeObject<Users>(responseContent);
                    return locationInfos;
                }catch(Exception e)
                {
                    
                }
                return null;
            }
        }
    }
}
