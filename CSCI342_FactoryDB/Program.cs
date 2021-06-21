using System;
using System.Threading.Tasks;

namespace CSCI342_FactoryDB
{
    class Program
    {
        // !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
        // Remember to change the database connection in the app.config!!!!!!!!!!
        // !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
        static async Task Main(string[] args)
        {
            int StudentId = 0;
            int returnCode = 0;

            Student student = new Student
            {
                FirstName = "Bob",
                LastName = "Jones",
                GPA = 0.0m,
                MajorCode = "CSCI"
            };

            // Note that this will add a new record every time the program is executed 
            StudentId = await StudentDB.AddAsync(student).ConfigureAwait(false);

            if (StudentId > 0)
            {
                student = await StudentDB.InquireAsync(StudentId).ConfigureAwait(false);
                Console.WriteLine(student.ToString());

                student.MajorCode = "NETW";
                student.GPA = 3.75m;

                returnCode = await StudentDB.UpdateAsync(student).ConfigureAwait(false);
                if (returnCode == 0)
                {
                    Console.WriteLine("\nAfter Updating GPA Major");
                    Console.WriteLine(student.ToString());

                    StudentId = student.StudentId; // save the Id 
                    Console.WriteLine($"\nDelete StudentId: {StudentId} from the database");
                    returnCode = await StudentDB.DeleteAsync(student).ConfigureAwait(false);
                    if (returnCode == 0)
                    {
                        student = await StudentDB.InquireAsync(student.StudentId).ConfigureAwait(false);
                        if (student == null)
                            Console.WriteLine($"StudentId: {StudentId} Not found in the database");
                        else
                            Console.WriteLine($"Hmmm... delete returned success yet the student record {StudentId} still exists...");
                    }
                    else
                        Console.WriteLine($"There was an error deleting the student from the database");
                }
                else
                    Console.WriteLine($"There was an error updating the student in the database");
            }
            else
                Console.WriteLine($"There was an error adding the student to the database");
        }
    }
}