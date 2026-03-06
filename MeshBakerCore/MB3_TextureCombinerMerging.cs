using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace DigitalOpus.MB.Core
{
	public class MB3_TextureCombinerMerging
	{
		public static Rect BuildTransformMeshUV2AtlasRect(bool considerMeshUVs, Rect _atlasRect, Rect _obUVRect, Rect _sourceMaterialTiling, Rect _encapsulatingRect)
		{
			DRect drect = new DRect(_atlasRect);
			DRect drect2;
			if (considerMeshUVs)
			{
				drect2 = new DRect(_obUVRect);
			}
			else
			{
				drect2 = new DRect(0.0, 0.0, 1.0, 1.0);
			}
			DRect drect3 = new DRect(_sourceMaterialTiling);
			DRect drect4 = new DRect(_encapsulatingRect);
			DRect drect5 = MB3_UVTransformUtility.InverseTransform(ref drect4);
			DRect drect6 = MB3_UVTransformUtility.InverseTransform(ref drect2);
			DRect drect7 = MB3_UVTransformUtility.CombineTransforms(ref drect2, ref drect3);
			DRect shiftTransformToFitBinA = MB3_UVTransformUtility.GetShiftTransformToFitBinA(ref drect4, ref drect7);
			drect7 = MB3_UVTransformUtility.CombineTransforms(ref drect7, ref shiftTransformToFitBinA);
			DRect drect8 = MB3_UVTransformUtility.CombineTransforms(ref drect7, ref drect5);
			DRect drect9 = MB3_UVTransformUtility.CombineTransforms(ref drect6, ref drect8);
			drect9 = MB3_UVTransformUtility.CombineTransforms(ref drect9, ref drect);
			return drect9.GetRect();
		}

		public MB3_TextureCombinerMerging(bool considerNonTextureProps, MB3_TextureCombinerNonTextureProperties resultMaterialTexBlender, bool fixObUVs, MB2_LogLevel logLevel)
		{
			this.LOG_LEVEL = logLevel;
			this._considerNonTextureProperties = considerNonTextureProps;
			this.resultMaterialTextureBlender = resultMaterialTexBlender;
			this.fixOutOfBoundsUVs = fixObUVs;
		}

		public void MergeOverlappingDistinctMaterialTexturesAndCalcMaterialSubrects(List<MB_TexSet> distinctMaterialTextures)
		{
			if (this.LOG_LEVEL >= MB2_LogLevel.debug)
			{
				Debug.Log("MergeOverlappingDistinctMaterialTexturesAndCalcMaterialSubrects num atlas rects" + distinctMaterialTextures.Count.ToString());
			}
			int num = 0;
			for (int i = 0; i < distinctMaterialTextures.Count; i++)
			{
				MB_TexSet mb_TexSet = distinctMaterialTextures[i];
				int num2 = -1;
				bool flag = true;
				DRect a = default(DRect);
				for (int j = 0; j < mb_TexSet.ts.Length; j++)
				{
					if (num2 != -1)
					{
						if (!mb_TexSet.ts[j].isNull && a != mb_TexSet.ts[j].matTilingRect)
						{
							flag = false;
						}
					}
					else if (!mb_TexSet.ts[j].isNull)
					{
						num2 = j;
						a = mb_TexSet.ts[j].matTilingRect;
					}
				}
				if (this.LOG_LEVEL >= MB2_LogLevel.debug || MB3_TextureCombinerMerging.LOG_LEVEL_TRACE_MERGE_MAT_SUBRECTS)
				{
					if (flag)
					{
						Debug.LogFormat("TextureSet {0} allTexturesUseSameMatTiling = {1}", new object[]
						{
							i,
							flag
						});
					}
					else
					{
						Debug.Log(string.Format("Textures in material(s) do not all use the same material tiling. This set of textures will not be considered for merge: {0} ", mb_TexSet.GetDescription()));
					}
				}
				if (flag)
				{
					mb_TexSet.SetAllTexturesUseSameMatTilingTrue();
				}
			}
			for (int k = 0; k < distinctMaterialTextures.Count; k++)
			{
				MB_TexSet mb_TexSet2 = distinctMaterialTextures[k];
				for (int l = 0; l < mb_TexSet2.matsAndGOs.mats.Count; l++)
				{
					if (mb_TexSet2.matsAndGOs.gos.Count > 0)
					{
						mb_TexSet2.matsAndGOs.mats[l].objName = mb_TexSet2.matsAndGOs.gos[0].name;
					}
					else if (mb_TexSet2.ts[0] != null)
					{
						mb_TexSet2.matsAndGOs.mats[l].objName = string.Format("[objWithTx:{0} atlasBlock:{1} matIdx{2}]", mb_TexSet2.ts[0].GetTexName(), k, l);
					}
					else
					{
						mb_TexSet2.matsAndGOs.mats[l].objName = string.Format("[objWithTx:{0} atlasBlock:{1} matIdx{2}]", "Unknown", k, l);
					}
				}
				mb_TexSet2.CalcInitialFullSamplingRects(this.fixOutOfBoundsUVs);
				mb_TexSet2.CalcMatAndUVSamplingRects();
			}
			this._HasBeenInitialized = true;
			List<int> list = new List<int>();
			for (int m = 0; m < distinctMaterialTextures.Count; m++)
			{
				MB_TexSet mb_TexSet3 = distinctMaterialTextures[m];
				for (int n = m + 1; n < distinctMaterialTextures.Count; n++)
				{
					MB_TexSet mb_TexSet4 = distinctMaterialTextures[n];
					if (mb_TexSet4.AllTexturesAreSameForMerge(mb_TexSet3, this._considerNonTextureProperties, this.resultMaterialTextureBlender))
					{
						double num3 = 0.0;
						double num4 = 0.0;
						DRect drect = default(DRect);
						int num5 = -1;
						for (int num6 = 0; num6 < mb_TexSet3.ts.Length; num6++)
						{
							if (!mb_TexSet3.ts[num6].isNull && num5 == -1)
							{
								num5 = num6;
							}
						}
						DRect drect2 = default(DRect);
						DRect drect3 = default(DRect);
						if (num5 != -1)
						{
							drect2 = mb_TexSet4.matsAndGOs.mats[0].samplingRectMatAndUVTiling;
							for (int num7 = 1; num7 < mb_TexSet4.matsAndGOs.mats.Count; num7++)
							{
								DRect samplingRectMatAndUVTiling = mb_TexSet4.matsAndGOs.mats[num7].samplingRectMatAndUVTiling;
								drect2 = MB3_UVTransformUtility.GetEncapsulatingRectShifted(ref drect2, ref samplingRectMatAndUVTiling);
							}
							drect3 = mb_TexSet3.matsAndGOs.mats[0].samplingRectMatAndUVTiling;
							for (int num8 = 1; num8 < mb_TexSet3.matsAndGOs.mats.Count; num8++)
							{
								DRect samplingRectMatAndUVTiling2 = mb_TexSet3.matsAndGOs.mats[num8].samplingRectMatAndUVTiling;
								drect3 = MB3_UVTransformUtility.GetEncapsulatingRectShifted(ref drect3, ref samplingRectMatAndUVTiling2);
							}
							drect = MB3_UVTransformUtility.GetEncapsulatingRectShifted(ref drect2, ref drect3);
							num3 += drect.width * drect.height;
							num4 += drect2.width * drect2.height + drect3.width * drect3.height;
						}
						else
						{
							drect = new DRect(0f, 0f, 1f, 1f);
						}
						if (num3 < num4)
						{
							num++;
							StringBuilder stringBuilder = null;
							if (this.LOG_LEVEL >= MB2_LogLevel.info)
							{
								stringBuilder = new StringBuilder();
								stringBuilder.AppendFormat("About To Merge:\n   TextureSet1 {0}\n   TextureSet2 {1}\n", mb_TexSet4.GetDescription(), mb_TexSet3.GetDescription());
								if (this.LOG_LEVEL >= MB2_LogLevel.trace)
								{
									for (int num9 = 0; num9 < mb_TexSet4.matsAndGOs.mats.Count; num9++)
									{
										stringBuilder.AppendFormat("tx1 Mat {0} matAndMeshUVRect {1} fullSamplingRect {2}\n", mb_TexSet4.matsAndGOs.mats[num9].mat, mb_TexSet4.matsAndGOs.mats[num9].samplingRectMatAndUVTiling, mb_TexSet4.ts[0].GetEncapsulatingSamplingRect());
									}
									for (int num10 = 0; num10 < mb_TexSet3.matsAndGOs.mats.Count; num10++)
									{
										stringBuilder.AppendFormat("tx2 Mat {0} matAndMeshUVRect {1} fullSamplingRect {2}\n", mb_TexSet3.matsAndGOs.mats[num10].mat, mb_TexSet3.matsAndGOs.mats[num10].samplingRectMatAndUVTiling, mb_TexSet3.ts[0].GetEncapsulatingSamplingRect());
									}
								}
							}
							for (int num11 = 0; num11 < mb_TexSet3.matsAndGOs.gos.Count; num11++)
							{
								if (!mb_TexSet4.matsAndGOs.gos.Contains(mb_TexSet3.matsAndGOs.gos[num11]))
								{
									mb_TexSet4.matsAndGOs.gos.Add(mb_TexSet3.matsAndGOs.gos[num11]);
								}
							}
							for (int num12 = 0; num12 < mb_TexSet3.matsAndGOs.mats.Count; num12++)
							{
								mb_TexSet4.matsAndGOs.mats.Add(mb_TexSet3.matsAndGOs.mats[num12]);
							}
							mb_TexSet4.SetEncapsulatingSamplingRectWhenMergingTexSets(drect);
							if (!list.Contains(m))
							{
								list.Add(m);
							}
							if (this.LOG_LEVEL >= MB2_LogLevel.debug)
							{
								if (this.LOG_LEVEL >= MB2_LogLevel.trace)
								{
									stringBuilder.AppendFormat("=== After Merge TextureSet {0}\n", mb_TexSet4.GetDescription());
									for (int num13 = 0; num13 < mb_TexSet4.matsAndGOs.mats.Count; num13++)
									{
										stringBuilder.AppendFormat("tx1 Mat {0} matAndMeshUVRect {1} fullSamplingRect {2}\n", mb_TexSet4.matsAndGOs.mats[num13].mat, mb_TexSet4.matsAndGOs.mats[num13].samplingRectMatAndUVTiling, mb_TexSet4.ts[0].GetEncapsulatingSamplingRect());
									}
									if (MB3_TextureCombinerMerging.DO_INTEGRITY_CHECKS && MB3_TextureCombinerMerging.DO_INTEGRITY_CHECKS)
									{
										this.DoIntegrityCheckMergedEncapsulatingSamplingRects(distinctMaterialTextures);
									}
								}
								Debug.Log(stringBuilder.ToString());
								break;
							}
							break;
						}
						else if (this.LOG_LEVEL >= MB2_LogLevel.debug)
						{
							string format = "Considered merging {0} and {1} but there was not enough overlap. It is more efficient to bake these to separate rectangles.";
							string description = mb_TexSet4.GetDescription();
							DRect drect4 = drect2;
							object arg = description + drect4.ToString();
							string description2 = mb_TexSet3.GetDescription();
							drect4 = drect3;
							Debug.Log(string.Format(format, arg, description2 + drect4.ToString()));
						}
					}
				}
			}
			for (int num14 = list.Count - 1; num14 >= 0; num14--)
			{
				distinctMaterialTextures.RemoveAt(list[num14]);
			}
			list.Clear();
			if (this.LOG_LEVEL >= MB2_LogLevel.debug)
			{
				Debug.Log(string.Format("MergeOverlappingDistinctMaterialTexturesAndCalcMaterialSubrects complete merged {0} now have {1}", num, distinctMaterialTextures.Count));
			}
			if (MB3_TextureCombinerMerging.DO_INTEGRITY_CHECKS)
			{
				this.DoIntegrityCheckMergedEncapsulatingSamplingRects(distinctMaterialTextures);
			}
		}

		public void MergeDistinctMaterialTexturesThatWouldExceedMaxAtlasSizeAndCalcMaterialSubrects(List<MB_TexSet> distinctMaterialTextures, int maxAtlasSize)
		{
			if (this.LOG_LEVEL >= MB2_LogLevel.debug)
			{
				Debug.Log("MergeDistinctMaterialTexturesThatWouldExceedMaxAtlasSizeAndCalcMaterialSubrects num atlas rects" + distinctMaterialTextures.Count.ToString());
			}
			int num = 0;
			List<int> list = new List<int>();
			for (int i = 0; i < distinctMaterialTextures.Count; i++)
			{
				MB_TexSet mb_TexSet = distinctMaterialTextures[i];
				for (int j = i + 1; j < distinctMaterialTextures.Count; j++)
				{
					MB_TexSet mb_TexSet2 = distinctMaterialTextures[j];
					if (mb_TexSet2.AllTexturesAreSameForMerge(mb_TexSet, this._considerNonTextureProperties, this.resultMaterialTextureBlender))
					{
						DRect drect = default(DRect);
						int num2 = -1;
						for (int k = 0; k < mb_TexSet.ts.Length; k++)
						{
							if (!mb_TexSet.ts[k].isNull && num2 == -1)
							{
								num2 = k;
							}
						}
						DRect drect2 = default(DRect);
						DRect drect3 = default(DRect);
						if (num2 != -1)
						{
							drect2 = mb_TexSet2.matsAndGOs.mats[0].samplingRectMatAndUVTiling;
							for (int l = 1; l < mb_TexSet2.matsAndGOs.mats.Count; l++)
							{
								DRect samplingRectMatAndUVTiling = mb_TexSet2.matsAndGOs.mats[l].samplingRectMatAndUVTiling;
								drect2 = MB3_UVTransformUtility.GetEncapsulatingRectShifted(ref drect2, ref samplingRectMatAndUVTiling);
							}
							drect3 = mb_TexSet.matsAndGOs.mats[0].samplingRectMatAndUVTiling;
							for (int m = 1; m < mb_TexSet.matsAndGOs.mats.Count; m++)
							{
								DRect samplingRectMatAndUVTiling2 = mb_TexSet.matsAndGOs.mats[m].samplingRectMatAndUVTiling;
								drect3 = MB3_UVTransformUtility.GetEncapsulatingRectShifted(ref drect3, ref samplingRectMatAndUVTiling2);
							}
							drect = MB3_UVTransformUtility.GetEncapsulatingRectShifted(ref drect2, ref drect3);
						}
						else
						{
							drect = new DRect(0f, 0f, 1f, 1f);
						}
						Vector2 maxRawTextureHeightWidth = mb_TexSet2.GetMaxRawTextureHeightWidth();
						if (drect.width * (double)maxRawTextureHeightWidth.x > (double)maxAtlasSize || drect.height * (double)maxRawTextureHeightWidth.y > (double)maxAtlasSize)
						{
							num++;
							StringBuilder stringBuilder = null;
							if (this.LOG_LEVEL >= MB2_LogLevel.info)
							{
								stringBuilder = new StringBuilder();
								stringBuilder.AppendFormat("About To Merge:\n   TextureSet1 {0}\n   TextureSet2 {1}\n", mb_TexSet2.GetDescription(), mb_TexSet.GetDescription());
								if (this.LOG_LEVEL >= MB2_LogLevel.trace)
								{
									for (int n = 0; n < mb_TexSet2.matsAndGOs.mats.Count; n++)
									{
										stringBuilder.AppendFormat("tx1 Mat {0} matAndMeshUVRect {1} fullSamplingRect {2}\n", mb_TexSet2.matsAndGOs.mats[n].mat, mb_TexSet2.matsAndGOs.mats[n].samplingRectMatAndUVTiling, mb_TexSet2.ts[0].GetEncapsulatingSamplingRect());
									}
									for (int num3 = 0; num3 < mb_TexSet.matsAndGOs.mats.Count; num3++)
									{
										stringBuilder.AppendFormat("tx2 Mat {0} matAndMeshUVRect {1} fullSamplingRect {2}\n", mb_TexSet.matsAndGOs.mats[num3].mat, mb_TexSet.matsAndGOs.mats[num3].samplingRectMatAndUVTiling, mb_TexSet.ts[0].GetEncapsulatingSamplingRect());
									}
								}
							}
							for (int num4 = 0; num4 < mb_TexSet.matsAndGOs.gos.Count; num4++)
							{
								if (!mb_TexSet2.matsAndGOs.gos.Contains(mb_TexSet.matsAndGOs.gos[num4]))
								{
									mb_TexSet2.matsAndGOs.gos.Add(mb_TexSet.matsAndGOs.gos[num4]);
								}
							}
							for (int num5 = 0; num5 < mb_TexSet.matsAndGOs.mats.Count; num5++)
							{
								mb_TexSet2.matsAndGOs.mats.Add(mb_TexSet.matsAndGOs.mats[num5]);
							}
							mb_TexSet2.SetEncapsulatingSamplingRectWhenMergingTexSets(drect);
							if (!list.Contains(i))
							{
								list.Add(i);
							}
							if (this.LOG_LEVEL >= MB2_LogLevel.debug)
							{
								if (this.LOG_LEVEL >= MB2_LogLevel.trace)
								{
									stringBuilder.AppendFormat("=== After Merge TextureSet {0}\n", mb_TexSet2.GetDescription());
									for (int num6 = 0; num6 < mb_TexSet2.matsAndGOs.mats.Count; num6++)
									{
										stringBuilder.AppendFormat("tx1 Mat {0} matAndMeshUVRect {1} fullSamplingRect {2}\n", mb_TexSet2.matsAndGOs.mats[num6].mat, mb_TexSet2.matsAndGOs.mats[num6].samplingRectMatAndUVTiling, mb_TexSet2.ts[0].GetEncapsulatingSamplingRect());
									}
									if (MB3_TextureCombinerMerging.DO_INTEGRITY_CHECKS && MB3_TextureCombinerMerging.DO_INTEGRITY_CHECKS)
									{
										this.DoIntegrityCheckMergedEncapsulatingSamplingRects(distinctMaterialTextures);
									}
								}
								Debug.Log(stringBuilder.ToString());
								break;
							}
							break;
						}
						else if (this.LOG_LEVEL >= MB2_LogLevel.debug)
						{
							string format = "Considered merging {0} and {1} but there was not enough overlap. It is more efficient to bake these to separate rectangles.";
							string description = mb_TexSet2.GetDescription();
							DRect drect4 = drect2;
							object arg = description + drect4.ToString();
							string description2 = mb_TexSet.GetDescription();
							drect4 = drect3;
							Debug.Log(string.Format(format, arg, description2 + drect4.ToString()));
						}
					}
				}
			}
			for (int num7 = list.Count - 1; num7 >= 0; num7--)
			{
				distinctMaterialTextures.RemoveAt(list[num7]);
			}
			list.Clear();
			if (this.LOG_LEVEL >= MB2_LogLevel.debug)
			{
				Debug.Log(string.Format("MergeDistinctMaterialTexturesThatWouldExceedMaxAtlasSizeAndCalcMaterialSubrects complete merged {0} now have {1}", num, distinctMaterialTextures.Count));
			}
			if (MB3_TextureCombinerMerging.DO_INTEGRITY_CHECKS)
			{
				this.DoIntegrityCheckMergedEncapsulatingSamplingRects(distinctMaterialTextures);
			}
		}

		public void DoIntegrityCheckMergedEncapsulatingSamplingRects(List<MB_TexSet> distinctMaterialTextures)
		{
			if (MB3_TextureCombinerMerging.DO_INTEGRITY_CHECKS)
			{
				for (int i = 0; i < distinctMaterialTextures.Count; i++)
				{
					MB_TexSet mb_TexSet = distinctMaterialTextures[i];
					if (mb_TexSet.allTexturesUseSameMatTiling)
					{
						for (int j = 0; j < mb_TexSet.matsAndGOs.mats.Count; j++)
						{
							MatAndTransformToMerged matAndTransformToMerged = mb_TexSet.matsAndGOs.mats[j];
							DRect obUVRectIfTilingSame = matAndTransformToMerged.obUVRectIfTilingSame;
							DRect materialTiling = matAndTransformToMerged.materialTiling;
							if (!MB2_TextureBakeResults.IsMeshAndMaterialRectEnclosedByAtlasRect(mb_TexSet.tilingTreatment, obUVRectIfTilingSame.GetRect(), materialTiling.GetRect(), mb_TexSet.ts[0].GetEncapsulatingSamplingRect().GetRect(), MB2_LogLevel.info))
							{
								string[] array = new string[11];
								array[0] = "mesh ";
								array[1] = mb_TexSet.matsAndGOs.mats[j].objName;
								array[2] = "\n uv=";
								int num = 3;
								DRect drect = obUVRectIfTilingSame;
								array[num] = drect.ToString();
								array[4] = "\n mat=";
								array[5] = materialTiling.GetRect().ToString("f5");
								array[6] = "\n samplingRect=";
								array[7] = mb_TexSet.matsAndGOs.mats[j].samplingRectMatAndUVTiling.GetRect().ToString("f4");
								array[8] = "\n encapsulatingRect ";
								array[9] = mb_TexSet.ts[0].GetEncapsulatingSamplingRect().GetRect().ToString("f4");
								array[10] = "\n";
								Debug.LogErrorFormat(string.Concat(array), Array.Empty<object>());
								Debug.LogErrorFormat(string.Format("Integrity check failed. " + mb_TexSet.matsAndGOs.mats[j].objName + " Encapsulating sampling rect failed to contain potentialRect\n", Array.Empty<object>()), Array.Empty<object>());
								MB2_TextureBakeResults.IsMeshAndMaterialRectEnclosedByAtlasRect(mb_TexSet.tilingTreatment, obUVRectIfTilingSame.GetRect(), materialTiling.GetRect(), mb_TexSet.ts[0].GetEncapsulatingSamplingRect().GetRect(), MB2_LogLevel.trace);
							}
						}
					}
				}
			}
		}

		public static bool DO_INTEGRITY_CHECKS;

		private bool _HasBeenInitialized;

		private bool _considerNonTextureProperties;

		private MB3_TextureCombinerNonTextureProperties resultMaterialTextureBlender;

		private bool fixOutOfBoundsUVs = true;

		public MB2_LogLevel LOG_LEVEL = MB2_LogLevel.info;

		private static bool LOG_LEVEL_TRACE_MERGE_MAT_SUBRECTS;
	}
}
