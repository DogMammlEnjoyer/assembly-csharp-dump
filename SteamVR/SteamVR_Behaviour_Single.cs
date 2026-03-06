using System;
using UnityEngine;

namespace Valve.VR
{
	public class SteamVR_Behaviour_Single : MonoBehaviour
	{
		public bool isActive
		{
			get
			{
				return this.singleAction.GetActive(this.inputSource);
			}
		}

		protected virtual void OnEnable()
		{
			if (this.singleAction == null)
			{
				Debug.LogError("[SteamVR] Single action not set.", this);
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
			this.singleAction[this.inputSource].onUpdate += this.SteamVR_Behaviour_Single_OnUpdate;
			this.singleAction[this.inputSource].onChange += this.SteamVR_Behaviour_Single_OnChange;
			this.singleAction[this.inputSource].onAxis += this.SteamVR_Behaviour_Single_OnAxis;
		}

		protected void RemoveHandlers()
		{
			if (this.singleAction != null)
			{
				this.singleAction[this.inputSource].onUpdate -= this.SteamVR_Behaviour_Single_OnUpdate;
				this.singleAction[this.inputSource].onChange -= this.SteamVR_Behaviour_Single_OnChange;
				this.singleAction[this.inputSource].onAxis -= this.SteamVR_Behaviour_Single_OnAxis;
			}
		}

		private void SteamVR_Behaviour_Single_OnUpdate(SteamVR_Action_Single fromAction, SteamVR_Input_Sources fromSource, float newAxis, float newDelta)
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

		private void SteamVR_Behaviour_Single_OnChange(SteamVR_Action_Single fromAction, SteamVR_Input_Sources fromSource, float newAxis, float newDelta)
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

		private void SteamVR_Behaviour_Single_OnAxis(SteamVR_Action_Single fromAction, SteamVR_Input_Sources fromSource, float newAxis, float newDelta)
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
			if (this.singleAction != null)
			{
				return this.singleAction.GetLocalizedOriginPart(this.inputSource, localizedParts);
			}
			return null;
		}

		public SteamVR_Action_Single singleAction;

		[Tooltip("The device this action should apply to. Any if the action is not device specific.")]
		public SteamVR_Input_Sources inputSource;

		[Tooltip("Fires whenever the action's value has changed since the last update.")]
		public SteamVR_Behaviour_SingleEvent onChange;

		[Tooltip("Fires whenever the action's value has been updated.")]
		public SteamVR_Behaviour_SingleEvent onUpdate;

		[Tooltip("Fires whenever the action's value has been updated and is non-zero.")]
		public SteamVR_Behaviour_SingleEvent onAxis;

		public SteamVR_Behaviour_Single.ChangeHandler onChangeEvent;

		public SteamVR_Behaviour_Single.UpdateHandler onUpdateEvent;

		public SteamVR_Behaviour_Single.AxisHandler onAxisEvent;

		public delegate void AxisHandler(SteamVR_Behaviour_Single fromAction, SteamVR_Input_Sources fromSource, float newAxis, float newDelta);

		public delegate void ChangeHandler(SteamVR_Behaviour_Single fromAction, SteamVR_Input_Sources fromSource, float newAxis, float newDelta);

		public delegate void UpdateHandler(SteamVR_Behaviour_Single fromAction, SteamVR_Input_Sources fromSource, float newAxis, float newDelta);
	}
}
