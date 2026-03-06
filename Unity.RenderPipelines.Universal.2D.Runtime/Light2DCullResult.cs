using System;
using System.Collections.Generic;
using Unity.Mathematics;

namespace UnityEngine.Rendering.Universal
{
	internal class Light2DCullResult : ILight2DCullResult
	{
		public List<Light2D> visibleLights
		{
			get
			{
				return this.m_VisibleLights;
			}
		}

		public HashSet<ShadowCasterGroup2D> visibleShadows
		{
			get
			{
				return this.m_VisibleShadows;
			}
		}

		public bool IsSceneLit()
		{
			return Light2DManager.lights.Count > 0;
		}

		public LightStats GetLightStatsByLayer(int layerID, ref LayerBatch layer)
		{
			layer.lights.Clear();
			layer.shadowIndices.Clear();
			layer.shadowCasters.Clear();
			LightStats result = default(LightStats);
			foreach (Light2D light2D in this.visibleLights)
			{
				if (light2D.IsLitLayer(layerID))
				{
					if (light2D.normalMapQuality != Light2D.NormalMapQuality.Disabled)
					{
						result.totalNormalMapUsage++;
					}
					if (light2D.volumeIntensity > 0f && light2D.volumetricEnabled)
					{
						result.totalVolumetricUsage++;
					}
					if (light2D.volumeIntensity > 0f && light2D.volumetricEnabled && RendererLighting.CanCastShadows(light2D, layerID))
					{
						result.totalVolumetricShadowUsage++;
					}
					result.blendStylesUsed |= 1U << light2D.blendStyleIndex;
					if (light2D.lightType != Light2D.LightType.Global)
					{
						result.blendStylesWithLights |= 1U << light2D.blendStyleIndex;
					}
					bool flag = false;
					if (RendererLighting.CanCastShadows(light2D, layerID))
					{
						foreach (ShadowCasterGroup2D shadowCasterGroup2D in this.visibleShadows)
						{
							List<ShadowCaster2D> shadowCasters = shadowCasterGroup2D.GetShadowCasters();
							if (shadowCasters != null)
							{
								foreach (ShadowCaster2D shadowCaster2D in shadowCasters)
								{
									if (shadowCaster2D.IsLit(light2D) && shadowCaster2D.IsShadowedLayer(layerID))
									{
										flag = true;
										result.totalShadows++;
										if (!layer.shadowCasters.Contains(shadowCasterGroup2D))
										{
											layer.shadowCasters.Add(shadowCasterGroup2D);
										}
									}
								}
							}
						}
					}
					if (flag)
					{
						result.totalShadowLights++;
						layer.shadowIndices.Add(layer.lights.Count);
					}
					result.totalLights++;
					layer.lights.Add(light2D);
				}
			}
			return result;
		}

		public void SetupCulling(ref ScriptableCullingParameters cullingParameters, Camera camera)
		{
			this.m_VisibleLights.Clear();
			foreach (Light2D light2D in Light2DManager.lights)
			{
				if ((camera.cullingMask & 1 << light2D.gameObject.layer) != 0)
				{
					if (light2D.lightType == Light2D.LightType.Global)
					{
						this.m_VisibleLights.Add(light2D);
					}
					else
					{
						Vector3 position = light2D.boundingSphere.position;
						bool flag = false;
						for (int i = 0; i < cullingParameters.cullingPlaneCount; i++)
						{
							Plane cullingPlane = cullingParameters.GetCullingPlane(i);
							if (math.dot(position, cullingPlane.normal) + cullingPlane.distance < -light2D.boundingSphere.radius)
							{
								flag = true;
								break;
							}
						}
						if (!flag)
						{
							this.m_VisibleLights.Add(light2D);
						}
					}
				}
			}
			this.m_VisibleLights.Sort((Light2D l1, Light2D l2) => l1.lightOrder - l2.lightOrder);
			this.m_VisibleShadows.Clear();
			if (ShadowCasterGroup2DManager.shadowCasterGroups != null)
			{
				foreach (ShadowCasterGroup2D shadowCasterGroup2D in ShadowCasterGroup2DManager.shadowCasterGroups)
				{
					List<ShadowCaster2D> shadowCasters = shadowCasterGroup2D.GetShadowCasters();
					if (shadowCasters != null)
					{
						foreach (ShadowCaster2D shadowCaster2D in shadowCasters)
						{
							foreach (Light2D light in this.m_VisibleLights)
							{
								if (shadowCaster2D.IsLit(light) && !this.m_VisibleShadows.Contains(shadowCasterGroup2D))
								{
									this.m_VisibleShadows.Add(shadowCasterGroup2D);
									break;
								}
							}
							if (this.m_VisibleShadows.Contains(shadowCasterGroup2D))
							{
								break;
							}
						}
					}
				}
			}
		}

		private List<Light2D> m_VisibleLights = new List<Light2D>();

		private HashSet<ShadowCasterGroup2D> m_VisibleShadows = new HashSet<ShadowCasterGroup2D>();
	}
}
