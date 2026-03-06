using System;

namespace UnityEngine.Rendering
{
	internal struct InstanceCullingBatcherDesc
	{
		public static InstanceCullingBatcherDesc NewDefault()
		{
			return new InstanceCullingBatcherDesc
			{
				onCompleteCallback = null
			};
		}

		public OnCullingCompleteCallback onCompleteCallback;
	}
}
