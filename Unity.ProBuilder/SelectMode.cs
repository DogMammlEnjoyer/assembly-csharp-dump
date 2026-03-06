using System;

namespace UnityEngine.ProBuilder
{
	[Flags]
	public enum SelectMode
	{
		None = 0,
		Vertex = 1,
		Edge = 2,
		Face = 4,
		TextureVertex = 8,
		TextureEdge = 16,
		TextureFace = 32,
		InputTool = 64,
		Any = 255
	}
}
