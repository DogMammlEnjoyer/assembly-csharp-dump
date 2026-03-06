using System;
using System.Linq.Expressions;

namespace System.Dynamic
{
	/// <summary>Represents a dynamic object, that can have its operations bound at runtime.</summary>
	public interface IDynamicMetaObjectProvider
	{
		/// <summary>Returns the <see cref="T:System.Dynamic.DynamicMetaObject" /> responsible for binding operations performed on this object.</summary>
		/// <param name="parameter">The expression tree representation of the runtime value.</param>
		/// <returns>The <see cref="T:System.Dynamic.DynamicMetaObject" /> to bind this object.</returns>
		DynamicMetaObject GetMetaObject(Expression parameter);
	}
}
