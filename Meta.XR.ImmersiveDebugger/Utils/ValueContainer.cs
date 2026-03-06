using System;
using UnityEngine;

namespace Meta.XR.ImmersiveDebugger.Utils
{
	internal class ValueContainer<T> : ScriptableObject
	{
		private static string Path
		{
			get
			{
				return "Values/";
			}
		}

		public static ValueContainer<T> Load(string assetName)
		{
			return Resources.Load<ValueContainer<T>>(ValueContainer<T>.Path + assetName);
		}

		public T this[string valueName]
		{
			get
			{
				return this.GetValue(valueName);
			}
		}

		public T GetValue(string valueName)
		{
			foreach (ValueStruct<T> valueStruct in this.Values)
			{
				if (valueStruct.ValueName.Equals(valueName))
				{
					return valueStruct.Value;
				}
			}
			Debug.LogWarning(string.Concat(new string[]
			{
				"Value ",
				valueName,
				" not found in ",
				base.name,
				"."
			}));
			return default(T);
		}

		public ValueStruct<T>[] Values;
	}
}
