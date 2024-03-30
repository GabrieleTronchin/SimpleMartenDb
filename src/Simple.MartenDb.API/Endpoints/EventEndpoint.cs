using Marten;
using Microsoft.AspNetCore.Mvc;
using Simple.MartenDb.API.Entities;
using Simple.MartenDb.API.Models;
using Simple.MartenDb.API.Projections;


namespace Simple.MartenDb.API.Endpoints;

public static class EventEndpoint
{
    public static void AddEventsEnpoint(this IEndpointRouteBuilder app)
    {
        app.MapPost("CreateCar", async (IDocumentStore store) =>
        {
            Guid carId = Guid.NewGuid();

            using var session = await store.LightweightSerializableSessionAsync();
            session.Events.StartStream(carId, new UpdateLocationRequest() { Latitute = 0, Longitude = 0 }); // a stream cannot start without an event
            await session.SaveChangesAsync();

            return carId;
        });

        app.MapPut("UpdateLocation/{carId}", async ([FromBody] UpdateLocationRequest request, Guid carId, IDocumentStore store) =>
        {
            using var session = await store.LightweightSerializableSessionAsync();
            session.Events.Append(carId, request);
            await session.SaveChangesAsync();
        });

        app.MapGet("LiveAggregation/{carId}", async (Guid carId, IQuerySession querySession) =>
        {
            return await querySession.Events.AggregateStreamAsync<CarAggregateEntity>(carId);
        }).WithOpenApi();

        app.MapGet("CurrentPosition/{carId}", async (Guid carId, IQuerySession querySession) =>
        {
            return await querySession.LoadAsync<CurrentCarPosition>(carId);
        }).WithOpenApi();

        app.MapPut("CarMaintenance", async ([FromBody] CarMaintenanceEvent request, IDocumentStore store) =>
        {
            using var session = await store.LightweightSerializableSessionAsync();
            session.Events.Append(request.CarId, request);
            await session.SaveChangesAsync();
        });

        app.MapDelete("CarMaintenance", async ([FromBody] RemoveCarMaintenance request, IDocumentStore store) =>
        {
            using var session = await store.LightweightSerializableSessionAsync();
            session.Events.Append(request.CarId, request);
            await session.SaveChangesAsync();
        });

        app.MapGet("CarMaintenance/{carId}", async (Guid carId, IQuerySession querySession) =>
        {
            return await querySession.LoadAsync<CarMaintenaceEntity>(carId);
        }).WithOpenApi();
    }

}
