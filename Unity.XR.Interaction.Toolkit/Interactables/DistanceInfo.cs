using System;
using UnityEngine.Scripting.APIUpdating;

namespace UnityEngine.XR.Interaction.Toolkit.Interactables
{
	[MovedFrom("UnityEngine.XR.Interaction.Toolkit")]
	public struct DistanceInfo
	{
		public Vector3 point { readonly get; set; }

		public float distanceSqr { readonly get; set; }

		public Collider collider { readonly get; set; }
	}
}
