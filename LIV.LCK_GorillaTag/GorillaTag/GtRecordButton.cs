using System;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Liv.Lck.DependencyInjection;
using Liv.Lck.Recorder;
using TMPro;
using UnityEngine;

namespace Liv.Lck.GorillaTag
{
	public class GtRecordButton : MonoBehaviour
	{
		public event Action onPressed;

		private void Start()
		{
			this.InitSetUp();
		}

		private void InitSetUp()
		{
			this._defaultLocalPosition = this._visualsTrans.localPosition;
			this.SetUp();
			this.UpdateVisualState();
		}

		private void UpdateVisualState()
		{
			this._bodyRenderer.material = ((this._currentState == GtRecordButton.State.Recording) ? this._settings.RecordingBodyMaterial : this._settings.DefaultBodyMaterial);
			this._label.color = ((this._currentState == GtRecordButton.State.Recording) ? this._settings.SecondaryTextColor : this._settings.PrimaryTextColor);
			switch (this._currentState)
			{
			case GtRecordButton.State.Idle:
				this._label.text = "RECORD";
				return;
			case GtRecordButton.State.Saving:
				this._label.text = "SAVING";
				return;
			case GtRecordButton.State.Error:
				this._label.text = "ERROR";
				this._label.color = this._settings.SecondaryTextColor;
				this._bodyRenderer.material = this._settings.RecordingBodyMaterial;
				return;
			}
			this._label.text = "00:00";
		}

		private void SetDisabled(bool isDisabled)
		{
			this._isDisabled = isDisabled;
			this._visualsTrans.localPosition = this._defaultLocalPosition;
		}

		private void OnError()
		{
			this._currentState = GtRecordButton.State.Error;
			this.UpdateVisualState();
			this.SetDisabled(true);
			this.ResetAfterError();
		}

		private Task ResetAfterError()
		{
			GtRecordButton.<ResetAfterError>d__22 <ResetAfterError>d__;
			<ResetAfterError>d__.<>t__builder = AsyncTaskMethodBuilder.Create();
			<ResetAfterError>d__.<>4__this = this;
			<ResetAfterError>d__.<>1__state = -1;
			<ResetAfterError>d__.<>t__builder.Start<GtRecordButton.<ResetAfterError>d__22>(ref <ResetAfterError>d__);
			return <ResetAfterError>d__.<>t__builder.Task;
		}

		private void SetUp()
		{
			this.UpdateVisualState();
			this._label.text = this._name.ToUpper();
			this._lckService.OnRecordingStarted += this.OnRecordingStarted;
			this._lckService.OnRecordingStopped += this.OnRecordingStopped;
			this._lckService.OnRecordingSaved += this.OnRecordingSaved;
		}

		private void OnRecordingSaved(LckResult<RecordingData> result)
		{
			if (!result.Success)
			{
				this.OnError();
				return;
			}
			this._currentState = GtRecordButton.State.Idle;
			this.UpdateVisualState();
			this._visualsTrans.localPosition = this._defaultLocalPosition;
			this._audioController.PlayDiscreetAudioClip(LckDiscreetAudioController.AudioClip.RecordingSaved);
		}

		private void OnDestroy()
		{
			if (this._lckService == null)
			{
				return;
			}
			this._lckService.OnRecordingStarted -= this.OnRecordingStarted;
			this._lckService.OnRecordingStopped -= this.OnRecordingStopped;
			this._lckService.OnRecordingSaved -= this.OnRecordingSaved;
		}

		private void OnRecordingStopped(LckResult result)
		{
			if (!result.Success)
			{
				this.OnError();
				return;
			}
			this._currentState = GtRecordButton.State.Saving;
			this.UpdateVisualState();
			this._visualsTrans.localPosition = this._defaultLocalPosition + Vector3.forward * -this._settings.ActiveButtonOffset;
		}

		private void OnRecordingStarted(LckResult result)
		{
			if (!result.Success)
			{
				this.OnError();
				return;
			}
			this._currentState = GtRecordButton.State.Recording;
			this.UpdateVisualState();
			this._audioController.PlayDiscreetAudioClip(LckDiscreetAudioController.AudioClip.RecordingStart);
		}

		public void TapStarted()
		{
			if (this._currentState == GtRecordButton.State.Saving || this._isDisabled)
			{
				return;
			}
			this._visualsTrans.localPosition = this._defaultLocalPosition + Vector3.forward * -this._settings.ActiveButtonOffset;
			this.onPressed();
			this._audioController.PlayDiscreetAudioClip(LckDiscreetAudioController.AudioClip.ClickDown);
		}

		public void TapEnded()
		{
			if (this._currentState == GtRecordButton.State.Saving || this._isDisabled)
			{
				return;
			}
			this._visualsTrans.localPosition = this._defaultLocalPosition;
			this._audioController.PlayDiscreetAudioClip(LckDiscreetAudioController.AudioClip.ClickUp);
		}

		private void Update()
		{
			LckResult<bool> lckResult = this._lckService.IsRecording();
			if (this._currentState == GtRecordButton.State.Recording && lckResult.Success && lckResult.Result)
			{
				this.UpdateRecordDurationText();
			}
		}

		private void UpdateRecordDurationText()
		{
			TimeSpan result = this._lckService.GetRecordingDuration().Result;
			int num = Mathf.FloorToInt((float)result.Hours);
			int num2 = Mathf.FloorToInt((float)result.Minutes);
			int num3 = Mathf.FloorToInt((float)result.Seconds);
			this._label.text = ((num == 0) ? string.Format("{0:00}:{1:00}", num2, num3) : string.Format("{0:00}:{1:00}:{2:00}", num, num2, num3));
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

		private const string _idleString = "RECORD";

		private const string _savingString = "SAVING";

		private const string _errorString = "ERROR";

		private bool _isDisabled;

		private Vector3 _defaultLocalPosition;

		private GtRecordButton.State _currentState;

		private enum State
		{
			Idle,
			Recording,
			Saving,
			Error
		}
	}
}
