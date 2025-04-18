public class TARoomBookingViewModel
{
    [Required]
    [Display(Name = "Room Code")]
    public string RoomCode { get; set; }

    [Required]
    [Display(Name = "Time Slot")]
    public string TimeSlot { get; set; }

    [Display(Name = "Tutorial Course")]
    public string SelectedTutorial { get; set; }

    [Range(1, 4)]
    [Display(Name = "Duration (hours)")]
    public int Duration { get; set; }

    public List<Tutorial> AvailableTutorials { get; set; }
    public int RemainingQuota { get; set; }
}