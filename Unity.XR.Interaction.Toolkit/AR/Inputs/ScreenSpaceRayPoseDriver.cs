using System;
using Unity.XR.CoreUtils;
using UnityEngine.XR.Interaction.Toolkit.Inputs.Readers;
using UnityEngine.XR.Interaction.Toolkit.Utilities;

namespace UnityEngine.XR.Interaction.Toolkit.AR.Inputs
{
	[AddComponentMenu("XR/Input/Screen Space Ray Pose Driver", 11)]
	[HelpURL("https://docs.unity3d.com/Packages/com.unity.xr.interaction.toolkit@3.2/api/UnityEngine.XR.Interaction.Toolkit.AR.Inputs.ScreenSpaceRayPoseDriver.html")]
	[DefaultExecutionOrder(-31000)]
	public class ScreenSpaceRayPoseDriver : MonoBehaviour
	{
		public Camera controllerCamera
		{
			get
			{
				return this.m_ControllerCamera;
			}
			set
			{
				this.m_ControllerCamera = value;
			}
		}

		public XRInputValueReader<Vector2> tapStartPositionInput
		{
			get
			{
				return this.m_TapStartPositionInput;
			}
			set
			{
				XRInputReaderUtility.SetInputProperty<Vector2>(ref this.m_TapStartPositionInput, value, this);
			}
		}

		public XRInputValueReader<Vector2> dragStartPositionInput
		{
			get
			{
				return this.m_DragStartPositionInput;
			}
			set
			{
				XRInputReaderUtility.SetInputProperty<Vector2>(ref this.m_DragStartPositionInput, value, this);
			}
		}

		public XRInputValueReader<Vector2> dragCurrentPositionInput
		{
			get
			{
				return this.m_DragCurrentPositionInput;
			}
			set
			{
				XRInputReaderUtility.SetInputProperty<Vector2>(ref this.m_DragCurrentPositionInput, value, this);
			}
		}

		public XRInputValueReader<int> screenTouchCountInput
		{
			get
			{
				return this.m_ScreenTouchCountInput;
			}
			set
			{
				XRInputReaderUtility.SetInputProperty<int>(ref this.m_ScreenTouchCountInput, value, this);
			}
		}

		protected void OnEnable()
		{
			if (this.m_ControllerCamera == null)
			{
				this.m_ControllerCamera = Camera.main;
				if (this.m_ControllerCamera == null)
				{
					Debug.LogWarning("Could not find associated Camera in scene.This ScreenSpaceRayPoseDriver will be disabled.", this);
					base.enabled = false;
					return;
				}
			}
			this.m_TapStartPositionInput.EnableDirectActionIfModeUsed();
			this.m_DragStartPositionInput.EnableDirectActionIfModeUsed();
			this.m_DragCurrentPositionInput.EnableDirectActionIfModeUsed();
			this.m_ScreenTouchCountInput.EnableDirectActionIfModeUsed();
		}

		protected void OnDisable()
		{
			this.m_TapStartPositionInput.DisableDirectActionIfModeUsed();
			this.m_DragStartPositionInput.DisableDirectActionIfModeUsed();
			this.m_DragCurrentPositionInput.DisableDirectActionIfModeUsed();
			this.m_ScreenTouchCountInput.DisableDirectActionIfModeUsed();
		}

		protected void Update()
		{
			Vector2 tapStartPosition = this.m_TapStartPosition;
			bool flag = this.m_TapStartPositionInput.TryReadValue(out this.m_TapStartPosition) && tapStartPosition != this.m_TapStartPosition;
			Vector2 dragStartPosition = this.m_DragStartPosition;
			bool flag2 = this.m_DragStartPositionInput.TryReadValue(out this.m_DragStartPosition) && dragStartPosition != this.m_DragStartPosition;
			int num;
			if (this.m_ScreenTouchCountInput.TryReadValue(out num) && num > 1)
			{
				return;
			}
			if (flag2)
			{
				this.ApplyPose(this.m_DragStartPosition);
				return;
			}
			Vector2 screenPosition;
			if (this.m_DragCurrentPositionInput.TryReadValue(out screenPosition))
			{
				this.ApplyPose(screenPosition);
				return;
			}
			if (flag)
			{
				this.ApplyPose(this.m_TapStartPosition);
				return;
			}
		}

		private void ApplyPose(Vector2 screenPosition)
		{
			Vector3 vector = this.m_ControllerCamera.ScreenToWorldPoint(new Vector3(screenPosition.x, screenPosition.y, this.m_ControllerCamera.nearClipPlane));
			Vector3 normalized = (vector - this.m_ControllerCamera.transform.position).normalized;
			Transform transform;
			Vector3 position = this.m_ParentTransformCache.TryGet(base.transform.parent, out transform) ? transform.InverseTransformPoint(vector) : vector;
			base.transform.SetLocalPose(new Pose(position, Quaternion.LookRotation(normalized)));
		}

		[SerializeField]
		[Tooltip("The camera associated with the screen, and through which screen presses/touches will be interpreted.")]
		private Camera m_ControllerCamera;

		[SerializeField]
		private XRInputValueReader<Vector2> m_TapStartPositionInput = new XRInputValueReader<Vector2>("Tap Start Position", XRInputValueReader.InputSourceMode.InputActionReference);

		[SerializeField]
		private XRInputValueReader<Vector2> m_DragStartPositionInput = new XRInputValueReader<Vector2>("Drag Start Position", XRInputValueReader.InputSourceMode.InputActionReference);

		[SerializeField]
		private XRInputValueReader<Vector2> m_DragCurrentPositionInput = new XRInputValueReader<Vector2>("Drag Current Position", XRInputValueReader.InputSourceMode.InputActionReference);

		[SerializeField]
		[Tooltip("The input used to read the screen touch count value.")]
		private XRInputValueReader<int> m_ScreenTouchCountInput = new XRInputValueReader<int>("Screen Touch Count", XRInputValueReader.InputSourceMode.InputActionReference);

		private readonly UnityObjectReferenceCache<Transform> m_ParentTransformCache = new UnityObjectReferenceCache<Transform>();

		private Vector2 m_TapStartPosition;

		private Vector2 m_DragStartPosition;
	}
}
