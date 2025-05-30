﻿using System.Collections.Generic;
using System.Data;
using System.Drawing;
using Microsoft.Data.SqlClient;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace Project.Models
{
    public class DB
    {
        private string ConnectionString = "Data Source= DESKTOP-042E9O7; Initial Catalog= UniversityManagementSystem; Integrated Security= True; Trust Server Certificate= True;";

        private SqlConnection con { get; set; }

        public DB()
        {
            con = new SqlConnection(ConnectionString);
        }
         public DataTable LoadRoomAvailabilityStatuses()
        {
            DataTable dt = new DataTable();
            string query = "SELECT ID AS RoomID, AvailabilityStatus FROM Room";
            SqlCommand cmd = new SqlCommand(query, con);

            try
            {
                con.Open();
                dt.Load(cmd.ExecuteReader());
            }
            finally
            {
                con.Close();
            }
            return dt;
        }

        public void ToggleRoomAvailability(string roomId)
        {
            string selectQuery = "SELECT AvailabilityStatus FROM Room WHERE ID = @RoomID";
            string currentStatus = "";

            using (SqlCommand selectCmd = new SqlCommand(selectQuery, con))
            {
                selectCmd.Parameters.AddWithValue("@RoomID", roomId);
                try
                {
                    con.Open();
                    var result = selectCmd.ExecuteScalar();
                    if (result != null)
                    {
                        currentStatus = result.ToString();
                    }
                }
                finally
                {
                    con.Close();
                }
            }

       public DataTable LoadAllCleaningRequestsForRoomServices()
        {
            DataTable dt = new DataTable();

            string query = @"
              SELECT 
                rr.RID,
                cr.RoomID,
                rr.Condition,
                rr.DayofR AS RequestDate,
                rr.HourofR AS RequestTime,
                u.Name AS RequestorName
            FROM CleaningRequest cr
            JOIN RequestOrReport rr ON cr.ID = rr.RID
            JOIN [User] u ON rr.UserID = u.UserID
            WHERE rr.RType = 'CleaningRequest'
            ORDER BY rr.DayofR DESC, rr.HourofR DESC;
            
            ";

            using (SqlCommand cmd = new SqlCommand(query, con))
            {
                try
                {
                    con.Open();
                    dt.Load(cmd.ExecuteReader());
                }
                finally
                {
                    con.Close();
                }
            }

            return dt;
        }


      public void InsertCleaningRequest(int userId, string roomId)
        {
            string insertMain = @"
        INSERT INTO RequestOrReport (RID, UserID, Condition, DayofR, HourofR, RType)
        VALUES ((SELECT ISNULL(MAX(RID), 0) + 1 FROM RequestOrReport), @UserID, 'Pending', CAST(GETDATE() AS DATE), CAST(GETDATE() AS TIME), 'CleaningRequest');
        DECLARE @NewRID INT = (SELECT MAX(RID) FROM RequestOrReport);
        INSERT INTO CleaningRequest (ID, RoomID) VALUES (@NewRID, @RoomID);
    ";

            using (SqlCommand cmd = new SqlCommand(insertMain, con))
            {
                cmd.Parameters.AddWithValue("@UserID", userId);
                cmd.Parameters.AddWithValue("@RoomID", roomId);
                try
                {
                    con.Open();
                    cmd.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    Console.WriteLine("InsertCleaningRequest Error: " + ex.Message);
                }
                finally
                {
                    con.Close();
                }
            }
        }



        public List<string> GetAvailableRoomIDs()
        {
            var list = new List<string>();
            string query = "SELECT ID FROM Room"; 
            SqlCommand cmd = new SqlCommand(query, con);
            try
            {
                con.Open();
                var reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    list.Add(reader["ID"].ToString());
                }
                reader.Close();
            }
            finally
            {
                con.Close();
            }
            return list;
        }

        public DataTable LoadRoomCleaningStatus()
        {
            DataTable dt = new DataTable();
            string query = "SELECT RoomID, Condition FROM CleaningRequest";

            SqlCommand cmd = new SqlCommand(query, con);
            try
            {
                con.Open();
                dt.Load(cmd.ExecuteReader());
            }
            catch (Exception ex)
            {
                // Log error 
            }
            finally
            {
                con.Close();
            }
            return dt;
        }

               public DataTable LoadQuotaRequests()
        {
            DataTable dt = new DataTable();
            string query = @"
        SELECT 
            aqr.ID,
            rr.UserID,
            u.UserType,
            aqr.NumOfExtraHours AS ExtraHours,
            aqr.Reason,
            rr.Condition AS Status
        FROM AdditionalQuotaRequest aqr
        JOIN RequestOrReport rr ON aqr.ID = rr.RID
        JOIN [User] u ON rr.UserID = u.UserID
        WHERE rr.Condition = 'Pending' AND rr.RType = 'AdditionalQuotaRequest'
        ORDER BY rr.DayofR DESC, rr.HourofR DESC;
    ";

            SqlCommand cmd = new SqlCommand(query, con);
            try
            {
                con.Open();
                dt.Load(cmd.ExecuteReader());
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error loading quota requests: " + ex.Message);
            }
            finally
            {
                con.Close();
            }

            return dt;
        }

        public void UpdateQuotaStatus(int requestId, string newStatus)
        {
            string query = @"
        UPDATE RequestOrReport
        SET Condition = @Status
        WHERE RID = @RID AND RType = 'AdditionalQuotaRequest';
    ";

            using (SqlCommand cmd = new SqlCommand(query, con))
            {
                cmd.Parameters.AddWithValue("@Status", newStatus);
                cmd.Parameters.AddWithValue("@RID", requestId);

                try
                {
                    con.Open();
                    cmd.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error updating quota status: " + ex.Message);
                }
                finally
                {
                    con.Close();
                }
            }
        }
           public List<string> GetAvailableRoomIDs()
        {
            var list = new List<string>();
            string query = "SELECT ID FROM Room"; // Adjust if needed
            SqlCommand cmd = new SqlCommand(query, con);
            try
            {
                con.Open();
                var reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    list.Add(reader["ID"].ToString());
                }
                reader.Close();
            }
            finally
            {
                con.Close();
            }
            return list;
        }


     

         public void UpdateReportCondition(int reportId, string status)
        {
            string query = @"
        UPDATE RequestOrReport
        SET Condition = @Condition
        WHERE RID = @RID AND RType = 'Report';
    ";

            using (SqlConnection con = new SqlConnection(ConnectionString))
            using (SqlCommand cmd = new SqlCommand(query, con))
            {
                cmd.Parameters.AddWithValue("@Condition", status);
                cmd.Parameters.AddWithValue("@RID", reportId);

                try
                {
                    con.Open();
                    cmd.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error updating report condition: " + ex.Message);
                    // Optional: throw or log
                }
            }
        }
              public DataTable LoadAllReports()
        {
            DataTable dt = new DataTable();
            string query = @"
        SELECT 
            r.ID,
            cr.RoomID, 
            rr.Condition,
            rr.DayofR AS RequestDate, 
            rr.HourofR AS RequestTime,
            u.Name AS RequestorName,
            r.Complaint  -- Complaint field from the Report table
        FROM Report cr
        JOIN RequestOrReport rr ON cr.ID = rr.RID
        JOIN [User] u ON rr.UserID = u.UserID
        LEFT JOIN Report r ON cr.RoomID = r.RoomID  -- Use RoomID for the join with the Report table
        WHERE rr.Condition IN ('Pending', 'In Progress') 
            AND rr.RType = 'Report'
        ORDER BY rr.DayofR DESC, rr.HourofR DESC;
    ";

            SqlCommand cmd = new SqlCommand(query, con);
            try
            {
                con.Open();
                dt.Load(cmd.ExecuteReader());
            }
            catch (Exception ex)
            {
                // Optionally log the error
                Console.WriteLine("Error: " + ex.Message);
            }
            finally
            {
                con.Close();
            }
            return dt;
        }
          public DataTable LoadCleaningRequestsForStaff()
        {
            string query = @"
        SELECT 
            rr.RID AS ID,
            cr.RoomID,
            u.Name AS RequestorName,
            rr.DayofR AS RequestDate,
            rr.HourofR AS RequestTime,
            rr.Condition
        FROM RequestOrReport rr
        JOIN CleaningRequest cr ON rr.RID = cr.ID
        JOIN [User] u ON rr.UserID = u.UserID
        WHERE rr.Condition IN ('Pending', 'In progress')
        ORDER BY rr.DayofR DESC, rr.HourofR DESC;";

            SqlCommand cmd = new SqlCommand(query, con);
            DataTable dt = new DataTable();

            try
            {
                con.Open();
                dt.Load(cmd.ExecuteReader());
            }
            finally
            {
                con.Close();
            }

            return dt;
        }
          public DataTable LoadDailyCleaningStatuses(DateTime? dateFilter = null)
        {
            DataTable dt = new DataTable();

            string query = @"
              SELECT
            cr.RoomID,
            rr.Condition
        FROM CleaningRequest cr
        JOIN RequestOrReport rr ON cr.ID = rr.RID
        WHERE rr.RType = 'CleaningRequest'
          AND rr.Condition IN ('Pending', 'In progress')
          AND   rr.DayofR = @SelectedDate
        ORDER BY cr.RoomID;
            ";

            SqlCommand cmd = new SqlCommand(query, con);
            cmd.Parameters.AddWithValue("@SelectedDate", (object?)dateFilter ?? DBNull.Value);

            try
            {
                con.Open();
                dt.Load(cmd.ExecuteReader());
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error loading filtered statuses: " + ex.Message);
            }
            finally
            {
                con.Close();
            }

            return dt;
        }

        public void UpdateCleaningRequestStatus(int requestId, string status)
        {
            string query = @"
        UPDATE RequestOrReport
        SET Condition = @Condition,
            DayofHandling = CAST(GETDATE() AS DATE),
            HourofHandling = CAST(GETDATE() AS TIME)
        WHERE RID = @RID AND RType = 'CleaningRequest';";

            SqlCommand cmd = new SqlCommand(query, con);
            cmd.Parameters.AddWithValue("@RID", requestId);
            cmd.Parameters.AddWithValue("@Condition", status);

            try
            {
                con.Open();
                cmd.ExecuteNonQuery();
            }
            finally
            {
                con.Close();
            }
        }
        
     public void UpdateDailyCleaningStatus(string roomId, string condition)
        {
            string query = @"
        UPDATE rr
        SET rr.Condition = @Condition,
            rr.DayofHandling = CAST(GETDATE() AS DATE),
            rr.HourofHandling = CAST(GETDATE() AS TIME)
        FROM RequestOrReport rr
        JOIN CleaningRequest cr ON rr.RID = cr.ID
        WHERE cr.RoomID = @RoomID";

            SqlCommand cmd = new SqlCommand(query, con);
            cmd.Parameters.AddWithValue("@Condition", condition);
            cmd.Parameters.AddWithValue("@RoomID", roomId);

            try
            {
                con.Open();
                cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error updating status: " + ex.Message);
            }
            finally
            {
                con.Close();
            }
        }

        public DataTable LoadCleaningRequests()
        {
            DataTable dt = new DataTable();
            string query = @"SELECT rr.ID, rr.RoomID, rr.Condition, rr.RequestTime, rr.RequestDate, u.Name AS RequestedBy
                     FROM RequestOrReport rr
                     JOIN [User] u ON rr.UserID = u.UserID
                     WHERE rr.Type = 'Cleaning' AND rr.Condition IN ('Pending', 'In progress')
                     ORDER BY rr.RequestDate DESC, rr.RequestTime DESC";

            SqlCommand cmd = new SqlCommand(query, con);

            try
            {
                con.Open();
                dt.Load(cmd.ExecuteReader());
            }
            catch (Exception ex)
            {
                // Optionally log or handle
            }
            finally
            {
                con.Close();
            }

            return dt;
        }
        public void MarkCleaningRequestAsHandled(int requestId)
        {
            string query = @"UPDATE RequestOrReport
                     SET Condition = 'Handled',
                         DayofHandling = CAST(GETDATE() AS DATE),
                         HourofHandling = CAST(GETDATE() AS TIME)
                     WHERE ID = @RequestID";

            SqlCommand cmd = new SqlCommand(query, con);
            cmd.Parameters.AddWithValue("@RequestID", requestId);

            try
            {
                con.Open();
                cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                // Optionally log or handle
            }
            finally
            {
                con.Close();
            }
        }

           public DataTable LoadDailyCleaningForRoomServices(DateTime date)
        {
            DataTable dt = new DataTable();
            string query = @"
        SELECT 
            cr.RoomID, 
            rr.Condition, 
            rr.DayofR AS RequestDate
        FROM CleaningRequest cr
        JOIN RequestOrReport rr ON cr.ID = rr.RID
        WHERE rr.RType = 'CleaningRequest'
        AND rr.DayofR = @Date";

            using (SqlCommand cmd = new SqlCommand(query, con))
            {
                cmd.Parameters.AddWithValue("@Date", date.Date);
                try
                {
                    con.Open();
                    dt.Load(cmd.ExecuteReader());
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error: " + ex.Message);
                }
                finally
                {
                    con.Close();
                }
            }
            return dt;
        }


        public string GetUserType(string Email)
        {
            string UserType = "";

            string query = "SELECT UserType FROM [User] WHERE Email = @Email\r\nUNION\r\nSELECT UserType FROM [Admin] WHERE Email =@Email";
            SqlCommand cmd = new SqlCommand(query, con);
            cmd.Parameters.AddWithValue("@Email", Email);
            try
            {
                con.Open();
                UserType = (string)cmd.ExecuteScalar();
            }
            catch (Exception ex)
            {

            }
            finally
            {
                con.Close();
            }
            return UserType;
        }

        public int GetUserID(string Email)
        {
            int ID = 0;

            string query = "SELECT UserID FROM [User] WHERE Email = @Email\r\nUNION\r\nSELECT ID FROM [Admin] WHERE Email =@Email";
            SqlCommand cmd = new SqlCommand(query, con);
            cmd.Parameters.AddWithValue("@Email", Email);
            try
            {
                con.Open();
                ID = (int)cmd.ExecuteScalar();
            }
            catch (Exception ex)
            {

            }
            finally
            {
                con.Close();
            }
            return ID;
        }

        public bool CheckPassword(int ID, string Password)
        {
            bool match = false;
            string p = "";
            string query = "SELECT [Password] FROM [User] WHERE UserID =@ID\r\nUNION\r\nSELECT [Password] FROM [Admin] WHERE ID =@ID";
            SqlCommand cmd = new SqlCommand(query, con);
            cmd.Parameters.AddWithValue("@ID", ID);
            try
            {
                con.Open();
                p = (string)cmd.ExecuteScalar();
                if (p == Password)
                {
                    match = true;
                }
            }
            catch (Exception ex)
            {

            }
            finally
            {
                con.Close();
            }
            return match;
        }

        public DataTable RoomSearchUsage(string CourseCode, string StaffName, string Type)
        {
            DataTable dt = new DataTable();

            string query = "SELECT R.*\r\nFROM Room R\r\nLEFT JOIN Course C ON R.ID = C.LectureRoom OR R.ID = C.TutorialRoom\r\nLEFT JOIN Professor P ON P.OfficeRoom = R.ID\r\nLEFT JOIN TA T ON T.OfficeRoom = R.ID\r\nLEFT JOIN [User] U ON U.UserID = P.ID OR U.UserID = T.ID\r\nWHERE\r\n    (@CourseCode IS NULL OR @CourseCode = '' OR C.Code = @CourseCode)\r\n                                AND (@StaffName IS NULL OR @StaffName = '' OR U.Name LIKE '%' + @StaffName + '%')\r\n                                AND (\r\n                                    @Type IS NULL OR @Type = '' OR \r\n                                    (@Type = 'Lecture' AND R.ID IN (SELECT LectureRoom FROM Course)) OR\r\n                                    (@Type = 'Tutorial' AND R.ID IN (SELECT TutorialRoom FROM Course)) OR\r\n                                    (@Type = 'Office' AND R.ID IN (SELECT OfficeRoom FROM Professor UNION SELECT OfficeRoom FROM TA))\r\n                                )";
            SqlCommand cmd = new SqlCommand(query, con);
            cmd.Parameters.AddWithValue("@CourseCode", CourseCode);
            cmd.Parameters.AddWithValue("@StaffName", StaffName);
            cmd.Parameters.AddWithValue("@Type", Type);
            try
            {
                con.Open();
                dt.Load(cmd.ExecuteReader());
            }
            catch (Exception ex)
            {

            }
            finally
            {
                con.Close();
            }
            return dt;
        }

        public DataTable RoomSearchLocation(string Building, string Floor, string Zone)
        {
            DataTable dt = new DataTable();
            string query = "SELECT R.*\r\nFROM Room R\r\nWHERE\r\n    (@Building IS NULL OR @Building = '' OR Building = @Building)\r\n                                AND (@Floor IS NULL OR @Floor = '' OR [Floor] = @Floor)\r\n                                AND (@Zone IS NULL OR @Zone = '' OR [Zone] = @Zone)";
            SqlCommand cmd = new SqlCommand(query, con);
            cmd.Parameters.AddWithValue("@Building", Building);
            cmd.Parameters.AddWithValue("@Floor", Floor);
            cmd.Parameters.AddWithValue("@Zone", Zone);
            try
            {
                con.Open();
                dt.Load(cmd.ExecuteReader());
            }
            catch (Exception ex)
            {

            }
            finally
            {
                con.Close();
            }
            return dt;
        }

        public DataTable SearchUsers(string UserType, string Name)
        {
            string query;
            DataTable dt = new DataTable();
            if (UserType == "") query = "Select UserID, UserType, [Name], Email From [User] Where [Name] like '%' + @Name + '%'";
            else if (Name == "") query = "Select UserID, UserType, [Name], Email From [User] Where [UserType] like '%' + @UserType + '%'";
            else if (Name != "" && UserType != "") query = "Select UserID, UserType, [Name], Email From [User] Where [UserType] like '%' + @UserType + '%' And [Name] like '%' + @Name + '%'";
            else query = "Select UserID, UserType, [Name], Email From [User]";
            SqlCommand cmd = new SqlCommand(query, con);
            cmd.Parameters.AddWithValue("@UserType", UserType);
            cmd.Parameters.AddWithValue("@Name", Name);
            try
            {
                con.Open();
                dt.Load(cmd.ExecuteReader());
            }
            catch (Exception ex)
            {

            }
            finally
            {
                con.Close();
            }
            return dt;
        }
        public void UpdateUserType(int UserID, string NewType)
        {
            string query = $"Update [User] Set UserType=@UserType Where UserID=@UserID";

            SqlCommand cmd = new SqlCommand(query, con);
            cmd.Parameters.AddWithValue("@UserType", NewType);
            cmd.Parameters.AddWithValue("@UserID", UserID);
            try
            {
                con.Open();
                cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {

            }
            finally
            {
                con.Close();
            }
        }
        public DataTable LoadCourses()
        {
            string query = "SELECT \r\n    C.[Name] AS [Name],\r\n    Prof.[Name] AS professor_name,\r\n    TA.[Name] AS ta_name,\r\n    JTA.[Name] AS jta_name,\r\n    COUNT(CS.Student_ID) AS Student_Count\r\nFROM Course C\r\nLEFT JOIN Professor P ON C.ProfessorID = P.ID\r\nLEFT JOIN [User] Prof ON P.ID = Prof.UserID\r\nLEFT JOIN Course_TA CT ON C.Code = CT.Course_Code\r\nLEFT JOIN TA T ON CT.TA_ID = T.ID\r\nLEFT JOIN [User] TA ON T.ID = TA.UserID\r\nLEFT JOIN JTA JT ON C.Code = JT.Course_Code\r\nLEFT JOIN Student JTASt ON JT.Student_ID = JTASt.ID\r\nLEFT JOIN [User] JTA ON JTASt.ID = JTA.UserID\r\nLEFT JOIN Course_Student CS ON C.Code = CS.Course_Code\r\nGROUP BY C.Code, C.[Name], Prof.[Name], TA.[Name], JTA.[Name]";
            DataTable dt = new DataTable();
            SqlCommand cmd = new SqlCommand(query, con);
            try
            {
                con.Open();
                dt.Load(cmd.ExecuteReader());
            }
            catch (Exception ex)
            {

            }
            finally
            {
                con.Close();
            }
            return dt;
        }
        public DataTable AllCoursesBriefed()
        {
            string query = "SELECT \r\n    C.Code AS Course_Code,\r\n    C.[Name] AS Course_Name,\r\n    U.[Name] AS Professor_Name\r\nFROM Course c\r\nLEFT JOIN [User] U ON C.ProfessorID = U.UserID";
            DataTable dt = new DataTable();
            SqlCommand cmd = new SqlCommand(query, con);
            try
            {
                con.Open();
                dt.Load(cmd.ExecuteReader());
            }
            catch (Exception ex)
            {

            }
            finally
            {
                con.Close();
            }
            return dt;
        }
        public void AddNewCourse(string Code, string Name, int PID, string LRoom, string LDay, TimeSpan LHour, int LDuration, string TRoom, string TDay, TimeSpan THour, int TDuration)
        {
            string query = "INSERT INTO Course (\r\n    Code, [Name], ProfessorID,\r\n    LectureRoom, LectureDay, LectureHour, LectureDuration,\r\n    TutorialRoom, TutorialDay, TutorialHour, TutorialDuration\r\n) VALUES (\r\n    @Code, @Name, @PID,\r\n    @LRoom, @LDay, @LHour, @LDuration,\r\n    @TRoom, @TDay, @THour, @TDuration\r\n);";
            SqlCommand cmd = new SqlCommand(query, con);
            cmd.Parameters.AddWithValue("@Code", Code);
            cmd.Parameters.AddWithValue("@Name", Name);
            cmd.Parameters.AddWithValue("@PID", PID);
            cmd.Parameters.AddWithValue("@LRoom", LRoom);
            cmd.Parameters.AddWithValue("@LDay", LDay);
            cmd.Parameters.AddWithValue("@LHour", LHour);
            cmd.Parameters.AddWithValue("@LDuration", LDuration);
            cmd.Parameters.AddWithValue("@TRoom", TRoom);
            cmd.Parameters.AddWithValue("@TDay", TDay);
            cmd.Parameters.AddWithValue("@THour", THour);
            cmd.Parameters.AddWithValue("@TDuration", TDuration);
            try
            {
                con.Open();
                cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {

            }
            finally
            {
                con.Close();
            }
        }
        public void ADD_TA_JTA_Students(string CourseCode, List<int> TAsIDs, List<int> JTAsIDs, List<int> StudentsIDs)
        {
            try
            {
                con.Open();
                foreach (var TAID in TAsIDs)
                {
                    string query = "INSERT INTO Course_TA (Course_Code, TA_ID) VALUES (@CourseCode, @TAID);\r\n";
                    SqlCommand cmd = new SqlCommand(query, con);
                    cmd.Parameters.AddWithValue("@CourseCode", CourseCode);
                    cmd.Parameters.AddWithValue("@TAID", TAID);
                    cmd.ExecuteNonQuery();
                }
                foreach (var JTAID in JTAsIDs)
                {
                    string query = "INSERT INTO JTA (Course_Code, Student_ID) VALUES (@CourseCode, @JTAID);\r\n";
                    SqlCommand cmd = new SqlCommand(query, con);
                    cmd.Parameters.AddWithValue("@CourseCode", CourseCode);
                    cmd.Parameters.AddWithValue("@JTAID", JTAID);
                    cmd.ExecuteNonQuery();
                }
                foreach (var StudentID in StudentsIDs)
                {
                    string query = "INSERT INTO Course_Student (Course_Code, Student_ID) VALUES (@CourseCode, @StudentID);\r\n";
                    SqlCommand cmd = new SqlCommand(query, con);
                    cmd.Parameters.AddWithValue("@CourseCode", CourseCode);
                    cmd.Parameters.AddWithValue("@StudentID", StudentID);
                    cmd.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {

            }
            finally
            {
                con.Close();
            }
        }
        public CourseDetailsResult GetCourseData(string CourseCode)
        {
            CourseDetailsResult result = new CourseDetailsResult();
            result.TA_IDs = new List<int>();
            result.JTA_IDs = new List<int>();
            result.Student_IDs = new List<int>();

            string FirstQuery = "SELECT \r\n    C.Code AS course_code,\r\n    C.[Name] AS course_name,\r\n    P.ProfessorID AS professor,\r\n    C.LectureRoom,\r\n    C.LectureDay,\r\n    C.LectureHour,\r\n    C.LectureDuration,\r\n    C.TutorialRoom,\t\r\n    C.TutorialDay,\r\n    C.TutorialHour,\r\n    C.TutorialDuration\r\nFROM Course C\r\nLEFT JOIN [User] P ON C.ProfessorID = P.UserID\r\nLEFT JOIN Course_TA ta ON Ta.Course_Code = C.Code\r\nLEFT JOIN JTA JTA ON JTA.Course_Code = C.Code\r\nLEFT JOIN course_Student CS ON CS.Course_code = C.Code\r\nWHERE C.Code = @CourseCode\r\nGROUP BY \r\n    C.Code, \r\n    C.[Name],\r\n    P.ProfessorID,\r\n    C.LectureRoom,\r\n    C.LectureDay,\r\n    C.LectureHour,\r\n    C.LectureDuration,\r\n    C.TutorialRoom,\r\n    C.TutorialDay,\r\n    C.TutorialHour,\r\n    C.TutorialDuration;";
            SqlCommand cmd_1 = new SqlCommand(FirstQuery, con);
            cmd_1.Parameters.AddWithValue("@CourseCode", CourseCode);


            string SecondQuery = "SELECT TA_ID\r\nFROM Course_TA\r\nWHERE Course_Code = @CourseCode;";
            SqlCommand cmd_2 = new SqlCommand(SecondQuery, con);
            cmd_2.Parameters.AddWithValue("@CourseCode", CourseCode);

            string ThirdQuery = "SELECT Student_ID\r\nFROM JTA\r\nWHERE Course_Code = @CourseCode;";
            SqlCommand cmd_3 = new SqlCommand(ThirdQuery, con);
            cmd_3.Parameters.AddWithValue("@CourseCode", CourseCode);

            string FourthQuery = "SELECT Student_ID\r\nFROM Course_Student\r\nWHERE Course_Code = @CourseCode;";
            SqlCommand cmd_4 = new SqlCommand(FourthQuery, con);
            cmd_4.Parameters.AddWithValue("@CourseCode", CourseCode);

            try
            {
                con.Open();

                SqlDataAdapter adapter = new SqlDataAdapter(cmd_1);
                DataTable courseInfo = new DataTable();
                adapter.Fill(courseInfo);
                result.CourseInfo = courseInfo;

                SqlDataReader reader = cmd_2.ExecuteReader();
                while (reader.Read())
                {
                    result.TA_IDs.Add(reader.GetInt32(0));
                }
                reader.Close();

                reader = cmd_3.ExecuteReader();
                while (reader.Read())
                {
                    result.JTA_IDs.Add(reader.GetInt32(0));
                }
                reader.Close();

                reader = cmd_4.ExecuteReader();
                while (reader.Read())
                {
                    result.Student_IDs.Add(reader.GetInt32(0));
                }
                reader.Close();

            }
            catch (Exception ex)
            {

            }
            finally
            {
                con.Close();
            }
            return result;
        }


        // updated prof queries 

        public DataTable GetProfessorCourses(int professorId)
        {
            DataTable dt = new DataTable();
            string query = @"
                SELECT 
                    c.Code AS CourseCode,
                    c.Name AS CourseName,
                    c.LectureRoom AS CurrentRoom,
                    (SELECT Building FROM Room WHERE ID = c.LectureRoom) AS Building,
                    CONVERT(varchar, c.LectureHour, 108) AS StartTime,
                    CONVERT(varchar, DATEADD(MINUTE, c.LectureDuration, c.LectureHour), 108) AS EndTime,
            c.LectureDay AS LectureDay
            FROM Course c
            WHERE c.ProfessorID = @ProfessorId;";

            SqlCommand cmd = new SqlCommand(query, con);
            cmd.Parameters.AddWithValue("@ProfessorId", professorId);

            try
            {
                con.Open();
                dt.Load(cmd.ExecuteReader());
            }
            catch (Exception ex)
            {
                // Handle error
            }
            finally
            {
                con.Close();
            }
            return dt;
        }

        public DataTable GetProfessorChangeRequests(int professorId)
        {
            DataTable dt = new DataTable();
            string query = @"SELECT 
            c.Code AS CourseCode,
            r.ID AS NewRoom,
            rc.Reason,
            rr.RID AS RequestId
            FROM RoomChangeRequest rc
            JOIN RequestOrReport rr ON rc.ID = rr.RID
            JOIN Course c ON rc.CourseCode = c.Code
            JOIN Room r ON rc.NewRoomID = r.ID
            WHERE c.ProfessorID = @ProfessorId 
            AND rr.Condition = 'Pending';";

            SqlCommand cmd = new SqlCommand(query, con);
            cmd.Parameters.AddWithValue("@ProfessorId", professorId);
            try
            {
                con.Open();
                dt.Load(cmd.ExecuteReader());
            }
            catch (Exception ex)
            {// Handle error
            }
            finally
            {
                con.Close();
            }
            return dt;
        }
        public DataTable GetPendingJTARequests(int professorId)
        {
            DataTable dt = new DataTable();
            string query = @"SELECT 
            rr.RID AS RequestID,
            u.Name AS StudentName,
            c.Code AS CourseCode,
            req.NumOfExtraHours AS RequestedHours,
            req.RoomID AS RelatedRoom,
            req.Reason
            FROM AdditionalQuotaRequest req
            JOIN RequestOrReport rr ON req.ID = rr.RID
            JOIN Course c ON req.CourseCode = c.Code
            JOIN Student s ON rr.UserID = s.ID
            JOIN [User] u ON s.ID = u.UserID
            WHERE c.ProfessorID = @ProfessorId 
            AND rr.Condition = 'Pending'";
            SqlCommand cmd = new SqlCommand(query, con);
            cmd.Parameters.AddWithValue("@ProfessorId", professorId);
            try
            {
                con.Open();
                dt.Load(cmd.ExecuteReader());
            }
            catch (Exception ex)
            {
                // Handle error
            }
            finally
            {
                con.Close();
            }
            return dt;
        }
        public void UpdateRequestStatus(int requestId, string status)
        {
            string query = "UPDATE RequestOrReport SET Condition = @Status WHERE RID = @RequestId";
            SqlCommand cmd = new SqlCommand(query, con);
            cmd.Parameters.AddWithValue("@Status", status);
            cmd.Parameters.AddWithValue("@RequestId", requestId);

            try
            {
                con.Open();
                cmd.ExecuteNonQuery();
            }
            finally
            {
                con.Close();
            }
        }
        public DataTable GetStudentCourses(int studentId)
        {
            DataTable dt = new DataTable();
            string query = @"
             SELECT 
             c.Code AS CourseCode,
             c.Name AS CourseName,
             c.LectureRoom AS Room,
             (SELECT Building FROM Room WHERE ID = c.LectureRoom) AS Building,
             CONCAT(c.LectureDay, ' ', c.LectureHour, '-', 
                   DATEADD(MINUTE, c.LectureDuration, c.LectureHour)) AS TimeSlot,
                   CASE 
                   WHEN j.Student_ID IS NOT NULL THEN 'JTA Course'
                   ELSE 'Enrolled Course'
                   END AS CourseType
                   FROM Course_Student cs
                   JOIN Course c ON cs.Course_Code = c.Code
                   LEFT JOIN JTA j ON cs.Student_ID = j.Student_ID AND cs.Course_Code = j.Course_Code
                   WHERE cs.Student_ID = @StudentId";
            SqlCommand cmd = new SqlCommand(query, con);
            cmd.Parameters.AddWithValue("@StudentId", studentId);
            try
            {
                con.Open();
                dt.Load(cmd.ExecuteReader());
            }
            catch (Exception ex)
            {
                // Log error
            }
            finally
            {
                con.Close();
            }
        }
            return dt;
        }

        public DataTable GetStudentReports(int studentId)
        {
            DataTable dt = new DataTable();
            string query = @"
        SELECT 
            r.RoomID AS RoomCode,
            r.Complaint AS ComplaintDetails,
            CONVERT(VARCHAR, rr.DayofR, 103) + ' ' + 
            CONVERT(VARCHAR, rr.HourofR, 108) AS SubmissionDateTime,
            rr.Condition AS Status
        FROM RequestOrReport rr
        JOIN Report r ON rr.RID = r.ID
        WHERE rr.UserID = @StudentId 
          AND rr.RType = 'Report'";

            SqlCommand cmd = new SqlCommand(query, con);
            cmd.Parameters.AddWithValue("@StudentId", studentId);

            try
            {
                con.Open();
                dt.Load(cmd.ExecuteReader());
            }
            catch (Exception ex)
            {
                // Log error
            }
            finally
            {
                con.Close();
            }
            return dt;
        }

        public int GetUserID(string email)
        {
            string query = @"SELECT UserID FROM [User] WHERE Email = @Email";
            SqlCommand cmd = new SqlCommand(query, con);
            cmd.Parameters.AddWithValue("@Email", email);

            try
            {
                con.Open();
                return (int)cmd.ExecuteScalar();
            }
            catch
            {
                return 0;
            }
            finally
            {
                con.Close();
            }
        }


        public DataTable GetStudentBookings(int studentId)
        {
            DataTable dt = new DataTable();
            string query = @"
        SELECT 
            rb.ID AS BookingID,
            rb.RoomID AS RoomCode,
            rb.Reason,
            CONVERT(VARCHAR, rr.DayofR, 103) + ' ' + 
            CONVERT(VARCHAR, rb.TimeSlotHour, 108) + ' - ' + 
            CONVERT(VARCHAR, DATEADD(MINUTE, rb.Duration, rb.TimeSlotHour), 108) AS TimeSlot,
            CASE 
                WHEN cbr.ID IS NOT NULL THEN 'Yes' 
                ELSE 'No' 
            END AS ForClinic,
            c.Name AS CourseName
        FROM RequestOrReport rr
        LEFT JOIN RoomBooking rb ON rr.RID = rb.ID
        LEFT JOIN ClinicBookingRequest cbr ON rr.RID = cbr.ID
        LEFT JOIN Course c ON cbr.CourseCode = c.Code
        WHERE rr.UserID = @StudentId";

            SqlCommand cmd = new SqlCommand(query, con);
            cmd.Parameters.AddWithValue("@StudentId", studentId);

            try
            {
                con.Open();
                dt.Load(cmd.ExecuteReader());
            }
            catch (Exception ex)
            {

                Console.WriteLine($"Error getting student bookings: {ex.Message}");
            }
            finally
            {
                con.Close();
            }
            return dt;
        }

        // ta pages queries 
        public DataTable GetTARooms(int taId)
        {
            DataTable dt = new DataTable();
            string query = @"
        SELECT 
            c.Code AS CourseCode,
            c.Name AS CourseName,
            c.TutorialRoom AS Room,
            (SELECT Building FROM Room WHERE ID = c.TutorialRoom) AS Building,
            CONCAT(c.TutorialDay, ' ', 
                   CONVERT(VARCHAR, c.TutorialHour, 108), '-', 
                   CONVERT(VARCHAR, DATEADD(MINUTE, c.TutorialDuration, c.TutorialHour), 108)) AS TimeSlot
        FROM Course_TA ct
        JOIN Course c ON ct.Course_Code = c.Code
        WHERE ct.TA_ID = @TAId";

            SqlCommand cmd = new SqlCommand(query, con);
            cmd.Parameters.AddWithValue("@TAId", taId);

            try
            {
                con.Open();
                dt.Load(cmd.ExecuteReader());
            }
            catch (Exception ex)
            {
                // Log error
                Console.WriteLine($"Error getting TA rooms: {ex.Message}");
            }
            finally
            {
                con.Close();
            }
            return dt;
        }

        public DataTable GetTAPendingRequests(int taId)
        {
            DataTable dt = new DataTable();
            string query = @"
        SELECT 
            c.Code AS CourseCode,
            rep.Complaint AS Reason,
            rr.Condition AS Status
        FROM Course_TA ct
        JOIN Course c ON ct.Course_Code = c.Code
        JOIN Report rep ON rep.RoomID = c.TutorialRoom
        JOIN RequestOrReport rr ON rep.ID = rr.RID
        WHERE ct.TA_ID = @TAId 
          AND rr.Condition = 'Pending'";

            SqlCommand cmd = new SqlCommand(query, con);
            cmd.Parameters.AddWithValue("@TAId", taId);

            try
            {
                con.Open();
                dt.Load(cmd.ExecuteReader());
            }
            catch (Exception ex)
            {
                // Log error
                Console.WriteLine($"Error getting TA pending requests: {ex.Message}");
            }
            finally
            {
                con.Close();
            }
            return dt;
        }

        public void SubmitTAReport(int taId, string roomCode, string description)
        {
            using (var connection = new SqlConnection(ConnectionString))
            {
                connection.Open();
                using (var transaction = connection.BeginTransaction())
                {
                    try
                    {
                        // Insert into RequestOrReport
                        var requestQuery = @"
                    INSERT INTO RequestOrReport 
                        (UserID, Condition, DayofR, HourofR, RType)
                    OUTPUT INSERTED.RID
                    VALUES (@UserID, 'Pending', GETDATE(), CONVERT(TIME, GETDATE()), 'Report')";

                        var requestCmd = new SqlCommand(requestQuery, connection, transaction);
                        requestCmd.Parameters.AddWithValue("@UserID", taId);
                        int newRid = (int)requestCmd.ExecuteScalar();

                        // Insert into Report
                        var reportQuery = @"
                    INSERT INTO Report 
                        (ID, RoomID, Complaint)
                    VALUES (@ID, @RoomID, @Complaint)";

                        var reportCmd = new SqlCommand(reportQuery, connection, transaction);
                        reportCmd.Parameters.AddWithValue("@ID", newRid);
                        reportCmd.Parameters.AddWithValue("@RoomID", roomCode);
                        reportCmd.Parameters.AddWithValue("@Complaint", description);
                        reportCmd.ExecuteNonQuery();

                        transaction.Commit();
                    }
                    catch
                    {
                        transaction.Rollback();
                        throw;
                    }
                }
            }
        }

        public DataTable GetTAReports(int taId)
        {
            DataTable dt = new DataTable();
            string query = @"
        SELECT 
            r.RoomID AS RoomCode,
            r.Complaint AS IssueDescription,
            CONVERT(VARCHAR, rr.DayofR, 103) + ' ' + 
            CONVERT(VARCHAR, rr.HourofR, 108) AS SubmissionDateTime,
            rr.Condition AS Status
        FROM RequestOrReport rr
        JOIN Report r ON rr.RID = r.ID
        WHERE rr.UserID = @TAId 
          AND rr.RType = 'Report'";

            SqlCommand cmd = new SqlCommand(query, con);
            cmd.Parameters.AddWithValue("@TAId", taId);

            try
            {
                con.Open();
                dt.Load(cmd.ExecuteReader());
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error getting TA reports: {ex.Message}");
            }
            finally
            {
                con.Close();
            }
            return dt;
        }

        public int GetTARemainingQuota(int taId)
        {
            string query = @"
        SELECT 
            t.Quota - ISNULL(SUM(rb.Duration), 0) AS RemainingQuota
        FROM TA t
        LEFT JOIN RequestOrReport rr ON rr.UserID = t.ID
        LEFT JOIN RoomBooking rb ON rr.RID = rb.ID
            AND rr.RType = 'RoomBooking'  
            AND MONTH(rr.DayofR) = MONTH(GETDATE())  
            AND YEAR(rr.DayofR) = YEAR(GETDATE())
        WHERE t.ID = @TAId
        GROUP BY t.Quota";

            SqlCommand cmd = new SqlCommand(query, con);
            cmd.Parameters.AddWithValue("@TAId", taId);

            try
            {
                con.Open();
                return Convert.ToInt32(cmd.ExecuteScalar());
            }
            catch
            {
                return 0;
            }
            finally
            {
                con.Close();
            }
        }

        public DataTable GetTAAssignedCourses(int taId)
        {
            DataTable dt = new DataTable();
            string query = @"
        SELECT 
            c.Code AS CourseCode,
            c.Name AS CourseName,
            c.TutorialRoom AS DefaultRoom
        FROM Course_TA ct
        JOIN Course c ON ct.Course_Code = c.Code
        WHERE ct.TA_ID = @TAId";

            SqlCommand cmd = new SqlCommand(query, con);
            cmd.Parameters.AddWithValue("@TAId", taId);

            try
            {
                con.Open();
                dt.Load(cmd.ExecuteReader());
            }
            finally
            {
                con.Close();
            }
            return dt;
        }

        public void SubmitTABooking(int taId, string roomCode, string day, TimeSpan time, string courseCode)
        {
            using (var connection = new SqlConnection(ConnectionString))
            {
                connection.Open();
                using (var transaction = connection.BeginTransaction())
                {
                    try
                    {
                        // Insert into RequestOrReport
                        var requestQuery = @"
                    INSERT INTO RequestOrReport 
                        (UserID, Condition, DayofR, HourofR, RType)
                    OUTPUT INSERTED.RID
                    VALUES (@UserID, 'Pending', GETDATE(), @Hour, 'RoomBooking')";

                        var requestCmd = new SqlCommand(requestQuery, connection, transaction);
                        requestCmd.Parameters.AddWithValue("@UserID", taId);
                        requestCmd.Parameters.AddWithValue("@Hour", time);
                        int newRid = (int)requestCmd.ExecuteScalar();

                        // Insert into RoomBooking
                        var bookingQuery = @"
                    INSERT INTO RoomBooking 
                        (ID, RoomID, Reason, TimeSlotDay, TimeSlotHour, Duration, CourseCode)
                    VALUES (@ID, @RoomID, 'Tutorial Session', @Day, @Hour, 120, @CourseCode)";

                        var bookingCmd = new SqlCommand(bookingQuery, connection, transaction);
                        bookingCmd.Parameters.AddWithValue("@ID", newRid);
                        bookingCmd.Parameters.AddWithValue("@RoomID", roomCode);
                        bookingCmd.Parameters.AddWithValue("@Day", day);
                        bookingCmd.Parameters.AddWithValue("@Hour", time);
                        bookingCmd.Parameters.AddWithValue("@CourseCode", courseCode);
                        bookingCmd.ExecuteNonQuery();

                        transaction.Commit();
                    }
                    catch
                    {
                        transaction.Rollback();
                        throw;
                    }
                }
            }
        }

    public class CourseDetailsResult
    {
        public DataTable CourseInfo { get; set; }
        public List<int> TA_IDs { get; set; }
        public List<int> JTA_IDs { get; set; }
        public List<int> Student_IDs { get; set; }

    }







 //student#
 public int student_quota(string email)
 {
     int quota = 0;
     string query = "\r\nSELECT Quota\r\nFROM   Student S\r\ninner Join [User] As u on u.UserID =S.ID\r\nWHere u.Email= @Email;";
     SqlCommand cmd = new SqlCommand(query, con);
     cmd.Parameters.Add("@Email", SqlDbType.NVarChar, 100).Value = email;

     try
     {
         con.Open();
         quota = (int)cmd.ExecuteScalar();
     }
     catch (Exception ex)
     {
         Console.WriteLine(ex.Message);
     }
     finally
     {
         con.Close();
     }
     return quota;
 }



        public void insert_extra_hours_quota(string email, int hours_extra,string reason)
        {
            string query = @"DECLARE @UserID INT = (SELECT u.UserID FROM[User] u WHERE u.Email = @Email);
            DECLARE @RID    INT = ISNULL((SELECT MAX(RID) FROM RequestOrReport), 0) +1;

            INSERT INTO RequestOrReport(RID, UserID, Condition, DayofR, HourofR, RType)
VALUES(@RID, @UserID, 'Pending', CAST(GETDATE() AS date), CAST(GETDATE() AS time), 'AdditionalQuotaRequest');

            INSERT INTO AdditionalQuotaRequest(ID, NumOfExtraHours, reason)
VALUES(@RID, @ExtraHours, @Reason); ";


            SqlCommand cmd = new SqlCommand(query, con);
            cmd.Parameters.Add("@Email", SqlDbType.NVarChar, 100).Value = email;
            cmd.Parameters.Add("@ExtraHours", SqlDbType.Int).Value = hours_extra;
            cmd.Parameters.Add("@Reason", SqlDbType.NVarChar, 255).Value = reason;

            try
            {
                con.Open();
                cmd.ExecuteNonQuery();
                
            }
            catch (Exception ex) { Console.WriteLine(ex.Message); }
            finally { con.Close(); }


        }



 ////////////////////////////////////////////////////////////////////////////////////////////
 ///TA///////////////////
 ///

 public int TA_quota(string email)
 {
     int quota = 0;
     string query = "\r\nSELECT Quota\r\nFROM   TA S\r\ninner Join [User] As u on u.UserID =S.ID\r\nWHere u.Email= @Email;";
     SqlCommand cmd = new SqlCommand(query, con);
     cmd.Parameters.Add("@Email", SqlDbType.NVarChar, 100).Value = email;

     try
     {
         con.Open();
         quota = (int)cmd.ExecuteScalar();
     }
     catch (Exception ex)
     {
         Console.WriteLine(ex.Message);
     }
     finally
     {
         con.Close();
     }
     return quota;
 }


    ////////////////////////////////////////////////////////////////////////////////////////
    //////////////Admin
    ///////creat user
    
    public void creat_user(int userId, string passwordHash, string name, string email, string userType,
                           string? officeRoom = null)
    {
    
        string query = @"INSERT INTO [User] (UserID,[Password],[Name],Email,UserType)
                             VALUES(@UserID,@Password,@UserName,@UserEmail,@UserType);
    
                             IF      @UserType = 'Professor'
                                 INSERT INTO Professor (ID,OfficeRoom)
                                 VALUES (@UserID, @OfficeRoom);
                             ELSE IF @UserType = 'Student'
                                 INSERT INTO Student (ID,Quota)
                                 VALUES (@UserID, 3);
                             ELSE IF @UserType = 'TA'
                                 INSERT INTO TA (ID,OfficeRoom,Quota)
                                 VALUES (@UserID, @OfficeRoom, 20);";
        SqlCommand cmd = new SqlCommand(query, con);
        cmd.Parameters.Add("@UserID", SqlDbType.Int).Value = userId;
        cmd.Parameters.Add("@Password", SqlDbType.NVarChar, 200).Value = passwordHash; // already hashed outside
        cmd.Parameters.Add("@UserName", SqlDbType.NVarChar, 100).Value = name;
        cmd.Parameters.Add("@UserEmail", SqlDbType.NVarChar, 100).Value = email;
        cmd.Parameters.Add("@UserType", SqlDbType.NVarChar, 20).Value = userType;
        cmd.Parameters.Add("@OfficeRoom", SqlDbType.NVarChar, 20).Value = (object?)officeRoom ?? DBNull.Value;
    
    
        try
        {
            con.Open();
            cmd.ExecuteNonQuery();
        }
        catch (Exception ex) { Console.WriteLine(ex.Message); }
        finally { con.Close(); }
    
    
    }


//stats of the admin////////////////

    
    public (int profs, int tas, int students, int handledBookings, int handledReports) Admin_stats()
    {
        /*  int profs, tas, students, handledBookings, handledReports;*/
        string query = "SELECT\r\n  (SELECT COUNT(*) FROM Professor)                               AS NumProfessors,\r\n  (SELECT COUNT(*) FROM TA)                                      AS NumTAs,\r\n  (SELECT COUNT(*) FROM Student)                                 AS NumStudents,\r\n  (SELECT COUNT(*) FROM RequestOrReport\r\n    WHERE RType IN ('RoomBookingRequest','ClinicBookingRequest')\r\n      AND Condition = 'Handled')                                 AS HandledBookingReqs,\r\n  (SELECT COUNT(*) FROM RequestOrReport\r\n    WHERE RType = 'Report'\r\n      AND Condition = 'Handled')                                 AS HandledReports;";
    
    
        SqlCommand cmd = new SqlCommand(query, con);
        try
        {
            con.Open();
            using var r = cmd.ExecuteReader();
            if (!r.Read()) return (0, 0, 0, 0, 0);
            return (r.GetInt32(0),
           r.GetInt32(1),
           r.GetInt32(2),
           r.GetInt32(3),
           r.GetInt32(4));
        }
        catch (Exception ex) { Console.WriteLine(ex.Message); throw; }
        finally { con.Close(); }
    
    }


     public void InsertSuppliesRequest(string email, DateTime expectedDelivery, string suppliesText)
 {
     
 
     const string query = @"DECLARE @UserID INT = (SELECT u.UserID FROM[User] u WHERE u.Email = @Email);
     DECLARE @RID    INT = ISNULL((SELECT MAX(RID) FROM RequestOrReport), 0) +1;

     INSERT INTO RequestOrReport(RID, UserID, Condition, DayofR, HourofR, RType)
     VALUES(@RID, @UserID, 'Pending', CAST(GETDATE() AS date), CAST(GETDATE() AS time), 'SuppliesRequest');

     INSERT INTO SuppliesRequest (ID, ExpectedDeliveryDate, Supplies)
     VALUES (@RID, @ExpectedDelivery, @SuppliesText);";



     SqlCommand cmd = new SqlCommand(query, con);
    

     
     cmd.Parameters.Add("@Email", SqlDbType.NVarChar, 100).Value = email;
     cmd.Parameters.Add("@ExpectedDelivery", SqlDbType.Date).Value = expectedDelivery;
     cmd.Parameters.Add("@SuppliesText", SqlDbType.NVarChar, -1).Value = suppliesText;
     try
     {
         con.Open();
         cmd.ExecuteNonQuery();
     }
     catch (Exception ex)
     {
         Console.WriteLine("Error in InsertSuppliesRequest: " + ex.Message);
         throw;

     }
     finally { con.Close(); }
     }


        public Dictionary<string, int> getFavouriteCodeEditors()
    {
        // 1) Prepare an empty dictionary
        var labelsAndCounts = new Dictionary<string, int>();

        // 2) Build a UNION ALL query that returns (code_editor, count) rows
        string query = @"
  SELECT 'Professors'               AS code_editor, COUNT(*) AS count FROM Professor
  UNION ALL
  SELECT 'TAs'                      AS code_editor, COUNT(*) AS count FROM TA
  UNION ALL
  SELECT 'Students'                 AS code_editor, COUNT(*) AS count FROM Student
  UNION ALL
  SELECT 'HandledBookingRequests'   AS code_editor, COUNT(*) AS count
    FROM RequestOrReport
   WHERE RType IN ('RoomBookingRequest','ClinicBookingRequest')
     AND Condition = 'Handled'
  UNION ALL
  SELECT 'HandledReports'           AS code_editor, COUNT(*) AS count
    FROM RequestOrReport
   WHERE RType = 'Report'
     AND Condition = 'Handled';
";

        SqlCommand cmd = new SqlCommand(query, con);
        try
        {
            con.Open();
            using var reader = cmd.ExecuteReader();

            // 3) Read each row and shove into your dictionary
            while (reader.Read())
            {
                string label = reader.GetString(reader.GetOrdinal("code_editor"));
                int cnt = reader.GetInt32(reader.GetOrdinal("count"));
                labelsAndCounts[label] = cnt;
            }
        }
        catch (Exception ex)
        {
            // bubble it up or log
            Console.WriteLine(ex.Message);
            throw;
        }
        finally
        {
            con.Close();
        }

        return labelsAndCounts;
    }
        

 }



}
