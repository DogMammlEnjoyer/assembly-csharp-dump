using System;

namespace System.Reflection
{
	/// <summary>Contains methods for converting <see cref="T:System.Type" /> objects.</summary>
	public static class IntrospectionExtensions
	{
		/// <summary>Returns the <see cref="T:System.Reflection.TypeInfo" /> representation of the specified type.</summary>
		/// <param name="type">The type to convert.</param>
		/// <returns>The converted object.</returns>
		public static TypeInfo GetTypeInfo(this Type type)
		{
			if (type == null)
			{
				throw new ArgumentNullException("type");
			}
			IReflectableType reflectableType = type as IReflectableType;
			if (reflectableType != null)
			{
				return reflectableType.GetTypeInfo();
			}
			return new TypeDelegator(type);
		}
	}
}
