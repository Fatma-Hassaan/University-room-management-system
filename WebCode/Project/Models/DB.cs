using System.Collections.Generic;
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
    }

    public class CourseDetailsResult
    {
        public DataTable CourseInfo { get; set; }
        public List<int> TA_IDs { get; set; }
        public List<int> JTA_IDs { get; set; }
        public List<int> Student_IDs { get; set; }

    }
}
