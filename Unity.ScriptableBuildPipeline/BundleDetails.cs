using System;

namespace UnityEngine.Build.Pipeline
{
	[Serializable]
	public struct BundleDetails : IEquatable<BundleDetails>
	{
		public string FileName
		{
			get
			{
				return this.m_FileName;
			}
			set
			{
				this.m_FileName = value;
			}
		}

		public uint Crc
		{
			get
			{
				return this.m_Crc;
			}
			set
			{
				this.m_Crc = value;
			}
		}

		public Hash128 Hash
		{
			get
			{
				return Hash128.Parse(this.m_Hash);
			}
			set
			{
				this.m_Hash = value.ToString();
			}
		}

		public string[] Dependencies
		{
			get
			{
				return this.m_Dependencies;
			}
			set
			{
				this.m_Dependencies = value;
			}
		}

		public override bool Equals(object obj)
		{
			return obj != null && obj is BundleDetails && this.Equals((BundleDetails)obj);
		}

		public override int GetHashCode()
		{
			return ((((this.FileName != null) ? this.FileName.GetHashCode() : 0) * 397 ^ (int)this.Crc) * 397 ^ this.Hash.GetHashCode()) * 397 ^ ((this.Dependencies != null) ? this.Dependencies.GetHashCode() : 0);
		}

		public static bool operator ==(BundleDetails a, BundleDetails b)
		{
			return a.Equals(b);
		}

		public static bool operator !=(BundleDetails a, BundleDetails b)
		{
			return !(a == b);
		}

		public bool Equals(BundleDetails other)
		{
			return string.Equals(this.FileName, other.FileName) && this.Crc == other.Crc && this.Hash.Equals(other.Hash) && object.Equals(this.Dependencies, other.Dependencies);
		}

		[SerializeField]
		private string m_FileName;

		[SerializeField]
		private uint m_Crc;

		[SerializeField]
		private string m_Hash;

		[SerializeField]
		private string[] m_Dependencies;
	}
}
