using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using DigitalOpus.MB.Core;
using UnityEngine;

public class MB2_TextureBakeResults : ScriptableObject
{
	public static int VERSION
	{
		get
		{
			return 3252;
		}
	}

	public MB2_TextureBakeResults()
	{
		this.version = MB2_TextureBakeResults.VERSION;
	}

	private void OnEnable()
	{
		if (this.version < 3251)
		{
			for (int i = 0; i < this.materialsAndUVRects.Length; i++)
			{
				this.materialsAndUVRects[i].allPropsUseSameTiling = true;
			}
		}
		this.version = MB2_TextureBakeResults.VERSION;
	}

	public int NumResultMaterials()
	{
		if (this.resultType == MB2_TextureBakeResults.ResultType.atlas)
		{
			return this.resultMaterials.Length;
		}
		return this.resultMaterialsTexArray.Length;
	}

	public Material GetCombinedMaterialForSubmesh(int idx)
	{
		if (this.resultType == MB2_TextureBakeResults.ResultType.atlas)
		{
			return this.resultMaterials[idx].combinedMaterial;
		}
		return this.resultMaterialsTexArray[idx].combinedMaterial;
	}

	public IEnumerator FindRuntimeMaterialsFromAddresses(MB2_TextureBakeResults.CoroutineResult isComplete)
	{
		yield return MBVersion.FindRuntimeMaterialsFromAddresses(this, isComplete);
		isComplete.isComplete = true;
		yield break;
	}

	public bool GetConsiderMeshUVs(int idxInSrcMats, Material srcMaterial)
	{
		if (this.resultType == MB2_TextureBakeResults.ResultType.atlas)
		{
			return this.resultMaterials[idxInSrcMats].considerMeshUVs;
		}
		List<MB_TexArraySlice> slices = this.resultMaterialsTexArray[idxInSrcMats].slices;
		for (int i = 0; i < slices.Count; i++)
		{
			if (slices[i].ContainsMaterial(srcMaterial))
			{
				return slices[i].considerMeshUVs;
			}
		}
		Debug.LogError("There were no source materials for any slice in this result material.");
		return false;
	}

	public List<Material> GetSourceMaterialsUsedByResultMaterial(int resultMatIdx)
	{
		if (this.resultType == MB2_TextureBakeResults.ResultType.atlas)
		{
			return this.resultMaterials[resultMatIdx].sourceMaterials;
		}
		HashSet<Material> hashSet = new HashSet<Material>();
		List<MB_TexArraySlice> slices = this.resultMaterialsTexArray[resultMatIdx].slices;
		for (int i = 0; i < slices.Count; i++)
		{
			List<Material> list = new List<Material>();
			slices[i].GetAllUsedMaterials(list);
			for (int j = 0; j < list.Count; j++)
			{
				hashSet.Add(list[j]);
			}
		}
		return new List<Material>(hashSet);
	}

	public static MB2_TextureBakeResults CreateForMaterialsOnRenderer(GameObject[] gos, List<Material> matsOnTargetRenderer)
	{
		HashSet<Material> hashSet = new HashSet<Material>(matsOnTargetRenderer);
		for (int i = 0; i < gos.Length; i++)
		{
			if (gos[i] == null)
			{
				Debug.LogError(string.Format("Game object {0} in list of objects to add was null", i));
				return null;
			}
			Material[] gomaterials = MB_Utility.GetGOMaterials(gos[i]);
			if (gomaterials.Length == 0)
			{
				Debug.LogError(string.Format("Game object {0} in list of objects to add no renderer", i));
				return null;
			}
			for (int j = 0; j < gomaterials.Length; j++)
			{
				if (!hashSet.Contains(gomaterials[j]))
				{
					hashSet.Add(gomaterials[j]);
				}
			}
		}
		Material[] array = new Material[hashSet.Count];
		hashSet.CopyTo(array);
		MB2_TextureBakeResults mb2_TextureBakeResults = (MB2_TextureBakeResults)ScriptableObject.CreateInstance(typeof(MB2_TextureBakeResults));
		List<MB_MaterialAndUVRect> list = new List<MB_MaterialAndUVRect>();
		for (int k = 0; k < array.Length; k++)
		{
			if (array[k] != null)
			{
				MB_MaterialAndUVRect item = new MB_MaterialAndUVRect(array[k], new Rect(0f, 0f, 1f, 1f), true, new Rect(0f, 0f, 1f, 1f), new Rect(0f, 0f, 1f, 1f), new Rect(0f, 0f, 0f, 0f), MB_TextureTilingTreatment.none, "");
				if (!list.Contains(item))
				{
					list.Add(item);
				}
			}
		}
		mb2_TextureBakeResults.resultMaterials = new MB_MultiMaterial[list.Count];
		for (int l = 0; l < list.Count; l++)
		{
			mb2_TextureBakeResults.resultMaterials[l] = new MB_MultiMaterial();
			List<Material> list2 = new List<Material>();
			list2.Add(list[l].material);
			mb2_TextureBakeResults.resultMaterials[l].sourceMaterials = list2;
			mb2_TextureBakeResults.resultMaterials[l].combinedMaterial = list[l].material;
			mb2_TextureBakeResults.resultMaterials[l].considerMeshUVs = false;
		}
		if (array.Length == 1)
		{
			mb2_TextureBakeResults.doMultiMaterial = false;
		}
		else
		{
			mb2_TextureBakeResults.doMultiMaterial = true;
		}
		mb2_TextureBakeResults.materialsAndUVRects = list.ToArray();
		return mb2_TextureBakeResults;
	}

	public bool DoAnyResultMatsUseConsiderMeshUVs()
	{
		if (this.resultType == MB2_TextureBakeResults.ResultType.atlas)
		{
			if (this.resultMaterials == null)
			{
				return false;
			}
			for (int i = 0; i < this.resultMaterials.Length; i++)
			{
				if (this.resultMaterials[i].considerMeshUVs)
				{
					return true;
				}
			}
			return false;
		}
		else
		{
			if (this.resultMaterialsTexArray == null)
			{
				return false;
			}
			for (int j = 0; j < this.resultMaterialsTexArray.Length; j++)
			{
				MB_MultiMaterialTexArray mb_MultiMaterialTexArray = this.resultMaterialsTexArray[j];
				for (int k = 0; k < mb_MultiMaterialTexArray.slices.Count; k++)
				{
					if (mb_MultiMaterialTexArray.slices[k].considerMeshUVs)
					{
						return true;
					}
				}
			}
			return false;
		}
	}

	public bool ContainsMaterial(Material m)
	{
		for (int i = 0; i < this.materialsAndUVRects.Length; i++)
		{
			if (this.materialsAndUVRects[i].material == m)
			{
				return true;
			}
		}
		return false;
	}

	public string GetDescription()
	{
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.Append("Shaders:\n");
		HashSet<Shader> hashSet = new HashSet<Shader>();
		if (this.materialsAndUVRects != null)
		{
			for (int i = 0; i < this.materialsAndUVRects.Length; i++)
			{
				if (this.materialsAndUVRects[i].material != null)
				{
					hashSet.Add(this.materialsAndUVRects[i].material.shader);
				}
			}
		}
		foreach (Shader shader in hashSet)
		{
			stringBuilder.Append("  ").Append(shader.name).AppendLine();
		}
		stringBuilder.Append("Materials:\n");
		if (this.materialsAndUVRects != null)
		{
			for (int j = 0; j < this.materialsAndUVRects.Length; j++)
			{
				if (this.materialsAndUVRects[j].material != null)
				{
					stringBuilder.Append("  ").Append(this.materialsAndUVRects[j].material.name).AppendLine();
				}
			}
		}
		return stringBuilder.ToString();
	}

	public void UpgradeToCurrentVersion(MB2_TextureBakeResults tbr)
	{
		if (tbr.version < 3252)
		{
			for (int i = 0; i < tbr.materialsAndUVRects.Length; i++)
			{
				tbr.materialsAndUVRects[i].allPropsUseSameTiling = true;
			}
		}
	}

	public static bool IsMeshAndMaterialRectEnclosedByAtlasRect(MB_TextureTilingTreatment tilingTreatment, Rect uvR, Rect sourceMaterialTiling, Rect samplingEncapsulatinRect, MB2_LogLevel logLevel)
	{
		Rect rect = default(Rect);
		rect = MB3_UVTransformUtility.CombineTransforms(ref uvR, ref sourceMaterialTiling);
		if (logLevel >= MB2_LogLevel.trace && logLevel >= MB2_LogLevel.trace)
		{
			Debug.Log(string.Concat(new string[]
			{
				"IsMeshAndMaterialRectEnclosedByAtlasRect Rect in atlas uvR=",
				uvR.ToString("f5"),
				" sourceMaterialTiling=",
				sourceMaterialTiling.ToString("f5"),
				"Potential Rect (must fit in encapsulating) ",
				rect.ToString("f5"),
				" encapsulating=",
				samplingEncapsulatinRect.ToString("f5"),
				" tilingTreatment=",
				tilingTreatment.ToString()
			}));
		}
		if (tilingTreatment == MB_TextureTilingTreatment.edgeToEdgeX)
		{
			if (MB3_UVTransformUtility.LineSegmentContainsShifted(samplingEncapsulatinRect.y, samplingEncapsulatinRect.height, rect.y, rect.height))
			{
				return true;
			}
		}
		else if (tilingTreatment == MB_TextureTilingTreatment.edgeToEdgeY)
		{
			if (MB3_UVTransformUtility.LineSegmentContainsShifted(samplingEncapsulatinRect.x, samplingEncapsulatinRect.width, rect.x, rect.width))
			{
				return true;
			}
		}
		else
		{
			if (tilingTreatment == MB_TextureTilingTreatment.edgeToEdgeXY)
			{
				return true;
			}
			if (MB3_UVTransformUtility.RectContainsShifted(ref samplingEncapsulatinRect, ref rect))
			{
				return true;
			}
		}
		return false;
	}

	public int version;

	public MB2_TextureBakeResults.ResultType resultType;

	[NonReorderable]
	public MB_MaterialAndUVRect[] materialsAndUVRects;

	[NonReorderable]
	public MB_MultiMaterial[] resultMaterials;

	[NonReorderable]
	public MB_MultiMaterialTexArray[] resultMaterialsTexArray;

	public bool doMultiMaterial;

	public class CoroutineResult
	{
		public bool isComplete;
	}

	public enum ResultType
	{
		atlas,
		textureArray
	}
}
