using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace System.ComponentModel.Design
{
	/// <summary>Provides a simple implementation of the <see cref="T:System.ComponentModel.Design.IServiceContainer" /> interface. This class cannot be inherited.</summary>
	public class ServiceContainer : IServiceContainer, IServiceProvider, IDisposable
	{
		/// <summary>Initializes a new instance of the <see cref="T:System.ComponentModel.Design.ServiceContainer" /> class.</summary>
		public ServiceContainer()
		{
		}

		/// <summary>Initializes a new instance of the <see cref="T:System.ComponentModel.Design.ServiceContainer" /> class using the specified parent service provider.</summary>
		/// <param name="parentProvider">A parent service provider.</param>
		public ServiceContainer(IServiceProvider parentProvider)
		{
			this._parentProvider = parentProvider;
		}

		private IServiceContainer Container
		{
			get
			{
				IServiceContainer result = null;
				if (this._parentProvider != null)
				{
					result = (IServiceContainer)this._parentProvider.GetService(typeof(IServiceContainer));
				}
				return result;
			}
		}

		/// <summary>Gets the default services implemented directly by <see cref="T:System.ComponentModel.Design.ServiceContainer" />.</summary>
		/// <returns>The default services.</returns>
		protected virtual Type[] DefaultServices
		{
			get
			{
				return ServiceContainer.s_defaultServices;
			}
		}

		private ServiceContainer.ServiceCollection<object> Services
		{
			get
			{
				ServiceContainer.ServiceCollection<object> result;
				if ((result = this._services) == null)
				{
					result = (this._services = new ServiceContainer.ServiceCollection<object>());
				}
				return result;
			}
		}

		/// <summary>Adds the specified service to the service container.</summary>
		/// <param name="serviceType">The type of service to add.</param>
		/// <param name="serviceInstance">An instance of the service to add. This object must implement or inherit from the type indicated by the <paramref name="serviceType" /> parameter.</param>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="serviceType" /> or <paramref name="serviceInstance" /> is <see langword="null" />.</exception>
		/// <exception cref="T:System.ArgumentException">A service of type <paramref name="serviceType" /> already exists in the container.</exception>
		public void AddService(Type serviceType, object serviceInstance)
		{
			this.AddService(serviceType, serviceInstance, false);
		}

		/// <summary>Adds the specified service to the service container.</summary>
		/// <param name="serviceType">The type of service to add.</param>
		/// <param name="serviceInstance">An instance of the service type to add. This object must implement or inherit from the type indicated by the <paramref name="serviceType" /> parameter.</param>
		/// <param name="promote">
		///   <see langword="true" /> if this service should be added to any parent service containers; otherwise, <see langword="false" />.</param>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="serviceType" /> or <paramref name="serviceInstance" /> is <see langword="null" />.</exception>
		/// <exception cref="T:System.ArgumentException">A service of type <paramref name="serviceType" /> already exists in the container.</exception>
		public virtual void AddService(Type serviceType, object serviceInstance, bool promote)
		{
			if (promote)
			{
				IServiceContainer container = this.Container;
				if (container != null)
				{
					container.AddService(serviceType, serviceInstance, promote);
					return;
				}
			}
			if (serviceType == null)
			{
				throw new ArgumentNullException("serviceType");
			}
			if (serviceInstance == null)
			{
				throw new ArgumentNullException("serviceInstance");
			}
			if (!(serviceInstance is ServiceCreatorCallback) && !serviceInstance.GetType().IsCOMObject && !serviceType.IsInstanceOfType(serviceInstance))
			{
				throw new ArgumentException(SR.Format("The service instance must derive from or implement {0}.", serviceType.FullName));
			}
			if (this.Services.ContainsKey(serviceType))
			{
				throw new ArgumentException(SR.Format("The service {0} already exists in the service container.", serviceType.FullName), "serviceType");
			}
			this.Services[serviceType] = serviceInstance;
		}

		/// <summary>Adds the specified service to the service container.</summary>
		/// <param name="serviceType">The type of service to add.</param>
		/// <param name="callback">A callback object that can create the service. This allows a service to be declared as available, but delays creation of the object until the service is requested.</param>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="serviceType" /> or <paramref name="callback" /> is <see langword="null" />.</exception>
		/// <exception cref="T:System.ArgumentException">A service of type <paramref name="serviceType" /> already exists in the container.</exception>
		public void AddService(Type serviceType, ServiceCreatorCallback callback)
		{
			this.AddService(serviceType, callback, false);
		}

		/// <summary>Adds the specified service to the service container.</summary>
		/// <param name="serviceType">The type of service to add.</param>
		/// <param name="callback">A callback object that can create the service. This allows a service to be declared as available, but delays creation of the object until the service is requested.</param>
		/// <param name="promote">
		///   <see langword="true" /> if this service should be added to any parent service containers; otherwise, <see langword="false" />.</param>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="serviceType" /> or <paramref name="callback" /> is <see langword="null" />.</exception>
		/// <exception cref="T:System.ArgumentException">A service of type <paramref name="serviceType" /> already exists in the container.</exception>
		public virtual void AddService(Type serviceType, ServiceCreatorCallback callback, bool promote)
		{
			if (promote)
			{
				IServiceContainer container = this.Container;
				if (container != null)
				{
					container.AddService(serviceType, callback, promote);
					return;
				}
			}
			if (serviceType == null)
			{
				throw new ArgumentNullException("serviceType");
			}
			if (callback == null)
			{
				throw new ArgumentNullException("callback");
			}
			if (this.Services.ContainsKey(serviceType))
			{
				throw new ArgumentException(SR.Format("The service {0} already exists in the service container.", serviceType.FullName), "serviceType");
			}
			this.Services[serviceType] = callback;
		}

		/// <summary>Disposes this service container.</summary>
		public void Dispose()
		{
			this.Dispose(true);
		}

		/// <summary>Disposes this service container.</summary>
		/// <param name="disposing">
		///   <see langword="true" /> if the <see cref="T:System.ComponentModel.Design.ServiceContainer" /> is in the process of being disposed of; otherwise, <see langword="false" />.</param>
		protected virtual void Dispose(bool disposing)
		{
			if (disposing)
			{
				ServiceContainer.ServiceCollection<object> services = this._services;
				this._services = null;
				if (services != null)
				{
					foreach (object obj in services.Values)
					{
						if (obj is IDisposable)
						{
							((IDisposable)obj).Dispose();
						}
					}
				}
			}
		}

		/// <summary>Gets the requested service.</summary>
		/// <param name="serviceType">The type of service to retrieve.</param>
		/// <returns>An instance of the service if it could be found, or <see langword="null" /> if it could not be found.</returns>
		public virtual object GetService(Type serviceType)
		{
			object obj = null;
			Type[] defaultServices = this.DefaultServices;
			for (int i = 0; i < defaultServices.Length; i++)
			{
				if (serviceType.IsEquivalentTo(defaultServices[i]))
				{
					obj = this;
					break;
				}
			}
			if (obj == null)
			{
				this.Services.TryGetValue(serviceType, out obj);
			}
			if (obj is ServiceCreatorCallback)
			{
				obj = ((ServiceCreatorCallback)obj)(this, serviceType);
				if (obj != null && !obj.GetType().IsCOMObject && !serviceType.IsInstanceOfType(obj))
				{
					obj = null;
				}
				this.Services[serviceType] = obj;
			}
			if (obj == null && this._parentProvider != null)
			{
				obj = this._parentProvider.GetService(serviceType);
			}
			return obj;
		}

		/// <summary>Removes the specified service type from the service container.</summary>
		/// <param name="serviceType">The type of service to remove.</param>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="serviceType" /> is <see langword="null" />.</exception>
		public void RemoveService(Type serviceType)
		{
			this.RemoveService(serviceType, false);
		}

		/// <summary>Removes the specified service type from the service container.</summary>
		/// <param name="serviceType">The type of service to remove.</param>
		/// <param name="promote">
		///   <see langword="true" /> if this service should be removed from any parent service containers; otherwise, <see langword="false" />.</param>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="serviceType" /> is <see langword="null" />.</exception>
		public virtual void RemoveService(Type serviceType, bool promote)
		{
			if (promote)
			{
				IServiceContainer container = this.Container;
				if (container != null)
				{
					container.RemoveService(serviceType, promote);
					return;
				}
			}
			if (serviceType == null)
			{
				throw new ArgumentNullException("serviceType");
			}
			this.Services.Remove(serviceType);
		}

		private ServiceContainer.ServiceCollection<object> _services;

		private IServiceProvider _parentProvider;

		private static Type[] s_defaultServices = new Type[]
		{
			typeof(IServiceContainer),
			typeof(ServiceContainer)
		};

		private static TraceSwitch s_TRACESERVICE = new TraceSwitch("TRACESERVICE", "ServiceProvider: Trace service provider requests.");

		private sealed class ServiceCollection<T> : Dictionary<Type, T>
		{
			public ServiceCollection() : base(ServiceContainer.ServiceCollection<T>.s_serviceTypeComparer)
			{
			}

			private static ServiceContainer.ServiceCollection<T>.EmbeddedTypeAwareTypeComparer s_serviceTypeComparer = new ServiceContainer.ServiceCollection<T>.EmbeddedTypeAwareTypeComparer();

			private sealed class EmbeddedTypeAwareTypeComparer : IEqualityComparer<Type>
			{
				public bool Equals(Type x, Type y)
				{
					return x.IsEquivalentTo(y);
				}

				public int GetHashCode(Type obj)
				{
					return obj.FullName.GetHashCode();
				}
			}
		}
	}
}
