using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Rendering;

namespace DigitalOpus.MB.Core
{
	public class MBVersionConcrete : MBVersionInterface
	{
		public string version()
		{
			return "3.39.0";
		}

		public bool Is_2017_1_OrNewer()
		{
			return true;
		}

		public bool Is_2018_3_OrNewer()
		{
			return true;
		}

		public bool GetActive(GameObject go)
		{
			return go.activeInHierarchy;
		}

		public void SetActive(GameObject go, bool isActive)
		{
			go.SetActive(isActive);
		}

		public void SetActiveRecursively(GameObject go, bool isActive)
		{
			go.SetActive(isActive);
		}

		public Object[] FindSceneObjectsOfType(Type t)
		{
			return Object.FindObjectsOfType(t);
		}

		public bool IsSwizzledNormalMapPlatform()
		{
			bool result = !GraphicsSettings.HasShaderDefine(BuiltinShaderDefine.UNITY_NO_DXT5nm);
			bool isEditor = Application.isEditor;
			return result;
		}

		public bool IsMaterialKeywordValid(Material mat, string keyword)
		{
			return mat.shader.keywordSpace.FindKeyword(keyword).isValid;
		}

		public void OptimizeMesh(Mesh m)
		{
		}

		public bool IsRunningAndMeshNotReadWriteable(Mesh m)
		{
			return Application.isPlaying && !m.isReadable;
		}

		public Vector2[] GetMeshUV1s(Mesh m, MB2_LogLevel LOG_LEVEL)
		{
			if (LOG_LEVEL >= MB2_LogLevel.warn)
			{
				MB2_Log.LogDebug("UV1 does not exist in Unity 5+", Array.Empty<object>());
			}
			Vector2[] array = m.uv;
			if (array.Length == 0)
			{
				if (LOG_LEVEL >= MB2_LogLevel.debug)
				{
					MB2_Log.LogDebug("Mesh " + ((m != null) ? m.ToString() : null) + " has no uv1s. Generating", Array.Empty<object>());
				}
				if (LOG_LEVEL >= MB2_LogLevel.warn)
				{
					Debug.LogWarning("Mesh " + ((m != null) ? m.ToString() : null) + " didn't have uv1s. Generating uv1s.");
				}
				array = new Vector2[m.vertexCount];
				for (int i = 0; i < array.Length; i++)
				{
					array[i] = this._HALF_UV;
				}
			}
			return array;
		}

		public Vector2[] GetMeshUVChannel(int channel, Mesh m, MB2_LogLevel LOG_LEVEL)
		{
			Vector2[] array = new Vector2[0];
			switch (channel)
			{
			case 0:
				array = m.uv;
				goto IL_91;
			case 2:
				array = m.uv2;
				goto IL_91;
			case 3:
				array = m.uv3;
				goto IL_91;
			case 4:
				array = m.uv4;
				goto IL_91;
			case 5:
				array = m.uv5;
				goto IL_91;
			case 6:
				array = m.uv6;
				goto IL_91;
			case 7:
				array = m.uv7;
				goto IL_91;
			case 8:
				array = m.uv8;
				goto IL_91;
			}
			Debug.LogError("Mesh does not have UV channel " + channel.ToString());
			IL_91:
			if (array.Length == 0)
			{
				if (LOG_LEVEL >= MB2_LogLevel.debug)
				{
					MB2_Log.LogDebug(string.Concat(new string[]
					{
						"Mesh ",
						(m != null) ? m.ToString() : null,
						" has no uv",
						channel.ToString(),
						". Generating"
					}), Array.Empty<object>());
				}
				array = new Vector2[m.vertexCount];
				for (int i = 0; i < array.Length; i++)
				{
					array[i] = this._HALF_UV;
				}
			}
			return array;
		}

		public void MeshClear(Mesh m, bool t)
		{
			m.Clear(t);
		}

		public void MeshAssignUVChannel(int channel, Mesh m, Vector2[] uvs)
		{
			switch (channel)
			{
			case 0:
				m.uv = uvs;
				return;
			case 2:
				m.uv2 = uvs;
				return;
			case 3:
				m.uv3 = uvs;
				return;
			case 4:
				m.uv4 = uvs;
				return;
			case 5:
				m.uv5 = uvs;
				return;
			case 6:
				m.uv6 = uvs;
				return;
			case 7:
				m.uv7 = uvs;
				return;
			case 8:
				m.uv8 = uvs;
				return;
			}
			Debug.LogError("Mesh does not have UV channel " + channel.ToString());
		}

		public Vector4 GetLightmapTilingOffset(Renderer r)
		{
			return r.lightmapScaleOffset;
		}

		public Transform[] GetBones(Renderer r, bool isSkinnedMeshWithBones)
		{
			if (isSkinnedMeshWithBones)
			{
				return ((SkinnedMeshRenderer)r).bones;
			}
			if (r is MeshRenderer || (r is SkinnedMeshRenderer && !isSkinnedMeshWithBones))
			{
				return new Transform[]
				{
					r.transform
				};
			}
			Debug.LogError("Could not getBones. Object does not have a renderer");
			return null;
		}

		public int GetBlendShapeFrameCount(Mesh m, int shapeIndex)
		{
			return m.GetBlendShapeFrameCount(shapeIndex);
		}

		public float GetBlendShapeFrameWeight(Mesh m, int shapeIndex, int frameIndex)
		{
			return m.GetBlendShapeFrameWeight(shapeIndex, frameIndex);
		}

		public void GetBlendShapeFrameVertices(Mesh m, int shapeIndex, int frameIndex, Vector3[] vs, Vector3[] ns, Vector3[] ts)
		{
			m.GetBlendShapeFrameVertices(shapeIndex, frameIndex, vs, ns, ts);
		}

		public void ClearBlendShapes(Mesh m)
		{
			m.ClearBlendShapes();
		}

		public void AddBlendShapeFrame(Mesh m, string nm, float wt, Vector3[] vs, Vector3[] ns, Vector3[] ts)
		{
			m.AddBlendShapeFrame(nm, wt, vs, ns, ts);
		}

		public int MaxMeshVertexCount()
		{
			return 2147483646;
		}

		public void SetMeshIndexFormatAndClearMesh(Mesh m, int numVerts, bool vertices, bool justClearTriangles)
		{
			if (vertices && numVerts > 65534 && m.indexFormat == IndexFormat.UInt16)
			{
				MBVersion.MeshClear(m, false);
				m.indexFormat = IndexFormat.UInt32;
				return;
			}
			if (vertices && numVerts <= 65534 && m.indexFormat == IndexFormat.UInt32)
			{
				MBVersion.MeshClear(m, false);
				m.indexFormat = IndexFormat.UInt16;
				return;
			}
			if (justClearTriangles)
			{
				MBVersion.MeshClear(m, true);
				return;
			}
			MBVersion.MeshClear(m, false);
		}

		public bool GraphicsUVStartsAtTop()
		{
			return false;
		}

		public bool IsTexture_sRGBgammaCorrected(Texture2D tex, bool hint)
		{
			return tex.isDataSRGB;
		}

		public bool IsTextureReadable(Texture2D tex)
		{
			return tex.isReadable;
		}

		public float GetScaleInLightmap(MeshRenderer r)
		{
			return 1f;
		}

		public bool CollectPropertyNames(List<ShaderTextureProperty> texPropertyNames, ShaderTextureProperty[] shaderTexPropertyNames, List<ShaderTextureProperty> _customShaderPropNames, Material resultMaterial, MB2_LogLevel LOG_LEVEL)
		{
			if (resultMaterial != null && resultMaterial.shader != null)
			{
				Shader shader = resultMaterial.shader;
				for (int i = 0; i < shader.GetPropertyCount(); i++)
				{
					if (shader.GetPropertyType(i) == ShaderPropertyType.Texture)
					{
						string propertyName = shader.GetPropertyName(i);
						if (resultMaterial.GetTextureOffset(propertyName) != new Vector2(0f, 0f) && LOG_LEVEL >= MB2_LogLevel.warn)
						{
							Debug.LogWarning("Result material has non-zero offset for property " + propertyName + ". This is probably incorrect.");
						}
						if (resultMaterial.GetTextureScale(propertyName) != new Vector2(1f, 1f) && LOG_LEVEL >= MB2_LogLevel.warn)
						{
							Debug.LogWarning("Result material should probably have tiling of 1,1 for propert " + propertyName);
						}
						ShaderTextureProperty shaderTextureProperty = null;
						for (int j = 0; j < shaderTexPropertyNames.Length; j++)
						{
							if (shaderTexPropertyNames[j].name == propertyName)
							{
								shaderTextureProperty = new ShaderTextureProperty(propertyName, shaderTexPropertyNames[j].isNormalMap, shaderTexPropertyNames[j].isGammaCorrected, shaderTexPropertyNames[j].isNormalDontKnow);
							}
						}
						for (int k = 0; k < _customShaderPropNames.Count; k++)
						{
							if (_customShaderPropNames[k].name == propertyName)
							{
								shaderTextureProperty = new ShaderTextureProperty(propertyName, _customShaderPropNames[k].isNormalMap, _customShaderPropNames[k].isGammaCorrected, _customShaderPropNames[k].isNormalDontKnow);
							}
						}
						if (shaderTextureProperty == null)
						{
							shaderTextureProperty = new ShaderTextureProperty(propertyName, false, true, true);
						}
						texPropertyNames.Add(shaderTextureProperty);
					}
				}
				MBVersion.PipelineType pipelineType = this.DetectPipeline();
				int num = texPropertyNames.FindIndex((ShaderTextureProperty pn) => pn.name.Equals("_MainTex"));
				if (pipelineType == MBVersion.PipelineType.URP && shader.name.StartsWith("Universal Render Pipeline/") && num != -1)
				{
					if (texPropertyNames.FindIndex((ShaderTextureProperty pn) => pn.name.Equals("_BaseMap")) != -1)
					{
						texPropertyNames.RemoveAt(num);
						return true;
					}
				}
				if (pipelineType == MBVersion.PipelineType.HDRP && shader.name.StartsWith("HDRP/") && num != -1)
				{
					if (texPropertyNames.FindIndex((ShaderTextureProperty pn) => pn.name.Equals("_BaseColorMap")) != -1)
					{
						texPropertyNames.RemoveAt(num);
					}
				}
			}
			return true;
		}

		public void DoSpecialRenderPipeline_TexturePackerFastSetup(GameObject cameraGameObject)
		{
		}

		public ColorSpace GetProjectColorSpace()
		{
			if (QualitySettings.desiredColorSpace != QualitySettings.activeColorSpace)
			{
				Debug.LogError(string.Concat(new string[]
				{
					"The active color space (",
					QualitySettings.activeColorSpace.ToString(),
					") is not the desired color space (",
					QualitySettings.desiredColorSpace.ToString(),
					"). Baked atlases may be off."
				}));
			}
			return QualitySettings.activeColorSpace;
		}

		public MBVersion.PipelineType DetectPipeline()
		{
			RenderPipelineAsset renderPipelineAsset = GraphicsSettings.defaultRenderPipeline;
			renderPipelineAsset = ((QualitySettings.renderPipeline != null) ? QualitySettings.renderPipeline : renderPipelineAsset);
			if (!(renderPipelineAsset != null))
			{
				return MBVersion.PipelineType.Default;
			}
			string text = renderPipelineAsset.GetType().ToString();
			if (text.Contains("HDRenderPipelineAsset"))
			{
				return MBVersion.PipelineType.HDRP;
			}
			if (text.Contains("UniversalRenderPipelineAsset") || text.Contains("LightweightRenderPipelineAsset"))
			{
				return MBVersion.PipelineType.URP;
			}
			return MBVersion.PipelineType.Unsupported;
		}

		public string UnescapeURL(string url)
		{
			return UnityWebRequest.UnEscapeURL(url);
		}

		public IEnumerator FindRuntimeMaterialsFromAddresses(MB2_TextureBakeResults texBakeResult, MB2_TextureBakeResults.CoroutineResult isComplete)
		{
			if (!Application.isPlaying)
			{
				Debug.LogError("FindRuntimeMaterialsFromAddresses is a coroutine. It is only necessary to use this at runtime.");
			}
			Debug.LogError("The MB_USING_ADDRESSABLES define was not set in PlayerSettings -> Script Define Symbols. If you are using addressables and you want to use this method, that must be set.");
			isComplete.isComplete = true;
			yield break;
		}

		public bool IsAssetInProject(Object target)
		{
			return false;
		}

		private Vector2 _HALF_UV = new Vector2(0.5f, 0.5f);
	}
}
