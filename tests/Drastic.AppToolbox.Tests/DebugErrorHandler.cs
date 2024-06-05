namespace Drastic.AppToolbox.Tests;

/// <summary>
/// Debug Error Handler.
/// </summary>
public class DebugErrorHandler : IErrorHandler
{
    /// <inheritdoc/>
    public void HandleError(Exception ex)
    {
        Assert.IsInstanceOfType<ExpectedErrorException>(ex);
    }
}