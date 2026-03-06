using System;
using System.Diagnostics;

namespace UnityEngine.Rendering
{
	[DebuggerDisplay("{m_Value} ({m_OverrideState})")]
	[Serializable]
	public class ClampedIntParameter : IntParameter
	{
		public override int value
		{
			get
			{
				return this.m_Value;
			}
			set
			{
				this.m_Value = Mathf.Clamp(value, this.min, this.max);
			}
		}

		public ClampedIntParameter(int value, int min, int max, bool overrideState = false) : base(value, overrideState)
		{
			this.min = min;
			this.max = max;
		}

		[NonSerialized]
		public int min;

		[NonSerialized]
		public int max;
	}
}
