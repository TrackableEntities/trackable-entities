@echo On
set debug=%1

REM Set Variables:
set config=%Configuration%
if "%Configuration%" == "" (
   set config=Release
)
set version=%PackageVersion%
set name=TrackableEntities.Client
set source=Source\%name%
set logs=Build\Logs\%name%
set output=Build\Output\%name%
set target-portable=portable-net45+wp80+win8+wpa81
set target-sl5=sl5
set target-net45=net45

REM Restore:
if "%debug%"=="1" pause
Source\.nuget\nuget.exe install "%source%\packages.config" -OutputDirectory Source\packages -NonInteractive
Source\.nuget\nuget.exe install "%source%.Net45\packages.config" -OutputDirectory Source\packages -NonInteractive
if "%debug%"=="1" pause
if not "%errorlevel%"=="0" exit

REM Build:
if "%debug%"=="1" pause
mkdir "%logs%"
%WINDIR%\Microsoft.NET\Framework\v4.0.30319\msbuild "%source%\%name%.csproj" /p:Configuration="%config%" /m /v:M /fl /flp:LogFile="%logs%\msbuild.log";Verbosity=Normal /nr:false
%WINDIR%\Microsoft.NET\Framework\v4.0.30319\msbuild "%source%.Net45\%name%.Net45.csproj" /p:Configuration="%config%" /m /v:M /fl /flp:LogFile="%logs%\msbuild.Net45.log";Verbosity=Normal /nr:false
if "%debug%"=="1" pause
if not "%errorlevel%"=="0" exit

REM Portable Copy:
if "%debug%"=="1" pause
mkdir "%output%\lib\%target-portable%"
xcopy "%source%\bin\%config%\%name%.dll" "%output%\lib\%target-portable%\" /y
xcopy "%source%\bin\%config%\%name%.xml" "%output%\lib\%target-portable%\" /y
xcopy "%source%\bin\%config%\%name%.pdb" "%output%\lib\%target-portable%\" /y

mkdir "%output%\src\%target-portable%\Properties"
xcopy "Source\AssemblyVersion.cs" "%output%\src\%target-portable%\Properties\" /y
xcopy "%source%\Properties\AssemblyInfo.cs" "%output%\src\%target-portable%\Properties\" /y
xcopy "%source%\*.cs" "%output%\src\%target-portable%\" /y
if "%debug%"=="1" pause
if not "%errorlevel%"=="0" exit

REM Portable-SL5 Copy:
if "%debug%"=="1" pause
mkdir "%output%\lib\%target-sl5%"
xcopy "%source%\bin\%config%\%name%.dll" "%output%\lib\%target-sl5%\" /y
xcopy "%source%\bin\%config%\%name%.xml" "%output%\lib\%target-sl5%\" /y
xcopy "%source%\bin\%config%\%name%.pdb" "%output%\lib\%target-sl5%\" /y

mkdir "%output%\src\%target-sl5%\Properties"
xcopy "Source\AssemblyVersion.cs" "%output%\src\%target-sl5%\Properties\" /y
xcopy "%source%\Properties\AssemblyInfo.cs" "%output%\src\%target-sl5%\Properties\" /y
xcopy "%source%\*.cs" "%output%\src\%target-sl5%\" /y
if "%debug%"=="1" pause
if not "%errorlevel%"=="0" exit

REM Net45 Copy:
if "%debug%"=="1" pause
mkdir "%output%\lib\%target-net45%"
xcopy "%source%.Net45\bin\%config%\%name%.dll" "%output%\lib\%target-net45%\" /y
xcopy "%source%.Net45\bin\%config%\%name%.xml" "%output%\lib\%target-net45%\" /y
xcopy "%source%.Net45\bin\%config%\%name%.pdb" "%output%\lib\%target-net45%\" /y

mkdir "%output%\src\%target-net45%\Properties"
xcopy "Source\AssemblyVersion.cs" "%output%\src\%target-net45%\Properties\" /y
xcopy "%source%\Properties\AssemblyInfo.cs" "%output%\src\%target-net45%\Properties\" /y
xcopy "%source%\*.cs" "%output%\src\%target-net45%\" /y
del "%output%\src\%target-net45%\EntityBasePortable.cs"
xcopy "%source%.Net45\*.cs" "%output%\src\%target-net45%\" /y
if "%debug%"=="1" pause
if not "%errorlevel%"=="0" exit

REM Package:
if "%debug%"=="1" pause
xcopy "%source%\%name%.nuspec" "%output%\" /y
Source\.nuget\nuget.exe pack "%output%\%name%.nuspec" -symbols -o "%output%" -p PackageVersion="%version%"
if "%debug%"=="1" pause
if not "%errorlevel%"=="0" exit
