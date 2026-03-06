using System;
using System.Diagnostics;

namespace UnityEngine.Rendering
{
	[DebuggerDisplay("{m_Value} ({m_OverrideState})")]
	[Serializable]
	public class MinFloatParameter : FloatParameter
	{
		public override float value
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

		public MinFloatParameter(float value, float min, bool overrideState = false) : base(value, overrideState)
		{
			this.min = min;
		}

		[NonSerialized]
		public float min;
	}
}
