using System;
using UnityEngine.Scripting.APIUpdating;

namespace UnityEngine.XR.Interaction.Toolkit.Interactors
{
	[MovedFrom("UnityEngine.XR.Interaction.Toolkit")]
	public interface IXRRayProvider
	{
		Transform GetOrCreateRayOrigin();

		Transform GetOrCreateAttachTransform();

		void SetRayOrigin(Transform newOrigin);

		void SetAttachTransform(Transform newAttach);

		Vector3 rayEndPoint { get; }

		Transform rayEndTransform { get; }
	}
}
