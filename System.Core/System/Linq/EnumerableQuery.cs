using System;
using System.Collections;
using System.Linq.Expressions;

namespace System.Linq
{
	/// <summary>Represents an <see cref="T:System.Collections.IEnumerable" /> as an <see cref="T:System.Linq.EnumerableQuery" /> data source. </summary>
	public abstract class EnumerableQuery
	{
		internal abstract Expression Expression { get; }

		internal abstract IEnumerable Enumerable { get; }

		internal static IQueryable Create(Type elementType, IEnumerable sequence)
		{
			return (IQueryable)Activator.CreateInstance(typeof(EnumerableQuery<>).MakeGenericType(new Type[]
			{
				elementType
			}), new object[]
			{
				sequence
			});
		}

		internal static IQueryable Create(Type elementType, Expression expression)
		{
			return (IQueryable)Activator.CreateInstance(typeof(EnumerableQuery<>).MakeGenericType(new Type[]
			{
				elementType
			}), new object[]
			{
				expression
			});
		}
	}
}
