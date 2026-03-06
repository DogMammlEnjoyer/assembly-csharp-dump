using System;
using Unity.XR.CoreUtils;
using UnityEngine.XR.Interaction.Toolkit.AffordanceSystem.Receiver.Primitives;
using UnityEngine.XR.Interaction.Toolkit.Interactors.Visuals;

namespace UnityEngine.XR.Interaction.Toolkit.AffordanceSystem.Receiver.Rendering
{
	[AddComponentMenu("Affordance System/Receiver/Rendering/Color Gradient Line Renderer Affordance Receiver", 12)]
	[HelpURL("https://docs.unity3d.com/Packages/com.unity.xr.interaction.toolkit@3.2/api/UnityEngine.XR.Interaction.Toolkit.AffordanceSystem.Receiver.Rendering.ColorGradientLineRendererAffordanceReceiver.html")]
	[Obsolete("The Affordance System namespace and all associated classes have been deprecated. The existing affordance system will be moved, replaced and updated with a new interaction feedback system in a future version of XRI.")]
	public class ColorGradientLineRendererAffordanceReceiver : ColorAffordanceReceiver
	{
		public LineRenderer lineRenderer
		{
			get
			{
				return this.m_LineRenderer;
			}
			set
			{
				this.m_LineRenderer = value;
			}
		}

		public ColorGradientLineRendererAffordanceReceiver.LineColorProperty lineColorProperty
		{
			get
			{
				return this.m_LineColorProperty;
			}
			set
			{
				this.m_LineColorProperty = value;
				this.CaptureInitialValue();
			}
		}

		public bool disableXRInteractorLineVisualColorControlIfPresent
		{
			get
			{
				return this.m_DisableXRInteractorLineVisualColorControlIfPresent;
			}
			set
			{
				this.m_DisableXRInteractorLineVisualColorControlIfPresent = value;
			}
		}

		protected override void Awake()
		{
			base.Awake();
			if (this.m_LineRenderer == null)
			{
				this.m_LineRenderer = base.GetComponentInParent<LineRenderer>();
				if (this.m_LineRenderer == null)
				{
					XRLoggingUtils.LogError("Missing Line Renderer on " + ((this != null) ? this.ToString() : null), this);
					base.enabled = false;
				}
			}
		}

		protected override void Start()
		{
			base.Start();
			if (this.m_DisableXRInteractorLineVisualColorControlIfPresent)
			{
				XRInteractorLineVisual componentInParent = base.GetComponentInParent<XRInteractorLineVisual>();
				if (componentInParent != null)
				{
					componentInParent.setLineColorGradient = false;
				}
			}
		}

		protected override void OnAffordanceValueUpdated(Color newValue)
		{
			base.OnAffordanceValueUpdated(newValue);
			ColorGradientLineRendererAffordanceReceiver.LineColorProperty lineColorProperty = this.m_LineColorProperty;
			if (lineColorProperty == ColorGradientLineRendererAffordanceReceiver.LineColorProperty.StartColor)
			{
				this.m_LineRenderer.startColor = newValue;
				return;
			}
			if (lineColorProperty != ColorGradientLineRendererAffordanceReceiver.LineColorProperty.EndColor)
			{
				return;
			}
			this.m_LineRenderer.endColor = newValue;
		}

		protected override void CaptureInitialValue()
		{
			if (base.initialValueCaptured)
			{
				return;
			}
			this.m_InitialStartColor = this.m_LineRenderer.startColor;
			this.m_InitialEndColor = this.m_LineRenderer.endColor;
			base.CaptureInitialValue();
		}

		protected override Color GetCurrentValueForCapture()
		{
			ColorGradientLineRendererAffordanceReceiver.LineColorProperty lineColorProperty = this.m_LineColorProperty;
			if (lineColorProperty == ColorGradientLineRendererAffordanceReceiver.LineColorProperty.StartColor)
			{
				return this.m_InitialStartColor;
			}
			if (lineColorProperty != ColorGradientLineRendererAffordanceReceiver.LineColorProperty.EndColor)
			{
				return base.GetCurrentValueForCapture();
			}
			return this.m_InitialEndColor;
		}

		[SerializeField]
		[Tooltip("Line Renderer on which to animate colors.")]
		private LineRenderer m_LineRenderer;

		[SerializeField]
		[Tooltip("Mode determining how color is applied to the associated Line Renderer.")]
		private ColorGradientLineRendererAffordanceReceiver.LineColorProperty m_LineColorProperty;

		[SerializeField]
		[Tooltip("Prevent XR Interactor Line Visual from controlling line rendering color if present.")]
		private bool m_DisableXRInteractorLineVisualColorControlIfPresent = true;

		private Color m_InitialStartColor;

		private Color m_InitialEndColor;

		public enum LineColorProperty
		{
			StartColor,
			EndColor
		}
	}
}
