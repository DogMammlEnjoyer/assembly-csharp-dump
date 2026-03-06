using System;
using System.Collections.Generic;

namespace UnityEngine.ProBuilder
{
	internal static class MaterialUtility
	{
		internal static int GetMaterialCount(Renderer renderer)
		{
			MaterialUtility.s_MaterialArray.Clear();
			renderer.GetSharedMaterials(MaterialUtility.s_MaterialArray);
			return MaterialUtility.s_MaterialArray.Count;
		}

		internal static Material GetSharedMaterial(Renderer renderer, int index)
		{
			MaterialUtility.s_MaterialArray.Clear();
			renderer.GetSharedMaterials(MaterialUtility.s_MaterialArray);
			int count = MaterialUtility.s_MaterialArray.Count;
			if (count < 1)
			{
				return null;
			}
			return MaterialUtility.s_MaterialArray[Math.Clamp(index, 0, count - 1)];
		}

		internal static List<Material> s_MaterialArray = new List<Material>();
	}
}
