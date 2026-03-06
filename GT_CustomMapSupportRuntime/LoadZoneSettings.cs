using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace GT_CustomMapSupportRuntime
{
	[NullableContext(1)]
	[Nullable(0)]
	[RequireComponent(typeof(BoxCollider))]
	[DisallowMultipleComponent]
	public class LoadZoneSettings : MonoBehaviour
	{
		public bool useDynamicLighting;

		public Color UberShaderAmbientDynamicLight = Color.black;

		public List<string> scenesToLoad = new List<string>();

		public List<string> scenesToUnload = new List<string>();
	}
}
