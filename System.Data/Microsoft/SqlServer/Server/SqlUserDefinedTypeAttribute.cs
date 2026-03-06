using System;
using System.Data.Common;

namespace Microsoft.SqlServer.Server
{
	/// <summary>Used to mark a type definition in an assembly as a user-defined type (UDT) in SQL Server. The properties on the attribute reflect the physical characteristics used when the type is registered with SQL Server. This class cannot be inherited.</summary>
	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct, AllowMultiple = false, Inherited = true)]
	public sealed class SqlUserDefinedTypeAttribute : Attribute
	{
		/// <summary>A required attribute on a user-defined type (UDT), used to confirm that the given type is a UDT and to indicate the storage format of the UDT.</summary>
		/// <param name="format">One of the <see cref="T:Microsoft.SqlServer.Server.Format" /> values representing the serialization format of the type.</param>
		public SqlUserDefinedTypeAttribute(Format format)
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

		/// <summary>The maximum size of the instance, in bytes.</summary>
		/// <returns>An <see cref="T:System.Int32" /> value representing the maximum size of the instance.</returns>
		public int MaxByteSize
		{
			get
			{
				return this.m_MaxByteSize;
			}
			set
			{
				if (value < -1)
				{
					throw ADP.ArgumentOutOfRange("MaxByteSize");
				}
				this.m_MaxByteSize = value;
			}
		}

		/// <summary>Indicates whether all instances of this user-defined type are the same length.</summary>
		/// <returns>
		///   <see langword="true" /> if all instances of this type are the same length; otherwise <see langword="false" />.</returns>
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

		/// <summary>Indicates whether the user-defined type is byte ordered.</summary>
		/// <returns>
		///   <see langword="true" /> if the user-defined type is byte ordered; otherwise <see langword="false" />.</returns>
		public bool IsByteOrdered
		{
			get
			{
				return this.m_IsByteOrdered;
			}
			set
			{
				this.m_IsByteOrdered = value;
			}
		}

		/// <summary>The serialization format as a <see cref="T:Microsoft.SqlServer.Server.Format" />.</summary>
		/// <returns>A <see cref="T:Microsoft.SqlServer.Server.Format" /> value representing the serialization format.</returns>
		public Format Format
		{
			get
			{
				return this.m_format;
			}
		}

		/// <summary>The name of the method used to validate instances of the user-defined type.</summary>
		/// <returns>A <see cref="T:System.String" /> representing the name of the method used to validate instances of the user-defined type.</returns>
		public string ValidationMethodName
		{
			get
			{
				return this.m_ValidationMethodName;
			}
			set
			{
				this.m_ValidationMethodName = value;
			}
		}

		/// <summary>The SQL Server name of the user-defined type.</summary>
		/// <returns>A <see cref="T:System.String" /> value representing the SQL Server name of the user-defined type.</returns>
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

		private bool m_IsFixedLength;

		private bool m_IsByteOrdered;

		private Format m_format;

		private string m_fName;

		internal const int YukonMaxByteSizeValue = 8000;

		private string m_ValidationMethodName;
	}
}
