using System;
using Meta.XR.Util;
using UnityEngine;
using UnityEngine.EventSystems;

[Feature(Feature.VirtualKeyboard)]
public class OVRVirtualKeyboardSampleInputHandler : MonoBehaviour
{
	private static float ApplyDeadzone(float value)
	{
		if (value > 0.2f)
		{
			return (value - 0.2f) / 0.8f;
		}
		if (value < -0.2f)
		{
			return (value + 0.2f) / 0.8f;
		}
		return 0f;
	}

	public float AnalogStickX
	{
		get
		{
			return OVRVirtualKeyboardSampleInputHandler.ApplyDeadzone(OVRInput.Get(OVRInput.Axis2D.PrimaryThumbstick, OVRInput.Controller.Active).x + OVRInput.Get(OVRInput.Axis2D.SecondaryThumbstick, OVRInput.Controller.Active).x);
		}
	}

	public float AnalogStickY
	{
		get
		{
			return OVRVirtualKeyboardSampleInputHandler.ApplyDeadzone(OVRInput.Get(OVRInput.Axis2D.PrimaryThumbstick, OVRInput.Controller.Active).y + OVRInput.Get(OVRInput.Axis2D.SecondaryThumbstick, OVRInput.Controller.Active).y);
		}
	}

	public Vector3 InputRayPosition
	{
		get
		{
			return this.inputModule.rayTransform.position;
		}
	}

	public Quaternion InputRayRotation
	{
		get
		{
			OVRInput.Controller? controller = this.interactionDevice_;
			OVRInput.Controller controller2 = OVRInput.Controller.LHand;
			if (!(controller.GetValueOrDefault() == controller2 & controller != null))
			{
				return this.inputModule.rayTransform.rotation;
			}
			return this.inputModule.rayTransform.rotation * Quaternion.Euler(Vector3.forward * 180f);
		}
	}

	private void Start()
	{
		this.rightLinePointer.enabled = (this.leftLinePointer.enabled = false);
		this.linePointerInitialWidth_ = Math.Max(this.rightLinePointer.startWidth, this.leftLinePointer.startWidth);
	}

	private void Update()
	{
		this.UpdateInteractionAnchor();
		this.UpdateLineRenderer();
	}

	private void UpdateLineRenderer()
	{
		if (this.leftLinePointer != null)
		{
			this.leftLinePointer.enabled = false;
			this.UpdateLineRendererFromSource(OVRVirtualKeyboard.InputSource.ControllerLeft);
			this.UpdateLineRendererFromSource(OVRVirtualKeyboard.InputSource.HandLeft);
		}
		if (this.rightLinePointer != null)
		{
			this.rightLinePointer.enabled = false;
			this.UpdateLineRendererFromSource(OVRVirtualKeyboard.InputSource.ControllerRight);
			this.UpdateLineRendererFromSource(OVRVirtualKeyboard.InputSource.HandRight);
		}
	}

	private void UpdateLineRendererFromSource(OVRVirtualKeyboard.InputSource source)
	{
		Transform transform = null;
		switch (source)
		{
		case OVRVirtualKeyboard.InputSource.ControllerLeft:
			transform = ((OVRInput.IsControllerConnected(OVRInput.Controller.LTouch) && (OVRInput.GetControllerIsInHandState(OVRInput.Hand.HandLeft) == OVRInput.ControllerInHandState.NoHand || OVRInput.GetControllerIsInHandState(OVRInput.Hand.HandLeft) == OVRInput.ControllerInHandState.ControllerInHand)) ? this.OVRVirtualKeyboard.leftControllerDirectTransform : null);
			break;
		case OVRVirtualKeyboard.InputSource.ControllerRight:
			transform = ((OVRInput.IsControllerConnected(OVRInput.Controller.RTouch) && (OVRInput.GetControllerIsInHandState(OVRInput.Hand.HandRight) == OVRInput.ControllerInHandState.NoHand || OVRInput.GetControllerIsInHandState(OVRInput.Hand.HandRight) == OVRInput.ControllerInHandState.ControllerInHand)) ? this.OVRVirtualKeyboard.rightControllerDirectTransform : null);
			break;
		case OVRVirtualKeyboard.InputSource.HandLeft:
			transform = (this.OVRVirtualKeyboard.handLeft.IsPointerPoseValid ? this.OVRVirtualKeyboard.handLeft.PointerPose : null);
			break;
		case OVRVirtualKeyboard.InputSource.HandRight:
			transform = (this.OVRVirtualKeyboard.handRight.IsPointerPoseValid ? this.OVRVirtualKeyboard.handRight.PointerPose : null);
			break;
		}
		if (transform == null || transform.position == Vector3.zero)
		{
			return;
		}
		Vector3 position = transform.position;
		LineRenderer lineRenderer = (source == OVRVirtualKeyboard.InputSource.ControllerLeft || source == OVRVirtualKeyboard.InputSource.HandLeft) ? this.leftLinePointer : this.rightLinePointer;
		lineRenderer.startWidth = this.linePointerInitialWidth_;
		if (this.OVRVirtualKeyboard && this.OVRVirtualKeyboard.isActiveAndEnabled && this.OVRVirtualKeyboard.Collider)
		{
			Vector3 vector = this.OVRVirtualKeyboard.transform.InverseTransformPoint(position) * this.OVRVirtualKeyboard.transform.localScale.x;
			Bounds bounds = new Bounds
			{
				size = this.OVRVirtualKeyboard.Collider.bounds.size
			};
			bounds.Expand(Vector3.one * 0.1f);
			if (bounds.Contains(vector))
			{
				lineRenderer.enabled = false;
				return;
			}
			float magnitude = (bounds.ClosestPoint(vector) - vector).magnitude;
			if (magnitude < 0.015f)
			{
				lineRenderer.startWidth = Mathf.Lerp(0f, this.linePointerInitialWidth_, magnitude / 0.015f);
			}
		}
		lineRenderer.endWidth = lineRenderer.startWidth;
		lineRenderer.enabled = true;
		lineRenderer.SetPosition(0, transform.position);
		Ray ray = new Ray(position, transform.rotation * Vector3.forward);
		RaycastHit raycastHit;
		if (this.OVRVirtualKeyboard.Collider && this.OVRVirtualKeyboard.Collider.Raycast(ray, out raycastHit, 100f))
		{
			lineRenderer.SetPosition(1, raycastHit.point);
			return;
		}
		lineRenderer.SetPosition(1, position + ray.direction * 2.5f);
	}

	private void UpdateInteractionAnchor()
	{
		OVRInput.Controller controller = OVRInput.Controller.None;
		controller = ((this.OVRVirtualKeyboard.leftControllerRootTransform != null && OVRInput.GetDown(OVRInput.RawButton.LIndexTrigger, OVRInput.Controller.Active)) ? OVRInput.Controller.LTouch : controller);
		controller = ((this.OVRVirtualKeyboard.rightControllerRootTransform != null && OVRInput.GetDown(OVRInput.RawButton.RIndexTrigger, OVRInput.Controller.Active)) ? OVRInput.Controller.RTouch : controller);
		controller = ((this.OVRVirtualKeyboard.handLeft != null && this.OVRVirtualKeyboard.handLeft.GetFingerIsPinching(OVRHand.HandFinger.Index)) ? OVRInput.Controller.LHand : controller);
		controller = ((this.OVRVirtualKeyboard.handRight != null && this.OVRVirtualKeyboard.handRight.GetFingerIsPinching(OVRHand.HandFinger.Index)) ? OVRInput.Controller.RHand : controller);
		if (controller == OVRInput.Controller.None)
		{
			return;
		}
		bool flag = controller == OVRInput.Controller.LHand || controller == OVRInput.Controller.LTouch;
		this.raycaster.pointer = (flag ? this.OVRVirtualKeyboard.handLeft.gameObject : this.OVRVirtualKeyboard.handRight.gameObject);
		this.interactionDevice_ = new OVRInput.Controller?(controller);
		OVRInputModule ovrinputModule = this.inputModule;
		Transform rayTransform;
		if (controller != OVRInput.Controller.LTouch)
		{
			if (controller != OVRInput.Controller.LHand)
			{
				if (controller != OVRInput.Controller.RHand)
				{
					rayTransform = this.OVRVirtualKeyboard.handRight.transform;
				}
				else
				{
					rayTransform = this.OVRVirtualKeyboard.handRight.PointerPose;
				}
			}
			else
			{
				rayTransform = this.OVRVirtualKeyboard.handLeft.PointerPose;
			}
		}
		else
		{
			rayTransform = this.OVRVirtualKeyboard.handLeft.transform;
		}
		ovrinputModule.rayTransform = rayTransform;
	}

	private const float RAY_MAX_DISTANCE = 100f;

	private const float THUMBSTICK_DEADZONE = 0.2f;

	private const float COLLISION_BOUNDS_ADDED_BLEED_PERCENT = 0.1f;

	private const float LINEPOINTER_THINNING_THRESHOLD = 0.015f;

	public OVRVirtualKeyboard OVRVirtualKeyboard;

	[SerializeField]
	private OVRRaycaster raycaster;

	[SerializeField]
	private OVRInputModule inputModule;

	[SerializeField]
	private LineRenderer leftLinePointer;

	[SerializeField]
	private LineRenderer rightLinePointer;

	private OVRInput.Controller? interactionDevice_;

	private float linePointerInitialWidth_;
}
