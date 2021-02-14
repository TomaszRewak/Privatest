using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Testing;
using System.Collections.Generic;
using System.Threading.Tasks;
using VerifyCS = Privatest.Test.CSharpAnalyzerVerifier<Privatest.ThisAnalyzer>;

namespace Privatest.Test
{
	public partial class AttributePlacementTests
	{
		private readonly List<DiagnosticResult> _results = new List<DiagnosticResult>();

		private void Expect(int location, Accessibility accessibility, string name)
		{
			var diagnostic = VerifyCS
				.Diagnostic("Privatest0001")
				.WithLocation(location)
				.WithArguments(new[] { accessibility.ToString(), name });

			_results.Add(diagnostic);
		}

		private Task Verify(string code)
		{
			var fullSource = $@"
				using Privatest;
				{code}
			";

			return VerifyCS.VerifyAnalyzerAsync(fullSource, _results.ToArray());
		}
	}
}
