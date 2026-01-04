#!/usr/bin/env python3
# go to line 115 to change time for checking time
"""
Process Suspender - Monitors for check line 114 to change the name and suspends its threads.
Windows only - No external dependencies required.
"""

import ctypes
import ctypes.wintypes as wintypes
import time
import sys

# ============== Windows API Constants ==============
TH32CS_SNAPPROCESS = 0x00000002
TH32CS_SNAPTHREAD = 0x00000004
THREAD_SUSPEND_RESUME = 0x0002

# ============== Structures ==============
class PROCESSENTRY32W(ctypes.Structure):
    _fields_ = [
        ("dwSize", wintypes.DWORD),
        ("cntUsage", wintypes.DWORD),
        ("th32ProcessID", wintypes.DWORD),
        ("th32DefaultHeapID", ctypes.POINTER(ctypes.c_ulong)),
        ("th32ModuleID", wintypes.DWORD),
        ("cntThreads", wintypes.DWORD),
        ("th32ParentProcessID", wintypes.DWORD),
        ("pcPriClassBase", ctypes.c_long),
        ("dwFlags", wintypes.DWORD),
        ("szExeFile", wintypes.WCHAR * 260)
    ]

class THREADENTRY32(ctypes.Structure):
    _fields_ = [
        ("dwSize", wintypes.DWORD),
        ("cntUsage", wintypes.DWORD),
        ("th32ThreadID", wintypes.DWORD),
        ("th32OwnerProcessID", wintypes.DWORD),
        ("tpBasePri", ctypes.c_long),
        ("tpDeltaPri", ctypes.c_long),
        ("dwFlags", wintypes.DWORD)
    ]

# ============== Load Kernel32 ==============
kernel32 = ctypes.windll.kernel32

# ============== Functions ==============
def is_admin():
    """Check if script is running with administrator privileges."""
    try:
        return ctypes.windll.shell32.IsUserAnAdmin()
    except:
        return False

def get_process_ids_by_name(process_name):
    """Get all PIDs for processes matching the given name."""
    pids = []
    h_snapshot = kernel32.CreateToolhelp32Snapshot(TH32CS_SNAPPROCESS, 0)
    
    if h_snapshot == -1:
        return pids
    
    pe32 = PROCESSENTRY32W()
    pe32.dwSize = ctypes.sizeof(PROCESSENTRY32W)
    
    try:
        if kernel32.Process32FirstW(h_snapshot, ctypes.byref(pe32)):
            while True:
                if pe32.szExeFile.lower() == process_name.lower():
                    pids.append(pe32.th32ProcessID)
                if not kernel32.Process32NextW(h_snapshot, ctypes.byref(pe32)):
                    break
    finally:
        kernel32.CloseHandle(h_snapshot)
    
    return pids

def suspend_threads(pid):
    """Suspend all threads belonging to the specified process ID."""
    h_snapshot = kernel32.CreateToolhelp32Snapshot(TH32CS_SNAPTHREAD, 0)
    
    if h_snapshot == -1:
        return 0
    
    te32 = THREADENTRY32()
    te32.dwSize = ctypes.sizeof(THREADENTRY32)
    
    suspended_count = 0
    
    try:
        if kernel32.Thread32First(h_snapshot, ctypes.byref(te32)):
            while True:
                if te32.th32OwnerProcessID == pid:
                    h_thread = kernel32.OpenThread(
                        THREAD_SUSPEND_RESUME, 
                        False, 
                        te32.th32ThreadID
                    )
                    if h_thread:
                        result = kernel32.SuspendThread(h_thread)
                        if result != 0xFFFFFFFF:  # -1 indicates failure
                            suspended_count += 1
                        kernel32.CloseHandle(h_thread)
                
                if not kernel32.Thread32Next(h_snapshot, ctypes.byref(te32)):
                    break
    finally:
        kernel32.CloseHandle(h_snapshot)
    
    return suspended_count

def main():
    # ============== Configuration ==============
    TARGET_PROCESS = "VolcanoUpdater.exe"
    SCAN_INTERVAL = 0.25  # seconds
    
    # ============== Startup ==============
    print("=" * 55)
    print("  Process Suspender")
    print("=" * 55)
    print(f"  Target Process : {TARGET_PROCESS}")
    print(f"  Scan Interval  : {SCAN_INTERVAL} seconds")
    print(f"  Admin Rights   : {'Yes' if is_admin() else 'No (may have limited access)'}")
    print("=" * 55)
    print("\n[*] Monitoring for target process...")
    print("[*] Press Ctrl+C to stop\n")
    
    # Track which PIDs we've already suspended
    suspended_pids = set()
    total_suspended = 0
    
    try:
        while True:
            # Get current instances of target process
            current_pids = set(get_process_ids_by_name(TARGET_PROCESS))
            
            # Find new processes we haven't suspended yet
            new_pids = current_pids - suspended_pids
            
            for pid in new_pids:
                timestamp = time.strftime("%H:%M:%S")
                print(f"[{timestamp}] [!] DETECTED: {TARGET_PROCESS} (PID: {pid})")
                
                # Suspend all threads
                thread_count = suspend_threads(pid)
                
                if thread_count > 0:
                    print(f"[{timestamp}] [+] SUSPENDED: {thread_count} thread(s)")
                    suspended_pids.add(pid)
                    total_suspended += 1
                else:
                    print(f"[{timestamp}] [-] FAILED: Could not suspend (access denied?)")
            
            # Clean up: remove PIDs that no longer exist
            # This allows re-catching if a new process gets the same PID
            suspended_pids &= current_pids
            
            time.sleep(SCAN_INTERVAL)
            
    except KeyboardInterrupt:
        print(f"\n{'=' * 55}")
        print(f"  Stopped by user")
        print(f"  Total processes suspended: {total_suspended}")
        print(f"{'=' * 55}")
        sys.exit(0)

if __name__ == "__main__":
    # Platform check
    if sys.platform != 'win32':
        print("[ERROR] This script only works on Windows.")
        sys.exit(1)
    
    main()
