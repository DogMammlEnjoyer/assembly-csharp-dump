using System;
using System.Collections;
using System.Collections.Generic;
using Oculus.Interaction.HandGrab.Visuals;
using Oculus.Interaction.Input;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

namespace Oculus.Interaction.HandGrab.Recorder
{
	public class HandGrabPoseLiveRecorder : MonoBehaviour, IActiveState
	{
		private HandGhostProvider GhostProvider
		{
			get
			{
				return this._handGhostProvider;
			}
		}

		private int CurrentStepIndex
		{
			get
			{
				return this._currentStepIndex;
			}
			set
			{
				this._currentStepIndex = value;
				UnityEvent<bool> whenCanUndo = this.WhenCanUndo;
				if (whenCanUndo != null)
				{
					whenCanUndo.Invoke(this._currentStepIndex >= 0);
				}
				UnityEvent<bool> whenCanRedo = this.WhenCanRedo;
				if (whenCanRedo == null)
				{
					return;
				}
				whenCanRedo.Invoke(this._currentStepIndex + 1 < this._recorderSteps.Count);
			}
		}

		public bool Active
		{
			get
			{
				return this._grabbingEnabled;
			}
		}

		private void Awake()
		{
			this._leftHand.InjectOptionalActiveState(this);
			this._rightHand.InjectOptionalActiveState(this);
		}

		private void Start()
		{
			this.ClearSnapshot();
			this._leftDetector = this._leftHand.Rigidbody.gameObject.AddComponent<RigidbodyDetector>();
			this._leftDetector.IgnoreBody(this._rightHand.Rigidbody);
			this._rightDetector = this._rightHand.Rigidbody.gameObject.AddComponent<RigidbodyDetector>();
			this._rightDetector.IgnoreBody(this._leftHand.Rigidbody);
			this.CurrentStepIndex = -1;
			this.EnableGrabbing(true);
		}

		public void Record()
		{
			this.ClearSnapshot();
			if (this._timerControl != null)
			{
				this._delayedSnapRoutine = base.StartCoroutine(this.DelayedSnapshot(this._timerControl.DelaySeconds));
				return;
			}
			this.TakeSnapshot();
		}

		private void ClearSnapshot()
		{
			if (this._delayedSnapRoutine != null)
			{
				base.StopCoroutine(this._delayedSnapRoutine);
				this._delayedSnapRoutine = null;
			}
			this._delayLabel.text = string.Empty;
		}

		private IEnumerator DelayedSnapshot(int seconds)
		{
			int num;
			for (int i = seconds; i > 0; i = num - 1)
			{
				this._delayLabel.text = i.ToString();
				UnityEvent whenTimeStep = this.WhenTimeStep;
				if (whenTimeStep != null)
				{
					whenTimeStep.Invoke();
				}
				yield return this._waitOneSeconds;
				num = i;
			}
			if (this.TakeSnapshot())
			{
				this._delayLabel.text = "<size=10>Snap!";
				UnityEvent whenSnapshot = this.WhenSnapshot;
				if (whenSnapshot != null)
				{
					whenSnapshot.Invoke();
				}
			}
			else
			{
				this._delayLabel.text = "<size=10>Error";
				UnityEvent whenError = this.WhenError;
				if (whenError != null)
				{
					whenError.Invoke();
				}
			}
			yield return this._waitOneSeconds;
			this._delayLabel.text = string.Empty;
			yield break;
		}

		private bool TakeSnapshot()
		{
			float num;
			Rigidbody rigidbody = this.FindNearestItem(this._leftHand.Rigidbody, this._leftDetector, out num);
			float num2;
			Rigidbody rigidbody2 = this.FindNearestItem(this._rightHand.Rigidbody, this._rightDetector, out num2);
			if (num < num2 && rigidbody != null)
			{
				return this.Record(this._leftHand.Hand, rigidbody);
			}
			if (rigidbody2 != null)
			{
				return this.Record(this._rightHand.Hand, rigidbody2);
			}
			Debug.LogError("No rigidbody detected near any hand");
			return false;
		}

		private Rigidbody FindNearestItem(Rigidbody handBody, RigidbodyDetector detector, out float bestDistance)
		{
			Vector3 worldCenterOfMass = handBody.worldCenterOfMass;
			float num = float.PositiveInfinity;
			Rigidbody result = null;
			foreach (Rigidbody rigidbody in detector.IntersectingBodies)
			{
				float num2 = Vector3.Distance(rigidbody.worldCenterOfMass, worldCenterOfMass);
				if (num2 < num)
				{
					num = num2;
					result = rigidbody;
				}
			}
			bestDistance = num;
			return result;
		}

		public void Undo()
		{
			if (this.CurrentStepIndex < 0)
			{
				return;
			}
			this._recorderSteps[this.CurrentStepIndex].ClearInteractable();
			int currentStepIndex = this.CurrentStepIndex;
			this.CurrentStepIndex = currentStepIndex - 1;
		}

		public void Redo()
		{
			if (this.CurrentStepIndex + 1 >= this._recorderSteps.Count)
			{
				return;
			}
			int currentStepIndex = this.CurrentStepIndex;
			this.CurrentStepIndex = currentStepIndex + 1;
			HandGrabPoseLiveRecorder.RecorderStep recorderStep = this._recorderSteps[this.CurrentStepIndex];
			HandGrabPose point;
			this.AddHandGrabPose(recorderStep, out recorderStep.interactable, out point);
			this.AttachGhost(point, recorderStep.HandScale);
			this._recorderSteps[this.CurrentStepIndex] = recorderStep;
		}

		public void EnableGrabbing(bool enable)
		{
			this._grabbingEnabled = enable;
			if (enable)
			{
				UnityEvent whenGrabAllowed = this.WhenGrabAllowed;
				if (whenGrabAllowed == null)
				{
					return;
				}
				whenGrabAllowed.Invoke();
				return;
			}
			else
			{
				UnityEvent whenGrabDisallowed = this.WhenGrabDisallowed;
				if (whenGrabDisallowed == null)
				{
					return;
				}
				whenGrabDisallowed.Invoke();
				return;
			}
		}

		private bool Record(IHand hand, Rigidbody item)
		{
			HandPose handPose = this.TrackedPose(hand);
			if (handPose == null)
			{
				Debug.LogError("Tracked Pose could not be retrieved", this);
				return false;
			}
			Pose to;
			if (!hand.GetRootPose(out to))
			{
				Debug.LogError("Hand Root pose could not be retrieved", this);
				return false;
			}
			Pose grabPoint = PoseUtils.DeltaScaled(item.transform, to);
			HandGrabPoseLiveRecorder.RecorderStep recorderStep = new HandGrabPoseLiveRecorder.RecorderStep(handPose, grabPoint, hand.Scale, item);
			HandGrabPose point;
			this.AddHandGrabPose(recorderStep, out recorderStep.interactable, out point);
			this.AttachGhost(point, recorderStep.HandScale);
			int num = this.CurrentStepIndex + 1;
			if (num < this._recorderSteps.Count)
			{
				this._recorderSteps.RemoveRange(num, this._recorderSteps.Count - num);
			}
			this._recorderSteps.Add(recorderStep);
			this.CurrentStepIndex = this._recorderSteps.Count - 1;
			return true;
		}

		private HandPose TrackedPose(IHand hand)
		{
			ReadOnlyHandJointPoses readOnlyHandJointPoses;
			if (!hand.GetJointPosesLocal(out readOnlyHandJointPoses))
			{
				return null;
			}
			HandPose handPose = new HandPose(hand.Handedness);
			for (int i = 0; i < FingersMetadata.HAND_JOINT_IDS.Length; i++)
			{
				HandJointId index = FingersMetadata.HAND_JOINT_IDS[i];
				handPose.JointRotations[i] = readOnlyHandJointPoses[index].rotation;
			}
			return handPose;
		}

		private void AddHandGrabPose(HandGrabPoseLiveRecorder.RecorderStep recorderStep, out HandGrabInteractable interactable, out HandGrabPose handGrabPose)
		{
			interactable = HandGrabUtils.CreateHandGrabInteractable(recorderStep.Item.transform, null);
			Grabbable pointableElement;
			if (recorderStep.Item.TryGetComponent<Grabbable>(out pointableElement))
			{
				interactable.InjectOptionalPointableElement(pointableElement);
			}
			HandGrabUtils.HandGrabPoseData poseData = new HandGrabUtils.HandGrabPoseData
			{
				handPose = recorderStep.RawHandPose,
				scale = recorderStep.HandScale / interactable.RelativeTo.lossyScale.x,
				gripPose = recorderStep.GrabPoint
			};
			handGrabPose = HandGrabUtils.LoadHandGrabPose(interactable, poseData);
		}

		private void AttachGhost(HandGrabPose point, float referenceScale)
		{
			if (this.GhostProvider == null)
			{
				return;
			}
			HandGhost handGhost = Object.Instantiate<HandGhost>(this.GhostProvider.GetHand(point.HandPose.Handedness), point.transform);
			handGhost.transform.localScale = Vector3.one * (referenceScale / point.transform.lossyScale.x);
			handGhost.SetPose(point);
		}

		[SerializeField]
		private HandGrabInteractor _leftHand;

		[SerializeField]
		private HandGrabInteractor _rightHand;

		[HideInInspector]
		[SerializeField]
		[Tooltip("Prototypes of the static hands (ghosts) that visualize holding poses")]
		private HandGhostProvider _ghostProvider;

		[SerializeField]
		[Tooltip("Prototypes of the static hands (ghosts) that visualize holding poses")]
		private HandGhostProvider _handGhostProvider;

		[SerializeField]
		[Optional]
		private TimerUIControl _timerControl;

		[SerializeField]
		[Optional]
		private TextMeshPro _delayLabel;

		private RigidbodyDetector _leftDetector;

		private RigidbodyDetector _rightDetector;

		private WaitForSeconds _waitOneSeconds = new WaitForSeconds(1f);

		private Coroutine _delayedSnapRoutine;

		public UnityEvent WhenTimeStep;

		public UnityEvent WhenSnapshot;

		public UnityEvent WhenError;

		[Space]
		public UnityEvent<bool> WhenCanUndo;

		public UnityEvent<bool> WhenCanRedo;

		public UnityEvent WhenGrabAllowed;

		public UnityEvent WhenGrabDisallowed;

		private List<HandGrabPoseLiveRecorder.RecorderStep> _recorderSteps = new List<HandGrabPoseLiveRecorder.RecorderStep>();

		private int _currentStepIndex;

		private bool _grabbingEnabled = true;

		private struct RecorderStep
		{
			public HandPose RawHandPose { readonly get; private set; }

			public Pose GrabPoint { readonly get; private set; }

			public Rigidbody Item { readonly get; private set; }

			public float HandScale { readonly get; private set; }

			public RecorderStep(HandPose rawPose, Pose grabPoint, float scale, Rigidbody item)
			{
				this.RawHandPose = new HandPose(rawPose);
				this.GrabPoint = grabPoint;
				this.HandScale = scale;
				this.Item = item;
				this.interactable = null;
			}

			public void ClearInteractable()
			{
				if (this.interactable != null)
				{
					Object.Destroy(this.interactable.gameObject);
				}
			}

			public HandGrabInteractable interactable;
		}
	}
}
