using System;
using UnityEngine.TextCore.LowLevel;

namespace TMPro
{
	[Serializable]
	public struct GlyphValueRecord_Legacy
	{
		internal GlyphValueRecord_Legacy(GlyphValueRecord valueRecord)
		{
			this.xPlacement = valueRecord.xPlacement;
			this.yPlacement = valueRecord.yPlacement;
			this.xAdvance = valueRecord.xAdvance;
			this.yAdvance = valueRecord.yAdvance;
		}

		public static GlyphValueRecord_Legacy operator +(GlyphValueRecord_Legacy a, GlyphValueRecord_Legacy b)
		{
			GlyphValueRecord_Legacy result;
			result.xPlacement = a.xPlacement + b.xPlacement;
			result.yPlacement = a.yPlacement + b.yPlacement;
			result.xAdvance = a.xAdvance + b.xAdvance;
			result.yAdvance = a.yAdvance + b.yAdvance;
			return result;
		}

		public float xPlacement;

		public float yPlacement;

		public float xAdvance;

		public float yAdvance;
	}
}
