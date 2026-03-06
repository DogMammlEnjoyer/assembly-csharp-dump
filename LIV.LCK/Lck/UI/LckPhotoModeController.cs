using System;
using System.Collections;
using Liv.Lck.DependencyInjection;
using Liv.Lck.Tablet;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Liv.Lck.UI
{
	public class LckPhotoModeController : MonoBehaviour
	{
		private void Start()
		{
			this._photoFlash.gameObject.SetActive(false);
			this._countdownBG.SetActive(false);
		}

		private void OnEnable()
		{
			this._lckService.OnRecordingStarted += this.OnRecordingStarted;
		}

		private void OnDisable()
		{
			this._lckService.OnRecordingStarted -= this.OnRecordingStarted;
			this.StopAndResetSequence();
		}

		private void OnRecordingStarted(LckResult result)
		{
			this.StopAndResetSequence();
		}

		public void PlayPhotoSequence()
		{
			this.StopAndResetSequence();
			base.StartCoroutine(this.CountdownSequence());
		}

		public void StopAndResetSequence()
		{
			base.StopAllCoroutines();
			this.ResetFlashVisuals();
			this.ResetCountdownVisuals();
		}

		private void ResetFlashVisuals()
		{
			this._photoFlash.gameObject.SetActive(false);
			Color color = this._photoFlash.color;
			color.a = this._flashAlpha;
			this._photoFlash.color = color;
		}

		private void ResetCountdownVisuals()
		{
			this._countdownBG.SetActive(false);
		}

		private IEnumerator CountdownSequence()
		{
			this._countdownText.text = "3";
			this._countdownBG.SetActive(true);
			this._audioController.PlayDiscreetAudioClip(LckDiscreetAudioController.AudioClip.ScreenshotBeepSound);
			yield return new WaitForSeconds(1f);
			this._countdownText.text = "2";
			this._audioController.PlayDiscreetAudioClip(LckDiscreetAudioController.AudioClip.ScreenshotBeepSound);
			yield return new WaitForSeconds(1f);
			this._countdownText.text = "1";
			this._audioController.PlayDiscreetAudioClip(LckDiscreetAudioController.AudioClip.ScreenshotBeepSound);
			yield return new WaitForSeconds(1f);
			this._countdownBG.SetActive(false);
			this._onPhotoCaptured.Invoke();
			yield return new WaitForSeconds(0.1f);
			base.StartCoroutine(this.FadeSequence());
			yield break;
		}

		private IEnumerator FadeSequence()
		{
			this._lckService.CapturePhoto();
			this._photoFlash.gameObject.SetActive(true);
			this._audioController.PlayDiscreetAudioClip(LckDiscreetAudioController.AudioClip.CameraShutterSound);
			yield return new WaitForSeconds(this._delayBeforeFade);
			yield return base.StartCoroutine(this.FadeImageAlpha(this._flashAlpha, 0f, this._fadeOutDuration));
			this._photoFlash.gameObject.SetActive(false);
			yield return new WaitForSeconds(0.5f);
			this._notificationController.ShowNotification(NotificationType.PhotoSaved);
			yield break;
		}

		private IEnumerator FadeImageAlpha(float startAlpha, float endAlpha, float duration)
		{
			float elapsedTime = 0f;
			Color currentColor = this._photoFlash.color;
			while (elapsedTime < duration)
			{
				elapsedTime += Time.deltaTime;
				float t = Mathf.Clamp01(elapsedTime / duration);
				currentColor.a = Mathf.Lerp(startAlpha, endAlpha, t);
				this._photoFlash.color = currentColor;
				yield return null;
			}
			currentColor.a = endAlpha;
			this._photoFlash.color = currentColor;
			yield break;
		}

		[InjectLck]
		private ILckService _lckService;

		[Tooltip("The UI Image component that creates the 'flash' effect when a photo is taken. It should cover the entire screen.")]
		[SerializeField]
		private Image _photoFlash;

		[Tooltip("The parent GameObject for the countdown UI elements. This will be enabled and disabled by the controller.")]
		[SerializeField]
		private GameObject _countdownBG;

		[Tooltip("The TextMeshPro text element used to display the '3, 2, 1' countdown.")]
		[SerializeField]
		private TMP_Text _countdownText;

		[Tooltip("The duration in seconds for the flash effect to fade out.")]
		[SerializeField]
		private float _fadeOutDuration = 0.5f;

		[Tooltip("A short delay in seconds after the flash appears before it begins to fade out.")]
		[SerializeField]
		private float _delayBeforeFade = 0.3f;

		[Tooltip("A reference to the controller responsible for playing short, non-diegetic audio clips like beeps and shutter sounds.")]
		[SerializeField]
		private LckDiscreetAudioController _audioController;

		[Tooltip("A reference to the controller used to show the 'Photo Saved' notification after the sequence is complete.")]
		[SerializeField]
		private LckNotificationController _notificationController;

		[Tooltip("This event is invoked at the exact moment the photo is captured. Use it to temporarily disable UI buttons or trigger other game-specific logic such as a flash light being enabled.")]
		[SerializeField]
		private UnityEvent _onPhotoCaptured;

		[Tooltip("The starting alpha (opacity) of the flash effect. A value of 0.9 is recommended over 1.0 to avoid a harsh, fully opaque flash.")]
		private float _flashAlpha = 0.9f;
	}
}
