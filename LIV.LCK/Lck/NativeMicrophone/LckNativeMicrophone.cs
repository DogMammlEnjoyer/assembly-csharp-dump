using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using AOT;
using Liv.Lck.Collections;
using Liv.Lck.Settings;
using UnityEngine;

namespace Liv.Lck.NativeMicrophone
{
	public class LckNativeMicrophone : IDisposable, ILckAudioSource
	{
		[DllImport("native_microphone", CallingConvention = CallingConvention.Cdecl)]
		private static extern ulong microphone_capture_new(uint sampleRate);

		[DllImport("native_microphone", CallingConvention = CallingConvention.Cdecl)]
		private static extern LckNativeMicrophone.ReturnCode microphone_capture_free(ulong audioCaptureKey);

		[DllImport("native_microphone", CallingConvention = CallingConvention.Cdecl)]
		private static extern LckNativeMicrophone.ReturnCode microphone_capture_start(ulong audioCaptureKey);

		[DllImport("native_microphone", CallingConvention = CallingConvention.Cdecl)]
		private static extern LckNativeMicrophone.ReturnCode microphone_capture_stop(ulong audioCaptureKey);

		[DllImport("native_microphone", CallingConvention = CallingConvention.Cdecl)]
		private static extern LckNativeMicrophone.ReturnCode microphone_capture_get_audio(ulong audioCaptureKey, IntPtr callback);

		[DllImport("native_microphone", CallingConvention = CallingConvention.Cdecl)]
		private static extern void set_max_log_level(LogLevel levelFilter);

		public LckNativeMicrophone(int sampleRate)
		{
			LckNativeMicrophone.SetMaxLogLevel(LckSettings.Instance.MicrophoneLogLevel);
			this._callback = new LckNativeMicrophone.AudioDataCallbackDelegate(LckNativeMicrophone.AudioDataCallback);
			this._callbackPtr = Marshal.GetFunctionPointerForDelegate<LckNativeMicrophone.AudioDataCallbackDelegate>(this._callback);
			this._nativeInstance = LckNativeMicrophone.microphone_capture_new((uint)sampleRate);
			LckNativeMicrophone._instances.Add(this._nativeInstance, this);
		}

		[MonoPInvokeCallback(typeof(LckNativeMicrophone.AudioDataCallbackDelegate))]
		private static void AudioDataCallback(IntPtr dataPtr, int length, ulong audioCaptureKey)
		{
			LckNativeMicrophone lckNativeMicrophone;
			if (LckNativeMicrophone._instances.TryGetValue(audioCaptureKey, out lckNativeMicrophone))
			{
				try
				{
					if (lckNativeMicrophone._audioBuffer.Capacity < length)
					{
						LckLog.LogWarning(string.Format("LCK Native Microphone dropping audio: {0} < {1}", lckNativeMicrophone._audioBuffer.Capacity, length));
					}
					int count = Mathf.Min(length, lckNativeMicrophone._audioBuffer.Capacity);
					if (!lckNativeMicrophone._audioBuffer.TryCopyFrom(dataPtr, count))
					{
						LckLog.LogError("LCK Mic Audio data copy failed");
					}
					return;
				}
				catch (Exception ex)
				{
					Debug.LogError("LCK Exception during mic audio copy: " + ex.Message);
					return;
				}
			}
			LckLog.LogError("LCK NativeMicrophone: Could not find instance for key: " + audioCaptureKey.ToString());
		}

		[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterAssembliesLoaded)]
		public static void InitMicrophone()
		{
		}

		public void Dispose()
		{
			if (this._nativeInstance != 0UL)
			{
				LckNativeMicrophone.microphone_capture_free(this._nativeInstance);
				LckNativeMicrophone._instances.Remove(this._nativeInstance);
				this._nativeInstance = 0UL;
			}
		}

		public bool IsCapturing()
		{
			return this._isCapturing;
		}

		public void GetAudioData(ILckAudioSource.AudioDataCallbackDelegate callback)
		{
			this._audioBuffer.Clear();
			if (this._isCapturing)
			{
				LckNativeMicrophone.microphone_capture_get_audio(this._nativeInstance, this._callbackPtr);
			}
			callback(this._audioBuffer);
		}

		public void EnableCapture()
		{
			this._shouldEnableCapture = true;
			this._shouldDisableCapture = false;
			if (this._setMicStateTask == null)
			{
				this._setMicStateTask = Task.Run(() => this.SetMicrophoneCaptureActive(true));
			}
		}

		public void DisableCapture()
		{
			this._shouldDisableCapture = true;
			this._shouldEnableCapture = false;
			if (this._setMicStateTask == null)
			{
				this._setMicStateTask = Task.Run(() => this.SetMicrophoneCaptureActive(false));
			}
		}

		private Task SetMicrophoneCaptureActive(bool active)
		{
			LckNativeMicrophone.<SetMicrophoneCaptureActive>d__26 <SetMicrophoneCaptureActive>d__;
			<SetMicrophoneCaptureActive>d__.<>t__builder = AsyncTaskMethodBuilder.Create();
			<SetMicrophoneCaptureActive>d__.<>4__this = this;
			<SetMicrophoneCaptureActive>d__.active = active;
			<SetMicrophoneCaptureActive>d__.<>1__state = -1;
			<SetMicrophoneCaptureActive>d__.<>t__builder.Start<LckNativeMicrophone.<SetMicrophoneCaptureActive>d__26>(ref <SetMicrophoneCaptureActive>d__);
			return <SetMicrophoneCaptureActive>d__.<>t__builder.Task;
		}

		public static void SetMaxLogLevel(LogLevel logLevel)
		{
			LckNativeMicrophone.set_max_log_level(logLevel);
		}

		private static Dictionary<ulong, LckNativeMicrophone> _instances = new Dictionary<ulong, LckNativeMicrophone>();

		private const string __DllName = "native_microphone";

		private ulong _nativeInstance;

		private LckNativeMicrophone.AudioDataCallbackDelegate _callback;

		private AudioBuffer _audioBuffer = new AudioBuffer(96000);

		private IntPtr _callbackPtr;

		private bool _isCapturing;

		private bool _shouldDisableCapture;

		private bool _shouldEnableCapture;

		private Task _setMicStateTask;

		public enum ReturnCode : uint
		{
			Ok,
			Error,
			InvalidKey,
			DefaultInputDeviceError,
			BuildStreamError,
			NoAudioData,
			LoggerAlreadySet,
			CaptureNotStarted
		}

		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		private delegate void AudioDataCallbackDelegate(IntPtr dataPtr, int length, ulong audioCaptureKey);
	}
}
