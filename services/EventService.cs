

public class EventService
{
private readonly List<CameraEvent> _events = new List<CameraEvent>();

private int _nextId = 1;


public CameraEvent Add(CameraEvent cameraEvent)
{
    cameraEvent.Id = _nextId++;
    _events.Add(cameraEvent);
    return cameraEvent;
}

public List<CameraEvent> GetAllEvents() // this method returns all the events that have been added to the service
{
    return _events;

}

}