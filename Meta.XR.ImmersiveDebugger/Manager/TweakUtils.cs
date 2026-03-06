using System;
using System.Collections.Generic;
using System.Reflection;
using Meta.XR.ImmersiveDebugger.Utils;
using UnityEngine;

namespace Meta.XR.ImmersiveDebugger.Manager
{
	internal static class TweakUtils
	{
		static TweakUtils()
		{
			Dictionary<Type, Type> types = TweakUtils._types;
			if (types != null)
			{
				types.Clear();
			}
			HashSet<Type> supportsValueRange = TweakUtils._supportsValueRange;
			if (supportsValueRange != null)
			{
				supportsValueRange.Clear();
			}
			HashSet<Type> supportsValueRange2 = TweakUtils._supportsValueRange;
			if (supportsValueRange2 != null)
			{
				supportsValueRange2.Add(typeof(int));
			}
			HashSet<Type> supportsValueRange3 = TweakUtils._supportsValueRange;
			if (supportsValueRange3 != null)
			{
				supportsValueRange3.Add(typeof(float));
			}
			TweakUtils.Register<float>(new Func<float, float, float, float>(Mathf.InverseLerp), new Func<float, float, float, float>(Mathf.Lerp), (float f) => f);
			TweakUtils.Register<int>((int start, int end, int value) => Mathf.InverseLerp((float)start, (float)end, (float)value), (int start, int end, float tween) => Mathf.RoundToInt(Mathf.Lerp((float)start, (float)end, tween)), (float f) => (int)f);
			TweakUtils.Register<bool>(delegate(bool _, bool _, bool value)
			{
				if (!value)
				{
					return 0f;
				}
				return 1f;
			}, (bool _, bool _, float tween) => tween > 0f, (float f) => f > 0f);
			HashSet<Type> supportsValueRange4 = TweakUtils._supportsValueRange;
			if (supportsValueRange4 != null)
			{
				supportsValueRange4.Add(typeof(Enum));
			}
			Dictionary<Type, Type> types2 = TweakUtils._types;
			if (types2 == null)
			{
				return;
			}
			types2.Add(typeof(Enum), typeof(TweakEnum));
		}

		public static bool IsTypeSupported(Type type)
		{
			return !(type == null) && (TweakUtils._types.ContainsKey(type) || TweakUtils.IsTypeSupported(type.BaseType));
		}

		public static bool IsTypeSupportsValueRange(Type t)
		{
			return t != null && TweakUtils._supportsValueRange.Contains(t);
		}

		public static Tweak Create(MemberInfo memberInfo, DebugMember attribute, InstanceHandle instanceHandle)
		{
			Type dataType = memberInfo.GetDataType();
			Type type;
			if (!TweakUtils._types.TryGetValue(dataType, out type))
			{
				return null;
			}
			return Activator.CreateInstance(type, new object[]
			{
				memberInfo,
				instanceHandle,
				attribute
			}) as Tweak;
		}

		public static TweakEnum Create(MemberInfo memberInfo, DebugMember attribute, InstanceHandle instanceHandle, Type enumType)
		{
			Type baseType = memberInfo.GetDataType().BaseType;
			if (baseType == null)
			{
				return null;
			}
			Type type;
			if (!TweakUtils._types.TryGetValue(baseType, out type))
			{
				return null;
			}
			return Activator.CreateInstance(type, new object[]
			{
				memberInfo,
				instanceHandle,
				attribute,
				enumType
			}) as TweakEnum;
		}

		private static void Register<T>(Func<T, T, T, float> inverseLerp, Func<T, T, float, T> lerp, Func<float, T> fromFloat)
		{
			TweakUtils._types.Add(typeof(T), typeof(Tweak<T>));
			Tweak<T>.InverseLerp = inverseLerp;
			Tweak<T>.Lerp = lerp;
			Tweak<T>.FromFloat = fromFloat;
		}

		public static bool IsMemberValidForTweak(MemberInfo member)
		{
			MemberTypes memberType = member.MemberType;
			if (memberType == MemberTypes.Field)
			{
				FieldInfo fieldInfo = member as FieldInfo;
				return TweakUtils.IsTypeSupported((fieldInfo != null) ? fieldInfo.FieldType : null);
			}
			if (memberType != MemberTypes.Property)
			{
				return false;
			}
			PropertyInfo propertyInfo = member as PropertyInfo;
			return propertyInfo.CanRead && propertyInfo.CanWrite && TweakUtils.IsTypeSupported(propertyInfo.PropertyType);
		}

		public static void ProcessMinMaxRange(MemberInfo member, DebugMember attribute, InstanceHandle instance)
		{
			Type dataType = member.GetDataType();
			double num = 0.0;
			if (dataType == typeof(float))
			{
				num = (double)((float)member.GetValue(instance.Instance));
			}
			else if (dataType == typeof(int))
			{
				num = (double)((int)member.GetValue(instance.Instance));
			}
			else if (dataType == typeof(double))
			{
				num = (double)member.GetValue(instance.Instance);
			}
			if ((double)attribute.Min <= num && num <= (double)attribute.Max)
			{
				return;
			}
			float num2 = Mathf.Abs((float)(num * 0.5));
			attribute.Min = TweakUtils.RoundToNearest((float)(num - (double)num2), "min");
			attribute.Max = TweakUtils.RoundToNearest((float)(num + (double)num2), "max");
		}

		internal static float RoundToNearest(float value, string op)
		{
			if (value >= 0f)
			{
				if (value < 1f)
				{
					return (float)((op == "min") ? 0 : 1);
				}
				if (value >= 10f)
				{
					float num = Mathf.Pow(10f, Mathf.Floor(Mathf.Log10(value)));
					return Mathf.Ceil(value / num) * num;
				}
				return Mathf.Round(value);
			}
			else if (value <= -1f)
			{
				if (value <= -10f)
				{
					float num = Mathf.Pow(10f, Mathf.Floor(Mathf.Log10(-value)));
					return ((op == "min") ? Mathf.Floor(value / num) : Mathf.Ceil(value / num)) * num;
				}
				return Mathf.Round(value);
			}
			else
			{
				if (!(op == "min"))
				{
					return 0f;
				}
				return -1f;
			}
		}

		private static readonly Dictionary<Type, Type> _types = new Dictionary<Type, Type>();

		private static readonly HashSet<Type> _supportsValueRange = new HashSet<Type>();

		private const string Min = "min";

		private const string Max = "max";
	}
}
