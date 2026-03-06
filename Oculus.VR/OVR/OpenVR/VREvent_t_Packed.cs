using System;
using System.Runtime.InteropServices;

namespace OVR.OpenVR
{
	[StructLayout(LayoutKind.Sequential, Pack = 4)]
	public struct VREvent_t_Packed
	{
		public VREvent_t_Packed(VREvent_t unpacked)
		{
			this.eventType = unpacked.eventType;
			this.trackedDeviceIndex = unpacked.trackedDeviceIndex;
			this.eventAgeSeconds = unpacked.eventAgeSeconds;
			this.data = unpacked.data;
		}

		public void Unpack(ref VREvent_t unpacked)
		{
			unpacked.eventType = this.eventType;
			unpacked.trackedDeviceIndex = this.trackedDeviceIndex;
			unpacked.eventAgeSeconds = this.eventAgeSeconds;
			unpacked.data = this.data;
		}

		public uint eventType;

		public uint trackedDeviceIndex;

		public float eventAgeSeconds;

		public VREvent_Data_t data;
	}
}
