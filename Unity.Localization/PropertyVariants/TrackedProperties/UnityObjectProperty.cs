using System;
using System.Collections.Generic;

namespace UnityEngine.Localization.PropertyVariants.TrackedProperties
{
	[Serializable]
	public class UnityObjectProperty : ITrackedPropertyValue<Object>, ITrackedProperty, ISerializationCallbackReceiver
	{
		public string PropertyPath
		{
			get
			{
				return this.m_PropertyPath;
			}
			set
			{
				this.m_PropertyPath = value;
			}
		}

		public Type PropertyType { get; set; }

		public bool HasVariant(LocaleIdentifier localeIdentifier)
		{
			return this.m_VariantLookup.ContainsKey(localeIdentifier);
		}

		public void RemoveVariant(LocaleIdentifier localeIdentifier)
		{
			this.m_VariantLookup.Remove(localeIdentifier);
		}

		public bool GetValue(LocaleIdentifier localeIdentifier, out Object foundValue)
		{
			UnityObjectProperty.LocaleIdentifierValuePair localeIdentifierValuePair;
			if (this.m_VariantLookup.TryGetValue(localeIdentifier, out localeIdentifierValuePair))
			{
				foundValue = localeIdentifierValuePair.value.asset;
				return true;
			}
			foundValue = null;
			return false;
		}

		public bool GetValue(LocaleIdentifier localeIdentifier, LocaleIdentifier fallback, out Object foundValue)
		{
			UnityObjectProperty.LocaleIdentifierValuePair localeIdentifierValuePair;
			if (this.m_VariantLookup.TryGetValue(localeIdentifier, out localeIdentifierValuePair) || this.m_VariantLookup.TryGetValue(fallback, out localeIdentifierValuePair))
			{
				foundValue = localeIdentifierValuePair.value.asset;
				return true;
			}
			foundValue = null;
			return false;
		}

		public void SetValue(LocaleIdentifier localeIdentifier, Object newValue)
		{
			UnityObjectProperty.LocaleIdentifierValuePair localeIdentifierValuePair;
			if (!this.m_VariantLookup.TryGetValue(localeIdentifier, out localeIdentifierValuePair))
			{
				localeIdentifierValuePair = new UnityObjectProperty.LocaleIdentifierValuePair
				{
					localeIdentifier = localeIdentifier
				};
				this.m_VariantLookup[localeIdentifier] = localeIdentifierValuePair;
			}
			localeIdentifierValuePair.value.asset = newValue;
		}

		public void OnBeforeSerialize()
		{
			Type propertyType = this.PropertyType;
			this.m_TypeString = ((propertyType != null) ? propertyType.AssemblyQualifiedName : null);
			this.m_VariantData.Clear();
			foreach (UnityObjectProperty.LocaleIdentifierValuePair item in this.m_VariantLookup.Values)
			{
				this.m_VariantData.Add(item);
			}
		}

		public void OnAfterDeserialize()
		{
			this.m_VariantLookup.Clear();
			foreach (UnityObjectProperty.LocaleIdentifierValuePair localeIdentifierValuePair in this.m_VariantData)
			{
				this.m_VariantLookup[localeIdentifierValuePair.localeIdentifier] = localeIdentifierValuePair;
			}
			if (!string.IsNullOrEmpty(this.m_TypeString))
			{
				this.PropertyType = Type.GetType(this.m_TypeString);
			}
		}

		[SerializeField]
		private string m_PropertyPath;

		[SerializeField]
		private string m_TypeString;

		[SerializeField]
		private List<UnityObjectProperty.LocaleIdentifierValuePair> m_VariantData = new List<UnityObjectProperty.LocaleIdentifierValuePair>();

		internal Dictionary<LocaleIdentifier, UnityObjectProperty.LocaleIdentifierValuePair> m_VariantLookup = new Dictionary<LocaleIdentifier, UnityObjectProperty.LocaleIdentifierValuePair>();

		[Serializable]
		internal class LocaleIdentifierValuePair
		{
			public LocaleIdentifier localeIdentifier;

			public LazyLoadReference<Object> value;
		}
	}
}
