using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using UnityEngine.UIElements;

namespace UnityEngine.Localization.SmartFormat.PersistentVariables
{
	[UxmlObject]
	[DisplayName("Short", null)]
	[Serializable]
	public class ShortVariable : Variable<short>
	{
		[CompilerGenerated]
		[DisplayName("Short", null)]
		[Serializable]
		public new class UxmlSerializedData : Variable<short>.UxmlSerializedData
		{
			[RegisterUxmlCache]
			[Conditional("UNITY_EDITOR")]
			public new static void Register()
			{
			}

			public override object CreateInstance()
			{
				return new ShortVariable();
			}
		}
	}
}
