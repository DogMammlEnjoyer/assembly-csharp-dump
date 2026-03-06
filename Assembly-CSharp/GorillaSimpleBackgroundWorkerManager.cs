using System;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public class GorillaSimpleBackgroundWorkerManager : MonoBehaviour
{
	protected void Awake()
	{
		if (GorillaSimpleBackgroundWorkerManager.hasInstance && GorillaSimpleBackgroundWorkerManager._instance != this)
		{
			Object.Destroy(this);
			return;
		}
		GorillaSimpleBackgroundWorkerManager.SetInstance(this);
	}

	private static void SetInstance(GorillaSimpleBackgroundWorkerManager manager)
	{
		GorillaSimpleBackgroundWorkerManager._instance = manager;
		GorillaSimpleBackgroundWorkerManager.hasInstance = true;
		if (Application.isPlaying)
		{
			Object.DontDestroyOnLoad(manager);
		}
	}

	public static void CreateManager()
	{
		GameObject gameObject = new GameObject("GorillaSimpleBackgroundWorkerManager");
		GorillaSimpleBackgroundWorkerManager instance = gameObject.AddComponent<GorillaSimpleBackgroundWorkerManager>();
		Object.DontDestroyOnLoad(gameObject);
		GorillaSimpleBackgroundWorkerManager.SetInstance(instance);
	}

	public static long DoWork(long ticksOfWork)
	{
		if (!GorillaSimpleBackgroundWorkerManager.hasInstance)
		{
			GorillaSimpleBackgroundWorkerManager.CreateManager();
		}
		return GorillaSimpleBackgroundWorkerManager._instance._DoWork(ticksOfWork);
	}

	public long _DoWork(long ticksOfWork)
	{
		this.stopwatch.Restart();
		if (ticksOfWork < GorillaSimpleBackgroundWorkerManager.MINIMUM_TICKS_OF_WORK)
		{
			ticksOfWork = GorillaSimpleBackgroundWorkerManager.MINIMUM_TICKS_OF_WORK;
		}
		while (this.stopwatch.ElapsedTicks < ticksOfWork && this.workerSignups.Count > 0)
		{
			IGorillaSimpleBackgroundWorker gorillaSimpleBackgroundWorker = this.workerSignups.Dequeue();
			if (gorillaSimpleBackgroundWorker != null)
			{
				gorillaSimpleBackgroundWorker.SimpleWork();
			}
		}
		return this.stopwatch.ElapsedTicks;
	}

	public static void WorkerSignup(IGorillaSimpleBackgroundWorker worker)
	{
		if (!GorillaSimpleBackgroundWorkerManager.hasInstance)
		{
			GorillaSimpleBackgroundWorkerManager.CreateManager();
		}
		GorillaSimpleBackgroundWorkerManager._instance.workerSignups.Enqueue(worker);
	}

	private static GorillaSimpleBackgroundWorkerManager _instance;

	private static bool hasInstance = false;

	private static long MINIMUM_TICKS_OF_WORK = 10000L;

	public Queue<IGorillaSimpleBackgroundWorker> workerSignups = new Queue<IGorillaSimpleBackgroundWorker>();

	private Stopwatch stopwatch = new Stopwatch();
}
