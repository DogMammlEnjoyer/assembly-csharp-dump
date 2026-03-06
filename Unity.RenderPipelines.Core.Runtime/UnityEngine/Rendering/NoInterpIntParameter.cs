using System;
using System.Diagnostics;

namespace UnityEngine.Rendering
{
	[DebuggerDisplay("{m_Value} ({m_OverrideState})")]
	[Serializable]
	public class NoInterpIntParameter : VolumeParameter<int>
	{
		public NoInterpIntParameter(int value, bool overrideState = false) : base(value, overrideState)
		{
		}
	}
}
