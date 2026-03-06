using System;
using UnityEngine;

[HelpURL("https://developer.oculus.com/documentation/unity/unity-core-overview/#scripts")]
public class OVRModeParms : MonoBehaviour
{
	private void Start()
	{
		if (!OVRManager.isHmdPresent)
		{
			base.enabled = false;
			return;
		}
		base.InvokeRepeating("TestPowerStateMode", 10f, 10f);
	}

	private void Update()
	{
		if (OVRInput.GetDown(this.resetButton, OVRInput.Controller.Active))
		{
			OVRPlugin.suggestedCpuPerfLevel = OVRPlugin.ProcessorPerformanceLevel.PowerSavings;
			OVRPlugin.suggestedGpuPerfLevel = OVRPlugin.ProcessorPerformanceLevel.SustainedLow;
		}
	}

	private void TestPowerStateMode()
	{
		if (OVRPlugin.powerSaving)
		{
			Debug.Log("POWER SAVE MODE ACTIVATED");
		}
	}

	public OVRInput.RawButton resetButton = OVRInput.RawButton.X;
}
