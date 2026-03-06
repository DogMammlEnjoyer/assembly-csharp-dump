using System;

namespace Meta.WitAi.Json
{
	[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = true)]
	public class JsonPropertyAttribute : Attribute
	{
		public string PropertyName { get; private set; }

		public object DefaultValue { get; private set; }

		public JsonPropertyAttribute()
		{
			this.PropertyName = null;
			this.DefaultValue = null;
		}

		public JsonPropertyAttribute(string propertyName)
		{
			this.PropertyName = propertyName;
			this.DefaultValue = null;
		}

		public JsonPropertyAttribute(string propertyName, object defaultValue)
		{
			this.PropertyName = propertyName;
			this.DefaultValue = defaultValue;
		}
	}
}
