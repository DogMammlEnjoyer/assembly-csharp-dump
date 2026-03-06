using System;
using System.Collections.Generic;

namespace Liv.Lck.DependencyInjection
{
	public class LckDiCollection
	{
		public void AddTransient<TService, TImplementation>() where TService : class where TImplementation : TService
		{
			try
			{
				this._registrations[typeof(TService)] = new LckDiServiceRegistration(typeof(TService), LckDiServiceRegistration.ServiceLifetime.Transient, typeof(TImplementation));
			}
			catch (Exception ex)
			{
				LckLog.LogError("LCK Error adding transient " + typeof(TService).Name + ": " + ex.Message);
			}
		}

		public void AddTransientFactory<TService>(Func<LckServiceProvider, TService> factory) where TService : class
		{
			try
			{
				this._registrations[typeof(TService)] = new LckDiServiceRegistration(typeof(TService), LckDiServiceRegistration.ServiceLifetime.Transient, (LckServiceProvider p) => factory(p));
			}
			catch (Exception ex)
			{
				LckLog.LogError("LCK Error adding transient factory " + typeof(TService).Name + ": " + ex.Message);
			}
		}

		public void AddSingleton<TService, TImplementation>() where TService : class where TImplementation : TService
		{
			try
			{
				this._registrations[typeof(TService)] = new LckDiServiceRegistration(typeof(TService), LckDiServiceRegistration.ServiceLifetime.Singleton, typeof(TImplementation));
			}
			catch (Exception ex)
			{
				LckLog.LogError("LCK Error adding singleton " + typeof(TService).Name + ": " + ex.Message);
			}
		}

		public void AddSingletonFactory<TService>(Func<LckServiceProvider, TService> factory) where TService : class
		{
			try
			{
				this._registrations[typeof(TService)] = new LckDiServiceRegistration(typeof(TService), LckDiServiceRegistration.ServiceLifetime.Singleton, (LckServiceProvider p) => factory(p));
			}
			catch (Exception ex)
			{
				LckLog.LogError("LCK Error adding singleton factory " + typeof(TService).Name + ": " + ex.Message);
			}
		}

		public void AddSingleton<TService>(TService instance) where TService : class
		{
			try
			{
				this._registrations[typeof(TService)] = new LckDiServiceRegistration(typeof(TService), instance);
			}
			catch (Exception ex)
			{
				LckLog.LogError("LCK Error adding singleton instance " + typeof(TService).Name + ": " + ex.Message);
			}
		}

		public void AddSingletonForward<TService, TForwardTo>() where TService : class where TForwardTo : class, TService
		{
			try
			{
				this._registrations[typeof(TService)] = new LckDiServiceRegistration(typeof(TService), typeof(TForwardTo));
			}
			catch (Exception ex)
			{
				LckLog.LogError("LCK Error adding singleton forward for " + typeof(TService).Name + ": " + ex.Message);
			}
		}

		public Dictionary<Type, LckDiServiceRegistration> GetRegistrations()
		{
			return this._registrations;
		}

		public LckDiServiceRegistration GetRegistration(Type serviceType)
		{
			LckDiServiceRegistration result;
			this._registrations.TryGetValue(serviceType, out result);
			return result;
		}

		public LckServiceProvider Build()
		{
			LckLog.Log("Building LCK Service Provider.");
			return new LckServiceProvider(new Dictionary<Type, LckDiServiceRegistration>(this._registrations));
		}

		private readonly Dictionary<Type, LckDiServiceRegistration> _registrations = new Dictionary<Type, LckDiServiceRegistration>();
	}
}
