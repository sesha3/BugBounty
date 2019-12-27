@echo off
@echo Local Nuget package cache clear Started.
for /d /r . %%d in (packages\*.*) do @if exist "%%d" (
rd /s/q "%%d"
@echo Deleted - %%d
)
cd build
powershell -executionpolicy remotesigned -Command .\build.ps1 -target clear-nuget-cache