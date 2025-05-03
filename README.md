# RDP Wrapper
[![Release](https://img.shields.io/github/v/release/sergiye/rdpWrapper?style=for-the-badge)](https://github.com/sergiye/rdpWrapper/releases/latest)
![Downloads](https://img.shields.io/github/downloads/sergiye/rdpWrapper/total?style=for-the-badge&color=ff4f42)
![Last commit](https://img.shields.io/github/last-commit/sergiye/rdpWrapper?style=for-the-badge&color=00AD00)

----

[<img src="https://github.com/sergiye/hiberbeeTheme/raw/master/assets/ukraine_flag_bar.png" alt="UA"/>](https://u24.gov.ua)


Support the Armed Forces of Ukraine and People Affected by Russia’s Aggression on UNITED24, the official fundraising platform of Ukraine: https://u24.gov.ua.

**Слава Україні!**

[<img src="https://github.com/sergiye/hiberbeeTheme/raw/master/assets/ukraine_flag_bar.png" alt="UA"/>](https://u24.gov.ua)

## Overview

`RDP Wrapper` is a RDP setup and configuration utility

This tool is inspired by the [stascorp's rdpwrap project](https://github.com/stascorp/rdpwrap).
However it is written in pure .NET instead of Pascal/Delphi.
The main idea was to create small and portable single-file application with all required functionality.
And yes, it can auto-generate offsets for new/updated Windows versions - thanks to the [llccd's RDPWrapOffsetFinder project](https://github.com/llccd/RDPWrapOffsetFinder).

RDP Wrapper works as a layer between Service Control Manager and Terminal Services, so the original termsrv.dll file remains untouched. Also this method is very strong against Windows Update.

It's recommended to have original termsrv.dll file with the RDP Wrapper installation. If you have modified it before with other patchers, it may become unstable and crash in any moment.

### What can it do?

The application is portable and has the following features:
 - RDP Wrapper does not patch termsrv.dll, it loads termsrv with different parameters
 - RDP host server on any Windows edition beginning from Vista
 - Using the same user simultaneously for local and remote logon (see configuration app)
 - Console and remote sessions at the same time
 - show RDP service current status
 - configure RDP options
 - install / uninstall wrapper
 - generate config for not supported OS (after windows update) - make sure you have [Microsoft Visual C++ Redistributable](https://learn.microsoft.com/en-us/cpp/windows/latest-supported-vc-redist?view=msvc-170#visual-studio-2015-2017-2019-and-2022) installed
 - check for app updates (from main window system menu)
 - Console and RDP session shadowing (using [Task Manager in Windows 7](http://cdn.freshdesk.com/data/helpdesk/attachments/production/1009641577/original/remote_control.png?1413476051) and lower, and [Remote Desktop Connection in Windows 8](http://woshub.com/rds-shadow-how-to-connect-to-a-user-session-in-windows-server-2012-r2/) and higher)
 - Windows 2000, XP and Server 2003 are not supported

### What does it look like?

Here's a preview of the app's UI running on Windows 10:

[<img src="https://github.com/sergiye/rdpWrapper/raw/master/preview.png" alt="Themes" width="300"/>](https://raw.githubusercontent.com/sergiye/rdpWrapper/master/preview.png)


## Download

**The recommended way to get the program is BUILD from source**
- Install git, Visual Studio
- `git clone https://github.com/sergiye/rdpWrapper.git`
- build

**or download build from [releases](https://github.com/sergiye/rdpWrapper/releases).**

## How can I help improve it?
The rdpWrapper team welcomes feedback and contributions!<br/>
You can check if it works properly on your PC. If you notice any inaccuracies, please send us a pull request. If you have any suggestions or improvements, don't hesitate to create an issue.

Also, don't forget to star the repository to help other people find it.

## License

RDP Wrapper is free and open source software licensed under GNU General Public License, Version 3 [GNU 3.0](https://www.gnu.org/licenses/gpl-3.0.html). You can use it for personal and commercial purposes.
