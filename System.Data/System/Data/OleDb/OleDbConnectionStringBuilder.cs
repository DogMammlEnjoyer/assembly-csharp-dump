using System;
using System.Collections;
using System.Data.Common;

namespace System.Data.OleDb
{
	/// <summary>Provides a simple way to create and manage the contents of connection strings used by the <see cref="T:System.Data.OleDb.OleDbConnection" /> class.</summary>
	[MonoTODO("OleDb is not implemented.")]
	public sealed class OleDbConnectionStringBuilder : DbConnectionStringBuilder
	{
		/// <summary>Initializes a new instance of the <see cref="T:System.Data.OleDb.OleDbConnectionStringBuilder" /> class.</summary>
		public OleDbConnectionStringBuilder()
		{
			throw ADP.OleDb();
		}

		/// <summary>Initializes a new instance of the <see cref="T:System.Data.OleDb.OleDbConnectionStringBuilder" /> class. The provided connection string provides the data for the instance's internal connection information.</summary>
		/// <param name="connectionString">The basis for the object's internal connection information. Parsed into key/value pairs.</param>
		/// <exception cref="T:System.ArgumentException">The connection string is incorrectly formatted (perhaps missing the required "=" within a key/value pair).</exception>
		public OleDbConnectionStringBuilder(string connectionString)
		{
			throw ADP.OleDb();
		}

		/// <summary>Gets or sets the name of the data source to connect to.</summary>
		/// <returns>The value of the <see cref="P:System.Data.OleDb.OleDbConnectionStringBuilder.DataSource" /> property, or <see langword="String.Empty" /> if none has been supplied.</returns>
		public string DataSource
		{
			get
			{
				throw ADP.OleDb();
			}
			set
			{
			}
		}

		/// <summary>Gets or sets the name of the Universal Data Link (UDL) file for connecting to the data source.</summary>
		/// <returns>The value of the <see cref="P:System.Data.OleDb.OleDbConnectionStringBuilder.FileName" /> property, or <see langword="String.Empty" /> if none has been supplied.</returns>
		public string FileName
		{
			get
			{
				throw ADP.OleDb();
			}
			set
			{
			}
		}

		public object Item
		{
			get
			{
				throw ADP.OleDb();
			}
			set
			{
			}
		}

		/// <summary>Gets an <see cref="T:System.Collections.ICollection" /> that contains the keys in the <see cref="T:System.Data.OleDb.OleDbConnectionStringBuilder" />.</summary>
		/// <returns>An <see cref="T:System.Collections.ICollection" /> that contains the keys in the <see cref="T:System.Data.OleDb.OleDbConnectionStringBuilder" />.</returns>
		public override ICollection Keys
		{
			get
			{
				throw ADP.OleDb();
			}
			set
			{
			}
		}

		/// <summary>Gets or sets the value to be passed for the OLE DB Services key within the connection string.</summary>
		/// <returns>The value corresponding to the OLE DB Services key within the connection string. By default, the value is -13.</returns>
		public int OleDbServices
		{
			get
			{
				throw ADP.OleDb();
			}
			set
			{
			}
		}

		/// <summary>Gets or sets a Boolean value that indicates whether security-sensitive information, such as the password, is returned as part of the connection if the connection is open or has ever been in an open state.</summary>
		/// <returns>The value of the <see cref="P:System.Data.OleDb.OleDbConnectionStringBuilder.PersistSecurityInfo" /> property, or <see langword="false" /> if none has been supplied.</returns>
		public bool PersistSecurityInfo
		{
			get
			{
				throw ADP.OleDb();
			}
			set
			{
			}
		}

		/// <summary>Gets or sets a string that contains the name of the data provider associated with the internal connection string.</summary>
		/// <returns>The value of the <see cref="P:System.Data.OleDb.OleDbConnectionStringBuilder.Provider" /> property, or <see langword="String.Empty" /> if none has been supplied.</returns>
		public string Provider
		{
			get
			{
				throw ADP.OleDb();
			}
			set
			{
			}
		}

		/// <summary>Clears the contents of the <see cref="T:System.Data.OleDb.OleDbConnectionStringBuilder" /> instance.</summary>
		public override void Clear()
		{
			throw ADP.OleDb();
		}

		/// <summary>Determines whether the <see cref="T:System.Data.OleDb.OleDbConnectionStringBuilder" /> contains a specific key.</summary>
		/// <param name="keyword">The key to locate in the <see cref="T:System.Data.OleDb.OleDbConnectionStringBuilder" />.</param>
		/// <returns>
		///   <see langword="true" /> if the <see cref="T:System.Data.OleDb.OleDbConnectionStringBuilder" /> contains an element that has the specified key; otherwise, <see langword="false" />.</returns>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="keyword" /> is null (<see langword="Nothing" /> in Visual Basic).</exception>
		public override bool ContainsKey(string keyword)
		{
			throw ADP.OleDb();
		}

		protected override void GetProperties(Hashtable propertyDescriptors)
		{
			throw ADP.OleDb();
		}

		/// <summary>Removes the entry with the specified key from the <see cref="T:System.Data.OleDb.OleDbConnectionStringBuilder" /> instance.</summary>
		/// <param name="keyword">The key of the key/value pair to be removed from the connection string in this <see cref="T:System.Data.OleDb.OleDbConnectionStringBuilder" />.</param>
		/// <returns>
		///   <see langword="true" /> if the key existed within the connection string and was removed, <see langword="false" /> if the key did not exist.</returns>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="keyword" /> is null (<see langword="Nothing" /> in Visual Basic).</exception>
		public override bool Remove(string keyword)
		{
			throw ADP.OleDb();
		}

		public bool TryGetValue(string keyword, object value)
		{
			throw ADP.OleDb();
		}
	}
}
