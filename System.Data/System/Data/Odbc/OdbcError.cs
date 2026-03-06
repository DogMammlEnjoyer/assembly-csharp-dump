using System;
using Unity;

namespace System.Data.Odbc
{
	/// <summary>Collects information relevant to a warning or error returned by the data source.</summary>
	[Serializable]
	public sealed class OdbcError
	{
		internal OdbcError(string source, string message, string state, int nativeerror)
		{
			this._source = source;
			this._message = message;
			this._state = state;
			this._nativeerror = nativeerror;
		}

		/// <summary>Gets a short description of the error.</summary>
		/// <returns>A description of the error.</returns>
		public string Message
		{
			get
			{
				if (this._message == null)
				{
					return string.Empty;
				}
				return this._message;
			}
		}

		/// <summary>Gets the five-character error code that follows the ANSI SQL standard for the database.</summary>
		/// <returns>The five-character error code, which identifies the source of the error if the error can be issued from more than one place.</returns>
		public string SQLState
		{
			get
			{
				return this._state;
			}
		}

		/// <summary>Gets the data source-specific error information.</summary>
		/// <returns>The data source-specific error information.</returns>
		public int NativeError
		{
			get
			{
				return this._nativeerror;
			}
		}

		/// <summary>Gets the name of the driver that generated the error.</summary>
		/// <returns>The name of the driver that generated the error.</returns>
		public string Source
		{
			get
			{
				if (this._source == null)
				{
					return string.Empty;
				}
				return this._source;
			}
		}

		internal void SetSource(string Source)
		{
			this._source = Source;
		}

		/// <summary>Gets the complete text of the error message.</summary>
		/// <returns>The complete text of the error.</returns>
		public override string ToString()
		{
			return this.Message;
		}

		internal OdbcError()
		{
			ThrowStub.ThrowNotSupportedException();
		}

		internal string _message;

		internal string _state;

		internal int _nativeerror;

		internal string _source;
	}
}
