using System;
using Connect4.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Connect4.Tests
{
    [TestClass]
    public class UnitTest1
    {
        private const bool YELLOW = true;
        private const bool RED = false;

        [TestMethod]
        public void CheckPlayingANewBoardIsNotAWin()
        {
            var game = new Game();


            var board = new BoardAPI(Game.NUMBER_OF_COLUMNS, Game.NUMBER_OF_ROWS);
            board.MakeMove(game.Cells, 1, true);
            var actualResult = board.WasWinningMove(game.Cells, 1, true);
            Assert.IsFalse(actualResult);
        }

        [TestMethod]
        public void CheckForWinningHorizontalMove()
        {
            var game = new Game();
            var board = new BoardAPI(Game.NUMBER_OF_COLUMNS, Game.NUMBER_OF_ROWS);
            board.MakeMove(game.Cells, 1, true);
            board.MakeMove(game.Cells, 2, true);
            board.MakeMove(game.Cells, 4, true);

            board.MakeMove(game.Cells, 3, true);
            var actualResult = board.WasWinningMove(game.Cells, 3, true);
            Assert.IsTrue(actualResult);
        }

        [TestMethod]
        public void CheckForWinningVerticalMove()
        {
            var game = new Game();
            var board = new BoardAPI(Game.NUMBER_OF_COLUMNS, Game.NUMBER_OF_ROWS);
            board.MakeMove(game.Cells, 2, true);
            board.MakeMove(game.Cells, 2, true);
            board.MakeMove(game.Cells, 2, true);

            board.MakeMove(game.Cells, 2, true);
            var actualResult = board.WasWinningMove(game.Cells, 2, true);
            Assert.IsTrue(actualResult);
        }

        [TestMethod]
        public void CheckForWinningDiagonalSEtoNW_Move()
        {
            var game = new Game();
            var board = new BoardAPI(Game.NUMBER_OF_COLUMNS, Game.NUMBER_OF_ROWS);
            MakeNonWinningMove(game, 1, RED);
            MakeNonWinningMove(game, 1, YELLOW);
            MakeNonWinningMove(game, 1, RED);
            MakeNonWinningMove(game, 1, YELLOW);

            MakeNonWinningMove(game, 2, YELLOW);
            MakeNonWinningMove(game, 2, RED);
            MakeNonWinningMove(game, 2, YELLOW);

            MakeNonWinningMove(game, 3, RED);
            MakeNonWinningMove(game, 3, YELLOW);

            board.MakeMove(game.Cells, 4, YELLOW);
            var actualResult = board.WasWinningMove(game.Cells, 4, YELLOW);
            Assert.IsTrue(actualResult);
        }


        [TestMethod]
        public void CheckForWinningDiagonalSWtoNE_Move()
        {
            var game = new Game();
            var board = new BoardAPI(Game.NUMBER_OF_COLUMNS, Game.NUMBER_OF_ROWS);
            MakeNonWinningMove(game, 4, RED);
            MakeNonWinningMove(game, 4, YELLOW);
            MakeNonWinningMove(game, 4, RED);
            MakeNonWinningMove(game, 4, YELLOW);

            MakeNonWinningMove(game, 3, YELLOW);
            MakeNonWinningMove(game, 3, RED);
            MakeNonWinningMove(game, 3, YELLOW);

            MakeNonWinningMove(game, 2, RED);
            MakeNonWinningMove(game, 2, YELLOW);

            board.MakeMove(game.Cells, 1, YELLOW);
            var actualResult = board.WasWinningMove(game.Cells, 1, YELLOW);
            Assert.IsTrue(actualResult);
        }

        private static void MakeNonWinningMove(Game game, int columnNumber, bool isYellow)
        {
            var board = new BoardAPI(Game.NUMBER_OF_COLUMNS, Game.NUMBER_OF_ROWS);
            board.MakeMove(game.Cells, columnNumber, isYellow);
            Assert.IsFalse(board.WasWinningMove(game.Cells, columnNumber, isYellow));
        }

        [TestMethod]
        public void CheckNewBoardIsNotFull()
        {
            var game = new Game();
            var board = new BoardAPI(Game.NUMBER_OF_COLUMNS, Game.NUMBER_OF_ROWS);
            var actualResult = board.IsBoardFull(game.Cells);
            Assert.IsFalse(actualResult);
        }

        [TestMethod]
        public void CheckFullyPlayedBoardIsFull()
        {
            var game = new Game();
            var board = new BoardAPI(Game.NUMBER_OF_COLUMNS, Game.NUMBER_OF_ROWS);

            for (int c = 0; c < Game.NUMBER_OF_COLUMNS; c++)
            {
                for (int r = 0; r < Game.NUMBER_OF_ROWS; r++)
                {
                    board.MakeMove(game.Cells, c, true);
                }
            }

            var actualResult = board.IsBoardFull(game.Cells);
            Assert.IsTrue(actualResult);
        }

        [TestMethod]
        public void CheckHeightOfEmptyColumnIs0()
        {
            var game = new Game();
            var board = new BoardAPI(10, 8);
            var actualResult = board.GetHeightOfColumn(game.Cells, 1);
            Assert.AreEqual(0, actualResult);
        }

        [TestMethod]
        public void CheckHeightOfPlayedColumnIsCorrect()
        {
            var game = new Game();
            var board = new BoardAPI(10, 8);
            board.MakeMove(game.Cells, 1, true);
            board.MakeMove(game.Cells, 1, true);
            board.MakeMove(game.Cells, 1, true);

            var actualResult = board.GetHeightOfColumn(game.Cells, 1);
            Assert.AreEqual(3, actualResult);
        }


        [TestMethod]
        public void TestMoveCannotBeMadeToInvalidNegativeColumn()
        {
            var game = new Game();
            var board = new BoardAPI(10, 8);

            var actualResult = board.IsMoveAllowed(game.Cells, -1);
            Assert.IsFalse(actualResult);
        }

        [TestMethod]
        public void TestMoveCannotBeMadeToInvalidPostiveColumn()
        {
            var game = new Game();
            var board = new BoardAPI(Game.NUMBER_OF_COLUMNS, Game.NUMBER_OF_ROWS);

            //valid is 0...n-1
            var actualResult = board.IsMoveAllowed(game.Cells, Game.NUMBER_OF_COLUMNS);
            Assert.IsFalse(actualResult);
        }

        [TestMethod]
        public void TestMoveCanBeMadeToValidColumn()
        {
            var game = new Game();
            var board = new BoardAPI(10, 8);

            var actualResult = board.IsMoveAllowed(game.Cells, 1) && board.IsMoveAllowed(game.Cells, Game.NUMBER_OF_COLUMNS - 1);
            Assert.IsTrue(actualResult);
        }
    }
}
