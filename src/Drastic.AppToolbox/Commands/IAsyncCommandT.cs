// <copyright file="IAsyncCommandT.cs" company="Drastic Actions">
// Copyright (c) Drastic Actions. All rights reserved.
// </copyright>

using System.Windows.Input;

namespace Drastic.AppToolbox.Commands
{
    /// <summary>
    /// IAsyncCommand.
    /// </summary>
    /// <typeparam name="T">Type of Command.</typeparam>
#pragma warning disable SA1649 // File name should match first type name
    public interface IAsyncCommand<in T> : ICommand, IDisposable
#pragma warning restore SA1649 // File name should match first type name
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
        /// <param name="parameter">parameter.</param>
        /// <returns><see cref="Task"/>.</returns>
        Task ExecuteAsync(T parameter);

        /// <summary>
        /// Execute.
        /// </summary>
        /// <param name="parameter">parameter.</param>
        void Execute(T parameter);

        /// <summary>
        /// Can Execute.
        /// </summary>
        /// <param name="parameter">parameter.</param>
        /// <returns>Bool.</returns>
        bool CanExecute(T parameter);

        /// <summary>
        /// Cancel Command.
        /// </summary>
        void Cancel();

        /// <summary>
        /// Update Title.
        /// </summary>
        /// <param name="title">Title to update.</param>
        void UpdateTitle(string title);
    }
}
