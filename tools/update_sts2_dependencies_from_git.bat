@echo off
setlocal EnableExtensions

set "MODS_DIR=%~dp0"
set "SRC_ROOT=%MODS_DIR%_dependency_sources"

set "BASELIB_REPO=https://github.com/Alchyr/BaseLib-StS2.git"
set "BASELIB_BRANCH=master"
set "BASELIB_SRC=%SRC_ROOT%\BaseLib-StS2"
set "BASELIB_PROJECT=%BASELIB_SRC%\BaseLib.csproj"

set "RITSULIB_REPO=https://github.com/BAKAOLC/STS2-RitsuLib.git"
set "RITSULIB_BRANCH=main"
set "RITSULIB_SRC=%SRC_ROOT%\STS2-RitsuLib"
set "RITSULIB_PROJECT=%RITSULIB_SRC%\STS2-RitsuLib.csproj"

echo === STS2 dependency updater ===
echo Mods directory: "%MODS_DIR%"
echo Source cache:   "%SRC_ROOT%"
echo.

where git >nul 2>nul
if errorlevel 1 (
    echo [ERROR] git was not found in PATH.
    exit /b 1
)

where dotnet >nul 2>nul
if errorlevel 1 (
    echo [ERROR] dotnet was not found in PATH.
    exit /b 1
)

if not exist "%SRC_ROOT%" mkdir "%SRC_ROOT%"

call :SyncRepo "BaseLib" "%BASELIB_REPO%" "%BASELIB_BRANCH%" "%BASELIB_SRC%"
if errorlevel 1 exit /b 1

call :SyncRepo "RitsuLib" "%RITSULIB_REPO%" "%RITSULIB_BRANCH%" "%RITSULIB_SRC%"
if errorlevel 1 exit /b 1

echo.
echo [OK] BaseLib and RitsuLib source trees were updated from git.
echo [NOTE] This script intentionally does not publish RitsuLib/BaseLib.
echo [NOTE] Build those projects manually after checking their upstream build notes.
exit /b 0

:SyncRepo
set "NAME=%~1"
set "REPO=%~2"
set "BRANCH=%~3"
set "DEST=%~4"

echo.
echo === Updating %NAME% ===
if exist "%DEST%\.git" (
    pushd "%DEST%" || exit /b 1
    git -c http.sslBackend=schannel fetch --prune origin
    if errorlevel 1 (
        popd
        exit /b 1
    )
    git -c http.sslBackend=schannel checkout "%BRANCH%"
    if errorlevel 1 (
        popd
        exit /b 1
    )
    git -c http.sslBackend=schannel pull --ff-only origin "%BRANCH%"
    set "PULL_RESULT=%ERRORLEVEL%"
    popd
    exit /b %PULL_RESULT%
)

git -c http.sslBackend=schannel clone --depth 1 --branch "%BRANCH%" "%REPO%" "%DEST%"
exit /b %ERRORLEVEL%
