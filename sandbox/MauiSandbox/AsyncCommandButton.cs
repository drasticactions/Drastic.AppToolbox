using System.Windows.Input;
using Drastic.AppToolbox.Commands;

namespace MauiSandbox;

public class AsyncCommandButton : Button
{
    public static readonly BindableProperty AsyncCommandProperty = BindableProperty.Create(nameof(AsyncCommand), typeof(ICommand), typeof(AsyncCommandButton), null, propertyChanged: OnCommandChanged);

    public static void OnCommandChanged(BindableObject bindable, object oldvalue, object newvalue)
    {
        var button = (AsyncCommandButton)bindable;
        if (newvalue is IAsyncCommand command)
        {
            button.BindingContext = command;
            button.SetBinding(Button.CommandProperty, new Binding(".", source: command));
            button.SetBinding(Button.TextProperty, nameof(IAsyncCommand.Title));
        }
    }
    
    public IAsyncCommand AsyncCommand
    {
        get => (IAsyncCommand)this.GetValue(AsyncCommandProperty);

        set
        {
            this.SetValue(AsyncCommandProperty, value);
        }
    }
    
    private void OnCommandChanged()
    {
        if (this.AsyncCommand != null)
        {
            this.SetBinding(Button.CommandProperty, new Binding(nameof(AsyncCommand), source: this.AsyncCommand));
            this.SetBinding(Button.TextProperty, new Binding(nameof(IAsyncCommand.Title), source: this.AsyncCommand));
        }
    }
}