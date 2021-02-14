using System;

namespace Privatest
{
	[AttributeUsage(AttributeTargets.Property | AttributeTargets.Method | AttributeTargets.Field, AllowMultiple = false)]
	public sealed class ThisAttribute : Attribute
	{
		public ThisAttribute() { }
		public ThisAttribute(string methodOrPropertyName) { }
	}
}
