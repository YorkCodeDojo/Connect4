using System;
using System.Linq;
using System.Threading.Tasks;


public class MonteCarlo
{
    public int SuggestMove(Game game, Guid playerID)
    {
        var weAreYellow = (game.YellowPlayerID == playerID);

        // Which move will give us the highest score?
        var winningColumns = new System.Collections.Concurrent.ConcurrentBag<Tuple<int, int>>();   //Column, Score

        // Where can be played
        var availableMoves = game.GetAvailableMoves();

        Parallel.For(0, 100000, g =>
        {
                //Pick a first column at random
                var column = availableMoves[new Random().Next(availableMoves.Count)];

                //Play the game
                var didYellowWin = PlayGame(game, weAreYellow, column);

                //If we won,  remember which column we play first
                if (didYellowWin.HasValue)
                if ((didYellowWin.Value == weAreYellow))
                    winningColumns.Add(new Tuple<int, int>(column, 1));
                else
                    winningColumns.Add(new Tuple<int, int>(column, -1));

        });

        // Which column won the most
        var columnScores = winningColumns.GroupBy(a => a);

        // Add up the totals in each group
        var bestScore = int.MinValue;
        var bestColumn = -1;
        foreach (var columnScore in columnScores)
        {
            var columnNumber = columnScore.Key.Item1;
            var total = columnScore.Select(a => a.Item2).Sum();
            if (total > bestScore)
            {
                bestScore = total;
                bestColumn = columnNumber;
            }
        }

        if (bestColumn == -1)
        {
            // Just play the first column
            for (int column = 0; column < Game.NUMBER_OF_COLUMNS; column++)
            {
                if (game.IsValidMove(column))
                {
                    bestColumn = column;
                    break;
                }
            }
        }

        return bestColumn;

    }

    /// <summary>
    /// Returns TRUE if yellow won
    /// NULL for a draw
    /// </summary>
    /// <param name="game"></param>
    /// <param name="itIsYellowsTurn"></param>
    /// <param name="columnToPlay"></param>
    /// <param name="doWeWantYellowToWin"></param>
    /// <returns></returns>
    private static bool? PlayGame(Game game, bool itIsYellowsTurn, int columnToPlay)
    {
        //Would we have won?
        if (game.IsWinningMove(columnToPlay, itIsYellowsTurn))
        {
            return itIsYellowsTurn;
        }

        //Make the move
        var newGame = game.EffectOfMakingMove(itIsYellowsTurn, columnToPlay);

        //Play the next move at random
        var availableMoves = newGame.GetAvailableMoves();
        if (!availableMoves.Any()) return null;  //draw
        while (true)
        {
            var nextColumnToPlay = availableMoves[new Random().Next(availableMoves.Count)];
            if (newGame.IsValidMove(nextColumnToPlay))
                return PlayGame(newGame, !itIsYellowsTurn, nextColumnToPlay);
        }
    }
}
