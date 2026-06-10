public enum IncidentLevel
{
    Low,
    Medium,
    High,
    Critical
}

public class IncidentReport
{
    public int Id { get; set; }
    public IncidentLevel IncidentLevel { get; set; }
    public string Summary { get; set; } = string.Empty;
    public string Details { get; set; } = string.Empty;
    public string RecommendedAction { get; set; } = string.Empty;
    public List<int> InvolvedCameras { get; set; } = new List<int>();
    public string TimeRange { get; set; } = string.Empty;
    public DateTime GeneratedAt { get; set; } = DateTime.UtcNow;
}

