using System;
using UnityEngine;

namespace Liv.Lck.GorillaTag
{
	public class GtTabletSocialCameraVisuals : MonoBehaviour, IGtCameraVisuals
	{
		public void SetVisualsActive(bool active)
		{
			this._visuals.SetActive(active);
			this.SetRecordingState(this._isRecording);
		}

		public void SetNetworkedVisualsActive(bool active)
		{
			this._visuals.SetActive(active);
		}

		public void SetRecordingState(bool isRecording)
		{
			this._recordingIndicatorRoot.SetActive(isRecording);
			this._isRecording = isRecording;
		}

		[SerializeField]
		private GameObject _visuals;

		[SerializeField]
		private GameObject _recordingIndicatorRoot;

		private bool _isRecording;
	}
}
