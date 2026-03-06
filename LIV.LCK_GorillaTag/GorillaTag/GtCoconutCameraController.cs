using System;
using Liv.Lck.DependencyInjection;
using UnityEngine;

namespace Liv.Lck.GorillaTag
{
	[DefaultExecutionOrder(100)]
	public class GtCoconutCameraController : MonoBehaviour
	{
		private void OnEnable()
		{
			if (this._lckService == null)
			{
				Debug.LogError("LckService is null");
				return;
			}
			this._lckService.OnRecordingStarted += this.OnRecordingStarted;
			this._lckService.OnRecordingStopped += this.OnRecordingStopped;
		}

		private void Start()
		{
			this._cocoCamera.SetVisualsActive(!this._hideOnStart);
		}

		private void OnDisable()
		{
			if (this._lckService == null)
			{
				return;
			}
			this._lckService.OnRecordingStarted -= this.OnRecordingStarted;
			this._lckService.OnRecordingStopped -= this.OnRecordingStopped;
		}

		private void OnRecordingStarted(LckResult result)
		{
			this._cocoCamera.SetRecordingState(true);
		}

		private void OnRecordingStopped(LckResult result)
		{
			this._cocoCamera.SetRecordingState(false);
		}

		[InjectLck]
		private ILckService _lckService;

		[SerializeField]
		private CoconutCamera _cocoCamera;

		[SerializeField]
		private bool _hideOnStart = true;
	}
}
