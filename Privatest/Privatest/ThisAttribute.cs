using System;

namespace Privatest
{
	[AttributeUsage(AttributeTargets.Property | AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
	public class ThisAttribute : Attribute
	{ }
}
