﻿// <copyright file="AsyncCommandT.cs" company="Drastic Actions">
// Copyright (c) Drastic Actions. All rights reserved.
// </copyright>

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
    public class AsyncCommand<T> : IAsyncCommand<T>, IDisposable
#pragma warning restore SA1649 // File name should match first type name
    {
        private readonly Func<T, Task> execute;
        private readonly Func<T, bool>? canExecute;
        private readonly IErrorHandler errorHandler;
        private readonly IAppDispatcher dispatcher;

        private bool isExecuting;

        /// <summary>
        /// Initializes a new instance of the <see cref="AsyncCommand{T}"/> class.
        /// </summary>
        /// <param name="execute">Task to Execute.</param>
        /// <param name="dispatcher">Dispatcher.</param>
        /// <param name="errorHandler">Error Handler.</param>
        /// <param name="canExecute">Can Execute Function.</param>
        public AsyncCommand(Func<T, Task> execute, IAppDispatcher dispatcher, IErrorHandler errorHandler, Func<T, bool>? canExecute = null)
        {
            this.execute = execute;
            this.dispatcher = dispatcher;
            this.canExecute = canExecute;
            this.errorHandler = errorHandler;
        }

        /// <summary>
        /// Can Execute Event.
        /// </summary>
        public event EventHandler? CanExecuteChanged;

        /// <summary>
        /// Returns a value if the given command can be executed.
        /// </summary>
        /// <param name="parameter">Can Execute Function.</param>
        /// <returns>Boolean.</returns>
        public bool CanExecute(T parameter)
        {
            return !this.isExecuting && (this.canExecute?.Invoke(parameter) ?? true);
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
                try
                {
                    this.isExecuting = true;
                    await this.execute(parameter).ConfigureAwait(false);
                }
                finally
                {
                    this.isExecuting = false;
                }
            }

            this.RaiseCanExecuteChanged();
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
        }
    }
}
