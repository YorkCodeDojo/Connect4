using System;
using Newtonsoft.Json;

namespace Connect4.Models
{
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

        public CellContent[,] Cells { get; set; }

        public GameState CurrentState;

        public Guid YellowPlayerID { get; set; }
        public Guid RedPlayerID { get; set; }
        public Guid ID { get; set; }

        public Game()
        {
            this.Cells = new CellContent[NUMBER_OF_COLUMNS, NUMBER_OF_ROWS];
            this.CurrentState = GameState.YellowToPlay; //Yellow goes first
        }

        internal void MakeMove(bool isYellow, int columnNumber)
        {
            var boardAPI = new BoardAPI(Game.NUMBER_OF_COLUMNS, Game.NUMBER_OF_ROWS);
            boardAPI.MakeMove(this.Cells, columnNumber, isYellow);


            if (boardAPI.WasWinningMove(this.Cells, columnNumber, isYellow))
            {
                // Someone just won.
                this.CurrentState = isYellow ? GameState.YellowWon : GameState.RedWon;
            }
            else if (boardAPI.IsBoardFull(this.Cells))
            {
                // Draw
                this.CurrentState = GameState.Draw;
            }
            else
            {
                // Other players turn
                this.CurrentState = isYellow ? GameState.RedToPlay : GameState.YellowToPlay;
            }

        }

        internal bool IsMoveAllowed(int columnNumber)
        {
            var boardAPI = new BoardAPI(Game.NUMBER_OF_COLUMNS, Game.NUMBER_OF_ROWS);
            return boardAPI.IsMoveAllowed(this.Cells, columnNumber);
        }

        internal bool YellowToPlay() => this.CurrentState == GameState.YellowToPlay;
        internal bool RedToPlay() => this.CurrentState == GameState.RedToPlay;

        /// <summary>
        /// Serialises this game into a string
        /// </summary>
        /// <returns></returns>
        internal string AsState() => JsonConvert.SerializeObject(this);

        /// <summary>
        /// Creates a game from the state string
        /// </summary>
        /// <param name="state"></param>
        /// <returns></returns>
        internal static Game LoadFromState(string state) => JsonConvert.DeserializeObject<Game>(state);

    }
}