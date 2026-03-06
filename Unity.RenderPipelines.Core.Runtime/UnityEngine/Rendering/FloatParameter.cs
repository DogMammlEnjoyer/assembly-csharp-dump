using System;
using System.Diagnostics;

namespace UnityEngine.Rendering
{
	[DebuggerDisplay("{m_Value} ({m_OverrideState})")]
	[Serializable]
	public class FloatParameter : VolumeParameter<float>
	{
		public FloatParameter(float value, bool overrideState = false) : base(value, overrideState)
		{
		}

		public sealed override void Interp(float from, float to, float t)
		{
			this.m_Value = from + (to - from) * t;
		}
	}
}
