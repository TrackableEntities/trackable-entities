@echo On
set debug=%1

REM Set Variables:
set version=%PackageVersion%
set name=TrackableEntities.CodeTemplates
set source=Extensions\Templates\%name%
set output=Build\Output\%name%

REM Copy:
if "%debug%"=="1" pause
mkdir "%output%\%name%.Client"
xcopy "%source%.Client\Content" "%output%\%name%.Client\" /e
if "%debug%"=="1" pause
if not "%errorlevel%"=="0" exit

REM Package:
if "%debug%"=="1" pause
xcopy "%source%.Client\%name%.Client.nuspec" "%output%\%name%.Client\" /y
Source\.nuget\nuget.exe pack "%output%\%name%.Client\%name%.Client.nuspec" -o "%output%\%name%.Client" -p PackageVersion="%version%"
