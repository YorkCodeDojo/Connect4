#pragma once
#include <string>

namespace Api
{

enum class CurrentGameState
{
    GameNotStarted = 0,
    RedWon = 1,
    YellowWon = 2,
    RedToPlay = 3,
    YellowToPlay = 4,
    Draw = 5
};

enum class CellContent
{
    Empty = 0,
    Red = 1,
    Yellow = 2
};

struct Game
{
    static const int NUMBER_OF_COLUMNS = 7;
    static const int NUMBER_OF_ROWS = 6;

    CellContent Cells[NUMBER_OF_COLUMNS][NUMBER_OF_ROWS];

    CurrentGameState CurrentState;
    std::string YellowPlayerID;
    std::string RedPlayerID;
    std::string ID;
    std::string PlayerID;
};

const std::string passwordParam("password");
const std::string playerIDParam("playerID");

// Register
const std::string registerApi("/api/Register");
const std::string teamNameParam("teamName");

// GameState
const std::string getGameApi("/api/GameState");

// MakeMove
const std::string makeMoveApi("/api/MakeMove");
const std::string columnNumberParam("columnNumber");

// NewGame
const std::string newGameApi("/api/NewGame");

bool Init();
std::string RegisterTeam(const std::string& teamName, const std::string& password);
bool GameState(const std::string& playerID, Game& game);
bool MakeMove(const std::string& playerID, const std::string& password, int columnNumber);
bool NewGame(const std::string& playerID);
std::string GetStatusString(Api::Game& game, bool& finished);

}