using System;
using System.Security;

namespace System.IO.IsolatedStorage
{
	/// <summary>Provides settings for maintaining the quota size for isolated storage.</summary>
	public class IsolatedStorageSecurityState : SecurityState
	{
		internal IsolatedStorageSecurityState()
		{
		}

		/// <summary>Gets the option for managing isolated storage security.</summary>
		/// <returns>The option to increase the isolated quota storage size.</returns>
		public IsolatedStorageSecurityOptions Options
		{
			get
			{
				return IsolatedStorageSecurityOptions.IncreaseQuotaForApplication;
			}
		}

		/// <summary>Gets or sets the current size of the quota for isolated storage.</summary>
		/// <returns>The current quota size, in bytes.</returns>
		public long Quota
		{
			get
			{
				throw new NotImplementedException();
			}
			set
			{
			}
		}

		/// <summary>Gets the current usage size in isolated storage.</summary>
		/// <returns>The current usage size, in bytes.</returns>
		public long UsedSize
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		/// <summary>Ensures that the state that is represented by <see cref="T:System.IO.IsolatedStorage.IsolatedStorageSecurityState" /> is available on the host.</summary>
		/// <exception cref="T:System.IO.IsolatedStorage.IsolatedStorageException">The state is not available.</exception>
		public override void EnsureState()
		{
			throw new NotImplementedException();
		}
	}
}
