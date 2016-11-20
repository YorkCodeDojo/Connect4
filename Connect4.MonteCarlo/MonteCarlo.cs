using System;
using System.Collections.Generic;
using System.Linq;

public class MonteCarlo
{
    // Nodes with statistics
    private Dictionary<string, Node> allKnownNodes = new Dictionary<string, Node>();

    /// <summary>
    /// Calculates Upper Confidence Bounds (UCB)
    /// </summary>
    /// <param name="v">the estimated value of the node</param>
    /// <param name="N">total number of times that its parent has been visited</param>
    /// <param name="n">of the times the node has been visited </param>
    /// <returns></returns>
    public double CalculateUCB(double v, int N, int n)
    {
        double C = 1.44; //C is a tunable bias parameter
        return v + C * Math.Sqrt((2 * Math.Log(N)) / n);
    }

    /// <summary>
    /// Performs the selection stage by returning the move which gives the highest UCB
    /// NULL can be returned if there are no more 
    /// </summary>
    /// <param name="parentNode"></param>
    /// <returns></returns>
    public Node Selection(Node parentNode, bool playerIsYellow, List<Node> playList)
    {
        // Find all the legal moves from this point
        var availableMoves = parentNode.AsGame().GetAvailableMoves();

        // Calculate the UCB for each move,  and then take the highest.  
        var bestUCB = 0D;
        var bestNode = default(Node);
        foreach (var columnToPlay in availableMoves)
        {
            // Get the game after applying the move
            var childGame = parentNode.AsGame().EffectOfMakingMove(playerIsYellow, columnToPlay);

            // Do we have the statistics for this node?
            var childHash = childGame.GenerateHash();
            var childNode = default(Node);
            if (this.allKnownNodes.TryGetValue(childHash, out childNode))
            {
                var ucb = CalculateUCB(childNode.NumberOfWins, parentNode.PlayCount, childNode.PlayCount);
                if (ucb > bestUCB)
                {
                    bestUCB = ucb;
                    bestNode = childNode;
                }

            }
            else
            {
                // We have a node without any statistics,  this ends the selection stage.
                return parentNode;
            }
        }

        // The statistics don't give us a node.
        if (bestNode == null) return parentNode;

        // bestNode is the best childnode,  recursive down from this point
        playList.Add(bestNode);
        return Selection(bestNode, !playerIsYellow, playList);
    }


    public Node Expansion(Node parentNode, bool playerIsYellow, List<Node> playList)
    {
        // Find all the legal moves from this point
        var availableMoves = parentNode.AsGame().GetAvailableMoves();

        // Select a child node at random
        var columnToPlay = availableMoves[new Random().Next(availableMoves.Count)];

        // What does the board now look like?
        var childGame = parentNode.AsGame().EffectOfMakingMove(playerIsYellow, columnToPlay);

        // Add the new node to our statistics tree
        var childNode = default(Node);
        var childHash = childGame.GenerateHash();
        if (!this.allKnownNodes.TryGetValue(childHash, out childNode))
        {
            childNode = new Node() { Hash = childHash, NumberOfWins = 0, PlayCount = 0, ColumnPlayed = columnToPlay };
            this.allKnownNodes.Add(childNode.Hash, childNode);
        }

        playList.Add(childNode);
        return childNode;
    }

    /// <summary>
    /// Keep playing moves at random until the game is complete.
    /// </summary>
    /// <param name="parentNode"></param>
    /// <param name="playerIsYellow"></param>
    /// <returns>True if yellow won,  False if red won and NULL for a draw</returns>
    public bool? Simulation(Node parentNode, bool playerIsYellow, List<Node> playList)
    {
        //Play the next move at random
        var availableMoves = parentNode.AsGame().GetAvailableMoves();
        if (!availableMoves.Any()) return null;  //draw

        // Select a child node at random
        var columnToPlay = availableMoves[new Random().Next(availableMoves.Count)];

        // What does the board now look like?
        var childGame = parentNode.AsGame().EffectOfMakingMove(playerIsYellow, columnToPlay);

        // Add the new node to our statistics tree
        var childNode = default(Node);
        var childHash = childGame.GenerateHash();
        if (!this.allKnownNodes.TryGetValue(childHash, out childNode))
        {
            childNode = new Node() { Hash = childHash, NumberOfWins = 0, PlayCount = 0, ColumnPlayed = columnToPlay };
            this.allKnownNodes.Add(childNode.Hash, childNode);
        }

        // Track the move
        playList.Add(childNode);

        // Would making this move result in a win?
        if (parentNode.AsGame().IsWinningMove(columnToPlay, playerIsYellow))
        {
            // Game over.
            return playerIsYellow;
        }
        else
        {
            // Keep going
            return Simulation(childNode, !playerIsYellow, playList);
        }

    }

    public void Update(List<Node> playList, bool playerIsYellow, bool yellowWon)
    {
        var yellowPlayedNode = (playList.Count % 2 == 0 ? playerIsYellow : !playerIsYellow);
        foreach (var node in playList)
        {
            node.NumberOfWins++;
            if (yellowWon == yellowPlayedNode) node.NumberOfWins++;
            yellowPlayedNode = !yellowPlayedNode;
        }
    }


    public int SuggestMove(Game game, Guid playerID)
    {
        const int NUMBER_OF_GAMES = 10000;

        // What colour are we playing?
        var weAreYellow = (game.YellowPlayerID == playerID);

        // The current state of the game
        var rootNode = new Node() { Hash = game.GenerateHash(), NumberOfWins = 0, PlayCount = 0 };

        for (int gameNumber = 1; gameNumber <= NUMBER_OF_GAMES; gameNumber++)
        {
            // The moves we get played out in this game
            var moves = new List<Node>();

            // 1. Peform selection based on statistics
            var bestNodeUsingStatistics = Selection(rootNode, weAreYellow, moves);

            // 2. Expansion
            var nextNode = Expansion(bestNodeUsingStatistics, (moves.Count % 2 == 0 ? weAreYellow : !weAreYellow), moves);

            // 3. Simulation
            var yellowWon = Simulation(nextNode, (moves.Count % 2 == 0 ? weAreYellow : !weAreYellow), moves);

            // 4. Back-propagation  (yellowWon will be NULL in the case of a draw)
            if (yellowWon.HasValue)
            {
                Update(moves, weAreYellow, yellowWon.Value);
            }

            if (gameNumber == NUMBER_OF_GAMES)
                return moves.First().ColumnPlayed;
        }

        return -1;
    }

}