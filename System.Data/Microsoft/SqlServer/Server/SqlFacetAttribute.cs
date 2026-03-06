using System;

namespace Microsoft.SqlServer.Server
{
	/// <summary>Annotates the returned result of a user-defined type (UDT) with additional information that can be used in Transact-SQL.</summary>
	[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter | AttributeTargets.ReturnValue, AllowMultiple = false, Inherited = false)]
	public class SqlFacetAttribute : Attribute
	{
		/// <summary>Indicates whether the return type of the user-defined type is of a fixed length.</summary>
		/// <returns>
		///   <see langword="true" /> if the return type is of a fixed length; otherwise <see langword="false" />.</returns>
		public bool IsFixedLength
		{
			get
			{
				return this.m_IsFixedLength;
			}
			set
			{
				this.m_IsFixedLength = value;
			}
		}

		/// <summary>The maximum size, in logical units, of the underlying field type of the user-defined type.</summary>
		/// <returns>An <see cref="T:System.Int32" /> representing the maximum size, in logical units, of the underlying field type.</returns>
		public int MaxSize
		{
			get
			{
				return this.m_MaxSize;
			}
			set
			{
				this.m_MaxSize = value;
			}
		}

		/// <summary>The precision of the return type of the user-defined type.</summary>
		/// <returns>An <see cref="T:System.Int32" /> representing the precision of the return type.</returns>
		public int Precision
		{
			get
			{
				return this.m_Precision;
			}
			set
			{
				this.m_Precision = value;
			}
		}

		/// <summary>The scale of the return type of the user-defined type.</summary>
		/// <returns>An <see cref="T:System.Int32" /> representing the scale of the return type.</returns>
		public int Scale
		{
			get
			{
				return this.m_Scale;
			}
			set
			{
				this.m_Scale = value;
			}
		}

		/// <summary>Indicates whether the return type of the user-defined type can be <see langword="null" />.</summary>
		/// <returns>
		///   <see langword="true" /> if the return type of the user-defined type can be <see langword="null" />; otherwise <see langword="false" />.</returns>
		public bool IsNullable
		{
			get
			{
				return this.m_IsNullable;
			}
			set
			{
				this.m_IsNullable = value;
			}
		}

		private bool m_IsFixedLength;

		private int m_MaxSize;

		private int m_Scale;

		private int m_Precision;

		private bool m_IsNullable;
	}
}
