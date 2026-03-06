using System;
using System.Text;

namespace OVR.OpenVR
{
	public struct VREvent_Keyboard_t
	{
		public string cNewInput
		{
			get
			{
				StringBuilder stringBuilder = new StringBuilder(8);
				stringBuilder.Append(this.cNewInput0);
				stringBuilder.Append(this.cNewInput1);
				stringBuilder.Append(this.cNewInput2);
				stringBuilder.Append(this.cNewInput3);
				stringBuilder.Append(this.cNewInput4);
				stringBuilder.Append(this.cNewInput5);
				stringBuilder.Append(this.cNewInput6);
				stringBuilder.Append(this.cNewInput7);
				return stringBuilder.ToString();
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
