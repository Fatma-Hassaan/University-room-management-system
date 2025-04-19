CREATE DATABASE UniversityManagementSystem;
Go

USE UniversityManagementSystem;
Go


CREATE TABLE [User] (
    UserID INT,
    [Password] VARCHAR(255) NOT NULL,
    [Name] VARCHAR(100) NOT NULL,
    Email VARCHAR(100) UNIQUE NOT NULL,
    UserType VARCHAR(20) NOT NULL CHECK (UserType IN ('Registrar', 'Professor', 'RoomServicesMember','Student','TA','CleaningStaffMember')),
	PRIMARY KEY (UserID)
);

CREATE TABLE Registrar (
    ID INT,
	PRIMARY KEY (ID),
    FOREIGN KEY (ID) REFERENCES [User](UserID) ON DELETE CASCADE,
);

CREATE TABLE RoomServicesMember (
    ID INT,
	PRIMARY KEY (ID),
    FOREIGN KEY (ID) REFERENCES [User](UserID) ON DELETE CASCADE,
);
CREATE TABLE CleaningStaffMember (
    ID INT,
	PRIMARY KEY (ID),
    FOREIGN KEY (ID) REFERENCES [User](UserID) ON DELETE CASCADE,
);

CREATE TABLE Professor (
    ID INT,
	OfficeRoom VARCHAR(50) Not Null,
	PRIMARY KEY (ID),
    FOREIGN KEY (ID) REFERENCES [User](UserID) ON DELETE CASCADE,
);

CREATE TABLE TA (
    ID INT,
	PRIMARY KEY (ID),
    FOREIGN KEY (ID) REFERENCES [User](UserID) ON DELETE CASCADE,
    OfficeRoom VARCHAR(50) NOT NULL,
    Quota INT NOT NULL Default 20,
);

CREATE TABLE Student (
    ID INT,
	PRIMARY KEY (ID),
    FOREIGN KEY (ID) REFERENCES [User](UserID) ON DELETE CASCADE,
    Quota INT Default 3,
);

CREATE TABLE [Admin] (
    ID INT,
    [Password] VARCHAR(255) NOT NULL,
    [Name] VARCHAR(100) NOT NULL,
    Email VARCHAR(100) UNIQUE NOT NULL,
	PRIMARY KEY (ID)
);


CREATE TABLE Course (
    Code VARCHAR(25),
    [Name] VARCHAR(100) NOT NULL,
    ProfessorID INT Not Null,
    LectureRoom VARCHAR(50) Not Null,
    LectureDay VARCHAR(20) Not Null,
    LectureHour TIME Not Null,
    LectureDuration INT Not Null,
    TutorialRoom VARCHAR(50) Not Null,
    TutorialDay VARCHAR(20) Not Null,
    TutorialHour TIME Not Null,
    TutorialDuration INT Not Null,
	PRIMARY KEY (Code),
    FOREIGN KEY (ProfessorID) REFERENCES Professor(ID)
);

CREATE TABLE Course_Student (
    Student_ID INT,
    Course_Code VARCHAR(25),
    PRIMARY KEY (Student_ID, Course_Code),
    FOREIGN KEY (Student_ID) REFERENCES Student(ID),
    FOREIGN KEY (Course_Code) REFERENCES Course(Code)
);

CREATE TABLE JTA (
    Student_ID INT,
    Course_Code VARCHAR(25),
    PRIMARY KEY (Student_ID, Course_Code),
    FOREIGN KEY (Student_ID) REFERENCES Student(ID),
    FOREIGN KEY (Course_Code) REFERENCES Course(Code)
);

CREATE TABLE Course_TA (
    TA_ID INT,
    Course_Code VARCHAR(25),
    PRIMARY KEY (TA_ID, Course_Code),
    FOREIGN KEY (TA_ID) REFERENCES TA(ID),
    FOREIGN KEY (Course_Code) REFERENCES Course(Code)
);

CREATE TABLE Room (
    ID VARCHAR(50),
    Building VARCHAR(50) NOT NULL,
    [Floor] INT NOT NULL,
    [Zone] VARCHAR(50) Not Null,
    Number VARCHAR(20) NOT NULL,
    Capacity INT,
    AvailabilityStatus VARCHAR(20) CHECK (AvailabilityStatus IN ('Available', 'Closed')) Not Null Default 'Available',
    DailyCleaningStatus VARCHAR(20) CHECK (DailyCleaningStatus IN ('Done','In progress','Pending')) Not Null Default 'Pending',
	PRIMARY KEY (ID)
);

CREATE TABLE RequestOrReport (
    RID INT,
	UserID INT,
    Condition VARCHAR(20) CHECK (Condition IN ('Approved', 'Declined','Pending', 'Handled','In progress')),
    DayofR Date NOT NULL,
	HourofR TIME Not Null,
	DayofHandling Date NOT NULL,
	HourofHandling TIME Not Null,
    RType VARCHAR(20) NOT NULL CHECK (RType IN ('ClinicBookingRequest','RoomBooking', 'CleaningRequest', 'SuppliesRequest','RoomChangeRequest','AdditionalQuotaRequest','Report')),
	PRIMARY KEY (RID)
);

CREATE TABLE RoomBooking (
    ID INT,
    RoomID VARCHAR(50) Not Null,
    Reason TEXT,
    TimeSlotDay VARCHAR(20) Not Null,
    TimeSlotHour TIME Not Null,
    Duration INT Not Null,
    FOREIGN KEY (RoomID) REFERENCES Room(ID),
	PRIMARY KEY (ID),
    FOREIGN KEY (ID) REFERENCES RequestOrReport(RID) ON DELETE CASCADE,
);

CREATE TABLE ClinicBookingRequest (
    ID INT,
    RoomID VARCHAR(50) Not Null,
    Reason TEXT,
    TimeSlotDay VARCHAR(20) Not Null,
    TimeSlotHour TIME Not Null,
    Duration INT Not Null,
	CourseCode VARCHAR(25) Not Null,
    FOREIGN KEY (RoomID) REFERENCES Room(ID),
	FOREIGN KEY (CourseCode) REFERENCES Course(Code),
	PRIMARY KEY (ID),
    FOREIGN KEY (ID) REFERENCES RequestOrReport(RID) ON DELETE CASCADE,
);

CREATE TABLE Report (
	ID INT,
    RoomID VARCHAR(50) Not Null,
    Complaint TEXT Not Null,
    FOREIGN KEY (RoomID) REFERENCES Room(ID),
	PRIMARY KEY (ID),
    FOREIGN KEY (ID) REFERENCES RequestOrReport(RID) ON DELETE CASCADE,
);

CREATE TABLE AdditionalQuotaRequest (
    ID INT,
    RoomID VARCHAR(50) Not Null,
    NumOfExtraHours INT Not Null,
    FOREIGN KEY (RoomID) REFERENCES Room(ID),
	PRIMARY KEY (ID),
    FOREIGN KEY (ID) REFERENCES RequestOrReport(RID) ON DELETE CASCADE,
);

CREATE TABLE RoomChangeRequest (
	ID INT,
    NewRoomID VARCHAR(50),
	CourseCode VARCHAR(25) Not Null,
	LectureOrTutorial INT Not Null, -- 1 for Lecture, 2 for Tutorial
    FOREIGN KEY (NewRoomID) REFERENCES Room(ID),
	FOREIGN KEY (CourseCode) REFERENCES Course(Code),
	PRIMARY KEY (ID),
    FOREIGN KEY (ID) REFERENCES RequestOrReport(RID) ON DELETE CASCADE,
);

CREATE TABLE SuppliesRequest (
	ID INT,
    ExpectedDeliveryDate Date Not Null,
    Supplies TEXT Not Null,
	PRIMARY KEY (ID),
    FOREIGN KEY (ID) REFERENCES RequestOrReport(RID) ON DELETE CASCADE,
);

CREATE TABLE CleaningRequest (
	ID INT,
    RoomID VARCHAR(50),
    FOREIGN KEY (RoomID) REFERENCES Room(ID),
	PRIMARY KEY (ID),
    FOREIGN KEY (ID) REFERENCES RequestOrReport(RID) ON DELETE CASCADE,
);