using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Dynamic.Utils;
using System.Reflection;
using Unity;

namespace System.Linq.Expressions
{
	/// <summary>Represents indexing a property or array.</summary>
	[DebuggerTypeProxy(typeof(Expression.IndexExpressionProxy))]
	public sealed class IndexExpression : Expression, IArgumentProvider
	{
		internal IndexExpression(Expression instance, PropertyInfo indexer, IReadOnlyList<Expression> arguments)
		{
			indexer == null;
			this.Object = instance;
			this.Indexer = indexer;
			this._arguments = arguments;
		}

		/// <summary>Returns the node type of this <see cref="T:System.Linq.Expressions.Expression" />.</summary>
		/// <returns>The <see cref="T:System.Linq.Expressions.ExpressionType" /> that represents this expression.</returns>
		public sealed override ExpressionType NodeType
		{
			get
			{
				return ExpressionType.Index;
			}
		}

		/// <summary>Gets the static type of the expression that this <see cref="T:System.Linq.Expressions.Expression" /> represents.</summary>
		/// <returns>The <see cref="P:System.Linq.Expressions.IndexExpression.Type" /> that represents the static type of the expression.</returns>
		public sealed override Type Type
		{
			get
			{
				if (this.Indexer != null)
				{
					return this.Indexer.PropertyType;
				}
				return this.Object.Type.GetElementType();
			}
		}

		/// <summary>An object to index.</summary>
		/// <returns>The <see cref="T:System.Linq.Expressions.Expression" /> representing the object to index.</returns>
		public Expression Object { get; }

		/// <summary>Gets the <see cref="T:System.Reflection.PropertyInfo" /> for the property if the expression represents an indexed property, returns null otherwise.</summary>
		/// <returns>The <see cref="T:System.Reflection.PropertyInfo" /> for the property if the expression represents an indexed property, otherwise null.</returns>
		public PropertyInfo Indexer { get; }

		/// <summary>Gets the arguments that will be used to index the property or array.</summary>
		/// <returns>The read-only collection containing the arguments that will be used to index the property or array.</returns>
		public ReadOnlyCollection<Expression> Arguments
		{
			get
			{
				return ExpressionUtils.ReturnReadOnly<Expression>(ref this._arguments);
			}
		}

		/// <summary>Creates a new expression that is like this one, but using the supplied children. If all of the children are the same, it will return this expression.</summary>
		/// <param name="object">The <see cref="P:System.Linq.Expressions.IndexExpression.Object" /> property of the result.</param>
		/// <param name="arguments">The <see cref="P:System.Linq.Expressions.IndexExpression.Arguments" /> property of the result.</param>
		/// <returns>This expression if no children are changed or an expression with the updated children.</returns>
		public IndexExpression Update(Expression @object, IEnumerable<Expression> arguments)
		{
			if ((@object == this.Object & arguments != null) && ExpressionUtils.SameElements<Expression>(ref arguments, this.Arguments))
			{
				return this;
			}
			return Expression.MakeIndex(@object, this.Indexer, arguments);
		}

		public Expression GetArgument(int index)
		{
			return this._arguments[index];
		}

		public int ArgumentCount
		{
			get
			{
				return this._arguments.Count;
			}
		}

		protected internal override Expression Accept(ExpressionVisitor visitor)
		{
			return visitor.VisitIndex(this);
		}

		internal Expression Rewrite(Expression instance, Expression[] arguments)
		{
			return Expression.MakeIndex(instance, this.Indexer, arguments ?? this._arguments);
		}

		internal IndexExpression()
		{
			ThrowStub.ThrowNotSupportedException();
		}

		private IReadOnlyList<Expression> _arguments;
	}
}
