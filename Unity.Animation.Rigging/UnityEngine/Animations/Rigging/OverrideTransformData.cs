using System;

namespace UnityEngine.Animations.Rigging
{
	[Serializable]
	public struct OverrideTransformData : IAnimationJobData, IOverrideTransformData
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

		public Transform sourceObject
		{
			get
			{
				return this.m_OverrideSource;
			}
			set
			{
				this.m_OverrideSource = value;
			}
		}

		public OverrideTransformData.Space space
		{
			get
			{
				return this.m_Space;
			}
			set
			{
				this.m_Space = value;
			}
		}

		public Vector3 position
		{
			get
			{
				return this.m_OverridePosition;
			}
			set
			{
				this.m_OverridePosition = value;
			}
		}

		public Vector3 rotation
		{
			get
			{
				return this.m_OverrideRotation;
			}
			set
			{
				this.m_OverrideRotation = value;
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

		int IOverrideTransformData.space
		{
			get
			{
				return (int)this.m_Space;
			}
		}

		string IOverrideTransformData.positionWeightFloatProperty
		{
			get
			{
				return ConstraintsUtils.ConstructConstraintDataPropertyName("m_PositionWeight");
			}
		}

		string IOverrideTransformData.rotationWeightFloatProperty
		{
			get
			{
				return ConstraintsUtils.ConstructConstraintDataPropertyName("m_RotationWeight");
			}
		}

		string IOverrideTransformData.positionVector3Property
		{
			get
			{
				return ConstraintsUtils.ConstructConstraintDataPropertyName("m_OverridePosition");
			}
		}

		string IOverrideTransformData.rotationVector3Property
		{
			get
			{
				return ConstraintsUtils.ConstructConstraintDataPropertyName("m_OverrideRotation");
			}
		}

		bool IAnimationJobData.IsValid()
		{
			return this.m_ConstrainedObject != null;
		}

		void IAnimationJobData.SetDefaultValues()
		{
			this.m_ConstrainedObject = null;
			this.m_OverrideSource = null;
			this.m_OverridePosition = Vector3.zero;
			this.m_OverrideRotation = Vector3.zero;
			this.m_Space = OverrideTransformData.Space.Pivot;
			this.m_PositionWeight = 1f;
			this.m_RotationWeight = 1f;
		}

		[SerializeField]
		private Transform m_ConstrainedObject;

		[SyncSceneToStream]
		[SerializeField]
		private Transform m_OverrideSource;

		[SyncSceneToStream]
		[SerializeField]
		private Vector3 m_OverridePosition;

		[SyncSceneToStream]
		[SerializeField]
		private Vector3 m_OverrideRotation;

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
		private OverrideTransformData.Space m_Space;

		[Serializable]
		public enum Space
		{
			World,
			Local,
			Pivot
		}
	}
}
