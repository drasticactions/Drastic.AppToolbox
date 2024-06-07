using System.ComponentModel;
using CoreAnimation;
using Drastic.AppToolbox.Commands;
using Drastic.AppToolbox.Services;
using SandboxCore.ViewModels;
using Drastic.PureLayout;

namespace SandboxUIKit;

public class ButtonProgressBar : UIButton
{
    private float cornerRadius = 5;
    public float Progress { get; private set; } = 0.0f;
    public bool Indeterminate { get; set; } = false;

    private CAShapeLayer progressLayer = new CAShapeLayer();
    private UIColor progressColor = UIColor.FromRGB(0, 140, 245);

    private NSTimer timer;

    public ButtonProgressBar() : base()
    {
        Initialize();
    }
    public ButtonProgressBar(CGRect frame) : base(frame)
    {
        Initialize();
    }

    public ButtonProgressBar(IntPtr handle) : base(handle)
    {
        Initialize();
    }

    private void Initialize()
    {
        Layer.CornerRadius = cornerRadius;
        Layer.MasksToBounds = true;
        BackgroundColor = UIColor.FromRGB(50, 100, 200);

        TitleLabel.TextAlignment = UITextAlignment.Center;
        TitleLabel.TextColor = UIColor.White;
        TitleLabel.Font = UIFont.BoldSystemFontOfSize(0);

        ImageView.ContentMode = UIViewContentMode.Center;
        ImageView.TintColor = UIColor.White;
        HideImage(true);

        var rectanglePath = UIBezierPath.FromRect(new CGRect(0, 0, Frame.Width, Frame.Height));

        progressLayer.Path = rectanglePath.CGPath;
        progressLayer.FillColor = UIColor.Clear.CGColor;
        progressLayer.StrokeColor = progressColor.CGColor;

        progressLayer.StrokeEnd = 0.0f;
        progressLayer.LineWidth = Frame.Height * 2;

        Layer.AddSublayer(progressLayer);
        BringSubviewToFront(TitleLabel);
        BringSubviewToFront(ImageView);
    }

    public void StartIndeterminate(double time = 2.0, double padding = 0.5)
    {
        timer?.Invalidate();
        ResetProgress();
        timer = NSTimer.CreateScheduledTimer(time, this, new ObjCRuntime.Selector("AnimateIndeterminate:"), NSNumber.FromDouble(padding), true);
        timer.Fire();
        NSRunLoop.Current.AddTimer(timer, NSRunLoopMode.Default);
    }

    [Export("AnimateIndeterminate:")]
    private void AnimateIndeterminate(NSTimer sender)
    {
        var va = (NSNumber)NSNumber.FromObject(sender.UserInfo);
        var time = sender.TimeInterval - va.DoubleValue;
        var stroke = CABasicAnimation.FromKeyPath("strokeEnd");
        stroke.From = NSNumber.FromFloat(0.0f);
        stroke.To = NSNumber.FromFloat(0.5f);
        stroke.Duration = time;
        stroke.FillMode = CAFillMode.Forwards;
        stroke.RemovedOnCompletion = false;
        stroke.TimingFunction = CAMediaTimingFunction.FromControlPoints(1, 0, 1, 1);
        progressLayer.AddAnimation(stroke, null);
    }

    public void StopIndeterminate()
    {
        timer?.Invalidate();
    }

    public void ResetProgress()
    {
        HideImage(true);
        HideTitle(false);
        SetProgress(0.0f, false);
    }

    public override void LayoutSubviews()
    {
        base.LayoutSubviews();
        TitleLabel.Frame = Bounds;
        TitleLabel.Font = TitleLabel.Font.WithSize(TitleLabel.Frame.Height * 0.45f);
        ImageView.Frame = Bounds;
    }

    public void SetProgress(float progress, bool animated)
    {
        // progress passed in is 0 to 100, we need to convert it to 0 to 1
        progress = progress / 100;
        if (!animated)
        {
            progressLayer.StrokeEnd = progress / 2;
        }
        else
        {
            var stroke = CABasicAnimation.FromKeyPath("strokeEnd");
            stroke.From = NSNumber.FromFloat(Progress);
            stroke.To = NSNumber.FromFloat(progress);
            stroke.FillMode = CAFillMode.Forwards;
            stroke.RemovedOnCompletion = false;
            stroke.TimingFunction = CAMediaTimingFunction.FromName(CAMediaTimingFunction.Linear);
            progressLayer.AddAnimation(stroke, null);
        }
        Progress = progress;
    }

    public override void SetTitle(string title, UIControlState state)
    {
        base.SetTitle(title, state);
    }

    public void HideTitle(bool hidden)
    {
        TitleLabel.Layer.Opacity = hidden ? 0.0f : 1.0f;
    }

    public void HideImage(bool hidden)
    {
        if (hidden)
        {
            ImageView.Layer.RemoveAllAnimations();
            ImageView.Layer.Transform = CATransform3D.MakeScale(0.0f, 0.0f, 0.0f);
        }
        else
        {
            var completionAnim = CABasicAnimation.FromKeyPath("transform");
            completionAnim.From = NSValue.FromCATransform3D(CATransform3D.MakeScale(0.0f, 0.0f, 0.0f));
            completionAnim.To = NSValue.FromCATransform3D(CATransform3D.Identity);
            completionAnim.FillMode = CAFillMode.Forwards;
            completionAnim.RemovedOnCompletion = false;
            completionAnim.TimingFunction = CAMediaTimingFunction.FromName(CAMediaTimingFunction.Linear);
            ImageView.Layer.AddAnimation(completionAnim, null);
        }
    }

    public void TriggerCompletion()
    {
        StopIndeterminate();
        SetProgress(1.0f, true);
        HideTitle(true);
        HideImage(false);
    }

    public void SetCompletionImage(UIImage image)
    {
        SetImage(image.ImageWithRenderingMode(UIImageRenderingMode.AlwaysTemplate), UIControlState.Normal);
    }

    public void SetProgressColor(UIColor color)
    {
        progressColor = color;
        progressLayer.StrokeColor = color.CGColor;
    }

    public void SetBackgroundColor(UIColor color)
    {
        BackgroundColor = color;
    }

    public override void TouchesBegan(NSSet touches, UIEvent evt)
    {
        base.TouchesBegan(touches, evt);
        UIView.Animate(0.05, () => {
            TitleLabel.Transform = CGAffineTransform.MakeScale(1.05f, 1.05f);
            Alpha = 0.85f;
        });
    }

    public override void TouchesEnded(NSSet touches, UIEvent evt)
    {
        base.TouchesEnded(touches, evt);
        UIView.Animate(0.1, () => {
            TitleLabel.Transform = CGAffineTransform.MakeScale(1.0f, 1.0f);
            Alpha = 1.0f;
        });
    }
}

public sealed class AsyncCommandUIButton : UIView, IDisposable
{
    public UIButton Button { get; }
    public IAsyncCommand Command { get; }
    public object? Parameter { get; }

    public AsyncCommandUIButton(IAsyncCommand command, object? parameter = default) : base()
    {
        // this.Button = new ButtonProgressBar();
        this.Button = UIButton.FromType(UIButtonType.System);
        this.Command = command;
        this.Command.PropertyChanged += CommandOnPropertyChanged;
        this.Parameter = parameter;
        this.Button.SetTitle(command.Title, UIControlState.Normal);
        this.Button.PrimaryActionTriggered += (s, e) => this.Command.Execute(this.Parameter);
        this.AddSubview(this.Button);
        this.Button.AutoPinEdgesToSuperviewEdges();
    }

    private void CommandOnPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        var command = (IAsyncCommand)sender!;
        System.Diagnostics.Debug.WriteLine($"{e.PropertyName}");
        switch (e.PropertyName)
        {
            case "Title":
                this.Button.SetTitle(command.Title, UIControlState.Normal);
                break;
            case "IsBusy":
                this.Button.Enabled = command.CanExecute();
                break;
            case "Progress":
                if (this.Button is ButtonProgressBar progressBar)
                {
                    progressBar.SetProgress(command.Progress, true);
                }
                break;
        }
    }

    protected override void Dispose(bool disposing)
    {
        base.Dispose(disposing);
        this.Button.Dispose();
        this.Command.PropertyChanged -= CommandOnPropertyChanged;
    }
}

public class DebugViewController : UIViewController
{
    private DebugViewModel viewModel;
    private IAppDispatcher _dispatcher;
    private IErrorHandler _errorHandler;
    private IAsyncCommandFactory _asyncCommandFactory;
    private AsyncCommandUIButton testButton;
    public DebugViewController()
    {
        this._dispatcher = new UIKitAppDispatcher();
        this._errorHandler = new UIKitErrorHandler();
        this._asyncCommandFactory = new AsyncCommandFactory(this._dispatcher, this._errorHandler);
        this.viewModel = new DebugViewModel(this._dispatcher, this._errorHandler, this._asyncCommandFactory);
        this.testButton = new AsyncCommandUIButton(this.viewModel.IncreaseProgressCommand, null);
    }

    public override void ViewDidLoad()
    {
        this.View!.AddSubview(this.testButton);
        this.testButton.AutoCenterInSuperview();
    }
}