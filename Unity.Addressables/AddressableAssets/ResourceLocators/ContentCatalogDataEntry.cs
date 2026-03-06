using System;
using System.Collections.Generic;

namespace UnityEngine.AddressableAssets.ResourceLocators
{
	public class ContentCatalogDataEntry
	{
		public string InternalId { get; set; }

		public string Provider { get; private set; }

		public List<object> Keys { get; private set; }

		public List<object> Dependencies { get; private set; }

		public object Data { get; set; }

		public Type ResourceType { get; private set; }

		public ContentCatalogDataEntry(Type type, string internalId, string provider, IEnumerable<object> keys, IEnumerable<object> dependencies = null, object extraData = null)
		{
			this.InternalId = internalId;
			this.Provider = provider;
			this.ResourceType = type;
			this.Keys = new List<object>(keys);
			this.Dependencies = ((dependencies == null) ? new List<object>() : new List<object>(dependencies));
			this.Data = extraData;
		}
	}
}
