using System;

namespace OVRSimpleJSON
{
	public enum JSONNodeType
	{
		Array = 1,
		Object,
		String,
		Number,
		NullValue,
		Boolean,
		None,
		Custom = 255
	}
}
