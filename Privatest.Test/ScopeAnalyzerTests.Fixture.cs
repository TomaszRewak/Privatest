using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Testing;
using System.Collections.Generic;
using System.Threading.Tasks;
using VerifyCS = Privatest.Test.CSharpAnalyzerVerifier<Privatest.ScopeAnalyzer>;

namespace Privatest.Test
{
	public partial class ScopeAnalyzerTests
	{
		private readonly List<DiagnosticResult> _results = new List<DiagnosticResult>();
		private readonly ScopeAnalyzer _analyzer = new ScopeAnalyzer();

		private void Expect(int location, string name, string expectedScopeName, string actualScopeName)
		{
			var diagnostic = VerifyCS
				.Diagnostic(_analyzer.SupportedDiagnostics[0])
				.WithLocation(location)
				.WithArguments(new[] { name, expectedScopeName, actualScopeName });

			_results.Add(diagnostic);
		}

		private Task Verify(string code)
		{
			var fullSource = $@"
				using System;

				namespace Privatest
				{{
					[AttributeUsage(AttributeTargets.Property | AttributeTargets.Method | AttributeTargets.Field, AllowMultiple = false)]
					public sealed class ThisAttribute : Attribute
					{{
						public ThisAttribute(string methodOrPropertyName) {{ }}
					}}

					[AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
					public sealed class BackingFieldAttribute : Attribute
					{{ }}

					{code}
				}}
			";

			return VerifyCS.VerifyAnalyzerAsync(fullSource, _results.ToArray());
		}
	}
}
