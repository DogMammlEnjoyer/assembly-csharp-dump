using System;
using System.Collections.ObjectModel;
using System.Dynamic.Utils;
using System.Runtime.CompilerServices;

namespace System.Linq.Expressions
{
	/// <summary>Represents a visitor or rewriter for expression trees.</summary>
	public abstract class ExpressionVisitor
	{
		/// <summary>Dispatches the expression to one of the more specialized visit methods in this class.</summary>
		/// <param name="node">The expression to visit.</param>
		/// <returns>The modified expression, if it or any subexpression was modified; otherwise, returns the original expression.</returns>
		public virtual Expression Visit(Expression node)
		{
			if (node == null)
			{
				return null;
			}
			return node.Accept(this);
		}

		/// <summary>Dispatches the list of expressions to one of the more specialized visit methods in this class.</summary>
		/// <param name="nodes">The expressions to visit.</param>
		/// <returns>The modified expression list, if any one of the elements were modified; otherwise, returns the original expression list.</returns>
		public ReadOnlyCollection<Expression> Visit(ReadOnlyCollection<Expression> nodes)
		{
			ContractUtils.RequiresNotNull(nodes, "nodes");
			Expression[] array = null;
			int i = 0;
			int count = nodes.Count;
			while (i < count)
			{
				Expression expression = this.Visit(nodes[i]);
				if (array != null)
				{
					array[i] = expression;
				}
				else if (expression != nodes[i])
				{
					array = new Expression[count];
					for (int j = 0; j < i; j++)
					{
						array[j] = nodes[j];
					}
					array[i] = expression;
				}
				i++;
			}
			if (array == null)
			{
				return nodes;
			}
			return new TrueReadOnlyCollection<Expression>(array);
		}

		private Expression[] VisitArguments(IArgumentProvider nodes)
		{
			return ExpressionVisitorUtils.VisitArguments(this, nodes);
		}

		private ParameterExpression[] VisitParameters(IParameterProvider nodes, string callerName)
		{
			return ExpressionVisitorUtils.VisitParameters(this, nodes, callerName);
		}

		/// <summary>Visits all nodes in the collection using a specified element visitor.</summary>
		/// <param name="nodes">The nodes to visit.</param>
		/// <param name="elementVisitor">A delegate that visits a single element, optionally replacing it with a new element.</param>
		/// <typeparam name="T">The type of the nodes.</typeparam>
		/// <returns>The modified node list, if any of the elements were modified; otherwise, returns the original node list.</returns>
		public static ReadOnlyCollection<T> Visit<T>(ReadOnlyCollection<T> nodes, Func<T, T> elementVisitor)
		{
			ContractUtils.RequiresNotNull(nodes, "nodes");
			ContractUtils.RequiresNotNull(elementVisitor, "elementVisitor");
			T[] array = null;
			int i = 0;
			int count = nodes.Count;
			while (i < count)
			{
				T t = elementVisitor(nodes[i]);
				if (array != null)
				{
					array[i] = t;
				}
				else if (t != nodes[i])
				{
					array = new T[count];
					for (int j = 0; j < i; j++)
					{
						array[j] = nodes[j];
					}
					array[i] = t;
				}
				i++;
			}
			if (array == null)
			{
				return nodes;
			}
			return new TrueReadOnlyCollection<T>(array);
		}

		/// <summary>Visits an expression, casting the result back to the original expression type.</summary>
		/// <param name="node">The expression to visit.</param>
		/// <param name="callerName">The name of the calling method; used to report to report a better error message.</param>
		/// <typeparam name="T">The type of the expression.</typeparam>
		/// <returns>The modified expression, if it or any subexpression was modified; otherwise, returns the original expression.</returns>
		/// <exception cref="T:System.InvalidOperationException">The visit method for this node returned a different type.</exception>
		public T VisitAndConvert<T>(T node, string callerName) where T : Expression
		{
			if (node == null)
			{
				return default(T);
			}
			node = (this.Visit(node) as T);
			if (node == null)
			{
				throw Error.MustRewriteToSameNode(callerName, typeof(T), callerName);
			}
			return node;
		}

		/// <summary>Visits an expression, casting the result back to the original expression type.</summary>
		/// <param name="nodes">The expression to visit.</param>
		/// <param name="callerName">The name of the calling method; used to report to report a better error message.</param>
		/// <typeparam name="T">The type of the expression.</typeparam>
		/// <returns>The modified expression, if it or any subexpression was modified; otherwise, returns the original expression.</returns>
		/// <exception cref="T:System.InvalidOperationException">The visit method for this node returned a different type.</exception>
		public ReadOnlyCollection<T> VisitAndConvert<T>(ReadOnlyCollection<T> nodes, string callerName) where T : Expression
		{
			ContractUtils.RequiresNotNull(nodes, "nodes");
			T[] array = null;
			int i = 0;
			int count = nodes.Count;
			while (i < count)
			{
				T t = this.Visit(nodes[i]) as T;
				if (t == null)
				{
					throw Error.MustRewriteToSameNode(callerName, typeof(T), callerName);
				}
				if (array != null)
				{
					array[i] = t;
				}
				else if (t != nodes[i])
				{
					array = new T[count];
					for (int j = 0; j < i; j++)
					{
						array[j] = nodes[j];
					}
					array[i] = t;
				}
				i++;
			}
			if (array == null)
			{
				return nodes;
			}
			return new TrueReadOnlyCollection<T>(array);
		}

		/// <summary>Visits the children of the <see cref="T:System.Linq.Expressions.BinaryExpression" />.</summary>
		/// <param name="node">The expression to visit.</param>
		/// <returns>The modified expression, if it or any subexpression was modified; otherwise, returns the original expression.</returns>
		protected internal virtual Expression VisitBinary(BinaryExpression node)
		{
			return ExpressionVisitor.ValidateBinary(node, node.Update(this.Visit(node.Left), this.VisitAndConvert<LambdaExpression>(node.Conversion, "VisitBinary"), this.Visit(node.Right)));
		}

		/// <summary>Visits the children of the <see cref="T:System.Linq.Expressions.BlockExpression" />.</summary>
		/// <param name="node">The expression to visit.</param>
		/// <returns>The modified expression, if it or any subexpression was modified; otherwise, returns the original expression.</returns>
		protected internal virtual Expression VisitBlock(BlockExpression node)
		{
			Expression[] array = ExpressionVisitorUtils.VisitBlockExpressions(this, node);
			ReadOnlyCollection<ParameterExpression> readOnlyCollection = this.VisitAndConvert<ParameterExpression>(node.Variables, "VisitBlock");
			if (readOnlyCollection == node.Variables && array == null)
			{
				return node;
			}
			return node.Rewrite(readOnlyCollection, array);
		}

		/// <summary>Visits the children of the <see cref="T:System.Linq.Expressions.ConditionalExpression" />.</summary>
		/// <param name="node">The expression to visit.</param>
		/// <returns>The modified expression, if it or any subexpression was modified; otherwise, returns the original expression.</returns>
		protected internal virtual Expression VisitConditional(ConditionalExpression node)
		{
			return node.Update(this.Visit(node.Test), this.Visit(node.IfTrue), this.Visit(node.IfFalse));
		}

		/// <summary>Visits the <see cref="T:System.Linq.Expressions.ConstantExpression" />.</summary>
		/// <param name="node">The expression to visit.</param>
		/// <returns>The modified expression, if it or any subexpression was modified; otherwise, returns the original expression.</returns>
		protected internal virtual Expression VisitConstant(ConstantExpression node)
		{
			return node;
		}

		/// <summary>Visits the <see cref="T:System.Linq.Expressions.DebugInfoExpression" />.</summary>
		/// <param name="node">The expression to visit.</param>
		/// <returns>The modified expression, if it or any subexpression was modified; otherwise, returns the original expression.</returns>
		protected internal virtual Expression VisitDebugInfo(DebugInfoExpression node)
		{
			return node;
		}

		/// <summary>Visits the <see cref="T:System.Linq.Expressions.DefaultExpression" />.</summary>
		/// <param name="node">The expression to visit.</param>
		/// <returns>The modified expression, if it or any subexpression was modified; otherwise, returns the original expression.</returns>
		protected internal virtual Expression VisitDefault(DefaultExpression node)
		{
			return node;
		}

		/// <summary>Visits the children of the extension expression.</summary>
		/// <param name="node">The expression to visit.</param>
		/// <returns>The modified expression, if it or any subexpression was modified; otherwise, returns the original expression.</returns>
		protected internal virtual Expression VisitExtension(Expression node)
		{
			return node.VisitChildren(this);
		}

		/// <summary>Visits the children of the <see cref="T:System.Linq.Expressions.GotoExpression" />.</summary>
		/// <param name="node">The expression to visit.</param>
		/// <returns>The modified expression, if it or any subexpression was modified; otherwise, returns the original expression.</returns>
		protected internal virtual Expression VisitGoto(GotoExpression node)
		{
			return node.Update(this.VisitLabelTarget(node.Target), this.Visit(node.Value));
		}

		/// <summary>Visits the children of the <see cref="T:System.Linq.Expressions.InvocationExpression" />.</summary>
		/// <param name="node">The expression to visit.</param>
		/// <returns>The modified expression, if it or any subexpression was modified; otherwise, returns the original expression.</returns>
		protected internal virtual Expression VisitInvocation(InvocationExpression node)
		{
			Expression expression = this.Visit(node.Expression);
			Expression[] array = this.VisitArguments(node);
			if (expression == node.Expression && array == null)
			{
				return node;
			}
			return node.Rewrite(expression, array);
		}

		/// <summary>Visits the <see cref="T:System.Linq.Expressions.LabelTarget" />.</summary>
		/// <param name="node">The expression to visit.</param>
		/// <returns>The modified expression, if it or any subexpression was modified; otherwise, returns the original expression.</returns>
		protected virtual LabelTarget VisitLabelTarget(LabelTarget node)
		{
			return node;
		}

		/// <summary>Visits the children of the <see cref="T:System.Linq.Expressions.LabelExpression" />.</summary>
		/// <param name="node">The expression to visit.</param>
		/// <returns>The modified expression, if it or any subexpression was modified; otherwise, returns the original expression.</returns>
		protected internal virtual Expression VisitLabel(LabelExpression node)
		{
			return node.Update(this.VisitLabelTarget(node.Target), this.Visit(node.DefaultValue));
		}

		/// <summary>Visits the children of the <see cref="T:System.Linq.Expressions.Expression`1" />.</summary>
		/// <param name="node">The expression to visit.</param>
		/// <typeparam name="T">The type of the delegate.</typeparam>
		/// <returns>The modified expression, if it or any subexpression was modified; otherwise, returns the original expression.</returns>
		protected internal virtual Expression VisitLambda<T>(Expression<T> node)
		{
			Expression expression = this.Visit(node.Body);
			ParameterExpression[] array = this.VisitParameters(node, "VisitLambda");
			if (expression == node.Body && array == null)
			{
				return node;
			}
			return node.Rewrite(expression, array);
		}

		/// <summary>Visits the children of the <see cref="T:System.Linq.Expressions.LoopExpression" />.</summary>
		/// <param name="node">The expression to visit.</param>
		/// <returns>The modified expression, if it or any subexpression was modified; otherwise, returns the original expression.</returns>
		protected internal virtual Expression VisitLoop(LoopExpression node)
		{
			return node.Update(this.VisitLabelTarget(node.BreakLabel), this.VisitLabelTarget(node.ContinueLabel), this.Visit(node.Body));
		}

		/// <summary>Visits the children of the <see cref="T:System.Linq.Expressions.MemberExpression" />.</summary>
		/// <param name="node">The expression to visit.</param>
		/// <returns>The modified expression, if it or any subexpression was modified; otherwise, returns the original expression.</returns>
		protected internal virtual Expression VisitMember(MemberExpression node)
		{
			return node.Update(this.Visit(node.Expression));
		}

		/// <summary>Visits the children of the <see cref="T:System.Linq.Expressions.IndexExpression" />.</summary>
		/// <param name="node">The expression to visit.</param>
		/// <returns>The modified expression, if it or any subexpression was modified; otherwise, returns the original expression.</returns>
		protected internal virtual Expression VisitIndex(IndexExpression node)
		{
			Expression expression = this.Visit(node.Object);
			Expression[] array = this.VisitArguments(node);
			if (expression == node.Object && array == null)
			{
				return node;
			}
			return node.Rewrite(expression, array);
		}

		/// <summary>Visits the children of the <see cref="T:System.Linq.Expressions.MethodCallExpression" />.</summary>
		/// <param name="node">The expression to visit.</param>
		/// <returns>The modified expression, if it or any subexpression was modified; otherwise, returns the original expression.</returns>
		protected internal virtual Expression VisitMethodCall(MethodCallExpression node)
		{
			Expression expression = this.Visit(node.Object);
			Expression[] array = this.VisitArguments(node);
			if (expression == node.Object && array == null)
			{
				return node;
			}
			return node.Rewrite(expression, array);
		}

		/// <summary>Visits the children of the <see cref="T:System.Linq.Expressions.NewArrayExpression" />.</summary>
		/// <param name="node">The expression to visit.</param>
		/// <returns>The modified expression, if it or any subexpression was modified; otherwise, returns the original expression.</returns>
		protected internal virtual Expression VisitNewArray(NewArrayExpression node)
		{
			return node.Update(this.Visit(node.Expressions));
		}

		/// <summary>Visits the children of the <see cref="T:System.Linq.Expressions.NewExpression" />.</summary>
		/// <param name="node">The expression to visit.</param>
		/// <returns>The modified expression, if it or any subexpression was modified; otherwise, returns the original expression.</returns>
		protected internal virtual Expression VisitNew(NewExpression node)
		{
			Expression[] array = this.VisitArguments(node);
			if (array == null)
			{
				return node;
			}
			return node.Update(array);
		}

		/// <summary>Visits the <see cref="T:System.Linq.Expressions.ParameterExpression" />.</summary>
		/// <param name="node">The expression to visit.</param>
		/// <returns>The modified expression, if it or any subexpression was modified; otherwise, returns the original expression.</returns>
		protected internal virtual Expression VisitParameter(ParameterExpression node)
		{
			return node;
		}

		/// <summary>Visits the children of the <see cref="T:System.Linq.Expressions.RuntimeVariablesExpression" />.</summary>
		/// <param name="node">The expression to visit.</param>
		/// <returns>The modified expression, if it or any subexpression was modified; otherwise, returns the original expression.</returns>
		protected internal virtual Expression VisitRuntimeVariables(RuntimeVariablesExpression node)
		{
			return node.Update(this.VisitAndConvert<ParameterExpression>(node.Variables, "VisitRuntimeVariables"));
		}

		/// <summary>Visits the children of the <see cref="T:System.Linq.Expressions.SwitchCase" />.</summary>
		/// <param name="node">The expression to visit.</param>
		/// <returns>The modified expression, if it or any subexpression was modified; otherwise, returns the original expression.</returns>
		protected virtual SwitchCase VisitSwitchCase(SwitchCase node)
		{
			return node.Update(this.Visit(node.TestValues), this.Visit(node.Body));
		}

		/// <summary>Visits the children of the <see cref="T:System.Linq.Expressions.SwitchExpression" />.</summary>
		/// <param name="node">The expression to visit.</param>
		/// <returns>The modified expression, if it or any subexpression was modified; otherwise, returns the original expression.</returns>
		protected internal virtual Expression VisitSwitch(SwitchExpression node)
		{
			return ExpressionVisitor.ValidateSwitch(node, node.Update(this.Visit(node.SwitchValue), ExpressionVisitor.Visit<SwitchCase>(node.Cases, new Func<SwitchCase, SwitchCase>(this.VisitSwitchCase)), this.Visit(node.DefaultBody)));
		}

		/// <summary>Visits the children of the <see cref="T:System.Linq.Expressions.CatchBlock" />.</summary>
		/// <param name="node">The expression to visit.</param>
		/// <returns>The modified expression, if it or any subexpression was modified; otherwise, returns the original expression.</returns>
		protected virtual CatchBlock VisitCatchBlock(CatchBlock node)
		{
			return node.Update(this.VisitAndConvert<ParameterExpression>(node.Variable, "VisitCatchBlock"), this.Visit(node.Filter), this.Visit(node.Body));
		}

		/// <summary>Visits the children of the <see cref="T:System.Linq.Expressions.TryExpression" />.</summary>
		/// <param name="node">The expression to visit.</param>
		/// <returns>The modified expression, if it or any subexpression was modified; otherwise, returns the original expression.</returns>
		protected internal virtual Expression VisitTry(TryExpression node)
		{
			return node.Update(this.Visit(node.Body), ExpressionVisitor.Visit<CatchBlock>(node.Handlers, new Func<CatchBlock, CatchBlock>(this.VisitCatchBlock)), this.Visit(node.Finally), this.Visit(node.Fault));
		}

		/// <summary>Visits the children of the <see cref="T:System.Linq.Expressions.TypeBinaryExpression" />.</summary>
		/// <param name="node">The expression to visit.</param>
		/// <returns>The modified expression, if it or any subexpression was modified; otherwise, returns the original expression.</returns>
		protected internal virtual Expression VisitTypeBinary(TypeBinaryExpression node)
		{
			return node.Update(this.Visit(node.Expression));
		}

		/// <summary>Visits the children of the <see cref="T:System.Linq.Expressions.UnaryExpression" />.</summary>
		/// <param name="node">The expression to visit.</param>
		/// <returns>The modified expression, if it or any subexpression was modified; otherwise, returns the original expression.</returns>
		protected internal virtual Expression VisitUnary(UnaryExpression node)
		{
			return ExpressionVisitor.ValidateUnary(node, node.Update(this.Visit(node.Operand)));
		}

		/// <summary>Visits the children of the <see cref="T:System.Linq.Expressions.MemberInitExpression" />.</summary>
		/// <param name="node">The expression to visit.</param>
		/// <returns>The modified expression, if it or any subexpression was modified; otherwise, returns the original expression.</returns>
		protected internal virtual Expression VisitMemberInit(MemberInitExpression node)
		{
			return node.Update(this.VisitAndConvert<NewExpression>(node.NewExpression, "VisitMemberInit"), ExpressionVisitor.Visit<MemberBinding>(node.Bindings, new Func<MemberBinding, MemberBinding>(this.VisitMemberBinding)));
		}

		/// <summary>Visits the children of the <see cref="T:System.Linq.Expressions.ListInitExpression" />.</summary>
		/// <param name="node">The expression to visit.</param>
		/// <returns>The modified expression, if it or any subexpression was modified; otherwise, returns the original expression.</returns>
		protected internal virtual Expression VisitListInit(ListInitExpression node)
		{
			return node.Update(this.VisitAndConvert<NewExpression>(node.NewExpression, "VisitListInit"), ExpressionVisitor.Visit<ElementInit>(node.Initializers, new Func<ElementInit, ElementInit>(this.VisitElementInit)));
		}

		/// <summary>Visits the children of the <see cref="T:System.Linq.Expressions.ElementInit" />.</summary>
		/// <param name="node">The expression to visit.</param>
		/// <returns>The modified expression, if it or any subexpression was modified; otherwise, returns the original expression.</returns>
		protected virtual ElementInit VisitElementInit(ElementInit node)
		{
			return node.Update(this.Visit(node.Arguments));
		}

		/// <summary>Visits the children of the <see cref="T:System.Linq.Expressions.MemberBinding" />.</summary>
		/// <param name="node">The expression to visit.</param>
		/// <returns>The modified expression, if it or any subexpression was modified; otherwise, returns the original expression.</returns>
		protected virtual MemberBinding VisitMemberBinding(MemberBinding node)
		{
			switch (node.BindingType)
			{
			case MemberBindingType.Assignment:
				return this.VisitMemberAssignment((MemberAssignment)node);
			case MemberBindingType.MemberBinding:
				return this.VisitMemberMemberBinding((MemberMemberBinding)node);
			case MemberBindingType.ListBinding:
				return this.VisitMemberListBinding((MemberListBinding)node);
			default:
				throw Error.UnhandledBindingType(node.BindingType);
			}
		}

		/// <summary>Visits the children of the <see cref="T:System.Linq.Expressions.MemberAssignment" />.</summary>
		/// <param name="node">The expression to visit.</param>
		/// <returns>The modified expression, if it or any subexpression was modified; otherwise, returns the original expression.</returns>
		protected virtual MemberAssignment VisitMemberAssignment(MemberAssignment node)
		{
			return node.Update(this.Visit(node.Expression));
		}

		/// <summary>Visits the children of the <see cref="T:System.Linq.Expressions.MemberMemberBinding" />.</summary>
		/// <param name="node">The expression to visit.</param>
		/// <returns>The modified expression, if it or any subexpression was modified; otherwise, returns the original expression.</returns>
		protected virtual MemberMemberBinding VisitMemberMemberBinding(MemberMemberBinding node)
		{
			return node.Update(ExpressionVisitor.Visit<MemberBinding>(node.Bindings, new Func<MemberBinding, MemberBinding>(this.VisitMemberBinding)));
		}

		/// <summary>Visits the children of the <see cref="T:System.Linq.Expressions.MemberListBinding" />.</summary>
		/// <param name="node">The expression to visit.</param>
		/// <returns>The modified expression, if it or any subexpression was modified; otherwise, returns the original expression.</returns>
		protected virtual MemberListBinding VisitMemberListBinding(MemberListBinding node)
		{
			return node.Update(ExpressionVisitor.Visit<ElementInit>(node.Initializers, new Func<ElementInit, ElementInit>(this.VisitElementInit)));
		}

		private static UnaryExpression ValidateUnary(UnaryExpression before, UnaryExpression after)
		{
			if (before != after && before.Method == null)
			{
				if (after.Method != null)
				{
					throw Error.MustRewriteWithoutMethod(after.Method, "VisitUnary");
				}
				if (before.Operand != null && after.Operand != null)
				{
					ExpressionVisitor.ValidateChildType(before.Operand.Type, after.Operand.Type, "VisitUnary");
				}
			}
			return after;
		}

		private static BinaryExpression ValidateBinary(BinaryExpression before, BinaryExpression after)
		{
			if (before != after && before.Method == null)
			{
				if (after.Method != null)
				{
					throw Error.MustRewriteWithoutMethod(after.Method, "VisitBinary");
				}
				ExpressionVisitor.ValidateChildType(before.Left.Type, after.Left.Type, "VisitBinary");
				ExpressionVisitor.ValidateChildType(before.Right.Type, after.Right.Type, "VisitBinary");
			}
			return after;
		}

		private static SwitchExpression ValidateSwitch(SwitchExpression before, SwitchExpression after)
		{
			if (before.Comparison == null && after.Comparison != null)
			{
				throw Error.MustRewriteWithoutMethod(after.Comparison, "VisitSwitch");
			}
			return after;
		}

		private static void ValidateChildType(Type before, Type after, string methodName)
		{
			if (before.IsValueType)
			{
				if (TypeUtils.AreEquivalent(before, after))
				{
					return;
				}
			}
			else if (!after.IsValueType)
			{
				return;
			}
			throw Error.MustRewriteChildToSameType(before, after, methodName);
		}

		/// <summary>Visits the children of the <see cref="T:System.Linq.Expressions.DynamicExpression" />.</summary>
		/// <param name="node">The expression to visit.</param>
		/// <returns>The modified expression, if it or any subexpression was modified; otherwise, returns the original expression.</returns>
		protected internal virtual Expression VisitDynamic(DynamicExpression node)
		{
			Expression[] array = this.VisitArguments(node);
			if (array == null)
			{
				return node;
			}
			return node.Rewrite(array);
		}
	}
}
