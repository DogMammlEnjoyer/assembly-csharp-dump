using System;
using System.Collections.Generic;
using Oculus.Interaction.Locomotion;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Oculus.Interaction.Samples
{
	public class LocomotionTutorialProgressTracker : MonoBehaviour
	{
		protected virtual void Awake()
		{
			this.LocomotionHandler = (this._locomotionHandler as ILocomotionEventHandler);
		}

		protected virtual void Start()
		{
			this.BeginStart(ref this._started, null);
			this._totalProgress = this._dots.Length;
			this.EndStart(ref this._started);
		}

		protected virtual void OnEnable()
		{
			if (this._started)
			{
				this.LocomotionHandler.WhenLocomotionEventHandled += this.LocomotionEventHandled;
				this.ResetProgress();
			}
		}

		protected virtual void OnDisable()
		{
			if (this._started)
			{
				this.LocomotionHandler.WhenLocomotionEventHandled -= this.LocomotionEventHandled;
			}
		}

		private void LocomotionEventHandled(LocomotionEvent arg1, Pose arg2)
		{
			if (this._consumeRotationEvents.Contains(arg1.Rotation) || this._consumeTranslationEvents.Contains(arg1.Translation))
			{
				this.Progress();
			}
		}

		private void Progress()
		{
			this._currentProgress++;
			if (this._currentProgress <= this._totalProgress)
			{
				this._dots[this._currentProgress - 1].sprite = this._completedSprite;
			}
			if (this._currentProgress < this._totalProgress)
			{
				this._dots[this._currentProgress].sprite = this._currentSprite;
			}
			if (this._currentProgress >= this._totalProgress)
			{
				this.WhenCompleted.Invoke();
			}
		}

		private void ResetProgress()
		{
			this._currentProgress = 0;
			for (int i = 0; i < this._dots.Length; i++)
			{
				this._dots[i].sprite = ((i == 0) ? this._currentSprite : this._pendingSprite);
			}
		}

		public void InjectAllLocomotionTutorialProgressTracker(Image[] dots, Sprite pendingSprite, Sprite currentSprite, Sprite completedSprite, List<LocomotionEvent.TranslationType> consumeTranslationEvents, List<LocomotionEvent.RotationType> consumeRotationEvents, ILocomotionEventHandler locomotionHandler)
		{
			this.InjectDots(dots);
			this.InjectPendingSprite(pendingSprite);
			this.InjectCurrentSprite(currentSprite);
			this.InjectCompletedSprite(completedSprite);
			this.InjectConsumeTranslationEvents(consumeTranslationEvents);
			this.InjectConsumeRotationEvents(consumeRotationEvents);
			this.InjectLocomotionHandler(locomotionHandler);
		}

		public void InjectDots(Image[] dots)
		{
			this._dots = dots;
		}

		public void InjectPendingSprite(Sprite pendingSprite)
		{
			this._pendingSprite = pendingSprite;
		}

		public void InjectCurrentSprite(Sprite currentSprite)
		{
			this._currentSprite = currentSprite;
		}

		public void InjectCompletedSprite(Sprite completedSprite)
		{
			this._completedSprite = completedSprite;
		}

		public void InjectConsumeTranslationEvents(List<LocomotionEvent.TranslationType> consumeTranslationEvents)
		{
			this._consumeTranslationEvents = consumeTranslationEvents;
		}

		public void InjectConsumeRotationEvents(List<LocomotionEvent.RotationType> consumeRotationEvents)
		{
			this._consumeRotationEvents = consumeRotationEvents;
		}

		public void InjectLocomotionHandler(ILocomotionEventHandler locomotionHandler)
		{
			this._locomotionHandler = (locomotionHandler as Object);
			this.LocomotionHandler = locomotionHandler;
		}

		[SerializeField]
		private Image[] _dots;

		[SerializeField]
		private Sprite _pendingSprite;

		[SerializeField]
		private Sprite _currentSprite;

		[SerializeField]
		private Sprite _completedSprite;

		[SerializeField]
		private List<LocomotionEvent.TranslationType> _consumeTranslationEvents = new List<LocomotionEvent.TranslationType>();

		[SerializeField]
		private List<LocomotionEvent.RotationType> _consumeRotationEvents = new List<LocomotionEvent.RotationType>();

		[SerializeField]
		[Interface(typeof(ILocomotionEventHandler), new Type[]
		{

		})]
		private Object _locomotionHandler;

		private ILocomotionEventHandler LocomotionHandler;

		public UnityEvent WhenCompleted;

		protected bool _started;

		private int _currentProgress;

		private int _totalProgress;
	}
}
