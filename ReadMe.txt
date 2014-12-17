Trackable Entities Source

Welcome to the source code repository for TrackableEntities!

Version 1.0 includes support for both ASP.NET Web API and WCF
on Visual Studio 2012 and 2013.  The VSIX installers include
all required NuGet packages.

Version 1.01 added an AcceptChanges method to TrackableEntities.EF
packages, to set items to Unchanged after persisting updates.
Also included is a MergeChanges on ChangeTrackingCollection,
which merges original and updated entities.

Version 1.03 included templates for Repository and Unit of Work.

Version 2.0 added support for all relationship types:
1-M, 1-1, M-1, and M-M. Improved MergeChanges was added,
as well as LoadRelatedEntities DbContext extension method.

Version 2.1 added support for the EF 6.x Tools for Visual
Studio, including both Code First from Database and EF Designer
from Database (aka Model First, or Database First). All NuGet
packages were updated in the samples and templates.

Version 2.2 addressed numerous issues with client and server-side
class libraries and included team member contributions. The
complete release notes can be found here:
https://trackable.codeplex.com/releases/view/126165

The repository contains source code, NuGet packages, samples,
and Visual Studio Extension projects.

For more information please contact the author:
Tony Sneed
Email: tony@tonysneed.com

