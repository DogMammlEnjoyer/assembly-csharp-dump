using System;
using System.Diagnostics;

namespace UnityEngine.Rendering
{
	[DebuggerDisplay("{m_Value} ({m_OverrideState})")]
	[Serializable]
	public class NoInterpMinIntParameter : VolumeParameter<int>
	{
		public override int value
		{
			get
			{
				return this.m_Value;
			}
			set
			{
				this.m_Value = Mathf.Max(value, this.min);
			}
		}

		public NoInterpMinIntParameter(int value, int min, bool overrideState = false) : base(value, overrideState)
		{
			this.min = min;
		}

		[NonSerialized]
		public int min;
	}
}
