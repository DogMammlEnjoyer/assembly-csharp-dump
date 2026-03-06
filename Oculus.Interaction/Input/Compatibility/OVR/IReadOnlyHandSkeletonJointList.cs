using System;

namespace Oculus.Interaction.Input.Compatibility.OVR
{
	public interface IReadOnlyHandSkeletonJointList
	{
		HandSkeletonJoint this[int jointId]
		{
			get;
		}
	}
}
