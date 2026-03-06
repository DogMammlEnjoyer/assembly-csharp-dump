using System;
using System.ComponentModel.Design;
using System.Globalization;
using System.IO;

namespace System.ComponentModel
{
	/// <summary>Provides an implementation of a <see cref="T:System.ComponentModel.LicenseProvider" />. The provider works in a similar fashion to the Microsoft .NET Framework standard licensing model.</summary>
	public class LicFileLicenseProvider : LicenseProvider
	{
		/// <summary>Determines whether the key that the <see cref="M:System.ComponentModel.LicFileLicenseProvider.GetLicense(System.ComponentModel.LicenseContext,System.Type,System.Object,System.Boolean)" /> method retrieves is valid for the specified type.</summary>
		/// <param name="key">The <see cref="P:System.ComponentModel.License.LicenseKey" /> to check.</param>
		/// <param name="type">A <see cref="T:System.Type" /> that represents the component requesting the <see cref="T:System.ComponentModel.License" />.</param>
		/// <returns>
		///   <see langword="true" /> if the key is a valid <see cref="P:System.ComponentModel.License.LicenseKey" /> for the specified type; otherwise, <see langword="false" />.</returns>
		protected virtual bool IsKeyValid(string key, Type type)
		{
			return key != null && key.StartsWith(this.GetKey(type));
		}

		/// <summary>Returns a key for the specified type.</summary>
		/// <param name="type">The object type to return the key.</param>
		/// <returns>A confirmation that the <paramref name="type" /> parameter is licensed.</returns>
		protected virtual string GetKey(Type type)
		{
			return string.Format(CultureInfo.InvariantCulture, "{0} is a licensed component.", type.FullName);
		}

		/// <summary>Returns a license for the instance of the component, if one is available.</summary>
		/// <param name="context">A <see cref="T:System.ComponentModel.LicenseContext" /> that specifies where you can use the licensed object.</param>
		/// <param name="type">A <see cref="T:System.Type" /> that represents the component requesting the <see cref="T:System.ComponentModel.License" />.</param>
		/// <param name="instance">An object that requests the <see cref="T:System.ComponentModel.License" />.</param>
		/// <param name="allowExceptions">
		///   <see langword="true" /> if a <see cref="T:System.ComponentModel.LicenseException" /> should be thrown when a component cannot be granted a license; otherwise, <see langword="false" />.</param>
		/// <returns>A valid <see cref="T:System.ComponentModel.License" />. If this method cannot find a valid <see cref="T:System.ComponentModel.License" /> or a valid <paramref name="context" /> parameter, it returns <see langword="null" />.</returns>
		public override License GetLicense(LicenseContext context, Type type, object instance, bool allowExceptions)
		{
			LicFileLicenseProvider.LicFileLicense licFileLicense = null;
			if (context != null)
			{
				if (context.UsageMode == LicenseUsageMode.Runtime)
				{
					string savedLicenseKey = context.GetSavedLicenseKey(type, null);
					if (savedLicenseKey != null && this.IsKeyValid(savedLicenseKey, type))
					{
						licFileLicense = new LicFileLicenseProvider.LicFileLicense(this, savedLicenseKey);
					}
				}
				if (licFileLicense == null)
				{
					string text = null;
					if (context != null)
					{
						ITypeResolutionService typeResolutionService = (ITypeResolutionService)context.GetService(typeof(ITypeResolutionService));
						if (typeResolutionService != null)
						{
							text = typeResolutionService.GetPathOfAssembly(type.Assembly.GetName());
						}
					}
					if (text == null)
					{
						text = type.Module.FullyQualifiedName;
					}
					string path = Path.GetDirectoryName(text) + "\\" + type.FullName + ".lic";
					if (File.Exists(path))
					{
						StreamReader streamReader = new StreamReader(new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read));
						string key = streamReader.ReadLine();
						streamReader.Close();
						if (this.IsKeyValid(key, type))
						{
							licFileLicense = new LicFileLicenseProvider.LicFileLicense(this, this.GetKey(type));
						}
					}
					if (licFileLicense != null)
					{
						context.SetSavedLicenseKey(type, licFileLicense.LicenseKey);
					}
				}
			}
			return licFileLicense;
		}

		private class LicFileLicense : License
		{
			public LicFileLicense(LicFileLicenseProvider owner, string key)
			{
				this._owner = owner;
				this.LicenseKey = key;
			}

			public override string LicenseKey { get; }

			public override void Dispose()
			{
				GC.SuppressFinalize(this);
			}

			private LicFileLicenseProvider _owner;
		}
	}
}
