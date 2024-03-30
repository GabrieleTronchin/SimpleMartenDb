# SimpleMartenDb

A simple implementation of MartenDB. MartenDB is a .NET Transactional Document DB and Event Store on PostgreSQL.
For more information about MartenDB, visit [here](https://github.com/JasperFx/marten).

Upon starting the application, you will be greeted with a Swagger page, offering a user-friendly interface to explore and interact with the API endpoints effortlessly.
Here a sample:

![Swagger](assets/swagger.png)

This API project is structured to provide two families of endpoints: 
 - CRUD (Create, Read, Update, Delete) operations 
 - Events

The CRUD endpoints are crafted to demonstrate the utilization of MartenDB for performing standard CRUD operations.
These endpoints will illustrate how easy it is to create, read, update, and delete using MartenDB's features.

Additionally, the Events endpoints are specifically designed to showcase the power of MartenDB in implementing an EventSource approach. 
By leveraging MartenDB's event sourcing capabilities, these endpoints demonstrate how events can be captured and managed efficiently.

## Event Sourcing

Event Sourcing is a software design pattern that involves capturing all changes to an application's state as a sequence of events. 
Instead of storing the current state of an entity, Event Sourcing involves storing a log of events that describe the actions or commands that have been applied to that entity over time. 

These events are immutable, meaning they cannot be changed once they are recorded.

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


The following chapter will provide you with a deeper understanding of what you discovered within this project.


## MartenDb Setup

The initial step is to incorporate a Marten reference into your C# program. By utilizing options, you can seamlessly interact with Marten. A fundamental configuration involves setting up the connection string. 

This setting enables communication with the database and provides access to basic features.


```csharp
builder.Services.AddMarten(options =>
{
    const string connectionString = "host=localhost;port=5432;database=cars;username=sa;password=MySecretPassword1234;";
    options.Connection(connectionString);
});
```

## Database Interaction

Marten equips you with interfaces to communicate effectively with PostgreSQL. Two pivotal concepts are:

- IQuerySession: Primarily for read operations, it facilitates querying and mapping objects from PostgreSQL. For further details, refer to the documentation [here](https://martendb.io/documents/querying/linq/).

- IDocumentStore: Geared towards write operations, it provides functionalities for Add/Update/Delete, as well as for initializing Streams and managing events. To explore more, visit the documentation [here](https://martendb.io/documents/sessions.html).


## CRUD Endpoints


Here's an example of a simple interaction using IQuerySession. 

The following code retrieves a single CarEntity object saved in the database. As you can observe, it employs a straightforward LINQ-like approach

```csharp
app.MapGet("Get/{carId}", async (Guid carId, IQuerySession querySession) =>
{
    return await querySession.Query<CarEntity>().Where(x => x.Id == carId).FirstOrDefaultAsync();
}).WithOpenApi();
```

Here's an example of a basic interaction using IDocumentStore. The following code adds an object of CarEntity with default values.


```csharp
using var session = await store.LightweightSerializableSessionAsync();
session.Store(new CarEntity() { Id = carId, CurrentPosition = new Location(0, 0), InitialPosition = new Location(0, 0), Traveled = 0 });
await session.SaveChangesAsync();
```

For a more comprehensive example, please refer to the source code for CRUD endpoints.


## Events Endpoints

Here's an example illustrating how to initiate a stream and enqueue an event using Marten.

Firstly, you initiate a stream for a specific ID, in this case, carId. Then, you proceed to enqueue an event. It's essential to enqueue an event to initiate the stream.

Subsequently, you have the flexibility to enqueue as many events as you desire for that carId. 

It's worth noting that these events don't necessarily have to be of the same class.

StartStream Sample with an initial event:

```csharp
session.Events.StartStream(carId, new UpdateLocationRequest() { Latitute = 0, Longitude = 0 }); 
```

Once the stream is initiated, you can proceed to enqueue events.
Sample:

```csharp
 using var session = await store.LightweightSerializableSessionAsync();
 session.Events.Append(carId, new UpdateLocationRequest() { Latitute = 1, Longitude = 1 });
 await session.SaveChangesAsync();
```

The CarAggregateEntity includes a method named Apply, which handles a specific event class. Multiple Apply methods can be implemented to manage each event you wish to handle.


```csharp
public class CarAggregateEntity
{

    public Guid Id { get; set; }

    public Location? InitialPosition { get; set; }

    public Location CurrentPosition { get; set; }

    public int Traveled { get; set; }

    public void Apply(UpdateLocationRequest e)
    {
        if (InitialPosition == null)
            InitialPosition = new Location(e.Latitute, e.Longitude);

        CurrentPosition = new Location(e.Latitute, e.Longitude);
        Traveled += e.Longitude; //just a sample traveled calculation
    }
}
```

By employing the AggregateStreamAsync method on the CarAggregateEntity, all events stored in PostgreSQL will be retrieved, and the Apply method will be invoked.

This approach is commonly known as Live Aggregation because each call to AggregateStreamAsync results in the parsing of all events.

This method is well-suited for classes with fewer events. However, as the number of events increases, performance may degrade. 

Nonetheless, such an approach allows for the addition of data to your entity and the derivation of its state from a series of events.

```csharp
await querySession.Events.AggregateStreamAsync<CarAggregateEntity>(carId);
```

## Projections

Another crucial concept in event sourcing is projections. Projections follow a pattern akin to materialized views. 
The idea is to subscribe to certain events and generate a read-only entity based on the data derived from these events.

Projections are responsible for materializing views of the application's state based on the events in the event store. 
These views represent the current state of specific entities or aspects of the system.

Marten DB provides several methods to manage and interact with projections. 

Here's a brief explanation of Projection Lifecycles in Marten:

- Inline Projections: These are executed at the time of event capture within the same unit of work used to persist the projected documents.
- Live Aggregations: These are executed on demand by loading event data and creating the projected view in memory, without persisting the projected documents.
- Asynchronous Projections: These are executed by a background process, ensuring eventual consistency.

More info [here](https://martendb.io/events/projections/)


Here's an example of a projection, the class CurrentCarPositionEventProjection contains a method named "Create" which instructs Marten on how to manage the initial state of the car.

Upon inspection of the project, you'll notice that this projection is registered as asynchronous. 
A Marten daemon takes charge of creating and regularly updating it.

While this approach offers better performance, there's a trade-off in certainty regarding the real-time update of the current car position.

```csharp
public class CurrentCarPositionEventProjection : EventProjection
{
    public CurrentCarPositionEventProjection()
    {
        ProjectionName = "CarActualLocation";
    }


    public CurrentCarPosition Create(UpdateLocationRequest lastLocation, IEvent e)
    {
        return new CurrentCarPosition(e.Id, new Location(lastLocation.Longitude, lastLocation.Latitute));
    }
}
```


Here's another example showcasing the capabilities of projections. 
In this instance, the CarMaintenanceEventProjection is tasked with intercepting CarMaintenanceEvent and RemoveCarMaintenance events. 

Each time the projection receives an event, it executes a set of operations on the database synchronously.

```csharp
public class CarMaintenanceEventProjection : EventProjection
{

    public async Task Project(CarMaintenanceEvent evt, IDocumentOperations ops)line
    {
        var doc = await ops.LoadAsync<CarMaintenaceEntity>(evt.CarId);
        doc ??= new()
        {
            CarId = evt.CarId,
        };

        var todo = doc.MaintainacePlanTodo.SingleOrDefault(x => x.Id == evt.Id);
        if (todo == null)
        {
            doc.MaintainacePlanTodo.Add(new MaintenaceEntity
            {
                Id = evt.Id,
                Name = evt.Name,
                Description = evt.Description,
            });
        }
        else
        {
            todo.Checked = evt.Checked;
        }

        ops.Store(doc);
    }

    public async Task Project(RemoveCarMaintenance evt, IDocumentOperations ops)
    {
        var doc = await ops.LoadAsync<CarMaintenaceEntity>(evt.CarId);

        doc.MaintainacePlanTodo.RemoveAll(x => x.Id == evt.Id);

        ops.Store(doc);
    }

}
```

This is the required modification to the initial Marten registration to accommodate projections.

```csharp
builder.Services.AddMarten(options =>
{
    const string connectionString = "host=localhost;port=5432;database=cars;username=sa;password=MySecretPassword1234;";
    options.Connection(connectionString);
    options.Projections.Add(new CarMaintenanceEventProjection(), ProjectionLifecycle.Inline);
    options.Projections.Add(new CurrentCarPositionEventProjection(), ProjectionLifecycle.Async);
})
.AddAsyncDaemon(DaemonMode.Solo);
```
