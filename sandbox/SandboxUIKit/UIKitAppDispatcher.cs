using CoreFoundation;
using Drastic.AppToolbox.Services;

namespace SandboxUIKit;

public class UIKitAppDispatcher : NSObject, IAppDispatcher
{
    public bool Dispatch(Action action)
    {
        DispatchQueue.MainQueue.DispatchAsync(action.Invoke);
        return true;
    }
}