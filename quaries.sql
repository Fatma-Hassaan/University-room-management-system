
DECLARE @UserID            INT            = 2022588,
        @Password NVARCHAR(255)  = , --here i will add the password and all the info --
        @UserName          NVARCHAR(100)  = ,
        @UserEmail         NVARCHAR(100)  = ,
        @UserType          VARCHAR(20)    = ,      -- or 'Professor', 'TA', …
        @OfficeRoom        VARCHAR(50)    = NULL,           -- only for staff roles
        @InitialQuota      INT            = NULL;           -- optional, only for Student


BEGIN TRAN;

INSERT INTO [User] (UserID,[Password],[Name],Email,UserType)
VALUES( @UserID,
        @Password,  -- always hash the pass word with unique one 
        @UserName,
        @UserEmail,
        @UserType );


IF      @UserType = 'Professor'
    INSERT INTO Professor (ID,OfficeRoom)
    VALUES (@UserID, @OfficeRoom);

ELSE IF @UserType = 'Student'
    INSERT INTO Student (ID,Quota)
    VALUES (@UserID, ISNULL(@InitialQuota,3));

ELSE IF @UserType = 'TA'
    INSERT INTO TA (ID,OfficeRoom,Quota)
    VALUES (@UserID, @OfficeRoom, 20);



COMMIT;


----------------------------------------------------------------------------------------------
--Supplies-Request page--

DECLARE @NewRID            INT            =  ,
        @ExpectedDelivery Date            =  , 
        @SuppliesText          NVARCHAR(100)  =  ,
		@UserID INT =                            ;

	



BEGIN TRAN;

DECLARE @NewRID INT;

INSERT INTO RequestOrReport
      (UserID,      Condition,  DayofR,         HourofR,        RType)
VALUES(@UserID,     'Pending',  CAST(GETDATE() AS date), CAST(GETDATE() AS time), 'SuppliesRequest');

SET @NewRID = SCOPE_IDENTITY();   -- matches the PK of the child row

/* 2. insert the details */
INSERT INTO SuppliesRequest (ID, ExpectedDeliveryDate, Supplies)
VALUES (@NewRID, @ExpectedDelivery, @SuppliesText);

COMMIT;


------------------------
--SHOW QUOTA

SELECT Quota
FROM   Student
WHERE  ID = @StudentID;

---------------------------------------------------
-- Submit a request for more hours


DECLARE @RID            INT            =  ,
        @ExpectedDelivery Date            =  , 
		@UserID INT =                            ,
		@RoomID INT = ,
		@ExtraHours INT = ;
BEGIN TRAN;



INSERT INTO RequestOrReport
      (UserID,  Condition,  DayofR,         HourofR,        RType)
VALUES(@UserID,'Pending',CAST(GETDATE() AS date), CAST(GETDATE() AS time), 'AdditionalQuotaRequest');

SET @RID = SCOPE_IDENTITY();


INSERT INTO AdditionalQuotaRequest (ID, RoomID, NumOfExtraHours)
VALUES (@RID, @RoomID, @ExtraHours);

COMMIT;
----------------------------------------------------
------ ststitics ----ADMIN
SELECT
  (SELECT COUNT(*) FROM Professor)                               AS NumProfessors,
  (SELECT COUNT(*) FROM TA)                                      AS NumTAs,
  (SELECT COUNT(*) FROM Student)                                 AS NumStudents,
  (SELECT COUNT(*) FROM RequestOrReport
    WHERE RType IN ('RoomBookingRequest','ClinicBookingRequest')
      AND Condition = 'Handled')                                 AS HandledBookingReqs,
  (SELECT COUNT(*) FROM RequestOrReport
    WHERE RType = 'Report'
      AND Condition = 'Handled')                                 AS HandledReports;
------------------------------------------------------------------
------ Room-Booking ------
