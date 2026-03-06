using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace SouthPointe.Serialization.MessagePack
{
	public class MapDefinition
	{
		internal MapDefinition(SerializationContext context, Type type)
		{
			this.Type = type;
			if (!this.IsSerializable(context, type))
			{
				throw new CustomAttributeFormatException(((type != null) ? type.ToString() : null) + " does not have System.SerializableAttribute defined");
			}
			this.FieldInfos = new Dictionary<string, FieldInfo>();
			foreach (FieldInfo fieldInfo in type.GetFields(context.MapOptions.FieldFlags))
			{
				if (this.IsFieldSerializable(context, fieldInfo))
				{
					this.FieldInfos[fieldInfo.Name] = fieldInfo;
				}
			}
			this.FieldHandlers = new Dictionary<string, ITypeHandler>();
			foreach (FieldInfo fieldInfo2 in this.FieldInfos.Values)
			{
				this.FieldHandlers.Add(fieldInfo2.Name, context.TypeHandlers.Get(fieldInfo2.FieldType));
			}
			this.Callbacks = new Dictionary<Type, MethodInfo[]>();
			MethodInfo[] methods = type.GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.InvokeMethod);
			foreach (Type type2 in MapDefinition.callbackTypes)
			{
				List<MethodInfo> list = new List<MethodInfo>();
				foreach (MethodInfo methodInfo in methods)
				{
					if (this.AttributesExist(methodInfo, type2))
					{
						list.Add(methodInfo);
					}
				}
				if (list.Count > 0)
				{
					this.Callbacks[type2] = list.ToArray();
				}
			}
		}

		private bool IsSerializable(SerializationContext context, Type type)
		{
			return !context.MapOptions.RequireSerializableAttribute || Array.IndexOf<Type>(MapDefinition.serializableUnityTypes, type) != -1 || type.IsSerializable;
		}

		private bool AttributesExist(MemberInfo info, Type attributeType)
		{
			return info.GetCustomAttributes(attributeType, true).Length != 0;
		}

		private bool IsFieldSerializable(SerializationContext context, FieldInfo info)
		{
			return !this.AttributesExist(info, typeof(NonSerializedAttribute)) && (!context.MapOptions.IgnoreAutoPropertyValues || !info.Name.StartsWith("<")) && this.IsSerializable(context, info.FieldType);
		}

		private const BindingFlags MethodFlags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.InvokeMethod;

		private static readonly Type[] serializableUnityTypes = new Type[]
		{
			typeof(Color),
			typeof(Color32),
			typeof(Vector2),
			typeof(Vector3),
			typeof(Vector4),
			typeof(Quaternion),
			typeof(Vector2Int),
			typeof(Vector3Int)
		};

		private static readonly Type[] callbackTypes = new Type[]
		{
			typeof(OnDeserializingAttribute),
			typeof(OnDeserializedAttribute),
			typeof(OnSerializingAttribute),
			typeof(OnSerializedAttribute)
		};

		public readonly Type Type;

		public readonly Dictionary<string, FieldInfo> FieldInfos;

		public readonly Dictionary<string, ITypeHandler> FieldHandlers;

		public readonly Dictionary<Type, MethodInfo[]> Callbacks;
	}
}
