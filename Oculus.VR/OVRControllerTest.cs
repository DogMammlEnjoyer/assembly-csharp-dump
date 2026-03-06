using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

public class OVRControllerTest : MonoBehaviour
{
	private void Start()
	{
		if (this.uiText != null)
		{
			this.uiText.supportRichText = false;
		}
		this.data = new StringBuilder(2048);
		List<OVRControllerTest.BoolMonitor> list = new List<OVRControllerTest.BoolMonitor>();
		list.Add(new OVRControllerTest.BoolMonitor("One", () => OVRInput.Get(OVRInput.Button.One, OVRInput.Controller.Active), 0.5f));
		list.Add(new OVRControllerTest.BoolMonitor("OneDown", () => OVRInput.GetDown(OVRInput.Button.One, OVRInput.Controller.Active), 0.5f));
		list.Add(new OVRControllerTest.BoolMonitor("OneUp", () => OVRInput.GetUp(OVRInput.Button.One, OVRInput.Controller.Active), 0.5f));
		list.Add(new OVRControllerTest.BoolMonitor("One (Touch)", () => OVRInput.Get(OVRInput.Touch.One, OVRInput.Controller.Active), 0.5f));
		list.Add(new OVRControllerTest.BoolMonitor("OneDown (Touch)", () => OVRInput.GetDown(OVRInput.Touch.One, OVRInput.Controller.Active), 0.5f));
		list.Add(new OVRControllerTest.BoolMonitor("OneUp (Touch)", () => OVRInput.GetUp(OVRInput.Touch.One, OVRInput.Controller.Active), 0.5f));
		list.Add(new OVRControllerTest.BoolMonitor("Two", () => OVRInput.Get(OVRInput.Button.Two, OVRInput.Controller.Active), 0.5f));
		list.Add(new OVRControllerTest.BoolMonitor("TwoDown", () => OVRInput.GetDown(OVRInput.Button.Two, OVRInput.Controller.Active), 0.5f));
		list.Add(new OVRControllerTest.BoolMonitor("TwoUp", () => OVRInput.GetUp(OVRInput.Button.Two, OVRInput.Controller.Active), 0.5f));
		list.Add(new OVRControllerTest.BoolMonitor("PrimaryIndexTrigger", () => OVRInput.Get(OVRInput.Button.PrimaryIndexTrigger, OVRInput.Controller.Active), 0.5f));
		list.Add(new OVRControllerTest.BoolMonitor("PrimaryIndexTriggerDown", () => OVRInput.GetDown(OVRInput.Button.PrimaryIndexTrigger, OVRInput.Controller.Active), 0.5f));
		list.Add(new OVRControllerTest.BoolMonitor("PrimaryIndexTriggerUp", () => OVRInput.GetUp(OVRInput.Button.PrimaryIndexTrigger, OVRInput.Controller.Active), 0.5f));
		list.Add(new OVRControllerTest.BoolMonitor("PrimaryIndexTrigger (Touch)", () => OVRInput.Get(OVRInput.Touch.PrimaryIndexTrigger, OVRInput.Controller.Active), 0.5f));
		list.Add(new OVRControllerTest.BoolMonitor("PrimaryIndexTriggerDown (Touch)", () => OVRInput.GetDown(OVRInput.Touch.PrimaryIndexTrigger, OVRInput.Controller.Active), 0.5f));
		list.Add(new OVRControllerTest.BoolMonitor("PrimaryIndexTriggerUp (Touch)", () => OVRInput.GetUp(OVRInput.Touch.PrimaryIndexTrigger, OVRInput.Controller.Active), 0.5f));
		list.Add(new OVRControllerTest.BoolMonitor("PrimaryHandTrigger", () => OVRInput.Get(OVRInput.Button.PrimaryHandTrigger, OVRInput.Controller.Active), 0.5f));
		list.Add(new OVRControllerTest.BoolMonitor("PrimaryHandTriggerDown", () => OVRInput.GetDown(OVRInput.Button.PrimaryHandTrigger, OVRInput.Controller.Active), 0.5f));
		list.Add(new OVRControllerTest.BoolMonitor("PrimaryHandTriggerUp", () => OVRInput.GetUp(OVRInput.Button.PrimaryHandTrigger, OVRInput.Controller.Active), 0.5f));
		list.Add(new OVRControllerTest.BoolMonitor("Up", () => OVRInput.Get(OVRInput.Button.Up, OVRInput.Controller.Active), 0.5f));
		list.Add(new OVRControllerTest.BoolMonitor("Down", () => OVRInput.Get(OVRInput.Button.Down, OVRInput.Controller.Active), 0.5f));
		list.Add(new OVRControllerTest.BoolMonitor("Left", () => OVRInput.Get(OVRInput.Button.Left, OVRInput.Controller.Active), 0.5f));
		list.Add(new OVRControllerTest.BoolMonitor("Right", () => OVRInput.Get(OVRInput.Button.Right, OVRInput.Controller.Active), 0.5f));
		list.Add(new OVRControllerTest.BoolMonitor("Start", () => OVRInput.Get(OVRInput.RawButton.Start, OVRInput.Controller.Active), 0.5f));
		list.Add(new OVRControllerTest.BoolMonitor("StartDown", () => OVRInput.GetDown(OVRInput.RawButton.Start, OVRInput.Controller.Active), 0.5f));
		list.Add(new OVRControllerTest.BoolMonitor("StartUp", () => OVRInput.GetUp(OVRInput.RawButton.Start, OVRInput.Controller.Active), 0.5f));
		list.Add(new OVRControllerTest.BoolMonitor("Back", () => OVRInput.Get(OVRInput.RawButton.Back, OVRInput.Controller.Active), 0.5f));
		list.Add(new OVRControllerTest.BoolMonitor("BackDown", () => OVRInput.GetDown(OVRInput.RawButton.Back, OVRInput.Controller.Active), 0.5f));
		list.Add(new OVRControllerTest.BoolMonitor("BackUp", () => OVRInput.GetUp(OVRInput.RawButton.Back, OVRInput.Controller.Active), 0.5f));
		list.Add(new OVRControllerTest.BoolMonitor("A", () => OVRInput.Get(OVRInput.RawButton.A, OVRInput.Controller.Active), 0.5f));
		list.Add(new OVRControllerTest.BoolMonitor("ADown", () => OVRInput.GetDown(OVRInput.RawButton.A, OVRInput.Controller.Active), 0.5f));
		list.Add(new OVRControllerTest.BoolMonitor("AUp", () => OVRInput.GetUp(OVRInput.RawButton.A, OVRInput.Controller.Active), 0.5f));
		this.monitors = list;
	}

	private void Update()
	{
		OVRInput.Controller activeController = OVRInput.GetActiveController();
		this.data.Length = 0;
		byte controllerBatteryPercentRemaining = OVRInput.GetControllerBatteryPercentRemaining(OVRInput.Controller.Active);
		this.data.AppendFormat("Battery: {0}\n", controllerBatteryPercentRemaining);
		float appFramerate = OVRPlugin.GetAppFramerate();
		this.data.AppendFormat("Framerate: {0:F2}\n", appFramerate);
		string arg = activeController.ToString();
		this.data.AppendFormat("Active: {0}\n", arg);
		string arg2 = OVRInput.GetConnectedControllers().ToString();
		this.data.AppendFormat("Connected: {0}\n", arg2);
		this.data.AppendFormat("PrevConnected: {0}\n", OVRControllerTest.prevConnected);
		OVRControllerTest.controllers.Update();
		OVRControllerTest.controllers.AppendToStringBuilder(ref this.data);
		OVRControllerTest.prevConnected = arg2;
		Quaternion localControllerRotation = OVRInput.GetLocalControllerRotation(activeController);
		this.data.AppendFormat("Orientation: ({0:F2}, {1:F2}, {2:F2}, {3:F2})\n", new object[]
		{
			localControllerRotation.x,
			localControllerRotation.y,
			localControllerRotation.z,
			localControllerRotation.w
		});
		Vector3 localControllerAngularVelocity = OVRInput.GetLocalControllerAngularVelocity(activeController);
		this.data.AppendFormat("AngVel: ({0:F2}, {1:F2}, {2:F2})\n", localControllerAngularVelocity.x, localControllerAngularVelocity.y, localControllerAngularVelocity.z);
		Vector3 localControllerAngularAcceleration = OVRInput.GetLocalControllerAngularAcceleration(activeController);
		this.data.AppendFormat("AngAcc: ({0:F2}, {1:F2}, {2:F2})\n", localControllerAngularAcceleration.x, localControllerAngularAcceleration.y, localControllerAngularAcceleration.z);
		Vector3 localControllerPosition = OVRInput.GetLocalControllerPosition(activeController);
		this.data.AppendFormat("Position: ({0:F2}, {1:F2}, {2:F2})\n", localControllerPosition.x, localControllerPosition.y, localControllerPosition.z);
		Vector3 localControllerVelocity = OVRInput.GetLocalControllerVelocity(activeController);
		this.data.AppendFormat("Vel: ({0:F2}, {1:F2}, {2:F2})\n", localControllerVelocity.x, localControllerVelocity.y, localControllerVelocity.z);
		Vector3 localControllerAcceleration = OVRInput.GetLocalControllerAcceleration(activeController);
		this.data.AppendFormat("Acc: ({0:F2}, {1:F2}, {2:F2})\n", localControllerAcceleration.x, localControllerAcceleration.y, localControllerAcceleration.z);
		float num = OVRInput.Get(OVRInput.Axis1D.PrimaryIndexTrigger, OVRInput.Controller.Active);
		this.data.AppendFormat("PrimaryIndexTriggerAxis1D: ({0:F2})\n", num);
		float num2 = OVRInput.Get(OVRInput.Axis1D.PrimaryHandTrigger, OVRInput.Controller.Active);
		this.data.AppendFormat("PrimaryHandTriggerAxis1D: ({0:F2})\n", num2);
		for (int i = 0; i < this.monitors.Count; i++)
		{
			this.monitors[i].Update();
			this.monitors[i].AppendToStringBuilder(ref this.data);
		}
		if (this.uiText != null)
		{
			this.uiText.text = this.data.ToString();
		}
	}

	public Text uiText;

	private List<OVRControllerTest.BoolMonitor> monitors;

	private StringBuilder data;

	private static string prevConnected = "";

	private static OVRControllerTest.BoolMonitor controllers = new OVRControllerTest.BoolMonitor("Controllers Changed", () => OVRInput.GetConnectedControllers().ToString() != OVRControllerTest.prevConnected, 0.5f);

	public class BoolMonitor
	{
		public BoolMonitor(string name, OVRControllerTest.BoolMonitor.BoolGenerator generator, float displayTimeout = 0.5f)
		{
			this.m_name = name;
			this.m_generator = generator;
			this.m_displayTimeout = displayTimeout;
		}

		public void Update()
		{
			this.m_prevValue = this.m_currentValue;
			this.m_currentValue = this.m_generator();
			if (this.m_currentValue != this.m_prevValue)
			{
				this.m_currentValueRecentlyChanged = true;
				this.m_displayTimer = this.m_displayTimeout;
			}
			if (this.m_displayTimer > 0f)
			{
				this.m_displayTimer -= Time.deltaTime;
				if (this.m_displayTimer <= 0f)
				{
					this.m_currentValueRecentlyChanged = false;
					this.m_displayTimer = 0f;
				}
			}
		}

		public void AppendToStringBuilder(ref StringBuilder sb)
		{
			sb.Append(this.m_name);
			if (this.m_currentValue && this.m_currentValueRecentlyChanged)
			{
				sb.Append(": *True*\n");
				return;
			}
			if (this.m_currentValue)
			{
				sb.Append(":  True \n");
				return;
			}
			if (!this.m_currentValue && this.m_currentValueRecentlyChanged)
			{
				sb.Append(": *False*\n");
				return;
			}
			if (!this.m_currentValue)
			{
				sb.Append(":  False \n");
			}
		}

		private string m_name = "";

		private OVRControllerTest.BoolMonitor.BoolGenerator m_generator;

		private bool m_prevValue;

		private bool m_currentValue;

		private bool m_currentValueRecentlyChanged;

		private float m_displayTimeout;

		private float m_displayTimer;

		public delegate bool BoolGenerator();
	}
}
