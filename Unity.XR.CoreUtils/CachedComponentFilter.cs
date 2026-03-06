using System;
using System.Collections.Generic;
using UnityEngine;

namespace Unity.XR.CoreUtils
{
	public class CachedComponentFilter<TFilterType, TRootType> : IDisposable where TFilterType : class where TRootType : Component
	{
		public CachedComponentFilter(TRootType componentRoot, CachedSearchType cachedSearchType = CachedSearchType.Children | CachedSearchType.Self, bool includeDisabled = true)
		{
			this.m_MasterComponentStorage = CollectionPool<List<TFilterType>, TFilterType>.GetCollection();
			CachedComponentFilter<TFilterType, TRootType>.k_TempComponentList.Clear();
			CachedComponentFilter<TFilterType, TRootType>.k_TempHostComponentList.Clear();
			if ((cachedSearchType & CachedSearchType.Self) == CachedSearchType.Self)
			{
				componentRoot.GetComponents<TFilterType>(CachedComponentFilter<TFilterType, TRootType>.k_TempComponentList);
				componentRoot.GetComponents<IComponentHost<TFilterType>>(CachedComponentFilter<TFilterType, TRootType>.k_TempHostComponentList);
				this.FilteredCopyToMaster(includeDisabled);
			}
			if ((cachedSearchType & CachedSearchType.Parents) == CachedSearchType.Parents)
			{
				Transform parent = componentRoot.transform.parent;
				while (parent != null && !(parent.GetComponent<TRootType>() != null))
				{
					parent.GetComponents<TFilterType>(CachedComponentFilter<TFilterType, TRootType>.k_TempComponentList);
					parent.GetComponents<IComponentHost<TFilterType>>(CachedComponentFilter<TFilterType, TRootType>.k_TempHostComponentList);
					this.FilteredCopyToMaster(includeDisabled);
					parent = parent.transform.parent;
				}
			}
			if ((cachedSearchType & CachedSearchType.Children) == CachedSearchType.Children)
			{
				foreach (object obj in componentRoot.transform)
				{
					Transform transform = (Transform)obj;
					transform.GetComponentsInChildren<TFilterType>(CachedComponentFilter<TFilterType, TRootType>.k_TempComponentList);
					transform.GetComponentsInChildren<IComponentHost<TFilterType>>(CachedComponentFilter<TFilterType, TRootType>.k_TempHostComponentList);
					this.FilteredCopyToMaster(includeDisabled, componentRoot);
				}
			}
		}

		public CachedComponentFilter(TFilterType[] componentList, bool includeDisabled = true)
		{
			if (componentList == null)
			{
				return;
			}
			this.m_MasterComponentStorage = CollectionPool<List<TFilterType>, TFilterType>.GetCollection();
			CachedComponentFilter<TFilterType, TRootType>.k_TempComponentList.Clear();
			CachedComponentFilter<TFilterType, TRootType>.k_TempComponentList.AddRange(componentList);
			this.FilteredCopyToMaster(includeDisabled);
		}

		public void StoreMatchingComponents<TChildType>(List<TChildType> outputList) where TChildType : class, TFilterType
		{
			foreach (TFilterType tfilterType in this.m_MasterComponentStorage)
			{
				TChildType tchildType = tfilterType as TChildType;
				if (tchildType != null)
				{
					outputList.Add(tchildType);
				}
			}
		}

		public TChildType[] GetMatchingComponents<TChildType>() where TChildType : class, TFilterType
		{
			int num = 0;
			using (List<TFilterType>.Enumerator enumerator = this.m_MasterComponentStorage.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					if (enumerator.Current is TChildType)
					{
						num++;
					}
				}
			}
			TChildType[] array = new TChildType[num];
			num = 0;
			foreach (TFilterType tfilterType in this.m_MasterComponentStorage)
			{
				TChildType tchildType = tfilterType as TChildType;
				if (tchildType != null)
				{
					array[num] = tchildType;
					num++;
				}
			}
			return array;
		}

		private void FilteredCopyToMaster(bool includeDisabled)
		{
			if (includeDisabled)
			{
				this.m_MasterComponentStorage.AddRange(CachedComponentFilter<TFilterType, TRootType>.k_TempComponentList);
				using (List<IComponentHost<TFilterType>>.Enumerator enumerator = CachedComponentFilter<TFilterType, TRootType>.k_TempHostComponentList.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						IComponentHost<TFilterType> componentHost = enumerator.Current;
						this.m_MasterComponentStorage.AddRange(componentHost.HostedComponents);
					}
					return;
				}
			}
			foreach (TFilterType tfilterType in CachedComponentFilter<TFilterType, TRootType>.k_TempComponentList)
			{
				Behaviour behaviour = tfilterType as Behaviour;
				if (!(behaviour != null) || behaviour.enabled)
				{
					this.m_MasterComponentStorage.Add(tfilterType);
				}
			}
			foreach (IComponentHost<TFilterType> componentHost2 in CachedComponentFilter<TFilterType, TRootType>.k_TempHostComponentList)
			{
				Behaviour behaviour2 = componentHost2 as Behaviour;
				if (!(behaviour2 != null) || behaviour2.enabled)
				{
					this.m_MasterComponentStorage.AddRange(componentHost2.HostedComponents);
				}
			}
		}

		private void FilteredCopyToMaster(bool includeDisabled, TRootType requiredRoot)
		{
			if (includeDisabled)
			{
				foreach (TFilterType tfilterType in CachedComponentFilter<TFilterType, TRootType>.k_TempComponentList)
				{
					Component component = tfilterType as Component;
					if (!(component.transform == requiredRoot) && !(component.GetComponentInParent<TRootType>() != requiredRoot))
					{
						this.m_MasterComponentStorage.Add(tfilterType);
					}
				}
				using (List<IComponentHost<TFilterType>>.Enumerator enumerator2 = CachedComponentFilter<TFilterType, TRootType>.k_TempHostComponentList.GetEnumerator())
				{
					while (enumerator2.MoveNext())
					{
						IComponentHost<TFilterType> componentHost = enumerator2.Current;
						Component component2 = componentHost as Component;
						if (!(component2.transform == requiredRoot) && !(component2.GetComponentInParent<TRootType>() != requiredRoot))
						{
							this.m_MasterComponentStorage.AddRange(componentHost.HostedComponents);
						}
					}
					return;
				}
			}
			foreach (TFilterType tfilterType2 in CachedComponentFilter<TFilterType, TRootType>.k_TempComponentList)
			{
				Behaviour behaviour = tfilterType2 as Behaviour;
				if (behaviour.enabled && !(behaviour.transform == requiredRoot) && !(behaviour.GetComponentInParent<TRootType>() != requiredRoot))
				{
					this.m_MasterComponentStorage.Add(tfilterType2);
				}
			}
			foreach (IComponentHost<TFilterType> componentHost2 in CachedComponentFilter<TFilterType, TRootType>.k_TempHostComponentList)
			{
				Behaviour behaviour2 = componentHost2 as Behaviour;
				if (behaviour2.enabled && !(behaviour2.transform == requiredRoot) && !(behaviour2.GetComponentInParent<TRootType>() != requiredRoot))
				{
					this.m_MasterComponentStorage.AddRange(componentHost2.HostedComponents);
				}
			}
		}

		protected virtual void Dispose(bool disposing)
		{
			if (this.m_DisposedValue)
			{
				return;
			}
			if (disposing && this.m_MasterComponentStorage != null)
			{
				CollectionPool<List<TFilterType>, TFilterType>.RecycleCollection(this.m_MasterComponentStorage);
			}
			this.m_DisposedValue = true;
		}

		public void Dispose()
		{
			this.Dispose(true);
		}

		private readonly List<TFilterType> m_MasterComponentStorage;

		private static readonly List<TFilterType> k_TempComponentList = new List<TFilterType>();

		private static readonly List<IComponentHost<TFilterType>> k_TempHostComponentList = new List<IComponentHost<TFilterType>>();

		private bool m_DisposedValue;
	}
}
