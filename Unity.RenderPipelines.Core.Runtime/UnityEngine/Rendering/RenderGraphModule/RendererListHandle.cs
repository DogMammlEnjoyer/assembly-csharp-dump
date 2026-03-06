using System;
using System.Diagnostics;
using UnityEngine.Scripting.APIUpdating;

namespace UnityEngine.Rendering.RenderGraphModule
{
	[DebuggerDisplay("RendererList ({handle})")]
	[MovedFrom(true, "UnityEngine.Experimental.Rendering.RenderGraphModule", "UnityEngine.Rendering.RenderGraphModule", null)]
	public struct RendererListHandle
	{
		internal int handle { readonly get; private set; }

		internal RendererListHandle(int handle, RendererListHandleType type = RendererListHandleType.Renderers)
		{
			this.handle = handle;
			this.m_IsValid = true;
			this.type = type;
		}

		public static implicit operator int(RendererListHandle handle)
		{
			return handle.handle;
		}

		public static implicit operator RendererList(RendererListHandle rendererList)
		{
			if (!rendererList.IsValid())
			{
				return RendererList.nullRendererList;
			}
			return RenderGraphResourceRegistry.current.GetRendererList(rendererList);
		}

		public bool IsValid()
		{
			return this.m_IsValid;
		}

		internal RendererListHandleType type;

		private bool m_IsValid;
	}
}
