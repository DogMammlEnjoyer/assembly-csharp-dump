using System;
using UnityEngine.ResourceManagement.ResourceLocations;

namespace UnityEngine.ResourceManagement.Util
{
	internal sealed class LocationCacheKey : IOperationCacheKey, IEquatable<IOperationCacheKey>
	{
		public LocationCacheKey(IResourceLocation location, Type desiredType)
		{
			if (location == null)
			{
				throw new NullReferenceException("Resource location cannot be null.");
			}
			if (desiredType == null)
			{
				throw new NullReferenceException("Desired type cannot be null.");
			}
			this.m_Location = location;
			this.m_DesiredType = desiredType;
		}

		public override int GetHashCode()
		{
			return this.m_Location.Hash(this.m_DesiredType);
		}

		public override bool Equals(object obj)
		{
			return this.Equals(obj as LocationCacheKey);
		}

		public bool Equals(IOperationCacheKey other)
		{
			return this.Equals(other as LocationCacheKey);
		}

		private bool Equals(LocationCacheKey other)
		{
			return this == other || (other != null && LocationUtils.LocationEquals(this.m_Location, other.m_Location) && object.Equals(this.m_DesiredType, other.m_DesiredType) && LocationUtils.DependenciesEqual(this.m_Location.Dependencies, other.m_Location.Dependencies));
		}

		private readonly IResourceLocation m_Location;

		private readonly Type m_DesiredType;
	}
}
