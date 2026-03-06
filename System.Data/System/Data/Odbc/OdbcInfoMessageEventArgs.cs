using System;
using System.Text;
using Unity;

namespace System.Data.Odbc
{
	/// <summary>Provides data for the <see cref="E:System.Data.Odbc.OdbcConnection.InfoMessage" /> event.</summary>
	public sealed class OdbcInfoMessageEventArgs : EventArgs
	{
		internal OdbcInfoMessageEventArgs(OdbcErrorCollection errors)
		{
			this._errors = errors;
		}

		/// <summary>Gets the collection of warnings sent from the data source.</summary>
		/// <returns>The collection of warnings sent from the data source.</returns>
		public OdbcErrorCollection Errors
		{
			get
			{
				return this._errors;
			}
		}

		/// <summary>Gets the full text of the error sent from the database.</summary>
		/// <returns>The full text of the error.</returns>
		public string Message
		{
			get
			{
				StringBuilder stringBuilder = new StringBuilder();
				foreach (object obj in this.Errors)
				{
					OdbcError odbcError = (OdbcError)obj;
					if (0 < stringBuilder.Length)
					{
						stringBuilder.Append(Environment.NewLine);
					}
					stringBuilder.Append(odbcError.Message);
				}
				return stringBuilder.ToString();
			}
		}

		/// <summary>Retrieves a string representation of the <see cref="E:System.Data.Odbc.OdbcConnection.InfoMessage" /> event.</summary>
		/// <returns>A string representing the <see cref="E:System.Data.Odbc.OdbcConnection.InfoMessage" /> event.</returns>
		public override string ToString()
		{
			return this.Message;
		}

		internal OdbcInfoMessageEventArgs()
		{
			ThrowStub.ThrowNotSupportedException();
		}

		private OdbcErrorCollection _errors;
	}
}
