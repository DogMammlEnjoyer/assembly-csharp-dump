using System;
using UnityEngine.Scripting.APIUpdating;

namespace UnityEngine.XR.Interaction.Toolkit.Locomotion.Comfort
{
	[MovedFrom("UnityEngine.XR.Interaction.Toolkit")]
	[Serializable]
	public sealed class VignetteParameters
	{
		public float apertureSize
		{
			get
			{
				return this.m_ApertureSize;
			}
			set
			{
				this.m_ApertureSize = value;
			}
		}

		public float featheringEffect
		{
			get
			{
				return this.m_FeatheringEffect;
			}
			set
			{
				this.m_FeatheringEffect = value;
			}
		}

		public float easeInTime
		{
			get
			{
				return this.m_EaseInTime;
			}
			set
			{
				this.m_EaseInTime = value;
			}
		}

		public float easeOutTime
		{
			get
			{
				return this.m_EaseOutTime;
			}
			set
			{
				this.m_EaseOutTime = value;
			}
		}

		public bool easeInTimeLock
		{
			get
			{
				return this.m_EaseInTimeLock;
			}
			set
			{
				this.m_EaseInTimeLock = value;
			}
		}

		public float easeOutDelayTime
		{
			get
			{
				return this.m_EaseOutDelayTime;
			}
			set
			{
				this.m_EaseOutDelayTime = value;
			}
		}

		public Color vignetteColor
		{
			get
			{
				return this.m_VignetteColor;
			}
			set
			{
				this.m_VignetteColor = value;
			}
		}

		public Color vignetteColorBlend
		{
			get
			{
				return this.m_VignetteColorBlend;
			}
			set
			{
				this.m_VignetteColorBlend = value;
			}
		}

		public float apertureVerticalPosition
		{
			get
			{
				return this.m_ApertureVerticalPosition;
			}
			set
			{
				this.m_ApertureVerticalPosition = value;
			}
		}

		public void CopyFrom(VignetteParameters parameters)
		{
			if (parameters == null)
			{
				throw new ArgumentNullException("parameters");
			}
			this.apertureSize = parameters.apertureSize;
			this.featheringEffect = parameters.featheringEffect;
			this.easeInTime = parameters.easeInTime;
			this.easeOutTime = parameters.easeOutTime;
			this.easeInTimeLock = parameters.easeInTimeLock;
			this.easeOutDelayTime = parameters.easeOutDelayTime;
			this.vignetteColor = parameters.vignetteColor;
			this.vignetteColorBlend = parameters.vignetteColorBlend;
			this.apertureVerticalPosition = parameters.apertureVerticalPosition;
		}

		[SerializeField]
		[Range(0f, 1f)]
		private float m_ApertureSize = 0.7f;

		[SerializeField]
		[Range(0f, 1f)]
		private float m_FeatheringEffect = 0.2f;

		[SerializeField]
		private float m_EaseInTime = 0.3f;

		[SerializeField]
		private float m_EaseOutTime = 0.3f;

		[SerializeField]
		private bool m_EaseInTimeLock;

		[SerializeField]
		private float m_EaseOutDelayTime;

		[SerializeField]
		private Color m_VignetteColor = VignetteParameters.Defaults.vignetteColorDefault;

		[SerializeField]
		private Color m_VignetteColorBlend = VignetteParameters.Defaults.vignetteColorBlendDefault;

		[SerializeField]
		[Range(-0.2f, 0.2f)]
		private float m_ApertureVerticalPosition;

		internal static class Defaults
		{
			public const float apertureSizeMax = 1f;

			public const float featheringEffectMax = 1f;

			public const float apertureVerticalPositionMax = 0.2f;

			public const float apertureVerticalPositionMin = -0.2f;

			public const float apertureSizeDefault = 0.7f;

			public const float featheringEffectDefault = 0.2f;

			public const float easeInTimeDefault = 0.3f;

			public const float easeOutTimeDefault = 0.3f;

			public const bool easeInTimeLockDefault = false;

			public const float easeOutDelayTimeDefault = 0f;

			public static readonly Color vignetteColorDefault = Color.black;

			public static readonly Color vignetteColorBlendDefault = Color.black;

			public const float apertureVerticalPositionDefault = 0f;

			public static readonly VignetteParameters defaultEffect = new VignetteParameters
			{
				apertureSize = 0.7f,
				featheringEffect = 0.2f,
				easeInTime = 0.3f,
				easeOutTime = 0.3f,
				easeInTimeLock = false,
				easeOutDelayTime = 0f,
				vignetteColor = VignetteParameters.Defaults.vignetteColorDefault,
				vignetteColorBlend = VignetteParameters.Defaults.vignetteColorBlendDefault,
				apertureVerticalPosition = 0f
			};

			public static readonly VignetteParameters noEffect = new VignetteParameters
			{
				apertureSize = 1f,
				featheringEffect = 0f,
				easeInTime = 0f,
				easeOutTime = 0f,
				easeInTimeLock = false,
				easeOutDelayTime = 0f,
				vignetteColor = VignetteParameters.Defaults.vignetteColorDefault,
				vignetteColorBlend = VignetteParameters.Defaults.vignetteColorBlendDefault,
				apertureVerticalPosition = 0f
			};
		}
	}
}
