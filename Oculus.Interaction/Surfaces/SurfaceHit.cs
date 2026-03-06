using System;
using UnityEngine;

namespace Oculus.Interaction.Surfaces
{
	public struct SurfaceHit
	{
		public Vector3 Point { readonly get; set; }

		public Vector3 Normal { readonly get; set; }

		public float Distance { readonly get; set; }
	}
}
