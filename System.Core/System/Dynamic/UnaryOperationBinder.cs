using System;
using System.Dynamic.Utils;
using System.Linq.Expressions;

namespace System.Dynamic
{
	/// <summary>Represents the unary dynamic operation at the call site, providing the binding semantic and the details about the operation.</summary>
	public abstract class UnaryOperationBinder : DynamicMetaObjectBinder
	{
		/// <summary>Initializes a new instance of the <see cref="T:System.Dynamic.BinaryOperationBinder" /> class.</summary>
		/// <param name="operation">The unary operation kind.</param>
		protected UnaryOperationBinder(ExpressionType operation)
		{
			ContractUtils.Requires(UnaryOperationBinder.OperationIsValid(operation), "operation");
			this.Operation = operation;
		}

		/// <summary>The result type of the operation.</summary>
		/// <returns>The <see cref="T:System.Type" /> object representing the result type of the operation.</returns>
		public sealed override Type ReturnType
		{
			get
			{
				ExpressionType operation = this.Operation;
				if (operation - ExpressionType.IsTrue <= 1)
				{
					return typeof(bool);
				}
				return typeof(object);
			}
		}

		/// <summary>The unary operation kind.</summary>
		/// <returns>The object of the <see cref="T:System.Linq.Expressions.ExpressionType" /> that represents the unary operation kind.</returns>
		public ExpressionType Operation { get; }

		/// <summary>Performs the binding of the unary dynamic operation if the target dynamic object cannot bind.</summary>
		/// <param name="target">The target of the dynamic unary operation.</param>
		/// <returns>The <see cref="T:System.Dynamic.DynamicMetaObject" /> representing the result of the binding.</returns>
		public DynamicMetaObject FallbackUnaryOperation(DynamicMetaObject target)
		{
			return this.FallbackUnaryOperation(target, null);
		}

		/// <summary>Performs the binding of the unary dynamic operation if the target dynamic object cannot bind.</summary>
		/// <param name="target">The target of the dynamic unary operation.</param>
		/// <param name="errorSuggestion">The binding result in case the binding fails, or null.</param>
		/// <returns>The <see cref="T:System.Dynamic.DynamicMetaObject" /> representing the result of the binding.</returns>
		public abstract DynamicMetaObject FallbackUnaryOperation(DynamicMetaObject target, DynamicMetaObject errorSuggestion);

		/// <summary>Performs the binding of the dynamic unary operation.</summary>
		/// <param name="target">The target of the dynamic operation.</param>
		/// <param name="args">An array of arguments of the dynamic operation.</param>
		/// <returns>The <see cref="T:System.Dynamic.DynamicMetaObject" /> representing the result of the binding.</returns>
		public sealed override DynamicMetaObject Bind(DynamicMetaObject target, DynamicMetaObject[] args)
		{
			ContractUtils.RequiresNotNull(target, "target");
			ContractUtils.Requires(args == null || args.Length == 0, "args");
			return target.BindUnaryOperation(this);
		}

		internal sealed override bool IsStandardBinder
		{
			get
			{
				return true;
			}
		}

		internal static bool OperationIsValid(ExpressionType operation)
		{
			if (operation <= ExpressionType.Decrement)
			{
				if (operation - ExpressionType.Negate > 1 && operation != ExpressionType.Not && operation != ExpressionType.Decrement)
				{
					return false;
				}
			}
			else if (operation != ExpressionType.Extension && operation != ExpressionType.Increment && operation - ExpressionType.OnesComplement > 2)
			{
				return false;
			}
			return true;
		}
	}
}
