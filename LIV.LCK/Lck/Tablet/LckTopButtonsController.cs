using System;
using System.Collections;
using System.Collections.Generic;
using Liv.Lck.DependencyInjection;
using Liv.Lck.UI;
using UnityEngine;
using UnityEngine.Events;

namespace Liv.Lck.Tablet
{
	public class LckTopButtonsController : MonoBehaviour
	{
		internal LckTopButtonsController.TopButtonPage CurrentPage
		{
			get
			{
				return this._currentPage;
			}
		}

		private void Start()
		{
			if (Application.platform != RuntimePlatform.Android && !Application.isEditor && this._topButtonsControllerGameObject)
			{
				this._topButtonsControllerGameObject.SetActive(false);
			}
			this._topButtonsHelper = base.GetComponent<ILckTopButtons>();
			this.ToggleCameraPage(true);
		}

		private void OnApplicationFocus(bool focus)
		{
			if (focus)
			{
				base.StartCoroutine(this.ResetAfterApplicationFocus());
			}
		}

		private IEnumerator ResetAfterApplicationFocus()
		{
			yield return 0;
			if (this._buttonsDisabled)
			{
				this.SetTopButtonsIsDisabledState(true);
			}
			yield break;
		}

		public void SetTopButtonsIsDisabledState(bool isDisabled)
		{
			this._buttonsDisabled = isDisabled;
			if (this._topButtonsHelper == null)
			{
				base.GetComponent<ILckTopButtons>();
			}
			if (isDisabled)
			{
				ILckTopButtons topButtonsHelper = this._topButtonsHelper;
				if (topButtonsHelper == null)
				{
					return;
				}
				topButtonsHelper.HideButtons();
				return;
			}
			else
			{
				ILckTopButtons topButtonsHelper2 = this._topButtonsHelper;
				if (topButtonsHelper2 == null)
				{
					return;
				}
				topButtonsHelper2.ShowButtons();
				return;
			}
		}

		public void ToggleCameraPage(bool state)
		{
			if (this._currentPage == LckTopButtonsController.TopButtonPage.Camera || !state || this._buttonsDisabled)
			{
				return;
			}
			this._currentPage = LckTopButtonsController.TopButtonPage.Camera;
			this._notificationController.HideNotifications();
			this._photoModeController.StopAndResetSequence();
			foreach (GameObject gameObject in this._cameraPageButtons)
			{
				gameObject.SetActive(true);
			}
			foreach (GameObject gameObject2 in this._streamPageButtons)
			{
				gameObject2.SetActive(false);
			}
			this._lckService.SetActiveCaptureType(LckCaptureType.Recording);
			this._onCameraPageOpened.Invoke();
		}

		public void ToggleStreamPage(bool state)
		{
			if (this._currentPage == LckTopButtonsController.TopButtonPage.Stream || !state || this._buttonsDisabled)
			{
				return;
			}
			this._currentPage = LckTopButtonsController.TopButtonPage.Stream;
			this._photoModeController.StopAndResetSequence();
			foreach (GameObject gameObject in this._streamPageButtons)
			{
				gameObject.SetActive(true);
			}
			foreach (GameObject gameObject2 in this._cameraPageButtons)
			{
				gameObject2.SetActive(false);
			}
			this._lckService.SetActiveCaptureType(LckCaptureType.Streaming);
			this._onStreamPageOpened.Invoke();
		}

		[InjectLck]
		private ILckService _lckService;

		[SerializeField]
		private GameObject _topButtonsControllerGameObject;

		[SerializeField]
		private LckNotificationController _notificationController;

		[SerializeField]
		private LckPhotoModeController _photoModeController;

		[SerializeField]
		private List<GameObject> _cameraPageButtons = new List<GameObject>();

		[SerializeField]
		private List<GameObject> _streamPageButtons = new List<GameObject>();

		[Header("Top Button Events")]
		[SerializeField]
		private UnityEvent _onCameraPageOpened = new UnityEvent();

		[SerializeField]
		private UnityEvent _onStreamPageOpened = new UnityEvent();

		private ILckTopButtons _topButtonsHelper;

		private LckTopButtonsController.TopButtonPage _currentPage;

		private bool _buttonsDisabled;

		internal enum TopButtonPage
		{
			Null,
			Camera,
			Stream
		}
	}
}
