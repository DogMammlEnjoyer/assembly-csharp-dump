using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Localization.PropertyVariants.TrackedObjects;
using UnityEngine.Localization.PropertyVariants.TrackedProperties;
using UnityEngine.Localization.Settings;
using UnityEngine.Pool;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace UnityEngine.Localization.PropertyVariants
{
	[ExecuteAlways]
	[DisallowMultipleComponent]
	public class GameObjectLocalizer : MonoBehaviour
	{
		internal AsyncOperationHandle CurrentOperation { get; set; }

		private void OnEnable()
		{
			LocalizationSettings.SelectedLocaleChanged += this.SelectedLocaleChanged;
			this.RegisterChanges();
			if (this.m_CurrentLocale != null)
			{
				Locale selectedLocale = LocalizationSettings.SelectedLocale;
				if (this.m_CurrentLocale != selectedLocale)
				{
					this.SelectedLocaleChanged(selectedLocale);
				}
			}
		}

		private void OnDisable()
		{
			this.UnregisterChanges();
			AddressablesInterface.SafeRelease(this.CurrentOperation);
			this.CurrentOperation = default(AsyncOperationHandle);
			LocalizationSettings.SelectedLocaleChanged -= this.SelectedLocaleChanged;
		}

		private IEnumerator Start()
		{
			this.m_CurrentLocale = null;
			AsyncOperationHandle<Locale> localeOp = LocalizationSettings.SelectedLocaleAsync;
			if (!localeOp.IsDone)
			{
				yield return localeOp;
			}
			this.SelectedLocaleChanged(localeOp.Result);
			yield break;
		}

		private void SelectedLocaleChanged(Locale locale)
		{
			this.m_CurrentLocale = locale;
			if (locale == null)
			{
				return;
			}
			this.ApplyLocaleVariant(locale);
		}

		public List<TrackedObject> TrackedObjects
		{
			get
			{
				return this.m_TrackedObjects;
			}
		}

		public T GetTrackedObject<T>(Object target, bool create = true) where T : TrackedObject, new()
		{
			TrackedObject trackedObject = this.GetTrackedObject(target);
			if (trackedObject != null)
			{
				return (T)((object)trackedObject);
			}
			if (!create)
			{
				return default(T);
			}
			Component component = target as Component;
			if (component == null)
			{
				return default(T);
			}
			if (component.gameObject != base.gameObject)
			{
				throw new Exception("Tracked Objects must share the same GameObject as the GameObjectLocalizer. " + string.Format("The Component {0} is attached to the GameObject {1}", component, component.gameObject) + string.Format(" but the GameObjectLocalizer is attached to {0}.", base.gameObject));
			}
			T t = Activator.CreateInstance<T>();
			t.Target = target;
			T t2 = t;
			this.TrackedObjects.Add(t2);
			return t2;
		}

		public TrackedObject GetTrackedObject(Object target)
		{
			if (target == null)
			{
				throw new ArgumentNullException("target");
			}
			foreach (TrackedObject trackedObject in this.TrackedObjects)
			{
				if (((trackedObject != null) ? trackedObject.Target : null) == target)
				{
					return trackedObject;
				}
			}
			return null;
		}

		public AsyncOperationHandle ApplyLocaleVariant(Locale locale)
		{
			return this.ApplyLocaleVariant(locale, LocalizationSettings.ProjectLocale);
		}

		public AsyncOperationHandle ApplyLocaleVariant(Locale locale, Locale fallback)
		{
			if (locale == null)
			{
				throw new ArgumentNullException("locale");
			}
			if (this.CurrentOperation.IsValid())
			{
				if (!this.CurrentOperation.IsDone)
				{
					Debug.LogWarning("Attempting to Apply Variant when the previous operation has not yet completed.", this);
				}
				AddressablesInterface.Release(this.CurrentOperation);
				this.CurrentOperation = default(AsyncOperationHandle);
			}
			List<AsyncOperationHandle> list = CollectionPool<List<AsyncOperationHandle>, AsyncOperationHandle>.Get();
			foreach (TrackedObject trackedObject in this.TrackedObjects)
			{
				if (trackedObject != null)
				{
					AsyncOperationHandle item = trackedObject.ApplyLocale(locale, fallback);
					if (!item.IsDone)
					{
						list.Add(item);
					}
				}
			}
			if (list.Count == 1)
			{
				AddressablesInterface.Acquire(list[0]);
				this.CurrentOperation = list[0];
				CollectionPool<List<AsyncOperationHandle>, AsyncOperationHandle>.Release(list);
				return this.CurrentOperation;
			}
			if (list.Count > 1)
			{
				this.CurrentOperation = AddressablesInterface.CreateGroupOperation(list);
				return this.CurrentOperation;
			}
			CollectionPool<List<AsyncOperationHandle>, AsyncOperationHandle>.Release(list);
			return default(AsyncOperationHandle);
		}

		private void RegisterChanges()
		{
			if (this.m_LocalizedStringChanged == null)
			{
				this.m_LocalizedStringChanged = delegate(string _)
				{
					this.RequestUpdate();
				};
			}
			try
			{
				this.m_IgnoreChange = true;
				foreach (TrackedObject trackedObject in this.m_TrackedObjects)
				{
					foreach (ITrackedProperty trackedProperty in trackedObject.TrackedProperties)
					{
						LocalizedStringProperty localizedStringProperty = trackedProperty as LocalizedStringProperty;
						if (localizedStringProperty != null)
						{
							localizedStringProperty.LocalizedString.StringChanged += this.m_LocalizedStringChanged;
						}
					}
				}
			}
			finally
			{
				this.m_IgnoreChange = false;
			}
		}

		private void UnregisterChanges()
		{
			if (this.m_LocalizedStringChanged == null)
			{
				return;
			}
			foreach (TrackedObject trackedObject in this.m_TrackedObjects)
			{
				foreach (ITrackedProperty trackedProperty in trackedObject.TrackedProperties)
				{
					LocalizedStringProperty localizedStringProperty = trackedProperty as LocalizedStringProperty;
					if (localizedStringProperty != null)
					{
						localizedStringProperty.LocalizedString.StringChanged -= this.m_LocalizedStringChanged;
					}
				}
			}
		}

		internal void RequestUpdate()
		{
			if (this.m_IgnoreChange || LocalizationSettings.Instance.IsChangingSelectedLocale || (this.CurrentOperation.IsValid() && !this.CurrentOperation.IsDone))
			{
				return;
			}
			if (this.m_CurrentLocale != null)
			{
				this.ApplyLocaleVariant(this.m_CurrentLocale);
			}
		}

		[SerializeReference]
		private List<TrackedObject> m_TrackedObjects = new List<TrackedObject>();

		private Locale m_CurrentLocale;

		private LocalizedString.ChangeHandler m_LocalizedStringChanged;

		private bool m_IgnoreChange;
	}
}
