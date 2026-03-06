using System;
using Unity.XR.CoreUtils.Datums;
using UnityEngine.Scripting.APIUpdating;

namespace UnityEngine.XR.Interaction.Toolkit.Locomotion.Climbing
{
	[MovedFrom("UnityEngine.XR.Interaction.Toolkit")]
	[Serializable]
	public class ClimbSettingsDatumProperty : DatumProperty<ClimbSettings, ClimbSettingsDatum>
	{
		public ClimbSettingsDatumProperty(ClimbSettings value) : base(value)
		{
		}

		public ClimbSettingsDatumProperty(ClimbSettingsDatum datum) : base(datum)
		{
		}
	}
}
