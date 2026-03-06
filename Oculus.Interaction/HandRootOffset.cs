using System;
using Oculus.Interaction.Input;
using UnityEngine;
using UnityEngine.Serialization;

namespace Oculus.Interaction
{
	public class HandRootOffset : MonoBehaviour
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

		public Vector3 Offset
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

		public Quaternion Rotation
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

		[Obsolete("Use MirrorOffsetsForLeftHand instead.")]
		public bool MirrorLeftRotation
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
			if (this.Hand.GetRootPose(out pose))
			{
				this.GetOffset(ref this._cachedPose);
				ref this._cachedPose.Postmultiply(pose);
				this._cachedPose.rotation = this.FreezeRotation(this._cachedPose.rotation);
				base.transform.SetPose(this._cachedPose, Space.World);
			}
		}

		public void GetOffset(ref Pose pose)
		{
			if (!this._started)
			{
				return;
			}
			this.GetOffset(ref pose, this.Hand.Handedness, this.Hand.Scale);
		}

		public void GetOffset(ref Pose pose, Handedness handedness, float scale)
		{
			if (this._mirrorOffsetsForLeftHand && handedness == Handedness.Left)
			{
				Vector3 offset = this.Offset;
				pose.position = HandMirroring.Mirror(offset) * scale;
				Quaternion rotation = this.Rotation;
				pose.rotation = HandMirroring.Mirror(rotation);
				return;
			}
			pose.position = this.Offset * scale;
			pose.rotation = this.Rotation;
		}

		public void GetWorldPose(ref Pose pose)
		{
			pose.position = base.transform.position;
			pose.rotation = base.transform.rotation;
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

		public void InjectAllHandRootOffset(IHand hand)
		{
			this.InjectHand(hand);
		}

		public void InjectHand(IHand hand)
		{
			this._hand = (hand as Object);
			this.Hand = hand;
		}

		[Obsolete("Use the Offset setter instead")]
		public void InjectOffset(Vector3 offset)
		{
			this.Offset = offset;
		}

		[Obsolete("Use the Rotation setter instead")]
		public void InjectRotation(Quaternion rotation)
		{
			this.Rotation = rotation;
		}

		[Obsolete("Use InjectAllHandRootOffset instead")]
		public void InjectAllHandWristOffset(IHand hand, Vector3 offset, Quaternion rotation)
		{
			this.InjectHand(hand);
			this.InjectOffset(offset);
			this.InjectRotation(rotation);
		}

		[SerializeField]
		[Interface(typeof(IHand), new Type[]
		{

		})]
		private Object _hand;

		[SerializeField]
		[InspectorName("Offset")]
		private Vector3 _offset;

		[SerializeField]
		[InspectorName("Rotation")]
		private Quaternion _rotation = Quaternion.identity;

		[SerializeField]
		[InspectorName("Offset")]
		private Vector3 _posOffset;

		[SerializeField]
		[InspectorName("Rotation")]
		private Quaternion _rotOffset = Quaternion.identity;

		[SerializeField]
		[FormerlySerializedAs("_mirrorLeftRotation")]
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
