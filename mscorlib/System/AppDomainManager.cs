using System;
using System.Reflection;
using System.Runtime.Hosting;
using System.Runtime.InteropServices;
using System.Security;
using System.Security.Permissions;
using System.Security.Policy;
using System.Threading;

namespace System
{
	/// <summary>Provides a managed equivalent of an unmanaged host.</summary>
	[ComVisible(true)]
	[SecurityPermission(SecurityAction.InheritanceDemand, Infrastructure = true)]
	[SecurityPermission(SecurityAction.LinkDemand, Infrastructure = true)]
	public class AppDomainManager : MarshalByRefObject
	{
		/// <summary>Initializes a new instance of the <see cref="T:System.AppDomainManager" /> class.</summary>
		public AppDomainManager()
		{
			this._flags = AppDomainManagerInitializationOptions.None;
		}

		/// <summary>Gets the application activator that handles the activation of add-ins and manifest-based applications for the domain.</summary>
		/// <returns>The application activator.</returns>
		public virtual ApplicationActivator ApplicationActivator
		{
			get
			{
				if (this._activator == null)
				{
					this._activator = new ApplicationActivator();
				}
				return this._activator;
			}
		}

		/// <summary>Gets the entry assembly for an application.</summary>
		/// <returns>The entry assembly for the application.</returns>
		public virtual Assembly EntryAssembly
		{
			get
			{
				return Assembly.GetEntryAssembly();
			}
		}

		/// <summary>Gets the host execution context manager that manages the flow of the execution context.</summary>
		/// <returns>The host execution context manager.</returns>
		[MonoTODO]
		public virtual HostExecutionContextManager HostExecutionContextManager
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		/// <summary>Gets the host security manager that participates in security decisions for the application domain.</summary>
		/// <returns>The host security manager.</returns>
		public virtual HostSecurityManager HostSecurityManager
		{
			get
			{
				return null;
			}
		}

		/// <summary>Gets the initialization flags for custom application domain managers.</summary>
		/// <returns>A bitwise combination of the enumeration values that describe the initialization action to perform. The default is <see cref="F:System.AppDomainManagerInitializationOptions.None" />.</returns>
		public AppDomainManagerInitializationOptions InitializationFlags
		{
			get
			{
				return this._flags;
			}
			set
			{
				this._flags = value;
			}
		}

		/// <summary>Returns a new or existing application domain.</summary>
		/// <param name="friendlyName">The friendly name of the domain.</param>
		/// <param name="securityInfo">An object that contains evidence mapped through the security policy to establish a top-of-stack permission set.</param>
		/// <param name="appDomainInfo">An object that contains application domain initialization information.</param>
		/// <returns>A new or existing application domain.</returns>
		public virtual AppDomain CreateDomain(string friendlyName, Evidence securityInfo, AppDomainSetup appDomainInfo)
		{
			this.InitializeNewDomain(appDomainInfo);
			AppDomain appDomain = AppDomainManager.CreateDomainHelper(friendlyName, securityInfo, appDomainInfo);
			if ((this.HostSecurityManager.Flags & HostSecurityManagerOptions.HostPolicyLevel) == HostSecurityManagerOptions.HostPolicyLevel)
			{
				PolicyLevel domainPolicy = this.HostSecurityManager.DomainPolicy;
				if (domainPolicy != null)
				{
					appDomain.SetAppDomainPolicy(domainPolicy);
				}
			}
			return appDomain;
		}

		/// <summary>Initializes the new application domain.</summary>
		/// <param name="appDomainInfo">An object that contains application domain initialization information.</param>
		public virtual void InitializeNewDomain(AppDomainSetup appDomainInfo)
		{
		}

		/// <summary>Indicates whether the specified operation is allowed in the application domain.</summary>
		/// <param name="state">A subclass of <see cref="T:System.Security.SecurityState" /> that identifies the operation whose security status is requested.</param>
		/// <returns>
		///   <see langword="true" /> if the host allows the operation specified by <paramref name="state" /> to be performed in the application domain; otherwise, <see langword="false" />.</returns>
		public virtual bool CheckSecuritySettings(SecurityState state)
		{
			return false;
		}

		/// <summary>Provides a helper method to create an application domain.</summary>
		/// <param name="friendlyName">The friendly name of the domain.</param>
		/// <param name="securityInfo">An object that contains evidence mapped through the security policy to establish a top-of-stack permission set.</param>
		/// <param name="appDomainInfo">An object that contains application domain initialization information.</param>
		/// <returns>A newly created application domain.</returns>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="friendlyName" /> is <see langword="null" />.</exception>
		protected static AppDomain CreateDomainHelper(string friendlyName, Evidence securityInfo, AppDomainSetup appDomainInfo)
		{
			return AppDomain.CreateDomain(friendlyName, securityInfo, appDomainInfo);
		}

		private ApplicationActivator _activator;

		private AppDomainManagerInitializationOptions _flags;
	}
}
