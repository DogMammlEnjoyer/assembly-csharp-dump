using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using OVR.OpenVR;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.XR;

[ExecuteInEditMode]
[HelpURL("https://developer.oculus.com/documentation/unity/unity-ovroverlay/")]
public class OVROverlay : MonoBehaviour
{
	public bool previewInEditor
	{
		get
		{
			return this._previewInEditor;
		}
		set
		{
			if (this._previewInEditor != value)
			{
				this._previewInEditor = value;
				this.SetupEditorPreview();
			}
		}
	}

	public void OverrideOverlayTextureInfo(Texture srcTexture, IntPtr nativePtr, XRNode node)
	{
		int num = (node == XRNode.RightEye) ? 1 : 0;
		if (this.textures.Length <= num)
		{
			return;
		}
		this.textures[num] = srcTexture;
		this.texturePtrs[num] = nativePtr;
		this.isOverridePending = true;
	}

	public int layerId { get; private set; }

	protected OVRPlugin.LayerLayout layout
	{
		get
		{
			return OVRPlugin.LayerLayout.Mono;
		}
	}

	public int layerIndex { get; protected set; } = -1;

	public bool isOverlayVisible { get; private set; }

	protected int texturesPerStage
	{
		get
		{
			if (this.layout != OVRPlugin.LayerLayout.Stereo)
			{
				return 1;
			}
			return 2;
		}
	}

	protected static bool NeedsTexturesForShape(OVROverlay.OverlayShape shape)
	{
		return !OVROverlay.IsPassthroughShape(shape);
	}

	protected bool CreateLayer(int mipLevels, int sampleCount, OVRPlugin.EyeTextureFormat etFormat, int flags, OVRPlugin.Sizei size, OVRPlugin.OverlayShape shape)
	{
		if (!this.layerIdHandle.IsAllocated || this.layerIdPtr == IntPtr.Zero)
		{
			this.layerIdHandle = GCHandle.Alloc(this.layerId, GCHandleType.Pinned);
			this.layerIdPtr = this.layerIdHandle.AddrOfPinnedObject();
		}
		if (this.layerIndex == -1)
		{
			this.layerIndex = OVROverlay.instances.IndexOf(this);
			if (this.layerIndex == -1)
			{
				this.layerIndex = OVROverlay.instances.IndexOf(null);
				if (this.layerIndex == -1)
				{
					this.layerIndex = OVROverlay.instances.Count;
					OVROverlay.instances.Add(this);
				}
				else
				{
					OVROverlay.instances[this.layerIndex] = this;
				}
			}
		}
		if (!this.isOverridePending && this.layerDesc.MipLevels == mipLevels && this.layerDesc.SampleCount == sampleCount && this.layerDesc.Format == etFormat && this.layerDesc.Layout == this.layout && this.layerDesc.LayerFlags == flags && this.layerDesc.TextureSize.Equals(size) && this.layerDesc.Shape == shape && this.layerCompositionDepth == this.compositionDepth)
		{
			return false;
		}
		OVRPlugin.LayerDesc desc = OVRPlugin.CalculateLayerDesc(shape, this.layout, size, mipLevels, sampleCount, etFormat, flags);
		OVRPlugin.EnqueueSetupLayer(desc, this.compositionDepth, this.layerIdPtr);
		this.layerId = (int)this.layerIdHandle.Target;
		if (this.layerId > 0)
		{
			this.layerDesc = desc;
			this.layerCompositionDepth = this.compositionDepth;
			if (this.isExternalSurface)
			{
				this.stageCount = 1;
			}
			else
			{
				this.stageCount = OVRPlugin.GetLayerTextureStageCount(this.layerId);
			}
		}
		this.isOverridePending = false;
		return true;
	}

	protected bool CreateLayerTextures(bool useMipmaps, OVRPlugin.Sizei size, bool isHdr)
	{
		if (this.isExternalSurface)
		{
			if (this.externalSurfaceObject == IntPtr.Zero)
			{
				this.externalSurfaceObject = OVRPlugin.GetLayerAndroidSurfaceObject(this.layerId);
				if (this.externalSurfaceObject != IntPtr.Zero)
				{
					Debug.LogFormat("GetLayerAndroidSurfaceObject returns {0}", new object[]
					{
						this.externalSurfaceObject
					});
					if (this.externalSurfaceObjectCreated != null)
					{
						this.externalSurfaceObjectCreated();
					}
				}
			}
			return false;
		}
		bool result = false;
		if (this.stageCount <= 0)
		{
			return false;
		}
		if (this.layerTextures == null)
		{
			this.layerTextures = new OVROverlay.LayerTexture[this.texturesPerStage];
		}
		for (int i = 0; i < this.texturesPerStage; i++)
		{
			if (this.layerTextures[i].swapChain == null)
			{
				this.layerTextures[i].swapChain = new Texture[this.stageCount];
			}
			if (this.layerTextures[i].swapChainPtr == null)
			{
				this.layerTextures[i].swapChainPtr = new IntPtr[this.stageCount];
			}
			for (int j = 0; j < this.stageCount; j++)
			{
				Texture texture = this.layerTextures[i].swapChain[j];
				IntPtr intPtr = this.layerTextures[i].swapChainPtr[j];
				if (!(texture != null) || !(intPtr != IntPtr.Zero) || size.w != texture.width || size.h != texture.height)
				{
					if (intPtr == IntPtr.Zero)
					{
						intPtr = OVRPlugin.GetLayerTexture(this.layerId, j, (OVRPlugin.Eye)i);
					}
					if (!(intPtr == IntPtr.Zero))
					{
						TextureFormat format = isHdr ? TextureFormat.RGBAHalf : TextureFormat.RGBA32;
						if (this.currentOverlayShape != OVROverlay.OverlayShape.Cubemap && this.currentOverlayShape != OVROverlay.OverlayShape.OffcenterCubemap)
						{
							texture = Texture2D.CreateExternalTexture(size.w, size.h, format, useMipmaps, false, intPtr);
						}
						else
						{
							texture = Cubemap.CreateExternalTexture(size.w, format, useMipmaps, intPtr);
						}
						this.layerTextures[i].swapChain[j] = texture;
						this.layerTextures[i].swapChainPtr[j] = intPtr;
						result = true;
					}
				}
			}
		}
		return result;
	}

	protected void DestroyLayerTextures()
	{
		if (this.isExternalSurface)
		{
			return;
		}
		int num = 0;
		while (this.layerTextures != null && num < this.texturesPerStage)
		{
			if (this.layerTextures[num].swapChain != null)
			{
				for (int i = 0; i < this.stageCount; i++)
				{
					Object.Destroy(this.layerTextures[num].swapChain[i]);
				}
			}
			num++;
		}
		this.layerTextures = null;
	}

	protected void DestroyLayer()
	{
		if (this.layerIndex != -1)
		{
			OVRPlugin.EnqueueSubmitLayer(true, false, false, IntPtr.Zero, IntPtr.Zero, -1, 0, OVRPose.identity.ToPosef_Legacy(), Vector3.one.ToVector3f(), this.layerIndex, (OVRPlugin.OverlayShape)this.prevOverlayShape, false, default(OVRPlugin.TextureRectMatrixf), false, default(Vector4), default(Vector4), false, false, false, false, false, false, false, false, false);
			OVROverlay.instances[this.layerIndex] = null;
			this.layerIndex = -1;
		}
		if (this.layerIdPtr != IntPtr.Zero)
		{
			OVRPlugin.EnqueueDestroyLayer(this.layerIdPtr);
			this.layerIdPtr = IntPtr.Zero;
			this.layerIdHandle.Free();
			this.layerId = 0;
		}
		this.layerDesc = default(OVRPlugin.LayerDesc);
		this.frameIndex = 0;
		this.prevFrameIndex = -1;
	}

	public void SetSrcDestRects(Rect srcLeft, Rect srcRight, Rect destLeft, Rect destRight)
	{
		this.srcRectLeft = srcLeft;
		this.srcRectRight = srcRight;
		this.destRectLeft = destLeft;
		this.destRectRight = destRight;
	}

	public void UpdateTextureRectMatrix()
	{
		Rect leftRect = new Rect(this.srcRectLeft.x, (this.isExternalSurface ^ this.invertTextureRects) ? (1f - this.srcRectLeft.y - this.srcRectLeft.height) : this.srcRectLeft.y, this.srcRectLeft.width, this.srcRectLeft.height);
		Rect rightRect = new Rect(this.srcRectRight.x, (this.isExternalSurface ^ this.invertTextureRects) ? (1f - this.srcRectRight.y - this.srcRectRight.height) : this.srcRectRight.y, this.srcRectRight.width, this.srcRectRight.height);
		Rect rect = new Rect(this.destRectLeft.x, (this.isExternalSurface ^ this.invertTextureRects) ? (1f - this.destRectLeft.y - this.destRectLeft.height) : this.destRectLeft.y, this.destRectLeft.width, this.destRectLeft.height);
		Rect rect2 = new Rect(this.destRectRight.x, (this.isExternalSurface ^ this.invertTextureRects) ? (1f - this.destRectRight.y - this.destRectRight.height) : this.destRectRight.y, this.destRectRight.width, this.destRectRight.height);
		this.textureRectMatrix.leftRect = leftRect;
		this.textureRectMatrix.rightRect = rightRect;
		if (this.currentOverlayShape == OVROverlay.OverlayShape.Fisheye)
		{
			rect.x -= 0.5f;
			rect.y -= 0.5f;
			rect2.x -= 0.5f;
			rect2.y -= 0.5f;
		}
		float num = this.srcRectLeft.width / this.destRectLeft.width;
		float num2 = this.srcRectLeft.height / this.destRectLeft.height;
		this.textureRectMatrix.leftScaleBias = new Vector4(num, num2, leftRect.x - rect.x * num, leftRect.y - rect.y * num2);
		float num3 = this.srcRectRight.width / this.destRectRight.width;
		float num4 = this.srcRectRight.height / this.destRectRight.height;
		this.textureRectMatrix.rightScaleBias = new Vector4(num3, num4, rightRect.x - rect2.x * num3, rightRect.y - rect2.y * num4);
	}

	public void SetPerLayerColorScaleAndOffset(Vector4 scale, Vector4 offset)
	{
		this.colorScale = scale;
		this.colorOffset = offset;
	}

	protected bool LatchLayerTextures()
	{
		if (this.isExternalSurface)
		{
			return true;
		}
		for (int i = 0; i < this.texturesPerStage; i++)
		{
			if ((this.textures[i] != this.layerTextures[i].appTexture || this.layerTextures[i].appTexturePtr == IntPtr.Zero) && this.textures[i] != null)
			{
				RenderTexture renderTexture = this.textures[i] as RenderTexture;
				if (renderTexture && !renderTexture.IsCreated())
				{
					renderTexture.Create();
				}
				this.layerTextures[i].appTexturePtr = ((this.texturePtrs[i] != IntPtr.Zero) ? this.texturePtrs[i] : this.textures[i].GetNativeTexturePtr());
				if (this.layerTextures[i].appTexturePtr != IntPtr.Zero)
				{
					this.layerTextures[i].appTexture = this.textures[i];
				}
			}
			if (this.currentOverlayShape == OVROverlay.OverlayShape.Cubemap && this.textures[i] as Cubemap == null)
			{
				Debug.LogError("Need Cubemap texture for cube map overlay");
				return false;
			}
		}
		if (this.currentOverlayShape == OVROverlay.OverlayShape.OffcenterCubemap)
		{
			Debug.LogWarning("Overlay shape " + this.currentOverlayShape.ToString() + " is not supported on current platform");
			return false;
		}
		return !(this.layerTextures[0].appTexture == null) && !(this.layerTextures[0].appTexturePtr == IntPtr.Zero);
	}

	protected OVRPlugin.LayerDesc GetCurrentLayerDesc()
	{
		OVRPlugin.Sizei textureSize = new OVRPlugin.Sizei
		{
			w = 0,
			h = 0
		};
		if (this.isExternalSurface)
		{
			textureSize.w = this.externalSurfaceWidth;
			textureSize.h = this.externalSurfaceHeight;
		}
		else if (OVROverlay.NeedsTexturesForShape(this.currentOverlayShape))
		{
			if (this.textures[0] == null)
			{
				Debug.LogWarning("textures[0] hasn't been set");
			}
			textureSize.w = (this.textures[0] ? this.textures[0].width : 0);
			textureSize.h = (this.textures[0] ? this.textures[0].height : 0);
		}
		OVRPlugin.LayerDesc result = new OVRPlugin.LayerDesc
		{
			Format = this.layerTextureFormat,
			LayerFlags = (this.isExternalSurface ? 0 : 8),
			Layout = this.layout,
			MipLevels = 1,
			SampleCount = 1,
			Shape = (OVRPlugin.OverlayShape)this.currentOverlayShape,
			TextureSize = textureSize
		};
		Texture2D texture2D = this.textures[0] as Texture2D;
		if (texture2D != null)
		{
			if (texture2D.format == TextureFormat.RGBAHalf || texture2D.format == TextureFormat.RGBAFloat)
			{
				result.Format = OVRPlugin.EyeTextureFormat.R16G16B16A16_FP;
			}
			result.MipLevels = texture2D.mipmapCount;
		}
		Cubemap cubemap = this.textures[0] as Cubemap;
		if (cubemap != null)
		{
			if (cubemap.format == TextureFormat.RGBAHalf || cubemap.format == TextureFormat.RGBAFloat)
			{
				result.Format = OVRPlugin.EyeTextureFormat.R16G16B16A16_FP;
			}
			result.MipLevels = cubemap.mipmapCount;
		}
		RenderTexture renderTexture = this.textures[0] as RenderTexture;
		if (renderTexture != null)
		{
			result.SampleCount = renderTexture.antiAliasing;
			if (renderTexture.format == RenderTextureFormat.ARGBHalf || renderTexture.format == RenderTextureFormat.ARGBFloat || renderTexture.format == RenderTextureFormat.RGB111110Float)
			{
				result.Format = OVRPlugin.EyeTextureFormat.R16G16B16A16_FP;
			}
		}
		if (this.isProtectedContent)
		{
			result.LayerFlags |= 64;
		}
		if (this.isExternalSurface)
		{
			result.LayerFlags |= 128;
		}
		if (this.useBicubicFiltering)
		{
			result.LayerFlags |= 16384;
		}
		return result;
	}

	protected Rect GetBlitRect(int eyeId, int width, int height, bool invertRect)
	{
		Rect rect;
		if (this.texturesPerStage == 2)
		{
			rect = ((eyeId == 0) ? this.srcRectLeft : this.srcRectRight);
		}
		else
		{
			float num = Mathf.Min(this.srcRectLeft.x, this.srcRectRight.x);
			float num2 = Mathf.Min(this.srcRectLeft.y, this.srcRectRight.y);
			float num3 = Mathf.Max(this.srcRectLeft.x + this.srcRectLeft.width, this.srcRectRight.x + this.srcRectRight.width);
			float num4 = Mathf.Max(this.srcRectLeft.y + this.srcRectLeft.height, this.srcRectRight.y + this.srcRectRight.height);
			rect = new Rect(num, num2, num3 - num, num4 - num2);
		}
		if (invertRect)
		{
			rect.y = 1f - rect.y - rect.height;
		}
		return new Rect(Mathf.Max(0f, Mathf.Floor((float)width * rect.x) - 2f), Mathf.Max(0f, Mathf.Floor((float)height * rect.y) - 2f), Mathf.Min((float)width, Mathf.Ceil((float)width * rect.xMax) - Mathf.Floor((float)width * rect.x) + 4f), Mathf.Min((float)height, Mathf.Ceil((float)height * rect.yMax) - Mathf.Floor((float)height * rect.y) + 4f));
	}

	protected void BlitSubImage(Texture src, int width, int height, Material mat, Rect rect)
	{
		this._commandBuffer.SetRenderTarget(OVROverlay._tempRenderTextureId, RenderBufferLoadAction.DontCare, RenderBufferStoreAction.Store);
		this._commandBuffer.SetProjectionMatrix(Matrix4x4.Ortho(-1f, 1f, -1f, 1f, -1f, 1f));
		this._commandBuffer.SetViewMatrix(Matrix4x4.identity);
		this._commandBuffer.EnableScissorRect(new Rect(0f, 0f, rect.width, rect.height));
		this._commandBuffer.SetViewport(new Rect(-rect.x, -rect.y, (float)width, (float)height));
		mat.mainTexture = src;
		mat.SetPass(0);
		if (this._blitMesh == null)
		{
			this._blitMesh = new Mesh
			{
				name = "OVROverlay Blit Mesh"
			};
			this._blitMesh.SetVertices(new Vector3[]
			{
				new Vector3(-1f, -1f, 0f),
				new Vector3(-1f, 3f, 0f),
				new Vector3(3f, -1f, 0f)
			});
			this._blitMesh.SetUVs(0, new Vector2[]
			{
				new Vector2(0f, 0f),
				new Vector2(0f, 2f),
				new Vector2(2f, 0f)
			});
			this._blitMesh.SetIndices(new ushort[]
			{
				0,
				1,
				2
			}, MeshTopology.Triangles, 0, true, 0);
			this._blitMesh.UploadMeshData(true);
		}
		this._commandBuffer.DrawMesh(this._blitMesh, Matrix4x4.identity, mat);
	}

	protected bool PopulateLayer(int mipLevels, bool isHdr, OVRPlugin.Sizei size, int sampleCount, int stage)
	{
		if (this.isExternalSurface)
		{
			return true;
		}
		bool flag = false;
		RenderTextureFormat colorFormat = isHdr ? RenderTextureFormat.ARGBHalf : RenderTextureFormat.ARGB32;
		if (this._commandBuffer == null)
		{
			this._commandBuffer = new CommandBuffer();
		}
		this._commandBuffer.Clear();
		this._commandBuffer.name = this.ToString();
		for (int i = 0; i < this.texturesPerStage; i++)
		{
			Texture texture = this.layerTextures[i].swapChain[stage];
			if (!(texture == null))
			{
				flag = true;
				bool flag2 = !this.isAlphaPremultiplied && !OVRPlugin.unpremultipliedAlphaLayersSupported;
				bool flag3 = this.isAlphaPremultiplied && !OVRPlugin.premultipliedAlphaLayersSupported;
				bool flag4 = SystemInfo.graphicsDeviceType == GraphicsDeviceType.OpenGLES3 || SystemInfo.graphicsDeviceType == GraphicsDeviceType.OpenGLES2;
				bool flag5 = texture.width == this.textures[i].width && texture.height == this.textures[i].height;
				bool flag6 = this.textures[i].mipmapCount == texture.mipmapCount;
				bool flag7 = this.currentOverlayShape == OVROverlay.OverlayShape.Cubemap || this.currentOverlayShape == OVROverlay.OverlayShape.OffcenterCubemap;
				if (Application.isMobilePlatform && !flag4 && flag5 && flag6 && !flag3)
				{
					this._commandBuffer.CopyTexture(this.textures[i], texture);
				}
				else
				{
					for (int j = 0; j < mipLevels; j++)
					{
						int num = size.w >> j;
						if (num < 1)
						{
							num = 1;
						}
						int num2 = size.h >> j;
						if (num2 < 1)
						{
							num2 = 1;
						}
						int width = num;
						int height = num2;
						if (this.overrideTextureRectMatrix && this.isDynamic)
						{
							Rect blitRect = this.GetBlitRect(i, num, num2, this.invertTextureRects);
							width = (int)blitRect.width;
							height = (int)blitRect.height;
						}
						RenderTextureDescriptor desc = new RenderTextureDescriptor(width, height, colorFormat, 0);
						desc.msaaSamples = sampleCount;
						desc.useMipMap = false;
						desc.autoGenerateMips = false;
						desc.sRGB = true;
						this._commandBuffer.GetTemporaryRT(OVROverlay._tempRenderTextureId, desc, FilterMode.Point);
						int num3 = flag7 ? 6 : 1;
						for (int k = 0; k < num3; k++)
						{
							Material material = flag7 ? OVROverlay.cubeMaterial[k] : OVROverlay.tex2DMaterial;
							material.SetInt("_premultiply", flag2 ? 1 : 0);
							material.SetInt("_unmultiply", flag3 ? 1 : 0);
							if (!flag7)
							{
								material.SetInt("_flip", (OVRPlugin.nativeXrApi == OVRPlugin.XrApi.OpenXR) ? 1 : 0);
							}
							if (!flag7 && this.overrideTextureRectMatrix && this.isDynamic)
							{
								Rect blitRect2 = this.GetBlitRect(i, num, num2, this.invertTextureRects);
								this.BlitSubImage(this.textures[i], num, num2, material, blitRect2);
								this._commandBuffer.CopyTexture(OVROverlay._tempRenderTextureId, 0, 0, 0, 0, (int)blitRect2.width, (int)blitRect2.height, texture, k, j, (int)blitRect2.x, (int)blitRect2.y);
							}
							else
							{
								this._commandBuffer.Blit(this.textures[i], OVROverlay._tempRenderTextureId, material);
								this._commandBuffer.CopyTexture(OVROverlay._tempRenderTextureId, 0, 0, texture, k, j);
							}
						}
						this._commandBuffer.ReleaseTemporaryRT(OVROverlay._tempRenderTextureId);
					}
				}
			}
		}
		if (flag)
		{
			Graphics.ExecuteCommandBuffer(this._commandBuffer);
		}
		return flag;
	}

	protected bool SubmitLayer(bool overlay, bool headLocked, bool noDepthBufferTesting, OVRPose pose, Vector3 scale, int frameIndex)
	{
		int num = (this.texturesPerStage >= 2) ? 1 : 0;
		if (this.overrideTextureRectMatrix)
		{
			this.UpdateTextureRectMatrix();
		}
		bool efficientSharpen = this.useEfficientSharpen;
		bool efficientSuperSample = this.useEfficientSupersample;
		bool premultipledAlpha = this.isAlphaPremultiplied && OVRPlugin.premultipliedAlphaLayersSupported;
		if (this.useAutomaticFiltering && !this.useEfficientSharpen && !this.useEfficientSupersample && !this.useExpensiveSharpen && !this.useExpensiveSuperSample)
		{
			efficientSharpen = true;
			efficientSuperSample = true;
		}
		if (!this.useAutomaticFiltering && ((this.useEfficientSharpen && this.useEfficientSupersample) || (this.useExpensiveSharpen && this.useExpensiveSuperSample) || (this.useEfficientSharpen && this.useExpensiveSuperSample) || (this.useExpensiveSharpen && this.useEfficientSupersample)))
		{
			Debug.LogError("Warning-XR sharpening and supersampling cannot be enabled simultaneously, either enable autofiltering or disable one of the options");
			return false;
		}
		bool flag = this.isExternalSurface || !OVROverlay.NeedsTexturesForShape(this.currentOverlayShape);
		bool result = OVRPlugin.EnqueueSubmitLayer(overlay, headLocked, noDepthBufferTesting, flag ? IntPtr.Zero : this.layerTextures[0].appTexturePtr, flag ? IntPtr.Zero : this.layerTextures[num].appTexturePtr, this.layerId, frameIndex, pose.flipZ().ToPosef_Legacy(), scale.ToVector3f(), this.layerIndex, (OVRPlugin.OverlayShape)this.currentOverlayShape, this.overrideTextureRectMatrix, this.textureRectMatrix, this.overridePerLayerColorScaleAndOffset, this.colorScale, this.colorOffset, this.useExpensiveSuperSample, this.useBicubicFiltering, efficientSuperSample, efficientSharpen, this.useExpensiveSharpen, this.hidden, this.isProtectedContent, this.useAutomaticFiltering, premultipledAlpha);
		this.prevOverlayShape = this.currentOverlayShape;
		return result;
	}

	protected void SetupEditorPreview()
	{
	}

	public void ResetEditorPreview()
	{
		this.previewInEditor = false;
		this.previewInEditor = true;
	}

	public static bool IsPassthroughShape(OVROverlay.OverlayShape shape)
	{
		return OVRPlugin.IsPassthroughShape((OVRPlugin.OverlayShape)shape);
	}

	private void Awake()
	{
		if (Application.isPlaying)
		{
			if (OVROverlay.tex2DMaterial == null)
			{
				OVROverlay.tex2DMaterial = new Material(Shader.Find("Oculus/Texture2D Blit"));
			}
			Shader shader = null;
			for (int i = 0; i < 6; i++)
			{
				if (OVROverlay.cubeMaterial[i] == null)
				{
					if (shader == null)
					{
						shader = Shader.Find("Oculus/Cubemap Blit");
					}
					OVROverlay.cubeMaterial[i] = new Material(shader);
				}
				OVROverlay.cubeMaterial[i].SetInt("_face", i);
			}
		}
		this.rend = base.GetComponent<Renderer>();
		if (this.textures.Length == 0)
		{
			this.textures = new Texture[1];
		}
		if (this.rend != null && this.textures[0] == null)
		{
			this.textures[0] = this.rend.sharedMaterial.mainTexture;
		}
		this.SetupEditorPreview();
	}

	public static string OpenVROverlayKey
	{
		get
		{
			return "unity:" + Application.companyName + "." + Application.productName;
		}
	}

	private void OnEnable()
	{
		if (OVRManager.OVRManagerinitialized)
		{
			this.InitOVROverlay();
		}
		if (!base.enabled)
		{
			return;
		}
		this.SetupEditorPreview();
		Camera.onPreRender = (Camera.CameraCallback)Delegate.Combine(Camera.onPreRender, new Camera.CameraCallback(this.HandlePreRender));
		RenderPipelineManager.beginCameraRendering += this.HandleBeginCameraRendering;
	}

	private void InitOVROverlay()
	{
		if (!OVRPlugin.UnityOpenXR.Enabled && !OVRManager.isHmdPresent)
		{
			base.enabled = false;
			return;
		}
		this.constructedOverlayXRDevice = OVRManager.XRDevice.Unknown;
		if (OVRManager.loadedXRDevice == OVRManager.XRDevice.OpenVR)
		{
			CVROverlay overlay = OpenVR.Overlay;
			if (overlay == null)
			{
				base.enabled = false;
				return;
			}
			if (overlay.CreateOverlay(OVROverlay.OpenVROverlayKey + base.transform.name, base.gameObject.name, ref this.OpenVROverlayHandle) != EVROverlayError.None)
			{
				base.enabled = false;
				return;
			}
		}
		this.constructedOverlayXRDevice = OVRManager.loadedXRDevice;
		this.xrDeviceConstructed = true;
	}

	private void OnDisable()
	{
		if (base.gameObject.scene.name == "DontDestroyOnLoad")
		{
			return;
		}
		this.DisableImmediately();
	}

	private void DisableImmediately()
	{
		Camera.onPreRender = (Camera.CameraCallback)Delegate.Remove(Camera.onPreRender, new Camera.CameraCallback(this.HandlePreRender));
		RenderPipelineManager.beginCameraRendering -= this.HandleBeginCameraRendering;
		if (!OVRManager.OVRManagerinitialized)
		{
			return;
		}
		if (OVRManager.loadedXRDevice != this.constructedOverlayXRDevice)
		{
			return;
		}
		if (OVRManager.loadedXRDevice == OVRManager.XRDevice.Oculus)
		{
			this.DestroyLayerTextures();
			this.DestroyLayer();
		}
		else if (OVRManager.loadedXRDevice == OVRManager.XRDevice.OpenVR && this.OpenVROverlayHandle != 0UL)
		{
			CVROverlay overlay = OpenVR.Overlay;
			if (overlay != null)
			{
				overlay.DestroyOverlay(this.OpenVROverlayHandle);
			}
			this.OpenVROverlayHandle = 0UL;
		}
		this.constructedOverlayXRDevice = OVRManager.XRDevice.Unknown;
		this.xrDeviceConstructed = false;
	}

	private void OnDestroy()
	{
		this.DisableImmediately();
		this.DestroyLayerTextures();
		this.DestroyLayer();
		if (this._commandBuffer != null)
		{
			this._commandBuffer.Dispose();
		}
		if (this._blitMesh != null)
		{
			Object.DestroyImmediate(this._blitMesh);
		}
	}

	private void ComputePoseAndScale(out OVRPose pose, out Vector3 scale, out bool overlay, out bool headLocked)
	{
		Camera camera = OVRManager.FindMainCamera();
		overlay = (this.currentOverlayType == OVROverlay.OverlayType.Overlay);
		headLocked = false;
		Transform transform = base.transform;
		while (transform != null && !headLocked)
		{
			headLocked |= (transform == camera.transform);
			transform = transform.parent;
		}
		pose = (headLocked ? base.transform.ToHeadSpacePose(camera) : base.transform.ToTrackingSpacePose(camera));
		scale = base.transform.lossyScale;
		for (int i = 0; i < 3; i++)
		{
			ref Vector3 ptr = ref scale;
			int index = i;
			ptr[index] /= camera.transform.lossyScale[i];
		}
		if (this.currentOverlayShape == OVROverlay.OverlayShape.Cubemap)
		{
			if (!this.useLegacyCubemapRotation)
			{
				pose.orientation *= Quaternion.AngleAxis(180f, Vector3.up);
			}
			pose.position = camera.transform.position;
		}
	}

	private bool ComputeSubmit(out OVRPose pose, out Vector3 scale, out bool overlay, out bool headLocked)
	{
		this.ComputePoseAndScale(out pose, out scale, out overlay, out headLocked);
		if (this.currentOverlayShape == OVROverlay.OverlayShape.OffcenterCubemap)
		{
			pose.position = base.transform.position;
			if (pose.position.magnitude > 1f)
			{
				Debug.LogWarning("Your cube map center offset's magnitude is greater than 1, which will cause some cube map pixel always invisible .");
				return false;
			}
		}
		if (OVRPlugin.nativeXrApi != OVRPlugin.XrApi.OpenXR && this.currentOverlayShape == OVROverlay.OverlayShape.Cylinder)
		{
			float num = scale.x / scale.z / 3.1415927f * 180f;
			if (num > 180f)
			{
				Debug.LogWarning("Cylinder overlay's arc angle has to be below 180 degree, current arc angle is " + num.ToString() + " degree.");
				return false;
			}
		}
		if (OVRPlugin.nativeXrApi == OVRPlugin.XrApi.OpenXR && this.currentOverlayShape == OVROverlay.OverlayShape.Fisheye)
		{
			Debug.LogWarning("Fisheye overlay shape is not support on OpenXR");
			return false;
		}
		return true;
	}

	private bool OpenVROverlayUpdate(Vector3 scale, OVRPose pose)
	{
		CVROverlay overlay = OpenVR.Overlay;
		if (overlay == null)
		{
			return false;
		}
		Texture texture = this.textures[0];
		if (texture == null)
		{
			return false;
		}
		EVROverlayError evroverlayError = overlay.ShowOverlay(this.OpenVROverlayHandle);
		if ((evroverlayError == EVROverlayError.InvalidHandle || evroverlayError == EVROverlayError.UnknownOverlay) && overlay.FindOverlay(OVROverlay.OpenVROverlayKey + base.transform.name, ref this.OpenVROverlayHandle) != EVROverlayError.None)
		{
			return false;
		}
		Texture_t texture_t = default(Texture_t);
		texture_t.handle = texture.GetNativeTexturePtr();
		texture_t.eType = (SystemInfo.graphicsDeviceVersion.StartsWith("OpenGL") ? ETextureType.OpenGL : ETextureType.DirectX);
		texture_t.eColorSpace = EColorSpace.Auto;
		overlay.SetOverlayTexture(this.OpenVROverlayHandle, ref texture_t);
		VRTextureBounds_t vrtextureBounds_t = default(VRTextureBounds_t);
		vrtextureBounds_t.uMin = (0f + this.OpenVRUVOffsetAndScale.x) * this.OpenVRUVOffsetAndScale.z;
		vrtextureBounds_t.vMin = (1f + this.OpenVRUVOffsetAndScale.y) * this.OpenVRUVOffsetAndScale.w;
		vrtextureBounds_t.uMax = (1f + this.OpenVRUVOffsetAndScale.x) * this.OpenVRUVOffsetAndScale.z;
		vrtextureBounds_t.vMax = (0f + this.OpenVRUVOffsetAndScale.y) * this.OpenVRUVOffsetAndScale.w;
		overlay.SetOverlayTextureBounds(this.OpenVROverlayHandle, ref vrtextureBounds_t);
		HmdVector2_t hmdVector2_t = default(HmdVector2_t);
		hmdVector2_t.v0 = this.OpenVRMouseScale.x;
		hmdVector2_t.v1 = this.OpenVRMouseScale.y;
		overlay.SetOverlayMouseScale(this.OpenVROverlayHandle, ref hmdVector2_t);
		overlay.SetOverlayWidthInMeters(this.OpenVROverlayHandle, scale.x);
		HmdMatrix34_t hmdMatrix34_t = Matrix4x4.TRS(pose.position, pose.orientation, Vector3.one).ConvertToHMDMatrix34();
		overlay.SetOverlayTransformAbsolute(this.OpenVROverlayHandle, ETrackingUniverseOrigin.TrackingUniverseStanding, ref hmdMatrix34_t);
		return true;
	}

	private void HandlePreRender(Camera camera)
	{
		if (camera == OVRManager.FindMainCamera())
		{
			this.isOverlayVisible = this.TrySubmitLayer();
		}
	}

	private void HandleBeginCameraRendering(ScriptableRenderContext context, Camera camera)
	{
		if (camera == OVRManager.FindMainCamera())
		{
			this.isOverlayVisible = this.TrySubmitLayer();
		}
	}

	private bool TrySubmitLayer()
	{
		if (!base.enabled)
		{
			this.DisableImmediately();
			return false;
		}
		if (!OVRManager.OVRManagerinitialized || !OVRPlugin.userPresent)
		{
			return false;
		}
		if (!this.xrDeviceConstructed)
		{
			this.InitOVROverlay();
		}
		if (OVRManager.loadedXRDevice != this.constructedOverlayXRDevice)
		{
			Debug.LogError("Warning-XR Device was switched during runtime with overlays still enabled. When doing so, all overlays constructed with the previous XR device must first be disabled.");
			return false;
		}
		bool flag = !this.isExternalSurface && OVROverlay.NeedsTexturesForShape(this.currentOverlayShape);
		if (this.currentOverlayType == OVROverlay.OverlayType.None || (flag && (this.textures.Length < this.texturesPerStage || this.textures[0] == null)))
		{
			return false;
		}
		OVRPose pose;
		Vector3 scale;
		bool overlay;
		bool headLocked;
		if (!this.ComputeSubmit(out pose, out scale, out overlay, out headLocked))
		{
			return false;
		}
		if (OVRManager.loadedXRDevice == OVRManager.XRDevice.OpenVR)
		{
			return this.currentOverlayShape == OVROverlay.OverlayShape.Quad && this.OpenVROverlayUpdate(scale, pose);
		}
		OVRPlugin.LayerDesc currentLayerDesc = this.GetCurrentLayerDesc();
		bool isHdr = currentLayerDesc.Format == OVRPlugin.EyeTextureFormat.R16G16B16A16_FP;
		bool flag2 = !this.layerDesc.TextureSize.Equals(currentLayerDesc.TextureSize) && this.layerId > 0;
		bool flag3 = OVROverlay.NeedsTexturesForShape(this.currentOverlayShape);
		bool flag4 = OVROverlay.NeedsTexturesForShape(this.prevOverlayShape) != flag3;
		if (flag2 || flag4)
		{
			this.DestroyLayerTextures();
			this.DestroyLayer();
		}
		bool flag5 = this.CreateLayer(currentLayerDesc.MipLevels, currentLayerDesc.SampleCount, currentLayerDesc.Format, currentLayerDesc.LayerFlags, currentLayerDesc.TextureSize, currentLayerDesc.Shape);
		if (this.layerIndex == -1 || this.layerId <= 0)
		{
			if (flag5)
			{
				this.prevOverlayShape = this.currentOverlayShape;
			}
			return false;
		}
		if (flag3)
		{
			bool useMipmaps = currentLayerDesc.MipLevels > 1;
			flag5 |= this.CreateLayerTextures(useMipmaps, currentLayerDesc.TextureSize, isHdr);
			if (!this.isExternalSurface && this.layerTextures[0].appTexture as RenderTexture != null)
			{
				this.isDynamic = true;
			}
			if (!this.LatchLayerTextures())
			{
				return false;
			}
			if (this.frameIndex > this.prevFrameIndex)
			{
				int stage = this.frameIndex % this.stageCount;
				if (!this.PopulateLayer(currentLayerDesc.MipLevels, isHdr, currentLayerDesc.TextureSize, currentLayerDesc.SampleCount, stage))
				{
					return false;
				}
			}
		}
		bool flag6 = this.SubmitLayer(overlay, headLocked, this.noDepthBufferTesting, pose, scale, this.frameIndex);
		this.prevFrameIndex = this.frameIndex;
		if (this.isDynamic)
		{
			this.frameIndex++;
		}
		if (this.rend)
		{
			this.rend.enabled = !flag6;
		}
		return flag6;
	}

	[Tooltip("Specify overlay's type")]
	public OVROverlay.OverlayType currentOverlayType = OVROverlay.OverlayType.Overlay;

	[Tooltip("If true, the texture's content is copied to the compositor each frame.")]
	public bool isDynamic;

	[Tooltip("If true, the layer would be used to present protected content (e.g. HDCP), the content won't be shown in screenshots or recordings.")]
	public bool isProtectedContent;

	public Rect srcRectLeft = new Rect(0f, 0f, 1f, 1f);

	public Rect srcRectRight = new Rect(0f, 0f, 1f, 1f);

	public Rect destRectLeft = new Rect(0f, 0f, 1f, 1f);

	public Rect destRectRight = new Rect(0f, 0f, 1f, 1f);

	public bool invertTextureRects;

	private OVRPlugin.TextureRectMatrixf textureRectMatrix = OVRPlugin.TextureRectMatrixf.zero;

	public bool overrideTextureRectMatrix;

	public bool overridePerLayerColorScaleAndOffset;

	public Vector4 colorScale = Vector4.one;

	public Vector4 colorOffset = Vector4.zero;

	public bool useExpensiveSuperSample;

	public bool useExpensiveSharpen;

	public bool hidden;

	[Tooltip("If true, the layer will be created as an external surface. externalSurfaceObject contains the Surface object. It's effective only on Android.")]
	public bool isExternalSurface;

	[Tooltip("The width which will be used to create the external surface. It's effective only on Android.")]
	public int externalSurfaceWidth;

	[Tooltip("The height which will be used to create the external surface. It's effective only on Android.")]
	public int externalSurfaceHeight;

	[Tooltip("The compositionDepth defines the order of the OVROverlays in composition. The overlay/underlay with smaller compositionDepth would be composited in the front of the overlay/underlay with larger compositionDepth.")]
	public int compositionDepth;

	private int layerCompositionDepth;

	[Tooltip("The noDepthBufferTesting will stop layer's depth buffer compositing even if the engine has \"Shared Depth Buffer\" enabled. The layer's ordering will be used instead which is determined by it's composition depth and overlay/underlay type.")]
	public bool noDepthBufferTesting = true;

	public OVRPlugin.EyeTextureFormat layerTextureFormat;

	[Tooltip("Specify overlay's shape")]
	public OVROverlay.OverlayShape currentOverlayShape;

	private OVROverlay.OverlayShape prevOverlayShape;

	[Tooltip("The left- and right-eye Textures to show in the layer.")]
	public Texture[] textures = new Texture[2];

	[Tooltip("When checked, the texture is treated as if the alpha was already premultiplied")]
	public bool isAlphaPremultiplied;

	[Tooltip("When checked, the layer will use bicubic filtering")]
	public bool useBicubicFiltering;

	[Tooltip("When checked, the cubemap will retain the legacy rotation which was rotated 180 degrees around the Y axis comapred to Unity's definition of cubemaps. This setting will be deprecated in the near future, therefore it is recommended to fix the cubemap texture instead.")]
	public bool useLegacyCubemapRotation;

	[Tooltip("When checked, the layer will use efficient super sampling")]
	public bool useEfficientSupersample;

	[Tooltip("When checked, the layer will use efficient sharpen.")]
	public bool useEfficientSharpen;

	[Tooltip("When checked, The runtime automatically chooses the appropriate sharpening or super sampling filter")]
	public bool useAutomaticFiltering;

	[SerializeField]
	internal bool _previewInEditor;

	protected IntPtr[] texturePtrs = new IntPtr[]
	{
		IntPtr.Zero,
		IntPtr.Zero
	};

	public IntPtr externalSurfaceObject;

	public OVROverlay.ExternalSurfaceObjectCreated externalSurfaceObjectCreated;

	protected bool isOverridePending;

	public static List<OVROverlay> instances = new List<OVROverlay>();

	protected static Material tex2DMaterial;

	protected static readonly Material[] cubeMaterial = new Material[6];

	protected OVROverlay.LayerTexture[] layerTextures;

	protected OVRPlugin.LayerDesc layerDesc;

	protected int stageCount = -1;

	protected GCHandle layerIdHandle;

	protected IntPtr layerIdPtr = IntPtr.Zero;

	protected int frameIndex;

	protected int prevFrameIndex = -1;

	protected Renderer rend;

	private static readonly int _tempRenderTextureId = Shader.PropertyToID("_OVROverlayTempTexture");

	private CommandBuffer _commandBuffer;

	private Mesh _blitMesh;

	private ulong OpenVROverlayHandle;

	private Vector4 OpenVRUVOffsetAndScale = new Vector4(0f, 0f, 1f, 1f);

	private Vector2 OpenVRMouseScale = new Vector2(1f, 1f);

	private OVRManager.XRDevice constructedOverlayXRDevice;

	private bool xrDeviceConstructed;

	public enum OverlayShape
	{
		Quad,
		Cylinder,
		Cubemap,
		OffcenterCubemap = 4,
		Equirect,
		ReconstructionPassthrough = 7,
		SurfaceProjectedPassthrough,
		Fisheye,
		KeyboardHandsPassthrough,
		KeyboardMaskedHandsPassthrough
	}

	public enum OverlayType
	{
		None,
		Underlay,
		Overlay
	}

	public delegate void ExternalSurfaceObjectCreated();

	protected struct LayerTexture
	{
		public Texture appTexture;

		public IntPtr appTexturePtr;

		public Texture[] swapChain;

		public IntPtr[] swapChainPtr;
	}
}
