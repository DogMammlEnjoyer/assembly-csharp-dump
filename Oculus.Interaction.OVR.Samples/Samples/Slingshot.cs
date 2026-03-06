using System;
using System.Collections;
using Oculus.Interaction.HandGrab;
using UnityEngine;

namespace Oculus.Interaction.Samples
{
	public class Slingshot : MonoBehaviour, ITransformer
	{
		private void OnTriggerEnter(Collider other)
		{
			if (this._loadedPellet != null)
			{
				return;
			}
			SlingshotPellet pellet;
			if (other.TryGetComponent<SlingshotPellet>(out pellet))
			{
				this.HandlePelletSnapped(pellet);
			}
		}

		private void HandlePelletSnapped(SlingshotPellet pellet)
		{
			HandGrabInteractor handGrabber = pellet.HandGrabber;
			if (handGrabber == null || handGrabber.State != InteractorState.Select)
			{
				return;
			}
			foreach (HandGrabInteractable handGrabInteractable in this._handGrabInteractables)
			{
				if (handGrabber.CanInteractWith(handGrabInteractable))
				{
					handGrabber.ForceRelease();
					handGrabber.ForceSelect(handGrabInteractable, true);
					this._loadedPellet = pellet;
					this._loadedPellet.Attach();
					return;
				}
			}
		}

		public void Initialize(IGrabbable grabbable)
		{
			this._grabbable = grabbable;
		}

		public void BeginTransform()
		{
			Pose pose = this._grabbable.GrabPoints[0];
			Transform transform = this._grabbable.Transform;
			this._grabDeltaInLocalSpace = new Pose(transform.InverseTransformVector(pose.position - transform.position), Quaternion.Inverse(pose.rotation) * transform.rotation);
			this._isGrabbed = true;
			this._positionVelocity = Vector3.zero;
			this._rotationVelocity = 0f;
			this.CurveHolder(this._rubberAngle);
		}

		public void UpdateTransform()
		{
			Pose pose = this._grabbable.GrabPoints[0];
			Transform transform = this._grabbable.Transform;
			Vector3 localPosition = transform.localPosition;
			Vector3 position = pose.position - transform.TransformVector(this._grabDeltaInLocalSpace.position);
			Quaternion rotation = pose.rotation * this._grabDeltaInLocalSpace.rotation;
			Transform parent = transform.parent;
			Pose pose2 = new Pose(position, rotation);
			Pose pose3 = parent.Delta(pose2);
			Vector3 vector = pose3.position - this._neutralPose.position;
			float num = Vector3.Distance(localPosition, this._neutralPose.position);
			float num2 = vector.magnitude;
			if (num2 > num)
			{
				float maxDelta = this._translationResistance.Evaluate(num) * Time.deltaTime;
				num2 = Mathf.MoveTowards(num, num2, maxDelta);
			}
			Vector3 normalized = Vector3.ProjectOnPlane(vector, Vector3.right).normalized;
			vector = Vector3.Slerp(vector, normalized, this._aimingResistance.Evaluate(num)).normalized;
			Vector3 vector2 = this._neutralPose.position + vector * num2;
			transform.localPosition = vector2;
			num = this.Tension(vector2);
			float t = this._aimingResistance.Evaluate(num);
			Quaternion b = Quaternion.LookRotation(-vector, pose3.up);
			transform.localRotation = Quaternion.SlerpUnclamped(pose3.rotation, b, t);
			this.OnStretch(num);
		}

		public void EndTransform()
		{
			this._isGrabbed = false;
			if (this._loadedPellet != null)
			{
				Vector3 force = this.SlingshotLaunchForce();
				this._loadedPellet.Eject(force);
				this._loadedPellet = null;
			}
			this.CurveHolder(0f);
		}

		private void Update()
		{
			if (!this._isGrabbed)
			{
				Transform transform = base.transform;
				Vector3 a = (this._neutralPose.position - transform.localPosition) * this._springForce;
				this._positionVelocity = this._positionVelocity * this._damping + a * Time.deltaTime;
				transform.localPosition += this._positionVelocity;
				float num;
				Vector3 vector;
				transform.localRotation.ToAngleAxis(out num, out vector);
				if (num > 180f)
				{
					num -= 360f;
				}
				this._rotationVelocity = this._rotationVelocity * this._damping + num * this._springForce * Time.deltaTime;
				transform.localRotation = Quaternion.AngleAxis(this._rotationVelocity, -vector.normalized) * transform.localRotation;
			}
		}

		private void LateUpdate()
		{
			if (this._loadedPellet != null)
			{
				this._loadedPellet.Move(this._holder);
			}
		}

		private void CurveHolder(float angle)
		{
			this._rightRubberPoint.localEulerAngles = Vector3.up * angle;
			this._leftRubberPoint.localEulerAngles = -Vector3.up * angle;
		}

		private float Tension(Vector3 localPoint)
		{
			return Vector3.Distance(localPoint, this._neutralPose.position);
		}

		private Vector3 SlingshotLaunchForce()
		{
			Transform transform = this._grabbable.Transform;
			float d = this.Tension(transform.localPosition);
			return (transform.parent.position - transform.position).normalized * d * this._slingshotStrength;
		}

		public void OnStretch(float currentTension)
		{
			if (Mathf.Abs(this._lastTensionStep - currentTension) > this._stretchAudioStep.Evaluate(currentTension) && Time.unscaledTime - this._lastTensionTime > 0.1f)
			{
				this.PlayStretchAudio(currentTension);
				this.PlayStretchHaptics(currentTension);
				this._lastTensionStep = currentTension;
				this._lastTensionTime = Time.unscaledTime;
			}
		}

		private void PlayStretchAudio(float tension)
		{
			float pitch = this._strecthAudioPitch.Evaluate(tension);
			this._audioSource.pitch = pitch;
			this._audioSource.PlayOneShot(this._stretchAudioClip, 1f);
		}

		private void PlayStretchHaptics(float tension)
		{
			float pitch = this._strecthAudioPitch.Evaluate(tension);
			base.StartCoroutine(this.HapticsRoutine(pitch));
		}

		private IEnumerator HapticsRoutine(float pitch)
		{
			OVRInput.Controller controllers = OVRInput.Controller.Touch;
			OVRInput.SetControllerVibration(pitch * 0.5f, pitch * 0.2f, controllers);
			yield return this._hapticsWait;
			OVRInput.SetControllerVibration(0f, 0f, controllers);
			yield break;
		}

		[SerializeField]
		private Pose _neutralPose;

		[SerializeField]
		private Transform _holder;

		[SerializeField]
		private Transform _leftRubberPoint;

		[SerializeField]
		private Transform _rightRubberPoint;

		[SerializeField]
		private float _rubberAngle = 60f;

		[SerializeField]
		private AnimationCurve _translationResistance;

		[SerializeField]
		private AnimationCurve _aimingResistance;

		[SerializeField]
		private float _springForce = 0.1f;

		[SerializeField]
		private float _damping = 0.95f;

		[SerializeField]
		private float _slingshotStrength = 10f;

		[SerializeField]
		private HandGrabInteractable[] _handGrabInteractables;

		[Header("Feedback")]
		[SerializeField]
		private AudioSource _audioSource;

		[SerializeField]
		private AudioClip _stretchAudioClip;

		[SerializeField]
		private AnimationCurve _strecthAudioPitch;

		[SerializeField]
		private AnimationCurve _stretchAudioStep;

		private IGrabbable _grabbable;

		private Pose _grabDeltaInLocalSpace;

		private bool _isGrabbed;

		private Vector3 _positionVelocity = Vector3.zero;

		private float _rotationVelocity;

		private SlingshotPellet _loadedPellet;

		private WaitForSeconds _hapticsWait = new WaitForSeconds(0.05f);

		private float _lastTensionStep;

		private float _lastTensionTime;

		private const float _tensionStepLength = 0.1f;
	}
}
