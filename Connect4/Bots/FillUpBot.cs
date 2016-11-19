using System;
using System.Linq;
using System.Threading.Tasks;
using Connect4.Models;

namespace Connect4.Bots
{
    /// <summary>
    /// This bot plays by filling the board left to right
    /// </summary>
    public class FillupBot : BaseBot
    {
        public static Guid GUID = new Guid("48D72B2C-C4CE-43F4-9153-55C28E75CBEA");

        /// <summary>
        /// Make our move and return the amended game
        /// </summary>
        /// <param name="game"></param>
        internal override Task MakeMove(Game game)
        {
            // Are we playing as yellow?
            var isYellow = (game.YellowPlayerID == FillupBot.GUID);

            // Play in the first column where there is space
            var columnNumber = Enumerable.Range(0, Game.NUMBER_OF_COLUMNS).First(c => game.IsMoveAllowed(c));
            game.MakeMove(isYellow, columnNumber);

            return Task.FromResult(0);
        }

    }
}
