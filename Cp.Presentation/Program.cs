using System.Collections.Immutable;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebSockets;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Completion;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Host.Mef;
using Microsoft.CodeAnalysis.Options;
using Microsoft.CodeAnalysis.Text;


var builder = WebApplication.CreateBuilder(args);
builder.Services.AddCors();

builder.Services.AddWebSockets(x => { });

var app = builder.Build();

app.MapGet("/", () => "Hello World!");

app.UseCors(
    x => x.AllowAnyOrigin()
        .AllowAnyHeader()
        .AllowAnyMethod()
);

app.MapPost(
    "/completion",
    async ([FromBody] CompletionRequest request) =>
    {
        var compilationOptions = new CSharpCompilationOptions(
            OutputKind.DynamicallyLinkedLibrary,
            usings: ["System"]
        );

        using var workspace = new AdhocWorkspace();

        var scriptProjectInfo = ProjectInfo.Create(
                ProjectId.CreateNewId(),
                VersionStamp.Create(),
                "Script",
                "Script",
                LanguageNames.CSharp,
                isSubmission: true
            )
            .WithMetadataReferences(
                new[]
                {
                    MetadataReference.CreateFromFile(typeof(object).Assembly.Location)
                }
            )
            .WithCompilationOptions(compilationOptions);

        var scriptProject = workspace.AddProject(scriptProjectInfo);
        var scriptDocumentInfo = DocumentInfo.Create(
            DocumentId.CreateNewId(scriptProject.Id),
            "Script.cs",
            sourceCodeKind: SourceCodeKind.Script,
            loader: TextLoader.From(TextAndVersion.Create(SourceText.From(request.Code), VersionStamp.Create()))
        );
        var scriptDocument = workspace.AddDocument(scriptDocumentInfo);
        var completionService = CompletionService.GetService(scriptDocument)!;

        var completions = await completionService.GetCompletionsAsync(scriptDocument, request.Code.Length - 1);

        return Results.Ok(completions.ItemsList.Select(x => x.DisplayText));
    }
);

app.Run();



record CompletionRequest(string Code);