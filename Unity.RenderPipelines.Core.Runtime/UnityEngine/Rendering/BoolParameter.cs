using System;
using System.Diagnostics;

namespace UnityEngine.Rendering
{
	[DebuggerDisplay("{m_Value} ({m_OverrideState})")]
	[Serializable]
	public class BoolParameter : VolumeParameter<bool>
	{
		public BoolParameter(bool value, bool overrideState = false) : base(value, overrideState)
		{
		}

		public BoolParameter(bool value, BoolParameter.DisplayType displayType, bool overrideState = false) : base(value, overrideState)
		{
			this.displayType = displayType;
		}

		[NonSerialized]
		public BoolParameter.DisplayType displayType;

		public enum DisplayType
		{
			Checkbox,
			EnumPopup
		}
	}
}
