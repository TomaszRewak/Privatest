using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Operations;
using Privatest.Extensions;
using System.Collections.Immutable;

namespace Privatest
{
	[DiagnosticAnalyzer(LanguageNames.CSharp)]
	internal sealed class ThisAnalyzer : DiagnosticAnalyzer
	{
		public const string DiagnosticId = "Privatest0002";

		private static readonly DiagnosticDescriptor Rule = new DiagnosticDescriptor(
			DiagnosticId,
			"The member is inaccesible due to its protection level",
			"`{0}` is inaccesible due to its protection level. It can only be accessed through the `this` reference in a private scope.",
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
			if (invocation.Instance == null || invocation.Instance.Kind == OperationKind.InstanceReference) return;

			var attribute = invocation.TargetMethod.GetAttribute<ThisAttribute>();
			if (attribute == null) return;

			context.ReportDiagnostic(Diagnostic.Create(Rule, invocation.Syntax.GetLocation(), new object[] { invocation.TargetMethod.Name }));
		}

		private static void AnalyzeFieldReference(OperationAnalysisContext context)
		{
			var operation = context.Operation;

			var reference = operation as IFieldReferenceOperation;
			if (reference.Instance == null || reference.Instance.Kind == OperationKind.InstanceReference) return;

			var attribute = reference.Field.GetAttribute<ThisAttribute>();
			if (attribute == null) return;

			context.ReportDiagnostic(Diagnostic.Create(Rule, reference.Syntax.GetLocation(), new object[] { reference.Field.Name }));
		}

		private static void AnalyzePropertyReference(OperationAnalysisContext context)
		{
			var operation = context.Operation;

			var reference = operation as IPropertyReferenceOperation;
			if (reference.Instance == null || reference.Instance.Kind == OperationKind.InstanceReference) return;

			var attributeOnProperty = reference.Property.HasAttribute<ThisAttribute>();
			var attributeOnSetter = (reference.Property.SetMethod?.HasAttribute<ThisAttribute>() ?? false) || attributeOnProperty;
			var attributeOnGetter = (reference.Property.GetMethod?.HasAttribute<ThisAttribute>() ?? false) || attributeOnProperty;

			if (!attributeOnSetter && !attributeOnGetter) return;

			var usage = operation.GetValueUsageInfo(context.ContainingSymbol);

			if (attributeOnSetter && usage.HasFlag(ValueUsageInfo.Write))
			{
				context.ReportDiagnostic(Diagnostic.Create(Rule, reference.Syntax.GetLocation(), new object[] { reference.Property.Name }));
				return;
			}

			if (attributeOnGetter && usage.HasFlag(ValueUsageInfo.Read))
			{
				context.ReportDiagnostic(Diagnostic.Create(Rule, reference.Syntax.GetLocation(), new object[] { reference.Property.Name }));
				return;
			}
		}
	}
}
