using System;
using Oculus.Interaction.Input;
using UnityEngine;

namespace Oculus.Interaction
{
	public class HandJoint : MonoBehaviour
	{
		public IHand Hand { get; private set; }

		[Obsolete("This property is provided for backwards compatibility only, and its function will be removed in a future version of Interaction SDK.")]
		public bool UseLegacyOrientation
		{
			get
			{
				return this._useLegacyOrientation;
			}
			set
			{
				this._useLegacyOrientation = value;
			}
		}

		public bool FreezeRotationX
		{
			get
			{
				return this._freezeRotationX;
			}
			set
			{
				this._freezeRotationX = value;
			}
		}

		public bool FreezeRotationY
		{
			get
			{
				return this._freezeRotationY;
			}
			set
			{
				this._freezeRotationY = value;
			}
		}

		public bool FreezeRotationZ
		{
			get
			{
				return this._freezeRotationZ;
			}
			set
			{
				this._freezeRotationZ = value;
			}
		}

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

		public HandJointId HandJointId
		{
			get
			{
				return this._jointId;
			}
			set
			{
				this._jointId = value;
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
			Pose pose;
			if (this.Hand.GetJointPose(this.HandJointId, out pose))
			{
				this.GetOffset(ref this._cachedPose, this.Hand.Handedness, this.Hand.Scale);
				ref this._cachedPose.Postmultiply(pose);
				if (this.UseLegacyOrientation)
				{
					this._cachedPose.rotation = pose.rotation * ((this.Hand.Handedness == Handedness.Left) ? Quaternion.Euler(HandJoint.LEFT_LEGACY_ROT) : Quaternion.Euler(HandJoint.RIGHT_LEGACY_ROT));
				}
				this._cachedPose.rotation = this.FreezeRotation(this._cachedPose.rotation);
				base.transform.SetPose(this._cachedPose, Space.World);
			}
		}

		private Quaternion FreezeRotation(Quaternion rotation)
		{
			if (this._freezeRotationX || this._freezeRotationY || this._freezeRotationZ)
			{
				Vector3 eulerAngles = rotation.eulerAngles;
				Quaternion rhs = Quaternion.Euler(new Vector3(eulerAngles.x, 0f, 0f));
				Quaternion rhs2 = Quaternion.Euler(new Vector3(0f, eulerAngles.y, 0f));
				Quaternion rhs3 = Quaternion.Euler(new Vector3(0f, 0f, eulerAngles.z));
				Quaternion quaternion = Quaternion.identity;
				if (!this._freezeRotationY)
				{
					quaternion *= rhs2;
				}
				if (!this._freezeRotationX)
				{
					quaternion *= rhs;
				}
				if (!this._freezeRotationZ)
				{
					quaternion *= rhs3;
				}
				rotation = quaternion;
			}
			return rotation;
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
		private HandJointId _handJointId;

		[SerializeField]
		[InspectorName("Offset")]
		private Vector3 _localPositionOffset;

		[SerializeField]
		[InspectorName("Rotation")]
		private Quaternion _rotationOffset = Quaternion.identity;

		[SerializeField]
		private HandJointId _jointId;

		[SerializeField]
		[InspectorName("Offset")]
		private Vector3 _posOffset;

		[SerializeField]
		[InspectorName("Rotation")]
		private Quaternion _rotOffset = Quaternion.identity;

		[Tooltip("Provided for backwards compatibility. When set, the rotation of the driven transform for this component will match the legacy hand skeleton joint orientation rather than the current OpenXR joint orientation.")]
		[SerializeField]
		private bool _useLegacyOrientation;

		private static readonly Vector3 LEFT_LEGACY_ROT = new Vector3(180f, 90f, 0f);

		private static readonly Vector3 RIGHT_LEGACY_ROT = new Vector3(0f, -90f, 0f);

		[SerializeField]
		[Tooltip("When the attached hand's handedness is set to Left, this property will mirror the offsets. This allows for offset values to be set in Right hand coordinates for both Left and Right hands.")]
		private bool _mirrorOffsetsForLeftHand = true;

		[Header("Freeze rotations")]
		[SerializeField]
		private bool _freezeRotationX;

		[SerializeField]
		private bool _freezeRotationY;

		[SerializeField]
		private bool _freezeRotationZ;

		private Pose _cachedPose = Pose.identity;

		protected bool _started;
	}
}
