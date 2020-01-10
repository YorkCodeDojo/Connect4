using Connect4.Core.Extensions;
using Connect4.Core.Models;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace Connect4.Core.Bots
{
    public class WebHookBot : IBot
    {
        private Uri _webHookURL;
        private Guid _playerID;

        public WebHookBot(Uri webHookURL, Guid playerID)
        {
            _webHookURL = webHookURL;
            _playerID = playerID;
        }

        async Task IBot.MakeMove(Game game)
        {
            // Are we playing as yellow?
            var isYellow = (game.YellowPlayerID == _playerID);

            // Post the game (as json) to the web hook url
            var httpClient = new HttpClient();
            var httpResponseMessage = await httpClient.PostAsJsonAsync(_webHookURL, game);
            if (!httpResponseMessage.IsSuccessStatusCode)
            {
                // Something has gone wrong
                var errors = await httpResponseMessage.Content.ReadAsStringAsync();
                throw new Exception(string.Format("Failed to call {0}.   Status {1}.  Reason {2}. {3}", "MakeMove", (int)httpResponseMessage.StatusCode, httpResponseMessage.ReasonPhrase, errors));
            }
            else
            {
                // All good
                var columnNumber = await httpResponseMessage.ReadAsJsonAsync<int>();
                game.MakeMove(isYellow, columnNumber);
            }

        }
    }
}