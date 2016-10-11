using System;

namespace Connect4.ExampleBot
{
    class Program
    {
        // Put the name of your team here
        private const string teamName = "David's Look ahead";

        // and a password for your team here.  This is to prevent cheating!
        private const string teamPassword = "MyPassword";

        // The location of the game server
        private const string serverURL = "http://yorkdojoconnect4.azurewebsites.net/";

        private const int ROWS = 6;
        private const int COLUMNS = 7;

        private static int WorkOutMove(Game game, Guid playerID)
        {
            // What colour are we?
            var weAreYellow = (game.YellowPlayerID == playerID);

            // Is there a move which will mean that we win?
            for (int column = 0; column < COLUMNS; column++)
            {
                if (IsValidMove(game, column))
                    if (IsWinningMove(game, column, weAreYellow)) return column;
            }

            // If the other player played their next turn,  would they win?
            for (int column = 0; column < COLUMNS; column++)
            {
                if (IsValidMove(game, column))
                    if (IsWinningMove(game, column, !weAreYellow)) return column;
            }

            // If we played in that column, then could the other play win in 
            // the following move.
            for (int column = 0; column < COLUMNS; column++)
            {
                if (IsValidMove(game, column))
                {
                    var game2 = EffectOfMakingMove(game, weAreYellow, column);
                    var goodMove = true;
                    for (int column2 = 0; column2 < COLUMNS; column2++)
                    {
                        if (IsWinningMove(game2, column2, !weAreYellow))
                        {
                            // Oh dear
                            goodMove = false;
                            break;
                        }
                    }

                    if (goodMove) return column;
                }
            }

            for (int column = 0; column < COLUMNS; column++)
            {
                if (IsValidMove(game, column))
                    return column;
            }


            // We are going to lost what ever happens.  Return the first valid move
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
