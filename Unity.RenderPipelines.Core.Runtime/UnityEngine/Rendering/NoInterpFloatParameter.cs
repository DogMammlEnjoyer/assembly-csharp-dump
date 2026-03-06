using System;
using System.Diagnostics;

namespace UnityEngine.Rendering
{
	[DebuggerDisplay("{m_Value} ({m_OverrideState})")]
	[Serializable]
	public class NoInterpFloatParameter : VolumeParameter<float>
	{
		public NoInterpFloatParameter(float value, bool overrideState = false) : base(value, overrideState)
		{
		}
	}
}
