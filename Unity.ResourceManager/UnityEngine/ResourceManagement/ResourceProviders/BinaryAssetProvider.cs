using System;
using System.ComponentModel;
using UnityEngine.ResourceManagement.Util;

namespace UnityEngine.ResourceManagement.ResourceProviders
{
	[DisplayName("Binary Asset Provider")]
	internal class BinaryAssetProvider<TAdapter> : BinaryDataProvider where TAdapter : BinaryStorageBuffer.ISerializationAdapter, new()
	{
		public override object Convert(Type type, byte[] data)
		{
			uint num;
			return new BinaryStorageBuffer.Reader(data, 1024, 0U, new BinaryStorageBuffer.ISerializationAdapter[]
			{
				Activator.CreateInstance<TAdapter>()
			}).ReadObject(type, 0U, out num, false);
		}
	}
}
