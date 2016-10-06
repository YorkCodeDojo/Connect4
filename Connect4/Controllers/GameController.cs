using System;
using System.Data.SqlClient;
using System.Threading;
using System.Threading.Tasks;
using System.Transactions;
using System.Web.Http;
using Connect4.Models;
using Connect4.Properties;

namespace Connect4.Controllers
{
    public class GameController : ApiController
    {
        // Well known player IDs
        private Guid FillUpBot = new Guid("48D72B2C-C4CE-43F4-9153-55C28E75CBEA");

        private readonly Database database;
        public GameController()
        {
            this.database = new Database();
        }

        /// <summary>
        /// Called by the client,  once when they are ready to start.  Returns their unique PlayerID
        /// </summary>
        /// <param name="teamName"></param>
        [HttpPost]
        public async Task<IHttpActionResult> Register(string teamName, string password)
        {
            // Generate a unique playerID
            var player = new Player(teamName);
            await database.SavePlayer(player, password);

            // Reload the player
            player = await database.LoadPlayer(player.ID);
            if (player == null) return BadRequest("No player with this ID exists");

            // Create them a game for them to develop against
            if (!player.CurrentGameID.HasValue)
            {
                await CreateInitialGame(player.ID);
            }
            return Ok(player.ID);
        }

        /// <summary>
        /// Player wishes to start a new game. 
        /// </summary>
        /// <param name="playerID"></param>
        [HttpPost]
        public async Task<IHttpActionResult> NewGame(Guid playerID)
        {
            // Get the details of this player
            var player = await database.LoadPlayer(playerID);
            if (player == null) return BadRequest("No player with this ID exists");

            // Retrieve the current state of the game
            if (player.CurrentGameID.HasValue)
            {
                var currentGame = await database.LoadGame(player.CurrentGameID.Value);

                // Set up a new game,  but they are planning the other player
                var newGame = new Game();
                newGame.YellowPlayerID = currentGame.RedPlayerID;
                newGame.RedPlayerID = currentGame.YellowPlayerID;

                if ((newGame.RedToPlay() && newGame.RedPlayerID == Bots.RandomBot.GUID) ||
                    (newGame.YellowToPlay() && newGame.YellowPlayerID == Bots.RandomBot.GUID))
                {
                    // Yellow goes first.
                    var bot = new Bots.RandomBot();
                    bot.MakeMove(newGame);
                }

                using (var tx = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
                {
                    // Create the new game
                    await database.SaveGame(newGame);

                    // and delete the old game
                    await database.DeleteGame(currentGame.ID);

                    tx.Complete();
                }
            }
            else
            {
                // For some reason the player doesn't current have a game
                await CreateInitialGame(playerID);
            }
            return Ok();

        }

        private async Task<Game> CreateInitialGame(Guid playerID)
        {
            var game = new Game();
            game.ID = Guid.NewGuid();
            game.YellowPlayerID = playerID;  //Yellow goes first
            game.RedPlayerID = Bots.RandomBot.GUID;
            await database.SaveGame(game);

            return game;
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
        [HttpGet]
        public async Task<IHttpActionResult> GetGameState(Guid playerID)
        {
            // Get the details of this player
            var player = await database.LoadPlayer(playerID);
            if (player == null) return BadRequest("The player with this ID does not exist");

            // Retrieve the current state of the game
            var game = default(Game);
            if (!player.CurrentGameID.HasValue) game = await CreateInitialGame(playerID);
            game = await database.LoadGame(player.CurrentGameID.Value);
            if (game == null) return BadRequest("The player with this ID does not exist");

            // If it's not this player's turn then we force them to wait.
            if (!((game.YellowToPlay() && game.YellowPlayerID == playerID) || (game.RedToPlay() && game.RedPlayerID == playerID)))
            {
                await Task.Run(() => Thread.Sleep(500));
            }
            return Ok(game);

        }

        [HttpPost]
        public async Task<IHttpActionResult> MakeMove(Guid playerID, int columnNumber)
        {
            // Get the details of this player
            var player = await database.LoadPlayer(playerID);
            if (player == null) return BadRequest("The player with this ID does not exist");

            // Retrieve the current state of the game
            var game = await database.LoadGame(player.CurrentGameID.Value);
            if (game == null) return BadRequest("Your player is not currently playing a game.  Call NewGame");

            // Is it the players turn
            var playerIsYellow = (game.YellowPlayerID == player.ID);
            if (playerIsYellow && !game.YellowToPlay())
                throw new Exception("It is RED's turn to play. You are YELLOW.");

            if ((!playerIsYellow) && game.YellowToPlay())
                throw new Exception("It is YELLOW's turn to play. You are RED.");

            // Is the move allowed?
            if (!game.IsMoveAllowed(columnNumber))
                throw new Exception("Sorry that move is not allowed");

            // Has the player won?
            game.MakeMove(playerIsYellow, columnNumber);

            // Store away the updated game
            await database.SaveGame(game);

            // Is the player playing against our bot?
            if ((game.RedToPlay() && game.RedPlayerID == Bots.RandomBot.GUID) ||
                (game.YellowToPlay() && game.YellowPlayerID == Bots.RandomBot.GUID))
            {
                var bot = new Bots.RandomBot();
                bot.MakeMove(game);
                await database.SaveGame(game);
            }
            return Ok();
        }



    }
}