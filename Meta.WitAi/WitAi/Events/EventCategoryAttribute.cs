using System;
using UnityEngine;

namespace Meta.WitAi.Events
{
	[AttributeUsage(AttributeTargets.Field, AllowMultiple = true)]
	public class EventCategoryAttribute : PropertyAttribute
	{
		public EventCategoryAttribute(string category = "")
		{
			this.Category = category;
		}

		public readonly string Category;
	}
}
