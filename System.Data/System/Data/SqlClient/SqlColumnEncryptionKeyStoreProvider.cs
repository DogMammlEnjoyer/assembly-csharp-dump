using System;
using Unity;

namespace System.Data.SqlClient
{
	/// <summary>Base class for all key store providers. A custom provider must derive from this class and override its member functions and then register it using SqlConnection.RegisterColumnEncryptionKeyStoreProviders(). For details see, Always Encrypted.</summary>
	public abstract class SqlColumnEncryptionKeyStoreProvider
	{
		/// <summary>Initializes a new instance of the SqlColumnEncryptionKeyStoreProviderClass.</summary>
		protected SqlColumnEncryptionKeyStoreProvider()
		{
			ThrowStub.ThrowNotSupportedException();
		}

		/// <summary>Decrypts the specified encrypted value of a column encryption key. The encrypted value is expected to be encrypted using the column master key with the specified key path and using the specified algorithm.</summary>
		/// <param name="masterKeyPath">The master key path.</param>
		/// <param name="encryptionAlgorithm">The encryption algorithm.</param>
		/// <param name="encryptedColumnEncryptionKey">The encrypted column encryption key.</param>
		/// <returns>Returns <see cref="T:System.Byte" />.The decrypted column encryption key.</returns>
		public abstract byte[] DecryptColumnEncryptionKey(string masterKeyPath, string encryptionAlgorithm, byte[] encryptedColumnEncryptionKey);

		/// <summary>Encrypts a column encryption key using the column master key with the specified key path and using the specified algorithm.</summary>
		/// <param name="masterKeyPath">The master key path.</param>
		/// <param name="encryptionAlgorithm">The encryption algorithm.</param>
		/// <param name="columnEncryptionKey">The encrypted column encryption key.</param>
		/// <returns>Returns <see cref="T:System.Byte" />.The encrypted column encryption key.</returns>
		public abstract byte[] EncryptColumnEncryptionKey(string masterKeyPath, string encryptionAlgorithm, byte[] columnEncryptionKey);

		/// <summary>When implemented in a derived class, digitally signs the column master key metadata with the column master key referenced by the <paramref name="masterKeyPath" /> parameter. The input values used to generate the signature should be the specified values of the <paramref name="masterKeyPath" /> and <paramref name="allowEnclaveComputations" /> parameters.</summary>
		/// <param name="masterKeyPath">The column master key path.</param>
		/// <param name="allowEnclaveComputations">
		///   <see langword="true" /> to indicate that the column master key supports enclave computations; otherwise, <see langword="false" />.</param>
		/// <returns>The signature of the column master key metadata.</returns>
		/// <exception cref="T:System.NotImplementedException">In all cases.</exception>
		public virtual byte[] SignColumnMasterKeyMetadata(string masterKeyPath, bool allowEnclaveComputations)
		{
			ThrowStub.ThrowNotSupportedException();
			return null;
		}

		/// <summary>When implemented in a derived class, this method is expected to verify the specified signature is valid for the column master key with the specified key path and the specified enclave behavior. The default implementation throws NotImplementedException.</summary>
		/// <param name="masterKeyPath">The column master key path.</param>
		/// <param name="allowEnclaveComputations">Indicates whether the column master key supports enclave computations.</param>
		/// <param name="signature">The signature of the column master key metadata.</param>
		/// <returns>When implemented in a derived class, the method is expected to return true if the specified signature is valid, or false if the specified signature is not valid. The default implementation throws NotImplementedException.</returns>
		public virtual bool VerifyColumnMasterKeyMetadata(string masterKeyPath, bool allowEnclaveComputations, byte[] signature)
		{
			ThrowStub.ThrowNotSupportedException();
			return default(bool);
		}
	}
}
