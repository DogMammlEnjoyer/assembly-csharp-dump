using System;
using Oculus.Interaction.GrabAPI;
using Oculus.Interaction.Input;
using UnityEngine;

namespace Oculus.Interaction
{
	public class HandPinchOffset : MonoBehaviour
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
			Vector3 position = this._handGrabApi.GetPinchCenter();
			if (this._collider != null)
			{
				position = this._collider.ClosestPoint(position);
			}
			Pose pose;
			this.Hand.GetRootPose(out pose);
			Pose pose2 = new Pose(position, pose.rotation);
			this.GetOffset(ref this._cachedPose, this.Hand.Handedness, this.Hand.Scale);
			ref this._cachedPose.Postmultiply(pose2);
			base.transform.SetPose(this._cachedPose, Space.World);
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

		public void InjectAllHandPinchOffset(IHand hand, HandGrabAPI handGrabApi)
		{
			this.InjectHand(hand);
			this.InjectHandGrabAPI(handGrabApi);
		}

		public void InjectHand(IHand hand)
		{
			this.Hand = hand;
			this._hand = (hand as Object);
		}

		public void InjectHandGrabAPI(HandGrabAPI handGrabApi)
		{
			this._handGrabApi = handGrabApi;
		}

		public void InjectOptionalCollider(Collider collider)
		{
			this._collider = collider;
		}

		[SerializeField]
		[Interface(typeof(IHand), new Type[]
		{

		})]
		private Object _hand;

		[SerializeField]
		private HandGrabAPI _handGrabApi;

		[SerializeField]
		[Optional]
		private Collider _collider;

		[SerializeField]
		[InspectorName("Offset")]
		private Vector3 _localPositionOffset;

		[SerializeField]
		[InspectorName("Rotation")]
		private Quaternion _rotationOffset = Quaternion.identity;

		[SerializeField]
		[InspectorName("Offset")]
		private Vector3 _posOffset;

		[SerializeField]
		[InspectorName("Rotation")]
		private Quaternion _rotOffset = Quaternion.identity;

		[SerializeField]
		[Tooltip("When the attached hand's handedness is set to Left, this property will mirror the offsets. This allows for offset values to be set in Right hand coordinates for both Left and Right hands.")]
		private bool _mirrorOffsetsForLeftHand = true;

		protected bool _started;

		private Pose _cachedPose = Pose.identity;
	}
}
