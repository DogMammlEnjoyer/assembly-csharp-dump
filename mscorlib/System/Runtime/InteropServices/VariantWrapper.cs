using System;

namespace System.Runtime.InteropServices
{
	/// <summary>Marshals data of type <see langword="VT_VARIANT | VT_BYREF" /> from managed to unmanaged code. This class cannot be inherited.</summary>
	public sealed class VariantWrapper
	{
		/// <summary>Initializes a new instance of the <see cref="T:System.Runtime.InteropServices.VariantWrapper" /> class for the specified <see cref="T:System.Object" /> parameter.</summary>
		/// <param name="obj">The object to marshal.</param>
		public VariantWrapper(object obj)
		{
			this.m_WrappedObject = obj;
		}

		/// <summary>Gets the object wrapped by the <see cref="T:System.Runtime.InteropServices.VariantWrapper" /> object.</summary>
		/// <returns>The object wrapped by the <see cref="T:System.Runtime.InteropServices.VariantWrapper" /> object.</returns>
		public object WrappedObject
		{
			get
			{
				return this.m_WrappedObject;
			}
		}

		private object m_WrappedObject;
	}
}
