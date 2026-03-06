using System;

namespace Liv.Lck.DependencyInjection
{
	public class LckDiServiceRegistration
	{
		public Type ServiceType { get; }

		public LckDiServiceRegistration.ServiceLifetime Lifetime { get; }

		public Type ImplementationType { get; }

		public object Instance { get; private set; }

		public Func<LckServiceProvider, object> Factory { get; }

		public Type ForwardToServiceType { get; }

		public LckDiServiceRegistration(Type serviceType, LckDiServiceRegistration.ServiceLifetime lifetime, Type implementationType)
		{
			this.ServiceType = serviceType;
			this.Lifetime = lifetime;
			this.ImplementationType = implementationType;
		}

		public LckDiServiceRegistration(Type serviceType, object instance)
		{
			this.ServiceType = serviceType;
			this.Lifetime = 1;
			this.Instance = instance;
		}

		public LckDiServiceRegistration(Type serviceType, LckDiServiceRegistration.ServiceLifetime lifetime, Func<LckServiceProvider, object> factory)
		{
			this.ServiceType = serviceType;
			this.Lifetime = lifetime;
			this.Factory = factory;
		}

		public LckDiServiceRegistration(Type serviceType, Type forwardToServiceType)
		{
			this.ServiceType = serviceType;
			this.Lifetime = 1;
			this.ForwardToServiceType = forwardToServiceType;
		}

		public void SetInstance(object instance)
		{
			if (this.Lifetime == LckDiServiceRegistration.ServiceLifetime.Singleton)
			{
				this.Instance = instance;
			}
		}

		public enum ServiceLifetime
		{
			Transient,
			Singleton
		}
	}
}
