using System;
using UnityEngine;

namespace Oculus.Interaction.Grab.GrabSurfaces
{
	[Serializable]
	public class BoxGrabSurfaceData : ICloneable
	{
		public object Clone()
		{
			return new BoxGrabSurfaceData
			{
				widthOffset = this.widthOffset,
				snapOffset = this.snapOffset,
				size = this.size,
				eulerAngles = this.eulerAngles
			};
		}

		public BoxGrabSurfaceData Mirror()
		{
			BoxGrabSurfaceData boxGrabSurfaceData = this.Clone() as BoxGrabSurfaceData;
			boxGrabSurfaceData.snapOffset = new Vector4(-boxGrabSurfaceData.snapOffset.y, -boxGrabSurfaceData.snapOffset.x, -boxGrabSurfaceData.snapOffset.w, -boxGrabSurfaceData.snapOffset.z);
			return boxGrabSurfaceData;
		}

		[Range(0f, 1f)]
		public float widthOffset = 0.5f;

		public Vector4 snapOffset;

		public Vector3 size = new Vector3(0.1f, 0f, 0.1f);

		public Vector3 eulerAngles;
	}
}
