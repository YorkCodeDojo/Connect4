using System;
using System.Threading.Tasks;
using System.Web.Http;
using Connect4.Bots;

namespace Connect4.Controllers
{
    /// <summary>
    /// Methods for playing the game
    /// </summary>
    public class MakeMoveController : APIControllerBase
    {
        /// <summary>
        /// Play a move in the specified column
        /// </summary>
        /// <param name="playerID">The unique ID of your player,  obtained via the initial call to Register</param>
        /// <param name="columnNumber">The column number 0...</param>
        /// <param name="password">Your team's password</param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IHttpActionResult> POST(Guid playerID, int columnNumber, string password)
        {
            // Get the details of this player
            var player = await database.LoadPlayer(playerID);
            if (player == null) return BadRequest("The player with this ID does not exist");

            if (player.Password != password) return BadRequest("Incorrect Password");

            // Retrieve the current state of the game
            var game = await database.LoadGame(player.CurrentGameID.Value);
            if (game == null) return BadRequest("Your player is not currently playing a game.  Call NewGame");

            // Is it the players turn
            var playerIsYellow = (game.YellowPlayerID == player.ID);
            if (playerIsYellow && !game.YellowToPlay())
                throw new Exception("It is RED's turn to play. You are YELLOW.");

            if ((!playerIsYellow) && game.YellowToPlay())
                throw new Exception("It is YELLOW's turn to play. You are RED.");

            // Is the move allowed?
            if (!game.IsMoveAllowed(columnNumber))
                throw new Exception("Sorry that move is not allowed");

            // Has the player won?
            game.MakeMove(playerIsYellow, columnNumber);

            // Store away the updated game
            await database.SaveGame(game);

            // Is the player playing against our bot?
            if (game.CurrentState == Models.GameState.RedToPlay || game.CurrentState == Models.GameState.YellowToPlay)
            {
                var otherPlayerID = (game.RedPlayerID == playerID) ? game.YellowPlayerID : game.RedPlayerID;
                var otherPlayer = await database.LoadPlayer(otherPlayerID);
                if (otherPlayer.SystemBot)
                {
                    var bot = BaseBot.GetBot(otherPlayerID);
                    bot.MakeMove(game);
                    await database.SaveGame(game);
                }
            }
            return Ok();
        }



    }
}