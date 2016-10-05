using System;
using System.Net.Http;

namespace Connect4.ExampleBot
{
    internal static class API
    {
        internal static Game GetGame(Guid playerID, string serverURL)
        {
            var httpClient = new HttpClient();
            httpClient.BaseAddress = new Uri(serverURL);

            var httpResponseMessage = httpClient.GetAsync($"api/Game/GetGameState?playerID={playerID}").Result;
            if (!httpResponseMessage.IsSuccessStatusCode)
            {
                // Something has gone wrong
                var errors = httpResponseMessage.Content.ReadAsStringAsync().Result;
                throw new Exception(string.Format("Failed to call {0}.   Status {1}.  Reason {2}. {3}", "GetGame", (int)httpResponseMessage.StatusCode, httpResponseMessage.ReasonPhrase, errors));
            }
            else
            {
                // All good
                var result = httpResponseMessage.Content.ReadAsAsync<Game>().Result;
                return result;
            }
        }
        internal static Guid RegisterTeam(string TeamName, string teamPassword, string serverURL)
        {
            var httpClient = new HttpClient();
            httpClient.BaseAddress = new Uri(serverURL);

            var httpResponseMessage = httpClient.PostAsync($"api/Game/Register?teamName={TeamName}&password={teamPassword}", null).Result;
            if (!httpResponseMessage.IsSuccessStatusCode)
            {
                // Something has gone wrong
                var errors = httpResponseMessage.Content.ReadAsStringAsync().Result;
                throw new Exception(string.Format("Failed to call {0}.   Status {1}.  Reason {2}. {3}", "RegisterTeam", (int)httpResponseMessage.StatusCode, httpResponseMessage.ReasonPhrase, errors));
            }
            else
            {
                // All good
                var result = httpResponseMessage.Content.ReadAsAsync<string>().Result;
                return new Guid(result);
            }

        }
        internal static void MakeMove(Guid playerID, string serverURL, int columnNumber)
        {
            var httpClient = new HttpClient();
            httpClient.BaseAddress = new Uri(serverURL);

            var httpResponseMessage = httpClient.PostAsync($"api/Game/MakeMove?playerID={playerID}&columnNumber={columnNumber}", null).Result;
            if (!httpResponseMessage.IsSuccessStatusCode)
            {
                // Something has gone wrong
                var errors = httpResponseMessage.Content.ReadAsStringAsync().Result;
                throw new Exception(string.Format("Failed to call {0}.   Status {1}.  Reason {2}. {3}", "MakeMove", (int)httpResponseMessage.StatusCode, httpResponseMessage.ReasonPhrase, errors));
            }

        }

    }
}
