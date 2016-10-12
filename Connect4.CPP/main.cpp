#include "Api.h"
#include <cstdio>
#include <iostream>
#include <sstream>
#include <vector>
#include <thread>
#include <chrono>

#ifdef  WIN32
#include "DrawWindow.h"
#endif

const std::string password("Foobar");
const std::string teamName("The_Morninator");

void PrintBoard(Api::Game& game)
{
    SetGame(game);
    std::ostringstream str;
    str << "---------" << std::endl;
    for (int row = Api::Game::NUMBER_OF_ROWS - 1; row >= 0; row--)
    {
        for (int column = 0; column < Api::Game::NUMBER_OF_COLUMNS; column++)
        {
            if (column == 0)
            {
                str << "|";
            }

            switch (game.Cells[column][row])
            {
            case Api::CellContent::Empty:
                str << "O";
                break;
            case Api::CellContent::Red:
                str << "R";
                break;
            case Api::CellContent::Yellow:
                str << "Y";
                break;
            }

            if (column == Api::Game::NUMBER_OF_COLUMNS - 1)
            {
                str << "|";
            }
        }
        str << std::endl;
    }
    str << "---------" << std::endl;
    bool finished;
    auto statusText = Api::GetStatusString(game, finished);
    str << statusText << std::endl << std::endl;
    std::cout << str.str();
}

bool MyTurn(Api::Game& game, const std::string& playerID)
{
    if (game.CurrentState == Api::CurrentGameState::YellowToPlay)
    {
        return playerID == game.YellowPlayerID;
    }
    else if (game.CurrentState == Api::CurrentGameState::RedToPlay)
    {
        return playerID == game.RedPlayerID;
    }
    return false;
}

struct Location
{
    int column;
    int row;
};
std::vector<Location> GetValidMoves(Api::Game& game)
{
    std::vector<Location> moves;
    for (int column = 0; column < Api::Game::NUMBER_OF_COLUMNS; column++)
    {
        for (int row = 0; row < Api::Game::NUMBER_OF_ROWS; row++)
        {
            if (game.Cells[column][row] == Api::CellContent::Empty)
            {
                moves.push_back(Location{ column, row });
                break;
            }
        }
    }
    return moves;
}

bool PlayMove(Api::Game& game, const std::string& playerID)
{
    auto moves = GetValidMoves(game);

    if (moves.empty())
        return false;

    int moveNum = (rand() % moves.size());

    // Add the play, so we can 'see' it before the server returns
    if (Api::MakeMove(playerID, password, moves[moveNum].column))
    {
        game.Cells[moves[moveNum].column][moves[moveNum].row] = game.CurrentState == Api::CurrentGameState::YellowToPlay ? Api::CellContent::Yellow : Api::CellContent::Red;
        game.CurrentState = game.CurrentState == Api::CurrentGameState::YellowToPlay ? Api::CurrentGameState::RedToPlay : Api::CurrentGameState::YellowToPlay;
        return true;
    }
    return false;
}

int main(int num, void** ppArg)
{
    if (!Api::Init())
    {
        std::cout << "Failed init!" << std::endl;
        return 1;
    }

    std::string playerID = Api::RegisterTeam(teamName, password);
    if (playerID.empty())
    {
        std::cout << "Failed team register!" << std::endl;
        return 1;
    }

#ifdef WIN32
    CreateGameWindow();
#endif

    do
    {
        Api::Game game;
        if (!Api::GameState(playerID, game))
        {
            std::cout << "Game state failed!" << std::endl;
            return 1;
        }

        PrintBoard(game);
        std::this_thread::sleep_for(std::chrono::seconds(1));

        bool finished;
        auto statusText = Api::GetStatusString(game, finished);
        if (!finished)
        {
            if (MyTurn(game, playerID))
            {
                PlayMove(game, playerID);
            }
            PrintBoard(game);
            std::this_thread::sleep_for(std::chrono::seconds(1));
        }
        else
        {
            std::cout << "n for new game, q to quit:";
            auto k = getchar();
            if (k == 'n')
            {
                Api::NewGame(playerID);
            }
            else if (k == 'q')
            {
                break;
            }
        }
    } while (1);

}