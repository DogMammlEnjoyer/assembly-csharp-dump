using System;
using UnityEngine.Localization.SmartFormat.PersistentVariables;

namespace UnityEngine.Localization.SmartFormat.GlobalVariables
{
	[Obsolete("Please use UnityEngine.Localization.SmartFormat.PersistentVariables.Variable instead.")]
	[Serializable]
	public class GlobalVariable<T> : Variable<T>
	{
	}
}
