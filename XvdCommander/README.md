# XvdCommander – `xvdctl` CLI for XVD Files

`xvdctl` is a PowerShell 7+ command-line tool that simplifies working with **XVD** (Xbox Virtual Disk) files. It acts as a smart wrapper around [`xvdtool`](https://github.com/emoose/xvdtool) – automatically installing it, handling administrative elevation, logging every operation, and providing a consistent, easy-to-remember interface.

## Features

- 🔍 **Inspect** – Show low‑level information about an XVD file.
- 🔗 **Mount** – Mount an XVD as a drive letter (requires admin).
- 🧲 **Unmount** – Safely unmount a previously mounted XVD.
- 📦 **Extract** – Extract as a VHD (`vhd`) or as an unencrypted XVD (`embedded`).
- 🔓 **Decrypt** – Fully decrypt an XVD (remove encryption layer).
- 📜 **Forensic logging** – Every action is written to a JSON log file.
- ⚡ **Auto‑elevation** – Mount / unmount automatically request UAC elevation.
- 🐧 **WSL integration** – If WSL is present, `info` also shows a Linux‑style `ls` of the file.

## Prerequisites

- **Windows** (10 / 11, or Windows Server)
- **PowerShell 7.0 or higher** (`pwsh`)  
  [Download PowerShell 7](https://learn.microsoft.com/en-us/powershell/scripting/install/installing-powershell-on-windows)
- **Internet connection** – to download `xvdtool` on first run.

## Installation

```powershell
# Clone the repository
git clone https://github.com/SleepTheGod/XvdCommander.git
cd XvdCommander

# (Optional) Add the folder to your PATH so you can call xvdctl from anywhere
$userPath = [Environment]::GetEnvironmentVariable("Path", "User")
[Environment]::SetEnvironmentVariable("Path", "$userPath;$PWD", "User")
```

Now you can run the tool using `./main.ps1` (or `pwsh main.ps1`).  
For convenience, you may rename `main.ps1` to `xvdctl.ps1`.

## Usage

```
xvdctl info <file>
xvdctl mount <file> -DriveLetter X:
xvdctl unmount <file>

xvdctl extract vhd <file> -OutputPath out.vhd
xvdctl extract embedded <file> -OutputPath out.xvd

xvdctl decrypt <file> -OutputPath out.xvd
```

### Global options

| Option | Description |
|--------|-------------|
| `-DriveLetter` | Used with `mount` to specify the drive letter (e.g. `Z:`).<br>With `extract` it selects the extraction mode: `vhd` or `embedded`. |
| `-OutputPath` | Required for `extract` and `decrypt`. Destination file path. |

### Examples

**Get information about an XVD file**  
```powershell
./main.ps1 info "C:\Games\title.xvd"
```

**Mount an XVD as drive X:**  
```powershell
./main.ps1 mount "D:\xbox\content.xvd" -DriveLetter X:
```
If you are not already admin, a UAC prompt will appear.

**Unmount the same XVD**  
```powershell
./main.ps1 unmount "D:\xbox\content.xvd"
```

**Extract a decrypted VHD**  
```powershell
./main.ps1 extract vhd "encrypted.xvd" -OutputPath "disk.vhd"
```

**Extract embedded (unencrypted) XVD**  
```powershell
./main.ps1 extract embedded "locked.xvd" -OutputPath "unlocked.xvd"
```

**Fully decrypt an XVD**  
```powershell
./main.ps1 decrypt "secure.xvd" -OutputPath "decrypted.xvd"
```

## How it works

1. On first run, `xvdctl` downloads the latest `xvdtool.exe` from [emoose/xvdtool](https://github.com/emoose/xvdtool) into `%USERPROFILE%\.xvdctl\`.
2. All subsequent commands call that local `xvdtool.exe` with appropriate arguments.
3. Every operation is logged as a JSON object in `%USERPROFILE%\.xvdctl\xvdctl.log.json` (timestamp, event, command line, exit code, etc.).
4. `mount` and `unmount` automatically request administrator rights via UAC – no need to manually start an elevated shell.
5. The `info` command also tries to invoke `wsl ls -lah` on the same file if WSL is installed, giving you a quick file system view.

## Logging

All actions are recorded in a **forensic JSON log** located at:

```
%USERPROFILE%\.xvdctl\xvdctl.log.json
```

Example log entry:
```json
{
  "time": "2025-01-15T14:23:07.1234567+01:00",
  "event": "exec",
  "message": "xvdtool execution",
  "pid": 12345,
  "data": {
    "args": ["-i", "C:\\Games\\file.xvd"],
    "op": "info"
  }
}
```

## Troubleshooting

### “File not found” when mounting
Make sure the XVD path is correct. The tool does **not** resolve relative paths – use an absolute path or `Resolve-Path`.

### “xvdtool installation failed”
Check your internet connection. The tool downloads the latest ZIP from GitHub releases. If the repository changes its asset naming, update the script accordingly.

### Mount / unmount fails with “Access denied”
The script requests elevation automatically. If UAC is disabled or you deny the prompt, the operation cannot proceed. Run PowerShell as Administrator manually, then retry.

### PowerShell version error
The script requires **PowerShell 7**. Verify with `$PSVersionTable.PSVersion`.  
If you have Windows PowerShell 5.1, install PowerShell 7 from [Microsoft](https://learn.microsoft.com/en-us/powershell/scripting/install/installing-powershell-on-windows).

## Contributing

Issues and pull requests are welcome!  
Please follow the existing coding style (strict mode, logging, parameter validation).

## License

This project is licensed under the **MIT License**.  
`xvdtool` is property of its respective author (emoose).

## Acknowledgements

- [emoose/xvdtool](https://github.com/emoose/xvdtool) – the core utility that makes XVD manipulation possible.
- The PowerShell community for scripting patterns and best practices.

---

**Repository**: [https://github.com/SleepTheGod/XvdCommander](https://github.com/SleepTheGod/XvdCommander)  
**Main script**: `main.ps1` – run with `pwsh ./main.ps1 <command> ...`
