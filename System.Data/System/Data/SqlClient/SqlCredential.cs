using System;
using System.Security;

namespace System.Data.SqlClient
{
	/// <summary>
	///   <see cref="T:System.Data.SqlClient.SqlCredential" /> provides a more secure way to specify the password for a login attempt using SQL Server Authentication.  
	/// <see cref="T:System.Data.SqlClient.SqlCredential" /> is comprised of a user id and a password that will be used for SQL Server Authentication. The password in a <see cref="T:System.Data.SqlClient.SqlCredential" /> object is of type <see cref="T:System.Security.SecureString" />.  
	/// <see cref="T:System.Data.SqlClient.SqlCredential" /> cannot be inherited.  
	/// Windows Authentication (<see langword="Integrated Security = true" />) remains the most secure way to log in to a SQL Server database.</summary>
	[Serializable]
	public sealed class SqlCredential
	{
		/// <summary>Creates an object of type <see cref="T:System.Data.SqlClient.SqlCredential" />.</summary>
		/// <param name="userId">The user id.</param>
		/// <param name="password">The password; a <see cref="T:System.Security.SecureString" /> value marked as read-only.  Passing a read/write <see cref="T:System.Security.SecureString" /> parameter will raise an <see cref="T:System.ArgumentException" />.</param>
		public SqlCredential(string userId, SecureString password)
		{
			if (userId == null)
			{
				throw new ArgumentNullException("userId");
			}
			if (password == null)
			{
				throw new ArgumentNullException("password");
			}
			this.uid = userId;
			this.pwd = password;
		}

		/// <summary>Gets the user ID component of the <see cref="T:System.Data.SqlClient.SqlCredential" /> object.</summary>
		/// <returns>The user ID component of the <see cref="T:System.Data.SqlClient.SqlCredential" /> object.</returns>
		public string UserId
		{
			get
			{
				return this.uid;
			}
		}

		/// <summary>Gets the password component of the <see cref="T:System.Data.SqlClient.SqlCredential" /> object.</summary>
		/// <returns>The password component of the <see cref="T:System.Data.SqlClient.SqlCredential" /> object.</returns>
		public SecureString Password
		{
			get
			{
				return this.pwd;
			}
		}

		private string uid = "";

		private SecureString pwd;
	}
}
