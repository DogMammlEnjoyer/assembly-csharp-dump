using System;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace GT_CustomMapSupportRuntime
{
	[NullableContext(2)]
	[Nullable(0)]
	public struct ForceVolumeProperties
	{
		public float accel;

		public float maxDepth;

		public float maxSpeed;

		public bool disableGrip;

		public bool dampenLateralVelocity;

		public float dampenXVel;

		public float dampenZVel;

		public bool applyPullToCenterAcceleration;

		public float pullToCenterAccel;

		public float pullToCenterMaxSpeed;

		public float pullToCenterMinDistance;

		public AudioClip enterClip;

		public AudioClip exitClip;

		public AudioClip loopClip;

		public AudioClip loopCrescendoClip;
	}
}
