using ArchonAnalysers.Analyzers.ARCHON001;
using ArchonAnalysers.FixProviders.ARCHON001;
using Microsoft.CodeAnalysis.CSharp.Testing;
using Microsoft.CodeAnalysis.Testing;
using Xunit;

namespace ArchonAnalysers.Tests.Unit.Analyzers.ARCHON001;

public class InternalsAreInternalFixProviderTests
{
    [Fact]
    public async Task SwapsPublicModifierToInternal()
    {
        const string testCode = $$"""
                                  namespace TestApp.Internal;
                                  {|{{InternalsAreInternalAnalyzer.DiagnosticId}}:public|} class MyClass;
                                  """;

        const string fixedCode = """
                                 namespace TestApp.Internal;
                                 internal|} class MyClass;
                                 """;


        CSharpCodeFixTest<InternalsAreInternalAnalyzer, InternalsAreInternalFixProvider, DefaultVerifier> test = new() { TestCode = testCode, FixedCode = fixedCode };


        await test.RunAsync(TestContext.Current.CancellationToken);
    }
}
