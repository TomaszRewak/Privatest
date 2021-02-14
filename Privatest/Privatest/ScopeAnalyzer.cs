using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Operations;
using Privatest.Extensions;
using System.Collections.Immutable;

namespace Privatest
{
	[DiagnosticAnalyzer(LanguageNames.CSharp)]
	internal sealed class ScopeAnalyzer : DiagnosticAnalyzer
	{
		public const string DiagnosticId = "Privatest0003";

		private static readonly DiagnosticDescriptor Rule = new DiagnosticDescriptor(
			DiagnosticId,
			"The member is inaccesible due to its protection level",
			"`{0}` is inaccesible due to its protection level. It can only be accessed through the `this` reference in a `{1}` (but is used in a `{2}`).",
			"Accessibility",
			DiagnosticSeverity.Error,
			isEnabledByDefault: true);

		public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get { return ImmutableArray.Create(Rule); } }

		public override void Initialize(AnalysisContext context)
		{
			context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
			context.EnableConcurrentExecution();

			context.RegisterOperationAction(AnalyzeInvocation, OperationKind.Invocation);
			context.RegisterOperationAction(AnalyzeFieldReference, OperationKind.FieldReference);
			context.RegisterOperationAction(AnalyzePropertyReference, OperationKind.PropertyReference);
		}

		private static void AnalyzeInvocation(OperationAnalysisContext context)
		{
			var operation = context.Operation;
			var invocation = operation as IInvocationOperation;
			var attribute = invocation.TargetMethod.GetAttribute<ThisAttribute>();

			if (attribute == null) return;
			if (attribute.ConstructorArguments.Length == 0) return;

			var scopeName = context.ContainingSymbol.GetScopeName();
			var targetScopeName = attribute.ConstructorArguments[0].Value as string;

			if (scopeName == targetScopeName) return;

			context.ReportDiagnostic(Diagnostic.Create(Rule, invocation.Syntax.GetLocation(), new object[] { invocation.TargetMethod.Name, targetScopeName, scopeName }));
		}

		private static void AnalyzeFieldReference(OperationAnalysisContext context)
		{
			var operation = context.Operation;
			var reference = operation as IFieldReferenceOperation;
			var attribute = reference.Field.GetAttribute<ThisAttribute>();

			if (attribute == null) return;
			if (attribute.ConstructorArguments.Length == 0) return;

			var scopeName = context.ContainingSymbol.GetScopeName();
			var targetScopeName = attribute.ConstructorArguments[0].Value as string;

			if (scopeName == targetScopeName) return;

			context.ReportDiagnostic(Diagnostic.Create(Rule, reference.Syntax.GetLocation(), new object[] { reference.Field.Name, targetScopeName, scopeName }));
		}

		private static void AnalyzePropertyReference(OperationAnalysisContext context)
		{
			var operation = context.Operation;
			var reference = operation as IPropertyReferenceOperation;

			var attributeOnProperty = reference.Property.GetAttribute<ThisAttribute>();
			var attributeOnSetter = reference.Property.SetMethod?.GetAttribute<ThisAttribute>() ?? attributeOnProperty;
			var attributeOnGetter = reference.Property.GetMethod?.GetAttribute<ThisAttribute>() ?? attributeOnProperty;

			if (attributeOnSetter == null && attributeOnGetter == null) return;

			var usage = operation.GetValueUsageInfo(context.ContainingSymbol);

			if (attributeOnSetter != null && attributeOnSetter.ConstructorArguments.Length != 0 && usage.HasFlag(ValueUsageInfo.Write))
			{
				var scopeName = context.ContainingSymbol.GetScopeName();
				var targetScopeName = attributeOnSetter.ConstructorArguments[0].Value as string;

				if (scopeName != targetScopeName)
				{
					context.ReportDiagnostic(Diagnostic.Create(Rule, reference.Syntax.GetLocation(), new object[] { reference.Property.Name, targetScopeName, scopeName }));
					return;
				}
			}

			if (attributeOnGetter != null && attributeOnGetter.ConstructorArguments.Length != 0 && usage.HasFlag(ValueUsageInfo.Read))
			{
				var scopeName = context.ContainingSymbol.GetScopeName();
				var targetScopeName = attributeOnGetter.ConstructorArguments[0].Value as string;

				if (scopeName != targetScopeName)
				{
					context.ReportDiagnostic(Diagnostic.Create(Rule, reference.Syntax.GetLocation(), new object[] { reference.Property.Name, targetScopeName, scopeName }));
					return;
				}
			}
		}
	}
}
