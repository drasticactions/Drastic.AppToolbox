using Drastic.AppToolbox.Commands;
using Drastic.AppToolbox.Services;
using Drastic.AppToolbox.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SandboxCore.ViewModels;

public class DebugViewModel : BaseViewModel
{
    private AsyncCommand testCommand;
    private AsyncCommand cancelTestCommand;
    private AsyncCommand increaseProgressCommand;

    public DebugViewModel(IAppDispatcher dispatcher, IErrorHandler errorHandler, IAsyncCommandFactory asyncCommandFactory)
        : base(dispatcher, errorHandler, asyncCommandFactory)
    {
        this.Title = "Tests";
        testCommand = (AsyncCommand)asyncCommandFactory.Create("Test Command", async (x, y, z) =>
        {
            this.Title = "Test Command Running 2";
            this.testCommand!.UpdateTitle("Test Command Running");
            await Task.Delay(5000);
        });

        increaseProgressCommand = (AsyncCommand)asyncCommandFactory.Create("Increase Progress", async (x, y, z) =>
        {
            for (int i = 0; i <= 100; i++)
            {
                this.IncreaseProgressCommand.UpdateTitle($"Progress: {i}%");
                await Task.Delay(100);
                y.Report(i);
            }
        });

        cancelTestCommand = new AsyncCommand("Cancel Test Command", async (x, y, z) =>
        {
            this.cancelTestCommand!.UpdateTitle("Cancel");
            await Task.Delay(5000);
        }, this.Dispatcher, this.ErrorHandler, blockWhileExecuting: false);
    }

    public AsyncCommand CancelTestCommand => cancelTestCommand;

    public AsyncCommand TestCommand => testCommand;

    public AsyncCommand IncreaseProgressCommand => increaseProgressCommand;
}
