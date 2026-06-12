# RDP Wrapper
[![Release](https://img.shields.io/github/v/release/sergiye/rdpWrapper)](https://github.com/sergiye/rdpWrapper/releases/latest)
[![Downloads](https://img.shields.io/github/downloads/sergiye/rdpWrapper/total?color=ff4f42)](https://sergiye.github.io/github-release-stats/?username=sergiye&repository=rdpWrapper&page=1&per_page=100)
![Last commit](https://img.shields.io/github/last-commit/sergiye/rdpWrapper?color=00AD00)

[![](https://img.shields.io/badge/WINDOWS-7%20%E2%80%93%2011-blue)](https://endoflife.date/windows)
[![](https://img.shields.io/badge/SERVER-2012%20%E2%80%93%202025-blue)](https://endoflife.date/windows-server)

----

## Overview

`RDP Wrapper` is a RDP setup and configuration utility

This tool is inspired by the [stascorp's rdpwrap project](https://github.com/stascorp/rdpwrap).
However it is written in pure .NET instead of Pascal/Delphi.
The main idea was to create small and portable single-file application with all required functionality.

And yes, it can auto-generate offsets for new/updated Windows versions - thanks to the @llccd projects:
 - [TermWrap](https://github.com/llccd/TermWrap)
 - [RDPWrapOffsetFinder](https://github.com/llccd/RDPWrapOffsetFinder).

RDP Wrapper works as a layer between Service Control Manager and Terminal Services, so the original termsrv.dll file remains untouched. Also this method is very strong against Windows Update.

It's recommended to have original termsrv.dll file with the RDP Wrapper installation. If you have modified it before with other patchers, it may become unstable and crash in any moment.

### What can it do?

The application is portable and has the following features:
 - RDP Wrapper does not patch termsrv.dll, it loads termsrv with different parameters
 - RDP host server on any Windows edition beginning from Vista
 - Using the same user simultaneously for local and remote logon (see configuration app)
 - Console and remote sessions at the same time
 - Enabled camera and USB redirection (when TermWrap installed)
 - show RDP service current status
 - configure RDP options
 - install / uninstall wrapper
 - generate config for not supported OS (after windows update) - make sure you have [Microsoft Visual C++ Redistributable](https://learn.microsoft.com/en-us/cpp/windows/latest-supported-vc-redist?view=msvc-170#visual-studio-2015-2017-2019-and-2022) installed
 - check for app updates (from main window system menu)
 - Console and RDP session shadowing (using [Task Manager in Windows 7](http://cdn.freshdesk.com/data/helpdesk/attachments/production/1009641577/original/remote_control.png?1413476051) and lower, and [Remote Desktop Connection in Windows 8](http://woshub.com/rds-shadow-how-to-connect-to-a-user-session-in-windows-server-2012-r2/) and higher)
 - Windows 2000, XP and Server 2003 are not supported

 #### Additional application features

  - auto-update application to the latest version
  - open 'wrap.ini' in editor
  - console mode with file output ([#16](https://github.com/sergiye/rdpWrapper/issues/16))
  - choose where to store settings: file or registry
  - optionally add Defender exclusion ([#6](https://github.com/sergiye/rdpWrapper/issues/6))
  - optionally add firewall rule when RDP port changed
  - create a user and add to rdp group ([#9](https://github.com/sergiye/rdpWrapper/issues/9))
  - open user management (new/old style)
  - check if Microsoft Visual C++ 2015-2022 Redistributable is installed ([#7](https://github.com/sergiye/rdpWrapper/issues/7))
  - fix Microsoft account local cache ([#10](https://github.com/sergiye/rdpWrapper/issues/10))
  - configuring display/hide of [client security warnings](https://learn.microsoft.com/en-us/windows-server/remote/remote-desktop-services/remotepc/understanding-security-warnings)
  - configuring client USB device redirection restrictions
  - optional guarded `termsrv.dll` patch helper for known builds where wrapper mode still leaves Windows restricted to one user

  and you can get full/lite/x64/x86 version of the application - the one that best suits your needs

### What does it look like?

Here's a preview of the app's UI running on Windows 10:

[<img src="https://github.com/sergiye/rdpWrapper/raw/master/preview.png" alt="Themes" width="300"/>](https://raw.githubusercontent.com/sergiye/rdpWrapper/master/preview.png)

Also there are:
 - `Light`/`Dark` themes with auto switching mode.
 - Custom `themes` supported from external files

To add custom theme to the app, just create a `themes` folder next to the executable file and place any {themeName}.json files there.
Custom theme.json file content example:
```json
{
  "DisplayName": "Custom Theme",
  "DarkMode": true,
  "BackgroundColor": "#1E1E1E",
  "ForegroundColor": "#E9E9E9",
  "HyperlinkColor": "#00D980",
  "SelectedBackgroundColor": "#4CBB17",
  "SelectedForegroundColor": "#000000",
  "LineColor": "#262626",
  "StrongLineColor": "#454545",
  "WarnColor": "#FF4500"
}
```
Don't forget to restart the app to scan for new theme files!

## Download

The published version can be obtained from [releases](https://github.com/sergiye/rdpWrapper/releases).

> [!WARNING]
>Microsoft and other major antivirus vendors have flagged RdpWrapper as "malware". This is likely due to Microsoft's hatred against RdpWrapper, not because it contains a virus or such. Flags from Microsoft usually spread to other antivirus vendors.
>RdpWrapper has a history of being falsely detected as malware by antiviruses (including Defender). This is likely due to its behavior, such as placing binaries in `c:\Program Files\` and performing in-memory extraction of wrapping libraries.

Currently, Defender does not flag this release, but it is likely that future updates may being flagged by Defender's machine learning-based detection systems within a few days of release.

> [!IMPORTANT]
>If Defender or another antivirus detects any part of RdpWrapper as malware, it may prevent proper work or cause application to fail to start.
> RdpWrapper will not start if this file exists but is blocked from being loaded.
>We strongly recommend excluding RdpWrapper's binaries from antivirus scans


> [!TIP]
> Please include the following folder in your antivirus' exclusion list to prevent issues due to antivirus detections:
> c:\Program Files\RDP Wrapper\

RdpWrapper can add this exception automatically only for Defender, but if it does not, you can do it manually

For Defender, you can run the following script in PowerShell as an administrator:
```powershell
Add-MpPreference -ExclusionPath "c:\Program Files\RDP Wrapper\"
```

> [!CAUTION]
> If your antivirus deletes the downloaded app file, you may need to temporarily disable real-time protection or save the file in an excluded folder.
> If you are uncomfortable with this process, or if your antivirus is managed by your company, we advise against using RdpWrapper. Please consider alternative solutions instead.

## Notes

### termsrv.dll patch helper

RDP Wrapper normally keeps the original `termsrv.dll` untouched. For known
Windows builds where wrapper mode still leaves Windows restricted to one user,
the application also includes an optional guarded patch helper.

For `termsrv.dll` x64 `10.0.26100.8521`, the helper validates that the expected
byte pattern occurs exactly once, verifies the computed patch offset is
`0x9C547`, and changes that byte from `75` to `EB`. It backs up the original
file under `C:\Program Files\RDP Wrapper\termsrv-backups`, replaces the local
system file, restores `TrustedInstaller` ownership, and restarts `TermService`
if it was running.

Usage:

- UI: `Tools` -> `Patch termsrv.dll 26100.8521`
- CLI: `rdpWrapper.exe -patchtermsrv`
- Restore: `Tools` -> `Restore termsrv.dll backup`
- Restore CLI: `rdpWrapper.exe -restoretermsrv`

Do not use DLLs downloaded from third-party file sharing links. Patch only the
local system file, and re-check after Windows updates because servicing may
restore `termsrv.dll`.

### Enable USB redirection
To enable remote desktop USB redirection, additional group policy settings are required (gpedit):

`Computer Configuration\Administrative Templates\System\Device Installation` - `Allow remote access to the Plug and Play interface` -> Enabled

`Computer Configuration\Administrative Templates\Windows Components\Remote Desktop Services\Remote Desktop Session Host\Device and Resource Redirection` - `Do not allow supported Plug and Play device redirection` -> Disabled

`Computer Configuration\Administrative Templates\Windows Components\Remote Desktop Services\Remote Desktop Connection Client\RemoteFX USB Device Redirection` - `Allow RDP redirection of other supported RemoteFX USB devices from this computer` -> Enabled

### Notes on audio recording redirection
Just like UmWrap, EndpWrap is only needed on server and home editions. It gets loaded in all applications that play/record remote audio, and may cause some tricky applications to stuck or crash.

To enable audio recording redirection, both EndpWrap.dll and Zydis.dll needs to be copied to the system32 folder. After that, change the following registry entry from rdpendp.dll to EndpWrap.dll:

HKEY_LOCAL_MACHINE\SYSTEM\CurrentControlSet\Control\Terminal Server\WinStations\RDP-Tcp\AudioEnumeratorDll


## How can I help improve it?
The rdpWrapper team welcomes feedback and contributions!<br/>
You can check if it works properly on your PC. If you notice any inaccuracies, please send us a pull request. If you have any suggestions or improvements, don't hesitate to create an issue.

Also, don't forget to ★ star ★ the repository to help other people find it.

<!-- [![Star History Chart](https://api.star-history.com/svg?repos=sergiye/rdpwrapper&type=Date)](https://star-history.com/#sergiye/rdpwrapper&Date) -->

[//]: # ([![Stargazers over time]&#40;https://starchart.cc/sergiye/rdpwrapper.svg?variant=adaptive&#41;]&#40;https://starchart.cc/sergiye/rdpwrapper&#41;)

<!-- [![Stargazers](https://reporoster.com/stars/sergiye/rdpWrapper)](https://star-history.com/#sergiye/rdpWrapper&Date) -->

<!-- [![Forkers](https://reporoster.com/forks/sergiye/rdpWrapper)](https://github.com/sergiye/rdpWrapper/network/members) -->

## Donate!
Every [cup of coffee](https://patreon.com/SergiyE) you donate will help this app become better and let me know that this project is in demand.
