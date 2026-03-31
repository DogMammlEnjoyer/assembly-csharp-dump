using System;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using AOT;
using Liv.Lck.Core.FFI;

namespace Liv.Lck.Core
{
	public static class LckCore
	{
		[MonoPInvokeCallback(typeof(LckCoreNative.start_login_attempt_callback_delegate))]
		private static void StartLoginAttemptCallback(ReturnCode returnCode, IntPtr loginCodePtr)
		{
			object loginLock = LckCore._loginLock;
			lock (loginLock)
			{
				LckCore._lastReturnCode = returnCode;
				if (returnCode == ReturnCode.Ok)
				{
					LckCore._loginCode = InteropUtilities.UTF8PointerToString(loginCodePtr);
				}
			}
		}

		public static void SetMaxLogLevel(LevelFilter levelFilter)
		{
			LckCoreNative.set_max_log_level(levelFilter);
		}

		public static Result<bool> Initialize(string trackingId, GameInfo gameInfo, LckInfo lckInfo)
		{
			if (string.IsNullOrEmpty(trackingId))
			{
				return Result<bool>.NewError(CoreError.MissingTrackingId, "Tracking ID cannot be null or empty.");
			}
			IntPtr intPtr = InteropUtilities.StringToUTF8Pointer(trackingId);
			GameInfo game_info = GameInfo.AllocateFromGameInfo(gameInfo);
			LckInfo lck_info = LckInfo.AllocateFromLckInfo(lckInfo);
			ReturnCode returnCode;
			try
			{
				returnCode = LckCoreNative.initialize(intPtr, game_info, lck_info);
			}
			finally
			{
				InteropUtilities.Free(intPtr);
				game_info.Free();
				lck_info.Free();
			}
			if (returnCode == ReturnCode.Ok)
			{
				return Result<bool>.NewSuccess(true);
			}
			if (returnCode == ReturnCode.InvalidArgument)
			{
				return Result<bool>.NewError(CoreError.InvalidArgument, "Invalid argument provided to initialize LckCore.");
			}
			if (returnCode != ReturnCode.InvalidTrackingId)
			{
				return Result<bool>.NewError(CoreError.InternalError, string.Format("Failed to initialize LckCore: {0}", returnCode));
			}
			return Result<bool>.NewError(CoreError.InvalidTrackingId, "Provided Tracking ID is not valid.");
		}

		public static Task<Result<bool>> HasUserConfiguredStreaming()
		{
			LckCore.<HasUserConfiguredStreaming>d__6 <HasUserConfiguredStreaming>d__;
			<HasUserConfiguredStreaming>d__.<>t__builder = AsyncTaskMethodBuilder<Result<bool>>.Create();
			<HasUserConfiguredStreaming>d__.<>1__state = -1;
			<HasUserConfiguredStreaming>d__.<>t__builder.Start<LckCore.<HasUserConfiguredStreaming>d__6>(ref <HasUserConfiguredStreaming>d__);
			return <HasUserConfiguredStreaming>d__.<>t__builder.Task;
		}

		private static ValueTuple<CoreError, string> MapReturnCodeToCoreError(ReturnCode returnCode)
		{
			if (returnCode == ReturnCode.BackendUnavailable)
			{
				return new ValueTuple<CoreError, string>(CoreError.ServiceUnavailable, "LIV backend service is unavailable.");
			}
			if (returnCode == ReturnCode.UserNotLoggedIn)
			{
				return new ValueTuple<CoreError, string>(CoreError.UserNotLoggedIn, "User is not logged in.");
			}
			if (returnCode != ReturnCode.RateLimiterBackoff)
			{
				return new ValueTuple<CoreError, string>(CoreError.InternalError, string.Format("Operation failed with return code: {0}", returnCode));
			}
			return new ValueTuple<CoreError, string>(CoreError.RateLimiterBackoff, "Client is in rate limiter backoff due to previous errors.");
		}

		public static Task<Result<bool>> IsUserSubscribed()
		{
			LckCore.<IsUserSubscribed>d__8 <IsUserSubscribed>d__;
			<IsUserSubscribed>d__.<>t__builder = AsyncTaskMethodBuilder<Result<bool>>.Create();
			<IsUserSubscribed>d__.<>1__state = -1;
			<IsUserSubscribed>d__.<>t__builder.Start<LckCore.<IsUserSubscribed>d__8>(ref <IsUserSubscribed>d__);
			return <IsUserSubscribed>d__.<>t__builder.Task;
		}

		public static Task<Result<string>> StartLoginAttemptAsync()
		{
			LckCore.<StartLoginAttemptAsync>d__9 <StartLoginAttemptAsync>d__;
			<StartLoginAttemptAsync>d__.<>t__builder = AsyncTaskMethodBuilder<Result<string>>.Create();
			<StartLoginAttemptAsync>d__.<>1__state = -1;
			<StartLoginAttemptAsync>d__.<>t__builder.Start<LckCore.<StartLoginAttemptAsync>d__9>(ref <StartLoginAttemptAsync>d__);
			return <StartLoginAttemptAsync>d__.<>t__builder.Task;
		}

		public static Task<Result<bool>> CheckLoginCompletedAsync()
		{
			LckCore.<CheckLoginCompletedAsync>d__10 <CheckLoginCompletedAsync>d__;
			<CheckLoginCompletedAsync>d__.<>t__builder = AsyncTaskMethodBuilder<Result<bool>>.Create();
			<CheckLoginCompletedAsync>d__.<>1__state = -1;
			<CheckLoginCompletedAsync>d__.<>t__builder.Start<LckCore.<CheckLoginCompletedAsync>d__10>(ref <CheckLoginCompletedAsync>d__);
			return <CheckLoginCompletedAsync>d__.<>t__builder.Task;
		}

		public static Task<Result<float>> GetRemainingBackoffTimeSeconds()
		{
			LckCore.<GetRemainingBackoffTimeSeconds>d__11 <GetRemainingBackoffTimeSeconds>d__;
			<GetRemainingBackoffTimeSeconds>d__.<>t__builder = AsyncTaskMethodBuilder<Result<float>>.Create();
			<GetRemainingBackoffTimeSeconds>d__.<>1__state = -1;
			<GetRemainingBackoffTimeSeconds>d__.<>t__builder.Start<LckCore.<GetRemainingBackoffTimeSeconds>d__11>(ref <GetRemainingBackoffTimeSeconds>d__);
			return <GetRemainingBackoffTimeSeconds>d__.<>t__builder.Task;
		}

		public static void Log(LogType level, string message, string memberName = "", string filePath = "", int lineNumber = 0)
		{
			IntPtr intPtr = InteropUtilities.StringToUTF8Pointer(message);
			IntPtr intPtr2 = InteropUtilities.StringToUTF8Pointer(memberName);
			IntPtr intPtr3 = InteropUtilities.StringToUTF8Pointer(filePath);
			try
			{
				LckCoreNative.log(level, intPtr, intPtr2, intPtr3, lineNumber);
			}
			finally
			{
				InteropUtilities.Free(intPtr);
				InteropUtilities.Free(intPtr2);
				InteropUtilities.Free(intPtr3);
			}
		}

		public static void Dispose()
		{
			ReturnCode returnCode = LckCoreNative.dispose();
			if (returnCode != ReturnCode.Ok)
			{
				throw new InvalidOperationException(string.Format("Failed to dispose LckCore: {0}", returnCode));
			}
		}

		private static readonly object _loginLock = new object();

		private static ReturnCode _lastReturnCode;

		private static string _loginCode;
	}
}
