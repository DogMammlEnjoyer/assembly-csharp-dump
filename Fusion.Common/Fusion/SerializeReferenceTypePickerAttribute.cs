using System;

namespace Fusion
{
	[AttributeUsage(AttributeTargets.Field)]
	public class SerializeReferenceTypePickerAttribute : DecoratingPropertyAttribute
	{
		public SerializeReferenceTypePickerAttribute(params Type[] types)
		{
			this.Types = types;
		}

		public Type[] Types { get; private set; }

		public bool GroupTypesByNamespace = true;

		public bool ShowFullName = false;
	}
}
