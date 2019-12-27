@echo off
@echo bin and obj folders delete started.
for /d /r . %%d in (bin,obj) do @if exist "%%d" (
rd /s/q "%%d"
@echo Deleted - %%d
)
set /p input = "All bin and obj folders are deleted."