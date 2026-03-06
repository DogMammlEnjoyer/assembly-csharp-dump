using System;
using UnityEngine;

namespace Oculus.Interaction.Grab.GrabSurfaces
{
	[Serializable]
	public class SphereGrabSurfaceData : ICloneable
	{
		public object Clone()
		{
			return new SphereGrabSurfaceData
			{
				centre = this.centre
			};
		}

		public SphereGrabSurfaceData Mirror()
		{
			return this.Clone() as SphereGrabSurfaceData;
		}

		public Vector3 centre = Vector3.zero;
	}
}
