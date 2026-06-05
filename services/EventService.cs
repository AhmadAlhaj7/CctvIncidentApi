

public class EventService
{
private readonly List<CameraEvent> _events = new List<CameraEvent>();

private int _nextId = 1;

public List<CameraEvent> GetAllEvents()
{
    return _events;

}

}