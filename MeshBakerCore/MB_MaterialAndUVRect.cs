using System;
using System.Collections.Generic;
using DigitalOpus.MB.Core;
using UnityEngine;
using UnityEngine.Serialization;

[Serializable]
public class MB_MaterialAndUVRect
{
	public MB_MaterialAndUVRect(Material mat, Rect destRect, bool allPropsUseSameTiling, Rect sourceMaterialTiling, Rect samplingEncapsulatingRect, Rect srcUVsamplingRect, MB_TextureTilingTreatment treatment, string objName)
	{
		this.material = mat;
		this.atlasRect = destRect;
		this.tilingTreatment = treatment;
		this.allPropsUseSameTiling = allPropsUseSameTiling;
		this.allPropsUseSameTiling_sourceMaterialTiling = sourceMaterialTiling;
		this.allPropsUseSameTiling_samplingEncapsulatinRect = samplingEncapsulatingRect;
		this.propsUseDifferntTiling_srcUVsamplingRect = srcUVsamplingRect;
		this.srcObjName = objName;
	}

	public override int GetHashCode()
	{
		return this.material.GetInstanceID() ^ this.allPropsUseSameTiling_samplingEncapsulatinRect.GetHashCode() ^ this.propsUseDifferntTiling_srcUVsamplingRect.GetHashCode();
	}

	public override bool Equals(object obj)
	{
		if (!(obj is MB_MaterialAndUVRect))
		{
			return false;
		}
		MB_MaterialAndUVRect mb_MaterialAndUVRect = (MB_MaterialAndUVRect)obj;
		return this.material == mb_MaterialAndUVRect.material && this.allPropsUseSameTiling_samplingEncapsulatinRect == mb_MaterialAndUVRect.allPropsUseSameTiling_samplingEncapsulatinRect && this.allPropsUseSameTiling_sourceMaterialTiling == mb_MaterialAndUVRect.allPropsUseSameTiling_sourceMaterialTiling && this.allPropsUseSameTiling == mb_MaterialAndUVRect.allPropsUseSameTiling && this.propsUseDifferntTiling_srcUVsamplingRect == mb_MaterialAndUVRect.propsUseDifferntTiling_srcUVsamplingRect;
	}

	public Rect GetEncapsulatingRect()
	{
		if (this.allPropsUseSameTiling)
		{
			return this.allPropsUseSameTiling_samplingEncapsulatinRect;
		}
		return this.propsUseDifferntTiling_srcUVsamplingRect;
	}

	public Rect GetMaterialTilingRect()
	{
		if (this.allPropsUseSameTiling)
		{
			return this.allPropsUseSameTiling_sourceMaterialTiling;
		}
		return new Rect(0f, 0f, 1f, 1f);
	}

	public Material material;

	public Rect atlasRect;

	public string srcObjName;

	public int textureArraySliceIdx = -1;

	public bool allPropsUseSameTiling = true;

	[FormerlySerializedAs("sourceMaterialTiling")]
	public Rect allPropsUseSameTiling_sourceMaterialTiling;

	[FormerlySerializedAs("samplingEncapsulatinRect")]
	public Rect allPropsUseSameTiling_samplingEncapsulatinRect;

	public Rect propsUseDifferntTiling_srcUVsamplingRect;

	[NonSerialized]
	public List<GameObject> objectsThatUse;

	public MB_TextureTilingTreatment tilingTreatment = MB_TextureTilingTreatment.unknown;
}
