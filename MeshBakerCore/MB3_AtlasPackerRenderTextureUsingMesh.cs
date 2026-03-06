using System;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

namespace DigitalOpus.MB.Core
{
	public class MB3_AtlasPackerRenderTextureUsingMesh
	{
		public void Initialize(int camMaskLayer, int width, int height, int padding, MB2_LogLevel LOG_LEVEL = MB2_LogLevel.info)
		{
			this.camMaskLayer = camMaskLayer;
			this.width = width;
			this.height = height;
			this.padding = padding;
			this.LOG_LEVEL = LOG_LEVEL;
			this._initialized = true;
		}

		internal void SetupCameraGameObject(GameObject camGameObject)
		{
			LayerMask mask = 1 << this.camMaskLayer;
			Camera camera = camGameObject.AddComponent<Camera>();
			camera.enabled = false;
			camera.orthographic = true;
			camera.orthographicSize = (float)this.height / 2f;
			camera.aspect = (float)this.width / (float)this.height;
			camera.rect = new Rect(0f, 0f, 1f, 1f);
			camera.clearFlags = CameraClearFlags.Color;
			camera.cullingMask = mask;
			Transform component = camera.GetComponent<Transform>();
			component.localPosition = new Vector3((float)this.width / 2f, (float)this.height / 2f, 0f);
			component.localRotation = Quaternion.Euler(0f, 0f, 0f);
			MBVersion.DoSpecialRenderPipeline_TexturePackerFastSetup(camGameObject);
			this._camSetup = true;
		}

		internal Texture2D DoRenderAtlas(GameObject go, int width, int height, bool isNormalMap, ShaderTextureProperty propertyName)
		{
			Stopwatch stopwatch = new Stopwatch();
			stopwatch.Start();
			RenderTexture renderTexture;
			if (isNormalMap)
			{
				renderTexture = new RenderTexture(width, height, 24, RenderTextureFormat.ARGB32, RenderTextureReadWrite.sRGB);
			}
			else
			{
				renderTexture = new RenderTexture(width, height, 24, RenderTextureFormat.ARGB32, RenderTextureReadWrite.sRGB);
			}
			renderTexture.filterMode = FilterMode.Point;
			Camera component = go.GetComponent<Camera>();
			component.targetTexture = renderTexture;
			if (this.LOG_LEVEL >= MB2_LogLevel.debug)
			{
				Debug.Log(string.Format("Begin Camera.Render destTex w={0} h={1} camPos={2} camSize={3} camAspect={4}", new object[]
				{
					width,
					height,
					go.transform.localPosition,
					component.orthographicSize,
					component.aspect.ToString("f5")
				}));
			}
			component.Render();
			Stopwatch stopwatch2 = new Stopwatch();
			stopwatch2.Start();
			Texture2D texture2D = new Texture2D(renderTexture.width, renderTexture.height, TextureFormat.ARGB32, true, false);
			MB_TextureCombinerRenderTexture.ConvertRenderTextureToTexture2D(renderTexture, MB_TextureCombinerRenderTexture.YisFlipped(this.LOG_LEVEL), isNormalMap, this.LOG_LEVEL, texture2D);
			if (this.LOG_LEVEL >= MB2_LogLevel.trace)
			{
				Debug.Log(string.Concat(new string[]
				{
					"Finished rendering atlas ",
					propertyName.name,
					"  db_time_DoRenderAtlas:",
					((float)stopwatch.ElapsedMilliseconds * 0.001f).ToString(),
					"  db_ConvertRenderTextureToTexture2D:",
					((float)stopwatch2.ElapsedMilliseconds * 0.001f).ToString()
				}));
			}
			MB_Utility.Destroy(renderTexture);
			return texture2D;
		}

		public int camMaskLayer;

		public int width;

		public int height;

		public int padding;

		public MB2_LogLevel LOG_LEVEL = MB2_LogLevel.info;

		private bool _initialized;

		private bool _camSetup;

		public class MeshRectInfo
		{
			public int vertIdx;

			public int triIdx;

			public int atlasIdx;
		}

		public class MeshAtlas
		{
			internal static void BuildAtlas(AtlasPackingResult packedAtlasRects, List<MB_TexSet> distinctMaterialTextures, int propIdx, int atlasSizeX, int atlasSizeY, Mesh m, List<Material> generatedMats, ShaderTextureProperty property, MB3_TextureCombinerPipeline.TexturePipelineData data, MB3_TextureCombiner combiner, MB2_EditorMethodsInterface textureEditorMethods, MB2_LogLevel LOG_LEVEL)
			{
				generatedMats.Clear();
				List<Vector3> list = new List<Vector3>();
				List<Vector2> list2 = new List<Vector2>();
				List<int>[] array = new List<int>[distinctMaterialTextures.Count];
				for (int i = 0; i < array.Length; i++)
				{
					array[i] = new List<int>();
				}
				MeshBakerMaterialTexture.readyToBuildAtlases = true;
				GC.Collect();
				MB3_TextureCombinerPackerRoot.CreateTemporaryTexturesForAtlas(data.distinctMaterialTextures, combiner, propIdx, data);
				Rect[] rects = packedAtlasRects.rects;
				for (int j = 0; j < distinctMaterialTextures.Count; j++)
				{
					MB_TexSet mb_TexSet = distinctMaterialTextures[j];
					MeshBakerMaterialTexture meshBakerMaterialTexture = mb_TexSet.ts[propIdx];
					if (LOG_LEVEL >= MB2_LogLevel.trace)
					{
						Debug.Log(string.Format("Adding texture {0} to atlas {1} for texSet {2} srcMat {3}", new object[]
						{
							meshBakerMaterialTexture.GetTexName(),
							property.name,
							j,
							mb_TexSet.matsAndGOs.mats[0].GetMaterialName()
						}));
					}
					Rect rect = rects[j];
					Texture2D texture2D = meshBakerMaterialTexture.GetTexture2D();
					int num = Mathf.RoundToInt(rect.x * (float)atlasSizeX);
					int num2 = Mathf.RoundToInt(rect.y * (float)atlasSizeY);
					int num3 = Mathf.RoundToInt(rect.width * (float)atlasSizeX);
					int num4 = Mathf.RoundToInt(rect.height * (float)atlasSizeY);
					rect = new Rect((float)num, (float)num2, (float)num3, (float)num4);
					if (num3 == 0 || num4 == 0)
					{
						string str = "Image in atlas has no height or width ";
						Rect rect2 = rect;
						Debug.LogError(str + rect2.ToString());
					}
					DRect encapsulatingSamplingRect = mb_TexSet.ts[propIdx].GetEncapsulatingSamplingRect();
					AtlasPadding atlasPadding = packedAtlasRects.padding[j];
					MB3_AtlasPackerRenderTextureUsingMesh.MeshAtlas.AddNineSlicedRect(rect, (float)atlasPadding.leftRight, (float)atlasPadding.topBottom, encapsulatingSamplingRect.GetRect(), list, list2, array[j], (float)texture2D.width, (float)texture2D.height, texture2D.name);
					Material material = new Material(Shader.Find("MeshBaker/Unlit/UnlitWithAlpha"));
					bool isSavingAsANormalMapAssetThatWillBeImported = property.isNormalMap && data._saveAtlasesAsAssets;
					MBVersion.PipelineType pipelineType = MBVersion.DetectPipeline();
					if (pipelineType == MBVersion.PipelineType.URP)
					{
						MB3_AtlasPackerRenderTextureUsingMesh.MeshAtlas.ConfigureMaterial_DefaultPipeline(material, texture2D, isSavingAsANormalMapAssetThatWillBeImported, LOG_LEVEL);
					}
					else if (pipelineType == MBVersion.PipelineType.HDRP)
					{
						MB3_AtlasPackerRenderTextureUsingMesh.MeshAtlas.ConfigureMaterial_DefaultPipeline(material, texture2D, isSavingAsANormalMapAssetThatWillBeImported, LOG_LEVEL);
					}
					else
					{
						MB3_AtlasPackerRenderTextureUsingMesh.MeshAtlas.ConfigureMaterial_DefaultPipeline(material, texture2D, isSavingAsANormalMapAssetThatWillBeImported, LOG_LEVEL);
					}
					generatedMats.Add(material);
				}
				m.Clear();
				m.vertices = list.ToArray();
				m.uv = list2.ToArray();
				m.subMeshCount = array.Length;
				for (int k = 0; k < m.subMeshCount; k++)
				{
					m.SetIndices(array[k].ToArray(), MeshTopology.Triangles, k);
				}
				MeshBakerMaterialTexture.readyToBuildAtlases = false;
			}

			private static void ConfigureMaterial_DefaultPipeline(Material mt, Texture2D t, bool isSavingAsANormalMapAssetThatWillBeImported, MB2_LogLevel LOG_LEVEL)
			{
				Shader shader = Shader.Find("MeshBaker/Unlit/UnlitWithAlpha");
				mt.shader = shader;
				mt.SetTexture("_MainTex", t);
				if (isSavingAsANormalMapAssetThatWillBeImported)
				{
					if (LOG_LEVEL >= MB2_LogLevel.trace)
					{
						Debug.Log("Unswizling normal map channels NM");
					}
					mt.SetFloat("_SwizzleNormalMapChannelsNM", 1f);
					mt.EnableKeyword("_SWIZZLE_NORMAL_CHANNELS_NM");
					return;
				}
				mt.SetFloat("_SwizzleNormalMapChannelsNM", 0f);
				mt.DisableKeyword("_SWIZZLE_NORMAL_CHANNELS_NM");
			}

			public static MB3_AtlasPackerRenderTextureUsingMesh.MeshRectInfo AddQuad(Rect wldRect, Rect uvRect, List<Vector3> verts, List<Vector2> uvs, List<int> tris)
			{
				MB3_AtlasPackerRenderTextureUsingMesh.MeshRectInfo meshRectInfo = new MB3_AtlasPackerRenderTextureUsingMesh.MeshRectInfo();
				int num = meshRectInfo.vertIdx = verts.Count;
				meshRectInfo.triIdx = tris.Count;
				verts.Add(new Vector3(wldRect.x, wldRect.y, 0f));
				verts.Add(new Vector3(wldRect.x + wldRect.width, wldRect.y, 0f));
				verts.Add(new Vector3(wldRect.x, wldRect.y + wldRect.height, 0f));
				verts.Add(new Vector3(wldRect.x + wldRect.width, wldRect.y + wldRect.height, 0f));
				uvs.Add(new Vector2(uvRect.x, uvRect.y));
				uvs.Add(new Vector2(uvRect.x + uvRect.width, uvRect.y));
				uvs.Add(new Vector2(uvRect.x, uvRect.y + uvRect.height));
				uvs.Add(new Vector2(uvRect.x + uvRect.width, uvRect.y + uvRect.height));
				tris.Add(num);
				tris.Add(num + 2);
				tris.Add(num + 1);
				tris.Add(num + 2);
				tris.Add(num + 3);
				tris.Add(num + 1);
				return meshRectInfo;
			}

			public static void AddNineSlicedRect(Rect atlasRectRaw, float paddingX, float paddingY, Rect srcUVRectt, List<Vector3> verts, List<Vector2> uvs, List<int> tris, float srcTexWidth, float srcTexHeight, string texName)
			{
				float num = 0.5f / srcTexWidth;
				float num2 = 0.5f / srcTexHeight;
				float num3 = 0f;
				float num4 = 0f;
				Rect uvRect = srcUVRectt;
				Rect rect = srcUVRectt;
				rect.x += num;
				rect.y += num2;
				rect.width -= num * 2f;
				rect.height -= num2 * 2f;
				Rect rect2 = atlasRectRaw;
				MB3_AtlasPackerRenderTextureUsingMesh.MeshAtlas.AddQuad(atlasRectRaw, uvRect, verts, uvs, tris);
				bool flag = paddingY > 0f;
				bool flag2 = paddingX > 0f;
				if (flag)
				{
					Rect uvRect2 = new Rect(uvRect.x, uvRect.y + uvRect.height - num2 - num4, uvRect.width, num4);
					MB3_AtlasPackerRenderTextureUsingMesh.MeshAtlas.AddQuad(new Rect(rect2.x, rect2.y + rect2.height, rect2.width, paddingY), uvRect2, verts, uvs, tris);
				}
				if (flag)
				{
					Rect uvRect2 = new Rect(uvRect.x, uvRect.y + num2 - num4, uvRect.width, num4);
					MB3_AtlasPackerRenderTextureUsingMesh.MeshAtlas.AddQuad(new Rect(rect2.x, rect2.y - paddingY, rect2.width, paddingY), uvRect2, verts, uvs, tris);
				}
				if (flag2)
				{
					Rect uvRect2 = new Rect(uvRect.x + num, uvRect.y, num3, uvRect.height);
					MB3_AtlasPackerRenderTextureUsingMesh.MeshAtlas.AddQuad(new Rect(rect2.x - paddingX, rect2.y, paddingX, rect2.height), uvRect2, verts, uvs, tris);
				}
				if (flag2)
				{
					Rect uvRect2 = new Rect(uvRect.x + uvRect.width - num - num3, uvRect.y, num3, uvRect.height);
					MB3_AtlasPackerRenderTextureUsingMesh.MeshAtlas.AddQuad(new Rect(rect2.x + rect2.width, rect2.y, paddingX, rect2.height), uvRect2, verts, uvs, tris);
				}
				if (flag && flag2)
				{
					Rect uvRect2 = new Rect(uvRect.x + num, uvRect.y + num2, num3, num4);
					MB3_AtlasPackerRenderTextureUsingMesh.MeshAtlas.AddQuad(new Rect(rect2.x - paddingX, rect2.y - paddingY, paddingX, paddingY), uvRect2, verts, uvs, tris);
				}
				if (flag && flag2)
				{
					Rect uvRect2 = new Rect(uvRect.x + num, uvRect.y + uvRect.height - num2 - num4, num3, num4);
					MB3_AtlasPackerRenderTextureUsingMesh.MeshAtlas.AddQuad(new Rect(rect2.x - paddingX, rect2.y + rect2.height, paddingX, paddingY), uvRect2, verts, uvs, tris);
				}
				if (flag && flag2)
				{
					Rect uvRect2 = new Rect(uvRect.x + uvRect.width - num - num3, uvRect.y + uvRect.height - num2 - num4, num3, num4);
					MB3_AtlasPackerRenderTextureUsingMesh.MeshAtlas.AddQuad(new Rect(rect2.x + rect2.width, rect2.y + rect2.height, paddingX, paddingY), uvRect2, verts, uvs, tris);
				}
				if (flag && flag2)
				{
					Rect uvRect2 = new Rect(uvRect.x + uvRect.width - num - num3, uvRect.y + num2 - num4, num3, num4);
					MB3_AtlasPackerRenderTextureUsingMesh.MeshAtlas.AddQuad(new Rect(rect2.x + rect2.width, rect2.y - paddingY, paddingX, paddingY), uvRect2, verts, uvs, tris);
				}
			}
		}
	}
}
