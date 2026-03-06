using System;
using System.Data.Common;

namespace Microsoft.SqlServer.Server
{
	/// <summary>Indicates that the type should be registered as a user-defined aggregate. The properties on the attribute reflect the physical attributes used when the type is registered with SQL Server. This class cannot be inherited.</summary>
	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct, AllowMultiple = false, Inherited = false)]
	public sealed class SqlUserDefinedAggregateAttribute : Attribute
	{
		/// <summary>A required attribute on a user-defined aggregate, used to indicate that the given type is a user-defined aggregate and the storage format of the user-defined aggregate.</summary>
		/// <param name="format">One of the <see cref="T:Microsoft.SqlServer.Server.Format" /> values representing the serialization format of the aggregate.</param>
		public SqlUserDefinedAggregateAttribute(Format format)
		{
			if (format == Format.Unknown)
			{
				throw ADP.NotSupportedUserDefinedTypeSerializationFormat(format, "format");
			}
			if (format - Format.Native > 1)
			{
				throw ADP.InvalidUserDefinedTypeSerializationFormat(format);
			}
			this.m_format = format;
		}

		/// <summary>The maximum size, in bytes, of the aggregate instance.</summary>
		/// <returns>An <see cref="T:System.Int32" /> value representing the maximum size of the aggregate instance.</returns>
		public int MaxByteSize
		{
			get
			{
				return this.m_MaxByteSize;
			}
			set
			{
				if (value < -1 || value > 8000)
				{
					throw ADP.ArgumentOutOfRange(Res.GetString("range: 0-8000"), "MaxByteSize", value);
				}
				this.m_MaxByteSize = value;
			}
		}

		/// <summary>Indicates whether the aggregate is invariant to duplicates.</summary>
		/// <returns>
		///   <see langword="true" /> if the aggregate is invariant to duplicates; otherwise <see langword="false" />.</returns>
		public bool IsInvariantToDuplicates
		{
			get
			{
				return this.m_fInvariantToDup;
			}
			set
			{
				this.m_fInvariantToDup = value;
			}
		}

		/// <summary>Indicates whether the aggregate is invariant to nulls.</summary>
		/// <returns>
		///   <see langword="true" /> if the aggregate is invariant to nulls; otherwise <see langword="false" />.</returns>
		public bool IsInvariantToNulls
		{
			get
			{
				return this.m_fInvariantToNulls;
			}
			set
			{
				this.m_fInvariantToNulls = value;
			}
		}

		/// <summary>Indicates whether the aggregate is invariant to order.</summary>
		/// <returns>
		///   <see langword="true" /> if the aggregate is invariant to order; otherwise <see langword="false" />.</returns>
		public bool IsInvariantToOrder
		{
			get
			{
				return this.m_fInvariantToOrder;
			}
			set
			{
				this.m_fInvariantToOrder = value;
			}
		}

		/// <summary>Indicates whether the aggregate returns <see langword="null" /> if no values have been accumulated.</summary>
		/// <returns>
		///   <see langword="true" /> if the aggregate returns <see langword="null" /> if no values have been accumulated; otherwise <see langword="false" />.</returns>
		public bool IsNullIfEmpty
		{
			get
			{
				return this.m_fNullIfEmpty;
			}
			set
			{
				this.m_fNullIfEmpty = value;
			}
		}

		/// <summary>The serialization format as a <see cref="T:Microsoft.SqlServer.Server.Format" />.</summary>
		/// <returns>A <see cref="T:Microsoft.SqlServer.Server.Format" /> representing the serialization format.</returns>
		public Format Format
		{
			get
			{
				return this.m_format;
			}
		}

		/// <summary>The name of the aggregate.</summary>
		/// <returns>A <see cref="T:System.String" /> value representing the name of the aggregate.</returns>
		public string Name
		{
			get
			{
				return this.m_fName;
			}
			set
			{
				this.m_fName = value;
			}
		}

		private int m_MaxByteSize;

		private bool m_fInvariantToDup;

		private bool m_fInvariantToNulls;

		private bool m_fInvariantToOrder = true;

		private bool m_fNullIfEmpty;

		private Format m_format;

		private string m_fName;

		/// <summary>The maximum size, in bytes, required to store the state of this aggregate instance during computation.</summary>
		public const int MaxByteSizeValue = 8000;
	}
}
