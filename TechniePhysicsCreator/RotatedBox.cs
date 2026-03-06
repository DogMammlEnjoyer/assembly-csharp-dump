using System;
using UnityEngine;

namespace Technie.PhysicsCreator
{
	public class RotatedBox
	{
		public float VolumeCm3
		{
			get
			{
				return this.volume * 1000000f;
			}
		}

		public RotatedBox(ConstructionPlane p, Vector3 localCenter, Vector3 c, Vector3 s)
		{
			this.plane = p;
			this.localCenter = localCenter;
			this.center = c;
			this.size = s;
			this.volume = this.size.x * this.size.y * this.size.z;
		}

		public void DrawWireframe()
		{
			Gizmos.matrix = Matrix4x4.TRS(this.center, this.plane.rotation, Vector3.one);
			Gizmos.DrawWireCube(Vector3.zero, this.size);
			Gizmos.matrix = Matrix4x4.identity;
		}

		public ConstructionPlane plane;

		public Vector3 localCenter;

		public Vector3 center;

		public Vector3 size;

		public float volume;
	}
}
