set CURRENT_DIR=%CD%

mkdir build > nul
cd build
cmake -G "Visual Studio 14" ..\
cd "%CURRENT_DIR%"

