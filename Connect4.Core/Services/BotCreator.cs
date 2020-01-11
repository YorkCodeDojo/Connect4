using Connect4.Core.Bots;
using System;

namespace Connect4.Core.Services
{
    public class BotCreator
    {
        public IBot GetSystemBot(Guid botGUID)
        {
            return botGUID.ToString().ToUpperInvariant() switch
            {
                RandomBot.GUID => new RandomBot(),
                FillupBot.GUID => new FillupBot(),
                _ => throw new Exception("Unknown system bot")
            };
        }

        public IBot GetWebHookBot(Uri webHookURL, Guid playerID)
        {
            return new WebHookBot(webHookURL, playerID);
        }
    }
}
