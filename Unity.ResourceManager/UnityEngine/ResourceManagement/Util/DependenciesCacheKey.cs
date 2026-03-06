using System;
using System.Collections.Generic;
using UnityEngine.ResourceManagement.ResourceLocations;

namespace UnityEngine.ResourceManagement.Util
{
	internal sealed class DependenciesCacheKey : IOperationCacheKey, IEquatable<IOperationCacheKey>
	{
		public DependenciesCacheKey(IList<IResourceLocation> dependencies, int dependenciesHash)
		{
			this.m_Dependencies = dependencies;
			this.m_DependenciesHash = dependenciesHash;
		}

		public override int GetHashCode()
		{
			return this.m_DependenciesHash;
		}

		public override bool Equals(object obj)
		{
			return this.Equals(obj as DependenciesCacheKey);
		}

		public bool Equals(IOperationCacheKey other)
		{
			return this.Equals(other as DependenciesCacheKey);
		}

		private bool Equals(DependenciesCacheKey other)
		{
			return this == other || (other != null && LocationUtils.DependenciesEqual(this.m_Dependencies, other.m_Dependencies));
		}

		private readonly IList<IResourceLocation> m_Dependencies;

		private readonly int m_DependenciesHash;
	}
}
