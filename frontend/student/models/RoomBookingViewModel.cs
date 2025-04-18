public class RoomBookingViewModel
{
    [Required]
    [Display(Name = "Room Code")]
    public string RoomCode { get; set; }

    [Required]
    public string Reason { get; set; }

    [Required]
    [Display(Name = "Time Slot")]
    public string TimeSlot { get; set; }

    [Display(Name = "For a Clinic?")]
    public bool IsClinic { get; set; }

    [Required]
    [Display(Name = "Course")]
    public string SelectedCourse { get; set; }
}