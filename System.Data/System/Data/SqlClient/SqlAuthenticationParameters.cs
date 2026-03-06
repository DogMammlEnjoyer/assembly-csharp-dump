using System;
using System.Runtime.CompilerServices;
using Unity;

namespace System.Data.SqlClient
{
	/// <summary>Represents AD authentication parameters passed by a driver to authentication providers.</summary>
	public class SqlAuthenticationParameters
	{
		/// <summary>Initializes a new instance of the <see cref="T:System.Data.SqlClient.SqlAuthenticationParameters" /> class using the specified authentication method, server name, database name, resource URI, authority URI, user login name/ID, user password and connection ID.</summary>
		/// <param name="authenticationMethod">One of the enumeration values that specifies the authentication method.</param>
		/// <param name="serverName">The server name.</param>
		/// <param name="databaseName">The database name.</param>
		/// <param name="resource">The resource URI.</param>
		/// <param name="authority">The authority URI.</param>
		/// <param name="userId">The user login name/ID.</param>
		/// <param name="password">The user password.</param>
		/// <param name="connectionId">The connection ID.</param>
		protected SqlAuthenticationParameters(SqlAuthenticationMethod authenticationMethod, string serverName, string databaseName, string resource, string authority, string userId, string password, Guid connectionId)
		{
			ThrowStub.ThrowNotSupportedException();
		}

		/// <summary>Gets the authentication method.</summary>
		/// <returns>The authentication method.</returns>
		public SqlAuthenticationMethod AuthenticationMethod
		{
			[CompilerGenerated]
			get
			{
				ThrowStub.ThrowNotSupportedException();
				return SqlAuthenticationMethod.NotSpecified;
			}
		}

		/// <summary>Gets the authority URI.</summary>
		/// <returns>The authority URI.</returns>
		public string Authority
		{
			[CompilerGenerated]
			get
			{
				ThrowStub.ThrowNotSupportedException();
				return null;
			}
		}

		/// <summary>Gets the connection ID.</summary>
		/// <returns>The connection ID.</returns>
		public Guid ConnectionId
		{
			[CompilerGenerated]
			get
			{
				ThrowStub.ThrowNotSupportedException();
				return default(Guid);
			}
		}

		/// <summary>Gets the database name.</summary>
		/// <returns>The database name.</returns>
		public string DatabaseName
		{
			[CompilerGenerated]
			get
			{
				ThrowStub.ThrowNotSupportedException();
				return null;
			}
		}

		/// <summary>Gets the user password.</summary>
		/// <returns>The user password.</returns>
		public string Password
		{
			[CompilerGenerated]
			get
			{
				ThrowStub.ThrowNotSupportedException();
				return null;
			}
		}

		/// <summary>Gets the resource URI.</summary>
		/// <returns>The resource URI.</returns>
		public string Resource
		{
			[CompilerGenerated]
			get
			{
				ThrowStub.ThrowNotSupportedException();
				return null;
			}
		}

		/// <summary>Gets the server name.</summary>
		/// <returns>The server name.</returns>
		public string ServerName
		{
			[CompilerGenerated]
			get
			{
				ThrowStub.ThrowNotSupportedException();
				return null;
			}
		}

		/// <summary>Gets the user login name/ID.</summary>
		/// <returns>The user login name/ID.</returns>
		public string UserId
		{
			[CompilerGenerated]
			get
			{
				ThrowStub.ThrowNotSupportedException();
				return null;
			}
		}
	}
}
