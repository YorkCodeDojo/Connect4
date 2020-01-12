using System;
using System.Threading.Tasks;

namespace Connect4.ExampleBot
{
    class Program
    {
        static class Constants
        {
            // Put the name of your team here
            public const string TeamName = "David's Super Stars";

            // and a password for your team here.  This is to prevent cheating!
            public const string TeamPassword = "MyPassword";

            // The location of the game server
            public const string ServerURL = "https://localhost:5001";
        }

        private static async Task MakeMove(Game game, Guid playerID, API api)
        {
            // PUT YOUR CODE IN HERE
            await api.MakeMove(playerID, 0, Constants.TeamPassword);  //Place a counter in the first column

            Console.WriteLine("Press to play");
            Console.ReadKey(true);
        }

        static async Task Main()
        {
            var api = new API(new Uri(Constants.ServerURL));

            // First stage is to register your team name.  This gives
            // you back a TeamID which you need to use in all following
            // class.
            var playerID = await api.RegisterTeam(Constants.TeamName, Constants.TeamPassword);
            Console.WriteLine($"PlayerID is {playerID}");

            // Start a new game
            await api.NewGame(playerID);

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
                            await MakeMove(game, playerID, api);
                        }
                        else
                        {
                            await WaitForOtherPlayer();
                        }
                        break;

                    case GameState.YellowToPlay:
                        if (game.YellowPlayerID == playerID)
                        {
                            await MakeMove(game, playerID, api);
                        }
                        else
                        {
                            await WaitForOtherPlayer();
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

        private static Task WaitForOtherPlayer() => Task.Delay(100);

    }
}
