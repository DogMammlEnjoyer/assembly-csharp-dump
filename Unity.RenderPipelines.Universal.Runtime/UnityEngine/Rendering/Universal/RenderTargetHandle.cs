using System;

namespace UnityEngine.Rendering.Universal
{
	[Obsolete("Deprecated in favor of RTHandle", true)]
	public struct RenderTargetHandle
	{
		public int id { readonly get; set; }

		private RenderTargetIdentifier rtid { readonly get; set; }

		public RenderTargetHandle(RenderTargetIdentifier renderTargetIdentifier)
		{
			this.id = -2;
			this.rtid = renderTargetIdentifier;
		}

		public RenderTargetHandle(RTHandle rtHandle)
		{
			if (rtHandle.nameID == BuiltinRenderTextureType.CameraTarget)
			{
				this.id = -1;
			}
			else if (rtHandle.name.Length == 0)
			{
				this.id = -2;
			}
			else
			{
				this.id = Shader.PropertyToID(rtHandle.name);
			}
			this.rtid = rtHandle.nameID;
			if (rtHandle.rt != null && this.id != this.rtid)
			{
				this.id = -2;
			}
		}

		internal static RenderTargetHandle GetCameraTarget(ref CameraData cameraData)
		{
			if (cameraData.xr.enabled)
			{
				return new RenderTargetHandle(cameraData.xr.renderTarget);
			}
			return RenderTargetHandle.CameraTarget;
		}

		public void Init(string shaderProperty)
		{
			this.id = Shader.PropertyToID(shaderProperty);
		}

		public void Init(RenderTargetIdentifier renderTargetIdentifier)
		{
			this.id = -2;
			this.rtid = renderTargetIdentifier;
		}

		public RenderTargetIdentifier Identifier()
		{
			if (this.id == -1)
			{
				return BuiltinRenderTextureType.CameraTarget;
			}
			if (this.id == -2)
			{
				return this.rtid;
			}
			return new RenderTargetIdentifier(this.id, 0, CubemapFace.Unknown, -1);
		}

		public bool HasInternalRenderTargetId()
		{
			return this.id == -2;
		}

		public bool Equals(RenderTargetHandle other)
		{
			if (this.id == -2 || other.id == -2)
			{
				return this.Identifier() == other.Identifier();
			}
			return this.id == other.id;
		}

		public override bool Equals(object obj)
		{
			return obj != null && obj is RenderTargetHandle && this.Equals((RenderTargetHandle)obj);
		}

		public override int GetHashCode()
		{
			return this.id;
		}

		public static bool operator ==(RenderTargetHandle c1, RenderTargetHandle c2)
		{
			return c1.Equals(c2);
		}

		public static bool operator !=(RenderTargetHandle c1, RenderTargetHandle c2)
		{
			return !c1.Equals(c2);
		}

		public static readonly RenderTargetHandle CameraTarget = new RenderTargetHandle
		{
			id = -1
		};
	}
}
