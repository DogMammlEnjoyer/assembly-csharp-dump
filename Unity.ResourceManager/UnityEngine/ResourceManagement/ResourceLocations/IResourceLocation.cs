using System;
using System.Collections.Generic;

namespace UnityEngine.ResourceManagement.ResourceLocations
{
	public interface IResourceLocation
	{
		string InternalId { get; }

		string ProviderId { get; }

		IList<IResourceLocation> Dependencies { get; }

		int Hash(Type resultType);

		int DependencyHashCode { get; }

		bool HasDependencies { get; }

		object Data { get; }

		string PrimaryKey { get; }

		Type ResourceType { get; }
	}
}
