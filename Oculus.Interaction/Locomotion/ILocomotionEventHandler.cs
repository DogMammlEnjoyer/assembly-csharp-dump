using System;
using UnityEngine;

namespace Oculus.Interaction.Locomotion
{
	public interface ILocomotionEventHandler
	{
		void HandleLocomotionEvent(LocomotionEvent locomotionEvent);

		event Action<LocomotionEvent, Pose> WhenLocomotionEventHandled;
	}
}
