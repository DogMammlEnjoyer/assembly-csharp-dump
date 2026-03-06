using System;
using System.Diagnostics;

namespace UnityEngine.Rendering
{
	[DebuggerDisplay("{m_Value} ({m_OverrideState})")]
	[Serializable]
	public class MaxFloatParameter : FloatParameter
	{
		public override float value
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

		public MaxFloatParameter(float value, float max, bool overrideState = false) : base(value, overrideState)
		{
			this.max = max;
		}

		[NonSerialized]
		public float max;
	}
}
