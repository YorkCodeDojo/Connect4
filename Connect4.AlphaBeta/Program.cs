using System;
using System.Linq;
using System.Threading.Tasks;

namespace Connect4.ExampleBot
{
    class Program
    {
        // Put the name of your team here
        private const string teamName = "AlphaBeta";

        // and a password for your team here.  This is to prevent cheating!
        private const string teamPassword = "MyPassword";

        // The location of the game server
        private const string serverURL = "http://yorkdojoconnect4.azurewebsites.net/";

        private const int ROWS = 6;
        private const int COLUMNS = 7;

        private static int WorkOutMove(Game game, Guid playerID)
        {
            const int MAXDEPTH = 3;

            // What colour are we?
            var weAreYellow = (game.YellowPlayerID == playerID);

            // Which move will give us the highest score?
            var counts = new System.Collections.Concurrent.ConcurrentBag<Tuple<int, int>>();   //Column, Score
            Parallel.For(0, Game.NUMBER_OF_COLUMNS, column =>
            {
                if (IsValidMove(game, column))
                {
                    var nextMove = EffectOfMakingMove(game, weAreYellow, column);
                    var ourScore = MiniMax(nextMove, false, !weAreYellow, MAXDEPTH, int.MinValue, int.MaxValue);
                    counts.Add(new Tuple<int, int>(column, ourScore));
                }
            });

            var bestColumn = counts.OrderByDescending(a => a.Item2).First().Item1;
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

            return bestColumn;
        }

        private static int MiniMax(Game game, bool maximisingPlayer, bool weAreYellow, int depth, int alpha, int beta)
        {
            // Is this a terminal node.  This happens when
            //    Depth reaches 0 (our limit)
            //    A player has won
            //    There are no more moves.
            if (game.CurrentState == GameState.Draw) return 0;
            if (depth == 0) return ScoreBoard(game, weAreYellow);

            if (maximisingPlayer)
            {
                // Find the highest move
                var v = int.MinValue;
                for (int column = 0; column < COLUMNS; column++)
                {
                    if (IsValidMove(game, column))
                    {
                        var wasWinningMove = IsWinningMove(game, column, weAreYellow);
                        if (wasWinningMove) return int.MaxValue;

                        var nextMove = EffectOfMakingMove(game, weAreYellow, column);
                        var childValue = MiniMax(nextMove, false, !weAreYellow, depth - 1, alpha,beta);
                        v = Math.Max(v, childValue);
                        alpha = Math.Max(alpha, v);
                        if (beta <= alpha) return v;
                    }
                }
                return v;
            }
            else
            {
                // Find the lowest move
                var v = int.MaxValue;
                for (int column = 0; column < COLUMNS; column++)
                {
                    if (IsValidMove(game, column))
                    {
                        var wasWinningMove = IsWinningMove(game, column, weAreYellow);
                        if (wasWinningMove) return int.MinValue;

                        var nextMove = EffectOfMakingMove(game, weAreYellow, column);
                        var childValue = MiniMax(nextMove, true, !weAreYellow, depth - 1, alpha, beta);
                        v = Math.Min(v, childValue);
                        beta = Math.Min(beta, v);
                        if (beta <= alpha) return v;
                    }
                }
                return v;
            }

        }


        /// <summary>
        /// Work out the score for the board
        /// </summary>
        /// <returns></returns>
        private static int ScoreBoard(Game game, bool weAreYellow)
        {
            // What colour are we matching
            var toMatch = (weAreYellow ? CellContent.Yellow : CellContent.Red);

            var totalScore = 0;
            for (int column = 0; column < COLUMNS; column++)
            {
                for (int row = 0; row < ROWS; row++)
                {
                    totalScore += ScoreCell(game, weAreYellow, column, row);
                }
            }

            return totalScore;
        }

        private static int ScoreCell(Game game, bool weAreYellow, int column, int row)
        {
            var rowToCheck = row;
            var columnToCheck = column;
            var pattern = "";
            var score = 0;

            // Left to right
            while (columnToCheck < COLUMNS)
            {
                pattern = AddToPattern(game, weAreYellow, rowToCheck, columnToCheck, pattern);
                columnToCheck++;
            }
            score += ScorePattern(pattern);

            //Top to bottom
            rowToCheck = row;
            columnToCheck = column;
            pattern = "";
            while (rowToCheck >= 0)
            {
                pattern = AddToPattern(game, weAreYellow, rowToCheck, columnToCheck, pattern);
                rowToCheck--;
            }
            score += ScorePattern(pattern);

            //Diagonal up to the right
            rowToCheck = row;
            columnToCheck = column;
            pattern = "";
            while (columnToCheck < COLUMNS && rowToCheck < ROWS)
            {
                pattern = AddToPattern(game, weAreYellow, rowToCheck, columnToCheck, pattern);
                rowToCheck++;
                columnToCheck++;
            }
            score += ScorePattern(pattern);

            //Diagonal up to the left
            rowToCheck = row;
            columnToCheck = column;
            pattern = "";
            while (columnToCheck >= 0 && rowToCheck < ROWS)
            {
                pattern = AddToPattern(game, weAreYellow, rowToCheck, columnToCheck, pattern);
                rowToCheck++;
                columnToCheck--;
            }
            score += ScorePattern(pattern);

            return score;
        }

        private static string AddToPattern(Game game, bool weAreYellow, int rowToCheck, int columnToCheck, string pattern)
        {
            switch (game.Cells[columnToCheck, rowToCheck])
            {
                case CellContent.Empty:
                    pattern += "_";
                    break;
                case CellContent.Red:
                    pattern += weAreYellow ? "x" : "*";
                    break;
                case CellContent.Yellow:
                    pattern += weAreYellow ? "*" : "x";
                    break;
            }

            return pattern;
        }

        /// <summary>
        /// * means our colour
        /// _ means empty cell
        /// x means other player
        /// </summary>
        /// <param name="pattern"></param>
        /// <returns></returns>
        private static int ScorePattern(string pattern)
        {
            /* Scoring is:
             *  Line of 4 = 100 points    ****
             *  Line of 3 with a gap at both ends = 8 points   _**_
             *  Line of 3 with a gap at either end = 7 points  ?__?
             *  Line of 2 then gap then 1  = 7 points **_*
             *  Line of 2 with a gap at both ends = 6 points
             *  Line of 2 with a gap (of length >= 2) at one end = 5 points
             *  
             *  Look at each cell in turn.   We only look to the right/down of each position.
             */

            if (pattern.StartsWith("****")) return 100;
            if (pattern == "_***_") return 9;
            if (pattern.StartsWith("_***")) return 7;
            if (pattern == "x***_") return 7;
            if (pattern.StartsWith("**_*")) return 7;
            if (pattern.StartsWith("*_**")) return 7;
            if (pattern.StartsWith("_**_")) return 6;
            if (pattern.StartsWith("__**")) return 5;
            if (pattern.StartsWith("**__")) return 5;

            return 0;
        }

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
            var columnToPlayIn = WorkOutMove(game, playerID);
            API.MakeMove(playerID, serverURL, columnToPlayIn, teamPassword);

            //Console.WriteLine("Press to play");
            //Console.ReadKey(true);
        }


        static void Main(string[] args)
        {

            var testGame = new Game();
            testGame.Cells = new CellContent[Game.NUMBER_OF_COLUMNS, Game.NUMBER_OF_ROWS];
            testGame.Cells[0, 0] = CellContent.Red;
            testGame.Cells[0, 1] = CellContent.Red;
            testGame.Cells[0, 2] = CellContent.Red;
            testGame.Cells[2, 0] = CellContent.Yellow;
            testGame.Cells[6, 0] = CellContent.Yellow;
            testGame.YellowPlayerID = Guid.NewGuid();
            testGame.RedPlayerID = Guid.NewGuid();
            var move = WorkOutMove(testGame, testGame.YellowPlayerID);

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
