using System;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using Connect4.Models;

namespace Connect4.Controllers.API
{
    /// <summary>
    /// Methods to get the current state of the game
    /// </summary>
    public class GameStateController : APIControllerBase
    {
        /// <summary>
        /// Called by the client to get the current state of the game.  
        /// </summary>
        /// <param name="playerID">The unique ID of your player,  obtained via the initial call to Register</param>
        [HttpGet]
        [ResponseType(typeof(Game))]
        public async Task<IHttpActionResult> GET(Guid playerID)
        {
            // Get the details of this player
            var player = await database.LoadPlayer(playerID);
            if (player == null) return BadRequest("The player with this ID does not exist");

            // Retrieve the current state of the game
            var game = default(Game);
            if (!player.CurrentGameID.HasValue) game = await CreateInitialGame(playerID);
            game = await database.LoadGame(player.CurrentGameID.Value);
            if (game == null) return BadRequest("The player with this ID does not exist");

            // If it's not this player's turn then we force them to wait.
            if (!((game.YellowToPlay() && game.YellowPlayerID == playerID) || (game.RedToPlay() && game.RedPlayerID == playerID)))
            {
                await Task.Run(() => Thread.Sleep(500));
            }
            return Ok(game);
        }

    }
}
