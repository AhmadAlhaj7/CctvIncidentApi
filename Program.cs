var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton<EventService>(); // this is for dependency injection of the event service which is used by the event controller
builder.Services.AddEndpointsApiExplorer();// this is for endpoint explorer which is used by swagger ui
builder.Services.AddSwaggerGen();// this is for swagger ui
builder.Services.AddScoped<AiAnalysisService>();

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

app.MapPost("/analyze", async (AiAnalysisService analysisService, EventService eventService) =>
{
    var events = eventService.GetAllEvents();

    if (events.Count == 0)
    {
        return Results.BadRequest("No events to analyze. Add events first.");
    }

    var report = await analysisService.AnalyzeEvents(events);
    return Results.Ok(report);
});

app.Run();