namespace Drastic.AppToolbox.Tests;

/// <summary>
/// Expected Error Exception.
/// </summary>
public class ExpectedErrorException : Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ExpectedErrorException"/> class.
    /// </summary>
    public ExpectedErrorException()
        : base("Expected Error")
    {
    }
}