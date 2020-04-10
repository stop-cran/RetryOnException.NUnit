using NUnit.Framework;
using NUnit.Framework.Interfaces;
using NUnit.Framework.Internal.Commands;
using System;

namespace RetryOnException.NUnit
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true, Inherited = false)]
    public sealed class RetryOnAttribute : NUnitAttribute, IRepeatTest
    {
        private readonly Type exceptionType;
        private readonly int retryCount;

        public RetryOnAttribute(Type exceptionType, int retryCount)
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

        public TestCommand Wrap(TestCommand command) =>
            new RetryOnCommand(command, exceptionType, retryCount);
    }
}
