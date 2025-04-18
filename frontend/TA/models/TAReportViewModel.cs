public class TAReportViewModel
{
    [Required]
    [Display(Name = "Room Code")]
    public string RoomCode { get; set; }

    [Required]
    [StringLength(500)]
    [Display(Name = "Issue Description")]
    public string Description { get; set; }

    [Required]
    [Display(Name = "Priority Level")]
    public PriorityLevel Priority { get; set; }
}

public enum PriorityLevel
{
    Low,
    Medium,
    High
}