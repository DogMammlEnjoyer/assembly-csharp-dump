using System;
using Unity.Properties;

namespace UnityEngine.UIElements
{
	public class ConverterGroup
	{
		public string id { get; }

		public string displayName { get; }

		public string description { get; }

		internal TypeConverterRegistry registry { get; }

		public ConverterGroup(string id, string displayName = null, string description = null)
		{
			this.id = id;
			this.displayName = displayName;
			this.description = description;
			this.registry = TypeConverterRegistry.Create();
		}

		public void AddConverter<TSource, TDestination>(TypeConverter<TSource, TDestination> converter)
		{
			this.registry.Register(typeof(TSource), typeof(TDestination), converter);
		}

		public bool TryConvert<TSource, TDestination>(ref TSource source, out TDestination destination)
		{
			Delegate @delegate;
			TypeConverter<TSource, TDestination> typeConverter;
			bool flag;
			if (this.registry.TryGetConverter(typeof(TSource), typeof(TDestination), out @delegate))
			{
				typeConverter = (@delegate as TypeConverter<TSource, TDestination>);
				flag = (typeConverter != null);
			}
			else
			{
				flag = false;
			}
			bool flag2 = flag;
			bool result;
			if (flag2)
			{
				destination = typeConverter(ref source);
				result = true;
			}
			else
			{
				destination = default(TDestination);
				result = false;
			}
			return result;
		}

		public bool TrySetValue<TContainer, TValue>(ref TContainer container, in PropertyPath path, TValue value, out VisitReturnCode returnCode)
		{
			bool isEmpty = path.IsEmpty;
			bool result;
			if (isEmpty)
			{
				returnCode = VisitReturnCode.InvalidPath;
				result = false;
			}
			else
			{
				SetValueVisitor<TValue> setValueVisitor = SetValueVisitor<TValue>.Pool.Get();
				setValueVisitor.group = this;
				setValueVisitor.Path = path;
				setValueVisitor.Value = value;
				try
				{
					bool flag = !PropertyContainer.TryAccept<TContainer>(setValueVisitor, ref container, out returnCode, default(VisitParameters));
					if (flag)
					{
						return false;
					}
					returnCode = setValueVisitor.ReturnCode;
				}
				finally
				{
					SetValueVisitor<TValue>.Pool.Release(setValueVisitor);
				}
				result = (returnCode == VisitReturnCode.Ok);
			}
			return result;
		}
	}
}
