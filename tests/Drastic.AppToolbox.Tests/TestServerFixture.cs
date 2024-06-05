// <copyright file="TestServerFixture.cs" company="Drastic Actions">
// Copyright (c) Drastic Actions. All rights reserved.
// </copyright>

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;


namespace Drastic.AppToolbox.Tests;

public class TestServerFixture
{
    public const string Hello = "Hello World!";

    public TestServer TestServer { get; }

    public TestServerFixture()
    {
        var hostBuilder = new HostBuilder()
            .ConfigureWebHost(webHost =>
            {
                webHost
                    .UseTestServer()
                    .ConfigureServices(services =>
                    {
                        services.AddRouting();
                    })
                    .Configure(app =>
                    {
                        app.UseRouting();
                        app.UseEndpoints(endpoints =>
                        {
                            endpoints.MapGet("/", async context =>
                            {
                                await context.Response.WriteAsync(Hello);
                            });

                            endpoints.MapGet("/error", async context =>
                            {
                                context.Response.StatusCode = 500;
                                await context.Response.WriteAsync("Error");
                            });

                            endpoints.MapGet("/longpoll", async context =>
                            {
                                var token = context.RequestAborted;
                                await Task.Delay(5000, token);
                                await context.Response.WriteAsync("Done");
                            });
                        });
                    });
            });

        var host = hostBuilder.Start();
        TestServer = host.GetTestServer();
    }
}
