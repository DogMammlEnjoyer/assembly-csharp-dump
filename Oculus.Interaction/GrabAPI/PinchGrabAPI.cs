using System;
using System.Collections.Generic;
using Oculus.Interaction.Input;
using UnityEngine;

namespace Oculus.Interaction.GrabAPI
{
	public class PinchGrabAPI : IFingerAPI
	{
		private float DistanceStart
		{
			get
			{
				if (!this._isPinchVisibilityGood)
				{
					return 0.02f;
				}
				return 0.016f;
			}
		}

		private float DistanceStopMax
		{
			get
			{
				if (!this._isPinchVisibilityGood)
				{
					return 0.1f;
				}
				return 0.1f;
			}
		}

		private float DistanceStopOffset
		{
			get
			{
				if (!this._isPinchVisibilityGood)
				{
					return 0.04f;
				}
				return 0.016f;
			}
		}

		public PinchGrabAPI(IHmd hmd = null)
		{
			this._hmd = hmd;
		}

		public bool GetFingerIsGrabbing(HandFinger finger)
		{
			return this._fingersPinchData[(int)finger].IsPinching;
		}

		public Vector3 GetWristOffsetLocal()
		{
			float num = this._fingersPinchData[0].PinchStrength;
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
			Pose rootPose;
			hand.GetRootPose(out rootPose);
			ReadOnlyHandJointPoses handPoses;
			hand.GetJointPosesLocal(out handPoses);
			this.Update(handPoses, hand.Handedness, rootPose, hand.Scale);
		}

		internal void Update(IReadOnlyList<Pose> handPoses, Handedness handedness, Pose rootPose, float handScale)
		{
			this.ClearState();
			this._shadowHand.SetRoot(Pose.identity);
			this._shadowHand.FromJoints(handPoses, false);
			this._rootPose = rootPose;
			this._handScale = handScale;
			this._isPinchVisibilityGood = this.PinchHasGoodVisibility(handedness);
			this.UpdateThumb(handedness);
			this.UpdateFinger(HandFinger.Index);
			this.UpdateFinger(HandFinger.Middle);
			this.UpdateFinger(HandFinger.Ring);
			this.UpdateFinger(HandFinger.Pinky);
		}

		private void UpdateThumb(Handedness handedness)
		{
			int num = 0;
			this._fingersPinchData[num].UpdateTipPosition(this._shadowHand);
			float distance = float.PositiveInfinity;
			if (this.IsThumbNearIndex(handedness))
			{
				Vector3 position = this._shadowHand.GetWorldPose(HandJointId.HandThumb3).position;
				Vector3 tipPosition = this._fingersPinchData[num].TipPosition;
				distance = this.GetClosestDistanceToJoints(position, tipPosition, this.INDEX_JOINTS, 0.5f);
			}
			this.UpdatePinchData(distance, num, 0.03f, 0.04f, 0.05f);
		}

		private bool IsThumbNearIndex(Handedness handedness)
		{
			Pose worldPose = this._shadowHand.GetWorldPose(HandJointId.HandThumbTip);
			Pose worldPose2 = this._shadowHand.GetWorldPose(HandJointId.HandIndex2);
			Vector3 inNormal = worldPose2.rotation * ((handedness == Handedness.Left) ? Constants.LeftThumbSide : Constants.RightThumbSide);
			Plane plane = new Plane(inNormal, worldPose2.position);
			float num = Mathf.Abs(plane.GetDistanceToPoint(worldPose.position));
			return num > 0f && num < 0.05f;
		}

		private void UpdateFinger(HandFinger finger)
		{
			this._fingersPinchData[(int)finger].UpdateTipPosition(this._shadowHand);
			float distance = float.PositiveInfinity;
			if (this._fingersPinchData[(int)finger].IsPinching)
			{
				distance = this.GetClosestDistanceToJoints(this._fingersPinchData[(int)finger].TipPosition, this.THUMB_JOINTS_MAINTAIN);
			}
			if (this.IsPointNearThumb(this._fingersPinchData[(int)finger].TipPosition, this.THUMB_JOINTS_SELECT))
			{
				distance = this.GetClosestDistanceToJoints(this._fingersPinchData[(int)finger].TipPosition, this.THUMB_JOINTS_SELECT);
			}
			this.UpdatePinchData(distance, (int)finger, this.DistanceStart, this.DistanceStopOffset, this.DistanceStopMax);
		}

		private void UpdatePinchData(float distance, int fingerIndex, float distanceStart, float distanceStopOffset, float distanceStopMax)
		{
			this._fingersPinchData[fingerIndex].UpdateIsPinching(distance, distanceStart, distanceStopOffset, distanceStopMax);
			float value = (distance - distanceStart) / (distanceStopMax - distanceStart);
			float pinchStrength = 1f - Mathf.Clamp01(value);
			this._fingersPinchData[fingerIndex].PinchStrength = pinchStrength;
		}

		private void ClearState()
		{
			for (int i = 0; i < 5; i++)
			{
				this._fingersPinchData[i].ClearState();
			}
		}

		private bool IsPointNearThumb(Vector3 position, HandJointId[] thumbJoints)
		{
			Pose worldPose = this._shadowHand.GetWorldPose(thumbJoints[0]);
			ref Pose worldPose2 = this._shadowHand.GetWorldPose(thumbJoints[1]);
			Vector3 position2 = worldPose.position;
			Vector3 rhs = worldPose2.position - position2;
			return Vector3.Dot(Vector3.Project(position - position2, rhs.normalized), rhs) > 0f;
		}

		private float GetClosestDistanceToJoints(Vector3 edgeStart, Vector3 edgeEnd, HandJointId[] targetJoints, float maximumDotAllowed = 1f)
		{
			float num = float.PositiveInfinity;
			for (int i = 0; i < targetJoints.Length - 1; i++)
			{
				Pose worldPose = this._shadowHand.GetWorldPose(targetJoints[i]);
				Pose worldPose2 = this._shadowHand.GetWorldPose(targetJoints[i + 1]);
				if (maximumDotAllowed >= 1f || Vector3.Dot((edgeEnd - edgeStart).normalized, (worldPose2.position - worldPose.position).normalized) < maximumDotAllowed)
				{
					float b = this.DistanceSegmentToSegment(edgeStart, edgeEnd, worldPose.position, worldPose2.position);
					num = Mathf.Min(num, b);
				}
			}
			return num;
		}

		private float GetClosestDistanceToJoints(Vector3 position, HandJointId[] targetJoints)
		{
			float num = float.PositiveInfinity;
			for (int i = 0; i < targetJoints.Length - 1; i++)
			{
				Pose worldPose = this._shadowHand.GetWorldPose(targetJoints[i]);
				Pose worldPose2 = this._shadowHand.GetWorldPose(targetJoints[i + 1]);
				num = Mathf.Min(num, this.DistancePointToSegment(position, worldPose.position, worldPose2.position));
			}
			return num;
		}

		private float DistancePointToSegment(Vector3 point, Vector3 a0, Vector3 a1)
		{
			Vector3 vector = a1 - a0;
			float d = Mathf.Clamp01(Vector3.Dot(point - a0, vector) / Vector3.Dot(vector, vector));
			return (a0 + d * vector - point).magnitude;
		}

		private float DistanceSegmentToSegment(Vector3 a0, Vector3 a1, Vector3 b0, Vector3 b1)
		{
			Vector3 vector = a1 - a0;
			Vector3 vector2 = b1 - b0;
			Vector3 planeNormal = Vector3.Cross(vector, vector2);
			Vector3 vector3 = Vector3.ProjectOnPlane(a0, planeNormal);
			Vector3 vector4 = Vector3.ProjectOnPlane(b0, planeNormal);
			Vector3 vector5 = Vector3.ProjectOnPlane(vector, planeNormal);
			Vector3 onNormal = Vector3.ProjectOnPlane(vector2, planeNormal);
			Vector3 vector6 = vector4 + Vector3.Project(vector3 - vector4, onNormal) - vector3;
			float num = Vector3.Dot(vector5.normalized, vector6.normalized);
			float num2 = vector6.magnitude / num;
			Vector3 a2 = a0 + vector * Mathf.Clamp01(num2 / vector5.magnitude);
			Vector3 vector7 = Vector3.Project(a2 - b0, vector2);
			if (Vector3.Dot(vector7, vector2) < 0f)
			{
				vector7 = Vector3.zero;
			}
			else if (vector7.sqrMagnitude > vector2.sqrMagnitude)
			{
				vector7 = vector2;
			}
			Vector3 b2 = b0 + vector7;
			return Vector3.Distance(a2, b2);
		}

		private bool PinchHasGoodVisibility(Handedness handedness)
		{
			Pose pose;
			if (this._hmd == null || !this._hmd.TryGetRootPose(out pose))
			{
				return false;
			}
			Vector3 from = this._rootPose.rotation * ((handedness == Handedness.Left) ? Constants.LeftPinkySide : Constants.RightPinkySide);
			Vector3 forward = pose.forward;
			return Vector3.Angle(from, forward) <= 40f;
		}

		private bool _isPinchVisibilityGood;

		private const float PINCH_DISTANCE_START = 0.02f;

		private const float PINCH_DISTANCE_STOP_MAX = 0.1f;

		private const float PINCH_DISTANCE_STOP_OFFSET = 0.04f;

		private const float PINCH_HQ_DISTANCE_START = 0.016f;

		private const float PINCH_HQ_DISTANCE_STOP_MAX = 0.1f;

		private const float PINCH_HQ_DISTANCE_STOP_OFFSET = 0.016f;

		private const float THUMB_DISTANCE_START = 0.03f;

		private const float THUMB_DISTANCE_STOP_MAX = 0.05f;

		private const float THUMB_DISTANCE_STOP_OFFSET = 0.04f;

		private const float THUMB_MAX_DOT = 0.5f;

		private const float PINCH_HQ_VIEW_ANGLE_THRESHOLD = 40f;

		private readonly HandJointId[] THUMB_JOINTS_SELECT = new HandJointId[]
		{
			HandJointId.HandThumb3,
			HandJointId.HandThumbTip
		};

		private readonly HandJointId[] THUMB_JOINTS_MAINTAIN = new HandJointId[]
		{
			HandJointId.HandThumb2,
			HandJointId.HandThumb3,
			HandJointId.HandThumbTip
		};

		private readonly HandJointId[] INDEX_JOINTS = new HandJointId[]
		{
			HandJointId.HandIndex1,
			HandJointId.HandIndex2,
			HandJointId.HandIndex3,
			HandJointId.HandIndexTip
		};

		private readonly PinchGrabAPI.FingerPinchData[] _fingersPinchData = new PinchGrabAPI.FingerPinchData[]
		{
			new PinchGrabAPI.FingerPinchData(HandFinger.Thumb),
			new PinchGrabAPI.FingerPinchData(HandFinger.Index),
			new PinchGrabAPI.FingerPinchData(HandFinger.Middle),
			new PinchGrabAPI.FingerPinchData(HandFinger.Ring),
			new PinchGrabAPI.FingerPinchData(HandFinger.Pinky)
		};

		private IHmd _hmd;

		private readonly ShadowHand _shadowHand = new ShadowHand();

		private float _handScale;

		private Pose _rootPose;

		private class FingerPinchData
		{
			public Vector3 TipPosition { get; private set; }

			public bool IsPinchingChanged { get; private set; }

			public FingerPinchData(HandFinger fingerId)
			{
				this._tipId = HandJointUtils.GetHandFingerTip(fingerId);
			}

			public void UpdateTipPosition(ShadowHand hand)
			{
				Pose worldPose = hand.GetWorldPose(this._tipId);
				this.TipPosition = worldPose.position;
			}

			public void UpdateIsPinching(float distance, float start, float stopOffset, float stopMax)
			{
				if (!this.IsPinching)
				{
					if (distance < start)
					{
						this.IsPinching = true;
						this.IsPinchingChanged = true;
						this._minPinchDistance = distance;
						return;
					}
				}
				else
				{
					this._minPinchDistance = Mathf.Min(this._minPinchDistance, distance);
					if (distance > stopMax || distance > this._minPinchDistance + stopOffset)
					{
						this.IsPinching = false;
						this.IsPinchingChanged = true;
						this._minPinchDistance = float.MaxValue;
					}
				}
			}

			public void ClearState()
			{
				this.IsPinchingChanged = false;
			}

			private readonly HandJointId _tipId;

			private float _minPinchDistance;

			public float PinchStrength;

			public bool IsPinching;
		}
	}
}
