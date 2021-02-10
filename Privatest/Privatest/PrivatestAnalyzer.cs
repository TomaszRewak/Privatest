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

		private static readonly DiagnosticDescriptor AttributeRule = new DiagnosticDescriptor(
			DiagnosticId,
			"[This] attribute is applied on a non-private member",
			"[This] attribute can be applied only on private members, but was applied on '{0}' property '{1}'",
			"Accessibility",
			DiagnosticSeverity.Error,
			isEnabledByDefault: true);
		private static readonly DiagnosticDescriptor ThisAccessibilityRule = new DiagnosticDescriptor(
			DiagnosticId,
			"The member is inaccesible due to its protection level",
			"`{0}` is inaccesible due to its protection level. It can only be accessed through the `this` reference in a private scope.",
			"Accessibility",
			DiagnosticSeverity.Error,
			isEnabledByDefault: true);
		private static readonly DiagnosticDescriptor MemberAccessibilityRule = new DiagnosticDescriptor(
			DiagnosticId,
			"The member is inaccesible due to its protection level",
			"`{0}` is inaccesible due to its protection level. It can only be accessed through the `this` reference in a `{1}` (but is used in a `{2}`).",
			"Accessibility",
			DiagnosticSeverity.Error,
			isEnabledByDefault: true);

		public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get { return ImmutableArray.Create(AttributeRule, ThisAccessibilityRule, MemberAccessibilityRule); } }

		public override void Initialize(AnalysisContext context)
		{
			context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
			context.EnableConcurrentExecution();

			context.RegisterSymbolAction(AnalyzeAttributePlacement, SymbolKind.Property);
			context.RegisterSymbolAction(AnalyzeAttributePlacement, SymbolKind.Field);

			context.RegisterOperationAction(AnalyzeInvocation, OperationKind.Invocation);
			context.RegisterOperationAction(AnalyzeFieldReference, OperationKind.FieldReference);
			context.RegisterOperationAction(AnalyzePropertyReference, OperationKind.PropertyReference);
		}

		private static void AnalyzeAttributePlacement(SymbolAnalysisContext context)
		{
			var symbol = context.Symbol;

			if (symbol.DeclaredAccessibility == Accessibility.Private) return;
			if (!symbol.HasAttribute<ThisAttribute>()) return;

			var diagnostic = Diagnostic.Create(AttributeRule, symbol.Locations[0], new object[] { symbol.DeclaredAccessibility, symbol.Name });
			context.ReportDiagnostic(diagnostic);
		}

		private static void AnalyzeInvocation(OperationAnalysisContext context)
		{
			var operation = context.Operation;
			var invocation = operation as IInvocationOperation;
			var attribute = invocation.TargetMethod.GetAttribute<ThisAttribute>();

			if (attribute == null) return;

			if (invocation.Instance.Kind != OperationKind.InstanceReference)
			{
				context.ReportDiagnostic(Diagnostic.Create(ThisAccessibilityRule, invocation.Syntax.GetLocation(), new object[] { invocation.TargetMethod.Name }));
				return;
			}

			if (attribute.ConstructorArguments.Length == 0) return;

			var scopeName = GetScopeName(context.ContainingSymbol);
			var targetScopeName = attribute.ConstructorArguments[0].Value as string;

			if (scopeName == targetScopeName) return;

			context.ReportDiagnostic(Diagnostic.Create(MemberAccessibilityRule, invocation.Syntax.GetLocation(), new object[] { invocation.TargetMethod.Name, targetScopeName, scopeName }));
		}

		private static void AnalyzeFieldReference(OperationAnalysisContext context)
		{
			var operation = context.Operation;
			var reference = operation as IFieldReferenceOperation;

			if (reference.Instance.Kind == OperationKind.InstanceReference) return;
			if (!reference.Field.HasAttribute<ThisAttribute>()) return;

			var diagnostic = Diagnostic.Create(ThisAccessibilityRule, reference.Syntax.GetLocation(), new object[] { reference.Field.Name });
			context.ReportDiagnostic(diagnostic);
		}

		private static void AnalyzePropertyReference(OperationAnalysisContext context)
		{
			var operation = context.Operation;
			var reference = operation as IPropertyReferenceOperation;

			if (reference.Instance.Kind == OperationKind.InstanceReference) return;

			var attributeOnProperty = reference.Property.HasAttribute<ThisAttribute>();
			var attributeOnSetter = attributeOnProperty || (reference.Property.SetMethod?.HasAttribute<ThisAttribute>() ?? false);
			var attributeOnGetter = attributeOnProperty || (reference.Property.GetMethod?.HasAttribute<ThisAttribute>() ?? false);

			if (!attributeOnSetter && !attributeOnGetter) return;

			var usage = operation.GetValueUsageInfo(context.ContainingSymbol);

			if (attributeOnSetter && usage.HasFlag(ValueUsageInfo.Write))
			{
				var diagnostic = Diagnostic.Create(ThisAccessibilityRule, operation.Syntax.GetLocation(), new object[] { reference.Property.Name });
				context.ReportDiagnostic(diagnostic);
			}

			if (attributeOnGetter && usage.HasFlag(ValueUsageInfo.Read))
			{
				var diagnostic = Diagnostic.Create(ThisAccessibilityRule, operation.Syntax.GetLocation(), new object[] { reference.Property.Name });
				context.ReportDiagnostic(diagnostic);
			}
		}

		private static string GetScopeName(ISymbol symbol)
		{
			while (symbol != null && !(symbol.ContainingSymbol is ITypeSymbol))
				symbol = symbol.ContainingSymbol;

			if (symbol is IMethodSymbol methodSymbol)
				return methodSymbol.AssociatedSymbol?.Name ?? symbol.Name;

			return null;
		}
	}
}
