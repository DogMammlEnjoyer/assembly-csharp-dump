using System;
using UnityEngine;

namespace Valve.VR
{
	[Serializable]
	public class SteamVR_Action_Vector2 : SteamVR_Action_In<SteamVR_Action_Vector2_Source_Map, SteamVR_Action_Vector2_Source>, ISteamVR_Action_Vector2, ISteamVR_Action_In_Source, ISteamVR_Action_Source, ISerializationCallbackReceiver
	{
		public event SteamVR_Action_Vector2.ChangeHandler onChange
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

		public event SteamVR_Action_Vector2.UpdateHandler onUpdate
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

		public event SteamVR_Action_Vector2.AxisHandler onAxis
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

		public event SteamVR_Action_Vector2.ActiveChangeHandler onActiveChange
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

		public event SteamVR_Action_Vector2.ActiveChangeHandler onActiveBindingChange
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

		public Vector2 axis
		{
			get
			{
				return this.sourceMap[SteamVR_Input_Sources.Any].axis;
			}
		}

		public Vector2 lastAxis
		{
			get
			{
				return this.sourceMap[SteamVR_Input_Sources.Any].lastAxis;
			}
		}

		public Vector2 delta
		{
			get
			{
				return this.sourceMap[SteamVR_Input_Sources.Any].delta;
			}
		}

		public Vector2 lastDelta
		{
			get
			{
				return this.sourceMap[SteamVR_Input_Sources.Any].lastDelta;
			}
		}

		public Vector2 GetAxis(SteamVR_Input_Sources inputSource)
		{
			return this.sourceMap[inputSource].axis;
		}

		public Vector2 GetAxisDelta(SteamVR_Input_Sources inputSource)
		{
			return this.sourceMap[inputSource].delta;
		}

		public Vector2 GetLastAxis(SteamVR_Input_Sources inputSource)
		{
			return this.sourceMap[inputSource].lastAxis;
		}

		public Vector2 GetLastAxisDelta(SteamVR_Input_Sources inputSource)
		{
			return this.sourceMap[inputSource].lastDelta;
		}

		public void AddOnActiveChangeListener(SteamVR_Action_Vector2.ActiveChangeHandler functionToCall, SteamVR_Input_Sources inputSource)
		{
			this.sourceMap[inputSource].onActiveChange += functionToCall;
		}

		public void RemoveOnActiveChangeListener(SteamVR_Action_Vector2.ActiveChangeHandler functionToStopCalling, SteamVR_Input_Sources inputSource)
		{
			this.sourceMap[inputSource].onActiveChange -= functionToStopCalling;
		}

		public void AddOnActiveBindingChangeListener(SteamVR_Action_Vector2.ActiveChangeHandler functionToCall, SteamVR_Input_Sources inputSource)
		{
			this.sourceMap[inputSource].onActiveBindingChange += functionToCall;
		}

		public void RemoveOnActiveBindingChangeListener(SteamVR_Action_Vector2.ActiveChangeHandler functionToStopCalling, SteamVR_Input_Sources inputSource)
		{
			this.sourceMap[inputSource].onActiveBindingChange -= functionToStopCalling;
		}

		public void AddOnChangeListener(SteamVR_Action_Vector2.ChangeHandler functionToCall, SteamVR_Input_Sources inputSource)
		{
			this.sourceMap[inputSource].onChange += functionToCall;
		}

		public void RemoveOnChangeListener(SteamVR_Action_Vector2.ChangeHandler functionToStopCalling, SteamVR_Input_Sources inputSource)
		{
			this.sourceMap[inputSource].onChange -= functionToStopCalling;
		}

		public void AddOnUpdateListener(SteamVR_Action_Vector2.UpdateHandler functionToCall, SteamVR_Input_Sources inputSource)
		{
			this.sourceMap[inputSource].onUpdate += functionToCall;
		}

		public void RemoveOnUpdateListener(SteamVR_Action_Vector2.UpdateHandler functionToStopCalling, SteamVR_Input_Sources inputSource)
		{
			this.sourceMap[inputSource].onUpdate -= functionToStopCalling;
		}

		public void AddOnAxisListener(SteamVR_Action_Vector2.AxisHandler functionToCall, SteamVR_Input_Sources inputSource)
		{
			this.sourceMap[inputSource].onAxis += functionToCall;
		}

		public void RemoveOnAxisListener(SteamVR_Action_Vector2.AxisHandler functionToStopCalling, SteamVR_Input_Sources inputSource)
		{
			this.sourceMap[inputSource].onAxis -= functionToStopCalling;
		}

		public void RemoveAllListeners(SteamVR_Input_Sources input_Sources)
		{
			this.sourceMap[input_Sources].RemoveAllListeners();
		}

		void ISerializationCallbackReceiver.OnBeforeSerialize()
		{
		}

		void ISerializationCallbackReceiver.OnAfterDeserialize()
		{
			base.InitAfterDeserialize();
		}

		public delegate void AxisHandler(SteamVR_Action_Vector2 fromAction, SteamVR_Input_Sources fromSource, Vector2 axis, Vector2 delta);

		public delegate void ActiveChangeHandler(SteamVR_Action_Vector2 fromAction, SteamVR_Input_Sources fromSource, bool active);

		public delegate void ChangeHandler(SteamVR_Action_Vector2 fromAction, SteamVR_Input_Sources fromSource, Vector2 axis, Vector2 delta);

		public delegate void UpdateHandler(SteamVR_Action_Vector2 fromAction, SteamVR_Input_Sources fromSource, Vector2 axis, Vector2 delta);
	}
}
