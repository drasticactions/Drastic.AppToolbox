namespace Drastic.AppToolbox.Tests;

/// <summary>
/// Debug App Dispatcher.
/// </summary>
public class DebugAppDispatcher : IAppDispatcher
{
    /// <inheritdoc/>
    public bool Dispatch(Action action)
    {
        action.Invoke();
        return true;
    }
}