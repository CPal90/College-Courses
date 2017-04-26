namespace CollegeCourses
{
    public static class ErrorMessages
    {
        public const string ERROR_INPUT_WITH_ZERO_COURSES = "Invalid Input. Please include at least 1 course.";
        public const string ERROR_INPUT_MISSING_COLON = "Invalid Input. Please review the formatting of the course names, each value must include a colon and a space followed by the course's prerequisite (if it has any) e.g. Course1: Course2 OR Course1: ";
        public const string ERROR_INPUT_HAS_PREREQUISITE_THAT_WASNT_INCLUDED_AS_COURSE = "Invalid Input. The input included a prerequisite that wasn't also added as a course: {0}";
        public const string ERROR_INPUT_WITH_DUPLICATE_COURSES = "Invalid Input. The input contains duplicate course names: {0}";
        public const string ERROR_INPUT_WITH_CIRCULAR_DEPENDENCIES = "Invalid Input. The input contains circular dependencies: {0} and {1}";
    }
}