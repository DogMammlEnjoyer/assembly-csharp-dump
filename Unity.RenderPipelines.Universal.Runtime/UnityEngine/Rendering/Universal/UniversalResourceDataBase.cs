using System;
using UnityEngine.Rendering.RenderGraphModule;

namespace UnityEngine.Rendering.Universal
{
	public abstract class UniversalResourceDataBase : ContextItem
	{
		internal bool isAccessible { get; set; }

		internal void InitFrame()
		{
			this.isAccessible = true;
		}

		internal void EndFrame()
		{
			this.isAccessible = false;
		}

		protected void CheckAndSetTextureHandle(ref TextureHandle handle, TextureHandle newHandle)
		{
			if (!this.CheckAndWarnAboutAccessibility())
			{
				return;
			}
			handle = newHandle;
		}

		protected TextureHandle CheckAndGetTextureHandle(ref TextureHandle handle)
		{
			if (!this.CheckAndWarnAboutAccessibility())
			{
				return TextureHandle.nullHandle;
			}
			return handle;
		}

		protected void CheckAndSetTextureHandle(ref TextureHandle[] handle, TextureHandle[] newHandle)
		{
			if (!this.CheckAndWarnAboutAccessibility())
			{
				return;
			}
			if (handle == null || handle.Length != newHandle.Length)
			{
				handle = new TextureHandle[newHandle.Length];
			}
			for (int i = 0; i < newHandle.Length; i++)
			{
				handle[i] = newHandle[i];
			}
		}

		protected TextureHandle[] CheckAndGetTextureHandle(ref TextureHandle[] handle)
		{
			if (!this.CheckAndWarnAboutAccessibility())
			{
				return new TextureHandle[]
				{
					TextureHandle.nullHandle
				};
			}
			return handle;
		}

		protected bool CheckAndWarnAboutAccessibility()
		{
			if (!this.isAccessible)
			{
				Debug.LogError("Trying to access Universal Resources outside of the current frame setup.");
			}
			return this.isAccessible;
		}

		internal enum ActiveID
		{
			Camera,
			BackBuffer
		}
	}
}
