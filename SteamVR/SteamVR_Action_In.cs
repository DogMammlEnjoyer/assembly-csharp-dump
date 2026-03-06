using System;

namespace Valve.VR
{
	[Serializable]
	public abstract class SteamVR_Action_In<SourceMap, SourceElement> : SteamVR_Action<SourceMap, SourceElement>, ISteamVR_Action_In, ISteamVR_Action, ISteamVR_Action_Source, ISteamVR_Action_In_Source where SourceMap : SteamVR_Action_In_Source_Map<SourceElement>, new() where SourceElement : SteamVR_Action_In_Source, new()
	{
		public bool changed
		{
			get
			{
				return this.sourceMap[SteamVR_Input_Sources.Any].changed;
			}
		}

		public bool lastChanged
		{
			get
			{
				return this.sourceMap[SteamVR_Input_Sources.Any].changed;
			}
		}

		public float changedTime
		{
			get
			{
				return this.sourceMap[SteamVR_Input_Sources.Any].changedTime;
			}
		}

		public float updateTime
		{
			get
			{
				return this.sourceMap[SteamVR_Input_Sources.Any].updateTime;
			}
		}

		public ulong activeOrigin
		{
			get
			{
				return this.sourceMap[SteamVR_Input_Sources.Any].activeOrigin;
			}
		}

		public ulong lastActiveOrigin
		{
			get
			{
				return this.sourceMap[SteamVR_Input_Sources.Any].lastActiveOrigin;
			}
		}

		public SteamVR_Input_Sources activeDevice
		{
			get
			{
				return this.sourceMap[SteamVR_Input_Sources.Any].activeDevice;
			}
		}

		public uint trackedDeviceIndex
		{
			get
			{
				return this.sourceMap[SteamVR_Input_Sources.Any].trackedDeviceIndex;
			}
		}

		public string renderModelComponentName
		{
			get
			{
				return this.sourceMap[SteamVR_Input_Sources.Any].renderModelComponentName;
			}
		}

		public string localizedOriginName
		{
			get
			{
				return this.sourceMap[SteamVR_Input_Sources.Any].localizedOriginName;
			}
		}

		public virtual void UpdateValues()
		{
			this.sourceMap.UpdateValues();
		}

		public virtual string GetRenderModelComponentName(SteamVR_Input_Sources inputSource)
		{
			return this.sourceMap[inputSource].renderModelComponentName;
		}

		public virtual SteamVR_Input_Sources GetActiveDevice(SteamVR_Input_Sources inputSource)
		{
			return this.sourceMap[inputSource].activeDevice;
		}

		public virtual uint GetDeviceIndex(SteamVR_Input_Sources inputSource)
		{
			return this.sourceMap[inputSource].trackedDeviceIndex;
		}

		public virtual bool GetChanged(SteamVR_Input_Sources inputSource)
		{
			return this.sourceMap[inputSource].changed;
		}

		public override float GetTimeLastChanged(SteamVR_Input_Sources inputSource)
		{
			return this.sourceMap[inputSource].changedTime;
		}

		public string GetLocalizedOriginPart(SteamVR_Input_Sources inputSource, params EVRInputStringBits[] localizedParts)
		{
			return this.sourceMap[inputSource].GetLocalizedOriginPart(localizedParts);
		}

		public string GetLocalizedOrigin(SteamVR_Input_Sources inputSource)
		{
			return this.sourceMap[inputSource].GetLocalizedOrigin();
		}

		public override bool IsUpdating(SteamVR_Input_Sources inputSource)
		{
			return this.sourceMap.IsUpdating(inputSource);
		}

		public void ForceAddSourceToUpdateList(SteamVR_Input_Sources inputSource)
		{
			this.sourceMap.ForceAddSourceToUpdateList(inputSource);
		}

		public string GetControllerType(SteamVR_Input_Sources inputSource)
		{
			return SteamVR.instance.GetStringProperty(ETrackedDeviceProperty.Prop_ControllerType_String, this.GetDeviceIndex(inputSource));
		}
	}
}
