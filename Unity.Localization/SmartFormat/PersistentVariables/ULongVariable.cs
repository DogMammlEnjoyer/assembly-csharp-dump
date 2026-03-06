using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using UnityEngine.UIElements;

namespace UnityEngine.Localization.SmartFormat.PersistentVariables
{
	[UxmlObject]
	[DisplayName("Unsigned Long", null)]
	[Serializable]
	public class ULongVariable : Variable<ulong>
	{
		[CompilerGenerated]
		[DisplayName("Unsigned Long", null)]
		[Serializable]
		public new class UxmlSerializedData : Variable<ulong>.UxmlSerializedData
		{
			[RegisterUxmlCache]
			[Conditional("UNITY_EDITOR")]
			public new static void Register()
			{
			}

			public override object CreateInstance()
			{
				return new ULongVariable();
			}
		}
	}
}
