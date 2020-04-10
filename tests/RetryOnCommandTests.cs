using Moq;
using NUnit.Framework;
using NUnit.Framework.Internal;
using NUnit.Framework.Internal.Commands;
using Shouldly;
using System;

namespace RetryOnException.NUnit.Tests
{
    public class RetryOnCommandTests
    {
        [Test]
        public void ShouldCatchException()
        {
            (var context, var innerCommand, var testResult) = Arrange();

            innerCommand.Setup(c => c.Execute(It.IsAny<TestExecutionContext>()))
                .Throws<SystemException>();

            new RetryOnCommand(innerCommand.Object, typeof(SystemException), 1)
                .Execute(context)
                .ShouldBeSameAs(testResult);
        }

        [Test]
        public void ShouldCatchNUnitException()
        {
            (var context, var innerCommand, var testResult) = Arrange();

            innerCommand.Setup(c => c.Execute(It.IsAny<TestExecutionContext>()))
                .Throws(new NUnitException("a message", new SystemException()));

            new RetryOnCommand(innerCommand.Object, typeof(SystemException), 1)
                .Execute(context)
                .ShouldBeSameAs(testResult);
        }

        [Test]
        public void ShouldNotCatchOtherException()
        {
            (var context, var innerCommand, var testResult) = Arrange();

            innerCommand.Setup(c => c.Execute(It.IsAny<TestExecutionContext>()))
                .Throws<ApplicationException>();

            Should.Throw<ApplicationException>(() =>
            new RetryOnCommand(innerCommand.Object, typeof(SystemException), 1)
                    .Execute(context));
        }

        [Test]
        public void ShouldNotCatchOtherNUnitException()
        {
            (var context, var innerCommand, var testResult) = Arrange();

            innerCommand.Setup(c => c.Execute(It.IsAny<TestExecutionContext>()))
                .Throws(new NUnitException("a message", new ApplicationException()));

            Should.Throw<NUnitException>(() =>
                new RetryOnCommand(innerCommand.Object, typeof(SystemException), 1)
                    .Execute(context));
        }

        private (TestExecutionContext context, Mock<TestCommand> innerCommand, TestResult testResult) Arrange()
        {
            var test = new Mock<Test>("some test");
            var innerCommand = new Mock<TestCommand>(test.Object);
            var testResult = new Mock<TestResult>(test.Object);

            test.Setup(t => t.MakeTestResult()).Returns(testResult.Object);

            return (new TestExecutionContext
            {
                CurrentTest = test.Object
            },
                innerCommand,
                testResult.Object);
        }
    }
}
