using System;
using System.Diagnostics;
using System.Dynamic.Utils;
using System.Reflection;
using System.Runtime.CompilerServices;
using Unity;

namespace System.Linq.Expressions
{
	/// <summary>Represents an expression that has a unary operator.</summary>
	[DebuggerTypeProxy(typeof(Expression.UnaryExpressionProxy))]
	public sealed class UnaryExpression : Expression
	{
		internal UnaryExpression(ExpressionType nodeType, Expression expression, Type type, MethodInfo method)
		{
			this.Operand = expression;
			this.Method = method;
			this.NodeType = nodeType;
			this.Type = type;
		}

		/// <summary>Gets the static type of the expression that this <see cref="T:System.Linq.Expressions.Expression" /> represents.</summary>
		/// <returns>The <see cref="P:System.Linq.Expressions.UnaryExpression.Type" /> that represents the static type of the expression.</returns>
		public sealed override Type Type { get; }

		/// <summary>Returns the node type of this <see cref="T:System.Linq.Expressions.Expression" />.</summary>
		/// <returns>The <see cref="T:System.Linq.Expressions.ExpressionType" /> that represents this expression.</returns>
		public sealed override ExpressionType NodeType { get; }

		/// <summary>Gets the operand of the unary operation.</summary>
		/// <returns>An <see cref="T:System.Linq.Expressions.Expression" /> that represents the operand of the unary operation.</returns>
		public Expression Operand { get; }

		/// <summary>Gets the implementing method for the unary operation.</summary>
		/// <returns>The <see cref="T:System.Reflection.MethodInfo" /> that represents the implementing method.</returns>
		public MethodInfo Method { get; }

		/// <summary>Gets a value that indicates whether the expression tree node represents a lifted call to an operator.</summary>
		/// <returns>
		///     <see langword="true" /> if the node represents a lifted call; otherwise, <see langword="false" />.</returns>
		public bool IsLifted
		{
			get
			{
				if (this.NodeType == ExpressionType.TypeAs || this.NodeType == ExpressionType.Quote || this.NodeType == ExpressionType.Throw)
				{
					return false;
				}
				bool flag = this.Operand.Type.IsNullableType();
				bool flag2 = this.Type.IsNullableType();
				if (this.Method != null)
				{
					return (flag && !TypeUtils.AreEquivalent(this.Method.GetParametersCached()[0].ParameterType, this.Operand.Type)) || (flag2 && !TypeUtils.AreEquivalent(this.Method.ReturnType, this.Type));
				}
				return flag || flag2;
			}
		}

		/// <summary>Gets a value that indicates whether the expression tree node represents a lifted call to an operator whose return type is lifted to a nullable type.</summary>
		/// <returns>
		///     <see langword="true" /> if the operator's return type is lifted to a nullable type; otherwise, <see langword="false" />.</returns>
		public bool IsLiftedToNull
		{
			get
			{
				return this.IsLifted && this.Type.IsNullableType();
			}
		}

		protected internal override Expression Accept(ExpressionVisitor visitor)
		{
			return visitor.VisitUnary(this);
		}

		/// <summary>Gets a value that indicates whether the expression tree node can be reduced.</summary>
		/// <returns>True if a node can be reduced, otherwise false.</returns>
		public override bool CanReduce
		{
			get
			{
				ExpressionType nodeType = this.NodeType;
				return nodeType - ExpressionType.PreIncrementAssign <= 3;
			}
		}

		/// <summary>Reduces the expression node to a simpler expression. </summary>
		/// <returns>The reduced expression.</returns>
		public override Expression Reduce()
		{
			if (!this.CanReduce)
			{
				return this;
			}
			ExpressionType nodeType = this.Operand.NodeType;
			if (nodeType == ExpressionType.MemberAccess)
			{
				return this.ReduceMember();
			}
			if (nodeType == ExpressionType.Index)
			{
				return this.ReduceIndex();
			}
			return this.ReduceVariable();
		}

		private bool IsPrefix
		{
			get
			{
				return this.NodeType == ExpressionType.PreIncrementAssign || this.NodeType == ExpressionType.PreDecrementAssign;
			}
		}

		private UnaryExpression FunctionalOp(Expression operand)
		{
			ExpressionType nodeType;
			if (this.NodeType == ExpressionType.PreIncrementAssign || this.NodeType == ExpressionType.PostIncrementAssign)
			{
				nodeType = ExpressionType.Increment;
			}
			else
			{
				nodeType = ExpressionType.Decrement;
			}
			return new UnaryExpression(nodeType, operand, operand.Type, this.Method);
		}

		private Expression ReduceVariable()
		{
			if (this.IsPrefix)
			{
				return Expression.Assign(this.Operand, this.FunctionalOp(this.Operand));
			}
			ParameterExpression parameterExpression = Expression.Parameter(this.Operand.Type, null);
			return Expression.Block(new TrueReadOnlyCollection<ParameterExpression>(new ParameterExpression[]
			{
				parameterExpression
			}), new TrueReadOnlyCollection<Expression>(new Expression[]
			{
				Expression.Assign(parameterExpression, this.Operand),
				Expression.Assign(this.Operand, this.FunctionalOp(parameterExpression)),
				parameterExpression
			}));
		}

		private Expression ReduceMember()
		{
			MemberExpression memberExpression = (MemberExpression)this.Operand;
			if (memberExpression.Expression == null)
			{
				return this.ReduceVariable();
			}
			ParameterExpression parameterExpression = Expression.Parameter(memberExpression.Expression.Type, null);
			BinaryExpression binaryExpression = Expression.Assign(parameterExpression, memberExpression.Expression);
			memberExpression = Expression.MakeMemberAccess(parameterExpression, memberExpression.Member);
			if (this.IsPrefix)
			{
				return Expression.Block(new TrueReadOnlyCollection<ParameterExpression>(new ParameterExpression[]
				{
					parameterExpression
				}), new TrueReadOnlyCollection<Expression>(new Expression[]
				{
					binaryExpression,
					Expression.Assign(memberExpression, this.FunctionalOp(memberExpression))
				}));
			}
			ParameterExpression parameterExpression2 = Expression.Parameter(memberExpression.Type, null);
			return Expression.Block(new TrueReadOnlyCollection<ParameterExpression>(new ParameterExpression[]
			{
				parameterExpression,
				parameterExpression2
			}), new TrueReadOnlyCollection<Expression>(new Expression[]
			{
				binaryExpression,
				Expression.Assign(parameterExpression2, memberExpression),
				Expression.Assign(memberExpression, this.FunctionalOp(parameterExpression2)),
				parameterExpression2
			}));
		}

		private Expression ReduceIndex()
		{
			bool isPrefix = this.IsPrefix;
			IndexExpression indexExpression = (IndexExpression)this.Operand;
			int argumentCount = indexExpression.ArgumentCount;
			Expression[] array = new Expression[argumentCount + (isPrefix ? 2 : 4)];
			ParameterExpression[] array2 = new ParameterExpression[argumentCount + (isPrefix ? 1 : 2)];
			ParameterExpression[] array3 = new ParameterExpression[argumentCount];
			int i = 0;
			array2[i] = Expression.Parameter(indexExpression.Object.Type, null);
			array[i] = Expression.Assign(array2[i], indexExpression.Object);
			for (i++; i <= argumentCount; i++)
			{
				Expression argument = indexExpression.GetArgument(i - 1);
				array3[i - 1] = (array2[i] = Expression.Parameter(argument.Type, null));
				array[i] = Expression.Assign(array2[i], argument);
			}
			Expression instance = array2[0];
			PropertyInfo indexer = indexExpression.Indexer;
			Expression[] list = array3;
			indexExpression = Expression.MakeIndex(instance, indexer, new TrueReadOnlyCollection<Expression>(list));
			if (!isPrefix)
			{
				ParameterExpression parameterExpression = array2[i] = Expression.Parameter(indexExpression.Type, null);
				array[i] = Expression.Assign(array2[i], indexExpression);
				i++;
				array[i++] = Expression.Assign(indexExpression, this.FunctionalOp(parameterExpression));
				array[i++] = parameterExpression;
			}
			else
			{
				array[i++] = Expression.Assign(indexExpression, this.FunctionalOp(indexExpression));
			}
			return Expression.Block(new TrueReadOnlyCollection<ParameterExpression>(array2), new TrueReadOnlyCollection<Expression>(array));
		}

		/// <summary>Creates a new expression that is like this one, but using the supplied children. If all of the children are the same, it will return this expression.</summary>
		/// <param name="operand">The <see cref="P:System.Linq.Expressions.UnaryExpression.Operand" /> property of the result.</param>
		/// <returns>This expression if no children are changed or an expression with the updated children.</returns>
		public UnaryExpression Update(Expression operand)
		{
			if (operand == this.Operand)
			{
				return this;
			}
			return Expression.MakeUnary(this.NodeType, operand, this.Type, this.Method);
		}

		internal UnaryExpression()
		{
			ThrowStub.ThrowNotSupportedException();
		}
	}
}
