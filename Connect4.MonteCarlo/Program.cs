using System;
using System.Linq;
using System.Threading.Tasks;

namespace Connect4.ExampleBot
{
    class Program
    {
        // Put the name of your team here
        private const string teamName = "David's Monte Carlo";

        // and a password for your team here.  This is to prevent cheating!
        private const string teamPassword = "MyPassword";

        // The location of the game server
        private const string serverURL = "http://yorkdojoconnect4.azurewebsites.net/";

        private const int ROWS = 6;
        private const int COLUMNS = 7;

        /// <summary>
        /// Would playing this move win?
        /// </summary>
        /// <param name="game"></param>
        /// <param name="column"></param>
        /// <param name="weAreYellow"></param>
        /// <returns></returns>
        private static bool IsWinningMove(Game game, int columnToPlay, bool weAreYellow)
        {
            // Which row would the counter go in?
            var rowToPlay = -1;
            for (int row = 0; row < ROWS; row++)
            {
                if (game.Cells[columnToPlay, row] == CellContent.Empty)
                {
                    rowToPlay = row;
                    break;
                }
            }
            if (rowToPlay == -1) return false;  //Column is full

            // What colour are we matching
            var toMatch = (weAreYellow ? CellContent.Yellow : CellContent.Red);

            // Downwards.  (Already at the top)
            var r = rowToPlay - 1;
            var c = columnToPlay;
            var chainLength = 0;
            while (r >= 0 && (game.Cells[c, r] == toMatch))
            {
                r--;
                chainLength++;
            }
            if (chainLength >= 3) return true;

            // Left to right
            r = rowToPlay;
            c = columnToPlay - 1;
            chainLength = 0;
            while (c >= 0 && (game.Cells[c, r] == toMatch))  //Left
            {
                c--;
                chainLength++;
            }
            c = columnToPlay + 1;
            while (c < COLUMNS && (game.Cells[c, r] == toMatch))  //Left
            {
                c++;
                chainLength++;
            }
            if (chainLength >= 3) return true;

            // +vc diagonal
            r = rowToPlay - 1;
            c = columnToPlay - 1;
            chainLength = 0;
            while (c >= 0 && r >= 0 && (game.Cells[c, r] == toMatch))  //Left-down
            {
                c--;
                r--;
                chainLength++;
            }
            c = columnToPlay + 1;
            r = rowToPlay + 1;
            while (r < ROWS && c < COLUMNS && (game.Cells[c, r] == toMatch))  //Right-Up
            {
                c++;
                r++;
                chainLength++;
            }
            if (chainLength >= 3) return true;

            // -vc diagonal
            r = rowToPlay + 1;
            c = columnToPlay - 1;
            chainLength = 0;
            while (c >= 0 && r < ROWS && (game.Cells[c, r] == toMatch))  //Left-up
            {
                c--; //left
                r++; //up
                chainLength++;
            }
            c = columnToPlay + 1;
            r = rowToPlay - 1;
            while (r >= 0 && c < COLUMNS && (game.Cells[c, r] == toMatch))  //Right-down
            {
                c++;  //right
                r--;  //down
                chainLength++;
            }
            if (chainLength >= 3) return true;

            return false;

        }

        /// <summary>
        /// Returns a new board,  but with that move played
        /// </summary>
        /// <param name="game"></param>
        /// <param name="weAreYellow"></param>
        /// <param name="column"></param>
        /// <returns></returns>
        private static Game EffectOfMakingMove(Game game, bool weAreYellow, int columnToPlay)
        {
            var newGame = game.Clone();

            // Play in the first space in the column
            for (int row = 0; row < ROWS; row++)
            {
                if (newGame.Cells[columnToPlay, row] == CellContent.Empty)
                {
                    newGame.Cells[columnToPlay, row] = (weAreYellow ? CellContent.Yellow : CellContent.Red);
                    break;
                }
            }

            return newGame;
        }

        /// <summary>
        /// A move is valid if there is still space in the column
        /// </summary>
        /// <param name="game"></param>
        /// <param name="column"></param>
        /// <returns></returns>
        private static bool IsValidMove(Game game, int column)
        {
            return (game.Cells[column, ROWS - 1] == CellContent.Empty);
        }

        private static void MakeMove(Game game, Guid playerID, string serverURL)
        {
            var weAreYellow = (game.YellowPlayerID == playerID);

            // Which move will give us the highest score?
            var winningColumns = new System.Collections.Concurrent.ConcurrentBag<Tuple<int, int>>();   //Column, Score

            Parallel.For(0, 100000, g =>
             {
                //Pick a first column at random
                var column = new Random().Next(Game.NUMBER_OF_COLUMNS);

                 if (IsValidMove(game, column))
                 {
                     //Play the game
                     var didYellowWin = PlayGame(game, weAreYellow, column);

                     //If we won,  remember which column we play first
                     if (didYellowWin.HasValue)
                         if ((didYellowWin.Value == weAreYellow))
                             winningColumns.Add(new Tuple<int, int>(column, 1));
                         else
                             winningColumns.Add(new Tuple<int, int>(column, -1));
                 }
             });

            // Which column won the most
            var columnScores = winningColumns.GroupBy(a => a);

            // Add up the totals in each group
            var bestScore = int.MinValue;
            var bestColumn = -1;
            foreach (var columnScore in columnScores)
            {
                var columnNumber = columnScore.Key.Item1;
                var total = columnScore.Select(a => a.Item2).Sum();
                if (total > bestScore)
                {
                    bestScore = total;
                    bestColumn = columnNumber;
                }
            }

            if (bestColumn == -1)
            {
                // Just play the first column
                for (int column = 0; column < COLUMNS; column++)
                {
                    if (IsValidMove(game, column))
                    {
                        bestColumn = column;
                        break;
                    }
                }
            }


            API.MakeMove(playerID, serverURL, bestColumn, teamPassword);

        }

        /// <summary>
        /// Returns TRUE if yellow won
        /// NULL for a draw
        /// </summary>
        /// <param name="game"></param>
        /// <param name="itIsYellowsTurn"></param>
        /// <param name="columnToPlay"></param>
        /// <param name="doWeWantYellowToWin"></param>
        /// <returns></returns>
        private static bool? PlayGame(Game game, bool itIsYellowsTurn, int columnToPlay)
        {
            //Would we have won?
            if (IsWinningMove(game, columnToPlay, itIsYellowsTurn))
            {
                return itIsYellowsTurn;
            }

            //Make the move
            var newGame = EffectOfMakingMove(game, itIsYellowsTurn, columnToPlay);

            //Play the next move at random
            var availableMoves = newGame.GetAvailableMoves();
            if (!availableMoves.Any()) return null;  //draw
            while (true)
            {
                var nextColumnToPlay = availableMoves[new Random().Next(availableMoves.Count)];
                if (IsValidMove(newGame, nextColumnToPlay))
                    return PlayGame(newGame, !itIsYellowsTurn, nextColumnToPlay);
            }
        }

        static void Main(string[] args)
        {

            //var testGame = new Game();
            //testGame.Cells = new CellContent[Game.NUMBER_OF_COLUMNS, Game.NUMBER_OF_ROWS];
            //testGame.Cells[0, 0] = CellContent.Red;
            //testGame.Cells[0, 1] = CellContent.Red;
            //testGame.Cells[0, 2] = CellContent.Red;
            //testGame.Cells[2, 0] = CellContent.Yellow;
            //testGame.Cells[6, 0] = CellContent.Yellow;
            //testGame.YellowPlayerID = Guid.NewGuid();
            //testGame.RedPlayerID = Guid.NewGuid();
            //var move = WorkOutMove(testGame, testGame.YellowPlayerID);

            // First stage is to register your team name.  This gives
            // you back a TeamID which you need to use in all following
            // class.
            var playerID = API.RegisterTeam(teamName, teamPassword, serverURL);
            Console.WriteLine($"PlayerID is {playerID}");


            // This is the main game loop
            var gameIsComplete = false;
            while (!gameIsComplete)
            {
                var game = API.GetGame(playerID, serverURL);

                switch (game.CurrentState)
                {
                    case GameState.RedWon:
                        gameIsComplete = true;
                        Console.WriteLine((game.RedPlayerID == playerID) ? "You Won" : "You Lost");
                        break;

                    case GameState.YellowWon:
                        gameIsComplete = true;
                        Console.WriteLine((game.YellowPlayerID == playerID) ? "You Won" : "You Lost");
                        break;

                    case GameState.RedToPlay:
                        if (game.RedPlayerID == playerID)
                        {
                            MakeMove(game, playerID, serverURL);
                        }
                        break;

                    case GameState.YellowToPlay:
                        if (game.YellowPlayerID == playerID)
                        {
                            MakeMove(game, playerID, serverURL);
                        }
                        break;

                    case GameState.Draw:
                        gameIsComplete = true;
                        Console.WriteLine("Draw");
                        break;

                    case GameState.GameNotStarted:
                        break;

                    default:
                        break;
                }
            }


        }
    }
}
