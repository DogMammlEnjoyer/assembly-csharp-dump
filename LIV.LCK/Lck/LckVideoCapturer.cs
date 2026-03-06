using System;
using System.Collections;
using System.Diagnostics;
using Liv.Lck.Encoding;
using UnityEngine.Scripting;

namespace Liv.Lck
{
	internal class LckVideoCapturer : ILckVideoCapturer, IDisposable
	{
		[Preserve]
		public LckVideoCapturer(ILckVideoTextureProvider videoTextureProvider, ILckActiveCameraConfigurer activeCameraConfigurer, ILckPreviewer previewer, ILckEncoder encoder, ILckOutputConfigurer outputConfigurer, ILckEventBus eventBus)
		{
			this._videoTextureProvider = videoTextureProvider;
			this._activeCameraConfigurer = activeCameraConfigurer;
			this._previewer = previewer;
			this._encoder = encoder;
			this._eventBus = eventBus;
			uint framerate = outputConfigurer.GetActiveCameraTrackDescriptor().Result.Framerate;
			this.SetTargetCaptureFramerate(framerate);
			this._eventBus.AddListener<LckEvents.CameraFramerateChangedEvent>(new Action<LckEvents.CameraFramerateChangedEvent>(this.OnCameraFramerateChanged));
		}

		private void OnCameraFramerateChanged(LckEvents.CameraFramerateChangedEvent cameraFramerateChangedEvent)
		{
			LckResult<uint> result = cameraFramerateChangedEvent.Result;
			if (result.Success)
			{
				this.SetTargetCaptureFramerate(result.Result);
			}
		}

		public bool ForceCaptureAllFrames { get; set; }

		public bool IsCapturing { get; private set; }

		public void StartCapturing()
		{
			this.IsCapturing = true;
			LckMonoBehaviourMediator.StartCoroutine("LckCaptureLooper:CaptureLoopCoroutine", this.CaptureLoopCoroutine());
		}

		public void StopCapturing()
		{
			this.IsCapturing = false;
			LckMonoBehaviourMediator.StopCoroutineByName("LckCaptureLooper:CaptureLoopCoroutine");
		}

		public bool HasCurrentFrameBeenCaptured()
		{
			return this._frameHasBeenRendered;
		}

		private void SetTargetCaptureFramerate(uint targetCaptureFramerate)
		{
			this._targetSecondsPerCapture = 1.0 / targetCaptureFramerate;
		}

		private IEnumerator CaptureLoopCoroutine()
		{
			this._captureStopwatch.Start();
			this._captureTimeOverflow = 0.0;
			while (this.IsCapturing)
			{
				this.HandleCameraFrame();
				yield return null;
			}
			yield break;
		}

		private void PrepareCameraForCapture(ILckCamera camera)
		{
			if (this.CaptureCanBeCulled())
			{
				this._frameHasBeenRendered = false;
				camera.DeactivateCamera();
				return;
			}
			this._frameHasBeenRendered = true;
			camera.ActivateCamera(this._videoTextureProvider.CameraTrackTexture);
		}

		private void HandleCameraFrame(ILckCamera activeCamera)
		{
			double totalSeconds = this._captureStopwatch.Elapsed.TotalSeconds;
			bool flag = totalSeconds + this._captureTimeOverflow >= this._targetSecondsPerCapture;
			if (this.ForceCaptureAllFrames || flag)
			{
				double num = totalSeconds - this._targetSecondsPerCapture;
				this._captureTimeOverflow = (this._captureTimeOverflow + num) % this._targetSecondsPerCapture;
				this._captureStopwatch.Restart();
				this.PrepareCameraForCapture(activeCamera);
				return;
			}
			this._frameHasBeenRendered = false;
			activeCamera.DeactivateCamera();
		}

		private void HandleCameraFrame()
		{
			LckResult<ILckCamera> activeCamera = this._activeCameraConfigurer.GetActiveCamera();
			if (!activeCamera.Success)
			{
				return;
			}
			ILckCamera result = activeCamera.Result;
			if (result == null)
			{
				return;
			}
			this.HandleCameraFrame(result);
		}

		private bool CaptureCanBeCulled()
		{
			return !this._encoder.IsActive() && !this._previewer.IsPreviewActive;
		}

		public void Dispose()
		{
			if (this.IsCapturing)
			{
				this.StopCapturing();
			}
			this._eventBus.RemoveListener<LckEvents.CameraFramerateChangedEvent>(new Action<LckEvents.CameraFramerateChangedEvent>(this.OnCameraFramerateChanged));
		}

		private readonly ILckVideoTextureProvider _videoTextureProvider;

		private readonly ILckActiveCameraConfigurer _activeCameraConfigurer;

		private readonly ILckPreviewer _previewer;

		private readonly ILckEncoder _encoder;

		private readonly ILckEventBus _eventBus;

		private readonly Stopwatch _captureStopwatch = new Stopwatch();

		private bool _frameHasBeenRendered;

		private double _captureTimeOverflow;

		private double _targetSecondsPerCapture;

		private const string CaptureLoopCoroutineName = "LckCaptureLooper:CaptureLoopCoroutine";
	}
}
