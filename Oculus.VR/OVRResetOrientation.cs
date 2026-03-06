using System;
using UnityEngine;

[HelpURL("https://developer.oculus.com/documentation/unity/unity-core-overview/#scriptss")]
public class OVRResetOrientation : MonoBehaviour
{
	private void Update()
	{
		if (OVRInput.GetDown(this.resetButton, OVRInput.Controller.Active))
		{
			OVRManager.display.RecenterPose();
		}
	}

	public OVRInput.RawButton resetButton = OVRInput.RawButton.Y;
}
