using System;
using System.Collections.Generic;

namespace UnityEngine.Accessibility
{
	internal class ServiceManager
	{
		public ServiceManager()
		{
			this.m_Services = new Dictionary<Type, IService>();
			AccessibilityManager.screenReaderStatusChanged += this.ScreenReaderStatusChanged;
			this.UpdateServices(AssistiveSupport.isScreenReaderEnabled);
		}

		public T GetService<T>() where T : IService
		{
			Type typeFromHandle = typeof(T);
			IService service;
			this.m_Services.TryGetValue(typeFromHandle, out service);
			return (T)((object)service);
		}

		private void StartService<T>() where T : IService
		{
			T t = this.GetService<T>();
			bool flag = t == null;
			if (flag)
			{
				Type typeFromHandle = typeof(T);
				t = (T)((object)Activator.CreateInstance(typeFromHandle));
				t.Start();
				this.m_Services.Add(typeFromHandle, t);
			}
		}

		private void StopService<T>() where T : IService
		{
			T service = this.GetService<T>();
			bool flag = service != null;
			if (flag)
			{
				service.Stop();
				this.m_Services.Remove(typeof(T));
			}
		}

		private void UpdateServices(bool isScreenReaderEnabled)
		{
			if (isScreenReaderEnabled)
			{
				bool flag = !this.m_Services.ContainsKey(typeof(AccessibilityHierarchyService));
				if (flag)
				{
					AccessibilityHierarchyService accessibilityHierarchyService = new AccessibilityHierarchyService();
					accessibilityHierarchyService.Start();
					this.m_Services.Add(typeof(AccessibilityHierarchyService), accessibilityHierarchyService);
				}
			}
			else
			{
				this.StopService<AccessibilityHierarchyService>();
			}
		}

		protected void ScreenReaderStatusChanged(bool isScreenReaderEnabled)
		{
			this.UpdateServices(isScreenReaderEnabled);
		}

		private readonly IDictionary<Type, IService> m_Services;
	}
}
