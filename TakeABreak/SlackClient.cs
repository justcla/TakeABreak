using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace TakeABreak
{
    public class SlackClient
    {

        //private readonly string _webhookUrlStr;
        private readonly Uri _webhookUrl;
        private readonly HttpClient _httpClient = new HttpClient();

        public SlackClient(string webhookUrlStr)
        {
            //_webhookUrlStr = webhookUrlStr;
            _webhookUrl = new Uri(webhookUrlStr);
        }

        public SlackClient(Uri webhookUrl)
        {
            _webhookUrl = webhookUrl;
        }

        public async Task<HttpResponseMessage> SendMessageAsync(string message, string channel = null, string username = null, string bearerToken = null)
        {
            var payload = new
            {
                text = message,
                channel,
                username,
            };
            var serializedPayload = JsonConvert.SerializeObject(payload);

            if (bearerToken != null)
            {
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", bearerToken);
            }

            //string requestStr = $"{_webhookUrlStr}?channel={channel}&text={Uri.EscapeDataString(message)}";
            //HttpResponseMessage response = await _httpClient.GetAsync(requestStr);

            var response = await _httpClient.PostAsync(_webhookUrl, new StringContent(serializedPayload, Encoding.UTF8, "application/json"));

            return response;
        }

    }

}
