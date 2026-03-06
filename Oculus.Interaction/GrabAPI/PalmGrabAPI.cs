using System;
using Oculus.Interaction.Input;
using Oculus.Interaction.PoseDetection;
using UnityEngine;

namespace Oculus.Interaction.GrabAPI
{
	public class PalmGrabAPI : IFingerAPI
	{
		public bool GetFingerIsGrabbing(HandFinger finger)
		{
			return this._fingersGrabData[(int)finger].IsGrabbing;
		}

		public bool GetFingerIsGrabbingChanged(HandFinger finger, bool targetGrabState)
		{
			return this._fingersGrabData[(int)finger].IsGrabbingChanged && this._fingersGrabData[(int)finger].IsGrabbing == targetGrabState;
		}

		public float GetFingerGrabScore(HandFinger finger)
		{
			return this._fingersGrabData[(int)finger].GrabStrength;
		}

		public void Update(IHand hand)
		{
			this.ClearState();
			if (hand == null || !hand.IsTrackedDataValid)
			{
				return;
			}
			this.UpdateVolumeCenter(hand);
			for (int i = 0; i < 5; i++)
			{
				this._fingersGrabData[i].UpdateGrabStrength(hand, this._fingerShapes);
				this._fingersGrabData[i].UpdateIsGrabbing(PalmGrabAPI.START_THRESHOLD, PalmGrabAPI.RELEASE_THRESHOLD);
			}
		}

		private void UpdateVolumeCenter(IHand hand)
		{
			this._poseVolumeCenterOffset = ((hand.Handedness == Handedness.Left) ? (Constants.LeftDistal * PalmGrabAPI.POSE_VOLUME_OFFSET.x + Constants.LeftDorsal * PalmGrabAPI.POSE_VOLUME_OFFSET.y + Constants.LeftThumbSide * PalmGrabAPI.POSE_VOLUME_OFFSET.z) : (Constants.RightDistal * PalmGrabAPI.POSE_VOLUME_OFFSET.x + Constants.RightDorsal * PalmGrabAPI.POSE_VOLUME_OFFSET.y + Constants.RightThumbSide * PalmGrabAPI.POSE_VOLUME_OFFSET.z));
		}

		private void ClearState()
		{
			for (int i = 0; i < 5; i++)
			{
				this._fingersGrabData[i].ClearState();
			}
		}

		public Vector3 GetWristOffsetLocal()
		{
			return this._poseVolumeCenterOffset;
		}

		private Vector3 _poseVolumeCenterOffset = Vector3.zero;

		private static readonly Vector3 POSE_VOLUME_OFFSET = new Vector3(0.07f, -0.03f, 0f);

		private static readonly float START_THRESHOLD = 0.9f;

		private static readonly float RELEASE_THRESHOLD = 0.6f;

		private static readonly Vector2[] CURL_RANGE = new Vector2[]
		{
			new Vector2(190f, 220f),
			new Vector2(180f, 250f),
			new Vector2(180f, 250f),
			new Vector2(180f, 250f),
			new Vector2(180f, 245f)
		};

		private FingerShapes _fingerShapes = new FingerShapes();

		private readonly PalmGrabAPI.FingerGrabData[] _fingersGrabData = new PalmGrabAPI.FingerGrabData[]
		{
			new PalmGrabAPI.FingerGrabData(HandFinger.Thumb),
			new PalmGrabAPI.FingerGrabData(HandFinger.Index),
			new PalmGrabAPI.FingerGrabData(HandFinger.Middle),
			new PalmGrabAPI.FingerGrabData(HandFinger.Ring),
			new PalmGrabAPI.FingerGrabData(HandFinger.Pinky)
		};

		private class FingerGrabData
		{
			public bool IsGrabbingChanged { get; private set; }

			public FingerGrabData(HandFinger fingerId)
			{
				this._fingerID = fingerId;
				Vector2 vector = PalmGrabAPI.CURL_RANGE[(int)this._fingerID];
				this._curlNormalizationParams = new Vector2(vector.x, vector.y - vector.x);
			}

			public void UpdateGrabStrength(IHand hand, FingerShapes fingerShapes)
			{
				float num = fingerShapes.GetCurlValue(this._fingerID, hand);
				if (this._fingerID != HandFinger.Thumb)
				{
					num = (num * 2f + fingerShapes.GetFlexionValue(this._fingerID, hand)) / 3f;
				}
				this.GrabStrength = Mathf.Clamp01((num - this._curlNormalizationParams.x) / this._curlNormalizationParams.y);
			}

			public void UpdateIsGrabbing(float startThreshold, float releaseThreshold)
			{
				if (this.GrabStrength > startThreshold)
				{
					if (!this.IsGrabbing)
					{
						this.IsGrabbing = true;
						this.IsGrabbingChanged = true;
					}
					return;
				}
				if (this.GrabStrength < releaseThreshold && this.IsGrabbing)
				{
					this.IsGrabbing = false;
					this.IsGrabbingChanged = true;
				}
			}

			public void ClearState()
			{
				this.IsGrabbingChanged = false;
			}

			private readonly HandFinger _fingerID;

			private readonly Vector2 _curlNormalizationParams;

			public float GrabStrength;

			public bool IsGrabbing;
		}
	}
}
