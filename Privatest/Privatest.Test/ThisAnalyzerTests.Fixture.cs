using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Testing;
using System.Collections.Generic;
using System.Threading.Tasks;
using VerifyCS = Privatest.Test.CSharpAnalyzerVerifier<Privatest.ThisAnalyzer>;

namespace Privatest.Test
{
	public partial class ThisAnalyzerTests
	{
		private readonly List<DiagnosticResult> _results = new List<DiagnosticResult>();
		private readonly ThisAnalyzer _analyzer = new ThisAnalyzer();

		private void Expect(int location, string name)
		{
			var diagnostic = VerifyCS
				.Diagnostic(_analyzer.SupportedDiagnostics[0])
				.WithLocation(location)
				.WithArguments(new[] { name });

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
					{{ }}

					{code}
				}}
			";

			return VerifyCS.VerifyAnalyzerAsync(fullSource, _results.ToArray());
		}
	}
}
