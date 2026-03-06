using System;
using System.Collections.Generic;

namespace UnityEngine.XR.Interaction.Toolkit
{
	[AddComponentMenu("XR/Locomotion/Legacy/Continuous Move Provider (Device-based)", 11)]
	[HelpURL("https://docs.unity3d.com/Packages/com.unity.xr.interaction.toolkit@3.2/api/UnityEngine.XR.Interaction.Toolkit.DeviceBasedContinuousMoveProvider.html")]
	[Obsolete("DeviceBasedContinuousMoveProvider has been deprecated in version 3.0.0. Use ContinuousMoveProvider instead.")]
	public class DeviceBasedContinuousMoveProvider : ContinuousMoveProviderBase
	{
		public DeviceBasedContinuousMoveProvider.InputAxes inputBinding
		{
			get
			{
				return this.m_InputBinding;
			}
			set
			{
				this.m_InputBinding = value;
			}
		}

		public List<XRBaseController> controllers
		{
			get
			{
				return this.m_Controllers;
			}
			set
			{
				this.m_Controllers = value;
			}
		}

		public float deadzoneMin
		{
			get
			{
				return this.m_DeadzoneMin;
			}
			set
			{
				this.m_DeadzoneMin = value;
			}
		}

		public float deadzoneMax
		{
			get
			{
				return this.m_DeadzoneMax;
			}
			set
			{
				this.m_DeadzoneMax = value;
			}
		}

		protected override Vector2 ReadInput()
		{
			if (this.m_Controllers.Count == 0)
			{
				return Vector2.zero;
			}
			Vector2 vector = Vector2.zero;
			InputFeatureUsage<Vector2> usage = DeviceBasedContinuousMoveProvider.k_Vec2UsageList[(int)this.m_InputBinding];
			for (int i = 0; i < this.m_Controllers.Count; i++)
			{
				XRController xrcontroller = this.m_Controllers[i] as XRController;
				Vector2 value;
				if (xrcontroller != null && xrcontroller.enableInputActions && xrcontroller.inputDevice.TryGetFeatureValue(usage, out value))
				{
					vector += this.GetDeadzoneAdjustedValue(value);
				}
			}
			return vector;
		}

		protected Vector2 GetDeadzoneAdjustedValue(Vector2 value)
		{
			float magnitude = value.magnitude;
			float deadzoneAdjustedValue = this.GetDeadzoneAdjustedValue(magnitude);
			if (Mathf.Approximately(deadzoneAdjustedValue, 0f))
			{
				value = Vector2.zero;
			}
			else
			{
				value *= deadzoneAdjustedValue / magnitude;
			}
			return value;
		}

		protected float GetDeadzoneAdjustedValue(float value)
		{
			float deadzoneMin = this.m_DeadzoneMin;
			float deadzoneMax = this.m_DeadzoneMax;
			float num = Mathf.Abs(value);
			if (num < deadzoneMin)
			{
				return 0f;
			}
			if (num > deadzoneMax)
			{
				return Mathf.Sign(value);
			}
			return Mathf.Sign(value) * ((num - deadzoneMin) / (deadzoneMax - deadzoneMin));
		}

		[SerializeField]
		[Tooltip("The 2D Input Axis on the controller devices that will be used to trigger a move.")]
		private DeviceBasedContinuousMoveProvider.InputAxes m_InputBinding;

		[SerializeField]
		[Tooltip("A list of controllers that allow move.  If an XRController is not enabled, or does not have input actions enabled, move will not work.")]
		private List<XRBaseController> m_Controllers = new List<XRBaseController>();

		[SerializeField]
		[Tooltip("Value below which input values will be clamped. After clamping, values will be renormalized to [0, 1] between min and max.")]
		private float m_DeadzoneMin = 0.125f;

		[SerializeField]
		[Tooltip("Value above which input values will be clamped. After clamping, values will be renormalized to [0, 1] between min and max.")]
		private float m_DeadzoneMax = 0.925f;

		private static readonly InputFeatureUsage<Vector2>[] k_Vec2UsageList = new InputFeatureUsage<Vector2>[]
		{
			CommonUsages.primary2DAxis,
			CommonUsages.secondary2DAxis
		};

		public enum InputAxes
		{
			Primary2DAxis,
			Secondary2DAxis
		}
	}
}
