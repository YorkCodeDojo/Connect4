using System;

namespace Connect4.ExampleBot
{
    public enum GameState
    {
        GameNotStarted = 0,
        RedWon = 1,
        YellowWon = 2,
        RedToPlay = 3,
        YellowToPlay = 4,
        Draw = 5
    }




    public enum CellContent
    {
        Empty = 0,
        Red = 1,
        Yellow = 2
    }
    public class Game
    {
        public const int NUMBER_OF_COLUMNS = 7;
        public const int NUMBER_OF_ROWS = 6;

        public CellContent[,] Cells;

        public GameState CurrentState;
        public Guid YellowPlayerID { get; set; }
        public Guid RedPlayerID { get; set; }
        public Guid ID { get; set; }

    }
}