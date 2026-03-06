using System;

namespace System.Runtime.InteropServices
{
	/// <summary>Marshals data of type <see langword="VT_BSTR" /> from managed to unmanaged code. This class cannot be inherited.</summary>
	public sealed class BStrWrapper
	{
		/// <summary>Initializes a new instance of the <see cref="T:System.Runtime.InteropServices.BStrWrapper" /> class with the specified <see cref="T:System.String" /> object.</summary>
		/// <param name="value">The object to wrap and marshal as <see langword="VT_BSTR" />.</param>
		public BStrWrapper(string value)
		{
			this.m_WrappedObject = value;
		}

		/// <summary>Initializes a new instance of the <see cref="T:System.Runtime.InteropServices.BStrWrapper" /> class with the specified <see cref="T:System.Object" /> object.</summary>
		/// <param name="value">The object to wrap and marshal as <see langword="VT_BSTR" />.</param>
		public BStrWrapper(object value)
		{
			this.m_WrappedObject = (string)value;
		}

		/// <summary>Gets the wrapped <see cref="T:System.String" /> object to marshal as type <see langword="VT_BSTR" />.</summary>
		/// <returns>The object that is wrapped by <see cref="T:System.Runtime.InteropServices.BStrWrapper" />.</returns>
		public string WrappedObject
		{
			get
			{
				return this.m_WrappedObject;
			}
		}

		private string m_WrappedObject;
	}
}
