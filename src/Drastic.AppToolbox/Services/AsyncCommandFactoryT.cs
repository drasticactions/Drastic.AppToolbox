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

    /// <inheritdoc/>
    public IAsyncCommand<T> Create(string title, Func<T, CancellationToken, IProgress<int>, IProgress<string>, Task> execute, Func<T, bool>? canExecute = null, bool resetTitleOnTaskComplete = true)
        => new AsyncCommand<T>(title, execute, this.dispatcher, this.errorHandler, canExecute);
}