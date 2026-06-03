@echo off
setlocal EnableExtensions
chcp 65001 > nul

set "PYTHONUTF8=1"
set "SCRIPT_URL=https://raw.githubusercontent.com/skbyunea413-beep/STS2-BossPlayable/main/tools/install_prism_latest.py"
set "TEMP_SCRIPT=%TEMP%\install_prism_latest_%RANDOM%%RANDOM%.py"

where uv > nul 2> nul
if errorlevel 1 (
  echo uv was not found. Installing uv from the official installer...
  powershell -NoProfile -ExecutionPolicy Bypass -Command "irm https://astral.sh/uv/install.ps1 | iex"
  if errorlevel 1 (
    echo uv install failed.
    pause
    exit /b 1
  )
  set "PATH=%USERPROFILE%\.local\bin;%APPDATA%\uv\bin;%LOCALAPPDATA%\Programs\uv;%PATH%"
)

where uv > nul 2> nul
if errorlevel 1 (
  echo uv still was not found after install.
  echo Close this window, open a new Command Prompt, and run this BAT again.
  pause
  exit /b 1
)

echo Downloading latest PrismMod installer script...
powershell -NoProfile -ExecutionPolicy Bypass -Command "[Net.ServicePointManager]::SecurityProtocol=[Net.SecurityProtocolType]::Tls12; Invoke-WebRequest -Uri '%SCRIPT_URL%' -OutFile '%TEMP_SCRIPT%'"
if errorlevel 1 (
  echo Failed to download installer script from GitHub.
  pause
  exit /b 1
)

uv run --python 3.13 python "%TEMP_SCRIPT%" %*
set "RESULT=%ERRORLEVEL%"

del "%TEMP_SCRIPT%" > nul 2> nul
pause
exit /b %RESULT%
