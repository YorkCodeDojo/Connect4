using Connect4.Core.Models;
using System;
using System.Threading.Tasks;

namespace Connect4.Core.Services
{
    public class GameCreator
    {
        private readonly Database _database;

        public GameCreator(Database database)
        {
            _database = database;
        }

        /// <summary>
        /// Creates the default game for a new player,  which is them playing against the RandomBot
        /// </summary>
        /// <param name="playerID"></param>
        /// <returns></returns>
        public async Task<Game> CreateInitialGame(Guid playerID)
        {
            var game = new Game();
            game.ID = Guid.NewGuid();
            game.YellowPlayerID = playerID;  //Yellow goes first
            game.RedPlayerID = new Guid(Bots.RandomBot.GUID);
            await _database.SaveGame(game);

            return game;
        }
    }
}
