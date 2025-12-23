using System.Collections.Immutable;
using ArchonAnalysers.Analyzers.ARCHON002;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace ArchonAnalysers.FixProviders.ARCHON002;

public class PublicsArePublicFixProvider : CodeFixProvider
{
    public override ImmutableArray<string> FixableDiagnosticIds => [PublicsArePublicAnalyzer.DiagnosticId];

    public override Task RegisterCodeFixesAsync(CodeFixContext context)
    {
        Diagnostic? diagnostic = context.Diagnostics.FirstOrDefault();

        if (diagnostic == null)
        {
            return Task.CompletedTask;
        }

        CodeAction action = CodeAction.Create("Make Type Public", ct => CreateChangedDocument(context, ct), diagnostic.Id + ":" + diagnostic.Location.SourceSpan);
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


        SyntaxNode? parent = troublesomeToken.Parent;

        if (parent is not MemberDeclarationSyntax memberDeclarationSyntax)
        {
            return context.Document;
        }

        SyntaxNode newNode = RemoveTroublesomeModifiers(memberDeclarationSyntax);
        SyntaxNode newRoot = syntaxRoot.ReplaceNode(parent, newNode);
        Document newDocument = context.Document.WithSyntaxRoot(newRoot);

        return newDocument;
    }

    private static MemberDeclarationSyntax RemoveTroublesomeModifiers(MemberDeclarationSyntax declarationSyntax)
    {
        SyntaxTokenList validTokens = new(declarationSyntax.Modifiers.Where(m => !m.IsKind(SyntaxKind.InternalKeyword) && !m.IsKind(SyntaxKind.PrivateKeyword) && !m.IsKind(SyntaxKind.PublicKeyword)));
        validTokens = validTokens.Add(SyntaxFactory.Token(SyntaxKind.PublicKeyword).WithTriviaFrom(declarationSyntax.Modifiers.First()));
        MemberDeclarationSyntax newNode = declarationSyntax.WithModifiers(validTokens);
        return newNode;
    }


}
