using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Modio.Extensions;

namespace Modio.Metrics
{
	public class MetricsManager
	{
		private string Secret
		{
			get
			{
				if (this._settings != null)
				{
					return this._settings.Secret;
				}
				return string.Empty;
			}
		}

		public MetricsManager()
		{
			ModioSettings modioSettings = ModioServices.Resolve<ModioSettings>();
			this._settings = modioSettings.GetPlatformSettings<MetricsSettings>();
			if (string.IsNullOrEmpty(this.Secret))
			{
				ModioLog error = ModioLog.Error;
				if (error == null)
				{
					return;
				}
				error.Log("Metrics Secret has not been set.");
			}
		}

		public Task<ValueTuple<string, Error>> StartSession()
		{
			MetricsManager.<StartSession>d__6 <StartSession>d__;
			<StartSession>d__.<>t__builder = AsyncTaskMethodBuilder<ValueTuple<string, Error>>.Create();
			<StartSession>d__.<>4__this = this;
			<StartSession>d__.<>1__state = -1;
			<StartSession>d__.<>t__builder.Start<MetricsManager.<StartSession>d__6>(ref <StartSession>d__);
			return <StartSession>d__.<>t__builder.Task;
		}

		public Task<Error> StartSession(string id)
		{
			MetricsManager.<StartSession>d__7 <StartSession>d__;
			<StartSession>d__.<>t__builder = AsyncTaskMethodBuilder<Error>.Create();
			<StartSession>d__.<>4__this = this;
			<StartSession>d__.id = id;
			<StartSession>d__.<>1__state = -1;
			<StartSession>d__.<>t__builder.Start<MetricsManager.<StartSession>d__7>(ref <StartSession>d__);
			return <StartSession>d__.<>t__builder.Task;
		}

		public Task<ValueTuple<string, Error>> StartSession(long[] mods)
		{
			MetricsManager.<StartSession>d__8 <StartSession>d__;
			<StartSession>d__.<>t__builder = AsyncTaskMethodBuilder<ValueTuple<string, Error>>.Create();
			<StartSession>d__.<>4__this = this;
			<StartSession>d__.mods = mods;
			<StartSession>d__.<>1__state = -1;
			<StartSession>d__.<>t__builder.Start<MetricsManager.<StartSession>d__8>(ref <StartSession>d__);
			return <StartSession>d__.<>t__builder.Task;
		}

		public Task<Error> StartSession(string id, long[] mods)
		{
			MetricsManager.<StartSession>d__9 <StartSession>d__;
			<StartSession>d__.<>t__builder = AsyncTaskMethodBuilder<Error>.Create();
			<StartSession>d__.<>4__this = this;
			<StartSession>d__.id = id;
			<StartSession>d__.mods = mods;
			<StartSession>d__.<>1__state = -1;
			<StartSession>d__.<>t__builder.Start<MetricsManager.<StartSession>d__9>(ref <StartSession>d__);
			return <StartSession>d__.<>t__builder.Task;
		}

		private Task Heartbeat(string id)
		{
			MetricsManager.<Heartbeat>d__10 <Heartbeat>d__;
			<Heartbeat>d__.<>t__builder = AsyncTaskMethodBuilder.Create();
			<Heartbeat>d__.<>4__this = this;
			<Heartbeat>d__.id = id;
			<Heartbeat>d__.<>1__state = -1;
			<Heartbeat>d__.<>t__builder.Start<MetricsManager.<Heartbeat>d__10>(ref <Heartbeat>d__);
			return <Heartbeat>d__.<>t__builder.Task;
		}

		public Task<Error> EndSession(string id)
		{
			MetricsManager.<EndSession>d__11 <EndSession>d__;
			<EndSession>d__.<>t__builder = AsyncTaskMethodBuilder<Error>.Create();
			<EndSession>d__.<>4__this = this;
			<EndSession>d__.id = id;
			<EndSession>d__.<>1__state = -1;
			<EndSession>d__.<>t__builder.Start<MetricsManager.<EndSession>d__11>(ref <EndSession>d__);
			return <EndSession>d__.<>t__builder.Task;
		}

		public void EndAllSessions()
		{
			foreach (MetricsSession metricsSession in from session in this._sessions.Values
			where session.Active
			select session)
			{
				this.EndSession(metricsSession.SessionId).ForgetTaskSafely();
			}
		}

		private const int HEARTBEAT_INTERVAL = 150;

		private readonly Dictionary<string, MetricsSession> _sessions = new Dictionary<string, MetricsSession>();

		private readonly MetricsSettings _settings;
	}
}
