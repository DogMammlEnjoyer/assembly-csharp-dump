using System;
using System.Runtime.InteropServices;

namespace System.Security.Permissions
{
	/// <summary>Allows the use of declarative security actions to determine host protection requirements. This class cannot be inherited.</summary>
	[ComVisible(true)]
	[AttributeUsage(AttributeTargets.Assembly | AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Constructor | AttributeTargets.Method | AttributeTargets.Delegate, AllowMultiple = true, Inherited = false)]
	[Serializable]
	public sealed class HostProtectionAttribute : CodeAccessSecurityAttribute
	{
		/// <summary>Initializes a new instance of the <see cref="T:System.Security.Permissions.HostProtectionAttribute" /> class with default values.</summary>
		public HostProtectionAttribute() : base(SecurityAction.LinkDemand)
		{
		}

		/// <summary>Initializes a new instance of the <see cref="T:System.Security.Permissions.HostProtectionAttribute" /> class with the specified <see cref="T:System.Security.Permissions.SecurityAction" /> value.</summary>
		/// <param name="action">One of the <see cref="T:System.Security.Permissions.SecurityAction" /> values.</param>
		/// <exception cref="T:System.ArgumentException">
		///   <paramref name="action" /> is not <see cref="F:System.Security.Permissions.SecurityAction.LinkDemand" />.</exception>
		public HostProtectionAttribute(SecurityAction action) : base(action)
		{
			if (action != SecurityAction.LinkDemand)
			{
				throw new ArgumentException(string.Format(Locale.GetText("Only {0} is accepted."), SecurityAction.LinkDemand), "action");
			}
		}

		/// <summary>Gets or sets a value indicating whether external process management is exposed.</summary>
		/// <returns>
		///   <see langword="true" /> if external process management is exposed; otherwise, <see langword="false" />. The default is <see langword="false" />.</returns>
		public bool ExternalProcessMgmt
		{
			get
			{
				return (this._resources & HostProtectionResource.ExternalProcessMgmt) > HostProtectionResource.None;
			}
			set
			{
				if (value)
				{
					this._resources |= HostProtectionResource.ExternalProcessMgmt;
					return;
				}
				this._resources &= ~HostProtectionResource.ExternalProcessMgmt;
			}
		}

		/// <summary>Gets or sets a value indicating whether external threading is exposed.</summary>
		/// <returns>
		///   <see langword="true" /> if external threading is exposed; otherwise, <see langword="false" />. The default is <see langword="false" />.</returns>
		public bool ExternalThreading
		{
			get
			{
				return (this._resources & HostProtectionResource.ExternalThreading) > HostProtectionResource.None;
			}
			set
			{
				if (value)
				{
					this._resources |= HostProtectionResource.ExternalThreading;
					return;
				}
				this._resources &= ~HostProtectionResource.ExternalThreading;
			}
		}

		/// <summary>Gets or sets a value indicating whether resources might leak memory if the operation is terminated.</summary>
		/// <returns>
		///   <see langword="true" /> if resources might leak memory on termination; otherwise, <see langword="false" />.</returns>
		public bool MayLeakOnAbort
		{
			get
			{
				return (this._resources & HostProtectionResource.MayLeakOnAbort) > HostProtectionResource.None;
			}
			set
			{
				if (value)
				{
					this._resources |= HostProtectionResource.MayLeakOnAbort;
					return;
				}
				this._resources &= ~HostProtectionResource.MayLeakOnAbort;
			}
		}

		/// <summary>Gets or sets a value indicating whether the security infrastructure is exposed.</summary>
		/// <returns>
		///   <see langword="true" /> if the security infrastructure is exposed; otherwise, <see langword="false" />. The default is <see langword="false" />.</returns>
		[ComVisible(true)]
		public bool SecurityInfrastructure
		{
			get
			{
				return (this._resources & HostProtectionResource.SecurityInfrastructure) > HostProtectionResource.None;
			}
			set
			{
				if (value)
				{
					this._resources |= HostProtectionResource.SecurityInfrastructure;
					return;
				}
				this._resources &= ~HostProtectionResource.SecurityInfrastructure;
			}
		}

		/// <summary>Gets or sets a value indicating whether self-affecting process management is exposed.</summary>
		/// <returns>
		///   <see langword="true" /> if self-affecting process management is exposed; otherwise, <see langword="false" />. The default is <see langword="false" />.</returns>
		public bool SelfAffectingProcessMgmt
		{
			get
			{
				return (this._resources & HostProtectionResource.SelfAffectingProcessMgmt) > HostProtectionResource.None;
			}
			set
			{
				if (value)
				{
					this._resources |= HostProtectionResource.SelfAffectingProcessMgmt;
					return;
				}
				this._resources &= ~HostProtectionResource.SelfAffectingProcessMgmt;
			}
		}

		/// <summary>Gets or sets a value indicating whether self-affecting threading is exposed.</summary>
		/// <returns>
		///   <see langword="true" /> if self-affecting threading is exposed; otherwise, <see langword="false" />. The default is <see langword="false" />.</returns>
		public bool SelfAffectingThreading
		{
			get
			{
				return (this._resources & HostProtectionResource.SelfAffectingThreading) > HostProtectionResource.None;
			}
			set
			{
				if (value)
				{
					this._resources |= HostProtectionResource.SelfAffectingThreading;
					return;
				}
				this._resources &= ~HostProtectionResource.SelfAffectingThreading;
			}
		}

		/// <summary>Gets or sets a value indicating whether shared state is exposed.</summary>
		/// <returns>
		///   <see langword="true" /> if shared state is exposed; otherwise, <see langword="false" />. The default is <see langword="false" />.</returns>
		public bool SharedState
		{
			get
			{
				return (this._resources & HostProtectionResource.SharedState) > HostProtectionResource.None;
			}
			set
			{
				if (value)
				{
					this._resources |= HostProtectionResource.SharedState;
					return;
				}
				this._resources &= ~HostProtectionResource.SharedState;
			}
		}

		/// <summary>Gets or sets a value indicating whether synchronization is exposed.</summary>
		/// <returns>
		///   <see langword="true" /> if synchronization is exposed; otherwise, <see langword="false" />. The default is <see langword="false" />.</returns>
		public bool Synchronization
		{
			get
			{
				return (this._resources & HostProtectionResource.Synchronization) > HostProtectionResource.None;
			}
			set
			{
				if (value)
				{
					this._resources |= HostProtectionResource.Synchronization;
					return;
				}
				this._resources &= ~HostProtectionResource.Synchronization;
			}
		}

		/// <summary>Gets or sets a value indicating whether the user interface is exposed.</summary>
		/// <returns>
		///   <see langword="true" /> if the user interface is exposed; otherwise, <see langword="false" />. The default is <see langword="false" />.</returns>
		public bool UI
		{
			get
			{
				return (this._resources & HostProtectionResource.UI) > HostProtectionResource.None;
			}
			set
			{
				if (value)
				{
					this._resources |= HostProtectionResource.UI;
					return;
				}
				this._resources &= ~HostProtectionResource.UI;
			}
		}

		/// <summary>Gets or sets flags specifying categories of functionality that are potentially harmful to the host.</summary>
		/// <returns>A bitwise combination of the <see cref="T:System.Security.Permissions.HostProtectionResource" /> values. The default is <see cref="F:System.Security.Permissions.HostProtectionResource.None" />.</returns>
		public HostProtectionResource Resources
		{
			get
			{
				return this._resources;
			}
			set
			{
				this._resources = value;
			}
		}

		/// <summary>Creates and returns a new host protection permission.</summary>
		/// <returns>An <see cref="T:System.Security.IPermission" /> that corresponds to the current attribute.</returns>
		public override IPermission CreatePermission()
		{
			return new HostProtectionPermission(this._resources);
		}

		private HostProtectionResource _resources;
	}
}
