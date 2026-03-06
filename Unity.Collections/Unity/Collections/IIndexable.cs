using System;
using System.Runtime.CompilerServices;

namespace Unity.Collections
{
	public interface IIndexable<[IsUnmanaged] T> where T : struct, ValueType
	{
		int Length { get; set; }

		ref T ElementAt(int index);
	}
}
