using System;
using UnityEngine.Events;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

namespace UnityEngine.XR.Interaction.Toolkit
{
	[Obsolete("XRInteractableEvent has been deprecated. Use events specific to each state change instead.", true)]
	[Serializable]
	public class XRInteractableEvent : UnityEvent<XRBaseInteractor>
	{
	}
}
