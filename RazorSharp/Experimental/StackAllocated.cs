using System;
using System.Collections;
using System.Diagnostics;
using System.Linq;
using RazorCommon;
using RazorCommon.Extensions;
using RazorSharp.Analysis;
using RazorSharp.Utilities;

namespace RazorSharp.Experimental
{
	using CSUnsafe = System.Runtime.CompilerServices.Unsafe;

	/// <summary>
	/// Creates types in stack memory.<para></para>
	///
	/// Types that cannot be created in stack memory:<para></para>
	/// - String <para></para>
	/// - IList <para></para>
	/// </summary>
	/// <typeparam name="T"></typeparam>
	public unsafe struct StackAllocated<T> where T : class
	{
		/// <summary>
		/// Types that can't be created in stack memory
		/// (out of the types that have been tested)
		/// </summary>
		private static readonly Type[] DisallowedTypes =
		{
			typeof(string),
			typeof(IList),
		};

		private readonly byte* m_stackPtr;

		// 'Heap' pointer points to m_stackPtr + sizeof(ObjHeader)
		private T m_dummy;

		public T Value {
			get => m_dummy;
			set => m_dummy = ReAllocateRefOnStack(ref value);
		}

		private T ReAllocateRefOnStack(ref T refValue)
		{
			var refMem  = Unsafe.MemoryOf(ref refValue);
			var allocSize = Unsafe.BaseInstanceSize<T>();
			Debug.Assert(refMem.Length == allocSize);

			for (int i = 0; i < allocSize; i++) {
				m_stackPtr[i] = refMem[i];
			}

			// Skip over ObjHeader
			Unsafe.WriteReference(ref refValue, m_stackPtr + IntPtr.Size);
			return refValue;
		}

		/// <summary>
		/// Use: Use stackalloc to allocate "Unsafe.BaseInstanceSize" bytes on
		/// the stack. Then pass the byte* pointer.
		/// </summary>
		/// <param name="stackPtr"></param>
		public StackAllocated(byte* stackPtr)
		{
			if (DisallowedTypes.Contains(typeof(T))) {
				throw new TypeException($"Type {typeof(T).Name} cannot be created in stack memory.");
			}

			m_stackPtr = stackPtr;

			T dummy = Activator.CreateInstance<T>();
			m_dummy = dummy;
			m_dummy = ReAllocateRefOnStack(ref dummy);

		}

		public override string ToString()
		{
			var table = new ConsoleTable("Field", "Value");
			table.AddRow("Value", Value);
			table.AddRow("Stack", Hex.ToHex(m_stackPtr));
			table.AddRow("Dummy heap pointer", Hex.ToHex(Unsafe.AddressOfHeap(ref m_dummy)));
			return table.ToMarkDownString();
		}
	}

}