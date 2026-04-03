using System;
using UnityEngine;

namespace GorillaTag.Gravity
{
	public class ChangingBasicGravityZone : BasicGravityZone
	{
		protected override void Awake()
		{
			base.Awake();
			this.m_thisCallbackUnique = this;
			this.m_strengthDirty = false;
			this.m_directionDity = false;
		}

		protected override void OnDisable()
		{
			base.OnDisable();
			if (this.m_strengthDirty)
			{
				this.m_strengthDirty = false;
				this.gravityStrength = this.m_targetGravityStrength;
			}
			if (this.m_directionDity)
			{
				this.m_directionDity = false;
				this.m_gravityDirection = this.m_targetGravityDirection;
			}
		}

		public void Update()
		{
			if (this.lastValueWhenSet == this.ExternalTriggerSetGravityStrength)
			{
				this.lastExternalTriggerSetMatched = true;
				return;
			}
			if (!this.lastExternalTriggerSetMatched)
			{
				this.SetGravityStrength(this.ExternalSetGravityStrength);
				this.lastValueWhenSet = this.ExternalTriggerSetGravityStrength;
				this.lastExternalTriggerSetMatched = true;
				return;
			}
			this.ExternalTriggerSetGravityStrength = this.lastValueWhenSet;
			this.lastExternalTriggerSetMatched = false;
		}

		public void SetGravityStrength(float strength)
		{
			this.SetGravityStrength(strength, this.m_changeStrengthTime);
		}

		public void SetGravityDirection(Vector3 dir)
		{
			this.SetGravityDirection(dir, this.m_changeDirectionTime);
		}

		public void SetGravityStrength(float strength, float time)
		{
			this.m_targetGravityStrength = strength;
			if (time == 0f || !this.m_thisCallbackUnique.Registered)
			{
				this.gravityStrength = this.m_targetGravityStrength;
				this.m_strengthDirty = false;
				return;
			}
			this.m_lerpToGravitySpeed = (strength - this.gravityStrength) / time;
			this.m_strengthDirty = true;
		}

		public void SetGravityDirection(Vector3 direction, float time)
		{
			this.m_targetGravityDirection = direction.normalized;
			if (time == 0f || !this.m_thisCallbackUnique.Registered)
			{
				this.m_gravityDirection = this.m_targetGravityDirection;
				this.m_directionDity = false;
				return;
			}
			float num = Vector3.Angle(this.m_gravityDirection, direction) * 0.017453292f;
			this.m_lerpToDirectionSpeed = num / time;
			this.m_directionDity = true;
		}

		public void SetRotationIntent(bool rotate)
		{
			this.rotateTarget = rotate;
		}

		public override void CallBack()
		{
			if (this.m_strengthDirty)
			{
				this.gravityStrength = Mathf.MoveTowards(this.gravityStrength, this.m_targetGravityStrength, this.m_lerpToGravitySpeed * Time.fixedDeltaTime);
				if (Mathf.Approximately(this.gravityStrength, this.m_targetGravityStrength))
				{
					this.m_strengthDirty = false;
				}
			}
			if (this.m_directionDity)
			{
				this.m_gravityDirection = Vector3.RotateTowards(this.m_gravityDirection, this.m_targetGravityDirection, this.m_lerpToDirectionSpeed * Time.fixedDeltaTime, 0f);
				if (this.m_gravityDirection == this.m_targetGravityDirection)
				{
					this.m_directionDity = false;
				}
			}
			base.CallBack();
		}

		[Header("Change Value To Trigger Gravity Strength Change At Set Value (false to true and true to false both work, but value must change the frame you want it changed)")]
		public bool ExternalTriggerSetGravityStrength;

		public float ExternalSetGravityStrength;

		private bool lastExternalTriggerSetMatched = true;

		private bool lastValueWhenSet;

		private bool m_strengthDirty;

		private float m_targetGravityStrength;

		private float m_lerpToGravitySpeed;

		private bool m_directionDity;

		private Vector3 m_targetGravityDirection;

		private float m_lerpToDirectionSpeed;

		[SerializeField]
		private float m_changeStrengthTime;

		[SerializeField]
		private float m_changeDirectionTime;

		private ICallbackUnique m_thisCallbackUnique;
	}
}
