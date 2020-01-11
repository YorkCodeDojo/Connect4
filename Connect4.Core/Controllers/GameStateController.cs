using Connect4.Core.Models;
using Connect4.Core.Services;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Connect4.Core.Controllers
{
    [ApiController]
    public class GameStateController : ControllerBase
    {
        private readonly Database _database;
        private readonly GameCreator _gameCreator;

        public GameStateController(Database database, GameCreator gameCreator)
        {
            _database = database;
            _gameCreator = gameCreator;
        }

        /// <summary>
        ///     Called by the client to get the current state of the game.  
        /// </summary>
        /// <remarks>
        /// Sample request:
        ///
        ///     GET api/GameState/abcedf
        ///
        /// </remarks>
        /// <returns>The state of the game</returns>
        /// <response code="200">Success</response>
        /// <response code="404">The player does not exist</response>
        [Route("api/GameState/{playerID}")]
        [HttpGet]
        public async Task<ActionResult<Game>> GET([FromRoute]Guid playerID)
        {
            // Get the details of this player
            var player = await _database.LoadPlayer(playerID);
            if (player is null) return NotFound();

            // Retrieve the current state of the game
            var game = default(Game);
            if (!player.CurrentGameID.HasValue)
                game = await _gameCreator.CreateInitialGame(playerID);
            else
                game = await _database.LoadGame(player.CurrentGameID.Value);

            if (game == null) return BadRequest("There is no game for this player");

            // If it's not this player's turn then we force them to wait.
            if (!((game.YellowToPlay() && game.YellowPlayerID == playerID) || (game.RedToPlay() && game.RedPlayerID == playerID)))
            {
                await Task.Run(() => Thread.Sleep(500));
            }
            return game;
        }

    }
}
