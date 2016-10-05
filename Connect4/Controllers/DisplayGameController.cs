using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace Connect4.Controllers
{
    public class DisplayGameController : Controller
    {
        private readonly Database database;
        public DisplayGameController()
        {
            this.database = new Database();
        }

        public async Task<ActionResult> Index(Guid? gameID)
        {
            if (!gameID.HasValue) return RedirectToAction("Index", "Home");

            var game = await this.database.LoadGame(gameID.Value);
            if (game == null) return RedirectToAction("Index", "Home");

            ViewData.Add("RedPlayer", await GetPlayerName(game.RedPlayerID));
            ViewData.Add("YellowPlayer", await GetPlayerName(game.YellowPlayerID));

            return View(game);
        }

        private async Task<string> GetPlayerName(Guid playerID)
        {
            if (playerID == Bots.RandomBot.GUID)
                return "Random Bot";
            else
            {
                var player =await this.database.LoadPlayer(playerID);
                if (player == null) return "Unknown";
                return player.Name;
            }
        }
    }
}