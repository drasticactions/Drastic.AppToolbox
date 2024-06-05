namespace Drastic.AppToolbox.Tests;

/// <summary>
/// Debug Error Handler.
/// </summary>
public class DebugErrorHandler : IErrorHandler
{
    private List<Type> expectedTypes;

    public DebugErrorHandler(List<Type>? types = default)
    {
        this.expectedTypes = types ?? new();
    }

    /// <inheritdoc/>
    public void HandleError(Exception ex)
    {
        if (this.expectedTypes.Contains(ex.GetType()))
        {
            return;
        }

        if (ex is ExpectedErrorException)
        {
            return;
        }

        Assert.Fail($"Got {ex.GetType()}, which was not expected.");
    }
}