@echo On
set debug=%1

REM Set Variables:
set config=%Configuration%
if "%Configuration%" == "" (
   set config=Release
)
set version=%PackageVersion%
set name=TrackableEntities.EF.5
set source=Source\%name%
set logs=Build\Logs\%name%
set output=Build\Output\%name%

REM Restore:
if "%debug%"=="1" pause
Source\.nuget\nuget.exe install "%source%\packages.config" -OutputDirectory Source\packages -NonInteractive
if "%debug%"=="1" pause
if not "%errorlevel%"=="0" exit

REM Build:
if "%debug%"=="1" pause
mkdir "%logs%"
%WINDIR%\Microsoft.NET\Framework\v4.0.30319\msbuild "%source%\%name%.csproj" /p:Configuration="%config%" /m /v:M /fl /flp:LogFile="%logs%\msbuild.log";Verbosity=Normal /nr:false
if "%debug%"=="1" pause
if not "%errorlevel%"=="0" exit

REM Package:
if "%debug%"=="1" pause
mkdir "%output%"
Source\.nuget\nuget.exe pack "%source%\%name%.csproj" -symbols -o "%output%" -p Configuration="%config%";PackageVersion="%version%"
if "%debug%"=="1" pause
if not "%errorlevel%"=="0" exit
