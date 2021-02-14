using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;

namespace Privatest.Test
{
	[TestClass]
	public partial class ScopeAnalyzerTests
	{
		[TestMethod]
		public async Task SpecifyingScope_WithString_Test()
		{
			Expect(0, "Property", "Method1", "Method2");

			await Verify(@"
				class Class
				{
					[This(""Method1"")] private int Property { get; set; }

					public void Method1()
					{
						Property = 2;
					}

					public void Method2()
					{
						{|#0:Property|} = 2;
					}
				}
			");
		}

		[TestMethod]
		public async Task SpecifyingScope_WithNameOf_Test()
		{
			Expect(0, "Property", "Method1", "Method2");

			await Verify(@"
				class Class
				{
					[This(nameof(Method1))] private int Property { get; set; }

					public void Method1()
					{
						Property = 2;
					}

					public void Method2()
					{
						{|#0:Property|} = 2;
					}
				}
			");
		}

		[TestMethod]
		public async Task AccessingProperty_InCorrectScope_Test()
		{
			await Verify(@"
				class Class
				{
					[This(nameof(Method))] private int Property { get; set; }

					public void Method()
					{
						var temp = Property;
						Property = temp;
					}
				}
			");
		}

		[TestMethod]
		public async Task AccessingField_InCorrectScope_Test()
		{
			await Verify(@"
				class Class
				{
					[This(nameof(Method))] private int _field;

					public void Method()
					{
						var temp = _field;
						_field = temp;
					}
				}
			");
		}

		[TestMethod]
		public async Task AccessingMethod_InCorrectScope_Test()
		{
			await Verify(@"
				class Class
				{
					[This(nameof(Method2))] private void Method1()
					{ }

					public void Method2()
					{
						Method1();
					}
				}
			");
		}

		[TestMethod]
		public async Task AccessingProperty_InCorrectScope_WithExplicitAttributesOnAccessors_Test()
		{
			await Verify(@"
				class Class
				{
					private int Property { [This(nameof(Method))] get; [This(nameof(Method))] set; }

					public void Method()
					{
						var temp = Property;
						Property = temp;
					}
				}
			");
		}

		[TestMethod]
		public async Task AccessingGetter_WithExplicitAttributesOnSetter_Test()
		{
			await Verify(@"
				class Class
				{
					private int Property { get; [This(""Method1"")] set; }

					public void Method2(Class other)
					{
						var temp = Property;
					}
				}
			");
		}

		[TestMethod]
		public async Task AccessingSetter_WithExplicitAttributesOnGetter_Test()
		{
			await Verify(@"
				class Class
				{
					private int Property { [This(""Method1"")] get; set; }

					public void Method2(Class other)
					{
						Property = 2;
					}
				}
			");
		}

		[TestMethod]
		public async Task SettingProperty_InIncorrectScope_Test()
		{
			Expect(0, "Property", "Method1", "Method2");

			await Verify(@"
				class Class
				{
					[This(""Method1"")] private int Property { get; set; }

					public void Method2(Class other)
					{
						{|#0:Property|} = 2;
					}
				}
			");
		}

		[TestMethod]
		public async Task GettingProperty_InIncorrectScope_Test()
		{
			Expect(0, "Property", "Method1", "Method2");

			await Verify(@"
				class Class
				{
					[This(""Method1"")] private int Property { get; set; }

					public void Method2(Class other)
					{
						var temp = {|#0:Property|};
					}
				}
			");
		}

		[TestMethod]
		public async Task SettingProperty_InIncorrectScope_WithExplicitAttributesOnSetter_Test()
		{
			Expect(0, "Property", "Method1", "Method2");

			await Verify(@"
				class Class
				{
					private int Property { get; [This(""Method1"")] set; }

					public void Method2(Class other)
					{
						{|#0:Property|} = 2;
					}
				}
			");
		}

		[TestMethod]
		public async Task GettingProperty_InIncorrectScope_WithExplicitAttributesOnGetter_Test()
		{
			Expect(0, "Property", "Method1", "Method2");

			await Verify(@"
				class Class
				{
					private int Property { [This(""Method1"")] get; set; }

					public void Method2(Class other)
					{
						var temp = {|#0:Property|};
					}
				}
			");
		}

		[TestMethod]
		public async Task SettingField_InIncorrectScope_Test()
		{
			Expect(0, "_filed", "Method1", "Method2");

			await Verify(@"
				class Class
				{
					[This(""Method1"")] private int _filed;

					public void Method2(Class other)
					{
						{|#0:_filed|} = 2;
					}
				}
			");
		}

		[TestMethod]
		public async Task GettingField_InIncorrectScope_Test()
		{
			Expect(0, "_filed", "Method1", "Method2");

			await Verify(@"
				class Class
				{
					[This(""Method1"")] private int _filed;

					public void Method2(Class other)
					{
						var temp = {|#0:_filed|};
					}
				}
			");
		}

		[TestMethod]
		public async Task AccessingMethod_InIncorrectScope_Test()
		{
			Expect(0, "Method1", "Method2", "Method3");

			await Verify(@"
				class Class
				{
					[This(""Method2"")] private void Method1()
					{ }

					public void Method3(Class other)
					{
						{|#0:Method1()|};
					}
				}
			");
		}

		[TestMethod]
		public async Task OverwritingScope_OfGetter_Test()
		{
			Expect(0, "Property", "Method1", "Method2");

			await Verify(@"
				class Class
				{
					[This(nameof(Method2))] private int Property { [This(nameof(Method1))] get; set; }

					public void Method1(Class other)
					{
						var temp = other.Property;
					}

					public void Method2(Class other)
					{
						var temp = {|#0:other.Property|};
					}
				}
			");
		}

		[TestMethod]
		public async Task OverwritingScope_OfSetter_Test()
		{
			Expect(0, "Property", "Method1", "Method2");

			await Verify(@"
				class Class
				{
					[This(nameof(Method2))] private int Property { get; [This(nameof(Method1))] set; }

					public void Method1(Class other)
					{
						other.Property = 2;
					}

					public void Method2(Class other)
					{
						{|#0:other.Property|} = 2;
					}
				}
			");
		}
	}
}
