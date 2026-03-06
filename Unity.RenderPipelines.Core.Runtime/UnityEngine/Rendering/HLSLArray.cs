using System;

namespace UnityEngine.Rendering
{
	[AttributeUsage(AttributeTargets.Field)]
	public class HLSLArray : Attribute
	{
		public HLSLArray(int arraySize, Type elementType)
		{
			this.arraySize = arraySize;
			this.elementType = elementType;
		}

		public int arraySize;

		public Type elementType;
	}
}
