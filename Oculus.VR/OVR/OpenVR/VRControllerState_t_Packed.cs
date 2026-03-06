using System;
using System.Runtime.InteropServices;

namespace OVR.OpenVR
{
	[StructLayout(LayoutKind.Sequential, Pack = 4)]
	public struct VRControllerState_t_Packed
	{
		public VRControllerState_t_Packed(VRControllerState_t unpacked)
		{
			this.unPacketNum = unpacked.unPacketNum;
			this.ulButtonPressed = unpacked.ulButtonPressed;
			this.ulButtonTouched = unpacked.ulButtonTouched;
			this.rAxis0 = unpacked.rAxis0;
			this.rAxis1 = unpacked.rAxis1;
			this.rAxis2 = unpacked.rAxis2;
			this.rAxis3 = unpacked.rAxis3;
			this.rAxis4 = unpacked.rAxis4;
		}

		public void Unpack(ref VRControllerState_t unpacked)
		{
			unpacked.unPacketNum = this.unPacketNum;
			unpacked.ulButtonPressed = this.ulButtonPressed;
			unpacked.ulButtonTouched = this.ulButtonTouched;
			unpacked.rAxis0 = this.rAxis0;
			unpacked.rAxis1 = this.rAxis1;
			unpacked.rAxis2 = this.rAxis2;
			unpacked.rAxis3 = this.rAxis3;
			unpacked.rAxis4 = this.rAxis4;
		}

		public uint unPacketNum;

		public ulong ulButtonPressed;

		public ulong ulButtonTouched;

		public VRControllerAxis_t rAxis0;

		public VRControllerAxis_t rAxis1;

		public VRControllerAxis_t rAxis2;

		public VRControllerAxis_t rAxis3;

		public VRControllerAxis_t rAxis4;
	}
}
