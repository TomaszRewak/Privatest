using System;

namespace Privatest.Example
{
	public class Class1
	{
		private int Property { get; [This] set; }

		[This] private void Method1()
		{

		}

		private void Method2()
		{
			Method1();
			Property = 4;
		}

		private void Method3(Class1 c)
		{
			c.Method1();
			c.Property = 1;
		}
	}
}
