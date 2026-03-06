using System;
using System.Diagnostics;

namespace UnityEngine.Rendering
{
	[DebuggerDisplay("{m_Value} ({m_OverrideState})")]
	[Serializable]
	public class TextureParameter : VolumeParameter<Texture>
	{
		public TextureParameter(Texture value, bool overrideState = false) : this(value, TextureDimension.Any, overrideState)
		{
		}

		public TextureParameter(Texture value, TextureDimension dimension, bool overrideState = false) : base(value, overrideState)
		{
			this.dimension = dimension;
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

		public TextureDimension dimension;
	}
}
