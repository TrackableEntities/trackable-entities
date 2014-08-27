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
call Build\Scripts\TrackableEntities.Common.cmd
if not "%errorlevel%"=="0" goto failure

REM TrackableEntities.Client:
call Build\Scripts\TrackableEntities.Client.cmd
if not "%errorlevel%"=="0" goto failure

:success
if "%debug%"=="0" exit 0

:failure
if "%debug%"=="0" exit -1
