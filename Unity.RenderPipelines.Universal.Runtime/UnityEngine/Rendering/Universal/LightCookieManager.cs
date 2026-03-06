using System;
using System.Runtime.InteropServices;
using Unity.Mathematics;
using UnityEngine.Experimental.Rendering;

namespace UnityEngine.Rendering.Universal
{
	internal class LightCookieManager : IDisposable
	{
		internal bool IsKeywordLightCookieEnabled { get; private set; }

		internal RTHandle AdditionalLightsCookieAtlasTexture
		{
			get
			{
				Texture2DAtlas additionalLightsCookieAtlas = this.m_AdditionalLightsCookieAtlas;
				if (additionalLightsCookieAtlas == null)
				{
					return null;
				}
				return additionalLightsCookieAtlas.AtlasTexture;
			}
		}

		public LightCookieManager(ref LightCookieManager.Settings settings)
		{
			this.m_Settings = settings;
			this.m_WorkMem = new LightCookieManager.WorkMemory();
		}

		private void InitAdditionalLights(int size)
		{
			Vector2Int resolution = this.m_Settings.atlas.resolution;
			int x = resolution.x;
			resolution = this.m_Settings.atlas.resolution;
			this.m_AdditionalLightsCookieAtlas = new Texture2DAtlas(x, resolution.y, this.m_Settings.atlas.format, FilterMode.Bilinear, false, "Universal Light Cookie Atlas", false);
			this.m_AdditionalLightsCookieShaderData = new LightCookieManager.LightCookieShaderData(size, this.m_Settings.useStructuredBuffer);
			this.m_VisibleLightIndexToShaderDataIndex = new int[this.m_Settings.maxAdditionalLights + 1];
			this.m_CookieSizeDivisor = 1;
			this.m_PrevCookieRequestPixelCount = uint.MaxValue;
		}

		public bool isInitialized()
		{
			return this.m_AdditionalLightsCookieAtlas != null && this.m_AdditionalLightsCookieShaderData != null;
		}

		public void Dispose()
		{
			Texture2DAtlas additionalLightsCookieAtlas = this.m_AdditionalLightsCookieAtlas;
			if (additionalLightsCookieAtlas != null)
			{
				additionalLightsCookieAtlas.Release();
			}
			LightCookieManager.LightCookieShaderData additionalLightsCookieShaderData = this.m_AdditionalLightsCookieShaderData;
			if (additionalLightsCookieShaderData == null)
			{
				return;
			}
			additionalLightsCookieShaderData.Dispose();
		}

		public int GetLightCookieShaderDataIndex(int visibleLightIndex)
		{
			if (!this.isInitialized())
			{
				return -1;
			}
			return this.m_VisibleLightIndexToShaderDataIndex[visibleLightIndex];
		}

		public void Setup(CommandBuffer cmd, UniversalLightData lightData)
		{
			using (new ProfilingScope(cmd, ProfilingSampler.Get<URPProfileId>(URPProfileId.LightCookies)))
			{
				bool flag = lightData.mainLightIndex >= 0;
				if (flag)
				{
					VisibleLight visibleLight = lightData.visibleLights[lightData.mainLightIndex];
					flag = this.SetupMainLight(cmd, ref visibleLight);
				}
				bool flag2 = lightData.additionalLightsCount > 0;
				if (flag2)
				{
					flag2 = this.SetupAdditionalLights(cmd, lightData);
				}
				if (!flag2)
				{
					if (this.m_VisibleLightIndexToShaderDataIndex != null && this.m_AdditionalLightsCookieShaderData.isUploaded)
					{
						int num = this.m_VisibleLightIndexToShaderDataIndex.Length;
						for (int i = 0; i < num; i++)
						{
							this.m_VisibleLightIndexToShaderDataIndex[i] = -1;
						}
					}
					LightCookieManager.LightCookieShaderData additionalLightsCookieShaderData = this.m_AdditionalLightsCookieShaderData;
					if (additionalLightsCookieShaderData != null)
					{
						additionalLightsCookieShaderData.Clear(cmd);
					}
				}
				this.IsKeywordLightCookieEnabled = (flag || flag2);
				cmd.SetKeyword(ShaderGlobalKeywords.LightCookies, this.IsKeywordLightCookieEnabled);
			}
		}

		private bool SetupMainLight(CommandBuffer cmd, ref VisibleLight visibleMainLight)
		{
			Light light = visibleMainLight.light;
			Texture cookie = light.cookie;
			bool flag = cookie != null;
			if (flag)
			{
				Matrix4x4 identity = Matrix4x4.identity;
				float value = (float)this.GetLightCookieShaderFormat(cookie.graphicsFormat);
				UniversalAdditionalLightData universalAdditionalLightData;
				if (light.TryGetComponent<UniversalAdditionalLightData>(out universalAdditionalLightData))
				{
					this.GetLightUVScaleOffset(ref universalAdditionalLightData, ref identity);
				}
				Matrix4x4 value2 = LightCookieManager.s_DirLightProj * identity * visibleMainLight.localToWorldMatrix.inverse;
				cmd.SetGlobalTexture(LightCookieManager.ShaderProperty.mainLightTexture, cookie);
				cmd.SetGlobalMatrix(LightCookieManager.ShaderProperty.mainLightWorldToLight, value2);
				cmd.SetGlobalFloat(LightCookieManager.ShaderProperty.mainLightCookieTextureFormat, value);
				return flag;
			}
			cmd.SetGlobalTexture(LightCookieManager.ShaderProperty.mainLightTexture, Texture2D.whiteTexture);
			cmd.SetGlobalMatrix(LightCookieManager.ShaderProperty.mainLightWorldToLight, Matrix4x4.identity);
			cmd.SetGlobalFloat(LightCookieManager.ShaderProperty.mainLightCookieTextureFormat, -1f);
			return flag;
		}

		private LightCookieManager.LightCookieShaderFormat GetLightCookieShaderFormat(GraphicsFormat cookieFormat)
		{
			if (cookieFormat <= GraphicsFormat.R16_UInt)
			{
				if (cookieFormat <= GraphicsFormat.R8_UInt)
				{
					if (cookieFormat <= GraphicsFormat.R8_UNorm)
					{
						if (cookieFormat == GraphicsFormat.R8_SRGB || cookieFormat == GraphicsFormat.R8_UNorm)
						{
							return LightCookieManager.LightCookieShaderFormat.Red;
						}
					}
					else if (cookieFormat == GraphicsFormat.R8_SNorm || cookieFormat == GraphicsFormat.R8_UInt)
					{
						return LightCookieManager.LightCookieShaderFormat.Red;
					}
				}
				else if (cookieFormat <= GraphicsFormat.R16_UNorm)
				{
					if (cookieFormat == GraphicsFormat.R8_SInt || cookieFormat == GraphicsFormat.R16_UNorm)
					{
						return LightCookieManager.LightCookieShaderFormat.Red;
					}
				}
				else if (cookieFormat == GraphicsFormat.R16_SNorm || cookieFormat == GraphicsFormat.R16_UInt)
				{
					return LightCookieManager.LightCookieShaderFormat.Red;
				}
			}
			else if (cookieFormat <= GraphicsFormat.R16_SFloat)
			{
				if (cookieFormat <= GraphicsFormat.R32_UInt)
				{
					if (cookieFormat == GraphicsFormat.R16_SInt || cookieFormat == GraphicsFormat.R32_UInt)
					{
						return LightCookieManager.LightCookieShaderFormat.Red;
					}
				}
				else if (cookieFormat == GraphicsFormat.R32_SInt || cookieFormat == GraphicsFormat.R16_SFloat)
				{
					return LightCookieManager.LightCookieShaderFormat.Red;
				}
			}
			else if (cookieFormat <= (GraphicsFormat)55)
			{
				if (cookieFormat == GraphicsFormat.R32_SFloat)
				{
					return LightCookieManager.LightCookieShaderFormat.Red;
				}
				if (cookieFormat - (GraphicsFormat)54 <= 1)
				{
					return LightCookieManager.LightCookieShaderFormat.Alpha;
				}
			}
			else if (cookieFormat - GraphicsFormat.R_BC4_UNorm <= 1 || cookieFormat - GraphicsFormat.R_EAC_UNorm <= 1)
			{
				return LightCookieManager.LightCookieShaderFormat.Red;
			}
			return LightCookieManager.LightCookieShaderFormat.RGB;
		}

		private void GetLightUVScaleOffset(ref UniversalAdditionalLightData additionalLightData, ref Matrix4x4 uvTransform)
		{
			Vector2 vector = Vector2.one / additionalLightData.lightCookieSize;
			Vector2 lightCookieOffset = additionalLightData.lightCookieOffset;
			if (Mathf.Abs(vector.x) < half.MinValue)
			{
				vector.x = Mathf.Sign(vector.x) * half.MinValue;
			}
			if (Mathf.Abs(vector.y) < half.MinValue)
			{
				vector.y = Mathf.Sign(vector.y) * half.MinValue;
			}
			uvTransform = Matrix4x4.Scale(new Vector3(vector.x, vector.y, 1f));
			uvTransform.SetColumn(3, new Vector4(-lightCookieOffset.x * vector.x, -lightCookieOffset.y * vector.y, 0f, 1f));
		}

		private bool SetupAdditionalLights(CommandBuffer cmd, UniversalLightData lightData)
		{
			int size = Math.Min(this.m_Settings.maxAdditionalLights, lightData.visibleLights.Length);
			this.m_WorkMem.Resize(size);
			int num = this.FilterAndValidateAdditionalLights(lightData, this.m_WorkMem.lightMappings);
			if (num <= 0)
			{
				return false;
			}
			if (!this.isInitialized())
			{
				this.InitAdditionalLights(num);
			}
			LightCookieManager.WorkSlice<LightCookieManager.LightCookieMapping> workSlice = new LightCookieManager.WorkSlice<LightCookieManager.LightCookieMapping>(this.m_WorkMem.lightMappings, num);
			int srcLen = this.UpdateAdditionalLightsAtlas(cmd, ref workSlice, this.m_WorkMem.uvRects);
			LightCookieManager.WorkSlice<Vector4> workSlice2 = new LightCookieManager.WorkSlice<Vector4>(this.m_WorkMem.uvRects, srcLen);
			this.UploadAdditionalLights(cmd, lightData, ref workSlice, ref workSlice2);
			return workSlice2.length > 0;
		}

		private int FilterAndValidateAdditionalLights(UniversalLightData lightData, LightCookieManager.LightCookieMapping[] validLightMappings)
		{
			int mainLightIndex = lightData.mainLightIndex;
			int num = 0;
			int num2 = 0;
			int length = lightData.visibleLights.Length;
			for (int i = 0; i < length; i++)
			{
				if (i == mainLightIndex)
				{
					num--;
				}
				else
				{
					ref VisibleLight ptr = ref lightData.visibleLights.UnsafeElementAtMutable(i);
					Light light = ptr.light;
					if (!(light.cookie == null))
					{
						LightType lightType = ptr.lightType;
						if (lightType != LightType.Spot && lightType != LightType.Point && lightType != LightType.Directional)
						{
							Debug.LogWarning(string.Concat(new string[]
							{
								"Additional ",
								lightType.ToString(),
								" light called '",
								light.name,
								"' has a light cookie which will not be visible."
							}), light);
						}
						else
						{
							LightCookieManager.LightCookieMapping lightCookieMapping;
							lightCookieMapping.visibleLightIndex = (ushort)i;
							lightCookieMapping.lightBufferIndex = (ushort)(i + num);
							lightCookieMapping.light = light;
							if ((int)lightCookieMapping.lightBufferIndex >= validLightMappings.Length || num2 + 1 >= validLightMappings.Length)
							{
								if (length > this.m_Settings.maxAdditionalLights && Time.frameCount - this.m_PrevWarnFrame > 3600)
								{
									this.m_PrevWarnFrame = Time.frameCount;
									Debug.LogWarning(string.Concat(new string[]
									{
										"Max light cookies (",
										validLightMappings.Length.ToString(),
										") reached. Some visible lights (",
										(length - i - 1).ToString(),
										") might skip light cookie rendering."
									}));
									break;
								}
								break;
							}
							else
							{
								validLightMappings[num2++] = lightCookieMapping;
							}
						}
					}
				}
			}
			return num2;
		}

		private int UpdateAdditionalLightsAtlas(CommandBuffer cmd, ref LightCookieManager.WorkSlice<LightCookieManager.LightCookieMapping> validLightMappings, Vector4[] textureAtlasUVRects)
		{
			validLightMappings.Sort(LightCookieManager.LightCookieMapping.s_CompareByCookieSize);
			uint num = this.ComputeCookieRequestPixelCount(ref validLightMappings);
			Vector2Int referenceSize = this.m_AdditionalLightsCookieAtlas.AtlasTexture.referenceSize;
			float requestAtlasRatio = num / (float)(referenceSize.x * referenceSize.y);
			int num2 = this.ApproximateCookieSizeDivisor(requestAtlasRatio);
			if (num2 < this.m_CookieSizeDivisor && num < this.m_PrevCookieRequestPixelCount)
			{
				this.m_AdditionalLightsCookieAtlas.ResetAllocator();
				this.m_CookieSizeDivisor = num2;
			}
			int i = 0;
			while (i <= 0)
			{
				i = this.FetchUVRects(cmd, ref validLightMappings, textureAtlasUVRects, this.m_CookieSizeDivisor);
				if (i <= 0)
				{
					this.m_AdditionalLightsCookieAtlas.ResetAllocator();
					this.m_CookieSizeDivisor = Mathf.Max(this.m_CookieSizeDivisor + 1, num2);
					this.m_PrevCookieRequestPixelCount = num;
				}
			}
			return i;
		}

		private int FetchUVRects(CommandBuffer cmd, ref LightCookieManager.WorkSlice<LightCookieManager.LightCookieMapping> validLightMappings, Vector4[] textureAtlasUVRects, int cookieSizeDivisor)
		{
			int result = 0;
			int i = 0;
			while (i < validLightMappings.length)
			{
				Texture cookie = validLightMappings[i].light.cookie;
				Vector4 vector = Vector4.zero;
				if (cookie.dimension == TextureDimension.Cube)
				{
					vector = this.FetchCube(cmd, cookie, cookieSizeDivisor);
				}
				else
				{
					vector = this.Fetch2D(cmd, cookie, cookieSizeDivisor);
				}
				if (!(vector != Vector4.zero))
				{
					if (cookieSizeDivisor > 16)
					{
						Debug.LogWarning("Light cookies atlas is extremely full! Some of the light cookies were discarded. Increase light cookie atlas space or reduce the amount of unique light cookies.");
						return result;
					}
					return 0;
				}
				else
				{
					if (!SystemInfo.graphicsUVStartsAtTop)
					{
						vector.w = 1f - vector.w - vector.y;
					}
					textureAtlasUVRects[result++] = vector;
					i++;
				}
			}
			return result;
		}

		private uint ComputeCookieRequestPixelCount(ref LightCookieManager.WorkSlice<LightCookieManager.LightCookieMapping> validLightMappings)
		{
			uint num = 0U;
			int num2 = 0;
			for (int i = 0; i < validLightMappings.length; i++)
			{
				Texture cookie = validLightMappings[i].light.cookie;
				int instanceID = cookie.GetInstanceID();
				if (instanceID != num2)
				{
					num2 = instanceID;
					int num3 = cookie.width * cookie.height;
					num += (uint)num3;
				}
			}
			return num;
		}

		private int ApproximateCookieSizeDivisor(float requestAtlasRatio)
		{
			return (int)Mathf.Max(Mathf.Ceil(Mathf.Sqrt(requestAtlasRatio)), 1f);
		}

		private Vector4 Fetch2D(CommandBuffer cmd, Texture cookie, int cookieSizeDivisor = 1)
		{
			Vector4 zero = Vector4.zero;
			int num = Mathf.Max(cookie.width / cookieSizeDivisor, 4);
			int num2 = Mathf.Max(cookie.height / cookieSizeDivisor, 4);
			Vector2 vector = new Vector2((float)num, (float)num2);
			if (this.m_AdditionalLightsCookieAtlas.IsCached(out zero, cookie))
			{
				this.m_AdditionalLightsCookieAtlas.UpdateTexture(cmd, cookie, ref zero, true, true);
			}
			else
			{
				this.m_AdditionalLightsCookieAtlas.AllocateTexture(cmd, ref zero, cookie, num, num2, -1);
			}
			this.AdjustUVRect(ref zero, cookie, ref vector);
			return zero;
		}

		private Vector4 FetchCube(CommandBuffer cmd, Texture cookie, int cookieSizeDivisor = 1)
		{
			Vector4 zero = Vector4.zero;
			int num = Mathf.Max(this.ComputeOctahedralCookieSize(cookie) / cookieSizeDivisor, 4);
			if (this.m_AdditionalLightsCookieAtlas.IsCached(out zero, cookie))
			{
				this.m_AdditionalLightsCookieAtlas.UpdateTexture(cmd, cookie, ref zero, true, true);
			}
			else
			{
				this.m_AdditionalLightsCookieAtlas.AllocateTexture(cmd, ref zero, cookie, num, num, -1);
			}
			Vector2 vector = Vector2.one * (float)num;
			this.AdjustUVRect(ref zero, cookie, ref vector);
			return zero;
		}

		private int ComputeOctahedralCookieSize(Texture cookie)
		{
			int num = Math.Max(cookie.width, cookie.height);
			LightCookieManager.Settings.AtlasSettings atlas = this.m_Settings.atlas;
			if (atlas.isPow2)
			{
				num *= Mathf.NextPowerOfTwo((int)this.m_Settings.cubeOctahedralSizeScale);
			}
			else
			{
				num = (int)((float)num * this.m_Settings.cubeOctahedralSizeScale + 0.5f);
			}
			return num;
		}

		private void AdjustUVRect(ref Vector4 uvScaleOffset, Texture cookie, ref Vector2 cookieSize)
		{
			if (uvScaleOffset != Vector4.zero)
			{
				this.ShrinkUVRect(ref uvScaleOffset, 0.5f, ref cookieSize);
			}
		}

		private void ShrinkUVRect(ref Vector4 uvScaleOffset, float amountPixels, ref Vector2 cookieSize)
		{
			Vector2 vector = Vector2.one * amountPixels / cookieSize;
			Vector2 vector2 = (cookieSize - Vector2.one * (amountPixels * 2f)) / cookieSize;
			uvScaleOffset.z += uvScaleOffset.x * vector.x;
			uvScaleOffset.w += uvScaleOffset.y * vector.y;
			uvScaleOffset.x *= vector2.x;
			uvScaleOffset.y *= vector2.y;
		}

		private void UploadAdditionalLights(CommandBuffer cmd, UniversalLightData lightData, ref LightCookieManager.WorkSlice<LightCookieManager.LightCookieMapping> validLightMappings, ref LightCookieManager.WorkSlice<Vector4> validUvRects)
		{
			cmd.SetGlobalTexture(LightCookieManager.ShaderProperty.additionalLightsCookieAtlasTexture, this.m_AdditionalLightsCookieAtlas.AtlasTexture);
			cmd.SetGlobalFloat(LightCookieManager.ShaderProperty.additionalLightsCookieAtlasTextureFormat, (float)this.GetLightCookieShaderFormat(this.m_AdditionalLightsCookieAtlas.AtlasTexture.rt.graphicsFormat));
			if (this.m_VisibleLightIndexToShaderDataIndex.Length < lightData.visibleLights.Length)
			{
				this.m_VisibleLightIndexToShaderDataIndex = new int[lightData.visibleLights.Length];
			}
			int num = Math.Min(this.m_VisibleLightIndexToShaderDataIndex.Length, lightData.visibleLights.Length);
			for (int i = 0; i < num; i++)
			{
				this.m_VisibleLightIndexToShaderDataIndex[i] = -1;
			}
			this.m_AdditionalLightsCookieShaderData.Resize(this.m_Settings.maxAdditionalLights);
			Matrix4x4[] worldToLights = this.m_AdditionalLightsCookieShaderData.worldToLights;
			ShaderBitArray cookieEnableBits = this.m_AdditionalLightsCookieShaderData.cookieEnableBits;
			Vector4[] atlasUVRects = this.m_AdditionalLightsCookieShaderData.atlasUVRects;
			float[] lightTypes = this.m_AdditionalLightsCookieShaderData.lightTypes;
			Array.Clear(atlasUVRects, 0, atlasUVRects.Length);
			cookieEnableBits.Clear();
			for (int j = 0; j < validUvRects.length; j++)
			{
				int visibleLightIndex = (int)validLightMappings[j].visibleLightIndex;
				int lightBufferIndex = (int)validLightMappings[j].lightBufferIndex;
				this.m_VisibleLightIndexToShaderDataIndex[visibleLightIndex] = lightBufferIndex;
				ref VisibleLight ptr = ref lightData.visibleLights.UnsafeElementAtMutable(visibleLightIndex);
				lightTypes[lightBufferIndex] = (float)ptr.lightType;
				worldToLights[lightBufferIndex] = ptr.localToWorldMatrix.inverse;
				atlasUVRects[lightBufferIndex] = validUvRects[j];
				cookieEnableBits[lightBufferIndex] = true;
				if (ptr.lightType == LightType.Spot)
				{
					float spotAngle = ptr.spotAngle;
					float range = ptr.range;
					Matrix4x4 lhs = Matrix4x4.Perspective(spotAngle, 1f, 0.001f, range);
					lhs.SetColumn(2, lhs.GetColumn(2) * -1f);
					worldToLights[lightBufferIndex] = lhs * worldToLights[lightBufferIndex];
				}
				else if (ptr.lightType == LightType.Directional)
				{
					UniversalAdditionalLightData universalAdditionalLightData;
					ptr.light.TryGetComponent<UniversalAdditionalLightData>(out universalAdditionalLightData);
					Matrix4x4 identity = Matrix4x4.identity;
					this.GetLightUVScaleOffset(ref universalAdditionalLightData, ref identity);
					Matrix4x4 matrix4x = LightCookieManager.s_DirLightProj * identity * ptr.localToWorldMatrix.inverse;
					worldToLights[lightBufferIndex] = matrix4x;
				}
			}
			this.m_AdditionalLightsCookieShaderData.Upload(cmd);
		}

		private static readonly Matrix4x4 s_DirLightProj = Matrix4x4.Ortho(-0.5f, 0.5f, -0.5f, 0.5f, -0.5f, 0.5f);

		private Texture2DAtlas m_AdditionalLightsCookieAtlas;

		private LightCookieManager.LightCookieShaderData m_AdditionalLightsCookieShaderData;

		private readonly LightCookieManager.Settings m_Settings;

		private LightCookieManager.WorkMemory m_WorkMem;

		private int[] m_VisibleLightIndexToShaderDataIndex;

		private const int k_MaxCookieSizeDivisor = 16;

		private int m_CookieSizeDivisor = 1;

		private uint m_PrevCookieRequestPixelCount = uint.MaxValue;

		private int m_PrevWarnFrame = -1;

		private static class ShaderProperty
		{
			public static readonly int mainLightTexture = Shader.PropertyToID("_MainLightCookieTexture");

			public static readonly int mainLightWorldToLight = Shader.PropertyToID("_MainLightWorldToLight");

			public static readonly int mainLightCookieTextureFormat = Shader.PropertyToID("_MainLightCookieTextureFormat");

			public static readonly int additionalLightsCookieAtlasTexture = Shader.PropertyToID("_AdditionalLightsCookieAtlasTexture");

			public static readonly int additionalLightsCookieAtlasTextureFormat = Shader.PropertyToID("_AdditionalLightsCookieAtlasTextureFormat");

			public static readonly int additionalLightsCookieEnableBits = Shader.PropertyToID("_AdditionalLightsCookieEnableBits");

			public static readonly int additionalLightsCookieAtlasUVRectBuffer = Shader.PropertyToID("_AdditionalLightsCookieAtlasUVRectBuffer");

			public static readonly int additionalLightsCookieAtlasUVRects = Shader.PropertyToID("_AdditionalLightsCookieAtlasUVRects");

			public static readonly int additionalLightsWorldToLightBuffer = Shader.PropertyToID("_AdditionalLightsWorldToLightBuffer");

			public static readonly int additionalLightsLightTypeBuffer = Shader.PropertyToID("_AdditionalLightsLightTypeBuffer");

			public static readonly int additionalLightsWorldToLights = Shader.PropertyToID("_AdditionalLightsWorldToLights");

			public static readonly int additionalLightsLightTypes = Shader.PropertyToID("_AdditionalLightsLightTypes");
		}

		private enum LightCookieShaderFormat
		{
			None = -1,
			RGB,
			Alpha,
			Red
		}

		public struct Settings
		{
			public static LightCookieManager.Settings Create()
			{
				LightCookieManager.Settings result;
				result.atlas.resolution = new Vector2Int(1024, 1024);
				result.atlas.format = GraphicsFormat.R8G8B8A8_SRGB;
				result.maxAdditionalLights = UniversalRenderPipeline.maxVisibleAdditionalLights;
				result.cubeOctahedralSizeScale = 2.5f;
				result.useStructuredBuffer = RenderingUtils.useStructuredBuffer;
				return result;
			}

			public LightCookieManager.Settings.AtlasSettings atlas;

			public int maxAdditionalLights;

			public float cubeOctahedralSizeScale;

			public bool useStructuredBuffer;

			public struct AtlasSettings
			{
				public bool isPow2
				{
					get
					{
						return Mathf.IsPowerOfTwo(this.resolution.x) && Mathf.IsPowerOfTwo(this.resolution.y);
					}
				}

				public bool isSquare
				{
					get
					{
						return this.resolution.x == this.resolution.y;
					}
				}

				public Vector2Int resolution;

				public GraphicsFormat format;
			}
		}

		private struct LightCookieMapping
		{
			public ushort visibleLightIndex;

			public ushort lightBufferIndex;

			public Light light;

			public static Func<LightCookieManager.LightCookieMapping, LightCookieManager.LightCookieMapping, int> s_CompareByCookieSize = delegate(LightCookieManager.LightCookieMapping a, LightCookieManager.LightCookieMapping b)
			{
				Texture cookie = a.light.cookie;
				Texture cookie2 = b.light.cookie;
				int num = cookie.width * cookie.height;
				int num2 = cookie2.width * cookie2.height - num;
				if (num2 == 0)
				{
					int instanceID = cookie.GetInstanceID();
					int instanceID2 = cookie2.GetInstanceID();
					return instanceID - instanceID2;
				}
				return num2;
			};

			public static Func<LightCookieManager.LightCookieMapping, LightCookieManager.LightCookieMapping, int> s_CompareByBufferIndex = (LightCookieManager.LightCookieMapping a, LightCookieManager.LightCookieMapping b) => (int)(a.lightBufferIndex - b.lightBufferIndex);
		}

		private readonly struct WorkSlice<T>
		{
			public WorkSlice(T[] src, int srcLen = -1)
			{
				this = new LightCookieManager.WorkSlice<T>(src, 0, srcLen);
			}

			public WorkSlice(T[] src, int srcStart, int srcLen = -1)
			{
				this.m_Data = src;
				this.m_Start = srcStart;
				this.m_Length = ((srcLen < 0) ? src.Length : Math.Min(srcLen, src.Length));
			}

			public T this[int index]
			{
				get
				{
					return this.m_Data[this.m_Start + index];
				}
				set
				{
					this.m_Data[this.m_Start + index] = value;
				}
			}

			public int length
			{
				get
				{
					return this.m_Length;
				}
			}

			public int capacity
			{
				get
				{
					return this.m_Data.Length;
				}
			}

			public void Sort(Func<T, T, int> compare)
			{
				if (this.m_Length > 1)
				{
					Sorting.QuickSort<T>(this.m_Data, this.m_Start, this.m_Start + this.m_Length - 1, compare);
				}
			}

			private readonly T[] m_Data;

			private readonly int m_Start;

			private readonly int m_Length;
		}

		private class WorkMemory
		{
			public void Resize(int size)
			{
				int num = size;
				LightCookieManager.LightCookieMapping[] array = this.lightMappings;
				int? num2 = (array != null) ? new int?(array.Length) : null;
				if (num <= num2.GetValueOrDefault() & num2 != null)
				{
					return;
				}
				size = Math.Max(size, (size + 15) / 16 * 16);
				this.lightMappings = new LightCookieManager.LightCookieMapping[size];
				this.uvRects = new Vector4[size];
			}

			public LightCookieManager.LightCookieMapping[] lightMappings;

			public Vector4[] uvRects;
		}

		private class LightCookieShaderData : IDisposable
		{
			public Matrix4x4[] worldToLights
			{
				get
				{
					return this.m_WorldToLightCpuData;
				}
			}

			public ShaderBitArray cookieEnableBits
			{
				get
				{
					return this.m_CookieEnableBitsCpuData;
				}
			}

			public Vector4[] atlasUVRects
			{
				get
				{
					return this.m_AtlasUVRectCpuData;
				}
			}

			public float[] lightTypes
			{
				get
				{
					return this.m_LightTypeCpuData;
				}
			}

			public bool isUploaded { get; set; }

			public LightCookieShaderData(int size, bool useStructuredBuffer)
			{
				this.m_UseStructuredBuffer = useStructuredBuffer;
				this.Resize(size);
			}

			public void Dispose()
			{
				if (this.m_UseStructuredBuffer)
				{
					ComputeBuffer worldToLightBuffer = this.m_WorldToLightBuffer;
					if (worldToLightBuffer != null)
					{
						worldToLightBuffer.Dispose();
					}
					ComputeBuffer atlasUVRectBuffer = this.m_AtlasUVRectBuffer;
					if (atlasUVRectBuffer != null)
					{
						atlasUVRectBuffer.Dispose();
					}
					ComputeBuffer lightTypeBuffer = this.m_LightTypeBuffer;
					if (lightTypeBuffer == null)
					{
						return;
					}
					lightTypeBuffer.Dispose();
				}
			}

			public void Resize(int size)
			{
				if (size <= this.m_Size)
				{
					return;
				}
				if (this.m_Size > 0)
				{
					this.Dispose();
				}
				this.m_WorldToLightCpuData = new Matrix4x4[size];
				this.m_AtlasUVRectCpuData = new Vector4[size];
				this.m_LightTypeCpuData = new float[size];
				this.m_CookieEnableBitsCpuData.Resize(size);
				if (this.m_UseStructuredBuffer)
				{
					this.m_WorldToLightBuffer = new ComputeBuffer(size, Marshal.SizeOf<Matrix4x4>());
					this.m_AtlasUVRectBuffer = new ComputeBuffer(size, Marshal.SizeOf<Vector4>());
					this.m_LightTypeBuffer = new ComputeBuffer(size, Marshal.SizeOf<float>());
				}
				this.m_Size = size;
			}

			public void Upload(CommandBuffer cmd)
			{
				if (this.m_UseStructuredBuffer)
				{
					this.m_WorldToLightBuffer.SetData(this.m_WorldToLightCpuData);
					this.m_AtlasUVRectBuffer.SetData(this.m_AtlasUVRectCpuData);
					this.m_LightTypeBuffer.SetData(this.m_LightTypeCpuData);
					cmd.SetGlobalBuffer(LightCookieManager.ShaderProperty.additionalLightsWorldToLightBuffer, this.m_WorldToLightBuffer);
					cmd.SetGlobalBuffer(LightCookieManager.ShaderProperty.additionalLightsCookieAtlasUVRectBuffer, this.m_AtlasUVRectBuffer);
					cmd.SetGlobalBuffer(LightCookieManager.ShaderProperty.additionalLightsLightTypeBuffer, this.m_LightTypeBuffer);
				}
				else
				{
					cmd.SetGlobalMatrixArray(LightCookieManager.ShaderProperty.additionalLightsWorldToLights, this.m_WorldToLightCpuData);
					cmd.SetGlobalVectorArray(LightCookieManager.ShaderProperty.additionalLightsCookieAtlasUVRects, this.m_AtlasUVRectCpuData);
					cmd.SetGlobalFloatArray(LightCookieManager.ShaderProperty.additionalLightsLightTypes, this.m_LightTypeCpuData);
				}
				cmd.SetGlobalFloatArray(LightCookieManager.ShaderProperty.additionalLightsCookieEnableBits, this.m_CookieEnableBitsCpuData.data);
				this.isUploaded = true;
			}

			public void Clear(CommandBuffer cmd)
			{
				if (this.isUploaded)
				{
					this.m_CookieEnableBitsCpuData.Clear();
					cmd.SetGlobalFloatArray(LightCookieManager.ShaderProperty.additionalLightsCookieEnableBits, this.m_CookieEnableBitsCpuData.data);
					this.isUploaded = false;
				}
			}

			private int m_Size;

			private bool m_UseStructuredBuffer;

			private Matrix4x4[] m_WorldToLightCpuData;

			private Vector4[] m_AtlasUVRectCpuData;

			private float[] m_LightTypeCpuData;

			private ShaderBitArray m_CookieEnableBitsCpuData;

			private ComputeBuffer m_WorldToLightBuffer;

			private ComputeBuffer m_AtlasUVRectBuffer;

			private ComputeBuffer m_LightTypeBuffer;
		}
	}
}
