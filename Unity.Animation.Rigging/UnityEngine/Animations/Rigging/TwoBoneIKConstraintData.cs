using System;

namespace UnityEngine.Animations.Rigging
{
	[Serializable]
	public struct TwoBoneIKConstraintData : IAnimationJobData, ITwoBoneIKConstraintData
	{
		public Transform root
		{
			get
			{
				return this.m_Root;
			}
			set
			{
				this.m_Root = value;
			}
		}

		public Transform mid
		{
			get
			{
				return this.m_Mid;
			}
			set
			{
				this.m_Mid = value;
			}
		}

		public Transform tip
		{
			get
			{
				return this.m_Tip;
			}
			set
			{
				this.m_Tip = value;
			}
		}

		public Transform target
		{
			get
			{
				return this.m_Target;
			}
			set
			{
				this.m_Target = value;
			}
		}

		public Transform hint
		{
			get
			{
				return this.m_Hint;
			}
			set
			{
				this.m_Hint = value;
			}
		}

		public float targetPositionWeight
		{
			get
			{
				return this.m_TargetPositionWeight;
			}
			set
			{
				this.m_TargetPositionWeight = Mathf.Clamp01(value);
			}
		}

		public float targetRotationWeight
		{
			get
			{
				return this.m_TargetRotationWeight;
			}
			set
			{
				this.m_TargetRotationWeight = Mathf.Clamp01(value);
			}
		}

		public float hintWeight
		{
			get
			{
				return this.m_HintWeight;
			}
			set
			{
				this.m_HintWeight = Mathf.Clamp01(value);
			}
		}

		public bool maintainTargetPositionOffset
		{
			get
			{
				return this.m_MaintainTargetPositionOffset;
			}
			set
			{
				this.m_MaintainTargetPositionOffset = value;
			}
		}

		public bool maintainTargetRotationOffset
		{
			get
			{
				return this.m_MaintainTargetRotationOffset;
			}
			set
			{
				this.m_MaintainTargetRotationOffset = value;
			}
		}

		string ITwoBoneIKConstraintData.targetPositionWeightFloatProperty
		{
			get
			{
				return ConstraintsUtils.ConstructConstraintDataPropertyName("m_TargetPositionWeight");
			}
		}

		string ITwoBoneIKConstraintData.targetRotationWeightFloatProperty
		{
			get
			{
				return ConstraintsUtils.ConstructConstraintDataPropertyName("m_TargetRotationWeight");
			}
		}

		string ITwoBoneIKConstraintData.hintWeightFloatProperty
		{
			get
			{
				return ConstraintsUtils.ConstructConstraintDataPropertyName("m_HintWeight");
			}
		}

		bool IAnimationJobData.IsValid()
		{
			return this.m_Tip != null && this.m_Mid != null && this.m_Root != null && this.m_Target != null && this.m_Tip.IsChildOf(this.m_Mid) && this.m_Mid.IsChildOf(this.m_Root);
		}

		void IAnimationJobData.SetDefaultValues()
		{
			this.m_Root = null;
			this.m_Mid = null;
			this.m_Tip = null;
			this.m_Target = null;
			this.m_Hint = null;
			this.m_TargetPositionWeight = 1f;
			this.m_TargetRotationWeight = 1f;
			this.m_HintWeight = 1f;
			this.m_MaintainTargetPositionOffset = false;
			this.m_MaintainTargetRotationOffset = false;
		}

		[SerializeField]
		private Transform m_Root;

		[SerializeField]
		private Transform m_Mid;

		[SerializeField]
		private Transform m_Tip;

		[SyncSceneToStream]
		[SerializeField]
		private Transform m_Target;

		[SyncSceneToStream]
		[SerializeField]
		private Transform m_Hint;

		[SyncSceneToStream]
		[SerializeField]
		[Range(0f, 1f)]
		private float m_TargetPositionWeight;

		[SyncSceneToStream]
		[SerializeField]
		[Range(0f, 1f)]
		private float m_TargetRotationWeight;

		[SyncSceneToStream]
		[SerializeField]
		[Range(0f, 1f)]
		private float m_HintWeight;

		[NotKeyable]
		[SerializeField]
		private bool m_MaintainTargetPositionOffset;

		[NotKeyable]
		[SerializeField]
		private bool m_MaintainTargetRotationOffset;
	}
}
