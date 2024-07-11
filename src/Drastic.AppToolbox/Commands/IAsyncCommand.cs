// <copyright file="IAsyncCommand.cs" company="Drastic Actions">
// Copyright (c) Drastic Actions. All rights reserved.
// </copyright>

using System.ComponentModel;
using System.Windows.Input;

namespace Drastic.AppToolbox.Commands
{
    /// <summary>
    /// IAsyncCommand.
    /// </summary>
    public interface IAsyncCommand : ICommand, IDisposable, INotifyPropertyChanged
    {
        /// <summary>
        /// Gets a value indicating whether the command is executing.
        /// </summary>
        public bool IsBusy { get; }

        /// <summary>
        /// Gets a value indicating the progress of the command.
        /// </summary>
        public int Progress { get; }

        /// <summary>
        /// Gets the Title of the Command.
        /// </summary>
        public string Title { get; }

        /// <summary>
        /// Execute Async.
        /// </summary>
        /// <returns><see cref="Task"/>.</returns>
        Task ExecuteAsync();

        /// <summary>
        /// Execute.
        /// </summary>
        void Execute();

        /// <summary>
        /// Can execute Command.
        /// </summary>
        /// <returns>Boolean.</returns>
        bool CanExecute();

        /// <summary>
        /// Cancel Command.
        /// </summary>
        void Cancel();

        /// <summary>
        /// Update Title.
        /// </summary>
        /// <param name="title">Title to update.</param>
        void UpdateTitle(string title);

        /// <summary>
        /// Raise Can Execute Changed.
        /// </summary>
        void RaiseCanExecuteChanged();
    }
}
