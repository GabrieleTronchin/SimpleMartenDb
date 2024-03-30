# SimpleMartenDb
A simple martendb implementation. Marten Db is a .NET Transactional Document DB and Event Store on PostgreSQL.

Some more info about MartenDB:

'The Marten library provides .NET developers with the ability to use the proven PostgreSQL database engine and its fantastic JSON support as a fully fledged document database. The Marten team believes that a document database has far reaching benefits for developer productivity over relational databases with or without an ORM tool.'

For more detail [here](https://github.com/JasperFx/marten)


## Event Sourcing

The Event Store pattern is a method used in software architecture to manage the state of an application by recording the full history of events that have occurred. Instead of storing the current state of an object or system, the Event Store maintains a log of all changes or events that have happened over time. These events represent actions or occurrences that have affected the system's state.


Event Sourcing is a software design pattern that involves capturing all changes to an application's state as a sequence of events. Instead of storing the current state of an entity, Event Sourcing involves storing a log of events that describe the actions or commands that have been applied to that entity over time. These events are immutable, meaning they cannot be changed once they are recorded.

https://learn.microsoft.com/en-us/azure/architecture/patterns/event-sourcing


## Prerequirement for test 

### How to setup Postgress

if you don't have postgress installaed you can use the "docker-compose.yml" file inside docker folder.

Docker compose file will configure:
1. an istance of postgres rechble on port 5432
2. an istance of pgadmin rechble on port 5050

You can configure you user and pass editring file ".env" inside folder "docker/config/.env"

### Create DB on Postgress using PGAdmin

Open admin page

![PGAdmin](assets/pgAdmin.png)

Create DB

![CreateDb](assets/createDb.png)

# Code Explaination

![Swagger](assets/swagger.png)

## MartenDb Setup


builder.Services.AddMarten(options =>
{
    const string connectionString = "host=localhost;port=5432;database=cars;username=sa;password=MySecretPassword1234;";
    options.Connection(connectionString);
});


## Projects Endpoint


### CRUD

### Events

### Projections

builder.Services.AddMarten(options =>
{
    const string connectionString = "host=localhost;port=5432;database=cars;username=sa;password=MySecretPassword1234;";
    options.Connection(connectionString);
    options.Projections.Add(new CarMaintenanceEventProjection(), ProjectionLifecycle.Async);
    options.Projections.Add(new CurrentCarPositionEventProjection(), ProjectionLifecycle.Async);
})
// Turn on the async daemon in "Solo" mode
.AddAsyncDaemon(DaemonMode.Solo);