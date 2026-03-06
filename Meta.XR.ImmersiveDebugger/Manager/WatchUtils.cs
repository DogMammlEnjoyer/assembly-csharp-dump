using System;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;
using Meta.XR.ImmersiveDebugger.Utils;
using UnityEngine;

namespace Meta.XR.ImmersiveDebugger.Manager
{
	internal static class WatchUtils
	{
		static WatchUtils()
		{
			Dictionary<Type, Type> types = WatchUtils.Types;
			if (types != null)
			{
				types.Clear();
			}
			WatchUtils.Register<float>(delegate(float value, ref string[] valuesContainer)
			{
				valuesContainer[0] = WatchUtils.FormatFloat(value);
			}, 1);
			WatchUtils.Register<bool>(delegate(bool value, ref string[] valuesContainer)
			{
				valuesContainer[0] = (value ? "True" : "False");
			}, 1);
			WatchUtils.Register<Vector3>(delegate(Vector3 value, ref string[] valuesContainer)
			{
				valuesContainer[0] = WatchUtils.FormatFloat(value.x);
				valuesContainer[1] = WatchUtils.FormatFloat(value.y);
				valuesContainer[2] = WatchUtils.FormatFloat(value.z);
			}, 3);
			WatchUtils.Register<Vector2>(delegate(Vector2 value, ref string[] valuesContainer)
			{
				valuesContainer[0] = WatchUtils.FormatFloat(value.x);
				valuesContainer[1] = WatchUtils.FormatFloat(value.y);
			}, 2);
			WatchUtils.Register<string>(delegate(string value, ref string[] valuesContainer)
			{
				valuesContainer[0] = ((value != null && value.Length > 64) ? (value.Substring(0, 64) + "...") : value);
			}, 1);
			WatchUtils.RegisterTexture(typeof(Texture2D));
		}

		public static Watch Create(MemberInfo memberInfo, InstanceHandle instanceHandle, DebugMember attribute)
		{
			Type dataType = memberInfo.GetDataType();
			Type type;
			if (!WatchUtils.Types.TryGetValue(dataType, out type))
			{
				type = WatchUtils.Register(dataType);
			}
			return Activator.CreateInstance(type, new object[]
			{
				memberInfo,
				instanceHandle,
				attribute
			}) as Watch;
		}

		internal static string FormatFloat(float value)
		{
			if (value <= -10000000f || value >= 10000000f)
			{
				return value.ToString("g3", CultureInfo.InvariantCulture);
			}
			return value.ToString("0.00", CultureInfo.InvariantCulture);
		}

		private static Type Register<T>(Watch<T>.ToDisplayStringSignature toDisplayString, int numberOfValues)
		{
			Watch<T>.Setup(toDisplayString, numberOfValues);
			Type typeFromHandle = typeof(Watch<T>);
			WatchUtils.Types.Add(typeof(T), typeFromHandle);
			return typeFromHandle;
		}

		private static Type Register(Type type)
		{
			Type type2 = typeof(Watch<>).MakeGenericType(new Type[]
			{
				type
			});
			WatchUtils.Types.Add(type, type2);
			return type2;
		}

		private static Type RegisterTexture(Type type)
		{
			Type typeFromHandle = typeof(WatchTexture);
			WatchUtils.Types.Add(type, typeFromHandle);
			return typeFromHandle;
		}

		internal static readonly Dictionary<Type, Type> Types = new Dictionary<Type, Type>();

		private const int MaxLetterCount = 64;
	}
}
