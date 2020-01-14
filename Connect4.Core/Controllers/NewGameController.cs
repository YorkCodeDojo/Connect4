using Connect4.Core.Models;
using Connect4.Core.Services;
using Connect4.Core.ViewModels;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using System.Transactions;

namespace Connect4.Core.Controllers
{
    [ApiController]
    public class NewGameController : ControllerBase
    {
        private readonly Database _database;
        private readonly GameCreator _gameCreator;
        private readonly BotCreator _botCreator;

        public NewGameController(Database database, GameCreator gameCreator, BotCreator botCreator)
        {
            _database = database;
            _gameCreator = gameCreator;
            _botCreator = botCreator;
        }

        /// <summary>
        ///     Start a new game still against the same opponent.
        /// </summary>
        /// <remarks>
        /// Sample request:
        ///
        ///     POST /api/NewGame
        ///     
        ///     {
        ///         "playerID" : "abcdef...",
        ///     }
        ///
        /// </remarks>
        /// <returns>The ID of your player</returns>
        /// <response code="200">Success</response>
        [Route("api/NewGame")]
        [HttpPost]
        public async Task<ActionResult> POST([FromBody] NewGameViewModel newGameViewModel)
        {
            // Get the details of this player
            var player = await _database.LoadPlayer(newGameViewModel.PlayerID);
            if (player == null) return BadRequest("No player with this ID exists");

            // Retrieve the current state of the game
            if (player.CurrentGameID.HasValue)
            {
                var currentGame = await _database.LoadGame(player.CurrentGameID.Value);

                // Set up a new game,  but they are planning the other player
                var newGame = new Game();
                newGame.ID = Guid.NewGuid();
                newGame.YellowPlayerID = currentGame.RedPlayerID;
                newGame.RedPlayerID = currentGame.YellowPlayerID;

                // Is the player playing against our bot?  Yellow goes first.
                var otherPlayerID = (newGame.RedPlayerID == newGameViewModel.PlayerID) ? newGame.YellowPlayerID : newGame.RedPlayerID;
                if (otherPlayerID == newGame.YellowPlayerID)
                {
                    var otherPlayer = await _database.LoadPlayer(otherPlayerID);
                    if (otherPlayer is null) throw new Exception("Other player missing from the database!");

                    if (otherPlayer.SystemBot)
                    {
                        var bot = _botCreator.GetSystemBot(otherPlayerID);
                        await bot.MakeMove(newGame);
                    }

                    // The other player supports http webhooks
                    if (!string.IsNullOrWhiteSpace(otherPlayer.WebHook))
                    {
                        var bot = _botCreator.GetWebHookBot(new Uri(otherPlayer.WebHook), otherPlayerID);
                        await bot.MakeMove(newGame);
                    }

                }

                using (var tx = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
                {
                    // and delete the old game
                    await _database.DeleteGame(currentGame.ID);

                    // Create the new game
                    await _database.SaveGame(newGame);

                    tx.Complete();
                }
            }
            else
            {
                // For some reason the player doesn't current have a game
                await _gameCreator.CreateInitialGame(newGameViewModel.PlayerID);
            }
            return Ok();
        }

    }
}
