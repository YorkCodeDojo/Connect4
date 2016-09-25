using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Connect4.Controllers
{
    public class GameController : ApiController
    {
        /// <summary>
        /// Called by the client when a game is about to start. Returns a PlayerID which must
        /// be passed to the other columns.
        /// </summary>
        /// <param name="teamName"></param>
        public void Register(string teamName)
        {
            // Generate a unique playerID

            // Store the new game state
        }

        /// <summary>
        /// Called by the client to get the current state of the game.  We return
        /// 1. The board
        /// 2. The player's colur
        /// 3. Which player is next to move
        /// 4. The name of the other player
        /// 5. The current state of the game,  (Won/Lost,Drawn)
        /// </summary>
        /// <param name="teamName"></param>
        public void GetGameState(Guid playerID)
        {
            // Retrieve the current state of the game
        }

        public void MakeMove(Guid playerID, int columnNumber)
        {
            // Retrieve the current state of the game

            // Is it the players turn

            // Is the move allowed

            // Has the player won

            // Is it now a draw?

            // Store the new game state
        }

    }
}
