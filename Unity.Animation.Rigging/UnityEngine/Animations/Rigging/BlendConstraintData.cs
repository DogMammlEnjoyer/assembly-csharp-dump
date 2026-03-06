using System;

namespace UnityEngine.Animations.Rigging
{
	[Serializable]
	public struct BlendConstraintData : IAnimationJobData, IBlendConstraintData
	{
		public Transform constrainedObject
		{
			get
			{
				return this.m_ConstrainedObject;
			}
			set
			{
				this.m_ConstrainedObject = value;
			}
		}

		public Transform sourceObjectA
		{
			get
			{
				return this.m_SourceA;
			}
			set
			{
				this.m_SourceA = value;
			}
		}

		public Transform sourceObjectB
		{
			get
			{
				return this.m_SourceB;
			}
			set
			{
				this.m_SourceB = value;
			}
		}

		public bool blendPosition
		{
			get
			{
				return this.m_BlendPosition;
			}
			set
			{
				this.m_BlendPosition = value;
			}
		}

		public bool blendRotation
		{
			get
			{
				return this.m_BlendRotation;
			}
			set
			{
				this.m_BlendRotation = value;
			}
		}

		public float positionWeight
		{
			get
			{
				return this.m_PositionWeight;
			}
			set
			{
				this.m_PositionWeight = Mathf.Clamp01(value);
			}
		}

		public float rotationWeight
		{
			get
			{
				return this.m_RotationWeight;
			}
			set
			{
				this.m_RotationWeight = Mathf.Clamp01(value);
			}
		}

		public bool maintainPositionOffsets
		{
			get
			{
				return this.m_MaintainPositionOffsets;
			}
			set
			{
				this.m_MaintainPositionOffsets = value;
			}
		}

		public bool maintainRotationOffsets
		{
			get
			{
				return this.m_MaintainRotationOffsets;
			}
			set
			{
				this.m_MaintainRotationOffsets = value;
			}
		}

		string IBlendConstraintData.blendPositionBoolProperty
		{
			get
			{
				return ConstraintsUtils.ConstructConstraintDataPropertyName("m_BlendPosition");
			}
		}

		string IBlendConstraintData.blendRotationBoolProperty
		{
			get
			{
				return ConstraintsUtils.ConstructConstraintDataPropertyName("m_BlendRotation");
			}
		}

		string IBlendConstraintData.positionWeightFloatProperty
		{
			get
			{
				return ConstraintsUtils.ConstructConstraintDataPropertyName("m_PositionWeight");
			}
		}

		string IBlendConstraintData.rotationWeightFloatProperty
		{
			get
			{
				return ConstraintsUtils.ConstructConstraintDataPropertyName("m_RotationWeight");
			}
		}

		bool IAnimationJobData.IsValid()
		{
			return !(this.m_ConstrainedObject == null) && !(this.m_SourceA == null) && !(this.m_SourceB == null);
		}

		void IAnimationJobData.SetDefaultValues()
		{
			this.m_ConstrainedObject = null;
			this.m_SourceA = null;
			this.m_SourceB = null;
			this.m_BlendPosition = true;
			this.m_BlendRotation = true;
			this.m_PositionWeight = 0.5f;
			this.m_RotationWeight = 0.5f;
			this.m_MaintainPositionOffsets = false;
			this.m_MaintainRotationOffsets = false;
		}

		[SerializeField]
		private Transform m_ConstrainedObject;

		[SyncSceneToStream]
		[SerializeField]
		private Transform m_SourceA;

		[SyncSceneToStream]
		[SerializeField]
		private Transform m_SourceB;

		[SyncSceneToStream]
		[SerializeField]
		private bool m_BlendPosition;

		[SyncSceneToStream]
		[SerializeField]
		private bool m_BlendRotation;

		[SyncSceneToStream]
		[SerializeField]
		[Range(0f, 1f)]
		private float m_PositionWeight;

		[SyncSceneToStream]
		[SerializeField]
		[Range(0f, 1f)]
		private float m_RotationWeight;

		[NotKeyable]
		[SerializeField]
		private bool m_MaintainPositionOffsets;

		[NotKeyable]
		[SerializeField]
		private bool m_MaintainRotationOffsets;
	}
}
