using Marten;
using Marten.Events.Daemon.Resiliency;
using Marten.Events.Projections;
using Simple.MartenDb.API.Endpoints;
using Simple.MartenDb.API.Entities;
using Simple.MartenDb.API.Projections;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddMarten(options =>
{
    const string connectionString = "host=localhost;port=5432;database=cars;username=sa;password=MySecretPassword1234;";
    options.Connection(connectionString);
    options.Projections.Add(new CarMaintenanceEventProjection(), ProjectionLifecycle.Async);
    options.Projections.Add(new CurrentCarPositionEventProjection(), ProjectionLifecycle.Async);
})
// Turn on the async daemon in "Solo" mode
.AddAsyncDaemon(DaemonMode.Solo);


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

var events = app.MapGroup("/events");
events.AddEventsEnpoint();

var crud = app.MapGroup("/crud");
crud.AddCrudEnpoint();

app.Run();
