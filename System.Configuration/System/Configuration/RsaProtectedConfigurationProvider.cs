using System;
using System.Collections.Specialized;
using System.IO;
using System.Security.Cryptography;
using System.Security.Cryptography.Xml;
using System.Xml;
using Unity;

namespace System.Configuration
{
	/// <summary>Provides a <see cref="T:System.Configuration.ProtectedConfigurationProvider" /> instance that uses RSA encryption to encrypt and decrypt configuration data.</summary>
	public sealed class RsaProtectedConfigurationProvider : ProtectedConfigurationProvider
	{
		private RSACryptoServiceProvider GetProvider()
		{
			if (this.rsa == null)
			{
				CspParameters cspParameters = new CspParameters();
				cspParameters.ProviderName = this.cspProviderName;
				cspParameters.KeyContainerName = this.keyContainerName;
				if (this.useMachineContainer)
				{
					cspParameters.Flags |= CspProviderFlags.UseMachineKeyStore;
				}
				this.rsa = new RSACryptoServiceProvider(cspParameters);
			}
			return this.rsa;
		}

		/// <summary>Decrypts the XML node passed to it.</summary>
		/// <param name="encryptedNode">The <see cref="T:System.Xml.XmlNode" /> to decrypt.</param>
		/// <returns>The decrypted XML node.</returns>
		[MonoTODO]
		public override XmlNode Decrypt(XmlNode encryptedNode)
		{
			ConfigurationXmlDocument configurationXmlDocument = new ConfigurationXmlDocument();
			configurationXmlDocument.Load(new StringReader(encryptedNode.OuterXml));
			EncryptedXml encryptedXml = new EncryptedXml(configurationXmlDocument);
			encryptedXml.AddKeyNameMapping("Rsa Key", this.GetProvider());
			encryptedXml.DecryptDocument();
			return configurationXmlDocument.DocumentElement;
		}

		/// <summary>Encrypts the XML node passed to it.</summary>
		/// <param name="node">The <see cref="T:System.Xml.XmlNode" /> to encrypt.</param>
		/// <returns>An encrypted <see cref="T:System.Xml.XmlNode" /> object.</returns>
		[MonoTODO]
		public override XmlNode Encrypt(XmlNode node)
		{
			XmlDocument xmlDocument = new ConfigurationXmlDocument();
			xmlDocument.Load(new StringReader(node.OuterXml));
			EncryptedXml encryptedXml = new EncryptedXml(xmlDocument);
			encryptedXml.AddKeyNameMapping("Rsa Key", this.GetProvider());
			return encryptedXml.Encrypt(xmlDocument.DocumentElement, "Rsa Key").GetXml();
		}

		/// <summary>Initializes the provider with default settings.</summary>
		/// <param name="name">The provider name to use for the object.</param>
		/// <param name="configurationValues">A <see cref="T:System.Collections.Specialized.NameValueCollection" /> collection of values to use when initializing the object.</param>
		/// <exception cref="T:System.Configuration.ConfigurationErrorsException">
		///   <paramref name="configurationValues" /> includes one or more unrecognized values.</exception>
		[MonoTODO]
		public override void Initialize(string name, NameValueCollection configurationValues)
		{
			base.Initialize(name, configurationValues);
			this.keyContainerName = configurationValues["keyContainerName"];
			this.cspProviderName = configurationValues["cspProviderName"];
			string text = configurationValues["useMachineContainer"];
			if (text != null && text.ToLower() == "true")
			{
				this.useMachineContainer = true;
			}
			text = configurationValues["useOAEP"];
			if (text != null && text.ToLower() == "true")
			{
				this.useOAEP = true;
			}
		}

		/// <summary>Adds a key to the RSA key container.</summary>
		/// <param name="keySize">The size of the key to add.</param>
		/// <param name="exportable">
		///   <see langword="true" /> to indicate that the key is exportable; otherwise, <see langword="false" />.</param>
		[MonoTODO]
		public void AddKey(int keySize, bool exportable)
		{
			throw new NotImplementedException();
		}

		/// <summary>Removes a key from the RSA key container.</summary>
		[MonoTODO]
		public void DeleteKey()
		{
			throw new NotImplementedException();
		}

		/// <summary>Exports an RSA key from the key container.</summary>
		/// <param name="xmlFileName">The file name and path to export the key to.</param>
		/// <param name="includePrivateParameters">
		///   <see langword="true" /> to indicate that private parameters are exported; otherwise, <see langword="false" />.</param>
		/// <exception cref="T:System.ArgumentException">
		///   <paramref name="path" /> is a zero-length string, contains only white space, or contains one or more invalid characters as defined by <see cref="F:System.IO.Path.InvalidPathChars" />.</exception>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="path" /> is <see langword="null" />.</exception>
		/// <exception cref="T:System.IO.PathTooLongException">The specified path, file name, or both exceed the system-defined maximum length.</exception>
		/// <exception cref="T:System.IO.DirectoryNotFoundException">The specified path is invalid, such as being on an unmapped drive.</exception>
		/// <exception cref="T:System.IO.IOException">An error occurred while opening the file.</exception>
		/// <exception cref="T:System.UnauthorizedAccessException">
		///   <paramref name="path" /> specified a file that is read-only.  
		/// -or-  
		/// This operation is not supported on the current platform.  
		/// -or-  
		/// <paramref name="path" /> specified a directory.  
		/// -or-  
		/// The caller does not have the required permission.</exception>
		/// <exception cref="T:System.IO.FileNotFoundException">The file specified in <paramref name="path" /> was not found.</exception>
		/// <exception cref="T:System.NotSupportedException">
		///   <paramref name="path" /> is in an invalid format.</exception>
		/// <exception cref="T:System.Security.SecurityException">The caller does not have the required permission.</exception>
		[MonoTODO]
		public void ExportKey(string xmlFileName, bool includePrivateParameters)
		{
			string value = this.GetProvider().ToXmlString(includePrivateParameters);
			StreamWriter streamWriter = new StreamWriter(new FileStream(xmlFileName, FileMode.OpenOrCreate, FileAccess.Write));
			streamWriter.Write(value);
			streamWriter.Close();
		}

		/// <summary>Imports an RSA key into the key container.</summary>
		/// <param name="xmlFileName">The file name and path to import the key from.</param>
		/// <param name="exportable">
		///   <see langword="true" /> to indicate that the key is exportable; otherwise, <see langword="false" />.</param>
		/// <exception cref="T:System.ArgumentException">
		///   <paramref name="path" /> is a zero-length string, contains only white space, or contains one or more invalid characters as defined by <see cref="F:System.IO.Path.InvalidPathChars" />.</exception>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="path" /> is <see langword="null" />.</exception>
		/// <exception cref="T:System.IO.PathTooLongException">The specified path, file name, or both exceed the system-defined maximum length.</exception>
		/// <exception cref="T:System.IO.DirectoryNotFoundException">The specified path is invalid, such as being on an unmapped drive.</exception>
		/// <exception cref="T:System.IO.IOException">An error occurred while opening the file.</exception>
		/// <exception cref="T:System.UnauthorizedAccessException">
		///   <paramref name="path" /> specified a file that is write-only.  
		/// -or-  
		/// This operation is not supported on the current platform.  
		/// -or-  
		/// <paramref name="path" /> specified a directory.  
		/// -or-  
		/// The caller does not have the required permission.</exception>
		/// <exception cref="T:System.IO.FileNotFoundException">The file specified in <paramref name="path" /> was not found.</exception>
		/// <exception cref="T:System.NotSupportedException">
		///   <paramref name="path" /> is in an invalid format.</exception>
		[MonoTODO]
		public void ImportKey(string xmlFileName, bool exportable)
		{
			throw new NotImplementedException();
		}

		/// <summary>Gets the name of the Windows cryptography API (crypto API) cryptographic service provider (CSP).</summary>
		/// <returns>The name of the CryptoAPI cryptographic service provider.</returns>
		public string CspProviderName
		{
			get
			{
				return this.cspProviderName;
			}
		}

		/// <summary>Gets the name of the key container.</summary>
		/// <returns>The name of the key container.</returns>
		public string KeyContainerName
		{
			get
			{
				return this.keyContainerName;
			}
		}

		/// <summary>Gets the public key used by the provider.</summary>
		/// <returns>An <see cref="T:System.Security.Cryptography.RSAParameters" /> object that contains the public key used by the provider.</returns>
		public RSAParameters RsaPublicKey
		{
			get
			{
				return this.GetProvider().ExportParameters(false);
			}
		}

		/// <summary>Gets a value that indicates whether the <see cref="T:System.Configuration.RsaProtectedConfigurationProvider" /> object is using the machine key container.</summary>
		/// <returns>
		///   <see langword="true" /> if the <see cref="T:System.Configuration.RsaProtectedConfigurationProvider" /> object is using the machine key container; otherwise, <see langword="false" />.</returns>
		public bool UseMachineContainer
		{
			get
			{
				return this.useMachineContainer;
			}
		}

		/// <summary>Gets a value that indicates whether the provider is using Optimal Asymmetric Encryption Padding (OAEP) key exchange data.</summary>
		/// <returns>
		///   <see langword="true" /> if the <see cref="T:System.Configuration.RsaProtectedConfigurationProvider" /> object is using Optimal Asymmetric Encryption Padding (OAEP) key exchange data; otherwise, <see langword="false" />.</returns>
		public bool UseOAEP
		{
			get
			{
				return this.useOAEP;
			}
		}

		/// <summary>Gets a value indicating whether the provider uses FIPS.</summary>
		/// <returns>
		///   <see langword="true" /> if the provider uses FIPS; otherwise, <see langword="false" />.</returns>
		public bool UseFIPS
		{
			get
			{
				ThrowStub.ThrowNotSupportedException();
				return default(bool);
			}
		}

		private string cspProviderName;

		private string keyContainerName;

		private bool useMachineContainer;

		private bool useOAEP;

		private RSACryptoServiceProvider rsa;
	}
}
