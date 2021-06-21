using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSCI342_FactoryDB
{
    public class Student
    {
        public int StudentId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string MajorCode { get; set; }
        public decimal GPA { get; set; }

        // Audit information (not intended to be modified directly) 
        public DateTime AddDateTime { get; set; }

        public Student()
        {
            StudentId = 0;
            FirstName = null;
            LastName = null;
            MajorCode = null;
            GPA = 0.0m;
        }

        public Student(int StudentId)
        {
            var s = StudentDB.Inquire(StudentId);
            if (s != null)
            {
                this.StudentId = s.StudentId;
                FirstName = s.FirstName;
                LastName = s.LastName;
                MajorCode = s.MajorCode;
                GPA = s.GPA;
            }
        }

        public override string ToString()
        {
            return $"StudentId: {StudentId}\nFirst Name: {FirstName}\nLast Name: {LastName}\nMajor: {MajorCode}\nGPA: {GPA}\nAdd DateTime: {AddDateTime}";
        }
    }
}

