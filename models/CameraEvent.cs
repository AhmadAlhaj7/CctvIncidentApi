public enum DetectedObject
{
    MalePerson,
    FemalePerson,
    vehicle,
    Animal,
    Unknown
}


public enum SeverityLevel
{
    Low,
    Medium,
    High
}


public class CameraEvent
{
    public int Id { get; set; }
    public int CameraId { get; set; }
    public String EventType { get; set; } = string.Empty;
    public string Zone { get; set; } = string.Empty;
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    public SeverityLevel Severity { get; set; } = SeverityLevel.Low;
    public DetectedObject DetectedObject { get; set; } = DetectedObject.Unknown;
}