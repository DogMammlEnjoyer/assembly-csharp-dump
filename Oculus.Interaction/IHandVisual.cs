using System;
using Oculus.Interaction.Input;
using UnityEngine;

namespace Oculus.Interaction
{
	public interface IHandVisual
	{
		IHand Hand { get; }

		bool IsVisible { get; }

		bool ForceOffVisibility { get; set; }

		Pose GetJointPose(HandJointId jointId, Space space);

		event Action WhenHandVisualUpdated;
	}
}
