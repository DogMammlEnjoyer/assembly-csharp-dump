using System;
using Meta.XR.Util;

namespace Oculus.Interaction.Input
{
	[Feature(Feature.Interaction)]
	public static class OVRControllerUtility
	{
		public static float GetPinchAmount(OVRInput.Controller ovrController)
		{
			return OVRInput.Get(OVRInput.Axis1D.PrimaryIndexTrigger, ovrController);
		}

		public static float GetIndexCurl(OVRInput.Controller ovrController)
		{
			float result;
			if (OVRControllerUtility.SupportsAnalogIndex(ovrController))
			{
				result = OVRInput.Get(OVRInput.Axis1D.PrimaryIndexTriggerCurl, ovrController);
			}
			else
			{
				result = (float)((!OVRInput.Get(OVRInput.NearTouch.PrimaryIndexTrigger, ovrController) && OVRInput.Get(OVRInput.Axis1D.PrimaryIndexTrigger, ovrController) == 0f) ? 0 : 1);
			}
			return result;
		}

		public static float GetIndexSlide(OVRInput.Controller ovrController)
		{
			float result;
			if (OVRControllerUtility.SupportsAnalogIndex(ovrController))
			{
				result = OVRInput.Get(OVRInput.Axis1D.PrimaryIndexTriggerSlide, ovrController);
			}
			else
			{
				result = 0f;
			}
			return result;
		}

		private static bool SupportsAnalogIndex(OVRInput.Controller ovrController)
		{
			return (ovrController == OVRInput.Controller.LTouch || ovrController == OVRInput.Controller.RTouch) && OVRInput.GetCurrentInteractionProfile((ovrController == OVRInput.Controller.LTouch) ? OVRInput.Hand.HandLeft : OVRInput.Hand.HandRight) != OVRInput.InteractionProfile.Touch;
		}
	}
}
