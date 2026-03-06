using System;

namespace UnityEngine.Rendering
{
	internal struct GPUInstanceComponentDesc
	{
		public GPUInstanceComponentDesc(int inPropertyID, int inByteSize, bool inIsOverriden, bool inPerInstance, InstanceType inInstanceType, InstanceComponentGroup inComponentType)
		{
			this.propertyID = inPropertyID;
			this.byteSize = inByteSize;
			this.isOverriden = inIsOverriden;
			this.isPerInstance = inPerInstance;
			this.instanceType = inInstanceType;
			this.componentGroup = inComponentType;
		}

		public int propertyID;

		public int byteSize;

		public bool isOverriden;

		public bool isPerInstance;

		public InstanceType instanceType;

		public InstanceComponentGroup componentGroup;
	}
}
