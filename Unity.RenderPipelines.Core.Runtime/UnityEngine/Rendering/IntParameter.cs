using System;
using System.Diagnostics;

namespace UnityEngine.Rendering
{
	[DebuggerDisplay("{m_Value} ({m_OverrideState})")]
	[Serializable]
	public class IntParameter : VolumeParameter<int>
	{
		public IntParameter(int value, bool overrideState = false) : base(value, overrideState)
		{
		}

		public sealed override void Interp(int from, int to, float t)
		{
			this.m_Value = (int)((float)from + (float)(to - from) * t);
		}
	}
}
