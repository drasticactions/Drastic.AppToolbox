// <copyright file="AsyncCommandFactoryT.cs" company="Drastic Actions">
// Copyright (c) Drastic Actions. All rights reserved.
// </copyright>

namespace Drastic.AppToolbox.Services;

#pragma warning disable SA1649
/// <summary>
/// A factory for creating instances of IAsyncCommand of type T.
/// </summary>
#pragma warning disable SA1618
public class AsyncCommandFactory<T> : IAsyncCommandFactory<T>
#pragma warning restore SA1618
#pragma warning restore SA1649
{
    private IAppDispatcher dispatcher;
    private IErrorHandler errorHandler;

    /// <summary>
    /// Initializes a new instance of the <see cref="AsyncCommandFactory{T}"/> class.
    /// </summary>
    /// <param name="dispatcher">An instance of <see cref="IAppDispatcher"/>. This is used to dispatch actions in the application.</param>
    /// <param name="errorHandler">An instance of <see cref="IErrorHandler"/>. This is used to handle errors in the application.</param>
    /// <exception cref="ArgumentNullException">Thrown when either <paramref name="dispatcher"/> or <paramref name="errorHandler"/> is null.</exception>
    public AsyncCommandFactory(IAppDispatcher dispatcher, IErrorHandler errorHandler)
    {
        this.dispatcher = dispatcher ?? throw new ArgumentNullException(nameof(dispatcher));
        this.errorHandler = errorHandler ?? throw new ArgumentNullException(nameof(errorHandler));
    }

    /// <summary>
    /// Creates an instance of IAsyncCommand of type T.
    /// </summary>
    /// <param name="execute">A function that returns a Task. This function is executed when the command is invoked.</param>
    /// <param name="canExecute">An optional function that returns a boolean. This function is used to determine whether the command can execute in its current state.</param>
    /// <returns>An instance of IAsyncCommand of type T.</returns>
    public IAsyncCommand<T> Create(Func<T, Task> execute, Func<T, bool>? canExecute = null)
        => new AsyncCommand<T>(execute, this.dispatcher, this.errorHandler, canExecute);
}