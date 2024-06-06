using Drastic.AppToolbox.Services;
using SandboxCore.ViewModels;
using Drastic.PureLayout;

namespace SandboxUIKit;

public class DebugViewController : UIViewController
{
    private DebugViewModel viewModel;
    private IAppDispatcher _dispatcher;
    private IErrorHandler _errorHandler;
    private IAsyncCommandFactory _asyncCommandFactory;
    private UIButton testButton;
    private PropertyBinder<DebugViewModel> Binder;
    public DebugViewController()
    {
        this._dispatcher = new UIKitAppDispatcher();
        this._errorHandler = new UIKitErrorHandler();
        this._asyncCommandFactory = new AsyncCommandFactory(this._dispatcher, this._errorHandler);
        this.viewModel = new DebugViewModel(this._dispatcher, this._errorHandler, this._asyncCommandFactory);
        this.testButton = new UIButton(UIButtonType.System);
        Binder = new PropertyBinder<DebugViewModel>(this.viewModel);
        Binder.BindButton(this.testButton, "Test Command", this.viewModel.TestCommand);
    }

    public override void ViewDidLoad()
    {
        this.View!.AddSubview(this.testButton);
        this.testButton.AutoCenterInSuperview();
    }
}