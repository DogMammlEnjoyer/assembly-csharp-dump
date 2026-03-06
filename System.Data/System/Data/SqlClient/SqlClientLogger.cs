using System;
using Unity;

namespace System.Data.SqlClient
{
	/// <summary>Represents a SQL client logger.</summary>
	public class SqlClientLogger
	{
		/// <summary>Initializes a new instance of the <see cref="T:System.Data.SqlClient.SqlClientLogger" /> class.</summary>
		public SqlClientLogger()
		{
			ThrowStub.ThrowNotSupportedException();
		}

		/// <summary>Gets a value that indicates whether bid tracing is enabled.</summary>
		/// <returns>
		///   <see langword="true" /> if bid tracing is enabled; otherwise, <see langword="false" />.</returns>
		public bool IsLoggingEnabled
		{
			get
			{
				ThrowStub.ThrowNotSupportedException();
				return default(bool);
			}
		}

		/// <summary>Logs the specified message if <paramref name="value" /> is <see langword="false" />.</summary>
		/// <param name="value">
		///   <see langword="false" /> to log the message; otherwise, <see langword="true" />.</param>
		/// <param name="type">The type to be logged.</param>
		/// <param name="method">The logging method.</param>
		/// <param name="message">The message to be logged.</param>
		/// <returns>
		///   <see langword="true" /> if the message is not logged; otherwise, <see langword="false" />.</returns>
		public bool LogAssert(bool value, string type, string method, string message)
		{
			ThrowStub.ThrowNotSupportedException();
			return default(bool);
		}

		/// <summary>Logs an error through a specified method of the current instance type.</summary>
		/// <param name="type">The type to be logged.</param>
		/// <param name="method">The logging method.</param>
		/// <param name="message">The message to be logged.</param>
		public void LogError(string type, string method, string message)
		{
			ThrowStub.ThrowNotSupportedException();
		}

		/// <summary>Logs information through a specified method of the current instance type.</summary>
		/// <param name="type">The type to be logged.</param>
		/// <param name="method">The logging method.</param>
		/// <param name="message">The message to be logged.</param>
		public void LogInfo(string type, string method, string message)
		{
			ThrowStub.ThrowNotSupportedException();
		}
	}
}
