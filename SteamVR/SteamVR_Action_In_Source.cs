using System;
using System.Runtime.InteropServices;
using UnityEngine;

namespace Valve.VR
{
	public abstract class SteamVR_Action_In_Source : SteamVR_Action_Source, ISteamVR_Action_In_Source, ISteamVR_Action_Source
	{
		public bool isUpdating { get; set; }

		public float updateTime { get; protected set; }

		public abstract ulong activeOrigin { get; }

		public abstract ulong lastActiveOrigin { get; }

		public abstract bool changed { get; protected set; }

		public abstract bool lastChanged { get; protected set; }

		public SteamVR_Input_Sources activeDevice
		{
			get
			{
				this.UpdateOriginTrackedDeviceInfo();
				return SteamVR_Input_Source.GetSource(this.inputOriginInfo.devicePath);
			}
		}

		public uint trackedDeviceIndex
		{
			get
			{
				this.UpdateOriginTrackedDeviceInfo();
				return this.inputOriginInfo.trackedDeviceIndex;
			}
		}

		public string renderModelComponentName
		{
			get
			{
				this.UpdateOriginTrackedDeviceInfo();
				return this.inputOriginInfo.rchRenderModelComponentName;
			}
		}

		public string localizedOriginName
		{
			get
			{
				this.UpdateOriginTrackedDeviceInfo();
				return this.GetLocalizedOrigin();
			}
		}

		public float changedTime { get; protected set; }

		protected int lastOriginGetFrame { get; set; }

		public abstract void UpdateValue();

		public override void Initialize()
		{
			base.Initialize();
			if (SteamVR_Action_In_Source.inputOriginInfo_size == 0U)
			{
				SteamVR_Action_In_Source.inputOriginInfo_size = (uint)Marshal.SizeOf(typeof(InputOriginInfo_t));
			}
		}

		protected void UpdateOriginTrackedDeviceInfo()
		{
			if (this.lastOriginGetFrame != Time.frameCount)
			{
				EVRInputError originTrackedDeviceInfo = OpenVR.Input.GetOriginTrackedDeviceInfo(this.activeOrigin, ref this.inputOriginInfo, SteamVR_Action_In_Source.inputOriginInfo_size);
				if (originTrackedDeviceInfo != EVRInputError.None)
				{
					Debug.LogError(string.Concat(new string[]
					{
						"<b>[SteamVR]</b> GetOriginTrackedDeviceInfo error (",
						base.fullPath,
						"): ",
						originTrackedDeviceInfo.ToString(),
						" handle: ",
						base.handle.ToString(),
						" activeOrigin: ",
						this.activeOrigin.ToString(),
						" active: ",
						this.active.ToString()
					}));
				}
				this.lastInputOriginInfo = this.inputOriginInfo;
				this.lastOriginGetFrame = Time.frameCount;
			}
		}

		public string GetLocalizedOriginPart(params EVRInputStringBits[] localizedParts)
		{
			this.UpdateOriginTrackedDeviceInfo();
			if (this.active)
			{
				return SteamVR_Input.GetLocalizedName(this.activeOrigin, localizedParts);
			}
			return null;
		}

		public string GetLocalizedOrigin()
		{
			this.UpdateOriginTrackedDeviceInfo();
			if (this.active)
			{
				return SteamVR_Input.GetLocalizedName(this.activeOrigin, new EVRInputStringBits[]
				{
					EVRInputStringBits.VRInputString_All
				});
			}
			return null;
		}

		protected static uint inputOriginInfo_size;

		protected InputOriginInfo_t inputOriginInfo;

		protected InputOriginInfo_t lastInputOriginInfo;
	}
}
