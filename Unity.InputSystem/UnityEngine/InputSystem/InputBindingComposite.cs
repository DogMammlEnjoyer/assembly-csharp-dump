using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine.InputSystem.Layouts;
using UnityEngine.InputSystem.Utilities;

namespace UnityEngine.InputSystem
{
	public abstract class InputBindingComposite
	{
		public abstract Type valueType { get; }

		public abstract int valueSizeInBytes { get; }

		public unsafe abstract void ReadValue(ref InputBindingCompositeContext context, void* buffer, int bufferSize);

		public abstract object ReadValueAsObject(ref InputBindingCompositeContext context);

		public virtual float EvaluateMagnitude(ref InputBindingCompositeContext context)
		{
			return -1f;
		}

		protected virtual void FinishSetup(ref InputBindingCompositeContext context)
		{
		}

		internal void CallFinishSetup(ref InputBindingCompositeContext context)
		{
			this.FinishSetup(ref context);
		}

		internal static Type GetValueType(string composite)
		{
			if (string.IsNullOrEmpty(composite))
			{
				throw new ArgumentNullException("composite");
			}
			Type type = InputBindingComposite.s_Composites.LookupTypeRegistration(composite);
			if (type == null)
			{
				return null;
			}
			return TypeHelpers.GetGenericTypeArgumentFromHierarchy(type, typeof(InputBindingComposite<>), 0);
		}

		public static string GetExpectedControlLayoutName(string composite, string part)
		{
			if (string.IsNullOrEmpty(composite))
			{
				throw new ArgumentNullException("composite");
			}
			if (string.IsNullOrEmpty(part))
			{
				throw new ArgumentNullException("part");
			}
			Type type = InputBindingComposite.s_Composites.LookupTypeRegistration(composite);
			if (type == null)
			{
				return null;
			}
			FieldInfo field = type.GetField(part, BindingFlags.IgnoreCase | BindingFlags.Instance | BindingFlags.Public);
			if (field == null)
			{
				return null;
			}
			InputControlAttribute customAttribute = field.GetCustomAttribute(false);
			if (customAttribute == null)
			{
				return null;
			}
			return customAttribute.layout;
		}

		internal static IEnumerable<string> GetPartNames(string composite)
		{
			if (string.IsNullOrEmpty(composite))
			{
				throw new ArgumentNullException("composite");
			}
			Type type = InputBindingComposite.s_Composites.LookupTypeRegistration(composite);
			if (type == null)
			{
				yield break;
			}
			foreach (FieldInfo fieldInfo in type.GetFields(BindingFlags.Instance | BindingFlags.Public))
			{
				if (fieldInfo.GetCustomAttribute<InputControlAttribute>() != null)
				{
					yield return fieldInfo.Name;
				}
			}
			FieldInfo[] array = null;
			yield break;
		}

		internal static string GetDisplayFormatString(string composite)
		{
			if (string.IsNullOrEmpty(composite))
			{
				throw new ArgumentNullException("composite");
			}
			Type type = InputBindingComposite.s_Composites.LookupTypeRegistration(composite);
			if (type == null)
			{
				return null;
			}
			DisplayStringFormatAttribute customAttribute = type.GetCustomAttribute<DisplayStringFormatAttribute>();
			if (customAttribute == null)
			{
				return null;
			}
			return customAttribute.formatString;
		}

		internal static TypeTable s_Composites;
	}
}
