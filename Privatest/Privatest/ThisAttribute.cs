using System;

namespace Privatest
{
	[AttributeUsage(AttributeTargets.Property | AttributeTargets.Method | AttributeTargets.Field, AllowMultiple = false, Inherited = false)]
	public class ThisAttribute : Attribute
	{ }
}
