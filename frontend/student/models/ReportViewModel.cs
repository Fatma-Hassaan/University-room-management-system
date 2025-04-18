public class ReportViewModel
{
    [Required]
    [Display(Name = "Room Code")]
    public string RoomCode { get; set; }

    [Required]
    [Display(Name = "Complaint Details")]
    [StringLength(500, MinimumLength = 10)]
    public string Complaint { get; set; }
}