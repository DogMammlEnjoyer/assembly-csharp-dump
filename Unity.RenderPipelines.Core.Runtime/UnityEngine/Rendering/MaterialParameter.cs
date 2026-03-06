using System;
using System.Diagnostics;

namespace UnityEngine.Rendering
{
	[DebuggerDisplay("{m_Value} ({m_OverrideState})")]
	[Serializable]
	public class MaterialParameter : VolumeParameter<Material>
	{
		public MaterialParameter(Material value, bool overrideState = false) : base(value, overrideState)
		{
		}
	}
}
