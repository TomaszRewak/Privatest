using System;

namespace Privatest
{
	/// <summary>
	/// Used to limit the accessibility of a member to the instance-level
	/// </summary>
	[AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
	public sealed class BackingFieldAttribute : Attribute
	{ }
}
