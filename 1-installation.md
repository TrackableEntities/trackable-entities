---
layout: default
title: Installation
---

## Prerequisites
To get started building N-Tier apps using Trackable Entities, you'll need to install some prerequisites.

### Microsoft Visual Studio
- At a minimum you'll need the *free* **Community Edition** of Visual Studio [2013](https://www.visualstudio.com/en-us/products/visual-studio-community-vs.aspx) or [2015](https://www.visualstudio.com/en-us/downloads/visual-studio-2015-downloads-vs.aspx). Of course, the *paid* **Professional Edition** (or higher) is just fine.
- *Visual Studio 2012* will also work with Trackable Entities, but you won't get all the templates or the latest entity generators. So I encourage you to make the move to VS 2013 or higher, which allows you to target older versions of the .NET Framework.

### Entity Framework 6.x Tools for Visual Studio
You can use the [EF 6.x Tools for Visual Studio](https://www.microsoft.com/en-us/download/details.aspx?id=40762) to generate trackable entities with customizable T4 templates. Make sure to install the latest version.

- To use generate trackable entities using the EF Tools, simply *add a new item* to the project, click on the **Data** tab and select **ADO.NET Entity Data Model**.
- From there select or create a **Data Connection** to a database.
- Then select either **EF Designer from Database** or **Code First from Database**, and select the tables for which you want to generate entities.
- Keep in mind that the EF Tools for VS are only compatible with .NET 4.5 or higher. If you wish to create client entities in a Portable Class Library, you'll either need to create links to a .NET 4.5 Class Library project, or use the [Entity Framework Power Tools](http://visualstudiogallery.msdn.microsoft.com/72a60b14-1581-4b9b-89f2-846072eff19d).

### Entity Framework Power Tools (Optional)
If you are using code-based modeling (aka Code First) and would like to generate entities for a [Portable Class Library](https://msdn.microsoft.com/en-us/library/gg597391%28v=vs.110%29.aspx), you may find it more convenient to use the [Entity Framework Power Tools](http://visualstudiogallery.msdn.microsoft.com/72a60b14-1581-4b9b-89f2-846072eff19d).

- An advantage of the EF Power Tools is that they use the EF fluent mapping API, rather than data annotation attributes, some of which are non-portable.
- A disadvantage is that they generate entities for **all** the tables in a database and do not allow you to pick and choose which tables to add.  If you have numerable tables, this can take quite a while.
- We are considering support for additional code-generation tools in the future.

### Microsoft SQL Server Express (or Full)
Installing SQL Server Express (or Full) locally will make it more convenient to run the sample projects without altering the connection string.

- [SQL Server 2014 Express](https://www.microsoft.com/en-us/download/details.aspx?id=42299) with Tools is recommended.
- Select **SQLEXPRWT** (64 or 32 bit) to include SQL Management Studio and SQL Profiler.

### NorthwindSlim Database
The sample projects all use a scaled down version of the (infamous) Northwind sample database, which has been modified to fewer tables which reflect all relation types.

- Create a database called **NorthwindSlim**.  You can either use SQL Management Studio or Visual Studio for this purpose.
- Download the **zip file** which contains a SQL script for creating tables and populating them with data: [http://bit.ly/northwindslim](http://bit.ly/northwindslim).
- Execute the **SQL script** to create and populate the tables.

## Visual Studio Extensions
While it's possible to use just the Trackable Entities NuGet packages without Visual Studio integration, much of the *raw power* of Trackable Entities comes from the Visual Studio **project and item templates** that are added by the VSIX installer.  The easiest way to install the *Visual Studio extension* for **Trackable Entities** is to do it right from within Visual Studio.

![vs extension](images/te-vs-extension.png)

- Open Visual Studio, then select Tools, **Extensions and Updates**.
- Click the **Online** tab and search for **trackable**. Select **Trackable Entities for Visual Studio 2013 (or 2015)** and click Install.
- You can also download the VSIX installer directly from the online **Visual Studio Extensions Gallery** for VS [2012](https://visualstudiogallery.msdn.microsoft.com/e6754f27-894d-45c4-833c-57aaa3288a31), [2013](http://visualstudiogallery.msdn.microsoft.com/74e6d323-c827-48be-90da-703a9fa8f530) or [2015](https://visualstudiogallery.msdn.microsoft.com/1815bc2c-e2ee-4df7-866f-fb8c45987515).
- The VSIX installer is also available from the Trackable Entities [release page](https://github.com/tonysneed/trackable-entities/releases) on GitHub.

## NuGet Packages
Trackable Entities is implemented as a set of **NuGet packages** which provide *change tracking for entities across service boundaries*.  One of the nice things about the Visual Studio templates is that they install the NuGet packages automatically when selecting one of the Trackable Entities project templates.  But you can also install the packages independently.  The packages are also versioned independently, and you should update them to the latest versions as they become available.

- [TrackableEntities.Common](https://www.nuget.org/packages/TrackableEntities.Common): Common interfaces and types for **change tracking**	 on both the client and server.
- [TrackableEntities.Client](https://www.nuget.org/packages/TrackableEntities.Client): Classes for tracking changes on the client, including EntityBase and ChangeTrackingCollection<T>, which has GetChanges and MergeChanges methods.
- [TrackableEntities.EF.6](https://www.nuget.org/packages/TrackableEntities.EF.6): DbContext extension methods for applying change state to an EF context in a disconnected fashion and for loading related entities.  Methods are asynchronous and perform recursive graph traversal for applying changes to entities across all relation types (1-M, M-1, 1-1, M-M).
- [TrackableEntities.Patterns](https://www.nuget.org/packages/TrackableEntities.Patterns): Interfaces for **Repository and Unit of Work** patterns.
- [TrackableEntities.Patterns.EF.6](https://www.nuget.org/packages/TrackableEntities.Patterns.EF.6): A generic implementation of **Repository and Unit of Work** patterns for Entity Framework.
- [TrackableEntities.CodeTemplates](https://www.nuget.org/packages/TrackableEntities.CodeTemplates.Shared.Net45): Customizable **T4 code generation templates** for producing trackable entities from a database using either the EF 6.x Tools for Visual Studio (EDMX or code-based) or the Entity Framework Power Tools (code-based).
