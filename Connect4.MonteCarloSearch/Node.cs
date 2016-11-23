using System;

public class Node
{
    /// <summary>
    /// How many games have involved this node
    /// </summary>
    public int PlayCount { get; set; }

    /// <summary>
    /// How many games were won by the player is plays this node
    /// </summary>
    public int NumberOfWins { get; set; }

    public Game Game { get; set; }

    public Node Parent { get; set; }
}

