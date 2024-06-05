// <copyright file="AsyncCommandTests.cs" company="Drastic Actions">
// Copyright (c) Drastic Actions. All rights reserved.
// </copyright>

namespace Drastic.AppToolbox.Tests;

[TestClass]
public class AsyncCommandTests
{
    private static IAppDispatcher dispatcher = new DebugAppDispatcher();
    private static IErrorHandler errorHandler = new DebugErrorHandler();
    private static IAsyncCommandFactory asyncCommandFactory = new AsyncCommandFactory(dispatcher, errorHandler);

    [TestMethod]
    public void AsyncCommand_CanExecute()
    {
        var canExecute = true;
        var command = new AsyncCommand(() => Task.CompletedTask, dispatcher, errorHandler, () => canExecute);
        Assert.IsTrue(command.CanExecute());
        canExecute = false;
        Assert.IsFalse(command.CanExecute());
    }

    [TestMethod]
    public void AsyncCommand_HandleError()
    {
        var command = new AsyncCommand(() => throw new ExpectedErrorException(), dispatcher, errorHandler);
        command.Execute();
    }

    [TestMethod]
    public void AsyncCommandFactory_Create()
    {
        var canExecute = true;
        var command = asyncCommandFactory.Create(() => Task.CompletedTask, () => canExecute);
        Assert.IsNotNull(command);
        Assert.IsTrue(command.CanExecute());
        canExecute = false;
        Assert.IsFalse(command.CanExecute());
    }

    [TestMethod]
    public void AsyncCommandFactoryT_Create()
    {
        var factory = new AsyncCommandFactory<string>(dispatcher, errorHandler);
        var command = factory.Create(
            (x) =>
        {
            Assert.IsNotNull(x);
            return Task.CompletedTask;
        },
            (x) => !string.IsNullOrEmpty(x));
        Assert.IsNotNull(command);
        Assert.IsTrue(command.CanExecute("x"));
        Assert.IsFalse(command.CanExecute(string.Empty));
    }
}