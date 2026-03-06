using System;

namespace Unity.Cinemachine
{
	public sealed class RequiredTargetAttribute : Attribute
	{
		public RequiredTargetAttribute.RequiredTargets RequiredTarget { get; private set; }

		public RequiredTargetAttribute(RequiredTargetAttribute.RequiredTargets requiredTarget)
		{
			this.RequiredTarget = requiredTarget;
		}

		public enum RequiredTargets
		{
			None,
			Tracking,
			LookAt,
			GroupLookAt
		}
	}
}
