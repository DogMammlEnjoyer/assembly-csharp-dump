using System;
using System.Threading;
using AOT;
using UnityEngine;
using UnityEngine.XR;
using UnityEngine.XR.OpenXR;

namespace Oculus.Haptics
{
	public class Haptics : IDisposable
	{
		public static bool IsPCMHaptics { get; private set; }

		public static Haptics Instance
		{
			get
			{
				if (!Haptics.IsSupportedPlatform())
				{
					Debug.LogError("Error: This platform is not supported for haptics");
					Haptics.instance = null;
					return null;
				}
				if (Haptics.instance == null)
				{
					Haptics.instance = new Haptics();
				}
				if (!Haptics.EnsureInitialized())
				{
					Haptics.instance = null;
				}
				return Haptics.instance;
			}
		}

		private static bool IsSupportedPlatform()
		{
			return true;
		}

		private static bool IsPcmHapticsExtensionEnabled()
		{
			string[] enabledExtensions = OpenXRRuntime.GetEnabledExtensions();
			for (int i = 0; i < enabledExtensions.Length; i++)
			{
				if (enabledExtensions[i].Equals("XR_FB_haptic_pcm"))
				{
					return true;
				}
			}
			return false;
		}

		[MonoPInvokeCallback(typeof(Ffi.HapticsSdkPlayCallback))]
		private static void PlayCallback(IntPtr context, Ffi.Controller controller, float duration, float amplitude)
		{
			Haptics.syncContext.Post(delegate(object _)
			{
				Ffi.Controller controller2 = controller;
				if (controller2 == Ffi.Controller.Left)
				{
					InputDevices.GetDeviceAtXRNode(XRNode.LeftHand).SendHapticImpulse(0U, amplitude, duration);
					return;
				}
				if (controller2 != Ffi.Controller.Right)
				{
					return;
				}
				InputDevices.GetDeviceAtXRNode(XRNode.RightHand).SendHapticImpulse(0U, amplitude, duration);
			}, null);
		}

		protected Haptics()
		{
		}

		private static bool EnsureInitialized()
		{
			if (Haptics.IsInitialized())
			{
				return true;
			}
			if (Haptics.IsPcmHapticsExtensionEnabled() && Ffi.Succeeded(Ffi.initialize_with_ovr_plugin("Unity", Application.unityVersion, "78.0.0-mainline.0")))
			{
				Debug.Log("Initialized with OVRPlugin backend");
				Haptics.IsPCMHaptics = true;
				return true;
			}
			if (Ffi.Succeeded(Ffi.initialize_with_callback_backend(IntPtr.Zero, new Ffi.HapticsSdkPlayCallback(Haptics.PlayCallback))))
			{
				Debug.Log("Initialized with callback backend");
				Haptics.syncContext = SynchronizationContext.Current;
				return true;
			}
			Debug.LogError("Error: " + Ffi.error_message());
			return false;
		}

		private static bool IsInitialized()
		{
			bool result;
			if (Ffi.Failed(Ffi.initialized(out result)))
			{
				Debug.LogError("Failed to get initialization state");
				return false;
			}
			return result;
		}

		public int LoadClip(string clipJson)
		{
			int result = -1;
			Ffi.Result result2 = Ffi.load_clip(clipJson, out result);
			if (result2 == Ffi.Result.LoadClipFailed)
			{
				throw new FormatException("Invalid format for clip: " + clipJson + ".");
			}
			if (result2 != Ffi.Result.InvalidUtf8)
			{
				return result;
			}
			throw new FormatException("Invalid UTF8 encoding for clip: " + clipJson + ".");
		}

		public bool ReleaseClip(int clipId)
		{
			return Ffi.Succeeded(Ffi.release_clip(clipId));
		}

		public int CreateHapticPlayer()
		{
			int result = -1;
			Ffi.create_player(out result);
			return result;
		}

		public void SetHapticPlayerClip(int playerId, int clipId)
		{
			Ffi.Result result = Ffi.player_set_clip(playerId, clipId);
			if (result == Ffi.Result.PlayerIdInvalid)
			{
				throw new ArgumentException(string.Format("Invalid player ID: {0}.", playerId));
			}
			if (result != Ffi.Result.ClipIdInvalid)
			{
				return;
			}
			throw new ArgumentException(string.Format("Invalid clipId: {0}.", clipId));
		}

		public void PlayHapticPlayer(int playerId, Controller controller)
		{
			Ffi.Controller controller2 = Utils.ControllerToFfiController(controller);
			Ffi.Result result = Ffi.player_play(playerId, controller2);
			if (result == Ffi.Result.NoClipLoaded)
			{
				throw new InvalidOperationException(string.Format("Player with ID {0} has no clip loaded.", playerId));
			}
			if (result == Ffi.Result.PlayerIdInvalid)
			{
				throw new ArgumentException(string.Format("Invalid player ID: {0}.", playerId));
			}
		}

		public void PauseHapticPlayer(int playerId)
		{
			Ffi.Result result = Ffi.player_pause(playerId);
			if (result == Ffi.Result.NoClipLoaded)
			{
				throw new InvalidOperationException(string.Format("Player with ID {0} has no clip loaded.", playerId));
			}
			if (result == Ffi.Result.PlayerIdInvalid)
			{
				throw new ArgumentException(string.Format("Invalid player ID: {0}.", playerId));
			}
		}

		public void ResumeHapticPlayer(int playerId)
		{
			Ffi.Result result = Ffi.player_resume(playerId);
			if (result == Ffi.Result.NoClipLoaded)
			{
				throw new InvalidOperationException(string.Format("Player with ID {0} has no clip loaded.", playerId));
			}
			if (result == Ffi.Result.PlayerIdInvalid)
			{
				throw new ArgumentException(string.Format("Invalid player ID: {0}.", playerId));
			}
		}

		public void StopHapticPlayer(int playerId)
		{
			Ffi.Result result = Ffi.player_stop(playerId);
			if (result == Ffi.Result.NoClipLoaded)
			{
				throw new InvalidOperationException(string.Format("Player with ID {0} has no clip loaded.", playerId));
			}
			if (result == Ffi.Result.PlayerIdInvalid)
			{
				throw new ArgumentException(string.Format("Invalid player ID: {0}.", playerId));
			}
		}

		public void SeekPlaybackPositionHapticPlayer(int playerId, float time)
		{
			Ffi.Result result = Ffi.player_seek(playerId, time);
			if (result == Ffi.Result.PlayerInvalidSeekPosition)
			{
				throw new ArgumentOutOfRangeException(string.Format("Invalid time: {0} for player {1}.", time, playerId) + "Make sure the value is positive and within the playback duration of the currently loaded clip.");
			}
			if (result == Ffi.Result.NoClipLoaded)
			{
				throw new InvalidOperationException(string.Format("Player with ID {0} has no clip loaded.", playerId));
			}
			if (result == Ffi.Result.PlayerIdInvalid)
			{
				throw new ArgumentException(string.Format("Invalid player ID: {0}.", playerId));
			}
		}

		public float GetClipDuration(int clipId)
		{
			float result = 0f;
			if (Ffi.Result.ClipIdInvalid == Ffi.clip_duration(clipId, out result))
			{
				throw new ArgumentException(string.Format("Invalid clip ID: {0}.", clipId));
			}
			return result;
		}

		public void LoopHapticPlayer(int playerId, bool enabled)
		{
			if (Ffi.Result.PlayerIdInvalid == Ffi.player_set_looping_enabled(playerId, enabled))
			{
				throw new ArgumentException(string.Format("Invalid player ID: {0}.", playerId));
			}
		}

		public bool IsHapticPlayerLooping(int playerId)
		{
			bool result = false;
			if (Ffi.Result.PlayerIdInvalid == Ffi.player_looping_enabled(playerId, out result))
			{
				throw new ArgumentException(string.Format("Invalid player ID: {0}.", playerId));
			}
			return result;
		}

		public void SetAmplitudeHapticPlayer(int playerId, float amplitude)
		{
			Ffi.Result result = Ffi.player_set_amplitude(playerId, amplitude);
			if (result == Ffi.Result.PlayerInvalidAmplitude)
			{
				throw new ArgumentOutOfRangeException(string.Format("Invalid amplitude: {0} for player {1}.", amplitude, playerId) + "Make sure the value is non-negative.");
			}
			if (result == Ffi.Result.PlayerIdInvalid)
			{
				throw new ArgumentException(string.Format("Invalid player ID: {0}.", playerId));
			}
		}

		public float GetAmplitudeHapticPlayer(int playerId)
		{
			float result = 1f;
			if (Ffi.Result.PlayerIdInvalid == Ffi.player_amplitude(playerId, out result))
			{
				throw new ArgumentException(string.Format("Invalid player ID: {0}.", playerId));
			}
			return result;
		}

		public void SetFrequencyShiftHapticPlayer(int playerId, float amount)
		{
			Ffi.Result result = Ffi.player_set_frequency_shift(playerId, amount);
			if (result == Ffi.Result.PlayerInvalidFrequencyShift)
			{
				throw new ArgumentOutOfRangeException(string.Format("Invalid frequency shift amount: {0} for player {1}.", amount, playerId) + "Make sure the value is on the range -1.0 to 1.0 (inclusive).");
			}
			if (result == Ffi.Result.PlayerIdInvalid)
			{
				throw new ArgumentException(string.Format("Invalid player ID: {0}.", playerId));
			}
		}

		public float GetFrequencyShiftHapticPlayer(int playerId)
		{
			float result = 0f;
			if (Ffi.Result.PlayerIdInvalid == Ffi.player_frequency_shift(playerId, out result))
			{
				throw new ArgumentException(string.Format("Invalid player ID: {0}.", playerId));
			}
			return result;
		}

		private static uint MapPriority(uint input, int inMin, int inMax, int outMin, int outMax)
		{
			uint result;
			try
			{
				result = checked((uint)Math.Round((double)Utils.Map((int)input, inMin, inMax, outMin, outMax)));
			}
			catch (OverflowException)
			{
				throw new ArgumentOutOfRangeException(string.Format("Invalid priority value: {0}. ", input) + "Make sure the value is within the range 0 to 255 (inclusive).");
			}
			return result;
		}

		public void SetPriorityHapticPlayer(int playerId, uint value)
		{
			Ffi.Result result = Ffi.player_set_priority(playerId, Haptics.MapPriority(value, 0, 255, 1024, 0));
			if (result == Ffi.Result.PlayerInvalidPriority)
			{
				throw new ArgumentOutOfRangeException(string.Format("Invalid priority value: {0} for player {1}. ", value, playerId) + "Make sure the value is within the range 0 to 255 (inclusive).");
			}
			if (result == Ffi.Result.PlayerIdInvalid)
			{
				throw new ArgumentException(string.Format("Invalid player ID: {0}.", playerId));
			}
		}

		public uint GetPriorityHapticPlayer(int playerId)
		{
			uint input = 128U;
			if (Ffi.Result.PlayerIdInvalid == Ffi.player_priority(playerId, out input))
			{
				throw new ArgumentException(string.Format("Invalid player ID: {0}.", playerId));
			}
			return Haptics.MapPriority(input, 0, 1024, 255, 0);
		}

		public bool ReleaseHapticPlayer(int playerId)
		{
			return Ffi.Succeeded(Ffi.release_player(playerId));
		}

		public void Dispose()
		{
			this.Dispose(true);
			GC.SuppressFinalize(this);
		}

		protected virtual void Dispose(bool disposing)
		{
			if (Haptics.instance != null)
			{
				if (Haptics.IsInitialized() && Ffi.Failed(Ffi.uninitialize()))
				{
					Debug.LogError("Error: " + Ffi.error_message());
				}
				Haptics.instance = null;
			}
		}

		~Haptics()
		{
			this.Dispose(false);
		}

		protected static Haptics instance;

		public const string HapticsSDKTelemetryName = "haptics_sdk";

		private static SynchronizationContext syncContext;
	}
}
