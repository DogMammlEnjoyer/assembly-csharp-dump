using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using UnityEngine.UIElements;

namespace UnityEngine.Localization.SmartFormat.PersistentVariables
{
	[UxmlObject]
	[DisplayName("Unsigned Short", null)]
	[Serializable]
	public class UShortVariable : Variable<ushort>
	{
		[CompilerGenerated]
		[DisplayName("Unsigned Short", null)]
		[Serializable]
		public new class UxmlSerializedData : Variable<ushort>.UxmlSerializedData
		{
			[RegisterUxmlCache]
			[Conditional("UNITY_EDITOR")]
			public new static void Register()
			{
			}

			public override object CreateInstance()
			{
				return new UShortVariable();
			}
		}
	}
}
