// <copyright file="ViewModelTests.cs" company="Drastic Actions">
// Copyright (c) Drastic Actions. All rights reserved.
// </copyright>

namespace Drastic.AppToolbox.Tests;

[TestClass]
public class ViewModelTests
{
    private static IAppDispatcher dispatcher = new DebugAppDispatcher();
    private static IErrorHandler errorHandler = new DebugErrorHandler();
    private static IAsyncCommandFactory asyncCommandFactory = new AsyncCommandFactory(dispatcher, errorHandler);

    [TestMethod]
    public void ViewModel_IsBusy()
    {
        var vm = new DebugViewModel(dispatcher, errorHandler, asyncCommandFactory);
        Assert.IsNotNull(vm);
        Assert.IsFalse(vm.IsBusy);
        Assert.IsFalse(vm.PerformBusyAsyncTaskDelayTestCommand.CanExecute(1000));
        vm.CanExecute = true;
        Assert.IsTrue(vm.PerformBusyAsyncTaskDelayTestCommand.CanExecute(1000));
        vm.PerformBusyAsyncTaskDelayTestCommand.Execute(1000);
    }

    class DebugViewModel : BaseViewModel
    {
        private AsyncCommandFactory<int> intCommandFactory;
        private bool canExecute = false;

        public DebugViewModel(IAppDispatcher dispatcher, IErrorHandler errorHandler, IAsyncCommandFactory asyncCommandFactory)
            : base(dispatcher, errorHandler, asyncCommandFactory)
        {
            this.intCommandFactory = new AsyncCommandFactory<int>(dispatcher, errorHandler);
            this.PerformBusyAsyncTaskDelayTestCommand = this.intCommandFactory.Create(this.PerformBusyAsyncTaskDelay, (x) => this.CanExecute);
        }

        public bool CanExecute
        {
            get => this.canExecute;
            set => this.SetProperty(ref this.canExecute, value, true);
        }

        public IAsyncCommand<int> PerformBusyAsyncTaskDelayTestCommand { get; }

        private Task PerformBusyAsyncTaskDelay(int delay)
            => this.PerformBusyAsyncTask(async () =>
            {
                // Should be run within PerformBusyAsyncTask
                Assert.IsTrue(this.IsBusy);
                await Task.Delay(delay);
            });
    }
}