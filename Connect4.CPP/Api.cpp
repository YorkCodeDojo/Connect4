#include "Api.h"

#include "rest/Response.hpp"
#include "rest/UrlRequest.hpp"

#include "json.hpp"
using json = nlohmann::json;

namespace Api
{

const std::string site("yorkdojoconnect4.azurewebsites.net");

bool Init()
{
#ifdef WIN32
    auto wVersionRequested = MAKEWORD(2, 2);
    WSAData wsaData;
    int ret = WSAStartup(wVersionRequested, &wsaData);
    return ret == 0;
#else
    return true;
#endif
}

std::string RegisterTeam(const std::string& teamName, const std::string& password)
{
    UrlRequest request;
    request.host(site.c_str());
    request.uri(registerApi.c_str(),
    {
        {teamNameParam.c_str(), teamName.c_str()},
        {passwordParam.c_str(), password.c_str()}
    });
    request.method("POST");
    request.addHeader("Content-Type: application/json\nContent-Length: 0");
    try
    {
        auto response = std::move(request.perform());
        if (response.statusCode() == 200)
        {
            std::string body = response.body();
            // Unquote the response
            body = body.erase(0, 1);
            body = body.erase(body.size() - 1);
            return body;
        }
        else
        {
            cout << "status code = " << response.statusCode() << ", description = " << response.statusDescription() << endl;
            return "";
        }
    }
    catch (std::exception&)
    {
        return "";
    }
}

bool GameState(const std::string& playerID, Game& game)
{
    UrlRequest request;
    request.host(site.c_str());
    request.uri(getGameApi.c_str(),
    {
        {playerIDParam.c_str(), playerID}
    });
    request.addHeader("Content-Type: application/json\nContent-Length: 0");
    try
    {
        auto response = std::move(request.perform());
        if (response.statusCode() == 200)
        {
            std::string body = response.body();
            auto gameData = json::parse(body);
            game.PlayerID = playerID;
            game.CurrentState = CurrentGameState(gameData["CurrentState"].get<int>());
            game.YellowPlayerID = gameData["YellowPlayerID"].get<std::string>();
            game.RedPlayerID = gameData["RedPlayerID"].get<std::string>();
            game.ID = gameData["ID"].get<std::string>();

            auto cells = gameData["Cells"];
            for (int column = 0; column < Game::NUMBER_OF_COLUMNS; column++)
            {
                for (int row = 0; row < Game::NUMBER_OF_ROWS; row++)
                {
                    game.Cells[column][row] = CellContent(cells[column][row].get<int>());
                }
            }
            return true;
        }
        else
        {
            cout << "status code = " << response.statusCode() << ", description = " << response.statusDescription() << endl;
            return "";
        }
    }
    catch (std::exception&)
    {
        return "";
    }
}
bool MakeMove(const std::string& playerID, const std::string& password, int column)
{
    UrlRequest request;
    request.host(site.c_str());
    request.uri(makeMoveApi.c_str(),
    {
        {playerIDParam.c_str(), playerID},
        {passwordParam.c_str(), password},
        {columnNumberParam.c_str(), column},
    });
    request.method("POST");
    request.addHeader("Content-Type: application/json\nContent-Length: 0");
    try
    {
        auto response = std::move(request.perform());
        if (response.statusCode() == 200)
        {
            return true;
        }
        else
        {
            cout << "status code = " << response.statusCode() << ", description = " << response.statusDescription() << endl;
            return false;
        }
    }
    catch (std::exception&)
    {
        return "";
    }
}

bool NewGame(const std::string& playerID)
{
    UrlRequest request;
    request.host(site.c_str());
    request.uri(newGameApi.c_str(),
    {
        {playerIDParam.c_str(), playerID}
    });
    request.method("POST");
    request.addHeader("Content-Type: application/json\nContent-Length: 0");
    try
    {
        auto response = std::move(request.perform());
        if (response.statusCode() == 200)
        {
            return true;
        }
        else
        {
            cout << "status code = " << response.statusCode() << ", description = " << response.statusDescription() << endl;
            return false;
        }
    }
    catch (std::exception&)
    {
        return false;
    }
}

std::string GetStatusString(Api::Game& game, bool& finished)
{
    switch (game.CurrentState)
    {
    case Api::CurrentGameState::YellowToPlay:
        finished = false;
        if (game.PlayerID == game.YellowPlayerID)
        {
            return "Our move (Yellow)";
        }
        else
        {
            return "Their move (Yellow)";
        }
        break;
    case Api::CurrentGameState::RedToPlay:
        finished = false;
        if (game.PlayerID == game.RedPlayerID)
        {
            return "Our move (Red)";
        }
        else
        {
            return "Their move (Red)";
        }
        break;
    case Api::CurrentGameState::Draw:
        finished = true;
        return "Draw";
        break;
    case Api::CurrentGameState::GameNotStarted:
        finished = false;
        return "Not Started";
        break;
    case Api::CurrentGameState::RedWon:
        finished = true;
        if (game.PlayerID == game.RedPlayerID)
        {
            return "We Won (Red)";
        }
        else
        {
            return "We Lost (Yellow)";
        }
        break;
    case Api::CurrentGameState::YellowWon:
        finished = true;
        if (game.PlayerID == game.YellowPlayerID)
        {
            return "We Won (Yellow)";
        }
        else
        {
            return "We Lost (Red)";
        }
        break;
    default:
        break;
    }
    finished = true;
    return "";
}

}
