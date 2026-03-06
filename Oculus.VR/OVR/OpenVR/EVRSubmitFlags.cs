using System;

namespace OVR.OpenVR
{
	public enum EVRSubmitFlags
	{
		Submit_Default,
		Submit_LensDistortionAlreadyApplied,
		Submit_GlRenderBuffer,
		Submit_Reserved = 4,
		Submit_TextureWithPose = 8,
		Submit_TextureWithDepth = 16
	}
}
