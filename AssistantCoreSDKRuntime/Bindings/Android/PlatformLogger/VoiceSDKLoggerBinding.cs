using System;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Scripting;

namespace Oculus.Voice.Core.Bindings.Android.PlatformLogger
{
	public class VoiceSDKLoggerBinding : BaseServiceBinding
	{
		[Preserve]
		public VoiceSDKLoggerBinding(AndroidJavaObject loggerInstance) : base(loggerInstance)
		{
			this._scheduler = TaskScheduler.FromCurrentSynchronizationContext();
		}

		public void Connect()
		{
			this.Call<bool>("connect", Array.Empty<object>());
		}

		public void LogInteractionStart(string requestId, string startTime)
		{
			this.Call("logInteractionStart", new object[]
			{
				requestId,
				startTime
			});
		}

		public void LogInteractionEndSuccess(string endTime)
		{
			this.Call("logInteractionEndSuccess", new object[]
			{
				endTime
			});
		}

		public void LogInteractionEndFailure(string endTime, string errorMessage)
		{
			this.Call("logInteractionEndFailure", new object[]
			{
				endTime,
				errorMessage
			});
		}

		public void LogInteractionPoint(string interactionPoint, string time)
		{
			this.Call("logInteractionPoint", new object[]
			{
				interactionPoint,
				time
			});
		}

		public void LogAnnotation(string annotationKey, string annotationValue)
		{
			this.Call("logAnnotation", new object[]
			{
				annotationKey,
				annotationValue
			});
		}

		private Task Call(string methodName, params object[] parameters)
		{
			Task task = new Task(delegate()
			{
				this.binding.Call(methodName, parameters);
			});
			task.Start(this._scheduler);
			return task;
		}

		private Task<TReturnType> Call<TReturnType>(string methodName, params object[] parameters)
		{
			Task<TReturnType> task = new Task<TReturnType>(() => this.binding.Call<TReturnType>(methodName, parameters));
			task.Start(this._scheduler);
			return task;
		}

		private readonly TaskScheduler _scheduler;
	}
}
