using System;
using System.Collections.Generic;

namespace UnityEngine.XR.Management
{
	public abstract class XRLoaderHelper : XRLoader
	{
		public override T GetLoadedSubsystem<T>()
		{
			Type typeFromHandle = typeof(T);
			ISubsystem subsystem;
			this.m_SubsystemInstanceMap.TryGetValue(typeFromHandle, out subsystem);
			return subsystem as T;
		}

		protected void StartSubsystem<T>() where T : class, ISubsystem
		{
			T loadedSubsystem = this.GetLoadedSubsystem<T>();
			if (loadedSubsystem != null)
			{
				loadedSubsystem.Start();
			}
		}

		protected void StopSubsystem<T>() where T : class, ISubsystem
		{
			T loadedSubsystem = this.GetLoadedSubsystem<T>();
			if (loadedSubsystem != null)
			{
				loadedSubsystem.Stop();
			}
		}

		protected void DestroySubsystem<T>() where T : class, ISubsystem
		{
			T loadedSubsystem = this.GetLoadedSubsystem<T>();
			if (loadedSubsystem != null)
			{
				Type typeFromHandle = typeof(T);
				if (this.m_SubsystemInstanceMap.ContainsKey(typeFromHandle))
				{
					this.m_SubsystemInstanceMap.Remove(typeFromHandle);
				}
				loadedSubsystem.Destroy();
			}
		}

		protected void CreateSubsystem<TDescriptor, TSubsystem>(List<TDescriptor> descriptors, string id) where TDescriptor : ISubsystemDescriptor where TSubsystem : ISubsystem
		{
			if (descriptors == null)
			{
				throw new ArgumentNullException("descriptors");
			}
			SubsystemManager.GetSubsystemDescriptors<TDescriptor>(descriptors);
			if (descriptors.Count > 0)
			{
				foreach (TDescriptor tdescriptor in descriptors)
				{
					ISubsystem subsystem = null;
					if (string.Compare(tdescriptor.id, id, true) == 0)
					{
						subsystem = tdescriptor.Create();
					}
					if (subsystem != null)
					{
						this.m_SubsystemInstanceMap[typeof(TSubsystem)] = subsystem;
						break;
					}
				}
			}
		}

		[Obsolete("This method is obsolete. Please use the geenric CreateSubsystem method.", false)]
		protected void CreateIntegratedSubsystem<TDescriptor, TSubsystem>(List<TDescriptor> descriptors, string id) where TDescriptor : IntegratedSubsystemDescriptor where TSubsystem : IntegratedSubsystem
		{
			this.CreateSubsystem<TDescriptor, TSubsystem>(descriptors, id);
		}

		[Obsolete("This method is obsolete. Please use the generic CreateSubsystem method.", false)]
		protected void CreateStandaloneSubsystem<TDescriptor, TSubsystem>(List<TDescriptor> descriptors, string id) where TDescriptor : SubsystemDescriptor where TSubsystem : Subsystem
		{
			this.CreateSubsystem<TDescriptor, TSubsystem>(descriptors, id);
		}

		public override bool Deinitialize()
		{
			this.m_SubsystemInstanceMap.Clear();
			return base.Deinitialize();
		}

		protected Dictionary<Type, ISubsystem> m_SubsystemInstanceMap = new Dictionary<Type, ISubsystem>();
	}
}
