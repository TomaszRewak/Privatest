using Microsoft.CodeAnalysis;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;

namespace Privatest.Test
{
	[TestClass]
	public partial class AttributePlacementTests
	{
		[TestMethod]
		public async Task OnPrivateProperty()
		{
			await Verify(@"
				class Class
				{
					[This] private int Property { get; set; }
				}
			");
		}

		[TestMethod]
		public async Task OnSetterOfPrivateProperty()
		{
			await Verify(@"
				class Class
				{
					private int Property { get; [This] set; }
				}
			");
		}

		[TestMethod]
		public async Task OnGetterOfPrivateProperty()
		{
			await Verify(@"
				class Class
				{
					private int Property { [This] get; set; }
				}
			");
		}

		[TestMethod]
		public async Task OnPrivateSetterOfNonPrivateProperty()
		{
			await Verify(@"
				class Class
				{
					public int Property { get; [This] private set; }
				}
			");
		}

		[TestMethod]
		public async Task OnPrivateGetterOfNonPrivateProperty()
		{
			await Verify(@"
				class Class
				{
					public int Property { [This] private get; set; }
				}
			");
		}

		[TestMethod]
		public async Task OnPrivateField()
		{
			await Verify(@"
				class Class
				{
					[This] private int Field;
				}
			");
		}

		[TestMethod]
		public async Task OnPrivateMethod()
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
		public async Task OnNonPrivateProperty()
		{
			Expect(0, Accessibility.Public, "Property");

			await Verify(@"
				class Class
				{
					[This] public int {|#0:Property|} { get; set; }
				}
			");
		}

		[TestMethod]
		public async Task OnNonPrivateGetter()
		{
			Expect(0, Accessibility.Public, "get_Property");

			await Verify(@"
				class Class
				{
					public int Property { [This] {|#0:get|}; set; }
				}
			");
		}

		[TestMethod]
		public async Task OnNonPrivateSetter()
		{
			Expect(0, Accessibility.Public, "set_Property");

			await Verify(@"
				class Class
				{
					public int Property { get; [This] {|#0:set|}; }
				}
			");
		}

		[TestMethod]
		public async Task OnNonPrivateGetterAndSetter()
		{
			Expect(0, Accessibility.Public, "get_Property");
			Expect(1, Accessibility.Public, "set_Property");

			await Verify(@"
				class Class
				{
					public int Property { [This] {|#0:get|}; [This] {|#1:set|}; }
				}
			");
		}

		[TestMethod]
		public async Task OnNonPrivateField()
		{
			Expect(0, Accessibility.Public, "Field");

			await Verify(@"
				class Class
				{
					[This] public int {|#0:Field|};
				}
			");
		}

		[TestMethod]
		public async Task OnNonPrivateMethod()
		{
			Expect(0, Accessibility.Public, "Method");

			await Verify(@"
				class Class
				{
					[This] public void {|#0:Method|}()
					{ }
				}
			");
		}
	}
}
