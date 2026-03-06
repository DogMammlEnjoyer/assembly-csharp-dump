using System;

namespace UnityEngine.XR.Interaction.Toolkit
{
	[Serializable]
	public struct InteractionState
	{
		public float value
		{
			get
			{
				return this.m_Value;
			}
			set
			{
				this.m_Value = value;
			}
		}

		public bool active
		{
			get
			{
				return this.m_Active;
			}
			set
			{
				this.m_Active = value;
			}
		}

		public bool activatedThisFrame
		{
			get
			{
				return this.m_ActivatedThisFrame;
			}
			set
			{
				this.m_ActivatedThisFrame = value;
			}
		}

		public bool deactivatedThisFrame
		{
			get
			{
				return this.m_DeactivatedThisFrame;
			}
			set
			{
				this.m_DeactivatedThisFrame = value;
			}
		}

		public void SetFrameState(bool isActive)
		{
			this.SetFrameState(isActive, isActive ? 1f : 0f);
		}

		public void SetFrameState(bool isActive, float newValue)
		{
			this.value = newValue;
			this.activatedThisFrame = (!this.active && isActive);
			this.deactivatedThisFrame = (this.active && !isActive);
			this.active = isActive;
		}

		public void SetFrameDependent(bool wasActive)
		{
			this.activatedThisFrame = (!wasActive && this.active);
			this.deactivatedThisFrame = (wasActive && !this.active);
		}

		public void ResetFrameDependent()
		{
			this.activatedThisFrame = false;
			this.deactivatedThisFrame = false;
		}

		[Obsolete("deActivatedThisFrame has been deprecated. Use deactivatedThisFrame instead. (UnityUpgradable) -> deactivatedThisFrame", true)]
		public bool deActivatedThisFrame
		{
			get
			{
				return false;
			}
			set
			{
			}
		}

		[Obsolete("Reset has been renamed. Use ResetFrameDependent instead. (UnityUpgradable) -> ResetFrameDependent()", true)]
		public void Reset()
		{
		}

		[Range(0f, 1f)]
		[SerializeField]
		private float m_Value;

		[SerializeField]
		private bool m_Active;

		private bool m_ActivatedThisFrame;

		private bool m_DeactivatedThisFrame;
	}
}
