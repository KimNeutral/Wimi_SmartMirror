using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Windows.UI.Core;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Gmail.v1;
using Google.Apis.Gmail.v1.Data;
using Google.Apis.Services;
using Google.Apis.Util;
using System.IO;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace Wimi
{
    public class EmailMessage
    {
        public string Id { get; set; }
        public bool IsSelected { get; set; }
        public string Snippet { get; set; }
        public string SizeEstimate { get; set; }
        public string From { get; set; }
        public string Date { get; set; }
        public string Subject { get; set; }
    }

    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        private UserCredential _credential;
        const string AppName = "MailCheck";

        public async Task<UserCredential> GetCredential()
        {
            var scopes = new[] { GmailService.Scope.GmailModify };
            //var uri = new Uri("ms-appx:///client_id.json");
            var client = new ClientSecrets();
            client.ClientId = "304070051407-5i99fehloji9otv9tfc2omb1v1t5itiq.apps.googleusercontent.com";
            client.ClientSecret = "7VKmL - 3W7CAeZyP07iDuOlS9";
            _credential = await GoogleWebAuthorizationBroker.AuthorizeAsync(
                client,
                scopes, "user", CancellationToken.None);
            
            return _credential;
        }

        private async Task<List<EmailMessage>> GetEmailInfo()
        {
            var service = new GmailService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = _credential,
                ApplicationName = AppName,
            });
            var sizeEstimate = 0L;
            IList<Message> messages = null;
            List<EmailMessage> emailMessages = new List<EmailMessage>();
            await Task.Run(() =>
            {
                UsersResource.MessagesResource.ListRequest request =
                service.Users.Messages.List("me");
                request.Q = "all";
                request.MaxResults = 10;
                messages = request.Execute().Messages;

                for (int index = 0; index < messages.Count; index++)
                {
                    var message = messages[index];
                    var getRequest = service.Users.Messages.Get("me", message.Id);
                    getRequest.Format =
                        UsersResource.MessagesResource.GetRequest.FormatEnum.Metadata;
                    getRequest.MetadataHeaders = new Repeatable<string>(
                        new[] { "Subject", "Date", "From" });
                    messages[index] = getRequest.Execute();
                    sizeEstimate += messages[index].SizeEstimate ?? 0;
                    emailMessages.Add(new EmailMessage()
                    {
                        Id = messages[index].Id,
                        Snippet = WebUtility.HtmlDecode(messages[index].Snippet),
                        From = messages[index].Payload.Headers.FirstOrDefault(h =>
                            h.Name == "From").Value,
                        Subject = messages[index].Payload.Headers.FirstOrDefault(h =>
                            h.Name == "Subject").Value,
                        Date = messages[index].Payload.Headers.FirstOrDefault(h =>
                            h.Name == "Date").Value,
                    });
                    var index1 = index + 1;
                }
            });
            return emailMessages;
        }
    }
}
