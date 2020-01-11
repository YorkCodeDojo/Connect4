using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace EsthersConnectFour
{
    public class API
    {
        private readonly HttpClient _httpClient;

        public API(Uri serverUri)
        {
            _httpClient = new HttpClient
            {
                BaseAddress = serverUri
            };
        }
        /// <summary>
        /// Get the current state of the game
        /// </summary>
        /// <param name="playerID"></param>
        /// <returns></returns>
        internal async Task<Game> GetGame(Guid playerID)
        {
            var httpResponseMessage = await _httpClient.GetAsync($"api/GameState/{playerID}");
            if (!httpResponseMessage.IsSuccessStatusCode)
            {
                // Something has gone wrong
                var errors = await httpResponseMessage.Content.ReadAsStringAsync();
                throw new Exception(string.Format("Failed to call {0}.   Status {1}.  Reason {2}. {3}", "GameState", (int)httpResponseMessage.StatusCode, httpResponseMessage.ReasonPhrase, errors));
            }
            else
            {
                // All good
                var result = await httpResponseMessage.Content.ReadAsJsonAsync<Game>();
                return result;
            }
        }

        /// <summary>
        /// Register your team to get your unique player ID
        /// </summary>
        /// <param name="TeamName"></param>
        /// <param name="teamPassword"></param>
        /// <returns></returns>
        internal async Task<Guid> RegisterTeam(string teamName, string teamPassword)
        {
            var data = new
            {
                TeamName = teamName,
                Password = teamPassword,
            };

            var httpResponseMessage = await _httpClient.PostAsJsonAsync($"/api/Register", data).ConfigureAwait(false);
            if (!httpResponseMessage.IsSuccessStatusCode)
            {
                // Something has gone wrong
                var errors = httpResponseMessage.Content.ReadAsStringAsync().Result;
                throw new Exception(string.Format("Failed to call {0}.   Status {1}.  Reason {2}. {3}", "RegisterTeam", (int)httpResponseMessage.StatusCode, httpResponseMessage.ReasonPhrase, errors));
            }
            else
            {
                // All good
                var result = await httpResponseMessage.Content.ReadAsJsonAsync<Guid>().ConfigureAwait(false);
                return result;
            }
        }

        /// <summary>
        /// Plays a move.  ColumnNumber should be between 0 and 6
        /// </summary>
        /// <param name="playerID"></param>
        /// <param name="columnNumber"></param>
        internal async Task MakeMove(Guid playerID, int columnNumber, string teamPassword)
        {
            var data = new
            {
                playerID = playerID,
                columnNumber = columnNumber,
                password = teamPassword
            };

            var httpResponseMessage = await _httpClient.PostAsJsonAsync($"api/MakeMove", data);
            if (!httpResponseMessage.IsSuccessStatusCode)
            {
                // Something has gone wrong
                var errors = await httpResponseMessage.Content.ReadAsStringAsync();
                throw new Exception(string.Format("Failed to call {0}.   Status {1}.  Reason {2}. {3}", "MakeMove", (int)httpResponseMessage.StatusCode, httpResponseMessage.ReasonPhrase, errors));
            }
        }

        /// <summary>
        /// Starts a new game against the same player as the previous game.  Your colours will
        /// however be swapped (red => yellow and yellow => red)
        /// </summary>
        /// <param name="playerID"></param>
        internal async Task NewGame(Guid playerID)
        {
            var data = new
            {
                playerId = playerID,
            };

            var httpResponseMessage = await _httpClient.PostAsJsonAsync($"api/NewGame", data).ConfigureAwait(false);
            if (!httpResponseMessage.IsSuccessStatusCode)
            {
                // Something has gone wrong
                var errors = httpResponseMessage.Content.ReadAsStringAsync().Result;
                throw new Exception(string.Format("Failed to call {0}.   Status {1}.  Reason {2}. {3}", "NewGame", (int)httpResponseMessage.StatusCode, httpResponseMessage.ReasonPhrase, errors));
            }
        }
    }
}
