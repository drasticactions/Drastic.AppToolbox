// <copyright file="IAsyncCommandFactoryT.cs" company="Drastic Actions">
// Copyright (c) Drastic Actions. All rights reserved.
// </copyright>

using Drastic.AppToolbox.Commands;

namespace Drastic.AppToolbox.Services;

/// <summary>
/// IAsyncCommandFactory.
/// </summary>
/// <typeparam name="T">Type of object to inspect.</typeparam>
#pragma warning disable SA1649
public interface IAsyncCommandFactory<T>
#pragma warning restore SA1649
{
    /// <summary>
    /// Creates an instance of IAsyncCommand.
    /// </summary>
    /// <param name="execute">A function that returns a Task. This function is executed when the command is invoked.</param>
    /// <param name="canExecute">An optional function that returns a boolean. This function is used to determine whether the command can execute in its current state.</param>
    /// <returns>An instance of IAsyncCommand.</returns>
    public IAsyncCommand<T> Create(Func<T, Task> execute, Func<T, bool>? canExecute = null);
}