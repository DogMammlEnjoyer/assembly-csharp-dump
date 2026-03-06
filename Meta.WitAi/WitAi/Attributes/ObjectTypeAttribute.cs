using System;
using System.Collections.Generic;
using UnityEngine;

namespace Meta.WitAi.Attributes
{
	[AttributeUsage(AttributeTargets.Field)]
	public class ObjectTypeAttribute : PropertyAttribute
	{
		public Type[] TargetTypes { get; }

		public bool RequiresAllTypes { get; }

		public ObjectTypeAttribute(Type targetType, params Type[] additionalTargetTypes)
		{
			this.RequiresAllTypes = 0;
			this.TargetTypes = this.VerifyTypes(targetType, additionalTargetTypes);
		}

		public ObjectTypeAttribute(bool requireAll, Type targetType, params Type[] additionalTargetTypes)
		{
			this.RequiresAllTypes = requireAll;
			this.TargetTypes = this.VerifyTypes(targetType, additionalTargetTypes);
		}

		private Type[] VerifyTypes(Type targetType, Type[] additionalTargetTypes)
		{
			List<Type> list = new List<Type>();
			if (this.VerifyType(targetType))
			{
				list.Add(targetType);
			}
			if (additionalTargetTypes != null)
			{
				foreach (Type type in additionalTargetTypes)
				{
					if (this.VerifyType(type))
					{
						list.Add(type);
					}
				}
			}
			return list.ToArray();
		}

		private bool VerifyType(Type targetType)
		{
			if (targetType == null)
			{
				Debug.LogError(base.GetType().Name + " cannot use null target type");
				return false;
			}
			return true;
		}
	}
}
