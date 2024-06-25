using Microsoft.AspNetCore.WebSockets;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Completion;
using Microsoft.CodeAnalysis.Host.Mef;
using Microsoft.CodeAnalysis.Text;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddWebSockets(x => { });

var app = builder.Build();

app.MapGet("/", () => "Hello World!");

app.MapGet(
    "/completion",
    async () =>
    {
        var host = MefHostServices.Create(MefHostServices.DefaultAssemblies);
        var workspace = new AdhocWorkspace(host);
        var projectInfo = ProjectInfo.Create(
                ProjectId.CreateNewId(),
                VersionStamp.Create(),
                "MyProject",
                "MyProject",
                LanguageNames.CSharp
            )
            .WithMetadataReferences(
                new[]
                {
                    MetadataReference.CreateFromFile(typeof(object).Assembly.Location)
                }
            );

        var project = workspace.AddProject(projectInfo);
        var code = "Console.";
        var document = workspace.AddDocument(project.Id, "MyFile.cs", SourceText.From(code));

        var completionService = CompletionService.GetService(document);

        var completions = await completionService.GetCompletionsAsync(document, code.Length - 1);

        return Results.Ok(completions.ItemsList.Select(x => x.DisplayText));
    }
);

app.UseRouting();
app.UseEndpoints(endpoints => { });

app.Run();