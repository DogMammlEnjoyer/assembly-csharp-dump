using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using UnityEngine.UIElements;

namespace UnityEngine.Localization.SmartFormat.PersistentVariables
{
	[UxmlObject]
	[DisplayName("String", null)]
	[Serializable]
	public class StringVariable : Variable<string>
	{
		[CompilerGenerated]
		[DisplayName("String", null)]
		[Serializable]
		public new class UxmlSerializedData : Variable<string>.UxmlSerializedData
		{
			[RegisterUxmlCache]
			[Conditional("UNITY_EDITOR")]
			public new static void Register()
			{
			}

			public override object CreateInstance()
			{
				return new StringVariable();
			}
		}
	}
}
