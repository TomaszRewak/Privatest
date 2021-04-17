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

		private static readonly DiagnosticDescriptor ThisAttributeRule = new DiagnosticDescriptor(
			DiagnosticId,
			"[This] attribute is applied on a non-private member",
			"[This] attribute can be applied only on private members, but was applied on '{0}' member '{1}'",
			"Accessibility",
			DiagnosticSeverity.Error,
			isEnabledByDefault: true);

		private static readonly DiagnosticDescriptor BackingFieldAttributeRule = new DiagnosticDescriptor(
			DiagnosticId,
			"[BackingField] attribute is applied on a non-private member",
			"[BackingField] attribute can be applied only on private members, but was applied on '{0}' member '{1}'",
			"Accessibility",
			DiagnosticSeverity.Error,
			isEnabledByDefault: true);

		public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get { return ImmutableArray.Create(ThisAttributeRule, BackingFieldAttributeRule); } }

		public override void Initialize(AnalysisContext context)
		{
			context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
			context.EnableConcurrentExecution();

			context.RegisterSymbolAction(AnalyzeThisAttributePlacement, SymbolKind.Property);
			context.RegisterSymbolAction(AnalyzeThisAttributePlacement, SymbolKind.Field);
			context.RegisterSymbolAction(AnalyzeThisAttributePlacement, SymbolKind.Method);

			context.RegisterSymbolAction(AnalyzeBackingFieldAttributePlacement, SymbolKind.Field);
		}

		private static void AnalyzeThisAttributePlacement(SymbolAnalysisContext context)
		{
			var symbol = context.Symbol;

			if (symbol.DeclaredAccessibility == Accessibility.Private) return;
			if (!symbol.HasAttribute<ThisAttribute>()) return;

			var diagnostic = Diagnostic.Create(ThisAttributeRule, symbol.Locations[0], new object[] { symbol.DeclaredAccessibility, symbol.Name });
			context.ReportDiagnostic(diagnostic);
		}

		private static void AnalyzeBackingFieldAttributePlacement(SymbolAnalysisContext context)
		{
			var symbol = context.Symbol;

			if (symbol.DeclaredAccessibility == Accessibility.Private) return;
			if (!symbol.HasAttribute<BackingFieldAttribute>()) return;

			var diagnostic = Diagnostic.Create(BackingFieldAttributeRule, symbol.Locations[0], new object[] { symbol.DeclaredAccessibility, symbol.Name });
			context.ReportDiagnostic(diagnostic);
		}
	}
}
