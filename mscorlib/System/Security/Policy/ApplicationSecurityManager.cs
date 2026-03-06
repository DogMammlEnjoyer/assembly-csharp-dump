using System;
using System.Runtime.InteropServices;
using System.Security.Permissions;

namespace System.Security.Policy
{
	/// <summary>Manages trust decisions for manifest-activated applications.</summary>
	[ComVisible(true)]
	public static class ApplicationSecurityManager
	{
		/// <summary>Gets the current application trust manager.</summary>
		/// <returns>An <see cref="T:System.Security.Policy.IApplicationTrustManager" /> that represents the current trust manager.</returns>
		/// <exception cref="T:System.Security.Policy.PolicyException">The policy on this application does not have a trust manager.</exception>
		public static IApplicationTrustManager ApplicationTrustManager
		{
			[SecurityPermission(SecurityAction.Demand, ControlPolicy = true)]
			get
			{
				if (ApplicationSecurityManager._appTrustManager == null)
				{
					ApplicationSecurityManager._appTrustManager = new MonoTrustManager();
				}
				return ApplicationSecurityManager._appTrustManager;
			}
		}

		/// <summary>Gets an application trust collection that contains the cached trust decisions for the user.</summary>
		/// <returns>An <see cref="T:System.Security.Policy.ApplicationTrustCollection" /> that contains the cached trust decisions for the user.</returns>
		public static ApplicationTrustCollection UserApplicationTrusts
		{
			get
			{
				if (ApplicationSecurityManager._userAppTrusts == null)
				{
					ApplicationSecurityManager._userAppTrusts = new ApplicationTrustCollection();
				}
				return ApplicationSecurityManager._userAppTrusts;
			}
		}

		/// <summary>Determines whether the user approves the specified application to execute with the requested permission set.</summary>
		/// <param name="activationContext">An <see cref="T:System.ActivationContext" /> identifying the activation context for the application.</param>
		/// <param name="context">A <see cref="T:System.Security.Policy.TrustManagerContext" /> identifying the trust manager context for the application.</param>
		/// <returns>
		///   <see langword="true" /> to execute the specified application; otherwise, <see langword="false" />.</returns>
		/// <exception cref="T:System.ArgumentNullException">The <paramref name="activationContext" /> parameter is <see langword="null" />.</exception>
		[MonoTODO("Missing application manifest support")]
		[SecurityPermission(SecurityAction.Demand, ControlPolicy = true, ControlEvidence = true)]
		public static bool DetermineApplicationTrust(ActivationContext activationContext, TrustManagerContext context)
		{
			if (activationContext == null)
			{
				throw new NullReferenceException("activationContext");
			}
			return ApplicationSecurityManager.ApplicationTrustManager.DetermineApplicationTrust(activationContext, context).IsApplicationTrustedToRun;
		}

		private static IApplicationTrustManager _appTrustManager;

		private static ApplicationTrustCollection _userAppTrusts;
	}
}
