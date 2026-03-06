using System;
using UnityEngine;

namespace Oculus.Interaction
{
	[AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
	public class InterfaceAttribute : PropertyAttribute
	{
		public InterfaceAttribute(Type type, params Type[] types)
		{
			this.Types = new Type[types.Length + 1];
			this.Types[0] = type;
			for (int i = 0; i < types.Length; i++)
			{
				this.Types[i + 1] = types[i];
			}
		}

		public InterfaceAttribute(string typeFromFieldName)
		{
			this.TypeFromFieldName = typeFromFieldName;
		}

		public Type[] Types;

		public string TypeFromFieldName;
	}
}
