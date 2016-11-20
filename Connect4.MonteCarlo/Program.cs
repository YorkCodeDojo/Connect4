using System;
using System.Linq;
using System.Threading.Tasks;


class Program
{
    // Put the name of your team here
    private const string teamName = "David's Monte Carlo";

    // and a password for your team here.  This is to prevent cheating!
    private const string teamPassword = "MyPassword";
 
    // The location of the game server
    private const string serverURL = "http://yorkdojoconnect4.azurewebsites.net/";

    private static void MakeMove(Game game, Guid playerID, string serverURL)
    {
        var mc = new MonteCarlo();
        var column = mc.SuggestMove(game, playerID);
        API.MakeMove(playerID, serverURL, column, teamPassword);
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

