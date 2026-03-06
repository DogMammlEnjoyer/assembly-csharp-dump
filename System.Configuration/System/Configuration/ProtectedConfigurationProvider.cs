using System;
using System.Configuration.Provider;
using System.Xml;

namespace System.Configuration
{
	/// <summary>Is the base class to create providers for encrypting and decrypting protected-configuration data.</summary>
	public abstract class ProtectedConfigurationProvider : ProviderBase
	{
		/// <summary>Decrypts the passed <see cref="T:System.Xml.XmlNode" /> object from a configuration file.</summary>
		/// <param name="encryptedNode">The <see cref="T:System.Xml.XmlNode" /> object to decrypt.</param>
		/// <returns>The <see cref="T:System.Xml.XmlNode" /> object containing decrypted data.</returns>
		public abstract XmlNode Decrypt(XmlNode encryptedNode);

		/// <summary>Encrypts the passed <see cref="T:System.Xml.XmlNode" /> object from a configuration file.</summary>
		/// <param name="node">The <see cref="T:System.Xml.XmlNode" /> object to encrypt.</param>
		/// <returns>The <see cref="T:System.Xml.XmlNode" /> object containing encrypted data.</returns>
		public abstract XmlNode Encrypt(XmlNode node);
	}
}
