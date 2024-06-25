using MirrorSharp;
using MirrorSharp.AspNetCore;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

app.MapGet("/", () => "Hello World!");

app.MapMirrorSharp(
    "/mirrorsharp",
    new MirrorSharpOptions()
    {
        IncludeExceptionDetails = true,
        SelfDebugEnabled = true
    }
);

app.Run();