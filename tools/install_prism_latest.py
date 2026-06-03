#!/usr/bin/env python3
# -*- coding: utf-8 -*-
from __future__ import annotations

import argparse
import ctypes
import datetime as dt
import json
import os
import shutil
import subprocess
import sys
import tempfile
import urllib.request
import zipfile
from pathlib import Path
from typing import Any


PRISM_REPO = "skbyunea413-beep/STS2-BossPlayable"
BASELIB_REPO = "Alchyr/BaseLib-StS2"
RITSULIB_REPO = "BAKAOLC/STS2-RitsuLib"
APP_ID = "2868840"
GAME_FOLDER = "Slay the Spire 2"
USER_AGENT = "PrismMod-uv-bat-installer"


def setup_console() -> None:
    if hasattr(sys.stdout, "reconfigure"):
        sys.stdout.reconfigure(encoding="utf-8", errors="replace")
    if hasattr(sys.stderr, "reconfigure"):
        sys.stderr.reconfigure(encoding="utf-8", errors="replace")
    if os.name == "nt":
        os.system("chcp 65001 > nul")


def log(message: str) -> None:
    print(message, flush=True)


def fail(message: str) -> None:
    raise RuntimeError(message)


def request_json(url: str) -> dict[str, Any]:
    if os.name == "nt":
        with tempfile.NamedTemporaryFile(delete=False, suffix=".json") as temp:
            temp_path = Path(temp.name)
        try:
            powershell_download(url, temp_path)
            return json.loads(temp_path.read_text(encoding="utf-8"))
        finally:
            temp_path.unlink(missing_ok=True)

    req = urllib.request.Request(url, headers={"User-Agent": USER_AGENT})
    with urllib.request.urlopen(req, timeout=30) as response:
        return json.loads(response.read().decode("utf-8"))


def latest_release(repo: str) -> dict[str, Any]:
    return request_json(f"https://api.github.com/repos/{repo}/releases/latest")


def find_asset(release: dict[str, Any], contains: tuple[str, ...], suffix: str = ".zip") -> dict[str, Any]:
    for asset in release.get("assets") or []:
        name = str(asset.get("name", ""))
        lowered = name.lower()
        if lowered.endswith(suffix) and all(part.lower() in lowered for part in contains):
            return asset
    fail(f"릴리스 파일을 찾지 못했습니다: {contains} / {release.get('html_url', '(unknown release)')}")


def download(url: str, dest: Path) -> None:
    if os.name == "nt":
        powershell_download(url, dest)
        return

    req = urllib.request.Request(url, headers={"User-Agent": USER_AGENT})
    with urllib.request.urlopen(req, timeout=180) as response, dest.open("wb") as output:
        shutil.copyfileobj(response, output)


def powershell_download(url: str, dest: Path) -> None:
    url_literal = json.dumps(url)
    dest_literal = json.dumps(str(dest))
    agent_literal = json.dumps(USER_AGENT)
    script = (
        "$ProgressPreference='SilentlyContinue'; "
        "[Net.ServicePointManager]::SecurityProtocol=[Net.SecurityProtocolType]::Tls12; "
        f"Invoke-WebRequest -UseBasicParsing -Headers @{{ 'User-Agent' = {agent_literal} }} "
        f"-Uri {url_literal} -OutFile {dest_literal}"
    )
    result = subprocess.run(
        ["powershell", "-NoProfile", "-ExecutionPolicy", "Bypass", "-Command", script],
        text=True,
        capture_output=True,
    )
    if result.returncode != 0:
        detail = (result.stderr or result.stdout).strip()
        fail(f"다운로드 실패: {url}\n{detail}")


def unique_paths(paths: list[Path]) -> list[Path]:
    seen: set[str] = set()
    result: list[Path] = []
    for path in paths:
        key = str(path).lower()
        if key not in seen:
            seen.add(key)
            result.append(path)
    return result


def read_registry_value(root: int, subkey: str, value: str) -> str | None:
    if os.name != "nt":
        return None
    import winreg

    try:
        with winreg.OpenKey(root, subkey) as key:
            data, _ = winreg.QueryValueEx(key, value)
            return str(data).replace("/", "\\")
    except OSError:
        return None


def installed_game_path() -> Path | None:
    if os.name != "nt":
        return None
    import winreg

    keys = (
        (winreg.HKEY_LOCAL_MACHINE, rf"SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall\Steam App {APP_ID}"),
        (winreg.HKEY_LOCAL_MACHINE, rf"SOFTWARE\WOW6432Node\Microsoft\Windows\CurrentVersion\Uninstall\Steam App {APP_ID}"),
    )
    for root, key in keys:
        value = read_registry_value(root, key, "InstallLocation")
        if value and Path(value).exists():
            return Path(value)
    return None


def steam_paths() -> list[Path]:
    paths: list[Path] = []
    if os.name == "nt":
        import winreg

        steam = read_registry_value(winreg.HKEY_CURRENT_USER, r"Software\Valve\Steam", "SteamPath")
        if steam:
            paths.append(Path(steam))
    paths.append(Path(r"C:\Program Files (x86)\Steam"))
    return unique_paths(paths)


def steam_library_paths() -> list[Path]:
    libraries: list[Path] = []
    for steam in steam_paths():
        libraries.append(steam)
        vdf = steam / "steamapps" / "libraryfolders.vdf"
        if not vdf.exists():
            continue
        text = vdf.read_text(encoding="utf-8", errors="ignore")
        for line in text.splitlines():
            stripped = line.strip()
            if not stripped.startswith('"path"'):
                continue
            parts = stripped.split('"')
            if len(parts) >= 4:
                libraries.append(Path(parts[3].replace("\\\\", "\\")))
    libraries.append(Path(r"Y:\SteamLibrary"))
    return unique_paths(libraries)


def find_game_path() -> Path:
    registry_path = installed_game_path()
    if registry_path:
        return registry_path
    for library in steam_library_paths():
        candidate = library / "steamapps" / "common" / GAME_FOLDER
        if (candidate / "mods").exists() or (candidate / "data_sts2_windows_x86_64").exists():
            return candidate
    fail("Slay the Spire 2 설치 경로를 찾지 못했습니다. --mods-dir 로 mods 폴더를 지정해 주세요.")


def resolve_mods_dir(requested: str | None) -> Path:
    if requested:
        mods_dir = Path(requested).expanduser().resolve()
    elif os.environ.get("STS2_MODS_DIR"):
        mods_dir = Path(os.environ["STS2_MODS_DIR"]).expanduser().resolve()
    else:
        mods_dir = find_game_path() / "mods"
    mods_dir.mkdir(parents=True, exist_ok=True)
    return mods_dir


def resolve_backup_root(mods_dir: Path) -> Path:
    stamp = dt.datetime.now().strftime("%Y%m%d_%H%M%S")
    if os.name == "nt" and os.environ.get("LOCALAPPDATA"):
        root = Path(os.environ["LOCALAPPDATA"]) / "PrismModInstaller" / "backups"
    else:
        root = mods_dir.parent / "PrismModInstallerBackups"
    backup_root = root / stamp
    backup_root.mkdir(parents=True, exist_ok=True)
    return backup_root


def backup_path(backup_root: Path, source: Path) -> Path:
    target = backup_root / source.name
    index = 2
    while target.exists():
        target = backup_root / f"{source.name}.{index}"
        index += 1
    return target


def backup_matching(mods_dir: Path, backup_root: Path, prefixes: tuple[str, ...]) -> None:
    for entry in mods_dir.iterdir():
        if not entry.is_dir():
            continue
        lower = entry.name.lower()
        if any(lower.startswith(prefix.lower()) for prefix in prefixes):
            dest = backup_path(backup_root, entry)
            log(f"백업: {entry.name} -> {dest}")
            shutil.move(str(entry), str(dest))


def install_zip(zip_path: Path, mods_dir: Path, folder_name: str | None = None) -> Path:
    temp_extract = Path(tempfile.mkdtemp(prefix="prism_extract_"))
    try:
        with zipfile.ZipFile(zip_path) as archive:
            archive.extractall(temp_extract)
        children = [path for path in temp_extract.iterdir() if path.name != "__MACOSX"]
        source = children[0] if len(children) == 1 and children[0].is_dir() else temp_extract
        target = mods_dir / (folder_name or source.name)
        if target.exists():
            shutil.rmtree(target)
        shutil.copytree(source, target)
        return target
    finally:
        shutil.rmtree(temp_extract, ignore_errors=True)


def install_all(mods_dir: Path, backup_root: Path, force_prism: bool, dry_run: bool) -> None:
    log("GitHub 최신 릴리스 확인 중...")
    prism_release = latest_release(PRISM_REPO)
    base_release = latest_release(BASELIB_REPO)
    ritsu_release = latest_release(RITSULIB_REPO)

    prism_asset = find_asset(prism_release, ("prismmod",))
    base_asset = find_asset(base_release, ("baselib",))
    ritsu_asset = find_asset(ritsu_release, ("sts2-ritsulib", "variant-pack"))

    log(f"PrismMod: {prism_release.get('tag_name', '(unknown)')}")
    log(f"BaseLib: {base_release.get('tag_name', '(unknown)')}")
    log(f"RitsuLib: {ritsu_release.get('tag_name', '(unknown)')}")
    log(f"PrismMod ZIP: {prism_asset.get('name')}")
    log(f"BaseLib ZIP: {base_asset.get('name')}")
    log(f"RitsuLib ZIP: {ritsu_asset.get('name')}")

    if dry_run:
        log("dry-run: 다운로드와 설치를 건너뜁니다.")
        return

    with tempfile.TemporaryDirectory(prefix="prism_download_") as temp:
        temp_dir = Path(temp)
        downloads = (
            (prism_asset, temp_dir / prism_asset["name"]),
            (base_asset, temp_dir / base_asset["name"]),
            (ritsu_asset, temp_dir / ritsu_asset["name"]),
        )
        for asset, path in downloads:
            log(f"다운로드: {asset['name']}")
            download(asset["browser_download_url"], path)

        backup_matching(mods_dir, backup_root, ("BaseLib", "STS2-RitsuLib"))

        prism_target = mods_dir / "PrismMod"
        if prism_target.exists() and force_prism:
            dest = backup_path(backup_root, prism_target)
            log(f"백업: PrismMod -> {dest}")
            shutil.move(str(prism_target), str(dest))
        elif prism_target.exists():
            log("PrismMod: 기존 설치를 최신 릴리스로 교체합니다.")
            dest = backup_path(backup_root, prism_target)
            log(f"백업: PrismMod -> {dest}")
            shutil.move(str(prism_target), str(dest))

        log(f"설치 완료: {install_zip(downloads[0][1], mods_dir, 'PrismMod')}")
        log(f"설치 완료: {install_zip(downloads[1][1], mods_dir)}")
        log(f"설치 완료: {install_zip(downloads[2][1], mods_dir)}")


def parse_args() -> argparse.Namespace:
    parser = argparse.ArgumentParser(description="Install latest PrismMod and STS2 dependencies.")
    parser.add_argument("--mods-dir", help="Slay the Spire 2 mods 폴더를 직접 지정합니다.")
    parser.add_argument("--force-prism", action="store_true", help="PrismMod를 백업 후 다시 설치합니다.")
    parser.add_argument("--dry-run", action="store_true", help="최신 릴리스 확인만 하고 설치하지 않습니다.")
    return parser.parse_args()


def is_admin() -> bool:
    if os.name != "nt":
        return True
    try:
        return bool(ctypes.windll.shell32.IsUserAnAdmin())
    except Exception:
        return False


def main() -> int:
    setup_console()
    args = parse_args()

    log("PrismMod GitHub 최신 설치기")
    log("===========================")
    if not is_admin():
        log("참고: 관리자 권한이 아니어도 Steam 라이브러리 쓰기 권한이 있으면 설치됩니다.")

    mods_dir = resolve_mods_dir(args.mods_dir)
    backup_root = resolve_backup_root(mods_dir)
    log(f"mods 폴더: {mods_dir}")
    log(f"백업 폴더: {backup_root}")
    log("주의: 백업은 모드 인식을 피하기 위해 mods 폴더 밖에 저장합니다.")

    install_all(mods_dir, backup_root, args.force_prism, args.dry_run)

    log("")
    log("완료. 게임을 완전히 종료한 뒤 다시 실행하고 모드 목록에서 Prism Shirou를 켜세요.")
    return 0


if __name__ == "__main__":
    try:
        raise SystemExit(main())
    except Exception as exc:
        setup_console()
        log("")
        log(f"오류: {exc}")
        raise SystemExit(1)
