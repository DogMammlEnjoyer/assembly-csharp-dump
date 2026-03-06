using System;

namespace Technie.PhysicsCreator
{
	[Serializable]
	public class Hash160
	{
		public Hash160()
		{
			this.data = new byte[0];
		}

		public Hash160(byte[] data)
		{
			this.data = data;
		}

		public bool IsValid()
		{
			return this.data != null && this.data.Length != 0;
		}

		public override int GetHashCode()
		{
			if (this.data == null)
			{
				return 0;
			}
			int num = 0;
			for (int i = 0; i < this.data.Length; i += 4)
			{
				num |= (int)this.data[i + 1];
				num |= (int)this.data[i + 1] << 8;
				num |= (int)this.data[i + 1] << 16;
				num |= (int)this.data[i + 1] << 24;
			}
			return num;
		}

		public override bool Equals(object obj)
		{
			Hash160 hash = obj as Hash160;
			if (hash == null)
			{
				return false;
			}
			if (this.data == hash.data)
			{
				return true;
			}
			if (this.data == null || hash.data == null)
			{
				return false;
			}
			if (this.data.Length != hash.data.Length)
			{
				return false;
			}
			for (int i = 0; i < this.data.Length; i++)
			{
				if (this.data[i] != hash.data[i])
				{
					return false;
				}
			}
			return true;
		}

		public static bool operator ==(Hash160 lhs, Hash160 rhs)
		{
			if (lhs == null)
			{
				return rhs == null;
			}
			return lhs.Equals(rhs);
		}

		public static bool operator !=(Hash160 lhs, Hash160 rhs)
		{
			return !(lhs == rhs);
		}

		public byte[] data;
	}
}
