using System;
using UnityEngine;

[CreateAssetMenu(menuName = "Bakery lightmap group")]
public class BakeryLightmapGroup : ScriptableObject
{
	public BakeryLightmapGroupPlain GetPlainStruct()
	{
		BakeryLightmapGroupPlain result;
		result.name = base.name;
		result.id = this.id;
		result.resolution = this.resolution;
		result.vertexBake = (this.mode == BakeryLightmapGroup.ftLMGroupMode.Vertex);
		result.isImplicit = this.isImplicit;
		result.renderMode = (int)this.renderMode;
		result.renderDirMode = (int)this.renderDirMode;
		result.atlasPacker = (int)this.atlasPacker;
		result.computeSSS = this.computeSSS;
		result.sssSamples = this.sssSamples;
		result.sssDensity = this.sssDensity;
		result.sssR = this.sssColor.r * this.sssScale;
		result.sssG = this.sssColor.g * this.sssScale;
		result.sssB = this.sssColor.b * this.sssScale;
		result.containsTerrains = this.containsTerrains;
		result.probes = this.probes;
		result.fakeShadowBias = this.fakeShadowBias;
		result.transparentSelfShadow = this.transparentSelfShadow;
		result.flipNormal = this.flipNormal;
		result.parentName = this.parentName;
		result.sceneLodLevel = this.sceneLodLevel;
		return result;
	}

	[SerializeField]
	[Range(1f, 8192f)]
	public int resolution = 512;

	[SerializeField]
	public int bitmask = 1;

	[SerializeField]
	public int id = -1;

	public int sortingID = -1;

	[SerializeField]
	public bool isImplicit;

	[SerializeField]
	public float area;

	[SerializeField]
	public int totalVertexCount;

	[SerializeField]
	public int vertexCounter;

	[SerializeField]
	public int sceneLodLevel = -1;

	[SerializeField]
	public string sceneName;

	[SerializeField]
	public bool containsTerrains;

	[SerializeField]
	public bool probes;

	[SerializeField]
	public BakeryLightmapGroup.ftLMGroupMode mode = BakeryLightmapGroup.ftLMGroupMode.PackAtlas;

	[SerializeField]
	public BakeryLightmapGroup.RenderMode renderMode = BakeryLightmapGroup.RenderMode.Auto;

	[SerializeField]
	public BakeryLightmapGroup.RenderDirMode renderDirMode = BakeryLightmapGroup.RenderDirMode.Auto;

	[SerializeField]
	public BakeryLightmapGroup.AtlasPacker atlasPacker = BakeryLightmapGroup.AtlasPacker.Auto;

	[SerializeField]
	public bool computeSSS;

	[SerializeField]
	public int sssSamples = 16;

	[SerializeField]
	public float sssDensity = 10f;

	[SerializeField]
	public Color sssColor = Color.white;

	[SerializeField]
	public float sssScale = 1f;

	[SerializeField]
	public float fakeShadowBias;

	[SerializeField]
	public bool transparentSelfShadow;

	[SerializeField]
	public bool flipNormal;

	[SerializeField]
	public string parentName;

	[SerializeField]
	public string overridePath = "";

	[SerializeField]
	public bool fixPos3D;

	[SerializeField]
	public Vector3 voxelSize = Vector3.one;

	public int passedFilter;

	public enum ftLMGroupMode
	{
		OriginalUV,
		PackAtlas,
		Vertex
	}

	public enum RenderMode
	{
		FullLighting,
		Indirect,
		Shadowmask,
		Subtractive,
		AmbientOcclusionOnly,
		Auto = 1000
	}

	public enum RenderDirMode
	{
		None,
		BakedNormalMaps,
		DominantDirection,
		RNM,
		SH,
		ProbeSH,
		Auto = 1000
	}

	public enum AtlasPacker
	{
		Default,
		xatlas,
		Auto = 1000
	}
}
