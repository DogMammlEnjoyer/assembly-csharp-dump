using System;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using System.Security.Cryptography;
using System.Security.Permissions;
using System.Text;
using Unity;

namespace System.Security.Policy
{
	/// <summary>Provides evidence about the hash value for an assembly. This class cannot be inherited.</summary>
	[ComVisible(true)]
	[Serializable]
	public sealed class Hash : EvidenceBase, ISerializable, IBuiltInEvidence
	{
		/// <summary>Initializes a new instance of the <see cref="T:System.Security.Policy.Hash" /> class.</summary>
		/// <param name="assembly">The assembly for which to compute the hash value.</param>
		/// <exception cref="T:System.ArgumentNullException">The <paramref name="assembly" /> parameter is <see langword="null" />.</exception>
		/// <exception cref="T:System.ArgumentException">
		///   <paramref name="assembly" /> is not a run-time <see cref="T:System.Reflection.Assembly" /> object.</exception>
		public Hash(Assembly assembly)
		{
			if (assembly == null)
			{
				throw new ArgumentNullException("assembly");
			}
			this.assembly = assembly;
		}

		internal Hash()
		{
		}

		internal Hash(SerializationInfo info, StreamingContext context)
		{
			this.data = (byte[])info.GetValue("RawData", typeof(byte[]));
		}

		/// <summary>Gets the <see cref="T:System.Security.Cryptography.MD5" /> hash value for the assembly.</summary>
		/// <returns>A byte array that represents the <see cref="T:System.Security.Cryptography.MD5" /> hash value for the assembly.</returns>
		public byte[] MD5
		{
			get
			{
				if (this._md5 != null)
				{
					return this._md5;
				}
				if (this.assembly == null && this._sha1 != null)
				{
					throw new SecurityException(Locale.GetText("No assembly data. This instance was initialized with an MSHA1 digest value."));
				}
				HashAlgorithm hashAlg = System.Security.Cryptography.MD5.Create();
				this._md5 = this.GenerateHash(hashAlg);
				return this._md5;
			}
		}

		/// <summary>Gets the <see cref="T:System.Security.Cryptography.SHA1" /> hash value for the assembly.</summary>
		/// <returns>A byte array that represents the <see cref="T:System.Security.Cryptography.SHA1" /> hash value for the assembly.</returns>
		public byte[] SHA1
		{
			get
			{
				if (this._sha1 != null)
				{
					return this._sha1;
				}
				if (this.assembly == null && this._md5 != null)
				{
					throw new SecurityException(Locale.GetText("No assembly data. This instance was initialized with an MD5 digest value."));
				}
				HashAlgorithm hashAlg = System.Security.Cryptography.SHA1.Create();
				this._sha1 = this.GenerateHash(hashAlg);
				return this._sha1;
			}
		}

		/// <summary>Computes the hash value for the assembly using the specified hash algorithm.</summary>
		/// <param name="hashAlg">The hash algorithm to use to compute the hash value for the assembly.</param>
		/// <returns>A byte array that represents the hash value for the assembly.</returns>
		/// <exception cref="T:System.ArgumentNullException">The <paramref name="hashAlg" /> parameter is <see langword="null" />.</exception>
		/// <exception cref="T:System.Security.SecurityException">The hash value for the assembly cannot be generated.</exception>
		public byte[] GenerateHash(HashAlgorithm hashAlg)
		{
			if (hashAlg == null)
			{
				throw new ArgumentNullException("hashAlg");
			}
			return hashAlg.ComputeHash(this.GetData());
		}

		/// <summary>Gets the <see cref="T:System.Runtime.Serialization.SerializationInfo" /> object with the parameter name and additional exception information.</summary>
		/// <param name="info">The object that holds the serialized object data.</param>
		/// <param name="context">The contextual information about the source or destination.</param>
		[SecurityCritical]
		public void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			if (info == null)
			{
				throw new ArgumentNullException("info");
			}
			info.AddValue("RawData", this.GetData());
		}

		/// <summary>Returns a string representation of the current <see cref="T:System.Security.Policy.Hash" />.</summary>
		/// <returns>A representation of the current <see cref="T:System.Security.Policy.Hash" />.</returns>
		public override string ToString()
		{
			SecurityElement securityElement = new SecurityElement(base.GetType().FullName);
			securityElement.AddAttribute("version", "1");
			StringBuilder stringBuilder = new StringBuilder();
			byte[] array = this.GetData();
			for (int i = 0; i < array.Length; i++)
			{
				stringBuilder.Append(array[i].ToString("X2"));
			}
			securityElement.AddChild(new SecurityElement("RawData", stringBuilder.ToString()));
			return securityElement.ToString();
		}

		[FileIOPermission(SecurityAction.Assert, Unrestricted = true)]
		private byte[] GetData()
		{
			if (this.assembly == null && this.data == null)
			{
				throw new SecurityException(Locale.GetText("No assembly data."));
			}
			if (this.data == null)
			{
				FileStream fileStream = new FileStream(this.assembly.Location, FileMode.Open, FileAccess.Read);
				this.data = new byte[fileStream.Length];
				fileStream.Read(this.data, 0, (int)fileStream.Length);
			}
			return this.data;
		}

		int IBuiltInEvidence.GetRequiredSize(bool verbose)
		{
			if (!verbose)
			{
				return 0;
			}
			return 5;
		}

		[MonoTODO("IBuiltInEvidence")]
		int IBuiltInEvidence.InitFromBuffer(char[] buffer, int position)
		{
			return 0;
		}

		[MonoTODO("IBuiltInEvidence")]
		int IBuiltInEvidence.OutputToBuffer(char[] buffer, int position, bool verbose)
		{
			return 0;
		}

		/// <summary>Creates a <see cref="T:System.Security.Policy.Hash" /> object that contains an <see cref="T:System.Security.Cryptography.MD5" /> hash value.</summary>
		/// <param name="md5">A byte array that contains an <see cref="T:System.Security.Cryptography.MD5" /> hash value.</param>
		/// <returns>An object that contains the hash value provided by the <paramref name="md5" /> parameter.</returns>
		/// <exception cref="T:System.ArgumentNullException">The <paramref name="md5" /> parameter is <see langword="null" />.</exception>
		public static Hash CreateMD5(byte[] md5)
		{
			if (md5 == null)
			{
				throw new ArgumentNullException("md5");
			}
			return new Hash
			{
				_md5 = md5
			};
		}

		/// <summary>Creates a <see cref="T:System.Security.Policy.Hash" /> object that contains a <see cref="T:System.Security.Cryptography.SHA1" /> hash value.</summary>
		/// <param name="sha1">A byte array that contains a <see cref="T:System.Security.Cryptography.SHA1" /> hash value.</param>
		/// <returns>An object that contains the hash value provided by the <paramref name="sha1" /> parameter.</returns>
		/// <exception cref="T:System.ArgumentNullException">The <paramref name="sha1" /> parameter is <see langword="null" />.</exception>
		public static Hash CreateSHA1(byte[] sha1)
		{
			if (sha1 == null)
			{
				throw new ArgumentNullException("sha1");
			}
			return new Hash
			{
				_sha1 = sha1
			};
		}

		/// <summary>Gets the <see cref="T:System.Security.Cryptography.SHA256" /> hash value for the assembly.</summary>
		/// <returns>A byte array that represents the <see cref="T:System.Security.Cryptography.SHA256" /> hash value for the assembly.</returns>
		public byte[] SHA256
		{
			get
			{
				ThrowStub.ThrowNotSupportedException();
				return null;
			}
		}

		/// <summary>Creates a <see cref="T:System.Security.Policy.Hash" /> object that contains a <see cref="T:System.Security.Cryptography.SHA256" /> hash value.</summary>
		/// <param name="sha256">A byte array that contains a <see cref="T:System.Security.Cryptography.SHA256" /> hash value.</param>
		/// <returns>A hash object that contains the hash value provided by the <paramref name="sha256" /> parameter.</returns>
		/// <exception cref="T:System.ArgumentNullException">The <paramref name="sha256" /> parameter is <see langword="null" />.</exception>
		public static Hash CreateSHA256(byte[] sha256)
		{
			ThrowStub.ThrowNotSupportedException();
			return null;
		}

		private Assembly assembly;

		private byte[] data;

		internal byte[] _md5;

		internal byte[] _sha1;
	}
}
