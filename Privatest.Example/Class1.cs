using System;

namespace Privatest.Example
{
	public class Class1
	{
		[This] private int _field;
		private int Property { get; [This] set; }

		[This] private void Method1()
		{

		}

		private void Method2()
		{
			Method1();
			Property = 4;
			_field = 1;
		}

		private void Method3(Class1 c)
		{
			c.Method1();
			c.Property = 1;
			c._field = 3;
		}
	}
}
