using System;
using UnityEngine;

namespace Valve.VR
{
	public class SteamVR_Behaviour_Vector3 : MonoBehaviour
	{
		public bool isActive
		{
			get
			{
				return this.vector3Action.GetActive(this.inputSource);
			}
		}

		protected virtual void OnEnable()
		{
			if (this.vector3Action == null)
			{
				Debug.LogError("[SteamVR] Vector3 action not set.", this);
				return;
			}
			this.AddHandlers();
		}

		protected virtual void OnDisable()
		{
			this.RemoveHandlers();
		}

		protected void AddHandlers()
		{
			this.vector3Action[this.inputSource].onUpdate += this.SteamVR_Behaviour_Vector3_OnUpdate;
			this.vector3Action[this.inputSource].onChange += this.SteamVR_Behaviour_Vector3_OnChange;
			this.vector3Action[this.inputSource].onAxis += this.SteamVR_Behaviour_Vector3_OnAxis;
		}

		protected void RemoveHandlers()
		{
			if (this.vector3Action != null)
			{
				this.vector3Action[this.inputSource].onUpdate -= this.SteamVR_Behaviour_Vector3_OnUpdate;
				this.vector3Action[this.inputSource].onChange -= this.SteamVR_Behaviour_Vector3_OnChange;
				this.vector3Action[this.inputSource].onAxis -= this.SteamVR_Behaviour_Vector3_OnAxis;
			}
		}

		private void SteamVR_Behaviour_Vector3_OnUpdate(SteamVR_Action_Vector3 fromAction, SteamVR_Input_Sources fromSource, Vector3 newAxis, Vector3 newDelta)
		{
			if (this.onUpdate != null)
			{
				this.onUpdate.Invoke(this, fromSource, newAxis, newDelta);
			}
			if (this.onUpdateEvent != null)
			{
				this.onUpdateEvent(this, fromSource, newAxis, newDelta);
			}
		}

		private void SteamVR_Behaviour_Vector3_OnChange(SteamVR_Action_Vector3 fromAction, SteamVR_Input_Sources fromSource, Vector3 newAxis, Vector3 newDelta)
		{
			if (this.onChange != null)
			{
				this.onChange.Invoke(this, fromSource, newAxis, newDelta);
			}
			if (this.onChangeEvent != null)
			{
				this.onChangeEvent(this, fromSource, newAxis, newDelta);
			}
		}

		private void SteamVR_Behaviour_Vector3_OnAxis(SteamVR_Action_Vector3 fromAction, SteamVR_Input_Sources fromSource, Vector3 newAxis, Vector3 newDelta)
		{
			if (this.onAxis != null)
			{
				this.onAxis.Invoke(this, fromSource, newAxis, newDelta);
			}
			if (this.onAxisEvent != null)
			{
				this.onAxisEvent(this, fromSource, newAxis, newDelta);
			}
		}

		public string GetLocalizedName(params EVRInputStringBits[] localizedParts)
		{
			if (this.vector3Action != null)
			{
				return this.vector3Action.GetLocalizedOriginPart(this.inputSource, localizedParts);
			}
			return null;
		}

		public SteamVR_Action_Vector3 vector3Action;

		[Tooltip("The device this action should apply to. Any if the action is not device specific.")]
		public SteamVR_Input_Sources inputSource;

		[Tooltip("Fires whenever the action's value has changed since the last update.")]
		public SteamVR_Behaviour_Vector3Event onChange;

		[Tooltip("Fires whenever the action's value has been updated.")]
		public SteamVR_Behaviour_Vector3Event onUpdate;

		[Tooltip("Fires whenever the action's value has been updated and is non-zero.")]
		public SteamVR_Behaviour_Vector3Event onAxis;

		public SteamVR_Behaviour_Vector3.ChangeHandler onChangeEvent;

		public SteamVR_Behaviour_Vector3.UpdateHandler onUpdateEvent;

		public SteamVR_Behaviour_Vector3.AxisHandler onAxisEvent;

		public delegate void AxisHandler(SteamVR_Behaviour_Vector3 fromAction, SteamVR_Input_Sources fromSource, Vector3 newAxis, Vector3 newDelta);

		public delegate void ChangeHandler(SteamVR_Behaviour_Vector3 fromAction, SteamVR_Input_Sources fromSource, Vector3 newAxis, Vector3 newDelta);

		public delegate void UpdateHandler(SteamVR_Behaviour_Vector3 fromAction, SteamVR_Input_Sources fromSource, Vector3 newAxis, Vector3 newDelta);
	}
}
