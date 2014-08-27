@echo On

REM Set Variables:
set debug="%Debug%"
if %debug% == "" (
   set debug=0
)

set config="%Config%"
if %config% == "" (
   set config=Release
)

set version="%PackageVersion%"

REM Package Restore:
if %debug%=="1" pause
Source\.nuget\nuget.exe install Source\TrackableEntities.Client\packages.config -OutputDirectory Source\packages -NonInteractive

REM Build:
if %debug%=="1" pause
mkdir Build\Source\TrackableEntities.Common\portable-net40+sl5+win8+windowsphone8
%WINDIR%\Microsoft.NET\Framework\v4.0.30319\msbuild Source\TrackableEntities.Common\TrackableEntities.Common.csproj /p:Configuration="%config%" /m /v:M /fl /flp:LogFile=Build\Source\TrackableEntities.Common\portable-net40+sl5+win8+windowsphone8\msbuild.log;Verbosity=Normal /nr:false
if not "%errorlevel%"=="0" goto failure

REM Package:
if %debug%=="1" pause
Source\.nuget\nuget.exe pack "Source\TrackableEntities.Common\TrackableEntities.Common.csproj" -symbols -o Build\Source\TrackableEntities.Common\portable-net40+sl5+win8+windowsphone8 -p Configuration=%config%;PackageVersion=%version%
if not "%errorlevel%"=="0" goto failure

:success
if %debug%=="1" echo success else exit 0

:failure
if %debug%=="1" echo failure else exit -1
