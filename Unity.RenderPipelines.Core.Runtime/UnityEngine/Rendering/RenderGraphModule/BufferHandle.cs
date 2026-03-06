using System;
using System.Diagnostics;
using UnityEngine.Scripting.APIUpdating;

namespace UnityEngine.Rendering.RenderGraphModule
{
	[DebuggerDisplay("Buffer ({handle.index})")]
	[MovedFrom(true, "UnityEngine.Experimental.Rendering.RenderGraphModule", "UnityEngine.Rendering.RenderGraphModule", null)]
	public struct BufferHandle
	{
		public static BufferHandle nullHandle
		{
			get
			{
				return BufferHandle.s_NullHandle;
			}
		}

		internal BufferHandle(in ResourceHandle h)
		{
			this.handle = h;
		}

		internal BufferHandle(int handle, bool shared = false)
		{
			this.handle = new ResourceHandle(handle, RenderGraphResourceType.Buffer, shared);
		}

		public static implicit operator GraphicsBuffer(BufferHandle buffer)
		{
			if (!buffer.IsValid())
			{
				return null;
			}
			return RenderGraphResourceRegistry.current.GetBuffer(buffer);
		}

		public bool IsValid()
		{
			return this.handle.IsValid();
		}

		private static BufferHandle s_NullHandle;

		internal ResourceHandle handle;
	}
}
