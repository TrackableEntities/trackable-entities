REM Build:
if "%debug%"=="1" pause
mkdir Build\Source\Output\TrackableEntities.Common\portable-net40+sl5+win8+windowsphone8
%WINDIR%\Microsoft.NET\Framework\v4.0.30319\msbuild Source\TrackableEntities.Common\TrackableEntities.Common.csproj /p:Configuration="%config%" /m /v:M /fl /flp:LogFile=Build\Source\Output\TrackableEntities.Common\portable-net40+sl5+win8+windowsphone8\msbuild.log;Verbosity=Normal /nr:false
if not "%errorlevel%"=="0" exit

REM Package:
if "%debug%"=="1" pause
Source\.nuget\nuget.exe pack "Source\TrackableEntities.Common\TrackableEntities.Common.csproj" -symbols -o Build\Source\Output\TrackableEntities.Common\portable-net40+sl5+win8+windowsphone8 -p Configuration=%config%;PackageVersion=%version%
if not "%errorlevel%"=="0" exit