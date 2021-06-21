using System;
using System.Configuration;
using System.Data;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace CSCI342_FactoryDB
{
    public class StudentDB
    {

        // !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
        // Remember to change the database connection in the app.config!!!!!!!!!!
        // !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
        static readonly string cnFactoryDB = ConfigurationManager.ConnectionStrings["FactoryDB"].ConnectionString;

        static readonly string sqlSelect = @"SELECT 
StudentId,
StudentFirstName,
StudentLastName,
StudentMajorCode,
StudentGPA,
AddDateTime
FROM STT001_STUDENT
WHERE StudentId = @StudentId;";

        private static readonly string sqlInsert = @"
DECLARE @RC INT = 0;                -- this value holds the return code, which will be 0 (success) or -1 (failure) 

BEGIN TRY

SET @StudentMajorCode = UPPER(@StudentMajorCode); -- force this field to be uppercase 

BEGIN TRANSACTION

SET @StudentId = (SELECT ISNULL(MAX(StudentId), 0) FROM STT001_STUDENT); 
SET @StudentId = @StudentId + 1; 

INSERT INTO STT001_STUDENT
(
StudentId,
StudentFirstName,
StudentLastName,
StudentMajorCode,
StudentGPA
)
VALUES
(
@StudentId,
@StudentFirstName,
@StudentLastName,
@StudentMajorCode,
@StudentGPA
);

COMMIT TRANSACTION 

END TRY

BEGIN CATCH 

IF @@TRANCOUNT > 0
   ROLLBACK TRANSACTION;

 -- there has been an error so set the return code to -1
SET @RC = -1;

-- Uncomment this to cause an error to be thrown in the calling (C#) code 
-- THROW;  

END CATCH
";

        private static readonly string sqlUpdate = @"
DECLARE @RC INT = 0;                -- this value holds the return code, which will be 0 (success) or -1 (failure) 

BEGIN TRY

SET @StudentMajorCode = UPPER(@StudentMajorCode); -- force this field to be uppercase 

UPDATE STT001_STUDENT
SET
StudentFirstName = @StudentFirstName,
StudentLastName = @StudentLastName,
StudentMajorCode = @StudentMajorCode,
StudentGPA = @StudentGPA
WHERE StudentId = @StudentId; 

END TRY

BEGIN CATCH 

 -- there has been an error so set the return code to -1
SET @RC = -1;

-- Uncomment this to cause an error to be thrown in the calling (C#) code 
-- THROW;  

END CATCH
";

        private static readonly string sqlDelete = @"
DECLARE @RC INT = 0;                -- this value holds the return code, which will be 0 (success) or -1 (failure) 

BEGIN TRY

DELETE FROM STT001_STUDENT
WHERE StudentId = @StudentId; 

END TRY

BEGIN CATCH 

 -- there has been an error so set the return code to -1
SET @RC = -1;

-- Uncomment this to cause an error to be thrown in the calling (C#) code 
-- THROW;  

END CATCH
";

        public static Student Inquire(int StudentId)
        {
            Student student = null;

            using (SqlConnection cn = new SqlConnection(cnFactoryDB))
            {
                using (SqlCommand cm = new SqlCommand(sqlSelect, cn))
                {
                    SqlParameter pm = new SqlParameter("@StudentId", System.Data.SqlDbType.Int, 4);
                    pm.Direction = System.Data.ParameterDirection.Input;
                    pm.Value = StudentId;
                    cm.Parameters.Add(pm);

                    cn.Open();
                    using (SqlDataReader dr = cm.ExecuteReader())
                    {
                        if (dr.Read())  // Read() returns true if there is a record to read; false otherwise
                        {
                            student = new Student
                            {
                                StudentId = (int)dr["StudentId"],
                                FirstName = dr["StudentFirstName"] as string,
                                LastName = dr["StudentLastName"] as string,
                                GPA = (decimal)dr["StudentGPA"],
                                MajorCode = dr["StudentMajorCode"] as string,
                                AddDateTime = (DateTime)dr["AddDateTime"]
                            };
                        }
                    }

                }
            }

            return student;
        }

        public async static Task<Student> InquireAsync(int StudentId)
        {
            Student student = null;

            using (SqlConnection cn = new SqlConnection(cnFactoryDB))
            {
                using (SqlCommand cm = new SqlCommand(sqlSelect, cn))
                {
                    SqlParameter pm = new SqlParameter("@StudentId", System.Data.SqlDbType.Int, 4);
                    pm.Direction = System.Data.ParameterDirection.Input;
                    pm.Value = StudentId;
                    cm.Parameters.Add(pm);

                    await cn.OpenAsync().ConfigureAwait(false);
                    using (SqlDataReader dr = await cm.ExecuteReaderAsync().ConfigureAwait(false))
                    {
                        if (await dr.ReadAsync().ConfigureAwait(false))  // Read() returns true if there is a record to read; false otherwise
                        {
                            student = new Student
                            {
                                StudentId = (int)dr["StudentId"],
                                FirstName = dr["StudentFirstName"] as string,
                                LastName = dr["StudentLastName"] as string,
                                GPA = (decimal)dr["StudentGPA"],
                                MajorCode = dr["StudentMajorCode"] as string,
                                AddDateTime = (DateTime)dr["AddDateTime"]
                            };
                        }
                    }

                }
            }

            return student;
        }


        /// <summary>
        /// Inserts a student into the database
        /// </summary>
        /// <param name="student">A student object representing the student to be added to the database</param>
        /// <returns>The student id (greater than zero) upon success; otherwise -1.</returns>
        public static int Add(Student student)
        {
            int returnValue = 0; 

            if (student == null) throw new ArgumentNullException("Student cannot be null");

            using (SqlConnection cn = new SqlConnection(cnFactoryDB))
            {
                using (SqlCommand cm = new SqlCommand(sqlInsert, cn))
                {
                    SqlParameter pm = new SqlParameter("@StudentId", SqlDbType.Int, 4);
                    pm.Direction = ParameterDirection.Output;
                    cm.Parameters.Add(pm);

                    pm = new SqlParameter("@StudentFirstName", SqlDbType.VarChar, 50);
                    pm.Direction = ParameterDirection.Input;
                    pm.Value = student.FirstName;
                    cm.Parameters.Add(pm);

                    pm = new SqlParameter("@StudentLastName", SqlDbType.VarChar, 50);
                    pm.Direction = ParameterDirection.Input;
                    pm.Value = student.LastName;
                    cm.Parameters.Add(pm);

                    pm = new SqlParameter("@StudentMajorCode", SqlDbType.Char, 5);
                    pm.Direction = ParameterDirection.Input;
                    pm.Value = student.MajorCode;
                    cm.Parameters.Add(pm);

                    pm = new SqlParameter("@StudentGPA", SqlDbType.Decimal);
                    pm.Direction = ParameterDirection.Input;
                    pm.Precision = 3;   // total digits
                    pm.Scale = 2;       // digits to the right of the decimal point 
                    pm.Value = student.GPA;
                    cm.Parameters.Add(pm);

                    pm = new SqlParameter("@RC", SqlDbType.Int, 4);
                    pm.Direction = ParameterDirection.ReturnValue;
                    cm.Parameters.Add(pm);

                    cn.Open();
                    cm.ExecuteNonQuery();

                    returnValue = (int)cm.Parameters["@RC"].Value;
                    if(returnValue == 0 )
                    {
                        returnValue = (int)cm.Parameters["@StudentId"].Value;
                    }
                }
            }

            return returnValue;
        }

        /// <summary>
        /// Inserts a student into the database
        /// </summary>
        /// <param name="student">A student object representing the student to be added to the database</param>
        /// <returns>The student id (greater than zero) upon success; otherwise -1.</returns>
        public async static Task<int> AddAsync(Student student)
        {
            int returnValue = 0;

            if (student == null) throw new ArgumentNullException("Student cannot be null");

            using (SqlConnection cn = new SqlConnection(cnFactoryDB))
            {
                using (SqlCommand cm = new SqlCommand(sqlInsert, cn))
                {
                    SqlParameter pm = new SqlParameter("@StudentId", SqlDbType.Int, 4);
                    pm.Direction = ParameterDirection.Output;
                    cm.Parameters.Add(pm);

                    pm = new SqlParameter("@StudentFirstName", SqlDbType.VarChar, 50);
                    pm.Direction = ParameterDirection.Input;
                    pm.Value = student.FirstName;
                    cm.Parameters.Add(pm);

                    pm = new SqlParameter("@StudentLastName", SqlDbType.VarChar, 50);
                    pm.Direction = ParameterDirection.Input;
                    pm.Value = student.LastName;
                    cm.Parameters.Add(pm);

                    pm = new SqlParameter("@StudentMajorCode", SqlDbType.Char, 5);
                    pm.Direction = ParameterDirection.Input;
                    pm.Value = student.MajorCode;
                    cm.Parameters.Add(pm);

                    pm = new SqlParameter("@StudentGPA", SqlDbType.Decimal);
                    pm.Direction = ParameterDirection.Input;
                    pm.Precision = 3;   // total digits
                    pm.Scale = 2;       // digits to the right of the decimal point 
                    pm.Value = student.GPA;
                    cm.Parameters.Add(pm);

                    pm = new SqlParameter("@RC", SqlDbType.Int, 4);
                    pm.Direction = ParameterDirection.ReturnValue;
                    cm.Parameters.Add(pm);

                    await cn.OpenAsync();
                    await cm.ExecuteNonQueryAsync();

                    returnValue = (int)cm.Parameters["@RC"].Value;
                    if (returnValue == 0)
                    {
                        returnValue = (int)cm.Parameters["@StudentId"].Value;
                    }
                }
            }

            return returnValue;
        }

        /// <summary>
        /// Updates a student record in the database
        /// </summary>
        /// <param name="student">A student object representing the student to be updated to the database</param>
        /// <returns>The student id (greater than zero) upon success; otherwise -1.</returns>
        public static int Update(Student student)
        {
            int returnValue = 0;

            if (student == null) throw new ArgumentNullException("Student cannot be null");

            using (SqlConnection cn = new SqlConnection(cnFactoryDB))
            {
                using (SqlCommand cm = new SqlCommand(sqlUpdate, cn))
                {
                    SqlParameter pm = new SqlParameter("@StudentId", SqlDbType.Int, 4);
                    pm.Direction = ParameterDirection.Input;
                    pm.Value = student.StudentId;
                    cm.Parameters.Add(pm);

                    pm = new SqlParameter("@StudentFirstName", SqlDbType.VarChar, 50);
                    pm.Direction = ParameterDirection.Input;
                    pm.Value = student.FirstName;
                    cm.Parameters.Add(pm);

                    pm = new SqlParameter("@StudentLastName", SqlDbType.VarChar, 50);
                    pm.Direction = ParameterDirection.Input;
                    pm.Value = student.LastName;
                    cm.Parameters.Add(pm);

                    pm = new SqlParameter("@StudentMajorCode", SqlDbType.Char, 5);
                    pm.Direction = ParameterDirection.Input;
                    pm.Value = student.MajorCode;
                    cm.Parameters.Add(pm);

                    pm = new SqlParameter("@StudentGPA", SqlDbType.Decimal);
                    pm.Direction = ParameterDirection.Input;
                    pm.Precision = 3;   // total digits
                    pm.Scale = 2;       // digits to the right of the decimal point 
                    pm.Value = student.GPA;
                    cm.Parameters.Add(pm);

                    pm = new SqlParameter("@RC", SqlDbType.Int, 4);
                    pm.Direction = ParameterDirection.ReturnValue;
                    cm.Parameters.Add(pm);

                    cn.Open();
                    cm.ExecuteNonQuery();

                    returnValue = (int)cm.Parameters["@RC"].Value;
                }
            }

            return returnValue;
        }

        /// <summary>
        /// Updates a student record in the database
        /// </summary>
        /// <param name="student">A student object representing the student to be updated to the database</param>
        /// <returns>The student id (greater than zero) upon success; otherwise -1.</returns>
        public async static Task<int> UpdateAsync(Student student)
        {
            int returnValue = 0;

            if (student == null) throw new ArgumentNullException("Student cannot be null");

            using (SqlConnection cn = new SqlConnection(cnFactoryDB))
            {
                using (SqlCommand cm = new SqlCommand(sqlUpdate, cn))
                {
                    SqlParameter pm = new SqlParameter("@StudentId", SqlDbType.Int, 4);
                    pm.Direction = ParameterDirection.Input;
                    pm.Value = student.StudentId;
                    cm.Parameters.Add(pm);

                    pm = new SqlParameter("@StudentFirstName", SqlDbType.VarChar, 50);
                    pm.Direction = ParameterDirection.Input;
                    pm.Value = student.FirstName;
                    cm.Parameters.Add(pm);

                    pm = new SqlParameter("@StudentLastName", SqlDbType.VarChar, 50);
                    pm.Direction = ParameterDirection.Input;
                    pm.Value = student.LastName;
                    cm.Parameters.Add(pm);

                    pm = new SqlParameter("@StudentMajorCode", SqlDbType.Char, 5);
                    pm.Direction = ParameterDirection.Input;
                    pm.Value = student.MajorCode;
                    cm.Parameters.Add(pm);

                    pm = new SqlParameter("@StudentGPA", SqlDbType.Decimal);
                    pm.Direction = ParameterDirection.Input;
                    pm.Precision = 3;   // total digits
                    pm.Scale = 2;       // digits to the right of the decimal point 
                    pm.Value = student.GPA;
                    cm.Parameters.Add(pm);

                    pm = new SqlParameter("@RC", SqlDbType.Int, 4);
                    pm.Direction = ParameterDirection.ReturnValue;
                    cm.Parameters.Add(pm);

                    await cn.OpenAsync().ConfigureAwait(false);
                    await cm.ExecuteNonQueryAsync().ConfigureAwait(false);

                    returnValue = (int)cm.Parameters["@RC"].Value;
                }
            }

            return returnValue;
        }

        /// <summary>
        /// Updates a student record in the database
        /// </summary>
        /// <param name="student">A student object representing the student to be updated to the database</param>
        /// <returns>The student id (greater than zero) upon success; otherwise -1.</returns>
        public static int Delete(Student student)
        {
            int returnValue = 0;

            if (student == null) throw new ArgumentNullException("Student cannot be null");

            using (SqlConnection cn = new SqlConnection(cnFactoryDB))
            {
                using (SqlCommand cm = new SqlCommand(sqlDelete, cn))
                {
                    SqlParameter pm = new SqlParameter("@StudentId", SqlDbType.Int, 4);
                    pm.Direction = ParameterDirection.Input;
                    pm.Value = student.StudentId;
                    cm.Parameters.Add(pm);

                    pm = new SqlParameter("@RC", SqlDbType.Int, 4);
                    pm.Direction = ParameterDirection.ReturnValue;
                    cm.Parameters.Add(pm);

                    cn.Open();
                    cm.ExecuteNonQuery();

                    returnValue = (int)cm.Parameters["@RC"].Value;
                }
            }

            return returnValue;
        }

        /// <summary>
        /// Updates a student record in the database
        /// </summary>
        /// <param name="student">A student object representing the student to be updated to the database</param>
        /// <returns>The student id (greater than zero) upon success; otherwise -1.</returns>
        public async static Task<int> DeleteAsync(Student student)
        {
            int returnValue = 0;

            if (student == null) throw new ArgumentNullException("Student cannot be null");

            using (SqlConnection cn = new SqlConnection(cnFactoryDB))
            {
                using (SqlCommand cm = new SqlCommand(sqlDelete, cn))
                {
                    SqlParameter pm = new SqlParameter("@StudentId", SqlDbType.Int, 4);
                    pm.Direction = ParameterDirection.Input;
                    pm.Value = student.StudentId;
                    cm.Parameters.Add(pm);

                    pm = new SqlParameter("@RC", SqlDbType.Int, 4);
                    pm.Direction = ParameterDirection.ReturnValue;
                    cm.Parameters.Add(pm);

                    await cn.OpenAsync().ConfigureAwait(false);
                    await cm.ExecuteNonQueryAsync().ConfigureAwait(false);

                    returnValue = (int)cm.Parameters["@RC"].Value;
                }
            }

            return returnValue;
        }


    }
}
