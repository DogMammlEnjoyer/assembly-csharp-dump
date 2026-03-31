using System;
using UnityEngine;

namespace Liv.Lck.DependencyInjection
{
	public class LckDiContainer : MonoBehaviour
	{
		public static LckDiContainer Instance
		{
			get
			{
				if (LckDiContainer._instance == null)
				{
					LckDiContainer._instance = Object.FindObjectOfType<LckDiContainer>();
					if (LckDiContainer._instance == null)
					{
						GameObject gameObject = new GameObject();
						Object.DontDestroyOnLoad(gameObject);
						LckDiContainer._instance = gameObject.AddComponent<LckDiContainer>();
						gameObject.name = "LCK Service Singleton";
						LckLog.Log("LCK: Created LCK Dependency Injection Service Singleton", "Instance", ".\\Packages\\tv.liv.lck\\Runtime\\Scripts\\Util\\DependencyInjection\\LckDiContainer.cs", 21);
					}
				}
				return LckDiContainer._instance;
			}
		}

		private void Awake()
		{
			if (LckDiContainer._instance != null && LckDiContainer._instance != this)
			{
				Object.Destroy(base.gameObject);
				return;
			}
			LckDiContainer._instance = this;
			Object.DontDestroyOnLoad(base.gameObject);
		}

		public void AddTransient<TService, TImplementation>() where TService : class where TImplementation : TService
		{
			LckDiRegistry.Instance.AddTransient<TService, TImplementation>();
		}

		public void AddSingleton<TService, TImplementation>() where TService : class where TImplementation : TService
		{
			LckDiRegistry.Instance.AddSingleton<TService, TImplementation>();
		}

		public void AddTransientFactory<TService>(Func<LckServiceProvider, TService> factory) where TService : class
		{
			LckDiRegistry.Instance.AddTransientFactory<TService>(factory);
		}

		public void AddSingletonFactory<TService>(Func<LckServiceProvider, TService> factory) where TService : class
		{
			LckDiRegistry.Instance.AddSingletonFactory<TService>(factory);
		}

		public void AddSingleton<TService>(TService instance) where TService : class
		{
			LckDiRegistry.Instance.AddSingleton<TService>(instance);
		}

		public void AddSingletonForward<TService, TForwardTo>() where TService : class where TForwardTo : class, TService
		{
			LckDiRegistry.Instance.AddSingletonForward<TService, TForwardTo>();
		}

		public T GetService<T>() where T : class
		{
			return LckDiRegistry.Instance.GetService<T>();
		}

		public bool HasService<T>() where T : class
		{
			return LckDiRegistry.Instance.HasService<T>();
		}

		public LckMonoBehaviourDependencyInjector GetInjector()
		{
			return LckDiRegistry.Instance.GetInjector();
		}

		private void OnDestroy()
		{
			if (LckDiContainer._instance == this)
			{
				LckDiRegistry.Instance.Reset();
				LckDiContainer._instance = null;
			}
		}

		public void Build()
		{
			LckDiRegistry.Instance.Build();
		}

		private static LckDiContainer _instance;
	}
}
