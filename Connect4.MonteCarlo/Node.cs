using System;

public class Node
{
    public int PlayCount { get; set; }

    public int NumberOfWins { get; set; }
    public int ColumnPlayed { get; set; }

    /// <summary>
    /// The game as a hashed string
    /// </summary>
    public string Hash { get; set; }

    // Cache the game so that we only have to create it once.
    private Game game;

    /// <summary>
    /// Converts this node back into a game
    /// </summary>
    /// <returns></returns>
    public Game AsGame()
    {
        if (game == null)
            game = new Game(this.Hash);

        return game;
    }
}

