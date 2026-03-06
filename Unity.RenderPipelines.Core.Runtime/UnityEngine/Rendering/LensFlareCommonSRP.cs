using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine.Experimental.Rendering;

namespace UnityEngine.Rendering
{
	public sealed class LensFlareCommonSRP
	{
		private LensFlareCommonSRP()
		{
		}

		private static bool requireOcclusionRTRandomWrite
		{
			get
			{
				return LensFlareCommonSRP.mergeNeeded > 0;
			}
		}

		private static bool CheckOcclusionBasedOnDeviceType()
		{
			return SystemInfo.graphicsDeviceType != GraphicsDeviceType.Null && SystemInfo.graphicsDeviceType != GraphicsDeviceType.OpenGLES3 && SystemInfo.graphicsDeviceType != GraphicsDeviceType.OpenGLCore && SystemInfo.graphicsDeviceType != GraphicsDeviceType.WebGPU;
		}

		public static bool IsOcclusionRTCompatible()
		{
			if (LensFlareCommonSRP.requireOcclusionRTRandomWrite)
			{
				return LensFlareCommonSRP.CheckOcclusionBasedOnDeviceType() && (LensFlareCommonSRP.s_SupportsLensFlare16bitsFormatWithLoadStore || LensFlareCommonSRP.s_SupportsLensFlare32bitsFormatWithLoadStore);
			}
			return LensFlareCommonSRP.CheckOcclusionBasedOnDeviceType() && (LensFlareCommonSRP.s_SupportsLensFlare16bitsFormat || LensFlareCommonSRP.s_SupportsLensFlare32bitsFormat);
		}

		private static GraphicsFormat GetOcclusionRTFormat()
		{
			if (LensFlareCommonSRP.requireOcclusionRTRandomWrite ? LensFlareCommonSRP.s_SupportsLensFlare16bitsFormatWithLoadStore : LensFlareCommonSRP.s_SupportsLensFlare16bitsFormat)
			{
				return GraphicsFormat.R16_SFloat;
			}
			return GraphicsFormat.R32_SFloat;
		}

		public static void Initialize()
		{
			LensFlareCommonSRP.frameIdx = 0;
			if (LensFlareCommonSRP.IsOcclusionRTCompatible() && LensFlareCommonSRP.occlusionRT == null)
			{
				LensFlareCommonSRP.occlusionRT = RTHandles.Alloc(LensFlareCommonSRP.maxLensFlareWithOcclusion, Mathf.Max(LensFlareCommonSRP.mergeNeeded * (LensFlareCommonSRP.maxLensFlareWithOcclusionTemporalSample + 1), 1), LensFlareCommonSRP.GetOcclusionRTFormat(), TextureXR.slices, FilterMode.Point, TextureWrapMode.Repeat, TextureDimension.Tex2DArray, LensFlareCommonSRP.requireOcclusionRTRandomWrite, false, true, false, 1, 0f, MSAASamples.None, false, false, false, RenderTextureMemoryless.None, VRTextureUsage.None, "");
			}
		}

		public static void Dispose()
		{
			if (LensFlareCommonSRP.IsOcclusionRTCompatible() && LensFlareCommonSRP.occlusionRT != null)
			{
				RTHandles.Release(LensFlareCommonSRP.occlusionRT);
				LensFlareCommonSRP.occlusionRT = null;
			}
		}

		public static LensFlareCommonSRP Instance
		{
			get
			{
				if (LensFlareCommonSRP.m_Instance == null)
				{
					object padlock = LensFlareCommonSRP.m_Padlock;
					lock (padlock)
					{
						if (LensFlareCommonSRP.m_Instance == null)
						{
							LensFlareCommonSRP.m_Instance = new LensFlareCommonSRP();
						}
					}
				}
				return LensFlareCommonSRP.m_Instance;
			}
		}

		private List<LensFlareCommonSRP.LensFlareCompInfo> Data
		{
			get
			{
				return LensFlareCommonSRP.m_Data;
			}
		}

		public bool IsEmpty()
		{
			return this.Data.Count == 0;
		}

		private int GetNextAvailableIndex()
		{
			if (LensFlareCommonSRP.m_AvailableIndicies.Count == 0)
			{
				return LensFlareCommonSRP.m_Data.Count;
			}
			int result = LensFlareCommonSRP.m_AvailableIndicies[LensFlareCommonSRP.m_AvailableIndicies.Count - 1];
			LensFlareCommonSRP.m_AvailableIndicies.RemoveAt(LensFlareCommonSRP.m_AvailableIndicies.Count - 1);
			return result;
		}

		public void AddData(LensFlareComponentSRP newData)
		{
			if (!LensFlareCommonSRP.m_Data.Exists((LensFlareCommonSRP.LensFlareCompInfo x) => x.comp == newData))
			{
				LensFlareCommonSRP.m_Data.Add(new LensFlareCommonSRP.LensFlareCompInfo(this.GetNextAvailableIndex(), newData));
			}
		}

		public void RemoveData(LensFlareComponentSRP data)
		{
			LensFlareCommonSRP.LensFlareCompInfo lensFlareCompInfo = LensFlareCommonSRP.m_Data.Find((LensFlareCommonSRP.LensFlareCompInfo x) => x.comp == data);
			if (lensFlareCompInfo != null)
			{
				int index = lensFlareCompInfo.index;
				LensFlareCommonSRP.m_Data.Remove(lensFlareCompInfo);
				LensFlareCommonSRP.m_AvailableIndicies.Add(index);
				if (LensFlareCommonSRP.m_Data.Count == 0)
				{
					LensFlareCommonSRP.m_AvailableIndicies.Clear();
				}
			}
		}

		public static float ShapeAttenuationPointLight()
		{
			return 1f;
		}

		public static float ShapeAttenuationDirLight(Vector3 forward, Vector3 wo)
		{
			return Mathf.Max(Vector3.Dot(-forward, wo), 0f);
		}

		public static float ShapeAttenuationSpotConeLight(Vector3 forward, Vector3 wo, float spotAngle, float innerSpotPercent01)
		{
			float num = Mathf.Max(Mathf.Cos(0.5f * spotAngle * 0.017453292f), 0f);
			float num2 = Mathf.Max(Mathf.Cos(0.5f * spotAngle * 0.017453292f * innerSpotPercent01), 0f);
			return Mathf.Clamp01((Mathf.Max(Vector3.Dot(forward, wo), 0f) - num) / (num2 - num));
		}

		public static float ShapeAttenuationSpotBoxLight(Vector3 forward, Vector3 wo)
		{
			return Mathf.Max(Mathf.Sign(Vector3.Dot(forward, wo)), 0f);
		}

		public static float ShapeAttenuationSpotPyramidLight(Vector3 forward, Vector3 wo)
		{
			return LensFlareCommonSRP.ShapeAttenuationSpotBoxLight(forward, wo);
		}

		public static float ShapeAttenuationAreaTubeLight(Vector3 lightPositionWS, Vector3 lightSide, float lightWidth, Camera cam)
		{
			Transform transform = cam.transform;
			Vector3 position = lightPositionWS + 0.5f * lightWidth * lightSide;
			Vector3 position2 = lightPositionWS - 0.5f * lightWidth * lightSide;
			Vector3 position3 = lightPositionWS + 0.5f * lightWidth * transform.right;
			Vector3 position4 = lightPositionWS - 0.5f * lightWidth * transform.right;
			Vector3 p = transform.InverseTransformPoint(position);
			Vector3 p2 = transform.InverseTransformPoint(position2);
			Vector3 p3 = transform.InverseTransformPoint(position3);
			Vector3 p4 = transform.InverseTransformPoint(position4);
			float num = LensFlareCommonSRP.<ShapeAttenuationAreaTubeLight>g__DiffLineIntegral|62_2(p3, p4);
			float num2 = LensFlareCommonSRP.<ShapeAttenuationAreaTubeLight>g__DiffLineIntegral|62_2(p, p2);
			if (num <= 0f)
			{
				return 1f;
			}
			return num2 / num;
		}

		private static float ShapeAttenuateForwardLight(Vector3 forward, Vector3 wo)
		{
			return Mathf.Max(Vector3.Dot(forward, wo), 0f);
		}

		public static float ShapeAttenuationAreaRectangleLight(Vector3 forward, Vector3 wo)
		{
			return LensFlareCommonSRP.ShapeAttenuateForwardLight(forward, wo);
		}

		public static float ShapeAttenuationAreaDiscLight(Vector3 forward, Vector3 wo)
		{
			return LensFlareCommonSRP.ShapeAttenuateForwardLight(forward, wo);
		}

		private static bool IsLensFlareSRPHidden(Camera cam, LensFlareComponentSRP comp, LensFlareDataSRP data)
		{
			return !comp.enabled || !comp.gameObject.activeSelf || !comp.gameObject.activeInHierarchy || data == null || data.elements == null || data.elements.Length == 0 || comp.intensity <= 0f || (cam.cullingMask & 1 << comp.gameObject.layer) == 0;
		}

		public static Vector4 GetFlareData0(Vector2 screenPos, Vector2 translationScale, Vector2 rayOff0, Vector2 vLocalScreenRatio, float angleDeg, float position, float angularOffset, Vector2 positionOffset, bool autoRotate)
		{
			if (!SystemInfo.graphicsUVStartsAtTop)
			{
				angleDeg *= -1f;
				positionOffset.y *= -1f;
			}
			float num = Mathf.Cos(-angularOffset * 0.017453292f);
			float num2 = Mathf.Sin(-angularOffset * 0.017453292f);
			Vector2 vector = -translationScale * (screenPos + screenPos * (position - 1f));
			vector = new Vector2(num * vector.x - num2 * vector.y, num2 * vector.x + num * vector.y);
			float num3 = angleDeg;
			num3 += 180f;
			if (autoRotate)
			{
				Vector2 vector2 = vector.normalized * vLocalScreenRatio * translationScale;
				num3 += -57.29578f * Mathf.Atan2(vector2.y, vector2.x);
			}
			num3 *= 0.017453292f;
			float x = Mathf.Cos(-num3);
			float y = Mathf.Sin(-num3);
			return new Vector4(x, y, positionOffset.x + rayOff0.x * translationScale.x, -positionOffset.y + rayOff0.y * translationScale.y);
		}

		private static Vector2 GetLensFlareRayOffset(Vector2 screenPos, float position, float globalCos0, float globalSin0, Vector2 vAspectRatio)
		{
			Vector2 vector = -(screenPos + screenPos * (position - 1f));
			return new Vector2(globalCos0 * vector.x - globalSin0 * vector.y, globalSin0 * vector.x + globalCos0 * vector.y);
		}

		private static Vector3 WorldToViewport(Camera camera, bool isLocalLight, bool isCameraRelative, Matrix4x4 viewProjMatrix, Vector3 positionWS)
		{
			if (isLocalLight)
			{
				return LensFlareCommonSRP.WorldToViewportLocal(isCameraRelative, viewProjMatrix, camera.transform.position, positionWS);
			}
			return LensFlareCommonSRP.WorldToViewportDistance(camera, positionWS);
		}

		private static Vector3 WorldToViewportLocal(bool isCameraRelative, Matrix4x4 viewProjMatrix, Vector3 cameraPosWS, Vector3 positionWS)
		{
			Vector3 vector = positionWS;
			if (isCameraRelative)
			{
				vector -= cameraPosWS;
			}
			Vector4 vector2 = viewProjMatrix * vector;
			Vector3 vector3 = new Vector3(vector2.x, vector2.y, 0f);
			vector3 /= vector2.w;
			vector3.x = vector3.x * 0.5f + 0.5f;
			vector3.y = vector3.y * 0.5f + 0.5f;
			vector3.y = 1f - vector3.y;
			vector3.z = vector2.w;
			return vector3;
		}

		private static Vector3 WorldToViewportDistance(Camera cam, Vector3 positionWS)
		{
			Vector4 vector = cam.worldToCameraMatrix * positionWS;
			Vector4 vector2 = cam.projectionMatrix * vector;
			Vector3 vector3 = new Vector3(vector2.x, vector2.y, 0f);
			vector3 /= vector2.w;
			vector3.x = vector3.x * 0.5f + 0.5f;
			vector3.y = vector3.y * 0.5f + 0.5f;
			vector3.z = vector2.w;
			return vector3;
		}

		public static bool IsCloudLayerOpacityNeeded(Camera cam)
		{
			if (LensFlareCommonSRP.Instance.IsEmpty())
			{
				return false;
			}
			foreach (LensFlareCommonSRP.LensFlareCompInfo lensFlareCompInfo in LensFlareCommonSRP.Instance.Data)
			{
				if (lensFlareCompInfo != null && !(lensFlareCompInfo.comp == null))
				{
					LensFlareComponentSRP comp = lensFlareCompInfo.comp;
					LensFlareDataSRP lensFlareData = comp.lensFlareData;
					if (!LensFlareCommonSRP.IsLensFlareSRPHidden(cam, comp, lensFlareData) && comp.useOcclusion && (!comp.useOcclusion || comp.sampleCount != 0U) && comp.environmentOcclusion)
					{
						return true;
					}
				}
			}
			return false;
		}

		private static void SetOcclusionPermutation(CommandBuffer cmd, bool useFogOpacityOcclusion, int _FlareSunOcclusionTex, Texture sunOcclusionTexture)
		{
			uint num = 1U;
			if (useFogOpacityOcclusion && sunOcclusionTexture != null)
			{
				num |= 4U;
				cmd.SetGlobalTexture(_FlareSunOcclusionTex, sunOcclusionTexture);
			}
			int value = (int)num;
			cmd.SetGlobalInt(LensFlareCommonSRP._FlareOcclusionPermutation, value);
		}

		[Obsolete("Use ComputeOcclusion without _FlareOcclusionTex.._FlareData4 parameters.")]
		public static void ComputeOcclusion(Material lensFlareShader, Camera cam, XRPass xr, int xrIndex, float actualWidth, float actualHeight, bool usePanini, float paniniDistance, float paniniCropToFit, bool isCameraRelative, Vector3 cameraPositionWS, Matrix4x4 viewProjMatrix, UnsafeCommandBuffer cmd, bool taaEnabled, bool hasCloudLayer, Texture cloudOpacityTexture, Texture sunOcclusionTexture, int _FlareOcclusionTex, int _FlareCloudOpacity, int _FlareOcclusionIndex, int _FlareTex, int _FlareColorValue, int _FlareSunOcclusionTex, int _FlareData0, int _FlareData1, int _FlareData2, int _FlareData3, int _FlareData4)
		{
			LensFlareCommonSRP.ComputeOcclusion(lensFlareShader, cam, xr, xrIndex, actualWidth, actualHeight, usePanini, paniniDistance, paniniCropToFit, isCameraRelative, cameraPositionWS, viewProjMatrix, cmd.m_WrappedCommandBuffer, taaEnabled, hasCloudLayer, cloudOpacityTexture, sunOcclusionTexture, _FlareOcclusionTex, _FlareCloudOpacity, _FlareOcclusionIndex, _FlareTex, _FlareColorValue, _FlareSunOcclusionTex, _FlareData0, _FlareData1, _FlareData2, _FlareData3, _FlareData4);
		}

		public static void ComputeOcclusion(Material lensFlareShader, Camera cam, XRPass xr, int xrIndex, float actualWidth, float actualHeight, bool usePanini, float paniniDistance, float paniniCropToFit, bool isCameraRelative, Vector3 cameraPositionWS, Matrix4x4 viewProjMatrix, UnsafeCommandBuffer cmd, bool taaEnabled, bool hasCloudLayer, Texture cloudOpacityTexture, Texture sunOcclusionTexture)
		{
			LensFlareCommonSRP.ComputeOcclusion(lensFlareShader, cam, xr, xrIndex, actualWidth, actualHeight, usePanini, paniniDistance, paniniCropToFit, isCameraRelative, cameraPositionWS, viewProjMatrix, cmd.m_WrappedCommandBuffer, taaEnabled, hasCloudLayer, cloudOpacityTexture, sunOcclusionTexture);
		}

		[Obsolete("Use ComputeOcclusion without _FlareOcclusionTex.._FlareData4 parameters.")]
		public static void ComputeOcclusion(Material lensFlareShader, Camera cam, XRPass xr, int xrIndex, float actualWidth, float actualHeight, bool usePanini, float paniniDistance, float paniniCropToFit, bool isCameraRelative, Vector3 cameraPositionWS, Matrix4x4 viewProjMatrix, CommandBuffer cmd, bool taaEnabled, bool hasCloudLayer, Texture cloudOpacityTexture, Texture sunOcclusionTexture, int _FlareOcclusionTex, int _FlareCloudOpacity, int _FlareOcclusionIndex, int _FlareTex, int _FlareColorValue, int _FlareSunOcclusionTex, int _FlareData0, int _FlareData1, int _FlareData2, int _FlareData3, int _FlareData4)
		{
			LensFlareCommonSRP.ComputeOcclusion(lensFlareShader, cam, xr, xrIndex, actualWidth, actualHeight, usePanini, paniniDistance, paniniCropToFit, isCameraRelative, cameraPositionWS, viewProjMatrix, cmd, taaEnabled, hasCloudLayer, cloudOpacityTexture, sunOcclusionTexture);
		}

		private static bool ForceSingleElement(LensFlareDataElementSRP element)
		{
			return !element.allowMultipleElement || element.count == 1 || element.flareType == SRPLensFlareType.Ring;
		}

		public static void ComputeOcclusion(Material lensFlareShader, Camera cam, XRPass xr, int xrIndex, float actualWidth, float actualHeight, bool usePanini, float paniniDistance, float paniniCropToFit, bool isCameraRelative, Vector3 cameraPositionWS, Matrix4x4 viewProjMatrix, CommandBuffer cmd, bool taaEnabled, bool hasCloudLayer, Texture cloudOpacityTexture, Texture sunOcclusionTexture)
		{
			if (!LensFlareCommonSRP.IsOcclusionRTCompatible())
			{
				return;
			}
			xr.StopSinglePass(cmd);
			if (LensFlareCommonSRP.Instance.IsEmpty())
			{
				return;
			}
			Vector2 vector = new Vector2(actualWidth, actualHeight);
			float x = vector.x / vector.y;
			Vector2 vector2 = new Vector2(x, 1f);
			if (xr.enabled && xr.singlePassEnabled)
			{
				CoreUtils.SetRenderTarget(cmd, LensFlareCommonSRP.occlusionRT, ClearFlag.None, 0, CubemapFace.Unknown, xrIndex);
				cmd.SetGlobalInt(LensFlareCommonSRP._ViewId, xrIndex);
			}
			else
			{
				CoreUtils.SetRenderTarget(cmd, LensFlareCommonSRP.occlusionRT, ClearFlag.None, 0, CubemapFace.Unknown, -1);
				if (xr.enabled)
				{
					cmd.SetGlobalInt(LensFlareCommonSRP._ViewId, xr.multipassId);
				}
				else
				{
					cmd.SetGlobalInt(LensFlareCommonSRP._ViewId, -1);
				}
			}
			if (!taaEnabled)
			{
				cmd.ClearRenderTarget(false, true, Color.black);
			}
			float num = 1f / (float)LensFlareCommonSRP.maxLensFlareWithOcclusion;
			float num2 = 1f / (float)(LensFlareCommonSRP.maxLensFlareWithOcclusionTemporalSample + LensFlareCommonSRP.mergeNeeded);
			float num3 = 0.5f / (float)LensFlareCommonSRP.maxLensFlareWithOcclusion;
			float num4 = 0.5f / (float)(LensFlareCommonSRP.maxLensFlareWithOcclusionTemporalSample + LensFlareCommonSRP.mergeNeeded);
			foreach (LensFlareCommonSRP.LensFlareCompInfo lensFlareCompInfo in LensFlareCommonSRP.m_Data)
			{
				if (lensFlareCompInfo != null && !(lensFlareCompInfo.comp == null))
				{
					LensFlareComponentSRP comp = lensFlareCompInfo.comp;
					LensFlareDataSRP lensFlareData = comp.lensFlareData;
					if (!LensFlareCommonSRP.IsLensFlareSRPHidden(cam, comp, lensFlareData) && comp.useOcclusion && (!comp.useOcclusion || comp.sampleCount != 0U))
					{
						Light light = null;
						if (!comp.TryGetComponent<Light>(out light))
						{
							light = null;
						}
						bool flag = false;
						Vector3 vector3;
						if (light != null && light.type == LightType.Directional)
						{
							vector3 = -light.transform.forward * cam.farClipPlane;
							flag = true;
						}
						else
						{
							vector3 = comp.transform.position;
						}
						Vector3 vector4 = LensFlareCommonSRP.WorldToViewport(cam, !flag, isCameraRelative, viewProjMatrix, vector3);
						if (usePanini && cam == Camera.main)
						{
							vector4 = LensFlareCommonSRP.DoPaniniProjection(vector4, actualWidth, actualHeight, cam.fieldOfView, paniniCropToFit, paniniDistance);
						}
						if (vector4.z >= 0f && (comp.allowOffScreen || (vector4.x >= 0f && vector4.x <= 1f && vector4.y >= 0f && vector4.y <= 1f)))
						{
							Vector3 rhs = vector3 - cameraPositionWS;
							if (Vector3.Dot(cam.transform.forward, rhs) >= 0f)
							{
								float magnitude = rhs.magnitude;
								float time = magnitude / comp.maxAttenuationDistance;
								float time2 = magnitude / comp.maxAttenuationScale;
								float num5 = (!flag && comp.distanceAttenuationCurve.length > 0) ? comp.distanceAttenuationCurve.Evaluate(time) : 1f;
								if (!flag && comp.scaleByDistanceCurve.length >= 1)
								{
									comp.scaleByDistanceCurve.Evaluate(time2);
								}
								Vector3 a;
								if (flag)
								{
									a = comp.transform.forward;
								}
								else
								{
									a = (cam.transform.position - comp.transform.position).normalized;
								}
								Vector3 vector5 = LensFlareCommonSRP.WorldToViewport(cam, !flag, isCameraRelative, viewProjMatrix, vector3 + a * comp.occlusionOffset);
								float d = flag ? comp.celestialProjectedOcclusionRadius(cam) : comp.occlusionRadius;
								Vector2 b = vector4;
								float magnitude2 = (LensFlareCommonSRP.WorldToViewport(cam, !flag, isCameraRelative, viewProjMatrix, vector3 + cam.transform.up * d) - b).magnitude;
								cmd.SetGlobalVector(LensFlareCommonSRP._FlareData1, new Vector4(magnitude2, comp.sampleCount, vector5.z, actualHeight / actualWidth));
								LensFlareCommonSRP.SetOcclusionPermutation(cmd, comp.environmentOcclusion, LensFlareCommonSRP._FlareSunOcclusionTex, sunOcclusionTexture);
								cmd.EnableShaderKeyword("FLARE_COMPUTE_OCCLUSION");
								Vector2 vector6 = new Vector2(2f * vector4.x - 1f, -(2f * vector4.y - 1f));
								if (!SystemInfo.graphicsUVStartsAtTop && flag)
								{
									vector6.y = -vector6.y;
								}
								Vector2 vector7 = new Vector2(Mathf.Abs(vector6.x), Mathf.Abs(vector6.y));
								float time3 = Mathf.Max(vector7.x, vector7.y);
								float num6 = (comp.radialScreenAttenuationCurve.length > 0) ? comp.radialScreenAttenuationCurve.Evaluate(time3) : 1f;
								if (comp.intensity * num6 * num5 > 0f)
								{
									float globalCos = Mathf.Cos(0f);
									float globalSin = Mathf.Sin(0f);
									float position = 0f;
									float y = Mathf.Clamp01(0.999999f);
									cmd.SetGlobalVector(LensFlareCommonSRP._FlareData3, new Vector4(comp.allowOffScreen ? 1f : -1f, y, Mathf.Exp(Mathf.Lerp(0f, 4f, 1f)), 0.33333334f));
									Vector2 lensFlareRayOffset = LensFlareCommonSRP.GetLensFlareRayOffset(vector6, position, globalCos, globalSin, vector2);
									Vector4 flareData = LensFlareCommonSRP.GetFlareData0(vector6, Vector2.one, lensFlareRayOffset, vector2, 0f, position, 0f, Vector2.zero, false);
									cmd.SetGlobalVector(LensFlareCommonSRP._FlareData0, flareData);
									cmd.SetGlobalVector(LensFlareCommonSRP._FlareData2, new Vector4(vector6.x, vector6.y, 0f, 0f));
									Rect viewport;
									if (taaEnabled)
									{
										viewport = new Rect
										{
											x = (float)lensFlareCompInfo.index,
											y = (float)(LensFlareCommonSRP.frameIdx + LensFlareCommonSRP.mergeNeeded),
											width = 1f,
											height = 1f
										};
									}
									else
									{
										viewport = new Rect
										{
											x = (float)lensFlareCompInfo.index,
											y = 0f,
											width = 1f,
											height = 1f
										};
									}
									cmd.SetViewport(viewport);
									Blitter.DrawQuad(cmd, lensFlareShader, lensFlareShader.FindPass("LensFlareOcclusion"));
								}
							}
						}
					}
				}
			}
			if (taaEnabled)
			{
				CoreUtils.SetRenderTarget(cmd, LensFlareCommonSRP.occlusionRT, ClearFlag.None, 0, CubemapFace.Unknown, xrIndex);
				cmd.SetViewport(new Rect
				{
					x = (float)LensFlareCommonSRP.m_Data.Count,
					y = 0f,
					width = (float)(LensFlareCommonSRP.maxLensFlareWithOcclusion - LensFlareCommonSRP.m_Data.Count),
					height = (float)(LensFlareCommonSRP.maxLensFlareWithOcclusionTemporalSample + LensFlareCommonSRP.mergeNeeded)
				});
				cmd.ClearRenderTarget(false, true, Color.black);
			}
			LensFlareCommonSRP.frameIdx++;
			LensFlareCommonSRP.frameIdx %= LensFlareCommonSRP.maxLensFlareWithOcclusionTemporalSample;
			xr.StartSinglePass(cmd);
		}

		public static void ProcessLensFlareSRPElementsSingle(LensFlareDataElementSRP element, CommandBuffer cmd, Color globalColorModulation, Light light, float compIntensity, float scale, Material lensFlareShader, Vector2 screenPos, bool compAllowOffScreen, Vector2 vScreenRatio, Vector4 flareData1, bool preview, int depth)
		{
			LensFlareCommonSRP.<>c__DisplayClass79_0 CS$<>8__locals1;
			CS$<>8__locals1.screenPos = screenPos;
			CS$<>8__locals1.vScreenRatio = vScreenRatio;
			CS$<>8__locals1.element = element;
			if (CS$<>8__locals1.element == null || !CS$<>8__locals1.element.visible || (CS$<>8__locals1.element.lensFlareTexture == null && CS$<>8__locals1.element.flareType == SRPLensFlareType.Image) || CS$<>8__locals1.element.localIntensity <= 0f || CS$<>8__locals1.element.count <= 0 || (CS$<>8__locals1.element.flareType == SRPLensFlareType.LensFlareDataSRP && CS$<>8__locals1.element.lensFlareDataSRP == null))
			{
				return;
			}
			if (CS$<>8__locals1.element.flareType == SRPLensFlareType.LensFlareDataSRP && CS$<>8__locals1.element.lensFlareDataSRP != null)
			{
				LensFlareCommonSRP.ProcessLensFlareSRPElements(ref CS$<>8__locals1.element.lensFlareDataSRP.elements, cmd, globalColorModulation, light, compIntensity, scale, lensFlareShader, CS$<>8__locals1.screenPos, compAllowOffScreen, CS$<>8__locals1.vScreenRatio, flareData1, preview, depth + 1);
				return;
			}
			Color color = globalColorModulation;
			if (light != null && CS$<>8__locals1.element.modulateByLightColor)
			{
				if (light.useColorTemperature)
				{
					color *= light.color * Mathf.CorrelatedColorTemperatureToRGB(light.colorTemperature);
				}
				else
				{
					color *= light.color;
				}
			}
			Color color2 = color;
			float num = CS$<>8__locals1.element.localIntensity * compIntensity;
			if (num <= 0f)
			{
				return;
			}
			Texture lensFlareTexture = CS$<>8__locals1.element.lensFlareTexture;
			if (CS$<>8__locals1.element.flareType == SRPLensFlareType.Image)
			{
				CS$<>8__locals1.usedAspectRatio = (CS$<>8__locals1.element.preserveAspectRatio ? ((float)lensFlareTexture.height / (float)lensFlareTexture.width) : 1f);
			}
			else
			{
				CS$<>8__locals1.usedAspectRatio = 1f;
			}
			float rotation = CS$<>8__locals1.element.rotation;
			Vector2 vector;
			if (CS$<>8__locals1.element.preserveAspectRatio)
			{
				if (CS$<>8__locals1.usedAspectRatio >= 1f)
				{
					vector = new Vector2(CS$<>8__locals1.element.sizeXY.x / CS$<>8__locals1.usedAspectRatio, CS$<>8__locals1.element.sizeXY.y);
				}
				else
				{
					vector = new Vector2(CS$<>8__locals1.element.sizeXY.x, CS$<>8__locals1.element.sizeXY.y * CS$<>8__locals1.usedAspectRatio);
				}
			}
			else
			{
				vector = new Vector2(CS$<>8__locals1.element.sizeXY.x, CS$<>8__locals1.element.sizeXY.y);
			}
			float num2 = 0.1f;
			Vector2 vector2 = new Vector2(vector.x, vector.y);
			CS$<>8__locals1.combinedScale = num2 * CS$<>8__locals1.element.uniformScale * scale;
			vector2 *= CS$<>8__locals1.combinedScale;
			color2 *= CS$<>8__locals1.element.tint;
			float num3 = SystemInfo.graphicsUVStartsAtTop ? CS$<>8__locals1.element.angularOffset : (-CS$<>8__locals1.element.angularOffset);
			CS$<>8__locals1.globalCos0 = Mathf.Cos(-num3 * 0.017453292f);
			CS$<>8__locals1.globalSin0 = Mathf.Sin(-num3 * 0.017453292f);
			CS$<>8__locals1.position = 2f * CS$<>8__locals1.element.position;
			SRPLensFlareBlendMode blendMode = CS$<>8__locals1.element.blendMode;
			int shaderPass;
			if (blendMode == SRPLensFlareBlendMode.Additive)
			{
				shaderPass = lensFlareShader.FindPass("LensFlareAdditive");
			}
			else if (blendMode == SRPLensFlareBlendMode.Screen)
			{
				shaderPass = lensFlareShader.FindPass("LensFlareScreen");
			}
			else if (blendMode == SRPLensFlareBlendMode.Premultiply)
			{
				shaderPass = lensFlareShader.FindPass("LensFlarePremultiply");
			}
			else if (blendMode == SRPLensFlareBlendMode.Lerp)
			{
				shaderPass = lensFlareShader.FindPass("LensFlareLerp");
			}
			else
			{
				shaderPass = lensFlareShader.FindPass("LensFlareOcclusion");
			}
			flareData1.x = (float)CS$<>8__locals1.element.flareType;
			if (LensFlareCommonSRP.ForceSingleElement(CS$<>8__locals1.element))
			{
				cmd.SetGlobalVector(LensFlareCommonSRP._FlareData1, flareData1);
			}
			if (CS$<>8__locals1.element.flareType == SRPLensFlareType.Circle || CS$<>8__locals1.element.flareType == SRPLensFlareType.Polygon || CS$<>8__locals1.element.flareType == SRPLensFlareType.Ring)
			{
				if (CS$<>8__locals1.element.inverseSDF)
				{
					cmd.EnableShaderKeyword("FLARE_INVERSE_SDF");
				}
				else
				{
					cmd.DisableShaderKeyword("FLARE_INVERSE_SDF");
				}
			}
			else
			{
				cmd.DisableShaderKeyword("FLARE_INVERSE_SDF");
			}
			if (CS$<>8__locals1.element.lensFlareTexture != null)
			{
				cmd.SetGlobalTexture(LensFlareCommonSRP._FlareTex, CS$<>8__locals1.element.lensFlareTexture);
			}
			if (CS$<>8__locals1.element.tintColorType != SRPLensFlareColorType.Constant)
			{
				cmd.SetGlobalTexture(LensFlareCommonSRP._FlareRadialTint, CS$<>8__locals1.element.tintGradient.GetTexture());
			}
			float num4 = Mathf.Clamp01(1f - CS$<>8__locals1.element.edgeOffset - 1E-06f);
			if (CS$<>8__locals1.element.flareType == SRPLensFlareType.Polygon)
			{
				num4 = Mathf.Pow(num4 + 1f, 5f);
			}
			float sdfRoundness = CS$<>8__locals1.element.sdfRoundness;
			Vector4 value = new Vector4(compAllowOffScreen ? 1f : -1f, num4, Mathf.Exp(Mathf.Lerp(0f, 4f, Mathf.Clamp01(1f - CS$<>8__locals1.element.fallOff))), (CS$<>8__locals1.element.flareType == SRPLensFlareType.Ring) ? CS$<>8__locals1.element.ringThickness : (1f / (float)CS$<>8__locals1.element.sideCount));
			cmd.SetGlobalVector(LensFlareCommonSRP._FlareData3, value);
			if (CS$<>8__locals1.element.flareType == SRPLensFlareType.Polygon)
			{
				float num5 = 1f / (float)CS$<>8__locals1.element.sideCount;
				float num6 = Mathf.Cos(3.1415927f * num5);
				float num7 = num6 * sdfRoundness;
				float num8 = num6 - num7;
				float num9 = 6.2831855f * num5;
				float w = num8 * Mathf.Tan(0.5f * num9);
				cmd.SetGlobalVector(LensFlareCommonSRP._FlareData4, new Vector4(sdfRoundness, num8, num9, w));
			}
			else if (CS$<>8__locals1.element.flareType == SRPLensFlareType.Ring)
			{
				cmd.SetGlobalVector(LensFlareCommonSRP._FlareData4, new Vector4(CS$<>8__locals1.element.noiseAmplitude, (float)CS$<>8__locals1.element.noiseFrequency, CS$<>8__locals1.element.noiseSpeed, 0f));
			}
			else
			{
				cmd.SetGlobalVector(LensFlareCommonSRP._FlareData4, new Vector4(sdfRoundness, 0f, 0f, 0f));
			}
			cmd.SetGlobalVector(LensFlareCommonSRP._FlareData5, new Vector4((float)CS$<>8__locals1.element.tintColorType, num, CS$<>8__locals1.element.shapeCutOffSpeed, CS$<>8__locals1.element.shapeCutOffRadius));
			if (LensFlareCommonSRP.ForceSingleElement(CS$<>8__locals1.element))
			{
				Vector2 vector3 = vector2;
				Vector2 lensFlareRayOffset = LensFlareCommonSRP.GetLensFlareRayOffset(CS$<>8__locals1.screenPos, CS$<>8__locals1.position, CS$<>8__locals1.globalCos0, CS$<>8__locals1.globalSin0, CS$<>8__locals1.vScreenRatio);
				if (CS$<>8__locals1.element.enableRadialDistortion)
				{
					Vector2 lensFlareRayOffset2 = LensFlareCommonSRP.GetLensFlareRayOffset(CS$<>8__locals1.screenPos, 0f, CS$<>8__locals1.globalCos0, CS$<>8__locals1.globalSin0, CS$<>8__locals1.vScreenRatio);
					vector3 = LensFlareCommonSRP.<ProcessLensFlareSRPElementsSingle>g__ComputeLocalSize|79_0(lensFlareRayOffset, lensFlareRayOffset2, vector3, CS$<>8__locals1.element.distortionCurve, ref CS$<>8__locals1);
				}
				Vector4 flareData2 = LensFlareCommonSRP.GetFlareData0(CS$<>8__locals1.screenPos, CS$<>8__locals1.element.translationScale, lensFlareRayOffset, CS$<>8__locals1.vScreenRatio, rotation, CS$<>8__locals1.position, num3, CS$<>8__locals1.element.positionOffset, CS$<>8__locals1.element.autoRotate);
				cmd.SetGlobalVector(LensFlareCommonSRP._FlareData0, flareData2);
				cmd.SetGlobalVector(LensFlareCommonSRP._FlareData2, new Vector4(CS$<>8__locals1.screenPos.x, CS$<>8__locals1.screenPos.y, vector3.x, vector3.y));
				cmd.SetGlobalVector(LensFlareCommonSRP._FlareColorValue, color2);
				Blitter.DrawQuad(cmd, lensFlareShader, shaderPass);
				return;
			}
			float num10 = 2f * CS$<>8__locals1.element.lengthSpread / (float)(CS$<>8__locals1.element.count - 1);
			if (CS$<>8__locals1.element.distribution == SRPLensFlareDistribution.Uniform)
			{
				float num11 = 0f;
				for (int i = 0; i < CS$<>8__locals1.element.count; i++)
				{
					Vector2 vector4 = vector2;
					Vector2 lensFlareRayOffset3 = LensFlareCommonSRP.GetLensFlareRayOffset(CS$<>8__locals1.screenPos, CS$<>8__locals1.position, CS$<>8__locals1.globalCos0, CS$<>8__locals1.globalSin0, CS$<>8__locals1.vScreenRatio);
					if (CS$<>8__locals1.element.enableRadialDistortion)
					{
						Vector2 lensFlareRayOffset4 = LensFlareCommonSRP.GetLensFlareRayOffset(CS$<>8__locals1.screenPos, 0f, CS$<>8__locals1.globalCos0, CS$<>8__locals1.globalSin0, CS$<>8__locals1.vScreenRatio);
						vector4 = LensFlareCommonSRP.<ProcessLensFlareSRPElementsSingle>g__ComputeLocalSize|79_0(lensFlareRayOffset3, lensFlareRayOffset4, vector4, CS$<>8__locals1.element.distortionCurve, ref CS$<>8__locals1);
					}
					float time = (CS$<>8__locals1.element.count >= 2) ? ((float)i / (float)(CS$<>8__locals1.element.count - 1)) : 0.5f;
					Color b = CS$<>8__locals1.element.colorGradient.Evaluate(time);
					Vector4 flareData3 = LensFlareCommonSRP.GetFlareData0(CS$<>8__locals1.screenPos, CS$<>8__locals1.element.translationScale, lensFlareRayOffset3, CS$<>8__locals1.vScreenRatio, rotation + num11, CS$<>8__locals1.position, num3, CS$<>8__locals1.element.positionOffset, CS$<>8__locals1.element.autoRotate);
					cmd.SetGlobalVector(LensFlareCommonSRP._FlareData0, flareData3);
					flareData1.y = (float)i;
					cmd.SetGlobalVector(LensFlareCommonSRP._FlareData1, flareData1);
					cmd.SetGlobalVector(LensFlareCommonSRP._FlareData2, new Vector4(CS$<>8__locals1.screenPos.x, CS$<>8__locals1.screenPos.y, vector4.x, vector4.y));
					cmd.SetGlobalVector(LensFlareCommonSRP._FlareColorValue, color2 * b);
					Blitter.DrawQuad(cmd, lensFlareShader, shaderPass);
					CS$<>8__locals1.position += num10;
					num11 += CS$<>8__locals1.element.uniformAngle;
				}
				return;
			}
			if (CS$<>8__locals1.element.distribution == SRPLensFlareDistribution.Random)
			{
				Random.State state = Random.state;
				Random.InitState(CS$<>8__locals1.element.seed);
				Vector2 a = new Vector2(CS$<>8__locals1.globalSin0, CS$<>8__locals1.globalCos0);
				a *= CS$<>8__locals1.element.positionVariation.y;
				for (int j = 0; j < CS$<>8__locals1.element.count; j++)
				{
					float num12 = LensFlareCommonSRP.<ProcessLensFlareSRPElementsSingle>g__RandomRange|79_1(-1f, 1f) * CS$<>8__locals1.element.intensityVariation + 1f;
					Vector2 lensFlareRayOffset5 = LensFlareCommonSRP.GetLensFlareRayOffset(CS$<>8__locals1.screenPos, CS$<>8__locals1.position, CS$<>8__locals1.globalCos0, CS$<>8__locals1.globalSin0, CS$<>8__locals1.vScreenRatio);
					Vector2 vector5 = vector2;
					if (CS$<>8__locals1.element.enableRadialDistortion)
					{
						Vector2 lensFlareRayOffset6 = LensFlareCommonSRP.GetLensFlareRayOffset(CS$<>8__locals1.screenPos, 0f, CS$<>8__locals1.globalCos0, CS$<>8__locals1.globalSin0, CS$<>8__locals1.vScreenRatio);
						vector5 = LensFlareCommonSRP.<ProcessLensFlareSRPElementsSingle>g__ComputeLocalSize|79_0(lensFlareRayOffset5, lensFlareRayOffset6, vector5, CS$<>8__locals1.element.distortionCurve, ref CS$<>8__locals1);
					}
					vector5 += vector5 * (CS$<>8__locals1.element.scaleVariation * LensFlareCommonSRP.<ProcessLensFlareSRPElementsSingle>g__RandomRange|79_1(-1f, 1f));
					Color b2 = CS$<>8__locals1.element.colorGradient.Evaluate(LensFlareCommonSRP.<ProcessLensFlareSRPElementsSingle>g__RandomRange|79_1(0f, 1f));
					Vector2 positionOffset = CS$<>8__locals1.element.positionOffset + LensFlareCommonSRP.<ProcessLensFlareSRPElementsSingle>g__RandomRange|79_1(-1f, 1f) * a;
					float angleDeg = rotation + LensFlareCommonSRP.<ProcessLensFlareSRPElementsSingle>g__RandomRange|79_1(-3.1415927f, 3.1415927f) * CS$<>8__locals1.element.rotationVariation;
					if (num12 > 0f)
					{
						Vector4 flareData4 = LensFlareCommonSRP.GetFlareData0(CS$<>8__locals1.screenPos, CS$<>8__locals1.element.translationScale, lensFlareRayOffset5, CS$<>8__locals1.vScreenRatio, angleDeg, CS$<>8__locals1.position, num3, positionOffset, CS$<>8__locals1.element.autoRotate);
						cmd.SetGlobalVector(LensFlareCommonSRP._FlareData0, flareData4);
						flareData1.y = (float)j;
						cmd.SetGlobalVector(LensFlareCommonSRP._FlareData1, flareData1);
						cmd.SetGlobalVector(LensFlareCommonSRP._FlareData2, new Vector4(CS$<>8__locals1.screenPos.x, CS$<>8__locals1.screenPos.y, vector5.x, vector5.y));
						cmd.SetGlobalVector(LensFlareCommonSRP._FlareColorValue, color2 * b2 * num12);
						Blitter.DrawQuad(cmd, lensFlareShader, shaderPass);
					}
					CS$<>8__locals1.position += num10;
					CS$<>8__locals1.position += 0.5f * num10 * LensFlareCommonSRP.<ProcessLensFlareSRPElementsSingle>g__RandomRange|79_1(-1f, 1f) * CS$<>8__locals1.element.positionVariation.x;
				}
				Random.state = state;
				return;
			}
			if (CS$<>8__locals1.element.distribution == SRPLensFlareDistribution.Curve)
			{
				for (int k = 0; k < CS$<>8__locals1.element.count; k++)
				{
					float time2 = (CS$<>8__locals1.element.count >= 2) ? ((float)k / (float)(CS$<>8__locals1.element.count - 1)) : 0.5f;
					Color b3 = CS$<>8__locals1.element.colorGradient.Evaluate(time2);
					float num13 = (CS$<>8__locals1.element.positionCurve.length > 0) ? CS$<>8__locals1.element.positionCurve.Evaluate(time2) : 1f;
					float position = CS$<>8__locals1.position + 2f * CS$<>8__locals1.element.lengthSpread * num13;
					Vector2 lensFlareRayOffset7 = LensFlareCommonSRP.GetLensFlareRayOffset(CS$<>8__locals1.screenPos, position, CS$<>8__locals1.globalCos0, CS$<>8__locals1.globalSin0, CS$<>8__locals1.vScreenRatio);
					Vector2 vector6 = vector2;
					if (CS$<>8__locals1.element.enableRadialDistortion)
					{
						Vector2 lensFlareRayOffset8 = LensFlareCommonSRP.GetLensFlareRayOffset(CS$<>8__locals1.screenPos, 0f, CS$<>8__locals1.globalCos0, CS$<>8__locals1.globalSin0, CS$<>8__locals1.vScreenRatio);
						vector6 = LensFlareCommonSRP.<ProcessLensFlareSRPElementsSingle>g__ComputeLocalSize|79_0(lensFlareRayOffset7, lensFlareRayOffset8, vector6, CS$<>8__locals1.element.distortionCurve, ref CS$<>8__locals1);
					}
					float d = (CS$<>8__locals1.element.scaleCurve.length > 0) ? CS$<>8__locals1.element.scaleCurve.Evaluate(time2) : 1f;
					vector6 *= d;
					float num14 = CS$<>8__locals1.element.uniformAngleCurve.Evaluate(time2) * (180f - 180f / (float)CS$<>8__locals1.element.count);
					Vector4 flareData5 = LensFlareCommonSRP.GetFlareData0(CS$<>8__locals1.screenPos, CS$<>8__locals1.element.translationScale, lensFlareRayOffset7, CS$<>8__locals1.vScreenRatio, rotation + num14, position, num3, CS$<>8__locals1.element.positionOffset, CS$<>8__locals1.element.autoRotate);
					cmd.SetGlobalVector(LensFlareCommonSRP._FlareData0, flareData5);
					flareData1.y = (float)k;
					cmd.SetGlobalVector(LensFlareCommonSRP._FlareData1, flareData1);
					cmd.SetGlobalVector(LensFlareCommonSRP._FlareData2, new Vector4(CS$<>8__locals1.screenPos.x, CS$<>8__locals1.screenPos.y, vector6.x, vector6.y));
					cmd.SetGlobalVector(LensFlareCommonSRP._FlareColorValue, color2 * b3);
					Blitter.DrawQuad(cmd, lensFlareShader, shaderPass);
				}
			}
		}

		private static void ProcessLensFlareSRPElements(ref LensFlareDataElementSRP[] elements, CommandBuffer cmd, Color globalColorModulation, Light light, float compIntensity, float scale, Material lensFlareShader, Vector2 screenPos, bool compAllowOffScreen, Vector2 vScreenRatio, Vector4 flareData1, bool preview, int depth)
		{
			if (depth > 16)
			{
				Debug.LogWarning("LensFlareSRPAsset contains too deep recursive asset (> 16). Be careful to not have recursive aggregation, A contains B, B contains A, ... which will produce an infinite loop.");
				return;
			}
			LensFlareDataElementSRP[] array = elements;
			for (int i = 0; i < array.Length; i++)
			{
				LensFlareCommonSRP.ProcessLensFlareSRPElementsSingle(array[i], cmd, globalColorModulation, light, compIntensity, scale, lensFlareShader, screenPos, compAllowOffScreen, vScreenRatio, flareData1, preview, depth);
			}
		}

		[Obsolete("Use DoLensFlareDataDrivenCommon without _FlareOcclusionRemapTex.._FlareData4 parameters.")]
		public static void DoLensFlareDataDrivenCommon(Material lensFlareShader, Camera cam, Rect viewport, XRPass xr, int xrIndex, float actualWidth, float actualHeight, bool usePanini, float paniniDistance, float paniniCropToFit, bool isCameraRelative, Vector3 cameraPositionWS, Matrix4x4 viewProjMatrix, UnsafeCommandBuffer cmd, bool taaEnabled, bool hasCloudLayer, Texture cloudOpacityTexture, Texture sunOcclusionTexture, RenderTargetIdentifier colorBuffer, Func<Light, Camera, Vector3, float> GetLensFlareLightAttenuation, int _FlareOcclusionRemapTex, int _FlareOcclusionTex, int _FlareOcclusionIndex, int _FlareCloudOpacity, int _FlareSunOcclusionTex, int _FlareTex, int _FlareColorValue, int _FlareData0, int _FlareData1, int _FlareData2, int _FlareData3, int _FlareData4, bool debugView)
		{
			LensFlareCommonSRP.DoLensFlareDataDrivenCommon(lensFlareShader, cam, viewport, xr, xrIndex, actualWidth, actualHeight, usePanini, paniniDistance, paniniCropToFit, isCameraRelative, cameraPositionWS, viewProjMatrix, cmd, taaEnabled, hasCloudLayer, cloudOpacityTexture, sunOcclusionTexture, colorBuffer, GetLensFlareLightAttenuation, debugView);
		}

		public static void DoLensFlareDataDrivenCommon(Material lensFlareShader, Camera cam, Rect viewport, XRPass xr, int xrIndex, float actualWidth, float actualHeight, bool usePanini, float paniniDistance, float paniniCropToFit, bool isCameraRelative, Vector3 cameraPositionWS, Matrix4x4 viewProjMatrix, UnsafeCommandBuffer cmd, bool taaEnabled, bool hasCloudLayer, Texture cloudOpacityTexture, Texture sunOcclusionTexture, RenderTargetIdentifier colorBuffer, Func<Light, Camera, Vector3, float> GetLensFlareLightAttenuation, bool debugView)
		{
			LensFlareCommonSRP.DoLensFlareDataDrivenCommon(lensFlareShader, cam, viewport, xr, xrIndex, actualWidth, actualHeight, usePanini, paniniDistance, paniniCropToFit, isCameraRelative, cameraPositionWS, viewProjMatrix, cmd.m_WrappedCommandBuffer, taaEnabled, hasCloudLayer, cloudOpacityTexture, sunOcclusionTexture, colorBuffer, GetLensFlareLightAttenuation, debugView);
		}

		[Obsolete("Use DoLensFlareDataDrivenCommon without _FlareOcclusionRemapTex.._FlareData4 parameters.")]
		public static void DoLensFlareDataDrivenCommon(Material lensFlareShader, Camera cam, Rect viewport, XRPass xr, int xrIndex, float actualWidth, float actualHeight, bool usePanini, float paniniDistance, float paniniCropToFit, bool isCameraRelative, Vector3 cameraPositionWS, Matrix4x4 viewProjMatrix, CommandBuffer cmd, bool taaEnabled, bool hasCloudLayer, Texture cloudOpacityTexture, Texture sunOcclusionTexture, RenderTargetIdentifier colorBuffer, Func<Light, Camera, Vector3, float> GetLensFlareLightAttenuation, int _FlareOcclusionRemapTex, int _FlareOcclusionTex, int _FlareOcclusionIndex, int _FlareCloudOpacity, int _FlareSunOcclusionTex, int _FlareTex, int _FlareColorValue, int _FlareData0, int _FlareData1, int _FlareData2, int _FlareData3, int _FlareData4, bool debugView)
		{
			LensFlareCommonSRP.DoLensFlareDataDrivenCommon(lensFlareShader, cam, viewport, xr, xrIndex, actualWidth, actualHeight, usePanini, paniniDistance, paniniCropToFit, isCameraRelative, cameraPositionWS, viewProjMatrix, cmd, taaEnabled, hasCloudLayer, cloudOpacityTexture, sunOcclusionTexture, colorBuffer, GetLensFlareLightAttenuation, debugView);
		}

		public static void DoLensFlareDataDrivenCommon(Material lensFlareShader, Camera cam, Rect viewport, XRPass xr, int xrIndex, float actualWidth, float actualHeight, bool usePanini, float paniniDistance, float paniniCropToFit, bool isCameraRelative, Vector3 cameraPositionWS, Matrix4x4 viewProjMatrix, CommandBuffer cmd, bool taaEnabled, bool hasCloudLayer, Texture cloudOpacityTexture, Texture sunOcclusionTexture, RenderTargetIdentifier colorBuffer, Func<Light, Camera, Vector3, float> GetLensFlareLightAttenuation, bool debugView)
		{
			xr.StopSinglePass(cmd);
			if (LensFlareCommonSRP.Instance.IsEmpty())
			{
				return;
			}
			Vector2 vector = new Vector2(actualWidth, actualHeight);
			float x = vector.x / vector.y;
			Vector2 vScreenRatio = new Vector2(x, 1f);
			if (xr.enabled && xr.singlePassEnabled)
			{
				CoreUtils.SetRenderTarget(cmd, colorBuffer, ClearFlag.None, 0, CubemapFace.Unknown, xrIndex);
				cmd.SetGlobalInt(LensFlareCommonSRP._ViewId, xrIndex);
			}
			else
			{
				CoreUtils.SetRenderTarget(cmd, colorBuffer, ClearFlag.None, 0, CubemapFace.Unknown, -1);
				if (xr.enabled)
				{
					cmd.SetGlobalInt(LensFlareCommonSRP._ViewId, xr.multipassId);
				}
				else
				{
					cmd.SetGlobalInt(LensFlareCommonSRP._ViewId, 0);
				}
			}
			cmd.SetViewport(viewport);
			if (debugView)
			{
				cmd.ClearRenderTarget(false, true, Color.black);
			}
			foreach (LensFlareCommonSRP.LensFlareCompInfo lensFlareCompInfo in LensFlareCommonSRP.m_Data)
			{
				if (lensFlareCompInfo != null && !(lensFlareCompInfo.comp == null))
				{
					LensFlareComponentSRP comp = lensFlareCompInfo.comp;
					LensFlareDataSRP lensFlareData = comp.lensFlareData;
					if (!LensFlareCommonSRP.IsLensFlareSRPHidden(cam, comp, lensFlareData))
					{
						Light light = null;
						if (!comp.TryGetComponent<Light>(out light))
						{
							light = null;
						}
						bool flag = false;
						Vector3 vector2;
						if (light != null && light.type == LightType.Directional)
						{
							vector2 = -light.transform.forward * cam.farClipPlane;
							flag = true;
						}
						else
						{
							vector2 = comp.transform.position;
						}
						if (comp.lightOverride != null)
						{
							light = comp.lightOverride;
						}
						Vector3 vector3 = LensFlareCommonSRP.WorldToViewport(cam, !flag, isCameraRelative, viewProjMatrix, vector2);
						if (usePanini && cam == Camera.main)
						{
							vector3 = LensFlareCommonSRP.DoPaniniProjection(vector3, actualWidth, actualHeight, cam.fieldOfView, paniniCropToFit, paniniDistance);
						}
						if (vector3.z >= 0f && (comp.allowOffScreen || (vector3.x >= 0f && vector3.x <= 1f && vector3.y >= 0f && vector3.y <= 1f)))
						{
							Vector3 rhs = vector2 - cameraPositionWS;
							if (Vector3.Dot(cam.transform.forward, rhs) >= 0f)
							{
								float magnitude = rhs.magnitude;
								float time = magnitude / comp.maxAttenuationDistance;
								float time2 = magnitude / comp.maxAttenuationScale;
								float num = (!flag && comp.distanceAttenuationCurve.length > 0) ? comp.distanceAttenuationCurve.Evaluate(time) : 1f;
								float num2 = (!flag && comp.scaleByDistanceCurve.length >= 1) ? comp.scaleByDistanceCurve.Evaluate(time2) : 1f;
								Color color = Color.white;
								if (light != null && comp.attenuationByLightShape)
								{
									color *= GetLensFlareLightAttenuation(light, cam, -rhs.normalized);
								}
								Vector2 vector4 = new Vector2(2f * vector3.x - 1f, -(2f * vector3.y - 1f));
								if (!SystemInfo.graphicsUVStartsAtTop && flag)
								{
									vector4.y = -vector4.y;
								}
								Vector2 vector5 = new Vector2(Mathf.Abs(vector4.x), Mathf.Abs(vector4.y));
								float time3 = Mathf.Max(vector5.x, vector5.y);
								float num3 = (comp.radialScreenAttenuationCurve.length > 0) ? comp.radialScreenAttenuationCurve.Evaluate(time3) : 1f;
								float num4 = comp.intensity * num3 * num;
								if (num4 > 0f)
								{
									color *= num;
									Vector3 normalized = (cam.transform.position - comp.transform.position).normalized;
									Vector3 vector6 = LensFlareCommonSRP.WorldToViewport(cam, !flag, isCameraRelative, viewProjMatrix, vector2 + normalized * comp.occlusionOffset);
									float d = flag ? comp.celestialProjectedOcclusionRadius(cam) : comp.occlusionRadius;
									Vector2 b = vector3;
									float magnitude2 = (LensFlareCommonSRP.WorldToViewport(cam, !flag, isCameraRelative, viewProjMatrix, vector2 + cam.transform.up * d) - b).magnitude;
									if (comp.useOcclusion && LensFlareCommonSRP.occlusionRT != null)
									{
										cmd.SetGlobalTexture(LensFlareCommonSRP._FlareOcclusionTex, LensFlareCommonSRP.occlusionRT);
										cmd.EnableShaderKeyword("FLARE_HAS_OCCLUSION");
									}
									else
									{
										cmd.DisableShaderKeyword("FLARE_HAS_OCCLUSION");
									}
									if (LensFlareCommonSRP.IsOcclusionRTCompatible())
									{
										cmd.DisableShaderKeyword("FLARE_OPENGL3_OR_OPENGLCORE");
									}
									else
									{
										cmd.EnableShaderKeyword("FLARE_OPENGL3_OR_OPENGLCORE");
									}
									cmd.SetGlobalVector(LensFlareCommonSRP._FlareOcclusionIndex, new Vector4((float)lensFlareCompInfo.index, 0f, 0f, 0f));
									cmd.SetGlobalTexture(LensFlareCommonSRP._FlareOcclusionRemapTex, comp.occlusionRemapCurve.GetTexture());
									Vector4 flareData = new Vector4(0f, comp.sampleCount, vector6.z, actualHeight / actualWidth);
									LensFlareCommonSRP.ProcessLensFlareSRPElements(ref lensFlareData.elements, cmd, color, light, num4, num2 * comp.scale, lensFlareShader, vector4, comp.allowOffScreen, vScreenRatio, flareData, false, 0);
								}
							}
						}
					}
				}
			}
			xr.StartSinglePass(cmd);
		}

		public static void DoLensFlareScreenSpaceCommon(Material lensFlareShader, Camera cam, float actualWidth, float actualHeight, Color tintColor, Texture originalBloomTexture, Texture bloomMipTexture, Texture spectralLut, Texture streakTextureTmp, Texture streakTextureTmp2, Vector4 parameters1, Vector4 parameters2, Vector4 parameters3, Vector4 parameters4, Vector4 parameters5, UnsafeCommandBuffer cmd, RTHandle result, bool debugView)
		{
			LensFlareCommonSRP.DoLensFlareScreenSpaceCommon(lensFlareShader, cam, actualWidth, actualHeight, tintColor, originalBloomTexture, bloomMipTexture, spectralLut, streakTextureTmp, streakTextureTmp2, parameters1, parameters2, parameters3, parameters4, parameters5, cmd.m_WrappedCommandBuffer, result, debugView);
		}

		[Obsolete("Use DoLensFlareScreenSpaceCommon without _Shader IDs parameters.")]
		public static void DoLensFlareScreenSpaceCommon(Material lensFlareShader, Camera cam, float actualWidth, float actualHeight, Color tintColor, Texture originalBloomTexture, Texture bloomMipTexture, Texture spectralLut, Texture streakTextureTmp, Texture streakTextureTmp2, Vector4 parameters1, Vector4 parameters2, Vector4 parameters3, Vector4 parameters4, Vector4 parameters5, CommandBuffer cmd, RTHandle result, int _LensFlareScreenSpaceBloomMipTexture, int _LensFlareScreenSpaceResultTexture, int _LensFlareScreenSpaceSpectralLut, int _LensFlareScreenSpaceStreakTex, int _LensFlareScreenSpaceMipLevel, int _LensFlareScreenSpaceTintColor, int _LensFlareScreenSpaceParams1, int _LensFlareScreenSpaceParams2, int _LensFlareScreenSpaceParams3, int _LensFlareScreenSpaceParams4, int _LensFlareScreenSpaceParams5, bool debugView)
		{
			LensFlareCommonSRP.DoLensFlareScreenSpaceCommon(lensFlareShader, cam, actualWidth, actualHeight, tintColor, originalBloomTexture, bloomMipTexture, spectralLut, streakTextureTmp, streakTextureTmp2, parameters1, parameters2, parameters3, parameters4, parameters5, cmd, result, debugView);
		}

		public static void DoLensFlareScreenSpaceCommon(Material lensFlareShader, Camera cam, float actualWidth, float actualHeight, Color tintColor, Texture originalBloomTexture, Texture bloomMipTexture, Texture spectralLut, Texture streakTextureTmp, Texture streakTextureTmp2, Vector4 parameters1, Vector4 parameters2, Vector4 parameters3, Vector4 parameters4, Vector4 parameters5, CommandBuffer cmd, RTHandle result, bool debugView)
		{
			parameters2.x = Mathf.Pow(parameters2.x, 0.25f);
			parameters3.z /= 20f;
			parameters4.y *= 10f;
			parameters4.z /= 90f;
			parameters5.y = 1f / parameters5.y;
			parameters5.z = 1f / parameters5.z;
			cmd.SetViewport(new Rect
			{
				width = actualWidth,
				height = actualHeight
			});
			if (debugView)
			{
				cmd.ClearRenderTarget(false, true, Color.black);
			}
			float num = parameters5.y;
			num *= actualWidth / actualHeight;
			parameters5.y = num;
			float num2 = parameters4.y;
			num2 *= actualWidth * 0.0005f;
			parameters4.y = num2;
			int shaderPass = lensFlareShader.FindPass("LensFlareScreenSpac Prefilter");
			int shaderPass2 = lensFlareShader.FindPass("LensFlareScreenSpace Downsample");
			int shaderPass3 = lensFlareShader.FindPass("LensFlareScreenSpace Upsample");
			int shaderPass4 = lensFlareShader.FindPass("LensFlareScreenSpace Composition");
			int shaderPass5 = lensFlareShader.FindPass("LensFlareScreenSpace Write to BloomTexture");
			cmd.SetGlobalTexture(LensFlareCommonSRP._LensFlareScreenSpaceBloomMipTexture, bloomMipTexture);
			cmd.SetGlobalTexture(LensFlareCommonSRP._LensFlareScreenSpaceSpectralLut, spectralLut);
			cmd.SetGlobalVector(LensFlareCommonSRP._LensFlareScreenSpaceParams1, parameters1);
			cmd.SetGlobalVector(LensFlareCommonSRP._LensFlareScreenSpaceParams2, parameters2);
			cmd.SetGlobalVector(LensFlareCommonSRP._LensFlareScreenSpaceParams3, parameters3);
			cmd.SetGlobalVector(LensFlareCommonSRP._LensFlareScreenSpaceParams4, parameters4);
			cmd.SetGlobalVector(LensFlareCommonSRP._LensFlareScreenSpaceParams5, parameters5);
			cmd.SetGlobalColor(LensFlareCommonSRP._LensFlareScreenSpaceTintColor, tintColor);
			if (parameters4.x > 0f)
			{
				CoreUtils.SetRenderTarget(cmd, streakTextureTmp, ClearFlag.None, 0, CubemapFace.Unknown, -1);
				Blitter.DrawQuad(cmd, lensFlareShader, shaderPass);
				int b = Mathf.FloorToInt(Mathf.Log(Mathf.Max(actualHeight, actualWidth), 2f));
				int num3 = Mathf.Max(1, b);
				int num4 = 2;
				int num5 = 0;
				bool flag = false;
				for (int i = 0; i < num3; i++)
				{
					flag = (i % 2 == 0);
					cmd.SetGlobalInt(LensFlareCommonSRP._LensFlareScreenSpaceMipLevel, i);
					cmd.SetGlobalTexture(LensFlareCommonSRP._LensFlareScreenSpaceStreakTex, flag ? streakTextureTmp : streakTextureTmp2);
					CoreUtils.SetRenderTarget(cmd, flag ? streakTextureTmp2 : streakTextureTmp, ClearFlag.None, 0, CubemapFace.Unknown, -1);
					Blitter.DrawQuad(cmd, lensFlareShader, shaderPass2);
				}
				if (flag)
				{
					num5 = 1;
				}
				for (int j = num5; j < num5 + num4; j++)
				{
					flag = (j % 2 == 0);
					cmd.SetGlobalInt(LensFlareCommonSRP._LensFlareScreenSpaceMipLevel, j - num5);
					cmd.SetGlobalTexture(LensFlareCommonSRP._LensFlareScreenSpaceStreakTex, flag ? streakTextureTmp : streakTextureTmp2);
					CoreUtils.SetRenderTarget(cmd, flag ? streakTextureTmp2 : streakTextureTmp, ClearFlag.None, 0, CubemapFace.Unknown, -1);
					Blitter.DrawQuad(cmd, lensFlareShader, shaderPass3);
				}
				cmd.SetGlobalTexture(LensFlareCommonSRP._LensFlareScreenSpaceStreakTex, flag ? streakTextureTmp2 : streakTextureTmp);
			}
			CoreUtils.SetRenderTarget(cmd, result, ClearFlag.None, 0, CubemapFace.Unknown, -1);
			Blitter.DrawQuad(cmd, lensFlareShader, shaderPass4);
			cmd.SetGlobalTexture(LensFlareCommonSRP._LensFlareScreenSpaceResultTexture, result);
			CoreUtils.SetRenderTarget(cmd, originalBloomTexture, ClearFlag.None, 0, CubemapFace.Unknown, -1);
			Blitter.DrawQuad(cmd, lensFlareShader, shaderPass5);
		}

		private static Vector2 DoPaniniProjection(Vector2 screenPos, float actualWidth, float actualHeight, float fieldOfView, float paniniProjectionCropToFit, float paniniProjectionDistance)
		{
			Vector2 vector = LensFlareCommonSRP.CalcViewExtents(actualWidth, actualHeight, fieldOfView);
			Vector2 vector2 = LensFlareCommonSRP.CalcCropExtents(actualWidth, actualHeight, fieldOfView, paniniProjectionDistance);
			float a = vector2.x / vector.x;
			float b = vector2.y / vector.y;
			float value = Mathf.Min(a, b);
			float d = Mathf.Lerp(1f, Mathf.Clamp01(value), paniniProjectionCropToFit);
			Vector2 vector3 = LensFlareCommonSRP.Panini_Generic_Inv(new Vector2(2f * screenPos.x - 1f, 2f * screenPos.y - 1f) * vector, paniniProjectionDistance) / (vector * d);
			return new Vector2(0.5f * vector3.x + 0.5f, 0.5f * vector3.y + 0.5f);
		}

		private static Vector2 CalcViewExtents(float actualWidth, float actualHeight, float fieldOfView)
		{
			float num = fieldOfView * 0.017453292f;
			float num2 = actualWidth / actualHeight;
			float num3 = Mathf.Tan(0.5f * num);
			return new Vector2(num2 * num3, num3);
		}

		private static Vector2 CalcCropExtents(float actualWidth, float actualHeight, float fieldOfView, float d)
		{
			float num = 1f + d;
			Vector2 vector = LensFlareCommonSRP.CalcViewExtents(actualWidth, actualHeight, fieldOfView);
			float num2 = Mathf.Sqrt(vector.x * vector.x + 1f);
			float num3 = 1f / num2;
			float num4 = num3 + d;
			return vector * num3 * (num / num4);
		}

		private static Vector2 Panini_Generic_Inv(Vector2 projPos, float d)
		{
			float num = 1f + d;
			float num2 = Mathf.Sqrt(projPos.x * projPos.x + 1f);
			float num3 = 1f / num2;
			float num4 = num3 + d;
			return projPos * num3 * (num / num4);
		}

		[CompilerGenerated]
		internal static float <ShapeAttenuationAreaTubeLight>g__Fpo|62_0(float d, float l)
		{
			return l / (d * (d * d + l * l)) + Mathf.Atan(l / d) / (d * d);
		}

		[CompilerGenerated]
		internal static float <ShapeAttenuationAreaTubeLight>g__Fwt|62_1(float d, float l)
		{
			return l * l / (d * (d * d + l * l));
		}

		[CompilerGenerated]
		internal static float <ShapeAttenuationAreaTubeLight>g__DiffLineIntegral|62_2(Vector3 p1, Vector3 p2)
		{
			Vector3 normalized = (p2 - p1).normalized;
			float result;
			if ((double)p1.z <= 0.0 && (double)p2.z <= 0.0)
			{
				result = 0f;
			}
			else
			{
				if ((double)p1.z < 0.0)
				{
					p1 = (p1 * p2.z - p2 * p1.z) / (p2.z - p1.z);
				}
				if ((double)p2.z < 0.0)
				{
					p2 = (-p1 * p2.z + p2 * p1.z) / (-p2.z + p1.z);
				}
				float num = Vector3.Dot(p1, normalized);
				float l = Vector3.Dot(p2, normalized);
				Vector3 vector = p1 - num * normalized;
				float magnitude = vector.magnitude;
				result = ((LensFlareCommonSRP.<ShapeAttenuationAreaTubeLight>g__Fpo|62_0(magnitude, l) - LensFlareCommonSRP.<ShapeAttenuationAreaTubeLight>g__Fpo|62_0(magnitude, num)) * vector.z + (LensFlareCommonSRP.<ShapeAttenuationAreaTubeLight>g__Fwt|62_1(magnitude, l) - LensFlareCommonSRP.<ShapeAttenuationAreaTubeLight>g__Fwt|62_1(magnitude, num)) * normalized.z) / 3.1415927f;
			}
			return result;
		}

		[CompilerGenerated]
		internal static Vector2 <ProcessLensFlareSRPElementsSingle>g__ComputeLocalSize|79_0(Vector2 rayOff, Vector2 rayOff0, Vector2 curSize, AnimationCurve distortionCurve, ref LensFlareCommonSRP.<>c__DisplayClass79_0 A_4)
		{
			LensFlareCommonSRP.GetLensFlareRayOffset(A_4.screenPos, A_4.position, A_4.globalCos0, A_4.globalSin0, A_4.vScreenRatio);
			float time;
			if (!A_4.element.distortionRelativeToCenter)
			{
				Vector2 vector = (rayOff - rayOff0) * 0.5f;
				time = Mathf.Clamp01(Mathf.Max(Mathf.Abs(vector.x), Mathf.Abs(vector.y)));
			}
			else
			{
				time = Mathf.Clamp01((A_4.screenPos + (rayOff + new Vector2(A_4.element.positionOffset.x, -A_4.element.positionOffset.y)) * A_4.element.translationScale).magnitude);
			}
			float t = Mathf.Clamp01(distortionCurve.Evaluate(time));
			return new Vector2(Mathf.Lerp(curSize.x, A_4.element.targetSizeDistortion.x * A_4.combinedScale / A_4.usedAspectRatio, t), Mathf.Lerp(curSize.y, A_4.element.targetSizeDistortion.y * A_4.combinedScale, t));
		}

		[CompilerGenerated]
		internal static float <ProcessLensFlareSRPElementsSingle>g__RandomRange|79_1(float min, float max)
		{
			return Random.Range(min, max);
		}

		private static LensFlareCommonSRP m_Instance = null;

		private static readonly object m_Padlock = new object();

		private static List<LensFlareCommonSRP.LensFlareCompInfo> m_Data = new List<LensFlareCommonSRP.LensFlareCompInfo>();

		private static List<int> m_AvailableIndicies = new List<int>();

		public static int maxLensFlareWithOcclusion = 128;

		public static int maxLensFlareWithOcclusionTemporalSample = 8;

		public static int mergeNeeded = 1;

		public static RTHandle occlusionRT = null;

		private static int frameIdx = 0;

		internal static readonly int _FlareOcclusionPermutation = Shader.PropertyToID("_FlareOcclusionPermutation");

		internal static readonly int _FlareOcclusionRemapTex = Shader.PropertyToID("_FlareOcclusionRemapTex");

		internal static readonly int _FlareOcclusionTex = Shader.PropertyToID("_FlareOcclusionTex");

		internal static readonly int _FlareOcclusionIndex = Shader.PropertyToID("_FlareOcclusionIndex");

		internal static readonly int _FlareCloudOpacity = Shader.PropertyToID("_FlareCloudOpacity");

		internal static readonly int _FlareSunOcclusionTex = Shader.PropertyToID("_FlareSunOcclusionTex");

		internal static readonly int _FlareTex = Shader.PropertyToID("_FlareTex");

		internal static readonly int _FlareColorValue = Shader.PropertyToID("_FlareColorValue");

		internal static readonly int _FlareData0 = Shader.PropertyToID("_FlareData0");

		internal static readonly int _FlareData1 = Shader.PropertyToID("_FlareData1");

		internal static readonly int _FlareData2 = Shader.PropertyToID("_FlareData2");

		internal static readonly int _FlareData3 = Shader.PropertyToID("_FlareData3");

		internal static readonly int _FlareData4 = Shader.PropertyToID("_FlareData4");

		internal static readonly int _FlareData5 = Shader.PropertyToID("_FlareData5");

		internal static readonly int _FlareRadialTint = Shader.PropertyToID("_FlareRadialTint");

		internal static readonly int _ViewId = Shader.PropertyToID("_ViewId");

		internal static readonly int _LensFlareScreenSpaceBloomMipTexture = Shader.PropertyToID("_LensFlareScreenSpaceBloomMipTexture");

		internal static readonly int _LensFlareScreenSpaceResultTexture = Shader.PropertyToID("_LensFlareScreenSpaceResultTexture");

		internal static readonly int _LensFlareScreenSpaceSpectralLut = Shader.PropertyToID("_LensFlareScreenSpaceSpectralLut");

		internal static readonly int _LensFlareScreenSpaceStreakTex = Shader.PropertyToID("_LensFlareScreenSpaceStreakTex");

		internal static readonly int _LensFlareScreenSpaceMipLevel = Shader.PropertyToID("_LensFlareScreenSpaceMipLevel");

		internal static readonly int _LensFlareScreenSpaceTintColor = Shader.PropertyToID("_LensFlareScreenSpaceTintColor");

		internal static readonly int _LensFlareScreenSpaceParams1 = Shader.PropertyToID("_LensFlareScreenSpaceParams1");

		internal static readonly int _LensFlareScreenSpaceParams2 = Shader.PropertyToID("_LensFlareScreenSpaceParams2");

		internal static readonly int _LensFlareScreenSpaceParams3 = Shader.PropertyToID("_LensFlareScreenSpaceParams3");

		internal static readonly int _LensFlareScreenSpaceParams4 = Shader.PropertyToID("_LensFlareScreenSpaceParams4");

		internal static readonly int _LensFlareScreenSpaceParams5 = Shader.PropertyToID("_LensFlareScreenSpaceParams5");

		private static readonly bool s_SupportsLensFlare16bitsFormat = SystemInfo.IsFormatSupported(GraphicsFormat.R16_SFloat, GraphicsFormatUsage.Render);

		private static readonly bool s_SupportsLensFlare32bitsFormat = SystemInfo.IsFormatSupported(GraphicsFormat.R32_SFloat, GraphicsFormatUsage.Render);

		private static readonly bool s_SupportsLensFlare16bitsFormatWithLoadStore = SystemInfo.IsFormatSupported(GraphicsFormat.R16_SFloat, GraphicsFormatUsage.LoadStore);

		private static readonly bool s_SupportsLensFlare32bitsFormatWithLoadStore = SystemInfo.IsFormatSupported(GraphicsFormat.R32_SFloat, GraphicsFormatUsage.LoadStore);

		internal class LensFlareCompInfo
		{
			internal LensFlareCompInfo(int idx, LensFlareComponentSRP cmp)
			{
				this.index = idx;
				this.comp = cmp;
			}

			internal int index;

			internal LensFlareComponentSRP comp;
		}
	}
}
