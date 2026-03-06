using System;

namespace UnityEngine.Rendering.Universal
{
	internal static class TileSizeExtensions
	{
		public static bool IsValid(this TileSize tileSize)
		{
			return tileSize == TileSize._8 || tileSize == TileSize._16 || tileSize == TileSize._32 || tileSize == TileSize._64;
		}
	}
}
