using System;

namespace System.Runtime.InteropServices
{
	/// <summary>Wraps objects the marshaler should marshal as a <see langword="VT_UNKNOWN" />.</summary>
	public sealed class UnknownWrapper
	{
		/// <summary>Initializes a new instance of the <see cref="T:System.Runtime.InteropServices.UnknownWrapper" /> class with the object to be wrapped.</summary>
		/// <param name="obj">The object being wrapped.</param>
		public UnknownWrapper(object obj)
		{
			this.m_WrappedObject = obj;
		}

		/// <summary>Gets the object contained by this wrapper.</summary>
		/// <returns>The wrapped object.</returns>
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
