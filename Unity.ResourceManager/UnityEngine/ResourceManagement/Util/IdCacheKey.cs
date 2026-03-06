using System;

namespace UnityEngine.ResourceManagement.Util
{
	internal sealed class IdCacheKey : IOperationCacheKey, IEquatable<IOperationCacheKey>
	{
		public IdCacheKey(string id)
		{
			this.ID = id;
			this.locationType = typeof(object);
		}

		public IdCacheKey(Type locType, string id)
		{
			this.ID = id;
			this.locationType = locType;
		}

		private bool Equals(IdCacheKey other)
		{
			return this == other || (other != null && other.ID == this.ID && this.locationType == other.locationType);
		}

		public override int GetHashCode()
		{
			return (527 + this.ID.GetHashCode()) * 31 + this.locationType.GetHashCode();
		}

		public override bool Equals(object obj)
		{
			return this.Equals(obj as IdCacheKey);
		}

		public bool Equals(IOperationCacheKey other)
		{
			return this.Equals(other as IdCacheKey);
		}

		public string ID;

		public Type locationType;
	}
}
