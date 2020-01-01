﻿using System;
using System.Net.Http;

namespace EsthersConnectFour
{
    internal static class API
    {
        /// <summary>
        /// Get the current state of the game
        /// </summary>
        /// <param name="playerID"></param>
        /// <param name="serverURL"></param>
        /// <returns></returns>
        internal static Game GetGame(Guid playerID, string serverURL)
        {
            var httpClient = new HttpClient();
            httpClient.BaseAddress = new Uri(serverURL);

            var httpResponseMessage = httpClient.GetAsync($"api/GameState?playerID={playerID}").Result;
            if (!httpResponseMessage.IsSuccessStatusCode)
            {
                // Something has gone wrong
                var errors = httpResponseMessage.Content.ReadAsStringAsync().Result;
                throw new Exception(string.Format("Failed to call {0}.   Status {1}.  Reason {2}. {3}", "GameState", (int)httpResponseMessage.StatusCode, httpResponseMessage.ReasonPhrase, errors));
            }
            else
            {
                // All good
                var result = httpResponseMessage.Content.ReadAsJsonAsync<Game>().Result;
                return result;
            }
        }

        /// <summary>
        /// Register your team to get your unique player ID
        /// </summary>
        /// <param name="TeamName"></param>
        /// <param name="teamPassword"></param>
        /// <param name="serverURL"></param>
        /// <returns></returns>
        internal static Guid RegisterTeam(string TeamName, string teamPassword, string serverURL)
        {
            var httpClient = new HttpClient();
            httpClient.BaseAddress = new Uri(serverURL);

            var httpResponseMessage = httpClient.PostAsync($"api/Register?teamName={TeamName}&password={teamPassword}", null).Result;
            if (!httpResponseMessage.IsSuccessStatusCode)
            {
                // Something has gone wrong
                var errors = httpResponseMessage.Content.ReadAsStringAsync().Result;
                throw new Exception(string.Format("Failed to call {0}.   Status {1}.  Reason {2}. {3}", "RegisterTeam", (int)httpResponseMessage.StatusCode, httpResponseMessage.ReasonPhrase, errors));
            }
            else
            {
                // All good
                var result = httpResponseMessage.Content.ReadAsJsonAsync<Guid>().Result;
                return result;
            }

        }

        /// <summary>
        /// Plays a move.  ColumnNumber should be between 0 and 6
        /// </summary>
        /// <param name="playerID"></param>
        /// <param name="serverURL"></param>
        /// <param name="columnNumber"></param>
        internal static void MakeMove(Guid playerID, string serverURL, int columnNumber, string teamPassword)
        {
            var httpClient = new HttpClient();
            httpClient.BaseAddress = new Uri(serverURL);

            var httpResponseMessage = httpClient.PostAsync($"api/MakeMove?playerID={playerID}&columnNumber={columnNumber}&password={teamPassword}", null).Result;
            if (!httpResponseMessage.IsSuccessStatusCode)
            {
                // Something has gone wrong
                var errors = httpResponseMessage.Content.ReadAsStringAsync().Result;
                throw new Exception(string.Format("Failed to call {0}.   Status {1}.  Reason {2}. {3}", "MakeMove", (int)httpResponseMessage.StatusCode, httpResponseMessage.ReasonPhrase, errors));
            }
        }

        /// <summary>
        /// Starts a new game against the same player as the previous game.  Your colours will
        /// however be swapped (red => yellow and yellow => red)
        /// </summary>
        /// <param name="playerID"></param>
        /// <param name="serverURL"></param>
        internal static void NewGame(Guid playerID, string serverURL)
        {
            var httpClient = new HttpClient();
            httpClient.BaseAddress = new Uri(serverURL);

            var httpResponseMessage = httpClient.PostAsync($"api/NewGame?playerID={playerID}", null).Result;
            if (!httpResponseMessage.IsSuccessStatusCode)
            {
                // Something has gone wrong
                var errors = httpResponseMessage.Content.ReadAsStringAsync().Result;
                throw new Exception(string.Format("Failed to call {0}.   Status {1}.  Reason {2}. {3}", "NewGame", (int)httpResponseMessage.StatusCode, httpResponseMessage.ReasonPhrase, errors));
            }

        }

    }
}
