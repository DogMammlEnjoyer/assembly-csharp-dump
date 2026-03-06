using System;
using System.Diagnostics;

namespace UnityEngine.Rendering
{
	[DebuggerDisplay("{m_Value} ({m_OverrideState})")]
	[Serializable]
	public class MinIntParameter : IntParameter
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

		public MinIntParameter(int value, int min, bool overrideState = false) : base(value, overrideState)
		{
			this.min = min;
		}

		[NonSerialized]
		public int min;
	}
}
