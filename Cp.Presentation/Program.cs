using System.Collections.Immutable;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.OpenApi.Models;
using MirrorSharp;
using MirrorSharp.AspNetCore;


var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<ForwardedHeadersOptions>(options =>
{
    options.ForwardedHeaders =
        ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
});

builder.Services.AddCors();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(
    x =>
    {
        x.SwaggerDoc("Default", new OpenApiInfo()
        {
            Title = "Egop Interactive Course Platform",
            Version = "Default"
        });
        x.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, "Cp.Presentation.xml"));
    }
);

var app = builder.Build();

app.UseForwardedHeaders();

app.UseSwagger(
    x =>
    {
        x.RouteTemplate = "/api/swagger/{documentName}/swagger.json";
    });
app.UseSwaggerUI(x =>
{
    x.SwaggerEndpoint("Default/swagger.json", "Egop Interactive Course Platform");
});

app.UseDefaultFiles();
app.UseStaticFiles();
app.UseWebSockets();

app.MapGet("/", () => Results.Ok("App Runs"))
    .WithSummary("Default endpoint, returning \"App Runs\" string");

app.MapMirrorSharp(
    "/mirrorsharp",
    new MirrorSharpOptions
        {
            SelfDebugEnabled = true,
            IncludeExceptionDetails = true
        }
        .SetupCSharp(
            o =>
            {
                o.MetadataReferences = GetAllReferences()
                    .ToImmutableList();
                o.ParseOptions = o.ParseOptions.WithLanguageVersion(LanguageVersion.Latest);
                o.CompilationOptions = o.CompilationOptions.WithOutputKind(OutputKind.ConsoleApplication)
                    .WithUsings("System");
            }
        )
);


app.Run();

static IEnumerable<MetadataReference> GetAllReferences()
{
    yield return ReferenceAssembly("System");
    yield return ReferenceAssembly("System.Console");
    yield return ReferenceAssembly("System.Runtime");
    yield return ReferenceAssembly("System.Collections");
    yield return MetadataReference.CreateFromFile(typeof(Console).Assembly.Location);
}

static MetadataReference ReferenceAssembly(string name)
{
    var rootPath = Path.Combine(AppContext.BaseDirectory, "ref-assemblies");
    var assemblyPath = Path.Combine(rootPath, name + ".dll");
    var documentationPath = Path.Combine(rootPath, name + ".xml");

    return MetadataReference.CreateFromFile(
        assemblyPath,
        documentation: XmlDocumentationProvider.CreateFromFile(documentationPath)
    );
}
