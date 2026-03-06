using System;
using System.Collections.Generic;
using Oculus.Interaction.Input;
using UnityEngine;

namespace Oculus.Interaction
{
	public class HandJointsPose : MonoBehaviour
	{
		public IHand Hand { get; private set; }

		public bool MirrorOffsetsForLeftHand
		{
			get
			{
				return this._mirrorOffsetsForLeftHand;
			}
			set
			{
				this._mirrorOffsetsForLeftHand = value;
			}
		}

		public List<HandJointsPose.WeightedJoint> WeightedJoints
		{
			get
			{
				return this._joints;
			}
			set
			{
				this._joints = value;
			}
		}

		public Vector3 LocalPositionOffset
		{
			get
			{
				return this._posOffset;
			}
			set
			{
				this._posOffset = value;
			}
		}

		public Quaternion RotationOffset
		{
			get
			{
				return this._rotOffset;
			}
			set
			{
				this._rotOffset = value;
			}
		}

		protected virtual void Awake()
		{
			this.Hand = (this._hand as IHand);
		}

		protected virtual void Start()
		{
			this.BeginStart(ref this._started, null);
			this.EndStart(ref this._started);
		}

		protected virtual void OnEnable()
		{
			if (this._started)
			{
				this.Hand.WhenHandUpdated += this.HandleHandUpdated;
			}
		}

		protected virtual void OnDisable()
		{
			if (this._started)
			{
				this.Hand.WhenHandUpdated -= this.HandleHandUpdated;
			}
		}

		private void HandleHandUpdated()
		{
			Pose identity = Pose.identity;
			float num = 0f;
			foreach (HandJointsPose.WeightedJoint weightedJoint in this.WeightedJoints)
			{
				Pose pose;
				if (!this.Hand.GetJointPose(weightedJoint.handJointId, out pose))
				{
					return;
				}
				float t = weightedJoint.weight / (num + weightedJoint.weight);
				num += weightedJoint.weight;
				ref identity.Lerp(pose, t);
			}
			this.GetOffset(ref this._cachedPose, this.Hand.Handedness, this.Hand.Scale);
			ref this._cachedPose.Postmultiply(identity);
			base.transform.SetPose(identity, Space.World);
		}

		private void GetOffset(ref Pose pose, Handedness handedness, float scale)
		{
			if (this._mirrorOffsetsForLeftHand && handedness == Handedness.Left)
			{
				Vector3 vector = this.LocalPositionOffset * scale;
				pose.position = HandMirroring.Mirror(vector);
				Quaternion rotationOffset = this.RotationOffset;
				pose.rotation = HandMirroring.Mirror(rotationOffset);
				return;
			}
			pose.position = this.LocalPositionOffset * scale;
			pose.rotation = this.RotationOffset;
		}

		public void InjectAllHandJoint(IHand hand)
		{
			this.InjectHand(hand);
		}

		public void InjectHand(IHand hand)
		{
			this._hand = (hand as Object);
			this.Hand = hand;
		}

		[SerializeField]
		[Interface(typeof(IHand), new Type[]
		{

		})]
		private Object _hand;

		[SerializeField]
		[InspectorName("Weighted Joints")]
		private List<HandJointsPose.WeightedJoint> _weightedJoints;

		[SerializeField]
		[InspectorName("Offset")]
		private Vector3 _localPositionOffset;

		[SerializeField]
		[InspectorName("Rotation")]
		private Quaternion _rotationOffset = Quaternion.identity;

		[SerializeField]
		[InspectorName("Weighted Joints")]
		private List<HandJointsPose.WeightedJoint> _joints;

		[SerializeField]
		[InspectorName("Offset")]
		private Vector3 _posOffset;

		[SerializeField]
		[InspectorName("Rotation")]
		private Quaternion _rotOffset = Quaternion.identity;

		[SerializeField]
		[Tooltip("When the attached hand's handedness is set to Left, this property will mirror the offsets. This allows for offset values to be set in Right hand coordinates for both Left and Right hands.")]
		private bool _mirrorOffsetsForLeftHand = true;

		private Pose _cachedPose = Pose.identity;

		protected bool _started;

		[Serializable]
		public struct WeightedJoint
		{
			public HandJointId handJointId;

			public float weight;
		}
	}
}
