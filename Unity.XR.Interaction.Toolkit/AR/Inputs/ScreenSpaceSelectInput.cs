using System;
using UnityEngine.XR.Interaction.Toolkit.Inputs.Readers;

namespace UnityEngine.XR.Interaction.Toolkit.AR.Inputs
{
	[AddComponentMenu("XR/Input/Screen Space Select Input", 11)]
	[HelpURL("https://docs.unity3d.com/Packages/com.unity.xr.interaction.toolkit@3.2/api/UnityEngine.XR.Interaction.Toolkit.AR.Inputs.ScreenSpaceSelectInput.html")]
	[DefaultExecutionOrder(-30050)]
	public class ScreenSpaceSelectInput : MonoBehaviour, IXRInputButtonReader, IXRInputValueReader<float>, IXRInputValueReader
	{
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

		public XRInputValueReader<float> pinchGapDeltaInput
		{
			get
			{
				return this.m_PinchGapDeltaInput;
			}
			set
			{
				XRInputReaderUtility.SetInputProperty<float>(ref this.m_PinchGapDeltaInput, value, this);
			}
		}

		public XRInputValueReader<float> twistDeltaRotationInput
		{
			get
			{
				return this.m_TwistDeltaRotationInput;
			}
			set
			{
				XRInputReaderUtility.SetInputProperty<float>(ref this.m_TwistDeltaRotationInput, value, this);
			}
		}

		protected void Update()
		{
			bool isPerformed = this.m_IsPerformed;
			Vector2 tapStartPosition = this.m_TapStartPosition;
			bool flag = this.m_TapStartPositionInput.TryReadValue(out this.m_TapStartPosition) && tapStartPosition != this.m_TapStartPosition;
			float num;
			Vector2 vector;
			this.m_IsPerformed = (this.m_PinchGapDeltaInput.TryReadValue(out num) || this.m_TwistDeltaRotationInput.TryReadValue(out num) || this.m_DragCurrentPositionInput.TryReadValue(out vector) || flag);
			this.m_WasPerformedThisFrame = (!isPerformed && this.m_IsPerformed);
			this.m_WasCompletedThisFrame = (isPerformed && !this.m_IsPerformed);
		}

		public bool ReadIsPerformed()
		{
			return this.m_IsPerformed;
		}

		public bool ReadWasPerformedThisFrame()
		{
			return this.m_WasPerformedThisFrame;
		}

		public bool ReadWasCompletedThisFrame()
		{
			return this.m_WasCompletedThisFrame;
		}

		public float ReadValue()
		{
			if (!this.m_IsPerformed)
			{
				return 0f;
			}
			return 1f;
		}

		public bool TryReadValue(out float value)
		{
			value = (this.m_IsPerformed ? 1f : 0f);
			return this.m_IsPerformed;
		}

		[SerializeField]
		private XRInputValueReader<Vector2> m_TapStartPositionInput = new XRInputValueReader<Vector2>("Tap Start Position", XRInputValueReader.InputSourceMode.InputActionReference);

		[SerializeField]
		private XRInputValueReader<Vector2> m_DragCurrentPositionInput = new XRInputValueReader<Vector2>("Drag Current Position", XRInputValueReader.InputSourceMode.InputActionReference);

		[SerializeField]
		private XRInputValueReader<float> m_PinchGapDeltaInput = new XRInputValueReader<float>("Pinch Gap Delta", XRInputValueReader.InputSourceMode.InputActionReference);

		[SerializeField]
		private XRInputValueReader<float> m_TwistDeltaRotationInput = new XRInputValueReader<float>("Twist Delta Rotation", XRInputValueReader.InputSourceMode.InputActionReference);

		private bool m_IsPerformed;

		private bool m_WasPerformedThisFrame;

		private bool m_WasCompletedThisFrame;

		private Vector2 m_TapStartPosition;
	}
}
