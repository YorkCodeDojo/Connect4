using System;

namespace Connect4.ExampleBot
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.ReadKey(true);
            // Fixed URL for the game server
            var serverURL = "http://localhost:55011";

            // First stage is to register your team name.  This gives
            // you back a TeamID which you need to use in all following
            // class.
            var teamName = "David's Super Stars";
            var teamPassword = "MyPassword";
            var playerID = API.RegisterTeam(teamName, teamPassword, serverURL);

            // This is the main game loop
            var gameIsComplete = false;
            while (!gameIsComplete)
            {
                var game = API.GetGame(playerID, serverURL);

                var moved = false;
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
                            moved = true;
                        }
                        break;

                    case GameState.YellowToPlay:
                        if (game.YellowPlayerID == playerID)
                        {
                            MakeMove(game, playerID, serverURL);
                            moved = true;
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

                // If it's not our turn,  then wait 1 second to be nice to server
                if (!moved)
                    System.Threading.Thread.Sleep(1000);
            }


        }


        private static void MakeMove(Game game, Guid playerID, string serverURL)
        {
            // PUT YOUR CODE IN HERE
            API.MakeMove(playerID, serverURL, 0);

            Console.WriteLine("Press to play");
            Console.ReadKey(true);
        }


    }
}
