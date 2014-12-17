@echo On
set debug=%1

REM Set Variables:
set config="%Configuration%"
if %config% == "" (
   set config=Release
)
set version="%PackageVersion%"

REM Restore:
if "%debug%"=="1" pause
Source\.nuget\nuget.exe install Source\TrackableEntities.Client\packages.config -OutputDirectory Source\packages -NonInteractive
Source\.nuget\nuget.exe install Source\TrackableEntities.Client\packages.config -OutputDirectory packages -NonInteractive
if "%debug%"=="1" pause
if not "%errorlevel%"=="0" exit

REM Build:
if "%debug%"=="1" pause
mkdir Build\Source\Output\TrackableEntities.Client\portable-net45+sl5+win8+windowsphone8
%WINDIR%\Microsoft.NET\Framework\v4.0.30319\msbuild Source\TrackableEntities.Client\TrackableEntities.Client.csproj /p:Configuration="%config%" /m /v:M /fl /flp:LogFile=Build\Source\Output\TrackableEntities.Client\portable-net45+sl5+win8+windowsphone8\msbuild.log;Verbosity=Normal /nr:false
if "%debug%"=="1" pause
if not "%errorlevel%"=="0" exit

REM Package:
if "%debug%"=="1" pause
Source\.nuget\nuget.exe pack "Source\TrackableEntities.Client\TrackableEntities.Client.csproj" -symbols -o Build\Source\Output\TrackableEntities.Client\portable-net45+sl5+win8+windowsphone8 -p Configuration=%config%;PackageVersion=%version%
if "%debug%"=="1" pause
if not "%errorlevel%"=="0" exit
