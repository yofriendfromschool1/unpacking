#!/usr/bin/env pwsh
#Requires -Version 7.0

Set-StrictMode -Version Latest
$ErrorActionPreference = "Stop"

# =========================================================
# CONFIG
# =========================================================

$Global:ToolDir    = Join-Path $env:USERPROFILE ".xvdctl"
$Global:XvdToolExe = Join-Path $ToolDir "xvdtool.exe"
$Global:LogFile    = Join-Path $ToolDir "xvdctl.log.json"

# =========================================================
# ARGUMENTS (CLI STYLE)
# =========================================================

param(
    [Parameter(Position = 0)]
    [string]$Command,

    [Parameter(Position = 1)]
    [string]$Target,

    [string]$DriveLetter,
    [string]$OutputPath
)

# =========================================================
# LOGGING (FORENSIC JSON)
# =========================================================

function Write-Log {
    param(
        [string]$Event,
        [string]$Message,
        [hashtable]$Data = @{}
    )

    if (-not (Test-Path $Global:ToolDir)) {
        New-Item -ItemType Directory -Path $Global:ToolDir -Force | Out-Null
    }

    $entry = @{
        time    = (Get-Date).ToString("o")
        event   = $Event
        message = $Message
        pid     = $PID
        data    = $Data
    }

    ($entry | ConvertTo-Json -Depth 10) | Add-Content -Path $Global:LogFile
}

# =========================================================
# ADMIN CHECK + ELEVATION SHIM
# =========================================================

function Test-Admin {
    $id = [Security.Principal.WindowsIdentity]::GetCurrent()
    $p  = New-Object Security.Principal.WindowsPrincipal($id)
    return $p.IsInRole([Security.Principal.WindowsBuiltInRole]::Administrator)
}

function Ensure-Admin {
    if (Test-Admin) { return }

    Write-Log "elevation" "Requesting UAC elevation"

    $args = "-NoProfile -ExecutionPolicy Bypass -File `"$PSCommandPath`" $Command `"$Target`""
    Start-Process powershell -Verb RunAs -ArgumentList $args

    exit
}

# =========================================================
# INSTALL XVDTOOL
# =========================================================

function Ensure-XvdTool {

    if (Test-Path $Global:XvdToolExe) {
        return
    }

    Write-Log "install" "Installing xvdtool"

    New-Item -ItemType Directory -Path $Global:ToolDir -Force | Out-Null

    $api = "https://api.github.com/repos/emoose/xvdtool/releases/latest"
    $rel = Invoke-RestMethod $api

    $asset = $rel.assets | Where-Object { $_.name -like "*.zip" } | Select-Object -First 1

    if (-not $asset) {
        throw "xvdtool release asset not found"
    }

    $zip = Join-Path $Global:ToolDir "xvdtool.zip"

    Invoke-WebRequest $asset.browser_download_url -OutFile $zip
    Expand-Archive $zip -DestinationPath $Global:ToolDir -Force
    Remove-Item $zip -Force

    if (-not (Test-Path $Global:XvdToolExe)) {
        throw "xvdtool installation failed"
    }
}

# =========================================================
# EXECUTION WRAPPER
# =========================================================

function Invoke-XvdTool {
    param(
        [string[]]$Args,
        [string]$Op
    )

    Write-Log "exec" "xvdtool execution" @{ args = $Args; op = $Op }

    $proc = Start-Process -FilePath $Global:XvdToolExe `
        -ArgumentList $Args `
        -NoNewWindow -Wait -PassThru

    Write-Log "exit" "xvdtool finished" @{
        exitCode = $proc.ExitCode
        op       = $Op
    }

    if ($proc.ExitCode -ne 0) {
        throw "xvdtool failed ($($proc.ExitCode))"
    }
}

# =========================================================
# VALIDATION LAYER (MOUNT SAFETY)
# =========================================================

function Validate-Mount {
    param(
        [string]$File,
        [string]$Drive
    )

    if (-not (Test-Path $File)) {
        throw "File not found"
    }

    if ($Drive -notmatch '^[A-Z]:$') {
        throw "Invalid drive letter format (expected X:)"
    }

    $size = (Get-Item $File).Length

    Write-Log "validate" "mount preflight" @{
        file = $File
        drive = $Drive
        size = $size
    }
}

# =========================================================
# WSL INSPECTION HOOK (OPTIONAL)
# =========================================================

function Invoke-WSLInspect {
    param([string]$File)

    if (-not (Get-Command wsl -ErrorAction SilentlyContinue)) {
        return
    }

    Write-Log "wsl" "inspecting via WSL" @{ file = $File }

    try {
        wsl ls -lah "/mnt/c/$($File -replace '\\','/' -replace ':','')" 2>$null
    } catch {}
}

# =========================================================
# INIT
# =========================================================

Ensure-XvdTool

# =========================================================
# ROUTER
# =========================================================

switch ($Command) {

    "info" {
        Invoke-XvdTool @("-i", $Target) "info"
        Invoke-WSLInspect $Target
    }

    "mount" {
        Ensure-Admin
        Validate-Mount $Target $DriveLetter
        Invoke-XvdTool @("-m", "-mp", $DriveLetter, $Target) "mount"
    }

    "unmount" {
        Ensure-Admin
        Invoke-XvdTool @("-um", $Target) "unmount"
    }

    "extract" {
        switch ($DriveLetter) {

            "vhd" {
                Invoke-XvdTool @("-xv", $OutputPath, $Target) "extract-vhd"
            }

            "embedded" {
                Invoke-XvdTool @("-xe", $OutputPath, $Target) "extract-embedded"
            }

            default {
                throw "extract requires: vhd | embedded"
            }
        }
    }

    "decrypt" {
        Invoke-XvdTool @("-eu", "-o", $OutputPath, $Target) "decrypt"
    }

    default {
        Write-Host @"
xvdctl CLI

Usage:
  xvdctl info <file>
  xvdctl mount <file> -DriveLetter X:
  xvdctl unmount <file>

  xvdctl extract vhd <file>   -OutputPath out.vhd
  xvdctl extract embedded <file> -OutputPath out.xvd

  xvdctl decrypt <file> -OutputPath out.xvd
"@
    }
}
