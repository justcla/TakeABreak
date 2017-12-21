using System;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;

namespace TakeABreak
{
    public static class TakeABreakTimerFunction
    {
        /// <summary>
        ///  Trigger Chron format: (seconds minutes hours days months years)
        ///  ie. ("0 */15 6-20 * * *") = Every 15 minutes, between 06:00 AM and 08:59 PM
        //       ("0 0 0-5,21-23 * * *") = Every hour from 12:00 AM to 06:00 AM and 09:00 PM to 12:00 AM
        /// </summary>
        [FunctionName("TakeABreakTimerFunction")]
        public static async Task Run([TimerTrigger("*/5 * * * * *")]TimerInfo myTimer, TraceWriter log)
        {
            string message = $"C# Timer trigger function executed at: {DateTime.Now}";
            log.Info(message);
            await SendMessage(message, log);
        }

        private static async Task SendMessage(string message, TraceWriter log)

        {
            string channelsToNotify = Env("ChannelsToNotify");
            string slackbotUrl = Env("SlackbotUrl");
            string bearerToken = Env("SlackOAuthToken");

            log.Info($"Sending to {channelsToNotify}: {message}");

            var channels = Env("ChannelsToNotify").Split(',');
            foreach (var channel in channels)
            {
                HttpResponseMessage response = await new SlackClient(slackbotUrl).SendMessageAsync(message, channel, null, bearerToken);

                if (response.IsSuccessStatusCode)
                {
                    log.Info("Posted successfully.");
                } else
                {
                    log.Info($"Post failed with status code: {response.StatusCode}");
                }
            }
        }

        private static string Env(string name) => Environment.GetEnvironmentVariable(name, EnvironmentVariableTarget.Process);
    }
}
