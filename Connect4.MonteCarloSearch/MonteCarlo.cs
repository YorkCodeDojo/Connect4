using System;
using System.Linq;
using System.Threading.Tasks;

/// <summary>
/// First we explore all nodes in the tree to depth MAX_DEPTH
/// Then we play a number of games randomly until we reach their terminal nodes
/// The number of games played, and number won this then held against the parent nodes
/// and updated back up the tree 
/// </summary>
public class MonteCarloSearch
{
    private const int MAX_DEPTH = 3;
    private const int NO_OF_RANDOM_GAMES_PER_NODE = 200;

    /// <summary>
    /// Uses Monte Carlo search to suggest the number column to play in.
    /// </summary>
    /// <param name="game"></param>
    /// <param name="playerID"></param>
    /// <returns></returns>
    internal int SuggestMove(Game game, Guid playerID)
    {
        var playerIsYellow = game.YellowPlayerID == playerID;

        // Start with the current state of the board.
        var currentState = new Node() { NumberOfWins = 0, PlayCount = 0, Game = game, Parent = null };

        // Work out which is the best out of the available moves.
        var availableMoves = game.GetAvailableMoves();

        //Score, ColumnNumber
        var scores = new System.Collections.Concurrent.ConcurrentBag<Tuple<decimal, int>>();

        Parallel.For(0, availableMoves.Count, columnToPlay =>
        {
            // Have we just won,  in which case we are done!
            if (game.IsWinningMove(columnToPlay, playerIsYellow))
            {
                scores.Add(new Tuple<decimal, int>(decimal.MaxValue, columnToPlay));
            }
            else
            {
                // Get the game after applying the move
                var childGame = game.EffectOfMakingMove(playerIsYellow, columnToPlay);
                var childNode = new Node() { NumberOfWins = 0, PlayCount = 0, Game = childGame, Parent = null };

                // How successfully were we playing down from here?
                PlayToDepth(1, childNode, !playerIsYellow, playerIsYellow);

                if (childNode.PlayCount > 0)
                {
                    decimal score = (decimal)childNode.NumberOfWins / childNode.PlayCount;
                    scores.Add(new Tuple<decimal, int>(score, columnToPlay));
                }
            }
        });

        // Find the highest scoring column
        var bestScore = decimal.MinValue;
        var bestColumn = -1;
        foreach (var result in scores)
        {
            if (result.Item1 > bestScore)
            {
                bestScore = result.Item1;
                bestColumn = result.Item2;
            }
        }

        return bestColumn;
    }

    public void PlayToDepth(int depth, Node parentNode, bool yellowNextToPlay, bool playerIsYellow)
    {
        if (depth == MAX_DEPTH)
        {
            // We have expanded enough nodes,  we can now start playing at random.
            parentNode.NumberOfWins = PlayRandomGames(parentNode, yellowNextToPlay, NO_OF_RANDOM_GAMES_PER_NODE, playerIsYellow);
            parentNode.PlayCount = NO_OF_RANDOM_GAMES_PER_NODE;

            // Feed this back up the tree
            while (parentNode.Parent != null)
            {
                parentNode.Parent.NumberOfWins += parentNode.NumberOfWins;
                parentNode.Parent.PlayCount += parentNode.PlayCount;
                parentNode = parentNode.Parent;
            }

        }
        else
        {
            // We need to go down at least one more level
            var availableMoves = parentNode.Game.GetAvailableMoves();
            foreach (var columnToPlay in availableMoves)
            {
                // Have we just won,  in which case we are done!
                if (parentNode.Game.IsWinningMove(columnToPlay, yellowNextToPlay))
                {
                    if (playerIsYellow == yellowNextToPlay)
                    {
                        parentNode.NumberOfWins++;
                        parentNode.PlayCount++;
                    }
                    return;
                }

                // Get the game after applying the move
                var childGame = parentNode.Game.EffectOfMakingMove(yellowNextToPlay, columnToPlay);
                var childNode = new Node() { NumberOfWins = 0, PlayCount = 0, Game = childGame, Parent = parentNode };

                // Keep going
                PlayToDepth(depth + 1, childNode, !yellowNextToPlay, playerIsYellow);
            }
        }
    }

    /// <summary>
    /// Plays numberOfGamesToPlay games as random and returns the number which we won 
    /// </summary>
    /// <param name="parentNode"></param>
    /// <param name="yellowNextToPlay"></param>
    /// <param name="numberOfGamesToPlay"></param>
    /// <returns></returns>
    private int PlayRandomGames(Node parentNode, bool yellowNextToPlay, int numberOfGamesToPlay, bool playerIsYellow)
    {
        var numberOfWins = 0;
        for (int gameNumber = 0; gameNumber < numberOfGamesToPlay; gameNumber++)
        {
            var didYellowWin = PlayRandomGame(parentNode.Game, yellowNextToPlay);
            if (didYellowWin.HasValue)
            {
                if (didYellowWin.Value == playerIsYellow) numberOfWins++;
            }
        }

        return numberOfWins;
    }

    /// <summary>
    /// Play a game random until it reaches a conclusion.
    /// Returns
    /// True if yellow won
    /// False if red won
    /// NULL if it was a draw
    /// </summary>
    /// <param name="parentNode">The current state of the board</param>
    /// <param name="yellowNextToPlay">The next counter to place</param>
    /// <returns></returns>
    private bool? PlayRandomGame(Game parentNode, bool yellowNextToPlay)
    {
        //Play the next move at random
        var availableMoves = parentNode.GetAvailableMoves();
        if (!availableMoves.Any())
        {
            return null;  //draw
        }

        // Select a child node at random
        var columnToPlay = availableMoves[new Random().Next(availableMoves.Count)];

        // Would making this move result in a win?
        if (parentNode.IsWinningMove(columnToPlay, yellowNextToPlay))
        {
            return yellowNextToPlay;
        }
        else
        {
            // What does the board now look like?
            var childGame = parentNode.EffectOfMakingMove(yellowNextToPlay, columnToPlay);

            // Keep going
            return PlayRandomGame(childGame, !yellowNextToPlay);
        }
    }

}