using System;

namespace System.Runtime.InteropServices
{
	/// <summary>Wraps objects the marshaler should marshal as a <see langword="VT_CY" />.</summary>
	public sealed class CurrencyWrapper
	{
		/// <summary>Initializes a new instance of the <see cref="T:System.Runtime.InteropServices.CurrencyWrapper" /> class with the <see langword="Decimal" /> to be wrapped and marshaled as type <see langword="VT_CY" />.</summary>
		/// <param name="obj">The <see langword="Decimal" /> to be wrapped and marshaled as <see langword="VT_CY" />.</param>
		public CurrencyWrapper(decimal obj)
		{
			this.m_WrappedObject = obj;
		}

		/// <summary>Initializes a new instance of the <see cref="T:System.Runtime.InteropServices.CurrencyWrapper" /> class with the object containing the <see langword="Decimal" /> to be wrapped and marshaled as type <see langword="VT_CY" />.</summary>
		/// <param name="obj">The object containing the <see langword="Decimal" /> to be wrapped and marshaled as <see langword="VT_CY" />.</param>
		/// <exception cref="T:System.ArgumentException">The <paramref name="obj" /> parameter is not a <see cref="T:System.Decimal" /> type.</exception>
		public CurrencyWrapper(object obj)
		{
			if (!(obj is decimal))
			{
				throw new ArgumentException("Object must be of type Decimal.", "obj");
			}
			this.m_WrappedObject = (decimal)obj;
		}

		/// <summary>Gets the wrapped object to be marshaled as type <see langword="VT_CY" />.</summary>
		/// <returns>The wrapped object to be marshaled as type <see langword="VT_CY" />.</returns>
		public decimal WrappedObject
		{
			get
			{
				return this.m_WrappedObject;
			}
		}

		private decimal m_WrappedObject;
	}
}
