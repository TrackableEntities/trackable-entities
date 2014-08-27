@echo On

REM Set Variables:
set debug=%1
if "%debug%" == "" (
   set debug=0
)
if "%debug%" == "-debug" (
   set debug=1
)

set config="%Configuration%"
if %config% == "" (
   set config=Release
)

set version="%PackageVersion%"

REM TrackableEntities.Common:
call Build\Source\Scripts\TrackableEntities.Common.cmd
if not "%errorlevel%"=="0" goto failure

REM TrackableEntities.Client:
call Build\Source\Scripts\TrackableEntities.Client.cmd
if not "%errorlevel%"=="0" goto failure

REM TrackableEntities.Client.Net4:
call Build\Source\Scripts\TrackableEntities.Client.Net4.cmd
if not "%errorlevel%"=="0" goto failure

:success
if "%debug%"=="0" exit 0

:failure
if "%debug%"=="0" exit -1
