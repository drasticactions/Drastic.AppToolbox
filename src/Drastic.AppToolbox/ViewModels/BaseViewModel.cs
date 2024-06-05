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
    /// <param name="isLoadingText">Optional Is Loading text.</param>
    /// <returns>Task.</returns>
    public Task PerformBusyAsyncTask(Func<CancellationToken, IProgress<int>, IProgress<string>, Task> action, string isLoadingText = "")
    {
        using var command = this.CommandFactory.Create(isLoadingText, action);
        return this.PerformBusyAsyncTask(command, isLoadingText);
    }

    /// <summary>
    /// Performs an Async task while setting the <see cref="IsBusy"/> variable.
    /// If the task throws, it is handled by <see cref="ErrorHandler"/>.
    /// </summary>
    /// <param name="action">Task to run.</param>
    /// <param name="isLoadingText">Optional Is Loading text.</param>
    /// <returns>Task.</returns>
    public async Task PerformBusyAsyncTask(IAsyncCommand action, string isLoadingText = "")
    {
        try
        {
            this.IsLoadingText = isLoadingText;
            this.IsBusy = true;
            await action.ExecuteAsync();
        }
        catch (Exception ex)
        {
            this.ErrorHandler.HandleError(ex);
        }
        finally
        {
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