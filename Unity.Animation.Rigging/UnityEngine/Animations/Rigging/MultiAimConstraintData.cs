using System;
using System.Collections.Generic;

namespace UnityEngine.Animations.Rigging
{
	[Serializable]
	public struct MultiAimConstraintData : IAnimationJobData, IMultiAimConstraintData
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

		public WeightedTransformArray sourceObjects
		{
			get
			{
				return this.m_SourceObjects;
			}
			set
			{
				this.m_SourceObjects = value;
			}
		}

		public bool maintainOffset
		{
			get
			{
				return this.m_MaintainOffset;
			}
			set
			{
				this.m_MaintainOffset = value;
			}
		}

		public Vector3 offset
		{
			get
			{
				return this.m_Offset;
			}
			set
			{
				this.m_Offset = value;
			}
		}

		public Vector2 limits
		{
			get
			{
				return new Vector2(this.m_MinLimit, this.m_MaxLimit);
			}
			set
			{
				this.m_MinLimit = Mathf.Clamp(value.x, -180f, 180f);
				this.m_MaxLimit = Mathf.Clamp(value.y, -180f, 180f);
			}
		}

		public MultiAimConstraintData.Axis aimAxis
		{
			get
			{
				return this.m_AimAxis;
			}
			set
			{
				this.m_AimAxis = value;
			}
		}

		public MultiAimConstraintData.Axis upAxis
		{
			get
			{
				return this.m_UpAxis;
			}
			set
			{
				this.m_UpAxis = value;
			}
		}

		public MultiAimConstraintData.WorldUpType worldUpType
		{
			get
			{
				return this.m_WorldUpType;
			}
			set
			{
				this.m_WorldUpType = value;
			}
		}

		public MultiAimConstraintData.Axis worldUpAxis
		{
			get
			{
				return this.m_WorldUpAxis;
			}
			set
			{
				this.m_WorldUpAxis = value;
			}
		}

		public Transform worldUpObject
		{
			get
			{
				return this.m_WorldUpObject;
			}
			set
			{
				this.m_WorldUpObject = value;
			}
		}

		public bool constrainedXAxis
		{
			get
			{
				return this.m_ConstrainedAxes.x;
			}
			set
			{
				this.m_ConstrainedAxes.x = value;
			}
		}

		public bool constrainedYAxis
		{
			get
			{
				return this.m_ConstrainedAxes.y;
			}
			set
			{
				this.m_ConstrainedAxes.y = value;
			}
		}

		public bool constrainedZAxis
		{
			get
			{
				return this.m_ConstrainedAxes.z;
			}
			set
			{
				this.m_ConstrainedAxes.z = value;
			}
		}

		Vector3 IMultiAimConstraintData.aimAxis
		{
			get
			{
				return MultiAimConstraintData.Convert(this.m_AimAxis);
			}
		}

		Vector3 IMultiAimConstraintData.upAxis
		{
			get
			{
				return MultiAimConstraintData.Convert(this.m_UpAxis);
			}
		}

		int IMultiAimConstraintData.worldUpType
		{
			get
			{
				return (int)this.m_WorldUpType;
			}
		}

		Vector3 IMultiAimConstraintData.worldUpAxis
		{
			get
			{
				return MultiAimConstraintData.Convert(this.m_WorldUpAxis);
			}
		}

		string IMultiAimConstraintData.offsetVector3Property
		{
			get
			{
				return ConstraintsUtils.ConstructConstraintDataPropertyName("m_Offset");
			}
		}

		string IMultiAimConstraintData.minLimitFloatProperty
		{
			get
			{
				return ConstraintsUtils.ConstructConstraintDataPropertyName("m_MinLimit");
			}
		}

		string IMultiAimConstraintData.maxLimitFloatProperty
		{
			get
			{
				return ConstraintsUtils.ConstructConstraintDataPropertyName("m_MaxLimit");
			}
		}

		string IMultiAimConstraintData.sourceObjectsProperty
		{
			get
			{
				return ConstraintsUtils.ConstructConstraintDataPropertyName("m_SourceObjects");
			}
		}

		bool IAnimationJobData.IsValid()
		{
			if (this.m_ConstrainedObject == null || this.m_SourceObjects.Count == 0)
			{
				return false;
			}
			using (IEnumerator<WeightedTransform> enumerator = this.m_SourceObjects.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					if (enumerator.Current.transform == null)
					{
						return false;
					}
				}
			}
			return true;
		}

		void IAnimationJobData.SetDefaultValues()
		{
			this.m_ConstrainedObject = null;
			this.m_UpAxis = MultiAimConstraintData.Axis.Y;
			this.m_AimAxis = MultiAimConstraintData.Axis.Z;
			this.m_WorldUpType = MultiAimConstraintData.WorldUpType.None;
			this.m_WorldUpAxis = MultiAimConstraintData.Axis.Y;
			this.m_WorldUpObject = null;
			this.m_SourceObjects.Clear();
			this.m_MaintainOffset = false;
			this.m_Offset = Vector3.zero;
			this.m_ConstrainedAxes = new Vector3Bool(true);
			this.m_MinLimit = -180f;
			this.m_MaxLimit = 180f;
		}

		private static Vector3 Convert(MultiAimConstraintData.Axis axis)
		{
			switch (axis)
			{
			case MultiAimConstraintData.Axis.X:
				return Vector3.right;
			case MultiAimConstraintData.Axis.X_NEG:
				return Vector3.left;
			case MultiAimConstraintData.Axis.Y:
				return Vector3.up;
			case MultiAimConstraintData.Axis.Y_NEG:
				return Vector3.down;
			case MultiAimConstraintData.Axis.Z:
				return Vector3.forward;
			case MultiAimConstraintData.Axis.Z_NEG:
				return Vector3.back;
			default:
				return Vector3.up;
			}
		}

		internal const float k_MinAngularLimit = -180f;

		internal const float k_MaxAngularLimit = 180f;

		[SerializeField]
		private Transform m_ConstrainedObject;

		[SyncSceneToStream]
		[SerializeField]
		[WeightRange(0f, 1f)]
		private WeightedTransformArray m_SourceObjects;

		[SyncSceneToStream]
		[SerializeField]
		private Vector3 m_Offset;

		[SyncSceneToStream]
		[SerializeField]
		[Range(-180f, 180f)]
		private float m_MinLimit;

		[SyncSceneToStream]
		[SerializeField]
		[Range(-180f, 180f)]
		private float m_MaxLimit;

		[NotKeyable]
		[SerializeField]
		private MultiAimConstraintData.Axis m_AimAxis;

		[NotKeyable]
		[SerializeField]
		private MultiAimConstraintData.Axis m_UpAxis;

		[NotKeyable]
		[SerializeField]
		private MultiAimConstraintData.WorldUpType m_WorldUpType;

		[SyncSceneToStream]
		[SerializeField]
		private Transform m_WorldUpObject;

		[NotKeyable]
		[SerializeField]
		private MultiAimConstraintData.Axis m_WorldUpAxis;

		[NotKeyable]
		[SerializeField]
		private bool m_MaintainOffset;

		[NotKeyable]
		[SerializeField]
		private Vector3Bool m_ConstrainedAxes;

		public enum Axis
		{
			X,
			X_NEG,
			Y,
			Y_NEG,
			Z,
			Z_NEG
		}

		public enum WorldUpType
		{
			None,
			SceneUp,
			ObjectUp,
			ObjectRotationUp,
			Vector
		}
	}
}
