using System;
using UnityEngine.Rendering.RenderGraphModule;

namespace UnityEngine.Rendering.Universal
{
	internal class Universal2DResourceData : UniversalResourceDataBase
	{
		private TextureHandle[][] CheckAndGetTextureHandle(ref TextureHandle[][] handle)
		{
			if (!base.CheckAndWarnAboutAccessibility())
			{
				return new TextureHandle[][]
				{
					new TextureHandle[]
					{
						TextureHandle.nullHandle
					}
				};
			}
			return handle;
		}

		private void CheckAndSetTextureHandle(ref TextureHandle[][] handle, TextureHandle[][] newHandle)
		{
			if (!base.CheckAndWarnAboutAccessibility())
			{
				return;
			}
			if (handle == null || handle.Length != newHandle.Length)
			{
				handle = new TextureHandle[newHandle.Length][];
			}
			for (int i = 0; i < newHandle.Length; i++)
			{
				handle[i] = newHandle[i];
			}
		}

		internal TextureHandle[][] lightTextures
		{
			get
			{
				return this.CheckAndGetTextureHandle(ref this._lightTextures);
			}
			set
			{
				this.CheckAndSetTextureHandle(ref this._lightTextures, value);
			}
		}

		internal TextureHandle[] normalsTexture
		{
			get
			{
				return base.CheckAndGetTextureHandle(ref this._cameraNormalsTexture);
			}
			set
			{
				base.CheckAndSetTextureHandle(ref this._cameraNormalsTexture, value);
			}
		}

		internal TextureHandle normalsDepth
		{
			get
			{
				return base.CheckAndGetTextureHandle(ref this._normalsDepth);
			}
			set
			{
				base.CheckAndSetTextureHandle(ref this._normalsDepth, value);
			}
		}

		internal TextureHandle[][] shadowTextures
		{
			get
			{
				return this.CheckAndGetTextureHandle(ref this._shadowTextures);
			}
			set
			{
				this.CheckAndSetTextureHandle(ref this._shadowTextures, value);
			}
		}

		internal TextureHandle shadowDepth
		{
			get
			{
				return base.CheckAndGetTextureHandle(ref this._shadowDepth);
			}
			set
			{
				base.CheckAndSetTextureHandle(ref this._shadowDepth, value);
			}
		}

		internal TextureHandle upscaleTexture
		{
			get
			{
				return base.CheckAndGetTextureHandle(ref this._upscaleTexture);
			}
			set
			{
				base.CheckAndSetTextureHandle(ref this._upscaleTexture, value);
			}
		}

		internal TextureHandle cameraSortingLayerTexture
		{
			get
			{
				return base.CheckAndGetTextureHandle(ref this._cameraSortingLayerTexture);
			}
			set
			{
				base.CheckAndSetTextureHandle(ref this._cameraSortingLayerTexture, value);
			}
		}

		public override void Reset()
		{
			this._normalsDepth = TextureHandle.nullHandle;
			this._shadowDepth = TextureHandle.nullHandle;
			this._upscaleTexture = TextureHandle.nullHandle;
			this._cameraSortingLayerTexture = TextureHandle.nullHandle;
			for (int i = 0; i < this._cameraNormalsTexture.Length; i++)
			{
				this._cameraNormalsTexture[i] = TextureHandle.nullHandle;
			}
			for (int j = 0; j < this._shadowTextures.Length; j++)
			{
				for (int k = 0; k < this._shadowTextures[j].Length; k++)
				{
					this._shadowTextures[j][k] = TextureHandle.nullHandle;
				}
			}
			for (int l = 0; l < this._lightTextures.Length; l++)
			{
				for (int m = 0; m < this._lightTextures[l].Length; m++)
				{
					this._lightTextures[l][m] = TextureHandle.nullHandle;
				}
			}
		}

		private TextureHandle[][] _lightTextures = new TextureHandle[0][];

		private TextureHandle[] _cameraNormalsTexture = new TextureHandle[0];

		private TextureHandle _normalsDepth;

		private TextureHandle[][] _shadowTextures = new TextureHandle[0][];

		private TextureHandle _shadowDepth;

		private TextureHandle _upscaleTexture;

		private TextureHandle _cameraSortingLayerTexture;
	}
}
