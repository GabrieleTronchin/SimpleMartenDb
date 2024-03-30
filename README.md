# SimpleMartenDb

A simple implementation of MartenDB. MartenDB is a .NET Transactional Document DB and Event Store on PostgreSQL.

For more information about MartenDB, visit [here](https://github.com/JasperFx/marten).

## Event Sourcing

The Event Store pattern is a method used in software architecture to manage the state of an application by recording the full history of events that have occurred. Instead of storing the current state of an object or system, the Event Store maintains a log of all changes or events that have happened over time. These events represent actions or occurrences that have affected the system's state.

Event Sourcing is a software design pattern that involves capturing all changes to an application's state as a sequence of events. Instead of storing the current state of an entity, Event Sourcing involves storing a log of events that describe the actions or commands that have been applied to that entity over time. These events are immutable, meaning they cannot be changed once they are recorded.

For more details, refer to [Microsoft's documentation on Event Sourcing](https://learn.microsoft.com/en-us/azure/architecture/patterns/event-sourcing).

## Prerequisites for Testing

### How to Set up PostgreSQL

If you don't have PostgreSQL installed, you can use the "docker-compose.yml" file inside the docker folder.

The Docker Compose file will configure:
1. An instance of PostgreSQL reachable on port 5432
2. An instance of pgAdmin reachable on port 5050

You can configure your username and password by editing the ".env" file inside the "docker/config/" folder.

### Create DB on Postgress using PGAdmin

Open the admin page and follow the steps to create a database.

![PGAdmin](assets/pgAdmin.png)

![CreateDb](assets/createDb.png)

# Code Explaination

![Swagger](assets/swagger.png)

## MartenDb Setup

```csharp
builder.Services.AddMarten(options =>
{
    const string connectionString = "host=localhost;port=5432;database=cars;username=sa;password=MySecretPassword1234;";
    options.Connection(connectionString);
});
```

## Projects Endpoint

//TODO Add small description

### CRUD

//TODO Add crud enpoint explaination

### Events

//TODO Add events enpoint explaination


### Projections

How to setup projection on mattern:

```csharp
builder.Services.AddMarten(options =>
{
    const string connectionString = "host=localhost;port=5432;database=cars;username=sa;password=MySecretPassword1234;";
    options.Connection(connectionString);
    options.Projections.Add(new CarMaintenanceEventProjection(), ProjectionLifecycle.Async);
    options.Projections.Add(new CurrentCarPositionEventProjection(), ProjectionLifecycle.Async);
})
.AddAsyncDaemon(DaemonMode.Solo);
```

//TODO Add explaination about AddAsyncDaemon

//TODO Add sample of projection flow


