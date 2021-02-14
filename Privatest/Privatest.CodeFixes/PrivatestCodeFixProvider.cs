using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Editing;
using System.Collections.Immutable;
using System.Composition;
using System.Linq;
using System.Threading.Tasks;

namespace Privatest
{
	[ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(PrivatestCodeFixProvider)), Shared]
	internal sealed class PrivatestCodeFixProvider : CodeFixProvider
	{
		public sealed override ImmutableArray<string> FixableDiagnosticIds
		{
			get { return ImmutableArray.Create("Privatest0001"); }
		}

		public sealed override FixAllProvider GetFixAllProvider()
		{
			return WellKnownFixAllProviders.BatchFixer;
		}

		public sealed override async Task RegisterCodeFixesAsync(CodeFixContext context)
		{
			var root = await context.Document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false);

			var diagnostic = context.Diagnostics.First();
			var diagnosticSpan = diagnostic.Location.SourceSpan;

			var declaration = root
				.FindToken(diagnosticSpan.Start)
				.Parent
				.AncestorsAndSelf()
				.Where(s => s is AccessorDeclarationSyntax || s is MemberDeclarationSyntax)
				.FirstOrDefault();

			if (declaration == null) return;

			if (declaration is MemberDeclarationSyntax memberDeclarationSyntax)
			{
				context.RegisterCodeFix(
					CodeAction.Create(
						"Make private",
						_ => MakePrivate(context.Document, root, memberDeclarationSyntax)),
					diagnostic);
			}

			if (declaration is AccessorDeclarationSyntax accessorDeclarationSyntax)
			{
				context.RegisterCodeFix(
					CodeAction.Create(
						"Make private",
						_ => MakePrivate(context.Document, root, accessorDeclarationSyntax)),
					diagnostic);
			}
		}

		private Task<Solution> MakePrivate(Document document, SyntaxNode root, MemberDeclarationSyntax typeDecl)
		{
			var generator = SyntaxGenerator.GetGenerator(document);
			var newStatement = generator.WithAccessibility(typeDecl, Accessibility.Private);

			var newRoot = root.ReplaceNode(typeDecl, newStatement);
			return Task.FromResult(document.WithSyntaxRoot(newRoot).Project.Solution);
		}

		private Task<Solution> MakePrivate(Document document, SyntaxNode root, AccessorDeclarationSyntax typeDecl)
		{
			var generator = SyntaxGenerator.GetGenerator(document);
			var newStatement = generator.WithAccessibility(typeDecl, Accessibility.Private);

			var newRoot = root.ReplaceNode(typeDecl, newStatement);
			return Task.FromResult(document.WithSyntaxRoot(newRoot).Project.Solution);
		}
	}
}
