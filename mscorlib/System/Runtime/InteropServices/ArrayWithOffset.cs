using System;
using System.Security;

namespace System.Runtime.InteropServices
{
	/// <summary>Encapsulates an array and an offset within the specified array.</summary>
	[ComVisible(true)]
	[Serializable]
	public struct ArrayWithOffset
	{
		/// <summary>Initializes a new instance of the <see cref="T:System.Runtime.InteropServices.ArrayWithOffset" /> structure.</summary>
		/// <param name="array">A managed array.</param>
		/// <param name="offset">The offset in bytes, of the element to be passed through platform invoke.</param>
		/// <exception cref="T:System.ArgumentException">The array is larger than 2 gigabytes (GB).</exception>
		[SecuritySafeCritical]
		public ArrayWithOffset(object array, int offset)
		{
			this.m_array = array;
			this.m_offset = offset;
			this.m_count = 0;
			this.m_count = this.CalculateCount();
		}

		/// <summary>Returns the managed array referenced by this <see cref="T:System.Runtime.InteropServices.ArrayWithOffset" />.</summary>
		/// <returns>The managed array this instance references.</returns>
		public object GetArray()
		{
			return this.m_array;
		}

		/// <summary>Returns the offset provided when this <see cref="T:System.Runtime.InteropServices.ArrayWithOffset" /> was constructed.</summary>
		/// <returns>The offset for this instance.</returns>
		public int GetOffset()
		{
			return this.m_offset;
		}

		/// <summary>Returns a hash code for this value type.</summary>
		/// <returns>The hash code for this instance.</returns>
		public override int GetHashCode()
		{
			return this.m_count + this.m_offset;
		}

		/// <summary>Indicates whether the specified object matches the current <see cref="T:System.Runtime.InteropServices.ArrayWithOffset" /> object.</summary>
		/// <param name="obj">Object to compare with this instance.</param>
		/// <returns>
		///   <see langword="true" /> if the object matches this <see cref="T:System.Runtime.InteropServices.ArrayWithOffset" />; otherwise, <see langword="false" />.</returns>
		public override bool Equals(object obj)
		{
			return obj is ArrayWithOffset && this.Equals((ArrayWithOffset)obj);
		}

		/// <summary>Indicates whether the specified <see cref="T:System.Runtime.InteropServices.ArrayWithOffset" /> object matches the current instance.</summary>
		/// <param name="obj">An <see cref="T:System.Runtime.InteropServices.ArrayWithOffset" /> object to compare with this instance.</param>
		/// <returns>
		///   <see langword="true" /> if the specified <see cref="T:System.Runtime.InteropServices.ArrayWithOffset" /> object matches the current instance; otherwise, <see langword="false" />.</returns>
		public bool Equals(ArrayWithOffset obj)
		{
			return obj.m_array == this.m_array && obj.m_offset == this.m_offset && obj.m_count == this.m_count;
		}

		/// <summary>Determines whether two specified <see cref="T:System.Runtime.InteropServices.ArrayWithOffset" /> objects have the same value.</summary>
		/// <param name="a">An <see cref="T:System.Runtime.InteropServices.ArrayWithOffset" /> object to compare with the <paramref name="b" /> parameter.</param>
		/// <param name="b">An <see cref="T:System.Runtime.InteropServices.ArrayWithOffset" /> object to compare with the <paramref name="a" /> parameter.</param>
		/// <returns>
		///   <see langword="true" /> if the value of <paramref name="a" /> is the same as the value of <paramref name="b" />; otherwise, <see langword="false" />.</returns>
		public static bool operator ==(ArrayWithOffset a, ArrayWithOffset b)
		{
			return a.Equals(b);
		}

		/// <summary>Determines whether two specified <see cref="T:System.Runtime.InteropServices.ArrayWithOffset" /> objects no not have the same value.</summary>
		/// <param name="a">An <see cref="T:System.Runtime.InteropServices.ArrayWithOffset" /> object to compare with the <paramref name="b" /> parameter.</param>
		/// <param name="b">An <see cref="T:System.Runtime.InteropServices.ArrayWithOffset" /> object to compare with the <paramref name="a" /> parameter.</param>
		/// <returns>
		///   <see langword="true" /> if the value of <paramref name="a" /> is not the same as the value of <paramref name="b" />; otherwise, <see langword="false" />.</returns>
		public static bool operator !=(ArrayWithOffset a, ArrayWithOffset b)
		{
			return !(a == b);
		}

		private int CalculateCount()
		{
			Array array = this.m_array as Array;
			if (array == null)
			{
				throw new ArgumentException();
			}
			return array.Rank * array.Length - this.m_offset;
		}

		private object m_array;

		private int m_offset;

		private int m_count;
	}
}
