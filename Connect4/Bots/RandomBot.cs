using System;
using Connect4.Models;

namespace Connect4.Bots
{
    /// <summary>
    /// This bot plays completely at random
    /// </summary>
    public class RandomBot
    {
        public static Guid GUID = new Guid("A05BF67C-2BBB-4243-BF18-FE60C52CF4F9");

        /// <summary>
        /// Make our move and return the amended game
        /// </summary>
        /// <param name="game"></param>
        internal void MakeMove(Game game)
        {
            // Are we playing as yellow?
            var isYellow = (game.YellowPlayerID == RandomBot.GUID);

            // Pick columns at random until we have a move we can make
            while (true)
            {
                // Pick a column
                var columnNumber = (new Random()).Next(Game.NUMBER_OF_COLUMNS);

                // Is there space in this column?
                if (game.IsMoveAllowed(columnNumber))
                {
                    game.MakeMove(isYellow, columnNumber);
                    return;
                }
            }
        }
    }
}