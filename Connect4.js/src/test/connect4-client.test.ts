import * as assert from 'assert'
import Connect4Client from '../app/connect4-client'

const playerId = '3089e741-4d30-4ed3-a77e-16500de4526e';
const teamName = 'connect4js-intergration-test';
const teamPassword = 'Pa$$w0rd1!';

describe('given a connect 4 client', () => {

    const client = new Connect4Client();

    describe('when getting a game', () => {
        const response = client.getGame(playerId);

        it('Should return a game id', () => {
            response.then(x => {
                assert.ok(x.body.ID);
            });
            return response;
        })

        it('Should return current game state', () => {
            response.then(x => {
                assert.ok(x.body.CurrentState);
            });
            return response;
        })

        it('Should return player ids', () => {
            response.then(x => {
                assert.ok(x.body.RedPlayerID);
                assert.ok(x.body.YellowPlayerID);
            });
            return response;
        })

        it('Should return correct cell grid', () => {
            response.then(x => {
                assert.equal(x.body.Cells.length, 7);

                for (const column of x.body.Cells) {
                    assert.equal(column.length, 6);
                }
            });
            return response;
        })

        return response;
    });

    describe('when registering a team', () => {
        const response = client.registerTeam(teamName, teamPassword);

        it('should return the team id', () => {

            response.then(x => {
                assert.ok(x.body);
            })

            return response;
        })

        return response;
    });

    describe('when making a move', () => {
        const response = client.makeMove(playerId, teamPassword, 0);

        it('should returns ok', () => {

            response.then(x => {
                assert.ok(x.ok)
            })

            return response
        });

        return response;
    });

    describe('when creating a new game', () => {
        const response = client.newGame(playerId);

        it('should returns ok', () => {

            response.then(x => {
                assert.ok(x.ok)
            })

            return response
        });

        return response;
    });
});