using System;
using System.Collections.Generic;
using System.Linq;

namespace CollegeCourses
{
    public class Bootstrapper
    {
        private readonly List<Course> _courses;
        private readonly IOutputWriter _outputWriter;
        private string[] _inputArgs;

        private Course _courseBeingCheckedForCircularDependency;
        private int _currentPrerequisiteWeight;

        public Bootstrapper(IOutputWriter outputWriter)
        {
            _courses = new List<Course>();
            _outputWriter = outputWriter;
        }

        public void Start(string[] args)
        {
            _inputArgs = args;

            VerifyInputArrayHasValidValuesForProcessing();
            ConvertInputArrayToCoursesList();
            VerifyIfEveryPrerequisiteWasAlsoIncludedAsACourse();
            VerifyNoCircularDependenciesExistBetweenCourses();
            PrintCoursesInTheOrderTheyShouldBeTaken();
        }

        private void VerifyInputArrayHasValidValuesForProcessing()
        {
            if (!_inputArgs.Any())
            {
                throw new Exception(ErrorMessages.ERROR_INPUT_WITH_ZERO_COURSES);
            }

            if (!_inputArgs.All(x => x.Contains(": ")))
            {
                throw new Exception(ErrorMessages.ERROR_INPUT_MISSING_COLON);
            }
        }

        private void ConvertInputArrayToCoursesList()
        {
            foreach (var coursePackage in _inputArgs)
            {                
                var indexOfColon = coursePackage.IndexOf(":", StringComparison.Ordinal);
                var courseName = coursePackage.Substring(0, indexOfColon);                

                if (_courses.Any(x => x.Name.Equals(courseName)))
                {
                    throw new Exception(string.Format(ErrorMessages.ERROR_INPUT_WITH_DUPLICATE_COURSES, courseName));
                }

                var prerequisite = coursePackage.Substring(indexOfColon + 2);

                var course = new Course(courseName, prerequisite);
                _courses.Add(course);
            }
        }

        private void VerifyIfEveryPrerequisiteWasAlsoIncludedAsACourse()
        {
            foreach (var course in _courses)
            {
                if (course.Prerequisite.Equals(string.Empty)) continue;

                var prerequisiteExistsAsCourseToo = _courses.Exists(x => x.Name == course.Prerequisite);

                if (!prerequisiteExistsAsCourseToo)
                {
                    throw new Exception(
                        string.Format(ErrorMessages.ERROR_INPUT_HAS_PREREQUISITE_THAT_WASNT_INCLUDED_AS_COURSE,
                            course.Prerequisite));
                }
            }
        }

        private void VerifyNoCircularDependenciesExistBetweenCourses()
        {
            foreach (var course in _courses)
            {
                _courseBeingCheckedForCircularDependency = course;

                SearchForCircularDependenciesInCoursesByTheirPrerequisites(course.Prerequisite);

                _courses.First(x => x.Name == course.Name).PrerequisitesWeight = _currentPrerequisiteWeight;
                _currentPrerequisiteWeight = 0;
            }

            _courseBeingCheckedForCircularDependency = null;
        }

        private void SearchForCircularDependenciesInCoursesByTheirPrerequisites(string prerequisite)
        {           
            if (prerequisite.Equals(string.Empty)) return;

            _currentPrerequisiteWeight++;

            if (_courseBeingCheckedForCircularDependency.Name == prerequisite)
            {
                throw new Exception(string.Format(ErrorMessages.ERROR_INPUT_WITH_CIRCULAR_DEPENDENCIES,
                    _courseBeingCheckedForCircularDependency.Name,
                    _courseBeingCheckedForCircularDependency.Prerequisite));
            }

            var course = _courses.First(x => x.Name == prerequisite);

            SearchForCircularDependenciesInCoursesByTheirPrerequisites(course.Prerequisite);
        }

        private void PrintCoursesInTheOrderTheyShouldBeTaken()
        {
            var sortedCourses = _courses.OrderBy(o => o.PrerequisitesWeight).ToList();

            var coursesInOrder = sortedCourses.Aggregate(string.Empty, (current, sortedCourse) => current + sortedCourse.Name + ", ");
            coursesInOrder = coursesInOrder.Substring(0, coursesInOrder.Length - 2);

            _outputWriter.WriteLine(coursesInOrder);
        }
    }
}