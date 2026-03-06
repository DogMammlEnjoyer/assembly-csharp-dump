using System;
using System.Diagnostics;

namespace UnityEngine.Rendering
{
	[DebuggerDisplay("{m_Value} ({m_OverrideState})")]
	[Serializable]
	public class RenderTextureParameter : VolumeParameter<RenderTexture>
	{
		public RenderTextureParameter(RenderTexture value, bool overrideState = false) : base(value, overrideState)
		{
		}

		public override int GetHashCode()
		{
			int result = base.GetHashCode();
			if (this.value != null)
			{
				result = 23 * CoreUtils.GetTextureHash(this.value);
			}
			return result;
		}
	}
}
