public class ProfessorReportViewModel
{
    [Required]
    [Display(Name = "Room Code")]
    public string RoomCode { get; set; }

    [Required]
    [Display(Name = "Issue Description")]
    [StringLength(500, MinimumLength = 20)]
    public string Description { get; set; }

    [Required]
    [Display(Name = "Issue Category")]
    public ReportCategory Category { get; set; }

    [Display(Name = "Urgency Level")]
    public UrgencyLevel Urgency { get; set; } = UrgencyLevel.Medium;
}

public enum ReportCategory
{
    EquipmentFailure,
    SafetyHazard,
    MaintenanceRequired,
    CleaningNeeded,
    Other
}

public enum UrgencyLevel
{
    Low,
    Medium,
    High,
    Critical
}