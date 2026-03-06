using System;
using UnityEngine;

namespace DigitalOpus.MB.Core
{
	[Serializable]
	public class AtlasPackingResult
	{
		public AtlasPackingResult(AtlasPadding[] pds)
		{
			this.padding = pds;
		}

		public void CalcUsedWidthAndHeight()
		{
			float num = 0f;
			float num2 = 0f;
			float num3 = 0f;
			float num4 = 0f;
			for (int i = 0; i < this.rects.Length; i++)
			{
				num3 += (float)this.padding[i].leftRight * 2f;
				num4 += (float)this.padding[i].topBottom * 2f;
				num = Mathf.Max(num, this.rects[i].x + this.rects[i].width);
				num2 = Mathf.Max(num2, this.rects[i].y + this.rects[i].height);
			}
			this.usedW = Mathf.CeilToInt(num * (float)this.atlasX + num3);
			this.usedH = Mathf.CeilToInt(num2 * (float)this.atlasY + num4);
			if (this.usedW > this.atlasX)
			{
				this.usedW = this.atlasX;
			}
			if (this.usedH > this.atlasY)
			{
				this.usedH = this.atlasY;
			}
		}

		public override string ToString()
		{
			return string.Format("numRects: {0}, atlasX: {1} atlasY: {2} usedW: {3} usedH: {4}", new object[]
			{
				this.rects.Length,
				this.atlasX,
				this.atlasY,
				this.usedW,
				this.usedH
			});
		}

		public int atlasX;

		public int atlasY;

		public int usedW;

		public int usedH;

		public Rect[] rects;

		public AtlasPadding[] padding;

		public int[] srcImgIdxs;

		public object data;
	}
}
