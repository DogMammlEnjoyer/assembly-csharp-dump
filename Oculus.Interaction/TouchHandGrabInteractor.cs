using System;
using Oculus.Interaction.Input;
using Oculus.Interaction.PoseDetection;
using UnityEngine;

namespace Oculus.Interaction
{
	public class TouchHandGrabInteractor : PointerInteractor<TouchHandGrabInteractor, TouchHandGrabInteractable>, ITimeConsumer
	{
		private IHand Hand { get; set; }

		private IHand OpenHand { get; set; }

		public event Action WhenFingerLocked = delegate()
		{
		};

		public void SetTimeProvider(Func<float> timeProvider)
		{
			this._timeProvider = timeProvider;
		}

		private Vector3 GrabPosition
		{
			get
			{
				return this._grabLocation.position;
			}
		}

		private Quaternion GrabRotation
		{
			get
			{
				return this._grabLocation.rotation;
			}
		}

		protected override void Awake()
		{
			base.Awake();
			this.Hand = (this._hand as IHand);
			this.OpenHand = (this._openHand as IHand);
			this.HandSphereMap = (this._handSphereMap as IHandSphereMap);
			this.GrabPrerequisite = (this._grabPrerequisite as IActiveState);
			this._nativeId = 6084210691412554338UL;
			this._fingerStatuses = new TouchHandGrabInteractor.FingerStatus[5];
			for (int i = 0; i < 5; i++)
			{
				int[] array = FingersMetadata.FINGER_TO_JOINT_INDEX[i];
				HandJointId[] array2 = new HandJointId[array.Length];
				for (int j = 0; j < array.Length; j++)
				{
					array2[j] = FingersMetadata.HAND_JOINT_IDS[array[j]];
				}
				this._fingerStatuses[i] = new TouchHandGrabInteractor.FingerStatus
				{
					Joints = array2,
					LocalJoints = new Pose[array2.Length]
				};
			}
		}

		protected override void Start()
		{
			base.Start();
			this._touchShadowHand = new TouchShadowHand(this.HandSphereMap, this.Hand.Handedness, this._iterations);
			this._fromShadow.FromHand(this.Hand, false);
			this._toShadow.FromHand(this.Hand, false);
			this._previousTime = this._timeProvider();
			this._deltaTime = 0f;
		}

		public bool IsFingerLocked(HandFinger finger)
		{
			return (base.State != InteractorState.Select || !(this._selectedInteractable == null)) && this._fingerStatuses[(int)finger].Locked;
		}

		public Pose[] GetFingerJoints(HandFinger finger)
		{
			return this._fingerStatuses[(int)finger].LocalJoints;
		}

		protected override void DoPreprocess()
		{
			base.DoPreprocess();
			this._toShadow.FromHand(this.Hand, false);
			float previousTime = this._timeProvider();
			this._deltaTime = this._timeProvider() - this._previousTime;
			this._previousTime = previousTime;
		}

		protected override void DoPostprocess()
		{
			if (base.State != InteractorState.Select && this._interactable != null)
			{
				this._fromShadow.FromHand(this.Hand, false);
			}
			else
			{
				this._fromShadow.FromHandRoot(this.Hand);
				for (int i = 0; i < 5; i++)
				{
					TouchHandGrabInteractor.FingerStatus fingerStatus = this._fingerStatuses[i];
					if (!fingerStatus.Locked)
					{
						for (int j = 0; j < fingerStatus.Joints.Length; j++)
						{
							HandJointId handJointId = fingerStatus.Joints[j];
							Pose pose;
							if (this.Hand.GetJointPoseLocal(handJointId, out pose))
							{
								this._fromShadow.SetLocalPose(handJointId, pose);
							}
						}
					}
				}
			}
			base.DoPostprocess();
		}

		protected override bool ComputeShouldSelect()
		{
			return this.HandStatusSelecting();
		}

		protected override bool ComputeShouldUnselect()
		{
			return !this.HandStatusSelecting();
		}

		protected override void DoHoverUpdate()
		{
			TouchHandGrabInteractable interactable = this._interactable;
			if (interactable == null)
			{
				return;
			}
			TouchShadowHand.GrabTouchInfo grabTouchInfo = new TouchShadowHand.GrabTouchInfo();
			this._touchShadowHand.GrabTouch(this._fromShadow, this._toShadow, interactable.ColliderGroup, false, grabTouchInfo);
			if (!grabTouchInfo.grabbing)
			{
				this._touchShadowHand.GrabTouch(this._fromShadow, this._toShadow, interactable.ColliderGroup, true, grabTouchInfo);
			}
			if (!grabTouchInfo.grabbing)
			{
				return;
			}
			this._touchShadowHand.SetShadowRootFromHands(this._fromShadow, this._toShadow, grabTouchInfo.grabT);
			for (int i = 0; i < this._fingerStatuses.Length; i++)
			{
				TouchHandGrabInteractor.FingerStatus fingerStatus = this._fingerStatuses[i];
				this.ComputeNewTouching(i, this._interactable.ColliderGroup, grabTouchInfo.offset);
				if (grabTouchInfo.grabbingFingers[i] && !this._fingerStatuses[i].Locked)
				{
					this._openShadow.FromHand(this.OpenHand, this.OpenHand.Handedness != this.Hand.Handedness);
					if (this._touchShadowHand.PushoutFinger(i, this._fromShadow, this._openShadow, this._interactable.ColliderGroup, grabTouchInfo.offset))
					{
						for (int j = 0; j < fingerStatus.Joints.Length; j++)
						{
							HandJointId handJointId = fingerStatus.Joints[j];
							this._fromShadow.SetLocalPose(handJointId, this._touchShadowHand.ShadowHand.GetLocalPose(handJointId));
						}
						this.ComputeNewTouching(i, this._interactable.ColliderGroup, grabTouchInfo.offset);
					}
				}
			}
			if (!this.HandStatusSelecting())
			{
				this.ClearFingerLockStatuses();
			}
			else
			{
				this.GrabOffset = Vector3.zero;
				this._saveOffset = Quaternion.Inverse(this.GrabRotation) * grabTouchInfo.offset;
				this._firstSelect = true;
			}
			this.WhenFingerLocked();
		}

		private bool MeetsGrabPrerequisite()
		{
			return this.GrabPrerequisite == null || this.GrabPrerequisite.Active;
		}

		private bool HandStatusSelecting()
		{
			return this.MeetsGrabPrerequisite() && this._fingerStatuses[0].Selecting && (this._fingerStatuses[1].Selecting || this._fingerStatuses[2].Selecting || this._fingerStatuses[3].Selecting || this._fingerStatuses[4].Selecting);
		}

		private void ComputeNewTouching(int idx, ColliderGroup colliderGroup, Vector3 offset)
		{
			TouchHandGrabInteractor.FingerStatus fingerStatus = this._fingerStatuses[idx];
			if (fingerStatus.Locked)
			{
				return;
			}
			this._touchShadowHand.SetShadowFingerFrom(idx, this._fromShadow);
			if (this._touchShadowHand.CheckFingerTouch(idx, 0, colliderGroup, offset, null))
			{
				return;
			}
			if (!this._touchShadowHand.GrabConformFinger(idx, this._fromShadow, this._toShadow, colliderGroup, offset))
			{
				return;
			}
			fingerStatus.Locked = true;
			fingerStatus.Selecting = true;
			fingerStatus.Timer = 0f;
			this._touchShadowHand.GetJointsFromShadow(fingerStatus.Joints, fingerStatus.LocalJoints, true);
			Pose[] array = new Pose[fingerStatus.Joints.Length];
			for (int i = 0; i < fingerStatus.Joints.Length; i++)
			{
				array[i] = this._touchShadowHand.ShadowHand.GetWorldPose(fingerStatus.Joints[i]);
			}
			fingerStatus.CurlValueAtLock = FingerShapes.PosesListCurlValue(array);
			for (int j = 0; j < fingerStatus.Joints.Length; j++)
			{
				HandJointId handJointId = fingerStatus.Joints[j];
				this._fromShadow.SetLocalPose(handJointId, this._touchShadowHand.ShadowHand.GetLocalPose(handJointId));
			}
		}

		private void ComputeNewRelease(int idx, ColliderGroup colliderGroup, Vector3 offset)
		{
			TouchHandGrabInteractor.FingerStatus fingerStatus = this._fingerStatuses[idx];
			if (!fingerStatus.Locked)
			{
				return;
			}
			Pose[] array = new Pose[fingerStatus.Joints.Length];
			for (int i = 0; i < fingerStatus.Joints.Length; i++)
			{
				array[i] = this._toShadow.GetWorldPose(fingerStatus.Joints[i]);
			}
			if (FingerShapes.PosesListCurlValue(array) >= fingerStatus.CurlValueAtLock - this._curlDeltaThreshold)
			{
				fingerStatus.Timer = 0f;
				return;
			}
			if (!this._touchShadowHand.GrabReleaseFinger(idx, this._fromShadow, this._toShadow, colliderGroup, offset))
			{
				fingerStatus.Timer = 0f;
				return;
			}
			fingerStatus.Timer += this._deltaTime;
			if (fingerStatus.Timer < this._curlTimeThreshold)
			{
				return;
			}
			fingerStatus.Locked = false;
			fingerStatus.Selecting = false;
		}

		protected override void DoSelectUpdate()
		{
			if (this._firstSelect)
			{
				this.GrabOffset = this._saveOffset;
				this._saveOffset = Vector3.zero;
				this._firstSelect = false;
				return;
			}
			TouchHandGrabInteractable selectedInteractable = this._selectedInteractable;
			if (selectedInteractable == null)
			{
				for (int i = 0; i < this._fingerStatuses.Length; i++)
				{
					TouchHandGrabInteractor.FingerStatus fingerStatus = this._fingerStatuses[i];
					if (fingerStatus.Locked)
					{
						fingerStatus.Selecting = true;
						fingerStatus.Locked = false;
						fingerStatus.Timer = 0f;
						Pose[] array = new Pose[fingerStatus.Joints.Length];
						for (int j = 0; j < fingerStatus.Joints.Length; j++)
						{
							array[j] = this._toShadow.GetWorldPose(fingerStatus.Joints[j]);
						}
						fingerStatus.CurlValueAtLock = FingerShapes.PosesListCurlValue(array);
					}
				}
				for (int k = 0; k < this._fingerStatuses.Length; k++)
				{
					TouchHandGrabInteractor.FingerStatus fingerStatus2 = this._fingerStatuses[k];
					if (fingerStatus2.Selecting)
					{
						Pose[] array2 = new Pose[fingerStatus2.Joints.Length];
						for (int l = 0; l < fingerStatus2.Joints.Length; l++)
						{
							array2[l] = this._toShadow.GetWorldPose(fingerStatus2.Joints[l]);
						}
						if (FingerShapes.PosesListCurlValue(array2) >= fingerStatus2.CurlValueAtLock - this._curlDeltaThreshold)
						{
							fingerStatus2.Timer = 0f;
						}
						else
						{
							fingerStatus2.Timer += this._deltaTime;
							if (fingerStatus2.Timer < this._curlTimeThreshold)
							{
								return;
							}
							fingerStatus2.Selecting = false;
						}
					}
				}
				return;
			}
			this._touchShadowHand.ShadowHand.Copy(this._fromShadow);
			this._touchShadowHand.SetShadowRootFromHand(this._fromShadow);
			if (this.MeetsGrabPrerequisite())
			{
				for (int m = 0; m < this._fingerStatuses.Length; m++)
				{
					if (this._fingerStatuses[m].Locked)
					{
						this.ComputeNewRelease(m, selectedInteractable.ColliderGroup, Vector3.zero);
					}
					else
					{
						this.ComputeNewTouching(m, selectedInteractable.ColliderGroup, Vector3.zero);
					}
				}
			}
			this.WhenFingerLocked();
		}

		public override void Unselect()
		{
			if (!base.ShouldUnselect)
			{
				base.Unselect();
				return;
			}
			this.ClearFingerLockStatuses();
			this.GrabOffset = Vector3.zero;
			this.WhenFingerLocked();
			base.Unselect();
		}

		private void ClearFingerLockStatuses()
		{
			for (int i = 0; i < this._fingerStatuses.Length; i++)
			{
				this._fingerStatuses[i].Locked = false;
				this._fingerStatuses[i].Selecting = false;
			}
		}

		protected override TouchHandGrabInteractable ComputeCandidate()
		{
			TouchHandGrabInteractable result = null;
			float num = float.MaxValue;
			foreach (TouchHandGrabInteractable touchHandGrabInteractable in Interactable<TouchHandGrabInteractor, TouchHandGrabInteractable>.Registry.List())
			{
				foreach (Collider collider in touchHandGrabInteractable.ColliderGroup.Colliders)
				{
					float sqrMagnitude = (collider.ClosestPoint(this._hoverLocation.position) - this._hoverLocation.position).sqrMagnitude;
					if (sqrMagnitude < num && sqrMagnitude < this._minHoverDistance * this._minHoverDistance)
					{
						num = sqrMagnitude;
						result = touchHandGrabInteractable;
					}
				}
			}
			return result;
		}

		protected override Pose ComputePointerPose()
		{
			return new Pose(this.GrabPosition + this.GrabRotation * this.GrabOffset, this.GrabRotation);
		}

		public void InjectAllTouchHandGrabInteractor(IHand hand, IHand openHand, IHandSphereMap handSphereMap, Transform hoverLocation, Transform grabLocation)
		{
			this.InjectHand(hand);
			this.InjectOpenHand(openHand);
			this.InjectHandSphereMap(handSphereMap);
			this.InjectHoverLocation(hoverLocation);
			this.InjectGrabLocation(grabLocation);
		}

		public void InjectHand(IHand hand)
		{
			this.Hand = hand;
			this._hand = (hand as Object);
		}

		public void InjectOpenHand(IHand openHand)
		{
			this.OpenHand = openHand;
			this._openHand = (openHand as Object);
		}

		public void InjectHandSphereMap(IHandSphereMap handSphereMap)
		{
			this.HandSphereMap = handSphereMap;
			this._handSphereMap = (handSphereMap as Object);
		}

		public void InjectHoverLocation(Transform hoverLocation)
		{
			this._hoverLocation = hoverLocation;
		}

		public void InjectGrabLocation(Transform grabLocation)
		{
			this._grabLocation = grabLocation;
		}

		public void InjectOptionalGrabPrerequisite(IActiveState grabPrerequisite)
		{
			this.GrabPrerequisite = grabPrerequisite;
			this._grabPrerequisite = (grabPrerequisite as Object);
		}

		public void InjectOptionalMinHoverDistance(float minHoverDistance)
		{
			this._minHoverDistance = minHoverDistance;
		}

		public void InjectOptionalCurlDeltaThreshold(float threshold)
		{
			this._curlDeltaThreshold = threshold;
		}

		public void InjectOptionalCurlTimeThreshold(float seconds)
		{
			this._curlTimeThreshold = seconds;
		}

		public void InjectOptionalIterations(int iterations)
		{
			this._iterations = iterations;
		}

		[Obsolete("Use SetTimeProvide()")]
		public void InjectOptionalTimeProvider(Func<float> timeProvider)
		{
			this._timeProvider = timeProvider;
		}

		[SerializeField]
		[Interface(typeof(IHand), new Type[]
		{

		})]
		private Object _hand;

		[SerializeField]
		[Interface(typeof(IHand), new Type[]
		{

		})]
		private Object _openHand;

		[SerializeField]
		[Interface(typeof(IHandSphereMap), new Type[]
		{

		})]
		private Object _handSphereMap;

		protected IHandSphereMap HandSphereMap;

		[SerializeField]
		private Transform _hoverLocation;

		[SerializeField]
		private Transform _grabLocation;

		[SerializeField]
		private float _minHoverDistance = 0.05f;

		[SerializeField]
		private float _curlDeltaThreshold = 3f;

		[SerializeField]
		private float _curlTimeThreshold = 0.05f;

		[SerializeField]
		[Min(1f)]
		private int _iterations = 10;

		[SerializeField]
		[Interface(typeof(IActiveState), new Type[]
		{

		})]
		[Optional]
		private Object _grabPrerequisite;

		private Func<float> _timeProvider = () => Time.time;

		private Vector3 _saveOffset = Vector3.zero;

		private Vector3 GrabOffset = Vector3.zero;

		protected IActiveState GrabPrerequisite;

		private TouchHandGrabInteractor.FingerStatus[] _fingerStatuses;

		private TouchShadowHand _touchShadowHand;

		private readonly ShadowHand _fromShadow = new ShadowHand();

		private readonly ShadowHand _toShadow = new ShadowHand();

		private readonly ShadowHand _openShadow = new ShadowHand();

		private bool _firstSelect;

		private float _previousTime;

		private float _deltaTime;

		private class FingerStatus
		{
			public bool Locked;

			public bool Selecting;

			public HandJointId[] Joints;

			public Pose[] LocalJoints;

			public float CurlValueAtLock;

			public float Timer;
		}
	}
}
