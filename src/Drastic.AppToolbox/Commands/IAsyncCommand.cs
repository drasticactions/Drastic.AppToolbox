// <copyright file="IAsyncCommand.cs" company="Drastic Actions">
// Copyright (c) Drastic Actions. All rights reserved.
// </copyright>

using System.Windows.Input;

namespace Drastic.AppToolbox.Commands
{
    /// <summary>
    /// IAsyncCommand.
    /// </summary>
    public interface IAsyncCommand : ICommand
    {
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
    }
}
