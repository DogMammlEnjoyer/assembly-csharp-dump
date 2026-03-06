using System;
using System.Diagnostics;

namespace UnityEngine.Rendering
{
	[DebuggerDisplay("{m_Value} ({m_OverrideState})")]
	[Serializable]
	public class RenderingLayerMaskParameter : VolumeParameter<RenderingLayerMask>
	{
		public RenderingLayerMaskParameter(RenderingLayerMask value, bool overrideState = false) : base(value, overrideState)
		{
		}
	}
}
