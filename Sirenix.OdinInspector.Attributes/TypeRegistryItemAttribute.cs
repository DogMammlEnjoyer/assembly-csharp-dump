using System;
using UnityEngine;

namespace Sirenix.OdinInspector
{
	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Enum | AttributeTargets.Interface)]
	public class TypeRegistryItemAttribute : Attribute
	{
		public TypeRegistryItemAttribute(string name = null, string categoryPath = null, SdfIconType icon = SdfIconType.None, float lightIconColorR = 0f, float lightIconColorG = 0f, float lightIconColorB = 0f, float lightIconColorA = 0f, float darkIconColorR = 0f, float darkIconColorG = 0f, float darkIconColorB = 0f, float darkIconColorA = 0f, int priority = 0)
		{
			this.Name = name;
			this.CategoryPath = categoryPath;
			this.Icon = icon;
			bool flag = lightIconColorR != 0f || lightIconColorG != 0f || lightIconColorB != 0f || lightIconColorA > 0f;
			if (flag)
			{
				float a = (lightIconColorA > 0f) ? lightIconColorA : 1f;
				this.LightIconColor = new Color?(new Color(lightIconColorR, lightIconColorG, lightIconColorB, a));
			}
			else
			{
				this.LightIconColor = null;
			}
			bool flag2 = darkIconColorR != 0f || darkIconColorG != 0f || darkIconColorB != 0f || darkIconColorA > 0f;
			if (flag2)
			{
				float a2 = (darkIconColorA > 0f) ? darkIconColorA : 1f;
				this.DarkIconColor = new Color?(new Color(darkIconColorR, darkIconColorG, darkIconColorB, a2));
			}
			else
			{
				this.DarkIconColor = null;
			}
			this.Priority = priority;
		}

		public string Name;

		public string CategoryPath;

		public SdfIconType Icon;

		public Color? LightIconColor;

		public Color? DarkIconColor;

		public int Priority;
	}
}
