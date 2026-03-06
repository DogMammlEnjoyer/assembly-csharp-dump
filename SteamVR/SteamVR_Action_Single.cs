using System;
using UnityEngine;

namespace Valve.VR
{
	[Serializable]
	public class SteamVR_Action_Single : SteamVR_Action_In<SteamVR_Action_Single_Source_Map, SteamVR_Action_Single_Source>, ISteamVR_Action_Single, ISteamVR_Action_In_Source, ISteamVR_Action_Source, ISerializationCallbackReceiver
	{
		public event SteamVR_Action_Single.ChangeHandler onChange
		{
			add
			{
				this.sourceMap[SteamVR_Input_Sources.Any].onChange += value;
			}
			remove
			{
				this.sourceMap[SteamVR_Input_Sources.Any].onChange -= value;
			}
		}

		public event SteamVR_Action_Single.UpdateHandler onUpdate
		{
			add
			{
				this.sourceMap[SteamVR_Input_Sources.Any].onUpdate += value;
			}
			remove
			{
				this.sourceMap[SteamVR_Input_Sources.Any].onUpdate -= value;
			}
		}

		public event SteamVR_Action_Single.AxisHandler onAxis
		{
			add
			{
				this.sourceMap[SteamVR_Input_Sources.Any].onAxis += value;
			}
			remove
			{
				this.sourceMap[SteamVR_Input_Sources.Any].onAxis -= value;
			}
		}

		public event SteamVR_Action_Single.ActiveChangeHandler onActiveChange
		{
			add
			{
				this.sourceMap[SteamVR_Input_Sources.Any].onActiveChange += value;
			}
			remove
			{
				this.sourceMap[SteamVR_Input_Sources.Any].onActiveChange -= value;
			}
		}

		public event SteamVR_Action_Single.ActiveChangeHandler onActiveBindingChange
		{
			add
			{
				this.sourceMap[SteamVR_Input_Sources.Any].onActiveBindingChange += value;
			}
			remove
			{
				this.sourceMap[SteamVR_Input_Sources.Any].onActiveBindingChange -= value;
			}
		}

		public float axis
		{
			get
			{
				return this.sourceMap[SteamVR_Input_Sources.Any].axis;
			}
		}

		public float lastAxis
		{
			get
			{
				return this.sourceMap[SteamVR_Input_Sources.Any].lastAxis;
			}
		}

		public float delta
		{
			get
			{
				return this.sourceMap[SteamVR_Input_Sources.Any].delta;
			}
		}

		public float lastDelta
		{
			get
			{
				return this.sourceMap[SteamVR_Input_Sources.Any].lastDelta;
			}
		}

		public float GetAxis(SteamVR_Input_Sources inputSource)
		{
			return this.sourceMap[inputSource].axis;
		}

		public float GetAxisDelta(SteamVR_Input_Sources inputSource)
		{
			return this.sourceMap[inputSource].delta;
		}

		public float GetLastAxis(SteamVR_Input_Sources inputSource)
		{
			return this.sourceMap[inputSource].lastAxis;
		}

		public float GetLastAxisDelta(SteamVR_Input_Sources inputSource)
		{
			return this.sourceMap[inputSource].lastDelta;
		}

		public void AddOnActiveChangeListener(SteamVR_Action_Single.ActiveChangeHandler functionToCall, SteamVR_Input_Sources inputSource)
		{
			this.sourceMap[inputSource].onActiveChange += functionToCall;
		}

		public void RemoveOnActiveChangeListener(SteamVR_Action_Single.ActiveChangeHandler functionToStopCalling, SteamVR_Input_Sources inputSource)
		{
			this.sourceMap[inputSource].onActiveChange -= functionToStopCalling;
		}

		public void AddOnActiveBindingChangeListener(SteamVR_Action_Single.ActiveChangeHandler functionToCall, SteamVR_Input_Sources inputSource)
		{
			this.sourceMap[inputSource].onActiveBindingChange += functionToCall;
		}

		public void RemoveOnActiveBindingChangeListener(SteamVR_Action_Single.ActiveChangeHandler functionToStopCalling, SteamVR_Input_Sources inputSource)
		{
			this.sourceMap[inputSource].onActiveBindingChange -= functionToStopCalling;
		}

		public void AddOnChangeListener(SteamVR_Action_Single.ChangeHandler functionToCall, SteamVR_Input_Sources inputSource)
		{
			this.sourceMap[inputSource].onChange += functionToCall;
		}

		public void RemoveOnChangeListener(SteamVR_Action_Single.ChangeHandler functionToStopCalling, SteamVR_Input_Sources inputSource)
		{
			this.sourceMap[inputSource].onChange -= functionToStopCalling;
		}

		public void AddOnUpdateListener(SteamVR_Action_Single.UpdateHandler functionToCall, SteamVR_Input_Sources inputSource)
		{
			this.sourceMap[inputSource].onUpdate += functionToCall;
		}

		public void RemoveOnUpdateListener(SteamVR_Action_Single.UpdateHandler functionToStopCalling, SteamVR_Input_Sources inputSource)
		{
			this.sourceMap[inputSource].onUpdate -= functionToStopCalling;
		}

		public void AddOnAxisListener(SteamVR_Action_Single.AxisHandler functionToCall, SteamVR_Input_Sources inputSource)
		{
			this.sourceMap[inputSource].onAxis += functionToCall;
		}

		public void RemoveOnAxisListener(SteamVR_Action_Single.AxisHandler functionToStopCalling, SteamVR_Input_Sources inputSource)
		{
			this.sourceMap[inputSource].onAxis -= functionToStopCalling;
		}

		public void RemoveAllListeners(SteamVR_Input_Sources inputSource)
		{
			this.sourceMap[inputSource].RemoveAllListeners();
		}

		void ISerializationCallbackReceiver.OnBeforeSerialize()
		{
		}

		void ISerializationCallbackReceiver.OnAfterDeserialize()
		{
			base.InitAfterDeserialize();
		}

		public delegate void AxisHandler(SteamVR_Action_Single fromAction, SteamVR_Input_Sources fromSource, float newAxis, float newDelta);

		public delegate void ActiveChangeHandler(SteamVR_Action_Single fromAction, SteamVR_Input_Sources fromSource, bool active);

		public delegate void ChangeHandler(SteamVR_Action_Single fromAction, SteamVR_Input_Sources fromSource, float newAxis, float newDelta);

		public delegate void UpdateHandler(SteamVR_Action_Single fromAction, SteamVR_Input_Sources fromSource, float newAxis, float newDelta);
	}
}
