@echo off

rmdir /s /q .vs
rmdir /s /q .idea

for /f "delims=" %%e in ('dir /A:D /S /B *bin^|find /i "\bin"') do @if exist "%%e" (@rmdir /S /Q %%e)
for /f "delims=" %%e in ('dir /A:D /S /B *obj^|find /i "\obj"') do @if exist "%%e" (@rmdir /S /Q %%e)
for /f "delims=" %%e in ('dir /A:D /S /B *.vs^|find /i "\.vs"') do @if exist "%%e" (@rmdir /S /Q %%e)