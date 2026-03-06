using System;
using System.Diagnostics;

namespace Sirenix.OdinInspector
{
	[DontApplyToListElements]
	[AttributeUsage(AttributeTargets.All, AllowMultiple = false, Inherited = true)]
	[Conditional("UNITY_EDITOR")]
	public sealed class DisableContextMenuAttribute : Attribute
	{
		public DisableContextMenuAttribute(bool disableForMember = true, bool disableCollectionElements = false)
		{
			this.DisableForMember = disableForMember;
			this.DisableForCollectionElements = disableCollectionElements;
		}

		public bool DisableForMember;

		public bool DisableForCollectionElements;
	}
}
