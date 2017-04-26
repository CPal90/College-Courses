namespace CollegeCourses
{
    public class Course
    {
        public string Name { get; set; }
        public string Prerequisite { get; set; }
        public int PrerequisitesWeight { get; set; }

        public Course(string name, string prequisite)
        {
            Name = name;
            Prerequisite = prequisite;
        }

        public Course(string name)
        {
            Name = name;
        }
    }
}