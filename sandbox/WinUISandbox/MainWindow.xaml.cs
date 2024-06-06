using Drastic.AppToolbox.Services;
using Microsoft.UI.Dispatching;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using SandboxCore.ViewModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace WinUISandbox
{
    /// <summary>
    /// An empty window that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainWindow : Window
    {
        private DebugViewModel viewModel;
        private IAppDispatcher dispatcher;
        private IErrorHandler errorHandler;
        private IAsyncCommandFactory asyncCommandFactory;

        public MainWindow()
        {
            this.InitializeComponent();
            var dispatcherQueue = DispatcherQueue.GetForCurrentThread();
            dispatcher = new WinUIAppDispatcher(dispatcherQueue);
            errorHandler = new WinUIErrorHandler();
            asyncCommandFactory = new AsyncCommandFactory(dispatcher, errorHandler);
            viewModel = new DebugViewModel(dispatcher, errorHandler, asyncCommandFactory);
        }

        public DebugViewModel ViewModel => viewModel;
    }

    public class WinUIAppDispatcher : IAppDispatcher
    {
        private readonly DispatcherQueue dispatcherQueue;

        public WinUIAppDispatcher(DispatcherQueue dispatcherQueue)
        {
            this.dispatcherQueue = dispatcherQueue ?? throw new ArgumentNullException(nameof(dispatcherQueue));
        }

        public bool Dispatch(Action action)
        {
            _ = action ?? throw new ArgumentNullException(nameof(action));
            return this.dispatcherQueue.TryEnqueue(() => action());
        }
    }

    public class WinUIErrorHandler : IErrorHandler
    {
        public void HandleError(Exception ex)
        {
        }
    }
}
