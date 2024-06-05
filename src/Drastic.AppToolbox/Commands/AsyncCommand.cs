// <copyright file="AsyncCommand.cs" company="Drastic Actions">
// Copyright (c) Drastic Actions. All rights reserved.
// </copyright>

using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using Drastic.AppToolbox.Services;
using Drastic.AppToolbox.Tools;

namespace Drastic.AppToolbox.Commands
{
    /// <summary>
    /// Async Command.
    /// </summary>
    public class AsyncCommand : IAsyncCommand, IDisposable, INotifyPropertyChanged
    {
        private readonly Func<CancellationToken, IProgress<int>, IProgress<string>, Task>? execute;
        private readonly Func<bool>? canExecute;
        private readonly IErrorHandler errorHandler;
        private readonly IAppDispatcher dispatcher;
        private CancellationTokenSource cancellationTokenSource;
        private Progress<int> progressHandler;
        private Progress<string> titleHandler;
        private bool isBusy;
        private int progress;
        private string title = string.Empty;
        private string originalTitle = string.Empty;
        private bool resetTitleOnTaskComplete;

        /// <summary>
        /// Initializes a new instance of the <see cref="AsyncCommand"/> class.
        /// </summary>
        /// <param name="title">The title of the command.</param>
        /// <param name="execute">Command to execute.</param>
        /// <param name="dispatcher">Dispatcher.</param>
        /// <param name="errorHandler">Error handler.</param>
        /// <param name="canExecute">Can execute command.</param>
        /// <param name="resetTitleOnTaskComplete">Reset the title on task completion.</param>
        public AsyncCommand(
            string title,
            Func<CancellationToken, IProgress<int>, IProgress<string>, Task> execute,
            IAppDispatcher dispatcher,
            IErrorHandler errorHandler,
            Func<bool>? canExecute = null,
            bool resetTitleOnTaskComplete = true)
        {
            this.dispatcher = dispatcher;
            this.execute = execute;
            this.canExecute = canExecute;
            this.errorHandler = errorHandler;
            this.cancellationTokenSource = new CancellationTokenSource();
            this.progressHandler = new Progress<int>((x) => this.Progress = x);
            this.titleHandler = new Progress<string>((x) => this.Title = x);
            this.Title = title;
            this.originalTitle = title;
            this.resetTitleOnTaskComplete = resetTitleOnTaskComplete;
        }

        /// <inheritdoc/>
        public event PropertyChangedEventHandler? PropertyChanged;

        /// <summary>
        /// Can Execute Changed.
        /// </summary>
        public event EventHandler? CanExecuteChanged;

        /// <summary>
        /// Gets a value indicating whether the command is executing.
        /// </summary>
        public bool IsBusy
        {
            get => this.isBusy;
            private set => this.SetProperty(ref this.isBusy, value, true);
        }

        /// <inheritdoc/>
        public int Progress
        {
            get => this.progress;
            private set => this.SetProperty(ref this.progress, value);
        }

        /// <inheritdoc/>
        public string Title
        {
            get => this.title;
            private set => this.SetProperty(ref this.title, value);
        }

        /// <inheritdoc/>
        public bool CanExecute()
        {
            return !this.IsBusy && (this.canExecute?.Invoke() ?? true);
        }

        /// <inheritdoc/>
        public void Execute()
            => this.ExecuteAsync().FireAndForgetSafeAsync(this.errorHandler);

        /// <inheritdoc/>
        public async Task ExecuteAsync()
        {
            if (this.CanExecute())
            {
                this.cancellationTokenSource.TryReset();
                this.Progress = 0;
                if (this.execute is not null)
                {
                    try
                    {
                        this.IsBusy = true;
                        await this.execute(this.cancellationTokenSource.Token, this.progressHandler, this.titleHandler).ConfigureAwait(false);
                    }
                    catch (Exception ex)
                    {
                        this.errorHandler.HandleError(ex);
                    }
                    finally
                    {
                        this.IsBusy = false;
                        if (this.resetTitleOnTaskComplete)
                        {
                            this.Title = this.originalTitle;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Raises Can Execute Changed.
        /// </summary>
#pragma warning disable CA1030 // Use events where appropriate
        public void RaiseCanExecuteChanged()
#pragma warning restore CA1030 // Use events where appropriate
        {
            this.dispatcher?.Dispatch(() => this.CanExecuteChanged?.Invoke(this, EventArgs.Empty));
        }

        /// <inheritdoc/>
        public void Cancel()
        {
            if (this.IsBusy)
            {
                this.cancellationTokenSource.Cancel();
            }
        }

        /// <inheritdoc/>
        public void UpdateTitle(string title)
        {
            this.Title = title;
        }

        /// <inheritdoc/>
        bool ICommand.CanExecute(object? parameter)
        {
            return this.CanExecute();
        }

        /// <inheritdoc/>
        void ICommand.Execute(object? parameter)
        {
            this.ExecuteAsync().FireAndForgetSafeAsync(this.errorHandler);
        }

        /// <inheritdoc/>
        public void Dispose()
        {
            this.CanExecuteChanged = null;
            this.cancellationTokenSource.Dispose();
        }

#pragma warning disable SA1600 // Elements should be documented
        protected bool SetProperty<T>(ref T backingStore, T value, bool raiseCanExecuteChanged = false, [CallerMemberName] string propertyName = "", Action? onChanged = null)
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
                this.dispatcher.Dispatch(() => this.RaiseCanExecuteChanged());
            }

            return true;
        }

        /// <summary>
        /// On Property Changed.
        /// </summary>
        /// <param name="propertyName">Name of the property.</param>
        protected void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            this.dispatcher?.Dispatch(() =>
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
}
