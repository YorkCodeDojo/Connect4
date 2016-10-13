using System;
using System.Collections.Generic;

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

        public List<int> GetAvailableMoves()
        {
            var result = new List<int>();

            for (int columnToCheck = 0; columnToCheck < NUMBER_OF_COLUMNS; columnToCheck++)
            {
                if (this.Cells[columnToCheck, NUMBER_OF_ROWS-1] == CellContent.Empty)
                {
                    result.Add(columnToCheck);
                }
            }

            return result;
        }

        public Game Clone()
        {
            return new Game()
            {
                CurrentState = this.CurrentState,
                ID = this.ID,
                RedPlayerID = this.RedPlayerID,
                YellowPlayerID = this.YellowPlayerID,
                Cells = CloneCells()
            };
        }

        private CellContent[,] CloneCells()
        {
            var clone = new CellContent[NUMBER_OF_COLUMNS, NUMBER_OF_ROWS];
            for (var c = 0; c < NUMBER_OF_COLUMNS; c++)
            {
                for (var r = 0; r < NUMBER_OF_ROWS; r++)
                {
                    clone[c, r] = this.Cells[c, r];
                }
            }
            return clone;
        }
    }
}