﻿using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Testing;
using System.Collections.Generic;
using System.Threading.Tasks;
using VerifyCS = Privatest.Test.CSharpCodeFixVerifier<Privatest.AttributePlacementAnalyzer, Privatest.AttributePlacementCodeFixProvider>;

namespace Privatest.Test
{
	public partial class AttributePlacementAnalyzerTests
	{
		private readonly List<DiagnosticResult> _results = new List<DiagnosticResult>();
		private readonly AttributePlacementAnalyzer _analyzer = new AttributePlacementAnalyzer();

		private void Expect(int location, Accessibility accessibility, string name, int error)
		{
			var diagnostic = VerifyCS
				.Diagnostic(_analyzer.SupportedDiagnostics[error])
				.WithLocation(location)
				.WithArguments(new[] { accessibility.ToString(), name });

			_results.Add(diagnostic);
		}

		private Task Verify(string code)
		{
			return VerifyCS.VerifyAnalyzerAsync(ComposeFullSource(code), _results.ToArray());
		}

		private Task Verify(string code, string fixedCode)
		{
			return VerifyCS.VerifyCodeFixAsync(ComposeFullSource(code), _results.ToArray(), ComposeFullSource(fixedCode));
		}

		private string ComposeFullSource(string code)
		{
			return $@"
				using System;

				namespace Privatest
				{{
					[AttributeUsage(AttributeTargets.Property | AttributeTargets.Method | AttributeTargets.Field, AllowMultiple = false)]
					public sealed class ThisAttribute : Attribute
					{{ }}

					[AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
					public sealed class BackingFieldAttribute : Attribute
					{{ }}

					{code}
				}}
			";
		}
	}
}
