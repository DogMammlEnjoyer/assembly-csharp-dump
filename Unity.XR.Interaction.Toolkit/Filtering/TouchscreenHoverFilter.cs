using System;
using UnityEngine.XR.Interaction.Toolkit.Inputs.Readers;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

namespace UnityEngine.XR.Interaction.Toolkit.Filtering
{
	[AddComponentMenu("XR/AR/Touchscreen Hover Filter", 11)]
	[HelpURL("https://docs.unity3d.com/Packages/com.unity.xr.interaction.toolkit@3.2/api/UnityEngine.XR.Interaction.Toolkit.Filtering.TouchscreenHoverFilter.html")]
	public class TouchscreenHoverFilter : MonoBehaviour, IXRHoverFilter
	{
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

		public bool canProcess
		{
			get
			{
				return base.isActiveAndEnabled;
			}
		}

		protected void OnEnable()
		{
			this.m_ScreenTouchCountInput.EnableDirectActionIfModeUsed();
		}

		protected void OnDisable()
		{
			this.m_ScreenTouchCountInput.DisableDirectActionIfModeUsed();
		}

		public bool Process(IXRHoverInteractor interactor, IXRHoverInteractable interactable)
		{
			XRBaseInputInteractor xrbaseInputInteractor = interactor as XRBaseInputInteractor;
			if (xrbaseInputInteractor != null)
			{
				return xrbaseInputInteractor.selectInput.ReadIsPerformed() && this.m_ScreenTouchCountInput.ReadValue() <= 1;
			}
			return this.m_ScreenTouchCountInput.ReadValue() > 0;
		}

		[SerializeField]
		private XRInputValueReader<int> m_ScreenTouchCountInput = new XRInputValueReader<int>("Screen Touch Count", XRInputValueReader.InputSourceMode.InputActionReference);
	}
}
