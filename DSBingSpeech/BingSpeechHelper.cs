using Newtonsoft.Json;
using System;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Windows.Storage;

namespace DSBingSpeech
{
    public class BingSpeechHelper
    {
        private const string INTERACTIVE = "interactive";
        private const string CONVERSATION = "conversation";
        private const string DICTATION = "dictation";

        private const string BING_SPEECH_KEY = "<user_azure_bing_speech_api_key>";

        private readonly string _language = "ko-KR";
        //private readonly string _language = "en-US";
        private readonly string _requestUri;
        //private IAuthenticationService _authenticationService;

        public BingSpeechHelper()
        {
            //&format=detailed
            /*_requestUri =
                $@"https://speech.platform.bing.com/speech/recognition/{
                    INTERACTIVE}/cognitiveservices/v1?language={
                        _language}";*/
            _requestUri = @"https://speech.platform.bing.com/speech/recognition/interactive/cognitiveservices/v1?language=" + _language + "& format=simple";
        }

        public async Task<string> GetTextResultAsync(string recordedFilename)
        {
            var file = await ApplicationData.Current.LocalFolder.GetFileAsync(recordedFilename);


            using (var fileStream = new FileStream(file.Path, FileMode.Open, FileAccess.Read))
            {
                using (var client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Accept.Add(MediaTypeWithQualityHeaderValue.Parse("application/json"));
                    client.DefaultRequestHeaders.Accept.Add(MediaTypeWithQualityHeaderValue.Parse("text/xml"));
                    client.DefaultRequestHeaders.TransferEncoding.Add(TransferCodingHeaderValue.Parse("chunked"));
                    client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", BING_SPEECH_KEY);
                    //4fbb88204e0e4cb398afbc6b08035836
                    //26d7094ca0fc41f88b8bc482d89cc478
                    //둘 중 하나 
                    //client.Timeout = new TimeSpan(0, 0, 5);

                    var content = new StreamContent(fileStream);
                    content.Headers.Add("ContentType", new[] { "audio/wav", "codec=audio/pcm", "samplerate=16000" });

                    try
                    {
                        var response = await client.PostAsync(_requestUri, content);
                        var responseContent = await response.Content.ReadAsStringAsync();
                        var speechResults = JsonConvert.DeserializeObject<BinSpeechResult>(responseContent);

                        return speechResults.DisplayText;
                    }
                    catch (Exception e)
                    {
                        return "실패";
                    }
                    finally
                    {
                        content.Dispose();
                    }
                }
            }
        }
    }
}
