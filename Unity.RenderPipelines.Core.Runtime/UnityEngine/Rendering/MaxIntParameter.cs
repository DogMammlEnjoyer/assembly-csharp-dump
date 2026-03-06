using System;
using System.Diagnostics;

namespace UnityEngine.Rendering
{
	[DebuggerDisplay("{m_Value} ({m_OverrideState})")]
	[Serializable]
	public class MaxIntParameter : IntParameter
	{
		public override int value
		{
			get
			{
				return this.m_Value;
			}
			set
			{
				this.m_Value = Mathf.Min(value, this.max);
			}
		}

		public MaxIntParameter(int value, int max, bool overrideState = false) : base(value, overrideState)
		{
			this.max = max;
		}

		[NonSerialized]
		public int max;
	}
}
