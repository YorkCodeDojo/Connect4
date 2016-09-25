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
            var board = new Board(10, 8);
            board.MakeMove(1, true);
            var actualResult = board.WasWinningMove(1, true);
            Assert.IsFalse(actualResult);
        }

        [TestMethod]
        public void CheckForWinningHorizontalMove()
        {
            var board = new Board(10, 8);
            board.MakeMove(1, true);
            board.MakeMove(2, true);
            board.MakeMove(4, true);

            board.MakeMove(3, true);
            var actualResult = board.WasWinningMove(3, true);
            Assert.IsTrue(actualResult);
        }

        [TestMethod]
        public void CheckForWinningVerticalMove()
        {
            var board = new Board(10, 8);
            board.MakeMove(2, true);
            board.MakeMove(2, true);
            board.MakeMove(2, true);

            board.MakeMove(2, true);
            var actualResult = board.WasWinningMove(2, true);
            Assert.IsTrue(actualResult);
        }

        [TestMethod]
        public void CheckForWinningDiagonalSEtoNW_Move()
        {
            var board = new Board(10, 8);
            MakeNonWinningMove(board, 1, RED);
            MakeNonWinningMove(board, 1, YELLOW);
            MakeNonWinningMove(board, 1, RED);
            MakeNonWinningMove(board, 1, YELLOW);

            MakeNonWinningMove(board, 2, YELLOW);
            MakeNonWinningMove(board, 2, RED);
            MakeNonWinningMove(board, 2, YELLOW);

            MakeNonWinningMove(board, 3, RED);
            MakeNonWinningMove(board, 3, YELLOW);

            board.MakeMove(4, YELLOW);
            var actualResult = board.WasWinningMove(4, YELLOW);
            Assert.IsTrue(actualResult);
        }


        [TestMethod]
        public void CheckForWinningDiagonalSWtoNE_Move()
        {
            var board = new Board(10, 8);
            MakeNonWinningMove(board, 4, RED);
            MakeNonWinningMove(board, 4, YELLOW);
            MakeNonWinningMove(board, 4, RED);
            MakeNonWinningMove(board, 4, YELLOW);

            MakeNonWinningMove(board, 3, YELLOW);
            MakeNonWinningMove(board, 3, RED);
            MakeNonWinningMove(board, 3, YELLOW);

            MakeNonWinningMove(board, 2, RED);
            MakeNonWinningMove(board, 2, YELLOW);

            board.MakeMove(1, YELLOW);
            var actualResult = board.WasWinningMove(1, YELLOW);
            Assert.IsTrue(actualResult);
        }

        private static void MakeNonWinningMove(Board board, int columnNumber, bool isYellow)
        {
            board.MakeMove(columnNumber, isYellow);
            Assert.IsFalse(board.WasWinningMove(columnNumber, isYellow));
        }

        [TestMethod]
        public void CheckNewBoardIsNotFull()
        {
            var board = new Board(10, 8);
            var actualResult = board.IsBoardFull();
            Assert.IsFalse(actualResult);
        }

        [TestMethod]
        public void CheckFullyPlayedBoardIsFull()
        {
            var board = new Board(2, 2);

            for (int c = 0; c < board.NumberOfColumns; c++)
            {
                for (int r = 0; r < board.NumberOfRows; r++)
                {
                    board.MakeMove(c, true);
                }
            }

            var actualResult = board.IsBoardFull();
            Assert.IsTrue(actualResult);
        }

        [TestMethod]
        public void CheckHeightOfEmptyColumnIs0()
        {
            var board = new Board(10, 8);
            var actualResult = board.GetHeightOfColumn(1);
            Assert.AreEqual(0, actualResult);
        }

        [TestMethod]
        public void CheckHeightOfPlayedColumnIsCorrect()
        {
            var board = new Board(10, 8);
            board.MakeMove(1, true);
            board.MakeMove(1, true);
            board.MakeMove(1, true);

            var actualResult = board.GetHeightOfColumn(1);
            Assert.AreEqual(3, actualResult);
        }


        [TestMethod]
        public void TestMoveCannotBeMadeToInvalidNegativeColumn()
        {
            var board = new Board(10, 8);

            var actualResult = board.IsMoveAllowed(-1);
            Assert.IsFalse(actualResult);
        }

        [TestMethod]
        public void TestMoveCannotBeMadeToInvalidPostiveColumn()
        {
            var board = new Board(10, 8);

            //valid is 0...n-1
            var actualResult = board.IsMoveAllowed(board.NumberOfColumns);
            Assert.IsFalse(actualResult);
        }

        [TestMethod]
        public void TestMoveCanBeMadeToValidColumn()
        {
            var board = new Board(10, 8);

            var actualResult = board.IsMoveAllowed(1) && board.IsMoveAllowed(board.NumberOfColumns - 1);
            Assert.IsTrue(actualResult);
        }
    }
}
