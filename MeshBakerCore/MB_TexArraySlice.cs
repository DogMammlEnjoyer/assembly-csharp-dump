using System;
using System.Collections.Generic;
using DigitalOpus.MB.Core;
using UnityEngine;

[Serializable]
public class MB_TexArraySlice
{
	public bool ContainsMaterial(Material mat)
	{
		for (int i = 0; i < this.sourceMaterials.Count; i++)
		{
			if (this.sourceMaterials[i].sourceMaterial == mat)
			{
				return true;
			}
		}
		return false;
	}

	public HashSet<Material> GetDistinctMaterials()
	{
		HashSet<Material> hashSet = new HashSet<Material>();
		if (this.sourceMaterials == null)
		{
			return hashSet;
		}
		for (int i = 0; i < this.sourceMaterials.Count; i++)
		{
			hashSet.Add(this.sourceMaterials[i].sourceMaterial);
		}
		return hashSet;
	}

	public bool ContainsMaterialAndMesh(Material mat, Mesh mesh)
	{
		for (int i = 0; i < this.sourceMaterials.Count; i++)
		{
			if (this.sourceMaterials[i].sourceMaterial == mat && MB_Utility.GetMesh(this.sourceMaterials[i].renderer) == mesh)
			{
				return true;
			}
		}
		return false;
	}

	public List<Material> GetAllUsedMaterials(List<Material> usedMats)
	{
		usedMats.Clear();
		for (int i = 0; i < this.sourceMaterials.Count; i++)
		{
			usedMats.Add(this.sourceMaterials[i].sourceMaterial);
		}
		return usedMats;
	}

	public List<GameObject> GetAllUsedRenderers(List<GameObject> allObjsFromTextureBaker)
	{
		if (this.considerMeshUVs)
		{
			List<GameObject> list = new List<GameObject>();
			for (int i = 0; i < this.sourceMaterials.Count; i++)
			{
				list.Add(this.sourceMaterials[i].renderer);
			}
			return list;
		}
		return allObjsFromTextureBaker;
	}

	public bool considerMeshUVs;

	[NonReorderable]
	public List<MB_TexArraySliceRendererMatPair> sourceMaterials = new List<MB_TexArraySliceRendererMatPair>();
}
