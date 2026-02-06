using System;

namespace GorillaNetworking
{
	[Serializable]
	internal struct GorillaRigHelper : IComparable
	{
		public int CompareTo(object obj)
		{
			return this.sqrDistance.CompareTo(((GorillaRigHelper)obj).sqrDistance);
		}

		public VRRig rig;

		public CosmeticsThrottler.RigDrawState state;

		public float sqrDistance;

		public float prevSqrDistance;
	}
}
