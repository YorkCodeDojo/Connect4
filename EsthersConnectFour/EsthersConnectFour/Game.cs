using System;
using System.Drawing;

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

        public bool CounterDropping { get; private set; }
        public int DropColumn { get; private set; }
        public DateTime DropStarted { get; private set; }
        public Brush DroppingColour { get; private set; }
        public int HighLightedColumn { get; internal set; }

        internal void StartDroppingCounter(Brush droppingColour, int highLightedColumn)
        {
            CounterDropping = true;
            DropColumn = highLightedColumn;
            DropStarted = DateTime.Now;
            DroppingColour = droppingColour;
            HighLightedColumn = -1;
        }

        internal double UpdateDroppingCounter(int boardHeight, int rowHeight)
        {
            var MSpassed = (DateTime.Now - DropStarted).TotalMilliseconds;
            var dropPerMS = boardHeight / 1000.0;
            var distanceDropped = MSpassed * dropPerMS;
            var counterNumber = NumberOfCountersInColumn(DropColumn);
            var maximumDrop = boardHeight - 70 - (rowHeight * counterNumber);

            if (maximumDrop <= (distanceDropped + 10))
            {
                CounterDropping = false;
                distanceDropped = -1;
            }

            return distanceDropped;
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

        internal bool CanPlay(int column)
        {
            for (int row = 0; row < NUMBER_OF_ROWS; row++)
            {
                if (Cells[column, row] == CellContent.Empty)
                {
                    return true;
                }
            }

            return false;
        }

        internal bool Play(int column, CellContent cellContent)
        {
            for (int row = 0; row < NUMBER_OF_ROWS; row++)
            {
                if (Cells[column, row] == CellContent.Empty)
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
            for (int row = 0; row < NUMBER_OF_ROWS; row++)
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
