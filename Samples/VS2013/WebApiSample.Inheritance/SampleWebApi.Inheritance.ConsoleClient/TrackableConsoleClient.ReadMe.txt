Trackable Console Web API Client ReadMe

This is a client Console application which uses HttpClient
to communicate with the Web API service.  It references the
Client.Entities project uses the the TrackableEntities.Client
NuGet package to perform change-tracking on the client without
any knowledge of how the service will persist changes.

To start change-tracking, a ChangeTrackingCollection<T> is
created, passing one or more entities to the constructor.
Inserts, updates and deletes are automatically tracked.
Calling GetChanges returns only the items that have been
modified, which can then be passes to the service PUT
operation, where all changes are persisted in one round trip
and in a single transaction.

