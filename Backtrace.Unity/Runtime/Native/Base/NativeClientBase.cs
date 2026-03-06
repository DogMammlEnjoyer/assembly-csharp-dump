using System;
using System.Threading;
using Backtrace.Unity.Model;
using Backtrace.Unity.Model.Breadcrumbs;

namespace Backtrace.Unity.Runtime.Native.Base
{
	internal abstract class NativeClientBase
	{
		internal NativeClientBase(BacktraceConfiguration configuration, BacktraceBreadcrumbs breadcrumbs)
		{
			this._configuration = configuration;
			this._breadcrumbs = breadcrumbs;
			this._shouldLogAnrsInBreadcrumbs = this.ShouldStoreAnrBreadcrumbs();
			this.AnrWatchdogTimeout = ((configuration.AnrWatchdogTimeout > 1000) ? configuration.AnrWatchdogTimeout : 5000);
		}

		public void Update(float time)
		{
			this.LastUpdateTime = time;
			if (this._shouldLogAnrsInBreadcrumbs && this.LogAnr && Monitor.TryEnter(this._lockObject))
			{
				try
				{
					if (this._shouldLogAnrsInBreadcrumbs && this.LogAnr)
					{
						this._breadcrumbs.AddBreadcrumbs("ANRException: Blocked thread detected.", BreadcrumbLevel.System, UnityEngineLogLevel.Warning, null);
						this.LogAnr = false;
					}
				}
				finally
				{
					Monitor.Exit(this._lockObject);
				}
			}
		}

		internal void OnAnrDetection()
		{
			this.LogAnr = this._shouldLogAnrsInBreadcrumbs;
		}

		public void PauseAnrThread(bool stopAnr)
		{
			this.PreventAnr = stopAnr;
		}

		public virtual void Disable()
		{
			if (this.AnrThread != null)
			{
				this.StopAnr = true;
			}
		}

		private bool ShouldStoreAnrBreadcrumbs()
		{
			return this._breadcrumbs != null && this._breadcrumbs.BreadcrumbsLevel.HasFlag(BacktraceBreadcrumbType.System) && this._breadcrumbs.UnityLogLevel.HasFlag(UnityEngineLogLevel.Warning);
		}

		internal const string AnrMessage = "ANRException: Blocked thread detected.";

		protected const string HangType = "Hang";

		protected const string CrashType = "Crash";

		protected const string ErrorTypeAttribute = "error.type";

		protected int AnrWatchdogTimeout = 5000;

		protected volatile bool LogAnr;

		protected internal volatile float LastUpdateTime;

		internal volatile bool PreventAnr;

		internal volatile bool StopAnr;

		internal Thread AnrThread;

		protected bool CaptureNativeCrashes;

		protected bool HandlerANR;

		protected readonly BacktraceConfiguration _configuration;

		protected readonly BacktraceBreadcrumbs _breadcrumbs;

		private readonly bool _shouldLogAnrsInBreadcrumbs;

		private object _lockObject = new object();
	}
}
