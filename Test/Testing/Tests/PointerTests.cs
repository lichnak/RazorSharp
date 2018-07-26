using NUnit.Framework;
using RazorSharp.Pointers;

namespace Test.Testing.Tests
{

	[TestFixture]
	internal unsafe class PointerTests
	{
		[Test]
		public void Test()
		{
			string        x = "foo";
			string        y = "bar";
			Pointer<char> p = x;
			TestingAssertion.AssertElements(p, x);
			p = y;
			TestingAssertion.AssertElements(p, y);

			int[]        arr = {1, 2, 3};
			Pointer<int> p2  = arr;
			TestingAssertion.AssertElements(p2, arr);


			string        z     = "anime";
			Pointer<char> chPtr = z;

			Assert.That(chPtr[0], Is.EqualTo(z[0]));
			chPtr++;
			Assert.That(chPtr[0], Is.EqualTo(z[1]));

			//AssertElements(chPtr, z);
		}


	}

}