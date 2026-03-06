using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Dynamic.Utils;
using System.Reflection;
using Unity;

namespace System.Linq.Expressions
{
	/// <summary>Represents a control expression that handles multiple selections by passing control to <see cref="T:System.Linq.Expressions.SwitchCase" />.</summary>
	[DebuggerTypeProxy(typeof(Expression.SwitchExpressionProxy))]
	public sealed class SwitchExpression : Expression
	{
		internal SwitchExpression(Type type, Expression switchValue, Expression defaultBody, MethodInfo comparison, ReadOnlyCollection<SwitchCase> cases)
		{
			this.Type = type;
			this.SwitchValue = switchValue;
			this.DefaultBody = defaultBody;
			this.Comparison = comparison;
			this.Cases = cases;
		}

		/// <summary>Gets the static type of the expression that this <see cref="T:System.Linq.Expressions.Expression" /> represents.</summary>
		/// <returns>The <see cref="P:System.Linq.Expressions.SwitchExpression.Type" /> that represents the static type of the expression.</returns>
		public sealed override Type Type { get; }

		/// <summary>Returns the node type of this Expression. Extension nodes should return <see cref="F:System.Linq.Expressions.ExpressionType.Extension" /> when overriding this method.</summary>
		/// <returns>The <see cref="T:System.Linq.Expressions.ExpressionType" /> of the expression.</returns>
		public sealed override ExpressionType NodeType
		{
			get
			{
				return ExpressionType.Switch;
			}
		}

		/// <summary>Gets the test for the switch.</summary>
		/// <returns>The <see cref="T:System.Linq.Expressions.Expression" /> object representing the test for the switch.</returns>
		public Expression SwitchValue { get; }

		/// <summary>Gets the collection of <see cref="T:System.Linq.Expressions.SwitchCase" /> objects for the switch.</summary>
		/// <returns>The collection of <see cref="T:System.Linq.Expressions.SwitchCase" /> objects.</returns>
		public ReadOnlyCollection<SwitchCase> Cases { get; }

		/// <summary>Gets the test for the switch.</summary>
		/// <returns>The <see cref="T:System.Linq.Expressions.Expression" /> object representing the test for the switch.</returns>
		public Expression DefaultBody { get; }

		/// <summary>Gets the equality comparison method, if any.</summary>
		/// <returns>The <see cref="T:System.Reflection.MethodInfo" /> object representing the equality comparison method.</returns>
		public MethodInfo Comparison { get; }

		protected internal override Expression Accept(ExpressionVisitor visitor)
		{
			return visitor.VisitSwitch(this);
		}

		internal bool IsLifted
		{
			get
			{
				return this.SwitchValue.Type.IsNullableType() && (this.Comparison == null || !TypeUtils.AreEquivalent(this.SwitchValue.Type, this.Comparison.GetParametersCached()[0].ParameterType.GetNonRefType()));
			}
		}

		/// <summary>Creates a new expression that is like this one, but using the supplied children. If all of the children are the same, it will return this expression.</summary>
		/// <param name="switchValue">The <see cref="P:System.Linq.Expressions.SwitchExpression.SwitchValue" /> property of the result.</param>
		/// <param name="cases">The <see cref="P:System.Linq.Expressions.SwitchExpression.Cases" /> property of the result.</param>
		/// <param name="defaultBody">The <see cref="P:System.Linq.Expressions.SwitchExpression.DefaultBody" /> property of the result.</param>
		/// <returns>This expression if no children are changed or an expression with the updated children.</returns>
		public SwitchExpression Update(Expression switchValue, IEnumerable<SwitchCase> cases, Expression defaultBody)
		{
			if ((switchValue == this.SwitchValue & defaultBody == this.DefaultBody & cases != null) && ExpressionUtils.SameElements<SwitchCase>(ref cases, this.Cases))
			{
				return this;
			}
			return Expression.Switch(this.Type, switchValue, defaultBody, this.Comparison, cases);
		}

		internal SwitchExpression()
		{
			ThrowStub.ThrowNotSupportedException();
		}
	}
}
