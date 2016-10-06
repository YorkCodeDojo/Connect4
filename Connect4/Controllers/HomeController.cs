using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Mvc;
using Connect4.ViewModels;
using System.Linq;

namespace Connect4.Controllers
{
    public class HomeController : Controller
    {

        private readonly Database database;
        public HomeController()
        {
            this.database = new Database();
        }

        public async Task<ActionResult> Index()
        {
            // Display a list of all subscribed players
            var allPlayers = await this.database.GetAllPlayers();
            var allGames = await this.database.GetAllGames();

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

            return View(allTeams);
        }
    }
}
