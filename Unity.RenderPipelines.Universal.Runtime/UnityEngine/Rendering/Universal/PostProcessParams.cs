using System;
using UnityEngine.Experimental.Rendering;

namespace UnityEngine.Rendering.Universal
{
	internal struct PostProcessParams
	{
		public static PostProcessParams Create()
		{
			PostProcessParams result;
			result.blitMaterial = null;
			result.requestColorFormat = GraphicsFormat.None;
			return result;
		}

		public Material blitMaterial;

		public GraphicsFormat requestColorFormat;
	}
}
