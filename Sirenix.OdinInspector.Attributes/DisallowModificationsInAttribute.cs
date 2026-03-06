using System;
using System.Diagnostics;

namespace Sirenix.OdinInspector
{
	[Conditional("UNITY_EDITOR")]
	[AttributeUsage(AttributeTargets.All, AllowMultiple = false, Inherited = true)]
	public sealed class DisallowModificationsInAttribute : Attribute
	{
		public DisallowModificationsInAttribute(PrefabKind kind)
		{
			this.PrefabKind = kind;
		}

		public PrefabKind PrefabKind;
	}
}
