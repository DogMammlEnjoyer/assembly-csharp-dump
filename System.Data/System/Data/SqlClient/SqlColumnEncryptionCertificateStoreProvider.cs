using System;
using Unity;

namespace System.Data.SqlClient
{
	/// <summary>The implementation of the key store provider for Windows Certificate Store. This class enables using certificates stored in the Windows Certificate Store as column master keys. For details, see Always Encrypted.</summary>
	public class SqlColumnEncryptionCertificateStoreProvider : SqlColumnEncryptionKeyStoreProvider
	{
		/// <summary>Key store provider for Windows Certificate Store.</summary>
		public SqlColumnEncryptionCertificateStoreProvider()
		{
			ThrowStub.ThrowNotSupportedException();
		}

		/// <summary>Decrypts the specified encrypted value of a column encryption key. The encrypted value is expected to be encrypted using the certificate with the specified key path and using the specified algorithm. The format of the key path should be "Local Machine/My/&lt;certificate_thumbrint&gt;" or "Current User/My/&lt;certificate_thumbprint&gt;".</summary>
		/// <param name="masterKeyPath">The master key path.</param>
		/// <param name="encryptionAlgorithm">The encryption algorithm. Currently, the only valid value is: RSA_OAEP</param>
		/// <param name="encryptedColumnEncryptionKey">The encrypted column encryption key.</param>
		/// <returns>Returns <see cref="T:System.Byte" />.The decrypted column encryption key.</returns>
		public override byte[] DecryptColumnEncryptionKey(string masterKeyPath, string encryptionAlgorithm, byte[] encryptedColumnEncryptionKey)
		{
			ThrowStub.ThrowNotSupportedException();
			return null;
		}

		/// <summary>Encrypts a column encryption key using the certificate with the specified key path and using the specified algorithm. The format of the key path should be "Local Machine/My/&lt;certificate_thumbrint&gt;" or "Current User/My/&lt;certificate_thumbprint&gt;".</summary>
		/// <param name="masterKeyPath">The master key path.</param>
		/// <param name="encryptionAlgorithm">The encryption algorithm. Currently, the only valid value is: RSA_OAEP</param>
		/// <param name="columnEncryptionKey">The encrypted column encryption key.</param>
		/// <returns>Returns <see cref="T:System.Byte" />.The encrypted column encryption key.</returns>
		public override byte[] EncryptColumnEncryptionKey(string masterKeyPath, string encryptionAlgorithm, byte[] columnEncryptionKey)
		{
			ThrowStub.ThrowNotSupportedException();
			return null;
		}

		/// <summary>Digitally signs the column master key metadata with the column master key referenced by the <paramref name="masterKeyPath" /> parameter.</summary>
		/// <param name="masterKeyPath">The column master key path.</param>
		/// <param name="allowEnclaveComputations">
		///   <see langword="true" /> to indicate that the column master key supports enclave computations; otherwise, <see langword="false" />.</param>
		/// <returns>The signature of the column master key metadata.</returns>
		public override byte[] SignColumnMasterKeyMetadata(string masterKeyPath, bool allowEnclaveComputations)
		{
			ThrowStub.ThrowNotSupportedException();
			return null;
		}

		/// <summary>This function must be implemented by the corresponding Key Store providers. This function should use an asymmetric key identified by a key path and verify the masterkey metadata consisting of (masterKeyPath, allowEnclaveComputations, providerName).</summary>
		/// <param name="masterKeyPath">The complete path of an asymmetric key. The path format is specific to a key store provider.</param>
		/// <param name="allowEnclaveComputations">A Boolean that indicates if this key can be sent to the trusted enclave.</param>
		/// <param name="signature">The master key metadata siognature.</param>
		/// <returns>A Boolean value that indicates if the master key metadata can be verified based on the provided signature.</returns>
		public override bool VerifyColumnMasterKeyMetadata(string masterKeyPath, bool allowEnclaveComputations, byte[] signature)
		{
			ThrowStub.ThrowNotSupportedException();
			return default(bool);
		}

		/// <summary>The provider name.</summary>
		public const string ProviderName = "MSSQL_CERTIFICATE_STORE";
	}
}
