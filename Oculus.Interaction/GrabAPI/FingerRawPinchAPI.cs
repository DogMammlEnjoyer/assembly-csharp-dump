using System;
using Oculus.Interaction.Input;
using UnityEngine;

namespace Oculus.Interaction.GrabAPI
{
	public class FingerRawPinchAPI : IFingerAPI
	{
		public bool GetFingerIsGrabbing(HandFinger finger)
		{
			return this._fingersPinchData[(int)finger].IsPinching;
		}

		public Vector3 GetWristOffsetLocal()
		{
			float num = float.NegativeInfinity;
			Vector3 tipPosition = this._fingersPinchData[0].TipPosition;
			Vector3 result = tipPosition;
			for (int i = 1; i < 5; i++)
			{
				float pinchStrength = this._fingersPinchData[i].PinchStrength;
				if (pinchStrength > num)
				{
					num = pinchStrength;
					Vector3 tipPosition2 = this._fingersPinchData[i].TipPosition;
					result = (tipPosition + tipPosition2) * 0.5f;
				}
			}
			return result;
		}

		public bool GetFingerIsGrabbingChanged(HandFinger finger, bool targetPinchState)
		{
			return this._fingersPinchData[(int)finger].IsPinchingChanged && this._fingersPinchData[(int)finger].IsPinching == targetPinchState;
		}

		public float GetFingerGrabScore(HandFinger finger)
		{
			return this._fingersPinchData[(int)finger].PinchStrength;
		}

		public void Update(IHand hand)
		{
			this.ClearState();
			for (int i = 0; i < 5; i++)
			{
				this._fingersPinchData[i].UpdateIsPinching(hand);
			}
		}

		private void ClearState()
		{
			for (int i = 0; i < 5; i++)
			{
				this._fingersPinchData[i].ClearState();
			}
		}

		private readonly FingerRawPinchAPI.FingerPinchData[] _fingersPinchData = new FingerRawPinchAPI.FingerPinchData[]
		{
			new FingerRawPinchAPI.FingerPinchData(HandFinger.Thumb),
			new FingerRawPinchAPI.FingerPinchData(HandFinger.Index),
			new FingerRawPinchAPI.FingerPinchData(HandFinger.Middle),
			new FingerRawPinchAPI.FingerPinchData(HandFinger.Ring),
			new FingerRawPinchAPI.FingerPinchData(HandFinger.Pinky)
		};

		private class FingerPinchData
		{
			public bool IsPinchingChanged { get; private set; }

			public Vector3 TipPosition { get; private set; }

			public FingerPinchData(HandFinger fingerId)
			{
				this._finger = fingerId;
				this._tipId = HandJointUtils.GetHandFingerTip(fingerId);
			}

			private void UpdateTipPosition(IHand hand)
			{
				Pose pose;
				if (hand.GetJointPoseFromWrist(this._tipId, out pose))
				{
					this.TipPosition = pose.position;
				}
			}

			public void UpdateIsPinching(IHand hand)
			{
				this.UpdateTipPosition(hand);
				this.PinchStrength = hand.GetFingerPinchStrength(this._finger);
				bool fingerIsPinching = hand.GetFingerIsPinching(this._finger);
				if (fingerIsPinching != this.IsPinching)
				{
					this.IsPinchingChanged = true;
				}
				this.IsPinching = fingerIsPinching;
			}

			public void ClearState()
			{
				this.IsPinchingChanged = false;
			}

			private readonly HandFinger _finger;

			private readonly HandJointId _tipId;

			public float PinchStrength;

			public bool IsPinching;
		}
	}
}
