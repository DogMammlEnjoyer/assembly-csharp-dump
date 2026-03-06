using System;

namespace UnityEngine.Rendering.Universal
{
	[Serializable]
	internal class PixelPerfectCameraInternal : ISerializationCallbackReceiver
	{
		internal PixelPerfectCameraInternal(IPixelPerfectCamera component)
		{
			this.m_Component = component;
		}

		public void OnBeforeSerialize()
		{
			this.m_SerializableComponent = (this.m_Component as PixelPerfectCamera);
		}

		public void OnAfterDeserialize()
		{
			if (this.m_SerializableComponent != null)
			{
				this.m_Component = this.m_SerializableComponent;
			}
		}

		internal void CalculateCameraProperties(int screenWidth, int screenHeight)
		{
			int assetsPPU = this.m_Component.assetsPPU;
			int refResolutionX = this.m_Component.refResolutionX;
			int refResolutionY = this.m_Component.refResolutionY;
			bool upscaleRT = this.m_Component.upscaleRT;
			bool pixelSnapping = this.m_Component.pixelSnapping;
			bool cropFrameX = this.m_Component.cropFrameX;
			bool cropFrameY = this.m_Component.cropFrameY;
			bool stretchFill = this.m_Component.stretchFill;
			this.cropFrameXAndY = (cropFrameY && cropFrameX);
			this.cropFrameXOrY = (cropFrameY || cropFrameX);
			this.useStretchFill = (this.cropFrameXAndY && stretchFill);
			this.requiresUpscaling = this.useStretchFill;
			int val = screenHeight / refResolutionY;
			int val2 = screenWidth / refResolutionX;
			this.zoom = Math.Max(1, Math.Min(val, val2));
			this.useOffscreenRT = false;
			this.offscreenRTWidth = 0;
			this.offscreenRTHeight = 0;
			if (this.cropFrameXOrY)
			{
				this.useOffscreenRT = true;
				if (!upscaleRT)
				{
					if (this.cropFrameXAndY)
					{
						this.offscreenRTWidth = this.zoom * refResolutionX;
						this.offscreenRTHeight = this.zoom * refResolutionY;
					}
					else if (cropFrameY)
					{
						this.offscreenRTWidth = screenWidth;
						this.offscreenRTHeight = this.zoom * refResolutionY;
					}
					else
					{
						this.offscreenRTWidth = this.zoom * refResolutionX;
						this.offscreenRTHeight = screenHeight;
					}
				}
				else if (this.cropFrameXAndY)
				{
					this.offscreenRTWidth = refResolutionX;
					this.offscreenRTHeight = refResolutionY;
				}
				else if (cropFrameY)
				{
					this.offscreenRTWidth = screenWidth / this.zoom / 2 * 2;
					this.offscreenRTHeight = refResolutionY;
				}
				else
				{
					this.offscreenRTWidth = refResolutionX;
					this.offscreenRTHeight = screenHeight / this.zoom / 2 * 2;
				}
			}
			else if (upscaleRT && this.zoom > 1)
			{
				this.useOffscreenRT = true;
				this.offscreenRTWidth = screenWidth / this.zoom / 2 * 2;
				this.offscreenRTHeight = screenHeight / this.zoom / 2 * 2;
			}
			if (this.useOffscreenRT)
			{
				this.pixelRect = new Rect(0f, 0f, (float)this.offscreenRTWidth, (float)this.offscreenRTHeight);
			}
			else
			{
				this.pixelRect = Rect.zero;
			}
			if (cropFrameY)
			{
				this.orthoSize = (float)refResolutionY * 0.5f / (float)assetsPPU;
			}
			else if (cropFrameX)
			{
				float num = (this.pixelRect == Rect.zero) ? ((float)screenWidth / (float)screenHeight) : (this.pixelRect.width / this.pixelRect.height);
				this.orthoSize = (float)refResolutionX / num * 0.5f / (float)assetsPPU;
			}
			else if (upscaleRT && this.zoom > 1)
			{
				this.orthoSize = (float)this.offscreenRTHeight * 0.5f / (float)assetsPPU;
			}
			else
			{
				float num2 = (this.pixelRect == Rect.zero) ? ((float)screenHeight) : this.pixelRect.height;
				this.orthoSize = num2 * 0.5f / (float)(this.zoom * assetsPPU);
			}
			if (upscaleRT || (!upscaleRT && pixelSnapping))
			{
				this.unitsPerPixel = 1f / (float)assetsPPU;
				return;
			}
			this.unitsPerPixel = 1f / (float)(this.zoom * assetsPPU);
		}

		internal Rect CalculateFinalBlitPixelRect(int screenWidth, int screenHeight)
		{
			Rect result = default(Rect);
			if (this.useStretchFill)
			{
				float num = (float)screenWidth / (float)screenHeight;
				float num2 = (float)this.m_Component.refResolutionX / (float)this.m_Component.refResolutionY;
				if (num > num2)
				{
					result.height = (float)screenHeight;
					result.width = (float)screenHeight * num2;
					result.x = (float)((screenWidth - (int)result.width) / 2);
					result.y = 0f;
				}
				else
				{
					result.width = (float)screenWidth;
					result.height = (float)screenWidth / num2;
					result.y = (float)((screenHeight - (int)result.height) / 2);
					result.x = 0f;
				}
				if (screenWidth % this.m_Component.refResolutionX == 0)
				{
					this.requiresUpscaling = (num2 < num);
				}
				else if (screenHeight % this.m_Component.refResolutionY == 0)
				{
					this.requiresUpscaling = (num2 > num);
				}
			}
			else
			{
				if (this.m_Component.upscaleRT)
				{
					result.height = (float)(this.zoom * this.offscreenRTHeight);
					result.width = (float)(this.zoom * this.offscreenRTWidth);
				}
				else
				{
					result.height = (float)this.offscreenRTHeight;
					result.width = (float)this.offscreenRTWidth;
				}
				result.x = (float)((screenWidth - (int)result.width) / 2);
				result.y = (float)((screenHeight - (int)result.height) / 2);
			}
			return result;
		}

		internal float CorrectCinemachineOrthoSize(float targetOrthoSize)
		{
			float result;
			if (this.m_Component.upscaleRT)
			{
				this.cinemachineVCamZoom = Math.Max(1, Mathf.RoundToInt(this.orthoSize / targetOrthoSize));
				result = this.orthoSize / (float)this.cinemachineVCamZoom;
			}
			else
			{
				this.cinemachineVCamZoom = Math.Max(1, Mathf.RoundToInt((float)this.zoom * this.orthoSize / targetOrthoSize));
				result = (float)this.zoom * this.orthoSize / (float)this.cinemachineVCamZoom;
			}
			if (!this.m_Component.upscaleRT && !this.m_Component.pixelSnapping)
			{
				this.unitsPerPixel = 1f / (float)(this.cinemachineVCamZoom * this.m_Component.assetsPPU);
			}
			return result;
		}

		[NonSerialized]
		private IPixelPerfectCamera m_Component;

		private PixelPerfectCamera m_SerializableComponent;

		internal float originalOrthoSize;

		internal bool hasPostProcessLayer;

		internal bool cropFrameXAndY;

		internal bool cropFrameXOrY;

		internal bool useStretchFill;

		internal int zoom = 1;

		internal bool useOffscreenRT;

		internal int offscreenRTWidth;

		internal int offscreenRTHeight;

		internal Rect pixelRect = Rect.zero;

		internal float orthoSize = 1f;

		internal float unitsPerPixel;

		internal int cinemachineVCamZoom = 1;

		internal bool requiresUpscaling;
	}
}
