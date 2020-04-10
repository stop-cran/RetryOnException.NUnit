using NUnit.Framework;
using System;

namespace RetryOnException.NUnit.Tests
{
    public class Tests
    {
        [Test, RetryOn(typeof(Exception), 1)]
        public void ShouldPass()
        { }

        [Test, RetryOn(typeof(ApplicationException), 2)]
        public void ShouldRetry()
        {
            if (TestContext.CurrentContext.CurrentRepeatCount == 0)
                throw new ApplicationException();
        }
    }
}