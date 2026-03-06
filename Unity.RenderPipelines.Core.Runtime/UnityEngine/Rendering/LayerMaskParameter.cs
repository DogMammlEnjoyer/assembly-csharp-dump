using System;
using System.Diagnostics;

namespace UnityEngine.Rendering
{
	[DebuggerDisplay("{m_Value} ({m_OverrideState})")]
	[Serializable]
	public class LayerMaskParameter : VolumeParameter<LayerMask>
	{
		public LayerMaskParameter(LayerMask value, bool overrideState = false) : base(value, overrideState)
		{
		}
	}
}
