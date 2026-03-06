using System;

namespace UnityEngine.VFX
{
	[AttributeUsage(AttributeTargets.Struct, AllowMultiple = false)]
	public class VFXTypeAttribute : Attribute
	{
		public VFXTypeAttribute(VFXTypeAttribute.Usage usages = VFXTypeAttribute.Usage.Default, string name = null)
		{
			this.usages = usages;
			this.name = name;
		}

		internal VFXTypeAttribute.Usage usages { get; private set; }

		internal string name { get; private set; }

		[Flags]
		public enum Usage
		{
			Default = 1,
			GraphicsBuffer = 2,
			ExcludeFromProperty = 4
		}
	}
}
