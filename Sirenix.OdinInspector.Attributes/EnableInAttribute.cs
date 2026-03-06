using System;
using System.Diagnostics;

namespace Sirenix.OdinInspector
{
	[DontApplyToListElements]
	[AttributeUsage(AttributeTargets.All)]
	[Conditional("UNITY_EDITOR")]
	public class EnableInAttribute : Attribute
	{
		public EnableInAttribute(PrefabKind prefabKind)
		{
			this.PrefabKind = prefabKind;
		}

		public PrefabKind PrefabKind;
	}
}
