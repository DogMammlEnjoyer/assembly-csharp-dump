using System;

namespace Oculus.Interaction.Input
{
	public interface IReadOnlyHandSkeletonJointList
	{
		HandSkeletonJoint this[int jointId]
		{
			get;
		}
	}
}
