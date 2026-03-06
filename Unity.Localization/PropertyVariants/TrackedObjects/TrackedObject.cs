using System;
using System.Collections.Generic;
using UnityEngine.Localization.PropertyVariants.TrackedProperties;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace UnityEngine.Localization.PropertyVariants.TrackedObjects
{
	[Serializable]
	public abstract class TrackedObject : ISerializationCallbackReceiver
	{
		public Object Target
		{
			get
			{
				return this.m_Target;
			}
			set
			{
				this.m_Target = value;
			}
		}

		public IList<ITrackedProperty> TrackedProperties
		{
			get
			{
				return this.m_TrackedProperties.items;
			}
		}

		public virtual bool CanTrackProperty(string propertyPath)
		{
			return true;
		}

		public T AddTrackedProperty<T>(string propertyPath) where T : ITrackedProperty, new()
		{
			T t = Activator.CreateInstance<T>();
			t.PropertyPath = propertyPath;
			T t2 = t;
			this.AddTrackedProperty(t2);
			return t2;
		}

		public virtual void AddTrackedProperty(ITrackedProperty trackedProperty)
		{
			if (trackedProperty == null)
			{
				throw new ArgumentNullException("trackedProperty");
			}
			if (string.IsNullOrEmpty(trackedProperty.PropertyPath))
			{
				throw new ArgumentException("Property path must not be null or empty.");
			}
			if (this.m_PropertiesLookup.ContainsKey(trackedProperty.PropertyPath))
			{
				throw new ArgumentException(trackedProperty.PropertyPath + " is already tracked.");
			}
			this.m_PropertiesLookup[trackedProperty.PropertyPath] = trackedProperty;
			this.TrackedProperties.Add(trackedProperty);
		}

		public virtual bool RemoveTrackedProperty(ITrackedProperty trackedProperty)
		{
			this.m_PropertiesLookup.Remove(trackedProperty.PropertyPath);
			return this.TrackedProperties.Remove(trackedProperty);
		}

		public T GetTrackedProperty<T>(string propertyPath, bool create = true) where T : ITrackedProperty, new()
		{
			ITrackedProperty trackedProperty = this.GetTrackedProperty(propertyPath);
			if (trackedProperty is T)
			{
				return (T)((object)trackedProperty);
			}
			if (!create)
			{
				return default(T);
			}
			return this.AddTrackedProperty<T>(propertyPath);
		}

		public virtual ITrackedProperty GetTrackedProperty(string propertyPath)
		{
			ITrackedProperty result;
			if (!this.m_PropertiesLookup.TryGetValue(propertyPath, out result))
			{
				return null;
			}
			return result;
		}

		public virtual ITrackedProperty CreateCustomTrackedProperty(string propertyPath)
		{
			return null;
		}

		public abstract AsyncOperationHandle ApplyLocale(Locale variantLocale, Locale defaultLocale);

		protected virtual void PostApplyTrackedProperties()
		{
		}

		public void OnAfterDeserialize()
		{
			this.m_PropertiesLookup.Clear();
			foreach (ITrackedProperty trackedProperty in this.m_TrackedProperties.items)
			{
				if (trackedProperty != null)
				{
					this.m_PropertiesLookup[trackedProperty.PropertyPath] = trackedProperty;
				}
			}
		}

		public void OnBeforeSerialize()
		{
			this.m_TrackedProperties.items.Clear();
			foreach (ITrackedProperty item in this.m_PropertiesLookup.Values)
			{
				this.m_TrackedProperties.items.Add(item);
			}
		}

		[SerializeField]
		[HideInInspector]
		private Object m_Target;

		[SerializeField]
		private TrackedObject.TrackedPropertiesCollection m_TrackedProperties = new TrackedObject.TrackedPropertiesCollection();

		private readonly Dictionary<string, ITrackedProperty> m_PropertiesLookup = new Dictionary<string, ITrackedProperty>();

		[Serializable]
		internal class TrackedPropertiesCollection
		{
			[SerializeReference]
			public List<ITrackedProperty> items = new List<ITrackedProperty>();
		}
	}
}
