using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using UnityEngine;

namespace DigitalOpus.MB.Core
{
	public class MB3_TextureCombinerPipeline
	{
		internal static bool _ShouldWeCreateAtlasForThisProperty(int propertyIndex, bool considerNonTextureProperties, MB3_TextureCombinerPipeline.CreateAtlasForProperty[] allTexturesAreNullAndSameColor)
		{
			MB3_TextureCombinerPipeline.CreateAtlasForProperty createAtlasForProperty = allTexturesAreNullAndSameColor[propertyIndex];
			if (considerNonTextureProperties)
			{
				return !createAtlasForProperty.allNonTexturePropsAreSame || !createAtlasForProperty.allTexturesAreNull;
			}
			return !createAtlasForProperty.allTexturesAreNull;
		}

		internal static bool _DoAnySrcMatsHaveProperty(int propertyIndex, MB3_TextureCombinerPipeline.CreateAtlasForProperty[] allTexturesAreNullAndSameColor)
		{
			return !allTexturesAreNullAndSameColor[propertyIndex].allSrcMatsOmittedTextureProperty;
		}

		internal static bool _CollectPropertyNames(MB3_TextureCombinerPipeline.TexturePipelineData data, MB2_LogLevel LOG_LEVEL)
		{
			return MB3_TextureCombinerPipeline._CollectPropertyNames(data.texPropertyNames, data._customShaderPropNames, data.texPropNamesToIgnore, data.resultMaterial, LOG_LEVEL);
		}

		internal static bool _CollectPropertyNames(List<ShaderTextureProperty> texPropertyNames, List<ShaderTextureProperty> _customShaderPropNames, List<string> texPropsToIgnore, Material resultMaterial, MB2_LogLevel LOG_LEVEL)
		{
			int i;
			Predicate<ShaderTextureProperty> <>9__0;
			int l;
			for (i = 0; i < texPropertyNames.Count; i = l + 1)
			{
				Predicate<ShaderTextureProperty> match;
				if ((match = <>9__0) == null)
				{
					match = (<>9__0 = ((ShaderTextureProperty x) => x.name.Equals(texPropertyNames[i].name)));
				}
				ShaderTextureProperty shaderTextureProperty = _customShaderPropNames.Find(match);
				if (shaderTextureProperty != null)
				{
					_customShaderPropNames.Remove(shaderTextureProperty);
				}
				l = i;
			}
			if (resultMaterial == null)
			{
				Debug.LogError("Please assign a result material. The combined mesh will use this material.");
				return false;
			}
			MBVersion.CollectPropertyNames(texPropertyNames, MB3_TextureCombinerPipeline.shaderTexPropertyNames, _customShaderPropNames, resultMaterial, LOG_LEVEL);
			for (int j = texPropertyNames.Count - 1; j >= 0; j--)
			{
				for (int k = 0; k < texPropsToIgnore.Count; k++)
				{
					if (texPropsToIgnore[k].Equals(texPropertyNames[j].name))
					{
						texPropertyNames.RemoveAt(j);
					}
				}
			}
			return true;
		}

		public static Texture GetTextureConsideringStandardShaderKeywords(string shaderName, Material mat, string propertyName)
		{
			if ((!shaderName.Equals("Standard") && !shaderName.Equals("Standard (Specular setup)") && !shaderName.Equals("Standard (Roughness setup")) || !propertyName.Equals("_EmissionMap"))
			{
				return mat.GetTexture(propertyName);
			}
			if (mat.IsKeywordEnabled("_EMISSION"))
			{
				return mat.GetTexture(propertyName);
			}
			return null;
		}

		internal virtual IEnumerator __Step1_CollectDistinctMatTexturesAndUsedObjects(ProgressUpdateDelegate progressInfo, MB3_TextureCombiner.CombineTexturesIntoAtlasesCoroutineResult result, MB3_TextureCombinerPipeline.TexturePipelineData data, MB3_TextureCombiner combiner, MB2_EditorMethodsInterface textureEditorMethods, List<GameObject> usedObjsToMesh, MB2_LogLevel LOG_LEVEL)
		{
			Stopwatch stopwatch = new Stopwatch();
			stopwatch.Start();
			bool flag = false;
			Dictionary<int, MB_Utility.MeshAnalysisResult[]> dictionary = new Dictionary<int, MB_Utility.MeshAnalysisResult[]>();
			for (int i = 0; i < data.allObjsToMesh.Count; i++)
			{
				GameObject gameObject = data.allObjsToMesh[i];
				if (progressInfo != null)
				{
					string str = "Collecting textures for ";
					GameObject gameObject2 = gameObject;
					progressInfo(str + ((gameObject2 != null) ? gameObject2.ToString() : null), (float)i / (float)data.allObjsToMesh.Count / 2f);
				}
				if (LOG_LEVEL >= MB2_LogLevel.debug)
				{
					string str2 = "Collecting textures for object ";
					GameObject gameObject3 = gameObject;
					Debug.Log(str2 + ((gameObject3 != null) ? gameObject3.ToString() : null));
				}
				if (gameObject == null)
				{
					Debug.LogError("The list of objects to mesh contained nulls.");
					result.success = false;
					yield break;
				}
				Mesh mesh = MB_Utility.GetMesh(gameObject);
				if (mesh == null)
				{
					Debug.LogError("Object " + gameObject.name + " in the list of objects to mesh has no mesh.");
					result.success = false;
					yield break;
				}
				Material[] gomaterials = MB_Utility.GetGOMaterials(gameObject);
				if (gomaterials.Length == 0)
				{
					Debug.LogError("Object " + gameObject.name + " in the list of objects has no materials.");
					result.success = false;
					yield break;
				}
				MB_Utility.MeshAnalysisResult[] array;
				if (!dictionary.TryGetValue(mesh.GetInstanceID(), out array))
				{
					array = new MB_Utility.MeshAnalysisResult[mesh.subMeshCount];
					for (int j = 0; j < mesh.subMeshCount; j++)
					{
						MB_Utility.hasOutOfBoundsUVs(mesh, ref array[j], j, 0);
						if (data._normalizeTexelDensity)
						{
							array[j].submeshArea = MB3_TextureCombinerPipeline.GetSubmeshArea(mesh, j);
						}
						if (data._fixOutOfBoundsUVs && !array[j].hasUVs)
						{
							array[j].uvRect = new Rect(0f, 0f, 1f, 1f);
							string str3 = "Mesh for object ";
							GameObject gameObject4 = gameObject;
							Debug.LogWarning(str3 + ((gameObject4 != null) ? gameObject4.ToString() : null) + " has no UV channel but 'consider UVs' is enabled. Assuming UVs will be generated filling 0,0,1,1 rectangle.");
						}
					}
					dictionary.Add(mesh.GetInstanceID(), array);
				}
				if (data._fixOutOfBoundsUVs && LOG_LEVEL >= MB2_LogLevel.trace)
				{
					string[] array2 = new string[8];
					array2[0] = "Mesh Analysis for object ";
					int num = 1;
					GameObject gameObject5 = gameObject;
					array2[num] = ((gameObject5 != null) ? gameObject5.ToString() : null);
					array2[2] = " numSubmesh=";
					array2[3] = array.Length.ToString();
					array2[4] = " HasOBUV=";
					array2[5] = array[0].hasOutOfBoundsUVs.ToString();
					array2[6] = " UVrectSubmesh0=";
					int num2 = 7;
					Rect uvRect = array[0].uvRect;
					array2[num2] = uvRect.ToString();
					Debug.Log(string.Concat(array2));
				}
				for (int k = 0; k < gomaterials.Length; k++)
				{
					if (progressInfo != null)
					{
						progressInfo(string.Format("Collecting textures for {0} submesh {1}", gameObject, k), (float)i / (float)data.allObjsToMesh.Count / 2f);
					}
					Material material = gomaterials[k];
					if (data.allowedMaterialsFilter == null || data.allowedMaterialsFilter.Contains(material))
					{
						flag = (flag || array[k].hasOutOfBoundsUVs);
						if (material.name.Contains("(Instance)"))
						{
							Debug.LogWarning("The sharedMaterial on object " + gameObject.name + " has been 'Instanced'. This was probably caused by a script accessing the meshRender.material property in the editor.  The material to UV Rectangle mapping may be incorrect. To fix this recreate the object from its prefab or re-assign its material from the correct asset.");
						}
						if (data._fixOutOfBoundsUVs && !MB_Utility.AreAllSharedMaterialsDistinct(gomaterials) && LOG_LEVEL >= MB2_LogLevel.warn)
						{
							Debug.LogWarning("Object " + gameObject.name + " uses the same material on multiple submeshes. This may generate strange resultAtlasesAndRects especially when used with Consider Mesh UVs. Try duplicating the material.");
						}
						MeshBakerMaterialTexture[] array3 = new MeshBakerMaterialTexture[data.texPropertyNames.Count];
						for (int l = 0; l < data.texPropertyNames.Count; l++)
						{
							Texture texture = null;
							Vector2 one = Vector2.one;
							Vector2 zero = Vector2.zero;
							float texelDens = 0f;
							int isImportedAsNormalMap = 0;
							if (material.HasProperty(data.texPropertyNames[l].name))
							{
								Texture textureConsideringStandardShaderKeywords = MB3_TextureCombinerPipeline.GetTextureConsideringStandardShaderKeywords(data.resultMaterial.shader.name, material, data.texPropertyNames[l].name);
								if (textureConsideringStandardShaderKeywords != null)
								{
									if (!(textureConsideringStandardShaderKeywords is Texture2D))
									{
										Debug.LogError("Object '" + gameObject.name + "' in the list of objects to mesh uses a Texture that is not a Texture2D. Cannot build atlases with this object.");
										result.success = false;
										yield break;
									}
									texture = textureConsideringStandardShaderKeywords;
									TextureFormat format = ((Texture2D)texture).format;
									bool flag2 = false;
									if (!Application.isPlaying && textureEditorMethods != null)
									{
										flag2 = textureEditorMethods.IsNormalMap((Texture2D)texture);
										isImportedAsNormalMap = (flag2 ? -1 : 1);
									}
									if ((format != TextureFormat.ARGB32 && format != TextureFormat.RGBA32 && format != TextureFormat.BGRA32 && format != TextureFormat.RGB24 && format != TextureFormat.Alpha8) || flag2)
									{
										if (Application.isPlaying && data.resultType == MB2_TextureBakeResults.ResultType.atlas && data._packingAlgorithm != MB2_PackingAlgorithmEnum.MeshBakerTexturePacker_Fast && data._packingAlgorithm != MB2_PackingAlgorithmEnum.MeshBakerTexturePaker_Fast_V2_Beta)
										{
											Debug.LogError(string.Concat(new string[]
											{
												"Object ",
												gameObject.name,
												" in the list of objects to mesh uses Texture ",
												texture.name,
												" uses format ",
												format.ToString(),
												" that is not in: ARGB32, RGBA32, BGRA32, RGB24, Alpha8 or DXT. These textures cannot be resized at runtime. Try changing texture format. If format says 'compressed' try changing it to 'truecolor'"
											}));
											result.success = false;
											yield break;
										}
										texture = (Texture2D)material.GetTexture(data.texPropertyNames[l].name);
									}
								}
								if (texture != null && data._normalizeTexelDensity)
								{
									if (array[l].submeshArea == 0f)
									{
										texelDens = 0f;
									}
									else
									{
										texelDens = (float)(texture.width * texture.height) / array[l].submeshArea;
									}
								}
								MB3_TextureCombinerPipeline.GetMaterialScaleAndOffset(material, data.texPropertyNames[l].name, out zero, out one);
							}
							array3[l] = new MeshBakerMaterialTexture(texture, zero, one, texelDens, isImportedAsNormalMap);
						}
						data.nonTexturePropertyBlender.CollectAverageValuesOfNonTextureProperties(data.resultMaterial, material);
						Vector2 vector = new Vector2(array[k].uvRect.width, array[k].uvRect.height);
						Vector2 vector2 = new Vector2(array[k].uvRect.x, array[k].uvRect.y);
						MB_TextureTilingTreatment treatment = MB_TextureTilingTreatment.none;
						if (data._fixOutOfBoundsUVs)
						{
							treatment = MB_TextureTilingTreatment.considerUVs;
						}
						MB_TexSet setOfTexs = new MB_TexSet(array3, vector2, vector, treatment);
						MatAndTransformToMerged item = new MatAndTransformToMerged(new DRect(vector2, vector), data._fixOutOfBoundsUVs, material);
						setOfTexs.matsAndGOs.mats.Add(item);
						MB_TexSet mb_TexSet = data.distinctMaterialTextures.Find((MB_TexSet x) => x.IsEqual(setOfTexs, data._fixOutOfBoundsUVs, data.nonTexturePropertyBlender));
						if (mb_TexSet != null)
						{
							setOfTexs = mb_TexSet;
						}
						else
						{
							data.distinctMaterialTextures.Add(setOfTexs);
						}
						if (!setOfTexs.matsAndGOs.mats.Contains(item))
						{
							setOfTexs.matsAndGOs.mats.Add(item);
						}
						if (!setOfTexs.matsAndGOs.gos.Contains(gameObject))
						{
							setOfTexs.matsAndGOs.gos.Add(gameObject);
							if (!usedObjsToMesh.Contains(gameObject))
							{
								usedObjsToMesh.Add(gameObject);
							}
						}
					}
				}
			}
			if (LOG_LEVEL >= MB2_LogLevel.debug)
			{
				Debug.Log(string.Format("Step1_CollectDistinctTextures collected {0} sets of textures fixOutOfBoundsUV={1} considerNonTextureProperties={2}", data.distinctMaterialTextures.Count, data._fixOutOfBoundsUVs, data._considerNonTextureProperties));
			}
			if (data.distinctMaterialTextures.Count == 0)
			{
				string[] array4 = new string[data.allowedMaterialsFilter.Count];
				for (int m = 0; m < array4.Length; m++)
				{
					array4[m] = data.allowedMaterialsFilter[m].name;
				}
				string text = string.Join(", ", array4);
				string[] array5 = new string[5];
				array5[0] = "None of the materials on the objects to combine matched any of the allowed materials for submesh with result material: ";
				int num3 = 1;
				Material resultMaterial = data.resultMaterial;
				array5[num3] = ((resultMaterial != null) ? resultMaterial.ToString() : null);
				array5[2] = " allowedMaterials: ";
				array5[3] = text;
				array5[4] = ". Do any of the source objects use the allowed materials?";
				Debug.LogError(string.Concat(array5));
				result.success = false;
				yield break;
			}
			MB3_TextureCombinerMerging mb3_TextureCombinerMerging = new MB3_TextureCombinerMerging(data._considerNonTextureProperties, data.nonTexturePropertyBlender, data._fixOutOfBoundsUVs, LOG_LEVEL);
			mb3_TextureCombinerMerging.MergeOverlappingDistinctMaterialTexturesAndCalcMaterialSubrects(data.distinctMaterialTextures);
			if (data.doMergeDistinctMaterialTexturesThatWouldExceedAtlasSize)
			{
				mb3_TextureCombinerMerging.MergeDistinctMaterialTexturesThatWouldExceedMaxAtlasSizeAndCalcMaterialSubrects(data.distinctMaterialTextures, Mathf.Max(data._maxAtlasHeight, data._maxAtlasWidth));
			}
			for (int n = 0; n < data.texPropertyNames.Count; n++)
			{
				ShaderTextureProperty shaderTextureProperty = data.texPropertyNames[n];
				if (shaderTextureProperty.isNormalDontKnow)
				{
					int num4 = 0;
					for (int num5 = 0; num5 < data.distinctMaterialTextures.Count; num5++)
					{
						MeshBakerMaterialTexture meshBakerMaterialTexture = data.distinctMaterialTextures[num5].ts[n];
						num4 += meshBakerMaterialTexture.isImportedAsNormalMap;
					}
					shaderTextureProperty.isNormalMap = (num4 < 0);
					shaderTextureProperty.isNormalDontKnow = false;
				}
			}
			if (LOG_LEVEL >= MB2_LogLevel.debug)
			{
				Debug.Log("Total time Step1_CollectDistinctTextures " + stopwatch.ElapsedMilliseconds.ToString("f5"));
			}
			yield break;
		}

		private static MB3_TextureCombinerPipeline.CreateAtlasForProperty[] CalculateAllTexturesAreNullAndSameColor(MB3_TextureCombinerPipeline.TexturePipelineData data, MB2_LogLevel LOG_LEVEL)
		{
			MB3_TextureCombinerPipeline.CreateAtlasForProperty[] array = new MB3_TextureCombinerPipeline.CreateAtlasForProperty[data.texPropertyNames.Count];
			for (int i = 0; i < data.texPropertyNames.Count; i++)
			{
				MeshBakerMaterialTexture meshBakerMaterialTexture = data.distinctMaterialTextures[0].ts[i];
				Color rhs = Color.black;
				if (data._considerNonTextureProperties)
				{
					rhs = data.nonTexturePropertyBlender.GetColorAsItWouldAppearInAtlasIfNoTexture(data.distinctMaterialTextures[0].matsAndGOs.mats[0].mat, data.texPropertyNames[i]);
				}
				int num = 0;
				int num2 = 0;
				int num3 = 0;
				bool flag = true;
				for (int j = 0; j < data.distinctMaterialTextures.Count; j++)
				{
					MB_TexSet mb_TexSet = data.distinctMaterialTextures[j];
					if (!mb_TexSet.ts[i].isNull)
					{
						num++;
					}
					if (meshBakerMaterialTexture.AreTexturesEqual(mb_TexSet.ts[i]))
					{
						num2++;
					}
					if (data._considerNonTextureProperties && data.nonTexturePropertyBlender.GetColorAsItWouldAppearInAtlasIfNoTexture(mb_TexSet.matsAndGOs.mats[0].mat, data.texPropertyNames[i]) == rhs)
					{
						num3++;
					}
					for (int k = 0; k < mb_TexSet.matsAndGOs.mats.Count; k++)
					{
						flag = !mb_TexSet.matsAndGOs.mats[k].mat.HasProperty(data.texPropertyNames[i].name);
					}
				}
				array[i].allTexturesAreNull = (num == 0);
				array[i].allTexturesAreSame = (num2 == data.distinctMaterialTextures.Count);
				array[i].allNonTexturePropsAreSame = (num3 == data.distinctMaterialTextures.Count);
				MB3_TextureCombinerPipeline.CreateAtlasForProperty[] array2 = array;
				int num4 = i;
				array2[num4].allSrcMatsOmittedTextureProperty = (array2[num4].allSrcMatsOmittedTextureProperty || flag);
				if (LOG_LEVEL >= MB2_LogLevel.trace)
				{
					Debug.Log(string.Format("AllTexturesAreNullAndSameColor prop: {0} createAtlas:{1}  val:{2}", data.texPropertyNames[i].name, MB3_TextureCombinerPipeline._ShouldWeCreateAtlasForThisProperty(i, data._considerNonTextureProperties, array), array[i]));
				}
			}
			return array;
		}

		internal virtual IEnumerator CalculateIdealSizesForTexturesInAtlasAndPadding(ProgressUpdateDelegate progressInfo, MB3_TextureCombiner.CombineTexturesIntoAtlasesCoroutineResult result, MB3_TextureCombinerPipeline.TexturePipelineData data, MB3_TextureCombiner combiner, MB2_EditorMethodsInterface textureEditorMethods, MB2_LogLevel LOG_LEVEL)
		{
			Stopwatch stopwatch = new Stopwatch();
			stopwatch.Start();
			MeshBakerMaterialTexture.readyToBuildAtlases = true;
			data.allTexturesAreNullAndSameColor = MB3_TextureCombinerPipeline.CalculateAllTexturesAreNullAndSameColor(data, LOG_LEVEL);
			if (MB3_MeshCombiner.EVAL_VERSION)
			{
				List<int> list = new List<int>();
				for (int i = 0; i < data.allTexturesAreNullAndSameColor.Length; i++)
				{
					if (MB3_TextureCombinerPipeline._ShouldWeCreateAtlasForThisProperty(i, data._considerNonTextureProperties, data.allTexturesAreNullAndSameColor))
					{
						if ((data.texPropertyNames[i].name.Equals("_Albedo") || data.texPropertyNames[i].name.Equals("_MainTex") || data.texPropertyNames[i].name.Equals("_BaseMap") || data.texPropertyNames[i].name.Equals("_BaseColorMap")) && list.Count < 2)
						{
							list.Add(i);
						}
						if ((data.texPropertyNames[i].name.Equals("_BumpMap") || data.texPropertyNames[i].name.Equals("_Normal") || data.texPropertyNames[i].name.Equals("_NormalMap") || data.texPropertyNames[i].name.Equals("_BentNormalMap")) && list.Count < 2)
						{
							list.Add(i);
						}
					}
				}
				List<string> list2 = new List<string>();
				List<int> list3 = new List<int>();
				for (int j = 0; j < data.allTexturesAreNullAndSameColor.Length; j++)
				{
					if (MB3_TextureCombinerPipeline._ShouldWeCreateAtlasForThisProperty(j, data._considerNonTextureProperties, data.allTexturesAreNullAndSameColor) && list.Count >= 2 && !list.Contains(j))
					{
						list2.Add(data.texPropertyNames[j].name);
						list3.Add(j);
					}
				}
				for (int k = 0; k < list3.Count; k++)
				{
					data.allTexturesAreNullAndSameColor[list3[k]].allTexturesAreNull = true;
					data.allTexturesAreNullAndSameColor[list3[k]].allTexturesAreSame = true;
					data.allTexturesAreNullAndSameColor[list3[k]].allNonTexturePropsAreSame = true;
				}
				if (list2.Count > 0)
				{
					Debug.LogError("The free version of Mesh Baker will generate a maximum of two atlases per combined material. The source materials had more than two properties with textures. Atlases will not be generated for: " + string.Join(",", list2.ToArray()));
				}
			}
			int num = data._atlasPadding_pix;
			if (data.distinctMaterialTextures.Count == 1 && !data._fixOutOfBoundsUVs && !data._considerNonTextureProperties)
			{
				if (LOG_LEVEL >= MB2_LogLevel.info)
				{
					Debug.Log("All objects use the same textures in this set of atlases. Original textures will be reused instead of creating atlases.");
				}
				num = 0;
				data.distinctMaterialTextures[0].SetThisIsOnlyTexSetInAtlasTrue();
				data.distinctMaterialTextures[0].SetTilingTreatmentAndAdjustEncapsulatingSamplingRect(MB_TextureTilingTreatment.edgeToEdgeXY);
			}
			for (int l = 0; l < data.distinctMaterialTextures.Count; l++)
			{
				if (LOG_LEVEL >= MB2_LogLevel.debug)
				{
					Debug.Log("Calculating ideal sizes for texSet TexSet " + l.ToString() + " of " + data.distinctMaterialTextures.Count.ToString());
				}
				MB_TexSet mb_TexSet = data.distinctMaterialTextures[l];
				mb_TexSet.idealWidth_pix = 1;
				mb_TexSet.idealHeight_pix = 1;
				int num2 = 1;
				int num3 = 1;
				for (int m = 0; m < data.texPropertyNames.Count; m++)
				{
					if (MB3_TextureCombinerPipeline._ShouldWeCreateAtlasForThisProperty(m, data._considerNonTextureProperties, data.allTexturesAreNullAndSameColor))
					{
						MeshBakerMaterialTexture meshBakerMaterialTexture = mb_TexSet.ts[m];
						if (LOG_LEVEL >= MB2_LogLevel.trace)
						{
							Debug.Log(string.Format("Calculating ideal size for texSet {0} property {1}", l, data.texPropertyNames[m].name));
						}
						if (!meshBakerMaterialTexture.matTilingRect.size.Equals(Vector2.one) && data.distinctMaterialTextures.Count > 1 && LOG_LEVEL >= MB2_LogLevel.warn)
						{
							Debug.LogWarning(string.Concat(new string[]
							{
								"Texture ",
								meshBakerMaterialTexture.GetTexName(),
								"is tiled by ",
								meshBakerMaterialTexture.matTilingRect.size.ToString(),
								" tiling will be baked into a texture with maxSize:",
								data._maxTilingBakeSize.ToString()
							}));
						}
						if (!mb_TexSet.obUVscale.Equals(Vector2.one) && data.distinctMaterialTextures.Count > 1 && data._fixOutOfBoundsUVs && LOG_LEVEL >= MB2_LogLevel.warn)
						{
							Debug.LogWarning(string.Concat(new string[]
							{
								"Texture ",
								meshBakerMaterialTexture.GetTexName(),
								" has out of bounds UVs that effectively tile by ",
								mb_TexSet.obUVscale.ToString(),
								" tiling will be baked into a texture with maxSize:",
								data._maxTilingBakeSize.ToString()
							}));
						}
						if (meshBakerMaterialTexture.isNull)
						{
							Vector2 adjustedForScaleAndOffset2Dimensions = MB3_TextureCombinerPipeline.GetAdjustedForScaleAndOffset2Dimensions(meshBakerMaterialTexture, mb_TexSet.obUVoffset, mb_TexSet.obUVscale, data, LOG_LEVEL);
							if ((int)(adjustedForScaleAndOffset2Dimensions.x * adjustedForScaleAndOffset2Dimensions.y) > num2 * num3)
							{
								if (LOG_LEVEL >= MB2_LogLevel.trace)
								{
									string[] array = new string[8];
									array[0] = "    matTex ";
									array[1] = meshBakerMaterialTexture.GetTexName();
									array[2] = " ";
									int num4 = 3;
									Vector2 vector = adjustedForScaleAndOffset2Dimensions;
									array[num4] = vector.ToString();
									array[4] = " has a bigger size than ";
									array[5] = num2.ToString();
									array[6] = " ";
									array[7] = num3.ToString();
									Debug.Log(string.Concat(array));
								}
								num2 = (int)adjustedForScaleAndOffset2Dimensions.x;
								num3 = (int)adjustedForScaleAndOffset2Dimensions.y;
							}
							if (LOG_LEVEL >= MB2_LogLevel.trace)
							{
								Debug.Log(string.Format("No source texture creating a 16x16 texture for {0} texSet {1} srcMat {2}", data.texPropertyNames[m].name, l, mb_TexSet.matsAndGOs.mats[0].GetMaterialName()));
							}
						}
						if (!meshBakerMaterialTexture.isNull)
						{
							Vector2 adjustedForScaleAndOffset2Dimensions2 = MB3_TextureCombinerPipeline.GetAdjustedForScaleAndOffset2Dimensions(meshBakerMaterialTexture, mb_TexSet.obUVoffset, mb_TexSet.obUVscale, data, LOG_LEVEL);
							if ((int)(adjustedForScaleAndOffset2Dimensions2.x * adjustedForScaleAndOffset2Dimensions2.y) > num2 * num3)
							{
								if (LOG_LEVEL >= MB2_LogLevel.trace)
								{
									string[] array2 = new string[8];
									array2[0] = "    matTex ";
									array2[1] = meshBakerMaterialTexture.GetTexName();
									array2[2] = " ";
									int num5 = 3;
									Vector2 vector = adjustedForScaleAndOffset2Dimensions2;
									array2[num5] = vector.ToString();
									array2[4] = " has a bigger size than ";
									array2[5] = num2.ToString();
									array2[6] = " ";
									array2[7] = num3.ToString();
									Debug.Log(string.Concat(array2));
								}
								num2 = (int)adjustedForScaleAndOffset2Dimensions2.x;
								num3 = (int)adjustedForScaleAndOffset2Dimensions2.y;
							}
						}
					}
				}
				if (data._resizePowerOfTwoTextures)
				{
					if (num2 <= num * 5)
					{
						Debug.LogWarning(string.Format("Some of the textures have widths close to the size of the padding. It is not recommended to use _resizePowerOfTwoTextures with widths this small.", mb_TexSet.ToString()));
					}
					if (num3 <= num * 5)
					{
						Debug.LogWarning(string.Format("Some of the textures have heights close to the size of the padding. It is not recommended to use _resizePowerOfTwoTextures with heights this small.", mb_TexSet.ToString()));
					}
					if (MB3_TextureCombinerPipeline.IsPowerOfTwo(num2))
					{
						num2 -= num * 2;
					}
					if (MB3_TextureCombinerPipeline.IsPowerOfTwo(num3))
					{
						num3 -= num * 2;
					}
					if (num2 < 1)
					{
						num2 = 1;
					}
					if (num3 < 1)
					{
						num3 = 1;
					}
				}
				if (LOG_LEVEL >= MB2_LogLevel.trace)
				{
					Debug.Log("    Ideal size is " + num2.ToString() + " " + num3.ToString());
				}
				mb_TexSet.idealWidth_pix = num2;
				mb_TexSet.idealHeight_pix = num3;
			}
			data._atlasPadding_pix = num;
			if (LOG_LEVEL >= MB2_LogLevel.debug)
			{
				Debug.Log("Total time Step2 Calculate Ideal Sizes part1: " + stopwatch.Elapsed.ToString());
			}
			yield break;
		}

		internal virtual AtlasPackingResult[] RunTexturePackerOnly(MB3_TextureCombinerPipeline.TexturePipelineData data, bool doSplitIntoMultiAtlasIfTooBig, MB_AtlasesAndRects resultAtlasesAndRects, MB_ITextureCombinerPacker texturePacker, MB2_LogLevel LOG_LEVEL)
		{
			AtlasPackingResult[] array = texturePacker.CalculateAtlasRectangles(data, doSplitIntoMultiAtlasIfTooBig, LOG_LEVEL);
			this.FillAtlasPackingResultAuxillaryData(data, array);
			Texture2D[] atlases = new Texture2D[data.texPropertyNames.Count];
			if (!doSplitIntoMultiAtlasIfTooBig)
			{
				this.FillResultAtlasesAndRects(data, array[0], resultAtlasesAndRects, atlases);
			}
			return array;
		}

		internal virtual MB_ITextureCombinerPacker CreatePacker(bool onlyOneTextureInAtlasReuseTextures, MB2_PackingAlgorithmEnum packingAlgorithm)
		{
			if (onlyOneTextureInAtlasReuseTextures)
			{
				return new MB3_TextureCombinerPackerOneTextureInAtlas();
			}
			if (packingAlgorithm == MB2_PackingAlgorithmEnum.UnitysPackTextures)
			{
				return new MB3_TextureCombinerPackerUnity();
			}
			if (packingAlgorithm == MB2_PackingAlgorithmEnum.MeshBakerTexturePacker_Horizontal)
			{
				if (MB3_TextureCombinerPipeline.USE_EXPERIMENTAL_HOIZONTALVERTICAL)
				{
					return new MB3_TextureCombinerPackerMeshBakerHorizontalVertical(MB3_TextureCombinerPackerMeshBakerHorizontalVertical.AtlasDirection.horizontal);
				}
				return new MB3_TextureCombinerPackerMeshBaker();
			}
			else if (packingAlgorithm == MB2_PackingAlgorithmEnum.MeshBakerTexturePacker_Vertical)
			{
				if (MB3_TextureCombinerPipeline.USE_EXPERIMENTAL_HOIZONTALVERTICAL)
				{
					return new MB3_TextureCombinerPackerMeshBakerHorizontalVertical(MB3_TextureCombinerPackerMeshBakerHorizontalVertical.AtlasDirection.vertical);
				}
				return new MB3_TextureCombinerPackerMeshBaker();
			}
			else
			{
				if (packingAlgorithm == MB2_PackingAlgorithmEnum.MeshBakerTexturePacker)
				{
					return new MB3_TextureCombinerPackerMeshBaker();
				}
				if (packingAlgorithm == MB2_PackingAlgorithmEnum.MeshBakerTexturePaker_Fast_V2_Beta)
				{
					return new MB3_TextureCombinerPackerMeshBakerFastV2();
				}
				if (packingAlgorithm == MB2_PackingAlgorithmEnum.MeshBakerTexturePacker_Fast)
				{
					return new MB3_TextureCombinerPackerMeshBakerFast();
				}
				Debug.LogError("Unknown texture packer type. " + packingAlgorithm.ToString() + " This should never happen.");
				return null;
			}
		}

		internal virtual IEnumerator __Step3_BuildAndSaveAtlasesAndStoreResults(MB3_TextureCombiner.CombineTexturesIntoAtlasesCoroutineResult result, ProgressUpdateDelegate progressInfo, MB3_TextureCombinerPipeline.TexturePipelineData data, MB3_TextureCombiner combiner, MB_ITextureCombinerPacker packer, AtlasPackingResult atlasPackingResult, MB2_EditorMethodsInterface textureEditorMethods, MB_AtlasesAndRects resultAtlasesAndRects, StringBuilder report, MB2_LogLevel LOG_LEVEL)
		{
			Stopwatch sw = new Stopwatch();
			sw.Start();
			GC.Collect();
			Texture2D[] atlases = new Texture2D[data.numAtlases];
			if (LOG_LEVEL >= MB2_LogLevel.debug)
			{
				Debug.Log("time Step 3 Create And Save Atlases part 1 " + sw.Elapsed.ToString());
			}
			MB_TextureCombinerSRPCustom.ConfigureMaterialKeywordsIfNecessary(data);
			yield return packer.CreateAtlases(progressInfo, data, combiner, atlasPackingResult, atlases, textureEditorMethods, LOG_LEVEL);
			float num = (float)sw.ElapsedMilliseconds;
			data.nonTexturePropertyBlender.AdjustNonTextureProperties(data.resultMaterial, data.texPropertyNames, textureEditorMethods);
			if (data.distinctMaterialTextures.Count > 0)
			{
				data.distinctMaterialTextures[0].AdjustResultMaterialNonTextureProperties(data.resultMaterial, data.texPropertyNames);
			}
			if (progressInfo != null)
			{
				progressInfo("Building Report", 0.7f);
			}
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.AppendLine("---- Atlases ------");
			for (int i = 0; i < data.numAtlases; i++)
			{
				if (atlases[i] != null)
				{
					stringBuilder.AppendLine(string.Concat(new string[]
					{
						"Created Atlas For: ",
						data.texPropertyNames[i].name,
						" h=",
						atlases[i].height.ToString(),
						" w=",
						atlases[i].width.ToString()
					}));
				}
				else if (!MB3_TextureCombinerPipeline._ShouldWeCreateAtlasForThisProperty(i, data._considerNonTextureProperties, data.allTexturesAreNullAndSameColor))
				{
					stringBuilder.AppendLine("Did not create atlas for " + data.texPropertyNames[i].name + " because all source textures were null.");
				}
			}
			report.Append(stringBuilder.ToString());
			this.FillResultAtlasesAndRects(data, atlasPackingResult, resultAtlasesAndRects, atlases);
			if (progressInfo != null)
			{
				progressInfo("Restoring Texture Formats & Read Flags", 0.8f);
			}
			combiner._destroyAllTemporaryTextures();
			if (textureEditorMethods != null)
			{
				textureEditorMethods.RestoreReadFlagsAndFormats(progressInfo);
			}
			if (report != null && LOG_LEVEL >= MB2_LogLevel.info)
			{
				Debug.Log(report.ToString());
			}
			if (LOG_LEVEL >= MB2_LogLevel.debug)
			{
				Debug.Log("Time Step 3 Create And Save Atlases part 3 " + ((float)sw.ElapsedMilliseconds - num).ToString("f5"));
			}
			if (LOG_LEVEL >= MB2_LogLevel.debug)
			{
				Debug.Log("Total time Step 3 Create And Save Atlases " + sw.Elapsed.ToString());
			}
			yield break;
		}

		private void FillAtlasPackingResultAuxillaryData(MB3_TextureCombinerPipeline.TexturePipelineData data, AtlasPackingResult[] atlasPackingResults)
		{
			foreach (AtlasPackingResult atlasPackingResult in atlasPackingResults)
			{
				List<MB_MaterialAndUVRect> list = new List<MB_MaterialAndUVRect>();
				for (int j = 0; j < atlasPackingResult.srcImgIdxs.Length; j++)
				{
					int index = atlasPackingResult.srcImgIdxs[j];
					MB_TexSet mb_TexSet = data.distinctMaterialTextures[index];
					List<MatAndTransformToMerged> mats = mb_TexSet.matsAndGOs.mats;
					Rect samplingEncapsulatingRect;
					Rect srcUVsamplingRect;
					mb_TexSet.GetRectsForTextureBakeResults(out samplingEncapsulatingRect, out srcUVsamplingRect);
					for (int k = 0; k < mats.Count; k++)
					{
						Rect materialTilingRectForTextureBakerResults = mb_TexSet.GetMaterialTilingRectForTextureBakerResults(k);
						list.Add(new MB_MaterialAndUVRect(mats[k].mat, atlasPackingResult.rects[j], mb_TexSet.allTexturesUseSameMatTiling, materialTilingRectForTextureBakerResults, samplingEncapsulatingRect, srcUVsamplingRect, mb_TexSet.tilingTreatment, mats[k].objName)
						{
							objectsThatUse = new List<GameObject>(mb_TexSet.matsAndGOs.gos)
						});
					}
				}
				atlasPackingResult.data = list;
			}
		}

		private void FillResultAtlasesAndRects(MB3_TextureCombinerPipeline.TexturePipelineData data, AtlasPackingResult atlasPackingResult, MB_AtlasesAndRects resultAtlasesAndRects, Texture2D[] atlases)
		{
			List<MB_MaterialAndUVRect> list = new List<MB_MaterialAndUVRect>();
			for (int i = 0; i < data.distinctMaterialTextures.Count; i++)
			{
				MB_TexSet mb_TexSet = data.distinctMaterialTextures[i];
				List<MatAndTransformToMerged> mats = mb_TexSet.matsAndGOs.mats;
				Rect samplingEncapsulatingRect;
				Rect srcUVsamplingRect;
				mb_TexSet.GetRectsForTextureBakeResults(out samplingEncapsulatingRect, out srcUVsamplingRect);
				for (int j = 0; j < mats.Count; j++)
				{
					Rect materialTilingRectForTextureBakerResults = mb_TexSet.GetMaterialTilingRectForTextureBakerResults(j);
					MB_MaterialAndUVRect item = new MB_MaterialAndUVRect(mats[j].mat, atlasPackingResult.rects[i], mb_TexSet.allTexturesUseSameMatTiling, materialTilingRectForTextureBakerResults, samplingEncapsulatingRect, srcUVsamplingRect, mb_TexSet.tilingTreatment, mats[j].objName);
					if (!list.Contains(item))
					{
						list.Add(item);
					}
				}
			}
			resultAtlasesAndRects.atlases = atlases;
			resultAtlasesAndRects.texPropertyNames = ShaderTextureProperty.GetNames(data.texPropertyNames);
			resultAtlasesAndRects.mat2rect_map = list;
		}

		internal virtual StringBuilder GenerateReport(MB3_TextureCombinerPipeline.TexturePipelineData data)
		{
			StringBuilder stringBuilder = new StringBuilder();
			if (data.numAtlases > 0)
			{
				stringBuilder = new StringBuilder();
				stringBuilder.AppendLine("Report");
				if (data.texPropNamesToIgnore.Count > 0)
				{
					stringBuilder.Append("Ignoring texture properties: ");
					for (int i = 0; i < data.texPropNamesToIgnore.Count; i++)
					{
						stringBuilder.Append(data.texPropNamesToIgnore[i]);
						stringBuilder.Append(", ");
					}
					stringBuilder.AppendLine();
				}
				for (int j = 0; j < data.distinctMaterialTextures.Count; j++)
				{
					MB_TexSet mb_TexSet = data.distinctMaterialTextures[j];
					stringBuilder.AppendLine("----------");
					stringBuilder.Append(string.Concat(new string[]
					{
						"This set of textures will be a rectangle in the atlas. It will be resized to:",
						mb_TexSet.idealWidth_pix.ToString(),
						"x",
						mb_TexSet.idealHeight_pix.ToString(),
						"\n"
					}));
					for (int k = 0; k < mb_TexSet.ts.Length; k++)
					{
						if (!mb_TexSet.ts[k].isNull)
						{
							stringBuilder.Append(string.Concat(new string[]
							{
								"   [",
								data.texPropertyNames[k].name,
								" ",
								mb_TexSet.ts[k].GetTexName(),
								" ",
								mb_TexSet.ts[k].width.ToString(),
								"x",
								mb_TexSet.ts[k].height.ToString(),
								"]"
							}));
							if (mb_TexSet.ts[k].matTilingRect.size != Vector2.one || mb_TexSet.ts[k].matTilingRect.min != Vector2.zero)
							{
								stringBuilder.AppendFormat(" material scale {0} offset{1} ", mb_TexSet.ts[k].matTilingRect.size.ToString("G4"), mb_TexSet.ts[k].matTilingRect.min.ToString("G4"));
							}
							if (mb_TexSet.obUVscale != Vector2.one || mb_TexSet.obUVoffset != Vector2.zero)
							{
								stringBuilder.AppendFormat(" obUV scale {0} offset{1} ", mb_TexSet.obUVscale.ToString("G4"), mb_TexSet.obUVoffset.ToString("G4"));
							}
							stringBuilder.AppendLine("");
						}
						else
						{
							stringBuilder.Append("   [" + data.texPropertyNames[k].name + " null ");
							if (!MB3_TextureCombinerPipeline._ShouldWeCreateAtlasForThisProperty(k, data._considerNonTextureProperties, data.allTexturesAreNullAndSameColor))
							{
								stringBuilder.Append("no atlas will be created all textures null]\n");
							}
							else
							{
								stringBuilder.AppendFormat("a 16x16 texture will be created]\n", Array.Empty<object>());
							}
						}
					}
					stringBuilder.AppendLine("");
					stringBuilder.Append("Materials using this rectangle:");
					for (int l = 0; l < mb_TexSet.matsAndGOs.mats.Count; l++)
					{
						stringBuilder.Append(mb_TexSet.matsAndGOs.mats[l].mat.name + ", ");
					}
					stringBuilder.AppendLine("");
				}
			}
			return stringBuilder;
		}

		internal static MB2_TexturePacker CreateTexturePacker(MB2_PackingAlgorithmEnum _packingAlgorithm)
		{
			if (_packingAlgorithm == MB2_PackingAlgorithmEnum.MeshBakerTexturePacker)
			{
				return new MB2_TexturePackerRegular();
			}
			if (_packingAlgorithm == MB2_PackingAlgorithmEnum.MeshBakerTexturePacker_Fast)
			{
				return new MB2_TexturePackerRegular();
			}
			if (_packingAlgorithm == MB2_PackingAlgorithmEnum.MeshBakerTexturePaker_Fast_V2_Beta)
			{
				return new MB2_TexturePackerRegular();
			}
			if (_packingAlgorithm == MB2_PackingAlgorithmEnum.MeshBakerTexturePacker_Horizontal)
			{
				return new MB2_TexturePackerHorizontalVert
				{
					packingOrientation = MB2_TexturePackerHorizontalVert.TexturePackingOrientation.horizontal
				};
			}
			if (_packingAlgorithm == MB2_PackingAlgorithmEnum.MeshBakerTexturePacker_Vertical)
			{
				return new MB2_TexturePackerHorizontalVert
				{
					packingOrientation = MB2_TexturePackerHorizontalVert.TexturePackingOrientation.vertical
				};
			}
			Debug.LogError("packing algorithm must be one of the MeshBaker options to create a Texture Packer");
			return null;
		}

		internal static Vector2 GetAdjustedForScaleAndOffset2Dimensions(MeshBakerMaterialTexture source, Vector2 obUVoffset, Vector2 obUVscale, MB3_TextureCombinerPipeline.TexturePipelineData data, MB2_LogLevel LOG_LEVEL)
		{
			if (source.matTilingRect.x == 0.0 && source.matTilingRect.y == 0.0 && source.matTilingRect.width == 1.0 && source.matTilingRect.height == 1.0)
			{
				if (!data._fixOutOfBoundsUVs)
				{
					return new Vector2((float)source.width, (float)source.height);
				}
				if (obUVoffset.x == 0f && obUVoffset.y == 0f && obUVscale.x == 1f && obUVscale.y == 1f)
				{
					return new Vector2((float)source.width, (float)source.height);
				}
			}
			if (LOG_LEVEL >= MB2_LogLevel.debug)
			{
				string[] array = new string[6];
				array[0] = "GetAdjustedForScaleAndOffset2Dimensions: ";
				array[1] = source.GetTexName();
				array[2] = " ";
				int num = 3;
				Vector2 vector = obUVoffset;
				array[num] = vector.ToString();
				array[4] = " ";
				int num2 = 5;
				vector = obUVscale;
				array[num2] = vector.ToString();
				Debug.Log(string.Concat(array));
			}
			Rect rect = source.GetEncapsulatingSamplingRect().GetRect();
			float num3 = rect.width * (float)source.width;
			float num4 = rect.height * (float)source.height;
			if (num3 > (float)data._maxTilingBakeSize)
			{
				num3 = (float)data._maxTilingBakeSize;
			}
			if (num4 > (float)data._maxTilingBakeSize)
			{
				num4 = (float)data._maxTilingBakeSize;
			}
			if (num3 < 1f)
			{
				num3 = 1f;
			}
			if (num4 < 1f)
			{
				num4 = 1f;
			}
			return new Vector2(num3, num4);
		}

		internal static Color32 ConvertNormalFormatFromUnity_ToStandard(Color32 c)
		{
			Vector3 zero = Vector3.zero;
			zero.x = (float)c.a * 2f - 1f;
			zero.y = (float)c.g * 2f - 1f;
			zero.z = Mathf.Sqrt(1f - zero.x * zero.x - zero.y * zero.y);
			return new Color32
			{
				a = 1,
				r = (byte)((zero.x + 1f) * 0.5f),
				g = (byte)((zero.y + 1f) * 0.5f),
				b = (byte)((zero.z + 1f) * 0.5f)
			};
		}

		internal static void GetMaterialScaleAndOffset(Material mat, string propertyName, out Vector2 offset, out Vector2 scale)
		{
			if (mat == null)
			{
				Debug.LogError("Material was null. Should never happen.");
				offset = Vector2.zero;
				scale = Vector2.one;
				return;
			}
			MB3_ShadersThatShareTiling.GetScaleAndOffsetForTextureProp(mat, propertyName, out offset, out scale);
		}

		internal static float GetSubmeshArea(Mesh m, int submeshIdx)
		{
			if (submeshIdx >= m.subMeshCount || submeshIdx < 0)
			{
				return 0f;
			}
			Vector3[] vertices = m.vertices;
			int[] indices = m.GetIndices(submeshIdx);
			float num = 0f;
			for (int i = 0; i < indices.Length; i += 3)
			{
				Vector3 b = vertices[indices[i]];
				Vector3 a = vertices[indices[i + 1]];
				Vector3 a2 = vertices[indices[i + 2]];
				num += Vector3.Cross(a - b, a2 - b).magnitude / 2f;
			}
			return num;
		}

		internal static bool IsPowerOfTwo(int x)
		{
			return (x & x - 1) == 0;
		}

		public static bool USE_EXPERIMENTAL_HOIZONTALVERTICAL = true;

		public static ShaderTextureProperty[] shaderTexPropertyNames = new ShaderTextureProperty[]
		{
			new ShaderTextureProperty("_MainTex", false, true, false),
			new ShaderTextureProperty("_BaseMap", false, true, false),
			new ShaderTextureProperty("_BaseColorMap", false, true, false),
			new ShaderTextureProperty("_BumpMap", true, false, false),
			new ShaderTextureProperty("_Normal", true, false, false),
			new ShaderTextureProperty("_NormalMap", true, false, false),
			new ShaderTextureProperty("_BumpSpecMap", false, true, false),
			new ShaderTextureProperty("_DecalTex", false, true, false),
			new ShaderTextureProperty("_MaskMap", false, false, false),
			new ShaderTextureProperty("_BentNormalMap", false, false, false),
			new ShaderTextureProperty("_TangentMap", false, false, false),
			new ShaderTextureProperty("_AnisotropyMap", false, false, false),
			new ShaderTextureProperty("_SubsurfaceMaskMap", false, false, false),
			new ShaderTextureProperty("_ThicknessMap", false, false, false),
			new ShaderTextureProperty("_IridescenceThicknessMap", false, false, false),
			new ShaderTextureProperty("_IridescenceMaskMap", false, false, false),
			new ShaderTextureProperty("_SpecularColorMap", false, true, true),
			new ShaderTextureProperty("_EmissiveColorMap", false, true, false),
			new ShaderTextureProperty("_DistortionVectorMap", false, false, false),
			new ShaderTextureProperty("_TransmittanceColorMap", false, true, false),
			new ShaderTextureProperty("_Detail", false, true, false),
			new ShaderTextureProperty("_GlossMap", false, true, false),
			new ShaderTextureProperty("_Illum", false, true, false),
			new ShaderTextureProperty("_LightTextureB0", false, true, false),
			new ShaderTextureProperty("_ParallaxMap", false, false, false),
			new ShaderTextureProperty("_ShadowOffset", false, true, false),
			new ShaderTextureProperty("_TranslucencyMap", false, true, false),
			new ShaderTextureProperty("_SpecMap", false, true, false),
			new ShaderTextureProperty("_SpecGlossMap", false, false, false),
			new ShaderTextureProperty("_TranspMap", false, false, true),
			new ShaderTextureProperty("_MetallicGlossMap", false, false, true),
			new ShaderTextureProperty("_OcclusionMap", false, true, false),
			new ShaderTextureProperty("_EmissionMap", false, true, false),
			new ShaderTextureProperty("_DetailMask", false, false, false)
		};

		public struct CreateAtlasForProperty
		{
			public override string ToString()
			{
				return string.Format("AllTexturesNull={0} areSame={1} nonTexPropsAreSame={2} allSrcMatsOmittedTextureProperty={3}", new object[]
				{
					this.allTexturesAreNull,
					this.allTexturesAreSame,
					this.allNonTexturePropsAreSame,
					this.allSrcMatsOmittedTextureProperty
				});
			}

			public bool allTexturesAreNull;

			public bool allTexturesAreSame;

			public bool allNonTexturePropsAreSame;

			public bool allSrcMatsOmittedTextureProperty;
		}

		internal class TexturePipelineData
		{
			internal int numAtlases
			{
				get
				{
					if (this.texPropertyNames != null)
					{
						return this.texPropertyNames.Count;
					}
					return 0;
				}
			}

			internal bool OnlyOneTextureInAtlasReuseTextures()
			{
				return this.distinctMaterialTextures != null && this.distinctMaterialTextures.Count == 1 && this.distinctMaterialTextures[0].thisIsOnlyTexSetInAtlas && !this._fixOutOfBoundsUVs && !this._considerNonTextureProperties;
			}

			internal MB2_TextureBakeResults _textureBakeResults;

			internal int _atlasPadding_pix = 1;

			internal int _maxAtlasWidth = 1;

			internal int _maxAtlasHeight = 1;

			internal bool _useMaxAtlasHeightOverride;

			internal bool _useMaxAtlasWidthOverride;

			internal bool _resizePowerOfTwoTextures;

			internal bool _fixOutOfBoundsUVs;

			internal int _maxTilingBakeSize = 1024;

			internal bool _saveAtlasesAsAssets;

			internal MB2_PackingAlgorithmEnum _packingAlgorithm;

			internal int _layerTexturePackerFastV2 = -1;

			internal bool _meshBakerTexturePackerForcePowerOfTwo = true;

			internal List<ShaderTextureProperty> _customShaderPropNames = new List<ShaderTextureProperty>();

			internal bool _normalizeTexelDensity;

			internal bool _considerNonTextureProperties;

			internal bool doMergeDistinctMaterialTexturesThatWouldExceedAtlasSize;

			internal ColorSpace colorSpace;

			internal MB3_TextureCombinerNonTextureProperties nonTexturePropertyBlender;

			internal List<MB_TexSet> distinctMaterialTextures;

			internal List<GameObject> allObjsToMesh;

			internal List<Material> allowedMaterialsFilter;

			internal List<ShaderTextureProperty> texPropertyNames;

			internal List<string> texPropNamesToIgnore;

			internal MB3_TextureCombinerPipeline.CreateAtlasForProperty[] allTexturesAreNullAndSameColor;

			internal MB2_TextureBakeResults.ResultType resultType;

			internal Material resultMaterial;
		}
	}
}
