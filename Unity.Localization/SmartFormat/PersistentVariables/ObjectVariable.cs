using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using UnityEngine.UIElements;

namespace UnityEngine.Localization.SmartFormat.PersistentVariables
{
	[UxmlObject]
	[DisplayName("Object Reference", null)]
	[Serializable]
	public class ObjectVariable : Variable<Object>
	{
		[CompilerGenerated]
		[DisplayName("Object Reference", null)]
		[Serializable]
		public new class UxmlSerializedData : Variable<Object>.UxmlSerializedData
		{
			[RegisterUxmlCache]
			[Conditional("UNITY_EDITOR")]
			public new static void Register()
			{
			}

			public override object CreateInstance()
			{
				return new ObjectVariable();
			}
		}
	}
}
