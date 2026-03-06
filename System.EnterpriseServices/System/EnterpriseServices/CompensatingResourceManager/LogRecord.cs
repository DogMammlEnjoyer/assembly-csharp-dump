using System;

namespace System.EnterpriseServices.CompensatingResourceManager
{
	/// <summary>Represents an unstructured log record delivered as a COM+ <see langword="CrmLogRecordRead" /> structure. This class cannot be inherited.</summary>
	public sealed class LogRecord
	{
		[MonoTODO]
		internal LogRecord()
		{
		}

		[MonoTODO]
		internal LogRecord(_LogRecord logRecord)
		{
			this.flags = (LogRecordFlags)logRecord.dwCrmFlags;
			this.sequence = logRecord.dwSequenceNumber;
			this.record = logRecord.blobUserData;
		}

		/// <summary>Gets a value that indicates when the log record was written.</summary>
		/// <returns>A bitwise combination of the <see cref="T:System.EnterpriseServices.CompensatingResourceManager.LogRecordFlags" /> values which provides information about when this record was written.</returns>
		public LogRecordFlags Flags
		{
			get
			{
				return this.flags;
			}
		}

		/// <summary>Gets the log record user data.</summary>
		/// <returns>A single BLOB that contains the user data.</returns>
		public object Record
		{
			get
			{
				return this.record;
			}
		}

		/// <summary>The sequence number of the log record.</summary>
		/// <returns>An integer value that specifies the sequence number of the log record.</returns>
		public int Sequence
		{
			get
			{
				return this.sequence;
			}
		}

		private LogRecordFlags flags;

		private object record;

		private int sequence;
	}
}
