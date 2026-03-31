using System;
using System.Collections;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Liv.Lck.DependencyInjection;
using Liv.Lck.Settings;
using Liv.Lck.Streaming;
using Liv.Lck.UI;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Liv.Lck.Tablet
{
	public class LckStreamButton : MonoBehaviour, IPointerEnterHandler, IEventSystemHandler, IPointerDownHandler, IPointerUpHandler, IPointerExitHandler
	{
		private void Start()
		{
			if (this._lckService != null)
			{
				this._lckService.OnStreamingStarted += this.OnStreamingStarted;
				this._lckService.OnStreamingStopped += this.OnStreamingStopped;
			}
			this.ValidateMeshColors(false, false);
		}

		private void Update()
		{
			if (this._state == LckStreamButton.State.Streaming && this._lckService != null)
			{
				this.UpdateStreamDurationText();
			}
		}

		private void UpdateStreamDurationText()
		{
			this._streamButtonText.text = "00:00";
			LckResult<TimeSpan> streamDuration = this._lckService.GetStreamDuration();
			if (!streamDuration.Success)
			{
				return;
			}
			TimeSpan result = streamDuration.Result;
			int num = Mathf.FloorToInt((float)result.Hours);
			int num2 = Mathf.FloorToInt((float)result.Minutes);
			int num3 = Mathf.FloorToInt((float)result.Seconds);
			this._streamButtonText.text = ((num == 0) ? string.Format("{0:00}:{1:00}", num2, num3) : string.Format("{0:00}:{1:00}:{2:00}", num, num2, num3));
		}

		[ContextMenu("test error")]
		public void OnError()
		{
			this._state = LckStreamButton.State.Error;
			this._streamButtonText.text = "ERROR";
			this.ValidateMeshColors(false, false);
			this.ResetAfterError();
		}

		private Task ResetAfterError()
		{
			LckStreamButton.<ResetAfterError>d__17 <ResetAfterError>d__;
			<ResetAfterError>d__.<>t__builder = AsyncTaskMethodBuilder.Create();
			<ResetAfterError>d__.<>4__this = this;
			<ResetAfterError>d__.<>1__state = -1;
			<ResetAfterError>d__.<>t__builder.Start<LckStreamButton.<ResetAfterError>d__17>(ref <ResetAfterError>d__);
			return <ResetAfterError>d__.<>t__builder.Task;
		}

		private void OnStreamingStarted(LckResult result)
		{
			if (!result.Success)
			{
				this.OnError();
				return;
			}
			this.SetStoppingAnimationValue(1f);
			this._audioController.PlayDiscreetAudioClip(LckDiscreetAudioController.AudioClip.StreamingStarted);
			this._state = LckStreamButton.State.Streaming;
		}

		private void OnStreamingStopped(LckResult result)
		{
			this._audioController.PlayDiscreetAudioClip(LckDiscreetAudioController.AudioClip.StreamingStopped);
			if (!result.Success)
			{
				this.SetStoppingAnimationValue(0f);
				this._streamingController.GoToErrorState();
				this.OnError();
				return;
			}
			this.SetStoppingAnimationValue(0f);
			if (this._state == LckStreamButton.State.StoppingAnimationCompleted)
			{
				this._state = LckStreamButton.State.WaitUntilTriggerExitOrDelay;
				this.WaitForTriggerExitOrDelay();
			}
			else
			{
				this._state = LckStreamButton.State.Idle;
			}
			this.ValidateMeshColors(false, false);
			this._streamButtonText.text = "GO LIVE";
		}

		private Task WaitForTriggerExitOrDelay()
		{
			LckStreamButton.<WaitForTriggerExitOrDelay>d__20 <WaitForTriggerExitOrDelay>d__;
			<WaitForTriggerExitOrDelay>d__.<>t__builder = AsyncTaskMethodBuilder.Create();
			<WaitForTriggerExitOrDelay>d__.<>4__this = this;
			<WaitForTriggerExitOrDelay>d__.<>1__state = -1;
			<WaitForTriggerExitOrDelay>d__.<>t__builder.Start<LckStreamButton.<WaitForTriggerExitOrDelay>d__20>(ref <WaitForTriggerExitOrDelay>d__);
			return <WaitForTriggerExitOrDelay>d__.<>t__builder.Task;
		}

		private void OnDestroy()
		{
			if (this._lckService != null)
			{
				this._lckService.OnStreamingStarted -= this.OnStreamingStarted;
				this._lckService.OnStreamingStopped -= this.OnStreamingStopped;
			}
		}

		public void OnPointerEnter(PointerEventData eventData)
		{
			if (this._state == LckStreamButton.State.Error)
			{
				return;
			}
			this.ValidateMeshColors(false, true);
			this._audioController.PlayDiscreetAudioClip(LckDiscreetAudioController.AudioClip.HoverSound);
		}

		public void OnPointerDown(PointerEventData eventData)
		{
			if (this._state == LckStreamButton.State.Error)
			{
				return;
			}
			if (this._state == LckStreamButton.State.Streaming)
			{
				base.StopAllCoroutines();
				this.SetStoppingAnimationValue(1f);
				base.StartCoroutine(this.StoppingAnimationVisual());
			}
			this.ValidateMeshColors(true, false);
			this._visuals.anchoredPosition3D = this._buttonPressedPosition;
			this._audioController.PlayDiscreetAudioClip(LckDiscreetAudioController.AudioClip.ClickDown);
			if (eventData != null)
			{
				this._clickedObject = eventData.pointerEnter;
			}
		}

		public void OnPointerUp(PointerEventData eventData)
		{
			if (this._state == LckStreamButton.State.Error)
			{
				return;
			}
			if (this._state == LckStreamButton.State.Idle)
			{
				this._state = LckStreamButton.State.WaitingForStreamingStart;
				this._streamButtonText.text = "STARTING...";
				this._streamingController.StartStreaming();
			}
			else if (this._state == LckStreamButton.State.DoingStoppingAnimation)
			{
				base.StopAllCoroutines();
				this.SetStoppingAnimationValue(1f);
				this._state = LckStreamButton.State.Streaming;
			}
			else if (this._state == LckStreamButton.State.WaitUntilTriggerExitOrDelay)
			{
				this._state = LckStreamButton.State.Idle;
				this.ValidateMeshColors(false, false);
				this._streamButtonText.text = "GO LIVE";
			}
			this._visuals.anchoredPosition3D = new Vector3(0f, 0f, 0f);
			this._audioController.PlayDiscreetAudioClip(LckDiscreetAudioController.AudioClip.ClickUp);
			if (eventData != null && this._clickedObject != eventData.pointerEnter)
			{
				this.ValidateMeshColors(false, false);
				return;
			}
			this.ValidateMeshColors(false, true);
		}

		public void OnPointerExit(PointerEventData eventData)
		{
			if (this._state == LckStreamButton.State.Error)
			{
				return;
			}
			this._visuals.anchoredPosition3D = new Vector3(0f, 0f, 0f);
			if (this._state == LckStreamButton.State.WaitUntilTriggerExitOrDelay)
			{
				this._state = LckStreamButton.State.Idle;
				this.ValidateMeshColors(false, false);
				this._streamButtonText.text = "GO LIVE";
			}
			this.ValidateMeshColors(false, false);
		}

		private void OnTriggerEnter(Collider other)
		{
			if (other.gameObject.CompareTag(LckSettings.Instance.TriggerEnterTag) && this.IsValidTap(other.ClosestPoint(base.transform.position)) && !LCKCameraController.ColliderButtonsInUse)
			{
				LCKCameraController.ColliderButtonsInUse = true;
				this._collided = true;
				this.OnPointerDown(null);
			}
		}

		private void OnTriggerExit(Collider other)
		{
			if (this._collided)
			{
				this.OnPointerUp(null);
				this.OnPointerExit(null);
				this._collided = false;
				LCKCameraController.ColliderButtonsInUse = false;
			}
		}

		private bool IsValidTap(Vector3 tapPosition)
		{
			Vector3 to = tapPosition - base.transform.position;
			return Vector3.Angle(-base.transform.forward, to) < 90f;
		}

		private void OnApplicationFocus(bool focus)
		{
			if (focus)
			{
				this._collided = false;
				this._visuals.anchoredPosition3D = new Vector3(0f, 0f, 0f);
				this.ValidateMeshColors(false, false);
			}
		}

		private void ValidateMeshColors(bool isPressed = false, bool isHovering = false)
		{
			if (!this._renderer)
			{
				return;
			}
			if (this._state == LckStreamButton.State.Error)
			{
				this.SetDefaultColor(this._streamingColors.NormalColor);
				this.SetStreamingColor(this._streamingColors.NormalColor);
				return;
			}
			if (!isPressed)
			{
				if (!isHovering)
				{
					this.SetDefaultColor(this._defaultColors.NormalColor);
					this.SetStreamingColor(this._streamingColors.NormalColor);
					return;
				}
				if (isHovering)
				{
					this.SetDefaultColor(this._defaultColors.HighlightedColor);
					this.SetStreamingColor(this._streamingColors.HighlightedColor);
					return;
				}
			}
			else if (isPressed)
			{
				this.SetDefaultColor(this._defaultColors.PressedColor);
				this.SetStreamingColor(this._streamingColors.PressedColor);
			}
		}

		private void SetStoppingAnimationValue(float value)
		{
			float value2 = Mathf.Clamp01(value);
			if (this._renderer != null && this._renderer.material != null)
			{
				this._renderer.material.SetFloat("_ProgressValue", value2);
			}
		}

		private void SetDefaultColor(Color color)
		{
			this._renderer.material.SetColor("_DefaultColor", color);
		}

		private void SetStreamingColor(Color color)
		{
			this._renderer.material.SetColor("_StreamingColor", color);
		}

		private IEnumerator StoppingAnimationVisual()
		{
			float startTime = Time.time;
			float currentProgress = 1f;
			float stoppingDuration = 2f;
			this._state = LckStreamButton.State.DoingStoppingAnimation;
			this._streamButtonText.text = "STOPPING...";
			while (currentProgress > 0f)
			{
				float num = Time.time - startTime;
				currentProgress = Mathf.Lerp(1f, 0f, num / stoppingDuration);
				if (this._renderer != null)
				{
					this._renderer.material.SetFloat("_ProgressValue", currentProgress);
				}
				yield return null;
			}
			if (this._renderer != null)
			{
				this._renderer.material.SetFloat("_ProgressValue", 0f);
			}
			this._state = LckStreamButton.State.StoppingAnimationCompleted;
			this._streamingController.StopStreaming();
			this.ValidateMeshColors(false, false);
			this._streamButtonText.text = "GO LIVE";
			this._visuals.anchoredPosition3D = new Vector3(0f, 0f, 0f);
			yield break;
		}

		[InjectLck]
		private ILckService _lckService;

		[Header("References")]
		[SerializeField]
		private LckStreamingController _streamingController;

		[SerializeField]
		private LckDiscreetAudioController _audioController;

		[SerializeField]
		private TMP_Text _streamButtonText;

		[SerializeField]
		private Renderer _renderer;

		[SerializeField]
		private RectTransform _visuals;

		[Header("Settings")]
		[SerializeField]
		private LckButtonColors _defaultColors;

		[SerializeField]
		private LckButtonColors _streamingColors;

		[SerializeField]
		private Vector3 _buttonPressedPosition = new Vector3(0f, 0f, 40f);

		private bool _collided;

		private GameObject _clickedObject;

		private LckStreamButton.State _state;

		private enum State
		{
			Idle,
			WaitingForStreamingStart,
			Streaming,
			DoingStoppingAnimation,
			StoppingAnimationCompleted,
			WaitUntilTriggerExitOrDelay,
			Error
		}
	}
}
