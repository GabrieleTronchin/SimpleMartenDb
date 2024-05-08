using Marten;
using Microsoft.AspNetCore.Mvc;
using Simple.MartenDb.API.Entities;
using Simple.MartenDb.API.Models;

namespace Simple.MartenDb.API.Endpoints;

public static class CrudEndpoint
{
    public static void AddCrudEnpoint(this IEndpointRouteBuilder app)
    {
        app.MapPost(
            "/Create",
            async (IDocumentStore store) =>
            {
                Guid carId = Guid.NewGuid();

                using var session = await store.LightweightSerializableSessionAsync();
                session.Store(
                    new CarEntity()
                    {
                        Id = carId,
                        CurrentPosition = new Location(0, 0),
                        InitialPosition = new Location(0, 0),
                        Traveled = 0
                    }
                );
                await session.SaveChangesAsync();

                return carId;
            }
        );

        app.MapPut(
            "/Update/{carId}",
            async (
                [FromBody] UpdateLocationRequest request,
                Guid carId,
                IQuerySession querySession,
                IDocumentStore store
            ) =>
            {
                var car = await querySession
                    .Query<CarEntity>()
                    .Where(x => x.Id == carId)
                    .FirstOrDefaultAsync();

                if (car == null)
                    return;

                car.CurrentPosition = new Location(request.Latitute, request.Longitude);
                car.Traveled += request.Longitude;

                using var session = await store.LightweightSerializableSessionAsync();
                session.Update(car);

                await session.SaveChangesAsync();
            }
        );

        app.MapGet(
                "Get/{carId}",
                async (Guid carId, IQuerySession querySession) =>
                {
                    return await querySession
                        .Query<CarEntity>()
                        .Where(x => x.Id == carId)
                        .FirstOrDefaultAsync();
                }
            )
            .WithOpenApi();
    }
}
