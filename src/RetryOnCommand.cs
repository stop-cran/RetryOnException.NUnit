using NUnit.Framework.Interfaces;
using NUnit.Framework.Internal;
using NUnit.Framework.Internal.Commands;
using System;

namespace RetryOnException.NUnit
{
    public sealed class RetryOnCommand : DelegatingTestCommand
    {
        private readonly Type exceptionType;
        private readonly int retryCount;

        public RetryOnCommand(TestCommand innerCommand, Type exceptionType, int retryCount)
            : base(innerCommand)
        {
            if (!typeof(Exception).IsAssignableFrom(exceptionType))
                throw new ArgumentException(
                    "exceptionType should be Exception-inherited.",
                    nameof(exceptionType));
            if (retryCount <= 0)
                throw new ArgumentOutOfRangeException(nameof(retryCount));

            this.exceptionType = exceptionType;
            this.retryCount = retryCount;
        }

        public override TestResult Execute(TestExecutionContext context)
        {
            int count;

            for (count = 0; count <= retryCount; count++)
                try
                {
                    context.CurrentResult = innerCommand.Execute(context);

                    if (count > 0)
                        context.CurrentResult.OutWriter
                            .WriteLine($"The test succeeded after {count} retries.");

                    return context.CurrentResult;
                }
                catch (Exception ex)
                when (exceptionType.IsAssignableFrom(ex.GetType()) ||
                    ex is NUnitException && exceptionType.IsAssignableFrom(ex.InnerException.GetType()))
                {
                    if (ex is NUnitException)
                        ex = ex.InnerException;

                    var result = context.CurrentTest.MakeTestResult();

                    result.SetResult(ResultState.Failure, ex.Message, ex.StackTrace);

                    context.CurrentResult = result;
                    context.CurrentRepeatCount++;
                }

            context.CurrentResult.OutWriter.WriteLine($"The test failed after {count} retries.");

            return context.CurrentResult;
        }
    }
}
