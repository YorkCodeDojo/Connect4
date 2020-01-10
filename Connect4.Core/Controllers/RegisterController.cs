using Connect4.Core.Models;
using Connect4.Core.Services;
using Connect4.Core.ViewModels;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace Connect4.Core.Controllers
{
    [ApiController]
    public class RegisterController : ControllerBase
    {
        private readonly Database _database;
        private readonly GameCreator _gameCreator;

        public RegisterController(Database database, GameCreator gameCreator)
        {
            _database = database;
            _gameCreator = gameCreator;
        }

        /// <summary>
        ///     Call this to register your team name.  This call returns your PlayerID which you will need to supply in all other calls
        /// </summary>
        /// <remarks>
        /// Sample request:
        ///
        ///     POST api/Register
        ///     
        ///     {
        ///         "teamName" : "David",
        ///         "password" : "Secret",
        ///     }
        ///
        /// </remarks>
        /// <returns>The ID of your player</returns>
        /// <response code="200">Success</response>
        [Route("api/Register")]
        [HttpPost]
        public async Task<ActionResult<Guid>> POST([FromBody] RegisterTeamViewModel registerTeamViewModel)
        {
            // Generate a unique playerID
            var player = new Player(registerTeamViewModel.TeamName);
            await _database.SavePlayer(player, registerTeamViewModel.Password);

            // Reload the player
            player = await _database.LoadPlayer(player.ID);
            if (player == null) return BadRequest("No player with this ID exists");

            // Create them a game for them to develop against
            if (!player.CurrentGameID.HasValue)
            {
                await _gameCreator.CreateInitialGame(player.ID);
            }
            return player.ID;
        }

    }
}
