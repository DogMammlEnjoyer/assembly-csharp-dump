using System;
using System.Threading.Tasks;
using Unity;

namespace System.Data.SqlClient
{
	/// <summary>Defines the core behavior of authentication providers and provides a base class for derived classes.</summary>
	public abstract class SqlAuthenticationProvider
	{
		/// <summary>Called from constructors in derived classes to initialize the <see cref="T:System.Data.SqlClient.SqlAuthenticationProvider" /> class.</summary>
		protected SqlAuthenticationProvider()
		{
			ThrowStub.ThrowNotSupportedException();
		}

		/// <summary>Acquires a security token from the authority.</summary>
		/// <param name="parameters">The Active Directory authentication parameters passed by the driver to authentication providers.</param>
		/// <returns>Represents an asynchronous operation that returns the AD authentication token.</returns>
		public abstract Task<SqlAuthenticationToken> AcquireTokenAsync(SqlAuthenticationParameters parameters);

		/// <summary>This method is called immediately before the provider is added to SQL drivers registry.</summary>
		/// <param name="authenticationMethod">The authentication method.</param>
		public virtual void BeforeLoad(SqlAuthenticationMethod authenticationMethod)
		{
			ThrowStub.ThrowNotSupportedException();
		}

		/// <summary>This method is called immediately before the provider is removed from the SQL drivers registry.</summary>
		/// <param name="authenticationMethod">The authentication method.</param>
		public virtual void BeforeUnload(SqlAuthenticationMethod authenticationMethod)
		{
			ThrowStub.ThrowNotSupportedException();
		}

		/// <summary>Gets an authentication provider by method.</summary>
		/// <param name="authenticationMethod">The authentication method.</param>
		/// <returns>The authentication provider or <see langword="null" /> if not found.</returns>
		public static SqlAuthenticationProvider GetProvider(SqlAuthenticationMethod authenticationMethod)
		{
			ThrowStub.ThrowNotSupportedException();
			return null;
		}

		/// <summary>Indicates whether the specified authentication method is supported.</summary>
		/// <param name="authenticationMethod">The authentication method.</param>
		/// <returns>
		///   <see langword="true" /> if the specified authentication method is supported; otherwise, <see langword="false" />.</returns>
		public abstract bool IsSupported(SqlAuthenticationMethod authenticationMethod);

		/// <summary>Sets an authentication provider by method.</summary>
		/// <param name="authenticationMethod">The authentication method.</param>
		/// <param name="provider">The authentication provider.</param>
		/// <returns>
		///   <see langword="true" /> if the operation succeeded; otherwise, <see langword="false" /> (for example, the existing provider disallows overriding).</returns>
		public static bool SetProvider(SqlAuthenticationMethod authenticationMethod, SqlAuthenticationProvider provider)
		{
			ThrowStub.ThrowNotSupportedException();
			return default(bool);
		}
	}
}
