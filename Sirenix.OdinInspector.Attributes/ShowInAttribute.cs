using System;
using System.Diagnostics;

namespace Sirenix.OdinInspector
{
	[AttributeUsage(AttributeTargets.All)]
	[Conditional("UNITY_EDITOR")]
	public class ShowInAttribute : Attribute
	{
		public ShowInAttribute(PrefabKind prefabKind)
		{
			this.PrefabKind = prefabKind;
		}

		public PrefabKind PrefabKind;
	}
}
