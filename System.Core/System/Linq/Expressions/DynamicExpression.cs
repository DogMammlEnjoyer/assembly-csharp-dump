using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using System.Dynamic.Utils;
using System.Runtime.CompilerServices;
using Unity;

namespace System.Linq.Expressions
{
	/// <summary>Represents a dynamic operation.</summary>
	public class DynamicExpression : Expression, IDynamicExpression, IArgumentProvider
	{
		internal DynamicExpression(Type delegateType, CallSiteBinder binder)
		{
			this.DelegateType = delegateType;
			this.Binder = binder;
		}

		public override bool CanReduce
		{
			get
			{
				return true;
			}
		}

		public override Expression Reduce()
		{
			ConstantExpression constantExpression = Expression.Constant(CallSite.Create(this.DelegateType, this.Binder));
			return Expression.Invoke(Expression.Field(constantExpression, "Target"), this.Arguments.AddFirst(constantExpression));
		}

		internal static DynamicExpression Make(Type returnType, Type delegateType, CallSiteBinder binder, ReadOnlyCollection<Expression> arguments)
		{
			if (returnType == typeof(object))
			{
				return new DynamicExpressionN(delegateType, binder, arguments);
			}
			return new TypedDynamicExpressionN(returnType, delegateType, binder, arguments);
		}

		internal static DynamicExpression Make(Type returnType, Type delegateType, CallSiteBinder binder, Expression arg0)
		{
			if (returnType == typeof(object))
			{
				return new DynamicExpression1(delegateType, binder, arg0);
			}
			return new TypedDynamicExpression1(returnType, delegateType, binder, arg0);
		}

		internal static DynamicExpression Make(Type returnType, Type delegateType, CallSiteBinder binder, Expression arg0, Expression arg1)
		{
			if (returnType == typeof(object))
			{
				return new DynamicExpression2(delegateType, binder, arg0, arg1);
			}
			return new TypedDynamicExpression2(returnType, delegateType, binder, arg0, arg1);
		}

		internal static DynamicExpression Make(Type returnType, Type delegateType, CallSiteBinder binder, Expression arg0, Expression arg1, Expression arg2)
		{
			if (returnType == typeof(object))
			{
				return new DynamicExpression3(delegateType, binder, arg0, arg1, arg2);
			}
			return new TypedDynamicExpression3(returnType, delegateType, binder, arg0, arg1, arg2);
		}

		internal static DynamicExpression Make(Type returnType, Type delegateType, CallSiteBinder binder, Expression arg0, Expression arg1, Expression arg2, Expression arg3)
		{
			if (returnType == typeof(object))
			{
				return new DynamicExpression4(delegateType, binder, arg0, arg1, arg2, arg3);
			}
			return new TypedDynamicExpression4(returnType, delegateType, binder, arg0, arg1, arg2, arg3);
		}

		/// <summary>Gets the static type of the expression that this <see cref="T:System.Linq.Expressions.Expression" /> represents.</summary>
		/// <returns>The <see cref="P:System.Linq.Expressions.DynamicExpression.Type" /> that represents the static type of the expression.</returns>
		public override Type Type
		{
			get
			{
				return typeof(object);
			}
		}

		/// <summary>Returns the node type of this expression. Extension nodes should return <see cref="F:System.Linq.Expressions.ExpressionType.Extension" /> when overriding this method.</summary>
		/// <returns>The <see cref="T:System.Linq.Expressions.ExpressionType" /> of the expression.</returns>
		public sealed override ExpressionType NodeType
		{
			get
			{
				return ExpressionType.Dynamic;
			}
		}

		/// <summary>Gets the <see cref="T:System.Runtime.CompilerServices.CallSiteBinder" />, which determines the run-time behavior of the dynamic site.</summary>
		/// <returns>The <see cref="T:System.Runtime.CompilerServices.CallSiteBinder" />, which determines the run-time behavior of the dynamic site.</returns>
		public CallSiteBinder Binder { get; }

		/// <summary>Gets the type of the delegate used by the <see cref="T:System.Runtime.CompilerServices.CallSite" />.</summary>
		/// <returns>The <see cref="T:System.Type" /> object representing the type of the delegate used by the <see cref="T:System.Runtime.CompilerServices.CallSite" />.</returns>
		public Type DelegateType { get; }

		/// <summary>Gets the arguments to the dynamic operation.</summary>
		/// <returns>The read-only collections containing the arguments to the dynamic operation.</returns>
		public ReadOnlyCollection<Expression> Arguments
		{
			get
			{
				return this.GetOrMakeArguments();
			}
		}

		[ExcludeFromCodeCoverage]
		internal virtual ReadOnlyCollection<Expression> GetOrMakeArguments()
		{
			throw ContractUtils.Unreachable;
		}

		/// <summary>Dispatches to the specific visit method for this node type. For example, <see cref="T:System.Linq.Expressions.MethodCallExpression" /> calls the <see cref="M:System.Linq.Expressions.ExpressionVisitor.VisitMethodCall(System.Linq.Expressions.MethodCallExpression)" />.</summary>
		/// <param name="visitor">The visitor to visit this node with.</param>
		/// <returns>The result of visiting this node.</returns>
		protected internal override Expression Accept(ExpressionVisitor visitor)
		{
			DynamicExpressionVisitor dynamicExpressionVisitor = visitor as DynamicExpressionVisitor;
			if (dynamicExpressionVisitor != null)
			{
				return dynamicExpressionVisitor.VisitDynamic(this);
			}
			return visitor.VisitDynamic(this);
		}

		[ExcludeFromCodeCoverage]
		internal virtual DynamicExpression Rewrite(Expression[] args)
		{
			throw ContractUtils.Unreachable;
		}

		/// <summary>Compares the value sent to the parameter, arguments, to the <see langword="Arguments" /> property of the current instance of <see langword="DynamicExpression" />. If the values of the parameter and the property are equal, the current instance is returned. If they are not equal, a new <see langword="DynamicExpression" /> instance is returned that is identical to the current instance except that the <see langword="Arguments" /> property is set to the value of parameter arguments. </summary>
		/// <param name="arguments">The <see cref="P:System.Linq.Expressions.DynamicExpression.Arguments" /> property of the result.</param>
		/// <returns>This expression if no children are changed or an expression with the updated children.</returns>
		public DynamicExpression Update(IEnumerable<Expression> arguments)
		{
			ICollection<Expression> collection;
			if (arguments == null)
			{
				collection = null;
			}
			else
			{
				collection = (arguments as ICollection<Expression>);
				if (collection == null)
				{
					collection = (arguments = arguments.ToReadOnly<Expression>());
				}
			}
			if (this.SameArguments(collection))
			{
				return this;
			}
			return ExpressionExtension.MakeDynamic(this.DelegateType, this.Binder, arguments);
		}

		[ExcludeFromCodeCoverage]
		internal virtual bool SameArguments(ICollection<Expression> arguments)
		{
			throw ContractUtils.Unreachable;
		}

		/// <summary>Returns the argument at index, throwing if index is out of bounds.  You should not use this member.  It is only public due to assembly refactoring, and it is used internally for performance optimizations.</summary>
		/// <param name="index">The index of the argument.</param>
		/// <returns>Returns <see cref="T:System.Linq.Expressions.Expression" />.</returns>
		[ExcludeFromCodeCoverage]
		Expression IArgumentProvider.GetArgument(int index)
		{
			throw ContractUtils.Unreachable;
		}

		/// <summary>Returns the number of arguments to the expression tree node.  You should not use this member.  It is only public due to assembly refactoring, and it is used internally for performance optimizations.</summary>
		/// <returns>Returns <see cref="T:System.Int32" />.</returns>
		[ExcludeFromCodeCoverage]
		int IArgumentProvider.ArgumentCount
		{
			get
			{
				throw ContractUtils.Unreachable;
			}
		}

		/// <summary>Creates a <see cref="T:System.Linq.Expressions.DynamicExpression" /> that represents a dynamic operation bound by the provided <see cref="T:System.Runtime.CompilerServices.CallSiteBinder" />.</summary>
		/// <param name="binder">The runtime binder for the dynamic operation.</param>
		/// <param name="returnType">The result type of the dynamic expression.</param>
		/// <param name="arguments">The arguments to the dynamic operation.</param>
		/// <returns>A <see cref="T:System.Linq.Expressions.DynamicExpression" /> that has <see cref="P:System.Linq.Expressions.DynamicExpression.NodeType" /> equal to <see cref="F:System.Linq.Expressions.ExpressionType.Dynamic" />, and has the <see cref="P:System.Linq.Expressions.DynamicExpression.Binder" /> and <see cref="P:System.Linq.Expressions.DynamicExpression.Arguments" /> set to the specified values.</returns>
		public new static DynamicExpression Dynamic(CallSiteBinder binder, Type returnType, params Expression[] arguments)
		{
			return ExpressionExtension.Dynamic(binder, returnType, arguments);
		}

		/// <summary>Creates a <see cref="T:System.Linq.Expressions.DynamicExpression" /> that represents a dynamic operation bound by the provided <see cref="T:System.Runtime.CompilerServices.CallSiteBinder" />.</summary>
		/// <param name="binder">The runtime binder for the dynamic operation.</param>
		/// <param name="returnType">The result type of the dynamic expression.</param>
		/// <param name="arguments">The arguments to the dynamic operation.</param>
		/// <returns>A <see cref="T:System.Linq.Expressions.DynamicExpression" /> that has <see cref="P:System.Linq.Expressions.DynamicExpression.NodeType" /> equal to <see cref="F:System.Linq.Expressions.ExpressionType.Dynamic" />,  and has the <see cref="P:System.Linq.Expressions.DynamicExpression.Binder" /> and <see cref="P:System.Linq.Expressions.DynamicExpression.Arguments" /> set to the specified values.</returns>
		public new static DynamicExpression Dynamic(CallSiteBinder binder, Type returnType, IEnumerable<Expression> arguments)
		{
			return ExpressionExtension.Dynamic(binder, returnType, arguments);
		}

		/// <summary>Creates a <see cref="T:System.Linq.Expressions.DynamicExpression" /> that represents a dynamic operation bound by the provided <see cref="T:System.Runtime.CompilerServices.CallSiteBinder" />.</summary>
		/// <param name="binder">The runtime binder for the dynamic operation.</param>
		/// <param name="returnType">The result type of the dynamic expression.</param>
		/// <param name="arg0">The first argument to the dynamic operation.</param>
		/// <returns>A <see cref="T:System.Linq.Expressions.DynamicExpression" /> that has <see cref="P:System.Linq.Expressions.DynamicExpression.NodeType" /> equal to <see cref="F:System.Linq.Expressions.ExpressionType.Dynamic" />,  and has the <see cref="P:System.Linq.Expressions.DynamicExpression.Binder" /> and <see cref="P:System.Linq.Expressions.DynamicExpression.Arguments" /> set to the specified values.</returns>
		public new static DynamicExpression Dynamic(CallSiteBinder binder, Type returnType, Expression arg0)
		{
			return ExpressionExtension.Dynamic(binder, returnType, arg0);
		}

		/// <summary>Creates a <see cref="T:System.Linq.Expressions.DynamicExpression" /> that represents a dynamic operation bound by the provided <see cref="T:System.Runtime.CompilerServices.CallSiteBinder" />.</summary>
		/// <param name="binder">The runtime binder for the dynamic operation.</param>
		/// <param name="returnType">The result type of the dynamic expression.</param>
		/// <param name="arg0">The first argument to the dynamic operation.</param>
		/// <param name="arg1">The second argument to the dynamic operation.</param>
		/// <returns>A <see cref="T:System.Linq.Expressions.DynamicExpression" /> that has <see cref="P:System.Linq.Expressions.DynamicExpression.NodeType" /> equal to <see cref="F:System.Linq.Expressions.ExpressionType.Dynamic" />, and has the <see cref="P:System.Linq.Expressions.DynamicExpression.Binder" /> and <see cref="P:System.Linq.Expressions.DynamicExpression.Arguments" /> set to the specified values.</returns>
		public new static DynamicExpression Dynamic(CallSiteBinder binder, Type returnType, Expression arg0, Expression arg1)
		{
			return ExpressionExtension.Dynamic(binder, returnType, arg0, arg1);
		}

		/// <summary>Creates a <see cref="T:System.Linq.Expressions.DynamicExpression" /> that represents a dynamic operation bound by the provided <see cref="T:System.Runtime.CompilerServices.CallSiteBinder" />.</summary>
		/// <param name="binder">The runtime binder for the dynamic operation.</param>
		/// <param name="returnType">The result type of the dynamic expression.</param>
		/// <param name="arg0">The first argument to the dynamic operation.</param>
		/// <param name="arg1">The second argument to the dynamic operation.</param>
		/// <param name="arg2">The third argument to the dynamic operation.</param>
		/// <returns>A <see cref="T:System.Linq.Expressions.DynamicExpression" /> that has <see cref="P:System.Linq.Expressions.DynamicExpression.NodeType" /> equal to <see cref="F:System.Linq.Expressions.ExpressionType.Dynamic" />, and has the <see cref="P:System.Linq.Expressions.DynamicExpression.Binder" /> and <see cref="P:System.Linq.Expressions.DynamicExpression.Arguments" /> set to the specified values.</returns>
		public new static DynamicExpression Dynamic(CallSiteBinder binder, Type returnType, Expression arg0, Expression arg1, Expression arg2)
		{
			return ExpressionExtension.Dynamic(binder, returnType, arg0, arg1, arg2);
		}

		/// <summary>Creates a <see cref="T:System.Linq.Expressions.DynamicExpression" /> that represents a dynamic operation bound by the provided <see cref="T:System.Runtime.CompilerServices.CallSiteBinder" />.</summary>
		/// <param name="binder">The runtime binder for the dynamic operation.</param>
		/// <param name="returnType">The result type of the dynamic expression.</param>
		/// <param name="arg0">The first argument to the dynamic operation.</param>
		/// <param name="arg1">The second argument to the dynamic operation.</param>
		/// <param name="arg2">The third argument to the dynamic operation.</param>
		/// <param name="arg3">The fourth argument to the dynamic operation.</param>
		/// <returns>A <see cref="T:System.Linq.Expressions.DynamicExpression" /> that has <see cref="P:System.Linq.Expressions.DynamicExpression.NodeType" /> equal to <see cref="F:System.Linq.Expressions.ExpressionType.Dynamic" />, and has the <see cref="P:System.Linq.Expressions.DynamicExpression.Binder" /> and <see cref="P:System.Linq.Expressions.DynamicExpression.Arguments" /> set to the specified values.</returns>
		public new static DynamicExpression Dynamic(CallSiteBinder binder, Type returnType, Expression arg0, Expression arg1, Expression arg2, Expression arg3)
		{
			return ExpressionExtension.Dynamic(binder, returnType, arg0, arg1, arg2, arg3);
		}

		/// <summary>Creates a <see cref="T:System.Linq.Expressions.DynamicExpression" /> that represents a dynamic operation bound by the provided <see cref="T:System.Runtime.CompilerServices.CallSiteBinder" />.</summary>
		/// <param name="delegateType">The type of the delegate used by the <see cref="T:System.Runtime.CompilerServices.CallSite" />.</param>
		/// <param name="binder">The runtime binder for the dynamic operation.</param>
		/// <param name="arguments">The arguments to the dynamic operation.</param>
		/// <returns>A <see cref="T:System.Linq.Expressions.DynamicExpression" /> that has <see cref="P:System.Linq.Expressions.DynamicExpression.NodeType" /> equal to <see cref="F:System.Linq.Expressions.ExpressionType.Dynamic" />, and has the <see cref="P:System.Linq.Expressions.DynamicExpression.DelegateType" />, <see cref="P:System.Linq.Expressions.DynamicExpression.Binder" />, and <see cref="P:System.Linq.Expressions.DynamicExpression.Arguments" /> set to the specified values.</returns>
		public new static DynamicExpression MakeDynamic(Type delegateType, CallSiteBinder binder, IEnumerable<Expression> arguments)
		{
			return ExpressionExtension.MakeDynamic(delegateType, binder, arguments);
		}

		/// <summary>Creates a <see cref="T:System.Linq.Expressions.DynamicExpression" /> that represents a dynamic operation bound by the provided <see cref="T:System.Runtime.CompilerServices.CallSiteBinder" />.</summary>
		/// <param name="delegateType">The type of the delegate used by the <see cref="T:System.Runtime.CompilerServices.CallSite" />.</param>
		/// <param name="binder">The runtime binder for the dynamic operation.</param>
		/// <param name="arguments">The arguments to the dynamic operation.</param>
		/// <returns>A <see cref="T:System.Linq.Expressions.DynamicExpression" /> that has <see cref="P:System.Linq.Expressions.DynamicExpression.NodeType" /> equal to <see cref="F:System.Linq.Expressions.ExpressionType.Dynamic" />, and has the <see cref="P:System.Linq.Expressions.DynamicExpression.DelegateType" />, <see cref="P:System.Linq.Expressions.DynamicExpression.Binder" />, and <see cref="P:System.Linq.Expressions.DynamicExpression.Arguments" /> set to the specified values.</returns>
		public new static DynamicExpression MakeDynamic(Type delegateType, CallSiteBinder binder, params Expression[] arguments)
		{
			return ExpressionExtension.MakeDynamic(delegateType, binder, arguments);
		}

		/// <summary>Creates a <see cref="T:System.Linq.Expressions.DynamicExpression" /> that represents a dynamic operation bound by the provided <see cref="T:System.Runtime.CompilerServices.CallSiteBinder" /> and one argument.</summary>
		/// <param name="delegateType">The type of the delegate used by the <see cref="T:System.Runtime.CompilerServices.CallSite" />.</param>
		/// <param name="binder">The runtime binder for the dynamic operation.</param>
		/// <param name="arg0">The argument to the dynamic operation.</param>
		/// <returns>A <see cref="T:System.Linq.Expressions.DynamicExpression" /> that has <see cref="P:System.Linq.Expressions.DynamicExpression.NodeType" /> equal to <see cref="F:System.Linq.Expressions.ExpressionType.Dynamic" />, and has the <see cref="P:System.Linq.Expressions.DynamicExpression.DelegateType" />, <see cref="P:System.Linq.Expressions.DynamicExpression.Binder" />, and <see cref="P:System.Linq.Expressions.DynamicExpression.Arguments" /> set to the specified values.</returns>
		public new static DynamicExpression MakeDynamic(Type delegateType, CallSiteBinder binder, Expression arg0)
		{
			return ExpressionExtension.MakeDynamic(delegateType, binder, arg0);
		}

		/// <summary>Creates a <see cref="T:System.Linq.Expressions.DynamicExpression" /> that represents a dynamic operation bound by the provided <see cref="T:System.Runtime.CompilerServices.CallSiteBinder" /> and two arguments.</summary>
		/// <param name="delegateType">The type of the delegate used by the <see cref="T:System.Runtime.CompilerServices.CallSite" />.</param>
		/// <param name="binder">The runtime binder for the dynamic operation.</param>
		/// <param name="arg0">The first argument to the dynamic operation.</param>
		/// <param name="arg1">The second argument to the dynamic operation.</param>
		/// <returns>A <see cref="T:System.Linq.Expressions.DynamicExpression" /> that has <see cref="P:System.Linq.Expressions.DynamicExpression.NodeType" /> equal to <see cref="F:System.Linq.Expressions.ExpressionType.Dynamic" />, and has the <see cref="P:System.Linq.Expressions.DynamicExpression.DelegateType" />, <see cref="P:System.Linq.Expressions.DynamicExpression.Binder" />, and <see cref="P:System.Linq.Expressions.DynamicExpression.Arguments" /> set to the specified values.</returns>
		public new static DynamicExpression MakeDynamic(Type delegateType, CallSiteBinder binder, Expression arg0, Expression arg1)
		{
			return ExpressionExtension.MakeDynamic(delegateType, binder, arg0, arg1);
		}

		/// <summary>Creates a <see cref="T:System.Linq.Expressions.DynamicExpression" /> that represents a dynamic operation bound by the provided <see cref="T:System.Runtime.CompilerServices.CallSiteBinder" /> and three arguments.</summary>
		/// <param name="delegateType">The type of the delegate used by the <see cref="T:System.Runtime.CompilerServices.CallSite" />.</param>
		/// <param name="binder">The runtime binder for the dynamic operation.</param>
		/// <param name="arg0">The first argument to the dynamic operation.</param>
		/// <param name="arg1">The second argument to the dynamic operation.</param>
		/// <param name="arg2">The third argument to the dynamic operation.</param>
		/// <returns>A <see cref="T:System.Linq.Expressions.DynamicExpression" /> that has <see cref="P:System.Linq.Expressions.DynamicExpression.NodeType" /> equal to <see cref="F:System.Linq.Expressions.ExpressionType.Dynamic" />, and has the <see cref="P:System.Linq.Expressions.DynamicExpression.DelegateType" />, <see cref="P:System.Linq.Expressions.DynamicExpression.Binder" />, and <see cref="P:System.Linq.Expressions.DynamicExpression.Arguments" /> set to the specified values.</returns>
		public new static DynamicExpression MakeDynamic(Type delegateType, CallSiteBinder binder, Expression arg0, Expression arg1, Expression arg2)
		{
			return ExpressionExtension.MakeDynamic(delegateType, binder, arg0, arg1, arg2);
		}

		/// <summary>Creates a <see cref="T:System.Linq.Expressions.DynamicExpression" /> that represents a dynamic operation bound by the provided <see cref="T:System.Runtime.CompilerServices.CallSiteBinder" /> and four arguments.</summary>
		/// <param name="delegateType">The type of the delegate used by the <see cref="T:System.Runtime.CompilerServices.CallSite" />.</param>
		/// <param name="binder">The runtime binder for the dynamic operation.</param>
		/// <param name="arg0">The first argument to the dynamic operation.</param>
		/// <param name="arg1">The second argument to the dynamic operation.</param>
		/// <param name="arg2">The third argument to the dynamic operation.</param>
		/// <param name="arg3">The fourth argument to the dynamic operation.</param>
		/// <returns>A <see cref="T:System.Linq.Expressions.DynamicExpression" /> that has <see cref="P:System.Linq.Expressions.DynamicExpression.NodeType" /> equal to <see cref="F:System.Linq.Expressions.ExpressionType.Dynamic" />, and has the <see cref="P:System.Linq.Expressions.DynamicExpression.DelegateType" />, <see cref="P:System.Linq.Expressions.DynamicExpression.Binder" />, and <see cref="P:System.Linq.Expressions.DynamicExpression.Arguments" /> set to the specified values.</returns>
		public new static DynamicExpression MakeDynamic(Type delegateType, CallSiteBinder binder, Expression arg0, Expression arg1, Expression arg2, Expression arg3)
		{
			return ExpressionExtension.MakeDynamic(delegateType, binder, arg0, arg1, arg2, arg3);
		}

		/// <summary>Rewrite this node replacing the dynamic expression’s arguments with the provided values.  The number of args needs to match the number of the current expression.  You should not use this type.  It is only public due to assembly refactoring, and it is used internally for performance optimizations.  This helper method allows re-writing of nodes to be independent of the specific implementation class deriving from DynamicExpression that is being used at the call site.</summary>
		/// <param name="args">The arguments.</param>
		/// <returns>Returns <see cref="T:System.Linq.Expressions.Expression" />, the rewritten expression.</returns>
		Expression IDynamicExpression.Rewrite(Expression[] args)
		{
			return this.Rewrite(args);
		}

		/// <summary>Optionally creates the CallSite and returns the CallSite for the DynamicExpression’s polymorphic inline cache.  You should not use this member.  It is only public due to assembly refactoring, and it is used internally for performance optimizations.</summary>
		/// <returns>Returns <see cref="T:System.Object" />.</returns>
		object IDynamicExpression.CreateCallSite()
		{
			return CallSite.Create(this.DelegateType, this.Binder);
		}

		internal DynamicExpression()
		{
			ThrowStub.ThrowNotSupportedException();
		}
	}
}
