@echo On

REM Restore:
if "%debug%"=="1" pause
Source\.nuget\nuget.exe install Source\TrackableEntities.Client\packages.config -OutputDirectory Source\packages -NonInteractive
Source\.nuget\nuget.exe install Source\TrackableEntities.Client\packages.config -OutputDirectory packages -NonInteractive
if not "%errorlevel%"=="0" exit

REM Build:
if "%debug%"=="1" pause
mkdir Build\Source\TrackableEntities.Client\portable-net45+sl5+win8+windowsphone8
%WINDIR%\Microsoft.NET\Framework\v4.0.30319\msbuild Source\TrackableEntities.Client\TrackableEntities.Client.csproj /p:Configuration="%config%" /m /v:M /fl /flp:LogFile=Build\Source\TrackableEntities.Client\portable-net45+sl5+win8+windowsphone8\msbuild.log;Verbosity=Normal /nr:false
if not "%errorlevel%"=="0" exit

REM Package:
if "%debug%"=="1" pause
Source\.nuget\nuget.exe pack "Source\TrackableEntities.Client\TrackableEntities.Client.csproj" -symbols -o Build\Source\TrackableEntities.Client\portable-net45+sl5+win8+windowsphone8 -p Configuration=%config%;PackageVersion=%version%
if not "%errorlevel%"=="0" exit
