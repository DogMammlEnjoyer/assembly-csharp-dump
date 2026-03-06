using System;
using Oculus.Interaction.Input;
using UnityEngine;

namespace Oculus.Interaction
{
	public class IndexPinchSafeReleaseSelector : MonoBehaviour, ISelector, IActiveState
	{
		public IHand Hand { get; private set; }

		public bool SelectOnRelease
		{
			get
			{
				return this._selectOnRelease;
			}
			set
			{
				this._selectOnRelease = value;
			}
		}

		public float SafeReleaseThreshold
		{
			get
			{
				return this._safeReleaseThreshold;
			}
			set
			{
				this._safeReleaseThreshold = value;
			}
		}

		public bool Active
		{
			get
			{
				return this._active;
			}
		}

		public event Action WhenSelected = delegate()
		{
		};

		public event Action WhenUnselected = delegate()
		{
		};

		protected virtual void Awake()
		{
			if (this.Hand == null)
			{
				this.Hand = (this._hand as IHand);
			}
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
				this._wasPinching = this.Hand.GetIndexFingerIsPinching();
			}
		}

		protected virtual void OnDisable()
		{
			if (this._started)
			{
				this.Hand.WhenHandUpdated -= this.HandleHandUpdated;
				if (this._active)
				{
					this._active = false;
					this.WhenUnselected();
				}
				this._pendingUnselect = false;
			}
		}

		private void HandleHandUpdated()
		{
			if (this._selectOnRelease && this._pendingUnselect)
			{
				this._pendingUnselect = false;
				this.WhenUnselected();
			}
			bool indexFingerIsPinching = this.Hand.GetIndexFingerIsPinching();
			if (this._wasPinching != indexFingerIsPinching)
			{
				this._wasPinching = indexFingerIsPinching;
				if (indexFingerIsPinching)
				{
					this._active = true;
					if (!this._selectOnRelease)
					{
						this.WhenSelected();
					}
				}
			}
			if (this._active && !indexFingerIsPinching && this.IsIndexExtended())
			{
				if (this._selectOnRelease)
				{
					this.WhenSelected();
					this._pendingUnselect = true;
				}
				else
				{
					this.WhenUnselected();
				}
				this._active = false;
			}
		}

		protected virtual bool IsIndexExtended()
		{
			if (this.Hand.GetFingerPinchStrength(HandFinger.Index) == 0f)
			{
				return true;
			}
			Pose pose;
			Pose pose2;
			Pose pose3;
			if (!this.Hand.GetJointPoseFromWrist(HandJointId.HandIndex1, out pose) || !this.Hand.GetJointPoseFromWrist(HandJointId.HandIndex2, out pose2) || !this.Hand.GetJointPoseFromWrist(HandJointId.HandIndexTip, out pose3))
			{
				return true;
			}
			Vector3 normalized = (pose2.position - pose.position).normalized;
			Vector3 normalized2 = (pose3.position - pose2.position).normalized;
			return Vector3.Dot(normalized, normalized2) >= this._safeReleaseThreshold;
		}

		[Obsolete("Disable the component to Cancel any ongoing pinch")]
		public void Cancel()
		{
		}

		public void InjectAllIndexPinchSafeReleaseSelector(IHand hand)
		{
			this.InjectHand(hand);
		}

		[Obsolete("Use SelectOnRelease setter instead.")]
		public void InjectSelectOnRelease(bool selectOnRelease)
		{
		}

		public void InjectHand(IHand hand)
		{
			this._hand = (hand as Object);
			this.Hand = hand;
		}

		[Tooltip("The hand to check.")]
		[SerializeField]
		[Interface(typeof(IHand), new Type[]
		{

		})]
		private Object _hand;

		[Tooltip("If checked, the selector will select during the frame when the pinch is released as opposed to when it's pinching.")]
		[SerializeField]
		private bool _selectOnRelease = true;

		[Tooltip("Indicates how extended the index needs to be in order to be safe to unpinch.")]
		[SerializeField]
		[Range(-1f, 1f)]
		private float _safeReleaseThreshold = 0.5f;

		private bool _wasPinching;

		private bool _active;

		private bool _pendingUnselect;

		protected bool _started;
	}
}
