using System;
using UnityEngine;

namespace Oculus.Interaction.Locomotion
{
	public class LocomotorSound : MonoBehaviour
	{
		private ILocomotionEventHandler Locomotor { get; set; }

		protected virtual void Awake()
		{
			this.Locomotor = (this._locomotor as ILocomotionEventHandler);
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
				this.Locomotor.WhenLocomotionEventHandled += this.HandleLocomotionEvent;
			}
		}

		protected virtual void OnDisable()
		{
			if (this._started)
			{
				this.Locomotor.WhenLocomotionEventHandled -= this.HandleLocomotionEvent;
			}
		}

		private void HandleLocomotionEvent(LocomotionEvent locomotionEvent, Pose delta)
		{
			if (locomotionEvent.Translation == LocomotionEvent.TranslationType.Absolute || locomotionEvent.Translation == LocomotionEvent.TranslationType.AbsoluteEyeLevel || locomotionEvent.Translation == LocomotionEvent.TranslationType.Relative)
			{
				this.PlayTranslationSound(delta.position.magnitude);
			}
			if (locomotionEvent.Rotation == LocomotionEvent.RotationType.Relative)
			{
				this.PlayRotationSound(delta.rotation.y * delta.rotation.w);
			}
			if (locomotionEvent.Translation == LocomotionEvent.TranslationType.None && locomotionEvent.Rotation == LocomotionEvent.RotationType.None)
			{
				this.PlayDenialSound(delta.position.magnitude);
			}
		}

		private void PlayTranslationSound(float translationDistance)
		{
			float num = this._translationCurve.Evaluate(translationDistance);
			float pitchT = num + Random.Range(-this._pitchVariance, this._pitchVariance);
			this._translationSound.PlayAudio(num, pitchT, 0f);
		}

		private void PlayDenialSound(float translationDistance)
		{
			float num = this._translationCurve.Evaluate(translationDistance);
			float pitchT = num + Random.Range(-this._pitchVariance, this._pitchVariance);
			this._translationDeniedSound.PlayAudio(num, pitchT, 0f);
		}

		private void PlayRotationSound(float rotationLength)
		{
			float num = this._rotationCurve.Evaluate(Mathf.Abs(rotationLength));
			float pitchT = num + Random.Range(-this._pitchVariance, this._pitchVariance);
			this._snapTurnSound.PlayAudio(num, pitchT, rotationLength);
		}

		public void InjectAllLocomotorSound(ILocomotionEventHandler locomotor)
		{
			this.InjectPlayerLocomotor(locomotor);
		}

		public void InjectPlayerLocomotor(ILocomotionEventHandler locomotor)
		{
			this._locomotor = (locomotor as Object);
			this.Locomotor = locomotor;
		}

		[SerializeField]
		[Interface(typeof(ILocomotionEventHandler), new Type[]
		{

		})]
		private Object _locomotor;

		[SerializeField]
		private AdjustableAudio _translationSound;

		[SerializeField]
		private AdjustableAudio _translationDeniedSound;

		[SerializeField]
		private AdjustableAudio _snapTurnSound;

		[SerializeField]
		private AnimationCurve _translationCurve = AnimationCurve.EaseInOut(0f, 0f, 2f, 1f);

		[SerializeField]
		private AnimationCurve _rotationCurve = AnimationCurve.EaseInOut(0f, 0f, 180f, 1f);

		[SerializeField]
		private float _pitchVariance = 0.05f;

		protected bool _started;
	}
}
