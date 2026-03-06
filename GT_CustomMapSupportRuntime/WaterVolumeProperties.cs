using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace GT_CustomMapSupportRuntime
{
	public struct WaterVolumeProperties
	{
		[Nullable(2)]
		public Transform surfacePlane;

		[Nullable(1)]
		public List<MeshCollider> surfaceColliders;

		public CMSZoneShaderSettings.EZoneLiquidType liquidType;
	}
}
