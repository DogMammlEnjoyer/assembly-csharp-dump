using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public static class MaterialUtils
{
	public static string GetTrimmedMaterialName(Material material)
	{
		return material.name.Replace(" (Instance)", "").Trim();
	}

	public static void SwapMaterial(MeshAndMaterials meshAndMaterial, bool isOnToOff)
	{
		List<Material> list;
		using (ListPool<Material>.Get(out list))
		{
			meshAndMaterial.meshRenderer.GetSharedMaterials(list);
			for (int i = 0; i < list.Count; i++)
			{
				string trimmedMaterialName = MaterialUtils.GetTrimmedMaterialName(list[i]);
				string text = isOnToOff ? ((meshAndMaterial.onMaterial != null) ? MaterialUtils.GetTrimmedMaterialName(meshAndMaterial.onMaterial) : null) : ((meshAndMaterial.offMaterial != null) ? MaterialUtils.GetTrimmedMaterialName(meshAndMaterial.offMaterial) : null);
				if (text != null && trimmedMaterialName == text)
				{
					list[i] = (isOnToOff ? meshAndMaterial.offMaterial : meshAndMaterial.onMaterial);
				}
			}
			meshAndMaterial.meshRenderer.SetSharedMaterials(list);
		}
	}
}
