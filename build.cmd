@echo On

set debug=%1
if "%debug%" == "" (
   set debug=0
)
if "%debug%" == "-debug" (
   set debug=1
)

REM TrackableEntities.Common:
call Build\Source\Scripts\TrackableEntities.Common.cmd %debug%
if not "%errorlevel%"=="0" goto failure

REM TrackableEntities.Client:
call Build\Source\Scripts\TrackableEntities.Client.cmd %debug%
if not "%errorlevel%"=="0" goto failure

REM TrackableEntities.Client.Net4:
call Build\Source\Scripts\TrackableEntities.Client.Net4.cmd %debug%
if not "%errorlevel%"=="0" goto failure

REM TrackableEntities.EF.5:
call Build\Source\Scripts\TrackableEntities.EF.5.cmd %debug%
if not "%errorlevel%"=="0" goto failure

REM TrackableEntities.EF.6:
call Build\Source\Scripts\TrackableEntities.EF.6.cmd %debug%
if not "%errorlevel%"=="0" goto failure

REM TrackableEntities.Patterns:
call Build\Source\Scripts\TrackableEntities.Patterns.cmd %debug%
if not "%errorlevel%"=="0" goto failure

REM TrackableEntities.Patterns.EF.6:
call Build\Source\Scripts\TrackableEntities.Patterns.EF.6.cmd %debug%
if not "%errorlevel%"=="0" goto failure

:success
if "%debug%"=="0" exit 0

:failure
if "%debug%"=="0" exit -1
