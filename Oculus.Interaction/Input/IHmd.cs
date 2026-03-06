using System;
using UnityEngine;

namespace Oculus.Interaction.Input
{
	public interface IHmd
	{
		bool TryGetRootPose(out Pose pose);

		event Action WhenUpdated;
	}
}
