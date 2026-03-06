using System;

namespace UnityEngine.XR.Interaction.Toolkit.Locomotion
{
	public static class LocomotionStateExtensions
	{
		public static bool IsActive(this LocomotionState state)
		{
			return state == LocomotionState.Preparing || state == LocomotionState.Moving;
		}
	}
}
