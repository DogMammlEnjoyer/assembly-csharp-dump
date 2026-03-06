using System;
using System.Data.Common;

namespace System.Data.OleDb
{
	/// <summary>Provides a mechanism for enumerating all available OLE DB providers within the local network.</summary>
	[MonoTODO("OleDb is not implemented.")]
	public sealed class OleDbEnumerator
	{
		/// <summary>Retrieves a <see cref="T:System.Data.DataTable" /> that contains information about all visible OLE DB providers.</summary>
		/// <returns>A <see cref="T:System.Data.DataTable" /> that contains information about the visible OLE DB providers.</returns>
		/// <exception cref="T:System.InvalidCastException">The provider does not support ISourcesRowset.</exception>
		/// <exception cref="T:System.Data.OleDb.OleDbException">Exception has occurred in the underlying provider.</exception>
		public DataTable GetElements()
		{
			throw ADP.OleDb();
		}

		/// <summary>Uses a specific OLE DB enumerator to return an <see cref="T:System.Data.OleDb.OleDbDataReader" /> that contains information about the currently installed OLE DB providers, without requiring an instance of the <see cref="T:System.Data.OleDb.OleDbEnumerator" /> class.</summary>
		/// <param name="type">A <see cref="T:System.Type" />.</param>
		/// <returns>An <see cref="T:System.Data.OleDb.OleDbDataReader" /> that contains information about the requested OLE DB providers, using the specified OLE DB enumerator.</returns>
		/// <exception cref="T:System.InvalidCastException">The provider does not support ISourcesRowset.</exception>
		/// <exception cref="T:System.Data.OleDb.OleDbException">An exception has occurred in the underlying provider.</exception>
		public static OleDbDataReader GetEnumerator(Type type)
		{
			throw ADP.OleDb();
		}

		/// <summary>Returns an <see cref="T:System.Data.OleDb.OleDbDataReader" /> that contains information about the currently installed OLE DB providers, without requiring an instance of the <see cref="T:System.Data.OleDb.OleDbEnumerator" /> class.</summary>
		/// <returns>A <see cref="T:System.Data.OleDb.OleDbDataReader" /> that contains information about the visible OLE DB providers.</returns>
		/// <exception cref="T:System.InvalidCastException">The provider does not support ISourcesRowset.</exception>
		/// <exception cref="T:System.Data.OleDb.OleDbException">Exception has occurred in the underlying provider.</exception>
		public static OleDbDataReader GetRootEnumerator()
		{
			throw ADP.OleDb();
		}
	}
}
