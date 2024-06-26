using Drastic.AppToolbox.Services;

namespace MauiSandbox;

public class MauiAppDispatcher : IAppDispatcher
{
    public bool Dispatch(Action action)
    {
        return Microsoft.Maui.Controls.Application.Current!.Dispatcher.Dispatch(action);
    }
}