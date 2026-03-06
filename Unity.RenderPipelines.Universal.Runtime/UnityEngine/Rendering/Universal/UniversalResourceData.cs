using System;
using UnityEngine.Rendering.RenderGraphModule;

namespace UnityEngine.Rendering.Universal
{
	public class UniversalResourceData : UniversalResourceDataBase
	{
		internal UniversalResourceDataBase.ActiveID activeColorID { get; set; }

		public TextureHandle activeColorTexture
		{
			get
			{
				if (!base.CheckAndWarnAboutAccessibility())
				{
					return TextureHandle.nullHandle;
				}
				UniversalResourceDataBase.ActiveID activeColorID = this.activeColorID;
				if (activeColorID == UniversalResourceDataBase.ActiveID.Camera)
				{
					return this.cameraColor;
				}
				if (activeColorID != UniversalResourceDataBase.ActiveID.BackBuffer)
				{
					throw new ArgumentOutOfRangeException();
				}
				return this.backBufferColor;
			}
		}

		internal UniversalResourceDataBase.ActiveID activeDepthID { get; set; }

		public TextureHandle activeDepthTexture
		{
			get
			{
				if (!base.CheckAndWarnAboutAccessibility())
				{
					return TextureHandle.nullHandle;
				}
				UniversalResourceDataBase.ActiveID activeDepthID = this.activeDepthID;
				if (activeDepthID == UniversalResourceDataBase.ActiveID.Camera)
				{
					return this.cameraDepth;
				}
				if (activeDepthID != UniversalResourceDataBase.ActiveID.BackBuffer)
				{
					throw new ArgumentOutOfRangeException();
				}
				return this.backBufferDepth;
			}
		}

		public bool isActiveTargetBackBuffer
		{
			get
			{
				if (!base.isAccessible)
				{
					Debug.LogError("Trying to access frameData outside of the current frame setup.");
					return false;
				}
				return this.activeColorID == UniversalResourceDataBase.ActiveID.BackBuffer;
			}
		}

		public TextureHandle backBufferColor
		{
			get
			{
				return base.CheckAndGetTextureHandle(ref this._backBufferColor);
			}
			internal set
			{
				base.CheckAndSetTextureHandle(ref this._backBufferColor, value);
			}
		}

		public TextureHandle backBufferDepth
		{
			get
			{
				return base.CheckAndGetTextureHandle(ref this._backBufferDepth);
			}
			internal set
			{
				base.CheckAndSetTextureHandle(ref this._backBufferDepth, value);
			}
		}

		public TextureHandle cameraColor
		{
			get
			{
				return base.CheckAndGetTextureHandle(ref this._cameraColor);
			}
			set
			{
				base.CheckAndSetTextureHandle(ref this._cameraColor, value);
			}
		}

		public TextureHandle cameraDepth
		{
			get
			{
				return base.CheckAndGetTextureHandle(ref this._cameraDepth);
			}
			set
			{
				base.CheckAndSetTextureHandle(ref this._cameraDepth, value);
			}
		}

		public TextureHandle mainShadowsTexture
		{
			get
			{
				return base.CheckAndGetTextureHandle(ref this._mainShadowsTexture);
			}
			set
			{
				base.CheckAndSetTextureHandle(ref this._mainShadowsTexture, value);
			}
		}

		public TextureHandle additionalShadowsTexture
		{
			get
			{
				return base.CheckAndGetTextureHandle(ref this._additionalShadowsTexture);
			}
			set
			{
				base.CheckAndSetTextureHandle(ref this._additionalShadowsTexture, value);
			}
		}

		public TextureHandle[] gBuffer
		{
			get
			{
				return base.CheckAndGetTextureHandle(ref this._gBuffer);
			}
			set
			{
				base.CheckAndSetTextureHandle(ref this._gBuffer, value);
			}
		}

		public TextureHandle cameraOpaqueTexture
		{
			get
			{
				return base.CheckAndGetTextureHandle(ref this._cameraOpaqueTexture);
			}
			internal set
			{
				base.CheckAndSetTextureHandle(ref this._cameraOpaqueTexture, value);
			}
		}

		public TextureHandle cameraDepthTexture
		{
			get
			{
				return base.CheckAndGetTextureHandle(ref this._cameraDepthTexture);
			}
			internal set
			{
				base.CheckAndSetTextureHandle(ref this._cameraDepthTexture, value);
			}
		}

		public TextureHandle cameraNormalsTexture
		{
			get
			{
				return base.CheckAndGetTextureHandle(ref this._cameraNormalsTexture);
			}
			internal set
			{
				base.CheckAndSetTextureHandle(ref this._cameraNormalsTexture, value);
			}
		}

		public TextureHandle motionVectorColor
		{
			get
			{
				return base.CheckAndGetTextureHandle(ref this._motionVectorColor);
			}
			set
			{
				base.CheckAndSetTextureHandle(ref this._motionVectorColor, value);
			}
		}

		public TextureHandle motionVectorDepth
		{
			get
			{
				return base.CheckAndGetTextureHandle(ref this._motionVectorDepth);
			}
			set
			{
				base.CheckAndSetTextureHandle(ref this._motionVectorDepth, value);
			}
		}

		public TextureHandle internalColorLut
		{
			get
			{
				return base.CheckAndGetTextureHandle(ref this._internalColorLut);
			}
			set
			{
				base.CheckAndSetTextureHandle(ref this._internalColorLut, value);
			}
		}

		internal TextureHandle debugScreenColor
		{
			get
			{
				return base.CheckAndGetTextureHandle(ref this._debugScreenColor);
			}
			set
			{
				base.CheckAndSetTextureHandle(ref this._debugScreenColor, value);
			}
		}

		internal TextureHandle debugScreenDepth
		{
			get
			{
				return base.CheckAndGetTextureHandle(ref this._debugScreenDepth);
			}
			set
			{
				base.CheckAndSetTextureHandle(ref this._debugScreenDepth, value);
			}
		}

		public TextureHandle afterPostProcessColor
		{
			get
			{
				return base.CheckAndGetTextureHandle(ref this._afterPostProcessColor);
			}
			internal set
			{
				base.CheckAndSetTextureHandle(ref this._afterPostProcessColor, value);
			}
		}

		public TextureHandle overlayUITexture
		{
			get
			{
				return base.CheckAndGetTextureHandle(ref this._overlayUITexture);
			}
			internal set
			{
				base.CheckAndSetTextureHandle(ref this._overlayUITexture, value);
			}
		}

		public TextureHandle renderingLayersTexture
		{
			get
			{
				return base.CheckAndGetTextureHandle(ref this._renderingLayersTexture);
			}
			internal set
			{
				base.CheckAndSetTextureHandle(ref this._renderingLayersTexture, value);
			}
		}

		public TextureHandle[] dBuffer
		{
			get
			{
				return base.CheckAndGetTextureHandle(ref this._dBuffer);
			}
			set
			{
				base.CheckAndSetTextureHandle(ref this._dBuffer, value);
			}
		}

		public TextureHandle dBufferDepth
		{
			get
			{
				return base.CheckAndGetTextureHandle(ref this._dBufferDepth);
			}
			set
			{
				base.CheckAndSetTextureHandle(ref this._dBufferDepth, value);
			}
		}

		public TextureHandle ssaoTexture
		{
			get
			{
				return base.CheckAndGetTextureHandle(ref this._ssaoTexture);
			}
			internal set
			{
				base.CheckAndSetTextureHandle(ref this._ssaoTexture, value);
			}
		}

		internal TextureHandle stpDebugView
		{
			get
			{
				return base.CheckAndGetTextureHandle(ref this._stpDebugView);
			}
			set
			{
				base.CheckAndSetTextureHandle(ref this._stpDebugView, value);
			}
		}

		public override void Reset()
		{
			this._backBufferColor = TextureHandle.nullHandle;
			this._backBufferDepth = TextureHandle.nullHandle;
			this._cameraColor = TextureHandle.nullHandle;
			this._cameraDepth = TextureHandle.nullHandle;
			this._mainShadowsTexture = TextureHandle.nullHandle;
			this._additionalShadowsTexture = TextureHandle.nullHandle;
			this._cameraOpaqueTexture = TextureHandle.nullHandle;
			this._cameraDepthTexture = TextureHandle.nullHandle;
			this._cameraNormalsTexture = TextureHandle.nullHandle;
			this._motionVectorColor = TextureHandle.nullHandle;
			this._motionVectorDepth = TextureHandle.nullHandle;
			this._internalColorLut = TextureHandle.nullHandle;
			this._debugScreenColor = TextureHandle.nullHandle;
			this._debugScreenDepth = TextureHandle.nullHandle;
			this._afterPostProcessColor = TextureHandle.nullHandle;
			this._overlayUITexture = TextureHandle.nullHandle;
			this._renderingLayersTexture = TextureHandle.nullHandle;
			this._dBufferDepth = TextureHandle.nullHandle;
			this._ssaoTexture = TextureHandle.nullHandle;
			this._stpDebugView = TextureHandle.nullHandle;
			for (int i = 0; i < this._gBuffer.Length; i++)
			{
				this._gBuffer[i] = TextureHandle.nullHandle;
			}
			for (int j = 0; j < this._dBuffer.Length; j++)
			{
				this._dBuffer[j] = TextureHandle.nullHandle;
			}
		}

		private TextureHandle _backBufferColor;

		private TextureHandle _backBufferDepth;

		private TextureHandle _cameraColor;

		private TextureHandle _cameraDepth;

		private TextureHandle _mainShadowsTexture;

		private TextureHandle _additionalShadowsTexture;

		private TextureHandle[] _gBuffer = new TextureHandle[7];

		private TextureHandle _cameraOpaqueTexture;

		private TextureHandle _cameraDepthTexture;

		private TextureHandle _cameraNormalsTexture;

		private TextureHandle _motionVectorColor;

		private TextureHandle _motionVectorDepth;

		private TextureHandle _internalColorLut;

		internal TextureHandle _debugScreenColor;

		internal TextureHandle _debugScreenDepth;

		private TextureHandle _afterPostProcessColor;

		private TextureHandle _overlayUITexture;

		private TextureHandle _renderingLayersTexture;

		private TextureHandle[] _dBuffer = new TextureHandle[3];

		private TextureHandle _dBufferDepth;

		private TextureHandle _ssaoTexture;

		private TextureHandle _stpDebugView;
	}
}
