echo off
set /p mytextfile=< build.txt
set /A COUNTER=%mytextfile%+1
echo %COUNTER% > build.txt
pause
