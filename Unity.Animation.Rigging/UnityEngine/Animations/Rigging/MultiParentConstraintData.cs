using System;
using System.Collections.Generic;

namespace UnityEngine.Animations.Rigging
{
	[Serializable]
	public struct MultiParentConstraintData : IAnimationJobData, IMultiParentConstraintData
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

		public bool maintainPositionOffset
		{
			get
			{
				return this.m_MaintainPositionOffset;
			}
			set
			{
				this.m_MaintainPositionOffset = value;
			}
		}

		public bool maintainRotationOffset
		{
			get
			{
				return this.m_MaintainRotationOffset;
			}
			set
			{
				this.m_MaintainRotationOffset = value;
			}
		}

		public bool constrainedPositionXAxis
		{
			get
			{
				return this.m_ConstrainedPositionAxes.x;
			}
			set
			{
				this.m_ConstrainedPositionAxes.x = value;
			}
		}

		public bool constrainedPositionYAxis
		{
			get
			{
				return this.m_ConstrainedPositionAxes.y;
			}
			set
			{
				this.m_ConstrainedPositionAxes.y = value;
			}
		}

		public bool constrainedPositionZAxis
		{
			get
			{
				return this.m_ConstrainedPositionAxes.z;
			}
			set
			{
				this.m_ConstrainedPositionAxes.z = value;
			}
		}

		public bool constrainedRotationXAxis
		{
			get
			{
				return this.m_ConstrainedRotationAxes.x;
			}
			set
			{
				this.m_ConstrainedRotationAxes.x = value;
			}
		}

		public bool constrainedRotationYAxis
		{
			get
			{
				return this.m_ConstrainedRotationAxes.y;
			}
			set
			{
				this.m_ConstrainedRotationAxes.y = value;
			}
		}

		public bool constrainedRotationZAxis
		{
			get
			{
				return this.m_ConstrainedRotationAxes.z;
			}
			set
			{
				this.m_ConstrainedRotationAxes.z = value;
			}
		}

		string IMultiParentConstraintData.sourceObjectsProperty
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
			this.m_ConstrainedPositionAxes = new Vector3Bool(true);
			this.m_ConstrainedRotationAxes = new Vector3Bool(true);
			this.m_SourceObjects.Clear();
			this.m_MaintainPositionOffset = false;
			this.m_MaintainRotationOffset = false;
		}

		[SerializeField]
		private Transform m_ConstrainedObject;

		[SerializeField]
		[SyncSceneToStream]
		[WeightRange(0f, 1f)]
		private WeightedTransformArray m_SourceObjects;

		[NotKeyable]
		[SerializeField]
		private Vector3Bool m_ConstrainedPositionAxes;

		[NotKeyable]
		[SerializeField]
		private Vector3Bool m_ConstrainedRotationAxes;

		[NotKeyable]
		[SerializeField]
		private bool m_MaintainPositionOffset;

		[NotKeyable]
		[SerializeField]
		private bool m_MaintainRotationOffset;
	}
}
