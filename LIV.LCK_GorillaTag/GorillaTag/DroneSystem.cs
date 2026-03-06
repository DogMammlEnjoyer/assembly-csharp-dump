using System;
using Liv.Lck.DependencyInjection;
using UnityEngine;

namespace Liv.Lck.GorillaTag
{
	public class DroneSystem : MonoBehaviour
	{
		public event DroneSystem.OnRequestDroneModeDelegate OnRequestDroneModeState;

		private void Awake()
		{
			if (Application.platform == RuntimePlatform.Android)
			{
				Object.Destroy(this);
				return;
			}
			this._droneController = Object.Instantiate<GameObject>(this._dronePrefab).GetComponent<DroneController>();
			this._droneController.GetModel().OnIsDroneModeActive += this.ProcessDroneMode;
		}

		private void Start()
		{
			if (this._lckService != null)
			{
				this._lckService.OnRecordingStarted += this.OnRecordingStarted;
			}
		}

		private void OnRecordingStarted(LckResult result)
		{
			if (result.Success)
			{
				this._gtController.SetOrientationQualityAndTopButtonsIsDisabledState(true);
			}
		}

		private void ProcessDroneMode(bool value)
		{
			DroneSystem.OnRequestDroneModeDelegate onRequestDroneModeState = this.OnRequestDroneModeState;
			if (onRequestDroneModeState == null)
			{
				return;
			}
			onRequestDroneModeState(value);
		}

		internal void SetDronePositionAndRotation(Vector3 position, Quaternion rotation)
		{
			this._droneController.SetDronePositionAndRotation(position, rotation);
		}

		internal ILckCamera GetLckCamera()
		{
			return this._droneController.GetLckCamera();
		}

		private void OnDestroy()
		{
			if (this._droneController != null)
			{
				Object.Destroy(this._droneController.gameObject);
			}
			if (this._lckService != null)
			{
				this._lckService.OnRecordingStarted -= this.OnRecordingStarted;
			}
		}

		[SerializeField]
		private GameObject _dronePrefab;

		private DroneController _droneController;

		[SerializeField]
		private GTLckController _gtController;

		[InjectLck]
		private ILckService _lckService;

		public delegate void OnRequestDroneModeDelegate(bool isActive);
	}
}
