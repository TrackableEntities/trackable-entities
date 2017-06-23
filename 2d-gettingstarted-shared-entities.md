---
layout: default
title: Getting Started - Adding Trackable Entities to an existing project
home: true
---

This sums up the requirements a project has to comply to for **Trackable Entities** to work properly.

## Declaration
### 1. Server entities
 - Implement `ITrackable`
 - Implement `IMergeable`

### 2. Client entities
 - Inherit from `EntityBase`
 - All collection reference properties must be an instance of `ChangeTrackingCollection` (CTC).* All navigation properties must internally wrap the navigated property in a singular CTC as demonstrated [here](https://github.com/TrackableEntities/trackable-entities/blob/develop/Samples/VS2013/WebApiSample.Inheritance/SampleWebApi.Inheritance.Shared.Entities/Models/Product.cs#L83)

##Usage
### 1. Server (controller etc.)
 - Call `ApplyChanges` ex. method on provided entity argument
 - Call the `DbContext`'s `SaveChanges(Async)`
 - Call `LoadRelatedEntitiesAsync` ex. method to load updated stuff
 - Call `AcceptChanges` to flatten the changed data and mark it unchanged
 - Return a success code along with the updated entity  

### 2. Client
 - Wrap root entity/s in a CTC
 - If using JSON, serializing using `TypeNameHandling.Auto` in settings
 - Call the CTC's `GetChanges` method which returns a collection containing the root entity/s - if there are changes
 - Determine if the collection contains any elements, and send to server
 - Upon a successful result, call the local CTC's `MergeChanges` passing the server-returned updated entity