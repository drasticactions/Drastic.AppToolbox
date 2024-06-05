// <copyright file="IAsyncCommandFactory.cs" company="Drastic Actions">
// Copyright (c) Drastic Actions. All rights reserved.
// </copyright>

namespace Drastic.AppToolbox.Services;

/// <summary>
/// IAsyncCommandFactory.
/// </summary>
public interface IAsyncCommandFactory
{
    /// <summary>
    /// Creates an instance of IAsyncCommand.
    /// </summary>
    /// <param name="execute">A function that returns a Task. This function is executed when the command is invoked.</param>
    /// <param name="canExecute">An optional function that returns a boolean. This function is used to determine whether the command can execute in its current state.</param>
    /// <returns>An instance of IAsyncCommand.</returns>
    public IAsyncCommand Create(Func<Task> execute, Func<bool>? canExecute = null);
}