using System;

namespace UnityEngine.XR.OpenXR.NativeTypes
{
	[Flags]
	public enum XrReferenceSpaceType
	{
		View = 1,
		Local = 2,
		Stage = 3,
		UnboundedMsft = 1000038000,
		CombinedEyeVarjo = 1000121000
	}
}
