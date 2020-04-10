# Overview [![NuGet](https://img.shields.io/nuget/v/RetryOnException.NUnit.svg)](https://www.nuget.org/packages/RetryOnException.NUnit) [![Actions Status](https://github.com/stop-cran/RetryOnException.NUnit/workflows/.NET%20Core/badge.svg)](https://github.com/stop-cran/RetryOnException.NUnit/actions)

An attribute to retry NUnit tests in case of an exception of a given type has been thrown.

# Installation

NuGet package is available [here](https://www.nuget.org/packages/RetryOnException.NUnit/).

```PowerShell
PM> Install-Package RetryOnException.NUnit
```

# Example

```C#
// Fails
[Test, RetryOn(typeof(ApplicationException), 2)]
public void ShouldRetry()
{
    throw new ApplicationException();
}

// Fails
[Test, RetryOn(typeof(SystemException), 2)]
public void ShouldRetry()
{
    if (TestContext.CurrentContext.CurrentRepeatCount == 0)
        throw new ApplicationException();
}

// Succeeds
[Test, RetryOn(typeof(ApplicationException), 2)]
public void ShouldRetry()
{
    if (TestContext.CurrentContext.CurrentRepeatCount == 0)
        throw new ApplicationException();
}

// Succeeds
[Test]
[RetryOn(typeof(ApplicationException), 2)]
[RetryOn(typeof(SystemException), 2)]
public void ShouldRetry()
{
    if (TestContext.CurrentContext.CurrentRepeatCount == 0)
        throw new ApplicationException();
}
```

# Remarks

NUnit `Retry` attribute does not retry on exceptions. See e.g. this [discussion](https://github.com/nunit/nunit/issues/2785). Vice versa `RetryOn` attribute does not retry on failed assertions. It is proposed only to make a concise test code with retrying on intermittent exceptions. Avoid intermittent exceptions in tests and use `Assert.Throws` whenever possible.