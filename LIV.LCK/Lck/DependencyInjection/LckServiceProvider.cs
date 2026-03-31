using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Liv.Lck.DependencyInjection
{
	public class LckServiceProvider : IDisposable
	{
		internal LckServiceProvider(Dictionary<Type, LckDiServiceRegistration> registrations)
		{
			this._registrations = registrations;
		}

		public T GetService<T>() where T : class
		{
			if (this._disposed)
			{
				throw new ObjectDisposedException("LckServiceProvider");
			}
			T result;
			try
			{
				result = (T)((object)this.ProvideService(typeof(T)));
			}
			catch (Exception ex)
			{
				LckLog.LogError("LCK Error: Failed to get service of type " + typeof(T).Name + ". Exception: " + ex.Message, "GetService", ".\\Packages\\tv.liv.lck\\Runtime\\Scripts\\Util\\DependencyInjection\\LckServiceProvider.cs", 29);
				result = default(T);
			}
			return result;
		}

		public object GetService(Type serviceType)
		{
			object result;
			try
			{
				result = this.ProvideService(serviceType);
			}
			catch (Exception ex)
			{
				LckLog.LogError("LCK Error: Failed to get service of type " + serviceType.Name + ". Exception: " + ex.Message, "GetService", ".\\Packages\\tv.liv.lck\\Runtime\\Scripts\\Util\\DependencyInjection\\LckServiceProvider.cs", 42);
				result = null;
			}
			return result;
		}

		private object ProvideService(Type serviceType)
		{
			LckDiServiceRegistration lckDiServiceRegistration;
			if (!this._registrations.TryGetValue(serviceType, out lckDiServiceRegistration))
			{
				LckLog.LogError("LCK Error: Service of type " + serviceType.Name + " has not been registered.", "ProvideService", ".\\Packages\\tv.liv.lck\\Runtime\\Scripts\\Util\\DependencyInjection\\LckServiceProvider.cs", 51);
				throw new InvalidOperationException("Service of type " + serviceType.Name + " has not been registered.");
			}
			object result;
			try
			{
				if (lckDiServiceRegistration.ForwardToServiceType != null)
				{
					result = this.ProvideService(lckDiServiceRegistration.ForwardToServiceType);
				}
				else if (lckDiServiceRegistration.Instance != null)
				{
					result = lckDiServiceRegistration.Instance;
				}
				else if (lckDiServiceRegistration.Factory != null)
				{
					object obj = lckDiServiceRegistration.Factory(this);
					if (lckDiServiceRegistration.Lifetime == LckDiServiceRegistration.ServiceLifetime.Singleton)
					{
						lckDiServiceRegistration.SetInstance(obj);
					}
					result = obj;
				}
				else
				{
					ConstructorInfo[] constructors = lckDiServiceRegistration.ImplementationType.GetConstructors(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
					if (constructors.Length < 1)
					{
						LckLog.LogError(string.Format("LCK Error: {0} has no public constructors.", lckDiServiceRegistration.ImplementationType), "ProvideService", ".\\Packages\\tv.liv.lck\\Runtime\\Scripts\\Util\\DependencyInjection\\LckServiceProvider.cs", 80);
						result = null;
					}
					else
					{
						result = this.CreateInstance(constructors, lckDiServiceRegistration);
					}
				}
			}
			catch (Exception ex)
			{
				LckLog.LogError("LCK Error: Failed to provide service " + serviceType.Name + ". Exception: " + ex.Message, "ProvideService", ".\\Packages\\tv.liv.lck\\Runtime\\Scripts\\Util\\DependencyInjection\\LckServiceProvider.cs", 88);
				throw;
			}
			return result;
		}

		private object CreateInstance(ConstructorInfo[] constructors, LckDiServiceRegistration registration)
		{
			ConstructorInfo constructorInfo = (from c in constructors
			orderby c.GetParameters().Length descending
			select c).First<ConstructorInfo>();
			ParameterInfo[] parameters = constructorInfo.GetParameters();
			List<object> list = new List<object>();
			object result;
			try
			{
				foreach (ParameterInfo parameterInfo in parameters)
				{
					object obj = this.ProvideService(parameterInfo.ParameterType);
					if (obj == null)
					{
						LckLog.LogError(string.Concat(new string[]
						{
							"LCK Error: Failed to resolve parameter '",
							parameterInfo.Name,
							"' of type '",
							parameterInfo.ParameterType.Name,
							"' for '",
							registration.ImplementationType.Name,
							"'."
						}), "CreateInstance", ".\\Packages\\tv.liv.lck\\Runtime\\Scripts\\Util\\DependencyInjection\\LckServiceProvider.cs", 107);
						return null;
					}
					list.Add(obj);
				}
				object obj2 = constructorInfo.Invoke(list.ToArray());
				if (registration.Lifetime == LckDiServiceRegistration.ServiceLifetime.Singleton)
				{
					registration.SetInstance(obj2);
				}
				LckLog.Log("Successfully instantiated " + registration.ImplementationType.Name + ".", "CreateInstance", ".\\Packages\\tv.liv.lck\\Runtime\\Scripts\\Util\\DependencyInjection\\LckServiceProvider.cs", 120);
				result = obj2;
			}
			catch (Exception ex)
			{
				string format = "LCK Error: {0} failed to instantiate. Exception: {1}";
				object implementationType = registration.ImplementationType;
				Exception innerException = ex.InnerException;
				LckLog.LogError(string.Format(format, implementationType, ((innerException != null) ? innerException.Message : null) ?? ex.Message), "CreateInstance", ".\\Packages\\tv.liv.lck\\Runtime\\Scripts\\Util\\DependencyInjection\\LckServiceProvider.cs", 125);
				result = null;
			}
			return result;
		}

		public void Dispose()
		{
			if (this._disposed)
			{
				return;
			}
			LckLog.Log("Disposing LCK Service Provider and all disposable singleton services.", "Dispose", ".\\Packages\\tv.liv.lck\\Runtime\\Scripts\\Util\\DependencyInjection\\LckServiceProvider.cs", 134);
			foreach (LckDiServiceRegistration lckDiServiceRegistration in this._registrations.Values)
			{
				if (lckDiServiceRegistration.Lifetime == LckDiServiceRegistration.ServiceLifetime.Singleton && lckDiServiceRegistration.Instance != null)
				{
					IDisposable disposable = lckDiServiceRegistration.Instance as IDisposable;
					if (disposable != null)
					{
						try
						{
							disposable.Dispose();
						}
						catch (Exception ex)
						{
							LckLog.LogError("LCK Error: Failed to dispose service of type " + lckDiServiceRegistration.Instance.GetType().Name + ". Exception: " + ex.Message, "Dispose", ".\\Packages\\tv.liv.lck\\Runtime\\Scripts\\Util\\DependencyInjection\\LckServiceProvider.cs", 148);
						}
					}
				}
			}
			this._disposed = true;
		}

		private readonly Dictionary<Type, LckDiServiceRegistration> _registrations;

		private bool _disposed;
	}
}
