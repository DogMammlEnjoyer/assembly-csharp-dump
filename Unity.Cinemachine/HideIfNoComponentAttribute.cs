using System;
using UnityEngine;

namespace Unity.Cinemachine
{
	public sealed class HideIfNoComponentAttribute : PropertyAttribute
	{
		public HideIfNoComponentAttribute(Type type)
		{
			this.ComponentType = type;
		}

		public Type ComponentType;
	}
}
