using System;
using UnityEngine;

namespace Unity.XR.CoreUtils.GUI
{
	public sealed class EnumDisplayAttribute : PropertyAttribute
	{
		public EnumDisplayAttribute(params object[] enumValues)
		{
			this.Names = new string[enumValues.Length];
			this.Values = new int[enumValues.Length];
			int i = 0;
			while (i < this.Values.Length)
			{
				Enum @enum = enumValues[i] as Enum;
				if (@enum == null)
				{
					Debug.LogError(string.Format("Non-enum passed into EnumDisplay Attribute: {0}", enumValues[i]));
				}
				else
				{
					this.Names[i] = @enum.ToString();
					this.Values[i] = Convert.ToInt32(@enum);
					i++;
				}
			}
		}

		public string[] Names;

		public int[] Values;
	}
}
