using System;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Liv.Lck.DependencyInjection;
using Liv.Lck.Recorder;
using Liv.Lck.UI;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Liv.Lck.Tablet
{
	public class LckRecordButton : MonoBehaviour
	{
		private void Start()
		{
			this.EnsureLckService();
			if (this._lckService != null)
			{
				this._lckService.OnRecordingStarted += this.OnRecordingStarted;
				this._lckService.OnRecordingStopped += this.OnRecordingStopped;
				this._lckService.OnRecordingPaused += this.OnRecordingPaused;
				this._lckService.OnRecordingResumed += this.OnRecordingResumed;
				this._lckService.OnRecordingSaved += this.OnRecordingSaved;
			}
		}

		private void Update()
		{
			this.EnsureLckService();
			if (this._state == LckRecordButton.State.Recording && this._lckService != null)
			{
				this.UpdateRecordDurationText();
			}
		}

		private void UpdateRecordDurationText()
		{
			LckResult<TimeSpan> recordingDuration = this._lckService.GetRecordingDuration();
			if (!recordingDuration.Success)
			{
				return;
			}
			TimeSpan result = recordingDuration.Result;
			int num = Mathf.FloorToInt((float)result.Hours);
			int num2 = Mathf.FloorToInt((float)result.Minutes);
			int num3 = Mathf.FloorToInt((float)result.Seconds);
			this._recordButtonText.text = ((num == 0) ? string.Format("{0:00}:{1:00}", num2, num3) : string.Format("{0:00}:{1:00}:{2:00}", num, num2, num3));
		}

		private void OnError()
		{
			this._state = LckRecordButton.State.Error;
			this._recordButtonText.text = "ERROR";
			this._recordLckToggle.enabled = false;
			this._recordToggle.interactable = false;
			if (this._collider)
			{
				this._collider.enabled = false;
			}
			this.ResetAfterError();
		}

		private Task ResetAfterError()
		{
			LckRecordButton.<ResetAfterError>d__12 <ResetAfterError>d__;
			<ResetAfterError>d__.<>t__builder = AsyncTaskMethodBuilder.Create();
			<ResetAfterError>d__.<>4__this = this;
			<ResetAfterError>d__.<>1__state = -1;
			<ResetAfterError>d__.<>t__builder.Start<LckRecordButton.<ResetAfterError>d__12>(ref <ResetAfterError>d__);
			return <ResetAfterError>d__.<>t__builder.Task;
		}

		private void OnRecordingStarted(LckResult result)
		{
			if (!result.Success)
			{
				this.OnError();
				return;
			}
			this._audioController.PlayDiscreetAudioClip(LckDiscreetAudioController.AudioClip.RecordingStart);
			this._state = LckRecordButton.State.Recording;
		}

		private void OnRecordingPaused(LckResult result)
		{
			if (!result.Success)
			{
				this.OnError();
				return;
			}
			this._state = LckRecordButton.State.Paused;
			if (this._recordButtonText == null || this._recordLckToggle == null)
			{
				return;
			}
			this._recordButtonText.text = "PAUSED";
		}

		private void OnRecordingResumed(LckResult result)
		{
			if (!result.Success)
			{
				this.OnError();
				return;
			}
			this._state = LckRecordButton.State.Recording;
		}

		private void OnRecordingStopped(LckResult result)
		{
			if (!result.Success)
			{
				this.OnError();
				return;
			}
			this._state = LckRecordButton.State.Saving;
			if (this._recordButtonText == null || this._recordLckToggle == null)
			{
				return;
			}
			this._recordButtonText.text = "SAVING";
			this._recordLckToggle.SetToggleVisualsOff();
			this._recordLckToggle.enabled = false;
			this._recordToggle.interactable = false;
		}

		private void OnRecordingSaved(LckResult<RecordingData> result)
		{
			if (!result.Success)
			{
				this.OnError();
				return;
			}
			this._state = LckRecordButton.State.Idle;
			this._audioController.PlayDiscreetAudioClip(LckDiscreetAudioController.AudioClip.RecordingSaved);
			if (this._recordButtonText == null || this._recordLckToggle == null)
			{
				return;
			}
			this.ResetButtonVisuals();
		}

		private void ResetButtonVisuals()
		{
			this._recordButtonText.text = "RECORD";
			this._recordLckToggle.enabled = true;
			this._recordToggle.interactable = true;
			this._recordLckToggle.SetToggleVisualsOff();
		}

		private void EnsureLckService()
		{
			if (this._lckService == null)
			{
				LckLog.LogWarning("LCK Could not get Service", "EnsureLckService", ".\\Packages\\tv.liv.lck\\Runtime\\Scripts\\Tablet\\LckRecordButton.cs", 199);
			}
		}

		private void OnDestroy()
		{
			if (this._lckService != null)
			{
				this._lckService.OnRecordingStarted -= this.OnRecordingStarted;
				this._lckService.OnRecordingStopped -= this.OnRecordingStopped;
				this._lckService.OnRecordingPaused -= this.OnRecordingPaused;
				this._lckService.OnRecordingResumed -= this.OnRecordingResumed;
				this._lckService.OnRecordingSaved -= this.OnRecordingSaved;
			}
		}

		[InjectLck]
		private ILckService _lckService;

		[Header("References")]
		[SerializeField]
		private LckDiscreetAudioController _audioController;

		[SerializeField]
		private TMP_Text _recordButtonText;

		[SerializeField]
		private LckToggle _recordLckToggle;

		[SerializeField]
		private Toggle _recordToggle;

		[Header("Toggle collider when using Direct Tablet")]
		[SerializeField]
		private BoxCollider _collider;

		private LckRecordButton.State _state;

		private enum State
		{
			Idle,
			Saving,
			Paused,
			Recording,
			Error
		}
	}
}
