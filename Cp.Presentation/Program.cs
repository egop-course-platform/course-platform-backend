using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using MirrorSharp;
using MirrorSharp.AspNetCore;


var builder = WebApplication.CreateBuilder(args);
builder.Services.AddCors();

var app = builder.Build();
app.UseDefaultFiles();
app.UseStaticFiles();
app.UseWebSockets();

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
