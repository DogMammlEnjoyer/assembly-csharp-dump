using System;
using System.Collections;
using System.ComponentModel.Design;
using System.Reflection;

namespace System.ComponentModel
{
	/// <summary>Provides properties and methods to add a license to a component and to manage a <see cref="T:System.ComponentModel.LicenseProvider" />. This class cannot be inherited.</summary>
	public sealed class LicenseManager
	{
		private LicenseManager()
		{
		}

		/// <summary>Gets or sets the current <see cref="T:System.ComponentModel.LicenseContext" />, which specifies when you can use the licensed object.</summary>
		/// <returns>A <see cref="T:System.ComponentModel.LicenseContext" /> that specifies when you can use the licensed object.</returns>
		/// <exception cref="T:System.InvalidOperationException">The <see cref="P:System.ComponentModel.LicenseManager.CurrentContext" /> property is currently locked and cannot be changed.</exception>
		public static LicenseContext CurrentContext
		{
			get
			{
				if (LicenseManager.s_context == null)
				{
					object obj = LicenseManager.s_internalSyncObject;
					lock (obj)
					{
						if (LicenseManager.s_context == null)
						{
							LicenseManager.s_context = new RuntimeLicenseContext();
						}
					}
				}
				return LicenseManager.s_context;
			}
			set
			{
				object obj = LicenseManager.s_internalSyncObject;
				lock (obj)
				{
					if (LicenseManager.s_contextLockHolder != null)
					{
						throw new InvalidOperationException("The CurrentContext property of the LicenseManager is currently locked and cannot be changed.");
					}
					LicenseManager.s_context = value;
				}
			}
		}

		/// <summary>Gets the <see cref="T:System.ComponentModel.LicenseUsageMode" /> which specifies when you can use the licensed object for the <see cref="P:System.ComponentModel.LicenseManager.CurrentContext" />.</summary>
		/// <returns>One of the <see cref="T:System.ComponentModel.LicenseUsageMode" /> values, as specified in the <see cref="P:System.ComponentModel.LicenseManager.CurrentContext" /> property.</returns>
		public static LicenseUsageMode UsageMode
		{
			get
			{
				if (LicenseManager.s_context != null)
				{
					return LicenseManager.s_context.UsageMode;
				}
				return LicenseUsageMode.Runtime;
			}
		}

		private static void CacheProvider(Type type, LicenseProvider provider)
		{
			if (LicenseManager.s_providers == null)
			{
				LicenseManager.s_providers = new Hashtable();
			}
			LicenseManager.s_providers[type] = provider;
			if (provider != null)
			{
				if (LicenseManager.s_providerInstances == null)
				{
					LicenseManager.s_providerInstances = new Hashtable();
				}
				LicenseManager.s_providerInstances[provider.GetType()] = provider;
			}
		}

		/// <summary>Creates an instance of the specified type, given a context in which you can use the licensed instance.</summary>
		/// <param name="type">A <see cref="T:System.Type" /> that represents the type to create.</param>
		/// <param name="creationContext">A <see cref="T:System.ComponentModel.LicenseContext" /> that specifies when you can use the licensed instance.</param>
		/// <returns>An instance of the specified type.</returns>
		public static object CreateWithContext(Type type, LicenseContext creationContext)
		{
			return LicenseManager.CreateWithContext(type, creationContext, Array.Empty<object>());
		}

		/// <summary>Creates an instance of the specified type with the specified arguments, given a context in which you can use the licensed instance.</summary>
		/// <param name="type">A <see cref="T:System.Type" /> that represents the type to create.</param>
		/// <param name="creationContext">A <see cref="T:System.ComponentModel.LicenseContext" /> that specifies when you can use the licensed instance.</param>
		/// <param name="args">An array of type <see cref="T:System.Object" /> that represents the arguments for the type.</param>
		/// <returns>An instance of the specified type with the given array of arguments.</returns>
		public static object CreateWithContext(Type type, LicenseContext creationContext, object[] args)
		{
			object result = null;
			object obj = LicenseManager.s_internalSyncObject;
			lock (obj)
			{
				LicenseContext currentContext = LicenseManager.CurrentContext;
				try
				{
					LicenseManager.CurrentContext = creationContext;
					LicenseManager.LockContext(LicenseManager.s_selfLock);
					try
					{
						result = SecurityUtils.SecureCreateInstance(type, args);
					}
					catch (TargetInvocationException ex)
					{
						throw ex.InnerException;
					}
				}
				finally
				{
					LicenseManager.UnlockContext(LicenseManager.s_selfLock);
					LicenseManager.CurrentContext = currentContext;
				}
			}
			return result;
		}

		private static bool GetCachedNoLicenseProvider(Type type)
		{
			return LicenseManager.s_providers != null && LicenseManager.s_providers.ContainsKey(type);
		}

		private static LicenseProvider GetCachedProvider(Type type)
		{
			Hashtable hashtable = LicenseManager.s_providers;
			return (LicenseProvider)((hashtable != null) ? hashtable[type] : null);
		}

		private static LicenseProvider GetCachedProviderInstance(Type providerType)
		{
			Hashtable hashtable = LicenseManager.s_providerInstances;
			return (LicenseProvider)((hashtable != null) ? hashtable[providerType] : null);
		}

		/// <summary>Returns whether the given type has a valid license.</summary>
		/// <param name="type">The <see cref="T:System.Type" /> to find a valid license for.</param>
		/// <returns>
		///   <see langword="true" /> if the given type is licensed; otherwise, <see langword="false" />.</returns>
		public static bool IsLicensed(Type type)
		{
			License license;
			bool result = LicenseManager.ValidateInternal(type, null, false, out license);
			if (license != null)
			{
				license.Dispose();
				license = null;
			}
			return result;
		}

		/// <summary>Determines whether a valid license can be granted for the specified type.</summary>
		/// <param name="type">A <see cref="T:System.Type" /> that represents the type of object that requests the <see cref="T:System.ComponentModel.License" />.</param>
		/// <returns>
		///   <see langword="true" /> if a valid license can be granted; otherwise, <see langword="false" />.</returns>
		public static bool IsValid(Type type)
		{
			License license;
			bool result = LicenseManager.ValidateInternal(type, null, false, out license);
			if (license != null)
			{
				license.Dispose();
				license = null;
			}
			return result;
		}

		/// <summary>Determines whether a valid license can be granted for the specified instance of the type. This method creates a valid <see cref="T:System.ComponentModel.License" />.</summary>
		/// <param name="type">A <see cref="T:System.Type" /> that represents the type of object that requests the license.</param>
		/// <param name="instance">An object of the specified type or a type derived from the specified type.</param>
		/// <param name="license">A <see cref="T:System.ComponentModel.License" /> that is a valid license, or <see langword="null" /> if a valid license cannot be granted.</param>
		/// <returns>
		///   <see langword="true" /> if a valid <see cref="T:System.ComponentModel.License" /> can be granted; otherwise, <see langword="false" />.</returns>
		public static bool IsValid(Type type, object instance, out License license)
		{
			return LicenseManager.ValidateInternal(type, instance, false, out license);
		}

		/// <summary>Prevents changes being made to the current <see cref="T:System.ComponentModel.LicenseContext" /> of the given object.</summary>
		/// <param name="contextUser">The object whose current context you want to lock.</param>
		/// <exception cref="T:System.InvalidOperationException">The context is already locked.</exception>
		public static void LockContext(object contextUser)
		{
			object obj = LicenseManager.s_internalSyncObject;
			lock (obj)
			{
				if (LicenseManager.s_contextLockHolder != null)
				{
					throw new InvalidOperationException("The CurrentContext property of the LicenseManager is already locked by another user.");
				}
				LicenseManager.s_contextLockHolder = contextUser;
			}
		}

		/// <summary>Allows changes to be made to the current <see cref="T:System.ComponentModel.LicenseContext" /> of the given object.</summary>
		/// <param name="contextUser">The object whose current context you want to unlock.</param>
		/// <exception cref="T:System.ArgumentException">
		///   <paramref name="contextUser" /> represents a different user than the one specified in a previous call to <see cref="M:System.ComponentModel.LicenseManager.LockContext(System.Object)" />.</exception>
		public static void UnlockContext(object contextUser)
		{
			object obj = LicenseManager.s_internalSyncObject;
			lock (obj)
			{
				if (LicenseManager.s_contextLockHolder != contextUser)
				{
					throw new ArgumentException("The CurrentContext property of the LicenseManager can only be unlocked with the same contextUser.");
				}
				LicenseManager.s_contextLockHolder = null;
			}
		}

		private static bool ValidateInternal(Type type, object instance, bool allowExceptions, out License license)
		{
			string text;
			return LicenseManager.ValidateInternalRecursive(LicenseManager.CurrentContext, type, instance, allowExceptions, out license, out text);
		}

		private static bool ValidateInternalRecursive(LicenseContext context, Type type, object instance, bool allowExceptions, out License license, out string licenseKey)
		{
			LicenseProvider licenseProvider = LicenseManager.GetCachedProvider(type);
			if (licenseProvider == null && !LicenseManager.GetCachedNoLicenseProvider(type))
			{
				LicenseProviderAttribute licenseProviderAttribute = (LicenseProviderAttribute)Attribute.GetCustomAttribute(type, typeof(LicenseProviderAttribute), false);
				if (licenseProviderAttribute != null)
				{
					Type licenseProvider2 = licenseProviderAttribute.LicenseProvider;
					licenseProvider = (LicenseManager.GetCachedProviderInstance(licenseProvider2) ?? ((LicenseProvider)SecurityUtils.SecureCreateInstance(licenseProvider2)));
				}
				LicenseManager.CacheProvider(type, licenseProvider);
			}
			license = null;
			bool flag = true;
			licenseKey = null;
			if (licenseProvider != null)
			{
				license = licenseProvider.GetLicense(context, type, instance, allowExceptions);
				if (license == null)
				{
					flag = false;
				}
				else
				{
					licenseKey = license.LicenseKey;
				}
			}
			if (flag && instance == null)
			{
				Type baseType = type.BaseType;
				if (baseType != typeof(object) && baseType != null)
				{
					if (license != null)
					{
						license.Dispose();
						license = null;
					}
					string text;
					flag = LicenseManager.ValidateInternalRecursive(context, baseType, null, allowExceptions, out license, out text);
					if (license != null)
					{
						license.Dispose();
						license = null;
					}
				}
			}
			return flag;
		}

		/// <summary>Determines whether a license can be granted for the specified type.</summary>
		/// <param name="type">A <see cref="T:System.Type" /> that represents the type of object that requests the license.</param>
		/// <exception cref="T:System.ComponentModel.LicenseException">A <see cref="T:System.ComponentModel.License" /> cannot be granted.</exception>
		public static void Validate(Type type)
		{
			License license;
			if (!LicenseManager.ValidateInternal(type, null, true, out license))
			{
				throw new LicenseException(type);
			}
			if (license != null)
			{
				license.Dispose();
				license = null;
			}
		}

		/// <summary>Determines whether a license can be granted for the instance of the specified type.</summary>
		/// <param name="type">A <see cref="T:System.Type" /> that represents the type of object that requests the license.</param>
		/// <param name="instance">An <see cref="T:System.Object" /> of the specified type or a type derived from the specified type.</param>
		/// <returns>A valid <see cref="T:System.ComponentModel.License" />.</returns>
		/// <exception cref="T:System.ComponentModel.LicenseException">The type is licensed, but a <see cref="T:System.ComponentModel.License" /> cannot be granted.</exception>
		public static License Validate(Type type, object instance)
		{
			License result;
			if (!LicenseManager.ValidateInternal(type, instance, true, out result))
			{
				throw new LicenseException(type, instance);
			}
			return result;
		}

		private static readonly object s_selfLock = new object();

		private static volatile LicenseContext s_context;

		private static object s_contextLockHolder;

		private static volatile Hashtable s_providers;

		private static volatile Hashtable s_providerInstances;

		private static readonly object s_internalSyncObject = new object();
	}
}
