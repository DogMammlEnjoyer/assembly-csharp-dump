using System;
using Liv.Lck.DependencyInjection;
using UnityEngine;

namespace Liv.Lck.GorillaTag
{
	public class GtDummyTablet : MonoBehaviour
	{
		private void OnEnable()
		{
			if (this._lckService == null)
			{
				return;
			}
			this._lckService.OnRecordingStarted += this.OnCaptureStarted;
			this._lckService.OnRecordingStopped += this.OnCaptureStopped;
			this._lckService.OnStreamingStarted += this.OnCaptureStarted;
			this._lckService.OnStreamingStopped += this.OnCaptureStopped;
		}

		public void OnTabletCosmeticSpawned(GameObject cosmetic)
		{
			if (this._isLCKWallCameraSpawner)
			{
				cosmetic.SetActive(true);
			}
			else
			{
				cosmetic.SetActive(!this._ghostBody.activeSelf);
			}
			this._cosmeticTablet = cosmetic;
		}

		public void OnEmobiCosmeticSpawned(GameObject cosmetic)
		{
			if (this._isLCKWallCameraSpawner)
			{
				cosmetic.SetActive(true);
			}
			else
			{
				cosmetic.SetActive(!this._ghostBody.activeSelf);
			}
			this._cosmeticEmobi = cosmetic;
		}

		public void SetDummyTabletBodyState(bool isActive)
		{
			if (this._isLCKWallCameraSpawner)
			{
				return;
			}
			this._ghostBody.SetActive(!isActive);
			if (this._recordingIndicator != null)
			{
				this._recordingIndicator.SetActive(isActive);
			}
			if (this._cosmeticTablet != null && this._cosmeticEmobi != null)
			{
				this._body.SetActive(false);
				this._cosmeticTablet.SetActive(isActive);
				this._cosmeticEmobi.SetActive(isActive);
				return;
			}
			this._body.SetActive(isActive);
		}

		private void OnDisable()
		{
			if (this._lckService == null)
			{
				return;
			}
			this._lckService.OnRecordingStarted -= this.OnCaptureStarted;
			this._lckService.OnRecordingStopped -= this.OnCaptureStopped;
			this._lckService.OnStreamingStarted -= this.OnCaptureStarted;
			this._lckService.OnStreamingStopped -= this.OnCaptureStopped;
		}

		private void Start()
		{
			this.SetState(this._isCapturing);
		}

		public void SetState(bool isCapturing)
		{
			this._isCapturing = isCapturing;
			Material[] materials = this._renderer.materials;
			materials[this._recordingButtonIndex] = (this._isCapturing ? this._recordingMaterial : this._defaultMaterial);
			this._renderer.materials = materials;
		}

		private void OnCaptureStarted(LckResult result)
		{
			if (result.Success)
			{
				this.SetState(true);
			}
		}

		private void OnCaptureStopped(LckResult result)
		{
			this.SetState(false);
		}

		[InjectLck]
		private ILckService _lckService;

		[SerializeField]
		private bool _isLCKWallCameraSpawner;

		[SerializeField]
		private MeshRenderer _renderer;

		[SerializeField]
		private GameObject _body;

		[SerializeField]
		private GameObject _ghostBody;

		[SerializeField]
		private Material _defaultMaterial;

		[SerializeField]
		private Material _recordingMaterial;

		private GameObject _cosmeticTablet;

		private GameObject _cosmeticEmobi;

		[SerializeField]
		private int _recordingButtonIndex;

		[SerializeField]
		private GameObject _recordingIndicator;

		private bool _isCapturing;
	}
}
