using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Liv.Lck.Core;
using Liv.Lck.DependencyInjection;
using Liv.Lck.Recorder;
using UnityEngine;

namespace Liv.Lck.Tablet
{
	public class LckNotificationController : MonoBehaviour
	{
		private void Awake()
		{
			this.InitializeNotifications();
		}

		private void Start()
		{
			this.CheckInitializationAfterDelay();
		}

		private void CheckInitializationAfterDelay()
		{
			LckNotificationController.<CheckInitializationAfterDelay>d__9 <CheckInitializationAfterDelay>d__;
			<CheckInitializationAfterDelay>d__.<>t__builder = AsyncVoidMethodBuilder.Create();
			<CheckInitializationAfterDelay>d__.<>4__this = this;
			<CheckInitializationAfterDelay>d__.<>1__state = -1;
			<CheckInitializationAfterDelay>d__.<>t__builder.Start<LckNotificationController.<CheckInitializationAfterDelay>d__9>(ref <CheckInitializationAfterDelay>d__);
		}

		private void OnValidate()
		{
			foreach (InitializerNotification initializerNotification in this._notificationsInitializer)
			{
				initializerNotification.Name = ((initializerNotification.prefab != null) ? initializerNotification.Type.ToString() : null);
			}
		}

		private void OnEnable()
		{
			this._lckService.OnRecordingStarted += this.OnCaptureStarted;
			this._lckService.OnStreamingStarted += this.OnCaptureStarted;
			this._lckService.OnRecordingSaved += this.OnRecordingSaved;
			if (this._currentNotification != null && this._currentNotification.RemainOnScreen)
			{
				this._onScreenUIController.OnNotificationStarted();
			}
		}

		private void OnDisable()
		{
			if (this._lckService == null)
			{
				return;
			}
			this._lckService.OnRecordingStarted -= this.OnCaptureStarted;
			this._lckService.OnStreamingStarted -= this.OnCaptureStarted;
			this._lckService.OnRecordingSaved -= this.OnRecordingSaved;
			if (this._currentNotification != null && !this._currentNotification.RemainOnScreen)
			{
				this.HideNotifications();
			}
		}

		public void SetNotificationStreamCode(string code)
		{
			LckBaseNotification lckBaseNotification;
			if (this._notifications.TryGetValue(NotificationType.EnterStreamCode, out lckBaseNotification))
			{
				LckNormalNotification lckNormalNotification = lckBaseNotification as LckNormalNotification;
				if (lckNormalNotification != null)
				{
					lckNormalNotification.Text.text = code;
					return;
				}
			}
			else
			{
				Debug.LogError("No 'EnterStreamCode' notification prefab is configured in the LckNotificationController.");
			}
		}

		private void OnCaptureStarted(LckResult result)
		{
			if (result.Success)
			{
				this.HideNotifications();
			}
		}

		private void OnRecordingSaved(LckResult<RecordingData> result)
		{
			if (result.Success)
			{
				this.ShowNotification(NotificationType.VideoSaved);
				return;
			}
			Debug.LogWarning(string.Format("Failed to show 'VideoSaved' notification. Error: {0}, Message: {1}", result.Error, result.Message));
		}

		public void HideNotifications()
		{
			base.StopAllCoroutines();
			if (this._currentNotification != null)
			{
				this._onScreenUIController.OnNotificationEnded();
			}
			this._currentNotification = null;
			foreach (KeyValuePair<NotificationType, LckBaseNotification> keyValuePair in this._notifications)
			{
				keyValuePair.Value.HideNotification();
			}
		}

		public void InitializeNotifications()
		{
			this.DestroyNotifications();
			foreach (InitializerNotification initializerNotification in this._notificationsInitializer)
			{
				GameObject gameObject = Object.Instantiate<GameObject>(initializerNotification.prefab);
				gameObject.SetActive(false);
				gameObject.transform.SetParent(this._notificationsTransform, false);
				LckBaseNotification component = gameObject.GetComponent<LckBaseNotification>();
				if (component != null)
				{
					this._notifications.Add(initializerNotification.Type, component);
					component.SetSpawnedGameObject(gameObject);
				}
				else
				{
					Debug.LogError(string.Format("Prefab for notification type '{0}' is missing a component that inherits from LckBaseNotification.", initializerNotification.Type), gameObject);
				}
			}
		}

		public void DestroyNotifications()
		{
			this._notifications.Clear();
			if (this._notificationsTransform.childCount > 0)
			{
				for (int i = this._notificationsTransform.childCount - 1; i >= 0; i--)
				{
					GameObject gameObject = this._notificationsTransform.GetChild(i).gameObject;
					if (Application.isPlaying)
					{
						Object.Destroy(gameObject);
					}
					else
					{
						Object.DestroyImmediate(gameObject);
					}
				}
			}
		}

		public void ShowNotification(NotificationType type)
		{
			Result<bool> lckCoreInitializationResult = LckCoreHandler.LckCoreInitializationResult;
			if (lckCoreInitializationResult != null && !lckCoreInitializationResult.IsOk)
			{
				Debug.LogError("Failed to show notification: " + type.ToString() + " LckCore failed initialization");
				return;
			}
			this.HideNotifications();
			base.StartCoroutine(this.CreateNotification(type));
		}

		private IEnumerator CreateNotification(NotificationType type)
		{
			this._onScreenUIController.OnNotificationStarted();
			if (!this._notifications.TryGetValue(type, out this._currentNotification))
			{
				Debug.LogError("No notification found with type: " + type.ToString());
				this._onScreenUIController.OnNotificationEnded();
				yield break;
			}
			this._currentNotification.ShowNotification();
			if (this._currentNotification.RemainOnScreen)
			{
				yield break;
			}
			yield return new WaitForSeconds(this._notificationShowDuration);
			this._currentNotification.HideNotification();
			this._onScreenUIController.OnNotificationEnded();
			this._currentNotification = null;
			yield break;
		}

		[InjectLck]
		private ILckService _lckService;

		[Tooltip("Configure the list of all possible notifications. Drag your notification prefabs here and assign them a type.")]
		[SerializeField]
		private List<InitializerNotification> _notificationsInitializer = new List<InitializerNotification>();

		private readonly Dictionary<NotificationType, LckBaseNotification> _notifications = new Dictionary<NotificationType, LckBaseNotification>();

		private LckBaseNotification _currentNotification;

		[Tooltip("The default duration in seconds that a notification will remain on screen before automatically hiding. This can be overridden by the notification itself.")]
		[SerializeField]
		private float _notificationShowDuration = 3f;

		[Tooltip("The parent Transform under which all notification prefabs will be instantiated.")]
		[SerializeField]
		private Transform _notificationsTransform;

		[Tooltip("A reference to a higher-level UI controller that may need to react when notifications appear or disappear (e.g., to adjust layout).")]
		[SerializeField]
		private LckOnScreenUIController _onScreenUIController;
	}
}
