using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;

namespace Privatest.Test
{
	[TestClass]
	public partial class ThisAnalyzerTests
	{
		[TestMethod]
		public async Task AccessingProperty_OfCurrentObject_Test()
		{
			await Verify(@"
				class Class
				{
					[This] private int Property { get; set; }

					public void Method()
					{
						var temp = Property;
						Property = temp;
					}
				}
			");
		}

		[TestMethod]
		public async Task SettingProperty_OfCurrentObject_WithExplicitThis_Test()
		{
			await Verify(@"
				class Class
				{
					[This] private int Property { get; set; }

					public void Method()
					{
						var temp = this.Property;
						this.Property = temp;
					}
				}
			");
		}

		[TestMethod]
		public async Task AccessingField_OfCurrentObject_Test()
		{
			await Verify(@"
				class Class
				{
					[This] private int _field;

					public void Method()
					{
						var temp = _field;
						_field = temp;
					}
				}
			");
		}

		[TestMethod]
		public async Task SettingField_OfCurrentObject_WithExplicitThis_Test()
		{
			await Verify(@"
				class Class
				{
					[This] private int _field;

					public void Method()
					{
						var temp = this._field;
						this._field = temp;
					}
				}
			");
		}

		[TestMethod]
		public async Task AccessingMethod_OfCurrentObject_Test()
		{
			await Verify(@"
				class Class
				{
					[This] private void Method1()
					{ }

					public void Method2()
					{
						Method1();
					}
				}
			");
		}

		[TestMethod]
		public async Task SettingMethod_OfCurrentObject_WithExplicitThis_Test()
		{
			await Verify(@"
				class Class
				{
					[This] private void Method1()
					{ }

					public void Method2()
					{
						this.Method1();
					}
				}
			");
		}

		[TestMethod]
		public async Task AccessingProperty_OfCurrentObject_WithExplicitAttributesOnAccessors_Test()
		{
			await Verify(@"
				class Class
				{
					private int Property { [This] get; [This] set; }

					public void Method()
					{
						var temp = Property;
						Property = temp;
					}
				}
			");
		}

		[TestMethod]
		public async Task AccessingGetter_OfDifferentObject_WithExplicitAttributesOnSetter_Test()
		{
			await Verify(@"
				class Class
				{
					private int Property { get; [This] set; }

					public void Method(Class other)
					{
						Property = other.Property;
					}
				}
			");
		}

		[TestMethod]
		public async Task AccessingSetter_OfDifferentObject_WithExplicitAttributesOnGetter_Test()
		{
			await Verify(@"
				class Class
				{
					private int Property { [This] get; set; }

					public void Method(Class other)
					{
						other.Property = Property;
					}
				}
			");
		}

		[TestMethod]
		public async Task SettingProperty_OfDifferentObject_Test()
		{
			Expect(0, "Property");

			await Verify(@"
				class Class
				{
					[This] private int Property { get; set; }

					public void Method(Class other)
					{
						{|#0:other.Property|} = Property;
					}
				}
			");
		}

		[TestMethod]
		public async Task GettingProperty_OfDifferentObject_Test()
		{
			Expect(0, "Property");

			await Verify(@"
				class Class
				{
					[This] private int Property { get; set; }

					public void Method(Class other)
					{
						Property = {|#0:other.Property|};
					}
				}
			");
		}

		[TestMethod]
		public async Task SettingProperty_OfDifferentObject_WithExplicitAttributesOnSetter_Test()
		{
			Expect(0, "Property");

			await Verify(@"
				class Class
				{
					private int Property { get; [This] set; }

					public void Method(Class other)
					{
						{|#0:other.Property|} = Property;
					}
				}
			");
		}

		[TestMethod]
		public async Task GettingProperty_OfDifferentObject_WithExplicitAttributesOnGetter_Test()
		{
			Expect(0, "Property");

			await Verify(@"
				class Class
				{
					private int Property { [This] get; set; }

					public void Method(Class other)
					{
						Property = {|#0:other.Property|};
					}
				}
			");
		}

		[TestMethod]
		public async Task SettingField_OfDifferentObject_Test()
		{
			Expect(0, "_filed");

			await Verify(@"
				class Class
				{
					[This] private int _filed;

					public void Method(Class other)
					{
						{|#0:other._filed|} = _filed;
					}
				}
			");
		}

		[TestMethod]
		public async Task GettingField_OfDifferentObject_Test()
		{
			Expect(0, "_filed");

			await Verify(@"
				class Class
				{
					[This] private int _filed;

					public void Method(Class other)
					{
						_filed = {|#0:other._filed|};
					}
				}
			");
		}

		[TestMethod]
		public async Task AccessingMethod_OfDifferentObject_Test()
		{
			Expect(0, "Method1");

			await Verify(@"
				class Class
				{
					[This] private void Method1()
					{ }

					public void Method2(Class other)
					{
						{|#0:other.Method1()|};
					}
				}
			");
		}
	}
}
