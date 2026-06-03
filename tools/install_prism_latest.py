#!/usr/bin/env python3
# -*- coding: utf-8 -*-
from __future__ import annotations

import argparse
import ctypes
import datetime as dt
import hashlib
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


def ko(text: str) -> str:
    return text.encode("ascii").decode("unicode_escape")


def setup_console() -> None:
    if hasattr(sys.stdout, "reconfigure"):
        sys.stdout.reconfigure(encoding="utf-8", errors="replace")
    if hasattr(sys.stderr, "reconfigure"):
        sys.stderr.reconfigure(encoding="utf-8", errors="replace")
    if os.name == "nt":
        os.system("chcp 65001 > nul")


def log(message: str = "") -> None:
    print(message, flush=True)


def row(state: str, name: str, detail: str = "") -> None:
    if detail:
        log(f"{state:<8} {name:<16} {detail}")
    else:
        log(f"{state:<8} {name}")


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
    fail(ko("\\ub9b4\\ub9ac\\uc2a4 \\ud30c\\uc77c\\uc744 \\ucc3e\\uc9c0 \\ubabb\\ud588\\uc2b5\\ub2c8\\ub2e4: ") + f"{contains}")


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
        fail(ko("\\ub2e4\\uc6b4\\ub85c\\ub4dc \\uc2e4\\ud328: ") + f"{url}\n{detail}")


def download(url: str, dest: Path) -> None:
    if os.name == "nt":
        powershell_download(url, dest)
        return
    req = urllib.request.Request(url, headers={"User-Agent": USER_AGENT})
    with urllib.request.urlopen(req, timeout=180) as response, dest.open("wb") as output:
        shutil.copyfileobj(response, output)


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
        row("[OK]", "Game", ko("\\ub808\\uc9c0\\uc2a4\\ud2b8\\ub9ac\\uc5d0\\uc11c \\ucc3e\\uc74c: ") + str(registry_path))
        return registry_path
    for library in steam_library_paths():
        candidate = library / "steamapps" / "common" / GAME_FOLDER
        if (candidate / "mods").exists() or (candidate / "data_sts2_windows_x86_64").exists():
            row("[OK]", "Game", ko("Steam \\ub77c\\uc774\\ube0c\\ub7ec\\ub9ac\\uc5d0\\uc11c \\ucc3e\\uc74c: ") + str(candidate))
            return candidate
    fail(ko("Slay the Spire 2 \\uc124\\uce58 \\uacbd\\ub85c\\ub97c \\ucc3e\\uc9c0 \\ubabb\\ud588\\uc2b5\\ub2c8\\ub2e4. --mods-dir \\ub85c mods \\ud3f4\\ub354\\ub97c \\uc9c0\\uc815\\ud574 \\uc8fc\\uc138\\uc694."))


def resolve_mods_dir(requested: str | None) -> Path:
    if requested:
        mods_dir = Path(requested).expanduser().resolve()
        row("[OK]", "Mods", ko("\\uc9c0\\uc815\\ub41c \\uacbd\\ub85c: ") + str(mods_dir))
    elif os.environ.get("STS2_MODS_DIR"):
        mods_dir = Path(os.environ["STS2_MODS_DIR"]).expanduser().resolve()
        row("[OK]", "Mods", ko("STS2_MODS_DIR: ") + str(mods_dir))
    else:
        mods_dir = find_game_path() / "mods"
        row("[OK]", "Mods", ko("\\uc790\\ub3d9 \\ud0d0\\uc0c9: ") + str(mods_dir))
    mods_dir.mkdir(parents=True, exist_ok=True)
    return mods_dir


def local_root() -> Path:
    if os.name == "nt" and os.environ.get("LOCALAPPDATA"):
        return Path(os.environ["LOCALAPPDATA"]) / "PrismModInstaller"
    return Path.home() / ".prism-mod-installer"


def resolve_backup_root(create: bool) -> Path:
    stamp = dt.datetime.now().strftime("%Y%m%d_%H%M%S")
    backup_root = local_root() / "backups" / stamp
    if create:
        backup_root.mkdir(parents=True, exist_ok=True)
    return backup_root


def state_path(mods_dir: Path) -> Path:
    key = hashlib.sha256(str(mods_dir).lower().encode("utf-8")).hexdigest()[:16]
    root = local_root() / "state"
    root.mkdir(parents=True, exist_ok=True)
    return root / f"{key}.json"


def load_state(mods_dir: Path) -> dict[str, Any]:
    path = state_path(mods_dir)
    try:
        return json.loads(path.read_text(encoding="utf-8"))
    except (OSError, json.JSONDecodeError):
        return {}


def save_state(mods_dir: Path, state: dict[str, Any]) -> None:
    path = state_path(mods_dir)
    path.write_text(json.dumps(state, indent=2, ensure_ascii=False), encoding="utf-8")


def backup_path(backup_root: Path, source: Path) -> Path:
    target = backup_root / source.name
    index = 2
    while target.exists():
        target = backup_root / f"{source.name}.{index}"
        index += 1
    return target


def read_manifest(folder: Path) -> dict[str, Any] | None:
    candidates = list(folder.glob("*.json"))
    manifest = folder / "mod_manifest.json"
    if manifest.exists():
        candidates.insert(0, manifest)
    for path in candidates:
        try:
            data = json.loads(path.read_text(encoding="utf-8-sig"))
        except (OSError, json.JSONDecodeError, UnicodeDecodeError):
            continue
        if isinstance(data.get("id"), str):
            return data
    return None


def find_installed_mods(mods_dir: Path, mod_id: str, prefixes: tuple[str, ...]) -> list[Path]:
    found: list[Path] = []
    for entry in mods_dir.iterdir():
        if not entry.is_dir():
            continue
        lower = entry.name.lower()
        manifest = read_manifest(entry)
        if any(lower.startswith(prefix.lower()) for prefix in prefixes) or (manifest and manifest.get("id") == mod_id):
            found.append(entry)
    return found


def installed_version(folder: Path) -> str | None:
    manifest = read_manifest(folder)
    if not manifest:
        return None
    value = manifest.get("version")
    return str(value).lstrip("v") if value else None


def backup_folder(source: Path, backup_root: Path) -> None:
    backup_root.mkdir(parents=True, exist_ok=True)
    dest = backup_path(backup_root, source)
    row("[BACKUP]", source.name, str(dest))
    shutil.move(str(source), str(dest))


def cleanup_stray_extracts(mods_dir: Path, backup_root: Path, dry_run: bool) -> None:
    strays = [p for p in mods_dir.iterdir() if p.is_dir() and p.name.startswith("prism_extract_")]
    if not strays:
        row("[OK]", "Cleanup", ko("\\uc784\\uc2dc \\ud3f4\\ub354 \\uc5c6\\uc74c"))
        return
    for folder in strays:
        if dry_run:
            row("[DRY]", "Cleanup", ko("\\uc774\\ub3d9 \\uc608\\uc815: ") + folder.name)
        else:
            backup_folder(folder, backup_root)


def install_zip(zip_path: Path, mods_dir: Path, folder_name: str) -> Path:
    temp_extract = Path(tempfile.mkdtemp(prefix="prism_extract_"))
    try:
        with zipfile.ZipFile(zip_path) as archive:
            archive.extractall(temp_extract)
        children = [path for path in temp_extract.iterdir() if path.name != "__MACOSX"]
        source = children[0] if len(children) == 1 and children[0].is_dir() else temp_extract
        target = mods_dir / folder_name
        if target.exists():
            shutil.rmtree(target)
        shutil.copytree(source, target)
        return target
    finally:
        shutil.rmtree(temp_extract, ignore_errors=True)


def plan_component(
    label: str,
    installed: list[Path],
    installed_ver: str | None,
    latest_ver: str,
    force: bool = False,
) -> bool:
    if not installed:
        row("[MISSING]", label, ko("\\uc124\\uce58\\ub428 \\uc5c6\\uc74c -> \\uc124\\uce58"))
        return True
    names = ", ".join(path.name for path in installed)
    if force:
        row("[UPDATE]", label, ko("\\uac15\\uc81c \\uc7ac\\uc124\\uce58: ") + names)
        return True
    if installed_ver and installed_ver == latest_ver:
        row("[SKIP]", label, f"{installed_ver} ({names})")
        return False
    current = installed_ver or ko("\\ubc84\\uc804 \\ud655\\uc778 \\ubd88\\uac00")
    row("[UPDATE]", label, f"{current} -> {latest_ver} ({names})")
    return True


def install_all(mods_dir: Path, backup_root: Path, force_prism: bool, dry_run: bool) -> None:
    row("[CHECK]", "GitHub", ko("\\ucd5c\\uc2e0 \\ub9b4\\ub9ac\\uc2a4 \\ud655\\uc778 \\uc911"))
    prism_release = latest_release(PRISM_REPO)
    base_release = latest_release(BASELIB_REPO)
    ritsu_release = latest_release(RITSULIB_REPO)

    prism_asset = find_asset(prism_release, ("prismmod",))
    base_asset = find_asset(base_release, ("baselib",))
    ritsu_asset = find_asset(ritsu_release, ("sts2-ritsulib", "variant-pack"))

    prism_tag = str(prism_release.get("tag_name", "") or "latest")
    base_version = str(base_release.get("tag_name", "")).lstrip("v") or "latest"
    ritsu_version = str(ritsu_release.get("tag_name", "")).lstrip("v") or "latest"

    row("[OK]", "PrismMod", f"{prism_tag} / {prism_asset.get('name')}")
    row("[OK]", "BaseLib", f"{base_version} / {base_asset.get('name')}")
    row("[OK]", "RitsuLib", f"{ritsu_version} / {ritsu_asset.get('name')}")

    state = load_state(mods_dir)
    prism_dirs = find_installed_mods(mods_dir, "PrismMod", ("PrismMod",))
    base_dirs = find_installed_mods(mods_dir, "BaseLib", ("BaseLib",))
    ritsu_dirs = find_installed_mods(mods_dir, "STS2-RitsuLib", ("STS2-RitsuLib",))

    row("[CHECK]", "Installed", ko("\\ud604\\uc7ac \\ud30c\\uc77c \\ud655\\uc778"))
    prism_needs_install = force_prism or not prism_dirs or state.get("prism_release") != prism_tag
    if prism_dirs and not prism_needs_install:
        row("[SKIP]", "PrismMod", f"{prism_tag} ({', '.join(p.name for p in prism_dirs)})")
    elif prism_dirs:
        old = state.get("prism_release") or ko("\\uc774\\uc804 \\uc0c1\\ud0dc \\uc5c6\\uc74c")
        row("[UPDATE]", "PrismMod", f"{old} -> {prism_tag} ({', '.join(p.name for p in prism_dirs)})")
    else:
        row("[MISSING]", "PrismMod", ko("\\uc124\\uce58\\ub428 \\uc5c6\\uc74c -> \\uc124\\uce58"))

    base_needs_install = plan_component("BaseLib", base_dirs, installed_version(base_dirs[0]) if base_dirs else None, base_version)
    ritsu_needs_install = plan_component("RitsuLib", ritsu_dirs, installed_version(ritsu_dirs[0]) if ritsu_dirs else None, ritsu_version)

    cleanup_stray_extracts(mods_dir, backup_root, dry_run)

    if dry_run:
        row("[DRY]", "Install", ko("\\ub2e4\\uc6b4\\ub85c\\ub4dc\\uc640 \\uad50\\uccb4\\ub97c \\uac74\\ub108\\ub701\\ub2c8\\ub2e4"))
        return

    tasks: list[tuple[str, dict[str, Any], Path, str, list[Path]]] = []
    with tempfile.TemporaryDirectory(prefix="prism_download_") as temp:
        temp_dir = Path(temp)
        if prism_needs_install:
            tasks.append(("PrismMod", prism_asset, temp_dir / prism_asset["name"], "PrismMod", prism_dirs))
        if base_needs_install:
            tasks.append(("BaseLib", base_asset, temp_dir / base_asset["name"], f"BaseLib.{base_version}", base_dirs))
        if ritsu_needs_install:
            tasks.append(("RitsuLib", ritsu_asset, temp_dir / ritsu_asset["name"], "STS2-RitsuLib", ritsu_dirs))

        if not tasks:
            row("[DONE]", "Install", ko("\\uc774\\ubbf8 \\ubaa8\\ub450 \\ucd5c\\uc2e0\\uc785\\ub2c8\\ub2e4"))
            return

        for label, asset, path, _, _ in tasks:
            row("[GET]", label, str(asset.get("name")))
            download(asset["browser_download_url"], path)

        for label, _, path, folder_name, installed_dirs in tasks:
            for folder in installed_dirs:
                if folder.exists():
                    backup_folder(folder, backup_root)
            installed = install_zip(path, mods_dir, folder_name)
            row("[OK]", label, ko("\\uc124\\uce58 \\uc644\\ub8cc: ") + str(installed))

    state.update(
        {
            "mods_dir": str(mods_dir),
            "prism_release": prism_tag,
            "baselib_version": base_version,
            "ritsulib_version": ritsu_version,
            "updated_at": dt.datetime.now().isoformat(timespec="seconds"),
        }
    )
    save_state(mods_dir, state)
    row("[OK]", "State", str(state_path(mods_dir)))


def parse_args() -> argparse.Namespace:
    parser = argparse.ArgumentParser(description="Install latest PrismMod and STS2 dependencies.")
    parser.add_argument("--mods-dir", help="Slay the Spire 2 mods folder.")
    parser.add_argument("--force-prism", action="store_true", help="Reinstall PrismMod even if the latest release is recorded.")
    parser.add_argument("--dry-run", action="store_true", help="Only check status. Do not download or install.")
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

    log("PrismMod GitHub Installer")
    log("=========================")
    if not is_admin():
        row("[INFO]", "Admin", ko("\\uad00\\ub9ac\\uc790 \\uad8c\\ud55c \\uc5c6\\uc74c. Steam \\ud3f4\\ub354 \\uc4f0\\uae30 \\uad8c\\ud55c\\uc774 \\uc788\\uc73c\\uba74 \\uc9c4\\ud589\\ub429\\ub2c8\\ub2e4"))

    mods_dir = resolve_mods_dir(args.mods_dir)
    backup_root = resolve_backup_root(create=not args.dry_run)
    row("[OK]", "Backup", str(backup_root))
    row("[INFO]", "Backup", ko("mods \\ud3f4\\ub354 \\ubc16\\uc5d0 \\uc800\\uc7a5\\ud574 \\ubaa8\\ub4dc \\uc778\\uc2dd\\uc744 \\ud53c\\ud569\\ub2c8\\ub2e4"))
    log("")

    install_all(mods_dir, backup_root, args.force_prism, args.dry_run)

    log("")
    row("[DONE]", "Finish", ko("\\uac8c\\uc784\\uc744 \\uc644\\uc804\\ud788 \\uc885\\ub8cc\\ud55c \\ub4a4 \\ub2e4\\uc2dc \\uc2e4\\ud589\\ud558\\uace0 Prism Shirou\\ub97c \\ucf1c\\uc138\\uc694"))
    return 0


if __name__ == "__main__":
    try:
        raise SystemExit(main())
    except Exception as exc:
        setup_console()
        log("")
        row("[ERROR]", "Installer", str(exc))
        raise SystemExit(1)
