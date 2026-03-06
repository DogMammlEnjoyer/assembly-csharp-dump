using System;
using System.Collections.Generic;
using System.Diagnostics;
using DigitalOpus.MB.Core;
using UnityEngine;

public class MB_TextureCombinerRenderTexture
{
	public Texture2D DoRenderAtlas(GameObject gameObject, int width, int height, int padding, Rect[] rss, List<MB_TexSet> textureSetss, int indexOfTexSetToRenders, ShaderTextureProperty texPropertyname, MB3_TextureCombinerNonTextureProperties resultMaterialTextureBlender, bool isNormalMap, bool fixOutOfBoundsUVs, bool considerNonTextureProperties, MB3_TextureCombiner texCombiner, MB2_LogLevel LOG_LEV)
	{
		this.LOG_LEVEL = LOG_LEV;
		this.textureSets = textureSetss;
		this.indexOfTexSetToRender = indexOfTexSetToRenders;
		this._texPropertyName = texPropertyname;
		this._padding = padding;
		this._isNormalMap = isNormalMap;
		this._fixOutOfBoundsUVs = fixOutOfBoundsUVs;
		this._resultMaterialTextureBlender = resultMaterialTextureBlender;
		this.rs = rss;
		Shader shader;
		if (this._isNormalMap)
		{
			if (MBVersion.IsSwizzledNormalMapPlatform())
			{
				shader = Shader.Find("MeshBaker/NormalMapShaderSwizzle");
			}
			else
			{
				shader = Shader.Find("MeshBaker/AlbedoShader");
			}
		}
		else
		{
			shader = Shader.Find("MeshBaker/AlbedoShader");
		}
		if (shader == null)
		{
			Debug.LogError("Could not find shader for RenderTexture. Try reimporting mesh baker");
			return null;
		}
		this.mat = new Material(shader);
		RenderTextureReadWrite readWrite;
		if (texPropertyname.isGammaCorrected)
		{
			readWrite = RenderTextureReadWrite.sRGB;
		}
		else
		{
			readWrite = RenderTextureReadWrite.Linear;
		}
		this._destinationTexture = new RenderTexture(width, height, 24, RenderTextureFormat.ARGB32, readWrite);
		this._destinationTexture.filterMode = FilterMode.Point;
		this.myCamera = gameObject.GetComponent<Camera>();
		this.myCamera.orthographic = true;
		this.myCamera.orthographicSize = (float)(height >> 1);
		this.myCamera.aspect = (float)width / (float)height;
		this.myCamera.targetTexture = this._destinationTexture;
		this.myCamera.clearFlags = CameraClearFlags.Color;
		Transform component = this.myCamera.GetComponent<Transform>();
		component.localPosition = new Vector3((float)width / 2f, (float)height / 2f, 3f);
		component.localRotation = Quaternion.Euler(0f, 180f, 180f);
		this._doRenderAtlas = true;
		if (this.LOG_LEVEL >= MB2_LogLevel.debug)
		{
			Debug.Log(string.Format("Begin Camera.Render destTex w={0} h={1} camPos={2} camSize={3} camAspect={4}", new object[]
			{
				width,
				height,
				component.localPosition,
				this.myCamera.orthographicSize,
				this.myCamera.aspect.ToString("f5")
			}));
		}
		this.myCamera.Render();
		this._doRenderAtlas = false;
		MB_Utility.Destroy(this.mat);
		MB_Utility.Destroy(this._destinationTexture);
		if (this.LOG_LEVEL >= MB2_LogLevel.debug)
		{
			Debug.Log("Finished Camera.Render ");
		}
		Texture2D texture2D = this.targTex;
		this.targTex = null;
		if (texture2D == null)
		{
			Debug.LogError(" Generated atlas was null. This can happen when using HDRP. Try using the Texture Packer 'Mesh Baker Texture Packer Fast V2' ");
		}
		return texture2D;
	}

	public void OnRenderObject()
	{
		if (this._doRenderAtlas)
		{
			Stopwatch stopwatch = new Stopwatch();
			stopwatch.Start();
			bool yIsFlipped = MB_TextureCombinerRenderTexture.YisFlipped(this.LOG_LEVEL);
			for (int i = 0; i < this.rs.Length; i++)
			{
				MeshBakerMaterialTexture meshBakerMaterialTexture = this.textureSets[i].ts[this.indexOfTexSetToRender];
				Texture2D texture2D = meshBakerMaterialTexture.GetTexture2D();
				if (this.LOG_LEVEL >= MB2_LogLevel.trace && texture2D != null)
				{
					string[] array = new string[14];
					array[0] = "Added ";
					int num = 1;
					Texture2D texture2D2 = texture2D;
					array[num] = ((texture2D2 != null) ? texture2D2.ToString() : null);
					array[2] = " to atlas w=";
					array[3] = texture2D.width.ToString();
					array[4] = " h=";
					array[5] = texture2D.height.ToString();
					array[6] = " offset=";
					array[7] = meshBakerMaterialTexture.matTilingRect.min.ToString();
					array[8] = " scale=";
					array[9] = meshBakerMaterialTexture.matTilingRect.size.ToString();
					array[10] = " rect=";
					int num2 = 11;
					Rect rect = this.rs[i];
					array[num2] = rect.ToString();
					array[12] = " padding=";
					array[13] = this._padding.ToString();
					Debug.Log(string.Concat(array));
				}
				this.CopyScaledAndTiledToAtlas(this.textureSets[i], meshBakerMaterialTexture, this.textureSets[i].obUVoffset, this.textureSets[i].obUVscale, this.rs[i], this._texPropertyName, this._resultMaterialTextureBlender, yIsFlipped);
			}
			stopwatch.Stop();
			stopwatch.Start();
			if (this.LOG_LEVEL >= MB2_LogLevel.debug)
			{
				Debug.Log("Total time for Graphics.DrawTexture calls " + stopwatch.ElapsedMilliseconds.ToString("f5"));
			}
			if (this.LOG_LEVEL >= MB2_LogLevel.debug)
			{
				Debug.Log("Copying RenderTexture to Texture2D. destW" + this._destinationTexture.width.ToString() + " destH" + this._destinationTexture.height.ToString());
			}
			Texture2D tempTexture = new Texture2D(this._destinationTexture.width, this._destinationTexture.height, TextureFormat.ARGB32, true, !this._texPropertyName.isGammaCorrected);
			MB_TextureCombinerRenderTexture.ConvertRenderTextureToTexture2D(this._destinationTexture, yIsFlipped, false, this.LOG_LEVEL, tempTexture);
			this.myCamera.targetTexture = null;
			this.targTex = tempTexture;
			if (this.LOG_LEVEL >= MB2_LogLevel.debug)
			{
				Debug.Log("Total time to copy RenderTexture to Texture2D " + stopwatch.ElapsedMilliseconds.ToString("f5"));
			}
		}
	}

	public static void ConvertRenderTextureToTexture2D(RenderTexture _destinationTexture, bool yIsFlipped, bool doLinearColorSpace, MB2_LogLevel LOG_LEVEL, Texture2D tempTexture)
	{
		RenderTexture active = RenderTexture.active;
		RenderTexture.active = _destinationTexture;
		int num = Mathf.CeilToInt((float)_destinationTexture.width / 512f);
		int num2 = Mathf.CeilToInt((float)_destinationTexture.height / 512f);
		if (num == 0 || num2 == 0)
		{
			if (LOG_LEVEL >= MB2_LogLevel.trace)
			{
				Debug.Log("Copying all in one shot");
			}
			tempTexture.ReadPixels(new Rect(0f, 0f, (float)_destinationTexture.width, (float)_destinationTexture.height), 0, 0, true);
		}
		else
		{
			if (LOG_LEVEL >= MB2_LogLevel.trace)
			{
				Debug.Log("yIsFlipped copying blocks");
			}
			if (!yIsFlipped)
			{
				for (int i = 0; i < num; i++)
				{
					for (int j = 0; j < num2; j++)
					{
						int num3 = i * 512;
						int num4 = j * 512;
						Rect source = new Rect((float)num3, (float)num4, 512f, 512f);
						tempTexture.ReadPixels(source, i * 512, j * 512, true);
					}
				}
			}
			else
			{
				for (int k = 0; k < num; k++)
				{
					for (int l = 0; l < num2; l++)
					{
						int num5 = k * 512;
						int num6 = _destinationTexture.height - 512 - l * 512;
						Rect source2 = new Rect((float)num5, (float)num6, 512f, 512f);
						tempTexture.ReadPixels(source2, k * 512, l * 512, true);
					}
				}
			}
		}
		RenderTexture.active = active;
		tempTexture.Apply();
		if (LOG_LEVEL >= MB2_LogLevel.trace && tempTexture.height <= 16 && tempTexture.width <= 16)
		{
			MB_TextureCombinerRenderTexture._printTexture(tempTexture);
		}
	}

	private Color32 ConvertNormalFormatFromUnity_ToStandard(Color32 c)
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

	public static bool YisFlipped(MB2_LogLevel LOG_LEVEL)
	{
		bool result = MBVersion.GraphicsUVStartsAtTop();
		if (LOG_LEVEL == MB2_LogLevel.debug)
		{
			string str = SystemInfo.graphicsDeviceVersion.ToLower();
			Debug.Log("Graphics device version is: " + str + " flipY:" + result.ToString());
		}
		return result;
	}

	private void CopyScaledAndTiledToAtlas(MB_TexSet texSet, MeshBakerMaterialTexture source, Vector2 obUVoffset, Vector2 obUVscale, Rect rec, ShaderTextureProperty texturePropertyName, MB3_TextureCombinerNonTextureProperties resultMatTexBlender, bool yIsFlipped)
	{
		Rect rect = rec;
		this.myCamera.backgroundColor = resultMatTexBlender.GetColorForTemporaryTexture(texSet.matsAndGOs.mats[0].mat, texturePropertyName);
		rect.y = 1f - (rect.y + rect.height);
		rect.x *= (float)this._destinationTexture.width;
		rect.y *= (float)this._destinationTexture.height;
		rect.width *= (float)this._destinationTexture.width;
		rect.height *= (float)this._destinationTexture.height;
		Rect rect2 = rect;
		rect2.x -= (float)this._padding;
		rect2.y -= (float)this._padding;
		rect2.width += (float)(this._padding * 2);
		rect2.height += (float)(this._padding * 2);
		Rect screenRect = default(Rect);
		Rect rect3 = texSet.ts[this.indexOfTexSetToRender].GetEncapsulatingSamplingRect().GetRect();
		Texture2D texture2D = source.GetTexture2D();
		TextureWrapMode wrapMode = texture2D.wrapMode;
		if (rect3.width == 1f && rect3.height == 1f && rect3.x == 0f && rect3.y == 0f)
		{
			texture2D.wrapMode = TextureWrapMode.Clamp;
		}
		else
		{
			texture2D.wrapMode = TextureWrapMode.Repeat;
		}
		if (this.LOG_LEVEL >= MB2_LogLevel.trace)
		{
			string[] array = new string[8];
			array[0] = "DrawTexture tex=";
			array[1] = texture2D.name;
			array[2] = " destRect=";
			int num = 3;
			Rect rect4 = rect;
			array[num] = rect4.ToString();
			array[4] = " srcRect=";
			int num2 = 5;
			rect4 = rect3;
			array[num2] = rect4.ToString();
			array[6] = " Mat=";
			int num3 = 7;
			Material material = this.mat;
			array[num3] = ((material != null) ? material.ToString() : null);
			Debug.Log(string.Concat(array));
		}
		Rect sourceRect = default(Rect);
		sourceRect.x = rect3.x;
		sourceRect.y = rect3.y + 1f - 1f / (float)texture2D.height;
		sourceRect.width = rect3.width;
		sourceRect.height = 1f / (float)texture2D.height;
		screenRect.x = rect.x;
		screenRect.y = rect2.y;
		screenRect.width = rect.width;
		screenRect.height = (float)this._padding;
		RenderTexture active = RenderTexture.active;
		RenderTexture.active = this._destinationTexture;
		Graphics.DrawTexture(screenRect, texture2D, sourceRect, 0, 0, 0, 0, this.mat);
		sourceRect.x = rect3.x;
		sourceRect.y = rect3.y;
		sourceRect.width = rect3.width;
		sourceRect.height = 1f / (float)texture2D.height;
		screenRect.x = rect.x;
		screenRect.y = rect.y + rect.height;
		screenRect.width = rect.width;
		screenRect.height = (float)this._padding;
		Graphics.DrawTexture(screenRect, texture2D, sourceRect, 0, 0, 0, 0, this.mat);
		sourceRect.x = rect3.x;
		sourceRect.y = rect3.y;
		sourceRect.width = 1f / (float)texture2D.width;
		sourceRect.height = rect3.height;
		screenRect.x = rect2.x;
		screenRect.y = rect.y;
		screenRect.width = (float)this._padding;
		screenRect.height = rect.height;
		Graphics.DrawTexture(screenRect, texture2D, sourceRect, 0, 0, 0, 0, this.mat);
		sourceRect.x = rect3.x + 1f - 1f / (float)texture2D.width;
		sourceRect.y = rect3.y;
		sourceRect.width = 1f / (float)texture2D.width;
		sourceRect.height = rect3.height;
		screenRect.x = rect.x + rect.width;
		screenRect.y = rect.y;
		screenRect.width = (float)this._padding;
		screenRect.height = rect.height;
		Graphics.DrawTexture(screenRect, texture2D, sourceRect, 0, 0, 0, 0, this.mat);
		sourceRect.x = rect3.x;
		sourceRect.y = rect3.y + 1f - 1f / (float)texture2D.height;
		sourceRect.width = 1f / (float)texture2D.width;
		sourceRect.height = 1f / (float)texture2D.height;
		screenRect.x = rect2.x;
		screenRect.y = rect2.y;
		screenRect.width = (float)this._padding;
		screenRect.height = (float)this._padding;
		Graphics.DrawTexture(screenRect, texture2D, sourceRect, 0, 0, 0, 0, this.mat);
		sourceRect.x = rect3.x + 1f - 1f / (float)texture2D.width;
		sourceRect.y = rect3.y + 1f - 1f / (float)texture2D.height;
		sourceRect.width = 1f / (float)texture2D.width;
		sourceRect.height = 1f / (float)texture2D.height;
		screenRect.x = rect.x + rect.width;
		screenRect.y = rect2.y;
		screenRect.width = (float)this._padding;
		screenRect.height = (float)this._padding;
		Graphics.DrawTexture(screenRect, texture2D, sourceRect, 0, 0, 0, 0, this.mat);
		sourceRect.x = rect3.x;
		sourceRect.y = rect3.y;
		sourceRect.width = 1f / (float)texture2D.width;
		sourceRect.height = 1f / (float)texture2D.height;
		screenRect.x = rect2.x;
		screenRect.y = rect.y + rect.height;
		screenRect.width = (float)this._padding;
		screenRect.height = (float)this._padding;
		Graphics.DrawTexture(screenRect, texture2D, sourceRect, 0, 0, 0, 0, this.mat);
		sourceRect.x = rect3.x + 1f - 1f / (float)texture2D.width;
		sourceRect.y = rect3.y;
		sourceRect.width = 1f / (float)texture2D.width;
		sourceRect.height = 1f / (float)texture2D.height;
		screenRect.x = rect.x + rect.width;
		screenRect.y = rect.y + rect.height;
		screenRect.width = (float)this._padding;
		screenRect.height = (float)this._padding;
		Graphics.DrawTexture(screenRect, texture2D, sourceRect, 0, 0, 0, 0, this.mat);
		Graphics.DrawTexture(rect, texture2D, rect3, 0, 0, 0, 0, this.mat);
		RenderTexture.active = active;
		texture2D.wrapMode = wrapMode;
	}

	private static void _printTexture(Texture2D t)
	{
		if (t.width * t.height > 100)
		{
			Debug.Log("Not printing texture too large.");
			return;
		}
		try
		{
			Color32[] pixels = t.GetPixels32();
			string text = "";
			for (int i = 0; i < t.height; i++)
			{
				for (int j = 0; j < t.width; j++)
				{
					string str = text;
					Color32 color = pixels[i * t.width + j];
					text = str + color.ToString() + ", ";
				}
				text += "\n";
			}
			Debug.Log(text);
		}
		catch (Exception ex)
		{
			Debug.Log("Could not print texture. texture may not be readable." + ex.Message + "\n" + ex.StackTrace.ToString());
		}
	}

	public MB2_LogLevel LOG_LEVEL = MB2_LogLevel.info;

	private Material mat;

	private RenderTexture _destinationTexture;

	private Camera myCamera;

	private int _padding;

	private bool _isNormalMap;

	private bool _fixOutOfBoundsUVs;

	private bool _doRenderAtlas;

	private Rect[] rs;

	private List<MB_TexSet> textureSets;

	private int indexOfTexSetToRender;

	private ShaderTextureProperty _texPropertyName;

	private MB3_TextureCombinerNonTextureProperties _resultMaterialTextureBlender;

	private Texture2D targTex;
}
