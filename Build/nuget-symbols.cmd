rem Pushing Symbols packages to NuGet.org

set version=2.5.0-alpha4
NuGet.exe SetApiKey 5682793e-2994-4016-b7b4-c11be576703b
NuGet.exe push Output\TrackableEntities.Common\TrackableEntities.Common.%version%.symbols.nupkg
NuGet.exe push Output\TrackableEntities.Client\TrackableEntities.Client.%version%.symbols.nupkg
NuGet.exe push Output\TrackableEntities.Client.Net4\TrackableEntities.Client.Net4.%version%.symbols.nupkg
NuGet.exe push Output\TrackableEntities.EF.5\TrackableEntities.EF.5.%version%.symbols.nupkg
NuGet.exe push Output\TrackableEntities.EF.6\TrackableEntities.EF.6.%version%.symbols.nupkg
NuGet.exe push Output\TrackableEntities.Patterns\TrackableEntities.Patterns.%version%.symbols.nupkg
NuGet.exe push Output\TrackableEntities.Patterns.EF.6\TrackableEntities.Patterns.EF.6.%version%.symbols.nupkg
