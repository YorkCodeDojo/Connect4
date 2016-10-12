#include "DrawWindow.h"
#include <thread>
#include <mutex>
using namespace Gdiplus;
#pragma comment (lib,"Gdiplus.lib")

HWND hWnd;
RECT WinRect;
const int WinSize = 512;
const int TextAreaSize = 50;
const int WinBorder = 6;
Api::Game gameView;
std::mutex viewMutex;

void SetGame(const Api::Game& game)
{
    std::lock_guard<std::mutex> lock(viewMutex);
    gameView = game;
    InvalidateRect(hWnd, nullptr, false);
}

VOID OnPaint(HDC hdc)
{
    std::lock_guard<std::mutex> lock(viewMutex);
   
    Bitmap *pMemBitmap = new Bitmap(WinSize, WinSize + TextAreaSize, PixelFormat24bppRGB);
    
    Graphics* pMemGraphics = Graphics::FromImage(pMemBitmap);
    pMemGraphics->SetInterpolationMode(InterpolationMode::InterpolationModeHighQualityBicubic);
    pMemGraphics->SetSmoothingMode(SmoothingMode::SmoothingModeAntiAlias);

    Graphics graphics(hdc);
    graphics.SetInterpolationMode(InterpolationMode::InterpolationModeHighQuality);

    Font font(&FontFamily(L"Arial"), 12);
    pMemGraphics->Clear(Color::Black);

    int BoardWidth = WinSize - WinBorder * 2;
    int BoardHeight = WinSize - WinBorder * 2;
    int BoardOrigin = WinBorder;
    int CircleArea = BoardWidth - WinBorder * 2;
    int CircleOrigin = WinBorder * 2;
    int circleSize = int(CircleArea / 7.5f);
    int circleRemainX = CircleArea - (circleSize * Api::Game::NUMBER_OF_COLUMNS);
    int circleRemainY = CircleArea - (circleSize * Api::Game::NUMBER_OF_ROWS);
    int circlePadX = circleRemainX / (Api::Game::NUMBER_OF_COLUMNS - 1);
    int circlePadY = circleRemainY / (Api::Game::NUMBER_OF_ROWS - 1);
    SolidBrush blueBrush(Color::Blue);
    SolidBrush redBrush(Color::Red);
    SolidBrush yellowBrush(Color::Yellow);
    SolidBrush whiteBrush(Color::White);
    Pen blueLightBrush(Color::CornflowerBlue);
    blueLightBrush.SetWidth(4);

    pMemGraphics->FillRectangle(&blueBrush, BoardOrigin, BoardOrigin, BoardWidth, BoardHeight);
    for (int row = Api::Game::NUMBER_OF_ROWS - 1; row >= 0; row--)
    {
        for (int column = 0; column < Api::Game::NUMBER_OF_COLUMNS; column++)
        {
            int circleX = CircleOrigin + circleSize * column + circlePadX * column;
            int circleY = CircleOrigin + circleSize * row + circlePadY * row;
            circleY = WinSize  - circleY - circleSize;

            switch (gameView.Cells[column][row])
            {
            case Api::CellContent::Empty:
                pMemGraphics->FillEllipse(&whiteBrush, circleX, circleY, circleSize, circleSize);
                pMemGraphics->DrawEllipse(&blueLightBrush, circleX, circleY, circleSize, circleSize);
                break;
            case Api::CellContent::Red:
                pMemGraphics->FillEllipse(&redBrush, circleX, circleY, circleSize, circleSize);
                pMemGraphics->DrawEllipse(&blueLightBrush, circleX, circleY, circleSize, circleSize);
                break;
            case Api::CellContent::Yellow:
                pMemGraphics->FillEllipse(&yellowBrush, circleX, circleY, circleSize, circleSize);
                pMemGraphics->DrawEllipse(&blueLightBrush, circleX, circleY, circleSize, circleSize);
                break;
            }
        }
    }

    bool finished;
    auto statusText = Api::GetStatusString(gameView, finished);
    pMemGraphics->DrawString(std::wstring(statusText.begin(), statusText.end()).c_str(), statusText.length(), &font, PointF(REAL(BoardOrigin), REAL(BoardOrigin + BoardHeight)), &whiteBrush);
    graphics.DrawImage(pMemBitmap, 0, 0);
    delete pMemBitmap;
    delete pMemGraphics;

}

LRESULT CALLBACK WndProc(HWND, UINT, WPARAM, LPARAM);

ULONG_PTR           gdiplusToken;
void CreateGameWindow(HINSTANCE hInstance)
{
    MSG                 msg;
    WNDCLASS            wndClass;
    GdiplusStartupInput gdiplusStartupInput;


    // Initialize GDI+.
    GdiplusStartup(&gdiplusToken, &gdiplusStartupInput, NULL);
    wndClass.style = CS_HREDRAW | CS_VREDRAW;
    wndClass.lpfnWndProc = WndProc;
    wndClass.cbClsExtra = 0;
    wndClass.cbWndExtra = 0;
    wndClass.hInstance = hInstance;
    wndClass.hIcon = LoadIcon(NULL, IDI_APPLICATION);
    wndClass.hCursor = LoadCursor(NULL, IDC_ARROW);
    wndClass.hbrBackground = (HBRUSH)GetStockObject(WHITE_BRUSH);
    wndClass.lpszMenuName = NULL;
    wndClass.lpszClassName = TEXT("Connect4");

    RegisterClass(&wndClass);

    WinRect.left = 0;
    WinRect.top = 0;
    WinRect.right = WinSize;
    WinRect.bottom = WinSize + TextAreaSize;
    AdjustWindowRect(&WinRect, WS_OVERLAPPEDWINDOW, false);
    hWnd = CreateWindow(
        TEXT("Connect4"),   // window class name
        TEXT("Connect4"),  // window caption
        WS_OVERLAPPEDWINDOW,      // window style
        CW_USEDEFAULT,            // initial x position
        CW_USEDEFAULT,            // initial y position
        WinRect.right - WinRect.left,            // initial x size
        WinRect.bottom - WinRect.top,            // initial y size
        NULL,                     // parent window handle
        NULL,                     // window menu handle
        hInstance,                // program instance handle
        NULL);                    // creation parameters

    RECT rc;
    GetWindowRect(hWnd, &rc);

    int xPos = (GetSystemMetrics(SM_CXSCREEN) - rc.right) / 2;
    int yPos = (GetSystemMetrics(SM_CYSCREEN) - rc.bottom) / 2;

    SetWindowPos(hWnd, 0, xPos, yPos, 0, 0, SWP_NOZORDER | SWP_NOSIZE);

    ShowWindow(hWnd, SW_SHOW);
    UpdateWindow(hWnd);

    while (GetMessage(&msg, NULL, 0, 0))
    {
        TranslateMessage(&msg);
        DispatchMessage(&msg);
    }

}  // WinMain

LRESULT CALLBACK WndProc(HWND hWnd, UINT message,
    WPARAM wParam, LPARAM lParam)
{
    HDC          hdc;
    PAINTSTRUCT  ps;

    switch (message)
    {
    case WM_PAINT:
        hdc = BeginPaint(hWnd, &ps);
        OnPaint(hdc);
        EndPaint(hWnd, &ps);
        return 0;
    case WM_DESTROY:
        PostQuitMessage(0);
        GdiplusShutdown(gdiplusToken);
        return 0;
    default:
        return DefWindowProc(hWnd, message, wParam, lParam);
    }
} // WndProc

void CreateGameWindow()
{
    std::thread t1([&] { CreateGameWindow(GetModuleHandle(nullptr)); });
    t1.detach();
}

