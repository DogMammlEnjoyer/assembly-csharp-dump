using System;
using UnityEngine.Events;
using UnityEngine.Scripting.APIUpdating;

namespace UnityEngine.XR.Interaction.Toolkit.Locomotion.Teleportation
{
	[MovedFrom("UnityEngine.XR.Interaction.Toolkit")]
	[Serializable]
	public sealed class TeleportingEvent : UnityEvent<TeleportingEventArgs>
	{
	}
}
