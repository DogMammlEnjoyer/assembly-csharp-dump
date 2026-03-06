using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Dynamic.Utils;
using System.Linq.Expressions.Compiler;
using System.Runtime.CompilerServices;
using Unity;

namespace System.Linq.Expressions
{
	/// <summary>Represents a strongly typed lambda expression as a data structure in the form of an expression tree. This class cannot be inherited.</summary>
	/// <typeparam name="TDelegate">The type of the delegate that the <see cref="T:System.Linq.Expressions.Expression`1" /> represents.</typeparam>
	public class Expression<TDelegate> : LambdaExpression
	{
		internal Expression(Expression body) : base(body)
		{
		}

		internal sealed override Type TypeCore
		{
			get
			{
				return typeof(TDelegate);
			}
		}

		internal override Type PublicType
		{
			get
			{
				return typeof(Expression<TDelegate>);
			}
		}

		/// <summary>Compiles the lambda expression described by the expression tree into executable code and produces a delegate that represents the lambda expression.</summary>
		/// <returns>A delegate of type <paramref name="TDelegate" /> that represents the compiled lambda expression described by the <see cref="T:System.Linq.Expressions.Expression`1" />.</returns>
		public new TDelegate Compile()
		{
			return this.Compile(false);
		}

		/// <summary>Compiles the lambda expression described by the expression tree into interpreted or compiled code and produces a delegate that represents the lambda expression.</summary>
		/// <param name="preferInterpretation">
		///   <see langword="true" /> to indicate that the expression should be compiled to an interpreted form, if it is available; <see langword="false" /> otherwise.</param>
		/// <returns>A delegate that represents the compiled lambda expression described by the <see cref="T:System.Linq.Expressions.Expression`1" />.</returns>
		public new TDelegate Compile(bool preferInterpretation)
		{
			return (TDelegate)((object)LambdaCompiler.Compile(this));
		}

		/// <summary>Creates a new expression that is like this one, but using the supplied children. If all of the children are the same, it will return this expression.</summary>
		/// <param name="body">The <see cref="P:System.Linq.Expressions.LambdaExpression.Body" /> property of the result.</param>
		/// <param name="parameters">The <see cref="P:System.Linq.Expressions.LambdaExpression.Parameters" /> property of the result. </param>
		/// <returns>This expression if no children are changed or an expression with the updated children.</returns>
		public Expression<TDelegate> Update(Expression body, IEnumerable<ParameterExpression> parameters)
		{
			if (body == base.Body)
			{
				ICollection<ParameterExpression> collection;
				if (parameters == null)
				{
					collection = null;
				}
				else
				{
					collection = (parameters as ICollection<ParameterExpression>);
					if (collection == null)
					{
						collection = (parameters = parameters.ToReadOnly<ParameterExpression>());
					}
				}
				if (this.SameParameters(collection))
				{
					return this;
				}
			}
			return Expression.Lambda<TDelegate>(body, base.Name, base.TailCall, parameters);
		}

		[ExcludeFromCodeCoverage]
		internal virtual bool SameParameters(ICollection<ParameterExpression> parameters)
		{
			throw ContractUtils.Unreachable;
		}

		[ExcludeFromCodeCoverage]
		internal virtual Expression<TDelegate> Rewrite(Expression body, ParameterExpression[] parameters)
		{
			throw ContractUtils.Unreachable;
		}

		protected internal override Expression Accept(ExpressionVisitor visitor)
		{
			return visitor.VisitLambda<TDelegate>(this);
		}

		internal override LambdaExpression Accept(StackSpiller spiller)
		{
			return spiller.Rewrite<TDelegate>(this);
		}

		internal static Expression<TDelegate> Create(Expression body, string name, bool tailCall, IReadOnlyList<ParameterExpression> parameters)
		{
			if (name != null || tailCall)
			{
				return new FullExpression<TDelegate>(body, name, tailCall, parameters);
			}
			switch (parameters.Count)
			{
			case 0:
				return new Expression0<TDelegate>(body);
			case 1:
				return new Expression1<TDelegate>(body, parameters[0]);
			case 2:
				return new Expression2<TDelegate>(body, parameters[0], parameters[1]);
			case 3:
				return new Expression3<TDelegate>(body, parameters[0], parameters[1], parameters[2]);
			default:
				return new ExpressionN<TDelegate>(body, parameters);
			}
		}

		/// <summary>Produces a delegate that represents the lambda expression.</summary>
		/// <param name="debugInfoGenerator">Debugging information generator used by the compiler to mark sequence points and annotate local variables.</param>
		/// <returns>A delegate containing the compiled version of the lambda.</returns>
		public new TDelegate Compile(DebugInfoGenerator debugInfoGenerator)
		{
			return this.Compile();
		}

		internal Expression()
		{
			ThrowStub.ThrowNotSupportedException();
		}
	}
}
