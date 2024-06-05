using Drastic.AppToolbox.Commands;
using Drastic.AppToolbox.Services;
using Drastic.AppToolbox.ViewModels;

namespace MauiSandbox;

public partial class MainPage : ContentPage
{
	private IAppDispatcher dispatcher;
	private IErrorHandler errorHandler;
	private IAsyncCommandFactory asyncCommandFactory;
	private AsyncCommand testCommand;
	public MainPage()
	{
		InitializeComponent();
		this.dispatcher = new MauiAppDispatcher();
		this.errorHandler = new MauiErrorHandler();
		this.asyncCommandFactory = new AsyncCommandFactory(this.dispatcher, this.errorHandler);
		this.BindingContext = this.ViewModel = new DebugViewModel(this.dispatcher, this.errorHandler, this.asyncCommandFactory);
	}
	
	public DebugViewModel ViewModel { get; set; }
}

public class DebugViewModel : BaseViewModel
{
	private IAsyncCommand testCommand;
	public DebugViewModel(
		IAppDispatcher dispatcher,
		IErrorHandler errorHandler,
		IAsyncCommandFactory asyncCommandFactory)
		: base(dispatcher, errorHandler, asyncCommandFactory)
	{
		this.testCommand = asyncCommandFactory.Create("Test Command", async (ct, p, t) =>
		{
			await Task.Delay(1000);
		});
	}

	public IAsyncCommand TestCommand => this.testCommand;
}

