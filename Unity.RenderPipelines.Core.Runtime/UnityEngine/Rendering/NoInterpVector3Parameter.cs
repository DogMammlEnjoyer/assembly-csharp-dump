using System;
using System.Diagnostics;

namespace UnityEngine.Rendering
{
	[DebuggerDisplay("{m_Value} ({m_OverrideState})")]
	[Serializable]
	public class NoInterpVector3Parameter : VolumeParameter<Vector3>
	{
		public NoInterpVector3Parameter(Vector3 value, bool overrideState = false) : base(value, overrideState)
		{
		}
	}
}
