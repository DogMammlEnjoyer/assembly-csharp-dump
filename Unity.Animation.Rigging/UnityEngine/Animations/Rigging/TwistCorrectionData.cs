using System;

namespace UnityEngine.Animations.Rigging
{
	[Serializable]
	public struct TwistCorrectionData : IAnimationJobData, ITwistCorrectionData
	{
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

		public WeightedTransformArray twistNodes
		{
			get
			{
				return this.m_TwistNodes;
			}
			set
			{
				this.m_TwistNodes = value;
			}
		}

		public TwistCorrectionData.Axis twistAxis
		{
			get
			{
				return this.m_TwistAxis;
			}
			set
			{
				this.m_TwistAxis = value;
			}
		}

		Transform ITwistCorrectionData.source
		{
			get
			{
				return this.m_Source;
			}
		}

		Vector3 ITwistCorrectionData.twistAxis
		{
			get
			{
				return TwistCorrectionData.Convert(this.m_TwistAxis);
			}
		}

		string ITwistCorrectionData.twistNodesProperty
		{
			get
			{
				return ConstraintsUtils.ConstructConstraintDataPropertyName("m_TwistNodes");
			}
		}

		private static Vector3 Convert(TwistCorrectionData.Axis axis)
		{
			if (axis == TwistCorrectionData.Axis.X)
			{
				return Vector3.right;
			}
			if (axis == TwistCorrectionData.Axis.Y)
			{
				return Vector3.up;
			}
			return Vector3.forward;
		}

		bool IAnimationJobData.IsValid()
		{
			if (this.m_Source == null)
			{
				return false;
			}
			for (int i = 0; i < this.m_TwistNodes.Count; i++)
			{
				if (this.m_TwistNodes[i].transform == null)
				{
					return false;
				}
			}
			return true;
		}

		void IAnimationJobData.SetDefaultValues()
		{
			this.m_Source = null;
			this.m_TwistAxis = TwistCorrectionData.Axis.Z;
			this.m_TwistNodes.Clear();
		}

		[SyncSceneToStream]
		[SerializeField]
		private Transform m_Source;

		[NotKeyable]
		[SerializeField]
		private TwistCorrectionData.Axis m_TwistAxis;

		[SyncSceneToStream]
		[SerializeField]
		[WeightRange(-1f, 1f)]
		private WeightedTransformArray m_TwistNodes;

		public enum Axis
		{
			X,
			Y,
			Z
		}
	}
}
