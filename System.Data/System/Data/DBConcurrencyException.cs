using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace System.Data
{
	/// <summary>The exception that is thrown by the <see cref="T:System.Data.Common.DataAdapter" /> during an insert, update, or delete operation if the number of rows affected equals zero.</summary>
	[Serializable]
	public sealed class DBConcurrencyException : SystemException
	{
		/// <summary>Initializes a new instance of the <see cref="T:System.Data.DBConcurrencyException" /> class.</summary>
		public DBConcurrencyException() : this("DB concurrency violation.", null)
		{
		}

		/// <summary>Initializes a new instance of the <see cref="T:System.Data.DBConcurrencyException" /> class.</summary>
		/// <param name="message">The text string describing the details of the exception.</param>
		public DBConcurrencyException(string message) : this(message, null)
		{
		}

		/// <summary>Initializes a new instance of the <see cref="T:System.Data.DBConcurrencyException" /> class.</summary>
		/// <param name="message">The text string describing the details of the exception.</param>
		/// <param name="inner">A reference to an inner exception.</param>
		public DBConcurrencyException(string message, Exception inner) : base(message, inner)
		{
			base.HResult = -2146232011;
		}

		/// <summary>Initializes a new instance of the <see cref="T:System.Data.DBConcurrencyException" /> class.</summary>
		/// <param name="message">The error message that explains the reason for this exception.</param>
		/// <param name="inner">The exception that is the cause of the current exception, or a null reference (<see langword="Nothing" /> in Visual Basic) if no inner exception is specified.</param>
		/// <param name="dataRows">An array containing the <see cref="T:System.Data.DataRow" /> objects whose update failure generated this exception.</param>
		public DBConcurrencyException(string message, Exception inner, DataRow[] dataRows) : base(message, inner)
		{
			base.HResult = -2146232011;
			this._dataRows = dataRows;
		}

		private DBConcurrencyException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		/// <summary>Populates the aprcified serialization information object with the data needed to serialize the <see cref="T:System.Data.DBConcurrencyException" />.</summary>
		/// <param name="si">A <see cref="T:System.Runtime.Serialization.SerializationInfo" /> that holds the serialized data associated with the <see cref="T:System.Data.DBConcurrencyException" />.</param>
		/// <param name="context">A <see cref="T:System.Runtime.Serialization.StreamingContext" /> that contains the source and destination of the serialized stream associated with the <see cref="T:System.Data.DBConcurrencyException" />.</param>
		/// <exception cref="T:System.ArgumentNullException">The <paramref name="info" /> parameter is a null reference (<see langword="Nothing" /> in Visual Basic).</exception>
		[SecurityPermission(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.SerializationFormatter)]
		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
		}

		/// <summary>Gets or sets the value of the <see cref="T:System.Data.DataRow" /> that generated the <see cref="T:System.Data.DBConcurrencyException" />.</summary>
		/// <returns>The value of the <see cref="T:System.Data.DataRow" />.</returns>
		public DataRow Row
		{
			get
			{
				DataRow[] dataRows = this._dataRows;
				if (dataRows == null || dataRows.Length == 0)
				{
					return null;
				}
				return dataRows[0];
			}
			set
			{
				this._dataRows = new DataRow[]
				{
					value
				};
			}
		}

		/// <summary>Gets the number of rows whose update failed, generating this exception.</summary>
		/// <returns>An integer containing a count of the number of rows whose update failed.</returns>
		public int RowCount
		{
			get
			{
				DataRow[] dataRows = this._dataRows;
				if (dataRows == null)
				{
					return 0;
				}
				return dataRows.Length;
			}
		}

		/// <summary>Copies the <see cref="T:System.Data.DataRow" /> objects whose update failure generated this exception, to the specified array of <see cref="T:System.Data.DataRow" /> objects.</summary>
		/// <param name="array">The one-dimensional array of <see cref="T:System.Data.DataRow" /> objects to copy the <see cref="T:System.Data.DataRow" /> objects into.</param>
		public void CopyToRows(DataRow[] array)
		{
			this.CopyToRows(array, 0);
		}

		/// <summary>Copies the <see cref="T:System.Data.DataRow" /> objects whose update failure generated this exception, to the specified array of <see cref="T:System.Data.DataRow" /> objects, starting at the specified destination array index.</summary>
		/// <param name="array">The one-dimensional array of <see cref="T:System.Data.DataRow" /> objects to copy the <see cref="T:System.Data.DataRow" /> objects into.</param>
		/// <param name="arrayIndex">The destination array index to start copying into.</param>
		public void CopyToRows(DataRow[] array, int arrayIndex)
		{
			DataRow[] dataRows = this._dataRows;
			if (dataRows != null)
			{
				dataRows.CopyTo(array, arrayIndex);
			}
		}

		private DataRow[] _dataRows;
	}
}
