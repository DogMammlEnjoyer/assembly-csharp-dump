using System;
using UnityEngine.Experimental.Rendering;

namespace UnityEngine.Rendering.Universal
{
	internal static class Light2DLookupTexture
	{
		public static RTHandle GetLightLookupTexture_Rendergraph()
		{
			if (Light2DLookupTexture.s_PointLightLookupTexture == null || Light2DLookupTexture.m_LightLookupRTHandle == null)
			{
				Texture lightLookupTexture = Light2DLookupTexture.GetLightLookupTexture();
				RTHandle lightLookupRTHandle = Light2DLookupTexture.m_LightLookupRTHandle;
				if (lightLookupRTHandle != null)
				{
					lightLookupRTHandle.Release();
				}
				Light2DLookupTexture.m_LightLookupRTHandle = RTHandles.Alloc(lightLookupTexture);
			}
			return Light2DLookupTexture.m_LightLookupRTHandle;
		}

		public static RTHandle GetFallOffLookupTexture_Rendergraph()
		{
			if (Light2DLookupTexture.s_FalloffLookupTexture == null || Light2DLookupTexture.m_FalloffRTHandle == null)
			{
				Texture falloffLookupTexture = Light2DLookupTexture.GetFalloffLookupTexture();
				RTHandle falloffRTHandle = Light2DLookupTexture.m_FalloffRTHandle;
				if (falloffRTHandle != null)
				{
					falloffRTHandle.Release();
				}
				Light2DLookupTexture.m_FalloffRTHandle = RTHandles.Alloc(falloffLookupTexture);
			}
			return Light2DLookupTexture.m_FalloffRTHandle;
		}

		public static void Release()
		{
			RTHandle falloffRTHandle = Light2DLookupTexture.m_FalloffRTHandle;
			if (falloffRTHandle != null)
			{
				falloffRTHandle.Release();
			}
			RTHandle lightLookupRTHandle = Light2DLookupTexture.m_LightLookupRTHandle;
			if (lightLookupRTHandle != null)
			{
				lightLookupRTHandle.Release();
			}
			Light2DLookupTexture.m_FalloffRTHandle = null;
			Light2DLookupTexture.m_LightLookupRTHandle = null;
		}

		public static Texture GetLightLookupTexture()
		{
			if (Light2DLookupTexture.s_PointLightLookupTexture == null)
			{
				Light2DLookupTexture.s_PointLightLookupTexture = Light2DLookupTexture.CreatePointLightLookupTexture();
			}
			return Light2DLookupTexture.s_PointLightLookupTexture;
		}

		public static Texture GetFalloffLookupTexture()
		{
			if (Light2DLookupTexture.s_FalloffLookupTexture == null)
			{
				Light2DLookupTexture.s_FalloffLookupTexture = Light2DLookupTexture.CreateFalloffLookupTexture();
			}
			return Light2DLookupTexture.s_FalloffLookupTexture;
		}

		private static Texture2D CreatePointLightLookupTexture()
		{
			GraphicsFormat format = GraphicsFormat.R8G8B8A8_UNorm;
			if (SystemInfo.IsFormatSupported(GraphicsFormat.R16G16B16A16_SFloat, GraphicsFormatUsage.SetPixels))
			{
				format = GraphicsFormat.R16G16B16A16_SFloat;
			}
			else if (SystemInfo.IsFormatSupported(GraphicsFormat.R32G32B32A32_SFloat, GraphicsFormatUsage.SetPixels))
			{
				format = GraphicsFormat.R32G32B32A32_SFloat;
			}
			Texture2D texture2D = new Texture2D(256, 256, format, TextureCreationFlags.None);
			texture2D.name = Light2DLookupTexture.k_LightLookupProperty;
			texture2D.filterMode = FilterMode.Bilinear;
			texture2D.wrapMode = TextureWrapMode.Clamp;
			Vector2 vector = new Vector2(128f, 128f);
			for (int i = 0; i < 256; i++)
			{
				for (int j = 0; j < 256; j++)
				{
					Vector2 vector2 = new Vector2((float)j, (float)i);
					float num = Vector2.Distance(vector2, vector);
					Vector2 vector3 = vector2 - vector;
					Vector2 vector4 = vector - vector2;
					vector4.Normalize();
					float r;
					if (j == 255 || i == 255)
					{
						r = 0f;
					}
					else
					{
						r = Mathf.Clamp(1f - 2f * num / 256f, 0f, 1f);
					}
					float num2 = Mathf.Acos(Vector2.Dot(Vector2.down, vector3.normalized)) / 3.1415927f;
					float g = Mathf.Clamp(1f - num2, 0f, 1f);
					float x = vector4.x;
					float y = vector4.y;
					Color color = new Color(r, g, x, y);
					texture2D.SetPixel(j, i, color);
				}
			}
			texture2D.Apply();
			return texture2D;
		}

		private static Texture2D CreateFalloffLookupTexture()
		{
			Texture2D texture2D = new Texture2D(2048, 128, GraphicsFormat.R8G8B8A8_SRGB, TextureCreationFlags.None);
			texture2D.name = Light2DLookupTexture.k_FalloffLookupProperty;
			texture2D.filterMode = FilterMode.Bilinear;
			texture2D.wrapMode = TextureWrapMode.Clamp;
			for (int i = 0; i < 192; i++)
			{
				float num = (float)(i + 32) / 256f;
				float p = Mathf.Log(-num + 1f) / Mathf.Log(num);
				for (int j = 0; j < 2048; j++)
				{
					float r = Mathf.Pow((float)j / 2048f, p);
					Color color = new Color(r, 0f, 0f, 1f);
					if (i >= 32 && i < 160)
					{
						texture2D.SetPixel(j, i - 32, color);
					}
				}
			}
			texture2D.Apply();
			return texture2D;
		}

		internal static readonly string k_LightLookupProperty = "_LightLookup";

		internal static readonly string k_FalloffLookupProperty = "_FalloffLookup";

		internal static readonly int k_LightLookupID = Shader.PropertyToID(Light2DLookupTexture.k_LightLookupProperty);

		internal static readonly int k_FalloffLookupID = Shader.PropertyToID(Light2DLookupTexture.k_FalloffLookupProperty);

		private static Texture2D s_PointLightLookupTexture;

		private static Texture2D s_FalloffLookupTexture;

		private static RTHandle m_LightLookupRTHandle = null;

		private static RTHandle m_FalloffRTHandle = null;
	}
}
