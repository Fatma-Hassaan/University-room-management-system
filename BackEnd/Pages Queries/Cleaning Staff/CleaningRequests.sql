         SELECT 
            cr.ID, 
            cr.RoomID, 
            u.Name AS RequestorName,
            rr.DayofR AS RequestDate,
            rr.HourofR AS RequestTime,
            rr.Condition
        FROM CleaningRequest cr
        JOIN RequestOrReport rr ON cr.ID = rr.RID
        JOIN [User] u ON rr.UserID = u.UserID
        WHERE rr.Condition IN ('Pending', 'In Progress')


        -- Step 1: Update the Condition to 'Done'
        UPDATE RequestOrReport
        SET Condition = 'Done', 
            DayofHandling = CAST(GETDATE() AS DATE), 
            HourofHandling = CAST(GETDATE() AS TIME)
        WHERE RID = @RID AND RType = 'CleaningRequest';
        
        -- Step 2: Delete the request after marking it as done
        DELETE FROM RequestOrReport
        WHERE RID = @RID AND RType = 'CleaningRequest';
