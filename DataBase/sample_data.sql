-- Sample Data for University Room Management System

-- 1. Insert Users
INSERT INTO [User] (UserID, Password, Name, Email, UserType) VALUES
(1001, 'pass1', 'John Doe', 'john@uni.edu', 'Student'),
(1002, 'pass2', 'Alice Smith', 'alice@uni.edu', 'TA'),
(1003, 'pass3', 'Prof. Ahmed', 'ahmed@uni.edu', 'Professor'),
(1004, 'pass4', 'Maria Garcia', 'maria@uni.edu', 'Registrar'),
(1005, 'pass5', 'Emma Wilson', 'emma@uni.edu', 'RoomServicesMember'),
(1006, 'pass6', 'Liam Brown', 'liam@uni.edu', 'CleaningStaffMember'),
(1007, 'pass7', 'Sophia Lee', 'sophia@uni.edu', 'Student'),
(1008, 'pass8', 'Noah Clark', 'noah@uni.edu', 'TA'),
(1009, 'pass9', 'Olivia Martinez', 'olivia@uni.edu', 'Professor'),
(1010, 'pass10', 'James Davis', 'james@uni.edu', 'Student'),
(1011, 'pass11', 'Admin1', 'admin1@uni.edu', 'Admin'),
(1012, 'pass12', 'Admin2', 'admin2@uni.edu', 'Admin');

-- 2. Insert Subtypes (Students, TAs, Professors)
INSERT INTO Student (ID, Quota) VALUES
(1001, 3), (1007, 3), (1010, 3);

INSERT INTO TA (ID, OfficeRoom, Quota) VALUES
(1002, 'A-101', 20), (1008, 'B-202', 20);

INSERT INTO Professor (ID, OfficeRoom) VALUES
(1003, 'C-301'), (1009, 'D-401');

-- 3. Insert Rooms
INSERT INTO Room (ID, Building, Floor, Zone, Number, Capacity, AvailabilityStatus) VALUES
('A-101', 'Building A', 1, 'North Wing', '101', 30, 'Available'),
('B-202', 'Building B', 2, 'South Wing', '202', 50, 'Available'),
('C-301', 'Building C', 3, 'East Wing', '301', 20, 'Closed'),
('D-401', 'Building D', 4, 'West Wing', '401', 40, 'Available');

-- 4. Insert Courses
INSERT INTO Course (Code, Name, ProfessorID, LectureRoom, LectureDay, LectureHour, LectureDuration) VALUES
('CIE101', 'Intro to Infrastructure', 1003, 'A-101', 'Monday', '10:00', 120),
('MATH202', 'Calculus II', 1009, 'B-202', 'Wednesday', '14:00', 90);

-- 5. Insert Relationships (Course-Student, JTA)
-- Enroll students in courses
INSERT INTO Course_Student (Student_ID, Course_Code) VALUES
(1001, 'CIE101'), (1007, 'MATH202');

-- Assign JTAs to courses
INSERT INTO JTA (Student_ID, Course_Code) VALUES
(1001, 'CIE101');

-- 6. Insert Requests/Reports
-- Room Booking Requests
INSERT INTO RequestOrReport (RID, UserID, Condition, DayofR, HourofR, RType) VALUES
(2001, 1001, 'Approved', '2023-10-10', '09:00', 'RoomBooking'),
(2002, 1002, 'Pending', '2023-10-11', '11:00', 'RoomBooking');

INSERT INTO RoomBooking (ID, RoomID, Reason, TimeSlotDay, TimeSlotHour, Duration) VALUES
(2001, 'A-101', 'Study Group', 'Monday', '09:00', 120),
(2002, 'B-202', 'TA Office Hours', 'Tuesday', '11:00', 60);

-- Reports
INSERT INTO Report (ID, RoomID, Complaint, IssueCategory, UrgencyLevel) VALUES
(2003, 'C-301', 'Broken projector', 'EquipmentFailure', 'High');

INSERT INTO RoomBooking (ID, RoomID, Reason, TimeSlotDay, TimeSlotHour, Duration) VALUES
(2001, 'A-101', 'Study Group', 'Monday', '09:00', 120);
-- ... Add bookings, reports, etc. ...