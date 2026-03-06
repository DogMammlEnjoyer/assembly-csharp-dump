using System;
using System.Diagnostics;

namespace Sirenix.OdinInspector
{
	[DontApplyToListElements]
	[AttributeUsage(AttributeTargets.All)]
	[Conditional("UNITY_EDITOR")]
	public class DisableInAttribute : Attribute
	{
		public DisableInAttribute(PrefabKind prefabKind)
		{
			this.PrefabKind = prefabKind;
		}

		public PrefabKind PrefabKind;
	}
}
