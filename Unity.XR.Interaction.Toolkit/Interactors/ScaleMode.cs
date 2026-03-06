using System;
using UnityEngine.Scripting.APIUpdating;

namespace UnityEngine.XR.Interaction.Toolkit.Interactors
{
	[MovedFrom("UnityEngine.XR.Interaction.Toolkit")]
	public enum ScaleMode
	{
		None,
		ScaleOverTime,
		[Obsolete("Input has been renamed in version 3.0.0. Use ScaleOverTime instead. (UnityUpgradable) -> ScaleOverTime")]
		Input = 1,
		DistanceDelta,
		[Obsolete("Distance has been renamed in version 3.0.0. Use DistanceDelta instead. (UnityUpgradable) -> DistanceDelta")]
		Distance = 2
	}
}
