@echo off
setlocal EnableExtensions
chcp 65001 > nul

set "PYTHONUTF8=1"
set "SCRIPT=%~dp0install_prism_latest.py"

echo ============================================================
echo PrismMod Local Installer
echo Runs the installer script bundled next to this CMD file.
echo ============================================================
echo.

if not exist "%SCRIPT%" (
  echo [ERROR] install_prism_latest.py was not found next to this CMD file.
  pause
  exit /b 1
)

echo [CHECK] uv
where uv > nul 2> nul
if errorlevel 1 (
  echo [MISS]  uv is not installed.
  echo.
  echo Install uv from the official page, then run this CMD again:
  echo https://docs.astral.sh/uv/getting-started/installation/
  echo.
  echo Opening the official uv installation page...
  start "" "https://docs.astral.sh/uv/getting-started/installation/"
  pause
  exit /b 1
)

echo [OK]    uv found
echo [RUN]   local installer script
echo.
uv run --python 3.13 python "%SCRIPT%" %*
set "RESULT=%ERRORLEVEL%"

pause
exit /b %RESULT%
