using Connect4.Core.Services;
using Connect4.Core.ViewModels;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Connect4.Core.Pages
{
    public class IndexModel : PageModel
    {
        private readonly Database _database;

        public List<TeamViewModel> ViewModel { get; set; }

        public IndexModel(Database database)
        {
            _database = database;
        }

        public async Task OnGetAsync()
        {
            // Display a list of all subscribed players
            var allPlayers = await _database.GetAllPlayers();
            var allGames = await _database.GetAllGames();

            var allTeams = new List<TeamViewModel>();

            foreach (var player in allPlayers.Where(a => !a.SystemBot))
            {
                var otherPlayerName = "";
                var state = "";
                if (player.CurrentGameID.HasValue)
                {
                    var game = allGames.First(a => a.ID == player.CurrentGameID.Value);
                    var otherPlayerID = (game.RedPlayerID == player.ID) ? game.YellowPlayerID : game.RedPlayerID;
                    var otherPlayer = allPlayers.First(a => a.ID == otherPlayerID);
                    otherPlayerName = otherPlayer.Name;
                    state = game.CurrentState.ToString();
                }

                var team = new TeamViewModel()
                {
                    Name = player.Name,
                    Playing = otherPlayerName,
                    State = state,
                    CurrentGameID = player.CurrentGameID
                };

                allTeams.Add(team);
            }

            ViewModel = allTeams;
        }
    }
}
