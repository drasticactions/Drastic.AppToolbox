using System.Diagnostics;
using Drastic.AppToolbox.Services;

namespace SandboxUIKit;

public class UIKitErrorHandler : IErrorHandler
{
    public void HandleError(Exception ex)
    {
        Debugger.Break();
    }
}