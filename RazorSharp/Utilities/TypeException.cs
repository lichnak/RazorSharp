using System;

namespace RazorSharp.Utilities
{

	internal class TypeException : Exception
	{
		internal TypeException(string s) : base(s)
		{

		}

		internal TypeException(Type expected, Type actual) : base($"Expected: typeof({expected.Name}), actual: {actual.Name}")
		{

		}

		internal static void Throw<TExpected, TActual>()
		{
			throw new TypeException(typeof(TExpected), typeof(TActual));
		}
	}

}