using System.Collections.Immutable;
using ArchonAnalysers.Analyzers.ARCHON001;
using Microsoft.CodeAnalysis.CodeFixes;

namespace ArchonAnalysers.FixProviders.ARCHON001;

public class InternalsAreInternalFixProvider: CodeFixProvider
{
    public override ImmutableArray<string> FixableDiagnosticIds => [InternalsAreInternalAnalyzer.DiagnosticId];

    public override Task RegisterCodeFixesAsync(CodeFixContext context)
    {


        throw new NotImplementedException();
    }

    public override FixAllProvider GetFixAllProvider()
    {
        throw new NotImplementedException();
    }
}
