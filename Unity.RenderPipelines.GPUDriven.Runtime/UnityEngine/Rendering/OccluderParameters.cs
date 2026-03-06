using System;
using UnityEngine.Rendering.RenderGraphModule;

namespace UnityEngine.Rendering
{
	public struct OccluderParameters
	{
		public OccluderParameters(int viewInstanceID)
		{
			this.viewInstanceID = viewInstanceID;
			this.subviewCount = 1;
			this.depthTexture = TextureHandle.nullHandle;
			this.depthSize = Vector2Int.zero;
			this.depthIsArray = false;
		}

		public int viewInstanceID;

		public int subviewCount;

		public TextureHandle depthTexture;

		public Vector2Int depthSize;

		public bool depthIsArray;
	}
}
