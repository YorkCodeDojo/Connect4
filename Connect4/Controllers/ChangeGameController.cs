using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Transactions;
using System.Web;
using System.Web.Mvc;
using Connect4.Bots;
using Connect4.Models;
using Connect4.ViewModels;

namespace Connect4.Controllers
{
    public class ChangeGameController : Controller
    {

        private readonly Database database;
        public ChangeGameController()
        {
            this.database = new Database();
        }


        [HttpPost]
        public async Task<ActionResult> Index(ChangeGameViewModel model)
        {
            var allPlayers = await this.database.GetAllPlayers();
            var thisPlayer = allPlayers.SingleOrDefault(a => a.Name == model.PlayerName);
            if (thisPlayer == null) return RedirectToAction("Index", "Home");

            var currentGameID = thisPlayer.CurrentGameID;

            var newGame = new Game();
            newGame.ID = Guid.NewGuid();
            newGame.YellowPlayerID = model.OtherPlayerID.Value;
            newGame.RedPlayerID = thisPlayer.ID;

            var otherPlayer = await database.LoadPlayer(model.OtherPlayerID.Value);
            if (otherPlayer.SystemBot)
            {
                var bot = BaseBot.GetBot(model.OtherPlayerID.Value);
                bot.MakeMove(newGame);
            }

            using (var tx = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                // and delete the old game
                if (currentGameID.HasValue)
                    await database.DeleteGame(currentGameID.Value);
 
                // Create the new game
                await database.SaveGame(newGame);

                tx.Complete();
            }

            return RedirectToAction("Index", "DisplayGame", new { gameID = newGame.ID });
        }

        // GET: ChangeGame
        public async Task<ActionResult> Index(string playerName)
        {
            var allPlayers = await this.database.GetAllPlayers();
            var thisPlayer = allPlayers.SingleOrDefault(a => a.Name == playerName);
            if (thisPlayer == null) return RedirectToAction("Index", "Home");

            var otherPlayerID = default(Guid?);
            if (thisPlayer.CurrentGameID.HasValue)
            {
                var currentGame = await this.database.LoadGame(thisPlayer.CurrentGameID.Value);
                otherPlayerID = currentGame.YellowPlayerID == thisPlayer.ID ? currentGame.RedPlayerID : currentGame.YellowPlayerID;
            }

            var vm = new ChangeGameViewModel()
            {
                PlayerName = playerName,
                AllOtherPlayers = allPlayers
                                        .Where(a => a.ID != thisPlayer.ID)
                                        .Select(a => new SelectListItem()
                                        {
                                            Text = a.Name,
                                            Value = a.ID.ToString()
                                        }
                                        ),
                OtherPlayerID = otherPlayerID
            };

            return View(vm);
        }
    }
}