// <copyright file="BaseViewModel.cs" company="Drastic Actions">
// Copyright (c) Drastic Actions. All rights reserved.
// </copyright>

using System.ComponentModel;
using System.Runtime.CompilerServices;
using Drastic.AppToolbox.Services;

namespace Drastic.AppToolbox.ViewModels;

/// <summary>
/// Base View Model.
/// </summary>
public abstract class BaseViewModel : INotifyPropertyChanged
{
    private bool isBusy;
    private string title = string.Empty;
    private string isLoadingText = string.Empty;

    /// <summary>
    /// Initializes a new instance of the <see cref="BaseViewModel"/> class.
    /// </summary>
    /// <param name="dispatcher"><see cref="IAppDispatcher"/>.</param>
    /// <param name="errorHandler"><see cref="IErrorHandler"/>.</param>
    /// <param name="asyncCommandFactory"><see cref="IAsyncCommandFactory"/>.</param>
    public BaseViewModel(IAppDispatcher dispatcher, IErrorHandler errorHandler, IAsyncCommandFactory asyncCommandFactory)
    {
        this.Dispatcher = dispatcher;
        this.ErrorHandler = errorHandler;
        this.CommandFactory = asyncCommandFactory;
    }

    /// <inheritdoc/>
    public event PropertyChangedEventHandler? PropertyChanged;

    /// <summary>
    /// Gets the Dispatcher.
    /// </summary>
    public IAppDispatcher Dispatcher { get; }

    /// <summary>
    /// Gets the Command Factory.
    /// </summary>
    public IAsyncCommandFactory CommandFactory { get; }

    /// <summary>
    /// Gets the Error Handler.
    /// </summary>
    public IErrorHandler ErrorHandler { get; }

    /// <summary>
    /// Gets a value indicating whether the VM is busy.
    /// </summary>
    public bool IsBusy
    {
        get => this.isBusy;
        private set => this.SetProperty(ref this.isBusy, value);
    }

    /// <summary>
    /// Gets or sets the Is loading text.
    /// </summary>
    public string IsLoadingText
    {
        get => this.isLoadingText;
        set => this.SetProperty(ref this.isLoadingText, value);
    }

    /// <summary>
    /// Gets or sets the title.
    /// </summary>
    public string Title
    {
        get => this.title;
        set => this.SetProperty(ref this.title, value);
    }

    /// <summary>
    /// Performs an Async task while setting the <see cref="IsBusy"/> variable.
    /// If the task throws, it is handled by <see cref="ErrorHandler"/>.
    /// </summary>
    /// <param name="action">Task to run.</param>
    /// <param name="loadingText">Optional Is Loading text.</param>
    /// <param name="cancellationToken">Optional cancellation token.</param>
    /// <returns>Task.</returns>
    public async Task PerformBusyAsyncTask(Func<Task> action, string loadingText = "", CancellationToken? cancellationToken = default)
    {
        this.IsLoadingText = loadingText;
        this.IsBusy = true;

        try
        {
            // Start the action task
            var actionTask = action.Invoke();
            var token = cancellationToken ?? CancellationToken.None;

            // Create a task that completes when the cancellation token is triggered
            var cancellationTask = Task.Delay(Timeout.Infinite, token);

            // Await the first task to complete: either the action task or the cancellation task
            var completedTask = await Task.WhenAny(actionTask, cancellationTask);

            if (completedTask == cancellationTask)
            {
                // If the cancellation task completed first, throw an OperationCanceledException
                throw new OperationCanceledException(token);
            }

            // Await the action task to propagate any exceptions
            await actionTask;
        }
        catch (OperationCanceledException)
        {
            // Handle the cancellation specifically if needed
            // Optionally log or notify about the cancellation
        }
        catch (Exception ex)
        {
            // Handle other exceptions
            this.ErrorHandler.HandleError(ex);
        }
        finally
        {
            // Ensure that these properties are reset even if an exception occurs
            this.IsBusy = false;
            this.IsLoadingText = string.Empty;
        }
    }

    /// <summary>
    /// Called when wanting to raise a Command Can Execute.
    /// </summary>
    public virtual void RaiseCanExecuteChanged()
    {
    }

#pragma warning disable SA1600 // Elements should be documented
    protected bool SetProperty<T>(ref T backingStore, T value,  bool raiseCanExecuteChanged = false, [CallerMemberName] string propertyName = "", Action? onChanged = null)
#pragma warning restore SA1600 // Elements should be documented
    {
        if (EqualityComparer<T>.Default.Equals(backingStore, value))
        {
            return false;
        }

        backingStore = value;
        onChanged?.Invoke();
        this.OnPropertyChanged(propertyName);
        if (raiseCanExecuteChanged)
        {
            this.Dispatcher.Dispatch(() => this.RaiseCanExecuteChanged());
        }

        return true;
    }

    /// <summary>
    /// On Property Changed.
    /// </summary>
    /// <param name="propertyName">Name of the property.</param>
    protected void OnPropertyChanged([CallerMemberName] string propertyName = "")
    {
        this.Dispatcher?.Dispatch(() =>
        {
            var changed = this.PropertyChanged;
            if (changed == null)
            {
                return;
            }

            changed.Invoke(this, new PropertyChangedEventArgs(propertyName));
        });
    }
}