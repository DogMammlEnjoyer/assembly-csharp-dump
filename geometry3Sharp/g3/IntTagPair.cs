using System;

namespace g3
{
	public struct IntTagPair
	{
		public IntTagPair(byte type, int value)
		{
			this.type = type;
			this.value = value;
		}

		public IntTagPair(int combined)
		{
			this.type = (byte)(combined >> 24);
			this.value = (combined & 16777215);
		}

		public int intValue
		{
			get
			{
				return (int)this.type << 24 | this.value;
			}
		}

		public byte type;

		public int value;
	}
}
