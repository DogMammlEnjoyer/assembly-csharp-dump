using System;
using System.Collections;
using System.Collections.Generic;
using Liv.Lck.DependencyInjection;
using Liv.Lck.Recorder;
using UnityEngine;

namespace Liv.Lck.GorillaTag
{
	public class GtNotificationController : MonoBehaviour
	{
		private void Start()
		{
			this._questMessage.SetActive(false);
			this._pcMessage.SetActive(true);
		}

		private void OnEnable()
		{
			this._ui.SetActive(false);
			this._lckService.OnRecordingStarted += this.OnRecordingStarted;
			this._lckService.OnRecordingSaved += this.OnRecordingSaved;
		}

		private void OnDisable()
		{
			if (this._lckService != null)
			{
				this._lckService.OnRecordingStarted -= this.OnRecordingStarted;
				this._lckService.OnRecordingSaved -= this.OnRecordingSaved;
			}
		}

		private void OnRecordingStarted(LckResult result)
		{
			this._ui.SetActive(false);
			base.StopAllCoroutines();
			if (!this._hiddenObjectsState)
			{
				this.SetHiddenObjectsState(true);
			}
		}

		private void OnRecordingSaved(LckResult<RecordingData> result)
		{
			if (!result.Success)
			{
				Debug.LogWarning("Failed to create notification. Error: " + result.Error.ToString() + " Message: " + result.Message);
				return;
			}
			this._ui.SetActive(true);
			base.StartCoroutine(this.NotificationTimer());
		}

		private IEnumerator NotificationTimer()
		{
			this.SetHiddenObjectsState(false);
			yield return new WaitForSeconds(this._notificationShowDuration);
			this._ui.SetActive(false);
			this.SetHiddenObjectsState(true);
			yield break;
		}

		private void SetHiddenObjectsState(bool state)
		{
			this._hiddenObjectsState = state;
			foreach (GameObject gameObject in this._hiddenDuringNotification)
			{
				gameObject.SetActive(state);
			}
		}

		[InjectLck]
		private ILckService _lckService;

		[SerializeField]
		private GameObject _ui;

		[SerializeField]
		private GameObject _questMessage;

		[SerializeField]
		private GameObject _pcMessage;

		[SerializeField]
		private float _notificationShowDuration = 4f;

		[SerializeField]
		private List<GameObject> _hiddenDuringNotification = new List<GameObject>();

		private bool _hiddenObjectsState = true;
	}
}
