using Connect4.Core.Services;
using Connect4.Core.ViewModels;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace Connect4.Core.Controllers
{
    [ApiController]
    public class MakeMoveController : ControllerBase
    {
        private readonly Database _database;
        private readonly BotCreator _botCreator;
        private readonly PasswordHashing _passwordHashing;

        public MakeMoveController(Database database, BotCreator botCreator, PasswordHashing passwordHashing)
        {
            _database = database;
            _botCreator = botCreator;
            _passwordHashing = passwordHashing;
        }

        /// <summary>
        ///     Play a move in the specified column
        /// </summary>
        /// <remarks>
        /// Sample request:
        ///
        ///     POST /api/MakeMove
        ///     {
        ///         "PlayerID" : "abcde..."
        ///         "Password" : "secret"
        ///         "ColumnNumber" : 3
        ///     }
        ///
        /// </remarks>
        /// <returns>The state of the game</returns>
        /// <response code="200">Success</response>
        /// <response code="404">The player does not exist</response>
        [Route("api/MakeMove")]
        [HttpPost]
        public async Task<ActionResult> POST([FromBody]MakeMoveViewModel makeMoveViewModel)
        {
            // Get the details of this player
            var player = await _database.LoadPlayer(makeMoveViewModel.PlayerId);
            if (player == null) return BadRequest("The player with this ID does not exist");

            if (!_passwordHashing.CompareHashes(makeMoveViewModel.Password, player.Password))
            {
                return Forbid();
            }

            // Retrieve the current state of the game
            if (!player.CurrentGameID.HasValue) return BadRequest("Your player is not currently playing a game.  Call NewGame");
            var game = await _database.LoadGame(player.CurrentGameID.Value);
            if (game == null) return BadRequest("Your player is not currently playing a game.  Call NewGame");

            if (game.CurrentState != Models.GameState.RedToPlay && game.CurrentState != Models.GameState.YellowToPlay)
            {
                throw new Exception("This game is not playable");
            }

            // Is it the players turn
            var playerIsYellow = (game.YellowPlayerID == player.ID);
            if (playerIsYellow && !game.YellowToPlay())
                throw new Exception("It is RED's turn to play. You are YELLOW.");

            if ((!playerIsYellow) && game.YellowToPlay())
                throw new Exception("It is YELLOW's turn to play. You are RED.");

            // Is the move allowed?
            if (!game.IsMoveAllowed(makeMoveViewModel.ColumnNumber))
                throw new Exception("Sorry that move is not allowed");

            // Has the player won?
            game.MakeMove(playerIsYellow, makeMoveViewModel.ColumnNumber);

            // Store away the updated game
            await _database.SaveGame(game);

            // Is the player playing against our bot?
            if (game.CurrentState == Models.GameState.RedToPlay || game.CurrentState == Models.GameState.YellowToPlay)
            {
                var otherPlayerID = (game.RedPlayerID == makeMoveViewModel.PlayerId) ? game.YellowPlayerID : game.RedPlayerID;
                var otherPlayer = await _database.LoadPlayer(otherPlayerID);
                if (otherPlayer is null) throw new Exception("Player does not exist.");

                if (otherPlayer.SystemBot)
                {
                    var bot = _botCreator.GetSystemBot(otherPlayerID);
                    await bot.MakeMove(game);
                    await _database.SaveGame(game);
                }

                // The other player supports http webhooks
                if (!string.IsNullOrWhiteSpace(otherPlayer.WebHook))
                {
                    var bot = _botCreator.GetWebHookBot(new Uri(otherPlayer.WebHook), otherPlayerID);
                    await bot.MakeMove(game);
                    await _database.SaveGame(game);
                }
            }

            return Ok();
        }

    }
}
