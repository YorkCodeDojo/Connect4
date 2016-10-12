#include <cstdio>
#include "rest/Response.hpp"
#include "rest/UrlRequest.hpp"

int main(int num, void** ppArg)
{
    auto wVersionRequested = MAKEWORD(2, 2);
    WSAData wsaData;
    WSAStartup(wVersionRequested, &wsaData);

    UrlRequest request;
    request.host("yorkdojoconnect4.azurewebsites.net");
    request.uri("/api/Register", {
        {"teamName","The_Morninator"},
        {"password", "Foobar"}
    });
    request.method("POST");
    request.addHeader("Content-Type: application/json\nContent-Length: 0");
    auto response = std::move(request.perform());
    if (response.statusCode() == 200)
    {
        cout << "status code = " << response.statusCode() << ", body = *" << response.body() << "*" << endl;
    }
    else
    {
        cout << "status code = " << response.statusCode() << ", description = " << response.statusDescription() << endl;
    }
}