using System.Diagnostics;
using Drastic.AppToolbox.Services;

namespace MauiSandbox;

public class MauiErrorHandler : IErrorHandler
{
    public void HandleError(Exception ex)
    {
        Debugger.Break();
    }
}