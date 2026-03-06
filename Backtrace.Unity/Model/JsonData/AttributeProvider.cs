using System;
using System.Collections.Generic;
using Backtrace.Unity.Model.Attributes;

namespace Backtrace.Unity.Model.JsonData
{
	public sealed class AttributeProvider
	{
		public string ApplicationVersion
		{
			get
			{
				return this["application.version"];
			}
		}

		public string ApplicationSessionKey
		{
			get
			{
				string result;
				if (this._attributes.TryGetValue("application.session", out result))
				{
					return result;
				}
				return null;
			}
		}

		public string ApplicationGuid
		{
			get
			{
				return this["guid"];
			}
		}

		internal AttributeProvider() : this(new List<IScopeAttributeProvider>
		{
			new MachineAttributeProvider(),
			new RuntimeAttributeProvider(),
			new PiiAttributeProvider()
		}, new List<IDynamicAttributeProvider>
		{
			new MachineStateAttributeProvider(),
			new ProcessAttributeProvider(),
			new SceneAttributeProvider()
		})
		{
		}

		internal AttributeProvider(IEnumerable<IScopeAttributeProvider> scopeAttributeProvider, IList<IDynamicAttributeProvider> dynamicAttributeProvider)
		{
			if (scopeAttributeProvider == null)
			{
				throw new ArgumentException("Scoped attributes provider collection is not defined");
			}
			if (dynamicAttributeProvider == null)
			{
				throw new ArgumentException("dynamic attributes provider colleciton is not defined");
			}
			foreach (IScopeAttributeProvider scopeAttributeProvider2 in scopeAttributeProvider)
			{
				scopeAttributeProvider2.GetAttributes(this._attributes);
			}
			this._dynamicAttributeProvider = dynamicAttributeProvider;
		}

		public string this[string index]
		{
			get
			{
				return this._attributes[index];
			}
			set
			{
				this._attributes[index] = value;
			}
		}

		public int Count()
		{
			return this._attributes.Count;
		}

		public void AddDynamicAttributeProvider(IDynamicAttributeProvider attributeProvider)
		{
			if (attributeProvider == null)
			{
				return;
			}
			this._dynamicAttributeProvider.Add(attributeProvider);
		}

		internal void AddScopedAttributeProvider(IScopeAttributeProvider attributeProvider)
		{
			if (attributeProvider == null)
			{
				return;
			}
			attributeProvider.GetAttributes(this._attributes);
		}

		internal void AddAttributes(IDictionary<string, string> source, bool includeDynamic = true)
		{
			if (includeDynamic)
			{
				foreach (IDynamicAttributeProvider dynamicAttributeProvider in this._dynamicAttributeProvider)
				{
					dynamicAttributeProvider.GetAttributes(source);
				}
			}
			foreach (KeyValuePair<string, string> keyValuePair in this._attributes)
			{
				source[keyValuePair.Key] = keyValuePair.Value;
			}
		}

		internal IDictionary<string, string> GenerateAttributes(bool includeDynamic = true)
		{
			Dictionary<string, string> dictionary = new Dictionary<string, string>();
			this.AddAttributes(dictionary, includeDynamic);
			return dictionary;
		}

		private readonly IDictionary<string, string> _attributes = new Dictionary<string, string>();

		private readonly IList<IDynamicAttributeProvider> _dynamicAttributeProvider;
	}
}
