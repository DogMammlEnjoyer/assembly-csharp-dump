using System;

namespace Valve.Newtonsoft.Json.Serialization
{
	public abstract class NamingStrategy
	{
		public bool ProcessDictionaryKeys { get; set; }

		public bool OverrideSpecifiedNames { get; set; }

		public virtual string GetPropertyName(string name, bool hasSpecifiedName)
		{
			if (hasSpecifiedName && !this.OverrideSpecifiedNames)
			{
				return name;
			}
			return this.ResolvePropertyName(name);
		}

		public virtual string GetDictionaryKey(string key)
		{
			if (!this.ProcessDictionaryKeys)
			{
				return key;
			}
			return this.ResolvePropertyName(key);
		}

		protected abstract string ResolvePropertyName(string name);
	}
}
