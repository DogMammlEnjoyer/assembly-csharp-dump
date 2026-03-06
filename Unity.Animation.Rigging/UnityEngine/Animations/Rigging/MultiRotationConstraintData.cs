using System;
using System.Collections.Generic;

namespace UnityEngine.Animations.Rigging
{
	[Serializable]
	public struct MultiRotationConstraintData : IAnimationJobData, IMultiRotationConstraintData
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

		string IMultiRotationConstraintData.offsetVector3Property
		{
			get
			{
				return ConstraintsUtils.ConstructConstraintDataPropertyName("m_Offset");
			}
		}

		string IMultiRotationConstraintData.sourceObjectsProperty
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
			this.m_ConstrainedAxes = new Vector3Bool(true);
			this.m_SourceObjects.Clear();
			this.m_MaintainOffset = false;
			this.m_Offset = Vector3.zero;
		}

		[SerializeField]
		private Transform m_ConstrainedObject;

		[SyncSceneToStream]
		[SerializeField]
		[WeightRange(0f, 1f)]
		private WeightedTransformArray m_SourceObjects;

		[SyncSceneToStream]
		[SerializeField]
		private Vector3 m_Offset;

		[NotKeyable]
		[SerializeField]
		private Vector3Bool m_ConstrainedAxes;

		[NotKeyable]
		[SerializeField]
		private bool m_MaintainOffset;
	}
}
