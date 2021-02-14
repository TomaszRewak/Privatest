using Microsoft.CodeAnalysis;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using VerifyCS = Privatest.Test.CSharpAnalyzerVerifier<Privatest.ThisAnalyzer>;

namespace Privatest.Test
{
	[TestClass]
	public partial class AttributePlacementTests
	{
		[TestMethod]
		public async Task OnPublicProperty()
		{
			Expect(0, Accessibility.Public, "Property1");

			await Verify(@"
				class Class1
				{
					[This] public int |#0:Property1| { get; set; }
				}
			");
		}
	}
}
