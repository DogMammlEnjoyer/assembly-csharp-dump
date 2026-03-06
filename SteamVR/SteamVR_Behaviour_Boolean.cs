using System;
using UnityEngine;

namespace Valve.VR
{
	public class SteamVR_Behaviour_Boolean : MonoBehaviour
	{
		public event SteamVR_Behaviour_Boolean.ChangeHandler onChangeEvent;

		public event SteamVR_Behaviour_Boolean.UpdateHandler onUpdateEvent;

		public event SteamVR_Behaviour_Boolean.StateHandler onPressEvent;

		public event SteamVR_Behaviour_Boolean.StateDownHandler onPressDownEvent;

		public event SteamVR_Behaviour_Boolean.StateUpHandler onPressUpEvent;

		public bool isActive
		{
			get
			{
				return this.booleanAction[this.inputSource].active;
			}
		}

		public SteamVR_ActionSet actionSet
		{
			get
			{
				if (this.booleanAction != null)
				{
					return this.booleanAction.actionSet;
				}
				return null;
			}
		}

		protected virtual void OnEnable()
		{
			if (this.booleanAction == null)
			{
				Debug.LogError("[SteamVR] Boolean action not set.", this);
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
			this.booleanAction[this.inputSource].onUpdate += this.SteamVR_Behaviour_Boolean_OnUpdate;
			this.booleanAction[this.inputSource].onChange += this.SteamVR_Behaviour_Boolean_OnChange;
			this.booleanAction[this.inputSource].onState += this.SteamVR_Behaviour_Boolean_OnState;
			this.booleanAction[this.inputSource].onStateDown += this.SteamVR_Behaviour_Boolean_OnStateDown;
			this.booleanAction[this.inputSource].onStateUp += this.SteamVR_Behaviour_Boolean_OnStateUp;
		}

		protected void RemoveHandlers()
		{
			if (this.booleanAction != null)
			{
				this.booleanAction[this.inputSource].onUpdate -= this.SteamVR_Behaviour_Boolean_OnUpdate;
				this.booleanAction[this.inputSource].onChange -= this.SteamVR_Behaviour_Boolean_OnChange;
				this.booleanAction[this.inputSource].onState -= this.SteamVR_Behaviour_Boolean_OnState;
				this.booleanAction[this.inputSource].onStateDown -= this.SteamVR_Behaviour_Boolean_OnStateDown;
				this.booleanAction[this.inputSource].onStateUp -= this.SteamVR_Behaviour_Boolean_OnStateUp;
			}
		}

		private void SteamVR_Behaviour_Boolean_OnStateUp(SteamVR_Action_Boolean fromAction, SteamVR_Input_Sources fromSource)
		{
			if (this.onPressUp != null)
			{
				this.onPressUp.Invoke(this, fromSource, false);
			}
			if (this.onPressUpEvent != null)
			{
				this.onPressUpEvent(this, fromSource);
			}
		}

		private void SteamVR_Behaviour_Boolean_OnStateDown(SteamVR_Action_Boolean fromAction, SteamVR_Input_Sources fromSource)
		{
			if (this.onPressDown != null)
			{
				this.onPressDown.Invoke(this, fromSource, true);
			}
			if (this.onPressDownEvent != null)
			{
				this.onPressDownEvent(this, fromSource);
			}
		}

		private void SteamVR_Behaviour_Boolean_OnState(SteamVR_Action_Boolean fromAction, SteamVR_Input_Sources fromSource)
		{
			if (this.onPress != null)
			{
				this.onPress.Invoke(this, fromSource, true);
			}
			if (this.onPressEvent != null)
			{
				this.onPressEvent(this, fromSource);
			}
		}

		private void SteamVR_Behaviour_Boolean_OnUpdate(SteamVR_Action_Boolean fromAction, SteamVR_Input_Sources fromSource, bool newState)
		{
			if (this.onUpdate != null)
			{
				this.onUpdate.Invoke(this, fromSource, newState);
			}
			if (this.onUpdateEvent != null)
			{
				this.onUpdateEvent(this, fromSource, newState);
			}
		}

		private void SteamVR_Behaviour_Boolean_OnChange(SteamVR_Action_Boolean fromAction, SteamVR_Input_Sources fromSource, bool newState)
		{
			if (this.onChange != null)
			{
				this.onChange.Invoke(this, fromSource, newState);
			}
			if (this.onChangeEvent != null)
			{
				this.onChangeEvent(this, fromSource, newState);
			}
		}

		public string GetLocalizedName(params EVRInputStringBits[] localizedParts)
		{
			if (this.booleanAction != null)
			{
				return this.booleanAction.GetLocalizedOriginPart(this.inputSource, localizedParts);
			}
			return null;
		}

		[Tooltip("The SteamVR boolean action that this component should use")]
		public SteamVR_Action_Boolean booleanAction;

		[Tooltip("The device this action should apply to. Any if the action is not device specific.")]
		public SteamVR_Input_Sources inputSource;

		public SteamVR_Behaviour_BooleanEvent onChange;

		public SteamVR_Behaviour_BooleanEvent onUpdate;

		public SteamVR_Behaviour_BooleanEvent onPress;

		public SteamVR_Behaviour_BooleanEvent onPressDown;

		public SteamVR_Behaviour_BooleanEvent onPressUp;

		public delegate void StateDownHandler(SteamVR_Behaviour_Boolean fromAction, SteamVR_Input_Sources fromSource);

		public delegate void StateUpHandler(SteamVR_Behaviour_Boolean fromAction, SteamVR_Input_Sources fromSource);

		public delegate void StateHandler(SteamVR_Behaviour_Boolean fromAction, SteamVR_Input_Sources fromSource);

		public delegate void ActiveChangeHandler(SteamVR_Behaviour_Boolean fromAction, SteamVR_Input_Sources fromSource, bool active);

		public delegate void ChangeHandler(SteamVR_Behaviour_Boolean fromAction, SteamVR_Input_Sources fromSource, bool newState);

		public delegate void UpdateHandler(SteamVR_Behaviour_Boolean fromAction, SteamVR_Input_Sources fromSource, bool newState);
	}
}
