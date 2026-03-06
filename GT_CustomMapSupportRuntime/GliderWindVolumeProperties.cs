using System;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace GT_CustomMapSupportRuntime
{
	public struct GliderWindVolumeProperties
	{
		public float maxSpeed;

		public float maxAccel;

		[Nullable(1)]
		public AnimationCurve speedVsAccelCurve;

		public Vector3 localWindDirection;
	}
}
