using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;

public class ftLightmaps
{
	static ftLightmaps()
	{
		SceneManager.activeSceneChanged -= ftLightmaps.OnSceneChangedPlay;
		SceneManager.activeSceneChanged += ftLightmaps.OnSceneChangedPlay;
	}

	private static void SetDirectionalMode()
	{
		if (ftLightmaps.directionalMode >= 0)
		{
			LightmapSettings.lightmapsMode = ((ftLightmaps.directionalMode == 1) ? LightmapsMode.CombinedDirectional : LightmapsMode.NonDirectional);
		}
	}

	private static void OnSceneChangedPlay(Scene prev, Scene next)
	{
		ftLightmaps.SetDirectionalMode();
	}

	public static void RefreshFull()
	{
		Scene activeScene = SceneManager.GetActiveScene();
		int sceneCount = SceneManager.sceneCount;
		for (int i = 0; i < sceneCount; i++)
		{
			Scene sceneAt = SceneManager.GetSceneAt(i);
			if (sceneAt.isLoaded)
			{
				SceneManager.SetActiveScene(sceneAt);
				LightmapSettings.lightmaps = new LightmapData[0];
			}
		}
		for (int j = 0; j < sceneCount; j++)
		{
			ftLightmaps.RefreshScene(SceneManager.GetSceneAt(j), null, true);
		}
		SceneManager.SetActiveScene(activeScene);
	}

	public static GameObject FindInScene(string nm, Scene scn)
	{
		GameObject[] rootGameObjects = scn.GetRootGameObjects();
		for (int i = 0; i < rootGameObjects.Length; i++)
		{
			if (rootGameObjects[i].name == nm)
			{
				return rootGameObjects[i];
			}
			Transform transform = rootGameObjects[i].transform.Find(nm);
			if (transform != null)
			{
				return transform.gameObject;
			}
		}
		return null;
	}

	private static Texture2D GetEmptyDirectionTex(ftLightmapsStorage storage)
	{
		return storage.emptyDirectionTex;
	}

	public static void RefreshScene(Scene scene, ftLightmapsStorage storage = null, bool updateNonBaked = false)
	{
		int sceneCount = SceneManager.sceneCount;
		if (ftLightmaps.globalMapsAdditional == null)
		{
			ftLightmaps.globalMapsAdditional = new List<ftLightmaps.LightmapAdditionalData>();
		}
		List<LightmapData> list = new List<LightmapData>();
		List<ftLightmaps.LightmapAdditionalData> list2 = new List<ftLightmaps.LightmapAdditionalData>();
		LightmapData[] lightmaps = LightmapSettings.lightmaps;
		List<ftLightmaps.LightmapAdditionalData> list3 = ftLightmaps.globalMapsAdditional;
		if (storage == null)
		{
			if (!scene.isLoaded)
			{
				return;
			}
			SceneManager.SetActiveScene(scene);
			GameObject gameObject = ftLightmaps.FindInScene("!ftraceLightmaps", scene);
			if (gameObject == null)
			{
				return;
			}
			storage = gameObject.GetComponent<ftLightmapsStorage>();
			if (storage == null)
			{
				return;
			}
		}
		if (storage.idremap == null || storage.idremap.Length != storage.maps.Count)
		{
			storage.idremap = new int[storage.maps.Count];
		}
		ftLightmaps.directionalMode = ((storage.dirMaps.Count != 0) ? 1 : 0);
		bool flag = false;
		ftLightmaps.SetDirectionalMode();
		if (ftLightmaps.directionalMode == 1)
		{
			for (int i = 0; i < lightmaps.Length; i++)
			{
				if (lightmaps[i].lightmapDir == null)
				{
					LightmapData lightmapData = lightmaps[i];
					lightmapData.lightmapDir = ftLightmaps.GetEmptyDirectionTex(storage);
					lightmaps[i] = lightmapData;
					flag = true;
				}
			}
		}
		bool flag2 = false;
		if (lightmaps.Length == storage.maps.Count)
		{
			flag2 = true;
			for (int j = 0; j < storage.maps.Count; j++)
			{
				if (lightmaps[j].lightmapColor != storage.maps[j])
				{
					flag2 = false;
					break;
				}
				if (storage.rnmMaps0.Count > j && (list3.Count <= j || list3[j].rnm0 != storage.rnmMaps0[j]))
				{
					flag2 = false;
					break;
				}
			}
		}
		if (!flag2)
		{
			if (sceneCount >= 1)
			{
				for (int k = 0; k < lightmaps.Length; k++)
				{
					if ((lightmaps[k] != null && (!(lightmaps[k].lightmapColor == null) || !(lightmaps[k].shadowMask == null))) || (k != 0 && k != lightmaps.Length - 1))
					{
						list.Add(lightmaps[k]);
						if (list3.Count > k)
						{
							list2.Add(list3[k]);
						}
					}
				}
			}
			for (int l = 0; l < storage.maps.Count; l++)
			{
				Texture2D texture2D = storage.maps[l];
				Texture2D texture2D2 = null;
				Texture2D texture2D3 = null;
				Texture2D texture2D4 = null;
				Texture2D rnm = null;
				Texture2D rnm2 = null;
				int mode = 0;
				if (storage.masks.Count > l)
				{
					texture2D2 = storage.masks[l];
				}
				if (storage.dirMaps.Count > l)
				{
					texture2D3 = storage.dirMaps[l];
				}
				if (storage.rnmMaps0.Count > l)
				{
					texture2D4 = storage.rnmMaps0[l];
					rnm = storage.rnmMaps1[l];
					rnm2 = storage.rnmMaps2[l];
					mode = storage.mapsMode[l];
				}
				bool flag3 = false;
				int num = -1;
				int m = 0;
				while (m < list.Count)
				{
					if (list[m].lightmapColor == texture2D && list[m].shadowMask == texture2D2)
					{
						storage.idremap[l] = m;
						flag3 = true;
						if (texture2D4 != null)
						{
							if (list2.Count > m)
							{
								if (!(list2[m].rnm0 == null))
								{
									break;
								}
							}
							while (list2.Count <= m)
							{
								list2.Add(default(ftLightmaps.LightmapAdditionalData));
							}
							list2[m] = new ftLightmaps.LightmapAdditionalData
							{
								rnm0 = texture2D4,
								rnm1 = rnm,
								rnm2 = rnm2,
								mode = mode
							};
							break;
						}
						break;
					}
					else
					{
						if (num < 0 && list[m].lightmapColor == null && list[m].shadowMask == null)
						{
							storage.idremap[l] = m;
							num = m;
						}
						m++;
					}
				}
				if (!flag3)
				{
					LightmapData lightmapData2;
					if (num >= 0)
					{
						lightmapData2 = list[num];
					}
					else
					{
						lightmapData2 = new LightmapData();
					}
					lightmapData2.lightmapColor = texture2D;
					if (storage.masks.Count > l)
					{
						lightmapData2.shadowMask = texture2D2;
					}
					if (storage.dirMaps.Count > l && texture2D3 != null)
					{
						lightmapData2.lightmapDir = texture2D3;
					}
					else if (ftLightmaps.directionalMode == 1)
					{
						lightmapData2.lightmapDir = ftLightmaps.GetEmptyDirectionTex(storage);
					}
					if (num < 0)
					{
						list.Add(lightmapData2);
						storage.idremap[l] = list.Count - 1;
					}
					else
					{
						list[num] = lightmapData2;
					}
					if (storage.rnmMaps0.Count > l)
					{
						ftLightmaps.LightmapAdditionalData lightmapAdditionalData = default(ftLightmaps.LightmapAdditionalData);
						lightmapAdditionalData.rnm0 = texture2D4;
						lightmapAdditionalData.rnm1 = rnm;
						lightmapAdditionalData.rnm2 = rnm2;
						lightmapAdditionalData.mode = mode;
						if (num < 0)
						{
							while (list2.Count < list.Count - 1)
							{
								list2.Add(default(ftLightmaps.LightmapAdditionalData));
							}
							list2.Add(lightmapAdditionalData);
						}
						else
						{
							while (list2.Count < num + 1)
							{
								list2.Add(default(ftLightmaps.LightmapAdditionalData));
							}
							list2[num] = lightmapAdditionalData;
						}
					}
				}
			}
		}
		else
		{
			for (int n = 0; n < storage.maps.Count; n++)
			{
				storage.idremap[n] = n;
			}
		}
		if (flag2 && flag)
		{
			LightmapSettings.lightmaps = lightmaps;
		}
		if (!flag2)
		{
			LightmapSettings.lightmaps = list.ToArray();
			ftLightmaps.globalMapsAdditional = list2;
		}
		if (RenderSettings.ambientMode == AmbientMode.Skybox)
		{
			SphericalHarmonicsL2 ambientProbe = RenderSettings.ambientProbe;
			int num2 = -1;
			for (int num3 = 0; num3 < 3; num3++)
			{
				for (int num4 = 0; num4 < 9; num4++)
				{
					float num5 = Mathf.Abs(ambientProbe[num3, num4]);
					if (num5 > 1000f || num5 < 1E-06f)
					{
						num2 = 1;
						break;
					}
					if (ambientProbe[num3, num4] != 0f)
					{
						num2 = 0;
						break;
					}
				}
				if (num2 >= 0)
				{
					break;
				}
			}
			if (num2 != 0)
			{
				DynamicGI.UpdateEnvironment();
			}
		}
		Vector4 vector = new Vector4(1f, 1f, 0f, 0f);
		for (int num6 = 0; num6 < storage.bakedRenderers.Count; num6++)
		{
			Renderer renderer = storage.bakedRenderers[num6];
			if (!(renderer == null))
			{
				int num7 = storage.bakedIDs[num6];
				Mesh mesh = null;
				if (num6 < storage.bakedVertexColorMesh.Count)
				{
					mesh = storage.bakedVertexColorMesh[num6];
				}
				if (mesh != null)
				{
					MeshRenderer meshRenderer = renderer as MeshRenderer;
					if (meshRenderer == null)
					{
						Debug.LogError("Unity cannot use additionalVertexStreams on non-MeshRenderer");
					}
					else
					{
						meshRenderer.additionalVertexStreams = mesh;
						meshRenderer.lightmapIndex = 65535;
						MaterialPropertyBlock materialPropertyBlock = new MaterialPropertyBlock();
						materialPropertyBlock.SetFloat("bakeryLightmapMode", 1f);
						meshRenderer.SetPropertyBlock(materialPropertyBlock);
					}
				}
				else
				{
					int num8 = (num7 < 0 || num7 >= storage.idremap.Length) ? num7 : storage.idremap[num7];
					renderer.lightmapIndex = num8;
					if (!renderer.isPartOfStaticBatch)
					{
						Vector4 lightmapScaleOffset = (num7 < 0) ? vector : storage.bakedScaleOffset[num6];
						renderer.lightmapScaleOffset = lightmapScaleOffset;
					}
					if (renderer.lightmapIndex >= 0 && num8 < ftLightmaps.globalMapsAdditional.Count)
					{
						ftLightmaps.LightmapAdditionalData lightmapAdditionalData2 = ftLightmaps.globalMapsAdditional[num8];
						if (lightmapAdditionalData2.rnm0 != null)
						{
							MaterialPropertyBlock materialPropertyBlock2 = new MaterialPropertyBlock();
							materialPropertyBlock2.SetTexture("_RNM0", lightmapAdditionalData2.rnm0);
							materialPropertyBlock2.SetTexture("_RNM1", lightmapAdditionalData2.rnm1);
							materialPropertyBlock2.SetTexture("_RNM2", lightmapAdditionalData2.rnm2);
							materialPropertyBlock2.SetFloat("bakeryLightmapMode", (float)lightmapAdditionalData2.mode);
							renderer.SetPropertyBlock(materialPropertyBlock2);
						}
					}
				}
			}
		}
		if (updateNonBaked)
		{
			for (int num9 = 0; num9 < storage.nonBakedRenderers.Count; num9++)
			{
				Renderer renderer2 = storage.nonBakedRenderers[num9];
				if (!(renderer2 == null) && !renderer2.isPartOfStaticBatch)
				{
					renderer2.lightmapIndex = 65534;
				}
			}
		}
		for (int num10 = 0; num10 < storage.bakedRenderersTerrain.Count; num10++)
		{
			Terrain terrain = storage.bakedRenderersTerrain[num10];
			if (!(terrain == null))
			{
				int num11 = storage.bakedIDsTerrain[num10];
				terrain.lightmapIndex = ((num11 < 0 || num11 >= storage.idremap.Length) ? num11 : storage.idremap[num11]);
				Vector4 lightmapScaleOffset2 = (num11 < 0) ? vector : storage.bakedScaleOffsetTerrain[num10];
				terrain.lightmapScaleOffset = lightmapScaleOffset2;
				if (terrain.lightmapIndex >= 0 && terrain.lightmapIndex < ftLightmaps.globalMapsAdditional.Count)
				{
					ftLightmaps.LightmapAdditionalData lightmapAdditionalData3 = ftLightmaps.globalMapsAdditional[terrain.lightmapIndex];
					if (lightmapAdditionalData3.rnm0 != null)
					{
						MaterialPropertyBlock materialPropertyBlock3 = new MaterialPropertyBlock();
						materialPropertyBlock3.SetTexture("_RNM0", lightmapAdditionalData3.rnm0);
						materialPropertyBlock3.SetTexture("_RNM1", lightmapAdditionalData3.rnm1);
						materialPropertyBlock3.SetTexture("_RNM2", lightmapAdditionalData3.rnm2);
						materialPropertyBlock3.SetFloat("bakeryLightmapMode", (float)lightmapAdditionalData3.mode);
						terrain.SetSplatMaterialPropertyBlock(materialPropertyBlock3);
					}
				}
			}
		}
		for (int num12 = 0; num12 < storage.bakedLights.Count; num12++)
		{
			if (!(storage.bakedLights[num12] == null))
			{
				int num13 = storage.bakedLightChannels[num12];
				LightBakingOutput bakingOutput = default(LightBakingOutput);
				bakingOutput.isBaked = true;
				if (num13 < 0)
				{
					bakingOutput.lightmapBakeType = LightmapBakeType.Baked;
				}
				else
				{
					bakingOutput.lightmapBakeType = LightmapBakeType.Mixed;
					bakingOutput.mixedLightingMode = ((num13 > 100) ? MixedLightingMode.Subtractive : MixedLightingMode.Shadowmask);
					bakingOutput.occlusionMaskChannel = ((num13 > 100) ? -1 : num13);
					bakingOutput.probeOcclusionLightIndex = storage.bakedLights[num12].bakingOutput.probeOcclusionLightIndex;
				}
				storage.bakedLights[num12].bakingOutput = bakingOutput;
			}
		}
		if (ftLightmaps.lightmapRefCount == null)
		{
			ftLightmaps.lightmapRefCount = new List<int>();
		}
		for (int num14 = 0; num14 < storage.idremap.Length; num14++)
		{
			int num15 = storage.idremap[num14];
			while (ftLightmaps.lightmapRefCount.Count <= num15)
			{
				ftLightmaps.lightmapRefCount.Add(0);
			}
			if (ftLightmaps.lightmapRefCount[num15] < 0)
			{
				ftLightmaps.lightmapRefCount[num15] = 0;
			}
			List<int> list4 = ftLightmaps.lightmapRefCount;
			int index = num15;
			int num16 = list4[index];
			list4[index] = num16 + 1;
		}
	}

	public static void UnloadScene(ftLightmapsStorage storage)
	{
		if (ftLightmaps.lightmapRefCount == null)
		{
			return;
		}
		if (storage.idremap == null)
		{
			return;
		}
		LightmapData[] array = null;
		List<ftLightmaps.LightmapAdditionalData> list = null;
		for (int i = 0; i < storage.idremap.Length; i++)
		{
			int num = storage.idremap[i];
			if (num != 0 && ftLightmaps.lightmapRefCount.Count > num)
			{
				List<int> list2 = ftLightmaps.lightmapRefCount;
				int index = num;
				int num2 = list2[index];
				list2[index] = num2 - 1;
				if (ftLightmaps.lightmapRefCount[num] == 0)
				{
					if (array == null)
					{
						array = LightmapSettings.lightmaps;
					}
					if (array.Length > num)
					{
						array[num].lightmapColor = null;
						array[num].lightmapDir = null;
						array[num].shadowMask = null;
						if (list == null)
						{
							list = ftLightmaps.globalMapsAdditional;
						}
						if (list != null && list.Count > num)
						{
							list[num] = default(ftLightmaps.LightmapAdditionalData);
						}
					}
				}
			}
		}
		if (array != null)
		{
			LightmapSettings.lightmaps = array;
		}
	}

	public static void RefreshScene2(Scene scene, ftLightmapsStorage storage)
	{
		for (int i = 0; i < storage.bakedRenderers.Count; i++)
		{
			Renderer renderer = storage.bakedRenderers[i];
			if (!(renderer == null))
			{
				int num = storage.bakedIDs[i];
				renderer.lightmapIndex = ((num < 0 || num >= storage.idremap.Length) ? num : storage.idremap[num]);
			}
		}
		for (int j = 0; j < storage.bakedRenderersTerrain.Count; j++)
		{
			Terrain terrain = storage.bakedRenderersTerrain[j];
			if (!(terrain == null))
			{
				int num = storage.bakedIDsTerrain[j];
				terrain.lightmapIndex = ((num < 0 || num >= storage.idremap.Length) ? num : storage.idremap[num]);
			}
		}
		if (storage.anyVolumes)
		{
			if (storage.compressedVolumes)
			{
				Shader.EnableKeyword("BAKERY_COMPRESSED_VOLUME");
				return;
			}
			Shader.DisableKeyword("BAKERY_COMPRESSED_VOLUME");
		}
	}

	private static List<int> lightmapRefCount;

	private static List<ftLightmaps.LightmapAdditionalData> globalMapsAdditional;

	private static int directionalMode;

	private struct LightmapAdditionalData
	{
		public Texture2D rnm0;

		public Texture2D rnm1;

		public Texture2D rnm2;

		public int mode;
	}
}
