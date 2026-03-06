using System;
using System.Collections.Generic;

namespace UnityEngine.ResourceManagement.ResourceLocations
{
	internal class LocationWrapper : IResourceLocation
	{
		public LocationWrapper(IResourceLocation location)
		{
			this.m_InternalLocation = location;
		}

		public string InternalId
		{
			get
			{
				return this.m_InternalLocation.InternalId;
			}
		}

		public string ProviderId
		{
			get
			{
				return this.m_InternalLocation.ProviderId;
			}
		}

		public IList<IResourceLocation> Dependencies
		{
			get
			{
				return this.m_InternalLocation.Dependencies;
			}
		}

		public int DependencyHashCode
		{
			get
			{
				return this.m_InternalLocation.DependencyHashCode;
			}
		}

		public bool HasDependencies
		{
			get
			{
				return this.m_InternalLocation.HasDependencies;
			}
		}

		public object Data
		{
			get
			{
				return this.m_InternalLocation.Data;
			}
		}

		public string PrimaryKey
		{
			get
			{
				return this.m_InternalLocation.PrimaryKey;
			}
		}

		public Type ResourceType
		{
			get
			{
				return this.m_InternalLocation.ResourceType;
			}
		}

		public int Hash(Type resultType)
		{
			return this.m_InternalLocation.Hash(resultType);
		}

		private IResourceLocation m_InternalLocation;
	}
}
