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
        private readonly PasswordHashing _passwordHashing;

        public RegisterController(Database database, GameCreator gameCreator, PasswordHashing passwordHashing)
        {
            _database = database;
            _gameCreator = gameCreator;
            _passwordHashing = passwordHashing;
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
            var hashedPassword = _passwordHashing.HashPassword(registerTeamViewModel.Password);
            await _database.SavePlayer(player, hashedPassword);

            // Reload the player
            var reloadedPlayer = await _database.LoadPlayer(player.ID);
            if (reloadedPlayer == null) return BadRequest("No player with this ID exists");

            if (!_passwordHashing.CompareHashes(registerTeamViewModel.Password, reloadedPlayer.Password))
            {
                return Forbid();
            }

            // Create them a game for them to develop against
            if (!reloadedPlayer.CurrentGameID.HasValue)
            {
                await _gameCreator.CreateInitialGame(reloadedPlayer.ID);
            }
            return reloadedPlayer.ID;
        }

    }
}
