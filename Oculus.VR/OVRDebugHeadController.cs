using System;
using UnityEngine;
using UnityEngine.XR;

[HelpURL("https://developer.oculus.com/reference/unity/latest/class_o_v_r_debug_head_controller")]
public class OVRDebugHeadController : MonoBehaviour
{
	private void Awake()
	{
		OVRCameraRig[] componentsInChildren = base.gameObject.GetComponentsInChildren<OVRCameraRig>();
		if (componentsInChildren.Length == 0)
		{
			Debug.LogWarning("OVRCamParent: No OVRCameraRig attached.");
			return;
		}
		if (componentsInChildren.Length > 1)
		{
			Debug.LogWarning("OVRCamParent: More then 1 OVRCameraRig attached.");
			return;
		}
		this.CameraRig = componentsInChildren[0];
	}

	private void Start()
	{
	}

	private void Update()
	{
		if (this.AllowMovement)
		{
			float y = OVRInput.Get(OVRInput.RawAxis2D.LThumbstick, OVRInput.Controller.Active).y;
			float x = OVRInput.Get(OVRInput.RawAxis2D.LThumbstick, OVRInput.Controller.Active).x;
			Vector3 a = this.CameraRig.centerEyeAnchor.rotation * Vector3.forward * y * Time.deltaTime * this.ForwardSpeed;
			Vector3 b = this.CameraRig.centerEyeAnchor.rotation * Vector3.right * x * Time.deltaTime * this.StrafeSpeed;
			base.transform.position += a + b;
		}
		bool flag = false;
		XRDisplaySubsystem currentDisplaySubsystem = OVRManager.GetCurrentDisplaySubsystem();
		if (currentDisplaySubsystem != null)
		{
			flag = currentDisplaySubsystem.running;
		}
		if (!flag && (this.AllowYawLook || this.AllowPitchLook))
		{
			Quaternion quaternion = base.transform.rotation;
			if (this.AllowYawLook)
			{
				quaternion = Quaternion.AngleAxis(OVRInput.Get(OVRInput.RawAxis2D.RThumbstick, OVRInput.Controller.Active).x * Time.deltaTime * this.GamePad_YawDegreesPerSec, Vector3.up) * quaternion;
			}
			if (this.AllowPitchLook)
			{
				float num = OVRInput.Get(OVRInput.RawAxis2D.RThumbstick, OVRInput.Controller.Active).y;
				if (Mathf.Abs(num) > 0.0001f)
				{
					if (this.InvertPitch)
					{
						num *= -1f;
					}
					Quaternion rhs = Quaternion.AngleAxis(num * Time.deltaTime * this.GamePad_PitchDegreesPerSec, Vector3.left);
					quaternion *= rhs;
				}
			}
			base.transform.rotation = quaternion;
		}
	}

	[SerializeField]
	public bool AllowPitchLook;

	[SerializeField]
	public bool AllowYawLook = true;

	[SerializeField]
	public bool InvertPitch;

	[SerializeField]
	public float GamePad_PitchDegreesPerSec = 90f;

	[SerializeField]
	public float GamePad_YawDegreesPerSec = 90f;

	[SerializeField]
	public bool AllowMovement;

	[SerializeField]
	public float ForwardSpeed = 2f;

	[SerializeField]
	public float StrafeSpeed = 2f;

	protected OVRCameraRig CameraRig;
}
