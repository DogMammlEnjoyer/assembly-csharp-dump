using System;
using System.Collections;
using System.Reflection;

namespace System.ComponentModel.Design
{
	/// <summary>Represents a design-time license context that can support a license provider at design time.</summary>
	public class DesigntimeLicenseContext : LicenseContext
	{
		/// <summary>Gets the license usage mode.</summary>
		/// <returns>A <see cref="T:System.ComponentModel.LicenseUsageMode" /> indicating the licensing mode for the context.</returns>
		public override LicenseUsageMode UsageMode
		{
			get
			{
				return LicenseUsageMode.Designtime;
			}
		}

		/// <summary>Gets a saved license key.</summary>
		/// <param name="type">The type of the license key.</param>
		/// <param name="resourceAssembly">The assembly to get the key from.</param>
		/// <returns>The saved license key that matches the specified type.</returns>
		public override string GetSavedLicenseKey(Type type, Assembly resourceAssembly)
		{
			return null;
		}

		/// <summary>Sets a saved license key.</summary>
		/// <param name="type">The type of the license key.</param>
		/// <param name="key">The license key.</param>
		public override void SetSavedLicenseKey(Type type, string key)
		{
			this.savedLicenseKeys[type.AssemblyQualifiedName] = key;
		}

		internal Hashtable savedLicenseKeys = new Hashtable();
	}
}
