using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Dynamic.Utils;
using System.Threading;

namespace System.Linq.Expressions
{
	/// <summary>Represents a block that contains a sequence of expressions where variables can be defined.</summary>
	[DebuggerTypeProxy(typeof(Expression.BlockExpressionProxy))]
	public class BlockExpression : Expression
	{
		/// <summary>Gets the expressions in this block.</summary>
		/// <returns>The read-only collection containing all the expressions in this block.</returns>
		public ReadOnlyCollection<Expression> Expressions
		{
			get
			{
				return this.GetOrMakeExpressions();
			}
		}

		/// <summary>Gets the variables defined in this block.</summary>
		/// <returns>The read-only collection containing all the variables defined in this block.</returns>
		public ReadOnlyCollection<ParameterExpression> Variables
		{
			get
			{
				return this.GetOrMakeVariables();
			}
		}

		/// <summary>Gets the last expression in this block.</summary>
		/// <returns>The <see cref="T:System.Linq.Expressions.Expression" /> object representing the last expression in this block.</returns>
		public Expression Result
		{
			get
			{
				return this.GetExpression(this.ExpressionCount - 1);
			}
		}

		internal BlockExpression()
		{
		}

		/// <summary>Dispatches to the specific visit method for this node type. For example, <see cref="T:System.Linq.Expressions.MethodCallExpression" /> calls the <see cref="M:System.Linq.Expressions.ExpressionVisitor.VisitMethodCall(System.Linq.Expressions.MethodCallExpression)" />.</summary>
		/// <param name="visitor">The visitor to visit this node with.</param>
		/// <returns>The result of visiting this node.</returns>
		protected internal override Expression Accept(ExpressionVisitor visitor)
		{
			return visitor.VisitBlock(this);
		}

		/// <summary>Returns the node type of this expression. Extension nodes should return <see cref="F:System.Linq.Expressions.ExpressionType.Extension" /> when overriding this method.</summary>
		/// <returns>The <see cref="T:System.Linq.Expressions.ExpressionType" /> of the expression.</returns>
		public sealed override ExpressionType NodeType
		{
			get
			{
				return ExpressionType.Block;
			}
		}

		/// <summary>Gets the static type of the expression that this <see cref="T:System.Linq.Expressions.Expression" /> represents.</summary>
		/// <returns>The <see cref="P:System.Linq.Expressions.BlockExpression.Type" /> that represents the static type of the expression.</returns>
		public override Type Type
		{
			get
			{
				return this.GetExpression(this.ExpressionCount - 1).Type;
			}
		}

		/// <summary>Creates a new expression that is like this one, but using the supplied children. If all of the children are the same, it will return this expression.</summary>
		/// <param name="variables">The <see cref="P:System.Linq.Expressions.BlockExpression.Variables" /> property of the result. </param>
		/// <param name="expressions">The <see cref="P:System.Linq.Expressions.BlockExpression.Expressions" /> property of the result. </param>
		/// <returns>This expression if no children changed, or an expression with the updated children.</returns>
		public BlockExpression Update(IEnumerable<ParameterExpression> variables, IEnumerable<Expression> expressions)
		{
			if (expressions != null)
			{
				ICollection<ParameterExpression> collection;
				if (variables == null)
				{
					collection = null;
				}
				else
				{
					collection = (variables as ICollection<ParameterExpression>);
					if (collection == null)
					{
						collection = (variables = variables.ToReadOnly<ParameterExpression>());
					}
				}
				if (this.SameVariables(collection))
				{
					ICollection<Expression> collection2 = expressions as ICollection<Expression>;
					if (collection2 == null)
					{
						collection2 = (expressions = expressions.ToReadOnly<Expression>());
					}
					if (this.SameExpressions(collection2))
					{
						return this;
					}
				}
			}
			return Expression.Block(this.Type, variables, expressions);
		}

		internal virtual bool SameVariables(ICollection<ParameterExpression> variables)
		{
			return variables == null || variables.Count == 0;
		}

		[ExcludeFromCodeCoverage]
		internal virtual bool SameExpressions(ICollection<Expression> expressions)
		{
			throw ContractUtils.Unreachable;
		}

		[ExcludeFromCodeCoverage]
		internal virtual Expression GetExpression(int index)
		{
			throw ContractUtils.Unreachable;
		}

		[ExcludeFromCodeCoverage]
		internal virtual int ExpressionCount
		{
			get
			{
				throw ContractUtils.Unreachable;
			}
		}

		[ExcludeFromCodeCoverage]
		internal virtual ReadOnlyCollection<Expression> GetOrMakeExpressions()
		{
			throw ContractUtils.Unreachable;
		}

		internal virtual ReadOnlyCollection<ParameterExpression> GetOrMakeVariables()
		{
			return EmptyReadOnlyCollection<ParameterExpression>.Instance;
		}

		[ExcludeFromCodeCoverage]
		internal virtual BlockExpression Rewrite(ReadOnlyCollection<ParameterExpression> variables, Expression[] args)
		{
			throw ContractUtils.Unreachable;
		}

		internal static ReadOnlyCollection<Expression> ReturnReadOnlyExpressions(BlockExpression provider, ref object collection)
		{
			Expression expression = collection as Expression;
			if (expression != null)
			{
				Interlocked.CompareExchange(ref collection, new ReadOnlyCollection<Expression>(new BlockExpressionList(provider, expression)), expression);
			}
			return (ReadOnlyCollection<Expression>)collection;
		}
	}
}
