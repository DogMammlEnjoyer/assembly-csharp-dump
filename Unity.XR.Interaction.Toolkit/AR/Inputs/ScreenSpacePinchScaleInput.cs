using System;
using UnityEngine.XR.Interaction.Toolkit.Inputs.Readers;
using UnityEngine.XR.Interaction.Toolkit.Utilities;

namespace UnityEngine.XR.Interaction.Toolkit.AR.Inputs
{
	[AddComponentMenu("XR/Input/Screen Space Pinch Scale Input", 11)]
	[HelpURL("https://docs.unity3d.com/Packages/com.unity.xr.interaction.toolkit@3.2/api/UnityEngine.XR.Interaction.Toolkit.AR.Inputs.ScreenSpacePinchScaleInput.html")]
	public class ScreenSpacePinchScaleInput : MonoBehaviour, IXRInputValueReader<float>, IXRInputValueReader
	{
		public bool useRotationThreshold
		{
			get
			{
				return this.m_UseRotationThreshold;
			}
			set
			{
				this.m_UseRotationThreshold = value;
			}
		}

		public float rotationThreshold
		{
			get
			{
				return this.m_RotationThreshold;
			}
			set
			{
				this.m_RotationThreshold = value;
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

		protected void OnEnable()
		{
			this.m_PinchGapDeltaInput.EnableDirectActionIfModeUsed();
			this.m_TwistDeltaRotationInput.EnableDirectActionIfModeUsed();
		}

		protected void OnDisable()
		{
			this.m_PinchGapDeltaInput.DisableDirectActionIfModeUsed();
			this.m_TwistDeltaRotationInput.DisableDirectActionIfModeUsed();
		}

		public float ReadValue()
		{
			float result;
			this.TryReadValue(out result);
			return result;
		}

		public bool TryReadValue(out float value)
		{
			float f;
			if (this.m_UseRotationThreshold && this.m_TwistDeltaRotationInput.TryReadValue(out f) && Mathf.Abs(f) >= this.m_RotationThreshold)
			{
				value = 0f;
				return true;
			}
			float num;
			if (this.m_PinchGapDeltaInput.TryReadValue(out num))
			{
				value = num * DisplayUtility.screenDpiRatio;
				return true;
			}
			value = 0f;
			return false;
		}

		[SerializeField]
		[Tooltip("Enables a rotation threshold that blocks pinch scale gestures when surpassed.")]
		private bool m_UseRotationThreshold = true;

		[SerializeField]
		[Tooltip("The threshold at which a gestures will be interpreted only as rotation and not a pinch scale gesture.")]
		private float m_RotationThreshold = 0.02f;

		[SerializeField]
		[Tooltip("The input used to read the pinch gap delta value.")]
		private XRInputValueReader<float> m_PinchGapDeltaInput = new XRInputValueReader<float>("Pinch Gap Delta", XRInputValueReader.InputSourceMode.InputActionReference);

		[SerializeField]
		[Tooltip("The input used to read the twist delta rotation value.")]
		private XRInputValueReader<float> m_TwistDeltaRotationInput = new XRInputValueReader<float>("Twist Delta Rotation", XRInputValueReader.InputSourceMode.InputActionReference);
	}
}
