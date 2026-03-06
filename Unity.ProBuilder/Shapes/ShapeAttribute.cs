using System;

namespace UnityEngine.ProBuilder.Shapes
{
	[AttributeUsage(AttributeTargets.Class)]
	public class ShapeAttribute : Attribute
	{
		public ShapeAttribute(string n)
		{
			this.name = n;
		}

		public string name;
	}
}
