using System;
using UnityEngine;

[HelpURL("https://developer.oculus.com/documentation/unity/unity-core-overview/#scripts")]
public class OVRChromaticAberration : MonoBehaviour
{
	private void Start()
	{
		OVRManager.instance.chromatic = this.chromatic;
	}

	private void Update()
	{
		if (OVRInput.GetDown(this.toggleButton, OVRInput.Controller.Active))
		{
			this.chromatic = !this.chromatic;
			OVRManager.instance.chromatic = this.chromatic;
		}
	}

	public OVRInput.RawButton toggleButton = OVRInput.RawButton.X;

	private bool chromatic;
}
