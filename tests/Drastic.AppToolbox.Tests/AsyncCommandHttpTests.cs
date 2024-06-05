// <copyright file="AsyncCommandHttpTests.cs" company="Drastic Actions">
// Copyright (c) Drastic Actions. All rights reserved.
// </copyright>

using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.VisualStudio.TestPlatform.TestHost;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Drastic.AppToolbox.Tests;

[TestClass]
public class AsyncCommandHttpTests
{
    private static IAppDispatcher dispatcher = new DebugAppDispatcher();
    private static IErrorHandler errorHandler = new DebugErrorHandler(new List<Type>() { typeof(HttpRequestException) });
    private static TestServerFixture _fixture;

    [ClassInitialize]
    public static void ClassInitialize(TestContext context)
    {
        _fixture = new TestServerFixture();
    }

    [TestMethod]
    public async Task AsyncCommandHttp_Get()
    {
        using var client = _fixture.TestServer.CreateClient();
        var command = new AsyncCommand(
            "Test",
            async (ct, pro, title) =>
            {
                var response = await client.GetStringAsync("/", ct);
                Assert.AreEqual(TestServerFixture.Hello, response);
            },
            dispatcher,
            errorHandler);
        await command.ExecuteAsync();
    }

    [TestMethod]
    public async Task AsyncCommandHttp_GetError()
    {
        using var client = _fixture.TestServer.CreateClient();
        var command = new AsyncCommand(
            "Test",
            async (ct, pro, title) =>
            {
                await client.GetStringAsync("/error", ct);
            },
            dispatcher,
            errorHandler);
        await command.ExecuteAsync();
    }

    [TestMethod]
    public async Task AsyncCommandHttp_LongPollCancel()
    {
        using var client = _fixture.TestServer.CreateClient();
        var command = new AsyncCommand(
            "Test",
            async (ct, pro, title) =>
            {
                var response = await client.GetStreamAsync("/longpoll", ct);
                if (!ct.IsCancellationRequested)
                {
                    Assert.Fail("Should have been cancelled.");
                }
            },
            dispatcher,
            errorHandler);
        command.ExecuteAsync().FireAndForgetSafeAsync();
        await Task.Delay(500);
        Assert.IsTrue(command.IsBusy);
        command.Cancel();
        await Task.Delay(500);
        Assert.IsFalse(command.IsBusy);
    }
}
