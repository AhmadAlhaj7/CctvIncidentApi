var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton<EventService>(); // this is for dependency injection of the event service which is used by the event controller
builder.Services.AddEndpointsApiExplorer();// this is for endpoint explorer which is used by swagger ui
builder.Services.AddSwaggerGen();// this is for swagger ui

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapPost("/events", (CameraEvent cameraEvent, EventService eventService) =>
{
    var created = eventService.Add(cameraEvent);
    return Results.Created($"/events/{created.Id}", created);
});

app.MapGet("/events", (EventService eventService) =>
{
    return eventService.GetAllEvents();
});

app.MapPost("/analyze", (EventService eventService) =>
{
    var events = eventService.GetAllEvents();
    return Results.Ok("Analyzing");
});

app.Run();