using System;
using System.Diagnostics;

namespace UnityEngine.Rendering
{
	[DebuggerDisplay("{m_Value} ({m_OverrideState})")]
	[Serializable]
	public class ColorParameter : VolumeParameter<Color>
	{
		public ColorParameter(Color value, bool overrideState = false) : base(value, overrideState)
		{
		}

		public ColorParameter(Color value, bool hdr, bool showAlpha, bool showEyeDropper, bool overrideState = false) : base(value, overrideState)
		{
			this.hdr = hdr;
			this.showAlpha = showAlpha;
			this.showEyeDropper = showEyeDropper;
			this.overrideState = overrideState;
		}

		public override void Interp(Color from, Color to, float t)
		{
			this.m_Value.r = from.r + (to.r - from.r) * t;
			this.m_Value.g = from.g + (to.g - from.g) * t;
			this.m_Value.b = from.b + (to.b - from.b) * t;
			this.m_Value.a = from.a + (to.a - from.a) * t;
		}

		[NonSerialized]
		public bool hdr;

		[NonSerialized]
		public bool showAlpha = true;

		[NonSerialized]
		public bool showEyeDropper = true;
	}
}
