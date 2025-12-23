using System.Collections.Immutable;
using ArchonAnalysers.Analyzers.ARCHON001;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;

namespace ArchonAnalysers.FixProviders.ARCHON001;

public class InternalsAreInternalFixProvider : CodeFixProvider
{
    public override ImmutableArray<string> FixableDiagnosticIds => [InternalsAreInternalAnalyzer.DiagnosticId];

    public override Task RegisterCodeFixesAsync(CodeFixContext context)
    {
        Diagnostic? diagnostic = context.Diagnostics.FirstOrDefault();

        if (diagnostic == null)
        {
            return Task.CompletedTask;
        }

        CodeAction action = CodeAction.Create("Make Type Internal", ct => CreateChangedDocument(context, ct), diagnostic.Id);
        context.RegisterCodeFix(action, diagnostic);

        return Task.CompletedTask;
    }


    private static async Task<Document> CreateChangedDocument(CodeFixContext context, CancellationToken ct)
    {
        SyntaxNode? syntaxRoot = await context.Document.GetSyntaxRootAsync(ct);

        if (syntaxRoot == null)
        {
            return context.Document;
        }

        SyntaxToken troublesomeToken = syntaxRoot.FindToken(context.Span.Start);

        SyntaxToken internalTokenWithTrivia = GetTokenToReplace(troublesomeToken);

        SyntaxNode newTree = syntaxRoot.ReplaceToken(troublesomeToken, internalTokenWithTrivia);

        return context.Document.WithSyntaxRoot(newTree);
    }

    private static SyntaxToken GetTokenToReplace(SyntaxToken troublesomeToken) =>
        SyntaxFactory.Token(SyntaxKind.InternalKeyword)
            .WithLeadingTrivia(troublesomeToken.LeadingTrivia)
            .WithTrailingTrivia(troublesomeToken.TrailingTrivia);
}
