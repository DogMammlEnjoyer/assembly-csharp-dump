using System;
using System.Diagnostics;
using UnityEngine.XR.Interaction.Toolkit.Inputs.Readers;
using UnityEngine.XR.Interaction.Toolkit.Interactors;
using UnityEngine.XR.Interaction.Toolkit.Utilities;

namespace UnityEngine.XR.Interaction.Toolkit.AR.Inputs
{
	[AddComponentMenu("XR/Input/Screen Space Rotate Input", 11)]
	[HelpURL("https://docs.unity3d.com/Packages/com.unity.xr.interaction.toolkit@3.2/api/UnityEngine.XR.Interaction.Toolkit.AR.Inputs.ScreenSpaceRotateInput.html")]
	public class ScreenSpaceRotateInput : MonoBehaviour, IXRInputValueReader<Vector2>, IXRInputValueReader
	{
		public XRRayInteractor rayInteractor
		{
			get
			{
				return this.m_RayInteractor;
			}
			set
			{
				this.m_RayInteractor = value;
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

		public XRInputValueReader<Vector2> dragDeltaInput
		{
			get
			{
				return this.m_DragDeltaInput;
			}
			set
			{
				XRInputReaderUtility.SetInputProperty<Vector2>(ref this.m_DragDeltaInput, value, this);
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

		[Conditional("UNITY_EDITOR")]
		protected void Reset()
		{
		}

		protected void Awake()
		{
			if (this.m_RayInteractor == null)
			{
				this.m_RayInteractor = base.GetComponentInParent<XRRayInteractor>(true);
			}
		}

		protected void OnEnable()
		{
			this.m_TwistDeltaRotationInput.EnableDirectActionIfModeUsed();
			this.m_DragDeltaInput.EnableDirectActionIfModeUsed();
			this.m_ScreenTouchCountInput.EnableDirectActionIfModeUsed();
		}

		protected void OnDisable()
		{
			this.m_TwistDeltaRotationInput.DisableDirectActionIfModeUsed();
			this.m_DragDeltaInput.DisableDirectActionIfModeUsed();
			this.m_ScreenTouchCountInput.DisableDirectActionIfModeUsed();
		}

		public Vector2 ReadValue()
		{
			Vector2 result;
			this.TryReadValue(out result);
			return result;
		}

		public bool TryReadValue(out Vector2 value)
		{
			float num;
			if (this.m_TwistDeltaRotationInput.TryReadValue(out num))
			{
				value = new Vector2(-num, 0f);
				return true;
			}
			Vector2 v;
			if (this.m_ScreenTouchCountInput.ReadValue() > 1 && this.m_DragDeltaInput.TryReadValue(out v))
			{
				Transform attachTransform = this.m_RayInteractor.attachTransform;
				Vector3 vector = Quaternion.Inverse(Quaternion.LookRotation(attachTransform.forward, Vector3.up)) * attachTransform.rotation * v;
				value = new Vector2(vector.x * DisplayUtility.screenDpiRatio * -50f, 0f);
				return true;
			}
			value = Vector2.zero;
			return false;
		}

		[SerializeField]
		[Tooltip("The ray interactor to get the attach transform from.")]
		private XRRayInteractor m_RayInteractor;

		[SerializeField]
		[Tooltip("The input used to read the twist delta rotation value.")]
		private XRInputValueReader<float> m_TwistDeltaRotationInput = new XRInputValueReader<float>("Twist Delta Rotation", XRInputValueReader.InputSourceMode.InputActionReference);

		[SerializeField]
		[Tooltip("The input used to read the drag delta value.")]
		private XRInputValueReader<Vector2> m_DragDeltaInput = new XRInputValueReader<Vector2>("Drag Delta", XRInputValueReader.InputSourceMode.InputActionReference);

		[SerializeField]
		[Tooltip("The input used to read the screen touch count value.")]
		private XRInputValueReader<int> m_ScreenTouchCountInput = new XRInputValueReader<int>("Screen Touch Count", XRInputValueReader.InputSourceMode.InputActionReference);
	}
}
