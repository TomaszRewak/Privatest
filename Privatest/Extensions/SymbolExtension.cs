using Microsoft.CodeAnalysis;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Privatest.Extensions
{
	internal static class SymbolExtension
	{
		public static IEnumerable<AttributeData> GetAttributes<T>(this ISymbol type)
		{
			return type
				.GetAttributes()
				.Where(a => a.AttributeClass.GetFullName() == typeof(T).FullName);
		}

		public static bool HasAttribute<T>(this ISymbol type)
		{
			return type
				.GetAttributes<T>()
				.Any();
		}

		public static AttributeData GetAttribute<T>(this ISymbol type)
		{
			return type
				.GetAttributes<T>()
				.FirstOrDefault();
		}

		public static bool TryGetAttribute<T>(this ISymbol type, out AttributeData attribute)
		{
			attribute = type.GetAttribute<T>();
			return attribute != null;
		}

		public static string GetFullName(this ITypeSymbol symbol)
		{
			return $"{symbol.GetNamespace()}.{symbol.Name}";
		}

		public static string GetNamespace(this ISymbol symbol)
		{
			var namespaces = new List<INamespaceSymbol>();

			for (var currentNamespace = symbol.ContainingNamespace; !currentNamespace.IsGlobalNamespace; currentNamespace = currentNamespace.ContainingNamespace)
				namespaces.Add(currentNamespace);

			return string.Join(".", namespaces.Select(n => n.Name).Reverse());
		}

		public static string GetScopeName(this ISymbol symbol)
		{
			while (symbol != null && !(symbol.ContainingSymbol is ITypeSymbol))
				symbol = symbol.ContainingSymbol;

			if (symbol is IMethodSymbol methodSymbol)
				return methodSymbol.AssociatedSymbol?.Name ?? symbol.Name;

			return null;
		}
	}
}
