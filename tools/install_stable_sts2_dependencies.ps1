param(
    [string]$ModsDir = "",
    [string]$PrismProject = "",
    [string]$BaseLibVersion = "3.1.2",
    [string]$RitsuLibVersion = "0.2.28",
    [switch]$SkipPrismRebuild
)

$ErrorActionPreference = "Stop"
$ProgressPreference = "SilentlyContinue"

function Get-ModRoot {
    $candidate = Split-Path -Parent $PSScriptRoot
    return $candidate
}

function Get-PackageVersion {
    param([string]$ProjectPath, [string]$PackageName)

    [xml]$project = Get-Content -LiteralPath $ProjectPath -Raw
    $reference = $project.Project.ItemGroup.PackageReference |
        Where-Object { $_.Include -eq $PackageName } |
        Select-Object -First 1

    if (-not $reference -or -not $reference.Version) {
        throw "Could not read PackageReference '$PackageName' from $ProjectPath."
    }

    return [string]$reference.Version
}

function Get-ReleaseForVersion {
    param([string]$Repo, [string]$Version)

    $tags = @("v$Version", $Version)
    foreach ($tag in $tags) {
        try {
            return Invoke-RestMethod -Uri "https://api.github.com/repos/$Repo/releases/tags/$tag" -Headers @{
                "User-Agent" = "STS2-PrismMod-dependency-installer"
            }
        }
        catch {
            if ($tag -eq $tags[-1]) {
                throw "No release tag for $Repo version $Version was found."
            }
        }
    }
}

function Get-Asset {
    param($Release, [string]$Pattern)

    $asset = $Release.assets | Where-Object { $_.name -like $Pattern } | Select-Object -First 1
    if (-not $asset) {
        throw "No release asset matching '$Pattern' found in $($Release.html_url)."
    }

    return $asset
}

function Get-RegistrySts2Path {
    $keys = @(
        "HKLM:\SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall\Steam App 2868840",
        "HKLM:\SOFTWARE\WOW6432Node\Microsoft\Windows\CurrentVersion\Uninstall\Steam App 2868840"
    )

    foreach ($key in $keys) {
        if (Test-Path -LiteralPath $key) {
            $value = (Get-ItemProperty -LiteralPath $key -Name InstallLocation -ErrorAction SilentlyContinue).InstallLocation
            if ($value -and (Test-Path -LiteralPath $value)) {
                return $value
            }
        }
    }

    return $null
}

function Get-SteamPath {
    $steamPath = (Get-ItemProperty -LiteralPath "HKCU:\Software\Valve\Steam" -Name SteamPath -ErrorAction SilentlyContinue).SteamPath
    if ($steamPath) {
        return $steamPath
    }

    return "C:\Program Files (x86)\Steam"
}

function Resolve-ModsDir {
    param([string]$RequestedModsDir, [string]$ModRoot)

    if ($RequestedModsDir) {
        return (Resolve-Path -LiteralPath $RequestedModsDir).Path
    }

    if ($env:STS2_MODS_DIR) {
        return (Resolve-Path -LiteralPath $env:STS2_MODS_DIR).Path
    }

    $parent = Split-Path -Parent $ModRoot
    if ((Split-Path -Leaf $parent) -ieq "mods") {
        return (Resolve-Path -LiteralPath $parent).Path
    }

    $gamePath = Get-RegistrySts2Path
    if (-not $gamePath) {
        $steamPath = Get-SteamPath
        $candidates = @(
            (Join-Path $steamPath "steamapps\common\Slay the Spire 2"),
            "Y:\SteamLibrary\steamapps\common\Slay the Spire 2",
            "C:\Program Files (x86)\Steam\steamapps\common\Slay the Spire 2"
        )

        $gamePath = $candidates | Where-Object {
            Test-Path -LiteralPath (Join-Path $_ "data_sts2_windows_x86_64")
        } | Select-Object -First 1
    }

    if (-not $gamePath) {
        throw "Could not find Slay the Spire 2. Pass -ModsDir `"path\to\Slay the Spire 2\mods`" or set STS2_MODS_DIR."
    }

    $resolvedModsDir = Join-Path $gamePath "mods"
    if (-not (Test-Path -LiteralPath $resolvedModsDir)) {
        New-Item -ItemType Directory -Path $resolvedModsDir | Out-Null
    }

    return (Resolve-Path -LiteralPath $resolvedModsDir).Path
}

function Backup-ExistingDependency {
    param([string]$ModsDir, [string]$Pattern, [string]$BackupDir)

    Get-ChildItem -LiteralPath $ModsDir -Force |
        Where-Object { $_.PSIsContainer -and $_.Name -like $Pattern } |
        ForEach-Object {
            $dest = Join-Path $BackupDir $_.Name
            Write-Host "Backing up $($_.Name) -> $dest"
            Move-Item -LiteralPath $_.FullName -Destination $dest
        }
}

function Install-ZipToFolder {
    param($Asset, [string]$TargetDir, [string]$DownloadDir)

    $zipPath = Join-Path $DownloadDir $Asset.name
    Write-Host "Downloading $($Asset.name)"
    Invoke-WebRequest -Uri $Asset.browser_download_url -OutFile $zipPath

    if (Test-Path -LiteralPath $TargetDir) {
        Remove-Item -LiteralPath $TargetDir -Recurse -Force
    }

    New-Item -ItemType Directory -Path $TargetDir | Out-Null
    Expand-Archive -Path $zipPath -DestinationPath $TargetDir -Force
}

function Set-PackageVersion {
    param([string]$ProjectPath, [string]$PackageName, [string]$Version)

    $text = Get-Content -LiteralPath $ProjectPath -Raw
    $pattern = "(<PackageReference\s+Include=`"$([regex]::Escape($PackageName))`"\s+Version=`")[^`"]+(`")"
    $updated = [regex]::Replace($text, $pattern, "`${1}$Version`${2}")
    if ($updated -eq $text) {
        throw "Could not update PackageReference '$PackageName' in $ProjectPath."
    }

    Set-Content -LiteralPath $ProjectPath -Value $updated -NoNewline
}

$modRoot = Get-ModRoot
if (-not $PrismProject) {
    $PrismProject = Join-Path $modRoot "src\PrismMod.csproj"
}

$ModsDir = Resolve-ModsDir $ModsDir $modRoot
$stamp = Get-Date -Format "yyyyMMdd_HHmmss"
$workDir = Join-Path $env:TEMP "sts2_dependency_install_$stamp"
$backupDir = Join-Path $ModsDir "_dependency_backups\$stamp"

New-Item -ItemType Directory -Path $workDir | Out-Null
New-Item -ItemType Directory -Path $backupDir | Out-Null

Write-Host "Mods directory: $ModsDir"
Write-Host "Backup directory: $backupDir"

if (Test-Path -LiteralPath $PrismProject) {
    $baseVersion = Get-PackageVersion $PrismProject "Alchyr.Sts2.BaseLib"
    $ritsuVersion = Get-PackageVersion $PrismProject "STS2.RitsuLib"
}
else {
    Write-Host "PrismMod project not found, using bundled stable dependency versions."
    $baseVersion = $BaseLibVersion
    $ritsuVersion = $RitsuLibVersion
}

$baseRelease = Get-ReleaseForVersion "Alchyr/BaseLib-StS2" $baseVersion
$ritsuRelease = Get-ReleaseForVersion "BAKAOLC/STS2-RitsuLib" $ritsuVersion

$baseAsset = Get-Asset $baseRelease "BaseLib.*.zip"
$ritsuAsset = Get-Asset $ritsuRelease "STS2-RitsuLib.*.variant-pack.zip"

Write-Host "BaseLib: $baseVersion"
Write-Host "RitsuLib: $ritsuVersion"

Backup-ExistingDependency $ModsDir "BaseLib*" $backupDir
Backup-ExistingDependency $ModsDir "STS2-RitsuLib*" $backupDir

$baseTarget = Join-Path $ModsDir ([IO.Path]::GetFileNameWithoutExtension($baseAsset.name))
$ritsuTarget = Join-Path $ModsDir ([IO.Path]::GetFileNameWithoutExtension($ritsuAsset.name))

Install-ZipToFolder $baseAsset $baseTarget $workDir
Install-ZipToFolder $ritsuAsset $ritsuTarget $workDir

if (-not $SkipPrismRebuild -and (Test-Path -LiteralPath $PrismProject)) {
    Write-Host "Publishing PrismMod"
    dotnet publish $PrismProject
    if ($LASTEXITCODE -ne 0) {
        throw "PrismMod publish failed."
    }
}

Remove-Item -LiteralPath $workDir -Recurse -Force -ErrorAction SilentlyContinue
Write-Host "Done."
