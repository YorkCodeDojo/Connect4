using System;
using System.Linq;
using System.Threading.Tasks;
using System.Transactions;
using Connect4.Core.Models;
using Connect4.Core.Services;
using Connect4.Core.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Connect4.Core
{
    public class ChangeGameModel : PageModel
    {
        private readonly Database _database;
        private readonly BotCreator _botCreator;

        [BindProperty]
        public ChangeGameViewModel ViewModel { get; set; } = new ChangeGameViewModel();

        public ChangeGameModel(Database database, BotCreator botCreator)
        {
            _database = database;
            _botCreator = botCreator;
        }
        public async Task<ActionResult> OnGet(string playerName)
        {
            var allPlayers = await _database.GetAllPlayers();
            var thisPlayer = allPlayers.SingleOrDefault(a => a.Name == playerName);
            if (thisPlayer == null) return LocalRedirect("/Index");

            var otherPlayerID = default(Guid?);
            if (thisPlayer.CurrentGameID.HasValue)
            {
                var currentGame = await _database.LoadGame(thisPlayer.CurrentGameID.Value);
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

            ViewModel = vm;

            return Page();
        }

        public async Task<ActionResult> OnPost()
        {
            var allPlayers = await _database.GetAllPlayers();
            var thisPlayer = allPlayers.SingleOrDefault(a => a.Name == ViewModel.PlayerName);
            if (thisPlayer == null) return LocalRedirect("/Index");

            var currentGameID = thisPlayer.CurrentGameID;

            var newGame = new Game();
            newGame.ID = Guid.NewGuid();
            newGame.YellowPlayerID = ViewModel.OtherPlayerID.Value;
            newGame.RedPlayerID = thisPlayer.ID;

            var otherPlayer = await _database.LoadPlayer(ViewModel.OtherPlayerID.Value);
            if (otherPlayer.SystemBot)
            {
                var bot = _botCreator.GetSystemBot(ViewModel.OtherPlayerID.Value);
                await bot.MakeMove(newGame);
            }

            // The other player supports http webhooks
            if (!string.IsNullOrWhiteSpace(otherPlayer.WebHook))
            {
                var bot = _botCreator.GetWebHookBot(new Uri(otherPlayer.WebHook), ViewModel.OtherPlayerID.Value);
                await bot.MakeMove(newGame);
            }

            using (var tx = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                // and delete the old game
                if (currentGameID.HasValue)
                    await _database.DeleteGame(currentGameID.Value);

                // Delete the other player's previous game.  (System bots can play multiple games)
                if (!otherPlayer.SystemBot && otherPlayer.CurrentGameID.HasValue)
                    await _database.DeleteGame(otherPlayer.CurrentGameID.Value);

                // Create the new game
                await _database.SaveGame(newGame);

                tx.Complete();
            }

            return LocalRedirect($"/DisplayGame?gameID={newGame.ID}");
        }
    }
}