using System;

namespace UnityEngine.XR.Interaction.Toolkit.Interactors.Visuals
{
	[Serializable]
	public class LineProperties
	{
		public bool smoothlyCurveLine
		{
			get
			{
				return this.m_SmoothlyCurveLine;
			}
			set
			{
				this.m_SmoothlyCurveLine = value;
			}
		}

		public float lineBendRatio
		{
			get
			{
				return this.m_LineBendRatio;
			}
			set
			{
				this.m_LineBendRatio = value;
			}
		}

		public bool adjustWidth
		{
			get
			{
				return this.m_AdjustWidth;
			}
			set
			{
				this.m_AdjustWidth = value;
			}
		}

		public float starWidth
		{
			get
			{
				return this.m_StarWidth;
			}
			set
			{
				this.m_StarWidth = value;
			}
		}

		public float endWidth
		{
			get
			{
				return this.m_EndWidth;
			}
			set
			{
				this.m_EndWidth = value;
			}
		}

		public float endWidthScaleDistanceFactor
		{
			get
			{
				return this.m_EndWidthScaleDistanceFactor;
			}
			set
			{
				this.m_EndWidthScaleDistanceFactor = value;
			}
		}

		public bool adjustGradient
		{
			get
			{
				return this.m_AdjustGradient;
			}
			set
			{
				this.m_AdjustGradient = value;
			}
		}

		public Gradient gradient
		{
			get
			{
				return this.m_Gradient;
			}
			set
			{
				this.m_Gradient = value;
			}
		}

		public bool customizeExpandLineDrawPercent
		{
			get
			{
				return this.m_CustomizeExpandLineDrawPercent;
			}
			set
			{
				this.m_CustomizeExpandLineDrawPercent = value;
			}
		}

		public float expandModeLineDrawPercent
		{
			get
			{
				return this.m_ExpandModeLineDrawPercent;
			}
			set
			{
				this.m_ExpandModeLineDrawPercent = value;
			}
		}

		private const float k_DefaultLineWidth = 0.005f;

		[Header("Bend Settings")]
		[SerializeField]
		[Tooltip("Determine if the line should smoothly curve when this state property is active. If false, a straight line will be drawn.")]
		private bool m_SmoothlyCurveLine = true;

		[SerializeField]
		[Range(0f, 1f)]
		[Tooltip("Ratio to control the bend of the line by adjusting the mid-point. A value of 1 defaults to a straight line.")]
		private float m_LineBendRatio = 0.5f;

		[Header("Width Settings")]
		[SerializeField]
		[Tooltip("Determine if the line width should be customized from defaults when this state property is active.")]
		private bool m_AdjustWidth = true;

		[SerializeField]
		[Tooltip("Width of the line at the start.")]
		private float m_StarWidth = 0.005f;

		[SerializeField]
		[Tooltip("Width of the line at the end.")]
		private float m_EndWidth = 0.005f;

		[SerializeField]
		[Range(0f, 10f)]
		[Tooltip("If greater than 0, the curve end width will be scaled based on the the percentage of the line length to the max visual curve distance, multiplied by the scale factor.")]
		private float m_EndWidthScaleDistanceFactor = 2f;

		[Header("Gradient Settings")]
		[SerializeField]
		[Tooltip("Determine if the line color should change when this state property is active.")]
		private bool m_AdjustGradient = true;

		[SerializeField]
		[Tooltip("Color gradient to use when this state property is active.")]
		private Gradient m_Gradient;

		[Header("Expand Settings")]
		[SerializeField]
		[Tooltip("Determine if the line mode expansion should be customized from defaults")]
		private bool m_CustomizeExpandLineDrawPercent;

		[SerializeField]
		[Tooltip("Percent of the line to draw when using the expand from hit point mode when this state property is active.")]
		private float m_ExpandModeLineDrawPercent = 1f;
	}
}
