using System;
using System.Threading.Tasks;
using System.Web.Http;
using Connect4.Models;

namespace Connect4.Controllers
{
    /// <summary>
    /// Class from which all API controllers inherit
    /// </summary>
    public abstract class APIControllerBase : ApiController
    {
        protected readonly Database database;
        public APIControllerBase()
        {
            this.database = new Database();
        }


        /// <summary>
        /// Creates the default game for a new player,  which is them playing against the RandomBot
        /// </summary>
        /// <param name="playerID"></param>
        /// <returns></returns>
        protected async Task<Game> CreateInitialGame(Guid playerID)
        {
            var game = new Game();
            game.ID = Guid.NewGuid();
            game.YellowPlayerID = playerID;  //Yellow goes first
            game.RedPlayerID = Bots.RandomBot.GUID;
            await database.SaveGame(game);

            return game;
        }
    }
}