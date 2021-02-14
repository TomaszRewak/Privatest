using System;

namespace Privatest
{
	/// <summary>
	/// Used to limit the accessibility of a member to the instance-level
	/// </summary>
	[AttributeUsage(AttributeTargets.Property | AttributeTargets.Method | AttributeTargets.Field, AllowMultiple = false)]
	public sealed class ThisAttribute : Attribute
	{
		public ThisAttribute() { }
		public ThisAttribute(string methodOrPropertyName) { }
	}
}
