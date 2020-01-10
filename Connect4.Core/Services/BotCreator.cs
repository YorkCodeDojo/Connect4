using Connect4.Core.Bots;
using System;

namespace Connect4.Core.Services
{
    public class BotCreator
    {
        public IBot? GetBot(Guid botGUID)
        {
            return botGUID.ToString() switch
            {
                RandomBot.GUID => new RandomBot(),
                FillupBot.GUID => new FillupBot(),
                _ => null
            };
        }

        public IBot GetWebHookBot(Uri webHookURL, Guid playerID)
        {
            return new WebHookBot(webHookURL, playerID);
        }
    }
}
