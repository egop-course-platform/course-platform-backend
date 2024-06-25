using Microsoft.AspNetCore.WebSockets;
using MirrorSharp;
using MirrorSharp.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddWebSockets(x => { });

var app = builder.Build();

app.MapGet("/", () => "Hello World!");

((IApplicationBuilder)app).MapMirrorSharp(
    "/mirrorsharp",
    new MirrorSharpOptions()
    {
        IncludeExceptionDetails = true,
        SelfDebugEnabled = true
    }
);

app.UseRouting();
app.UseEndpoints(endpoints => { });

app.Run();