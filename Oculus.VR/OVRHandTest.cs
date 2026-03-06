using System;
using System.Collections.Generic;
using System.Text;
using Meta.XR.Util;
using UnityEngine;
using UnityEngine.UI;

[Feature(Feature.Hands)]
public class OVRHandTest : MonoBehaviour
{
	private void Start()
	{
		if (this.uiText != null)
		{
			this.uiText.supportRichText = false;
		}
		this.data = new StringBuilder(2048);
		List<OVRHandTest.BoolMonitor> list = new List<OVRHandTest.BoolMonitor>();
		list.Add(new OVRHandTest.BoolMonitor("One", () => OVRInput.Get(OVRInput.Button.One, OVRInput.Controller.Active), 0.5f));
		this.monitors = list;
		this.result_skel_LH = OVRPlugin.GetSkeleton(OVRPlugin.SkeletonType.HandLeft, out this.skel_LH);
		this.result_skel_RH = OVRPlugin.GetSkeleton(OVRPlugin.SkeletonType.HandRight, out this.skel_RH);
		this.result_mesh_LH = OVRPlugin.GetMesh(OVRPlugin.MeshType.HandLeft, out this.mesh_LH);
		this.result_mesh_RH = OVRPlugin.GetMesh(OVRPlugin.MeshType.HandRight, out this.mesh_RH);
	}

	private void Update()
	{
		this.data.Length = 0;
		OVRInput.Controller activeController = OVRInput.GetActiveController();
		string arg = activeController.ToString();
		this.data.AppendFormat("Active: {0}\n", arg);
		string arg2 = OVRInput.GetConnectedControllers().ToString();
		this.data.AppendFormat("Connected: {0}\n", arg2);
		this.data.AppendFormat("PrevConnected: {0}\n", OVRHandTest.prevConnected);
		OVRHandTest.controllers.Update();
		OVRHandTest.controllers.AppendToStringBuilder(ref this.data);
		OVRHandTest.prevConnected = arg2;
		Vector3 localControllerPosition = OVRInput.GetLocalControllerPosition(activeController);
		this.data.AppendFormat("Position: ({0:F2}, {1:F2}, {2:F2})\n", localControllerPosition.x, localControllerPosition.y, localControllerPosition.z);
		Quaternion localControllerRotation = OVRInput.GetLocalControllerRotation(activeController);
		this.data.AppendFormat("Orientation: ({0:F2}, {1:F2}, {2:F2}, {3:F2})\n", new object[]
		{
			localControllerRotation.x,
			localControllerRotation.y,
			localControllerRotation.z,
			localControllerRotation.w
		});
		this.data.AppendFormat("HandTrackingEnabled: {0}\n", OVRPlugin.GetHandTrackingEnabled());
		bool handState = OVRPlugin.GetHandState(OVRPlugin.Step.Render, OVRPlugin.Hand.HandLeft, ref this.hs_LH);
		this.data.AppendFormat("LH HS Query Res: {0}\n", handState);
		this.data.AppendFormat("LH HS Status: {0}\n", this.hs_LH.Status);
		this.data.AppendFormat("LH HS Pose: {0}\n", this.hs_LH.RootPose);
		this.data.AppendFormat("LH HS HandConf: {0}\n", this.hs_LH.HandConfidence);
		bool handState2 = OVRPlugin.GetHandState(OVRPlugin.Step.Render, OVRPlugin.Hand.HandRight, ref this.hs_RH);
		this.data.AppendFormat("RH HS Query Res: {0}\n", handState2);
		this.data.AppendFormat("RH HS Status: {0}\n", this.hs_RH.Status);
		this.data.AppendFormat("RH HS Pose: {0}\n", this.hs_RH.RootPose);
		this.data.AppendFormat("RH HS HandConf: {0}\n", this.hs_RH.HandConfidence);
		this.data.AppendFormat("LH Skel Query Res: {0}\n", this.result_skel_LH);
		this.data.AppendFormat("LH Skel Type: {0}\n", this.skel_LH.Type);
		this.data.AppendFormat("LH Skel NumBones: {0}\n", this.skel_LH.NumBones);
		this.data.AppendFormat("RH Skel Query Res: {0}\n", this.result_skel_RH);
		this.data.AppendFormat("RH Skel Type: {0}\n", this.skel_RH.Type);
		this.data.AppendFormat("RH Skel NumBones: {0}\n", this.skel_RH.NumBones);
		this.data.AppendFormat("LH Mesh Query Res: {0}\n", this.result_mesh_LH);
		this.data.AppendFormat("LH Mesh Type: {0}\n", this.mesh_LH.Type);
		this.data.AppendFormat("LH Mesh NumVers: {0}\n", this.mesh_LH.NumVertices);
		this.data.AppendFormat("RH Mesh Query Res: {0}\n", this.result_mesh_RH);
		this.data.AppendFormat("RH Mesh Type: {0}\n", this.mesh_RH.Type);
		this.data.AppendFormat("RH Mesh NumVers: {0}\n", this.mesh_RH.NumVertices);
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

	private List<OVRHandTest.BoolMonitor> monitors;

	private StringBuilder data;

	private OVRPlugin.HandState hs_LH;

	private OVRPlugin.HandState hs_RH;

	private OVRPlugin.Skeleton skel_LH;

	private OVRPlugin.Skeleton skel_RH;

	private OVRPlugin.Mesh mesh_LH = new OVRPlugin.Mesh();

	private OVRPlugin.Mesh mesh_RH = new OVRPlugin.Mesh();

	private bool result_skel_LH;

	private bool result_skel_RH;

	private bool result_mesh_LH;

	private bool result_mesh_RH;

	private static string prevConnected = "";

	private static OVRHandTest.BoolMonitor controllers = new OVRHandTest.BoolMonitor("Controllers Changed", () => OVRInput.GetConnectedControllers().ToString() != OVRHandTest.prevConnected, 0.5f);

	public class BoolMonitor
	{
		public BoolMonitor(string name, OVRHandTest.BoolMonitor.BoolGenerator generator, float displayTimeout = 0.5f)
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

		private OVRHandTest.BoolMonitor.BoolGenerator m_generator;

		private bool m_prevValue;

		private bool m_currentValue;

		private bool m_currentValueRecentlyChanged;

		private float m_displayTimeout;

		private float m_displayTimer;

		public delegate bool BoolGenerator();
	}
}
