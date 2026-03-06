using System;
using System.Diagnostics;

namespace UnityEngine.Rendering
{
	[DebuggerDisplay("{m_Value} ({m_OverrideState})")]
	[Serializable]
	public class FloatRangeParameter : VolumeParameter<Vector2>
	{
		public override Vector2 value
		{
			get
			{
				return this.m_Value;
			}
			set
			{
				this.m_Value.x = Mathf.Max(value.x, this.min);
				this.m_Value.y = Mathf.Min(value.y, this.max);
			}
		}

		public FloatRangeParameter(Vector2 value, float min, float max, bool overrideState = false) : base(value, overrideState)
		{
			this.min = min;
			this.max = max;
		}

		public override void Interp(Vector2 from, Vector2 to, float t)
		{
			this.m_Value.x = from.x + (to.x - from.x) * t;
			this.m_Value.y = from.y + (to.y - from.y) * t;
		}

		[NonSerialized]
		public float min;

		[NonSerialized]
		public float max;
	}
}
