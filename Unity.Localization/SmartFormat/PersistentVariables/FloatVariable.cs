using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using UnityEngine.UIElements;

namespace UnityEngine.Localization.SmartFormat.PersistentVariables
{
	[UxmlObject]
	[DisplayName("Float", null)]
	[Serializable]
	public class FloatVariable : Variable<float>
	{
		[CompilerGenerated]
		[DisplayName("Float", null)]
		[Serializable]
		public new class UxmlSerializedData : Variable<float>.UxmlSerializedData
		{
			[RegisterUxmlCache]
			[Conditional("UNITY_EDITOR")]
			public new static void Register()
			{
			}

			public override object CreateInstance()
			{
				return new FloatVariable();
			}
		}
	}
}
