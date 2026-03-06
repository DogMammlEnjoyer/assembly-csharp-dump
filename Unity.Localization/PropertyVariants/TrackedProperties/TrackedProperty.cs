using System;
using System.Collections.Generic;
using UnityEngine.Localization.SmartFormat;

namespace UnityEngine.Localization.PropertyVariants.TrackedProperties
{
	[Serializable]
	public class TrackedProperty<TPrimitive> : ITrackedPropertyValue<TPrimitive>, ITrackedProperty, IStringProperty, ISerializationCallbackReceiver, ITrackedPropertyRemoveVariant
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

		public bool HasVariant(LocaleIdentifier localeIdentifier)
		{
			return this.m_VariantLookup.ContainsKey(localeIdentifier);
		}

		public void RemoveVariant(LocaleIdentifier localeIdentifier)
		{
			this.m_VariantLookup.Remove(localeIdentifier);
		}

		public bool GetValue(LocaleIdentifier localeIdentifier, out TPrimitive foundValue)
		{
			TrackedProperty<TPrimitive>.LocaleIdentifierValuePair localeIdentifierValuePair;
			if (this.m_VariantLookup.TryGetValue(localeIdentifier, out localeIdentifierValuePair))
			{
				foundValue = localeIdentifierValuePair.value;
				return true;
			}
			foundValue = default(TPrimitive);
			return false;
		}

		public bool GetValue(LocaleIdentifier localeIdentifier, LocaleIdentifier fallback, out TPrimitive foundValue)
		{
			TrackedProperty<TPrimitive>.LocaleIdentifierValuePair localeIdentifierValuePair;
			if (this.m_VariantLookup.TryGetValue(localeIdentifier, out localeIdentifierValuePair) || this.m_VariantLookup.TryGetValue(fallback, out localeIdentifierValuePair))
			{
				foundValue = localeIdentifierValuePair.value;
				return true;
			}
			foundValue = default(TPrimitive);
			return false;
		}

		public void SetValue(LocaleIdentifier localeIdentifier, TPrimitive value)
		{
			TrackedProperty<TPrimitive>.LocaleIdentifierValuePair localeIdentifierValuePair;
			if (!this.m_VariantLookup.TryGetValue(localeIdentifier, out localeIdentifierValuePair))
			{
				localeIdentifierValuePair = new TrackedProperty<TPrimitive>.LocaleIdentifierValuePair
				{
					localeIdentifier = localeIdentifier
				};
				this.m_VariantLookup[localeIdentifier] = localeIdentifierValuePair;
			}
			localeIdentifierValuePair.value = value;
		}

		public string GetValueAsString(LocaleIdentifier localeIdentifier)
		{
			TPrimitive value;
			if (!this.GetValue(localeIdentifier, out value))
			{
				return null;
			}
			return this.ConvertToString(value);
		}

		public string GetValueAsString(LocaleIdentifier localeIdentifier, LocaleIdentifier fallback)
		{
			TPrimitive value;
			if (!this.GetValue(localeIdentifier, fallback, out value))
			{
				return null;
			}
			return this.ConvertToString(value);
		}

		public void SetValueFromString(LocaleIdentifier localeIdentifier, string stringValue)
		{
			TPrimitive value = this.ConvertFromString(stringValue);
			this.SetValue(localeIdentifier, value);
		}

		protected virtual string ConvertToString(TPrimitive value)
		{
			return Convert.ToString(value);
		}

		protected virtual TPrimitive ConvertFromString(string value)
		{
			return (TPrimitive)((object)Convert.ChangeType(value, typeof(TPrimitive)));
		}

		public void OnBeforeSerialize()
		{
			this.m_VariantData.Clear();
			foreach (TrackedProperty<TPrimitive>.LocaleIdentifierValuePair item in this.m_VariantLookup.Values)
			{
				this.m_VariantData.Add(item);
			}
		}

		public void OnAfterDeserialize()
		{
			this.m_VariantLookup.Clear();
			foreach (TrackedProperty<TPrimitive>.LocaleIdentifierValuePair localeIdentifierValuePair in this.m_VariantData)
			{
				this.m_VariantLookup[localeIdentifierValuePair.localeIdentifier] = localeIdentifierValuePair;
			}
		}

		public override string ToString()
		{
			return Smart.Format("{GetType().Name}({PropertyPath}) - {1:list:{Key}({Value.value})|, |, }", this, this.m_VariantLookup);
		}

		[SerializeField]
		private string m_PropertyPath;

		[SerializeField]
		private List<TrackedProperty<TPrimitive>.LocaleIdentifierValuePair> m_VariantData = new List<TrackedProperty<TPrimitive>.LocaleIdentifierValuePair>();

		internal Dictionary<LocaleIdentifier, TrackedProperty<TPrimitive>.LocaleIdentifierValuePair> m_VariantLookup = new Dictionary<LocaleIdentifier, TrackedProperty<TPrimitive>.LocaleIdentifierValuePair>();

		[Serializable]
		internal class LocaleIdentifierValuePair
		{
			public LocaleIdentifier localeIdentifier;

			public TPrimitive value;
		}
	}
}
