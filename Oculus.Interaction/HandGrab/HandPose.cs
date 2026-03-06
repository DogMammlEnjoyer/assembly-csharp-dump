using System;
using Oculus.Interaction.Input;
using UnityEngine;

namespace Oculus.Interaction.HandGrab
{
	[Serializable]
	public class HandPose
	{
		public Handedness Handedness
		{
			get
			{
				return this._handedness;
			}
			set
			{
				this._handedness = value;
			}
		}

		public Quaternion[] JointRotations
		{
			get
			{
				if (this._jointRotations == null || this._jointRotations.Length == 0)
				{
					this._jointRotations = new Quaternion[FingersMetadata.HAND_JOINT_IDS.Length];
				}
				return this._jointRotations;
			}
			set
			{
				this._jointRotations = value;
			}
		}

		public JointFreedom[] FingersFreedom
		{
			get
			{
				if (this._fingersFreedom == null || this._fingersFreedom.Length == 0)
				{
					this._fingersFreedom = FingersMetadata.DefaultFingersFreedom();
				}
				return this._fingersFreedom;
			}
		}

		public HandPose()
		{
		}

		public HandPose(Handedness handedness)
		{
			this._handedness = handedness;
		}

		public HandPose(HandPose other)
		{
			this.CopyFrom(other, false);
		}

		public void CopyFrom(HandPose from, bool mirrorHandedness = false)
		{
			if (!mirrorHandedness)
			{
				this._handedness = from.Handedness;
			}
			Array.Copy(from.FingersFreedom, this.FingersFreedom, 5);
			Array.Copy(from.JointRotations, this.JointRotations, FingersMetadata.HAND_JOINT_IDS.Length);
		}

		public static void Lerp(in HandPose from, in HandPose to, float t, ref HandPose result)
		{
			t = Mathf.Clamp01(t);
			int num = 0;
			while (num < from.JointRotations.Length && num < to.JointRotations.Length)
			{
				result.JointRotations[num] = Quaternion.SlerpUnclamped(from.JointRotations[num], to.JointRotations[num], t);
				num++;
			}
			HandPose handPose = (t <= 0.5f) ? from : to;
			result._handedness = handPose.Handedness;
			for (int i = 0; i < 5; i++)
			{
				result.FingersFreedom[i] = handPose.FingersFreedom[i];
			}
		}

		[SerializeField]
		private Handedness _handedness;

		[SerializeField]
		private JointFreedom[] _fingersFreedom = FingersMetadata.DefaultFingersFreedom();

		[SerializeField]
		private Quaternion[] _jointRotations = new Quaternion[FingersMetadata.HAND_JOINT_IDS.Length];
	}
}
