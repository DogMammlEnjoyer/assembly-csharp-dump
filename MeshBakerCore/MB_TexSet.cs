using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace DigitalOpus.MB.Core
{
	public class MB_TexSet
	{
		public bool allTexturesUseSameMatTiling { get; private set; }

		public bool thisIsOnlyTexSetInAtlas { get; private set; }

		public MB_TextureTilingTreatment tilingTreatment { get; private set; }

		public Vector2 obUVoffset { get; private set; }

		public Vector2 obUVscale { get; private set; }

		internal DRect obUVrect
		{
			get
			{
				return new DRect(this.obUVoffset, this.obUVscale);
			}
		}

		public MB_TexSet(MeshBakerMaterialTexture[] tss, Vector2 uvOffset, Vector2 uvScale, MB_TextureTilingTreatment treatment)
		{
			this.ts = tss;
			this.tilingTreatment = treatment;
			this.obUVoffset = uvOffset;
			this.obUVscale = uvScale;
			this.allTexturesUseSameMatTiling = false;
			this.thisIsOnlyTexSetInAtlas = false;
			this.matsAndGOs = new MatsAndGOs();
			this.matsAndGOs.mats = new List<MatAndTransformToMerged>();
			this.matsAndGOs.gos = new List<GameObject>();
			this.pipelineVariation = new MB_TexSet.PipelineVariationSomeTexturesUseDifferentMatTiling(this);
		}

		internal bool IsEqual(object obj, bool fixOutOfBoundsUVs, MB3_TextureCombinerNonTextureProperties resultMaterialTextureBlender)
		{
			if (!(obj is MB_TexSet))
			{
				return false;
			}
			MB_TexSet mb_TexSet = (MB_TexSet)obj;
			if (mb_TexSet.ts.Length != this.ts.Length)
			{
				return false;
			}
			for (int i = 0; i < this.ts.Length; i++)
			{
				if (this.ts[i].matTilingRect != mb_TexSet.ts[i].matTilingRect)
				{
					return false;
				}
				if (!this.ts[i].AreTexturesEqual(mb_TexSet.ts[i]))
				{
					return false;
				}
				if (!resultMaterialTextureBlender.NonTexturePropertiesAreEqual(this.matsAndGOs.mats[0].mat, mb_TexSet.matsAndGOs.mats[0].mat))
				{
					return false;
				}
			}
			return (!fixOutOfBoundsUVs || (this.obUVoffset.x == mb_TexSet.obUVoffset.x && this.obUVoffset.y == mb_TexSet.obUVoffset.y)) && (!fixOutOfBoundsUVs || (this.obUVscale.x == mb_TexSet.obUVscale.x && this.obUVscale.y == mb_TexSet.obUVscale.y));
		}

		public Vector2 GetMaxRawTextureHeightWidth()
		{
			Vector2 vector = new Vector2(0f, 0f);
			for (int i = 0; i < this.ts.Length; i++)
			{
				MeshBakerMaterialTexture meshBakerMaterialTexture = this.ts[i];
				if (!meshBakerMaterialTexture.isNull)
				{
					vector.x = Mathf.Max(vector.x, (float)meshBakerMaterialTexture.width);
					vector.y = Mathf.Max(vector.y, (float)meshBakerMaterialTexture.height);
				}
			}
			return vector;
		}

		private Rect GetEncapsulatingSamplingRectIfTilingSame()
		{
			if (this.ts.Length != 0)
			{
				return this.ts[0].GetEncapsulatingSamplingRect().GetRect();
			}
			return new Rect(0f, 0f, 1f, 1f);
		}

		public void SetEncapsulatingSamplingRectWhenMergingTexSets(DRect newEncapsulatingSamplingRect)
		{
			for (int i = 0; i < this.ts.Length; i++)
			{
				this.ts[i].SetEncapsulatingSamplingRect(this, newEncapsulatingSamplingRect);
			}
		}

		public void SetEncapsulatingSamplingRectForTesting(int propIdx, DRect newEncapsulatingSamplingRect)
		{
			this.ts[propIdx].SetEncapsulatingSamplingRect(this, newEncapsulatingSamplingRect);
		}

		public void SetEncapsulatingRect(int propIdx, bool considerMeshUVs)
		{
			if (considerMeshUVs)
			{
				this.ts[propIdx].SetEncapsulatingSamplingRect(this, this.obUVrect);
				return;
			}
			this.ts[propIdx].SetEncapsulatingSamplingRect(this, new DRect(0f, 0f, 1f, 1f));
		}

		public void CreateColoredTexToReplaceNull(string propName, int propIdx, bool considerMeshUVs, MB3_TextureCombiner combiner, Color col, bool isLinear)
		{
			MeshBakerMaterialTexture meshBakerMaterialTexture = this.ts[propIdx];
			Texture2D t = combiner._createTemporaryTexture(propName, 16, 16, TextureFormat.ARGB32, true, isLinear);
			meshBakerMaterialTexture.t = t;
			MB_Utility.setSolidColor(meshBakerMaterialTexture.GetTexture2D(), col);
		}

		public void SetThisIsOnlyTexSetInAtlasTrue()
		{
			this.thisIsOnlyTexSetInAtlas = true;
		}

		public void SetAllTexturesUseSameMatTilingTrue()
		{
			this.allTexturesUseSameMatTiling = true;
			this.pipelineVariation = new MB_TexSet.PipelineVariationAllTexturesUseSameMatTiling(this);
		}

		public void AdjustResultMaterialNonTextureProperties(Material resultMaterial, List<ShaderTextureProperty> props)
		{
			this.pipelineVariation.AdjustResultMaterialNonTextureProperties(resultMaterial, props);
		}

		public void SetTilingTreatmentAndAdjustEncapsulatingSamplingRect(MB_TextureTilingTreatment newTilingTreatment)
		{
			this.tilingTreatment = newTilingTreatment;
			this.pipelineVariation.SetTilingTreatmentAndAdjustEncapsulatingSamplingRect(newTilingTreatment);
		}

		internal void GetRectsForTextureBakeResults(out Rect allPropsUseSameTiling_encapsulatingSamplingRect, out Rect propsUseDifferntTiling_obUVRect)
		{
			this.pipelineVariation.GetRectsForTextureBakeResults(out allPropsUseSameTiling_encapsulatingSamplingRect, out propsUseDifferntTiling_obUVRect);
		}

		internal Rect GetMaterialTilingRectForTextureBakerResults(int materialIndex)
		{
			return this.pipelineVariation.GetMaterialTilingRectForTextureBakerResults(materialIndex);
		}

		internal void CalcInitialFullSamplingRects(bool fixOutOfBoundsUVs)
		{
			DRect r = new DRect(0f, 0f, 1f, 1f);
			if (fixOutOfBoundsUVs)
			{
				r = this.obUVrect;
			}
			for (int i = 0; i < this.ts.Length; i++)
			{
				if (!this.ts[i].isNull)
				{
					DRect matTilingRect = this.ts[i].matTilingRect;
					DRect obUVrect;
					if (fixOutOfBoundsUVs)
					{
						obUVrect = this.obUVrect;
					}
					else
					{
						obUVrect = new DRect(0.0, 0.0, 1.0, 1.0);
					}
					this.ts[i].SetEncapsulatingSamplingRect(this, MB3_UVTransformUtility.CombineTransforms(ref obUVrect, ref matTilingRect));
					r = this.ts[i].GetEncapsulatingSamplingRect();
				}
			}
			for (int j = 0; j < this.ts.Length; j++)
			{
				if (this.ts[j].isNull)
				{
					this.ts[j].SetEncapsulatingSamplingRect(this, r);
				}
			}
		}

		internal void CalcMatAndUVSamplingRects()
		{
			DRect matTilingRect = new DRect(0f, 0f, 1f, 1f);
			if (this.allTexturesUseSameMatTiling)
			{
				for (int i = 0; i < this.ts.Length; i++)
				{
					if (!this.ts[i].isNull)
					{
						matTilingRect = this.ts[i].matTilingRect;
						break;
					}
				}
			}
			for (int j = 0; j < this.matsAndGOs.mats.Count; j++)
			{
				this.matsAndGOs.mats[j].AssignInitialValuesForMaterialTilingAndSamplingRectMatAndUVTiling(this.allTexturesUseSameMatTiling, matTilingRect);
			}
		}

		public bool AllTexturesAreSameForMerge(MB_TexSet other, bool considerNonTextureProperties, MB3_TextureCombinerNonTextureProperties resultMaterialTextureBlender)
		{
			if (other.ts.Length != this.ts.Length)
			{
				return false;
			}
			if (!other.allTexturesUseSameMatTiling || !this.allTexturesUseSameMatTiling)
			{
				return false;
			}
			int num = -1;
			for (int i = 0; i < this.ts.Length; i++)
			{
				if (!this.ts[i].AreTexturesEqual(other.ts[i]))
				{
					return false;
				}
				if (num == -1 && !this.ts[i].isNull)
				{
					num = i;
				}
				if (considerNonTextureProperties && !resultMaterialTextureBlender.NonTexturePropertiesAreEqual(this.matsAndGOs.mats[0].mat, other.matsAndGOs.mats[0].mat))
				{
					return false;
				}
			}
			if (num != -1)
			{
				for (int j = 0; j < this.ts.Length; j++)
				{
					if (!this.ts[j].AreTexturesEqual(other.ts[j]))
					{
						return false;
					}
				}
			}
			return true;
		}

		public void DrawRectsToMergeGizmos(Color encC, Color innerC)
		{
			DRect encapsulatingSamplingRect = this.ts[0].GetEncapsulatingSamplingRect();
			encapsulatingSamplingRect.Expand(0.05f);
			Gizmos.color = encC;
			Gizmos.DrawWireCube(encapsulatingSamplingRect.center.GetVector2(), encapsulatingSamplingRect.size);
			for (int i = 0; i < this.matsAndGOs.mats.Count; i++)
			{
				DRect samplingRectMatAndUVTiling = this.matsAndGOs.mats[i].samplingRectMatAndUVTiling;
				DRect shiftTransformToFitBinA = MB3_UVTransformUtility.GetShiftTransformToFitBinA(ref encapsulatingSamplingRect, ref samplingRectMatAndUVTiling);
				Vector2 vector = MB3_UVTransformUtility.TransformPoint(ref shiftTransformToFitBinA, samplingRectMatAndUVTiling.min);
				samplingRectMatAndUVTiling.x = (double)vector.x;
				samplingRectMatAndUVTiling.y = (double)vector.y;
				Gizmos.color = innerC;
				Gizmos.DrawWireCube(samplingRectMatAndUVTiling.center.GetVector2(), samplingRectMatAndUVTiling.size);
			}
		}

		internal string GetDescription()
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.AppendFormat("[GAME_OBJS=", Array.Empty<object>());
			for (int i = 0; i < this.matsAndGOs.gos.Count; i++)
			{
				stringBuilder.AppendFormat("{0},", this.matsAndGOs.gos[i].name);
			}
			stringBuilder.AppendFormat("MATS=", Array.Empty<object>());
			for (int j = 0; j < this.matsAndGOs.mats.Count; j++)
			{
				stringBuilder.AppendFormat("{0},", this.matsAndGOs.mats[j].GetMaterialName());
			}
			stringBuilder.Append("]");
			return stringBuilder.ToString();
		}

		internal string GetMatSubrectDescriptions()
		{
			StringBuilder stringBuilder = new StringBuilder();
			for (int i = 0; i < this.matsAndGOs.mats.Count; i++)
			{
				stringBuilder.AppendFormat("\n    {0}={1},", this.matsAndGOs.mats[i].GetMaterialName(), this.matsAndGOs.mats[i].samplingRectMatAndUVTiling);
			}
			return stringBuilder.ToString();
		}

		public MeshBakerMaterialTexture[] ts;

		public MatsAndGOs matsAndGOs;

		public int idealWidth_pix;

		public int idealHeight_pix;

		private MB_TexSet.PipelineVariation pipelineVariation;

		private interface PipelineVariation
		{
			void GetRectsForTextureBakeResults(out Rect allPropsUseSameTiling_encapsulatingSamplingRect, out Rect propsUseDifferntTiling_obUVRect);

			void SetTilingTreatmentAndAdjustEncapsulatingSamplingRect(MB_TextureTilingTreatment newTilingTreatment);

			Rect GetMaterialTilingRectForTextureBakerResults(int materialIndex);

			void AdjustResultMaterialNonTextureProperties(Material resultMaterial, List<ShaderTextureProperty> props);
		}

		private class PipelineVariationAllTexturesUseSameMatTiling : MB_TexSet.PipelineVariation
		{
			public PipelineVariationAllTexturesUseSameMatTiling(MB_TexSet ts)
			{
				this.texSet = ts;
			}

			public void GetRectsForTextureBakeResults(out Rect allPropsUseSameTiling_encapsulatingSamplingRect, out Rect propsUseDifferntTiling_obUVRect)
			{
				propsUseDifferntTiling_obUVRect = new Rect(0f, 0f, 0f, 0f);
				allPropsUseSameTiling_encapsulatingSamplingRect = this.texSet.GetEncapsulatingSamplingRectIfTilingSame();
				if (this.texSet.tilingTreatment == MB_TextureTilingTreatment.edgeToEdgeX)
				{
					allPropsUseSameTiling_encapsulatingSamplingRect.x = 0f;
					allPropsUseSameTiling_encapsulatingSamplingRect.width = 1f;
					return;
				}
				if (this.texSet.tilingTreatment == MB_TextureTilingTreatment.edgeToEdgeY)
				{
					allPropsUseSameTiling_encapsulatingSamplingRect.y = 0f;
					allPropsUseSameTiling_encapsulatingSamplingRect.height = 1f;
					return;
				}
				if (this.texSet.tilingTreatment == MB_TextureTilingTreatment.edgeToEdgeXY)
				{
					allPropsUseSameTiling_encapsulatingSamplingRect = new Rect(0f, 0f, 1f, 1f);
				}
			}

			public void SetTilingTreatmentAndAdjustEncapsulatingSamplingRect(MB_TextureTilingTreatment newTilingTreatment)
			{
				if (this.texSet.tilingTreatment == MB_TextureTilingTreatment.edgeToEdgeX)
				{
					foreach (MeshBakerMaterialTexture meshBakerMaterialTexture in this.texSet.ts)
					{
						DRect encapsulatingSamplingRect = meshBakerMaterialTexture.GetEncapsulatingSamplingRect();
						encapsulatingSamplingRect.width = 1.0;
						meshBakerMaterialTexture.SetEncapsulatingSamplingRect(this.texSet, encapsulatingSamplingRect);
					}
					return;
				}
				if (this.texSet.tilingTreatment == MB_TextureTilingTreatment.edgeToEdgeY)
				{
					foreach (MeshBakerMaterialTexture meshBakerMaterialTexture2 in this.texSet.ts)
					{
						DRect encapsulatingSamplingRect2 = meshBakerMaterialTexture2.GetEncapsulatingSamplingRect();
						encapsulatingSamplingRect2.height = 1.0;
						meshBakerMaterialTexture2.SetEncapsulatingSamplingRect(this.texSet, encapsulatingSamplingRect2);
					}
					return;
				}
				if (this.texSet.tilingTreatment == MB_TextureTilingTreatment.edgeToEdgeXY)
				{
					foreach (MeshBakerMaterialTexture meshBakerMaterialTexture3 in this.texSet.ts)
					{
						DRect encapsulatingSamplingRect3 = meshBakerMaterialTexture3.GetEncapsulatingSamplingRect();
						encapsulatingSamplingRect3.height = 1.0;
						encapsulatingSamplingRect3.width = 1.0;
						meshBakerMaterialTexture3.SetEncapsulatingSamplingRect(this.texSet, encapsulatingSamplingRect3);
					}
				}
			}

			public Rect GetMaterialTilingRectForTextureBakerResults(int materialIndex)
			{
				return this.texSet.matsAndGOs.mats[materialIndex].materialTiling.GetRect();
			}

			public void AdjustResultMaterialNonTextureProperties(Material resultMaterial, List<ShaderTextureProperty> props)
			{
			}

			private MB_TexSet texSet;
		}

		private class PipelineVariationSomeTexturesUseDifferentMatTiling : MB_TexSet.PipelineVariation
		{
			public PipelineVariationSomeTexturesUseDifferentMatTiling(MB_TexSet ts)
			{
				this.texSet = ts;
			}

			public void GetRectsForTextureBakeResults(out Rect allPropsUseSameTiling_encapsulatingSamplingRect, out Rect propsUseDifferntTiling_obUVRect)
			{
				allPropsUseSameTiling_encapsulatingSamplingRect = new Rect(0f, 0f, 0f, 0f);
				propsUseDifferntTiling_obUVRect = this.texSet.obUVrect.GetRect();
				if (this.texSet.tilingTreatment == MB_TextureTilingTreatment.edgeToEdgeX)
				{
					propsUseDifferntTiling_obUVRect.x = 0f;
					propsUseDifferntTiling_obUVRect.width = 1f;
					return;
				}
				if (this.texSet.tilingTreatment == MB_TextureTilingTreatment.edgeToEdgeY)
				{
					propsUseDifferntTiling_obUVRect.y = 0f;
					propsUseDifferntTiling_obUVRect.height = 1f;
					return;
				}
				if (this.texSet.tilingTreatment == MB_TextureTilingTreatment.edgeToEdgeXY)
				{
					propsUseDifferntTiling_obUVRect = new Rect(0f, 0f, 1f, 1f);
				}
			}

			public void SetTilingTreatmentAndAdjustEncapsulatingSamplingRect(MB_TextureTilingTreatment newTilingTreatment)
			{
				if (this.texSet.tilingTreatment == MB_TextureTilingTreatment.edgeToEdgeX)
				{
					foreach (MeshBakerMaterialTexture meshBakerMaterialTexture in this.texSet.ts)
					{
						DRect encapsulatingSamplingRect = meshBakerMaterialTexture.GetEncapsulatingSamplingRect();
						encapsulatingSamplingRect.width = 1.0;
						meshBakerMaterialTexture.SetEncapsulatingSamplingRect(this.texSet, encapsulatingSamplingRect);
					}
					return;
				}
				if (this.texSet.tilingTreatment == MB_TextureTilingTreatment.edgeToEdgeY)
				{
					foreach (MeshBakerMaterialTexture meshBakerMaterialTexture2 in this.texSet.ts)
					{
						DRect encapsulatingSamplingRect2 = meshBakerMaterialTexture2.GetEncapsulatingSamplingRect();
						encapsulatingSamplingRect2.height = 1.0;
						meshBakerMaterialTexture2.SetEncapsulatingSamplingRect(this.texSet, encapsulatingSamplingRect2);
					}
					return;
				}
				if (this.texSet.tilingTreatment == MB_TextureTilingTreatment.edgeToEdgeXY)
				{
					foreach (MeshBakerMaterialTexture meshBakerMaterialTexture3 in this.texSet.ts)
					{
						DRect encapsulatingSamplingRect3 = meshBakerMaterialTexture3.GetEncapsulatingSamplingRect();
						encapsulatingSamplingRect3.height = 1.0;
						encapsulatingSamplingRect3.width = 1.0;
						meshBakerMaterialTexture3.SetEncapsulatingSamplingRect(this.texSet, encapsulatingSamplingRect3);
					}
				}
			}

			public Rect GetMaterialTilingRectForTextureBakerResults(int materialIndex)
			{
				return new Rect(0f, 0f, 0f, 0f);
			}

			public void AdjustResultMaterialNonTextureProperties(Material resultMaterial, List<ShaderTextureProperty> props)
			{
				if (this.texSet.thisIsOnlyTexSetInAtlas)
				{
					for (int i = 0; i < props.Count; i++)
					{
						if (resultMaterial.HasProperty(props[i].name))
						{
							resultMaterial.SetTextureOffset(props[i].name, this.texSet.ts[i].matTilingRect.min);
							resultMaterial.SetTextureScale(props[i].name, this.texSet.ts[i].matTilingRect.size);
						}
					}
				}
			}

			private MB_TexSet texSet;
		}
	}
}
