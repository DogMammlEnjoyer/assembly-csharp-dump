using System;
using System.Linq.Expressions;

namespace System.Linq
{
	/// <summary>Represents an expression tree and provides functionality to execute the expression tree after rewriting it.</summary>
	public abstract class EnumerableExecutor
	{
		internal abstract object ExecuteBoxed();

		internal static EnumerableExecutor Create(Expression expression)
		{
			return (EnumerableExecutor)Activator.CreateInstance(typeof(EnumerableExecutor<>).MakeGenericType(new Type[]
			{
				expression.Type
			}), new object[]
			{
				expression
			});
		}
	}
}
