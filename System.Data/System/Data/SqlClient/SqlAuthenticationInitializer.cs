using System;
using Unity;

namespace System.Data.SqlClient
{
	/// <summary>Defines the core behavior of authentication initializers that can be registered in the app.config file and provides a base for derived classes.</summary>
	public abstract class SqlAuthenticationInitializer
	{
		/// <summary>Called from constructors in derived classes to initialize the  <see cref="T:System.Data.SqlClient.SqlAuthenticationInitializer" /> class.</summary>
		protected SqlAuthenticationInitializer()
		{
			ThrowStub.ThrowNotSupportedException();
		}

		/// <summary>When overridden in a derived class, initializes the authentication initializer. This method is called by the <see cref="M:System.Data.SqlClient.SqlAuthenticationInitializer.#ctor" /> constructor during startup.</summary>
		public abstract void Initialize();
	}
}
