using System;

namespace EsthersConnectFour
{
    public class Game
    {
        public const int NUMBER_OF_COLUMNS = 7;
        public const int NUMBER_OF_ROWS = 6;

        public CellContent[,] Cells = new CellContent[NUMBER_OF_COLUMNS, NUMBER_OF_ROWS];

        public GameState CurrentState;
        public Guid YellowPlayerID { get; set; }
        public Guid RedPlayerID { get; set; }
        public Guid ID { get; set; }

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

        internal bool Play(int column, CellContent cellContent)
        {
            for (int row = NUMBER_OF_ROWS - 1; row >= 0; row--)
            {
                if (Cells[column,row] == CellContent.Empty)
                {
                    Cells[column, row] = cellContent;
                    return true;
                }
            }

            return false;
        }

        internal int NumberOfCountersInColumn(int column)
        {
            var result = 0;
            for (int row = NUMBER_OF_ROWS - 1; row >= 0; row--)
            {
                if (Cells[column, row] == CellContent.Empty)
                {
                    break;
                }
                result++;
            }

            return result;
        }
    }
}
