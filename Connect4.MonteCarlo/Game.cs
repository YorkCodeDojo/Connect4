using System;
using System.Collections.Generic;

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
            if (this.Cells[columnToCheck, NUMBER_OF_ROWS - 1] == CellContent.Empty)
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

    /// <summary>
    /// Would playing this move win?
    /// </summary>
    /// <param name="game"></param>
    /// <param name="column"></param>
    /// <param name="weAreYellow"></param>
    /// <returns></returns>
    public bool IsWinningMove(int columnToPlay, bool weAreYellow)
    {
        // Which row would the counter go in?
        var rowToPlay = -1;
        for (int row = 0; row < NUMBER_OF_ROWS; row++)
        {
            if (this.Cells[columnToPlay, row] == CellContent.Empty)
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
        while (r >= 0 && (this.Cells[c, r] == toMatch))
        {
            r--;
            chainLength++;
        }
        if (chainLength >= 3) return true;

        // Left to right
        r = rowToPlay;
        c = columnToPlay - 1;
        chainLength = 0;
        while (c >= 0 && (this.Cells[c, r] == toMatch))  //Left
        {
            c--;
            chainLength++;
        }
        c = columnToPlay + 1;
        while (c < NUMBER_OF_COLUMNS && (this.Cells[c, r] == toMatch))  //Left
        {
            c++;
            chainLength++;
        }
        if (chainLength >= 3) return true;

        // +vc diagonal
        r = rowToPlay - 1;
        c = columnToPlay - 1;
        chainLength = 0;
        while (c >= 0 && r >= 0 && (this.Cells[c, r] == toMatch))  //Left-down
        {
            c--;
            r--;
            chainLength++;
        }
        c = columnToPlay + 1;
        r = rowToPlay + 1;
        while (r < NUMBER_OF_ROWS && c < NUMBER_OF_COLUMNS && (this.Cells[c, r] == toMatch))  //Right-Up
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
        while (c >= 0 && r < NUMBER_OF_ROWS && (this.Cells[c, r] == toMatch))  //Left-up
        {
            c--; //left
            r++; //up
            chainLength++;
        }
        c = columnToPlay + 1;
        r = rowToPlay - 1;
        while (r >= 0 && c < NUMBER_OF_COLUMNS && (this.Cells[c, r] == toMatch))  //Right-down
        {
            c++;  //right
            r--;  //down
            chainLength++;
        }
        if (chainLength >= 3) return true;

        return false;

    }

    /// <summary>
    /// A move is valid if there is still space in the column
    /// </summary>
    /// <param name="game"></param>
    /// <param name="column"></param>
    /// <returns></returns>
    public bool IsValidMove(int column)
    {
        return (this.Cells[column, NUMBER_OF_ROWS - 1] == CellContent.Empty);
    }


    /// <summary>
    /// Returns a new board,  but with that move played
    /// </summary>
    /// <param name="game"></param>
    /// <param name="playerIsYellow"></param>
    /// <param name="column"></param>
    /// <returns></returns>
    public Game EffectOfMakingMove(bool playerIsYellow, int columnToPlay)
    {
        var newGame = this.Clone();

        // Play in the first space in the column
        for (int row = 0; row < NUMBER_OF_ROWS; row++)
        {
            if (newGame.Cells[columnToPlay, row] == CellContent.Empty)
            {
                newGame.Cells[columnToPlay, row] = (playerIsYellow ? CellContent.Yellow : CellContent.Red);
                break;
            }
        }

        return newGame;
    }
}
