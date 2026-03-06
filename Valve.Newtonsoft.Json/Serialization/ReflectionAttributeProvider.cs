using System;
using System.Collections.Generic;
using Valve.Newtonsoft.Json.Utilities;

namespace Valve.Newtonsoft.Json.Serialization
{
	public class ReflectionAttributeProvider : IAttributeProvider
	{
		public ReflectionAttributeProvider(object attributeProvider)
		{
			ValidationUtils.ArgumentNotNull(attributeProvider, "attributeProvider");
			this._attributeProvider = attributeProvider;
		}

		public IList<Attribute> GetAttributes(bool inherit)
		{
			return ReflectionUtils.GetAttributes(this._attributeProvider, null, inherit);
		}

		public IList<Attribute> GetAttributes(Type attributeType, bool inherit)
		{
			return ReflectionUtils.GetAttributes(this._attributeProvider, attributeType, inherit);
		}

		private readonly object _attributeProvider;
	}
}
