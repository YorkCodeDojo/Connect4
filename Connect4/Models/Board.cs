using System.Diagnostics;

namespace Connect4.Models
{
    public enum CellContent
    {
        Empty = 0,
        Red = 1,
        Yellow = 2
    }

    public class Board
    {
        //Connect 4
        private const int CONNECT_LENGTH = 4;

        public int NumberOfColumns { get; private set; }
        public int NumberOfRows { get; private set; }
        public CellContent[,] Cells { get; private set; }

        public Board(int numberOfColumns, int numberOfRows)
        {
            this.NumberOfColumns = numberOfColumns;
            this.NumberOfRows = numberOfRows;
            this.Cells = new CellContent[numberOfColumns, numberOfRows];
        }

        public bool IsMoveAllowed(int columnNumber)
        {
            //Outside of the number of columns
            if (columnNumber < 0 || columnNumber + 1 > NumberOfColumns) return false;

            //Is the column already full
            var columnHeight = GetHeightOfColumn(columnNumber);
            if (columnHeight + 1 == this.NumberOfRows) return false;

            // All good
            return true;
        }

        /// <summary>
        /// 0...
        /// </summary>
        /// <param name="columnNumber"></param>
        /// <returns></returns>
        public int GetHeightOfColumn(int columnNumber)
        {
            for (int i = 0; i < NumberOfRows; i++)
            {
                if (Cells[columnNumber, i] == CellContent.Empty) return i;
            }

            //Column is full!
            return NumberOfRows;
        }

        public void MakeMove(int columnNumber, bool isYellow)
        {
            var columnHeight = GetHeightOfColumn(columnNumber);
            Cells[columnNumber, columnHeight] = isYellow ? CellContent.Yellow : CellContent.Red;
        }

        /// <summary>
        /// Are there any spaces left to play?
        /// </summary>
        /// <returns></returns>
        public bool IsBoardFull()
        {
            for (int colNumber = 0; colNumber < this.NumberOfColumns; colNumber++)
            {
                //Column has at least one free row
                if (this.GetHeightOfColumn(colNumber) < this.NumberOfRows) return false;
            }

            //No empty columns
            return true;
        }

        /// <summary>
        /// Make the move before calling this method
        /// </summary>
        /// <param name="columnNumber"></param>
        /// <param name="isYellow"></param>
        /// <returns></returns>
        public bool WasWinningMove(int columnNumber, bool isYellow)
        {
            //What colour is the player
            var playersColour = isYellow ? CellContent.Yellow : CellContent.Red;

            //Which row (0..) did the player put this counter in
            var rowNumber = this.GetHeightOfColumn(columnNumber)-1;
            Debug.Assert(rowNumber >= 0);

            //Horizontal W-E
            var connectLength = 0;
            for (int c = 0; c < this.NumberOfColumns; c++)
            {
                if (Cells[c, rowNumber] == playersColour)
                {
                    connectLength++;
                }
                else
                {
                    connectLength = 0;
                }

                if (connectLength == CONNECT_LENGTH) return true;
            }

            //Vertical S-N
            connectLength = 0;
            for (int r = 0; r < this.NumberOfRows; r++)
            {
                if (Cells[columnNumber, r] == playersColour)
                {
                    connectLength++;
                }
                else
                {
                    connectLength = 0;
                }

                if (connectLength == CONNECT_LENGTH) return true;
            }

            //Diagonal SW-NE
            // Played col 0,  so row =1, col=0,  start is row=0, col=col-row+1
            var startRow = rowNumber;
            var startColumn = columnNumber;
            while (startRow > 0 && startColumn > 0)
            {
                startRow--;
                startColumn--;
            }

            connectLength = 0;
            while (startRow < this.NumberOfRows && startColumn < this.NumberOfColumns)
            {
                if (Cells[startColumn, startRow] == playersColour)
                {
                    connectLength++;
                }
                else
                {
                    connectLength = 0;
                }

                if (connectLength == CONNECT_LENGTH) return true;

                startColumn++;
                startRow++;
            }

            //Diagonal SE-NW
            startRow = rowNumber;
            startColumn = columnNumber;
            while (startRow > 0 && startColumn < this.NumberOfColumns - 1)
            {
                startRow--;
                startColumn++;
            }

            connectLength = 0;
            while (startRow < this.NumberOfRows && startColumn >= 0)
            {
                if (Cells[startColumn, startRow] == playersColour)
                {
                    connectLength++;
                }
                else
                {
                    connectLength = 0;
                }

                if (connectLength == CONNECT_LENGTH) return true;

                startColumn--;
                startRow++;
            }

            return false;
        }

    }
}