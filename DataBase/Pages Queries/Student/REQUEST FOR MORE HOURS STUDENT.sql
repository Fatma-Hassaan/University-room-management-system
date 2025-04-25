
DECLARE @RID            INT            =  ,
        @ExpectedDelivery Date            =  , 
		@UserID INT =                            ,
		@RoomID INT = ,
		@ExtraHours INT = ;




INSERT INTO RequestOrReport
      (UserID,  Condition,  DayofR,         HourofR,        RType)
VALUES(@UserID,'Pending',CAST(GETDATE() AS date), CAST(GETDATE() AS time), 'AdditionalQuotaRequest');

SET @RID = SCOPE_IDENTITY();


INSERT INTO AdditionalQuotaRequest (ID, RoomID, NumOfExtraHours)
VALUES (@RID, @RoomID, @ExtraHours);

