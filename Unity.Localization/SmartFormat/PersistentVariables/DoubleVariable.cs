using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using UnityEngine.UIElements;

namespace UnityEngine.Localization.SmartFormat.PersistentVariables
{
	[UxmlObject]
	[DisplayName("Double", null)]
	[Serializable]
	public class DoubleVariable : Variable<double>
	{
		[CompilerGenerated]
		[DisplayName("Double", null)]
		[Serializable]
		public new class UxmlSerializedData : Variable<double>.UxmlSerializedData
		{
			[RegisterUxmlCache]
			[Conditional("UNITY_EDITOR")]
			public new static void Register()
			{
			}

			public override object CreateInstance()
			{
				return new DoubleVariable();
			}
		}
	}
}
