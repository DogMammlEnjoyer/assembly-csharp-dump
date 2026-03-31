using System;
using System.Collections.Generic;

namespace Liv.Lck.DependencyInjection
{
	public class LckDiRegistry
	{
		public static LckDiRegistry Instance
		{
			get
			{
				LckDiRegistry result;
				if ((result = LckDiRegistry._instance) == null)
				{
					result = (LckDiRegistry._instance = new LckDiRegistry());
				}
				return result;
			}
		}

		public void AddTransient<TService, TImplementation>() where TService : class where TImplementation : TService
		{
			this._collection.AddTransient<TService, TImplementation>();
		}

		public void AddTransientFactory<TService>(Func<LckServiceProvider, TService> factory) where TService : class
		{
			this._collection.AddTransientFactory<TService>(factory);
		}

		public void AddSingleton<TService, TImplementation>() where TService : class where TImplementation : TService
		{
			this._collection.AddSingleton<TService, TImplementation>();
		}

		public void AddSingleton<TService>(TService instance) where TService : class
		{
			this._collection.AddSingleton<TService>(instance);
		}

		public void AddSingletonFactory<TService>(Func<LckServiceProvider, TService> factory) where TService : class
		{
			this._collection.AddSingletonFactory<TService>(factory);
		}

		public void AddSingletonForward<TService, TForwardTo>() where TService : class where TForwardTo : class, TService
		{
			this._collection.AddSingletonForward<TService, TForwardTo>();
		}

		public T GetService<T>() where T : class
		{
			T t;
			try
			{
				if (this._provider == null)
				{
					LckLog.Log("Service provider not built yet, building now.", "GetService", ".\\Packages\\tv.liv.lck\\Runtime\\Scripts\\Util\\DependencyInjection\\LckDiRegistry.cs", 51);
					this.Build();
				}
				LckServiceProvider provider = this._provider;
				T t2;
				if (provider == null)
				{
					t = default(T);
					t2 = t;
				}
				else
				{
					t2 = provider.GetService<T>();
				}
				t = t2;
			}
			catch (Exception ex)
			{
				LckLog.LogError("LCK Error: GetService failed for type " + typeof(T).Name + ". Exception: " + ex.Message, "GetService", ".\\Packages\\tv.liv.lck\\Runtime\\Scripts\\Util\\DependencyInjection\\LckDiRegistry.cs", 59);
				t = default(T);
			}
			return t;
		}

		public bool HasService<T>() where T : class
		{
			bool result;
			try
			{
				LckServiceProvider provider = this._provider;
				result = (((provider != null) ? provider.GetService<T>() : default(T)) != null);
			}
			catch
			{
				result = false;
			}
			return result;
		}

		public LckMonoBehaviourDependencyInjector GetInjector()
		{
			return this._lckMonoBehaviourDependencyInjector;
		}

		public void Build()
		{
			try
			{
				this._provider = this._collection.Build();
				this._lckMonoBehaviourDependencyInjector = new LckMonoBehaviourDependencyInjector(this._provider);
				LckLog.Log("LCK DI provider built successfully.", "Build", ".\\Packages\\tv.liv.lck\\Runtime\\Scripts\\Util\\DependencyInjection\\LckDiRegistry.cs", 87);
			}
			catch (Exception ex)
			{
				LckLog.LogError("LCK Error: Failed to build the service provider. Exception: " + ex.Message, "Build", ".\\Packages\\tv.liv.lck\\Runtime\\Scripts\\Util\\DependencyInjection\\LckDiRegistry.cs", 91);
			}
		}

		public Dictionary<Type, LckDiServiceRegistration> GetRegistrations()
		{
			return this._collection.GetRegistrations();
		}

		public void Reset()
		{
			try
			{
				LckServiceProvider provider = this._provider;
				if (provider != null)
				{
					provider.Dispose();
				}
				this._collection = new LckDiCollection();
				this._provider = null;
				this._lckMonoBehaviourDependencyInjector = null;
				LckLog.Log("LCK DI registry has been reset.", "Reset", ".\\Packages\\tv.liv.lck\\Runtime\\Scripts\\Util\\DependencyInjection\\LckDiRegistry.cs", 109);
			}
			catch (Exception ex)
			{
				LckLog.LogError("LCK Error: Failed to reset the DI registry. Exception: " + ex.Message, "Reset", ".\\Packages\\tv.liv.lck\\Runtime\\Scripts\\Util\\DependencyInjection\\LckDiRegistry.cs", 113);
			}
		}

		private static LckDiRegistry _instance;

		private LckDiCollection _collection = new LckDiCollection();

		private LckServiceProvider _provider;

		private LckMonoBehaviourDependencyInjector _lckMonoBehaviourDependencyInjector;
	}
}
