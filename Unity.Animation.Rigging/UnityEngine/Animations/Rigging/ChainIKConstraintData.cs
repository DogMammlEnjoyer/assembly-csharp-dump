using System;

namespace UnityEngine.Animations.Rigging
{
	[Serializable]
	public struct ChainIKConstraintData : IAnimationJobData, IChainIKConstraintData
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

		public float chainRotationWeight
		{
			get
			{
				return this.m_ChainRotationWeight;
			}
			set
			{
				this.m_ChainRotationWeight = Mathf.Clamp01(value);
			}
		}

		public float tipRotationWeight
		{
			get
			{
				return this.m_TipRotationWeight;
			}
			set
			{
				this.m_TipRotationWeight = Mathf.Clamp01(value);
			}
		}

		public int maxIterations
		{
			get
			{
				return this.m_MaxIterations;
			}
			set
			{
				this.m_MaxIterations = Mathf.Clamp(value, 1, 50);
			}
		}

		public float tolerance
		{
			get
			{
				return this.m_Tolerance;
			}
			set
			{
				this.m_Tolerance = Mathf.Clamp(value, 0f, 0.01f);
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

		string IChainIKConstraintData.chainRotationWeightFloatProperty
		{
			get
			{
				return ConstraintsUtils.ConstructConstraintDataPropertyName("m_ChainRotationWeight");
			}
		}

		string IChainIKConstraintData.tipRotationWeightFloatProperty
		{
			get
			{
				return ConstraintsUtils.ConstructConstraintDataPropertyName("m_TipRotationWeight");
			}
		}

		bool IAnimationJobData.IsValid()
		{
			if (this.m_Root == null || this.m_Tip == null || this.m_Target == null)
			{
				return false;
			}
			int num = 1;
			Transform transform = this.m_Tip;
			while (transform != null && transform != this.m_Root)
			{
				transform = transform.parent;
				num++;
			}
			return transform == this.m_Root && num > 2;
		}

		void IAnimationJobData.SetDefaultValues()
		{
			this.m_Root = null;
			this.m_Tip = null;
			this.m_Target = null;
			this.m_ChainRotationWeight = 1f;
			this.m_TipRotationWeight = 1f;
			this.m_MaxIterations = 15;
			this.m_Tolerance = 0.0001f;
			this.m_MaintainTargetPositionOffset = false;
			this.m_MaintainTargetRotationOffset = false;
		}

		internal const int k_MinIterations = 1;

		internal const int k_MaxIterations = 50;

		internal const float k_MinTolerance = 0f;

		internal const float k_MaxTolerance = 0.01f;

		[SerializeField]
		private Transform m_Root;

		[SerializeField]
		private Transform m_Tip;

		[SyncSceneToStream]
		[SerializeField]
		private Transform m_Target;

		[SyncSceneToStream]
		[SerializeField]
		[Range(0f, 1f)]
		private float m_ChainRotationWeight;

		[SyncSceneToStream]
		[SerializeField]
		[Range(0f, 1f)]
		private float m_TipRotationWeight;

		[NotKeyable]
		[SerializeField]
		[Range(1f, 50f)]
		private int m_MaxIterations;

		[NotKeyable]
		[SerializeField]
		[Range(0f, 0.01f)]
		private float m_Tolerance;

		[NotKeyable]
		[SerializeField]
		private bool m_MaintainTargetPositionOffset;

		[NotKeyable]
		[SerializeField]
		private bool m_MaintainTargetRotationOffset;
	}
}
