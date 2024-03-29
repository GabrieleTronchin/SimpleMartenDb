using Marten;
using Microsoft.AspNetCore.Mvc;
using Simple.MartenDb.API.Entities;
using Simple.MartenDb.API.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddMarten(options =>
{
    const string connectionString = "host=localhost;port=5432;database=cars;username=sa;password=MySecretPassword1234;";
    options.Connection(connectionString);
});


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}



app.MapPost("/CreateCar", async (IDocumentStore store) =>
{
    Guid carId = Guid.NewGuid();

    using var session = await store.LightweightSerializableSessionAsync();
    session.Events.StartStream(carId, new UpdateLocationRequest() { Latitute = 0, Longitude = 0 }); // a stream cannot start without an event
    await session.SaveChangesAsync();

    return carId;
});

app.MapPut("/UpdateLocation/{carId}", async ([FromBody] UpdateLocationRequest request, Guid carId, IDocumentStore store) =>
{
    using var session = await store.LightweightSerializableSessionAsync();
    session.Events.Append(carId, request);
    await session.SaveChangesAsync();
});


app.MapGet("LiveAggregation/{carId}", async (Guid carId, IQuerySession querySession) =>
{
    return await querySession.Events.AggregateStreamAsync<CarAggregateEntity>(carId);
}).WithOpenApi();

app.Run();
