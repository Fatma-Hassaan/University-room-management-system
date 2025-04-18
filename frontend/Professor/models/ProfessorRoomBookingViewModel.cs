public class ProfessorRoomBookingViewModel
{
    [Required]
    [Display(Name = "Room Code")]
    public string RoomCode { get; set; }

    [Required]
    [Display(Name = "Lecture Type")]
    public LectureType Type { get; set; }

    [Required]
    [Display(Name = "Time Slot")]
    public DateTime SelectedTime { get; set; }

    [Display(Name = "Recurring")]
    public string RecurringOption { get; set; }

    public List<DateTime> TimeSlots { get; set; }
    public List<string> RecurringOptions { get; set; }
    public List<ApprovalRequest> PendingRequests { get; set; }
}