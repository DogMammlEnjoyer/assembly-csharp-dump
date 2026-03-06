using System;
using System.Diagnostics;
using System.Dynamic.Utils;
using System.Runtime.CompilerServices;
using Unity;

namespace System.Linq.Expressions
{
	/// <summary>Represents an operation between an expression and a type.</summary>
	[DebuggerTypeProxy(typeof(Expression.TypeBinaryExpressionProxy))]
	public sealed class TypeBinaryExpression : Expression
	{
		internal TypeBinaryExpression(Expression expression, Type typeOperand, ExpressionType nodeType)
		{
			this.Expression = expression;
			this.TypeOperand = typeOperand;
			this.NodeType = nodeType;
		}

		/// <summary>Gets the static type of the expression that this <see cref="P:System.Linq.Expressions.TypeBinaryExpression.Expression" /> represents.</summary>
		/// <returns>The <see cref="P:System.Linq.Expressions.TypeBinaryExpression.Type" /> that represents the static type of the expression.</returns>
		public sealed override Type Type
		{
			get
			{
				return typeof(bool);
			}
		}

		/// <summary>Returns the node type of this Expression. Extension nodes should return <see cref="F:System.Linq.Expressions.ExpressionType.Extension" /> when overriding this method.</summary>
		/// <returns>The <see cref="T:System.Linq.Expressions.ExpressionType" /> of the expression.</returns>
		public sealed override ExpressionType NodeType { get; }

		/// <summary>Gets the expression operand of a type test operation.</summary>
		/// <returns>An <see cref="T:System.Linq.Expressions.Expression" /> that represents the expression operand of a type test operation.</returns>
		public Expression Expression { get; }

		/// <summary>Gets the type operand of a type test operation.</summary>
		/// <returns>A <see cref="T:System.Type" /> that represents the type operand of a type test operation.</returns>
		public Type TypeOperand { get; }

		internal Expression ReduceTypeEqual()
		{
			Type type = this.Expression.Type;
			if (type.IsValueType || this.TypeOperand.IsPointer)
			{
				if (!type.IsNullableType())
				{
					return Expression.Block(this.Expression, Utils.Constant(type == this.TypeOperand.GetNonNullableType()));
				}
				if (type.GetNonNullableType() != this.TypeOperand.GetNonNullableType())
				{
					return Expression.Block(this.Expression, Utils.Constant(false));
				}
				return Expression.NotEqual(this.Expression, Expression.Constant(null, this.Expression.Type));
			}
			else
			{
				if (this.Expression.NodeType == ExpressionType.Constant)
				{
					return this.ReduceConstantTypeEqual();
				}
				ParameterExpression parameterExpression = this.Expression as ParameterExpression;
				if (parameterExpression != null && !parameterExpression.IsByRef)
				{
					return this.ByValParameterTypeEqual(parameterExpression);
				}
				parameterExpression = Expression.Parameter(typeof(object));
				return Expression.Block(new TrueReadOnlyCollection<ParameterExpression>(new ParameterExpression[]
				{
					parameterExpression
				}), new TrueReadOnlyCollection<Expression>(new Expression[]
				{
					Expression.Assign(parameterExpression, this.Expression),
					this.ByValParameterTypeEqual(parameterExpression)
				}));
			}
		}

		private Expression ByValParameterTypeEqual(ParameterExpression value)
		{
			Expression expression = Expression.Call(value, CachedReflectionInfo.Object_GetType);
			if (this.TypeOperand.IsInterface)
			{
				ParameterExpression parameterExpression = Expression.Parameter(typeof(Type));
				expression = Expression.Block(new TrueReadOnlyCollection<ParameterExpression>(new ParameterExpression[]
				{
					parameterExpression
				}), new TrueReadOnlyCollection<Expression>(new Expression[]
				{
					Expression.Assign(parameterExpression, expression),
					parameterExpression
				}));
			}
			return Expression.AndAlso(Expression.ReferenceNotEqual(value, Utils.Null), Expression.ReferenceEqual(expression, Expression.Constant(this.TypeOperand.GetNonNullableType(), typeof(Type))));
		}

		private Expression ReduceConstantTypeEqual()
		{
			ConstantExpression constantExpression = this.Expression as ConstantExpression;
			if (constantExpression.Value == null)
			{
				return Utils.Constant(false);
			}
			return Utils.Constant(this.TypeOperand.GetNonNullableType() == constantExpression.Value.GetType());
		}

		protected internal override Expression Accept(ExpressionVisitor visitor)
		{
			return visitor.VisitTypeBinary(this);
		}

		/// <summary>Creates a new expression that is like this one, but using the supplied children. If all of the children are the same, it will return this expression.</summary>
		/// <param name="expression">The <see cref="P:System.Linq.Expressions.TypeBinaryExpression.Expression" /> property of the result.</param>
		/// <returns>This expression if no children are changed or an expression with the updated children.</returns>
		public TypeBinaryExpression Update(Expression expression)
		{
			if (expression == this.Expression)
			{
				return this;
			}
			if (this.NodeType == ExpressionType.TypeIs)
			{
				return Expression.TypeIs(expression, this.TypeOperand);
			}
			return Expression.TypeEqual(expression, this.TypeOperand);
		}

		internal TypeBinaryExpression()
		{
			ThrowStub.ThrowNotSupportedException();
		}
	}
}
