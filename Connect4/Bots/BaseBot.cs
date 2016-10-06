using System;
using Connect4.Models;

namespace Connect4.Bots
{
    public abstract class BaseBot
    {
        internal abstract void MakeMove(Game game);

        public static BaseBot GetBot(Guid botGUID)
        {
            if (botGUID == RandomBot.GUID) return new RandomBot();
            if (botGUID == FillupBot.GUID) return new FillupBot();

            return null;
        }
    }
}