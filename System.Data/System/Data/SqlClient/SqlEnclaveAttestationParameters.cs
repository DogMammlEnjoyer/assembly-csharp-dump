using System;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using Unity;

namespace System.Data.SqlClient
{
	/// <summary>Encapsulates the information SqlClient sends to SQL Server to initiate the process of attesting and creating a secure session with the enclave, SQL Server uses for computations on columns protected using Always Encrypted.</summary>
	public class SqlEnclaveAttestationParameters
	{
		/// <summary>Initializes a new instance of the <see cref="T:System.Data.SqlClient.SqlEnclaveAttestationParameters" /> class.</summary>
		/// <param name="protocol">The enclave attestation protocol.</param>
		/// <param name="input">The input of the enclave attestation protocol.</param>
		/// <param name="clientDiffieHellmanKey">A Diffie-Hellman algorithm that encapsulates a client-side key pair.</param>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="clientDiffieHellmanKey" /> is <see langword="null" />.</exception>
		public SqlEnclaveAttestationParameters(int protocol, byte[] input, ECDiffieHellmanCng clientDiffieHellmanKey)
		{
			ThrowStub.ThrowNotSupportedException();
		}

		/// <summary>Gets a Diffie-Hellman algorithm that encapsulates a key pair that SqlClient uses to establish a secure session with the enclave.</summary>
		/// <returns>The Diffie-Hellman algorithm.</returns>
		public ECDiffieHellmanCng ClientDiffieHellmanKey
		{
			[CompilerGenerated]
			get
			{
				ThrowStub.ThrowNotSupportedException();
				return null;
			}
		}

		/// <summary>Gets the enclave attestation protocol identifier.</summary>
		/// <returns>The enclave attestation protocol identifier.</returns>
		public int Protocol
		{
			[CompilerGenerated]
			get
			{
				ThrowStub.ThrowNotSupportedException();
				return 0;
			}
		}

		/// <summary>Gets the information used to initiate the process of attesting the enclave. The format and the content of this information is specific to the attestation protocol.</summary>
		/// <returns>The information required by SQL Server to execute attestation protocol identified by EnclaveAttestationProtocols.</returns>
		public byte[] GetInput()
		{
			ThrowStub.ThrowNotSupportedException();
			return null;
		}
	}
}
