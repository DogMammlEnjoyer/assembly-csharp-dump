using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Liv.Lck.Core;
using Liv.Lck.Settings;
using Liv.Lck.Telemetry;
using Liv.Lck.Utilities;
using Unity.Collections;
using Unity.Profiling;
using UnityEngine;
using UnityEngine.Experimental.Rendering;
using UnityEngine.Rendering;
using UnityEngine.Scripting;

namespace Liv.Lck
{
	internal class LckPhotoCapture : ILckPhotoCapture, IDisposable
	{
		[Preserve]
		public LckPhotoCapture(ILckVideoTextureProvider videoTextureProvider, ILckEventBus eventBus, ILckTelemetryClient telemetryClient)
		{
			this._videoTextureProvider = videoTextureProvider;
			this._eventBus = eventBus;
			this._telemetryClient = telemetryClient;
			this._renderTexture = this._videoTextureProvider.CameraTrackTexture;
			this._eventBus.AddListener<LckEvents.ActiveCameraTrackTextureChangedEvent>(new Action<LckEvents.ActiveCameraTrackTextureChangedEvent>(this.OnCameraTrackTextureChanged));
		}

		private void OnCameraTrackTextureChanged(LckEvents.ActiveCameraTrackTextureChangedEvent activeCameraTrackTextureChangedEvent)
		{
			this._renderTexture = activeCameraTrackTextureChangedEvent.CameraTrackTextureResult.Result;
		}

		public LckResult Capture()
		{
			if (this._renderTexture == null)
			{
				return LckResult.NewError(LckError.PhotoCaptureError, "Failed to capture photo - No render texture set on LckPhotoCapture");
			}
			Dictionary<string, object> context = new Dictionary<string, object>
			{
				{
					"photo.targetResolutionX",
					this._renderTexture.width
				},
				{
					"photo.targetResolutionY",
					this._renderTexture.height
				},
				{
					"photo.format",
					LckSettings.Instance.ImageCaptureFileFormat.ToString()
				}
			};
			this._telemetryClient.SendTelemetry(new LckTelemetryEvent(LckTelemetryEventType.PhotoCaptured, context));
			this._captureQueue.Enqueue(delegate
			{
				using (LckPhotoCapture._captureProfileMarker.Auto())
				{
					LckSettings.ImageFileFormat imageCaptureFileFormat = LckSettings.Instance.ImageCaptureFileFormat;
					this._imageFilePathBuilder.Clear();
					this._imageFilePathBuilder.Append(Path.Combine(Application.temporaryCachePath, FileUtility.GenerateFilename(LckPhotoCapture.ImageFileFormatStrings[(int)imageCaptureFileFormat])));
					this.SaveRenderTextureToFile(this._imageFilePathBuilder.ToString(), LckSettings.Instance.ImageCaptureFileFormat, new Action<LckResult>(this.OnCaptureComplete));
				}
			});
			if (!this._isCapturing)
			{
				this.ProcessQueue();
			}
			return LckResult.NewSuccess();
		}

		private void ProcessQueue()
		{
			if (this._captureQueue.Count > 0 && !this._isCapturing)
			{
				this._isCapturing = true;
				this._captureQueue.Dequeue()();
			}
		}

		private void OnCaptureComplete(LckResult result)
		{
			if (result.Success)
			{
				LckMonoBehaviourMediator.StartCoroutine("CopyImageToGalleryWhenReady", this.CopyImageToGalleryWhenReady());
				return;
			}
			this._eventBus.Trigger<LckEvents.PhotoCaptureSavedEvent>(new LckEvents.PhotoCaptureSavedEvent(result));
			this._isCapturing = false;
			this.ProcessQueue();
		}

		private IEnumerator CopyImageToGalleryWhenReady()
		{
			while (FileUtility.IsFileLocked(this._imageFilePathBuilder.ToString()) && File.Exists(this._imageFilePathBuilder.ToString()))
			{
				yield return this._copyPhotoSpinWait;
			}
			using (LckPhotoCapture._copyOutputFileToNativeGalleryProfileMarker.Auto())
			{
				Task task = FileUtility.CopyToGallery(this._imageFilePathBuilder.ToString(), LckSettings.Instance.RecordingAlbumName, delegate(bool success, string path)
				{
					LckMonoBehaviourMediator.Instance.EnqueueMainThreadAction(delegate
					{
						if (success)
						{
							LckLog.Log("LCK Photo saved to gallery: " + path);
							this._eventBus.Trigger<LckEvents.PhotoCaptureSavedEvent>(new LckEvents.PhotoCaptureSavedEvent(LckResult.NewSuccess()));
						}
						else
						{
							this._eventBus.Trigger<LckEvents.PhotoCaptureSavedEvent>(new LckEvents.PhotoCaptureSavedEvent(LckResult.NewError(LckError.FailedToCopyPhotoToGallery, "Failed to copy photo to Gallery")));
							LckLog.LogError("LCK Failed to save photo to gallery");
						}
						this._isCapturing = false;
						this.ProcessQueue();
					});
				});
				yield return new WaitUntil(() => task.IsCompleted);
			}
			ProfilerMarker.AutoScope autoScope = default(ProfilerMarker.AutoScope);
			yield break;
			yield break;
		}

		public void SetRenderTexture(RenderTexture renderTexture)
		{
			this._renderTexture = renderTexture;
		}

		private void SaveRenderTextureToFile(string filePath, LckSettings.ImageFileFormat fileFormat, Action<LckResult> onCaptureComplete)
		{
			if (!(this._renderTexture == null))
			{
				int width = this._renderTexture.width;
				int height = this._renderTexture.height;
				GraphicsFormat renderTextureGraphicsFormat = this._renderTexture.graphicsFormat;
				NativeArray<byte> narray = new NativeArray<byte>(width * height * 4, Allocator.Persistent, NativeArrayOptions.UninitializedMemory);
				Action <>9__3;
				Action <>9__4;
				Action <>9__1;
				Action <>9__2;
				AsyncGPUReadback.RequestIntoNativeArray<byte>(ref narray, this._renderTexture, 0, delegate(AsyncGPUReadbackRequest request)
				{
					using (LckPhotoCapture._asyncCallbackProfileMarker.Auto())
					{
						if (!request.hasError)
						{
							Action action;
							if ((action = <>9__1) == null)
							{
								action = (<>9__1 = delegate()
								{
									NativeArray<byte> nativeArray = default(NativeArray<byte>);
									LckPhotoCapture.FillAlphaChannel(narray);
									try
									{
										switch (fileFormat)
										{
										case LckSettings.ImageFileFormat.EXR:
											nativeArray = ImageConversion.EncodeNativeArrayToEXR<byte>(narray, renderTextureGraphicsFormat, (uint)width, (uint)height, 0U, Texture2D.EXRFlags.None);
											break;
										case LckSettings.ImageFileFormat.JPG:
											nativeArray = ImageConversion.EncodeNativeArrayToJPG<byte>(narray, renderTextureGraphicsFormat, (uint)width, (uint)height, 0U, 95);
											break;
										case LckSettings.ImageFileFormat.TGA:
											nativeArray = ImageConversion.EncodeNativeArrayToTGA<byte>(narray, renderTextureGraphicsFormat, (uint)width, (uint)height, 0U);
											break;
										default:
											nativeArray = ImageConversion.EncodeNativeArrayToPNG<byte>(narray, renderTextureGraphicsFormat, (uint)width, (uint)height, 0U);
											break;
										}
										File.WriteAllBytes(filePath, nativeArray.ToArray());
									}
									catch
									{
										LckLog.LogError("LCK Failed to encode image during Photo Capture");
										LckMonoBehaviourMediator instance2 = LckMonoBehaviourMediator.Instance;
										Action action3;
										if ((action3 = <>9__3) == null)
										{
											action3 = (<>9__3 = delegate()
											{
												Action<LckResult> onCaptureComplete3 = onCaptureComplete;
												if (onCaptureComplete3 == null)
												{
													return;
												}
												onCaptureComplete3(LckResult.NewError(LckError.PhotoCaptureError, "Failed to save photo to gallery"));
											});
										}
										instance2.EnqueueMainThreadAction(action3);
									}
									finally
									{
										if (nativeArray.IsCreated)
										{
											nativeArray.Dispose();
										}
										narray.Dispose();
										LckMonoBehaviourMediator instance3 = LckMonoBehaviourMediator.Instance;
										Action action4;
										if ((action4 = <>9__4) == null)
										{
											action4 = (<>9__4 = delegate()
											{
												Action<LckResult> onCaptureComplete3 = onCaptureComplete;
												if (onCaptureComplete3 == null)
												{
													return;
												}
												onCaptureComplete3(LckResult.NewSuccess());
											});
										}
										instance3.EnqueueMainThreadAction(action4);
									}
								});
							}
							Task.Run(action);
						}
						else
						{
							narray.Dispose();
							LckMonoBehaviourMediator instance = LckMonoBehaviourMediator.Instance;
							Action action2;
							if ((action2 = <>9__2) == null)
							{
								action2 = (<>9__2 = delegate()
								{
									Action<LckResult> onCaptureComplete3 = onCaptureComplete;
									if (onCaptureComplete3 == null)
									{
										return;
									}
									onCaptureComplete3(LckResult.NewError(LckError.PhotoCaptureError, "AsyncGPUReadback.RequestIntoNativeArray Failed"));
								});
							}
							instance.EnqueueMainThreadAction(action2);
						}
					}
				});
				return;
			}
			Action<LckResult> onCaptureComplete2 = onCaptureComplete;
			if (onCaptureComplete2 == null)
			{
				return;
			}
			onCaptureComplete2(LckResult.NewError(LckError.PhotoCaptureError, "RenderTexture is null"));
		}

		private static void FillAlphaChannel(NativeArray<byte> narray)
		{
			for (int i = 0; i < narray.Length; i += 4)
			{
				narray[i + 3] = byte.MaxValue;
			}
		}

		public void Dispose()
		{
			this._eventBus.RemoveListener<LckEvents.ActiveCameraTrackTextureChangedEvent>(new Action<LckEvents.ActiveCameraTrackTextureChangedEvent>(this.OnCameraTrackTextureChanged));
		}

		private readonly ILckVideoTextureProvider _videoTextureProvider;

		private readonly ILckEventBus _eventBus;

		private readonly ILckTelemetryClient _telemetryClient;

		private static readonly string[] ImageFileFormatStrings = new string[]
		{
			"exr",
			"jpg",
			"tga",
			"png"
		};

		private RenderTexture _renderTexture;

		private StringBuilder _imageFilePathBuilder = new StringBuilder(256);

		private Queue<Action> _captureQueue = new Queue<Action>();

		private bool _isCapturing;

		private static readonly ProfilerMarker _copyOutputFileToNativeGalleryProfileMarker = new ProfilerMarker("LckPhotoCapture.CopyOutputFileToPhotoGallery");

		private static readonly ProfilerMarker _captureProfileMarker = new ProfilerMarker("LckPhotoCapture.Capture");

		private static readonly ProfilerMarker _asyncCallbackProfileMarker = new ProfilerMarker("LckPhotoCapture.AsyncCallback");

		private WaitForSecondsRealtime _copyPhotoSpinWait = new WaitForSecondsRealtime(0.1f);
	}
}
