using System;

namespace UnityEngine.XR.OpenXR.NativeTypes
{
	public enum XrViewConfigurationType
	{
		PrimaryMono = 1,
		PrimaryStereo,
		PrimaryQuadVarjo = 1000037000,
		SecondaryMonoFirstPersonObserver = 1000054000,
		SecondaryMonoThirdPersonObserver = 1000145000
	}
}
