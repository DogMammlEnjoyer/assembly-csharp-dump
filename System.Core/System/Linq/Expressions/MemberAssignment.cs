using System;
using System.Reflection;
using Unity;

namespace System.Linq.Expressions
{
	/// <summary>Represents assignment operation for a field or property of an object.</summary>
	public sealed class MemberAssignment : MemberBinding
	{
		internal MemberAssignment(MemberInfo member, Expression expression) : base(MemberBindingType.Assignment, member)
		{
			this._expression = expression;
		}

		/// <summary>Gets the expression to assign to the field or property.</summary>
		/// <returns>The <see cref="T:System.Linq.Expressions.Expression" /> that represents the value to assign to the field or property.</returns>
		public Expression Expression
		{
			get
			{
				return this._expression;
			}
		}

		/// <summary>Creates a new expression that is like this one, but using the supplied children. If all of the children are the same, it will return this expression.</summary>
		/// <param name="expression">The <see cref="P:System.Linq.Expressions.MemberAssignment.Expression" /> property of the result.</param>
		/// <returns>This expression if no children are changed or an expression with the updated children.</returns>
		public MemberAssignment Update(Expression expression)
		{
			if (expression == this.Expression)
			{
				return this;
			}
			return Expression.Bind(base.Member, expression);
		}

		internal override void ValidateAsDefinedHere(int index)
		{
		}

		internal MemberAssignment()
		{
			ThrowStub.ThrowNotSupportedException();
		}

		private readonly Expression _expression;
	}
}
