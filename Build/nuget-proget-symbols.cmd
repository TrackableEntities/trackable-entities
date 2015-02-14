rem VS Setup: http://inedo.com/support/tutorials/debug-into-internal-nuget-packages-with-proget

set source=http://localhost:81/nuget/Default
set version=2.5.0
set key=Admin:Admin

NuGet.exe push Output\TrackableEntities.Common\TrackableEntities.Common.%version%.symbols.nupkg %key% -Source %source%
NuGet.exe push Output\TrackableEntities.Client\TrackableEntities.Client.%version%.symbols.nupkg %key% -Source %source%
NuGet.exe push Output\TrackableEntities.Client.Net4\TrackableEntities.Client.Net4.%version%.symbols.nupkg %key% -Source %source%
NuGet.exe push Output\TrackableEntities.EF.5\TrackableEntities.EF.5.%version%.symbols.nupkg %key% -Source %source%
NuGet.exe push Output\TrackableEntities.EF.6\TrackableEntities.EF.6.%version%.symbols.nupkg %key% -Source %source%
NuGet.exe push Output\TrackableEntities.Patterns\TrackableEntities.Patterns.%version%.symbols.nupkg %key% -Source %source%
NuGet.exe push Output\TrackableEntities.Patterns.EF.6\TrackableEntities.Patterns.EF.6.%version%.symbols.nupkg %key% -Source %source%
