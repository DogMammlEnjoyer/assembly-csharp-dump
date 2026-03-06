using System;

namespace System.Collections
{
	/// <summary>Provides objects for performing a structural comparison of two collection objects.</summary>
	public static class StructuralComparisons
	{
		/// <summary>Gets a predefined object that performs a structural comparison of two objects.</summary>
		/// <returns>A predefined object that is used to perform a structural comparison of two collection objects.</returns>
		public static IComparer StructuralComparer
		{
			get
			{
				IComparer comparer = StructuralComparisons.s_StructuralComparer;
				if (comparer == null)
				{
					comparer = new StructuralComparer();
					StructuralComparisons.s_StructuralComparer = comparer;
				}
				return comparer;
			}
		}

		/// <summary>Gets a predefined object that compares two objects for structural equality.</summary>
		/// <returns>A predefined object that is used to compare two collection objects for structural equality.</returns>
		public static IEqualityComparer StructuralEqualityComparer
		{
			get
			{
				IEqualityComparer equalityComparer = StructuralComparisons.s_StructuralEqualityComparer;
				if (equalityComparer == null)
				{
					equalityComparer = new StructuralEqualityComparer();
					StructuralComparisons.s_StructuralEqualityComparer = equalityComparer;
				}
				return equalityComparer;
			}
		}

		private static volatile IComparer s_StructuralComparer;

		private static volatile IEqualityComparer s_StructuralEqualityComparer;
	}
}
