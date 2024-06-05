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
    /// <param name="title">Title of the Command.</param>
    /// <param name="execute">A function that returns a Task. This function is executed when the command is invoked.</param>
    /// <param name="canExecute">An optional function that returns a boolean. This function is used to determine whether the command can execute in its current state.</param>
    /// <param name="resetTitleOnTaskComplete">Reset the title of the command when the task is complete.</param>
    /// <returns>An instance of IAsyncCommand.</returns>
    public IAsyncCommand Create(string title, Func<CancellationToken, IProgress<int>, IProgress<string>, Task> execute, Func<bool>? canExecute = null, bool resetTitleOnTaskComplete = true);
}