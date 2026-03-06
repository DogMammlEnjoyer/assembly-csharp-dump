using System;
using System.Diagnostics;

namespace UnityEngine.Rendering
{
	[DebuggerDisplay("{m_Value} ({m_OverrideState})")]
	[Serializable]
	public class NoInterpMinFloatParameter : VolumeParameter<float>
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

		public NoInterpMinFloatParameter(float value, float min, bool overrideState = false) : base(value, overrideState)
		{
			this.min = min;
		}

		[NonSerialized]
		public float min;
	}
}
