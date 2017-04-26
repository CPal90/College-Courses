using System;

namespace CollegeCourses
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            var bootstrapper = new Bootstrapper(new ConsoleOutputWriter());

            try
            {
                bootstrapper.Start(args);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);                
            }

            Console.ReadLine();
        }
    }
}