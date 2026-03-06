using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Dynamic.Utils;
using Unity;

namespace System.Linq.Expressions
{
	/// <summary>Represents a try/catch/finally/fault block.</summary>
	[DebuggerTypeProxy(typeof(Expression.TryExpressionProxy))]
	public sealed class TryExpression : Expression
	{
		internal TryExpression(Type type, Expression body, Expression @finally, Expression fault, ReadOnlyCollection<CatchBlock> handlers)
		{
			this.Type = type;
			this.Body = body;
			this.Handlers = handlers;
			this.Finally = @finally;
			this.Fault = fault;
		}

		/// <summary>Gets the static type of the expression that this <see cref="T:System.Linq.Expressions.Expression" /> represents.</summary>
		/// <returns>The <see cref="P:System.Linq.Expressions.TryExpression.Type" /> that represents the static type of the expression.</returns>
		public sealed override Type Type { get; }

		/// <summary>Returns the node type of this <see cref="T:System.Linq.Expressions.Expression" />.</summary>
		/// <returns>The <see cref="T:System.Linq.Expressions.ExpressionType" /> that represents this expression.</returns>
		public sealed override ExpressionType NodeType
		{
			get
			{
				return ExpressionType.Try;
			}
		}

		/// <summary>Gets the <see cref="T:System.Linq.Expressions.Expression" /> representing the body of the try block.</summary>
		/// <returns>The <see cref="T:System.Linq.Expressions.Expression" /> representing the body of the try block.</returns>
		public Expression Body { get; }

		/// <summary>Gets the collection of <see cref="T:System.Linq.Expressions.CatchBlock" /> expressions associated with the try block.</summary>
		/// <returns>The collection of <see cref="T:System.Linq.Expressions.CatchBlock" /> expressions associated with the try block.</returns>
		public ReadOnlyCollection<CatchBlock> Handlers { get; }

		/// <summary>Gets the <see cref="T:System.Linq.Expressions.Expression" /> representing the finally block.</summary>
		/// <returns>The <see cref="T:System.Linq.Expressions.Expression" /> representing the finally block.</returns>
		public Expression Finally { get; }

		/// <summary>Gets the <see cref="T:System.Linq.Expressions.Expression" /> representing the fault block.</summary>
		/// <returns>The <see cref="T:System.Linq.Expressions.Expression" /> representing the fault block.</returns>
		public Expression Fault { get; }

		protected internal override Expression Accept(ExpressionVisitor visitor)
		{
			return visitor.VisitTry(this);
		}

		/// <summary>Creates a new expression that is like this one, but using the supplied children. If all of the children are the same, it will return this expression.</summary>
		/// <param name="body">The <see cref="P:System.Linq.Expressions.TryExpression.Body" /> property of the result.</param>
		/// <param name="handlers">The <see cref="P:System.Linq.Expressions.TryExpression.Handlers" /> property of the result.</param>
		/// <param name="finally">The <see cref="P:System.Linq.Expressions.TryExpression.Finally" /> property of the result.</param>
		/// <param name="fault">The <see cref="P:System.Linq.Expressions.TryExpression.Fault" /> property of the result.</param>
		/// <returns>This expression if no children are changed or an expression with the updated children.</returns>
		public TryExpression Update(Expression body, IEnumerable<CatchBlock> handlers, Expression @finally, Expression fault)
		{
			if ((body == this.Body & @finally == this.Finally & fault == this.Fault) && ExpressionUtils.SameElements<CatchBlock>(ref handlers, this.Handlers))
			{
				return this;
			}
			return Expression.MakeTry(this.Type, body, @finally, fault, handlers);
		}

		internal TryExpression()
		{
			ThrowStub.ThrowNotSupportedException();
		}
	}
}
