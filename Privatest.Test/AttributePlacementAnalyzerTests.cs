using Microsoft.CodeAnalysis;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;

namespace Privatest.Test
{
	[TestClass]
	public partial class AttributePlacementAnalyzerTests
	{
		[TestMethod]
		public async Task OnPrivateProperty_Test()
		{
			await Verify(@"
				class Class
				{
					[This] private int Property { get; set; }
				}
			");
		}

		[TestMethod]
		public async Task OnSetter_OfPrivateProperty_Test()
		{
			await Verify(@"
				class Class
				{
					private int Property { get; [This] set; }
				}
			");
		}

		[TestMethod]
		public async Task OnGetter_OfPrivateProperty_Test()
		{
			await Verify(@"
				class Class
				{
					private int Property { [This] get; set; }
				}
			");
		}

		[TestMethod]
		public async Task OnPrivateSetter_OfNonPrivateProperty_Test()
		{
			await Verify(@"
				class Class
				{
					public int Property { get; [This] private set; }
				}
			");
		}

		[TestMethod]
		public async Task OnPrivateGetter_OfNonPrivateProperty_Test()
		{
			await Verify(@"
				class Class
				{
					public int Property { [This] private get; set; }
				}
			");
		}

		[TestMethod]
		public async Task OnPrivateField_Test()
		{
			await Verify(@"
				class Class
				{
					[This] private int Field;
				}
			");
		}

		[TestMethod]
		public async Task OnPrivateBackingField_Test()
		{
			await Verify(@"
				class Class
				{
					[BackingField] private int Field;
				}
			");
		}

		[TestMethod]
		public async Task OnPrivateMethod_Test()
		{
			await Verify(@"
				class Class
				{
					[This] private void Method()
					{ }
				}
			");
		}

		[TestMethod]
		public async Task OnNonPrivateProperty_Test()
		{
			Expect(0, Accessibility.Public, "Property", 0);

			await Verify(@"
				class Class
				{
					[This] public int {|#0:Property|} { get; set; }
				}
			");
		}

		[TestMethod]
		public async Task OnNonPrivateGetter_Test()
		{
			Expect(0, Accessibility.Public, "get_Property", 0);

			await Verify(@"
				class Class
				{
					public int Property { [This] {|#0:get|}; set; }
				}
			", @"
				class Class
				{
					public int Property { [This] private get; set; }
				}
			");
		}

		[TestMethod]
		public async Task OnNonPrivateSetter_Test()
		{
			Expect(0, Accessibility.Public, "set_Property", 0);

			await Verify(@"
				class Class
				{
					public int Property { get; [This] {|#0:set|}; }
				}
			", @"
				class Class
				{
					public int Property { get; [This] private set; }
				}
			");
		}

		[TestMethod]
		public async Task OnNonPrivateGetterAndSetter_Test()
		{
			Expect(0, Accessibility.Public, "get_Property", 0);
			Expect(1, Accessibility.Public, "set_Property", 0);

			await Verify(@"
				class Class
				{
					public int Property { [This] {|#0:get|}; [This] {|#1:set|}; }
				}
			");
		}

		[TestMethod]
		public async Task OnNonPrivateField_Test()
		{
			Expect(0, Accessibility.Public, "Field", 0);

			await Verify(@"
				class Class
				{
					[This] public int {|#0:Field|};
				}
			", @"
				class Class
				{
					[This] private int Field;
				}
			");
		}

		[TestMethod]
		public async Task OnNonPrivateMethod_Test()
		{
			Expect(0, Accessibility.Public, "Method", 0);

			await Verify(@"
				class Class
				{
					[This] public void {|#0:Method|}()
					{ }
				}
			");
		}

		[TestMethod]
		public async Task OnNonPrivateBackingField_Test()
		{
			Expect(0, Accessibility.Public, "Field", 1);

			await Verify(@"
				class Class
				{
					[BackingField] public int {|#0:Field|};
				}
			", @"
				class Class
				{
					[BackingField] private int Field;
				}
			");
		}
	}
}
