# **Trackable Entities**
## N-Tier Support for Entity Framework with WCF or ASP.NET Web API

### *Welcome to the Trackable Entities GitHub Repository!*

### To get started with Trackable Entities please visit the project home page: http://trackableentities.github.io. There you will find:
- Installation instructions
- Getting started videos
- Samples to download
- Debugging information
- Conribution guidelines

Trackable Entities supports *tracking changes* to entities in an object graph, so that they can be sent to a web service and persisted in a *single round trip* and within a *single transaction*.  This functionality is provided as a set of **NuGet packages**.

The server-side NuGet packages provide an `ApplyChanges` extension method to Entity Framework's `DbContext` class, which traverses one or more object graphs, informing Entity Framework of each entity's change state.  After calling `SaveChanges`, you can then call `AcceptChanges` to set the state of all entities to `Unchanged` before returning them to the client.  There is also a `LoadRelatedEntities` method which you can call to populate reference properties on added entities.

The client-side packages provide a *change-tracker* for marking entities as `Added`, `Modified` or `Deleted` whenever they are inserted, modified or removed at any level in an object graph.  To start tracking changes, simply add one or more entities to a `ChangeTrackingCollection` and set the `Tracking` property to `true`.  When you want to persist changes, you can call `GetChanges` on the change tracker to get only changed items, so that unchanged entities need not be sent to the server, saving bandwidth and increasing performance.  Trackable Entities will correlate entities so that you can merge updated entities back into the original object graph by calling `MergeChanges`.  While you can certainly set `TrackingState` on entities manually, the change tracker is deployed as a *portable class library*, so you can use it from any .NET client, including desktop or mobile applications.

While you can use the NuGet packages by themselves, Trackable Entities provides a powerful **Visual Studio extension**, which you can install from within Visual Studio [2012](https://visualstudiogallery.msdn.microsoft.com/e6754f27-894d-45c4-833c-57aaa3288a31), [2013](http://visualstudiogallery.msdn.microsoft.com/74e6d323-c827-48be-90da-703a9fa8f530) or [2015](https://visualstudiogallery.msdn.microsoft.com/1815bc2c-e2ee-4df7-866f-fb8c45987515), by selecting *Extensions and Updates* from the *Tools* menu and searching for "Trackable Entities" in the Visual Studio online extensions gallery.

After installing the VS extension, you will be able to add a **Trackable Entities Class Library** to an existing solution, selecting the kind of entities you wish to create: Service, Client or Shared (.NET 4.5 or Portable). The resulting project will include customizable T4 templates for generating entity classes from an Entity Framework data model (EDMX or Code First).

Trackable Entities also provides several **Visual Studio multi-project templates** which include all the projects needed to build a complete n-tier solution with end-to-end change tracking.  All the NuGet packages are present and inter-project references are correctly set.  This allows you to see how all the pieces fit together and can be a huge time-saver when you want to get up and running quickly. To build an n-tier solution with Trackable Entities and Entity Framework, you can choose from the following Visual Studio project templates, which you will see under the *Trackable* category in the *New Project* dialog:

- Trackable WCF Service Application
- Trackable Web API Application
- Trackable Web API Application with Repository and Unit of Work

You will then find a number of **Visual Studio item templates** to add WCF services, Web API controllers, and repository or unit of work classes.  The template for repository and unit of work patterns will install NuGet packages which include generic base classes for these patterns as well as bootstrap code for wiring up a *dependency injection* framework.

If you have questions, comments or bugs to report, please open an issue on the Trackable Entities GitHub repository.  You may also contact me directly.  Enjoy!

Tony Sneed
- Twitter: @tonysneed
- Email: tony@tonysneed.com
