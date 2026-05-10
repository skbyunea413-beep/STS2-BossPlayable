@echo off
setlocal

set "SCRIPT_DIR=%~dp0"
powershell -NoProfile -ExecutionPolicy Bypass -File "%SCRIPT_DIR%install_stable_sts2_dependencies.ps1" %*
exit /b %ERRORLEVEL%
