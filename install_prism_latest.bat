@echo off
setlocal EnableExtensions
chcp 65001 > nul

set "PYTHONUTF8=1"
set "SCRIPT_URL=https://raw.githubusercontent.com/skbyunea413-beep/STS2-BossPlayable/main/tools/install_prism_latest.py?cache=%RANDOM%%RANDOM%"
set "TEMP_SCRIPT=%TEMP%\install_prism_latest_%RANDOM%%RANDOM%.py"

echo PrismMod GitHub Installer
echo =========================

echo [CHECK]  uv               checking
where uv > nul 2> nul
if errorlevel 1 (
  echo [GET]    uv               installing from official installer
  powershell -NoProfile -ExecutionPolicy Bypass -Command "irm https://astral.sh/uv/install.ps1 | iex"
  if errorlevel 1 (
    echo [ERROR]  uv               install failed
    pause
    exit /b 1
  )
  set "PATH=%USERPROFILE%\.local\bin;%APPDATA%\uv\bin;%LOCALAPPDATA%\Programs\uv;%PATH%"
 ) else (
  echo [OK]     uv               found
)

where uv > nul 2> nul
if errorlevel 1 (
  echo [ERROR]  uv               still not found after install
  echo Close this window, open a new Command Prompt, and run this BAT again.
  pause
  exit /b 1
)

echo [GET]    Script           downloading latest installer from GitHub
powershell -NoProfile -ExecutionPolicy Bypass -Command "[Net.ServicePointManager]::SecurityProtocol=[Net.SecurityProtocolType]::Tls12; Invoke-WebRequest -Uri '%SCRIPT_URL%' -OutFile '%TEMP_SCRIPT%'"
if errorlevel 1 (
  echo [ERROR]  Script           download failed
  pause
  exit /b 1
)

echo [RUN]    Installer        starting
echo.
uv run --python 3.13 python "%TEMP_SCRIPT%" %*
set "RESULT=%ERRORLEVEL%"

del "%TEMP_SCRIPT%" > nul 2> nul
pause
exit /b %RESULT%
