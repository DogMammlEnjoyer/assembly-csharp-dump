using System;
using System.Text;

namespace UnityEngine.Rendering.RenderGraphModule.NativeRenderPassCompiler
{
	internal readonly struct Name
	{
		public Name(string name, bool computeUTF8ByteCount = false)
		{
			this.name = name;
			this.utf8ByteCount = ((name != null && name.Length > 0 && computeUTF8ByteCount) ? Encoding.UTF8.GetByteCount(name) : 0);
		}

		public readonly string name;

		public readonly int utf8ByteCount;
	}
}
