using System;
using System.Collections;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Liv.Lck.DependencyInjection;
using Liv.Lck.Streaming;
using TMPro;
using UnityEngine;

namespace Liv.Lck.GorillaTag
{
	public class GtStreamButton : MonoBehaviour
	{
		private void Start()
		{
			this._defaultLocalPosition = this._visualsTrans.localPosition;
			this.UpdateVisualState();
			if (this._lckService != null)
			{
				this._lckService.OnStreamingStarted += this.OnStreamingStarted;
				this._lckService.OnStreamingStopped += this.OnStreamingStopped;
			}
		}

		private void Update()
		{
			if (this._state == GtStreamButton.State.Streaming && this._lckService != null)
			{
				this.UpdateStreamDurationText();
			}
		}

		private void UpdateStreamDurationText()
		{
			this._label.text = "00:00";
			LckResult<TimeSpan> streamDuration = this._lckService.GetStreamDuration();
			if (!streamDuration.Success)
			{
				return;
			}
			TimeSpan result = streamDuration.Result;
			int num = Mathf.FloorToInt((float)result.Hours);
			int num2 = Mathf.FloorToInt((float)result.Minutes);
			int num3 = Mathf.FloorToInt((float)result.Seconds);
			this._label.text = ((num == 0) ? string.Format("{0:00}:{1:00}", num2, num3) : string.Format("{0:00}:{1:00}:{2:00}", num, num2, num3));
		}

		private void UpdateVisualState()
		{
			switch (this._state)
			{
			case GtStreamButton.State.Idle:
				this._label.text = "GO LIVE";
				this.SetDefaultColor(this._defaultColor);
				this.SetStreamingColor(this._streamingColor);
				this._label.color = this._settings.PrimaryTextColor;
				this.SetStoppingAnimationValue(0f);
				return;
			case GtStreamButton.State.WaitingForStreamingStart:
				this._label.text = "STARTING...";
				return;
			case GtStreamButton.State.Streaming:
				this.SetDefaultColor(this._defaultColor);
				this.SetStreamingColor(this._streamingColor);
				this._label.color = this._settings.SecondaryTextColor;
				return;
			case GtStreamButton.State.DoingStoppingAnimation:
				this._label.text = "STOPPING...";
				this._label.color = this._settings.PrimaryTextColor;
				return;
			case GtStreamButton.State.StoppingAnimationCompleted:
				this._label.text = "GO LIVE";
				this.SetDefaultColor(this._defaultColor);
				this.SetStreamingColor(this._streamingColor);
				this._label.color = this._settings.PrimaryTextColor;
				return;
			case GtStreamButton.State.WaitUntilTriggerExitOrDelay:
				break;
			case GtStreamButton.State.Error:
				this._label.text = "ERROR";
				this.SetDefaultColor(this._streamingColor);
				this.SetStreamingColor(this._streamingColor);
				this._label.color = this._settings.SecondaryTextColor;
				break;
			default:
				return;
			}
		}

		private void SetDefaultColor(Color color)
		{
			this._bodyRenderer.material.SetColor("_DefaultColor", color);
		}

		private void SetStreamingColor(Color color)
		{
			this._bodyRenderer.material.SetColor("_StreamingColor", color);
		}

		private IEnumerator StoppingAnimationVisual()
		{
			float startTime = Time.time;
			float currentProgress = 1f;
			float stoppingDuration = 2.5f;
			this._state = GtStreamButton.State.DoingStoppingAnimation;
			this.UpdateVisualState();
			while (currentProgress > 0f)
			{
				float num = Time.time - startTime;
				this._label.color = Color.Lerp(this._settings.PrimaryTextColor, this._settings.SecondaryTextColor, currentProgress);
				currentProgress = Mathf.Lerp(1f, 0f, num / stoppingDuration);
				if (this._bodyRenderer != null)
				{
					this._bodyRenderer.material.SetFloat("_ProgressValue", currentProgress);
				}
				yield return null;
			}
			if (this._bodyRenderer != null)
			{
				this._bodyRenderer.material.SetFloat("_ProgressValue", 0f);
			}
			this._state = GtStreamButton.State.StoppingAnimationCompleted;
			this._streamingController.StopStreaming();
			this.UpdateVisualState();
			yield break;
		}

		private void SetStoppingAnimationValue(float value)
		{
			float value2 = Mathf.Clamp01(value);
			if (this._bodyRenderer != null && this._bodyRenderer.material != null)
			{
				this._bodyRenderer.material.SetFloat("_ProgressValue", value2);
			}
		}

		private void SetDisabled(bool isDisabled)
		{
			this._isDisabled = isDisabled;
			this._visualsTrans.localPosition = this._defaultLocalPosition;
		}

		[ContextMenu("test error")]
		public void OnError()
		{
			this._state = GtStreamButton.State.Error;
			this.UpdateVisualState();
			this._visualsTrans.localPosition = this._defaultLocalPosition;
			this.SetDisabled(true);
			this.ResetAfterError();
		}

		private Task ResetAfterError()
		{
			GtStreamButton.<ResetAfterError>d__25 <ResetAfterError>d__;
			<ResetAfterError>d__.<>t__builder = AsyncTaskMethodBuilder.Create();
			<ResetAfterError>d__.<>4__this = this;
			<ResetAfterError>d__.<>1__state = -1;
			<ResetAfterError>d__.<>t__builder.Start<GtStreamButton.<ResetAfterError>d__25>(ref <ResetAfterError>d__);
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
			this._state = GtStreamButton.State.Streaming;
			this.UpdateVisualState();
		}

		private void OnStreamingStopped(LckResult result)
		{
			this._audioController.PlayDiscreetAudioClip(LckDiscreetAudioController.AudioClip.StreamingStopped);
			if (!result.Success || this._state != GtStreamButton.State.StoppingAnimationCompleted)
			{
				this._streamingController.GoToErrorState();
				this.OnError();
				return;
			}
			this._state = GtStreamButton.State.WaitUntilTriggerExitOrDelay;
			this.WaitForTriggerExitOrDelay();
		}

		private Task WaitForTriggerExitOrDelay()
		{
			GtStreamButton.<WaitForTriggerExitOrDelay>d__28 <WaitForTriggerExitOrDelay>d__;
			<WaitForTriggerExitOrDelay>d__.<>t__builder = AsyncTaskMethodBuilder.Create();
			<WaitForTriggerExitOrDelay>d__.<>4__this = this;
			<WaitForTriggerExitOrDelay>d__.<>1__state = -1;
			<WaitForTriggerExitOrDelay>d__.<>t__builder.Start<GtStreamButton.<WaitForTriggerExitOrDelay>d__28>(ref <WaitForTriggerExitOrDelay>d__);
			return <WaitForTriggerExitOrDelay>d__.<>t__builder.Task;
		}

		private void OnDestroy()
		{
			if (this._lckService == null)
			{
				return;
			}
			this._lckService.OnStreamingStarted -= this.OnStreamingStarted;
			this._lckService.OnStreamingStopped -= this.OnStreamingStopped;
		}

		public void TapStarted()
		{
			if (this._state == GtStreamButton.State.Error || this._isDisabled)
			{
				return;
			}
			if (this._state == GtStreamButton.State.Streaming)
			{
				base.StopAllCoroutines();
				this.SetStoppingAnimationValue(1f);
				base.StartCoroutine(this.StoppingAnimationVisual());
			}
			else if (this._state == GtStreamButton.State.Idle)
			{
				this._state = GtStreamButton.State.WaitingForStreamingStart;
				this._streamingController.StartStreaming();
				this.UpdateVisualState();
			}
			this._visualsTrans.localPosition = this._defaultLocalPosition + Vector3.forward * -this._settings.ActiveButtonOffset;
			this._audioController.PlayDiscreetAudioClip(LckDiscreetAudioController.AudioClip.ClickDown);
		}

		public void TapEnded()
		{
			if (this._state == GtStreamButton.State.Error || this._isDisabled)
			{
				return;
			}
			if (this._state == GtStreamButton.State.DoingStoppingAnimation)
			{
				base.StopAllCoroutines();
				this.SetStoppingAnimationValue(1f);
				this._state = GtStreamButton.State.Streaming;
			}
			if (this._state == GtStreamButton.State.WaitUntilTriggerExitOrDelay)
			{
				this._state = GtStreamButton.State.Idle;
			}
			this.UpdateVisualState();
			this._visualsTrans.localPosition = this._defaultLocalPosition;
			this._audioController.PlayDiscreetAudioClip(LckDiscreetAudioController.AudioClip.ClickUp);
		}

		[InjectLck]
		private ILckService _lckService;

		[Space(10f)]
		[Header("Global Settings")]
		[SerializeField]
		private GtUiSettings _settings;

		[Space(10f)]
		[Header("Parameters")]
		[SerializeField]
		private string _name;

		[SerializeField]
		private TextMeshPro _label;

		[SerializeField]
		private MeshRenderer _bodyRenderer;

		[SerializeField]
		private Transform _visualsTrans;

		[SerializeField]
		private LckDiscreetAudioController _audioController;

		[SerializeField]
		private LckStreamingController _streamingController;

		[Header("Parameters")]
		[SerializeField]
		private Color _defaultColor;

		[SerializeField]
		private Color _streamingColor;

		private const string _idleString = "GO LIVE";

		private Vector3 _defaultLocalPosition;

		private bool _isDisabled;

		private GtStreamButton.State _state;

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
