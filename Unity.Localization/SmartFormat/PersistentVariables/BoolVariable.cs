using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using UnityEngine.UIElements;

namespace UnityEngine.Localization.SmartFormat.PersistentVariables
{
	[UxmlObject]
	[DisplayName("Boolean", null)]
	[Serializable]
	public class BoolVariable : Variable<bool>
	{
		[CompilerGenerated]
		[DisplayName("Boolean", null)]
		[Serializable]
		public new class UxmlSerializedData : Variable<bool>.UxmlSerializedData
		{
			[RegisterUxmlCache]
			[Conditional("UNITY_EDITOR")]
			public new static void Register()
			{
			}

			public override object CreateInstance()
			{
				return new BoolVariable();
			}
		}
	}
}
