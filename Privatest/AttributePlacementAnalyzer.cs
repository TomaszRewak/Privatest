using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using Privatest.Extensions;
using System.Collections.Immutable;

namespace Privatest
{
	[DiagnosticAnalyzer(LanguageNames.CSharp)]
	internal sealed class AttributePlacementAnalyzer : DiagnosticAnalyzer
	{
		public const string DiagnosticId = "Privatest0001";

		private static readonly DiagnosticDescriptor Rule = new DiagnosticDescriptor(
			DiagnosticId,
			"[This] attribute is applied on a non-private member",
			"[This] attribute can be applied only on private members, but was applied on '{0}' member '{1}'",
			"Accessibility",
			DiagnosticSeverity.Error,
			isEnabledByDefault: true);

		public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get { return ImmutableArray.Create(Rule); } }

		public override void Initialize(AnalysisContext context)
		{
			context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
			context.EnableConcurrentExecution();

			context.RegisterSymbolAction(AnalyzeAttributePlacement, SymbolKind.Property);
			context.RegisterSymbolAction(AnalyzeAttributePlacement, SymbolKind.Field);
			context.RegisterSymbolAction(AnalyzeAttributePlacement, SymbolKind.Method);
		}

		private static void AnalyzeAttributePlacement(SymbolAnalysisContext context)
		{
			var symbol = context.Symbol;

			if (symbol.DeclaredAccessibility == Accessibility.Private) return;
			if (!symbol.HasAttribute<ThisAttribute>()) return;

			var diagnostic = Diagnostic.Create(Rule, symbol.Locations[0], new object[] { symbol.DeclaredAccessibility, symbol.Name });
			context.ReportDiagnostic(diagnostic);
		}
	}
}
