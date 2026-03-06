using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using UnityEngine.UIElements;

namespace UnityEngine.Localization.SmartFormat.PersistentVariables
{
	[UxmlObject]
	[DisplayName("Integer", null)]
	[Serializable]
	public class IntVariable : Variable<int>
	{
		[CompilerGenerated]
		[DisplayName("Integer", null)]
		[Serializable]
		public new class UxmlSerializedData : Variable<int>.UxmlSerializedData
		{
			[RegisterUxmlCache]
			[Conditional("UNITY_EDITOR")]
			public new static void Register()
			{
			}

			public override object CreateInstance()
			{
				return new IntVariable();
			}
		}
	}
}
