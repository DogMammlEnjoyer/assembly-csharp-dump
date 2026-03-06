using System;

namespace UnityEngine.Animations.Rigging
{
	[Serializable]
	public struct DampedTransformData : IAnimationJobData, IDampedTransformData
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
				return this.m_Source;
			}
			set
			{
				this.m_Source = value;
			}
		}

		public float dampPosition
		{
			get
			{
				return this.m_DampPosition;
			}
			set
			{
				this.m_DampPosition = Mathf.Clamp01(value);
			}
		}

		public float dampRotation
		{
			get
			{
				return this.m_DampRotation;
			}
			set
			{
				this.m_DampRotation = Mathf.Clamp01(value);
			}
		}

		public bool maintainAim
		{
			get
			{
				return this.m_MaintainAim;
			}
			set
			{
				this.m_MaintainAim = value;
			}
		}

		string IDampedTransformData.dampPositionFloatProperty
		{
			get
			{
				return ConstraintsUtils.ConstructConstraintDataPropertyName("m_DampPosition");
			}
		}

		string IDampedTransformData.dampRotationFloatProperty
		{
			get
			{
				return ConstraintsUtils.ConstructConstraintDataPropertyName("m_DampRotation");
			}
		}

		bool IAnimationJobData.IsValid()
		{
			return !(this.m_ConstrainedObject == null) && !(this.m_Source == null);
		}

		void IAnimationJobData.SetDefaultValues()
		{
			this.m_ConstrainedObject = null;
			this.m_Source = null;
			this.m_DampPosition = 0.5f;
			this.m_DampRotation = 0.5f;
			this.m_MaintainAim = true;
		}

		[SerializeField]
		private Transform m_ConstrainedObject;

		[SyncSceneToStream]
		[SerializeField]
		private Transform m_Source;

		[SyncSceneToStream]
		[SerializeField]
		[Range(0f, 1f)]
		private float m_DampPosition;

		[SyncSceneToStream]
		[SerializeField]
		[Range(0f, 1f)]
		private float m_DampRotation;

		[NotKeyable]
		[SerializeField]
		private bool m_MaintainAim;
	}
}
