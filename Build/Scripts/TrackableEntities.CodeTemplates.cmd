@echo On
set debug=%1

REM Set Variables:
set version=%PackageVersion%
set name=TrackableEntities.CodeTemplates
set source=Extensions\Templates\%name%
set output=Build\Output\%name%

rem Client.Portable
set ext=Client.Portable

REM Copy:
if "%debug%"=="1" pause
mkdir "%output%\%name%.%ext%"
xcopy "%source%.%ext%\Content" "%output%\%name%.%ext%\" /e /y
if "%debug%"=="1" pause
if not "%errorlevel%"=="0" exit

REM Package:
if "%debug%"=="1" pause
xcopy "%source%.%ext%\%name%.%ext%.nuspec" "%output%\%name%.%ext%\" /y
Source\.nuget\nuget.exe pack "%output%\%name%.%ext%\%name%.%ext%.nuspec" -o "%output%\%name%.%ext%" -p PackageVersion="%version%"
if "%debug%"=="1" pause
if not "%errorlevel%"=="0" exit

rem Client.Net45
set ext=Client.Net45

REM Copy:
if "%debug%"=="1" pause
mkdir "%output%\%name%.%ext%"
xcopy "%source%.%ext%\Content" "%output%\%name%.%ext%\" /e /y
if "%debug%"=="1" pause
if not "%errorlevel%"=="0" exit

REM Package:
if "%debug%"=="1" pause
xcopy "%source%.%ext%\%name%.%ext%.nuspec" "%output%\%name%.%ext%\" /y
Source\.nuget\nuget.exe pack "%output%\%name%.%ext%\%name%.%ext%.nuspec" -o "%output%\%name%.%ext%" -p PackageVersion="%version%"
if "%debug%"=="1" pause
if not "%errorlevel%"=="0" exit

rem Service.Net45
set ext=Service.Net45

REM Copy:
if "%debug%"=="1" pause
mkdir "%output%\%name%.%ext%"
xcopy "%source%.%ext%\Content" "%output%\%name%.%ext%\" /e /y
if "%debug%"=="1" pause
if not "%errorlevel%"=="0" exit

REM Package:
if "%debug%"=="1" pause
xcopy "%source%.%ext%\%name%.%ext%.nuspec" "%output%\%name%.%ext%\" /y
Source\.nuget\nuget.exe pack "%output%\%name%.%ext%\%name%.%ext%.nuspec" -o "%output%\%name%.%ext%" -p PackageVersion="%version%"
if "%debug%"=="1" pause
if not "%errorlevel%"=="0" exit

rem Shared.Net45
set ext=Shared.Net45

REM Copy:
if "%debug%"=="1" pause
mkdir "%output%\%name%.%ext%"
xcopy "%source%.%ext%\Content" "%output%\%name%.%ext%\" /e /y
if "%debug%"=="1" pause
if not "%errorlevel%"=="0" exit

REM Package:
if "%debug%"=="1" pause
xcopy "%source%.%ext%\%name%.%ext%.nuspec" "%output%\%name%.%ext%\" /y
Source\.nuget\nuget.exe pack "%output%\%name%.%ext%\%name%.%ext%.nuspec" -o "%output%\%name%.%ext%" -p PackageVersion="%version%"
if "%debug%"=="1" pause
if not "%errorlevel%"=="0" exit

