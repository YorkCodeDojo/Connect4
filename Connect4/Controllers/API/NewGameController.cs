using System;
using System.Threading.Tasks;
using System.Transactions;
using System.Web.Http;
using Connect4.Bots;
using Connect4.Models;

namespace Connect4.Controllers.API
{
    /// <summary>
    /// Methods for starting new games
    /// </summary>
    public class NewGameController : APIControllerBase
    {
        /// <summary>
        /// Start a new game still against the same opponent
        /// </summary>
        /// <param name="playerID">The unique ID of your player,  obtained via the initial call to Register</param>
        [HttpPost]
        public async Task<IHttpActionResult> POST(Guid playerID)
        {
            // Get the details of this player
            var player = await database.LoadPlayer(playerID);
            if (player == null) return BadRequest("No player with this ID exists");

            // Retrieve the current state of the game
            if (player.CurrentGameID.HasValue)
            {
                var currentGame = await database.LoadGame(player.CurrentGameID.Value);

                // Set up a new game,  but they are planning the other player
                var newGame = new Game();
                newGame.ID = Guid.NewGuid();
                newGame.YellowPlayerID = currentGame.RedPlayerID;
                newGame.RedPlayerID = currentGame.YellowPlayerID;

                // Is the player playing against our bot?  Yellow goes first.
                var otherPlayerID = (newGame.RedPlayerID == playerID) ? newGame.YellowPlayerID : newGame.RedPlayerID;
                if (otherPlayerID == newGame.YellowPlayerID)
                {
                    var otherPlayer = await database.LoadPlayer(otherPlayerID);
                    if (otherPlayer.SystemBot)
                    {
                        var bot = BaseBot.GetBot(otherPlayerID);
                        await bot.MakeMove(newGame);
                    }

                    // The other player supports http webhooks
                    if (!string.IsNullOrWhiteSpace(otherPlayer.WebHook))
                    {
                        var bot = BaseBot.GetWebHookBot(otherPlayer.WebHook, otherPlayerID);
                        await bot.MakeMove(newGame);
                    }

                }

                using (var tx = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
                {
                    // and delete the old game
                    await database.DeleteGame(currentGame.ID);

                    // Create the new game
                    await database.SaveGame(newGame);

                    tx.Complete();
                }
            }
            else
            {
                // For some reason the player doesn't current have a game
                await CreateInitialGame(playerID);
            }
            return Ok();

        }
    }
}
