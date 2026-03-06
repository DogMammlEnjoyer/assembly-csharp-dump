using System;
using UnityEngine.Scripting.APIUpdating;

namespace UnityEngine.TerrainTools
{
	[MovedFrom("UnityEngine.Experimental.TerrainAPI")]
	public struct BrushTransform
	{
		public readonly Vector2 brushOrigin { get; }

		public readonly Vector2 brushU { get; }

		public readonly Vector2 brushV { get; }

		public readonly Vector2 targetOrigin { get; }

		public readonly Vector2 targetX { get; }

		public readonly Vector2 targetY { get; }

		public BrushTransform(Vector2 brushOrigin, Vector2 brushU, Vector2 brushV)
		{
			float num = brushU.x * brushV.y - brushU.y * brushV.x;
			float d = Mathf.Approximately(num, 0f) ? 1f : (1f / num);
			Vector2 vector = new Vector2(brushV.y, -brushU.y) * d;
			Vector2 vector2 = new Vector2(-brushV.x, brushU.x) * d;
			Vector2 vector3 = -brushOrigin.x * vector - brushOrigin.y * vector2;
			this.brushOrigin = brushOrigin;
			this.brushU = brushU;
			this.brushV = brushV;
			this.targetOrigin = vector3;
			this.targetX = vector;
			this.targetY = vector2;
		}

		public Rect GetBrushXYBounds()
		{
			Vector2 vector = this.brushOrigin + this.brushU;
			Vector2 vector2 = this.brushOrigin + this.brushV;
			Vector2 vector3 = this.brushOrigin + this.brushU + this.brushV;
			float xmin = Mathf.Min(Mathf.Min(this.brushOrigin.x, vector.x), Mathf.Min(vector2.x, vector3.x));
			float xmax = Mathf.Max(Mathf.Max(this.brushOrigin.x, vector.x), Mathf.Max(vector2.x, vector3.x));
			float ymin = Mathf.Min(Mathf.Min(this.brushOrigin.y, vector.y), Mathf.Min(vector2.y, vector3.y));
			float ymax = Mathf.Max(Mathf.Max(this.brushOrigin.y, vector.y), Mathf.Max(vector2.y, vector3.y));
			return Rect.MinMaxRect(xmin, ymin, xmax, ymax);
		}

		public static BrushTransform FromRect(Rect brushRect)
		{
			Vector2 min = brushRect.min;
			Vector2 brushU = new Vector2(brushRect.width, 0f);
			Vector2 brushV = new Vector2(0f, brushRect.height);
			return new BrushTransform(min, brushU, brushV);
		}

		public Vector2 ToBrushUV(Vector2 targetXY)
		{
			return targetXY.x * this.targetX + targetXY.y * this.targetY + this.targetOrigin;
		}

		public Vector2 FromBrushUV(Vector2 brushUV)
		{
			return brushUV.x * this.brushU + brushUV.y * this.brushV + this.brushOrigin;
		}
	}
}
