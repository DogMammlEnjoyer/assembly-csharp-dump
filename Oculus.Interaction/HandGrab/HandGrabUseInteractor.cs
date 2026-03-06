using System;
using Oculus.Interaction.Grab;
using Oculus.Interaction.GrabAPI;
using Oculus.Interaction.Input;
using UnityEngine;

namespace Oculus.Interaction.HandGrab
{
	public class HandGrabUseInteractor : Interactor<HandGrabUseInteractor, HandGrabUseInteractable>, IHandGrabState
	{
		public IHand Hand { get; private set; }

		public IFingerUseAPI UseAPI { get; private set; }

		public HandGrabTarget HandGrabTarget { get; } = new HandGrabTarget();

		public bool IsGrabbing
		{
			get
			{
				return base.SelectedInteractable != null;
			}
		}

		public float WristStrength
		{
			get
			{
				return 0f;
			}
		}

		public float FingersStrength
		{
			get
			{
				if (!this.IsGrabbing)
				{
					return 0f;
				}
				return 1f;
			}
		}

		public Pose WristToGrabPoseOffset
		{
			get
			{
				return Pose.identity;
			}
		}

		public Action<IHandGrabState> WhenHandGrabStarted { get; set; } = delegate(IHandGrabState <p0>)
		{
		};

		public Action<IHandGrabState> WhenHandGrabEnded { get; set; } = delegate(IHandGrabState <p0>)
		{
		};

		protected override bool ComputeShouldSelect()
		{
			return this._handUseShouldSelect;
		}

		protected override bool ComputeShouldUnselect()
		{
			return this._handUseShouldUnselect || base.SelectedInteractable == null;
		}

		protected override void Awake()
		{
			base.Awake();
			this.Hand = (this._hand as IHand);
			this.UseAPI = (this._useAPI as IFingerUseAPI);
			this._nativeId = 5208257256664429413UL;
		}

		protected override void Start()
		{
			this.BeginStart(ref this._started, delegate
			{
				base.Start();
			});
			this.EndStart(ref this._started);
		}

		protected override void InteractableSelected(HandGrabUseInteractable interactable)
		{
			base.InteractableSelected(interactable);
			this.StartUsing();
		}

		protected override void InteractableUnselected(HandGrabUseInteractable interactable)
		{
			base.InteractableUnselected(interactable);
			this._fingersInUse = HandFingerFlags.None;
		}

		private void StartUsing()
		{
			HandGrabResult handGrabResult = new HandGrabResult
			{
				HasHandPose = true,
				HandPose = this._relaxedHandPose
			};
			this.HandGrabTarget.Set(base.SelectedInteractable.transform, HandAlignType.AlignOnGrab, GrabTypeFlags.None, handGrabResult);
		}

		protected override void DoHoverUpdate()
		{
			base.DoHoverUpdate();
			this._handUseShouldSelect = this.IsUsingInteractable(base.Interactable);
		}

		protected override void DoSelectUpdate()
		{
			base.DoSelectUpdate();
			if (base.SelectedInteractable == null)
			{
				return;
			}
			float strength = this.CalculateUseStrength(ref this._fingerUseStrength);
			float useProgress = base.SelectedInteractable.ComputeUseStrength(strength);
			this._handUseShouldUnselect = !this.IsUsingInteractable(base.Interactable);
			if (this._usesHandPose && !this._handUseShouldUnselect)
			{
				this.MoveFingers(ref this._fingerUseStrength, useProgress);
			}
		}

		private bool IsUsingInteractable(HandGrabUseInteractable interactable)
		{
			if (interactable == null)
			{
				return false;
			}
			for (int i = 0; i < 5; i++)
			{
				HandFinger handFinger = (HandFinger)i;
				if (interactable.UseFingers[handFinger] != FingerRequirement.Ignored && this.UseAPI.GetFingerUseStrength(handFinger) > interactable.StrengthDeadzone)
				{
					return true;
				}
			}
			return false;
		}

		private float CalculateUseStrength(ref float[] fingerUseStrength)
		{
			float num = 1f;
			float num2 = 0f;
			bool flag = false;
			for (int i = 0; i < 5; i++)
			{
				HandFinger handFinger = (HandFinger)i;
				if (base.SelectedInteractable.UseFingers[handFinger] == FingerRequirement.Ignored)
				{
					fingerUseStrength[i] = 0f;
				}
				else
				{
					float fingerUseStrength2 = this.UseAPI.GetFingerUseStrength(handFinger);
					fingerUseStrength[i] = Mathf.Clamp01((fingerUseStrength2 - base.SelectedInteractable.UseStrengthDeadZone) / (1f - base.SelectedInteractable.UseStrengthDeadZone));
					if (base.SelectedInteractable.UseFingers[handFinger] == FingerRequirement.Required)
					{
						flag = true;
						num = Mathf.Min(num, fingerUseStrength[i]);
					}
					else if (base.SelectedInteractable.UseFingers[handFinger] == FingerRequirement.Optional)
					{
						num2 = Mathf.Max(num2, fingerUseStrength[i]);
					}
					if (fingerUseStrength[i] > 0f)
					{
						this.MarkFingerInUse(handFinger);
					}
					else
					{
						this.UnmarkFingerInUse(handFinger);
					}
				}
			}
			if (!flag)
			{
				return num2;
			}
			return num;
		}

		private void MoveFingers(ref float[] fingerUseProgress, float useProgress)
		{
			for (int i = 0; i < 5; i++)
			{
				HandFinger finger = (HandFinger)i;
				float t = Mathf.Min(useProgress, fingerUseProgress[i]);
				this.LerpFingerRotation(this._relaxedHandPose.JointRotations, this._tightHandPose.JointRotations, this.HandGrabTarget.HandPose.JointRotations, finger, t);
			}
		}

		private void MarkFingerInUse(HandFinger finger)
		{
			this._fingersInUse |= (HandFingerFlags)(1 << (int)finger);
		}

		private void UnmarkFingerInUse(HandFinger finger)
		{
			this._fingersInUse &= (HandFingerFlags)(~(HandFingerFlags)(1 << (int)finger));
		}

		private void LerpFingerRotation(Quaternion[] from, Quaternion[] to, Quaternion[] result, HandFinger finger, float t)
		{
			foreach (int num in FingersMetadata.FINGER_TO_JOINT_INDEX[(int)finger])
			{
				result[num] = Quaternion.Slerp(from[num], to[num], t);
			}
		}

		public HandFingerFlags GrabbingFingers()
		{
			return this._fingersInUse;
		}

		protected override HandGrabUseInteractable ComputeCandidate()
		{
			float num = float.NegativeInfinity;
			HandGrabUseInteractable result = null;
			this._usesHandPose = false;
			foreach (HandGrabUseInteractable handGrabUseInteractable in Interactable<HandGrabUseInteractor, HandGrabUseInteractable>.Registry.List(this))
			{
				float num2;
				handGrabUseInteractable.FindBestHandPoses((this.Hand != null) ? this.Hand.Scale : 1f, ref this._cachedRelaxedHandPose, ref this._cachedTightHandPose, out num2);
				if (num2 > num)
				{
					num = num2;
					result = handGrabUseInteractable;
					this._relaxedHandPose.CopyFrom(this._cachedRelaxedHandPose, false);
					this._tightHandPose.CopyFrom(this._cachedTightHandPose, false);
					this._usesHandPose = true;
				}
			}
			return result;
		}

		public void InjectAllHandGrabUseInteractor(IFingerUseAPI useApi)
		{
			this.InjectUseApi(useApi);
		}

		public void InjectUseApi(IFingerUseAPI useApi)
		{
			this._useAPI = (useApi as Object);
			this.UseAPI = useApi;
		}

		public void InjectOptionalHand(IHand hand)
		{
			this._hand = (hand as Object);
			this.Hand = hand;
		}

		[Tooltip("The hand to use.")]
		[SerializeField]
		[Optional]
		[Interface(typeof(IHand), new Type[]
		{

		})]
		private Object _hand;

		[Tooltip("API that gets the finger use strength.")]
		[SerializeField]
		[Interface(typeof(IFingerUseAPI), new Type[]
		{

		})]
		private Object _useAPI;

		private HandPose _relaxedHandPose = new HandPose();

		private HandPose _tightHandPose = new HandPose();

		private HandPose _cachedRelaxedHandPose = new HandPose();

		private HandPose _cachedTightHandPose = new HandPose();

		private HandFingerFlags _fingersInUse;

		private float[] _fingerUseStrength = new float[5];

		private bool _usesHandPose;

		private bool _handUseShouldSelect;

		private bool _handUseShouldUnselect;
	}
}
