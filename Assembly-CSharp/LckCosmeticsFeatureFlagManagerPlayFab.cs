using System;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Liv.Lck;
using UnityEngine.Scripting;

[Preserve]
public class LckCosmeticsFeatureFlagManagerPlayFab : ILckCosmeticsFeatureFlagManager
{
	[Preserve]
	public LckCosmeticsFeatureFlagManagerPlayFab()
	{
	}

	public Task<bool> IsEnabledAsync()
	{
		if (this._initializationTask != null)
		{
			return this._initializationTask;
		}
		object @lock = this._lock;
		Task<bool> task2;
		lock (@lock)
		{
			Task<bool> task;
			if ((task = this._initializationTask) == null)
			{
				task2 = (this._initializationTask = this.GetEnabledStateWithRetryAsync());
				task = task2;
			}
			task2 = task;
		}
		return task2;
	}

	private Task<bool> GetEnabledStateWithRetryAsync()
	{
		LckCosmeticsFeatureFlagManagerPlayFab.<GetEnabledStateWithRetryAsync>d__7 <GetEnabledStateWithRetryAsync>d__;
		<GetEnabledStateWithRetryAsync>d__.<>t__builder = AsyncTaskMethodBuilder<bool>.Create();
		<GetEnabledStateWithRetryAsync>d__.<>1__state = -1;
		<GetEnabledStateWithRetryAsync>d__.<>t__builder.Start<LckCosmeticsFeatureFlagManagerPlayFab.<GetEnabledStateWithRetryAsync>d__7>(ref <GetEnabledStateWithRetryAsync>d__);
		return <GetEnabledStateWithRetryAsync>d__.<>t__builder.Task;
	}

	private const string TitleDataKey = "EnableLckCosmetics";

	private const int MaxRetries = 2;

	private const int RetryDelayMilliseconds = 5000;

	private Task<bool> _initializationTask;

	private readonly object _lock = new object();
}
