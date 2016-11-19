using System;
using System.Threading.Tasks;
using Connect4.Models;

namespace Connect4.Bots
{
    public abstract class BaseBot
    {
        internal abstract Task MakeMove(Game game);

        public static BaseBot GetBot(Guid botGUID)
        {
            if (botGUID == RandomBot.GUID) return new RandomBot();
            if (botGUID == FillupBot.GUID) return new FillupBot();

            return null;
        }

        internal static BaseBot GetWebHookBot(string webHookURL, Guid playerID)
        {
            return new WebHookBot(webHookURL, playerID);
        }
    }
}