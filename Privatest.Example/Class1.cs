﻿using System;

namespace Privatest.Example
{
	class C { private static int a; }

	public class Class1
	{
		[This] public int _field;
		[This] public int Property1 { get; set; }
		private int Property2 { get; [This(nameof(Method1))] set; }

		[This(nameof(FrontProperty))] private int _backField;
		public int FrontProperty
		{
			get => _backField;
			set => _backField = value;
		}

		private void Method()
		{
			_backField = 2;
		}













		public int Property3
		{
			get
			{
				Property2 = 3;
				var a = Property2;
				Method1();
				return 2;
			}
		}

		[This(nameof(FrontProperty))]
		private void Method1()
		{
			Property2 = 2;
			_backField = 3;

			FrontProperty = 2;
		}

		private void Method2()
		{
			Method1();
			Property1 = 4;
			_field = 1;

			FrontProperty = 1;

			C.a = 3;
		}

		private void Method3(Class1 c)
		{
			Method1();

			c.Method1();
			c.Property1 = 1;
			c._field = 3;
		}
	}
}
