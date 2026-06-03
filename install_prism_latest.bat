@echo off
setlocal EnableExtensions
chcp 65001 > nul

set "PYTHONUTF8=1"
set "SCRIPT_API=https://api.github.com/repos/skbyunea413-beep/STS2-BossPlayable/contents/tools/install_prism_latest.py?ref=main"
set "TEMP_SCRIPT=%TEMP%\install_prism_latest_%RANDOM%%RANDOM%.py"

echo ============================================================
echo PrismMod Installer Bootstrap
echo uv and the latest installer script are checked first.
echo ============================================================
echo.

echo -- Bootstrap ------------------------------------------------
echo [CHECK]  uv              checking
where uv > nul 2> nul
if errorlevel 1 (
  echo [GET]    uv              installing from official installer
  powershell -NoProfile -ExecutionPolicy Bypass -Command "irm https://astral.sh/uv/install.ps1 | iex"
  if errorlevel 1 (
    echo [ERROR]  uv              install failed
    pause
    exit /b 1
  )
  set "PATH=%USERPROFILE%\.local\bin;%APPDATA%\uv\bin;%LOCALAPPDATA%\Programs\uv;%PATH%"
 ) else (
  echo [OK]     uv              found
)

where uv > nul 2> nul
if errorlevel 1 (
  echo [ERROR]  uv              still not found after install
  echo Close this window, open a new Command Prompt, and run this BAT again.
  pause
  exit /b 1
)

echo [GET]    Script          downloading latest installer from GitHub
powershell -NoProfile -ExecutionPolicy Bypass -Command "$ProgressPreference='SilentlyContinue'; [Net.ServicePointManager]::SecurityProtocol=[Net.SecurityProtocolType]::Tls12; $r=Invoke-RestMethod -Headers @{ 'User-Agent'='PrismMod-installer-bat' } -Uri '%SCRIPT_API%'; $b64=($r.content -replace '\s',''); [IO.File]::WriteAllBytes('%TEMP_SCRIPT%', [Convert]::FromBase64String($b64))"
if errorlevel 1 (
  echo [ERROR]  Script          download failed
  pause
  exit /b 1
)

echo [RUN]    Installer       starting
echo.
uv run --python 3.13 python "%TEMP_SCRIPT%" %*
set "RESULT=%ERRORLEVEL%"

del "%TEMP_SCRIPT%" > nul 2> nul
pause
exit /b %RESULT%
