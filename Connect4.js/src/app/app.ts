import 'source-map-support/register'
import Connect4Client, { GameState, IGame } from './connect4-client'

const teamName = "CoffeeZero";
const teamPassword = "1234321";

const connect4Client = new Connect4Client();

function playGame(playerId: string) {
  connect4Client.getGame(playerId)
    .then(x => {
      const game = x.body;
      switch (game.CurrentState) {
        case GameState.RedWon:
          console.log(game.RedPlayerID === playerId ? "You Won" : "You Lost");
          break;

        case GameState.YellowWon:
          console.log(game.YellowPlayerID === playerId ? "You Won" : "You Lost");
          break;

        case GameState.RedToPlay:
          if (game.RedPlayerID === playerId) {
            makeMove(game, playerId)
              .then(x => { setTimeout(() => playGame(playerId), 1000); });
          }
          break;

        case GameState.YellowToPlay:
          if (game.YellowPlayerID === playerId) {
            makeMove(game, playerId)
              .then(x => { setTimeout(() => playGame(playerId), 1000); });
          }
          break;

        case GameState.Draw:
          console.log("Draw");
          break;

        case GameState.GameNotStarted:
        default:
          break;
      }
    });
}

function makeMove(game: IGame, playerId: string): Promise<void> {
  // PUT YOUR CODE IN HERE
  // Place a counter in the first column
  return connect4Client.makeMove(playerId, teamPassword, 0);
}

connect4Client.registerTeam(teamName, teamPassword)
  .then(x => playGame(x.body));
