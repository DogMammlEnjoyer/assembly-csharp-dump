using System;

namespace UnityEngine.XR.Interaction.Toolkit.Feedback
{
	[Serializable]
	public class HapticImpulseData
	{
		public float amplitude
		{
			get
			{
				return this.m_Amplitude;
			}
			set
			{
				this.m_Amplitude = value;
			}
		}

		public float duration
		{
			get
			{
				return this.m_Duration;
			}
			set
			{
				this.m_Duration = value;
			}
		}

		public float frequency
		{
			get
			{
				return this.m_Frequency;
			}
			set
			{
				this.m_Frequency = value;
			}
		}

		[SerializeField]
		[Range(0f, 1f)]
		private float m_Amplitude;

		[SerializeField]
		private float m_Duration;

		[SerializeField]
		private float m_Frequency;
	}
}
