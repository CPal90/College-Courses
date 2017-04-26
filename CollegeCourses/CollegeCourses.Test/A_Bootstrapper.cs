using NUnit.Framework;
using Rhino.Mocks;
using System;

namespace CollegeCourses.Test
{
    [TestFixture]
    public class A_Bootstrapper
    {
        private Bootstrapper _bootstrapper;
        private IOutputWriter _outputWriter;

        [SetUp]
        protected void Init()
        {
            _outputWriter = MockRepository.GenerateMock<IOutputWriter>();

            _bootstrapper = new Bootstrapper(_outputWriter);
        }        

        [Test]
        public void CanBeCreated()
        {
            Assert.IsNotNull(_bootstrapper);
            Assert.IsInstanceOf<Bootstrapper>(_bootstrapper);
        }
        
        [Test]
        public void ThrowsErrorIfInputArrayHasZeroCourses()
        {
            var invalidArgs = new string[0];

            var ex = Assert.Throws<Exception>(() => _bootstrapper.Start(invalidArgs));

            Assert.AreEqual(ex.Message, ErrorMessages.ERROR_INPUT_WITH_ZERO_COURSES);
        }

        [Test]
        public void ThrowsErrorIfCourseIsMissingSpaceBeforeThePrerequisite()
        {
            var invalidArgs = new[]
            {
                "Introduction to Paper Airplanes:Prerequisite",
                "Advanced Throwing Techniques: Introduction to Paper Airplanes"
            };

            var ex = Assert.Throws<Exception>(() => _bootstrapper.Start(invalidArgs));

            Assert.AreEqual(ex.Message, ErrorMessages.ERROR_INPUT_MISSING_COLON);
        }

        [Test]
        public void ThrowsErrorIfCourseIsMissingColon()
        {
            var invalidArgs = new[]
            {
                "Introduction to Paper Airplanes Prerequisite",
                "Advanced Throwing Techniques: Introduction to Paper Airplanes"
            };
            
            var ex = Assert.Throws<Exception>(() => _bootstrapper.Start(invalidArgs));

            Assert.AreEqual(ex.Message, ErrorMessages.ERROR_INPUT_MISSING_COLON);
        }

        [Test]
        public void ThrowsErrorIfInputArrayIsHasDuplicateCourses()
        {
            var invalidArgs = new[]
            {
                "Introduction to Paper Airplanes: ",
                "Advanced Throwing Techniques: Introduction to Paper Airplanes",
                "Introduction to Paper Airplanes: Prerequisite"
            };
            
            var ex = Assert.Throws<Exception>(() => _bootstrapper.Start(invalidArgs));

            Assert.AreEqual(ex.Message,
                string.Format(ErrorMessages.ERROR_INPUT_WITH_DUPLICATE_COURSES,
                    "Introduction to Paper Airplanes"));
        }

        [Test]
        public void ThrowsErrorIfAPrerequisiteIsNotIncludedInTheInputArrayAsACourse()
        {
            var invalidArgs = new[]
            {
                "Introduction to Paper Airplanes Prerequisite: Non-existent prerequisite",
                "Advanced Throwing Techniques: Introduction to Paper Airplanes"
            };            

            var ex = Assert.Throws<Exception>(() => _bootstrapper.Start(invalidArgs));

            Assert.AreEqual(ex.Message,
                string.Format(ErrorMessages.ERROR_INPUT_HAS_PREREQUISITE_THAT_WASNT_INCLUDED_AS_COURSE,
                    "Non-existent prerequisite"));
        }
       
        [Test]
        public void ThrowsErrorIfInputArrayHasAnyDirectCircularDependencyBetweenCourses()
        {
            var invalidArgs = new[]
            {
                "Introduction to Paper Airplanes: Advanced Throwing Techniques",
                "Advanced Throwing Techniques: Introduction to Paper Airplanes"
            };

            var ex = Assert.Throws<Exception>(() => _bootstrapper.Start(invalidArgs));

            Assert.AreEqual(ex.Message,
                string.Format(ErrorMessages.ERROR_INPUT_WITH_CIRCULAR_DEPENDENCIES,
                    "Introduction to Paper Airplanes", "Advanced Throwing Techniques"));

        }

        [Test]
        public void ThrowsErrorIfInputArrayHasAnyIndirectCircularDependencyBetweenCourses()
        {
            var invalidArgs = new[]
            {
                "Intro to Arguing on the Internet: Godwin's Law",
                "Understanding Circular Logic: Intro to Arguing on the Internet",
                "Godwin's Law: Understanding Circular Logic"
            };

            var ex = Assert.Throws<Exception>(() => _bootstrapper.Start(invalidArgs));

            Assert.AreEqual(ex.Message,
                string.Format(ErrorMessages.ERROR_INPUT_WITH_CIRCULAR_DEPENDENCIES,
                    "Intro to Arguing on the Internet", "Godwin's Law"));
        }

        [Test]
        public void PrintsListOfCoursesInTheOrderTheyShouldBeTaken()
        {
            var validArgs = new[]
            {
                "Introduction to Paper Airplanes: ",
                "Advanced Throwing Techniques: Introduction to Paper Airplanes",
                "History of Cubicle Siege Engines: Rubber Band Catapults 101",
                "Advanced Office Warfare: History of Cubicle Siege Engines",
                "Rubber Band Catapults 101: ",
                "Paper Jet Engines: Introduction to Paper Airplanes"
            };

            _outputWriter
                .Expect(x => x.WriteLine(
                    "Introduction to Paper Airplanes, Rubber Band Catapults 101, Advanced Throwing Techniques, History of Cubicle Siege Engines, Paper Jet Engines, Advanced Office Warfare"));

            _bootstrapper.Start(validArgs);

            _outputWriter.VerifyAllExpectations();
        }
    }
}