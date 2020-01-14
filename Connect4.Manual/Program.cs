using System;
using System.Threading.Tasks;

namespace Connect4.ExampleBot
{
    class Program
    {
        // Put the name of your team here
        private const string teamName = "Manual Console";

        // and a password for your team here.  This is to prevent cheating!
        private const string teamPassword = "MyPassword";

        // The location of the game server
        //private const string serverURL = "http://yorkdojoconnect4.azurewebsites.net/";
        private const string serverURL = "http://localhost:55011/";

        private static async Task MakeMove(API api, Game game, Guid playerID)
        {

            // Display the board
            var currentColor = Console.ForegroundColor;
            Console.WriteLine("");
            for (int row = Game.NUMBER_OF_ROWS - 1; row >= 0; row--)
            {
                for (int column = 0; column < Game.NUMBER_OF_COLUMNS; column++)
                {
                    switch (game.Cells[column, row])
                    {
                        case CellContent.Empty:
                            Console.Write("0");
                            break;
                        case CellContent.Red:
                            Console.ForegroundColor = ConsoleColor.Red;
                            Console.Write("R");
                            Console.ForegroundColor = currentColor;
                            break;
                        case CellContent.Yellow:
                            Console.ForegroundColor = ConsoleColor.Yellow;
                            Console.Write("Y");
                            Console.ForegroundColor = currentColor;
                            break;
                        default:
                            break;
                    }
                }
                Console.WriteLine("");
            }
            Console.WriteLine("");

            if (game.RedPlayerID == playerID)
                Console.ForegroundColor = ConsoleColor.Red;
            else
                Console.ForegroundColor = ConsoleColor.Yellow;

            Console.WriteLine("Enter the column number");
            Console.ForegroundColor = currentColor;
            var c = -1;
            while (c == -1)
            {
                var columnNumber = Console.ReadKey();
                if (int.TryParse(columnNumber.KeyChar.ToString(), out c))
                {
                    await api.MakeMove(playerID, c, teamPassword);
                }
            }
        }


        static async Task Main(string[] args)
        {
            // First stage is to register your team name.  This gives
            // you back a TeamID which you need to use in all following
            // class.
            var api = new API(new Uri(serverURL));
            var playerID = await api.RegisterTeam(teamName, teamPassword);
            Console.WriteLine($"PlayerID is {playerID}");

            // This is the main game loop
            var gameIsComplete = false;
            while (!gameIsComplete)
            {
                var game = await api.GetGame(playerID);

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
                            try
                            {
                                await MakeMove(api, game, playerID);
                            }
                            catch (Exception e)
                            {
                                Console.WriteLine(e.Message);
                            }

                        }
                        break;

                    case GameState.YellowToPlay:
                        if (game.YellowPlayerID == playerID)
                        {
                            try
                            {
                                await MakeMove(api, game, playerID);
                            }
                            catch (Exception e)
                            {
                                Console.WriteLine(e.Message);
                            }
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
