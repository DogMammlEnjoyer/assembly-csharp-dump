using System;

namespace Valve.VR
{
	public struct VREvent_Keyboard_t
	{
		public string cNewInput
		{
			get
			{
				return new string(new char[]
				{
					(char)this.cNewInput0,
					(char)this.cNewInput1,
					(char)this.cNewInput2,
					(char)this.cNewInput3,
					(char)this.cNewInput4,
					(char)this.cNewInput5,
					(char)this.cNewInput6,
					(char)this.cNewInput7
				}).TrimEnd('\0');
			}
		}

		public byte cNewInput0;

		public byte cNewInput1;

		public byte cNewInput2;

		public byte cNewInput3;

		public byte cNewInput4;

		public byte cNewInput5;

		public byte cNewInput6;

		public byte cNewInput7;

		public ulong uUserValue;
	}
}
