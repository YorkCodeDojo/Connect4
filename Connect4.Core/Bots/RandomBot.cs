﻿using System;
using System.Threading.Tasks;
using Connect4.Core.Models;

namespace Connect4.Core.Bots
{
    /// <summary>
    /// This bot plays completely at random
    /// </summary>
    public class RandomBot : IBot
    {
        public const string GUID = "A05BF67C-2BBB-4243-BF18-FE60C52CF4F9";

        /// <summary>
        /// Make our move and return the amended game
        /// </summary>
        /// <param name="game"></param>
        Task IBot.MakeMove(Game game)
        {
            // Are we playing as yellow?
            var isYellow = (game.YellowPlayerID == new Guid(RandomBot.GUID));

            // Pick columns at random until we have a move we can make
            while (true)
            {
                // Pick a column
                var columnNumber = (new Random()).Next(Game.NUMBER_OF_COLUMNS);

                // Is there space in this column?
                if (game.IsMoveAllowed(columnNumber))
                {
                    game.MakeMove(isYellow, columnNumber);
                    return Task.FromResult(0);
                }
            }
        }
    }
}