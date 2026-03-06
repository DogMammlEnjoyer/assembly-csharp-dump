using System;

namespace UnityEngine.Rendering
{
	[Serializable]
	public class TextureCurveParameter : VolumeParameter<TextureCurve>
	{
		public TextureCurveParameter(TextureCurve value, bool overrideState = false) : base(value, overrideState)
		{
		}

		public override void Release()
		{
			this.m_Value.Release();
		}
	}
}
