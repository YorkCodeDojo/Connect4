using System;
using System.Net.Http;
using System.Threading.Tasks;
using Connect4.Models;

namespace Connect4.Bots
{
    public class WebHookBot : BaseBot
    {
        private string webHookURL;
        private Guid playerID;

        public WebHookBot(string webHookURL, Guid playerID)
        {
            this.webHookURL = webHookURL;
            this.playerID = playerID;
        }

        internal async override Task MakeMove(Game game)
        {
            // Are we playing as yellow?
            var isYellow = (game.YellowPlayerID == this.playerID);

            // Post the game (as json) to the web hook url
            var httpClient = new HttpClient();
            var httpResponseMessage = await httpClient.PostAsJsonAsync(webHookURL, game);
            if (!httpResponseMessage.IsSuccessStatusCode)
            {
                // Something has gone wrong
                var errors = await httpResponseMessage.Content.ReadAsStringAsync();
                throw new Exception(string.Format("Failed to call {0}.   Status {1}.  Reason {2}. {3}", "MakeMove", (int)httpResponseMessage.StatusCode, httpResponseMessage.ReasonPhrase, errors));
            }
            else
            {
                // All good
                var columnNumber = await httpResponseMessage.Content.ReadAsAsync<int>();
                game.MakeMove(isYellow, columnNumber);
            }

        }
    }
}