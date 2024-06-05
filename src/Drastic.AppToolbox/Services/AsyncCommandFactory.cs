// <copyright file="AsyncCommandFactory.cs" company="Drastic Actions">
// Copyright (c) Drastic Actions. All rights reserved.
// </copyright>

namespace Drastic.AppToolbox.Services;

/// <summary>
/// Async Command Factory.
/// </summary>
public class AsyncCommandFactory : IAsyncCommandFactory
{
    private IAppDispatcher dispatcher;
    private IErrorHandler errorHandler;

    /// <summary>
    /// Initializes a new instance of the <see cref="AsyncCommandFactory"/> class.
    /// </summary>
    /// <param name="dispatcher">An instance of <see cref="IAppDispatcher"/>. This is used to dispatch actions in the application.</param>
    /// <param name="errorHandler">An instance of <see cref="IErrorHandler"/>. This is used to handle errors in the application.</param>
    /// <exception cref="ArgumentNullException">Thrown when either <paramref name="dispatcher"/> or <paramref name="errorHandler"/> is null.</exception>
    public AsyncCommandFactory(IAppDispatcher dispatcher, IErrorHandler errorHandler)
    {
        this.dispatcher = dispatcher ?? throw new ArgumentNullException(nameof(dispatcher));
        this.errorHandler = errorHandler ?? throw new ArgumentNullException(nameof(errorHandler));
    }

    /// <inheritdoc/>
    public IAsyncCommand Create(string title, Func<CancellationToken, IProgress<int>, IProgress<string>, Task> execute, Func<bool>? canExecute = null, bool resetTitleOnTaskComplete = true)
        => new AsyncCommand(title, execute, this.dispatcher, this.errorHandler, canExecute, resetTitleOnTaskComplete);
}