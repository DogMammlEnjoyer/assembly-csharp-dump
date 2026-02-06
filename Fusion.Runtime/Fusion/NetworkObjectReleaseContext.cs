using System;

namespace Fusion
{
	public readonly struct NetworkObjectReleaseContext
	{
		public NetworkObjectReleaseContext(NetworkObject obj, NetworkObjectTypeId typeId, bool isBeingDestroyed, bool isNested)
		{
			this.Object = obj;
			this.IsBeingDestroyed = isBeingDestroyed;
			this.TypeId = typeId;
			this.IsNestedObject = isNested;
		}

		public override string ToString()
		{
			return string.Format("[{0}, TypeId={1}, IsBeingDestroyed={2}, IsNestedObject={3}]", new object[]
			{
				this.Object,
				this.TypeId,
				this.IsBeingDestroyed,
				this.IsNestedObject
			});
		}

		public readonly NetworkObject Object;

		public readonly NetworkObjectTypeId TypeId;

		public readonly bool IsBeingDestroyed;

		public readonly bool IsNestedObject;
	}
}
