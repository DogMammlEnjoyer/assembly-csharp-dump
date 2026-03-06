using System;
using Meta.XR.ImmersiveDebugger;
using Meta.XR.ImmersiveDebugger.Gizmo;
using UnityEngine;

[RequireComponent(typeof(OVREyeGaze))]
internal class TestSceneUsage : MonoBehaviour
{
	private void Start()
	{
		this._eyeGazeComponent = base.GetComponent<OVREyeGaze>();
	}

	private void Update()
	{
		this._eyeGazePose.position = base.transform.position;
		this._eyeGazePose.position.z = this._eyeGazePose.position.z + 0.15f;
		this._eyeGazePose.rotation = base.transform.rotation;
		this._eyeGazePosition = this._eyeGazePose.position;
		Vector3 vector = this._eyeGazePosition;
		vector += this._eyeGazePose.rotation * Vector3.forward * 2f;
		this._eyeGazeDirection = Tuple.Create<Vector3, Vector3>(this._eyeGazePosition, vector);
		this._confidence = this._eyeGazeComponent.Confidence;
		DebugGizmos.LineWidth = this.drawingLineWidth;
		if (this.passthroughEnabled != this.previousPassthroughEnabled)
		{
			Object.FindAnyObjectByType<OVRCameraRig>().GetComponent<OVRManager>().isInsightPassthroughEnabled = this.passthroughEnabled;
			this.previousPassthroughEnabled = this.passthroughEnabled;
		}
	}

	[DebugMember(DebugColor.Gray)]
	private void TogglePassthrough()
	{
		TestSceneUsage.TogglePassthroughStatic();
	}

	[DebugMember(DebugColor.Gray)]
	private static void TogglePassthroughStatic()
	{
		OVRManager component = Object.FindAnyObjectByType<OVRCameraRig>().GetComponent<OVRManager>();
		component.isInsightPassthroughEnabled = !component.isInsightPassthroughEnabled;
	}

	private OVREyeGaze _eyeGazeComponent;

	[DebugMember(DebugColor.Gray, GizmoType = DebugGizmoType.Axis)]
	private Pose _eyeGazePose;

	[DebugMember(DebugColor.Gray, GizmoType = DebugGizmoType.Point)]
	private Vector3 _eyeGazePosition;

	[DebugMember(DebugColor.Gray, GizmoType = DebugGizmoType.Line)]
	private Tuple<Vector3, Vector3> _eyeGazeDirection;

	[DebugMember(DebugColor.Gray)]
	private float _confidence;

	[DebugMember(DebugColor.Gray, Tweakable = true, Min = 0.1f, Max = 1f)]
	private float drawingLineWidth = 0.01f;

	[DebugMember(DebugColor.Gray, Tweakable = true)]
	private bool passthroughEnabled = true;

	private bool previousPassthroughEnabled = true;
}
