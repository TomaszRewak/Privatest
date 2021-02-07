using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Operations;
using Privatest.Extensions;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading;

namespace Privatest
{
	[DiagnosticAnalyzer(LanguageNames.CSharp)]
	public class PrivatestAnalyzer : DiagnosticAnalyzer
	{
		public const string DiagnosticId = "Privatest";

		// You can change these strings in the Resources.resx file. If you do not want your analyzer to be localize-able, you can use regular strings for Title and MessageFormat.
		// See https://github.com/dotnet/roslyn/blob/master/docs/analyzers/Localizing%20Analyzers.md for more on localization
		private static readonly LocalizableString Title = new LocalizableResourceString(nameof(Resources.AnalyzerTitle), Resources.ResourceManager, typeof(Resources));
		private static readonly LocalizableString MessageFormat = new LocalizableResourceString(nameof(Resources.AnalyzerMessageFormat), Resources.ResourceManager, typeof(Resources));
		private static readonly LocalizableString Description = new LocalizableResourceString(nameof(Resources.AnalyzerDescription), Resources.ResourceManager, typeof(Resources));
		private const string Category = "Naming";

		private static readonly DiagnosticDescriptor Rule = new DiagnosticDescriptor(DiagnosticId, Title, MessageFormat, Category, DiagnosticSeverity.Warning, isEnabledByDefault: true, description: Description);
		private static readonly DiagnosticDescriptor AttributeRule = new DiagnosticDescriptor(
			DiagnosticId,
			"The [This] attribute is applied on a non-private property",
			"The [This] attribute can be applied only on private properties, but was applied on '{0}' property '{1}'",
			"Accessibility",
			DiagnosticSeverity.Error,
			isEnabledByDefault: true);
		private static readonly DiagnosticDescriptor InvocationRule = new DiagnosticDescriptor(
			DiagnosticId,
			"AAA",
			"BBB",
			"Accessibility",
			DiagnosticSeverity.Error,
			isEnabledByDefault: true);

		public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get { return ImmutableArray.Create(Rule, AttributeRule, InvocationRule); } }

		public override void Initialize(AnalysisContext context)
		{
			context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
			context.EnableConcurrentExecution();

			// TODO: Consider registering other actions that act on syntax instead of or in addition to symbols
			// See https://github.com/dotnet/roslyn/blob/master/docs/analyzers/Analyzer%20Actions%20Semantics.md for more information
			context.RegisterSymbolAction(AnalyzeNamedType, SymbolKind.NamedType);
			context.RegisterSymbolAction(AnalyzeProperty, SymbolKind.Property);
			context.RegisterOperationAction(AnalyzeInvocation, OperationKind.Invocation);
		}

		private static void AnalyzeNamedType(SymbolAnalysisContext context)
		{
			// TODO: Replace the following code with your own analysis, generating Diagnostic objects for any issues you find
			var namedTypeSymbol = (INamedTypeSymbol)context.Symbol;

			// Find just those named type symbols with names containing lowercase letters.
			if (namedTypeSymbol.Name.ToCharArray().Any(char.IsLower))
			{
				// For all such symbols, produce a diagnostic.
				var diagnostic = Diagnostic.Create(Rule, namedTypeSymbol.Locations[0], namedTypeSymbol.Name);

				context.ReportDiagnostic(diagnostic);
			}
		}

		private static void AnalyzeProperty(SymbolAnalysisContext context)
		{
			var symbol = context.Symbol;

			if (!symbol.HasAttribute<ThisAttribute>()) return;
			if (symbol.DeclaredAccessibility == Accessibility.Private) return;

			var diagnostic = Diagnostic.Create(AttributeRule, symbol.Locations[0], new object[] { symbol.DeclaredAccessibility, symbol.Name });
			context.ReportDiagnostic(diagnostic);
		}

		private static void AnalyzeInvocation(OperationAnalysisContext context)
		{
			var operation = context.Operation;
			var invocation = operation as IInvocationOperation;

			if (!invocation.TargetMethod.HasAttribute<ThisAttribute>()) return;
			if (invocation.Instance.Kind == OperationKind.InstanceReference) return;

			var diagnostic = Diagnostic.Create(InvocationRule, operation.Syntax.GetLocation());
			context.ReportDiagnostic(diagnostic);
		}
	}
}
