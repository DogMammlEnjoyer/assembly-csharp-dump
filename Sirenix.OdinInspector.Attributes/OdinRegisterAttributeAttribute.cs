using System;
using System.Diagnostics;

namespace Sirenix.OdinInspector
{
	[AttributeUsage(AttributeTargets.Assembly, AllowMultiple = true, Inherited = true)]
	[Conditional("UNITY_EDITOR")]
	public class OdinRegisterAttributeAttribute : Attribute
	{
		public OdinRegisterAttributeAttribute(Type attributeType, string category, string description, bool isEnterprise)
		{
			this.AttributeType = attributeType;
			this.Categories = category;
			this.Description = description;
			this.IsEnterprise = isEnterprise;
		}

		public OdinRegisterAttributeAttribute(Type attributeType, string category, string description, bool isEnterprise, string url)
		{
			this.AttributeType = attributeType;
			this.Categories = category;
			this.Description = description;
			this.IsEnterprise = isEnterprise;
			this.DocumentationUrl = url;
		}

		public Type AttributeType;

		public string Categories;

		public string Description;

		public string DocumentationUrl;

		public bool IsEnterprise;
	}
}
