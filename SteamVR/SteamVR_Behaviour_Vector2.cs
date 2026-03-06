using System;
using UnityEngine;

namespace Valve.VR
{
	public class SteamVR_Behaviour_Vector2 : MonoBehaviour
	{
		public bool isActive
		{
			get
			{
				return this.vector2Action.GetActive(this.inputSource);
			}
		}

		protected virtual void OnEnable()
		{
			if (this.vector2Action == null)
			{
				Debug.LogError("[SteamVR] Vector2 action not set.", this);
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
			this.vector2Action[this.inputSource].onUpdate += this.SteamVR_Behaviour_Vector2_OnUpdate;
			this.vector2Action[this.inputSource].onChange += this.SteamVR_Behaviour_Vector2_OnChange;
			this.vector2Action[this.inputSource].onAxis += this.SteamVR_Behaviour_Vector2_OnAxis;
		}

		protected void RemoveHandlers()
		{
			if (this.vector2Action != null)
			{
				this.vector2Action[this.inputSource].onUpdate -= this.SteamVR_Behaviour_Vector2_OnUpdate;
				this.vector2Action[this.inputSource].onChange -= this.SteamVR_Behaviour_Vector2_OnChange;
				this.vector2Action[this.inputSource].onAxis -= this.SteamVR_Behaviour_Vector2_OnAxis;
			}
		}

		private void SteamVR_Behaviour_Vector2_OnUpdate(SteamVR_Action_Vector2 fromAction, SteamVR_Input_Sources fromSource, Vector2 newAxis, Vector2 newDelta)
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

		private void SteamVR_Behaviour_Vector2_OnChange(SteamVR_Action_Vector2 fromAction, SteamVR_Input_Sources fromSource, Vector2 newAxis, Vector2 newDelta)
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

		private void SteamVR_Behaviour_Vector2_OnAxis(SteamVR_Action_Vector2 fromAction, SteamVR_Input_Sources fromSource, Vector2 newAxis, Vector2 newDelta)
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
			if (this.vector2Action != null)
			{
				return this.vector2Action.GetLocalizedOriginPart(this.inputSource, localizedParts);
			}
			return null;
		}

		public SteamVR_Action_Vector2 vector2Action;

		[Tooltip("The device this action should apply to. Any if the action is not device specific.")]
		public SteamVR_Input_Sources inputSource;

		[Tooltip("Fires whenever the action's value has changed since the last update.")]
		public SteamVR_Behaviour_Vector2Event onChange;

		[Tooltip("Fires whenever the action's value has been updated.")]
		public SteamVR_Behaviour_Vector2Event onUpdate;

		[Tooltip("Fires whenever the action's value has been updated and is non-zero.")]
		public SteamVR_Behaviour_Vector2Event onAxis;

		public SteamVR_Behaviour_Vector2.ChangeHandler onChangeEvent;

		public SteamVR_Behaviour_Vector2.UpdateHandler onUpdateEvent;

		public SteamVR_Behaviour_Vector2.AxisHandler onAxisEvent;

		public delegate void AxisHandler(SteamVR_Behaviour_Vector2 fromAction, SteamVR_Input_Sources fromSource, Vector2 newAxis, Vector2 newDelta);

		public delegate void ChangeHandler(SteamVR_Behaviour_Vector2 fromAction, SteamVR_Input_Sources fromSource, Vector2 newAxis, Vector2 newDelta);

		public delegate void UpdateHandler(SteamVR_Behaviour_Vector2 fromAction, SteamVR_Input_Sources fromSource, Vector2 newAxis, Vector2 newDelta);
	}
}
