using System;
using UnityEngine.Scripting;

namespace Meta.Conduit
{
	public struct ConduitParameterValue
	{
		[Preserve]
		public ConduitParameterValue(object value)
		{
			this.Value = value;
			this.DataType = value.GetType();
		}

		[Preserve]
		public ConduitParameterValue(object value, Type dataType)
		{
			this.Value = value;
			this.DataType = dataType;
		}

		public readonly object Value;

		public Type DataType;
	}
}
