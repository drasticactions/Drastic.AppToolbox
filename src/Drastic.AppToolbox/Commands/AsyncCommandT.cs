// <copyright file="AsyncCommandT.cs" company="Drastic Actions">
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
    /// <typeparam name="T">Generic Parameter.</typeparam>
#pragma warning disable SA1649 // File name should match first type name
    public class AsyncCommand<T> : IAsyncCommand<T>, IDisposable, INotifyPropertyChanged
#pragma warning restore SA1649 // File name should match first type name
    {
        private readonly Func<T, CancellationToken, IProgress<int>, IProgress<string>, Task> execute;
        private readonly Func<T, bool>? canExecute;
        private readonly IErrorHandler errorHandler;
        private readonly IAppDispatcher dispatcher;
        private CancellationTokenSource cancellationTokenSource;
        private int progress;
        private Progress<int> progressHandler;
        private Progress<string> titleHandler;
        private string title = string.Empty;
        private bool isBusy;
        private string originalTitle = string.Empty;
        private bool resetTitleOnTaskComplete;

        /// <summary>
        /// Initializes a new instance of the <see cref="AsyncCommand{T}"/> class.
        /// </summary>
        /// <param name="title">The title of the command.</param>
        /// <param name="execute">Task to Execute.</param>
        /// <param name="dispatcher">Dispatcher.</param>
        /// <param name="errorHandler">Error Handler.</param>
        /// <param name="canExecute">Can Execute Function.</param>
        /// <param name="resetTitleOnTaskComplete">Reset the title on task completion.</param>
        public AsyncCommand(
            string title,
            Func<T, CancellationToken,
                IProgress<int>, IProgress<string>, Task> execute,
            IAppDispatcher dispatcher,
            IErrorHandler errorHandler,
            Func<T, bool>? canExecute = null,
            bool resetTitleOnTaskComplete = true)
        {
            ArgumentNullException.ThrowIfNullOrEmpty(title);
            ArgumentNullException.ThrowIfNull(execute);
            this.execute = execute;
            this.dispatcher = dispatcher;
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
        /// Can Execute Event.
        /// </summary>
        public event EventHandler? CanExecuteChanged;

        /// <summary>
        /// Gets a value indicating whether the command is executing.
        /// </summary>
        public bool IsBusy
        {
            get => this.isBusy;
            private set => this.SetProperty(ref this.isBusy, value);
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

        /// <summary>
        /// Returns a value if the given command can be executed.
        /// </summary>
        /// <param name="parameter">Can Execute Function.</param>
        /// <returns>Boolean.</returns>
        public bool CanExecute(T parameter)
        {
            return !this.isBusy && (this.canExecute?.Invoke(parameter) ?? true);
        }

        /// <summary>
        /// Executes Command Async.
        /// </summary>
        /// <param name="parameter">Command to be Executed.</param>
        public void Execute(T parameter)
            => this.ExecuteAsync(parameter).FireAndForgetSafeAsync(this.errorHandler);

        /// <summary>
        /// Executes Command Async.
        /// </summary>
        /// <param name="parameter">Command to be Executed.</param>
        /// <returns>Task.</returns>
        public async Task ExecuteAsync(T parameter)
        {
            if (this.CanExecute(parameter))
            {
                this.cancellationTokenSource.TryReset();
                try
                {
                    this.isBusy = true;
                    await this.execute(parameter, this.cancellationTokenSource.Token, this.progressHandler, this.titleHandler).ConfigureAwait(false);
                }
                catch (Exception ex)
                {
                    this.errorHandler.HandleError(ex);
                }
                finally
                {
                    this.isBusy = false;
                    if (this.resetTitleOnTaskComplete)
                    {
                        this.Title = this.originalTitle;
                    }
                }
            }

            this.RaiseCanExecuteChanged();
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

        /// <summary>
        /// Raise Can Execute Changed.
        /// </summary>
        public void RaiseCanExecuteChanged()
        {
            this.dispatcher?.Dispatch(() => this.CanExecuteChanged?.Invoke(this, EventArgs.Empty));
        }

        /// <inheritdoc/>
        bool ICommand.CanExecute(object? parameter)
        {
            return parameter is null || this.CanExecute((T)parameter);
        }

        /// <inheritdoc/>
        void ICommand.Execute(object? parameter)
        {
            if (parameter is not null)
            {
                this.ExecuteAsync((T)parameter).FireAndForgetSafeAsync(this.errorHandler);
            }
        }

        /// <inheritdoc/>
        public void Dispose()
        {
            this.CanExecuteChanged = null;
            this.cancellationTokenSource.Dispose();
        }

#pragma warning disable SA1600 // Elements should be documented
#pragma warning disable SA1314 // Type parameter names should begin with T
        protected bool SetProperty<Y>(ref Y backingStore, Y value, bool raiseCanExecuteChanged = false, [CallerMemberName] string propertyName = "", Action? onChanged = null)
#pragma warning restore SA1314 // Type parameter names should begin with T
#pragma warning restore SA1600 // Elements should be documented
        {
            if (EqualityComparer<Y>.Default.Equals(backingStore, value))
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
