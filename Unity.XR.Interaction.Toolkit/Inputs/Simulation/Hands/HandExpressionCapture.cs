using System;

namespace UnityEngine.XR.Interaction.Toolkit.Inputs.Simulation.Hands
{
	internal class HandExpressionCapture : ScriptableObject
	{
		public Sprite icon
		{
			get
			{
				return this.m_Icon;
			}
			set
			{
				this.m_Icon = value;
			}
		}

		public Pose[] leftHandCapturedPoses
		{
			get
			{
				return this.m_LeftCapturedPoses;
			}
			set
			{
				this.m_LeftCapturedPoses = value;
			}
		}

		public Pose[] rightHandCapturedPoses
		{
			get
			{
				return this.m_RightCapturedPoses;
			}
			set
			{
				this.m_RightCapturedPoses = value;
			}
		}

		[SerializeField]
		[Tooltip("An icon to represent the hand expression.")]
		private Sprite m_Icon;

		[SerializeField]
		[Tooltip("The captured left hand joint poses.")]
		private Pose[] m_LeftCapturedPoses;

		[SerializeField]
		[Tooltip("The captured right hand joint poses.")]
		private Pose[] m_RightCapturedPoses;
	}
}
