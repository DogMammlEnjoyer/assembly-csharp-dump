using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SouthPointe.Serialization.MessagePack
{
	public class TypeHandlers
	{
		public TypeHandlers(SerializationContext context)
		{
			this.context = context;
			this.handlers = new Dictionary<Type, ITypeHandler>
			{
				{
					typeof(bool),
					new BoolHandler()
				},
				{
					typeof(sbyte),
					new SByteHandler()
				},
				{
					typeof(byte),
					new ByteHandler()
				},
				{
					typeof(short),
					new ShortHandler()
				},
				{
					typeof(ushort),
					new UShortHandler()
				},
				{
					typeof(int),
					new IntHandler()
				},
				{
					typeof(uint),
					new UIntHandler()
				},
				{
					typeof(long),
					new LongHandler()
				},
				{
					typeof(ulong),
					new ULongHandler()
				},
				{
					typeof(float),
					new FloatHandler()
				},
				{
					typeof(double),
					new DoubleHandler()
				},
				{
					typeof(string),
					new StringHandler()
				},
				{
					typeof(byte[]),
					new ByteArrayHandler()
				},
				{
					typeof(char),
					new CharHandler()
				},
				{
					typeof(decimal),
					new DecimalHandler(context)
				},
				{
					typeof(object),
					new ObjectHandler(context)
				},
				{
					typeof(DateTime),
					new DateTimeHandler(context)
				},
				{
					typeof(Color),
					new ColorHandler(context)
				},
				{
					typeof(Color32),
					new Color32Handler(context)
				},
				{
					typeof(Guid),
					new GuidHandler(context)
				},
				{
					typeof(Quaternion),
					new QuaternionHandler(context)
				},
				{
					typeof(TimeSpan),
					new TimeSpanHandler(context)
				},
				{
					typeof(Uri),
					new UriHandler(context)
				},
				{
					typeof(Vector2),
					new Vector2Handler(context)
				},
				{
					typeof(Vector3),
					new Vector3Handler(context)
				},
				{
					typeof(Vector4),
					new Vector4Handler(context)
				},
				{
					typeof(Vector2Int),
					new Vector2IntHandler(context)
				},
				{
					typeof(Vector3Int),
					new Vector3IntHandler(context)
				}
			};
			this.extHandlers = new Dictionary<sbyte, IExtTypeHandler>
			{
				{
					-1,
					new DateTimeHandler(context)
				}
			};
			this.mapDefinitions = new Dictionary<Type, MapDefinition>();
		}

		public ITypeHandler Get<T>()
		{
			return this.Get(typeof(T));
		}

		public ITypeHandler Get(Type type)
		{
			Dictionary<Type, ITypeHandler> obj = this.handlers;
			ITypeHandler result;
			lock (obj)
			{
				this.AddIfNotExist(type);
				result = this.handlers[type];
			}
			return result;
		}

		public IExtTypeHandler GetExt(sbyte extType)
		{
			Dictionary<Type, ITypeHandler> obj = this.handlers;
			IExtTypeHandler result;
			lock (obj)
			{
				result = this.extHandlers[extType];
			}
			return result;
		}

		public void SetHandler(Type type, ITypeHandler handler)
		{
			Dictionary<Type, ITypeHandler> obj = this.handlers;
			lock (obj)
			{
				this.handlers[type] = handler;
			}
			if (handler is IExtTypeHandler)
			{
				IExtTypeHandler extTypeHandler = (IExtTypeHandler)handler;
				Dictionary<sbyte, IExtTypeHandler> obj2 = this.extHandlers;
				lock (obj2)
				{
					this.extHandlers[extTypeHandler.ExtType] = extTypeHandler;
				}
			}
		}

		private void AddIfNotExist(Type type)
		{
			if (this.handlers.ContainsKey(type))
			{
				return;
			}
			if (type.IsEnum)
			{
				this.AddIfNotExist(type, new DynamicEnumHandler(this.context, type));
				return;
			}
			if (type.IsNullable())
			{
				this.AddIfNotExist(type, new DynamicNullableHandler(this.context, type));
				return;
			}
			if (type.IsArray)
			{
				this.AddIfNotExist(type, new DynamicArrayHandler(this.context, type));
				return;
			}
			if (typeof(IList).IsAssignableFrom(type))
			{
				this.AddIfNotExist(type, new DynamicListHandler(this.context, type));
				return;
			}
			if (typeof(IDictionary).IsAssignableFrom(type))
			{
				this.AddIfNotExist(type, new DynamicDictionaryHandler(this.context, type));
				return;
			}
			if (type.IsClass || type.IsValueType)
			{
				this.AddIfNotExist(type, new DynamicMapHandler(this.context, this.GetLazyMapDefinition(type)));
				return;
			}
			throw new FormatException("No TypeHandler found for type: " + ((type != null) ? type.ToString() : null));
		}

		private void AddIfNotExist(Type type, ITypeHandler handler)
		{
			if (!this.handlers.ContainsKey(type))
			{
				this.handlers.Add(type, handler);
			}
		}

		private Lazy<MapDefinition> GetLazyMapDefinition(Type type)
		{
			return new Lazy<MapDefinition>(delegate()
			{
				if (!this.mapDefinitions.ContainsKey(type))
				{
					this.mapDefinitions[type] = new MapDefinition(this.context, type);
				}
				return this.mapDefinitions[type];
			});
		}

		private readonly SerializationContext context;

		private readonly Dictionary<Type, ITypeHandler> handlers;

		private readonly Dictionary<sbyte, IExtTypeHandler> extHandlers;

		private readonly Dictionary<Type, MapDefinition> mapDefinitions;
	}
}
