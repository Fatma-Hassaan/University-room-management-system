public class StudentRoomsViewModel
{
    public List<CourseInfo> JTACourses { get; set; } = new();
    public List<CourseInfo> EnrolledCourses { get; set; } = new();
}

public class CourseInfo
{
    public string CourseCode { get; set; }
    public string CourseName { get; set; }
    public string RoomNumber { get; set; }
    public string RoomLocation { get; set; }
    public string TimeSlot { get; set; }
}