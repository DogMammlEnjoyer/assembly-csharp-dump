using System;
using System.Collections.Generic;

namespace UnityEngine.ResourceManagement.ResourceLocations
{
	public class ResourceLocationBase : IResourceLocation
	{
		public string InternalId
		{
			get
			{
				return this.m_Id;
			}
		}

		public string ProviderId
		{
			get
			{
				return this.m_ProviderId;
			}
		}

		public IList<IResourceLocation> Dependencies
		{
			get
			{
				return this.m_Dependencies;
			}
		}

		public bool HasDependencies
		{
			get
			{
				return this.m_Dependencies != null && this.m_Dependencies.Count > 0;
			}
		}

		public object Data
		{
			get
			{
				return this.m_Data;
			}
			set
			{
				this.m_Data = value;
			}
		}

		public string PrimaryKey
		{
			get
			{
				return this.m_PrimaryKey;
			}
			set
			{
				this.m_PrimaryKey = value;
			}
		}

		public int DependencyHashCode
		{
			get
			{
				return this.m_DependencyHashCode;
			}
		}

		public Type ResourceType
		{
			get
			{
				return this.m_Type;
			}
		}

		public int Hash(Type t)
		{
			return (this.m_HashCode * 31 + t.GetHashCode()) * 31 + this.DependencyHashCode;
		}

		public override string ToString()
		{
			return this.m_Id;
		}

		public ResourceLocationBase(string name, string id, string providerId, Type t, params IResourceLocation[] dependencies)
		{
			if (string.IsNullOrEmpty(id))
			{
				throw new ArgumentNullException("id");
			}
			if (string.IsNullOrEmpty(providerId))
			{
				throw new ArgumentNullException("providerId");
			}
			this.m_PrimaryKey = name;
			this.m_HashCode = (name.GetHashCode() * 31 + id.GetHashCode()) * 31 + providerId.GetHashCode();
			this.m_Name = name;
			this.m_Id = id;
			this.m_ProviderId = providerId;
			this.m_Dependencies = new List<IResourceLocation>(dependencies);
			this.m_Type = ((t == null) ? typeof(object) : t);
			this.ComputeDependencyHash();
		}

		public void ComputeDependencyHash()
		{
			this.m_DependencyHashCode = ((this.m_Dependencies.Count > 0) ? 17 : 0);
			foreach (IResourceLocation resourceLocation in this.m_Dependencies)
			{
				this.m_DependencyHashCode = this.m_DependencyHashCode * 31 + resourceLocation.Hash(typeof(object));
			}
		}

		private string m_Name;

		private string m_Id;

		private string m_ProviderId;

		private object m_Data;

		private int m_DependencyHashCode;

		private int m_HashCode;

		private Type m_Type;

		private List<IResourceLocation> m_Dependencies;

		private string m_PrimaryKey;
	}
}
