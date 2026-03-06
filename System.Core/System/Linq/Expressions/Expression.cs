using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Dynamic.Utils;
using System.Globalization;
using System.IO;
using System.Linq.Expressions.Compiler;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;

namespace System.Linq.Expressions
{
	/// <summary>Provides the base class from which the classes that represent expression tree nodes are derived. It also contains <see langword="static" /> (<see langword="Shared" /> in Visual Basic) factory methods to create the various node types. This is an <see langword="abstract" /> class.</summary>
	public abstract class Expression
	{
		/// <summary>Creates a <see cref="T:System.Linq.Expressions.BinaryExpression" /> that represents an assignment operation.</summary>
		/// <param name="left">An <see cref="T:System.Linq.Expressions.Expression" /> to set the <see cref="P:System.Linq.Expressions.BinaryExpression.Left" /> property equal to.</param>
		/// <param name="right">An <see cref="T:System.Linq.Expressions.Expression" /> to set the <see cref="P:System.Linq.Expressions.BinaryExpression.Right" /> property equal to.</param>
		/// <returns>A <see cref="T:System.Linq.Expressions.BinaryExpression" /> that has the <see cref="P:System.Linq.Expressions.Expression.NodeType" /> property equal to <see cref="F:System.Linq.Expressions.ExpressionType.Assign" /> and the <see cref="P:System.Linq.Expressions.BinaryExpression.Left" /> and <see cref="P:System.Linq.Expressions.BinaryExpression.Right" /> properties set to the specified values.</returns>
		public static BinaryExpression Assign(Expression left, Expression right)
		{
			Expression.RequiresCanWrite(left, "left");
			ExpressionUtils.RequiresCanRead(right, "right");
			TypeUtils.ValidateType(left.Type, "left", true, true);
			TypeUtils.ValidateType(right.Type, "right", true, true);
			if (!TypeUtils.AreReferenceAssignable(left.Type, right.Type))
			{
				throw Error.ExpressionTypeDoesNotMatchAssignment(right.Type, left.Type);
			}
			return new AssignBinaryExpression(left, right);
		}

		private static BinaryExpression GetUserDefinedBinaryOperator(ExpressionType binaryType, string name, Expression left, Expression right, bool liftToNull)
		{
			MethodInfo userDefinedBinaryOperator = Expression.GetUserDefinedBinaryOperator(binaryType, left.Type, right.Type, name);
			if (userDefinedBinaryOperator != null)
			{
				return new MethodBinaryExpression(binaryType, left, right, userDefinedBinaryOperator.ReturnType, userDefinedBinaryOperator);
			}
			if (left.Type.IsNullableType() && right.Type.IsNullableType())
			{
				Type nonNullableType = left.Type.GetNonNullableType();
				Type nonNullableType2 = right.Type.GetNonNullableType();
				userDefinedBinaryOperator = Expression.GetUserDefinedBinaryOperator(binaryType, nonNullableType, nonNullableType2, name);
				if (userDefinedBinaryOperator != null && userDefinedBinaryOperator.ReturnType.IsValueType && !userDefinedBinaryOperator.ReturnType.IsNullableType())
				{
					if (userDefinedBinaryOperator.ReturnType != typeof(bool) || liftToNull)
					{
						return new MethodBinaryExpression(binaryType, left, right, userDefinedBinaryOperator.ReturnType.GetNullableType(), userDefinedBinaryOperator);
					}
					return new MethodBinaryExpression(binaryType, left, right, typeof(bool), userDefinedBinaryOperator);
				}
			}
			return null;
		}

		private static BinaryExpression GetMethodBasedBinaryOperator(ExpressionType binaryType, Expression left, Expression right, MethodInfo method, bool liftToNull)
		{
			Expression.ValidateOperator(method);
			ParameterInfo[] parametersCached = method.GetParametersCached();
			if (parametersCached.Length != 2)
			{
				throw Error.IncorrectNumberOfMethodCallArguments(method, "method");
			}
			if (Expression.ParameterIsAssignable(parametersCached[0], left.Type) && Expression.ParameterIsAssignable(parametersCached[1], right.Type))
			{
				Expression.ValidateParamswithOperandsOrThrow(parametersCached[0].ParameterType, left.Type, binaryType, method.Name);
				Expression.ValidateParamswithOperandsOrThrow(parametersCached[1].ParameterType, right.Type, binaryType, method.Name);
				return new MethodBinaryExpression(binaryType, left, right, method.ReturnType, method);
			}
			if (!left.Type.IsNullableType() || !right.Type.IsNullableType() || !Expression.ParameterIsAssignable(parametersCached[0], left.Type.GetNonNullableType()) || !Expression.ParameterIsAssignable(parametersCached[1], right.Type.GetNonNullableType()) || !method.ReturnType.IsValueType || method.ReturnType.IsNullableType())
			{
				throw Error.OperandTypesDoNotMatchParameters(binaryType, method.Name);
			}
			if (method.ReturnType != typeof(bool) || liftToNull)
			{
				return new MethodBinaryExpression(binaryType, left, right, method.ReturnType.GetNullableType(), method);
			}
			return new MethodBinaryExpression(binaryType, left, right, typeof(bool), method);
		}

		private static BinaryExpression GetMethodBasedAssignOperator(ExpressionType binaryType, Expression left, Expression right, MethodInfo method, LambdaExpression conversion, bool liftToNull)
		{
			BinaryExpression binaryExpression = Expression.GetMethodBasedBinaryOperator(binaryType, left, right, method, liftToNull);
			if (conversion == null)
			{
				if (!TypeUtils.AreReferenceAssignable(left.Type, binaryExpression.Type))
				{
					throw Error.UserDefinedOpMustHaveValidReturnType(binaryType, binaryExpression.Method.Name);
				}
			}
			else
			{
				Expression.ValidateOpAssignConversionLambda(conversion, binaryExpression.Left, binaryExpression.Method, binaryExpression.NodeType);
				binaryExpression = new OpAssignMethodConversionBinaryExpression(binaryExpression.NodeType, binaryExpression.Left, binaryExpression.Right, binaryExpression.Left.Type, binaryExpression.Method, conversion);
			}
			return binaryExpression;
		}

		private static BinaryExpression GetUserDefinedBinaryOperatorOrThrow(ExpressionType binaryType, string name, Expression left, Expression right, bool liftToNull)
		{
			BinaryExpression userDefinedBinaryOperator = Expression.GetUserDefinedBinaryOperator(binaryType, name, left, right, liftToNull);
			if (userDefinedBinaryOperator != null)
			{
				ParameterInfo[] parametersCached = userDefinedBinaryOperator.Method.GetParametersCached();
				Expression.ValidateParamswithOperandsOrThrow(parametersCached[0].ParameterType, left.Type, binaryType, name);
				Expression.ValidateParamswithOperandsOrThrow(parametersCached[1].ParameterType, right.Type, binaryType, name);
				return userDefinedBinaryOperator;
			}
			throw Error.BinaryOperatorNotDefined(binaryType, left.Type, right.Type);
		}

		private static BinaryExpression GetUserDefinedAssignOperatorOrThrow(ExpressionType binaryType, string name, Expression left, Expression right, LambdaExpression conversion, bool liftToNull)
		{
			BinaryExpression binaryExpression = Expression.GetUserDefinedBinaryOperatorOrThrow(binaryType, name, left, right, liftToNull);
			if (conversion == null)
			{
				if (!TypeUtils.AreReferenceAssignable(left.Type, binaryExpression.Type))
				{
					throw Error.UserDefinedOpMustHaveValidReturnType(binaryType, binaryExpression.Method.Name);
				}
			}
			else
			{
				Expression.ValidateOpAssignConversionLambda(conversion, binaryExpression.Left, binaryExpression.Method, binaryExpression.NodeType);
				binaryExpression = new OpAssignMethodConversionBinaryExpression(binaryExpression.NodeType, binaryExpression.Left, binaryExpression.Right, binaryExpression.Left.Type, binaryExpression.Method, conversion);
			}
			return binaryExpression;
		}

		private static MethodInfo GetUserDefinedBinaryOperator(ExpressionType binaryType, Type leftType, Type rightType, string name)
		{
			Type[] types = new Type[]
			{
				leftType,
				rightType
			};
			Type nonNullableType = leftType.GetNonNullableType();
			Type nonNullableType2 = rightType.GetNonNullableType();
			MethodInfo methodInfo = nonNullableType.GetAnyStaticMethodValidated(name, types);
			if (methodInfo == null && !TypeUtils.AreEquivalent(leftType, rightType))
			{
				methodInfo = nonNullableType2.GetAnyStaticMethodValidated(name, types);
			}
			if (Expression.IsLiftingConditionalLogicalOperator(leftType, rightType, methodInfo, binaryType))
			{
				methodInfo = Expression.GetUserDefinedBinaryOperator(binaryType, nonNullableType, nonNullableType2, name);
			}
			return methodInfo;
		}

		private static bool IsLiftingConditionalLogicalOperator(Type left, Type right, MethodInfo method, ExpressionType binaryType)
		{
			return right.IsNullableType() && left.IsNullableType() && method == null && (binaryType == ExpressionType.AndAlso || binaryType == ExpressionType.OrElse);
		}

		internal static bool ParameterIsAssignable(ParameterInfo pi, Type argType)
		{
			Type type = pi.ParameterType;
			if (type.IsByRef)
			{
				type = type.GetElementType();
			}
			return TypeUtils.AreReferenceAssignable(type, argType);
		}

		private static void ValidateParamswithOperandsOrThrow(Type paramType, Type operandType, ExpressionType exprType, string name)
		{
			if (paramType.IsNullableType() && !operandType.IsNullableType())
			{
				throw Error.OperandTypesDoNotMatchParameters(exprType, name);
			}
		}

		private static void ValidateOperator(MethodInfo method)
		{
			Expression.ValidateMethodInfo(method, "method");
			if (!method.IsStatic)
			{
				throw Error.UserDefinedOperatorMustBeStatic(method, "method");
			}
			if (method.ReturnType == typeof(void))
			{
				throw Error.UserDefinedOperatorMustNotBeVoid(method, "method");
			}
		}

		private static void ValidateMethodInfo(MethodInfo method, string paramName)
		{
			if (method.ContainsGenericParameters)
			{
				throw method.IsGenericMethodDefinition ? Error.MethodIsGeneric(method, paramName) : Error.MethodContainsGenericParameters(method, paramName);
			}
		}

		private static bool IsNullComparison(Expression left, Expression right)
		{
			if (!Expression.IsNullConstant(left))
			{
				return Expression.IsNullConstant(right) && left.Type.IsNullableType();
			}
			return !Expression.IsNullConstant(right) && right.Type.IsNullableType();
		}

		private static bool IsNullConstant(Expression e)
		{
			ConstantExpression constantExpression = e as ConstantExpression;
			return constantExpression != null && constantExpression.Value == null;
		}

		private static void ValidateUserDefinedConditionalLogicOperator(ExpressionType nodeType, Type left, Type right, MethodInfo method)
		{
			Expression.ValidateOperator(method);
			ParameterInfo[] parametersCached = method.GetParametersCached();
			if (parametersCached.Length != 2)
			{
				throw Error.IncorrectNumberOfMethodCallArguments(method, "method");
			}
			if (!Expression.ParameterIsAssignable(parametersCached[0], left) && (!left.IsNullableType() || !Expression.ParameterIsAssignable(parametersCached[0], left.GetNonNullableType())))
			{
				throw Error.OperandTypesDoNotMatchParameters(nodeType, method.Name);
			}
			if (!Expression.ParameterIsAssignable(parametersCached[1], right) && (!right.IsNullableType() || !Expression.ParameterIsAssignable(parametersCached[1], right.GetNonNullableType())))
			{
				throw Error.OperandTypesDoNotMatchParameters(nodeType, method.Name);
			}
			if (parametersCached[0].ParameterType != parametersCached[1].ParameterType)
			{
				throw Error.UserDefinedOpMustHaveConsistentTypes(nodeType, method.Name);
			}
			if (method.ReturnType != parametersCached[0].ParameterType)
			{
				throw Error.UserDefinedOpMustHaveConsistentTypes(nodeType, method.Name);
			}
			if (Expression.IsValidLiftedConditionalLogicalOperator(left, right, parametersCached))
			{
				left = left.GetNonNullableType();
			}
			Type declaringType = method.DeclaringType;
			if (declaringType == null)
			{
				throw Error.LogicalOperatorMustHaveBooleanOperators(nodeType, method.Name);
			}
			MethodInfo booleanOperator = TypeUtils.GetBooleanOperator(declaringType, "op_True");
			MethodInfo booleanOperator2 = TypeUtils.GetBooleanOperator(declaringType, "op_False");
			if (booleanOperator == null || booleanOperator.ReturnType != typeof(bool) || booleanOperator2 == null || booleanOperator2.ReturnType != typeof(bool))
			{
				throw Error.LogicalOperatorMustHaveBooleanOperators(nodeType, method.Name);
			}
			Expression.VerifyOpTrueFalse(nodeType, left, booleanOperator2, "method");
			Expression.VerifyOpTrueFalse(nodeType, left, booleanOperator, "method");
		}

		private static void VerifyOpTrueFalse(ExpressionType nodeType, Type left, MethodInfo opTrue, string paramName)
		{
			ParameterInfo[] parametersCached = opTrue.GetParametersCached();
			if (parametersCached.Length != 1)
			{
				throw Error.IncorrectNumberOfMethodCallArguments(opTrue, paramName);
			}
			if (!Expression.ParameterIsAssignable(parametersCached[0], left) && (!left.IsNullableType() || !Expression.ParameterIsAssignable(parametersCached[0], left.GetNonNullableType())))
			{
				throw Error.OperandTypesDoNotMatchParameters(nodeType, opTrue.Name);
			}
		}

		private static bool IsValidLiftedConditionalLogicalOperator(Type left, Type right, ParameterInfo[] pms)
		{
			return TypeUtils.AreEquivalent(left, right) && right.IsNullableType() && TypeUtils.AreEquivalent(pms[1].ParameterType, right.GetNonNullableType());
		}

		/// <summary>Creates a <see cref="T:System.Linq.Expressions.BinaryExpression" />, given the left and right operands, by calling an appropriate factory method.</summary>
		/// <param name="binaryType">The <see cref="T:System.Linq.Expressions.ExpressionType" /> that specifies the type of binary operation.</param>
		/// <param name="left">An <see cref="T:System.Linq.Expressions.Expression" /> that represents the left operand.</param>
		/// <param name="right">An <see cref="T:System.Linq.Expressions.Expression" /> that represents the right operand.</param>
		/// <returns>The <see cref="T:System.Linq.Expressions.BinaryExpression" /> that results from calling the appropriate factory method.</returns>
		/// <exception cref="T:System.ArgumentException">
		///         <paramref name="binaryType" /> does not correspond to a binary expression node.</exception>
		/// <exception cref="T:System.ArgumentNullException">
		///         <paramref name="left" /> or <paramref name="right" /> is <see langword="null" />.</exception>
		public static BinaryExpression MakeBinary(ExpressionType binaryType, Expression left, Expression right)
		{
			return Expression.MakeBinary(binaryType, left, right, false, null, null);
		}

		/// <summary>Creates a <see cref="T:System.Linq.Expressions.BinaryExpression" />, given the left operand, right operand and implementing method, by calling the appropriate factory method.</summary>
		/// <param name="binaryType">The <see cref="T:System.Linq.Expressions.ExpressionType" /> that specifies the type of binary operation.</param>
		/// <param name="left">An <see cref="T:System.Linq.Expressions.Expression" /> that represents the left operand.</param>
		/// <param name="right">An <see cref="T:System.Linq.Expressions.Expression" /> that represents the right operand.</param>
		/// <param name="liftToNull">
		///       <see langword="true" /> to set <see cref="P:System.Linq.Expressions.BinaryExpression.IsLiftedToNull" /> to <see langword="true" />; <see langword="false" /> to set <see cref="P:System.Linq.Expressions.BinaryExpression.IsLiftedToNull" /> to <see langword="false" />.</param>
		/// <param name="method">A <see cref="T:System.Reflection.MethodInfo" /> that specifies the implementing method.</param>
		/// <returns>The <see cref="T:System.Linq.Expressions.BinaryExpression" /> that results from calling the appropriate factory method.</returns>
		/// <exception cref="T:System.ArgumentException">
		///         <paramref name="binaryType" /> does not correspond to a binary expression node.</exception>
		/// <exception cref="T:System.ArgumentNullException">
		///         <paramref name="left" /> or <paramref name="right" /> is <see langword="null" />.</exception>
		public static BinaryExpression MakeBinary(ExpressionType binaryType, Expression left, Expression right, bool liftToNull, MethodInfo method)
		{
			return Expression.MakeBinary(binaryType, left, right, liftToNull, method, null);
		}

		/// <summary>Creates a <see cref="T:System.Linq.Expressions.BinaryExpression" />, given the left operand, right operand, implementing method and type conversion function, by calling the appropriate factory method.</summary>
		/// <param name="binaryType">The <see cref="T:System.Linq.Expressions.ExpressionType" /> that specifies the type of binary operation.</param>
		/// <param name="left">An <see cref="T:System.Linq.Expressions.Expression" /> that represents the left operand.</param>
		/// <param name="right">An <see cref="T:System.Linq.Expressions.Expression" /> that represents the right operand.</param>
		/// <param name="liftToNull">
		///       <see langword="true" /> to set <see cref="P:System.Linq.Expressions.BinaryExpression.IsLiftedToNull" /> to <see langword="true" />; <see langword="false" /> to set <see cref="P:System.Linq.Expressions.BinaryExpression.IsLiftedToNull" /> to <see langword="false" />.</param>
		/// <param name="method">A <see cref="T:System.Reflection.MethodInfo" /> that specifies the implementing method.</param>
		/// <param name="conversion">A <see cref="T:System.Linq.Expressions.LambdaExpression" /> that represents a type conversion function. This parameter is used only if <paramref name="binaryType" /> is <see cref="F:System.Linq.Expressions.ExpressionType.Coalesce" /> or compound assignment..</param>
		/// <returns>The <see cref="T:System.Linq.Expressions.BinaryExpression" /> that results from calling the appropriate factory method.</returns>
		/// <exception cref="T:System.ArgumentException">
		///         <paramref name="binaryType" /> does not correspond to a binary expression node.</exception>
		/// <exception cref="T:System.ArgumentNullException">
		///         <paramref name="left" /> or <paramref name="right" /> is <see langword="null" />.</exception>
		public static BinaryExpression MakeBinary(ExpressionType binaryType, Expression left, Expression right, bool liftToNull, MethodInfo method, LambdaExpression conversion)
		{
			switch (binaryType)
			{
			case ExpressionType.Add:
				return Expression.Add(left, right, method);
			case ExpressionType.AddChecked:
				return Expression.AddChecked(left, right, method);
			case ExpressionType.And:
				return Expression.And(left, right, method);
			case ExpressionType.AndAlso:
				return Expression.AndAlso(left, right, method);
			case ExpressionType.ArrayIndex:
				return Expression.ArrayIndex(left, right);
			case ExpressionType.Coalesce:
				return Expression.Coalesce(left, right, conversion);
			case ExpressionType.Divide:
				return Expression.Divide(left, right, method);
			case ExpressionType.Equal:
				return Expression.Equal(left, right, liftToNull, method);
			case ExpressionType.ExclusiveOr:
				return Expression.ExclusiveOr(left, right, method);
			case ExpressionType.GreaterThan:
				return Expression.GreaterThan(left, right, liftToNull, method);
			case ExpressionType.GreaterThanOrEqual:
				return Expression.GreaterThanOrEqual(left, right, liftToNull, method);
			case ExpressionType.LeftShift:
				return Expression.LeftShift(left, right, method);
			case ExpressionType.LessThan:
				return Expression.LessThan(left, right, liftToNull, method);
			case ExpressionType.LessThanOrEqual:
				return Expression.LessThanOrEqual(left, right, liftToNull, method);
			case ExpressionType.Modulo:
				return Expression.Modulo(left, right, method);
			case ExpressionType.Multiply:
				return Expression.Multiply(left, right, method);
			case ExpressionType.MultiplyChecked:
				return Expression.MultiplyChecked(left, right, method);
			case ExpressionType.NotEqual:
				return Expression.NotEqual(left, right, liftToNull, method);
			case ExpressionType.Or:
				return Expression.Or(left, right, method);
			case ExpressionType.OrElse:
				return Expression.OrElse(left, right, method);
			case ExpressionType.Power:
				return Expression.Power(left, right, method);
			case ExpressionType.RightShift:
				return Expression.RightShift(left, right, method);
			case ExpressionType.Subtract:
				return Expression.Subtract(left, right, method);
			case ExpressionType.SubtractChecked:
				return Expression.SubtractChecked(left, right, method);
			case ExpressionType.Assign:
				return Expression.Assign(left, right);
			case ExpressionType.AddAssign:
				return Expression.AddAssign(left, right, method, conversion);
			case ExpressionType.AndAssign:
				return Expression.AndAssign(left, right, method, conversion);
			case ExpressionType.DivideAssign:
				return Expression.DivideAssign(left, right, method, conversion);
			case ExpressionType.ExclusiveOrAssign:
				return Expression.ExclusiveOrAssign(left, right, method, conversion);
			case ExpressionType.LeftShiftAssign:
				return Expression.LeftShiftAssign(left, right, method, conversion);
			case ExpressionType.ModuloAssign:
				return Expression.ModuloAssign(left, right, method, conversion);
			case ExpressionType.MultiplyAssign:
				return Expression.MultiplyAssign(left, right, method, conversion);
			case ExpressionType.OrAssign:
				return Expression.OrAssign(left, right, method, conversion);
			case ExpressionType.PowerAssign:
				return Expression.PowerAssign(left, right, method, conversion);
			case ExpressionType.RightShiftAssign:
				return Expression.RightShiftAssign(left, right, method, conversion);
			case ExpressionType.SubtractAssign:
				return Expression.SubtractAssign(left, right, method, conversion);
			case ExpressionType.AddAssignChecked:
				return Expression.AddAssignChecked(left, right, method, conversion);
			case ExpressionType.MultiplyAssignChecked:
				return Expression.MultiplyAssignChecked(left, right, method, conversion);
			case ExpressionType.SubtractAssignChecked:
				return Expression.SubtractAssignChecked(left, right, method, conversion);
			}
			throw Error.UnhandledBinary(binaryType, "binaryType");
		}

		/// <summary>Creates a <see cref="T:System.Linq.Expressions.BinaryExpression" /> that represents an equality comparison.</summary>
		/// <param name="left">An <see cref="T:System.Linq.Expressions.Expression" /> to set the <see cref="P:System.Linq.Expressions.BinaryExpression.Left" /> property equal to.</param>
		/// <param name="right">An <see cref="T:System.Linq.Expressions.Expression" /> to set the <see cref="P:System.Linq.Expressions.BinaryExpression.Right" /> property equal to.</param>
		/// <returns>A <see cref="T:System.Linq.Expressions.BinaryExpression" /> that has the <see cref="P:System.Linq.Expressions.Expression.NodeType" /> property equal to <see cref="F:System.Linq.Expressions.ExpressionType.Equal" /> and the <see cref="P:System.Linq.Expressions.BinaryExpression.Left" /> and <see cref="P:System.Linq.Expressions.BinaryExpression.Right" /> properties set to the specified values.</returns>
		/// <exception cref="T:System.ArgumentNullException">
		///         <paramref name="left" /> or <paramref name="right" /> is <see langword="null" />.</exception>
		/// <exception cref="T:System.InvalidOperationException">The equality operator is not defined for <paramref name="left" />.Type and <paramref name="right" />.Type.</exception>
		public static BinaryExpression Equal(Expression left, Expression right)
		{
			return Expression.Equal(left, right, false, null);
		}

		/// <summary>Creates a <see cref="T:System.Linq.Expressions.BinaryExpression" /> that represents an equality comparison. The implementing method can be specified.</summary>
		/// <param name="left">An <see cref="T:System.Linq.Expressions.Expression" /> to set the <see cref="P:System.Linq.Expressions.BinaryExpression.Left" /> property equal to.</param>
		/// <param name="right">An <see cref="T:System.Linq.Expressions.Expression" /> to set the <see cref="P:System.Linq.Expressions.BinaryExpression.Right" /> property equal to.</param>
		/// <param name="liftToNull">
		///       <see langword="true" /> to set <see cref="P:System.Linq.Expressions.BinaryExpression.IsLiftedToNull" /> to <see langword="true" />; <see langword="false" /> to set <see cref="P:System.Linq.Expressions.BinaryExpression.IsLiftedToNull" /> to <see langword="false" />.</param>
		/// <param name="method">A <see cref="T:System.Reflection.MethodInfo" /> to set the <see cref="P:System.Linq.Expressions.BinaryExpression.Method" /> property equal to.</param>
		/// <returns>A <see cref="T:System.Linq.Expressions.BinaryExpression" /> that has the <see cref="P:System.Linq.Expressions.Expression.NodeType" /> property equal to <see cref="F:System.Linq.Expressions.ExpressionType.Equal" /> and the <see cref="P:System.Linq.Expressions.BinaryExpression.Left" />, <see cref="P:System.Linq.Expressions.BinaryExpression.Right" />, <see cref="P:System.Linq.Expressions.BinaryExpression.IsLiftedToNull" />, and <see cref="P:System.Linq.Expressions.BinaryExpression.Method" /> properties set to the specified values.</returns>
		/// <exception cref="T:System.ArgumentNullException">
		///         <paramref name="left" /> or <paramref name="right" /> is <see langword="null" />.</exception>
		/// <exception cref="T:System.ArgumentException">
		///         <paramref name="method" /> is not <see langword="null" /> and the method it represents returns <see langword="void" />, is not <see langword="static" /> (<see langword="Shared" /> in Visual Basic), or does not take exactly two arguments.</exception>
		/// <exception cref="T:System.InvalidOperationException">
		///         <paramref name="method" /> is <see langword="null" /> and the equality operator is not defined for <paramref name="left" />.Type and <paramref name="right" />.Type.</exception>
		public static BinaryExpression Equal(Expression left, Expression right, bool liftToNull, MethodInfo method)
		{
			ExpressionUtils.RequiresCanRead(left, "left");
			ExpressionUtils.RequiresCanRead(right, "right");
			if (method == null)
			{
				return Expression.GetEqualityComparisonOperator(ExpressionType.Equal, "op_Equality", left, right, liftToNull);
			}
			return Expression.GetMethodBasedBinaryOperator(ExpressionType.Equal, left, right, method, liftToNull);
		}

		/// <summary>Creates a <see cref="T:System.Linq.Expressions.BinaryExpression" /> that represents a reference equality comparison.</summary>
		/// <param name="left">An <see cref="T:System.Linq.Expressions.Expression" /> to set the <see cref="P:System.Linq.Expressions.BinaryExpression.Left" /> property equal to.</param>
		/// <param name="right">An <see cref="T:System.Linq.Expressions.Expression" /> to set the <see cref="P:System.Linq.Expressions.BinaryExpression.Right" /> property equal to.</param>
		/// <returns>A <see cref="T:System.Linq.Expressions.BinaryExpression" /> that has the <see cref="P:System.Linq.Expressions.Expression.NodeType" /> property equal to <see cref="F:System.Linq.Expressions.ExpressionType.Equal" /> and the <see cref="P:System.Linq.Expressions.BinaryExpression.Left" /> and <see cref="P:System.Linq.Expressions.BinaryExpression.Right" /> properties set to the specified values.</returns>
		public static BinaryExpression ReferenceEqual(Expression left, Expression right)
		{
			ExpressionUtils.RequiresCanRead(left, "left");
			ExpressionUtils.RequiresCanRead(right, "right");
			if (TypeUtils.HasReferenceEquality(left.Type, right.Type))
			{
				return new LogicalBinaryExpression(ExpressionType.Equal, left, right);
			}
			throw Error.ReferenceEqualityNotDefined(left.Type, right.Type);
		}

		/// <summary>Creates a <see cref="T:System.Linq.Expressions.BinaryExpression" /> that represents an inequality comparison.</summary>
		/// <param name="left">An <see cref="T:System.Linq.Expressions.Expression" /> to set the <see cref="P:System.Linq.Expressions.BinaryExpression.Left" /> property equal to.</param>
		/// <param name="right">An <see cref="T:System.Linq.Expressions.Expression" /> to set the <see cref="P:System.Linq.Expressions.BinaryExpression.Right" /> property equal to.</param>
		/// <returns>A <see cref="T:System.Linq.Expressions.BinaryExpression" /> that has the <see cref="P:System.Linq.Expressions.Expression.NodeType" /> property equal to <see cref="F:System.Linq.Expressions.ExpressionType.NotEqual" /> and the <see cref="P:System.Linq.Expressions.BinaryExpression.Left" /> and <see cref="P:System.Linq.Expressions.BinaryExpression.Right" /> properties set to the specified values.</returns>
		/// <exception cref="T:System.ArgumentNullException">
		///         <paramref name="left" /> or <paramref name="right" /> is <see langword="null" />.</exception>
		/// <exception cref="T:System.InvalidOperationException">The inequality operator is not defined for <paramref name="left" />.Type and <paramref name="right" />.Type.</exception>
		public static BinaryExpression NotEqual(Expression left, Expression right)
		{
			return Expression.NotEqual(left, right, false, null);
		}

		/// <summary>Creates a <see cref="T:System.Linq.Expressions.BinaryExpression" /> that represents an inequality comparison.</summary>
		/// <param name="left">An <see cref="T:System.Linq.Expressions.Expression" /> to set the <see cref="P:System.Linq.Expressions.BinaryExpression.Left" /> property equal to.</param>
		/// <param name="right">An <see cref="T:System.Linq.Expressions.Expression" /> to set the <see cref="P:System.Linq.Expressions.BinaryExpression.Right" /> property equal to.</param>
		/// <param name="liftToNull">
		///       <see langword="true" /> to set <see cref="P:System.Linq.Expressions.BinaryExpression.IsLiftedToNull" /> to <see langword="true" />; <see langword="false" /> to set <see cref="P:System.Linq.Expressions.BinaryExpression.IsLiftedToNull" /> to <see langword="false" />.</param>
		/// <param name="method">A <see cref="T:System.Reflection.MethodInfo" /> to set the <see cref="P:System.Linq.Expressions.BinaryExpression.Method" /> property equal to.</param>
		/// <returns>A <see cref="T:System.Linq.Expressions.BinaryExpression" /> that has the <see cref="P:System.Linq.Expressions.Expression.NodeType" /> property equal to <see cref="F:System.Linq.Expressions.ExpressionType.NotEqual" /> and the <see cref="P:System.Linq.Expressions.BinaryExpression.Left" />, <see cref="P:System.Linq.Expressions.BinaryExpression.Right" />, <see cref="P:System.Linq.Expressions.BinaryExpression.IsLiftedToNull" />, and <see cref="P:System.Linq.Expressions.BinaryExpression.Method" /> properties set to the specified values.</returns>
		/// <exception cref="T:System.ArgumentNullException">
		///         <paramref name="left" /> or <paramref name="right" /> is <see langword="null" />.</exception>
		/// <exception cref="T:System.ArgumentException">
		///         <paramref name="method" /> is not <see langword="null" /> and the method it represents returns <see langword="void" />, is not <see langword="static" /> (<see langword="Shared" /> in Visual Basic), or does not take exactly two arguments.</exception>
		/// <exception cref="T:System.InvalidOperationException">
		///         <paramref name="method" /> is <see langword="null" /> and the inequality operator is not defined for <paramref name="left" />.Type and <paramref name="right" />.Type.</exception>
		public static BinaryExpression NotEqual(Expression left, Expression right, bool liftToNull, MethodInfo method)
		{
			ExpressionUtils.RequiresCanRead(left, "left");
			ExpressionUtils.RequiresCanRead(right, "right");
			if (method == null)
			{
				return Expression.GetEqualityComparisonOperator(ExpressionType.NotEqual, "op_Inequality", left, right, liftToNull);
			}
			return Expression.GetMethodBasedBinaryOperator(ExpressionType.NotEqual, left, right, method, liftToNull);
		}

		/// <summary>Creates a <see cref="T:System.Linq.Expressions.BinaryExpression" /> that represents a reference inequality comparison.</summary>
		/// <param name="left">An <see cref="T:System.Linq.Expressions.Expression" /> to set the <see cref="P:System.Linq.Expressions.BinaryExpression.Left" /> property equal to.</param>
		/// <param name="right">An <see cref="T:System.Linq.Expressions.Expression" /> to set the <see cref="P:System.Linq.Expressions.BinaryExpression.Right" /> property equal to.</param>
		/// <returns>A <see cref="T:System.Linq.Expressions.BinaryExpression" /> that has the <see cref="P:System.Linq.Expressions.Expression.NodeType" /> property equal to <see cref="F:System.Linq.Expressions.ExpressionType.NotEqual" /> and the <see cref="P:System.Linq.Expressions.BinaryExpression.Left" /> and <see cref="P:System.Linq.Expressions.BinaryExpression.Right" /> properties set to the specified values.</returns>
		public static BinaryExpression ReferenceNotEqual(Expression left, Expression right)
		{
			ExpressionUtils.RequiresCanRead(left, "left");
			ExpressionUtils.RequiresCanRead(right, "right");
			if (TypeUtils.HasReferenceEquality(left.Type, right.Type))
			{
				return new LogicalBinaryExpression(ExpressionType.NotEqual, left, right);
			}
			throw Error.ReferenceEqualityNotDefined(left.Type, right.Type);
		}

		private static BinaryExpression GetEqualityComparisonOperator(ExpressionType binaryType, string opName, Expression left, Expression right, bool liftToNull)
		{
			if (left.Type == right.Type && (left.Type.IsNumeric() || left.Type == typeof(object) || left.Type.IsBool() || left.Type.GetNonNullableType().IsEnum))
			{
				if (left.Type.IsNullableType() && liftToNull)
				{
					return new SimpleBinaryExpression(binaryType, left, right, typeof(bool?));
				}
				return new LogicalBinaryExpression(binaryType, left, right);
			}
			else
			{
				BinaryExpression userDefinedBinaryOperator = Expression.GetUserDefinedBinaryOperator(binaryType, opName, left, right, liftToNull);
				if (userDefinedBinaryOperator != null)
				{
					return userDefinedBinaryOperator;
				}
				if (!TypeUtils.HasBuiltInEqualityOperator(left.Type, right.Type) && !Expression.IsNullComparison(left, right))
				{
					throw Error.BinaryOperatorNotDefined(binaryType, left.Type, right.Type);
				}
				if (left.Type.IsNullableType() && liftToNull)
				{
					return new SimpleBinaryExpression(binaryType, left, right, typeof(bool?));
				}
				return new LogicalBinaryExpression(binaryType, left, right);
			}
		}

		/// <summary>Creates a <see cref="T:System.Linq.Expressions.BinaryExpression" /> that represents a "greater than" numeric comparison.</summary>
		/// <param name="left">An <see cref="T:System.Linq.Expressions.Expression" /> to set the <see cref="P:System.Linq.Expressions.BinaryExpression.Left" /> property equal to.</param>
		/// <param name="right">An <see cref="T:System.Linq.Expressions.Expression" /> to set the <see cref="P:System.Linq.Expressions.BinaryExpression.Right" /> property equal to.</param>
		/// <returns>A <see cref="T:System.Linq.Expressions.BinaryExpression" /> that has the <see cref="P:System.Linq.Expressions.Expression.NodeType" /> property equal to <see cref="F:System.Linq.Expressions.ExpressionType.GreaterThan" /> and the <see cref="P:System.Linq.Expressions.BinaryExpression.Left" /> and <see cref="P:System.Linq.Expressions.BinaryExpression.Right" /> properties set to the specified values.</returns>
		/// <exception cref="T:System.ArgumentNullException">
		///         <paramref name="left" /> or <paramref name="right" /> is <see langword="null" />.</exception>
		/// <exception cref="T:System.InvalidOperationException">The "greater than" operator is not defined for <paramref name="left" />.Type and <paramref name="right" />.Type.</exception>
		public static BinaryExpression GreaterThan(Expression left, Expression right)
		{
			return Expression.GreaterThan(left, right, false, null);
		}

		/// <summary>Creates a <see cref="T:System.Linq.Expressions.BinaryExpression" /> that represents a "greater than" numeric comparison. The implementing method can be specified.</summary>
		/// <param name="left">An <see cref="T:System.Linq.Expressions.Expression" /> to set the <see cref="P:System.Linq.Expressions.BinaryExpression.Left" /> property equal to.</param>
		/// <param name="right">An <see cref="T:System.Linq.Expressions.Expression" /> to set the <see cref="P:System.Linq.Expressions.BinaryExpression.Right" /> property equal to.</param>
		/// <param name="liftToNull">
		///       <see langword="true" /> to set <see cref="P:System.Linq.Expressions.BinaryExpression.IsLiftedToNull" /> to <see langword="true" />; <see langword="false" /> to set <see cref="P:System.Linq.Expressions.BinaryExpression.IsLiftedToNull" /> to <see langword="false" />.</param>
		/// <param name="method">A <see cref="T:System.Reflection.MethodInfo" /> to set the <see cref="P:System.Linq.Expressions.BinaryExpression.Method" /> property equal to.</param>
		/// <returns>A <see cref="T:System.Linq.Expressions.BinaryExpression" /> that has the <see cref="P:System.Linq.Expressions.Expression.NodeType" /> property equal to <see cref="F:System.Linq.Expressions.ExpressionType.GreaterThan" /> and the <see cref="P:System.Linq.Expressions.BinaryExpression.Left" />, <see cref="P:System.Linq.Expressions.BinaryExpression.Right" />, <see cref="P:System.Linq.Expressions.BinaryExpression.IsLiftedToNull" />, and <see cref="P:System.Linq.Expressions.BinaryExpression.Method" /> properties set to the specified values.</returns>
		/// <exception cref="T:System.ArgumentNullException">
		///         <paramref name="left" /> or <paramref name="right" /> is <see langword="null" />.</exception>
		/// <exception cref="T:System.ArgumentException">
		///         <paramref name="method" /> is not <see langword="null" /> and the method it represents returns <see langword="void" />, is not <see langword="static" /> (<see langword="Shared" /> in Visual Basic), or does not take exactly two arguments.</exception>
		/// <exception cref="T:System.InvalidOperationException">
		///         <paramref name="method" /> is <see langword="null" /> and the "greater than" operator is not defined for <paramref name="left" />.Type and <paramref name="right" />.Type.</exception>
		public static BinaryExpression GreaterThan(Expression left, Expression right, bool liftToNull, MethodInfo method)
		{
			ExpressionUtils.RequiresCanRead(left, "left");
			ExpressionUtils.RequiresCanRead(right, "right");
			if (method == null)
			{
				return Expression.GetComparisonOperator(ExpressionType.GreaterThan, "op_GreaterThan", left, right, liftToNull);
			}
			return Expression.GetMethodBasedBinaryOperator(ExpressionType.GreaterThan, left, right, method, liftToNull);
		}

		/// <summary>Creates a <see cref="T:System.Linq.Expressions.BinaryExpression" /> that represents a "less than" numeric comparison.</summary>
		/// <param name="left">An <see cref="T:System.Linq.Expressions.Expression" /> to set the <see cref="P:System.Linq.Expressions.BinaryExpression.Left" /> property equal to.</param>
		/// <param name="right">An <see cref="T:System.Linq.Expressions.Expression" /> to set the <see cref="P:System.Linq.Expressions.BinaryExpression.Right" /> property equal to.</param>
		/// <returns>A <see cref="T:System.Linq.Expressions.BinaryExpression" /> that has the <see cref="P:System.Linq.Expressions.Expression.NodeType" /> property equal to <see cref="F:System.Linq.Expressions.ExpressionType.LessThan" /> and the <see cref="P:System.Linq.Expressions.BinaryExpression.Left" /> and <see cref="P:System.Linq.Expressions.BinaryExpression.Right" /> properties set to the specified values.</returns>
		/// <exception cref="T:System.ArgumentNullException">
		///         <paramref name="left" /> or <paramref name="right" /> is <see langword="null" />.</exception>
		/// <exception cref="T:System.InvalidOperationException">The "less than" operator is not defined for <paramref name="left" />.Type and <paramref name="right" />.Type.</exception>
		public static BinaryExpression LessThan(Expression left, Expression right)
		{
			return Expression.LessThan(left, right, false, null);
		}

		/// <summary>Creates a <see cref="T:System.Linq.Expressions.BinaryExpression" /> that represents a "less than" numeric comparison.</summary>
		/// <param name="left">An <see cref="T:System.Linq.Expressions.Expression" /> to set the <see cref="P:System.Linq.Expressions.BinaryExpression.Left" /> property equal to.</param>
		/// <param name="right">An <see cref="T:System.Linq.Expressions.Expression" /> to set the <see cref="P:System.Linq.Expressions.BinaryExpression.Right" /> property equal to.</param>
		/// <param name="liftToNull">
		///       <see langword="true" /> to set <see cref="P:System.Linq.Expressions.BinaryExpression.IsLiftedToNull" /> to <see langword="true" />; <see langword="false" /> to set <see cref="P:System.Linq.Expressions.BinaryExpression.IsLiftedToNull" /> to <see langword="false" />.</param>
		/// <param name="method">A <see cref="T:System.Reflection.MethodInfo" /> to set the <see cref="P:System.Linq.Expressions.BinaryExpression.Method" /> property equal to.</param>
		/// <returns>A <see cref="T:System.Linq.Expressions.BinaryExpression" /> that has the <see cref="P:System.Linq.Expressions.Expression.NodeType" /> property equal to <see cref="F:System.Linq.Expressions.ExpressionType.LessThan" /> and the <see cref="P:System.Linq.Expressions.BinaryExpression.Left" />, <see cref="P:System.Linq.Expressions.BinaryExpression.Right" />, <see cref="P:System.Linq.Expressions.BinaryExpression.IsLiftedToNull" />, and <see cref="P:System.Linq.Expressions.BinaryExpression.Method" /> properties set to the specified values.</returns>
		/// <exception cref="T:System.ArgumentNullException">
		///         <paramref name="left" /> or <paramref name="right" /> is <see langword="null" />.</exception>
		/// <exception cref="T:System.ArgumentException">
		///         <paramref name="method" /> is not <see langword="null" /> and the method it represents returns <see langword="void" />, is not <see langword="static" /> (<see langword="Shared" /> in Visual Basic), or does not take exactly two arguments.</exception>
		/// <exception cref="T:System.InvalidOperationException">
		///         <paramref name="method" /> is <see langword="null" /> and the "less than" operator is not defined for <paramref name="left" />.Type and <paramref name="right" />.Type.</exception>
		public static BinaryExpression LessThan(Expression left, Expression right, bool liftToNull, MethodInfo method)
		{
			ExpressionUtils.RequiresCanRead(left, "left");
			ExpressionUtils.RequiresCanRead(right, "right");
			if (method == null)
			{
				return Expression.GetComparisonOperator(ExpressionType.LessThan, "op_LessThan", left, right, liftToNull);
			}
			return Expression.GetMethodBasedBinaryOperator(ExpressionType.LessThan, left, right, method, liftToNull);
		}

		/// <summary>Creates a <see cref="T:System.Linq.Expressions.BinaryExpression" /> that represents a "greater than or equal" numeric comparison.</summary>
		/// <param name="left">An <see cref="T:System.Linq.Expressions.Expression" /> to set the <see cref="P:System.Linq.Expressions.BinaryExpression.Left" /> property equal to.</param>
		/// <param name="right">An <see cref="T:System.Linq.Expressions.Expression" /> to set the <see cref="P:System.Linq.Expressions.BinaryExpression.Right" /> property equal to.</param>
		/// <returns>A <see cref="T:System.Linq.Expressions.BinaryExpression" /> that has the <see cref="P:System.Linq.Expressions.Expression.NodeType" /> property equal to <see cref="F:System.Linq.Expressions.ExpressionType.GreaterThanOrEqual" /> and the <see cref="P:System.Linq.Expressions.BinaryExpression.Left" /> and <see cref="P:System.Linq.Expressions.BinaryExpression.Right" /> properties set to the specified values.</returns>
		/// <exception cref="T:System.ArgumentNullException">
		///         <paramref name="left" /> or <paramref name="right" /> is <see langword="null" />.</exception>
		/// <exception cref="T:System.InvalidOperationException">The "greater than or equal" operator is not defined for <paramref name="left" />.Type and <paramref name="right" />.Type.</exception>
		public static BinaryExpression GreaterThanOrEqual(Expression left, Expression right)
		{
			return Expression.GreaterThanOrEqual(left, right, false, null);
		}

		/// <summary>Creates a <see cref="T:System.Linq.Expressions.BinaryExpression" /> that represents a "greater than or equal" numeric comparison.</summary>
		/// <param name="left">An <see cref="T:System.Linq.Expressions.Expression" /> to set the <see cref="P:System.Linq.Expressions.BinaryExpression.Left" /> property equal to.</param>
		/// <param name="right">An <see cref="T:System.Linq.Expressions.Expression" /> to set the <see cref="P:System.Linq.Expressions.BinaryExpression.Right" /> property equal to.</param>
		/// <param name="liftToNull">
		///       <see langword="true" /> to set <see cref="P:System.Linq.Expressions.BinaryExpression.IsLiftedToNull" /> to <see langword="true" />; <see langword="false" /> to set <see cref="P:System.Linq.Expressions.BinaryExpression.IsLiftedToNull" /> to <see langword="false" />.</param>
		/// <param name="method">A <see cref="T:System.Reflection.MethodInfo" /> to set the <see cref="P:System.Linq.Expressions.BinaryExpression.Method" /> property equal to.</param>
		/// <returns>A <see cref="T:System.Linq.Expressions.BinaryExpression" /> that has the <see cref="P:System.Linq.Expressions.Expression.NodeType" /> property equal to <see cref="F:System.Linq.Expressions.ExpressionType.GreaterThanOrEqual" /> and the <see cref="P:System.Linq.Expressions.BinaryExpression.Left" />, <see cref="P:System.Linq.Expressions.BinaryExpression.Right" />, <see cref="P:System.Linq.Expressions.BinaryExpression.IsLiftedToNull" />, and <see cref="P:System.Linq.Expressions.BinaryExpression.Method" /> properties set to the specified values.</returns>
		/// <exception cref="T:System.ArgumentNullException">
		///         <paramref name="left" /> or <paramref name="right" /> is <see langword="null" />.</exception>
		/// <exception cref="T:System.ArgumentException">
		///         <paramref name="method" /> is not <see langword="null" /> and the method it represents returns <see langword="void" />, is not <see langword="static" /> (<see langword="Shared" /> in Visual Basic), or does not take exactly two arguments.</exception>
		/// <exception cref="T:System.InvalidOperationException">
		///         <paramref name="method" /> is <see langword="null" /> and the "greater than or equal" operator is not defined for <paramref name="left" />.Type and <paramref name="right" />.Type.</exception>
		public static BinaryExpression GreaterThanOrEqual(Expression left, Expression right, bool liftToNull, MethodInfo method)
		{
			ExpressionUtils.RequiresCanRead(left, "left");
			ExpressionUtils.RequiresCanRead(right, "right");
			if (method == null)
			{
				return Expression.GetComparisonOperator(ExpressionType.GreaterThanOrEqual, "op_GreaterThanOrEqual", left, right, liftToNull);
			}
			return Expression.GetMethodBasedBinaryOperator(ExpressionType.GreaterThanOrEqual, left, right, method, liftToNull);
		}

		/// <summary>Creates a <see cref="T:System.Linq.Expressions.BinaryExpression" /> that represents a " less than or equal" numeric comparison.</summary>
		/// <param name="left">An <see cref="T:System.Linq.Expressions.Expression" /> to set the <see cref="P:System.Linq.Expressions.BinaryExpression.Left" /> property equal to.</param>
		/// <param name="right">An <see cref="T:System.Linq.Expressions.Expression" /> to set the <see cref="P:System.Linq.Expressions.BinaryExpression.Right" /> property equal to.</param>
		/// <returns>A <see cref="T:System.Linq.Expressions.BinaryExpression" /> that has the <see cref="P:System.Linq.Expressions.Expression.NodeType" /> property equal to <see cref="F:System.Linq.Expressions.ExpressionType.LessThanOrEqual" /> and the <see cref="P:System.Linq.Expressions.BinaryExpression.Left" /> and <see cref="P:System.Linq.Expressions.BinaryExpression.Right" /> properties set to the specified values.</returns>
		/// <exception cref="T:System.ArgumentNullException">
		///         <paramref name="left" /> or <paramref name="right" /> is <see langword="null" />.</exception>
		/// <exception cref="T:System.InvalidOperationException">The "less than or equal" operator is not defined for <paramref name="left" />.Type and <paramref name="right" />.Type.</exception>
		public static BinaryExpression LessThanOrEqual(Expression left, Expression right)
		{
			return Expression.LessThanOrEqual(left, right, false, null);
		}

		/// <summary>Creates a <see cref="T:System.Linq.Expressions.BinaryExpression" /> that represents a "less than or equal" numeric comparison.</summary>
		/// <param name="left">An <see cref="T:System.Linq.Expressions.Expression" /> to set the <see cref="P:System.Linq.Expressions.BinaryExpression.Left" /> property equal to.</param>
		/// <param name="right">An <see cref="T:System.Linq.Expressions.Expression" /> to set the <see cref="P:System.Linq.Expressions.BinaryExpression.Right" /> property equal to.</param>
		/// <param name="liftToNull">
		///       <see langword="true" /> to set <see cref="P:System.Linq.Expressions.BinaryExpression.IsLiftedToNull" /> to <see langword="true" />; <see langword="false" /> to set <see cref="P:System.Linq.Expressions.BinaryExpression.IsLiftedToNull" /> to <see langword="false" />.</param>
		/// <param name="method">A <see cref="T:System.Reflection.MethodInfo" /> to set the <see cref="P:System.Linq.Expressions.BinaryExpression.Method" /> property equal to.</param>
		/// <returns>A <see cref="T:System.Linq.Expressions.BinaryExpression" /> that has the <see cref="P:System.Linq.Expressions.Expression.NodeType" /> property equal to <see cref="F:System.Linq.Expressions.ExpressionType.LessThanOrEqual" /> and the <see cref="P:System.Linq.Expressions.BinaryExpression.Left" />, <see cref="P:System.Linq.Expressions.BinaryExpression.Right" />, <see cref="P:System.Linq.Expressions.BinaryExpression.IsLiftedToNull" />, and <see cref="P:System.Linq.Expressions.BinaryExpression.Method" /> properties set to the specified values.</returns>
		/// <exception cref="T:System.ArgumentNullException">
		///         <paramref name="left" /> or <paramref name="right" /> is <see langword="null" />.</exception>
		/// <exception cref="T:System.ArgumentException">
		///         <paramref name="method" /> is not <see langword="null" /> and the method it represents returns <see langword="void" />, is not <see langword="static" /> (<see langword="Shared" /> in Visual Basic), or does not take exactly two arguments.</exception>
		/// <exception cref="T:System.InvalidOperationException">
		///         <paramref name="method" /> is <see langword="null" /> and the "less than or equal" operator is not defined for <paramref name="left" />.Type and <paramref name="right" />.Type.</exception>
		public static BinaryExpression LessThanOrEqual(Expression left, Expression right, bool liftToNull, MethodInfo method)
		{
			ExpressionUtils.RequiresCanRead(left, "left");
			ExpressionUtils.RequiresCanRead(right, "right");
			if (method == null)
			{
				return Expression.GetComparisonOperator(ExpressionType.LessThanOrEqual, "op_LessThanOrEqual", left, right, liftToNull);
			}
			return Expression.GetMethodBasedBinaryOperator(ExpressionType.LessThanOrEqual, left, right, method, liftToNull);
		}

		private static BinaryExpression GetComparisonOperator(ExpressionType binaryType, string opName, Expression left, Expression right, bool liftToNull)
		{
			if (!(left.Type == right.Type) || !left.Type.IsNumeric())
			{
				return Expression.GetUserDefinedBinaryOperatorOrThrow(binaryType, opName, left, right, liftToNull);
			}
			if (left.Type.IsNullableType() && liftToNull)
			{
				return new SimpleBinaryExpression(binaryType, left, right, typeof(bool?));
			}
			return new LogicalBinaryExpression(binaryType, left, right);
		}

		/// <summary>Creates a <see cref="T:System.Linq.Expressions.BinaryExpression" /> that represents a conditional <see langword="AND" /> operation that evaluates the second operand only if the first operand evaluates to <see langword="true" />.</summary>
		/// <param name="left">A <see cref="T:System.Linq.Expressions.Expression" /> to set the <see cref="P:System.Linq.Expressions.BinaryExpression.Left" /> property equal to.</param>
		/// <param name="right">A <see cref="T:System.Linq.Expressions.Expression" /> to set the <see cref="P:System.Linq.Expressions.BinaryExpression.Right" /> property equal to.</param>
		/// <returns>A <see cref="T:System.Linq.Expressions.BinaryExpression" /> that has the <see cref="P:System.Linq.Expressions.Expression.NodeType" /> property equal to <see cref="F:System.Linq.Expressions.ExpressionType.AndAlso" /> and the <see cref="P:System.Linq.Expressions.BinaryExpression.Left" /> and <see cref="P:System.Linq.Expressions.BinaryExpression.Right" /> properties set to the specified values.</returns>
		/// <exception cref="T:System.ArgumentNullException">
		///         <paramref name="left" /> or <paramref name="right" /> is <see langword="null" />.</exception>
		/// <exception cref="T:System.InvalidOperationException">The bitwise <see langword="AND" /> operator is not defined for <paramref name="left" />.Type and <paramref name="right" />.Type.-or-
		///         <paramref name="left" />.Type and <paramref name="right" />.Type are not the same Boolean type.</exception>
		public static BinaryExpression AndAlso(Expression left, Expression right)
		{
			return Expression.AndAlso(left, right, null);
		}

		/// <summary>Creates a <see cref="T:System.Linq.Expressions.BinaryExpression" /> that represents a conditional <see langword="AND" /> operation that evaluates the second operand only if the first operand is resolved to true. The implementing method can be specified.</summary>
		/// <param name="left">A <see cref="T:System.Linq.Expressions.Expression" /> to set the <see cref="P:System.Linq.Expressions.BinaryExpression.Left" /> property equal to.</param>
		/// <param name="right">A <see cref="T:System.Linq.Expressions.Expression" /> to set the <see cref="P:System.Linq.Expressions.BinaryExpression.Right" /> property equal to.</param>
		/// <param name="method">A <see cref="T:System.Reflection.MethodInfo" /> to set the <see cref="P:System.Linq.Expressions.BinaryExpression.Method" /> property equal to.</param>
		/// <returns>A <see cref="T:System.Linq.Expressions.BinaryExpression" /> that has the <see cref="P:System.Linq.Expressions.Expression.NodeType" /> property equal to <see cref="F:System.Linq.Expressions.ExpressionType.AndAlso" /> and the <see cref="P:System.Linq.Expressions.BinaryExpression.Left" />, <see cref="P:System.Linq.Expressions.BinaryExpression.Right" />, and <see cref="P:System.Linq.Expressions.BinaryExpression.Method" /> properties set to the specified values.</returns>
		/// <exception cref="T:System.ArgumentNullException">
		///         <paramref name="left" /> or <paramref name="right" /> is <see langword="null" />.</exception>
		/// <exception cref="T:System.ArgumentException">
		///         <paramref name="method" /> is not <see langword="null" /> and the method it represents returns <see langword="void" />, is not <see langword="static" /> (<see langword="Shared" /> in Visual Basic), or does not take exactly two arguments.</exception>
		/// <exception cref="T:System.InvalidOperationException">
		///         <paramref name="method" /> is <see langword="null" /> and the bitwise <see langword="AND" /> operator is not defined for <paramref name="left" />.Type and <paramref name="right" />.Type.-or-
		///         <paramref name="method" /> is <see langword="null" /> and <paramref name="left" />.Type and <paramref name="right" />.Type are not the same Boolean type.</exception>
		public static BinaryExpression AndAlso(Expression left, Expression right, MethodInfo method)
		{
			ExpressionUtils.RequiresCanRead(left, "left");
			ExpressionUtils.RequiresCanRead(right, "right");
			if (!(method == null))
			{
				Expression.ValidateUserDefinedConditionalLogicOperator(ExpressionType.AndAlso, left.Type, right.Type, method);
				Type type = (left.Type.IsNullableType() && TypeUtils.AreEquivalent(method.ReturnType, left.Type.GetNonNullableType())) ? left.Type : method.ReturnType;
				return new MethodBinaryExpression(ExpressionType.AndAlso, left, right, type, method);
			}
			if (left.Type == right.Type)
			{
				if (left.Type == typeof(bool))
				{
					return new LogicalBinaryExpression(ExpressionType.AndAlso, left, right);
				}
				if (left.Type == typeof(bool?))
				{
					return new SimpleBinaryExpression(ExpressionType.AndAlso, left, right, left.Type);
				}
			}
			method = Expression.GetUserDefinedBinaryOperator(ExpressionType.AndAlso, left.Type, right.Type, "op_BitwiseAnd");
			if (method != null)
			{
				Expression.ValidateUserDefinedConditionalLogicOperator(ExpressionType.AndAlso, left.Type, right.Type, method);
				Type type = (left.Type.IsNullableType() && TypeUtils.AreEquivalent(method.ReturnType, left.Type.GetNonNullableType())) ? left.Type : method.ReturnType;
				return new MethodBinaryExpression(ExpressionType.AndAlso, left, right, type, method);
			}
			throw Error.BinaryOperatorNotDefined(ExpressionType.AndAlso, left.Type, right.Type);
		}

		/// <summary>Creates a <see cref="T:System.Linq.Expressions.BinaryExpression" /> that represents a conditional <see langword="OR" /> operation that evaluates the second operand only if the first operand evaluates to <see langword="false" />.</summary>
		/// <param name="left">An <see cref="T:System.Linq.Expressions.Expression" /> to set the <see cref="P:System.Linq.Expressions.BinaryExpression.Left" /> property equal to.</param>
		/// <param name="right">An <see cref="T:System.Linq.Expressions.Expression" /> to set the <see cref="P:System.Linq.Expressions.BinaryExpression.Right" /> property equal to.</param>
		/// <returns>A <see cref="T:System.Linq.Expressions.BinaryExpression" /> that has the <see cref="P:System.Linq.Expressions.Expression.NodeType" /> property equal to <see cref="F:System.Linq.Expressions.ExpressionType.OrElse" /> and the <see cref="P:System.Linq.Expressions.BinaryExpression.Left" /> and <see cref="P:System.Linq.Expressions.BinaryExpression.Right" /> properties set to the specified values.</returns>
		/// <exception cref="T:System.ArgumentNullException">
		///         <paramref name="left" /> or <paramref name="right" /> is <see langword="null" />.</exception>
		/// <exception cref="T:System.InvalidOperationException">The bitwise <see langword="OR" /> operator is not defined for <paramref name="left" />.Type and <paramref name="right" />.Type.-or-
		///         <paramref name="left" />.Type and <paramref name="right" />.Type are not the same Boolean type.</exception>
		public static BinaryExpression OrElse(Expression left, Expression right)
		{
			return Expression.OrElse(left, right, null);
		}

		/// <summary>Creates a <see cref="T:System.Linq.Expressions.BinaryExpression" /> that represents a conditional <see langword="OR" /> operation that evaluates the second operand only if the first operand evaluates to <see langword="false" />.</summary>
		/// <param name="left">An <see cref="T:System.Linq.Expressions.Expression" /> to set the <see cref="P:System.Linq.Expressions.BinaryExpression.Left" /> property equal to.</param>
		/// <param name="right">An <see cref="T:System.Linq.Expressions.Expression" /> to set the <see cref="P:System.Linq.Expressions.BinaryExpression.Right" /> property equal to.</param>
		/// <param name="method">A <see cref="T:System.Reflection.MethodInfo" /> to set the <see cref="P:System.Linq.Expressions.BinaryExpression.Method" /> property equal to.</param>
		/// <returns>A <see cref="T:System.Linq.Expressions.BinaryExpression" /> that has the <see cref="P:System.Linq.Expressions.Expression.NodeType" /> property equal to <see cref="F:System.Linq.Expressions.ExpressionType.OrElse" /> and the <see cref="P:System.Linq.Expressions.BinaryExpression.Left" />, <see cref="P:System.Linq.Expressions.BinaryExpression.Right" />, and <see cref="P:System.Linq.Expressions.BinaryExpression.Method" /> properties set to the specified values.</returns>
		/// <exception cref="T:System.ArgumentNullException">
		///         <paramref name="left" /> or <paramref name="right" /> is <see langword="null" />.</exception>
		/// <exception cref="T:System.ArgumentException">
		///         <paramref name="method" /> is not <see langword="null" /> and the method it represents returns <see langword="void" />, is not <see langword="static" /> (<see langword="Shared" /> in Visual Basic), or does not take exactly two arguments.</exception>
		/// <exception cref="T:System.InvalidOperationException">
		///         <paramref name="method" /> is <see langword="null" /> and the bitwise <see langword="OR" /> operator is not defined for <paramref name="left" />.Type and <paramref name="right" />.Type.-or-
		///         <paramref name="method" /> is <see langword="null" /> and <paramref name="left" />.Type and <paramref name="right" />.Type are not the same Boolean type.</exception>
		public static BinaryExpression OrElse(Expression left, Expression right, MethodInfo method)
		{
			ExpressionUtils.RequiresCanRead(left, "left");
			ExpressionUtils.RequiresCanRead(right, "right");
			if (!(method == null))
			{
				Expression.ValidateUserDefinedConditionalLogicOperator(ExpressionType.OrElse, left.Type, right.Type, method);
				Type type = (left.Type.IsNullableType() && method.ReturnType == left.Type.GetNonNullableType()) ? left.Type : method.ReturnType;
				return new MethodBinaryExpression(ExpressionType.OrElse, left, right, type, method);
			}
			if (left.Type == right.Type)
			{
				if (left.Type == typeof(bool))
				{
					return new LogicalBinaryExpression(ExpressionType.OrElse, left, right);
				}
				if (left.Type == typeof(bool?))
				{
					return new SimpleBinaryExpression(ExpressionType.OrElse, left, right, left.Type);
				}
			}
			method = Expression.GetUserDefinedBinaryOperator(ExpressionType.OrElse, left.Type, right.Type, "op_BitwiseOr");
			if (method != null)
			{
				Expression.ValidateUserDefinedConditionalLogicOperator(ExpressionType.OrElse, left.Type, right.Type, method);
				Type type = (left.Type.IsNullableType() && method.ReturnType == left.Type.GetNonNullableType()) ? left.Type : method.ReturnType;
				return new MethodBinaryExpression(ExpressionType.OrElse, left, right, type, method);
			}
			throw Error.BinaryOperatorNotDefined(ExpressionType.OrElse, left.Type, right.Type);
		}

		/// <summary>Creates a <see cref="T:System.Linq.Expressions.BinaryExpression" /> that represents a coalescing operation.</summary>
		/// <param name="left">An <see cref="T:System.Linq.Expressions.Expression" /> to set the <see cref="P:System.Linq.Expressions.BinaryExpression.Left" /> property equal to.</param>
		/// <param name="right">An <see cref="T:System.Linq.Expressions.Expression" /> to set the <see cref="P:System.Linq.Expressions.BinaryExpression.Right" /> property equal to.</param>
		/// <returns>A <see cref="T:System.Linq.Expressions.BinaryExpression" /> that has the <see cref="P:System.Linq.Expressions.Expression.NodeType" /> property equal to <see cref="F:System.Linq.Expressions.ExpressionType.Coalesce" /> and the <see cref="P:System.Linq.Expressions.BinaryExpression.Left" /> and <see cref="P:System.Linq.Expressions.BinaryExpression.Right" /> properties set to the specified values.</returns>
		/// <exception cref="T:System.ArgumentNullException">
		///         <paramref name="left" /> or <paramref name="right" /> is <see langword="null" />.</exception>
		/// <exception cref="T:System.InvalidOperationException">The <see cref="P:System.Linq.Expressions.Expression.Type" /> property of <paramref name="left" /> does not represent a reference type or a nullable value type.</exception>
		/// <exception cref="T:System.ArgumentException">
		///         <paramref name="left" />.Type and <paramref name="right" />.Type are not convertible to each other.</exception>
		public static BinaryExpression Coalesce(Expression left, Expression right)
		{
			return Expression.Coalesce(left, right, null);
		}

		/// <summary>Creates a <see cref="T:System.Linq.Expressions.BinaryExpression" /> that represents a coalescing operation, given a conversion function.</summary>
		/// <param name="left">An <see cref="T:System.Linq.Expressions.Expression" /> to set the <see cref="P:System.Linq.Expressions.BinaryExpression.Left" /> property equal to.</param>
		/// <param name="right">An <see cref="T:System.Linq.Expressions.Expression" /> to set the <see cref="P:System.Linq.Expressions.BinaryExpression.Right" /> property equal to.</param>
		/// <param name="conversion">A <see cref="T:System.Linq.Expressions.LambdaExpression" /> to set the <see cref="P:System.Linq.Expressions.BinaryExpression.Conversion" /> property equal to.</param>
		/// <returns>A <see cref="T:System.Linq.Expressions.BinaryExpression" /> that has the <see cref="P:System.Linq.Expressions.Expression.NodeType" /> property equal to <see cref="F:System.Linq.Expressions.ExpressionType.Coalesce" /> and the <see cref="P:System.Linq.Expressions.BinaryExpression.Left" />, <see cref="P:System.Linq.Expressions.BinaryExpression.Right" /> and <see cref="P:System.Linq.Expressions.BinaryExpression.Conversion" /> properties set to the specified values.</returns>
		/// <exception cref="T:System.ArgumentNullException">
		///         <paramref name="left" /> or <paramref name="right" /> is <see langword="null" />.</exception>
		/// <exception cref="T:System.ArgumentException">
		///         <paramref name="left" />.Type and <paramref name="right" />.Type are not convertible to each other.-or-
		///         <paramref name="conversion" /> is not <see langword="null" /> and <paramref name="conversion" />.Type is a delegate type that does not take exactly one argument.</exception>
		/// <exception cref="T:System.InvalidOperationException">The <see cref="P:System.Linq.Expressions.Expression.Type" /> property of <paramref name="left" /> does not represent a reference type or a nullable value type.-or-The <see cref="P:System.Linq.Expressions.Expression.Type" /> property of <paramref name="left" /> represents a type that is not assignable to the parameter type of the delegate type <paramref name="conversion" />.Type.-or-The <see cref="P:System.Linq.Expressions.Expression.Type" /> property of <paramref name="right" /> is not equal to the return type of the delegate type <paramref name="conversion" />.Type.</exception>
		public static BinaryExpression Coalesce(Expression left, Expression right, LambdaExpression conversion)
		{
			ExpressionUtils.RequiresCanRead(left, "left");
			ExpressionUtils.RequiresCanRead(right, "right");
			if (conversion == null)
			{
				Type type = Expression.ValidateCoalesceArgTypes(left.Type, right.Type);
				return new SimpleBinaryExpression(ExpressionType.Coalesce, left, right, type);
			}
			if (left.Type.IsValueType && !left.Type.IsNullableType())
			{
				throw Error.CoalesceUsedOnNonNullType();
			}
			MethodInfo invokeMethod = conversion.Type.GetInvokeMethod();
			if (invokeMethod.ReturnType == typeof(void))
			{
				throw Error.UserDefinedOperatorMustNotBeVoid(conversion, "conversion");
			}
			ParameterInfo[] parametersCached = invokeMethod.GetParametersCached();
			if (parametersCached.Length != 1)
			{
				throw Error.IncorrectNumberOfMethodCallArguments(conversion, "conversion");
			}
			if (!TypeUtils.AreEquivalent(invokeMethod.ReturnType, right.Type))
			{
				throw Error.OperandTypesDoNotMatchParameters(ExpressionType.Coalesce, conversion.ToString());
			}
			if (!Expression.ParameterIsAssignable(parametersCached[0], left.Type.GetNonNullableType()) && !Expression.ParameterIsAssignable(parametersCached[0], left.Type))
			{
				throw Error.OperandTypesDoNotMatchParameters(ExpressionType.Coalesce, conversion.ToString());
			}
			return new CoalesceConversionBinaryExpression(left, right, conversion);
		}

		private static Type ValidateCoalesceArgTypes(Type left, Type right)
		{
			Type nonNullableType = left.GetNonNullableType();
			if (left.IsValueType && !left.IsNullableType())
			{
				throw Error.CoalesceUsedOnNonNullType();
			}
			if (left.IsNullableType() && right.IsImplicitlyConvertibleTo(nonNullableType))
			{
				return nonNullableType;
			}
			if (right.IsImplicitlyConvertibleTo(left))
			{
				return left;
			}
			if (nonNullableType.IsImplicitlyConvertibleTo(right))
			{
				return right;
			}
			throw Error.ArgumentTypesMustMatch();
		}

		/// <summary>Creates a <see cref="T:System.Linq.Expressions.BinaryExpression" /> that represents an arithmetic addition operation that does not have overflow checking.</summary>
		/// <param name="left">A <see cref="T:System.Linq.Expressions.Expression" /> to set the <see cref="P:System.Linq.Expressions.BinaryExpression.Left" /> property equal to.</param>
		/// <param name="right">A <see cref="T:System.Linq.Expressions.Expression" /> to set the <see cref="P:System.Linq.Expressions.BinaryExpression.Right" /> property equal to.</param>
		/// <returns>A <see cref="T:System.Linq.Expressions.BinaryExpression" /> that has the <see cref="P:System.Linq.Expressions.Expression.NodeType" /> property equal to <see cref="F:System.Linq.Expressions.ExpressionType.Add" /> and the <see cref="P:System.Linq.Expressions.BinaryExpression.Left" /> and <see cref="P:System.Linq.Expressions.BinaryExpression.Right" /> properties set to the specified values.</returns>
		/// <exception cref="T:System.ArgumentNullException">
		///         <paramref name="left" /> or <paramref name="right" /> is <see langword="null" />.</exception>
		/// <exception cref="T:System.InvalidOperationException">The addition operator is not defined for <paramref name="left" />.Type and <paramref name="right" />.Type.</exception>
		public static BinaryExpression Add(Expression left, Expression right)
		{
			return Expression.Add(left, right, null);
		}

		/// <summary>Creates a <see cref="T:System.Linq.Expressions.BinaryExpression" /> that represents an arithmetic addition operation that does not have overflow checking. The implementing method can be specified.</summary>
		/// <param name="left">A <see cref="T:System.Linq.Expressions.Expression" /> to set the <see cref="P:System.Linq.Expressions.BinaryExpression.Left" /> property equal to.</param>
		/// <param name="right">A <see cref="T:System.Linq.Expressions.Expression" /> to set the <see cref="P:System.Linq.Expressions.BinaryExpression.Right" /> property equal to.</param>
		/// <param name="method">A <see cref="T:System.Reflection.MethodInfo" /> to set the <see cref="P:System.Linq.Expressions.BinaryExpression.Method" /> property equal to.</param>
		/// <returns>A <see cref="T:System.Linq.Expressions.BinaryExpression" /> that has the <see cref="P:System.Linq.Expressions.Expression.NodeType" /> property equal to <see cref="F:System.Linq.Expressions.ExpressionType.Add" /> and the <see cref="P:System.Linq.Expressions.BinaryExpression.Left" />, <see cref="P:System.Linq.Expressions.BinaryExpression.Right" /> and <see cref="P:System.Linq.Expressions.BinaryExpression.Method" /> properties set to the specified values.</returns>
		/// <exception cref="T:System.ArgumentNullException">
		///         <paramref name="left" /> or <paramref name="right" /> is <see langword="null" />.</exception>
		/// <exception cref="T:System.ArgumentException">
		///         <paramref name="method" /> is not <see langword="null" /> and the method it represents returns <see langword="void" />, is not <see langword="static" /> (<see langword="Shared" /> in Visual Basic), or does not take exactly two arguments.</exception>
		/// <exception cref="T:System.InvalidOperationException">
		///         <paramref name="method" /> is <see langword="null" /> and the addition operator is not defined for <paramref name="left" />.Type and <paramref name="right" />.Type.</exception>
		public static BinaryExpression Add(Expression left, Expression right, MethodInfo method)
		{
			ExpressionUtils.RequiresCanRead(left, "left");
			ExpressionUtils.RequiresCanRead(right, "right");
			if (!(method == null))
			{
				return Expression.GetMethodBasedBinaryOperator(ExpressionType.Add, left, right, method, true);
			}
			if (left.Type == right.Type && left.Type.IsArithmetic())
			{
				return new SimpleBinaryExpression(ExpressionType.Add, left, right, left.Type);
			}
			return Expression.GetUserDefinedBinaryOperatorOrThrow(ExpressionType.Add, "op_Addition", left, right, true);
		}

		/// <summary>Creates a <see cref="T:System.Linq.Expressions.BinaryExpression" /> that represents an addition assignment operation that does not have overflow checking.</summary>
		/// <param name="left">An <see cref="T:System.Linq.Expressions.Expression" /> to set the <see cref="P:System.Linq.Expressions.BinaryExpression.Left" /> property equal to.</param>
		/// <param name="right">An <see cref="T:System.Linq.Expressions.Expression" /> to set the <see cref="P:System.Linq.Expressions.BinaryExpression.Right" /> property equal to.</param>
		/// <returns>A <see cref="T:System.Linq.Expressions.BinaryExpression" /> that has the <see cref="P:System.Linq.Expressions.Expression.NodeType" /> property equal to <see cref="F:System.Linq.Expressions.ExpressionType.AddAssign" /> and the <see cref="P:System.Linq.Expressions.BinaryExpression.Left" /> and <see cref="P:System.Linq.Expressions.BinaryExpression.Right" /> properties set to the specified values.</returns>
		public static BinaryExpression AddAssign(Expression left, Expression right)
		{
			return Expression.AddAssign(left, right, null, null);
		}

		/// <summary>Creates a <see cref="T:System.Linq.Expressions.BinaryExpression" /> that represents an addition assignment operation that does not have overflow checking.</summary>
		/// <param name="left">An <see cref="T:System.Linq.Expressions.Expression" /> to set the <see cref="P:System.Linq.Expressions.BinaryExpression.Left" /> property equal to.</param>
		/// <param name="right">An <see cref="T:System.Linq.Expressions.Expression" /> to set the <see cref="P:System.Linq.Expressions.BinaryExpression.Right" /> property equal to.</param>
		/// <param name="method">A <see cref="T:System.Reflection.MethodInfo" /> to set the <see cref="P:System.Linq.Expressions.BinaryExpression.Method" /> property equal to.</param>
		/// <returns>A <see cref="T:System.Linq.Expressions.BinaryExpression" /> that has the <see cref="P:System.Linq.Expressions.Expression.NodeType" /> property equal to <see cref="F:System.Linq.Expressions.ExpressionType.AddAssign" /> and the <see cref="P:System.Linq.Expressions.BinaryExpression.Left" />, <see cref="P:System.Linq.Expressions.BinaryExpression.Right" />, and <see cref="P:System.Linq.Expressions.BinaryExpression.Method" /> properties set to the specified values.</returns>
		public static BinaryExpression AddAssign(Expression left, Expression right, MethodInfo method)
		{
			return Expression.AddAssign(left, right, method, null);
		}

		/// <summary>Creates a <see cref="T:System.Linq.Expressions.BinaryExpression" /> that represents an addition assignment operation that does not have overflow checking.</summary>
		/// <param name="left">An <see cref="T:System.Linq.Expressions.Expression" /> to set the <see cref="P:System.Linq.Expressions.BinaryExpression.Left" /> property equal to.</param>
		/// <param name="right">An <see cref="T:System.Linq.Expressions.Expression" /> to set the <see cref="P:System.Linq.Expressions.BinaryExpression.Right" /> property equal to.</param>
		/// <param name="method">A <see cref="T:System.Reflection.MethodInfo" /> to set the <see cref="P:System.Linq.Expressions.BinaryExpression.Method" /> property equal to.</param>
		/// <param name="conversion">A <see cref="T:System.Linq.Expressions.LambdaExpression" /> to set the <see cref="P:System.Linq.Expressions.BinaryExpression.Conversion" /> property equal to.</param>
		/// <returns>A <see cref="T:System.Linq.Expressions.BinaryExpression" /> that has the <see cref="P:System.Linq.Expressions.Expression.NodeType" /> property equal to <see cref="F:System.Linq.Expressions.ExpressionType.AddAssign" /> and the <see cref="P:System.Linq.Expressions.BinaryExpression.Left" />, <see cref="P:System.Linq.Expressions.BinaryExpression.Right" />, <see cref="P:System.Linq.Expressions.BinaryExpression.Method" />, and <see cref="P:System.Linq.Expressions.BinaryExpression.Conversion" /> properties set to the specified values.</returns>
		public static BinaryExpression AddAssign(Expression left, Expression right, MethodInfo method, LambdaExpression conversion)
		{
			ExpressionUtils.RequiresCanRead(left, "left");
			Expression.RequiresCanWrite(left, "left");
			ExpressionUtils.RequiresCanRead(right, "right");
			if (!(method == null))
			{
				return Expression.GetMethodBasedAssignOperator(ExpressionType.AddAssign, left, right, method, conversion, true);
			}
			if (!(left.Type == right.Type) || !left.Type.IsArithmetic())
			{
				return Expression.GetUserDefinedAssignOperatorOrThrow(ExpressionType.AddAssign, "op_Addition", left, right, conversion, true);
			}
			if (conversion != null)
			{
				throw Error.ConversionIsNotSupportedForArithmeticTypes();
			}
			return new SimpleBinaryExpression(ExpressionType.AddAssign, left, right, left.Type);
		}

		private static void ValidateOpAssignConversionLambda(LambdaExpression conversion, Expression left, MethodInfo method, ExpressionType nodeType)
		{
			MethodInfo invokeMethod = conversion.Type.GetInvokeMethod();
			ParameterInfo[] parametersCached = invokeMethod.GetParametersCached();
			if (parametersCached.Length != 1)
			{
				throw Error.IncorrectNumberOfMethodCallArguments(conversion, "conversion");
			}
			if (!TypeUtils.AreEquivalent(invokeMethod.ReturnType, left.Type))
			{
				throw Error.OperandTypesDoNotMatchParameters(nodeType, conversion.ToString());
			}
			if (!TypeUtils.AreEquivalent(parametersCached[0].ParameterType, method.ReturnType))
			{
				throw Error.OverloadOperatorTypeDoesNotMatchConversionType(nodeType, conversion.ToString());
			}
		}

		/// <summary>Creates a <see cref="T:System.Linq.Expressions.BinaryExpression" /> that represents an addition assignment operation that has overflow checking.</summary>
		/// <param name="left">An <see cref="T:System.Linq.Expressions.Expression" /> to set the <see cref="P:System.Linq.Expressions.BinaryExpression.Left" /> property equal to.</param>
		/// <param name="right">An <see cref="T:System.Linq.Expressions.Expression" /> to set the <see cref="P:System.Linq.Expressions.BinaryExpression.Right" /> property equal to.</param>
		/// <returns>A <see cref="T:System.Linq.Expressions.BinaryExpression" /> that has the <see cref="P:System.Linq.Expressions.Expression.NodeType" /> property equal to <see cref="F:System.Linq.Expressions.ExpressionType.AddAssignChecked" /> and the <see cref="P:System.Linq.Expressions.BinaryExpression.Left" /> and <see cref="P:System.Linq.Expressions.BinaryExpression.Right" /> properties set to the specified values.</returns>
		public static BinaryExpression AddAssignChecked(Expression left, Expression right)
		{
			return Expression.AddAssignChecked(left, right, null);
		}

		/// <summary>Creates a <see cref="T:System.Linq.Expressions.BinaryExpression" /> that represents an addition assignment operation that has overflow checking.</summary>
		/// <param name="left">An <see cref="T:System.Linq.Expressions.Expression" /> to set the <see cref="P:System.Linq.Expressions.BinaryExpression.Left" /> property equal to.</param>
		/// <param name="right">An <see cref="T:System.Linq.Expressions.Expression" /> to set the <see cref="P:System.Linq.Expressions.BinaryExpression.Right" /> property equal to.</param>
		/// <param name="method">A <see cref="T:System.Reflection.MethodInfo" /> to set the <see cref="P:System.Linq.Expressions.BinaryExpression.Method" /> property equal to.</param>
		/// <returns>A <see cref="T:System.Linq.Expressions.BinaryExpression" /> that has the <see cref="P:System.Linq.Expressions.Expression.NodeType" /> property equal to <see cref="F:System.Linq.Expressions.ExpressionType.AddAssignChecked" /> and the <see cref="P:System.Linq.Expressions.BinaryExpression.Left" />, <see cref="P:System.Linq.Expressions.BinaryExpression.Right" />, and <see cref="P:System.Linq.Expressions.BinaryExpression.Method" /> properties set to the specified values.</returns>
		public static BinaryExpression AddAssignChecked(Expression left, Expression right, MethodInfo method)
		{
			return Expression.AddAssignChecked(left, right, method, null);
		}

		/// <summary>Creates a <see cref="T:System.Linq.Expressions.BinaryExpression" /> that represents an addition assignment operation that has overflow checking.</summary>
		/// <param name="left">An <see cref="T:System.Linq.Expressions.Expression" /> to set the <see cref="P:System.Linq.Expressions.BinaryExpression.Left" /> property equal to.</param>
		/// <param name="right">An <see cref="T:System.Linq.Expressions.Expression" /> to set the <see cref="P:System.Linq.Expressions.BinaryExpression.Right" /> property equal to.</param>
		/// <param name="method">A <see cref="T:System.Reflection.MethodInfo" /> to set the <see cref="P:System.Linq.Expressions.BinaryExpression.Method" /> property equal to.</param>
		/// <param name="conversion">A <see cref="T:System.Linq.Expressions.LambdaExpression" /> to set the <see cref="P:System.Linq.Expressions.BinaryExpression.Conversion" /> property equal to.</param>
		/// <returns>A <see cref="T:System.Linq.Expressions.BinaryExpression" /> that has the <see cref="P:System.Linq.Expressions.Expression.NodeType" /> property equal to <see cref="F:System.Linq.Expressions.ExpressionType.AddAssignChecked" /> and the <see cref="P:System.Linq.Expressions.BinaryExpression.Left" />, <see cref="P:System.Linq.Expressions.BinaryExpression.Right" />, <see cref="P:System.Linq.Expressions.BinaryExpression.Method" />, and <see cref="P:System.Linq.Expressions.BinaryExpression.Conversion" /> properties set to the specified values.</returns>
		public static BinaryExpression AddAssignChecked(Expression left, Expression right, MethodInfo method, LambdaExpression conversion)
		{
			ExpressionUtils.RequiresCanRead(left, "left");
			Expression.RequiresCanWrite(left, "left");
			ExpressionUtils.RequiresCanRead(right, "right");
			if (!(method == null))
			{
				return Expression.GetMethodBasedAssignOperator(ExpressionType.AddAssignChecked, left, right, method, conversion, true);
			}
			if (!(left.Type == right.Type) || !left.Type.IsArithmetic())
			{
				return Expression.GetUserDefinedAssignOperatorOrThrow(ExpressionType.AddAssignChecked, "op_Addition", left, right, conversion, true);
			}
			if (conversion != null)
			{
				throw Error.ConversionIsNotSupportedForArithmeticTypes();
			}
			return new SimpleBinaryExpression(ExpressionType.AddAssignChecked, left, right, left.Type);
		}

		/// <summary>Creates a <see cref="T:System.Linq.Expressions.BinaryExpression" /> that represents an arithmetic addition operation that has overflow checking.</summary>
		/// <param name="left">A <see cref="T:System.Linq.Expressions.Expression" /> to set the <see cref="P:System.Linq.Expressions.BinaryExpression.Left" /> property equal to.</param>
		/// <param name="right">A <see cref="T:System.Linq.Expressions.Expression" /> to set the <see cref="P:System.Linq.Expressions.BinaryExpression.Right" /> property equal to.</param>
		/// <returns>A <see cref="T:System.Linq.Expressions.BinaryExpression" /> that has the <see cref="P:System.Linq.Expressions.Expression.NodeType" /> property equal to <see cref="F:System.Linq.Expressions.ExpressionType.AddChecked" /> and the <see cref="P:System.Linq.Expressions.BinaryExpression.Left" /> and <see cref="P:System.Linq.Expressions.BinaryExpression.Right" /> properties set to the specified values.</returns>
		/// <exception cref="T:System.ArgumentNullException">
		///         <paramref name="left" /> or <paramref name="right" /> is <see langword="null" />.</exception>
		/// <exception cref="T:System.InvalidOperationException">The addition operator is not defined for <paramref name="left" />.Type and <paramref name="right" />.Type.</exception>
		public static BinaryExpression AddChecked(Expression left, Expression right)
		{
			return Expression.AddChecked(left, right, null);
		}

		/// <summary>Creates a <see cref="T:System.Linq.Expressions.BinaryExpression" /> that represents an arithmetic addition operation that has overflow checking. The implementing method can be specified.</summary>
		/// <param name="left">A <see cref="T:System.Linq.Expressions.Expression" /> to set the <see cref="P:System.Linq.Expressions.BinaryExpression.Left" /> property equal to.</param>
		/// <param name="right">A <see cref="T:System.Linq.Expressions.Expression" /> to set the <see cref="P:System.Linq.Expressions.BinaryExpression.Right" /> property equal to.</param>
		/// <param name="method">A <see cref="T:System.Reflection.MethodInfo" /> to set the <see cref="P:System.Linq.Expressions.BinaryExpression.Method" /> property equal to.</param>
		/// <returns>A <see cref="T:System.Linq.Expressions.BinaryExpression" /> that has the <see cref="P:System.Linq.Expressions.Expression.NodeType" /> property equal to <see cref="F:System.Linq.Expressions.ExpressionType.AddChecked" /> and the <see cref="P:System.Linq.Expressions.BinaryExpression.Left" />, <see cref="P:System.Linq.Expressions.BinaryExpression.Right" /> and <see cref="P:System.Linq.Expressions.BinaryExpression.Method" /> properties set to the specified values.</returns>
		/// <exception cref="T:System.ArgumentNullException">
		///         <paramref name="left" /> or <paramref name="right" /> is <see langword="null" />.</exception>
		/// <exception cref="T:System.ArgumentException">
		///         <paramref name="method" /> is not <see langword="null" /> and the method it represents returns <see langword="void" />, is not <see langword="static" /> (<see langword="Shared" /> in Visual Basic), or does not take exactly two arguments.</exception>
		/// <exception cref="T:System.InvalidOperationException">
		///         <paramref name="method" /> is <see langword="null" /> and the addition operator is not defined for <paramref name="left" />.Type and <paramref name="right" />.Type.</exception>
		public static BinaryExpression AddChecked(Expression left, Expression right, MethodInfo method)
		{
			ExpressionUtils.RequiresCanRead(left, "left");
			ExpressionUtils.RequiresCanRead(right, "right");
			if (!(method == null))
			{
				return Expression.GetMethodBasedBinaryOperator(ExpressionType.AddChecked, left, right, method, true);
			}
			if (left.Type == right.Type && left.Type.IsArithmetic())
			{
				return new SimpleBinaryExpression(ExpressionType.AddChecked, left, right, left.Type);
			}
			return Expression.GetUserDefinedBinaryOperatorOrThrow(ExpressionType.AddChecked, "op_Addition", left, right, true);
		}

		/// <summary>Creates a <see cref="T:System.Linq.Expressions.BinaryExpression" /> that represents an arithmetic subtraction operation that does not have overflow checking.</summary>
		/// <param name="left">An <see cref="T:System.Linq.Expressions.Expression" /> to set the <see cref="P:System.Linq.Expressions.BinaryExpression.Left" /> property equal to.</param>
		/// <param name="right">A <see cref="T:System.Linq.Expressions.Expression" /> to set the <see cref="P:System.Linq.Expressions.BinaryExpression.Right" /> property equal to.</param>
		/// <returns>A <see cref="T:System.Linq.Expressions.BinaryExpression" /> that has the <see cref="P:System.Linq.Expressions.Expression.NodeType" /> property equal to <see cref="F:System.Linq.Expressions.ExpressionType.Subtract" /> and the <see cref="P:System.Linq.Expressions.BinaryExpression.Left" /> and <see cref="P:System.Linq.Expressions.BinaryExpression.Right" /> properties set to the specified values.</returns>
		/// <exception cref="T:System.ArgumentNullException">
		///         <paramref name="left" /> or <paramref name="right" /> is <see langword="null" />.</exception>
		/// <exception cref="T:System.InvalidOperationException">The subtraction operator is not defined for <paramref name="left" />.Type and <paramref name="right" />.Type.</exception>
		public static BinaryExpression Subtract(Expression left, Expression right)
		{
			return Expression.Subtract(left, right, null);
		}

		/// <summary>Creates a <see cref="T:System.Linq.Expressions.BinaryExpression" /> that represents an arithmetic subtraction operation that does not have overflow checking.</summary>
		/// <param name="left">A <see cref="T:System.Linq.Expressions.Expression" /> to set the <see cref="P:System.Linq.Expressions.BinaryExpression.Left" /> property equal to.</param>
		/// <param name="right">A <see cref="T:System.Linq.Expressions.Expression" /> to set the <see cref="P:System.Linq.Expressions.BinaryExpression.Right" /> property equal to.</param>
		/// <param name="method">A <see cref="T:System.Reflection.MethodInfo" /> to set the <see cref="P:System.Linq.Expressions.BinaryExpression.Method" /> property equal to.</param>
		/// <returns>A <see cref="T:System.Linq.Expressions.BinaryExpression" /> that has the <see cref="P:System.Linq.Expressions.Expression.NodeType" /> property equal to <see cref="F:System.Linq.Expressions.ExpressionType.Subtract" /> and the <see cref="P:System.Linq.Expressions.BinaryExpression.Left" />, <see cref="P:System.Linq.Expressions.BinaryExpression.Right" />, and <see cref="P:System.Linq.Expressions.BinaryExpression.Method" /> properties set to the specified values.</returns>
		/// <exception cref="T:System.ArgumentNullException">
		///         <paramref name="left" /> or <paramref name="right" /> is <see langword="null" />.</exception>
		/// <exception cref="T:System.ArgumentException">
		///         <paramref name="method" /> is not <see langword="null" /> and the method it represents returns <see langword="void" />, is not <see langword="static" /> (<see langword="Shared" /> in Visual Basic), or does not take exactly two arguments.</exception>
		/// <exception cref="T:System.InvalidOperationException">
		///         <paramref name="method" /> is <see langword="null" /> and the subtraction operator is not defined for <paramref name="left" />.Type and <paramref name="right" />.Type.</exception>
		public static BinaryExpression Subtract(Expression left, Expression right, MethodInfo method)
		{
			ExpressionUtils.RequiresCanRead(left, "left");
			ExpressionUtils.RequiresCanRead(right, "right");
			if (!(method == null))
			{
				return Expression.GetMethodBasedBinaryOperator(ExpressionType.Subtract, left, right, method, true);
			}
			if (left.Type == right.Type && left.Type.IsArithmetic())
			{
				return new SimpleBinaryExpression(ExpressionType.Subtract, left, right, left.Type);
			}
			return Expression.GetUserDefinedBinaryOperatorOrThrow(ExpressionType.Subtract, "op_Subtraction", left, right, true);
		}

		/// <summary>Creates a <see cref="T:System.Linq.Expressions.BinaryExpression" /> that represents a subtraction assignment operation that does not have overflow checking.</summary>
		/// <param name="left">An <see cref="T:System.Linq.Expressions.Expression" /> to set the <see cref="P:System.Linq.Expressions.BinaryExpression.Left" /> property equal to.</param>
		/// <param name="right">An <see cref="T:System.Linq.Expressions.Expression" /> to set the <see cref="P:System.Linq.Expressions.BinaryExpression.Right" /> property equal to.</param>
		/// <returns>A <see cref="T:System.Linq.Expressions.BinaryExpression" /> that has the <see cref="P:System.Linq.Expressions.Expression.NodeType" /> property equal to <see cref="F:System.Linq.Expressions.ExpressionType.SubtractAssign" /> and the <see cref="P:System.Linq.Expressions.BinaryExpression.Left" /> and <see cref="P:System.Linq.Expressions.BinaryExpression.Right" /> properties set to the specified values.</returns>
		public static BinaryExpression SubtractAssign(Expression left, Expression right)
		{
			return Expression.SubtractAssign(left, right, null, null);
		}

		/// <summary>Creates a <see cref="T:System.Linq.Expressions.BinaryExpression" /> that represents a subtraction assignment operation that does not have overflow checking.</summary>
		/// <param name="left">An <see cref="T:System.Linq.Expressions.Expression" /> to set the <see cref="P:System.Linq.Expressions.BinaryExpression.Left" /> property equal to.</param>
		/// <param name="right">An <see cref="T:System.Linq.Expressions.Expression" /> to set the <see cref="P:System.Linq.Expressions.BinaryExpression.Right" /> property equal to.</param>
		/// <param name="method">A <see cref="T:System.Reflection.MethodInfo" /> to set the <see cref="P:System.Linq.Expressions.BinaryExpression.Method" /> property equal to.</param>
		/// <returns>A <see cref="T:System.Linq.Expressions.BinaryExpression" /> that has the <see cref="P:System.Linq.Expressions.Expression.NodeType" /> property equal to <see cref="F:System.Linq.Expressions.ExpressionType.SubtractAssign" /> and the <see cref="P:System.Linq.Expressions.BinaryExpression.Left" />, <see cref="P:System.Linq.Expressions.BinaryExpression.Right" />, and <see cref="P:System.Linq.Expressions.BinaryExpression.Method" /> properties set to the specified values.</returns>
		public static BinaryExpression SubtractAssign(Expression left, Expression right, MethodInfo method)
		{
			return Expression.SubtractAssign(left, right, method, null);
		}

		/// <summary>Creates a <see cref="T:System.Linq.Expressions.BinaryExpression" /> that represents a subtraction assignment operation that does not have overflow checking.</summary>
		/// <param name="left">An <see cref="T:System.Linq.Expressions.Expression" /> to set the <see cref="P:System.Linq.Expressions.BinaryExpression.Left" /> property equal to.</param>
		/// <param name="right">An <see cref="T:System.Linq.Expressions.Expression" /> to set the <see cref="P:System.Linq.Expressions.BinaryExpression.Right" /> property equal to.</param>
		/// <param name="method">A <see cref="T:System.Reflection.MethodInfo" /> to set the <see cref="P:System.Linq.Expressions.BinaryExpression.Method" /> property equal to.</param>
		/// <param name="conversion">A <see cref="T:System.Linq.Expressions.LambdaExpression" /> to set the <see cref="P:System.Linq.Expressions.BinaryExpression.Conversion" /> property equal to.</param>
		/// <returns>A <see cref="T:System.Linq.Expressions.BinaryExpression" /> that has the <see cref="P:System.Linq.Expressions.Expression.NodeType" /> property equal to <see cref="F:System.Linq.Expressions.ExpressionType.SubtractAssign" /> and the <see cref="P:System.Linq.Expressions.BinaryExpression.Left" />, <see cref="P:System.Linq.Expressions.BinaryExpression.Right" />, <see cref="P:System.Linq.Expressions.BinaryExpression.Method" />, and <see cref="P:System.Linq.Expressions.BinaryExpression.Conversion" /> properties set to the specified values.</returns>
		public static BinaryExpression SubtractAssign(Expression left, Expression right, MethodInfo method, LambdaExpression conversion)
		{
			ExpressionUtils.RequiresCanRead(left, "left");
			Expression.RequiresCanWrite(left, "left");
			ExpressionUtils.RequiresCanRead(right, "right");
			if (!(method == null))
			{
				return Expression.GetMethodBasedAssignOperator(ExpressionType.SubtractAssign, left, right, method, conversion, true);
			}
			if (!(left.Type == right.Type) || !left.Type.IsArithmetic())
			{
				return Expression.GetUserDefinedAssignOperatorOrThrow(ExpressionType.SubtractAssign, "op_Subtraction", left, right, conversion, true);
			}
			if (conversion != null)
			{
				throw Error.ConversionIsNotSupportedForArithmeticTypes();
			}
			return new SimpleBinaryExpression(ExpressionType.SubtractAssign, left, right, left.Type);
		}

		/// <summary>Creates a <see cref="T:System.Linq.Expressions.BinaryExpression" /> that represents a subtraction assignment operation that has overflow checking.</summary>
		/// <param name="left">An <see cref="T:System.Linq.Expressions.Expression" /> to set the <see cref="P:System.Linq.Expressions.BinaryExpression.Left" /> property equal to.</param>
		/// <param name="right">An <see cref="T:System.Linq.Expressions.Expression" /> to set the <see cref="P:System.Linq.Expressions.BinaryExpression.Right" /> property equal to.</param>
		/// <returns>A <see cref="T:System.Linq.Expressions.BinaryExpression" /> that has the <see cref="P:System.Linq.Expressions.Expression.NodeType" /> property equal to <see cref="F:System.Linq.Expressions.ExpressionType.SubtractAssignChecked" /> and the <see cref="P:System.Linq.Expressions.BinaryExpression.Left" /> and <see cref="P:System.Linq.Expressions.BinaryExpression.Right" /> properties set to the specified values.</returns>
		public static BinaryExpression SubtractAssignChecked(Expression left, Expression right)
		{
			return Expression.SubtractAssignChecked(left, right, null);
		}

		/// <summary>Creates a <see cref="T:System.Linq.Expressions.BinaryExpression" /> that represents a subtraction assignment operation that has overflow checking.</summary>
		/// <param name="left">An <see cref="T:System.Linq.Expressions.Expression" /> to set the <see cref="P:System.Linq.Expressions.BinaryExpression.Left" /> property equal to.</param>
		/// <param name="right">An <see cref="T:System.Linq.Expressions.Expression" /> to set the <see cref="P:System.Linq.Expressions.BinaryExpression.Right" /> property equal to.</param>
		/// <param name="method">A <see cref="T:System.Reflection.MethodInfo" /> to set the <see cref="P:System.Linq.Expressions.BinaryExpression.Method" /> property equal to.</param>
		/// <returns>A <see cref="T:System.Linq.Expressions.BinaryExpression" /> that has the <see cref="P:System.Linq.Expressions.Expression.NodeType" /> property equal to <see cref="F:System.Linq.Expressions.ExpressionType.SubtractAssignChecked" /> and the <see cref="P:System.Linq.Expressions.BinaryExpression.Left" />, <see cref="P:System.Linq.Expressions.BinaryExpression.Right" />, and <see cref="P:System.Linq.Expressions.BinaryExpression.Method" /> properties set to the specified values.</returns>
		public static BinaryExpression SubtractAssignChecked(Expression left, Expression right, MethodInfo method)
		{
			return Expression.SubtractAssignChecked(left, right, method, null);
		}

		/// <summary>Creates a <see cref="T:System.Linq.Expressions.BinaryExpression" /> that represents a subtraction assignment operation that has overflow checking.</summary>
		/// <param name="left">An <see cref="T:System.Linq.Expressions.Expression" /> to set the <see cref="P:System.Linq.Expressions.BinaryExpression.Left" /> property equal to.</param>
		/// <param name="right">An <see cref="T:System.Linq.Expressions.Expression" /> to set the <see cref="P:System.Linq.Expressions.BinaryExpression.Right" /> property equal to.</param>
		/// <param name="method">A <see cref="T:System.Reflection.MethodInfo" /> to set the <see cref="P:System.Linq.Expressions.BinaryExpression.Method" /> property equal to.</param>
		/// <param name="conversion">A <see cref="T:System.Linq.Expressions.LambdaExpression" /> to set the <see cref="P:System.Linq.Expressions.BinaryExpression.Conversion" /> property equal to.</param>
		/// <returns>A <see cref="T:System.Linq.Expressions.BinaryExpression" /> that has the <see cref="P:System.Linq.Expressions.Expression.NodeType" /> property equal to <see cref="F:System.Linq.Expressions.ExpressionType.SubtractAssignChecked" /> and the <see cref="P:System.Linq.Expressions.BinaryExpression.Left" />, <see cref="P:System.Linq.Expressions.BinaryExpression.Right" />, <see cref="P:System.Linq.Expressions.BinaryExpression.Method" />, and <see cref="P:System.Linq.Expressions.BinaryExpression.Conversion" /> properties set to the specified values.</returns>
		public static BinaryExpression SubtractAssignChecked(Expression left, Expression right, MethodInfo method, LambdaExpression conversion)
		{
			ExpressionUtils.RequiresCanRead(left, "left");
			Expression.RequiresCanWrite(left, "left");
			ExpressionUtils.RequiresCanRead(right, "right");
			if (!(method == null))
			{
				return Expression.GetMethodBasedAssignOperator(ExpressionType.SubtractAssignChecked, left, right, method, conversion, true);
			}
			if (!(left.Type == right.Type) || !left.Type.IsArithmetic())
			{
				return Expression.GetUserDefinedAssignOperatorOrThrow(ExpressionType.SubtractAssignChecked, "op_Subtraction", left, right, conversion, true);
			}
			if (conversion != null)
			{
				throw Error.ConversionIsNotSupportedForArithmeticTypes();
			}
			return new SimpleBinaryExpression(ExpressionType.SubtractAssignChecked, left, right, left.Type);
		}

		/// <summary>Creates a <see cref="T:System.Linq.Expressions.BinaryExpression" /> that represents an arithmetic subtraction operation that has overflow checking.</summary>
		/// <param name="left">A <see cref="T:System.Linq.Expressions.Expression" /> to set the <see cref="P:System.Linq.Expressions.BinaryExpression.Left" /> property equal to.</param>
		/// <param name="right">A <see cref="T:System.Linq.Expressions.Expression" /> to set the <see cref="P:System.Linq.Expressions.BinaryExpression.Right" /> property equal to.</param>
		/// <returns>A <see cref="T:System.Linq.Expressions.BinaryExpression" /> that has the <see cref="P:System.Linq.Expressions.Expression.NodeType" /> property equal to <see cref="F:System.Linq.Expressions.ExpressionType.SubtractChecked" /> and the <see cref="P:System.Linq.Expressions.BinaryExpression.Left" /> and <see cref="P:System.Linq.Expressions.BinaryExpression.Right" /> properties set to the specified values.</returns>
		/// <exception cref="T:System.ArgumentNullException">
		///         <paramref name="left" /> or <paramref name="right" /> is <see langword="null" />.</exception>
		/// <exception cref="T:System.InvalidOperationException">The subtraction operator is not defined for <paramref name="left" />.Type and <paramref name="right" />.Type.</exception>
		public static BinaryExpression SubtractChecked(Expression left, Expression right)
		{
			return Expression.SubtractChecked(left, right, null);
		}

		/// <summary>Creates a <see cref="T:System.Linq.Expressions.BinaryExpression" /> that represents an arithmetic subtraction operation that has overflow checking.</summary>
		/// <param name="left">A <see cref="T:System.Linq.Expressions.Expression" /> to set the <see cref="P:System.Linq.Expressions.BinaryExpression.Left" /> property equal to.</param>
		/// <param name="right">A <see cref="T:System.Linq.Expressions.Expression" /> to set the <see cref="P:System.Linq.Expressions.BinaryExpression.Right" /> property equal to.</param>
		/// <param name="method">A <see cref="T:System.Reflection.MethodInfo" /> to set the <see cref="P:System.Linq.Expressions.BinaryExpression.Method" /> property equal to.</param>
		/// <returns>A <see cref="T:System.Linq.Expressions.BinaryExpression" /> that has the <see cref="P:System.Linq.Expressions.Expression.NodeType" /> property equal to <see cref="F:System.Linq.Expressions.ExpressionType.SubtractChecked" /> and the <see cref="P:System.Linq.Expressions.BinaryExpression.Left" />, <see cref="P:System.Linq.Expressions.BinaryExpression.Right" />, and <see cref="P:System.Linq.Expressions.BinaryExpression.Method" /> properties set to the specified values.</returns>
		/// <exception cref="T:System.ArgumentNullException">
		///         <paramref name="left" /> or <paramref name="right" /> is <see langword="null" />.</exception>
		/// <exception cref="T:System.ArgumentException">
		///         <paramref name="method" /> is not <see langword="null" /> and the method it represents returns <see langword="void" />, is not <see langword="static" /> (<see langword="Shared" /> in Visual Basic), or does not take exactly two arguments.</exception>
		/// <exception cref="T:System.InvalidOperationException">
		///         <paramref name="method" /> is <see langword="null" /> and the subtraction operator is not defined for <paramref name="left" />.Type and <paramref name="right" />.Type.</exception>
		public static BinaryExpression SubtractChecked(Expression left, Expression right, MethodInfo method)
		{
			ExpressionUtils.RequiresCanRead(left, "left");
			ExpressionUtils.RequiresCanRead(right, "right");
			if (!(method == null))
			{
				return Expression.GetMethodBasedBinaryOperator(ExpressionType.SubtractChecked, left, right, method, true);
			}
			if (left.Type == right.Type && left.Type.IsArithmetic())
			{
				return new SimpleBinaryExpression(ExpressionType.SubtractChecked, left, right, left.Type);
			}
			return Expression.GetUserDefinedBinaryOperatorOrThrow(ExpressionType.SubtractChecked, "op_Subtraction", left, right, true);
		}

		/// <summary>Creates a <see cref="T:System.Linq.Expressions.BinaryExpression" /> that represents an arithmetic division operation.</summary>
		/// <param name="left">An <see cref="T:System.Linq.Expressions.Expression" /> to set the <see cref="P:System.Linq.Expressions.BinaryExpression.Left" /> property to.</param>
		/// <param name="right">An <see cref="T:System.Linq.Expressions.Expression" /> to set the <see cref="P:System.Linq.Expressions.BinaryExpression.Right" /> property to.</param>
		/// <returns>A <see cref="T:System.Linq.Expressions.BinaryExpression" /> that has the <see cref="P:System.Linq.Expressions.Expression.NodeType" /> property equal to <see cref="F:System.Linq.Expressions.ExpressionType.Divide" /> and the <see cref="P:System.Linq.Expressions.BinaryExpression.Left" /> and <see cref="P:System.Linq.Expressions.BinaryExpression.Right" /> properties set to the specified values.</returns>
		/// <exception cref="T:System.ArgumentNullException">
		///         <paramref name="left" /> or <paramref name="right" /> is <see langword="null" />.</exception>
		/// <exception cref="T:System.InvalidOperationException">The division operator is not defined for <paramref name="left" />.Type and <paramref name="right" />.Type.</exception>
		public static BinaryExpression Divide(Expression left, Expression right)
		{
			return Expression.Divide(left, right, null);
		}

		/// <summary>Creates a <see cref="T:System.Linq.Expressions.BinaryExpression" /> that represents an arithmetic division operation. The implementing method can be specified.</summary>
		/// <param name="left">An <see cref="T:System.Linq.Expressions.Expression" /> to set the <see cref="P:System.Linq.Expressions.BinaryExpression.Left" /> property equal to.</param>
		/// <param name="right">An <see cref="T:System.Linq.Expressions.Expression" /> to set the <see cref="P:System.Linq.Expressions.BinaryExpression.Right" /> property equal to.</param>
		/// <param name="method">A <see cref="T:System.Reflection.MethodInfo" /> to set the <see cref="P:System.Linq.Expressions.BinaryExpression.Method" /> property equal to.</param>
		/// <returns>A <see cref="T:System.Linq.Expressions.BinaryExpression" /> that has the <see cref="P:System.Linq.Expressions.Expression.NodeType" /> property equal to <see cref="F:System.Linq.Expressions.ExpressionType.Divide" /> and the <see cref="P:System.Linq.Expressions.BinaryExpression.Left" />, <see cref="P:System.Linq.Expressions.BinaryExpression.Right" />, and <see cref="P:System.Linq.Expressions.BinaryExpression.Method" /> properties set to the specified values.</returns>
		/// <exception cref="T:System.ArgumentNullException">
		///         <paramref name="left" /> or <paramref name="right" /> is <see langword="null" />.</exception>
		/// <exception cref="T:System.ArgumentException">
		///         <paramref name="method" /> is not <see langword="null" /> and the method it represents returns <see langword="void" />, is not <see langword="static" /> (<see langword="Shared" /> in Visual Basic), or does not take exactly two arguments.</exception>
		/// <exception cref="T:System.InvalidOperationException">
		///         <paramref name="method" /> is <see langword="null" /> and the division operator is not defined for <paramref name="left" />.Type and <paramref name="right" />.Type.</exception>
		public static BinaryExpression Divide(Expression left, Expression right, MethodInfo method)
		{
			ExpressionUtils.RequiresCanRead(left, "left");
			ExpressionUtils.RequiresCanRead(right, "right");
			if (!(method == null))
			{
				return Expression.GetMethodBasedBinaryOperator(ExpressionType.Divide, left, right, method, true);
			}
			if (left.Type == right.Type && left.Type.IsArithmetic())
			{
				return new SimpleBinaryExpression(ExpressionType.Divide, left, right, left.Type);
			}
			return Expression.GetUserDefinedBinaryOperatorOrThrow(ExpressionType.Divide, "op_Division", left, right, true);
		}

		/// <summary>Creates a <see cref="T:System.Linq.Expressions.BinaryExpression" /> that represents a division assignment operation that does not have overflow checking.</summary>
		/// <param name="left">An <see cref="T:System.Linq.Expressions.Expression" /> to set the <see cref="P:System.Linq.Expressions.BinaryExpression.Left" /> property equal to.</param>
		/// <param name="right">An <see cref="T:System.Linq.Expressions.Expression" /> to set the <see cref="P:System.Linq.Expressions.BinaryExpression.Right" /> property equal to.</param>
		/// <returns>A <see cref="T:System.Linq.Expressions.BinaryExpression" /> that has the <see cref="P:System.Linq.Expressions.Expression.NodeType" /> property equal to <see cref="F:System.Linq.Expressions.ExpressionType.DivideAssign" /> and the <see cref="P:System.Linq.Expressions.BinaryExpression.Left" /> and <see cref="P:System.Linq.Expressions.BinaryExpression.Right" /> properties set to the specified values.</returns>
		public static BinaryExpression DivideAssign(Expression left, Expression right)
		{
			return Expression.DivideAssign(left, right, null, null);
		}

		/// <summary>Creates a <see cref="T:System.Linq.Expressions.BinaryExpression" /> that represents a division assignment operation that does not have overflow checking.</summary>
		/// <param name="left">An <see cref="T:System.Linq.Expressions.Expression" /> to set the <see cref="P:System.Linq.Expressions.BinaryExpression.Left" /> property equal to.</param>
		/// <param name="right">An <see cref="T:System.Linq.Expressions.Expression" /> to set the <see cref="P:System.Linq.Expressions.BinaryExpression.Right" /> property equal to.</param>
		/// <param name="method">A <see cref="T:System.Reflection.MethodInfo" /> to set the <see cref="P:System.Linq.Expressions.BinaryExpression.Method" /> property equal to.</param>
		/// <returns>A <see cref="T:System.Linq.Expressions.BinaryExpression" /> that has the <see cref="P:System.Linq.Expressions.Expression.NodeType" /> property equal to <see cref="F:System.Linq.Expressions.ExpressionType.DivideAssign" /> and the <see cref="P:System.Linq.Expressions.BinaryExpression.Left" />, <see cref="P:System.Linq.Expressions.BinaryExpression.Right" />, and <see cref="P:System.Linq.Expressions.BinaryExpression.Method" /> properties set to the specified values.</returns>
		public static BinaryExpression DivideAssign(Expression left, Expression right, MethodInfo method)
		{
			return Expression.DivideAssign(left, right, method, null);
		}

		/// <summary>Creates a <see cref="T:System.Linq.Expressions.BinaryExpression" /> that represents a division assignment operation that does not have overflow checking.</summary>
		/// <param name="left">An <see cref="T:System.Linq.Expressions.Expression" /> to set the <see cref="P:System.Linq.Expressions.BinaryExpression.Left" /> property equal to.</param>
		/// <param name="right">An <see cref="T:System.Linq.Expressions.Expression" /> to set the <see cref="P:System.Linq.Expressions.BinaryExpression.Right" /> property equal to.</param>
		/// <param name="method">A <see cref="T:System.Reflection.MethodInfo" /> to set the <see cref="P:System.Linq.Expressions.BinaryExpression.Method" /> property equal to.</param>
		/// <param name="conversion">A <see cref="T:System.Linq.Expressions.LambdaExpression" /> to set the <see cref="P:System.Linq.Expressions.BinaryExpression.Conversion" /> property equal to.</param>
		/// <returns>A <see cref="T:System.Linq.Expressions.BinaryExpression" /> that has the <see cref="P:System.Linq.Expressions.Expression.NodeType" /> property equal to <see cref="F:System.Linq.Expressions.ExpressionType.DivideAssign" /> and the <see cref="P:System.Linq.Expressions.BinaryExpression.Left" />, <see cref="P:System.Linq.Expressions.BinaryExpression.Right" />, <see cref="P:System.Linq.Expressions.BinaryExpression.Method" />, and <see cref="P:System.Linq.Expressions.BinaryExpression.Conversion" /> properties set to the specified values.</returns>
		public static BinaryExpression DivideAssign(Expression left, Expression right, MethodInfo method, LambdaExpression conversion)
		{
			ExpressionUtils.RequiresCanRead(left, "left");
			Expression.RequiresCanWrite(left, "left");
			ExpressionUtils.RequiresCanRead(right, "right");
			if (!(method == null))
			{
				return Expression.GetMethodBasedAssignOperator(ExpressionType.DivideAssign, left, right, method, conversion, true);
			}
			if (!(left.Type == right.Type) || !left.Type.IsArithmetic())
			{
				return Expression.GetUserDefinedAssignOperatorOrThrow(ExpressionType.DivideAssign, "op_Division", left, right, conversion, true);
			}
			if (conversion != null)
			{
				throw Error.ConversionIsNotSupportedForArithmeticTypes();
			}
			return new SimpleBinaryExpression(ExpressionType.DivideAssign, left, right, left.Type);
		}

		/// <summary>Creates a <see cref="T:System.Linq.Expressions.BinaryExpression" /> that represents an arithmetic remainder operation.</summary>
		/// <param name="left">An <see cref="T:System.Linq.Expressions.Expression" /> to set the <see cref="P:System.Linq.Expressions.BinaryExpression.Left" /> property equal to.</param>
		/// <param name="right">An <see cref="T:System.Linq.Expressions.Expression" /> to set the <see cref="P:System.Linq.Expressions.BinaryExpression.Right" /> property equal to.</param>
		/// <returns>A <see cref="T:System.Linq.Expressions.BinaryExpression" /> that has the <see cref="P:System.Linq.Expressions.Expression.NodeType" /> property equal to <see cref="F:System.Linq.Expressions.ExpressionType.Modulo" /> and the <see cref="P:System.Linq.Expressions.BinaryExpression.Left" /> and <see cref="P:System.Linq.Expressions.BinaryExpression.Right" /> properties set to the specified values.</returns>
		/// <exception cref="T:System.ArgumentNullException">
		///         <paramref name="left" /> or <paramref name="right" /> is <see langword="null" />.</exception>
		/// <exception cref="T:System.InvalidOperationException">The modulus operator is not defined for <paramref name="left" />.Type and <paramref name="right" />.Type.</exception>
		public static BinaryExpression Modulo(Expression left, Expression right)
		{
			return Expression.Modulo(left, right, null);
		}

		/// <summary>Creates a <see cref="T:System.Linq.Expressions.BinaryExpression" /> that represents an arithmetic remainder operation.</summary>
		/// <param name="left">An <see cref="T:System.Linq.Expressions.Expression" /> to set the <see cref="P:System.Linq.Expressions.BinaryExpression.Left" /> property equal to.</param>
		/// <param name="right">An <see cref="T:System.Linq.Expressions.Expression" /> to set the <see cref="P:System.Linq.Expressions.BinaryExpression.Right" /> property equal to.</param>
		/// <param name="method">A <see cref="T:System.Reflection.MethodInfo" /> to set the <see cref="P:System.Linq.Expressions.BinaryExpression.Method" /> property equal to.</param>
		/// <returns>A <see cref="T:System.Linq.Expressions.BinaryExpression" /> that has the <see cref="P:System.Linq.Expressions.Expression.NodeType" /> property equal to <see cref="F:System.Linq.Expressions.ExpressionType.Modulo" /> and the <see cref="P:System.Linq.Expressions.BinaryExpression.Left" />, <see cref="P:System.Linq.Expressions.BinaryExpression.Right" />, and <see cref="P:System.Linq.Expressions.BinaryExpression.Method" /> properties set to the specified values.</returns>
		/// <exception cref="T:System.ArgumentNullException">
		///         <paramref name="left" /> or <paramref name="right" /> is <see langword="null" />.</exception>
		/// <exception cref="T:System.ArgumentException">
		///         <paramref name="method" /> is not <see langword="null" /> and the method it represents returns <see langword="void" />, is not <see langword="static" /> (<see langword="Shared" /> in Visual Basic), or does not take exactly two arguments.</exception>
		/// <exception cref="T:System.InvalidOperationException">
		///         <paramref name="method" /> is <see langword="null" /> and the modulus operator is not defined for <paramref name="left" />.Type and <paramref name="right" />.Type.</exception>
		public static BinaryExpression Modulo(Expression left, Expression right, MethodInfo method)
		{
			ExpressionUtils.RequiresCanRead(left, "left");
			ExpressionUtils.RequiresCanRead(right, "right");
			if (!(method == null))
			{
				return Expression.GetMethodBasedBinaryOperator(ExpressionType.Modulo, left, right, method, true);
			}
			if (left.Type == right.Type && left.Type.IsArithmetic())
			{
				return new SimpleBinaryExpression(ExpressionType.Modulo, left, right, left.Type);
			}
			return Expression.GetUserDefinedBinaryOperatorOrThrow(ExpressionType.Modulo, "op_Modulus", left, right, true);
		}

		/// <summary>Creates a <see cref="T:System.Linq.Expressions.BinaryExpression" /> that represents a remainder assignment operation.</summary>
		/// <param name="left">An <see cref="T:System.Linq.Expressions.Expression" /> to set the <see cref="P:System.Linq.Expressions.BinaryExpression.Left" /> property equal to.</param>
		/// <param name="right">An <see cref="T:System.Linq.Expressions.Expression" /> to set the <see cref="P:System.Linq.Expressions.BinaryExpression.Right" /> property equal to.</param>
		/// <returns>A <see cref="T:System.Linq.Expressions.BinaryExpression" /> that has the <see cref="P:System.Linq.Expressions.Expression.NodeType" /> property equal to <see cref="F:System.Linq.Expressions.ExpressionType.ModuloAssign" /> and the <see cref="P:System.Linq.Expressions.BinaryExpression.Left" /> and <see cref="P:System.Linq.Expressions.BinaryExpression.Right" /> properties set to the specified values.</returns>
		public static BinaryExpression ModuloAssign(Expression left, Expression right)
		{
			return Expression.ModuloAssign(left, right, null, null);
		}

		/// <summary>Creates a <see cref="T:System.Linq.Expressions.BinaryExpression" /> that represents a remainder assignment operation.</summary>
		/// <param name="left">An <see cref="T:System.Linq.Expressions.Expression" /> to set the <see cref="P:System.Linq.Expressions.BinaryExpression.Left" /> property equal to.</param>
		/// <param name="right">An <see cref="T:System.Linq.Expressions.Expression" /> to set the <see cref="P:System.Linq.Expressions.BinaryExpression.Right" /> property equal to.</param>
		/// <param name="method">A <see cref="T:System.Reflection.MethodInfo" /> to set the <see cref="P:System.Linq.Expressions.BinaryExpression.Method" /> property equal to.</param>
		/// <returns>A <see cref="T:System.Linq.Expressions.BinaryExpression" /> that has the <see cref="P:System.Linq.Expressions.Expression.NodeType" /> property equal to <see cref="F:System.Linq.Expressions.ExpressionType.ModuloAssign" /> and the <see cref="P:System.Linq.Expressions.BinaryExpression.Left" />, <see cref="P:System.Linq.Expressions.BinaryExpression.Right" />, and <see cref="P:System.Linq.Expressions.BinaryExpression.Method" /> properties set to the specified values.</returns>
		public static BinaryExpression ModuloAssign(Expression left, Expression right, MethodInfo method)
		{
			return Expression.ModuloAssign(left, right, method, null);
		}

		/// <summary>Creates a <see cref="T:System.Linq.Expressions.BinaryExpression" /> that represents a remainder assignment operation.</summary>
		/// <param name="left">An <see cref="T:System.Linq.Expressions.Expression" /> to set the <see cref="P:System.Linq.Expressions.BinaryExpression.Left" /> property equal to.</param>
		/// <param name="right">An <see cref="T:System.Linq.Expressions.Expression" /> to set the <see cref="P:System.Linq.Expressions.BinaryExpression.Right" /> property equal to.</param>
		/// <param name="method">A <see cref="T:System.Reflection.MethodInfo" /> to set the <see cref="P:System.Linq.Expressions.BinaryExpression.Method" /> property equal to.</param>
		/// <param name="conversion">A <see cref="T:System.Linq.Expressions.LambdaExpression" /> to set the <see cref="P:System.Linq.Expressions.BinaryExpression.Conversion" /> property equal to.</param>
		/// <returns>A <see cref="T:System.Linq.Expressions.BinaryExpression" /> that has the <see cref="P:System.Linq.Expressions.Expression.NodeType" /> property equal to <see cref="F:System.Linq.Expressions.ExpressionType.ModuloAssign" /> and the <see cref="P:System.Linq.Expressions.BinaryExpression.Left" />, <see cref="P:System.Linq.Expressions.BinaryExpression.Right" />, <see cref="P:System.Linq.Expressions.BinaryExpression.Method" />, and <see cref="P:System.Linq.Expressions.BinaryExpression.Conversion" /> properties set to the specified values.</returns>
		public static BinaryExpression ModuloAssign(Expression left, Expression right, MethodInfo method, LambdaExpression conversion)
		{
			ExpressionUtils.RequiresCanRead(left, "left");
			Expression.RequiresCanWrite(left, "left");
			ExpressionUtils.RequiresCanRead(right, "right");
			if (!(method == null))
			{
				return Expression.GetMethodBasedAssignOperator(ExpressionType.ModuloAssign, left, right, method, conversion, true);
			}
			if (!(left.Type == right.Type) || !left.Type.IsArithmetic())
			{
				return Expression.GetUserDefinedAssignOperatorOrThrow(ExpressionType.ModuloAssign, "op_Modulus", left, right, conversion, true);
			}
			if (conversion != null)
			{
				throw Error.ConversionIsNotSupportedForArithmeticTypes();
			}
			return new SimpleBinaryExpression(ExpressionType.ModuloAssign, left, right, left.Type);
		}

		/// <summary>Creates a <see cref="T:System.Linq.Expressions.BinaryExpression" /> that represents an arithmetic multiplication operation that does not have overflow checking.</summary>
		/// <param name="left">An <see cref="T:System.Linq.Expressions.Expression" /> to set the <see cref="P:System.Linq.Expressions.BinaryExpression.Left" /> property equal to.</param>
		/// <param name="right">An <see cref="T:System.Linq.Expressions.Expression" /> to set the <see cref="P:System.Linq.Expressions.BinaryExpression.Right" /> property equal to.</param>
		/// <returns>A <see cref="T:System.Linq.Expressions.BinaryExpression" /> that has the <see cref="P:System.Linq.Expressions.Expression.NodeType" /> property equal to <see cref="F:System.Linq.Expressions.ExpressionType.Multiply" /> and the <see cref="P:System.Linq.Expressions.BinaryExpression.Left" /> and <see cref="P:System.Linq.Expressions.BinaryExpression.Right" /> properties set to the specified values.</returns>
		/// <exception cref="T:System.ArgumentNullException">
		///         <paramref name="left" /> or <paramref name="right" /> is <see langword="null" />.</exception>
		/// <exception cref="T:System.InvalidOperationException">The multiplication operator is not defined for <paramref name="left" />.Type and <paramref name="right" />.Type.</exception>
		public static BinaryExpression Multiply(Expression left, Expression right)
		{
			return Expression.Multiply(left, right, null);
		}

		/// <summary>Creates a <see cref="T:System.Linq.Expressions.BinaryExpression" /> that represents an arithmetic multiplication operation that does not have overflow checking.</summary>
		/// <param name="left">An <see cref="T:System.Linq.Expressions.Expression" /> to set the <see cref="P:System.Linq.Expressions.BinaryExpression.Left" /> property equal to.</param>
		/// <param name="right">An <see cref="T:System.Linq.Expressions.Expression" /> to set the <see cref="P:System.Linq.Expressions.BinaryExpression.Right" /> property equal to.</param>
		/// <param name="method">A <see cref="T:System.Reflection.MethodInfo" /> to set the <see cref="P:System.Linq.Expressions.BinaryExpression.Method" /> property equal to.</param>
		/// <returns>A <see cref="T:System.Linq.Expressions.BinaryExpression" /> that has the <see cref="P:System.Linq.Expressions.Expression.NodeType" /> property equal to <see cref="F:System.Linq.Expressions.ExpressionType.Multiply" /> and the <see cref="P:System.Linq.Expressions.BinaryExpression.Left" />, <see cref="P:System.Linq.Expressions.BinaryExpression.Right" />, and <see cref="P:System.Linq.Expressions.BinaryExpression.Method" /> properties set to the specified values.</returns>
		/// <exception cref="T:System.ArgumentNullException">
		///         <paramref name="left" /> or <paramref name="right" /> is <see langword="null" />.</exception>
		/// <exception cref="T:System.ArgumentException">
		///         <paramref name="method" /> is not <see langword="null" /> and the method it represents returns <see langword="void" />, is not <see langword="static" /> (<see langword="Shared" /> in Visual Basic), or does not take exactly two arguments.</exception>
		/// <exception cref="T:System.InvalidOperationException">
		///         <paramref name="method" /> is <see langword="null" /> and the multiplication operator is not defined for <paramref name="left" />.Type and <paramref name="right" />.Type.</exception>
		public static BinaryExpression Multiply(Expression left, Expression right, MethodInfo method)
		{
			ExpressionUtils.RequiresCanRead(left, "left");
			ExpressionUtils.RequiresCanRead(right, "right");
			if (!(method == null))
			{
				return Expression.GetMethodBasedBinaryOperator(ExpressionType.Multiply, left, right, method, true);
			}
			if (left.Type == right.Type && left.Type.IsArithmetic())
			{
				return new SimpleBinaryExpression(ExpressionType.Multiply, left, right, left.Type);
			}
			return Expression.GetUserDefinedBinaryOperatorOrThrow(ExpressionType.Multiply, "op_Multiply", left, right, true);
		}

		/// <summary>Creates a <see cref="T:System.Linq.Expressions.BinaryExpression" /> that represents a multiplication assignment operation that does not have overflow checking.</summary>
		/// <param name="left">An <see cref="T:System.Linq.Expressions.Expression" /> to set the <see cref="P:System.Linq.Expressions.BinaryExpression.Left" /> property equal to.</param>
		/// <param name="right">An <see cref="T:System.Linq.Expressions.Expression" /> to set the <see cref="P:System.Linq.Expressions.BinaryExpression.Right" /> property equal to.</param>
		/// <returns>A <see cref="T:System.Linq.Expressions.BinaryExpression" /> that has the <see cref="P:System.Linq.Expressions.Expression.NodeType" /> property equal to <see cref="F:System.Linq.Expressions.ExpressionType.MultiplyAssign" /> and the <see cref="P:System.Linq.Expressions.BinaryExpression.Left" /> and <see cref="P:System.Linq.Expressions.BinaryExpression.Right" /> properties set to the specified values.</returns>
		public static BinaryExpression MultiplyAssign(Expression left, Expression right)
		{
			return Expression.MultiplyAssign(left, right, null, null);
		}

		/// <summary>Creates a <see cref="T:System.Linq.Expressions.BinaryExpression" /> that represents a multiplication assignment operation that does not have overflow checking.</summary>
		/// <param name="left">An <see cref="T:System.Linq.Expressions.Expression" /> to set the <see cref="P:System.Linq.Expressions.BinaryExpression.Left" /> property equal to.</param>
		/// <param name="right">An <see cref="T:System.Linq.Expressions.Expression" /> to set the <see cref="P:System.Linq.Expressions.BinaryExpression.Right" /> property equal to.</param>
		/// <param name="method">A <see cref="T:System.Reflection.MethodInfo" /> to set the <see cref="P:System.Linq.Expressions.BinaryExpression.Method" /> property equal to.</param>
		/// <returns>A <see cref="T:System.Linq.Expressions.BinaryExpression" /> that has the <see cref="P:System.Linq.Expressions.Expression.NodeType" /> property equal to <see cref="F:System.Linq.Expressions.ExpressionType.MultiplyAssign" /> and the <see cref="P:System.Linq.Expressions.BinaryExpression.Left" />, <see cref="P:System.Linq.Expressions.BinaryExpression.Right" />, and <see cref="P:System.Linq.Expressions.BinaryExpression.Method" /> properties set to the specified values.</returns>
		public static BinaryExpression MultiplyAssign(Expression left, Expression right, MethodInfo method)
		{
			return Expression.MultiplyAssign(left, right, method, null);
		}

		/// <summary>Creates a <see cref="T:System.Linq.Expressions.BinaryExpression" /> that represents a multiplication assignment operation that does not have overflow checking.</summary>
		/// <param name="left">An <see cref="T:System.Linq.Expressions.Expression" /> to set the <see cref="P:System.Linq.Expressions.BinaryExpression.Left" /> property equal to.</param>
		/// <param name="right">An <see cref="T:System.Linq.Expressions.Expression" /> to set the <see cref="P:System.Linq.Expressions.BinaryExpression.Right" /> property equal to.</param>
		/// <param name="method">A <see cref="T:System.Reflection.MethodInfo" /> to set the <see cref="P:System.Linq.Expressions.BinaryExpression.Method" /> property equal to.</param>
		/// <param name="conversion">A <see cref="T:System.Linq.Expressions.LambdaExpression" /> to set the <see cref="P:System.Linq.Expressions.BinaryExpression.Conversion" /> property equal to.</param>
		/// <returns>A <see cref="T:System.Linq.Expressions.BinaryExpression" /> that has the <see cref="P:System.Linq.Expressions.Expression.NodeType" /> property equal to <see cref="F:System.Linq.Expressions.ExpressionType.MultiplyAssign" /> and the <see cref="P:System.Linq.Expressions.BinaryExpression.Left" />, <see cref="P:System.Linq.Expressions.BinaryExpression.Right" />, <see cref="P:System.Linq.Expressions.BinaryExpression.Method" />, and <see cref="P:System.Linq.Expressions.BinaryExpression.Conversion" /> properties set to the specified values.</returns>
		public static BinaryExpression MultiplyAssign(Expression left, Expression right, MethodInfo method, LambdaExpression conversion)
		{
			ExpressionUtils.RequiresCanRead(left, "left");
			Expression.RequiresCanWrite(left, "left");
			ExpressionUtils.RequiresCanRead(right, "right");
			if (!(method == null))
			{
				return Expression.GetMethodBasedAssignOperator(ExpressionType.MultiplyAssign, left, right, method, conversion, true);
			}
			if (!(left.Type == right.Type) || !left.Type.IsArithmetic())
			{
				return Expression.GetUserDefinedAssignOperatorOrThrow(ExpressionType.MultiplyAssign, "op_Multiply", left, right, conversion, true);
			}
			if (conversion != null)
			{
				throw Error.ConversionIsNotSupportedForArithmeticTypes();
			}
			return new SimpleBinaryExpression(ExpressionType.MultiplyAssign, left, right, left.Type);
		}

		/// <summary>Creates a <see cref="T:System.Linq.Expressions.BinaryExpression" /> that represents a multiplication assignment operation that has overflow checking.</summary>
		/// <param name="left">An <see cref="T:System.Linq.Expressions.Expression" /> to set the <see cref="P:System.Linq.Expressions.BinaryExpression.Left" /> property equal to.</param>
		/// <param name="right">An <see cref="T:System.Linq.Expressions.Expression" /> to set the <see cref="P:System.Linq.Expressions.BinaryExpression.Right" /> property equal to.</param>
		/// <returns>A <see cref="T:System.Linq.Expressions.BinaryExpression" /> that has the <see cref="P:System.Linq.Expressions.Expression.NodeType" /> property equal to <see cref="F:System.Linq.Expressions.ExpressionType.MultiplyAssignChecked" /> and the <see cref="P:System.Linq.Expressions.BinaryExpression.Left" /> and <see cref="P:System.Linq.Expressions.BinaryExpression.Right" /> properties set to the specified values.</returns>
		public static BinaryExpression MultiplyAssignChecked(Expression left, Expression right)
		{
			return Expression.MultiplyAssignChecked(left, right, null);
		}

		/// <summary>Creates a <see cref="T:System.Linq.Expressions.BinaryExpression" /> that represents a multiplication assignment operation that has overflow checking.</summary>
		/// <param name="left">An <see cref="T:System.Linq.Expressions.Expression" /> to set the <see cref="P:System.Linq.Expressions.BinaryExpression.Left" /> property equal to.</param>
		/// <param name="right">An <see cref="T:System.Linq.Expressions.Expression" /> to set the <see cref="P:System.Linq.Expressions.BinaryExpression.Right" /> property equal to.</param>
		/// <param name="method">A <see cref="T:System.Reflection.MethodInfo" /> to set the <see cref="P:System.Linq.Expressions.BinaryExpression.Method" /> property equal to.</param>
		/// <returns>A <see cref="T:System.Linq.Expressions.BinaryExpression" /> that has the <see cref="P:System.Linq.Expressions.Expression.NodeType" /> property equal to <see cref="F:System.Linq.Expressions.ExpressionType.MultiplyAssignChecked" /> and the <see cref="P:System.Linq.Expressions.BinaryExpression.Left" />, <see cref="P:System.Linq.Expressions.BinaryExpression.Right" />, and <see cref="P:System.Linq.Expressions.BinaryExpression.Method" /> properties set to the specified values.</returns>
		public static BinaryExpression MultiplyAssignChecked(Expression left, Expression right, MethodInfo method)
		{
			return Expression.MultiplyAssignChecked(left, right, method, null);
		}

		/// <summary>Creates a <see cref="T:System.Linq.Expressions.BinaryExpression" /> that represents a multiplication assignment operation that has overflow checking.</summary>
		/// <param name="left">An <see cref="T:System.Linq.Expressions.Expression" /> to set the <see cref="P:System.Linq.Expressions.BinaryExpression.Left" /> property equal to.</param>
		/// <param name="right">An <see cref="T:System.Linq.Expressions.Expression" /> to set the <see cref="P:System.Linq.Expressions.BinaryExpression.Right" /> property equal to.</param>
		/// <param name="method">A <see cref="T:System.Reflection.MethodInfo" /> to set the <see cref="P:System.Linq.Expressions.BinaryExpression.Method" /> property equal to.</param>
		/// <param name="conversion">A <see cref="T:System.Linq.Expressions.LambdaExpression" /> to set the <see cref="P:System.Linq.Expressions.BinaryExpression.Conversion" /> property equal to.</param>
		/// <returns>A <see cref="T:System.Linq.Expressions.BinaryExpression" /> that has the <see cref="P:System.Linq.Expressions.Expression.NodeType" /> property equal to <see cref="F:System.Linq.Expressions.ExpressionType.MultiplyAssignChecked" /> and the <see cref="P:System.Linq.Expressions.BinaryExpression.Left" />, <see cref="P:System.Linq.Expressions.BinaryExpression.Right" />, <see cref="P:System.Linq.Expressions.BinaryExpression.Method" />, and <see cref="P:System.Linq.Expressions.BinaryExpression.Conversion" /> properties set to the specified values.</returns>
		public static BinaryExpression MultiplyAssignChecked(Expression left, Expression right, MethodInfo method, LambdaExpression conversion)
		{
			ExpressionUtils.RequiresCanRead(left, "left");
			Expression.RequiresCanWrite(left, "left");
			ExpressionUtils.RequiresCanRead(right, "right");
			if (!(method == null))
			{
				return Expression.GetMethodBasedAssignOperator(ExpressionType.MultiplyAssignChecked, left, right, method, conversion, true);
			}
			if (!(left.Type == right.Type) || !left.Type.IsArithmetic())
			{
				return Expression.GetUserDefinedAssignOperatorOrThrow(ExpressionType.MultiplyAssignChecked, "op_Multiply", left, right, conversion, true);
			}
			if (conversion != null)
			{
				throw Error.ConversionIsNotSupportedForArithmeticTypes();
			}
			return new SimpleBinaryExpression(ExpressionType.MultiplyAssignChecked, left, right, left.Type);
		}

		/// <summary>Creates a <see cref="T:System.Linq.Expressions.BinaryExpression" /> that represents an arithmetic multiplication operation that has overflow checking.</summary>
		/// <param name="left">An <see cref="T:System.Linq.Expressions.Expression" /> to set the <see cref="P:System.Linq.Expressions.BinaryExpression.Left" /> property equal to.</param>
		/// <param name="right">An <see cref="T:System.Linq.Expressions.Expression" /> to set the <see cref="P:System.Linq.Expressions.BinaryExpression.Right" /> property equal to.</param>
		/// <returns>A <see cref="T:System.Linq.Expressions.BinaryExpression" /> that has the <see cref="P:System.Linq.Expressions.Expression.NodeType" /> property equal to <see cref="F:System.Linq.Expressions.ExpressionType.MultiplyChecked" /> and the <see cref="P:System.Linq.Expressions.BinaryExpression.Left" /> and <see cref="P:System.Linq.Expressions.BinaryExpression.Right" /> properties set to the specified values.</returns>
		/// <exception cref="T:System.ArgumentNullException">
		///         <paramref name="left" /> or <paramref name="right" /> is <see langword="null" />.</exception>
		/// <exception cref="T:System.InvalidOperationException">The multiplication operator is not defined for <paramref name="left" />.Type and <paramref name="right" />.Type.</exception>
		public static BinaryExpression MultiplyChecked(Expression left, Expression right)
		{
			return Expression.MultiplyChecked(left, right, null);
		}

		/// <summary>Creates a <see cref="T:System.Linq.Expressions.BinaryExpression" /> that represents an arithmetic multiplication operation that has overflow checking.</summary>
		/// <param name="left">An <see cref="T:System.Linq.Expressions.Expression" /> to set the <see cref="P:System.Linq.Expressions.BinaryExpression.Left" /> property equal to.</param>
		/// <param name="right">An <see cref="T:System.Linq.Expressions.Expression" /> to set the <see cref="P:System.Linq.Expressions.BinaryExpression.Right" /> property equal to.</param>
		/// <param name="method">A <see cref="T:System.Reflection.MethodInfo" /> to set the <see cref="P:System.Linq.Expressions.BinaryExpression.Method" /> property equal to.</param>
		/// <returns>A <see cref="T:System.Linq.Expressions.BinaryExpression" /> that has the <see cref="P:System.Linq.Expressions.Expression.NodeType" /> property equal to <see cref="F:System.Linq.Expressions.ExpressionType.MultiplyChecked" /> and the <see cref="P:System.Linq.Expressions.BinaryExpression.Left" />, <see cref="P:System.Linq.Expressions.BinaryExpression.Right" />, and <see cref="P:System.Linq.Expressions.BinaryExpression.Method" /> properties set to the specified values.</returns>
		/// <exception cref="T:System.ArgumentNullException">
		///         <paramref name="left" /> or <paramref name="right" /> is <see langword="null" />.</exception>
		/// <exception cref="T:System.ArgumentException">
		///         <paramref name="method" /> is not <see langword="null" /> and the method it represents returns <see langword="void" />, is not <see langword="static" /> (<see langword="Shared" /> in Visual Basic), or does not take exactly two arguments.</exception>
		/// <exception cref="T:System.InvalidOperationException">
		///         <paramref name="method" /> is <see langword="null" /> and the multiplication operator is not defined for <paramref name="left" />.Type and <paramref name="right" />.Type.</exception>
		public static BinaryExpression MultiplyChecked(Expression left, Expression right, MethodInfo method)
		{
			ExpressionUtils.RequiresCanRead(left, "left");
			ExpressionUtils.RequiresCanRead(right, "right");
			if (!(method == null))
			{
				return Expression.GetMethodBasedBinaryOperator(ExpressionType.MultiplyChecked, left, right, method, true);
			}
			if (left.Type == right.Type && left.Type.IsArithmetic())
			{
				return new SimpleBinaryExpression(ExpressionType.MultiplyChecked, left, right, left.Type);
			}
			return Expression.GetUserDefinedBinaryOperatorOrThrow(ExpressionType.MultiplyChecked, "op_Multiply", left, right, true);
		}

		private static bool IsSimpleShift(Type left, Type right)
		{
			return left.IsInteger() && right.GetNonNullableType() == typeof(int);
		}

		private static Type GetResultTypeOfShift(Type left, Type right)
		{
			if (!left.IsNullableType() && right.IsNullableType())
			{
				return typeof(Nullable<>).MakeGenericType(new Type[]
				{
					left
				});
			}
			return left;
		}

		/// <summary>Creates a <see cref="T:System.Linq.Expressions.BinaryExpression" /> that represents a bitwise left-shift operation.</summary>
		/// <param name="left">An <see cref="T:System.Linq.Expressions.Expression" /> to set the <see cref="P:System.Linq.Expressions.BinaryExpression.Left" /> property equal to.</param>
		/// <param name="right">An <see cref="T:System.Linq.Expressions.Expression" /> to set the <see cref="P:System.Linq.Expressions.BinaryExpression.Right" /> property equal to.</param>
		/// <returns>A <see cref="T:System.Linq.Expressions.BinaryExpression" /> that has the <see cref="P:System.Linq.Expressions.Expression.NodeType" /> property equal to <see cref="F:System.Linq.Expressions.ExpressionType.LeftShift" /> and the <see cref="P:System.Linq.Expressions.BinaryExpression.Left" /> and <see cref="P:System.Linq.Expressions.BinaryExpression.Right" /> properties set to the specified values.</returns>
		/// <exception cref="T:System.ArgumentNullException">
		///         <paramref name="left" /> or <paramref name="right" /> is <see langword="null" />.</exception>
		/// <exception cref="T:System.InvalidOperationException">The left-shift operator is not defined for <paramref name="left" />.Type and <paramref name="right" />.Type.</exception>
		public static BinaryExpression LeftShift(Expression left, Expression right)
		{
			return Expression.LeftShift(left, right, null);
		}

		/// <summary>Creates a <see cref="T:System.Linq.Expressions.BinaryExpression" /> that represents a bitwise left-shift operation.</summary>
		/// <param name="left">An <see cref="T:System.Linq.Expressions.Expression" /> to set the <see cref="P:System.Linq.Expressions.BinaryExpression.Left" /> property equal to.</param>
		/// <param name="right">An <see cref="T:System.Linq.Expressions.Expression" /> to set the <see cref="P:System.Linq.Expressions.BinaryExpression.Right" /> property equal to.</param>
		/// <param name="method">A <see cref="T:System.Reflection.MethodInfo" /> to set the <see cref="P:System.Linq.Expressions.BinaryExpression.Method" /> property equal to.</param>
		/// <returns>A <see cref="T:System.Linq.Expressions.BinaryExpression" /> that has the <see cref="P:System.Linq.Expressions.Expression.NodeType" /> property equal to <see cref="F:System.Linq.Expressions.ExpressionType.LeftShift" /> and the <see cref="P:System.Linq.Expressions.BinaryExpression.Left" />, <see cref="P:System.Linq.Expressions.BinaryExpression.Right" />, and <see cref="P:System.Linq.Expressions.BinaryExpression.Method" /> properties set to the specified values.</returns>
		/// <exception cref="T:System.ArgumentNullException">
		///         <paramref name="left" /> or <paramref name="right" /> is <see langword="null" />.</exception>
		/// <exception cref="T:System.ArgumentException">
		///         <paramref name="method" /> is not <see langword="null" /> and the method it represents returns <see langword="void" />, is not <see langword="static" /> (<see langword="Shared" /> in Visual Basic), or does not take exactly two arguments.</exception>
		/// <exception cref="T:System.InvalidOperationException">
		///         <paramref name="method" /> is <see langword="null" /> and the left-shift operator is not defined for <paramref name="left" />.Type and <paramref name="right" />.Type.</exception>
		public static BinaryExpression LeftShift(Expression left, Expression right, MethodInfo method)
		{
			ExpressionUtils.RequiresCanRead(left, "left");
			ExpressionUtils.RequiresCanRead(right, "right");
			if (!(method == null))
			{
				return Expression.GetMethodBasedBinaryOperator(ExpressionType.LeftShift, left, right, method, true);
			}
			if (Expression.IsSimpleShift(left.Type, right.Type))
			{
				Type resultTypeOfShift = Expression.GetResultTypeOfShift(left.Type, right.Type);
				return new SimpleBinaryExpression(ExpressionType.LeftShift, left, right, resultTypeOfShift);
			}
			return Expression.GetUserDefinedBinaryOperatorOrThrow(ExpressionType.LeftShift, "op_LeftShift", left, right, true);
		}

		/// <summary>Creates a <see cref="T:System.Linq.Expressions.BinaryExpression" /> that represents a bitwise left-shift assignment operation.</summary>
		/// <param name="left">An <see cref="T:System.Linq.Expressions.Expression" /> to set the <see cref="P:System.Linq.Expressions.BinaryExpression.Left" /> property equal to.</param>
		/// <param name="right">An <see cref="T:System.Linq.Expressions.Expression" /> to set the <see cref="P:System.Linq.Expressions.BinaryExpression.Right" /> property equal to.</param>
		/// <returns>A <see cref="T:System.Linq.Expressions.BinaryExpression" /> that has the <see cref="P:System.Linq.Expressions.Expression.NodeType" /> property equal to <see cref="F:System.Linq.Expressions.ExpressionType.LeftShiftAssign" /> and the <see cref="P:System.Linq.Expressions.BinaryExpression.Left" /> and <see cref="P:System.Linq.Expressions.BinaryExpression.Right" /> properties set to the specified values.</returns>
		public static BinaryExpression LeftShiftAssign(Expression left, Expression right)
		{
			return Expression.LeftShiftAssign(left, right, null, null);
		}

		/// <summary>Creates a <see cref="T:System.Linq.Expressions.BinaryExpression" /> that represents a bitwise left-shift assignment operation.</summary>
		/// <param name="left">An <see cref="T:System.Linq.Expressions.Expression" /> to set the <see cref="P:System.Linq.Expressions.BinaryExpression.Left" /> property equal to.</param>
		/// <param name="right">An <see cref="T:System.Linq.Expressions.Expression" /> to set the <see cref="P:System.Linq.Expressions.BinaryExpression.Right" /> property equal to.</param>
		/// <param name="method">A <see cref="T:System.Reflection.MethodInfo" /> to set the <see cref="P:System.Linq.Expressions.BinaryExpression.Method" /> property equal to.</param>
		/// <returns>A <see cref="T:System.Linq.Expressions.BinaryExpression" /> that has the <see cref="P:System.Linq.Expressions.Expression.NodeType" /> property equal to <see cref="F:System.Linq.Expressions.ExpressionType.LeftShiftAssign" /> and the <see cref="P:System.Linq.Expressions.BinaryExpression.Left" />, <see cref="P:System.Linq.Expressions.BinaryExpression.Right" />, and <see cref="P:System.Linq.Expressions.BinaryExpression.Method" /> properties set to the specified values.</returns>
		public static BinaryExpression LeftShiftAssign(Expression left, Expression right, MethodInfo method)
		{
			return Expression.LeftShiftAssign(left, right, method, null);
		}

		/// <summary>Creates a <see cref="T:System.Linq.Expressions.BinaryExpression" /> that represents a bitwise left-shift assignment operation.</summary>
		/// <param name="left">An <see cref="T:System.Linq.Expressions.Expression" /> to set the <see cref="P:System.Linq.Expressions.BinaryExpression.Left" /> property equal to.</param>
		/// <param name="right">An <see cref="T:System.Linq.Expressions.Expression" /> to set the <see cref="P:System.Linq.Expressions.BinaryExpression.Right" /> property equal to.</param>
		/// <param name="method">A <see cref="T:System.Reflection.MethodInfo" /> to set the <see cref="P:System.Linq.Expressions.BinaryExpression.Method" /> property equal to.</param>
		/// <param name="conversion">A <see cref="T:System.Linq.Expressions.LambdaExpression" /> to set the <see cref="P:System.Linq.Expressions.BinaryExpression.Conversion" /> property equal to.</param>
		/// <returns>A <see cref="T:System.Linq.Expressions.BinaryExpression" /> that has the <see cref="P:System.Linq.Expressions.Expression.NodeType" /> property equal to <see cref="F:System.Linq.Expressions.ExpressionType.LeftShiftAssign" /> and the <see cref="P:System.Linq.Expressions.BinaryExpression.Left" />, <see cref="P:System.Linq.Expressions.BinaryExpression.Right" />, <see cref="P:System.Linq.Expressions.BinaryExpression.Method" />, and <see cref="P:System.Linq.Expressions.BinaryExpression.Conversion" /> properties set to the specified values.</returns>
		public static BinaryExpression LeftShiftAssign(Expression left, Expression right, MethodInfo method, LambdaExpression conversion)
		{
			ExpressionUtils.RequiresCanRead(left, "left");
			Expression.RequiresCanWrite(left, "left");
			ExpressionUtils.RequiresCanRead(right, "right");
			if (!(method == null))
			{
				return Expression.GetMethodBasedAssignOperator(ExpressionType.LeftShiftAssign, left, right, method, conversion, true);
			}
			if (!Expression.IsSimpleShift(left.Type, right.Type))
			{
				return Expression.GetUserDefinedAssignOperatorOrThrow(ExpressionType.LeftShiftAssign, "op_LeftShift", left, right, conversion, true);
			}
			if (conversion != null)
			{
				throw Error.ConversionIsNotSupportedForArithmeticTypes();
			}
			Type resultTypeOfShift = Expression.GetResultTypeOfShift(left.Type, right.Type);
			return new SimpleBinaryExpression(ExpressionType.LeftShiftAssign, left, right, resultTypeOfShift);
		}

		/// <summary>Creates a <see cref="T:System.Linq.Expressions.BinaryExpression" /> that represents a bitwise right-shift operation.</summary>
		/// <param name="left">An <see cref="T:System.Linq.Expressions.Expression" /> to set the <see cref="P:System.Linq.Expressions.BinaryExpression.Left" /> property equal to.</param>
		/// <param name="right">An <see cref="T:System.Linq.Expressions.Expression" /> to set the <see cref="P:System.Linq.Expressions.BinaryExpression.Right" /> property equal to.</param>
		/// <returns>A <see cref="T:System.Linq.Expressions.BinaryExpression" /> that has the <see cref="P:System.Linq.Expressions.Expression.NodeType" /> property equal to <see cref="F:System.Linq.Expressions.ExpressionType.RightShift" /> and the <see cref="P:System.Linq.Expressions.BinaryExpression.Left" /> and <see cref="P:System.Linq.Expressions.BinaryExpression.Right" /> properties set to the specified values.</returns>
		/// <exception cref="T:System.ArgumentNullException">
		///         <paramref name="left" /> or <paramref name="right" /> is <see langword="null" />.</exception>
		/// <exception cref="T:System.InvalidOperationException">The right-shift operator is not defined for <paramref name="left" />.Type and <paramref name="right" />.Type.</exception>
		public static BinaryExpression RightShift(Expression left, Expression right)
		{
			return Expression.RightShift(left, right, null);
		}

		/// <summary>Creates a <see cref="T:System.Linq.Expressions.BinaryExpression" /> that represents a bitwise right-shift operation.</summary>
		/// <param name="left">An <see cref="T:System.Linq.Expressions.Expression" /> to set the <see cref="P:System.Linq.Expressions.BinaryExpression.Left" /> property equal to.</param>
		/// <param name="right">An <see cref="T:System.Linq.Expressions.Expression" /> to set the <see cref="P:System.Linq.Expressions.BinaryExpression.Right" /> property equal to.</param>
		/// <param name="method">A <see cref="T:System.Reflection.MethodInfo" /> to set the <see cref="P:System.Linq.Expressions.BinaryExpression.Method" /> property equal to.</param>
		/// <returns>A <see cref="T:System.Linq.Expressions.BinaryExpression" /> that has the <see cref="P:System.Linq.Expressions.Expression.NodeType" /> property equal to <see cref="F:System.Linq.Expressions.ExpressionType.RightShift" /> and the <see cref="P:System.Linq.Expressions.BinaryExpression.Left" />, <see cref="P:System.Linq.Expressions.BinaryExpression.Right" />, and <see cref="P:System.Linq.Expressions.BinaryExpression.Method" /> properties set to the specified values.</returns>
		/// <exception cref="T:System.ArgumentNullException">
		///         <paramref name="left" /> or <paramref name="right" /> is <see langword="null" />.</exception>
		/// <exception cref="T:System.ArgumentException">
		///         <paramref name="method" /> is not <see langword="null" /> and the method it represents returns <see langword="void" />, is not <see langword="static" /> (<see langword="Shared" /> in Visual Basic), or does not take exactly two arguments.</exception>
		/// <exception cref="T:System.InvalidOperationException">
		///         <paramref name="method" /> is <see langword="null" /> and the right-shift operator is not defined for <paramref name="left" />.Type and <paramref name="right" />.Type.</exception>
		public static BinaryExpression RightShift(Expression left, Expression right, MethodInfo method)
		{
			ExpressionUtils.RequiresCanRead(left, "left");
			ExpressionUtils.RequiresCanRead(right, "right");
			if (!(method == null))
			{
				return Expression.GetMethodBasedBinaryOperator(ExpressionType.RightShift, left, right, method, true);
			}
			if (Expression.IsSimpleShift(left.Type, right.Type))
			{
				Type resultTypeOfShift = Expression.GetResultTypeOfShift(left.Type, right.Type);
				return new SimpleBinaryExpression(ExpressionType.RightShift, left, right, resultTypeOfShift);
			}
			return Expression.GetUserDefinedBinaryOperatorOrThrow(ExpressionType.RightShift, "op_RightShift", left, right, true);
		}

		/// <summary>Creates a <see cref="T:System.Linq.Expressions.BinaryExpression" /> that represents a bitwise right-shift assignment operation.</summary>
		/// <param name="left">An <see cref="T:System.Linq.Expressions.Expression" /> to set the <see cref="P:System.Linq.Expressions.BinaryExpression.Left" /> property equal to.</param>
		/// <param name="right">An <see cref="T:System.Linq.Expressions.Expression" /> to set the <see cref="P:System.Linq.Expressions.BinaryExpression.Right" /> property equal to.</param>
		/// <returns>A <see cref="T:System.Linq.Expressions.BinaryExpression" /> that has the <see cref="P:System.Linq.Expressions.Expression.NodeType" /> property equal to <see cref="F:System.Linq.Expressions.ExpressionType.RightShiftAssign" /> and the <see cref="P:System.Linq.Expressions.BinaryExpression.Left" /> and <see cref="P:System.Linq.Expressions.BinaryExpression.Right" /> properties set to the specified values.</returns>
		public static BinaryExpression RightShiftAssign(Expression left, Expression right)
		{
			return Expression.RightShiftAssign(left, right, null, null);
		}

		/// <summary>Creates a <see cref="T:System.Linq.Expressions.BinaryExpression" /> that represents a bitwise right-shift assignment operation.</summary>
		/// <param name="left">An <see cref="T:System.Linq.Expressions.Expression" /> to set the <see cref="P:System.Linq.Expressions.BinaryExpression.Left" /> property equal to.</param>
		/// <param name="right">An <see cref="T:System.Linq.Expressions.Expression" /> to set the <see cref="P:System.Linq.Expressions.BinaryExpression.Right" /> property equal to.</param>
		/// <param name="method">A <see cref="T:System.Reflection.MethodInfo" /> to set the <see cref="P:System.Linq.Expressions.BinaryExpression.Method" /> property equal to.</param>
		/// <returns>A <see cref="T:System.Linq.Expressions.BinaryExpression" /> that has the <see cref="P:System.Linq.Expressions.Expression.NodeType" /> property equal to <see cref="F:System.Linq.Expressions.ExpressionType.RightShiftAssign" /> and the <see cref="P:System.Linq.Expressions.BinaryExpression.Left" />, <see cref="P:System.Linq.Expressions.BinaryExpression.Right" />, and <see cref="P:System.Linq.Expressions.BinaryExpression.Method" /> properties set to the specified values.</returns>
		public static BinaryExpression RightShiftAssign(Expression left, Expression right, MethodInfo method)
		{
			return Expression.RightShiftAssign(left, right, method, null);
		}

		/// <summary>Creates a <see cref="T:System.Linq.Expressions.BinaryExpression" /> that represents a bitwise right-shift assignment operation.</summary>
		/// <param name="left">An <see cref="T:System.Linq.Expressions.Expression" /> to set the <see cref="P:System.Linq.Expressions.BinaryExpression.Left" /> property equal to.</param>
		/// <param name="right">An <see cref="T:System.Linq.Expressions.Expression" /> to set the <see cref="P:System.Linq.Expressions.BinaryExpression.Right" /> property equal to.</param>
		/// <param name="method">A <see cref="T:System.Reflection.MethodInfo" /> to set the <see cref="P:System.Linq.Expressions.BinaryExpression.Method" /> property equal to.</param>
		/// <param name="conversion">A <see cref="T:System.Linq.Expressions.LambdaExpression" /> to set the <see cref="P:System.Linq.Expressions.BinaryExpression.Conversion" /> property equal to.</param>
		/// <returns>A <see cref="T:System.Linq.Expressions.BinaryExpression" /> that has the <see cref="P:System.Linq.Expressions.Expression.NodeType" /> property equal to <see cref="F:System.Linq.Expressions.ExpressionType.RightShiftAssign" /> and the <see cref="P:System.Linq.Expressions.BinaryExpression.Left" />, <see cref="P:System.Linq.Expressions.BinaryExpression.Right" />, <see cref="P:System.Linq.Expressions.BinaryExpression.Method" />, and <see cref="P:System.Linq.Expressions.BinaryExpression.Conversion" /> properties set to the specified values.</returns>
		public static BinaryExpression RightShiftAssign(Expression left, Expression right, MethodInfo method, LambdaExpression conversion)
		{
			ExpressionUtils.RequiresCanRead(left, "left");
			Expression.RequiresCanWrite(left, "left");
			ExpressionUtils.RequiresCanRead(right, "right");
			if (!(method == null))
			{
				return Expression.GetMethodBasedAssignOperator(ExpressionType.RightShiftAssign, left, right, method, conversion, true);
			}
			if (!Expression.IsSimpleShift(left.Type, right.Type))
			{
				return Expression.GetUserDefinedAssignOperatorOrThrow(ExpressionType.RightShiftAssign, "op_RightShift", left, right, conversion, true);
			}
			if (conversion != null)
			{
				throw Error.ConversionIsNotSupportedForArithmeticTypes();
			}
			Type resultTypeOfShift = Expression.GetResultTypeOfShift(left.Type, right.Type);
			return new SimpleBinaryExpression(ExpressionType.RightShiftAssign, left, right, resultTypeOfShift);
		}

		/// <summary>Creates a <see cref="T:System.Linq.Expressions.BinaryExpression" /> that represents a bitwise <see langword="AND" /> operation.</summary>
		/// <param name="left">A <see cref="T:System.Linq.Expressions.Expression" /> to set the <see cref="P:System.Linq.Expressions.BinaryExpression.Left" /> property equal to.</param>
		/// <param name="right">A <see cref="T:System.Linq.Expressions.Expression" /> to set the <see cref="P:System.Linq.Expressions.BinaryExpression.Right" /> property equal to.</param>
		/// <returns>A <see cref="T:System.Linq.Expressions.BinaryExpression" /> that has the <see cref="P:System.Linq.Expressions.Expression.NodeType" /> property equal to <see cref="F:System.Linq.Expressions.ExpressionType.And" /> and the <see cref="P:System.Linq.Expressions.BinaryExpression.Left" /> and <see cref="P:System.Linq.Expressions.BinaryExpression.Right" /> properties set to the specified values.</returns>
		/// <exception cref="T:System.ArgumentNullException">
		///         <paramref name="left" /> or <paramref name="right" /> is <see langword="null" />.</exception>
		/// <exception cref="T:System.InvalidOperationException">The bitwise <see langword="AND" /> operator is not defined for <paramref name="left" />.Type and <paramref name="right" />.Type.</exception>
		public static BinaryExpression And(Expression left, Expression right)
		{
			return Expression.And(left, right, null);
		}

		/// <summary>Creates a <see cref="T:System.Linq.Expressions.BinaryExpression" /> that represents a bitwise <see langword="AND" /> operation. The implementing method can be specified.</summary>
		/// <param name="left">An <see cref="T:System.Linq.Expressions.Expression" /> to set the <see cref="P:System.Linq.Expressions.BinaryExpression.Left" /> property equal to.</param>
		/// <param name="right">An <see cref="T:System.Linq.Expressions.Expression" /> to set the <see cref="P:System.Linq.Expressions.BinaryExpression.Right" /> property equal to.</param>
		/// <param name="method">A <see cref="T:System.Reflection.MethodInfo" /> to set the <see cref="P:System.Linq.Expressions.BinaryExpression.Method" /> property equal to.</param>
		/// <returns>A <see cref="T:System.Linq.Expressions.BinaryExpression" /> that has the <see cref="P:System.Linq.Expressions.Expression.NodeType" /> property equal to <see cref="F:System.Linq.Expressions.ExpressionType.And" /> and the <see cref="P:System.Linq.Expressions.BinaryExpression.Left" />, <see cref="P:System.Linq.Expressions.BinaryExpression.Right" />, and <see cref="P:System.Linq.Expressions.BinaryExpression.Method" /> properties set to the specified values.</returns>
		/// <exception cref="T:System.ArgumentNullException">
		///         <paramref name="left" /> or <paramref name="right" /> is <see langword="null" />.</exception>
		/// <exception cref="T:System.ArgumentException">
		///         <paramref name="method" /> is not <see langword="null" /> and the method it represents returns <see langword="void" />, is not <see langword="static" /> (<see langword="Shared" /> in Visual Basic), or does not take exactly two arguments.</exception>
		/// <exception cref="T:System.InvalidOperationException">
		///         <paramref name="method" /> is <see langword="null" /> and the bitwise <see langword="AND" /> operator is not defined for <paramref name="left" />.Type and <paramref name="right" />.Type.</exception>
		public static BinaryExpression And(Expression left, Expression right, MethodInfo method)
		{
			ExpressionUtils.RequiresCanRead(left, "left");
			ExpressionUtils.RequiresCanRead(right, "right");
			if (!(method == null))
			{
				return Expression.GetMethodBasedBinaryOperator(ExpressionType.And, left, right, method, true);
			}
			if (left.Type == right.Type && left.Type.IsIntegerOrBool())
			{
				return new SimpleBinaryExpression(ExpressionType.And, left, right, left.Type);
			}
			return Expression.GetUserDefinedBinaryOperatorOrThrow(ExpressionType.And, "op_BitwiseAnd", left, right, true);
		}

		/// <summary>Creates a <see cref="T:System.Linq.Expressions.BinaryExpression" /> that represents a bitwise AND assignment operation.</summary>
		/// <param name="left">An <see cref="T:System.Linq.Expressions.Expression" /> to set the <see cref="P:System.Linq.Expressions.BinaryExpression.Left" /> property equal to.</param>
		/// <param name="right">An <see cref="T:System.Linq.Expressions.Expression" /> to set the <see cref="P:System.Linq.Expressions.BinaryExpression.Right" /> property equal to.</param>
		/// <returns>A <see cref="T:System.Linq.Expressions.BinaryExpression" /> that has the <see cref="P:System.Linq.Expressions.Expression.NodeType" /> property equal to <see cref="F:System.Linq.Expressions.ExpressionType.AndAssign" /> and the <see cref="P:System.Linq.Expressions.BinaryExpression.Left" /> and <see cref="P:System.Linq.Expressions.BinaryExpression.Right" /> properties set to the specified values.</returns>
		public static BinaryExpression AndAssign(Expression left, Expression right)
		{
			return Expression.AndAssign(left, right, null, null);
		}

		/// <summary>Creates a <see cref="T:System.Linq.Expressions.BinaryExpression" /> that represents a bitwise AND assignment operation.</summary>
		/// <param name="left">An <see cref="T:System.Linq.Expressions.Expression" /> to set the <see cref="P:System.Linq.Expressions.BinaryExpression.Left" /> property equal to.</param>
		/// <param name="right">An <see cref="T:System.Linq.Expressions.Expression" /> to set the <see cref="P:System.Linq.Expressions.BinaryExpression.Right" /> property equal to.</param>
		/// <param name="method">A <see cref="T:System.Reflection.MethodInfo" /> to set the <see cref="P:System.Linq.Expressions.BinaryExpression.Method" /> property equal to.</param>
		/// <returns>A <see cref="T:System.Linq.Expressions.BinaryExpression" /> that has the <see cref="P:System.Linq.Expressions.Expression.NodeType" /> property equal to <see cref="F:System.Linq.Expressions.ExpressionType.AndAssign" /> and the <see cref="P:System.Linq.Expressions.BinaryExpression.Left" />, <see cref="P:System.Linq.Expressions.BinaryExpression.Right" />, and <see cref="P:System.Linq.Expressions.BinaryExpression.Method" /> properties set to the specified values.</returns>
		public static BinaryExpression AndAssign(Expression left, Expression right, MethodInfo method)
		{
			return Expression.AndAssign(left, right, method, null);
		}

		/// <summary>Creates a <see cref="T:System.Linq.Expressions.BinaryExpression" /> that represents a bitwise AND assignment operation.</summary>
		/// <param name="left">An <see cref="T:System.Linq.Expressions.Expression" /> to set the <see cref="P:System.Linq.Expressions.BinaryExpression.Left" /> property equal to.</param>
		/// <param name="right">An <see cref="T:System.Linq.Expressions.Expression" /> to set the <see cref="P:System.Linq.Expressions.BinaryExpression.Right" /> property equal to.</param>
		/// <param name="method">A <see cref="T:System.Reflection.MethodInfo" /> to set the <see cref="P:System.Linq.Expressions.BinaryExpression.Method" /> property equal to.</param>
		/// <param name="conversion">A <see cref="T:System.Linq.Expressions.LambdaExpression" /> to set the <see cref="P:System.Linq.Expressions.BinaryExpression.Conversion" /> property equal to.</param>
		/// <returns>A <see cref="T:System.Linq.Expressions.BinaryExpression" /> that has the <see cref="P:System.Linq.Expressions.Expression.NodeType" /> property equal to <see cref="F:System.Linq.Expressions.ExpressionType.AndAssign" /> and the <see cref="P:System.Linq.Expressions.BinaryExpression.Left" />, <see cref="P:System.Linq.Expressions.BinaryExpression.Right" />, <see cref="P:System.Linq.Expressions.BinaryExpression.Method" />, and <see cref="P:System.Linq.Expressions.BinaryExpression.Conversion" /> properties set to the specified values.</returns>
		public static BinaryExpression AndAssign(Expression left, Expression right, MethodInfo method, LambdaExpression conversion)
		{
			ExpressionUtils.RequiresCanRead(left, "left");
			Expression.RequiresCanWrite(left, "left");
			ExpressionUtils.RequiresCanRead(right, "right");
			if (!(method == null))
			{
				return Expression.GetMethodBasedAssignOperator(ExpressionType.AndAssign, left, right, method, conversion, true);
			}
			if (!(left.Type == right.Type) || !left.Type.IsIntegerOrBool())
			{
				return Expression.GetUserDefinedAssignOperatorOrThrow(ExpressionType.AndAssign, "op_BitwiseAnd", left, right, conversion, true);
			}
			if (conversion != null)
			{
				throw Error.ConversionIsNotSupportedForArithmeticTypes();
			}
			return new SimpleBinaryExpression(ExpressionType.AndAssign, left, right, left.Type);
		}

		/// <summary>Creates a <see cref="T:System.Linq.Expressions.BinaryExpression" /> that represents a bitwise <see langword="OR" /> operation.</summary>
		/// <param name="left">An <see cref="T:System.Linq.Expressions.Expression" /> to set the <see cref="P:System.Linq.Expressions.BinaryExpression.Left" /> property equal to.</param>
		/// <param name="right">An <see cref="T:System.Linq.Expressions.Expression" /> to set the <see cref="P:System.Linq.Expressions.BinaryExpression.Right" /> property equal to.</param>
		/// <returns>A <see cref="T:System.Linq.Expressions.BinaryExpression" /> that has the <see cref="P:System.Linq.Expressions.Expression.NodeType" /> property equal to <see cref="F:System.Linq.Expressions.ExpressionType.Or" /> and the <see cref="P:System.Linq.Expressions.BinaryExpression.Left" /> and <see cref="P:System.Linq.Expressions.BinaryExpression.Right" /> properties set to the specified values.</returns>
		/// <exception cref="T:System.ArgumentNullException">
		///         <paramref name="left" /> or <paramref name="right" /> is <see langword="null" />.</exception>
		/// <exception cref="T:System.InvalidOperationException">The bitwise <see langword="OR" /> operator is not defined for <paramref name="left" />.Type and <paramref name="right" />.Type.</exception>
		public static BinaryExpression Or(Expression left, Expression right)
		{
			return Expression.Or(left, right, null);
		}

		/// <summary>Creates a <see cref="T:System.Linq.Expressions.BinaryExpression" /> that represents a bitwise <see langword="OR" /> operation.</summary>
		/// <param name="left">An <see cref="T:System.Linq.Expressions.Expression" /> to set the <see cref="P:System.Linq.Expressions.BinaryExpression.Left" /> property equal to.</param>
		/// <param name="right">An <see cref="T:System.Linq.Expressions.Expression" /> to set the <see cref="P:System.Linq.Expressions.BinaryExpression.Right" /> property equal to.</param>
		/// <param name="method">A <see cref="T:System.Reflection.MethodInfo" /> to set the <see cref="P:System.Linq.Expressions.BinaryExpression.Method" /> property equal to.</param>
		/// <returns>A <see cref="T:System.Linq.Expressions.BinaryExpression" /> that has the <see cref="P:System.Linq.Expressions.Expression.NodeType" /> property equal to <see cref="F:System.Linq.Expressions.ExpressionType.Or" /> and the <see cref="P:System.Linq.Expressions.BinaryExpression.Left" />, <see cref="P:System.Linq.Expressions.BinaryExpression.Right" />, and <see cref="P:System.Linq.Expressions.BinaryExpression.Method" /> properties set to the specified values.</returns>
		/// <exception cref="T:System.ArgumentNullException">
		///         <paramref name="left" /> or <paramref name="right" /> is <see langword="null" />.</exception>
		/// <exception cref="T:System.ArgumentException">
		///         <paramref name="method" /> is not <see langword="null" /> and the method it represents returns <see langword="void" />, is not <see langword="static" /> (<see langword="Shared" /> in Visual Basic), or does not take exactly two arguments.</exception>
		/// <exception cref="T:System.InvalidOperationException">
		///         <paramref name="method" /> is <see langword="null" /> and the bitwise <see langword="OR" /> operator is not defined for <paramref name="left" />.Type and <paramref name="right" />.Type.</exception>
		public static BinaryExpression Or(Expression left, Expression right, MethodInfo method)
		{
			ExpressionUtils.RequiresCanRead(left, "left");
			ExpressionUtils.RequiresCanRead(right, "right");
			if (!(method == null))
			{
				return Expression.GetMethodBasedBinaryOperator(ExpressionType.Or, left, right, method, true);
			}
			if (left.Type == right.Type && left.Type.IsIntegerOrBool())
			{
				return new SimpleBinaryExpression(ExpressionType.Or, left, right, left.Type);
			}
			return Expression.GetUserDefinedBinaryOperatorOrThrow(ExpressionType.Or, "op_BitwiseOr", left, right, true);
		}

		/// <summary>Creates a <see cref="T:System.Linq.Expressions.BinaryExpression" /> that represents a bitwise OR assignment operation.</summary>
		/// <param name="left">An <see cref="T:System.Linq.Expressions.Expression" /> to set the <see cref="P:System.Linq.Expressions.BinaryExpression.Left" /> property equal to.</param>
		/// <param name="right">An <see cref="T:System.Linq.Expressions.Expression" /> to set the <see cref="P:System.Linq.Expressions.BinaryExpression.Right" /> property equal to.</param>
		/// <returns>A <see cref="T:System.Linq.Expressions.BinaryExpression" /> that has the <see cref="P:System.Linq.Expressions.Expression.NodeType" /> property equal to <see cref="F:System.Linq.Expressions.ExpressionType.OrAssign" /> and the <see cref="P:System.Linq.Expressions.BinaryExpression.Left" /> and <see cref="P:System.Linq.Expressions.BinaryExpression.Right" /> properties set to the specified values.</returns>
		public static BinaryExpression OrAssign(Expression left, Expression right)
		{
			return Expression.OrAssign(left, right, null, null);
		}

		/// <summary>Creates a <see cref="T:System.Linq.Expressions.BinaryExpression" /> that represents a bitwise OR assignment operation.</summary>
		/// <param name="left">An <see cref="T:System.Linq.Expressions.Expression" /> to set the <see cref="P:System.Linq.Expressions.BinaryExpression.Left" /> property equal to.</param>
		/// <param name="right">An <see cref="T:System.Linq.Expressions.Expression" /> to set the <see cref="P:System.Linq.Expressions.BinaryExpression.Right" /> property equal to.</param>
		/// <param name="method">A <see cref="T:System.Reflection.MethodInfo" /> to set the <see cref="P:System.Linq.Expressions.BinaryExpression.Method" /> property equal to.</param>
		/// <returns>A <see cref="T:System.Linq.Expressions.BinaryExpression" /> that has the <see cref="P:System.Linq.Expressions.Expression.NodeType" /> property equal to <see cref="F:System.Linq.Expressions.ExpressionType.OrAssign" /> and the <see cref="P:System.Linq.Expressions.BinaryExpression.Left" />, <see cref="P:System.Linq.Expressions.BinaryExpression.Right" />, and <see cref="P:System.Linq.Expressions.BinaryExpression.Method" /> properties set to the specified values.</returns>
		public static BinaryExpression OrAssign(Expression left, Expression right, MethodInfo method)
		{
			return Expression.OrAssign(left, right, method, null);
		}

		/// <summary>Creates a <see cref="T:System.Linq.Expressions.BinaryExpression" /> that represents a bitwise OR assignment operation.</summary>
		/// <param name="left">An <see cref="T:System.Linq.Expressions.Expression" /> to set the <see cref="P:System.Linq.Expressions.BinaryExpression.Left" /> property equal to.</param>
		/// <param name="right">An <see cref="T:System.Linq.Expressions.Expression" /> to set the <see cref="P:System.Linq.Expressions.BinaryExpression.Right" /> property equal to.</param>
		/// <param name="method">A <see cref="T:System.Reflection.MethodInfo" /> to set the <see cref="P:System.Linq.Expressions.BinaryExpression.Method" /> property equal to.</param>
		/// <param name="conversion">A <see cref="T:System.Linq.Expressions.LambdaExpression" /> to set the <see cref="P:System.Linq.Expressions.BinaryExpression.Conversion" /> property equal to.</param>
		/// <returns>A <see cref="T:System.Linq.Expressions.BinaryExpression" /> that has the <see cref="P:System.Linq.Expressions.Expression.NodeType" /> property equal to <see cref="F:System.Linq.Expressions.ExpressionType.OrAssign" /> and the <see cref="P:System.Linq.Expressions.BinaryExpression.Left" />, <see cref="P:System.Linq.Expressions.BinaryExpression.Right" />, <see cref="P:System.Linq.Expressions.BinaryExpression.Method" />, and <see cref="P:System.Linq.Expressions.BinaryExpression.Conversion" /> properties set to the specified values.</returns>
		public static BinaryExpression OrAssign(Expression left, Expression right, MethodInfo method, LambdaExpression conversion)
		{
			ExpressionUtils.RequiresCanRead(left, "left");
			Expression.RequiresCanWrite(left, "left");
			ExpressionUtils.RequiresCanRead(right, "right");
			if (!(method == null))
			{
				return Expression.GetMethodBasedAssignOperator(ExpressionType.OrAssign, left, right, method, conversion, true);
			}
			if (!(left.Type == right.Type) || !left.Type.IsIntegerOrBool())
			{
				return Expression.GetUserDefinedAssignOperatorOrThrow(ExpressionType.OrAssign, "op_BitwiseOr", left, right, conversion, true);
			}
			if (conversion != null)
			{
				throw Error.ConversionIsNotSupportedForArithmeticTypes();
			}
			return new SimpleBinaryExpression(ExpressionType.OrAssign, left, right, left.Type);
		}

		/// <summary>Creates a <see cref="T:System.Linq.Expressions.BinaryExpression" /> that represents a bitwise <see langword="XOR" /> operation, using op_ExclusiveOr for user-defined types.</summary>
		/// <param name="left">An <see cref="T:System.Linq.Expressions.Expression" /> to set the <see cref="P:System.Linq.Expressions.BinaryExpression.Left" /> property equal to.</param>
		/// <param name="right">An <see cref="T:System.Linq.Expressions.Expression" /> to set the <see cref="P:System.Linq.Expressions.BinaryExpression.Right" /> property equal to.</param>
		/// <returns>A <see cref="T:System.Linq.Expressions.BinaryExpression" /> that has the <see cref="P:System.Linq.Expressions.Expression.NodeType" /> property equal to <see cref="F:System.Linq.Expressions.ExpressionType.ExclusiveOr" /> and the <see cref="P:System.Linq.Expressions.BinaryExpression.Left" /> and <see cref="P:System.Linq.Expressions.BinaryExpression.Right" /> properties set to the specified values.</returns>
		/// <exception cref="T:System.ArgumentNullException">
		///         <paramref name="left" /> or <paramref name="right" /> is <see langword="null" />.</exception>
		/// <exception cref="T:System.InvalidOperationException">The <see langword="XOR" /> operator is not defined for <paramref name="left" />.Type and <paramref name="right" />.Type.</exception>
		public static BinaryExpression ExclusiveOr(Expression left, Expression right)
		{
			return Expression.ExclusiveOr(left, right, null);
		}

		/// <summary>Creates a <see cref="T:System.Linq.Expressions.BinaryExpression" /> that represents a bitwise <see langword="XOR" /> operation, using op_ExclusiveOr for user-defined types. The implementing method can be specified.</summary>
		/// <param name="left">An <see cref="T:System.Linq.Expressions.Expression" /> to set the <see cref="P:System.Linq.Expressions.BinaryExpression.Left" /> property equal to.</param>
		/// <param name="right">An <see cref="T:System.Linq.Expressions.Expression" /> to set the <see cref="P:System.Linq.Expressions.BinaryExpression.Right" /> property equal to.</param>
		/// <param name="method">A <see cref="T:System.Reflection.MethodInfo" /> to set the <see cref="P:System.Linq.Expressions.BinaryExpression.Method" /> property equal to.</param>
		/// <returns>A <see cref="T:System.Linq.Expressions.BinaryExpression" /> that has the <see cref="P:System.Linq.Expressions.Expression.NodeType" /> property equal to <see cref="F:System.Linq.Expressions.ExpressionType.ExclusiveOr" /> and the <see cref="P:System.Linq.Expressions.BinaryExpression.Left" />, <see cref="P:System.Linq.Expressions.BinaryExpression.Right" />, and <see cref="P:System.Linq.Expressions.BinaryExpression.Method" /> properties set to the specified values.</returns>
		/// <exception cref="T:System.ArgumentNullException">
		///         <paramref name="left" /> or <paramref name="right" /> is <see langword="null" />.</exception>
		/// <exception cref="T:System.ArgumentException">
		///         <paramref name="method" /> is not <see langword="null" /> and the method it represents returns <see langword="void" />, is not <see langword="static" /> (<see langword="Shared" /> in Visual Basic), or does not take exactly two arguments.</exception>
		/// <exception cref="T:System.InvalidOperationException">
		///         <paramref name="method" /> is <see langword="null" /> and the <see langword="XOR" /> operator is not defined for <paramref name="left" />.Type and <paramref name="right" />.Type.</exception>
		public static BinaryExpression ExclusiveOr(Expression left, Expression right, MethodInfo method)
		{
			ExpressionUtils.RequiresCanRead(left, "left");
			ExpressionUtils.RequiresCanRead(right, "right");
			if (!(method == null))
			{
				return Expression.GetMethodBasedBinaryOperator(ExpressionType.ExclusiveOr, left, right, method, true);
			}
			if (left.Type == right.Type && left.Type.IsIntegerOrBool())
			{
				return new SimpleBinaryExpression(ExpressionType.ExclusiveOr, left, right, left.Type);
			}
			return Expression.GetUserDefinedBinaryOperatorOrThrow(ExpressionType.ExclusiveOr, "op_ExclusiveOr", left, right, true);
		}

		/// <summary>Creates a <see cref="T:System.Linq.Expressions.BinaryExpression" /> that represents a bitwise XOR assignment operation, using op_ExclusiveOr for user-defined types.</summary>
		/// <param name="left">An <see cref="T:System.Linq.Expressions.Expression" /> to set the <see cref="P:System.Linq.Expressions.BinaryExpression.Left" /> property equal to.</param>
		/// <param name="right">An <see cref="T:System.Linq.Expressions.Expression" /> to set the <see cref="P:System.Linq.Expressions.BinaryExpression.Right" /> property equal to.</param>
		/// <returns>A <see cref="T:System.Linq.Expressions.BinaryExpression" /> that has the <see cref="P:System.Linq.Expressions.Expression.NodeType" /> property equal to <see cref="F:System.Linq.Expressions.ExpressionType.ExclusiveOrAssign" /> and the <see cref="P:System.Linq.Expressions.BinaryExpression.Left" /> and <see cref="P:System.Linq.Expressions.BinaryExpression.Right" /> properties set to the specified values.</returns>
		public static BinaryExpression ExclusiveOrAssign(Expression left, Expression right)
		{
			return Expression.ExclusiveOrAssign(left, right, null, null);
		}

		/// <summary>Creates a <see cref="T:System.Linq.Expressions.BinaryExpression" /> that represents a bitwise XOR assignment operation, using op_ExclusiveOr for user-defined types.</summary>
		/// <param name="left">An <see cref="T:System.Linq.Expressions.Expression" /> to set the <see cref="P:System.Linq.Expressions.BinaryExpression.Left" /> property equal to.</param>
		/// <param name="right">An <see cref="T:System.Linq.Expressions.Expression" /> to set the <see cref="P:System.Linq.Expressions.BinaryExpression.Right" /> property equal to.</param>
		/// <param name="method">A <see cref="T:System.Reflection.MethodInfo" /> to set the <see cref="P:System.Linq.Expressions.BinaryExpression.Method" /> property equal to.</param>
		/// <returns>A <see cref="T:System.Linq.Expressions.BinaryExpression" /> that has the <see cref="P:System.Linq.Expressions.Expression.NodeType" /> property equal to <see cref="F:System.Linq.Expressions.ExpressionType.ExclusiveOrAssign" /> and the <see cref="P:System.Linq.Expressions.BinaryExpression.Left" />, <see cref="P:System.Linq.Expressions.BinaryExpression.Right" />, and <see cref="P:System.Linq.Expressions.BinaryExpression.Method" /> properties set to the specified values.</returns>
		public static BinaryExpression ExclusiveOrAssign(Expression left, Expression right, MethodInfo method)
		{
			return Expression.ExclusiveOrAssign(left, right, method, null);
		}

		/// <summary>Creates a <see cref="T:System.Linq.Expressions.BinaryExpression" /> that represents a bitwise XOR assignment operation, using op_ExclusiveOr for user-defined types.</summary>
		/// <param name="left">An <see cref="T:System.Linq.Expressions.Expression" /> to set the <see cref="P:System.Linq.Expressions.BinaryExpression.Left" /> property equal to.</param>
		/// <param name="right">An <see cref="T:System.Linq.Expressions.Expression" /> to set the <see cref="P:System.Linq.Expressions.BinaryExpression.Right" /> property equal to.</param>
		/// <param name="method">A <see cref="T:System.Reflection.MethodInfo" /> to set the <see cref="P:System.Linq.Expressions.BinaryExpression.Method" /> property equal to.</param>
		/// <param name="conversion">A <see cref="T:System.Linq.Expressions.LambdaExpression" /> to set the <see cref="P:System.Linq.Expressions.BinaryExpression.Conversion" /> property equal to.</param>
		/// <returns>A <see cref="T:System.Linq.Expressions.BinaryExpression" /> that has the <see cref="P:System.Linq.Expressions.Expression.NodeType" /> property equal to <see cref="F:System.Linq.Expressions.ExpressionType.ExclusiveOrAssign" /> and the <see cref="P:System.Linq.Expressions.BinaryExpression.Left" />, <see cref="P:System.Linq.Expressions.BinaryExpression.Right" />, <see cref="P:System.Linq.Expressions.BinaryExpression.Method" />, and <see cref="P:System.Linq.Expressions.BinaryExpression.Conversion" /> properties set to the specified values.</returns>
		public static BinaryExpression ExclusiveOrAssign(Expression left, Expression right, MethodInfo method, LambdaExpression conversion)
		{
			ExpressionUtils.RequiresCanRead(left, "left");
			Expression.RequiresCanWrite(left, "left");
			ExpressionUtils.RequiresCanRead(right, "right");
			if (!(method == null))
			{
				return Expression.GetMethodBasedAssignOperator(ExpressionType.ExclusiveOrAssign, left, right, method, conversion, true);
			}
			if (!(left.Type == right.Type) || !left.Type.IsIntegerOrBool())
			{
				return Expression.GetUserDefinedAssignOperatorOrThrow(ExpressionType.ExclusiveOrAssign, "op_ExclusiveOr", left, right, conversion, true);
			}
			if (conversion != null)
			{
				throw Error.ConversionIsNotSupportedForArithmeticTypes();
			}
			return new SimpleBinaryExpression(ExpressionType.ExclusiveOrAssign, left, right, left.Type);
		}

		/// <summary>Creates a <see cref="T:System.Linq.Expressions.BinaryExpression" /> that represents raising a number to a power.</summary>
		/// <param name="left">An <see cref="T:System.Linq.Expressions.Expression" /> to set the <see cref="P:System.Linq.Expressions.BinaryExpression.Left" /> property equal to.</param>
		/// <param name="right">An <see cref="T:System.Linq.Expressions.Expression" /> to set the <see cref="P:System.Linq.Expressions.BinaryExpression.Right" /> property equal to.</param>
		/// <returns>A <see cref="T:System.Linq.Expressions.BinaryExpression" /> that has the <see cref="P:System.Linq.Expressions.Expression.NodeType" /> property equal to <see cref="F:System.Linq.Expressions.ExpressionType.Power" /> and the <see cref="P:System.Linq.Expressions.BinaryExpression.Left" /> and <see cref="P:System.Linq.Expressions.BinaryExpression.Right" /> properties set to the specified values.</returns>
		/// <exception cref="T:System.ArgumentNullException">
		///         <paramref name="left" /> or <paramref name="right" /> is <see langword="null" />.</exception>
		/// <exception cref="T:System.InvalidOperationException">The exponentiation operator is not defined for <paramref name="left" />.Type and <paramref name="right" />.Type.-or-
		///         <paramref name="left" />.Type and/or <paramref name="right" />.Type are not <see cref="T:System.Double" />.</exception>
		public static BinaryExpression Power(Expression left, Expression right)
		{
			return Expression.Power(left, right, null);
		}

		/// <summary>Creates a <see cref="T:System.Linq.Expressions.BinaryExpression" /> that represents raising a number to a power.</summary>
		/// <param name="left">An <see cref="T:System.Linq.Expressions.Expression" /> to set the <see cref="P:System.Linq.Expressions.BinaryExpression.Left" /> property equal to.</param>
		/// <param name="right">An <see cref="T:System.Linq.Expressions.Expression" /> to set the <see cref="P:System.Linq.Expressions.BinaryExpression.Right" /> property equal to.</param>
		/// <param name="method">A <see cref="T:System.Reflection.MethodInfo" /> to set the <see cref="P:System.Linq.Expressions.BinaryExpression.Method" /> property equal to.</param>
		/// <returns>A <see cref="T:System.Linq.Expressions.BinaryExpression" /> that has the <see cref="P:System.Linq.Expressions.Expression.NodeType" /> property equal to <see cref="F:System.Linq.Expressions.ExpressionType.Power" /> and the <see cref="P:System.Linq.Expressions.BinaryExpression.Left" />, <see cref="P:System.Linq.Expressions.BinaryExpression.Right" />, and <see cref="P:System.Linq.Expressions.BinaryExpression.Method" /> properties set to the specified values.</returns>
		/// <exception cref="T:System.ArgumentNullException">
		///         <paramref name="left" /> or <paramref name="right" /> is <see langword="null" />.</exception>
		/// <exception cref="T:System.ArgumentException">
		///         <paramref name="method" /> is not <see langword="null" /> and the method it represents returns <see langword="void" />, is not <see langword="static" /> (<see langword="Shared" /> in Visual Basic), or does not take exactly two arguments.</exception>
		/// <exception cref="T:System.InvalidOperationException">
		///         <paramref name="method" /> is <see langword="null" /> and the exponentiation operator is not defined for <paramref name="left" />.Type and <paramref name="right" />.Type.-or-
		///         <paramref name="method" /> is <see langword="null" /> and <paramref name="left" />.Type and/or <paramref name="right" />.Type are not <see cref="T:System.Double" />.</exception>
		public static BinaryExpression Power(Expression left, Expression right, MethodInfo method)
		{
			ExpressionUtils.RequiresCanRead(left, "left");
			ExpressionUtils.RequiresCanRead(right, "right");
			if (method == null)
			{
				if (!(left.Type == right.Type) || !left.Type.IsArithmetic())
				{
					string name = "op_Exponent";
					BinaryExpression userDefinedBinaryOperator = Expression.GetUserDefinedBinaryOperator(ExpressionType.Power, name, left, right, true);
					if (userDefinedBinaryOperator == null)
					{
						name = "op_Exponentiation";
						userDefinedBinaryOperator = Expression.GetUserDefinedBinaryOperator(ExpressionType.Power, name, left, right, true);
						if (userDefinedBinaryOperator == null)
						{
							throw Error.BinaryOperatorNotDefined(ExpressionType.Power, left.Type, right.Type);
						}
					}
					ParameterInfo[] parametersCached = userDefinedBinaryOperator.Method.GetParametersCached();
					Expression.ValidateParamswithOperandsOrThrow(parametersCached[0].ParameterType, left.Type, ExpressionType.Power, name);
					Expression.ValidateParamswithOperandsOrThrow(parametersCached[1].ParameterType, right.Type, ExpressionType.Power, name);
					return userDefinedBinaryOperator;
				}
				method = CachedReflectionInfo.Math_Pow_Double_Double;
			}
			return Expression.GetMethodBasedBinaryOperator(ExpressionType.Power, left, right, method, true);
		}

		/// <summary>Creates a <see cref="T:System.Linq.Expressions.BinaryExpression" /> that represents raising an expression to a power and assigning the result back to the expression.</summary>
		/// <param name="left">An <see cref="T:System.Linq.Expressions.Expression" /> to set the <see cref="P:System.Linq.Expressions.BinaryExpression.Left" /> property equal to.</param>
		/// <param name="right">An <see cref="T:System.Linq.Expressions.Expression" /> to set the <see cref="P:System.Linq.Expressions.BinaryExpression.Right" /> property equal to.</param>
		/// <returns>A <see cref="T:System.Linq.Expressions.BinaryExpression" /> that has the <see cref="P:System.Linq.Expressions.Expression.NodeType" /> property equal to <see cref="F:System.Linq.Expressions.ExpressionType.PowerAssign" /> and the <see cref="P:System.Linq.Expressions.BinaryExpression.Left" /> and <see cref="P:System.Linq.Expressions.BinaryExpression.Right" /> properties set to the specified values.</returns>
		public static BinaryExpression PowerAssign(Expression left, Expression right)
		{
			return Expression.PowerAssign(left, right, null, null);
		}

		/// <summary>Creates a <see cref="T:System.Linq.Expressions.BinaryExpression" /> that represents raising an expression to a power and assigning the result back to the expression.</summary>
		/// <param name="left">An <see cref="T:System.Linq.Expressions.Expression" /> to set the <see cref="P:System.Linq.Expressions.BinaryExpression.Left" /> property equal to.</param>
		/// <param name="right">An <see cref="T:System.Linq.Expressions.Expression" /> to set the <see cref="P:System.Linq.Expressions.BinaryExpression.Right" /> property equal to.</param>
		/// <param name="method">A <see cref="T:System.Reflection.MethodInfo" /> to set the <see cref="P:System.Linq.Expressions.BinaryExpression.Method" /> property equal to.</param>
		/// <returns>A <see cref="T:System.Linq.Expressions.BinaryExpression" /> that has the <see cref="P:System.Linq.Expressions.Expression.NodeType" /> property equal to <see cref="F:System.Linq.Expressions.ExpressionType.PowerAssign" /> and the <see cref="P:System.Linq.Expressions.BinaryExpression.Left" />, <see cref="P:System.Linq.Expressions.BinaryExpression.Right" />, and <see cref="P:System.Linq.Expressions.BinaryExpression.Method" /> properties set to the specified values.</returns>
		public static BinaryExpression PowerAssign(Expression left, Expression right, MethodInfo method)
		{
			return Expression.PowerAssign(left, right, method, null);
		}

		/// <summary>Creates a <see cref="T:System.Linq.Expressions.BinaryExpression" /> that represents raising an expression to a power and assigning the result back to the expression.</summary>
		/// <param name="left">An <see cref="T:System.Linq.Expressions.Expression" /> to set the <see cref="P:System.Linq.Expressions.BinaryExpression.Left" /> property equal to.</param>
		/// <param name="right">An <see cref="T:System.Linq.Expressions.Expression" /> to set the <see cref="P:System.Linq.Expressions.BinaryExpression.Right" /> property equal to.</param>
		/// <param name="method">A <see cref="T:System.Reflection.MethodInfo" /> to set the <see cref="P:System.Linq.Expressions.BinaryExpression.Method" /> property equal to.</param>
		/// <param name="conversion">A <see cref="T:System.Linq.Expressions.LambdaExpression" /> to set the <see cref="P:System.Linq.Expressions.BinaryExpression.Conversion" /> property equal to.</param>
		/// <returns>A <see cref="T:System.Linq.Expressions.BinaryExpression" /> that has the <see cref="P:System.Linq.Expressions.Expression.NodeType" /> property equal to <see cref="F:System.Linq.Expressions.ExpressionType.PowerAssign" /> and the <see cref="P:System.Linq.Expressions.BinaryExpression.Left" />, <see cref="P:System.Linq.Expressions.BinaryExpression.Right" />, <see cref="P:System.Linq.Expressions.BinaryExpression.Method" />, and <see cref="P:System.Linq.Expressions.BinaryExpression.Conversion" /> properties set to the specified values.</returns>
		public static BinaryExpression PowerAssign(Expression left, Expression right, MethodInfo method, LambdaExpression conversion)
		{
			ExpressionUtils.RequiresCanRead(left, "left");
			Expression.RequiresCanWrite(left, "left");
			ExpressionUtils.RequiresCanRead(right, "right");
			if (method == null)
			{
				method = CachedReflectionInfo.Math_Pow_Double_Double;
				if (method == null)
				{
					throw Error.BinaryOperatorNotDefined(ExpressionType.PowerAssign, left.Type, right.Type);
				}
			}
			return Expression.GetMethodBasedAssignOperator(ExpressionType.PowerAssign, left, right, method, conversion, true);
		}

		/// <summary>Creates a <see cref="T:System.Linq.Expressions.BinaryExpression" /> that represents applying an array index operator to an array of rank one.</summary>
		/// <param name="array">A <see cref="T:System.Linq.Expressions.Expression" /> to set the <see cref="P:System.Linq.Expressions.BinaryExpression.Left" /> property equal to.</param>
		/// <param name="index">A <see cref="T:System.Linq.Expressions.Expression" /> to set the <see cref="P:System.Linq.Expressions.BinaryExpression.Right" /> property equal to.</param>
		/// <returns>A <see cref="T:System.Linq.Expressions.BinaryExpression" /> that has the <see cref="P:System.Linq.Expressions.Expression.NodeType" /> property equal to <see cref="F:System.Linq.Expressions.ExpressionType.ArrayIndex" /> and the <see cref="P:System.Linq.Expressions.BinaryExpression.Left" /> and <see cref="P:System.Linq.Expressions.BinaryExpression.Right" /> properties set to the specified values.</returns>
		/// <exception cref="T:System.ArgumentNullException">
		///         <paramref name="array" /> or <paramref name="index" /> is <see langword="null" />.</exception>
		/// <exception cref="T:System.ArgumentException">
		///         <paramref name="array" />.Type does not represent an array type.-or-
		///         <paramref name="array" />.Type represents an array type whose rank is not 1.-or-
		///         <paramref name="index" />.Type does not represent the <see cref="T:System.Int32" /> type.</exception>
		public static BinaryExpression ArrayIndex(Expression array, Expression index)
		{
			ExpressionUtils.RequiresCanRead(array, "array");
			ExpressionUtils.RequiresCanRead(index, "index");
			if (index.Type != typeof(int))
			{
				throw Error.ArgumentMustBeArrayIndexType("index");
			}
			Type type = array.Type;
			if (!type.IsArray)
			{
				throw Error.ArgumentMustBeArray("array");
			}
			if (type.GetArrayRank() != 1)
			{
				throw Error.IncorrectNumberOfIndexes();
			}
			return new SimpleBinaryExpression(ExpressionType.ArrayIndex, array, index, type.GetElementType());
		}

		/// <summary>Creates a <see cref="T:System.Linq.Expressions.BlockExpression" /> that contains two expressions and has no variables.</summary>
		/// <param name="arg0">The first expression in the block.</param>
		/// <param name="arg1">The second expression in the block.</param>
		/// <returns>The created <see cref="T:System.Linq.Expressions.BlockExpression" />.</returns>
		public static BlockExpression Block(Expression arg0, Expression arg1)
		{
			ExpressionUtils.RequiresCanRead(arg0, "arg0");
			ExpressionUtils.RequiresCanRead(arg1, "arg1");
			return new Block2(arg0, arg1);
		}

		/// <summary>Creates a <see cref="T:System.Linq.Expressions.BlockExpression" /> that contains three expressions and has no variables.</summary>
		/// <param name="arg0">The first expression in the block.</param>
		/// <param name="arg1">The second expression in the block.</param>
		/// <param name="arg2">The third expression in the block.</param>
		/// <returns>The created <see cref="T:System.Linq.Expressions.BlockExpression" />.</returns>
		public static BlockExpression Block(Expression arg0, Expression arg1, Expression arg2)
		{
			ExpressionUtils.RequiresCanRead(arg0, "arg0");
			ExpressionUtils.RequiresCanRead(arg1, "arg1");
			ExpressionUtils.RequiresCanRead(arg2, "arg2");
			return new Block3(arg0, arg1, arg2);
		}

		/// <summary>Creates a <see cref="T:System.Linq.Expressions.BlockExpression" /> that contains four expressions and has no variables.</summary>
		/// <param name="arg0">The first expression in the block.</param>
		/// <param name="arg1">The second expression in the block.</param>
		/// <param name="arg2">The third expression in the block.</param>
		/// <param name="arg3">The fourth expression in the block.</param>
		/// <returns>The created <see cref="T:System.Linq.Expressions.BlockExpression" />.</returns>
		public static BlockExpression Block(Expression arg0, Expression arg1, Expression arg2, Expression arg3)
		{
			ExpressionUtils.RequiresCanRead(arg0, "arg0");
			ExpressionUtils.RequiresCanRead(arg1, "arg1");
			ExpressionUtils.RequiresCanRead(arg2, "arg2");
			ExpressionUtils.RequiresCanRead(arg3, "arg3");
			return new Block4(arg0, arg1, arg2, arg3);
		}

		/// <summary>Creates a <see cref="T:System.Linq.Expressions.BlockExpression" /> that contains five expressions and has no variables.</summary>
		/// <param name="arg0">The first expression in the block.</param>
		/// <param name="arg1">The second expression in the block.</param>
		/// <param name="arg2">The third expression in the block.</param>
		/// <param name="arg3">The fourth expression in the block.</param>
		/// <param name="arg4">The fifth expression in the block.</param>
		/// <returns>The created <see cref="T:System.Linq.Expressions.BlockExpression" />.</returns>
		public static BlockExpression Block(Expression arg0, Expression arg1, Expression arg2, Expression arg3, Expression arg4)
		{
			ExpressionUtils.RequiresCanRead(arg0, "arg0");
			ExpressionUtils.RequiresCanRead(arg1, "arg1");
			ExpressionUtils.RequiresCanRead(arg2, "arg2");
			ExpressionUtils.RequiresCanRead(arg3, "arg3");
			ExpressionUtils.RequiresCanRead(arg4, "arg4");
			return new Block5(arg0, arg1, arg2, arg3, arg4);
		}

		/// <summary>Creates a <see cref="T:System.Linq.Expressions.BlockExpression" /> that contains the given expressions and has no variables.</summary>
		/// <param name="expressions">The expressions in the block.</param>
		/// <returns>The created <see cref="T:System.Linq.Expressions.BlockExpression" />.</returns>
		public static BlockExpression Block(params Expression[] expressions)
		{
			ContractUtils.RequiresNotNull(expressions, "expressions");
			Expression.RequiresCanRead(expressions, "expressions");
			return Expression.GetOptimizedBlockExpression(expressions);
		}

		/// <summary>Creates a <see cref="T:System.Linq.Expressions.BlockExpression" /> that contains the given expressions and has no variables.</summary>
		/// <param name="expressions">The expressions in the block.</param>
		/// <returns>The created <see cref="T:System.Linq.Expressions.BlockExpression" />.</returns>
		public static BlockExpression Block(IEnumerable<Expression> expressions)
		{
			return Expression.Block(EmptyReadOnlyCollection<ParameterExpression>.Instance, expressions);
		}

		/// <summary>Creates a <see cref="T:System.Linq.Expressions.BlockExpression" /> that contains the given expressions, has no variables and has specific result type.</summary>
		/// <param name="type">The result type of the block.</param>
		/// <param name="expressions">The expressions in the block.</param>
		/// <returns>The created <see cref="T:System.Linq.Expressions.BlockExpression" />.</returns>
		public static BlockExpression Block(Type type, params Expression[] expressions)
		{
			ContractUtils.RequiresNotNull(expressions, "expressions");
			return Expression.Block(type, expressions);
		}

		/// <summary>Creates a <see cref="T:System.Linq.Expressions.BlockExpression" /> that contains the given expressions, has no variables and has specific result type.</summary>
		/// <param name="type">The result type of the block.</param>
		/// <param name="expressions">The expressions in the block.</param>
		/// <returns>The created <see cref="T:System.Linq.Expressions.BlockExpression" />.</returns>
		public static BlockExpression Block(Type type, IEnumerable<Expression> expressions)
		{
			return Expression.Block(type, EmptyReadOnlyCollection<ParameterExpression>.Instance, expressions);
		}

		/// <summary>Creates a <see cref="T:System.Linq.Expressions.BlockExpression" /> that contains the given variables and expressions.</summary>
		/// <param name="variables">The variables in the block.</param>
		/// <param name="expressions">The expressions in the block.</param>
		/// <returns>The created <see cref="T:System.Linq.Expressions.BlockExpression" />.</returns>
		public static BlockExpression Block(IEnumerable<ParameterExpression> variables, params Expression[] expressions)
		{
			return Expression.Block(variables, expressions);
		}

		/// <summary>Creates a <see cref="T:System.Linq.Expressions.BlockExpression" /> that contains the given variables and expressions.</summary>
		/// <param name="type">The result type of the block.</param>
		/// <param name="variables">The variables in the block.</param>
		/// <param name="expressions">The expressions in the block.</param>
		/// <returns>The created <see cref="T:System.Linq.Expressions.BlockExpression" />.</returns>
		public static BlockExpression Block(Type type, IEnumerable<ParameterExpression> variables, params Expression[] expressions)
		{
			return Expression.Block(type, variables, expressions);
		}

		/// <summary>Creates a <see cref="T:System.Linq.Expressions.BlockExpression" /> that contains the given variables and expressions.</summary>
		/// <param name="variables">The variables in the block.</param>
		/// <param name="expressions">The expressions in the block.</param>
		/// <returns>The created <see cref="T:System.Linq.Expressions.BlockExpression" />.</returns>
		public static BlockExpression Block(IEnumerable<ParameterExpression> variables, IEnumerable<Expression> expressions)
		{
			ContractUtils.RequiresNotNull(expressions, "expressions");
			ReadOnlyCollection<ParameterExpression> readOnlyCollection = variables.ToReadOnly<ParameterExpression>();
			if (readOnlyCollection.Count == 0)
			{
				IReadOnlyList<Expression> readOnlyList = (expressions as IReadOnlyList<Expression>) ?? expressions.ToReadOnly<Expression>();
				Expression.RequiresCanRead(readOnlyList, "expressions");
				return Expression.GetOptimizedBlockExpression(readOnlyList);
			}
			ReadOnlyCollection<Expression> readOnlyCollection2 = expressions.ToReadOnly<Expression>();
			Expression.RequiresCanRead(readOnlyCollection2, "expressions");
			return Expression.BlockCore(null, readOnlyCollection, readOnlyCollection2);
		}

		/// <summary>Creates a <see cref="T:System.Linq.Expressions.BlockExpression" /> that contains the given variables and expressions.</summary>
		/// <param name="type">The result type of the block.</param>
		/// <param name="variables">The variables in the block.</param>
		/// <param name="expressions">The expressions in the block.</param>
		/// <returns>The created <see cref="T:System.Linq.Expressions.BlockExpression" />.</returns>
		public static BlockExpression Block(Type type, IEnumerable<ParameterExpression> variables, IEnumerable<Expression> expressions)
		{
			ContractUtils.RequiresNotNull(type, "type");
			ContractUtils.RequiresNotNull(expressions, "expressions");
			ReadOnlyCollection<Expression> readOnlyCollection = expressions.ToReadOnly<Expression>();
			Expression.RequiresCanRead(readOnlyCollection, "expressions");
			ReadOnlyCollection<ParameterExpression> readOnlyCollection2 = variables.ToReadOnly<ParameterExpression>();
			if (readOnlyCollection2.Count == 0 && readOnlyCollection.Count != 0)
			{
				int count = readOnlyCollection.Count;
				if (count != 0 && readOnlyCollection[count - 1].Type == type)
				{
					return Expression.GetOptimizedBlockExpression(readOnlyCollection);
				}
			}
			return Expression.BlockCore(type, readOnlyCollection2, readOnlyCollection);
		}

		private static BlockExpression BlockCore(Type type, ReadOnlyCollection<ParameterExpression> variables, ReadOnlyCollection<Expression> expressions)
		{
			Expression.ValidateVariables(variables, "variables");
			if (type != null)
			{
				if (expressions.Count == 0)
				{
					if (type != typeof(void))
					{
						throw Error.ArgumentTypesMustMatch();
					}
					return new ScopeWithType(variables, expressions, type);
				}
				else
				{
					Expression expression = expressions.Last<Expression>();
					if (type != typeof(void) && !TypeUtils.AreReferenceAssignable(type, expression.Type))
					{
						throw Error.ArgumentTypesMustMatch();
					}
					if (!TypeUtils.AreEquivalent(type, expression.Type))
					{
						return new ScopeWithType(variables, expressions, type);
					}
				}
			}
			int count = expressions.Count;
			if (count == 0)
			{
				return new ScopeWithType(variables, expressions, typeof(void));
			}
			if (count != 1)
			{
				return new ScopeN(variables, expressions);
			}
			return new Scope1(variables, expressions[0]);
		}

		internal static void ValidateVariables(ReadOnlyCollection<ParameterExpression> varList, string collectionName)
		{
			int count = varList.Count;
			if (count != 0)
			{
				HashSet<ParameterExpression> hashSet = new HashSet<ParameterExpression>();
				for (int i = 0; i < count; i++)
				{
					ParameterExpression parameterExpression = varList[i];
					ContractUtils.RequiresNotNull(parameterExpression, collectionName, i);
					if (parameterExpression.IsByRef)
					{
						throw Error.VariableMustNotBeByRef(parameterExpression, parameterExpression.Type, collectionName, i);
					}
					if (!hashSet.Add(parameterExpression))
					{
						throw Error.DuplicateVariable(parameterExpression, collectionName, i);
					}
				}
			}
		}

		private static BlockExpression GetOptimizedBlockExpression(IReadOnlyList<Expression> expressions)
		{
			switch (expressions.Count)
			{
			case 0:
				return Expression.BlockCore(typeof(void), EmptyReadOnlyCollection<ParameterExpression>.Instance, EmptyReadOnlyCollection<Expression>.Instance);
			case 2:
				return new Block2(expressions[0], expressions[1]);
			case 3:
				return new Block3(expressions[0], expressions[1], expressions[2]);
			case 4:
				return new Block4(expressions[0], expressions[1], expressions[2], expressions[3]);
			case 5:
				return new Block5(expressions[0], expressions[1], expressions[2], expressions[3], expressions[4]);
			}
			IReadOnlyList<Expression> readOnlyList = expressions as ReadOnlyCollection<Expression>;
			return new BlockN(readOnlyList ?? expressions.ToArray<Expression>());
		}

		/// <summary>Creates a <see cref="T:System.Linq.Expressions.CatchBlock" /> representing a catch statement.</summary>
		/// <param name="type">The <see cref="P:System.Linq.Expressions.Expression.Type" /> of <see cref="T:System.Exception" /> this <see cref="T:System.Linq.Expressions.CatchBlock" /> will handle.</param>
		/// <param name="body">The body of the catch statement.</param>
		/// <returns>The created <see cref="T:System.Linq.Expressions.CatchBlock" />.</returns>
		public static CatchBlock Catch(Type type, Expression body)
		{
			return Expression.MakeCatchBlock(type, null, body, null);
		}

		/// <summary>Creates a <see cref="T:System.Linq.Expressions.CatchBlock" /> representing a catch statement with a reference to the caught <see cref="T:System.Exception" /> object for use in the handler body.</summary>
		/// <param name="variable">A <see cref="T:System.Linq.Expressions.ParameterExpression" /> representing a reference to the <see cref="T:System.Exception" /> object caught by this handler.</param>
		/// <param name="body">The body of the catch statement.</param>
		/// <returns>The created <see cref="T:System.Linq.Expressions.CatchBlock" />.</returns>
		public static CatchBlock Catch(ParameterExpression variable, Expression body)
		{
			ContractUtils.RequiresNotNull(variable, "variable");
			return Expression.MakeCatchBlock(variable.Type, variable, body, null);
		}

		/// <summary>Creates a <see cref="T:System.Linq.Expressions.CatchBlock" /> representing a catch statement with an <see cref="T:System.Exception" /> filter but no reference to the caught <see cref="T:System.Exception" /> object.</summary>
		/// <param name="type">The <see cref="P:System.Linq.Expressions.Expression.Type" /> of <see cref="T:System.Exception" /> this <see cref="T:System.Linq.Expressions.CatchBlock" /> will handle.</param>
		/// <param name="body">The body of the catch statement.</param>
		/// <param name="filter">The body of the <see cref="T:System.Exception" /> filter.</param>
		/// <returns>The created <see cref="T:System.Linq.Expressions.CatchBlock" />.</returns>
		public static CatchBlock Catch(Type type, Expression body, Expression filter)
		{
			return Expression.MakeCatchBlock(type, null, body, filter);
		}

		/// <summary>Creates a <see cref="T:System.Linq.Expressions.CatchBlock" /> representing a catch statement with an <see cref="T:System.Exception" /> filter and a reference to the caught <see cref="T:System.Exception" /> object.</summary>
		/// <param name="variable">A <see cref="T:System.Linq.Expressions.ParameterExpression" /> representing a reference to the <see cref="T:System.Exception" /> object caught by this handler.</param>
		/// <param name="body">The body of the catch statement.</param>
		/// <param name="filter">The body of the <see cref="T:System.Exception" /> filter.</param>
		/// <returns>The created <see cref="T:System.Linq.Expressions.CatchBlock" />.</returns>
		public static CatchBlock Catch(ParameterExpression variable, Expression body, Expression filter)
		{
			ContractUtils.RequiresNotNull(variable, "variable");
			return Expression.MakeCatchBlock(variable.Type, variable, body, filter);
		}

		/// <summary>Creates a <see cref="T:System.Linq.Expressions.CatchBlock" /> representing a catch statement with the specified elements.</summary>
		/// <param name="type">The <see cref="P:System.Linq.Expressions.Expression.Type" /> of <see cref="T:System.Exception" /> this <see cref="T:System.Linq.Expressions.CatchBlock" /> will handle.</param>
		/// <param name="variable">A <see cref="T:System.Linq.Expressions.ParameterExpression" /> representing a reference to the <see cref="T:System.Exception" /> object caught by this handler.</param>
		/// <param name="body">The body of the catch statement.</param>
		/// <param name="filter">The body of the <see cref="T:System.Exception" /> filter.</param>
		/// <returns>The created <see cref="T:System.Linq.Expressions.CatchBlock" />.</returns>
		public static CatchBlock MakeCatchBlock(Type type, ParameterExpression variable, Expression body, Expression filter)
		{
			ContractUtils.RequiresNotNull(type, "type");
			ContractUtils.Requires(variable == null || TypeUtils.AreEquivalent(variable.Type, type), "variable");
			if (variable == null)
			{
				TypeUtils.ValidateType(type, "type");
			}
			else if (variable.IsByRef)
			{
				throw Error.VariableMustNotBeByRef(variable, variable.Type, "variable");
			}
			ExpressionUtils.RequiresCanRead(body, "body");
			if (filter != null)
			{
				ExpressionUtils.RequiresCanRead(filter, "filter");
				if (filter.Type != typeof(bool))
				{
					throw Error.ArgumentMustBeBoolean("filter");
				}
			}
			return new CatchBlock(type, variable, body, filter);
		}

		/// <summary>Creates a <see cref="T:System.Linq.Expressions.ConditionalExpression" /> that represents a conditional statement.</summary>
		/// <param name="test">An <see cref="T:System.Linq.Expressions.Expression" /> to set the <see cref="P:System.Linq.Expressions.ConditionalExpression.Test" /> property equal to.</param>
		/// <param name="ifTrue">An <see cref="T:System.Linq.Expressions.Expression" /> to set the <see cref="P:System.Linq.Expressions.ConditionalExpression.IfTrue" /> property equal to.</param>
		/// <param name="ifFalse">An <see cref="T:System.Linq.Expressions.Expression" /> to set the <see cref="P:System.Linq.Expressions.ConditionalExpression.IfFalse" /> property equal to.</param>
		/// <returns>A <see cref="T:System.Linq.Expressions.ConditionalExpression" /> that has the <see cref="P:System.Linq.Expressions.Expression.NodeType" /> property equal to <see cref="F:System.Linq.Expressions.ExpressionType.Conditional" /> and the <see cref="P:System.Linq.Expressions.ConditionalExpression.Test" />, <see cref="P:System.Linq.Expressions.ConditionalExpression.IfTrue" />, and <see cref="P:System.Linq.Expressions.ConditionalExpression.IfFalse" /> properties set to the specified values.</returns>
		/// <exception cref="T:System.ArgumentNullException">
		///         <paramref name="test" /> or <paramref name="ifTrue" /> or <paramref name="ifFalse" /> is <see langword="null" />.</exception>
		/// <exception cref="T:System.ArgumentException">
		///         <paramref name="test" />.Type is not <see cref="T:System.Boolean" />.-or-
		///         <paramref name="ifTrue" />.Type is not equal to <paramref name="ifFalse" />.Type.</exception>
		public static ConditionalExpression Condition(Expression test, Expression ifTrue, Expression ifFalse)
		{
			ExpressionUtils.RequiresCanRead(test, "test");
			ExpressionUtils.RequiresCanRead(ifTrue, "ifTrue");
			ExpressionUtils.RequiresCanRead(ifFalse, "ifFalse");
			if (test.Type != typeof(bool))
			{
				throw Error.ArgumentMustBeBoolean("test");
			}
			if (!TypeUtils.AreEquivalent(ifTrue.Type, ifFalse.Type))
			{
				throw Error.ArgumentTypesMustMatch();
			}
			return ConditionalExpression.Make(test, ifTrue, ifFalse, ifTrue.Type);
		}

		/// <summary>Creates a <see cref="T:System.Linq.Expressions.ConditionalExpression" /> that represents a conditional statement.</summary>
		/// <param name="test">An <see cref="T:System.Linq.Expressions.Expression" /> to set the <see cref="P:System.Linq.Expressions.ConditionalExpression.Test" /> property equal to.</param>
		/// <param name="ifTrue">An <see cref="T:System.Linq.Expressions.Expression" /> to set the <see cref="P:System.Linq.Expressions.ConditionalExpression.IfTrue" /> property equal to.</param>
		/// <param name="ifFalse">An <see cref="T:System.Linq.Expressions.Expression" /> to set the <see cref="P:System.Linq.Expressions.ConditionalExpression.IfFalse" /> property equal to.</param>
		/// <param name="type">A <see cref="P:System.Linq.Expressions.Expression.Type" /> to set the <see cref="P:System.Linq.Expressions.Expression.Type" /> property equal to.</param>
		/// <returns>A <see cref="T:System.Linq.Expressions.ConditionalExpression" /> that has the <see cref="P:System.Linq.Expressions.Expression.NodeType" /> property equal to <see cref="F:System.Linq.Expressions.ExpressionType.Conditional" /> and the <see cref="P:System.Linq.Expressions.ConditionalExpression.Test" />, <see cref="P:System.Linq.Expressions.ConditionalExpression.IfTrue" />, and <see cref="P:System.Linq.Expressions.ConditionalExpression.IfFalse" /> properties set to the specified values.</returns>
		public static ConditionalExpression Condition(Expression test, Expression ifTrue, Expression ifFalse, Type type)
		{
			ExpressionUtils.RequiresCanRead(test, "test");
			ExpressionUtils.RequiresCanRead(ifTrue, "ifTrue");
			ExpressionUtils.RequiresCanRead(ifFalse, "ifFalse");
			ContractUtils.RequiresNotNull(type, "type");
			if (test.Type != typeof(bool))
			{
				throw Error.ArgumentMustBeBoolean("test");
			}
			if (type != typeof(void) && (!TypeUtils.AreReferenceAssignable(type, ifTrue.Type) || !TypeUtils.AreReferenceAssignable(type, ifFalse.Type)))
			{
				throw Error.ArgumentTypesMustMatch();
			}
			return ConditionalExpression.Make(test, ifTrue, ifFalse, type);
		}

		/// <summary>Creates a <see cref="T:System.Linq.Expressions.ConditionalExpression" /> that represents a conditional block with an <see langword="if" /> statement.</summary>
		/// <param name="test">An <see cref="T:System.Linq.Expressions.Expression" /> to set the <see cref="P:System.Linq.Expressions.ConditionalExpression.Test" /> property equal to.</param>
		/// <param name="ifTrue">An <see cref="T:System.Linq.Expressions.Expression" /> to set the <see cref="P:System.Linq.Expressions.ConditionalExpression.IfTrue" /> property equal to.</param>
		/// <returns>A <see cref="T:System.Linq.Expressions.ConditionalExpression" /> that has the <see cref="P:System.Linq.Expressions.Expression.NodeType" /> property equal to <see cref="F:System.Linq.Expressions.ExpressionType.Conditional" /> and the <see cref="P:System.Linq.Expressions.ConditionalExpression.Test" />, <see cref="P:System.Linq.Expressions.ConditionalExpression.IfTrue" />, properties set to the specified values. The <see cref="P:System.Linq.Expressions.ConditionalExpression.IfFalse" /> property is set to default expression and the type of the resulting <see cref="T:System.Linq.Expressions.ConditionalExpression" /> returned by this method is <see cref="T:System.Void" />.</returns>
		public static ConditionalExpression IfThen(Expression test, Expression ifTrue)
		{
			return Expression.Condition(test, ifTrue, Expression.Empty(), typeof(void));
		}

		/// <summary>Creates a <see cref="T:System.Linq.Expressions.ConditionalExpression" /> that represents a conditional block with <see langword="if" /> and <see langword="else" /> statements.</summary>
		/// <param name="test">An <see cref="T:System.Linq.Expressions.Expression" /> to set the <see cref="P:System.Linq.Expressions.ConditionalExpression.Test" /> property equal to.</param>
		/// <param name="ifTrue">An <see cref="T:System.Linq.Expressions.Expression" /> to set the <see cref="P:System.Linq.Expressions.ConditionalExpression.IfTrue" /> property equal to.</param>
		/// <param name="ifFalse">An <see cref="T:System.Linq.Expressions.Expression" /> to set the <see cref="P:System.Linq.Expressions.ConditionalExpression.IfFalse" /> property equal to.</param>
		/// <returns>A <see cref="T:System.Linq.Expressions.ConditionalExpression" /> that has the <see cref="P:System.Linq.Expressions.Expression.NodeType" /> property equal to <see cref="F:System.Linq.Expressions.ExpressionType.Conditional" /> and the <see cref="P:System.Linq.Expressions.ConditionalExpression.Test" />, <see cref="P:System.Linq.Expressions.ConditionalExpression.IfTrue" />, and <see cref="P:System.Linq.Expressions.ConditionalExpression.IfFalse" /> properties set to the specified values. The type of the resulting <see cref="T:System.Linq.Expressions.ConditionalExpression" /> returned by this method is <see cref="T:System.Void" />.</returns>
		public static ConditionalExpression IfThenElse(Expression test, Expression ifTrue, Expression ifFalse)
		{
			return Expression.Condition(test, ifTrue, ifFalse, typeof(void));
		}

		/// <summary>Creates a <see cref="T:System.Linq.Expressions.ConstantExpression" /> that has the <see cref="P:System.Linq.Expressions.ConstantExpression.Value" /> property set to the specified value.</summary>
		/// <param name="value">An <see cref="T:System.Object" /> to set the <see cref="P:System.Linq.Expressions.ConstantExpression.Value" /> property equal to.</param>
		/// <returns>A <see cref="T:System.Linq.Expressions.ConstantExpression" /> that has the <see cref="P:System.Linq.Expressions.Expression.NodeType" /> property equal to <see cref="F:System.Linq.Expressions.ExpressionType.Constant" /> and the <see cref="P:System.Linq.Expressions.ConstantExpression.Value" /> property set to the specified value.</returns>
		public static ConstantExpression Constant(object value)
		{
			return new ConstantExpression(value);
		}

		/// <summary>Creates a <see cref="T:System.Linq.Expressions.ConstantExpression" /> that has the <see cref="P:System.Linq.Expressions.ConstantExpression.Value" /> and <see cref="P:System.Linq.Expressions.Expression.Type" /> properties set to the specified values.</summary>
		/// <param name="value">An <see cref="T:System.Object" /> to set the <see cref="P:System.Linq.Expressions.ConstantExpression.Value" /> property equal to.</param>
		/// <param name="type">A <see cref="T:System.Type" /> to set the <see cref="P:System.Linq.Expressions.Expression.Type" /> property equal to.</param>
		/// <returns>A <see cref="T:System.Linq.Expressions.ConstantExpression" /> that has the <see cref="P:System.Linq.Expressions.Expression.NodeType" /> property equal to <see cref="F:System.Linq.Expressions.ExpressionType.Constant" /> and the <see cref="P:System.Linq.Expressions.ConstantExpression.Value" /> and <see cref="P:System.Linq.Expressions.Expression.Type" /> properties set to the specified values.</returns>
		/// <exception cref="T:System.ArgumentNullException">
		///         <paramref name="type" /> is <see langword="null" />.</exception>
		/// <exception cref="T:System.ArgumentException">
		///         <paramref name="value" /> is not <see langword="null" /> and <paramref name="type" /> is not assignable from the dynamic type of <paramref name="value" />.</exception>
		public static ConstantExpression Constant(object value, Type type)
		{
			ContractUtils.RequiresNotNull(type, "type");
			TypeUtils.ValidateType(type, "type");
			if (value == null)
			{
				if (type == typeof(object))
				{
					return new ConstantExpression(null);
				}
				if (!type.IsValueType || type.IsNullableType())
				{
					return new TypedConstantExpression(null, type);
				}
			}
			else
			{
				Type type2 = value.GetType();
				if (type == type2)
				{
					return new ConstantExpression(value);
				}
				if (type.IsAssignableFrom(type2))
				{
					return new TypedConstantExpression(value, type);
				}
			}
			throw Error.ArgumentTypesMustMatch();
		}

		/// <summary>Creates a <see cref="T:System.Linq.Expressions.DebugInfoExpression" /> with the specified span.</summary>
		/// <param name="document">The <see cref="T:System.Linq.Expressions.SymbolDocumentInfo" /> that represents the source file.</param>
		/// <param name="startLine">The start line of this <see cref="T:System.Linq.Expressions.DebugInfoExpression" />. Must be greater than 0.</param>
		/// <param name="startColumn">The start column of this <see cref="T:System.Linq.Expressions.DebugInfoExpression" />. Must be greater than 0.</param>
		/// <param name="endLine">The end line of this <see cref="T:System.Linq.Expressions.DebugInfoExpression" />. Must be greater or equal than the start line.</param>
		/// <param name="endColumn">The end column of this <see cref="T:System.Linq.Expressions.DebugInfoExpression" />. If the end line is the same as the start line, it must be greater or equal than the start column. In any case, must be greater than 0.</param>
		/// <returns>An instance of <see cref="T:System.Linq.Expressions.DebugInfoExpression" />.</returns>
		public static DebugInfoExpression DebugInfo(SymbolDocumentInfo document, int startLine, int startColumn, int endLine, int endColumn)
		{
			ContractUtils.RequiresNotNull(document, "document");
			if (startLine == 16707566 && startColumn == 0 && endLine == 16707566 && endColumn == 0)
			{
				return new ClearDebugInfoExpression(document);
			}
			Expression.ValidateSpan(startLine, startColumn, endLine, endColumn);
			return new SpanDebugInfoExpression(document, startLine, startColumn, endLine, endColumn);
		}

		/// <summary>Creates a <see cref="T:System.Linq.Expressions.DebugInfoExpression" /> for clearing a sequence point.</summary>
		/// <param name="document">The <see cref="T:System.Linq.Expressions.SymbolDocumentInfo" /> that represents the source file.</param>
		/// <returns>An instance of <see cref="T:System.Linq.Expressions.DebugInfoExpression" /> for clearning a sequence point.</returns>
		public static DebugInfoExpression ClearDebugInfo(SymbolDocumentInfo document)
		{
			ContractUtils.RequiresNotNull(document, "document");
			return new ClearDebugInfoExpression(document);
		}

		private static void ValidateSpan(int startLine, int startColumn, int endLine, int endColumn)
		{
			if (startLine < 1)
			{
				throw Error.OutOfRange("startLine", 1);
			}
			if (startColumn < 1)
			{
				throw Error.OutOfRange("startColumn", 1);
			}
			if (endLine < 1)
			{
				throw Error.OutOfRange("endLine", 1);
			}
			if (endColumn < 1)
			{
				throw Error.OutOfRange("endColumn", 1);
			}
			if (startLine > endLine)
			{
				throw Error.StartEndMustBeOrdered();
			}
			if (startLine == endLine && startColumn > endColumn)
			{
				throw Error.StartEndMustBeOrdered();
			}
		}

		/// <summary>Creates an empty expression that has <see cref="T:System.Void" /> type.</summary>
		/// <returns>A <see cref="T:System.Linq.Expressions.DefaultExpression" /> that has the <see cref="P:System.Linq.Expressions.Expression.NodeType" /> property equal to <see cref="F:System.Linq.Expressions.ExpressionType.Default" /> and the <see cref="P:System.Linq.Expressions.Expression.Type" /> property set to <see cref="T:System.Void" />.</returns>
		public static DefaultExpression Empty()
		{
			return new DefaultExpression(typeof(void));
		}

		/// <summary>Creates a <see cref="T:System.Linq.Expressions.DefaultExpression" /> that has the <see cref="P:System.Linq.Expressions.Expression.Type" /> property set to the specified type.</summary>
		/// <param name="type">A <see cref="T:System.Type" /> to set the <see cref="P:System.Linq.Expressions.Expression.Type" /> property equal to.</param>
		/// <returns>A <see cref="T:System.Linq.Expressions.DefaultExpression" /> that has the <see cref="P:System.Linq.Expressions.Expression.NodeType" /> property equal to <see cref="F:System.Linq.Expressions.ExpressionType.Default" /> and the <see cref="P:System.Linq.Expressions.Expression.Type" /> property set to the specified type.</returns>
		public static DefaultExpression Default(Type type)
		{
			ContractUtils.RequiresNotNull(type, "type");
			TypeUtils.ValidateType(type, "type");
			return new DefaultExpression(type);
		}

		/// <summary>Creates an <see cref="T:System.Linq.Expressions.ElementInit" />, given an array of values as the second argument.</summary>
		/// <param name="addMethod">A <see cref="T:System.Reflection.MethodInfo" /> to set the <see cref="P:System.Linq.Expressions.ElementInit.AddMethod" /> property equal to.</param>
		/// <param name="arguments">An array of <see cref="T:System.Linq.Expressions.Expression" /> objects to set the <see cref="P:System.Linq.Expressions.ElementInit.Arguments" /> property equal to.</param>
		/// <returns>An <see cref="T:System.Linq.Expressions.ElementInit" /> that has the <see cref="P:System.Linq.Expressions.ElementInit.AddMethod" /> and <see cref="P:System.Linq.Expressions.ElementInit.Arguments" /> properties set to the specified values.</returns>
		/// <exception cref="T:System.ArgumentNullException">
		///         <paramref name="addMethod" /> or <paramref name="arguments" /> is <see langword="null" />.</exception>
		/// <exception cref="T:System.ArgumentException">The method that addMethod represents is not named "Add" (case insensitive).-or-The method that addMethod represents is not an instance method.-or-arguments does not contain the same number of elements as the number of parameters for the method that addMethod represents.-or-The <see cref="P:System.Linq.Expressions.Expression.Type" /> property of one or more elements of <paramref name="arguments" /> is not assignable to the type of the corresponding parameter of the method that <paramref name="addMethod" /> represents.</exception>
		public static ElementInit ElementInit(MethodInfo addMethod, params Expression[] arguments)
		{
			return Expression.ElementInit(addMethod, arguments);
		}

		/// <summary>Creates an <see cref="T:System.Linq.Expressions.ElementInit" />, given an <see cref="T:System.Collections.Generic.IEnumerable`1" /> as the second argument.</summary>
		/// <param name="addMethod">A <see cref="T:System.Reflection.MethodInfo" /> to set the <see cref="P:System.Linq.Expressions.ElementInit.AddMethod" /> property equal to.</param>
		/// <param name="arguments">An <see cref="T:System.Collections.Generic.IEnumerable`1" /> that contains <see cref="T:System.Linq.Expressions.Expression" /> objects to set the <see cref="P:System.Linq.Expressions.ElementInit.Arguments" /> property equal to.</param>
		/// <returns>An <see cref="T:System.Linq.Expressions.ElementInit" /> that has the <see cref="P:System.Linq.Expressions.ElementInit.AddMethod" /> and <see cref="P:System.Linq.Expressions.ElementInit.Arguments" /> properties set to the specified values.</returns>
		/// <exception cref="T:System.ArgumentNullException">
		///         <paramref name="addMethod" /> or <paramref name="arguments" /> is <see langword="null" />.</exception>
		/// <exception cref="T:System.ArgumentException">The method that <paramref name="addMethod" /> represents is not named "Add" (case insensitive).-or-The method that <paramref name="addMethod" /> represents is not an instance method.-or-
		///         <paramref name="arguments" /> does not contain the same number of elements as the number of parameters for the method that <paramref name="addMethod" /> represents.-or-The <see cref="P:System.Linq.Expressions.Expression.Type" /> property of one or more elements of <paramref name="arguments" /> is not assignable to the type of the corresponding parameter of the method that <paramref name="addMethod" /> represents.</exception>
		public static ElementInit ElementInit(MethodInfo addMethod, IEnumerable<Expression> arguments)
		{
			ContractUtils.RequiresNotNull(addMethod, "addMethod");
			ContractUtils.RequiresNotNull(arguments, "arguments");
			ReadOnlyCollection<Expression> readOnlyCollection = arguments.ToReadOnly<Expression>();
			Expression.RequiresCanRead(readOnlyCollection, "arguments");
			Expression.ValidateElementInitAddMethodInfo(addMethod, "addMethod");
			Expression.ValidateArgumentTypes(addMethod, ExpressionType.Call, ref readOnlyCollection, "addMethod");
			return new ElementInit(addMethod, readOnlyCollection);
		}

		private static void ValidateElementInitAddMethodInfo(MethodInfo addMethod, string paramName)
		{
			Expression.ValidateMethodInfo(addMethod, paramName);
			ParameterInfo[] parametersCached = addMethod.GetParametersCached();
			if (parametersCached.Length == 0)
			{
				throw Error.ElementInitializerMethodWithZeroArgs(paramName);
			}
			if (!addMethod.Name.Equals("Add", StringComparison.OrdinalIgnoreCase))
			{
				throw Error.ElementInitializerMethodNotAdd(paramName);
			}
			if (addMethod.IsStatic)
			{
				throw Error.ElementInitializerMethodStatic(paramName);
			}
			foreach (ParameterInfo parameterInfo in parametersCached)
			{
				if (parameterInfo.ParameterType.IsByRef)
				{
					throw Error.ElementInitializerMethodNoRefOutParam(parameterInfo.Name, addMethod.Name, paramName);
				}
			}
		}

		/// <summary>Initializes a new instance of the <see cref="T:System.Linq.Expressions.Expression" /> class.</summary>
		/// <param name="nodeType">The <see cref="T:System.Linq.Expressions.ExpressionType" /> to set as the node type.</param>
		/// <param name="type">The <see cref="P:System.Linq.Expressions.Expression.Type" /> of this <see cref="T:System.Linq.Expressions.Expression" />.</param>
		[Obsolete("use a different constructor that does not take ExpressionType. Then override NodeType and Type properties to provide the values that would be specified to this constructor.")]
		protected Expression(ExpressionType nodeType, Type type)
		{
			if (Expression.s_legacyCtorSupportTable == null)
			{
				Interlocked.CompareExchange<ConditionalWeakTable<Expression, Expression.ExtensionInfo>>(ref Expression.s_legacyCtorSupportTable, new ConditionalWeakTable<Expression, Expression.ExtensionInfo>(), null);
			}
			Expression.s_legacyCtorSupportTable.Add(this, new Expression.ExtensionInfo(nodeType, type));
		}

		/// <summary>Constructs a new instance of <see cref="T:System.Linq.Expressions.Expression" />.</summary>
		protected Expression()
		{
		}

		/// <summary>Gets the node type of this <see cref="T:System.Linq.Expressions.Expression" />.</summary>
		/// <returns>One of the <see cref="T:System.Linq.Expressions.ExpressionType" /> values.</returns>
		public virtual ExpressionType NodeType
		{
			get
			{
				Expression.ExtensionInfo extensionInfo;
				if (Expression.s_legacyCtorSupportTable != null && Expression.s_legacyCtorSupportTable.TryGetValue(this, out extensionInfo))
				{
					return extensionInfo.NodeType;
				}
				throw Error.ExtensionNodeMustOverrideProperty("Expression.NodeType");
			}
		}

		/// <summary>Gets the static type of the expression that this <see cref="T:System.Linq.Expressions.Expression" /> represents.</summary>
		/// <returns>The <see cref="T:System.Type" /> that represents the static type of the expression.</returns>
		public virtual Type Type
		{
			get
			{
				Expression.ExtensionInfo extensionInfo;
				if (Expression.s_legacyCtorSupportTable != null && Expression.s_legacyCtorSupportTable.TryGetValue(this, out extensionInfo))
				{
					return extensionInfo.Type;
				}
				throw Error.ExtensionNodeMustOverrideProperty("Expression.Type");
			}
		}

		/// <summary>Indicates that the node can be reduced to a simpler node. If this returns true, Reduce() can be called to produce the reduced form.</summary>
		/// <returns>True if the node can be reduced, otherwise false.</returns>
		public virtual bool CanReduce
		{
			get
			{
				return false;
			}
		}

		/// <summary>Reduces this node to a simpler expression. If CanReduce returns true, this should return a valid expression. This method can return another node which itself must be reduced.</summary>
		/// <returns>The reduced expression.</returns>
		public virtual Expression Reduce()
		{
			if (this.CanReduce)
			{
				throw Error.ReducibleMustOverrideReduce();
			}
			return this;
		}

		/// <summary>Reduces the node and then calls the visitor delegate on the reduced expression. The method throws an exception if the node is not reducible.</summary>
		/// <param name="visitor">An instance of <see cref="T:System.Func`2" />.</param>
		/// <returns>The expression being visited, or an expression which should replace it in the tree.</returns>
		protected internal virtual Expression VisitChildren(ExpressionVisitor visitor)
		{
			if (!this.CanReduce)
			{
				throw Error.MustBeReducible();
			}
			return visitor.Visit(this.ReduceAndCheck());
		}

		/// <summary>Dispatches to the specific visit method for this node type. For example, <see cref="T:System.Linq.Expressions.MethodCallExpression" /> calls the <see cref="M:System.Linq.Expressions.ExpressionVisitor.VisitMethodCall(System.Linq.Expressions.MethodCallExpression)" />.</summary>
		/// <param name="visitor">The visitor to visit this node with.</param>
		/// <returns>The result of visiting this node.</returns>
		protected internal virtual Expression Accept(ExpressionVisitor visitor)
		{
			return visitor.VisitExtension(this);
		}

		/// <summary>Reduces this node to a simpler expression. If CanReduce returns true, this should return a valid expression. This method can return another node which itself must be reduced.</summary>
		/// <returns>The reduced expression.</returns>
		public Expression ReduceAndCheck()
		{
			if (!this.CanReduce)
			{
				throw Error.MustBeReducible();
			}
			Expression expression = this.Reduce();
			if (expression == null || expression == this)
			{
				throw Error.MustReduceToDifferent();
			}
			if (!TypeUtils.AreReferenceAssignable(this.Type, expression.Type))
			{
				throw Error.ReducedNotCompatible();
			}
			return expression;
		}

		/// <summary>Reduces the expression to a known node type (that is not an Extension node) or just returns the expression if it is already a known type.</summary>
		/// <returns>The reduced expression.</returns>
		public Expression ReduceExtensions()
		{
			Expression expression = this;
			while (expression.NodeType == ExpressionType.Extension)
			{
				expression = expression.ReduceAndCheck();
			}
			return expression;
		}

		/// <summary>Returns a textual representation of the <see cref="T:System.Linq.Expressions.Expression" />.</summary>
		/// <returns>A textual representation of the <see cref="T:System.Linq.Expressions.Expression" />.</returns>
		public override string ToString()
		{
			return ExpressionStringBuilder.ExpressionToString(this);
		}

		private string DebugView
		{
			get
			{
				string result;
				using (StringWriter stringWriter = new StringWriter(CultureInfo.CurrentCulture))
				{
					DebugViewWriter.WriteTo(this, stringWriter);
					result = stringWriter.ToString();
				}
				return result;
			}
		}

		private static void RequiresCanRead(IReadOnlyList<Expression> items, string paramName)
		{
			int i = 0;
			int count = items.Count;
			while (i < count)
			{
				ExpressionUtils.RequiresCanRead(items[i], paramName, i);
				i++;
			}
		}

		private static void RequiresCanWrite(Expression expression, string paramName)
		{
			if (expression == null)
			{
				throw new ArgumentNullException(paramName);
			}
			ExpressionType nodeType = expression.NodeType;
			if (nodeType != ExpressionType.MemberAccess)
			{
				if (nodeType == ExpressionType.Parameter)
				{
					return;
				}
				if (nodeType == ExpressionType.Index)
				{
					PropertyInfo indexer = ((IndexExpression)expression).Indexer;
					if (indexer == null || indexer.CanWrite)
					{
						return;
					}
				}
			}
			else
			{
				MemberInfo member = ((MemberExpression)expression).Member;
				PropertyInfo propertyInfo = member as PropertyInfo;
				if (propertyInfo != null)
				{
					if (propertyInfo.CanWrite)
					{
						return;
					}
				}
				else
				{
					FieldInfo fieldInfo = (FieldInfo)member;
					if (!fieldInfo.IsInitOnly && !fieldInfo.IsLiteral)
					{
						return;
					}
				}
			}
			throw Error.ExpressionMustBeWriteable(paramName);
		}

		/// <summary>Creates a <see cref="T:System.Linq.Expressions.DynamicExpression" /> that represents a dynamic operation bound by the provided <see cref="T:System.Runtime.CompilerServices.CallSiteBinder" />.</summary>
		/// <param name="binder">The runtime binder for the dynamic operation.</param>
		/// <param name="returnType">The result type of the dynamic expression.</param>
		/// <param name="arguments">The arguments to the dynamic operation.</param>
		/// <returns>A <see cref="T:System.Linq.Expressions.DynamicExpression" /> that has <see cref="P:System.Linq.Expressions.Expression.NodeType" /> equal to <see cref="F:System.Linq.Expressions.ExpressionType.Dynamic" /> and has the <see cref="P:System.Linq.Expressions.DynamicExpression.Binder" /> and <see cref="P:System.Linq.Expressions.DynamicExpression.Arguments" /> set to the specified values.</returns>
		public static DynamicExpression Dynamic(CallSiteBinder binder, Type returnType, IEnumerable<Expression> arguments)
		{
			return DynamicExpression.Dynamic(binder, returnType, arguments);
		}

		/// <summary>Creates a <see cref="T:System.Linq.Expressions.DynamicExpression" /> that represents a dynamic operation bound by the provided <see cref="T:System.Runtime.CompilerServices.CallSiteBinder" />.</summary>
		/// <param name="binder">The runtime binder for the dynamic operation.</param>
		/// <param name="returnType">The result type of the dynamic expression.</param>
		/// <param name="arg0">The first argument to the dynamic operation.</param>
		/// <returns>A <see cref="T:System.Linq.Expressions.DynamicExpression" /> that has <see cref="P:System.Linq.Expressions.Expression.NodeType" /> equal to <see cref="F:System.Linq.Expressions.ExpressionType.Dynamic" /> and has the <see cref="P:System.Linq.Expressions.DynamicExpression.Binder" /> and <see cref="P:System.Linq.Expressions.DynamicExpression.Arguments" /> set to the specified values.</returns>
		public static DynamicExpression Dynamic(CallSiteBinder binder, Type returnType, Expression arg0)
		{
			return DynamicExpression.Dynamic(binder, returnType, arg0);
		}

		/// <summary>Creates a <see cref="T:System.Linq.Expressions.DynamicExpression" /> that represents a dynamic operation bound by the provided <see cref="T:System.Runtime.CompilerServices.CallSiteBinder" />.</summary>
		/// <param name="binder">The runtime binder for the dynamic operation.</param>
		/// <param name="returnType">The result type of the dynamic expression.</param>
		/// <param name="arg0">The first argument to the dynamic operation.</param>
		/// <param name="arg1">The second argument to the dynamic operation.</param>
		/// <returns>A <see cref="T:System.Linq.Expressions.DynamicExpression" /> that has <see cref="P:System.Linq.Expressions.Expression.NodeType" /> equal to <see cref="F:System.Linq.Expressions.ExpressionType.Dynamic" /> and has the <see cref="P:System.Linq.Expressions.DynamicExpression.Binder" /> and <see cref="P:System.Linq.Expressions.DynamicExpression.Arguments" /> set to the specified values.</returns>
		public static DynamicExpression Dynamic(CallSiteBinder binder, Type returnType, Expression arg0, Expression arg1)
		{
			return DynamicExpression.Dynamic(binder, returnType, arg0, arg1);
		}

		/// <summary>Creates a <see cref="T:System.Linq.Expressions.DynamicExpression" /> that represents a dynamic operation bound by the provided <see cref="T:System.Runtime.CompilerServices.CallSiteBinder" />.</summary>
		/// <param name="binder">The runtime binder for the dynamic operation.</param>
		/// <param name="returnType">The result type of the dynamic expression.</param>
		/// <param name="arg0">The first argument to the dynamic operation.</param>
		/// <param name="arg1">The second argument to the dynamic operation.</param>
		/// <param name="arg2">The third argument to the dynamic operation.</param>
		/// <returns>A <see cref="T:System.Linq.Expressions.DynamicExpression" /> that has <see cref="P:System.Linq.Expressions.Expression.NodeType" /> equal to <see cref="F:System.Linq.Expressions.ExpressionType.Dynamic" /> and has the <see cref="P:System.Linq.Expressions.DynamicExpression.Binder" /> and <see cref="P:System.Linq.Expressions.DynamicExpression.Arguments" /> set to the specified values.</returns>
		public static DynamicExpression Dynamic(CallSiteBinder binder, Type returnType, Expression arg0, Expression arg1, Expression arg2)
		{
			return DynamicExpression.Dynamic(binder, returnType, arg0, arg1, arg2);
		}

		/// <summary>Creates a <see cref="T:System.Linq.Expressions.DynamicExpression" /> that represents a dynamic operation bound by the provided <see cref="T:System.Runtime.CompilerServices.CallSiteBinder" />.</summary>
		/// <param name="binder">The runtime binder for the dynamic operation.</param>
		/// <param name="returnType">The result type of the dynamic expression.</param>
		/// <param name="arg0">The first argument to the dynamic operation.</param>
		/// <param name="arg1">The second argument to the dynamic operation.</param>
		/// <param name="arg2">The third argument to the dynamic operation.</param>
		/// <param name="arg3">The fourth argument to the dynamic operation.</param>
		/// <returns>A <see cref="T:System.Linq.Expressions.DynamicExpression" /> that has <see cref="P:System.Linq.Expressions.Expression.NodeType" /> equal to <see cref="F:System.Linq.Expressions.ExpressionType.Dynamic" /> and has the <see cref="P:System.Linq.Expressions.DynamicExpression.Binder" /> and <see cref="P:System.Linq.Expressions.DynamicExpression.Arguments" /> set to the specified values.</returns>
		public static DynamicExpression Dynamic(CallSiteBinder binder, Type returnType, Expression arg0, Expression arg1, Expression arg2, Expression arg3)
		{
			return DynamicExpression.Dynamic(binder, returnType, arg0, arg1, arg2, arg3);
		}

		/// <summary>Creates a <see cref="T:System.Linq.Expressions.DynamicExpression" /> that represents a dynamic operation bound by the provided <see cref="T:System.Runtime.CompilerServices.CallSiteBinder" />.</summary>
		/// <param name="binder">The runtime binder for the dynamic operation.</param>
		/// <param name="returnType">The result type of the dynamic expression.</param>
		/// <param name="arguments">The arguments to the dynamic operation.</param>
		/// <returns>A <see cref="T:System.Linq.Expressions.DynamicExpression" /> that has <see cref="P:System.Linq.Expressions.Expression.NodeType" /> equal to <see cref="F:System.Linq.Expressions.ExpressionType.Dynamic" /> and has the <see cref="P:System.Linq.Expressions.DynamicExpression.Binder" /> and <see cref="P:System.Linq.Expressions.DynamicExpression.Arguments" /> set to the specified values.</returns>
		public static DynamicExpression Dynamic(CallSiteBinder binder, Type returnType, params Expression[] arguments)
		{
			return DynamicExpression.Dynamic(binder, returnType, arguments);
		}

		/// <summary>Creates a <see cref="T:System.Linq.Expressions.DynamicExpression" /> that represents a dynamic operation bound by the provided <see cref="T:System.Runtime.CompilerServices.CallSiteBinder" />.</summary>
		/// <param name="delegateType">The type of the delegate used by the <see cref="T:System.Runtime.CompilerServices.CallSite" />.</param>
		/// <param name="binder">The runtime binder for the dynamic operation.</param>
		/// <param name="arguments">The arguments to the dynamic operation.</param>
		/// <returns>A <see cref="T:System.Linq.Expressions.DynamicExpression" /> that has <see cref="P:System.Linq.Expressions.Expression.NodeType" /> equal to <see cref="F:System.Linq.Expressions.ExpressionType.Dynamic" /> and has the <see cref="P:System.Linq.Expressions.DynamicExpression.DelegateType" />, <see cref="P:System.Linq.Expressions.DynamicExpression.Binder" />, and <see cref="P:System.Linq.Expressions.DynamicExpression.Arguments" /> set to the specified values.</returns>
		public static DynamicExpression MakeDynamic(Type delegateType, CallSiteBinder binder, IEnumerable<Expression> arguments)
		{
			return DynamicExpression.MakeDynamic(delegateType, binder, arguments);
		}

		/// <summary>Creates a <see cref="T:System.Linq.Expressions.DynamicExpression" /> that represents a dynamic operation bound by the provided <see cref="T:System.Runtime.CompilerServices.CallSiteBinder" /> and one argument.</summary>
		/// <param name="delegateType">The type of the delegate used by the <see cref="T:System.Runtime.CompilerServices.CallSite" />.</param>
		/// <param name="binder">The runtime binder for the dynamic operation.</param>
		/// <param name="arg0">The argument to the dynamic operation.</param>
		/// <returns>A <see cref="T:System.Linq.Expressions.DynamicExpression" /> that has <see cref="P:System.Linq.Expressions.Expression.NodeType" /> equal to <see cref="F:System.Linq.Expressions.ExpressionType.Dynamic" /> and has the <see cref="P:System.Linq.Expressions.DynamicExpression.DelegateType" />, <see cref="P:System.Linq.Expressions.DynamicExpression.Binder" />, and <see cref="P:System.Linq.Expressions.DynamicExpression.Arguments" /> set to the specified values.</returns>
		public static DynamicExpression MakeDynamic(Type delegateType, CallSiteBinder binder, Expression arg0)
		{
			return DynamicExpression.MakeDynamic(delegateType, binder, arg0);
		}

		/// <summary>Creates a <see cref="T:System.Linq.Expressions.DynamicExpression" /> that represents a dynamic operation bound by the provided <see cref="T:System.Runtime.CompilerServices.CallSiteBinder" /> and two arguments.</summary>
		/// <param name="delegateType">The type of the delegate used by the <see cref="T:System.Runtime.CompilerServices.CallSite" />.</param>
		/// <param name="binder">The runtime binder for the dynamic operation.</param>
		/// <param name="arg0">The first argument to the dynamic operation.</param>
		/// <param name="arg1">The second argument to the dynamic operation.</param>
		/// <returns>A <see cref="T:System.Linq.Expressions.DynamicExpression" /> that has <see cref="P:System.Linq.Expressions.Expression.NodeType" /> equal to <see cref="F:System.Linq.Expressions.ExpressionType.Dynamic" /> and has the <see cref="P:System.Linq.Expressions.DynamicExpression.DelegateType" />, <see cref="P:System.Linq.Expressions.DynamicExpression.Binder" />, and <see cref="P:System.Linq.Expressions.DynamicExpression.Arguments" /> set to the specified values.</returns>
		public static DynamicExpression MakeDynamic(Type delegateType, CallSiteBinder binder, Expression arg0, Expression arg1)
		{
			return DynamicExpression.MakeDynamic(delegateType, binder, arg0, arg1);
		}

		/// <summary>Creates a <see cref="T:System.Linq.Expressions.DynamicExpression" /> that represents a dynamic operation bound by the provided <see cref="T:System.Runtime.CompilerServices.CallSiteBinder" /> and three arguments.</summary>
		/// <param name="delegateType">The type of the delegate used by the <see cref="T:System.Runtime.CompilerServices.CallSite" />.</param>
		/// <param name="binder">The runtime binder for the dynamic operation.</param>
		/// <param name="arg0">The first argument to the dynamic operation.</param>
		/// <param name="arg1">The second argument to the dynamic operation.</param>
		/// <param name="arg2">The third argument to the dynamic operation.</param>
		/// <returns>A <see cref="T:System.Linq.Expressions.DynamicExpression" /> that has <see cref="P:System.Linq.Expressions.Expression.NodeType" /> equal to <see cref="F:System.Linq.Expressions.ExpressionType.Dynamic" /> and has the <see cref="P:System.Linq.Expressions.DynamicExpression.DelegateType" />, <see cref="P:System.Linq.Expressions.DynamicExpression.Binder" />, and <see cref="P:System.Linq.Expressions.DynamicExpression.Arguments" /> set to the specified values.</returns>
		public static DynamicExpression MakeDynamic(Type delegateType, CallSiteBinder binder, Expression arg0, Expression arg1, Expression arg2)
		{
			return DynamicExpression.MakeDynamic(delegateType, binder, arg0, arg1, arg2);
		}

		/// <summary>Creates a <see cref="T:System.Linq.Expressions.DynamicExpression" /> that represents a dynamic operation bound by the provided <see cref="T:System.Runtime.CompilerServices.CallSiteBinder" /> and four arguments.</summary>
		/// <param name="delegateType">The type of the delegate used by the <see cref="T:System.Runtime.CompilerServices.CallSite" />.</param>
		/// <param name="binder">The runtime binder for the dynamic operation.</param>
		/// <param name="arg0">The first argument to the dynamic operation.</param>
		/// <param name="arg1">The second argument to the dynamic operation.</param>
		/// <param name="arg2">The third argument to the dynamic operation.</param>
		/// <param name="arg3">The fourth argument to the dynamic operation.</param>
		/// <returns>A <see cref="T:System.Linq.Expressions.DynamicExpression" /> that has <see cref="P:System.Linq.Expressions.Expression.NodeType" /> equal to <see cref="F:System.Linq.Expressions.ExpressionType.Dynamic" /> and has the <see cref="P:System.Linq.Expressions.DynamicExpression.DelegateType" />, <see cref="P:System.Linq.Expressions.DynamicExpression.Binder" />, and <see cref="P:System.Linq.Expressions.DynamicExpression.Arguments" /> set to the specified values.</returns>
		public static DynamicExpression MakeDynamic(Type delegateType, CallSiteBinder binder, Expression arg0, Expression arg1, Expression arg2, Expression arg3)
		{
			return DynamicExpression.MakeDynamic(delegateType, binder, arg0, arg1, arg2, arg3);
		}

		/// <summary>Creates a <see cref="T:System.Linq.Expressions.DynamicExpression" /> that represents a dynamic operation bound by the provided <see cref="T:System.Runtime.CompilerServices.CallSiteBinder" />.</summary>
		/// <param name="delegateType">The type of the delegate used by the <see cref="T:System.Runtime.CompilerServices.CallSite" />.</param>
		/// <param name="binder">The runtime binder for the dynamic operation.</param>
		/// <param name="arguments">The arguments to the dynamic operation.</param>
		/// <returns>A <see cref="T:System.Linq.Expressions.DynamicExpression" /> that has <see cref="P:System.Linq.Expressions.Expression.NodeType" /> equal to <see cref="F:System.Linq.Expressions.ExpressionType.Dynamic" /> and has the <see cref="P:System.Linq.Expressions.DynamicExpression.DelegateType" />, <see cref="P:System.Linq.Expressions.DynamicExpression.Binder" />, and <see cref="P:System.Linq.Expressions.DynamicExpression.Arguments" /> set to the specified values.</returns>
		public static DynamicExpression MakeDynamic(Type delegateType, CallSiteBinder binder, params Expression[] arguments)
		{
			return Expression.MakeDynamic(delegateType, binder, arguments);
		}

		/// <summary>Creates a <see cref="T:System.Linq.Expressions.GotoExpression" /> representing a break statement.</summary>
		/// <param name="target">The <see cref="T:System.Linq.Expressions.LabelTarget" /> that the <see cref="T:System.Linq.Expressions.GotoExpression" /> will jump to.</param>
		/// <returns>A <see cref="T:System.Linq.Expressions.GotoExpression" /> with <see cref="P:System.Linq.Expressions.GotoExpression.Kind" /> equal to Break, the <see cref="P:System.Linq.Expressions.GotoExpression.Target" /> property set to <paramref name="target" />, and a null value to be passed to the target label upon jumping.</returns>
		public static GotoExpression Break(LabelTarget target)
		{
			return Expression.MakeGoto(GotoExpressionKind.Break, target, null, typeof(void));
		}

		/// <summary>Creates a <see cref="T:System.Linq.Expressions.GotoExpression" /> representing a break statement. The value passed to the label upon jumping can be specified.</summary>
		/// <param name="target">The <see cref="T:System.Linq.Expressions.LabelTarget" /> that the <see cref="T:System.Linq.Expressions.GotoExpression" /> will jump to.</param>
		/// <param name="value">The value that will be passed to the associated label upon jumping.</param>
		/// <returns>A <see cref="T:System.Linq.Expressions.GotoExpression" /> with <see cref="P:System.Linq.Expressions.GotoExpression.Kind" /> equal to Break, the <see cref="P:System.Linq.Expressions.GotoExpression.Target" /> property set to <paramref name="target" />, and <paramref name="value" /> to be passed to the target label upon jumping.</returns>
		public static GotoExpression Break(LabelTarget target, Expression value)
		{
			return Expression.MakeGoto(GotoExpressionKind.Break, target, value, typeof(void));
		}

		/// <summary>Creates a <see cref="T:System.Linq.Expressions.GotoExpression" /> representing a break statement with the specified type.</summary>
		/// <param name="target">The <see cref="T:System.Linq.Expressions.LabelTarget" /> that the <see cref="T:System.Linq.Expressions.GotoExpression" /> will jump to.</param>
		/// <param name="type">An <see cref="T:System.Type" /> to set the <see cref="P:System.Linq.Expressions.Expression.Type" /> property equal to.</param>
		/// <returns>A <see cref="T:System.Linq.Expressions.GotoExpression" /> with <see cref="P:System.Linq.Expressions.GotoExpression.Kind" /> equal to Break, the <see cref="P:System.Linq.Expressions.GotoExpression.Target" /> property set to <paramref name="target" />, and the <see cref="P:System.Linq.Expressions.Expression.Type" /> property set to <paramref name="type" />.</returns>
		public static GotoExpression Break(LabelTarget target, Type type)
		{
			return Expression.MakeGoto(GotoExpressionKind.Break, target, null, type);
		}

		/// <summary>Creates a <see cref="T:System.Linq.Expressions.GotoExpression" /> representing a break statement with the specified type. The value passed to the label upon jumping can be specified.</summary>
		/// <param name="target">The <see cref="T:System.Linq.Expressions.LabelTarget" /> that the <see cref="T:System.Linq.Expressions.GotoExpression" /> will jump to.</param>
		/// <param name="value">The value that will be passed to the associated label upon jumping.</param>
		/// <param name="type">An <see cref="T:System.Type" /> to set the <see cref="P:System.Linq.Expressions.Expression.Type" /> property equal to.</param>
		/// <returns>A <see cref="T:System.Linq.Expressions.GotoExpression" /> with <see cref="P:System.Linq.Expressions.GotoExpression.Kind" /> equal to Break, the <see cref="P:System.Linq.Expressions.GotoExpression.Target" /> property set to <paramref name="target" />, the <see cref="P:System.Linq.Expressions.Expression.Type" /> property set to <paramref name="type" />, and <paramref name="value" /> to be passed to the target label upon jumping.</returns>
		public static GotoExpression Break(LabelTarget target, Expression value, Type type)
		{
			return Expression.MakeGoto(GotoExpressionKind.Break, target, value, type);
		}

		/// <summary>Creates a <see cref="T:System.Linq.Expressions.GotoExpression" /> representing a continue statement.</summary>
		/// <param name="target">The <see cref="T:System.Linq.Expressions.LabelTarget" /> that the <see cref="T:System.Linq.Expressions.GotoExpression" /> will jump to.</param>
		/// <returns>A <see cref="T:System.Linq.Expressions.GotoExpression" /> with <see cref="P:System.Linq.Expressions.GotoExpression.Kind" /> equal to Continue, the <see cref="P:System.Linq.Expressions.GotoExpression.Target" /> property set to <paramref name="target" />, and a null value to be passed to the target label upon jumping.</returns>
		public static GotoExpression Continue(LabelTarget target)
		{
			return Expression.MakeGoto(GotoExpressionKind.Continue, target, null, typeof(void));
		}

		/// <summary>Creates a <see cref="T:System.Linq.Expressions.GotoExpression" /> representing a continue statement with the specified type.</summary>
		/// <param name="target">The <see cref="T:System.Linq.Expressions.LabelTarget" /> that the <see cref="T:System.Linq.Expressions.GotoExpression" /> will jump to.</param>
		/// <param name="type">An <see cref="T:System.Type" /> to set the <see cref="P:System.Linq.Expressions.Expression.Type" /> property equal to.</param>
		/// <returns>A <see cref="T:System.Linq.Expressions.GotoExpression" /> with <see cref="P:System.Linq.Expressions.GotoExpression.Kind" /> equal to Continue, the <see cref="P:System.Linq.Expressions.GotoExpression.Target" /> property set to <paramref name="target" />, the <see cref="P:System.Linq.Expressions.Expression.Type" /> property set to <paramref name="type" />, and a null value to be passed to the target label upon jumping.</returns>
		public static GotoExpression Continue(LabelTarget target, Type type)
		{
			return Expression.MakeGoto(GotoExpressionKind.Continue, target, null, type);
		}

		/// <summary>Creates a <see cref="T:System.Linq.Expressions.GotoExpression" /> representing a return statement.</summary>
		/// <param name="target">The <see cref="T:System.Linq.Expressions.LabelTarget" /> that the <see cref="T:System.Linq.Expressions.GotoExpression" /> will jump to.</param>
		/// <returns>A <see cref="T:System.Linq.Expressions.GotoExpression" /> with <see cref="P:System.Linq.Expressions.GotoExpression.Kind" /> equal to Return, the <see cref="P:System.Linq.Expressions.GotoExpression.Target" /> property set to <paramref name="target" />, and a null value to be passed to the target label upon jumping.</returns>
		public static GotoExpression Return(LabelTarget target)
		{
			return Expression.MakeGoto(GotoExpressionKind.Return, target, null, typeof(void));
		}

		/// <summary>Creates a <see cref="T:System.Linq.Expressions.GotoExpression" /> representing a return statement with the specified type.</summary>
		/// <param name="target">The <see cref="T:System.Linq.Expressions.LabelTarget" /> that the <see cref="T:System.Linq.Expressions.GotoExpression" /> will jump to.</param>
		/// <param name="type">An <see cref="T:System.Type" /> to set the <see cref="P:System.Linq.Expressions.Expression.Type" /> property equal to.</param>
		/// <returns>A <see cref="T:System.Linq.Expressions.GotoExpression" /> with <see cref="P:System.Linq.Expressions.GotoExpression.Kind" /> equal to Return, the <see cref="P:System.Linq.Expressions.GotoExpression.Target" /> property set to <paramref name="target" />, the <see cref="P:System.Linq.Expressions.Expression.Type" /> property set to <paramref name="type" />, and a null value to be passed to the target label upon jumping.</returns>
		public static GotoExpression Return(LabelTarget target, Type type)
		{
			return Expression.MakeGoto(GotoExpressionKind.Return, target, null, type);
		}

		/// <summary>Creates a <see cref="T:System.Linq.Expressions.GotoExpression" /> representing a return statement. The value passed to the label upon jumping can be specified.</summary>
		/// <param name="target">The <see cref="T:System.Linq.Expressions.LabelTarget" /> that the <see cref="T:System.Linq.Expressions.GotoExpression" /> will jump to.</param>
		/// <param name="value">The value that will be passed to the associated label upon jumping.</param>
		/// <returns>A <see cref="T:System.Linq.Expressions.GotoExpression" /> with <see cref="P:System.Linq.Expressions.GotoExpression.Kind" /> equal to Continue, the <see cref="P:System.Linq.Expressions.GotoExpression.Target" /> property set to <paramref name="target" />, and <paramref name="value" /> to be passed to the target label upon jumping.</returns>
		public static GotoExpression Return(LabelTarget target, Expression value)
		{
			return Expression.MakeGoto(GotoExpressionKind.Return, target, value, typeof(void));
		}

		/// <summary>Creates a <see cref="T:System.Linq.Expressions.GotoExpression" /> representing a return statement with the specified type. The value passed to the label upon jumping can be specified.</summary>
		/// <param name="target">The <see cref="T:System.Linq.Expressions.LabelTarget" /> that the <see cref="T:System.Linq.Expressions.GotoExpression" /> will jump to.</param>
		/// <param name="value">The value that will be passed to the associated label upon jumping.</param>
		/// <param name="type">An <see cref="T:System.Type" /> to set the <see cref="P:System.Linq.Expressions.Expression.Type" /> property equal to.</param>
		/// <returns>A <see cref="T:System.Linq.Expressions.GotoExpression" /> with <see cref="P:System.Linq.Expressions.GotoExpression.Kind" /> equal to Continue, the <see cref="P:System.Linq.Expressions.GotoExpression.Target" /> property set to <paramref name="target" />, the <see cref="P:System.Linq.Expressions.Expression.Type" /> property set to <paramref name="type" />, and <paramref name="value" /> to be passed to the target label upon jumping.</returns>
		public static GotoExpression Return(LabelTarget target, Expression value, Type type)
		{
			return Expression.MakeGoto(GotoExpressionKind.Return, target, value, type);
		}

		/// <summary>Creates a <see cref="T:System.Linq.Expressions.GotoExpression" /> representing a "go to" statement.</summary>
		/// <param name="target">The <see cref="T:System.Linq.Expressions.LabelTarget" /> that the <see cref="T:System.Linq.Expressions.GotoExpression" /> will jump to.</param>
		/// <returns>A <see cref="T:System.Linq.Expressions.GotoExpression" /> with <see cref="P:System.Linq.Expressions.GotoExpression.Kind" /> equal to Goto, the <see cref="P:System.Linq.Expressions.GotoExpression.Target" /> property set to the specified value, and a null value to be passed to the target label upon jumping.</returns>
		public static GotoExpression Goto(LabelTarget target)
		{
			return Expression.MakeGoto(GotoExpressionKind.Goto, target, null, typeof(void));
		}

		/// <summary>Creates a <see cref="T:System.Linq.Expressions.GotoExpression" /> representing a "go to" statement with the specified type.</summary>
		/// <param name="target">The <see cref="T:System.Linq.Expressions.LabelTarget" /> that the <see cref="T:System.Linq.Expressions.GotoExpression" /> will jump to.</param>
		/// <param name="type">An <see cref="T:System.Type" /> to set the <see cref="P:System.Linq.Expressions.Expression.Type" /> property equal to.</param>
		/// <returns>A <see cref="T:System.Linq.Expressions.GotoExpression" /> with <see cref="P:System.Linq.Expressions.GotoExpression.Kind" /> equal to Goto, the <see cref="P:System.Linq.Expressions.GotoExpression.Target" /> property set to the specified value, the <see cref="P:System.Linq.Expressions.Expression.Type" /> property set to <paramref name="type" />, and a null value to be passed to the target label upon jumping.</returns>
		public static GotoExpression Goto(LabelTarget target, Type type)
		{
			return Expression.MakeGoto(GotoExpressionKind.Goto, target, null, type);
		}

		/// <summary>Creates a <see cref="T:System.Linq.Expressions.GotoExpression" /> representing a "go to" statement. The value passed to the label upon jumping can be specified.</summary>
		/// <param name="target">The <see cref="T:System.Linq.Expressions.LabelTarget" /> that the <see cref="T:System.Linq.Expressions.GotoExpression" /> will jump to.</param>
		/// <param name="value">The value that will be passed to the associated label upon jumping.</param>
		/// <returns>A <see cref="T:System.Linq.Expressions.GotoExpression" /> with <see cref="P:System.Linq.Expressions.GotoExpression.Kind" /> equal to Goto, the <see cref="P:System.Linq.Expressions.GotoExpression.Target" /> property set to <paramref name="target" />, and <paramref name="value" /> to be passed to the target label upon jumping.</returns>
		public static GotoExpression Goto(LabelTarget target, Expression value)
		{
			return Expression.MakeGoto(GotoExpressionKind.Goto, target, value, typeof(void));
		}

		/// <summary>Creates a <see cref="T:System.Linq.Expressions.GotoExpression" /> representing a "go to" statement with the specified type. The value passed to the label upon jumping can be specified.</summary>
		/// <param name="target">The <see cref="T:System.Linq.Expressions.LabelTarget" /> that the <see cref="T:System.Linq.Expressions.GotoExpression" /> will jump to.</param>
		/// <param name="value">The value that will be passed to the associated label upon jumping.</param>
		/// <param name="type">An <see cref="T:System.Type" /> to set the <see cref="P:System.Linq.Expressions.Expression.Type" /> property equal to.</param>
		/// <returns>A <see cref="T:System.Linq.Expressions.GotoExpression" /> with <see cref="P:System.Linq.Expressions.GotoExpression.Kind" /> equal to Goto, the <see cref="P:System.Linq.Expressions.GotoExpression.Target" /> property set to <paramref name="target" />, the <see cref="P:System.Linq.Expressions.Expression.Type" /> property set to <paramref name="type" />, and <paramref name="value" /> to be passed to the target label upon jumping.</returns>
		public static GotoExpression Goto(LabelTarget target, Expression value, Type type)
		{
			return Expression.MakeGoto(GotoExpressionKind.Goto, target, value, type);
		}

		/// <summary>Creates a <see cref="T:System.Linq.Expressions.GotoExpression" /> representing a jump of the specified <see cref="T:System.Linq.Expressions.GotoExpressionKind" />. The value passed to the label upon jumping can also be specified.</summary>
		/// <param name="kind">The <see cref="T:System.Linq.Expressions.GotoExpressionKind" /> of the <see cref="T:System.Linq.Expressions.GotoExpression" />.</param>
		/// <param name="target">The <see cref="T:System.Linq.Expressions.LabelTarget" /> that the <see cref="T:System.Linq.Expressions.GotoExpression" /> will jump to.</param>
		/// <param name="value">The value that will be passed to the associated label upon jumping.</param>
		/// <param name="type">An <see cref="T:System.Type" /> to set the <see cref="P:System.Linq.Expressions.Expression.Type" /> property equal to.</param>
		/// <returns>A <see cref="T:System.Linq.Expressions.GotoExpression" /> with <see cref="P:System.Linq.Expressions.GotoExpression.Kind" /> equal to <paramref name="kind" />, the <see cref="P:System.Linq.Expressions.GotoExpression.Target" /> property set to <paramref name="target" />, the <see cref="P:System.Linq.Expressions.Expression.Type" /> property set to <paramref name="type" />, and <paramref name="value" /> to be passed to the target label upon jumping.</returns>
		public static GotoExpression MakeGoto(GotoExpressionKind kind, LabelTarget target, Expression value, Type type)
		{
			Expression.ValidateGoto(target, ref value, "target", "value", type);
			return new GotoExpression(kind, target, value, type);
		}

		private static void ValidateGoto(LabelTarget target, ref Expression value, string targetParameter, string valueParameter, Type type)
		{
			ContractUtils.RequiresNotNull(target, targetParameter);
			if (value == null)
			{
				if (target.Type != typeof(void))
				{
					throw Error.LabelMustBeVoidOrHaveExpression("target");
				}
				if (type != null)
				{
					TypeUtils.ValidateType(type, "type");
					return;
				}
			}
			else
			{
				Expression.ValidateGotoType(target.Type, ref value, valueParameter);
			}
		}

		private static void ValidateGotoType(Type expectedType, ref Expression value, string paramName)
		{
			ExpressionUtils.RequiresCanRead(value, paramName);
			if (expectedType != typeof(void) && !TypeUtils.AreReferenceAssignable(expectedType, value.Type) && !Expression.TryQuote(expectedType, ref value))
			{
				throw Error.ExpressionTypeDoesNotMatchLabel(value.Type, expectedType);
			}
		}

		/// <summary>Creates an <see cref="T:System.Linq.Expressions.IndexExpression" /> that represents accessing an indexed property in an object.</summary>
		/// <param name="instance">The object to which the property belongs. It should be null if the property is <see langword="static" /> (<see langword="shared" /> in Visual Basic).</param>
		/// <param name="indexer">An <see cref="T:System.Linq.Expressions.Expression" /> representing the property to index.</param>
		/// <param name="arguments">An IEnumerable&lt;Expression&gt; (IEnumerable (Of Expression) in Visual Basic) that contains the arguments that will be used to index the property.</param>
		/// <returns>The created <see cref="T:System.Linq.Expressions.IndexExpression" />.</returns>
		public static IndexExpression MakeIndex(Expression instance, PropertyInfo indexer, IEnumerable<Expression> arguments)
		{
			if (indexer != null)
			{
				return Expression.Property(instance, indexer, arguments);
			}
			return Expression.ArrayAccess(instance, arguments);
		}

		/// <summary>Creates an <see cref="T:System.Linq.Expressions.IndexExpression" /> to access an array.</summary>
		/// <param name="array">An expression representing the array to index.</param>
		/// <param name="indexes">An array that contains expressions used to index the array.</param>
		/// <returns>The created <see cref="T:System.Linq.Expressions.IndexExpression" />.</returns>
		public static IndexExpression ArrayAccess(Expression array, params Expression[] indexes)
		{
			return Expression.ArrayAccess(array, indexes);
		}

		/// <summary>Creates an <see cref="T:System.Linq.Expressions.IndexExpression" /> to access a multidimensional array.</summary>
		/// <param name="array">An expression that represents the multidimensional array.</param>
		/// <param name="indexes">An <see cref="T:System.Collections.Generic.IEnumerable`1" /> containing expressions used to index the array.</param>
		/// <returns>The created <see cref="T:System.Linq.Expressions.IndexExpression" />.</returns>
		public static IndexExpression ArrayAccess(Expression array, IEnumerable<Expression> indexes)
		{
			ExpressionUtils.RequiresCanRead(array, "array");
			Type type = array.Type;
			if (!type.IsArray)
			{
				throw Error.ArgumentMustBeArray("array");
			}
			ReadOnlyCollection<Expression> readOnlyCollection = indexes.ToReadOnly<Expression>();
			if (type.GetArrayRank() != readOnlyCollection.Count)
			{
				throw Error.IncorrectNumberOfIndexes();
			}
			foreach (Expression expression in readOnlyCollection)
			{
				ExpressionUtils.RequiresCanRead(expression, "indexes");
				if (expression.Type != typeof(int))
				{
					throw Error.ArgumentMustBeArrayIndexType("indexes");
				}
			}
			return new IndexExpression(array, null, readOnlyCollection);
		}

		/// <summary>Creates an <see cref="T:System.Linq.Expressions.IndexExpression" /> representing the access to an indexed property.</summary>
		/// <param name="instance">The object to which the property belongs. If the property is static/shared, it must be null.</param>
		/// <param name="propertyName">The name of the indexer.</param>
		/// <param name="arguments">An array of <see cref="T:System.Linq.Expressions.Expression" /> objects that are used to index the property.</param>
		/// <returns>The created <see cref="T:System.Linq.Expressions.IndexExpression" />.</returns>
		public static IndexExpression Property(Expression instance, string propertyName, params Expression[] arguments)
		{
			ExpressionUtils.RequiresCanRead(instance, "instance");
			ContractUtils.RequiresNotNull(propertyName, "propertyName");
			PropertyInfo indexer = Expression.FindInstanceProperty(instance.Type, propertyName, arguments);
			return Expression.MakeIndexProperty(instance, indexer, "propertyName", arguments.ToReadOnly<Expression>());
		}

		private static PropertyInfo FindInstanceProperty(Type type, string propertyName, Expression[] arguments)
		{
			BindingFlags flags = BindingFlags.IgnoreCase | BindingFlags.Instance | BindingFlags.Public | BindingFlags.FlattenHierarchy;
			PropertyInfo propertyInfo = Expression.FindProperty(type, propertyName, arguments, flags);
			if (propertyInfo == null)
			{
				flags = (BindingFlags.IgnoreCase | BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.FlattenHierarchy);
				propertyInfo = Expression.FindProperty(type, propertyName, arguments, flags);
			}
			if (!(propertyInfo == null))
			{
				return propertyInfo;
			}
			if (arguments == null || arguments.Length == 0)
			{
				throw Error.InstancePropertyWithoutParameterNotDefinedForType(propertyName, type);
			}
			throw Error.InstancePropertyWithSpecifiedParametersNotDefinedForType(propertyName, Expression.GetArgTypesString(arguments), type, "propertyName");
		}

		private static string GetArgTypesString(Expression[] arguments)
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append('(');
			for (int i = 0; i < arguments.Length; i++)
			{
				if (i != 0)
				{
					stringBuilder.Append(", ");
				}
				StringBuilder stringBuilder2 = stringBuilder;
				Expression expression = arguments[i];
				stringBuilder2.Append((expression != null) ? expression.Type.Name : null);
			}
			stringBuilder.Append(')');
			return stringBuilder.ToString();
		}

		private static PropertyInfo FindProperty(Type type, string propertyName, Expression[] arguments, BindingFlags flags)
		{
			PropertyInfo propertyInfo = null;
			foreach (PropertyInfo propertyInfo2 in type.GetProperties(flags))
			{
				if (propertyInfo2.Name.Equals(propertyName, StringComparison.OrdinalIgnoreCase) && Expression.IsCompatible(propertyInfo2, arguments))
				{
					if (!(propertyInfo == null))
					{
						throw Error.PropertyWithMoreThanOneMatch(propertyName, type);
					}
					propertyInfo = propertyInfo2;
				}
			}
			return propertyInfo;
		}

		private static bool IsCompatible(PropertyInfo pi, Expression[] args)
		{
			MethodInfo methodInfo = pi.GetGetMethod(true);
			ParameterInfo[] array;
			if (methodInfo != null)
			{
				array = methodInfo.GetParametersCached();
			}
			else
			{
				methodInfo = pi.GetSetMethod(true);
				if (methodInfo == null)
				{
					return false;
				}
				array = methodInfo.GetParametersCached();
				if (array.Length == 0)
				{
					return false;
				}
				array = array.RemoveLast<ParameterInfo>();
			}
			if (args == null)
			{
				return array.Length == 0;
			}
			if (array.Length != args.Length)
			{
				return false;
			}
			for (int i = 0; i < args.Length; i++)
			{
				if (args[i] == null)
				{
					return false;
				}
				if (!TypeUtils.AreReferenceAssignable(array[i].ParameterType, args[i].Type))
				{
					return false;
				}
			}
			return true;
		}

		/// <summary>Creates an <see cref="T:System.Linq.Expressions.IndexExpression" /> representing the access to an indexed property.</summary>
		/// <param name="instance">The object to which the property belongs. If the property is static/shared, it must be null.</param>
		/// <param name="indexer">The <see cref="T:System.Reflection.PropertyInfo" /> that represents the property to index.</param>
		/// <param name="arguments">An array of <see cref="T:System.Linq.Expressions.Expression" /> objects that are used to index the property.</param>
		/// <returns>The created <see cref="T:System.Linq.Expressions.IndexExpression" />.</returns>
		public static IndexExpression Property(Expression instance, PropertyInfo indexer, params Expression[] arguments)
		{
			return Expression.Property(instance, indexer, arguments);
		}

		/// <summary>Creates an <see cref="T:System.Linq.Expressions.IndexExpression" /> representing the access to an indexed property.</summary>
		/// <param name="instance">The object to which the property belongs. If the property is static/shared, it must be null.</param>
		/// <param name="indexer">The <see cref="T:System.Reflection.PropertyInfo" /> that represents the property to index.</param>
		/// <param name="arguments">An <see cref="T:System.Collections.Generic.IEnumerable`1" /> of <see cref="T:System.Linq.Expressions.Expression" /> objects that are used to index the property.</param>
		/// <returns>The created <see cref="T:System.Linq.Expressions.IndexExpression" />.</returns>
		public static IndexExpression Property(Expression instance, PropertyInfo indexer, IEnumerable<Expression> arguments)
		{
			return Expression.MakeIndexProperty(instance, indexer, "indexer", arguments.ToReadOnly<Expression>());
		}

		private static IndexExpression MakeIndexProperty(Expression instance, PropertyInfo indexer, string paramName, ReadOnlyCollection<Expression> argList)
		{
			Expression.ValidateIndexedProperty(instance, indexer, paramName, ref argList);
			return new IndexExpression(instance, indexer, argList);
		}

		private static void ValidateIndexedProperty(Expression instance, PropertyInfo indexer, string paramName, ref ReadOnlyCollection<Expression> argList)
		{
			ContractUtils.RequiresNotNull(indexer, paramName);
			if (indexer.PropertyType.IsByRef)
			{
				throw Error.PropertyCannotHaveRefType(paramName);
			}
			if (indexer.PropertyType == typeof(void))
			{
				throw Error.PropertyTypeCannotBeVoid(paramName);
			}
			ParameterInfo[] array = null;
			MethodInfo getMethod = indexer.GetGetMethod(true);
			if (getMethod != null)
			{
				if (getMethod.ReturnType != indexer.PropertyType)
				{
					throw Error.PropertyTypeMustMatchGetter(paramName);
				}
				array = getMethod.GetParametersCached();
				Expression.ValidateAccessor(instance, getMethod, array, ref argList, paramName);
			}
			MethodInfo setMethod = indexer.GetSetMethod(true);
			if (setMethod != null)
			{
				ParameterInfo[] parametersCached = setMethod.GetParametersCached();
				if (parametersCached.Length == 0)
				{
					throw Error.SetterHasNoParams(paramName);
				}
				Type parameterType = parametersCached[parametersCached.Length - 1].ParameterType;
				if (parameterType.IsByRef)
				{
					throw Error.PropertyCannotHaveRefType(paramName);
				}
				if (setMethod.ReturnType != typeof(void))
				{
					throw Error.SetterMustBeVoid(paramName);
				}
				if (indexer.PropertyType != parameterType)
				{
					throw Error.PropertyTypeMustMatchSetter(paramName);
				}
				if (!(getMethod != null))
				{
					Expression.ValidateAccessor(instance, setMethod, parametersCached.RemoveLast<ParameterInfo>(), ref argList, paramName);
					return;
				}
				if (getMethod.IsStatic ^ setMethod.IsStatic)
				{
					throw Error.BothAccessorsMustBeStatic(paramName);
				}
				if (array.Length != parametersCached.Length - 1)
				{
					throw Error.IndexesOfSetGetMustMatch(paramName);
				}
				for (int i = 0; i < array.Length; i++)
				{
					if (array[i].ParameterType != parametersCached[i].ParameterType)
					{
						throw Error.IndexesOfSetGetMustMatch(paramName);
					}
				}
				return;
			}
			else
			{
				if (getMethod == null)
				{
					throw Error.PropertyDoesNotHaveAccessor(indexer, paramName);
				}
				return;
			}
		}

		private static void ValidateAccessor(Expression instance, MethodInfo method, ParameterInfo[] indexes, ref ReadOnlyCollection<Expression> arguments, string paramName)
		{
			ContractUtils.RequiresNotNull(arguments, "arguments");
			Expression.ValidateMethodInfo(method, "method");
			if ((method.CallingConvention & CallingConventions.VarArgs) != (CallingConventions)0)
			{
				throw Error.AccessorsCannotHaveVarArgs(paramName);
			}
			if (method.IsStatic)
			{
				if (instance != null)
				{
					throw Error.OnlyStaticPropertiesHaveNullInstance("instance");
				}
			}
			else
			{
				if (instance == null)
				{
					throw Error.OnlyStaticPropertiesHaveNullInstance("instance");
				}
				ExpressionUtils.RequiresCanRead(instance, "instance");
				Expression.ValidateCallInstanceType(instance.Type, method);
			}
			Expression.ValidateAccessorArgumentTypes(method, indexes, ref arguments, paramName);
		}

		private static void ValidateAccessorArgumentTypes(MethodInfo method, ParameterInfo[] indexes, ref ReadOnlyCollection<Expression> arguments, string paramName)
		{
			if (indexes.Length != 0)
			{
				if (indexes.Length != arguments.Count)
				{
					throw Error.IncorrectNumberOfMethodCallArguments(method, paramName);
				}
				Expression[] array = null;
				int i = 0;
				int num = indexes.Length;
				while (i < num)
				{
					Expression expression = arguments[i];
					ParameterInfo parameterInfo = indexes[i];
					ExpressionUtils.RequiresCanRead(expression, "arguments", i);
					Type parameterType = parameterInfo.ParameterType;
					if (parameterType.IsByRef)
					{
						throw Error.AccessorsCannotHaveByRefArgs("indexes", i);
					}
					TypeUtils.ValidateType(parameterType, "indexes", i);
					if (!TypeUtils.AreReferenceAssignable(parameterType, expression.Type) && !Expression.TryQuote(parameterType, ref expression))
					{
						throw Error.ExpressionTypeDoesNotMatchMethodParameter(expression.Type, parameterType, method, "arguments", i);
					}
					if (array == null && expression != arguments[i])
					{
						array = new Expression[arguments.Count];
						for (int j = 0; j < i; j++)
						{
							array[j] = arguments[j];
						}
					}
					if (array != null)
					{
						array[i] = expression;
					}
					i++;
				}
				if (array != null)
				{
					arguments = new TrueReadOnlyCollection<Expression>(array);
					return;
				}
			}
			else if (arguments.Count > 0)
			{
				throw Error.IncorrectNumberOfMethodCallArguments(method, paramName);
			}
		}

		internal static InvocationExpression Invoke(Expression expression)
		{
			ExpressionUtils.RequiresCanRead(expression, "expression");
			MethodInfo invokeMethod = Expression.GetInvokeMethod(expression);
			ParameterInfo[] parametersForValidation = Expression.GetParametersForValidation(invokeMethod, ExpressionType.Invoke);
			Expression.ValidateArgumentCount(invokeMethod, ExpressionType.Invoke, 0, parametersForValidation);
			return new InvocationExpression0(expression, invokeMethod.ReturnType);
		}

		internal static InvocationExpression Invoke(Expression expression, Expression arg0)
		{
			ExpressionUtils.RequiresCanRead(expression, "expression");
			MethodInfo invokeMethod = Expression.GetInvokeMethod(expression);
			ParameterInfo[] parametersForValidation = Expression.GetParametersForValidation(invokeMethod, ExpressionType.Invoke);
			Expression.ValidateArgumentCount(invokeMethod, ExpressionType.Invoke, 1, parametersForValidation);
			arg0 = Expression.ValidateOneArgument(invokeMethod, ExpressionType.Invoke, arg0, parametersForValidation[0], "expression", "arg0");
			return new InvocationExpression1(expression, invokeMethod.ReturnType, arg0);
		}

		internal static InvocationExpression Invoke(Expression expression, Expression arg0, Expression arg1)
		{
			ExpressionUtils.RequiresCanRead(expression, "expression");
			MethodInfo invokeMethod = Expression.GetInvokeMethod(expression);
			ParameterInfo[] parametersForValidation = Expression.GetParametersForValidation(invokeMethod, ExpressionType.Invoke);
			Expression.ValidateArgumentCount(invokeMethod, ExpressionType.Invoke, 2, parametersForValidation);
			arg0 = Expression.ValidateOneArgument(invokeMethod, ExpressionType.Invoke, arg0, parametersForValidation[0], "expression", "arg0");
			arg1 = Expression.ValidateOneArgument(invokeMethod, ExpressionType.Invoke, arg1, parametersForValidation[1], "expression", "arg1");
			return new InvocationExpression2(expression, invokeMethod.ReturnType, arg0, arg1);
		}

		internal static InvocationExpression Invoke(Expression expression, Expression arg0, Expression arg1, Expression arg2)
		{
			ExpressionUtils.RequiresCanRead(expression, "expression");
			MethodInfo invokeMethod = Expression.GetInvokeMethod(expression);
			ParameterInfo[] parametersForValidation = Expression.GetParametersForValidation(invokeMethod, ExpressionType.Invoke);
			Expression.ValidateArgumentCount(invokeMethod, ExpressionType.Invoke, 3, parametersForValidation);
			arg0 = Expression.ValidateOneArgument(invokeMethod, ExpressionType.Invoke, arg0, parametersForValidation[0], "expression", "arg0");
			arg1 = Expression.ValidateOneArgument(invokeMethod, ExpressionType.Invoke, arg1, parametersForValidation[1], "expression", "arg1");
			arg2 = Expression.ValidateOneArgument(invokeMethod, ExpressionType.Invoke, arg2, parametersForValidation[2], "expression", "arg2");
			return new InvocationExpression3(expression, invokeMethod.ReturnType, arg0, arg1, arg2);
		}

		internal static InvocationExpression Invoke(Expression expression, Expression arg0, Expression arg1, Expression arg2, Expression arg3)
		{
			ExpressionUtils.RequiresCanRead(expression, "expression");
			MethodInfo invokeMethod = Expression.GetInvokeMethod(expression);
			ParameterInfo[] parametersForValidation = Expression.GetParametersForValidation(invokeMethod, ExpressionType.Invoke);
			Expression.ValidateArgumentCount(invokeMethod, ExpressionType.Invoke, 4, parametersForValidation);
			arg0 = Expression.ValidateOneArgument(invokeMethod, ExpressionType.Invoke, arg0, parametersForValidation[0], "expression", "arg0");
			arg1 = Expression.ValidateOneArgument(invokeMethod, ExpressionType.Invoke, arg1, parametersForValidation[1], "expression", "arg1");
			arg2 = Expression.ValidateOneArgument(invokeMethod, ExpressionType.Invoke, arg2, parametersForValidation[2], "expression", "arg2");
			arg3 = Expression.ValidateOneArgument(invokeMethod, ExpressionType.Invoke, arg3, parametersForValidation[3], "expression", "arg3");
			return new InvocationExpression4(expression, invokeMethod.ReturnType, arg0, arg1, arg2, arg3);
		}

		internal static InvocationExpression Invoke(Expression expression, Expression arg0, Expression arg1, Expression arg2, Expression arg3, Expression arg4)
		{
			ExpressionUtils.RequiresCanRead(expression, "expression");
			MethodInfo invokeMethod = Expression.GetInvokeMethod(expression);
			ParameterInfo[] parametersForValidation = Expression.GetParametersForValidation(invokeMethod, ExpressionType.Invoke);
			Expression.ValidateArgumentCount(invokeMethod, ExpressionType.Invoke, 5, parametersForValidation);
			arg0 = Expression.ValidateOneArgument(invokeMethod, ExpressionType.Invoke, arg0, parametersForValidation[0], "expression", "arg0");
			arg1 = Expression.ValidateOneArgument(invokeMethod, ExpressionType.Invoke, arg1, parametersForValidation[1], "expression", "arg1");
			arg2 = Expression.ValidateOneArgument(invokeMethod, ExpressionType.Invoke, arg2, parametersForValidation[2], "expression", "arg2");
			arg3 = Expression.ValidateOneArgument(invokeMethod, ExpressionType.Invoke, arg3, parametersForValidation[3], "expression", "arg3");
			arg4 = Expression.ValidateOneArgument(invokeMethod, ExpressionType.Invoke, arg4, parametersForValidation[4], "expression", "arg4");
			return new InvocationExpression5(expression, invokeMethod.ReturnType, arg0, arg1, arg2, arg3, arg4);
		}

		/// <summary>Creates an <see cref="T:System.Linq.Expressions.InvocationExpression" /> that applies a delegate or lambda expression to a list of argument expressions.</summary>
		/// <param name="expression">An <see cref="T:System.Linq.Expressions.Expression" /> that represents the delegate or lambda expression to be applied.</param>
		/// <param name="arguments">An array of <see cref="T:System.Linq.Expressions.Expression" /> objects that represent the arguments that the delegate or lambda expression is applied to.</param>
		/// <returns>An <see cref="T:System.Linq.Expressions.InvocationExpression" /> that applies the specified delegate or lambda expression to the provided arguments.</returns>
		/// <exception cref="T:System.ArgumentNullException">
		///         <paramref name="expression" /> is <see langword="null" />.</exception>
		/// <exception cref="T:System.ArgumentException">
		///         <paramref name="expression" />.Type does not represent a delegate type or an <see cref="T:System.Linq.Expressions.Expression`1" />.-or-The <see cref="P:System.Linq.Expressions.Expression.Type" /> property of an element of <paramref name="arguments" /> is not assignable to the type of the corresponding parameter of the delegate represented by <paramref name="expression" />.</exception>
		/// <exception cref="T:System.InvalidOperationException">
		///         <paramref name="arguments" /> does not contain the same number of elements as the list of parameters for the delegate represented by <paramref name="expression" />.</exception>
		public static InvocationExpression Invoke(Expression expression, params Expression[] arguments)
		{
			return Expression.Invoke(expression, arguments);
		}

		/// <summary>Creates an <see cref="T:System.Linq.Expressions.InvocationExpression" /> that applies a delegate or lambda expression to a list of argument expressions.</summary>
		/// <param name="expression">An <see cref="T:System.Linq.Expressions.Expression" /> that represents the delegate or lambda expression to be applied to.</param>
		/// <param name="arguments">An <see cref="T:System.Collections.Generic.IEnumerable`1" /> that contains <see cref="T:System.Linq.Expressions.Expression" /> objects that represent the arguments that the delegate or lambda expression is applied to.</param>
		/// <returns>An <see cref="T:System.Linq.Expressions.InvocationExpression" /> that applies the specified delegate or lambda expression to the provided arguments.</returns>
		/// <exception cref="T:System.ArgumentNullException">
		///         <paramref name="expression" /> is <see langword="null" />.</exception>
		/// <exception cref="T:System.ArgumentException">
		///         <paramref name="expression" />.Type does not represent a delegate type or an <see cref="T:System.Linq.Expressions.Expression`1" />.-or-The <see cref="P:System.Linq.Expressions.Expression.Type" /> property of an element of <paramref name="arguments" /> is not assignable to the type of the corresponding parameter of the delegate represented by <paramref name="expression" />.</exception>
		/// <exception cref="T:System.InvalidOperationException">
		///         <paramref name="arguments" /> does not contain the same number of elements as the list of parameters for the delegate represented by <paramref name="expression" />.</exception>
		public static InvocationExpression Invoke(Expression expression, IEnumerable<Expression> arguments)
		{
			IReadOnlyList<Expression> readOnlyList = (arguments as IReadOnlyList<Expression>) ?? arguments.ToReadOnly<Expression>();
			switch (readOnlyList.Count)
			{
			case 0:
				return Expression.Invoke(expression);
			case 1:
				return Expression.Invoke(expression, readOnlyList[0]);
			case 2:
				return Expression.Invoke(expression, readOnlyList[0], readOnlyList[1]);
			case 3:
				return Expression.Invoke(expression, readOnlyList[0], readOnlyList[1], readOnlyList[2]);
			case 4:
				return Expression.Invoke(expression, readOnlyList[0], readOnlyList[1], readOnlyList[2], readOnlyList[3]);
			case 5:
				return Expression.Invoke(expression, readOnlyList[0], readOnlyList[1], readOnlyList[2], readOnlyList[3], readOnlyList[4]);
			default:
			{
				ExpressionUtils.RequiresCanRead(expression, "expression");
				ReadOnlyCollection<Expression> arguments2 = readOnlyList.ToReadOnly<Expression>();
				MethodInfo invokeMethod = Expression.GetInvokeMethod(expression);
				Expression.ValidateArgumentTypes(invokeMethod, ExpressionType.Invoke, ref arguments2, "expression");
				return new InvocationExpressionN(expression, arguments2, invokeMethod.ReturnType);
			}
			}
		}

		internal static MethodInfo GetInvokeMethod(Expression expression)
		{
			Type delegateType = expression.Type;
			if (!expression.Type.IsSubclassOf(typeof(MulticastDelegate)))
			{
				Type type = TypeUtils.FindGenericType(typeof(Expression<>), expression.Type);
				if (type == null)
				{
					throw Error.ExpressionTypeNotInvocable(expression.Type, "expression");
				}
				delegateType = type.GetGenericArguments()[0];
			}
			return delegateType.GetInvokeMethod();
		}

		/// <summary>Creates a <see cref="T:System.Linq.Expressions.LabelExpression" /> representing a label without a default value.</summary>
		/// <param name="target">The <see cref="T:System.Linq.Expressions.LabelTarget" /> which this <see cref="T:System.Linq.Expressions.LabelExpression" /> will be associated with.</param>
		/// <returns>A <see cref="T:System.Linq.Expressions.LabelExpression" /> without a default value.</returns>
		public static LabelExpression Label(LabelTarget target)
		{
			return Expression.Label(target, null);
		}

		/// <summary>Creates a <see cref="T:System.Linq.Expressions.LabelExpression" /> representing a label with the given default value.</summary>
		/// <param name="target">The <see cref="T:System.Linq.Expressions.LabelTarget" /> which this <see cref="T:System.Linq.Expressions.LabelExpression" /> will be associated with.</param>
		/// <param name="defaultValue">The value of this <see cref="T:System.Linq.Expressions.LabelExpression" /> when the label is reached through regular control flow.</param>
		/// <returns>A <see cref="T:System.Linq.Expressions.LabelExpression" /> with the given default value.</returns>
		public static LabelExpression Label(LabelTarget target, Expression defaultValue)
		{
			Expression.ValidateGoto(target, ref defaultValue, "target", "defaultValue", null);
			return new LabelExpression(target, defaultValue);
		}

		/// <summary>Creates a <see cref="T:System.Linq.Expressions.LabelTarget" /> representing a label with void type and no name.</summary>
		/// <returns>The new <see cref="T:System.Linq.Expressions.LabelTarget" />.</returns>
		public static LabelTarget Label()
		{
			return Expression.Label(typeof(void), null);
		}

		/// <summary>Creates a <see cref="T:System.Linq.Expressions.LabelTarget" /> representing a label with void type and the given name.</summary>
		/// <param name="name">The name of the label.</param>
		/// <returns>The new <see cref="T:System.Linq.Expressions.LabelTarget" />.</returns>
		public static LabelTarget Label(string name)
		{
			return Expression.Label(typeof(void), name);
		}

		/// <summary>Creates a <see cref="T:System.Linq.Expressions.LabelTarget" /> representing a label with the given type.</summary>
		/// <param name="type">The type of value that is passed when jumping to the label.</param>
		/// <returns>The new <see cref="T:System.Linq.Expressions.LabelTarget" />.</returns>
		public static LabelTarget Label(Type type)
		{
			return Expression.Label(type, null);
		}

		/// <summary>Creates a <see cref="T:System.Linq.Expressions.LabelTarget" /> representing a label with the given type and name.</summary>
		/// <param name="type">The type of value that is passed when jumping to the label.</param>
		/// <param name="name">The name of the label.</param>
		/// <returns>The new <see cref="T:System.Linq.Expressions.LabelTarget" />.</returns>
		public static LabelTarget Label(Type type, string name)
		{
			ContractUtils.RequiresNotNull(type, "type");
			TypeUtils.ValidateType(type, "type");
			return new LabelTarget(type, name);
		}

		internal static LambdaExpression CreateLambda(Type delegateType, Expression body, string name, bool tailCall, ReadOnlyCollection<ParameterExpression> parameters)
		{
			CacheDict<Type, Func<Expression, string, bool, ReadOnlyCollection<ParameterExpression>, LambdaExpression>> cacheDict = Expression.s_lambdaFactories;
			if (cacheDict == null)
			{
				cacheDict = (Expression.s_lambdaFactories = new CacheDict<Type, Func<Expression, string, bool, ReadOnlyCollection<ParameterExpression>, LambdaExpression>>(50));
			}
			Func<Expression, string, bool, ReadOnlyCollection<ParameterExpression>, LambdaExpression> func;
			if (!cacheDict.TryGetValue(delegateType, out func))
			{
				MethodInfo method = typeof(Expression<>).MakeGenericType(new Type[]
				{
					delegateType
				}).GetMethod("Create", BindingFlags.Static | BindingFlags.NonPublic);
				if (delegateType.IsCollectible)
				{
					return (LambdaExpression)method.Invoke(null, new object[]
					{
						body,
						name,
						tailCall,
						parameters
					});
				}
				func = (cacheDict[delegateType] = (Func<Expression, string, bool, ReadOnlyCollection<ParameterExpression>, LambdaExpression>)method.CreateDelegate(typeof(Func<Expression, string, bool, ReadOnlyCollection<ParameterExpression>, LambdaExpression>)));
			}
			return func(body, name, tailCall, parameters);
		}

		/// <summary>Creates an <see cref="T:System.Linq.Expressions.Expression`1" /> where the delegate type is known at compile time.</summary>
		/// <param name="body">An <see cref="T:System.Linq.Expressions.Expression" /> to set the <see cref="P:System.Linq.Expressions.LambdaExpression.Body" /> property equal to.</param>
		/// <param name="parameters">An array of <see cref="T:System.Linq.Expressions.ParameterExpression" /> objects to use to populate the <see cref="P:System.Linq.Expressions.LambdaExpression.Parameters" /> collection.</param>
		/// <typeparam name="TDelegate">A delegate type.</typeparam>
		/// <returns>An <see cref="T:System.Linq.Expressions.Expression`1" /> that has the <see cref="P:System.Linq.Expressions.Expression.NodeType" /> property equal to <see cref="F:System.Linq.Expressions.ExpressionType.Lambda" /> and the <see cref="P:System.Linq.Expressions.LambdaExpression.Body" /> and <see cref="P:System.Linq.Expressions.LambdaExpression.Parameters" /> properties set to the specified values.</returns>
		/// <exception cref="T:System.ArgumentNullException">
		///         <paramref name="body" /> is <see langword="null" />.-or-One or more elements in <paramref name="parameters" /> are <see langword="null" />.</exception>
		/// <exception cref="T:System.ArgumentException">
		///         <paramref name="TDelegate" /> is not a delegate type.-or-
		///         <paramref name="body" />.Type represents a type that is not assignable to the return type of <paramref name="TDelegate" />.-or-
		///         <paramref name="parameters" /> does not contain the same number of elements as the list of parameters for <paramref name="TDelegate" />.-or-The <see cref="P:System.Linq.Expressions.Expression.Type" /> property of an element of <paramref name="parameters" /> is not assignable from the type of the corresponding parameter type of <paramref name="TDelegate" />.</exception>
		public static Expression<TDelegate> Lambda<TDelegate>(Expression body, params ParameterExpression[] parameters)
		{
			return Expression.Lambda<TDelegate>(body, false, parameters);
		}

		/// <summary>Creates an <see cref="T:System.Linq.Expressions.Expression`1" /> where the delegate type is known at compile time.</summary>
		/// <param name="body">An <see cref="T:System.Linq.Expressions.Expression" /> to set the <see cref="P:System.Linq.Expressions.LambdaExpression.Body" /> property equal to.</param>
		/// <param name="tailCall">A <see cref="T:System.Boolean" /> that indicates if tail call optimization will be applied when compiling the created expression.</param>
		/// <param name="parameters">An array that contains <see cref="T:System.Linq.Expressions.ParameterExpression" /> objects to use to populate the <see cref="P:System.Linq.Expressions.LambdaExpression.Parameters" /> collection.</param>
		/// <typeparam name="TDelegate">The delegate type. </typeparam>
		/// <returns>An <see cref="T:System.Linq.Expressions.Expression`1" /> that has the <see cref="P:System.Linq.Expressions.Expression.NodeType" /> property equal to <see cref="F:System.Linq.Expressions.ExpressionType.Lambda" /> and the <see cref="P:System.Linq.Expressions.LambdaExpression.Body" /> and <see cref="P:System.Linq.Expressions.LambdaExpression.Parameters" /> properties set to the specified values.</returns>
		public static Expression<TDelegate> Lambda<TDelegate>(Expression body, bool tailCall, params ParameterExpression[] parameters)
		{
			return Expression.Lambda<TDelegate>(body, tailCall, parameters);
		}

		/// <summary>Creates an <see cref="T:System.Linq.Expressions.Expression`1" /> where the delegate type is known at compile time.</summary>
		/// <param name="body">An <see cref="T:System.Linq.Expressions.Expression" /> to set the <see cref="P:System.Linq.Expressions.LambdaExpression.Body" /> property equal to.</param>
		/// <param name="parameters">An <see cref="T:System.Collections.Generic.IEnumerable`1" /> that contains <see cref="T:System.Linq.Expressions.ParameterExpression" /> objects to use to populate the <see cref="P:System.Linq.Expressions.LambdaExpression.Parameters" /> collection.</param>
		/// <typeparam name="TDelegate">A delegate type.</typeparam>
		/// <returns>An <see cref="T:System.Linq.Expressions.Expression`1" /> that has the <see cref="P:System.Linq.Expressions.Expression.NodeType" /> property equal to <see cref="F:System.Linq.Expressions.ExpressionType.Lambda" /> and the <see cref="P:System.Linq.Expressions.LambdaExpression.Body" /> and <see cref="P:System.Linq.Expressions.LambdaExpression.Parameters" /> properties set to the specified values.</returns>
		/// <exception cref="T:System.ArgumentNullException">
		///         <paramref name="body" /> is <see langword="null" />.-or-One or more elements in <paramref name="parameters" /> are <see langword="null" />.</exception>
		/// <exception cref="T:System.ArgumentException">
		///         <paramref name="TDelegate" /> is not a delegate type.-or-
		///         <paramref name="body" />.Type represents a type that is not assignable to the return type of <paramref name="TDelegate" />.-or-
		///         <paramref name="parameters" /> does not contain the same number of elements as the list of parameters for <paramref name="TDelegate" />.-or-The <see cref="P:System.Linq.Expressions.Expression.Type" /> property of an element of <paramref name="parameters" /> is not assignable from the type of the corresponding parameter type of <paramref name="TDelegate" />.</exception>
		public static Expression<TDelegate> Lambda<TDelegate>(Expression body, IEnumerable<ParameterExpression> parameters)
		{
			return Expression.Lambda<TDelegate>(body, null, false, parameters);
		}

		/// <summary>Creates an <see cref="T:System.Linq.Expressions.Expression`1" /> where the delegate type is known at compile time.</summary>
		/// <param name="body">An <see cref="T:System.Linq.Expressions.Expression" /> to set the <see cref="P:System.Linq.Expressions.LambdaExpression.Body" /> property equal to.</param>
		/// <param name="tailCall">A <see cref="T:System.Boolean" /> that indicates if tail call optimization will be applied when compiling the created expression.</param>
		/// <param name="parameters">An <see cref="T:System.Collections.Generic.IEnumerable`1" /> that contains <see cref="T:System.Linq.Expressions.ParameterExpression" /> objects to use to populate the <see cref="P:System.Linq.Expressions.LambdaExpression.Parameters" /> collection.</param>
		/// <typeparam name="TDelegate">The delegate type. </typeparam>
		/// <returns>An <see cref="T:System.Linq.Expressions.Expression`1" /> that has the <see cref="P:System.Linq.Expressions.LambdaExpression.NodeType" /> property equal to <see cref="F:System.Linq.Expressions.ExpressionType.Lambda" /> and the <see cref="P:System.Linq.Expressions.LambdaExpression.Body" /> and <see cref="P:System.Linq.Expressions.LambdaExpression.Parameters" /> properties set to the specified values.</returns>
		public static Expression<TDelegate> Lambda<TDelegate>(Expression body, bool tailCall, IEnumerable<ParameterExpression> parameters)
		{
			return Expression.Lambda<TDelegate>(body, null, tailCall, parameters);
		}

		/// <summary>Creates an <see cref="T:System.Linq.Expressions.Expression`1" /> where the delegate type is known at compile time.</summary>
		/// <param name="body">An <see cref="T:System.Linq.Expressions.Expression" /> to set the <see cref="P:System.Linq.Expressions.LambdaExpression.Body" /> property equal to.</param>
		/// <param name="name">The name of the lambda. Used for generating debugging information.</param>
		/// <param name="parameters">An <see cref="T:System.Collections.Generic.IEnumerable`1" /> that contains <see cref="T:System.Linq.Expressions.ParameterExpression" /> objects to use to populate the <see cref="P:System.Linq.Expressions.LambdaExpression.Parameters" /> collection.</param>
		/// <typeparam name="TDelegate">The delegate type. </typeparam>
		/// <returns>An <see cref="T:System.Linq.Expressions.Expression`1" /> that has the <see cref="P:System.Linq.Expressions.Expression.NodeType" /> property equal to <see cref="F:System.Linq.Expressions.ExpressionType.Lambda" /> and the <see cref="P:System.Linq.Expressions.LambdaExpression.Body" /> and <see cref="P:System.Linq.Expressions.LambdaExpression.Parameters" /> properties set to the specified values.</returns>
		public static Expression<TDelegate> Lambda<TDelegate>(Expression body, string name, IEnumerable<ParameterExpression> parameters)
		{
			return Expression.Lambda<TDelegate>(body, name, false, parameters);
		}

		/// <summary>Creates an <see cref="T:System.Linq.Expressions.Expression`1" /> where the delegate type is known at compile time.</summary>
		/// <param name="body">An <see cref="T:System.Linq.Expressions.Expression" /> to set the <see cref="P:System.Linq.Expressions.LambdaExpression.Body" /> property equal to.</param>
		/// <param name="name">The name of the lambda. Used for generating debugging info.</param>
		/// <param name="tailCall">A <see cref="T:System.Boolean" /> that indicates if tail call optimization will be applied when compiling the created expression.</param>
		/// <param name="parameters">An <see cref="T:System.Collections.Generic.IEnumerable`1" /> that contains <see cref="T:System.Linq.Expressions.ParameterExpression" /> objects to use to populate the <see cref="P:System.Linq.Expressions.LambdaExpression.Parameters" /> collection.</param>
		/// <typeparam name="TDelegate">The delegate type. </typeparam>
		/// <returns>An <see cref="T:System.Linq.Expressions.Expression`1" /> that has the <see cref="P:System.Linq.Expressions.LambdaExpression.NodeType" /> property equal to <see cref="F:System.Linq.Expressions.ExpressionType.Lambda" /> and the <see cref="P:System.Linq.Expressions.LambdaExpression.Body" /> and <see cref="P:System.Linq.Expressions.LambdaExpression.Parameters" /> properties set to the specified values.</returns>
		public static Expression<TDelegate> Lambda<TDelegate>(Expression body, string name, bool tailCall, IEnumerable<ParameterExpression> parameters)
		{
			ReadOnlyCollection<ParameterExpression> parameters2 = parameters.ToReadOnly<ParameterExpression>();
			Expression.ValidateLambdaArgs(typeof(TDelegate), ref body, parameters2, "TDelegate");
			return (Expression<TDelegate>)Expression.CreateLambda(typeof(TDelegate), body, name, tailCall, parameters2);
		}

		/// <summary>Creates a <see cref="T:System.Linq.Expressions.LambdaExpression" /> by first constructing a delegate type.</summary>
		/// <param name="body">An <see cref="T:System.Linq.Expressions.Expression" /> to set the <see cref="P:System.Linq.Expressions.LambdaExpression.Body" /> property equal to.</param>
		/// <param name="parameters">An array of <see cref="T:System.Linq.Expressions.ParameterExpression" /> objects to use to populate the <see cref="P:System.Linq.Expressions.LambdaExpression.Parameters" /> collection.</param>
		/// <returns>A <see cref="T:System.Linq.Expressions.LambdaExpression" /> that has the <see cref="P:System.Linq.Expressions.Expression.NodeType" /> property equal to <see cref="F:System.Linq.Expressions.ExpressionType.Lambda" /> and the <see cref="P:System.Linq.Expressions.LambdaExpression.Body" /> and <see cref="P:System.Linq.Expressions.LambdaExpression.Parameters" /> properties set to the specified values.</returns>
		/// <exception cref="T:System.ArgumentNullException">
		///         <paramref name="body" /> is <see langword="null" />.-or-One or more elements of <paramref name="parameters" /> are <see langword="null" />.</exception>
		/// <exception cref="T:System.ArgumentException">
		///         <paramref name="parameters" /> contains more than sixteen elements.</exception>
		public static LambdaExpression Lambda(Expression body, params ParameterExpression[] parameters)
		{
			return Expression.Lambda(body, false, parameters);
		}

		/// <summary>Creates a LambdaExpression by first constructing a delegate type.</summary>
		/// <param name="body">An <see cref="T:System.Linq.Expressions.Expression" /> to set the <see cref="P:System.Linq.Expressions.LambdaExpression.Body" /> property equal to.</param>
		/// <param name="tailCall">A <see cref="T:System.Boolean" /> that indicates if tail call optimization will be applied when compiling the created expression.</param>
		/// <param name="parameters">An array that contains <see cref="T:System.Linq.Expressions.ParameterExpression" /> objects to use to populate the <see cref="P:System.Linq.Expressions.LambdaExpression.Parameters" /> collection.</param>
		/// <returns>A <see cref="T:System.Linq.Expressions.LambdaExpression" /> that has the <see cref="P:System.Linq.Expressions.LambdaExpression.NodeType" /> property equal to Lambda and the <see cref="P:System.Linq.Expressions.LambdaExpression.Body" /> and <see cref="P:System.Linq.Expressions.LambdaExpression.Parameters" /> properties set to the specified values.</returns>
		public static LambdaExpression Lambda(Expression body, bool tailCall, params ParameterExpression[] parameters)
		{
			return Expression.Lambda(body, tailCall, parameters);
		}

		/// <summary>Creates a LambdaExpression by first constructing a delegate type.</summary>
		/// <param name="body">An <see cref="T:System.Linq.Expressions.Expression" /> to set the <see cref="P:System.Linq.Expressions.LambdaExpression.Body" /> property equal to.</param>
		/// <param name="parameters">An <see cref="T:System.Collections.Generic.IEnumerable`1" /> that contains <see cref="T:System.Linq.Expressions.ParameterExpression" /> objects to use to populate the <see cref="P:System.Linq.Expressions.LambdaExpression.Parameters" /> collection.</param>
		/// <returns>A <see cref="T:System.Linq.Expressions.LambdaExpression" /> that has the <see cref="P:System.Linq.Expressions.LambdaExpression.NodeType" /> property equal to Lambda and the <see cref="P:System.Linq.Expressions.LambdaExpression.Body" /> and <see cref="P:System.Linq.Expressions.LambdaExpression.Parameters" /> properties set to the specified values.</returns>
		public static LambdaExpression Lambda(Expression body, IEnumerable<ParameterExpression> parameters)
		{
			return Expression.Lambda(body, null, false, parameters);
		}

		/// <summary>Creates a LambdaExpression by first constructing a delegate type.</summary>
		/// <param name="body">An <see cref="T:System.Linq.Expressions.Expression" /> to set the <see cref="P:System.Linq.Expressions.LambdaExpression.Body" /> property equal to.</param>
		/// <param name="tailCall">A <see cref="T:System.Boolean" /> that indicates if tail call optimization will be applied when compiling the created expression.</param>
		/// <param name="parameters">An <see cref="T:System.Collections.Generic.IEnumerable`1" /> that contains <see cref="T:System.Linq.Expressions.ParameterExpression" /> objects to use to populate the <see cref="P:System.Linq.Expressions.LambdaExpression.Parameters" /> collection.</param>
		/// <returns>A <see cref="T:System.Linq.Expressions.LambdaExpression" /> that has the <see cref="P:System.Linq.Expressions.LambdaExpression.NodeType" /> property equal to Lambda and the <see cref="P:System.Linq.Expressions.LambdaExpression.Body" /> and <see cref="P:System.Linq.Expressions.LambdaExpression.Parameters" /> properties set to the specified values.</returns>
		public static LambdaExpression Lambda(Expression body, bool tailCall, IEnumerable<ParameterExpression> parameters)
		{
			return Expression.Lambda(body, null, tailCall, parameters);
		}

		/// <summary>Creates a <see cref="T:System.Linq.Expressions.LambdaExpression" /> by first constructing a delegate type. It can be used when the delegate type is not known at compile time.</summary>
		/// <param name="delegateType">A <see cref="T:System.Type" /> that represents a delegate signature for the lambda.</param>
		/// <param name="body">An <see cref="T:System.Linq.Expressions.Expression" /> to set the <see cref="P:System.Linq.Expressions.LambdaExpression.Body" /> property equal to.</param>
		/// <param name="parameters">An array of <see cref="T:System.Linq.Expressions.ParameterExpression" /> objects to use to populate the <see cref="P:System.Linq.Expressions.LambdaExpression.Parameters" /> collection.</param>
		/// <returns>An object that represents a lambda expression which has the <see cref="P:System.Linq.Expressions.Expression.NodeType" /> property equal to <see cref="F:System.Linq.Expressions.ExpressionType.Lambda" /> and the <see cref="P:System.Linq.Expressions.LambdaExpression.Body" /> and <see cref="P:System.Linq.Expressions.LambdaExpression.Parameters" /> properties set to the specified values.</returns>
		/// <exception cref="T:System.ArgumentNullException">
		///         <paramref name="delegateType" /> or <paramref name="body" /> is <see langword="null" />.-or-One or more elements in <paramref name="parameters" /> are <see langword="null" />.</exception>
		/// <exception cref="T:System.ArgumentException">
		///         <paramref name="delegateType" /> does not represent a delegate type.-or-
		///         <paramref name="body" />.Type represents a type that is not assignable to the return type of the delegate type represented by <paramref name="delegateType" />.-or-
		///         <paramref name="parameters" /> does not contain the same number of elements as the list of parameters for the delegate type represented by <paramref name="delegateType" />.-or-The <see cref="P:System.Linq.Expressions.Expression.Type" /> property of an element of <paramref name="parameters" /> is not assignable from the type of the corresponding parameter type of the delegate type represented by <paramref name="delegateType" />.</exception>
		public static LambdaExpression Lambda(Type delegateType, Expression body, params ParameterExpression[] parameters)
		{
			return Expression.Lambda(delegateType, body, null, false, parameters);
		}

		/// <summary>Creates a LambdaExpression by first constructing a delegate type.</summary>
		/// <param name="delegateType">A <see cref="P:System.Linq.Expressions.Expression.Type" /> representing the delegate signature for the lambda.</param>
		/// <param name="body">An <see cref="T:System.Linq.Expressions.Expression" /> to set the <see cref="P:System.Linq.Expressions.LambdaExpression.Body" /> property equal to.</param>
		/// <param name="tailCall">A <see cref="T:System.Boolean" /> that indicates if tail call optimization will be applied when compiling the created expression.</param>
		/// <param name="parameters">An array that contains <see cref="T:System.Linq.Expressions.ParameterExpression" /> objects to use to populate the <see cref="P:System.Linq.Expressions.LambdaExpression.Parameters" /> collection.</param>
		/// <returns>A <see cref="T:System.Linq.Expressions.LambdaExpression" /> that has the <see cref="P:System.Linq.Expressions.LambdaExpression.NodeType" /> property equal to Lambda and the <see cref="P:System.Linq.Expressions.LambdaExpression.Body" /> and <see cref="P:System.Linq.Expressions.LambdaExpression.Parameters" /> properties set to the specified values.</returns>
		public static LambdaExpression Lambda(Type delegateType, Expression body, bool tailCall, params ParameterExpression[] parameters)
		{
			return Expression.Lambda(delegateType, body, null, tailCall, parameters);
		}

		/// <summary>Creates a <see cref="T:System.Linq.Expressions.LambdaExpression" /> by first constructing a delegate type. It can be used when the delegate type is not known at compile time.</summary>
		/// <param name="delegateType">A <see cref="T:System.Type" /> that represents a delegate signature for the lambda.</param>
		/// <param name="body">An <see cref="T:System.Linq.Expressions.Expression" /> to set the <see cref="P:System.Linq.Expressions.LambdaExpression.Body" /> property equal to.</param>
		/// <param name="parameters">An <see cref="T:System.Collections.Generic.IEnumerable`1" /> that contains <see cref="T:System.Linq.Expressions.ParameterExpression" /> objects to use to populate the <see cref="P:System.Linq.Expressions.LambdaExpression.Parameters" /> collection.</param>
		/// <returns>An object that represents a lambda expression which has the <see cref="P:System.Linq.Expressions.Expression.NodeType" /> property equal to <see cref="F:System.Linq.Expressions.ExpressionType.Lambda" /> and the <see cref="P:System.Linq.Expressions.LambdaExpression.Body" /> and <see cref="P:System.Linq.Expressions.LambdaExpression.Parameters" /> properties set to the specified values.</returns>
		/// <exception cref="T:System.ArgumentNullException">
		///         <paramref name="delegateType" /> or <paramref name="body" /> is <see langword="null" />.-or-One or more elements in <paramref name="parameters" /> are <see langword="null" />.</exception>
		/// <exception cref="T:System.ArgumentException">
		///         <paramref name="delegateType" /> does not represent a delegate type.-or-
		///         <paramref name="body" />.Type represents a type that is not assignable to the return type of the delegate type represented by <paramref name="delegateType" />.-or-
		///         <paramref name="parameters" /> does not contain the same number of elements as the list of parameters for the delegate type represented by <paramref name="delegateType" />.-or-The <see cref="P:System.Linq.Expressions.Expression.Type" /> property of an element of <paramref name="parameters" /> is not assignable from the type of the corresponding parameter type of the delegate type represented by <paramref name="delegateType" />.</exception>
		public static LambdaExpression Lambda(Type delegateType, Expression body, IEnumerable<ParameterExpression> parameters)
		{
			return Expression.Lambda(delegateType, body, null, false, parameters);
		}

		/// <summary>Creates a LambdaExpression by first constructing a delegate type.</summary>
		/// <param name="delegateType">A <see cref="P:System.Linq.Expressions.Expression.Type" /> representing the delegate signature for the lambda.</param>
		/// <param name="body">An <see cref="T:System.Linq.Expressions.Expression" /> to set the <see cref="P:System.Linq.Expressions.LambdaExpression.Body" /> property equal to.</param>
		/// <param name="tailCall">A <see cref="T:System.Boolean" /> that indicates if tail call optimization will be applied when compiling the created expression.</param>
		/// <param name="parameters">An <see cref="T:System.Collections.Generic.IEnumerable`1" /> that contains <see cref="T:System.Linq.Expressions.ParameterExpression" /> objects to use to populate the <see cref="P:System.Linq.Expressions.LambdaExpression.Parameters" /> collection.</param>
		/// <returns>A <see cref="T:System.Linq.Expressions.LambdaExpression" /> that has the <see cref="P:System.Linq.Expressions.LambdaExpression.NodeType" /> property equal to Lambda and the <see cref="P:System.Linq.Expressions.LambdaExpression.Body" /> and <see cref="P:System.Linq.Expressions.LambdaExpression.Parameters" /> properties set to the specified values.</returns>
		public static LambdaExpression Lambda(Type delegateType, Expression body, bool tailCall, IEnumerable<ParameterExpression> parameters)
		{
			return Expression.Lambda(delegateType, body, null, tailCall, parameters);
		}

		/// <summary>Creates a LambdaExpression by first constructing a delegate type.</summary>
		/// <param name="body">An <see cref="T:System.Linq.Expressions.Expression" /> to set the <see cref="P:System.Linq.Expressions.LambdaExpression.Body" /> property equal to.</param>
		/// <param name="name">The name for the lambda. Used for emitting debug information.</param>
		/// <param name="parameters">An <see cref="T:System.Collections.Generic.IEnumerable`1" /> that contains <see cref="T:System.Linq.Expressions.ParameterExpression" /> objects to use to populate the <see cref="P:System.Linq.Expressions.LambdaExpression.Parameters" /> collection.</param>
		/// <returns>A <see cref="T:System.Linq.Expressions.LambdaExpression" /> that has the <see cref="P:System.Linq.Expressions.LambdaExpression.NodeType" /> property equal to Lambda and the <see cref="P:System.Linq.Expressions.LambdaExpression.Body" /> and <see cref="P:System.Linq.Expressions.LambdaExpression.Parameters" /> properties set to the specified values.</returns>
		public static LambdaExpression Lambda(Expression body, string name, IEnumerable<ParameterExpression> parameters)
		{
			return Expression.Lambda(body, name, false, parameters);
		}

		/// <summary>Creates a LambdaExpression by first constructing a delegate type.</summary>
		/// <param name="body">An <see cref="T:System.Linq.Expressions.Expression" /> to set the <see cref="P:System.Linq.Expressions.LambdaExpression.Body" /> property equal to.</param>
		/// <param name="name">The name for the lambda. Used for emitting debug information.</param>
		/// <param name="tailCall">A <see cref="T:System.Boolean" /> that indicates if tail call optimization will be applied when compiling the created expression.</param>
		/// <param name="parameters">An <see cref="T:System.Collections.Generic.IEnumerable`1" /> that contains <see cref="T:System.Linq.Expressions.ParameterExpression" /> objects to use to populate the <see cref="P:System.Linq.Expressions.LambdaExpression.Parameters" /> collection.</param>
		/// <returns>A <see cref="T:System.Linq.Expressions.LambdaExpression" /> that has the <see cref="P:System.Linq.Expressions.LambdaExpression.NodeType" /> property equal to Lambda and the <see cref="P:System.Linq.Expressions.LambdaExpression.Body" /> and <see cref="P:System.Linq.Expressions.LambdaExpression.Parameters" /> properties set to the specified values.</returns>
		public static LambdaExpression Lambda(Expression body, string name, bool tailCall, IEnumerable<ParameterExpression> parameters)
		{
			ContractUtils.RequiresNotNull(body, "body");
			ReadOnlyCollection<ParameterExpression> readOnlyCollection = parameters.ToReadOnly<ParameterExpression>();
			int count = readOnlyCollection.Count;
			Type[] array = new Type[count + 1];
			if (count > 0)
			{
				HashSet<ParameterExpression> hashSet = new HashSet<ParameterExpression>();
				for (int i = 0; i < count; i++)
				{
					ParameterExpression parameterExpression = readOnlyCollection[i];
					ContractUtils.RequiresNotNull(parameterExpression, "parameter");
					array[i] = (parameterExpression.IsByRef ? parameterExpression.Type.MakeByRefType() : parameterExpression.Type);
					if (!hashSet.Add(parameterExpression))
					{
						throw Error.DuplicateVariable(parameterExpression, "parameters", i);
					}
				}
			}
			array[count] = body.Type;
			return Expression.CreateLambda(DelegateHelpers.MakeDelegateType(array), body, name, tailCall, readOnlyCollection);
		}

		/// <summary>Creates a LambdaExpression by first constructing a delegate type.</summary>
		/// <param name="delegateType">A <see cref="P:System.Linq.Expressions.Expression.Type" /> representing the delegate signature for the lambda.</param>
		/// <param name="body">An <see cref="T:System.Linq.Expressions.Expression" /> to set the <see cref="P:System.Linq.Expressions.LambdaExpression.Body" /> property equal to.</param>
		/// <param name="name">The name for the lambda. Used for emitting debug information.</param>
		/// <param name="parameters">An <see cref="T:System.Collections.Generic.IEnumerable`1" /> that contains <see cref="T:System.Linq.Expressions.ParameterExpression" /> objects to use to populate the <see cref="P:System.Linq.Expressions.LambdaExpression.Parameters" /> collection.</param>
		/// <returns>A <see cref="T:System.Linq.Expressions.LambdaExpression" /> that has the <see cref="P:System.Linq.Expressions.LambdaExpression.NodeType" /> property equal to Lambda and the <see cref="P:System.Linq.Expressions.LambdaExpression.Body" /> and <see cref="P:System.Linq.Expressions.LambdaExpression.Parameters" /> properties set to the specified values.</returns>
		public static LambdaExpression Lambda(Type delegateType, Expression body, string name, IEnumerable<ParameterExpression> parameters)
		{
			ReadOnlyCollection<ParameterExpression> parameters2 = parameters.ToReadOnly<ParameterExpression>();
			Expression.ValidateLambdaArgs(delegateType, ref body, parameters2, "delegateType");
			return Expression.CreateLambda(delegateType, body, name, false, parameters2);
		}

		/// <summary>Creates a LambdaExpression by first constructing a delegate type.</summary>
		/// <param name="delegateType">A <see cref="P:System.Linq.Expressions.Expression.Type" /> representing the delegate signature for the lambda.</param>
		/// <param name="body">An <see cref="T:System.Linq.Expressions.Expression" /> to set the <see cref="P:System.Linq.Expressions.LambdaExpression.Body" /> property equal to. </param>
		/// <param name="name">The name for the lambda. Used for emitting debug information.</param>
		/// <param name="tailCall">A <see cref="T:System.Boolean" /> that indicates if tail call optimization will be applied when compiling the created expression. </param>
		/// <param name="parameters">An <see cref="T:System.Collections.Generic.IEnumerable`1" /> that contains <see cref="T:System.Linq.Expressions.ParameterExpression" /> objects to use to populate the <see cref="P:System.Linq.Expressions.LambdaExpression.Parameters" /> collection. </param>
		/// <returns>A <see cref="T:System.Linq.Expressions.LambdaExpression" /> that has the <see cref="P:System.Linq.Expressions.LambdaExpression.NodeType" /> property equal to Lambda and the <see cref="P:System.Linq.Expressions.LambdaExpression.Body" /> and <see cref="P:System.Linq.Expressions.LambdaExpression.Parameters" /> properties set to the specified values.</returns>
		public static LambdaExpression Lambda(Type delegateType, Expression body, string name, bool tailCall, IEnumerable<ParameterExpression> parameters)
		{
			ReadOnlyCollection<ParameterExpression> parameters2 = parameters.ToReadOnly<ParameterExpression>();
			Expression.ValidateLambdaArgs(delegateType, ref body, parameters2, "delegateType");
			return Expression.CreateLambda(delegateType, body, name, tailCall, parameters2);
		}

		private static void ValidateLambdaArgs(Type delegateType, ref Expression body, ReadOnlyCollection<ParameterExpression> parameters, string paramName)
		{
			ContractUtils.RequiresNotNull(delegateType, "delegateType");
			ExpressionUtils.RequiresCanRead(body, "body");
			if (!typeof(MulticastDelegate).IsAssignableFrom(delegateType) || delegateType == typeof(MulticastDelegate))
			{
				throw Error.LambdaTypeMustBeDerivedFromSystemDelegate(paramName);
			}
			TypeUtils.ValidateType(delegateType, "delegateType", true, true);
			CacheDict<Type, MethodInfo> cacheDict = Expression.s_lambdaDelegateCache;
			MethodInfo invokeMethod;
			if (!cacheDict.TryGetValue(delegateType, out invokeMethod))
			{
				invokeMethod = delegateType.GetInvokeMethod();
				if (!delegateType.IsCollectible)
				{
					cacheDict[delegateType] = invokeMethod;
				}
			}
			ParameterInfo[] parametersCached = invokeMethod.GetParametersCached();
			if (parametersCached.Length != 0)
			{
				if (parametersCached.Length != parameters.Count)
				{
					throw Error.IncorrectNumberOfLambdaDeclarationParameters();
				}
				HashSet<ParameterExpression> hashSet = new HashSet<ParameterExpression>();
				int i = 0;
				int num = parametersCached.Length;
				while (i < num)
				{
					ParameterExpression parameterExpression = parameters[i];
					ParameterInfo parameterInfo = parametersCached[i];
					ExpressionUtils.RequiresCanRead(parameterExpression, "parameters", i);
					Type type = parameterInfo.ParameterType;
					if (parameterExpression.IsByRef)
					{
						if (!type.IsByRef)
						{
							throw Error.ParameterExpressionNotValidAsDelegate(parameterExpression.Type.MakeByRefType(), type);
						}
						type = type.GetElementType();
					}
					if (!TypeUtils.AreReferenceAssignable(parameterExpression.Type, type))
					{
						throw Error.ParameterExpressionNotValidAsDelegate(parameterExpression.Type, type);
					}
					if (!hashSet.Add(parameterExpression))
					{
						throw Error.DuplicateVariable(parameterExpression, "parameters", i);
					}
					i++;
				}
			}
			else if (parameters.Count > 0)
			{
				throw Error.IncorrectNumberOfLambdaDeclarationParameters();
			}
			if (invokeMethod.ReturnType != typeof(void) && !TypeUtils.AreReferenceAssignable(invokeMethod.ReturnType, body.Type) && !Expression.TryQuote(invokeMethod.ReturnType, ref body))
			{
				throw Error.ExpressionTypeDoesNotMatchReturn(body.Type, invokeMethod.ReturnType);
			}
		}

		private static Expression.TryGetFuncActionArgsResult ValidateTryGetFuncActionArgs(Type[] typeArgs)
		{
			if (typeArgs == null)
			{
				return Expression.TryGetFuncActionArgsResult.ArgumentNull;
			}
			foreach (Type type in typeArgs)
			{
				if (type == null)
				{
					return Expression.TryGetFuncActionArgsResult.ArgumentNull;
				}
				if (type.IsByRef)
				{
					return Expression.TryGetFuncActionArgsResult.ByRef;
				}
				if (type == typeof(void) || type.IsPointer)
				{
					return Expression.TryGetFuncActionArgsResult.PointerOrVoid;
				}
			}
			return Expression.TryGetFuncActionArgsResult.Valid;
		}

		/// <summary>Creates a <see cref="P:System.Linq.Expressions.Expression.Type" /> object that represents a generic System.Func delegate type that has specific type arguments. The last type argument specifies the return type of the created delegate.</summary>
		/// <param name="typeArgs">An array of one to seventeen <see cref="T:System.Type" /> objects that specify the type arguments for the <see langword="System.Func" /> delegate type.</param>
		/// <returns>The type of a System.Func delegate that has the specified type arguments.</returns>
		/// <exception cref="T:System.ArgumentException">
		///         <paramref name="typeArgs" /> contains fewer than one or more than seventeen elements.</exception>
		/// <exception cref="T:System.ArgumentNullException">
		///         <paramref name="typeArgs" /> is <see langword="null" />.</exception>
		public static Type GetFuncType(params Type[] typeArgs)
		{
			Expression.TryGetFuncActionArgsResult tryGetFuncActionArgsResult = Expression.ValidateTryGetFuncActionArgs(typeArgs);
			if (tryGetFuncActionArgsResult == Expression.TryGetFuncActionArgsResult.ArgumentNull)
			{
				throw new ArgumentNullException("typeArgs");
			}
			if (tryGetFuncActionArgsResult == Expression.TryGetFuncActionArgsResult.ByRef)
			{
				throw Error.TypeMustNotBeByRef("typeArgs");
			}
			Type funcType = DelegateHelpers.GetFuncType(typeArgs);
			if (funcType == null)
			{
				throw Error.IncorrectNumberOfTypeArgsForFunc("typeArgs");
			}
			return funcType;
		}

		/// <summary>Creates a <see cref="P:System.Linq.Expressions.Expression.Type" /> object that represents a generic System.Func delegate type that has specific type arguments. The last type argument specifies the return type of the created delegate.</summary>
		/// <param name="typeArgs">An array of Type objects that specify the type arguments for the System.Func delegate type.</param>
		/// <param name="funcType">When this method returns, contains the generic System.Func delegate type that has specific type arguments. Contains null if there is no generic System.Func delegate that matches the <paramref name="typeArgs" />.This parameter is passed uninitialized.</param>
		/// <returns>true if generic System.Func delegate type was created for specific <paramref name="typeArgs" />; false otherwise.</returns>
		public static bool TryGetFuncType(Type[] typeArgs, out Type funcType)
		{
			if (Expression.ValidateTryGetFuncActionArgs(typeArgs) == Expression.TryGetFuncActionArgsResult.Valid)
			{
				Type funcType2;
				funcType = (funcType2 = DelegateHelpers.GetFuncType(typeArgs));
				return funcType2 != null;
			}
			funcType = null;
			return false;
		}

		/// <summary>Creates a <see cref="T:System.Type" /> object that represents a generic System.Action delegate type that has specific type arguments.</summary>
		/// <param name="typeArgs">An array of up to sixteen <see cref="T:System.Type" /> objects that specify the type arguments for the <see langword="System.Action" /> delegate type.</param>
		/// <returns>The type of a System.Action delegate that has the specified type arguments.</returns>
		/// <exception cref="T:System.ArgumentException">
		///         <paramref name="typeArgs" /> contains more than sixteen elements.</exception>
		/// <exception cref="T:System.ArgumentNullException">
		///         <paramref name="typeArgs" /> is <see langword="null" />.</exception>
		public static Type GetActionType(params Type[] typeArgs)
		{
			Expression.TryGetFuncActionArgsResult tryGetFuncActionArgsResult = Expression.ValidateTryGetFuncActionArgs(typeArgs);
			if (tryGetFuncActionArgsResult == Expression.TryGetFuncActionArgsResult.ArgumentNull)
			{
				throw new ArgumentNullException("typeArgs");
			}
			if (tryGetFuncActionArgsResult == Expression.TryGetFuncActionArgsResult.ByRef)
			{
				throw Error.TypeMustNotBeByRef("typeArgs");
			}
			Type actionType = DelegateHelpers.GetActionType(typeArgs);
			if (actionType == null)
			{
				throw Error.IncorrectNumberOfTypeArgsForAction("typeArgs");
			}
			return actionType;
		}

		/// <summary>Creates a <see cref="P:System.Linq.Expressions.Expression.Type" /> object that represents a generic System.Action delegate type that has specific type arguments.</summary>
		/// <param name="typeArgs">An array of Type objects that specify the type arguments for the System.Action delegate type.</param>
		/// <param name="actionType">When this method returns, contains the generic System.Action delegate type that has specific type arguments. Contains null if there is no generic System.Action delegate that matches the <paramref name="typeArgs" />.This parameter is passed uninitialized.</param>
		/// <returns>true if generic System.Action delegate type was created for specific <paramref name="typeArgs" />; false otherwise.</returns>
		public static bool TryGetActionType(Type[] typeArgs, out Type actionType)
		{
			if (Expression.ValidateTryGetFuncActionArgs(typeArgs) == Expression.TryGetFuncActionArgsResult.Valid)
			{
				Type actionType2;
				actionType = (actionType2 = DelegateHelpers.GetActionType(typeArgs));
				return actionType2 != null;
			}
			actionType = null;
			return false;
		}

		/// <summary>Gets a <see cref="P:System.Linq.Expressions.Expression.Type" /> object that represents a generic System.Func or System.Action delegate type that has specific type arguments.</summary>
		/// <param name="typeArgs">The type arguments of the delegate.</param>
		/// <returns>The delegate type.</returns>
		public static Type GetDelegateType(params Type[] typeArgs)
		{
			ContractUtils.RequiresNotEmpty<Type>(typeArgs, "typeArgs");
			ContractUtils.RequiresNotNullItems<Type>(typeArgs, "typeArgs");
			return DelegateHelpers.MakeDelegateType(typeArgs);
		}

		/// <summary>Creates a <see cref="T:System.Linq.Expressions.ListInitExpression" /> that uses a method named "Add" to add elements to a collection.</summary>
		/// <param name="newExpression">A <see cref="T:System.Linq.Expressions.NewExpression" /> to set the <see cref="P:System.Linq.Expressions.ListInitExpression.NewExpression" /> property equal to.</param>
		/// <param name="initializers">An array of <see cref="T:System.Linq.Expressions.Expression" /> objects to use to populate the <see cref="P:System.Linq.Expressions.ListInitExpression.Initializers" /> collection.</param>
		/// <returns>A <see cref="T:System.Linq.Expressions.ListInitExpression" /> that has the <see cref="P:System.Linq.Expressions.Expression.NodeType" /> property equal to <see cref="F:System.Linq.Expressions.ExpressionType.ListInit" /> and the <see cref="P:System.Linq.Expressions.ListInitExpression.NewExpression" /> property set to the specified value.</returns>
		/// <exception cref="T:System.ArgumentNullException">
		///         <paramref name="newExpression" /> or <paramref name="initializers" /> is <see langword="null" />.-or-One or more elements of <paramref name="initializers" /> are <see langword="null" />.</exception>
		/// <exception cref="T:System.ArgumentException">
		///         <paramref name="newExpression" />.Type does not implement <see cref="T:System.Collections.IEnumerable" />.</exception>
		/// <exception cref="T:System.InvalidOperationException">There is no instance method named "Add" (case insensitive) declared in <paramref name="newExpression" />.Type or its base type.-or-The add method on <paramref name="newExpression" />.Type or its base type does not take exactly one argument.-or-The type represented by the <see cref="P:System.Linq.Expressions.Expression.Type" /> property of the first element of <paramref name="initializers" /> is not assignable to the argument type of the add method on <paramref name="newExpression" />.Type or its base type.-or-More than one argument-compatible method named "Add" (case-insensitive) exists on <paramref name="newExpression" />.Type and/or its base type.</exception>
		public static ListInitExpression ListInit(NewExpression newExpression, params Expression[] initializers)
		{
			return Expression.ListInit(newExpression, initializers);
		}

		/// <summary>Creates a <see cref="T:System.Linq.Expressions.ListInitExpression" /> that uses a method named "Add" to add elements to a collection.</summary>
		/// <param name="newExpression">A <see cref="T:System.Linq.Expressions.NewExpression" /> to set the <see cref="P:System.Linq.Expressions.ListInitExpression.NewExpression" /> property equal to.</param>
		/// <param name="initializers">An <see cref="T:System.Collections.Generic.IEnumerable`1" /> that contains <see cref="T:System.Linq.Expressions.Expression" /> objects to use to populate the <see cref="P:System.Linq.Expressions.ListInitExpression.Initializers" /> collection.</param>
		/// <returns>A <see cref="T:System.Linq.Expressions.ListInitExpression" /> that has the <see cref="P:System.Linq.Expressions.Expression.NodeType" /> property equal to <see cref="F:System.Linq.Expressions.ExpressionType.ListInit" /> and the <see cref="P:System.Linq.Expressions.ListInitExpression.NewExpression" /> property set to the specified value.</returns>
		/// <exception cref="T:System.ArgumentNullException">
		///         <paramref name="newExpression" /> or <paramref name="initializers" /> is <see langword="null" />.-or-One or more elements of <paramref name="initializers" /> are <see langword="null" />.</exception>
		/// <exception cref="T:System.ArgumentException">
		///         <paramref name="newExpression" />.Type does not implement <see cref="T:System.Collections.IEnumerable" />.</exception>
		/// <exception cref="T:System.InvalidOperationException">There is no instance method named "Add" (case insensitive) declared in <paramref name="newExpression" />.Type or its base type.-or-The add method on <paramref name="newExpression" />.Type or its base type does not take exactly one argument.-or-The type represented by the <see cref="P:System.Linq.Expressions.Expression.Type" /> property of the first element of <paramref name="initializers" /> is not assignable to the argument type of the add method on <paramref name="newExpression" />.Type or its base type.-or-More than one argument-compatible method named "Add" (case-insensitive) exists on <paramref name="newExpression" />.Type and/or its base type.</exception>
		public static ListInitExpression ListInit(NewExpression newExpression, IEnumerable<Expression> initializers)
		{
			ContractUtils.RequiresNotNull(newExpression, "newExpression");
			ContractUtils.RequiresNotNull(initializers, "initializers");
			ReadOnlyCollection<Expression> readOnlyCollection = initializers.ToReadOnly<Expression>();
			if (readOnlyCollection.Count == 0)
			{
				return new ListInitExpression(newExpression, EmptyReadOnlyCollection<System.Linq.Expressions.ElementInit>.Instance);
			}
			MethodInfo addMethod = Expression.FindMethod(newExpression.Type, "Add", null, new Expression[]
			{
				readOnlyCollection[0]
			}, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
			return Expression.ListInit(newExpression, addMethod, readOnlyCollection);
		}

		/// <summary>Creates a <see cref="T:System.Linq.Expressions.ListInitExpression" /> that uses a specified method to add elements to a collection.</summary>
		/// <param name="newExpression">A <see cref="T:System.Linq.Expressions.NewExpression" /> to set the <see cref="P:System.Linq.Expressions.ListInitExpression.NewExpression" /> property equal to.</param>
		/// <param name="addMethod">A <see cref="T:System.Reflection.MethodInfo" /> that represents an instance method that takes one argument, that adds an element to a collection.</param>
		/// <param name="initializers">An array of <see cref="T:System.Linq.Expressions.Expression" /> objects to use to populate the <see cref="P:System.Linq.Expressions.ListInitExpression.Initializers" /> collection.</param>
		/// <returns>A <see cref="T:System.Linq.Expressions.ListInitExpression" /> that has the <see cref="P:System.Linq.Expressions.Expression.NodeType" /> property equal to <see cref="F:System.Linq.Expressions.ExpressionType.ListInit" /> and the <see cref="P:System.Linq.Expressions.ListInitExpression.NewExpression" /> property set to the specified value.</returns>
		/// <exception cref="T:System.ArgumentNullException">
		///         <paramref name="newExpression" /> or <paramref name="initializers" /> is <see langword="null" />.-or-One or more elements of <paramref name="initializers" /> are <see langword="null" />.</exception>
		/// <exception cref="T:System.ArgumentException">
		///         <paramref name="newExpression" />.Type does not implement <see cref="T:System.Collections.IEnumerable" />.-or-
		///         <paramref name="addMethod" /> is not <see langword="null" /> and it does not represent an instance method named "Add" (case insensitive) that takes exactly one argument.-or-
		///         <paramref name="addMethod" /> is not <see langword="null" /> and the type represented by the <see cref="P:System.Linq.Expressions.Expression.Type" /> property of one or more elements of <paramref name="initializers" /> is not assignable to the argument type of the method that <paramref name="addMethod" /> represents.</exception>
		/// <exception cref="T:System.InvalidOperationException">
		///         <paramref name="addMethod" /> is <see langword="null" /> and no instance method named "Add" that takes one type-compatible argument exists on <paramref name="newExpression" />.Type or its base type.</exception>
		public static ListInitExpression ListInit(NewExpression newExpression, MethodInfo addMethod, params Expression[] initializers)
		{
			return Expression.ListInit(newExpression, addMethod, initializers);
		}

		/// <summary>Creates a <see cref="T:System.Linq.Expressions.ListInitExpression" /> that uses a specified method to add elements to a collection.</summary>
		/// <param name="newExpression">A <see cref="T:System.Linq.Expressions.NewExpression" /> to set the <see cref="P:System.Linq.Expressions.ListInitExpression.NewExpression" /> property equal to.</param>
		/// <param name="addMethod">A <see cref="T:System.Reflection.MethodInfo" /> that represents an instance method named "Add" (case insensitive), that adds an element to a collection.</param>
		/// <param name="initializers">An <see cref="T:System.Collections.Generic.IEnumerable`1" /> that contains <see cref="T:System.Linq.Expressions.Expression" /> objects to use to populate the <see cref="P:System.Linq.Expressions.ListInitExpression.Initializers" /> collection.</param>
		/// <returns>A <see cref="T:System.Linq.Expressions.ListInitExpression" /> that has the <see cref="P:System.Linq.Expressions.Expression.NodeType" /> property equal to <see cref="F:System.Linq.Expressions.ExpressionType.ListInit" /> and the <see cref="P:System.Linq.Expressions.ListInitExpression.NewExpression" /> property set to the specified value.</returns>
		/// <exception cref="T:System.ArgumentNullException">
		///         <paramref name="newExpression" /> or <paramref name="initializers" /> is <see langword="null" />.-or-One or more elements of <paramref name="initializers" /> are <see langword="null" />.</exception>
		/// <exception cref="T:System.ArgumentException">
		///         <paramref name="newExpression" />.Type does not implement <see cref="T:System.Collections.IEnumerable" />.-or-
		///         <paramref name="addMethod" /> is not <see langword="null" /> and it does not represent an instance method named "Add" (case insensitive) that takes exactly one argument.-or-
		///         <paramref name="addMethod" /> is not <see langword="null" /> and the type represented by the <see cref="P:System.Linq.Expressions.Expression.Type" /> property of one or more elements of <paramref name="initializers" /> is not assignable to the argument type of the method that <paramref name="addMethod" /> represents.</exception>
		/// <exception cref="T:System.InvalidOperationException">
		///         <paramref name="addMethod" /> is <see langword="null" /> and no instance method named "Add" that takes one type-compatible argument exists on <paramref name="newExpression" />.Type or its base type.</exception>
		public static ListInitExpression ListInit(NewExpression newExpression, MethodInfo addMethod, IEnumerable<Expression> initializers)
		{
			if (addMethod == null)
			{
				return Expression.ListInit(newExpression, initializers);
			}
			ContractUtils.RequiresNotNull(newExpression, "newExpression");
			ContractUtils.RequiresNotNull(initializers, "initializers");
			ReadOnlyCollection<Expression> readOnlyCollection = initializers.ToReadOnly<Expression>();
			ElementInit[] array = new ElementInit[readOnlyCollection.Count];
			for (int i = 0; i < readOnlyCollection.Count; i++)
			{
				array[i] = Expression.ElementInit(addMethod, new Expression[]
				{
					readOnlyCollection[i]
				});
			}
			return Expression.ListInit(newExpression, new TrueReadOnlyCollection<ElementInit>(array));
		}

		/// <summary>Creates a <see cref="T:System.Linq.Expressions.ListInitExpression" /> that uses specified <see cref="T:System.Linq.Expressions.ElementInit" /> objects to initialize a collection.</summary>
		/// <param name="newExpression">A <see cref="T:System.Linq.Expressions.NewExpression" /> to set the <see cref="P:System.Linq.Expressions.ListInitExpression.NewExpression" /> property equal to.</param>
		/// <param name="initializers">An array of <see cref="T:System.Linq.Expressions.ElementInit" /> objects to use to populate the <see cref="P:System.Linq.Expressions.ListInitExpression.Initializers" /> collection.</param>
		/// <returns>A <see cref="T:System.Linq.Expressions.ListInitExpression" /> that has the <see cref="P:System.Linq.Expressions.Expression.NodeType" /> property equal to <see cref="F:System.Linq.Expressions.ExpressionType.ListInit" /> and the <see cref="P:System.Linq.Expressions.ListInitExpression.NewExpression" /> and <see cref="P:System.Linq.Expressions.ListInitExpression.Initializers" /> properties set to the specified values.</returns>
		/// <exception cref="T:System.ArgumentNullException">
		///         <paramref name="newExpression" /> or <paramref name="initializers" /> is <see langword="null" />.-or-One or more elements of <paramref name="initializers" /> are <see langword="null" />.</exception>
		/// <exception cref="T:System.ArgumentException">
		///         <paramref name="newExpression" />.Type does not implement <see cref="T:System.Collections.IEnumerable" />.</exception>
		public static ListInitExpression ListInit(NewExpression newExpression, params ElementInit[] initializers)
		{
			return Expression.ListInit(newExpression, initializers);
		}

		/// <summary>Creates a <see cref="T:System.Linq.Expressions.ListInitExpression" /> that uses specified <see cref="T:System.Linq.Expressions.ElementInit" /> objects to initialize a collection.</summary>
		/// <param name="newExpression">A <see cref="T:System.Linq.Expressions.NewExpression" /> to set the <see cref="P:System.Linq.Expressions.ListInitExpression.NewExpression" /> property equal to.</param>
		/// <param name="initializers">An <see cref="T:System.Collections.Generic.IEnumerable`1" /> that contains <see cref="T:System.Linq.Expressions.ElementInit" /> objects to use to populate the <see cref="P:System.Linq.Expressions.ListInitExpression.Initializers" /> collection.</param>
		/// <returns>A <see cref="T:System.Linq.Expressions.ListInitExpression" /> that has the <see cref="P:System.Linq.Expressions.Expression.NodeType" /> property equal to <see cref="F:System.Linq.Expressions.ExpressionType.ListInit" /> and the <see cref="P:System.Linq.Expressions.ListInitExpression.NewExpression" /> and <see cref="P:System.Linq.Expressions.ListInitExpression.Initializers" /> properties set to the specified values.</returns>
		/// <exception cref="T:System.ArgumentNullException">
		///         <paramref name="newExpression" /> or <paramref name="initializers" /> is <see langword="null" />.-or-One or more elements of <paramref name="initializers" /> are <see langword="null" />.</exception>
		/// <exception cref="T:System.ArgumentException">
		///         <paramref name="newExpression" />.Type does not implement <see cref="T:System.Collections.IEnumerable" />.</exception>
		public static ListInitExpression ListInit(NewExpression newExpression, IEnumerable<ElementInit> initializers)
		{
			ContractUtils.RequiresNotNull(newExpression, "newExpression");
			ContractUtils.RequiresNotNull(initializers, "initializers");
			ReadOnlyCollection<ElementInit> initializers2 = initializers.ToReadOnly<ElementInit>();
			Expression.ValidateListInitArgs(newExpression.Type, initializers2, "newExpression");
			return new ListInitExpression(newExpression, initializers2);
		}

		/// <summary>Creates a <see cref="T:System.Linq.Expressions.LoopExpression" /> with the given body.</summary>
		/// <param name="body">The body of the loop.</param>
		/// <returns>The created <see cref="T:System.Linq.Expressions.LoopExpression" />.</returns>
		public static LoopExpression Loop(Expression body)
		{
			return Expression.Loop(body, null);
		}

		/// <summary>Creates a <see cref="T:System.Linq.Expressions.LoopExpression" /> with the given body and break target.</summary>
		/// <param name="body">The body of the loop.</param>
		/// <param name="break">The break target used by the loop body.</param>
		/// <returns>The created <see cref="T:System.Linq.Expressions.LoopExpression" />.</returns>
		public static LoopExpression Loop(Expression body, LabelTarget @break)
		{
			return Expression.Loop(body, @break, null);
		}

		/// <summary>Creates a <see cref="T:System.Linq.Expressions.LoopExpression" /> with the given body.</summary>
		/// <param name="body">The body of the loop.</param>
		/// <param name="break">The break target used by the loop body.</param>
		/// <param name="continue">The continue target used by the loop body.</param>
		/// <returns>The created <see cref="T:System.Linq.Expressions.LoopExpression" />.</returns>
		public static LoopExpression Loop(Expression body, LabelTarget @break, LabelTarget @continue)
		{
			ExpressionUtils.RequiresCanRead(body, "body");
			if (@continue != null && @continue.Type != typeof(void))
			{
				throw Error.LabelTypeMustBeVoid("continue");
			}
			return new LoopExpression(body, @break, @continue);
		}

		/// <summary>Creates a <see cref="T:System.Linq.Expressions.MemberAssignment" /> that represents the initialization of a field or property.</summary>
		/// <param name="member">A <see cref="T:System.Reflection.MemberInfo" /> to set the <see cref="P:System.Linq.Expressions.MemberBinding.Member" /> property equal to.</param>
		/// <param name="expression">An <see cref="T:System.Linq.Expressions.Expression" /> to set the <see cref="P:System.Linq.Expressions.MemberAssignment.Expression" /> property equal to.</param>
		/// <returns>A <see cref="T:System.Linq.Expressions.MemberAssignment" /> that has <see cref="P:System.Linq.Expressions.MemberBinding.BindingType" /> equal to <see cref="F:System.Linq.Expressions.MemberBindingType.Assignment" /> and the <see cref="P:System.Linq.Expressions.MemberBinding.Member" /> and <see cref="P:System.Linq.Expressions.MemberAssignment.Expression" /> properties set to the specified values.</returns>
		/// <exception cref="T:System.ArgumentNullException">
		///         <paramref name="member" /> or <paramref name="expression" /> is <see langword="null" />.</exception>
		/// <exception cref="T:System.ArgumentException">
		///         <paramref name="member" /> does not represent a field or property.-or-The property represented by <paramref name="member" /> does not have a <see langword="set" /> accessor.-or-
		///         <paramref name="expression" />.Type is not assignable to the type of the field or property that <paramref name="member" /> represents.</exception>
		public static MemberAssignment Bind(MemberInfo member, Expression expression)
		{
			ContractUtils.RequiresNotNull(member, "member");
			ExpressionUtils.RequiresCanRead(expression, "expression");
			Type type;
			Expression.ValidateSettableFieldOrPropertyMember(member, out type);
			if (!type.IsAssignableFrom(expression.Type))
			{
				throw Error.ArgumentTypesMustMatch();
			}
			return new MemberAssignment(member, expression);
		}

		/// <summary>Creates a <see cref="T:System.Linq.Expressions.MemberAssignment" /> that represents the initialization of a member by using a property accessor method.</summary>
		/// <param name="propertyAccessor">A <see cref="T:System.Reflection.MethodInfo" /> that represents a property accessor method.</param>
		/// <param name="expression">An <see cref="T:System.Linq.Expressions.Expression" /> to set the <see cref="P:System.Linq.Expressions.MemberAssignment.Expression" /> property equal to.</param>
		/// <returns>A <see cref="T:System.Linq.Expressions.MemberAssignment" /> that has the <see cref="P:System.Linq.Expressions.MemberBinding.BindingType" /> property equal to <see cref="F:System.Linq.Expressions.MemberBindingType.Assignment" />, the <see cref="P:System.Linq.Expressions.MemberBinding.Member" /> property set to the <see cref="T:System.Reflection.PropertyInfo" /> that represents the property accessed in <paramref name="propertyAccessor" />, and the <see cref="P:System.Linq.Expressions.MemberAssignment.Expression" /> property set to <paramref name="expression" />.</returns>
		/// <exception cref="T:System.ArgumentNullException">
		///         <paramref name="propertyAccessor" /> or <paramref name="expression" /> is <see langword="null" />.</exception>
		/// <exception cref="T:System.ArgumentException">
		///         <paramref name="propertyAccessor" /> does not represent a property accessor method.-or-The property accessed by <paramref name="propertyAccessor" /> does not have a <see langword="set" /> accessor.-or-
		///         <paramref name="expression" />.Type is not assignable to the type of the field or property that <paramref name="member" /> represents.</exception>
		public static MemberAssignment Bind(MethodInfo propertyAccessor, Expression expression)
		{
			ContractUtils.RequiresNotNull(propertyAccessor, "propertyAccessor");
			ContractUtils.RequiresNotNull(expression, "expression");
			Expression.ValidateMethodInfo(propertyAccessor, "propertyAccessor");
			return Expression.Bind(Expression.GetProperty(propertyAccessor, "propertyAccessor", -1), expression);
		}

		private static void ValidateSettableFieldOrPropertyMember(MemberInfo member, out Type memberType)
		{
			Type declaringType = member.DeclaringType;
			if (declaringType == null)
			{
				throw Error.NotAMemberOfAnyType(member, "member");
			}
			TypeUtils.ValidateType(declaringType, null);
			PropertyInfo propertyInfo = member as PropertyInfo;
			if (propertyInfo == null)
			{
				FieldInfo fieldInfo = member as FieldInfo;
				if (fieldInfo == null)
				{
					throw Error.ArgumentMustBeFieldInfoOrPropertyInfo("member");
				}
				memberType = fieldInfo.FieldType;
				return;
			}
			else
			{
				if (!propertyInfo.CanWrite)
				{
					throw Error.PropertyDoesNotHaveSetter(propertyInfo, "member");
				}
				memberType = propertyInfo.PropertyType;
				return;
			}
		}

		/// <summary>Creates a <see cref="T:System.Linq.Expressions.MemberExpression" /> that represents accessing a field.</summary>
		/// <param name="expression">An <see cref="T:System.Linq.Expressions.Expression" /> to set the <see cref="P:System.Linq.Expressions.MemberExpression.Expression" /> property equal to. For <see langword="static" /> (<see langword="Shared" /> in Visual Basic), <paramref name="expression" /> must be <see langword="null" />.</param>
		/// <param name="field">The <see cref="T:System.Reflection.FieldInfo" /> to set the <see cref="P:System.Linq.Expressions.MemberExpression.Member" /> property equal to.</param>
		/// <returns>A <see cref="T:System.Linq.Expressions.MemberExpression" /> that has the <see cref="P:System.Linq.Expressions.Expression.NodeType" /> property equal to <see cref="F:System.Linq.Expressions.ExpressionType.MemberAccess" /> and the <see cref="P:System.Linq.Expressions.MemberExpression.Expression" /> and <see cref="P:System.Linq.Expressions.MemberExpression.Member" /> properties set to the specified values.</returns>
		/// <exception cref="T:System.ArgumentNullException">
		///         <paramref name="field" /> is <see langword="null" />.-or-The field represented by <paramref name="field" /> is not <see langword="static" /> (<see langword="Shared" /> in Visual Basic) and <paramref name="expression" /> is <see langword="null" />.</exception>
		/// <exception cref="T:System.ArgumentException">
		///         <paramref name="expression" />.Type is not assignable to the declaring type of the field represented by <paramref name="field" />.</exception>
		public static MemberExpression Field(Expression expression, FieldInfo field)
		{
			ContractUtils.RequiresNotNull(field, "field");
			if (field.IsStatic)
			{
				if (expression != null)
				{
					throw Error.OnlyStaticFieldsHaveNullInstance("expression");
				}
			}
			else
			{
				if (expression == null)
				{
					throw Error.OnlyStaticFieldsHaveNullInstance("field");
				}
				ExpressionUtils.RequiresCanRead(expression, "expression");
				if (!TypeUtils.AreReferenceAssignable(field.DeclaringType, expression.Type))
				{
					throw Error.FieldInfoNotDefinedForType(field.DeclaringType, field.Name, expression.Type);
				}
			}
			return MemberExpression.Make(expression, field);
		}

		/// <summary>Creates a <see cref="T:System.Linq.Expressions.MemberExpression" /> that represents accessing a field given the name of the field.</summary>
		/// <param name="expression">An <see cref="T:System.Linq.Expressions.Expression" /> whose <see cref="P:System.Linq.Expressions.Expression.Type" /> contains a field named <paramref name="fieldName" />. This can be null for static fields.</param>
		/// <param name="fieldName">The name of a field to be accessed.</param>
		/// <returns>A <see cref="T:System.Linq.Expressions.MemberExpression" /> that has the <see cref="P:System.Linq.Expressions.Expression.NodeType" /> property equal to <see cref="F:System.Linq.Expressions.ExpressionType.MemberAccess" />, the <see cref="P:System.Linq.Expressions.MemberExpression.Expression" /> property set to <paramref name="expression" />, and the <see cref="P:System.Linq.Expressions.MemberExpression.Member" /> property set to the <see cref="T:System.Reflection.FieldInfo" /> that represents the field denoted by <paramref name="fieldName" />.</returns>
		/// <exception cref="T:System.ArgumentNullException">
		///         <paramref name="expression" /> or <paramref name="fieldName" /> is <see langword="null" />.</exception>
		/// <exception cref="T:System.ArgumentException">No field named <paramref name="fieldName" /> is defined in <paramref name="expression" />.Type or its base types.</exception>
		public static MemberExpression Field(Expression expression, string fieldName)
		{
			ExpressionUtils.RequiresCanRead(expression, "expression");
			ContractUtils.RequiresNotNull(fieldName, "fieldName");
			FieldInfo fieldInfo = expression.Type.GetField(fieldName, BindingFlags.IgnoreCase | BindingFlags.Instance | BindingFlags.Public | BindingFlags.FlattenHierarchy) ?? expression.Type.GetField(fieldName, BindingFlags.IgnoreCase | BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.FlattenHierarchy);
			if (fieldInfo == null)
			{
				throw Error.InstanceFieldNotDefinedForType(fieldName, expression.Type);
			}
			return Expression.Field(expression, fieldInfo);
		}

		/// <summary>Creates a <see cref="T:System.Linq.Expressions.MemberExpression" /> that represents accessing a field.</summary>
		/// <param name="expression">The containing object of the field. This can be null for static fields.</param>
		/// <param name="type">The <see cref="P:System.Linq.Expressions.Expression.Type" /> that contains the field.</param>
		/// <param name="fieldName">The field to be accessed.</param>
		/// <returns>The created <see cref="T:System.Linq.Expressions.MemberExpression" />.</returns>
		public static MemberExpression Field(Expression expression, Type type, string fieldName)
		{
			ContractUtils.RequiresNotNull(type, "type");
			FieldInfo fieldInfo = type.GetField(fieldName, BindingFlags.IgnoreCase | BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.FlattenHierarchy) ?? type.GetField(fieldName, BindingFlags.IgnoreCase | BindingFlags.Instance | BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.FlattenHierarchy);
			if (fieldInfo == null)
			{
				throw Error.FieldNotDefinedForType(fieldName, type);
			}
			return Expression.Field(expression, fieldInfo);
		}

		/// <summary>Creates a <see cref="T:System.Linq.Expressions.MemberExpression" /> that represents accessing a property.</summary>
		/// <param name="expression">An <see cref="T:System.Linq.Expressions.Expression" /> whose <see cref="P:System.Linq.Expressions.Expression.Type" /> contains a property named <paramref name="propertyName" />. This can be <see langword="null" /> for static properties.</param>
		/// <param name="propertyName">The name of a property to be accessed.</param>
		/// <returns>A <see cref="T:System.Linq.Expressions.MemberExpression" /> that has the <see cref="P:System.Linq.Expressions.Expression.NodeType" /> property equal to <see cref="F:System.Linq.Expressions.ExpressionType.MemberAccess" />, the <see cref="P:System.Linq.Expressions.MemberExpression.Expression" /> property set to <paramref name="expression" />, and the <see cref="P:System.Linq.Expressions.MemberExpression.Member" /> property set to the <see cref="T:System.Reflection.PropertyInfo" /> that represents the property denoted by <paramref name="propertyName" />.</returns>
		/// <exception cref="T:System.ArgumentNullException">
		///         <paramref name="expression" /> or <paramref name="propertyName" /> is <see langword="null" />.</exception>
		/// <exception cref="T:System.ArgumentException">No property named <paramref name="propertyName" /> is defined in <paramref name="expression" />.Type or its base types.</exception>
		public static MemberExpression Property(Expression expression, string propertyName)
		{
			ExpressionUtils.RequiresCanRead(expression, "expression");
			ContractUtils.RequiresNotNull(propertyName, "propertyName");
			PropertyInfo propertyInfo = expression.Type.GetProperty(propertyName, BindingFlags.IgnoreCase | BindingFlags.Instance | BindingFlags.Public | BindingFlags.FlattenHierarchy) ?? expression.Type.GetProperty(propertyName, BindingFlags.IgnoreCase | BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.FlattenHierarchy);
			if (propertyInfo == null)
			{
				throw Error.InstancePropertyNotDefinedForType(propertyName, expression.Type, "propertyName");
			}
			return Expression.Property(expression, propertyInfo);
		}

		/// <summary>Creates a <see cref="T:System.Linq.Expressions.MemberExpression" /> accessing a property.</summary>
		/// <param name="expression">The containing object of the property. This can be null for static properties.</param>
		/// <param name="type">The <see cref="P:System.Linq.Expressions.Expression.Type" /> that contains the property.</param>
		/// <param name="propertyName">The property to be accessed.</param>
		/// <returns>The created <see cref="T:System.Linq.Expressions.MemberExpression" />.</returns>
		public static MemberExpression Property(Expression expression, Type type, string propertyName)
		{
			ContractUtils.RequiresNotNull(type, "type");
			ContractUtils.RequiresNotNull(propertyName, "propertyName");
			PropertyInfo propertyInfo = type.GetProperty(propertyName, BindingFlags.IgnoreCase | BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.FlattenHierarchy) ?? type.GetProperty(propertyName, BindingFlags.IgnoreCase | BindingFlags.Instance | BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.FlattenHierarchy);
			if (propertyInfo == null)
			{
				throw Error.PropertyNotDefinedForType(propertyName, type, "propertyName");
			}
			return Expression.Property(expression, propertyInfo);
		}

		/// <summary>Creates a <see cref="T:System.Linq.Expressions.MemberExpression" /> that represents accessing a property.</summary>
		/// <param name="expression">An <see cref="T:System.Linq.Expressions.Expression" /> to set the <see cref="P:System.Linq.Expressions.MemberExpression.Expression" /> property equal to. This can be null for static properties.</param>
		/// <param name="property">The <see cref="T:System.Reflection.PropertyInfo" /> to set the <see cref="P:System.Linq.Expressions.MemberExpression.Member" /> property equal to.</param>
		/// <returns>A <see cref="T:System.Linq.Expressions.MemberExpression" /> that has the <see cref="P:System.Linq.Expressions.Expression.NodeType" /> property equal to <see cref="F:System.Linq.Expressions.ExpressionType.MemberAccess" /> and the <see cref="P:System.Linq.Expressions.MemberExpression.Expression" /> and <see cref="P:System.Linq.Expressions.MemberExpression.Member" /> properties set to the specified values.</returns>
		/// <exception cref="T:System.ArgumentNullException">
		///         <paramref name="property" /> is <see langword="null" />.-or-The property that <paramref name="property" /> represents is not <see langword="static" /> (<see langword="Shared" /> in Visual Basic) and <paramref name="expression" /> is <see langword="null" />.</exception>
		/// <exception cref="T:System.ArgumentException">
		///         <paramref name="expression" />.Type is not assignable to the declaring type of the property that <paramref name="property" /> represents.</exception>
		public static MemberExpression Property(Expression expression, PropertyInfo property)
		{
			ContractUtils.RequiresNotNull(property, "property");
			MethodInfo methodInfo = property.GetGetMethod(true);
			if (methodInfo == null)
			{
				methodInfo = property.GetSetMethod(true);
				if (methodInfo == null)
				{
					throw Error.PropertyDoesNotHaveAccessor(property, "property");
				}
				if (methodInfo.GetParametersCached().Length != 1)
				{
					throw Error.IncorrectNumberOfMethodCallArguments(methodInfo, "property");
				}
			}
			else if (methodInfo.GetParametersCached().Length != 0)
			{
				throw Error.IncorrectNumberOfMethodCallArguments(methodInfo, "property");
			}
			if (methodInfo.IsStatic)
			{
				if (expression != null)
				{
					throw Error.OnlyStaticPropertiesHaveNullInstance("expression");
				}
			}
			else
			{
				if (expression == null)
				{
					throw Error.OnlyStaticPropertiesHaveNullInstance("property");
				}
				ExpressionUtils.RequiresCanRead(expression, "expression");
				if (!TypeUtils.IsValidInstanceType(property, expression.Type))
				{
					throw Error.PropertyNotDefinedForType(property, expression.Type, "property");
				}
			}
			Expression.ValidateMethodInfo(methodInfo, "property");
			return MemberExpression.Make(expression, property);
		}

		/// <summary>Creates a <see cref="T:System.Linq.Expressions.MemberExpression" /> that represents accessing a property by using a property accessor method.</summary>
		/// <param name="expression">An <see cref="T:System.Linq.Expressions.Expression" /> to set the <see cref="P:System.Linq.Expressions.MemberExpression.Expression" /> property equal to. This can be null for static properties.</param>
		/// <param name="propertyAccessor">The <see cref="T:System.Reflection.MethodInfo" /> that represents a property accessor method.</param>
		/// <returns>A <see cref="T:System.Linq.Expressions.MemberExpression" /> that has the <see cref="P:System.Linq.Expressions.Expression.NodeType" /> property equal to <see cref="F:System.Linq.Expressions.ExpressionType.MemberAccess" />, the <see cref="P:System.Linq.Expressions.MemberExpression.Expression" /> property set to <paramref name="expression" /> and the <see cref="P:System.Linq.Expressions.MemberExpression.Member" /> property set to the <see cref="T:System.Reflection.PropertyInfo" /> that represents the property accessed in <paramref name="propertyAccessor" />.</returns>
		/// <exception cref="T:System.ArgumentNullException">
		///         <paramref name="propertyAccessor" /> is <see langword="null" />.-or-The method that <paramref name="propertyAccessor" /> represents is not <see langword="static" /> (<see langword="Shared" /> in Visual Basic) and <paramref name="expression" /> is <see langword="null" />.</exception>
		/// <exception cref="T:System.ArgumentException">
		///         <paramref name="expression" />.Type is not assignable to the declaring type of the method represented by <paramref name="propertyAccessor" />.-or-The method that <paramref name="propertyAccessor" /> represents is not a property accessor method.</exception>
		public static MemberExpression Property(Expression expression, MethodInfo propertyAccessor)
		{
			ContractUtils.RequiresNotNull(propertyAccessor, "propertyAccessor");
			Expression.ValidateMethodInfo(propertyAccessor, "propertyAccessor");
			return Expression.Property(expression, Expression.GetProperty(propertyAccessor, "propertyAccessor", -1));
		}

		private static PropertyInfo GetProperty(MethodInfo mi, string paramName, int index = -1)
		{
			Type declaringType = mi.DeclaringType;
			if (declaringType != null)
			{
				BindingFlags bindingFlags = BindingFlags.Public | BindingFlags.NonPublic;
				bindingFlags |= (mi.IsStatic ? BindingFlags.Static : BindingFlags.Instance);
				foreach (PropertyInfo propertyInfo in declaringType.GetProperties(bindingFlags))
				{
					if (propertyInfo.CanRead && Expression.CheckMethod(mi, propertyInfo.GetGetMethod(true)))
					{
						return propertyInfo;
					}
					if (propertyInfo.CanWrite && Expression.CheckMethod(mi, propertyInfo.GetSetMethod(true)))
					{
						return propertyInfo;
					}
				}
			}
			throw Error.MethodNotPropertyAccessor(mi.DeclaringType, mi.Name, paramName, index);
		}

		private static bool CheckMethod(MethodInfo method, MethodInfo propertyMethod)
		{
			if (method.Equals(propertyMethod))
			{
				return true;
			}
			Type declaringType = method.DeclaringType;
			return declaringType.IsInterface && method.Name == propertyMethod.Name && declaringType.GetMethod(method.Name) == propertyMethod;
		}

		/// <summary>Creates a <see cref="T:System.Linq.Expressions.MemberExpression" /> that represents accessing a property or field.</summary>
		/// <param name="expression">An <see cref="T:System.Linq.Expressions.Expression" /> whose <see cref="P:System.Linq.Expressions.Expression.Type" /> contains a property or field named <paramref name="propertyOrFieldName" />. This can be null for static members.</param>
		/// <param name="propertyOrFieldName">The name of a property or field to be accessed.</param>
		/// <returns>A <see cref="T:System.Linq.Expressions.MemberExpression" /> that has the <see cref="P:System.Linq.Expressions.Expression.NodeType" /> property equal to <see cref="F:System.Linq.Expressions.ExpressionType.MemberAccess" />, the <see cref="P:System.Linq.Expressions.MemberExpression.Expression" /> property set to <paramref name="expression" />, and the <see cref="P:System.Linq.Expressions.MemberExpression.Member" /> property set to the <see cref="T:System.Reflection.PropertyInfo" /> or <see cref="T:System.Reflection.FieldInfo" /> that represents the property or field denoted by <paramref name="propertyOrFieldName" />.</returns>
		/// <exception cref="T:System.ArgumentNullException">
		///         <paramref name="expression" /> or <paramref name="propertyOrFieldName" /> is <see langword="null" />.</exception>
		/// <exception cref="T:System.ArgumentException">No property or field named <paramref name="propertyOrFieldName" /> is defined in <paramref name="expression" />.Type or its base types.</exception>
		public static MemberExpression PropertyOrField(Expression expression, string propertyOrFieldName)
		{
			ExpressionUtils.RequiresCanRead(expression, "expression");
			PropertyInfo property = expression.Type.GetProperty(propertyOrFieldName, BindingFlags.IgnoreCase | BindingFlags.Instance | BindingFlags.Public | BindingFlags.FlattenHierarchy);
			if (property != null)
			{
				return Expression.Property(expression, property);
			}
			FieldInfo field = expression.Type.GetField(propertyOrFieldName, BindingFlags.IgnoreCase | BindingFlags.Instance | BindingFlags.Public | BindingFlags.FlattenHierarchy);
			if (field != null)
			{
				return Expression.Field(expression, field);
			}
			property = expression.Type.GetProperty(propertyOrFieldName, BindingFlags.IgnoreCase | BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.FlattenHierarchy);
			if (property != null)
			{
				return Expression.Property(expression, property);
			}
			field = expression.Type.GetField(propertyOrFieldName, BindingFlags.IgnoreCase | BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.FlattenHierarchy);
			if (field != null)
			{
				return Expression.Field(expression, field);
			}
			throw Error.NotAMemberOfType(propertyOrFieldName, expression.Type, "propertyOrFieldName");
		}

		/// <summary>Creates a <see cref="T:System.Linq.Expressions.MemberExpression" /> that represents accessing either a field or a property.</summary>
		/// <param name="expression">An <see cref="T:System.Linq.Expressions.Expression" /> that represents the object that the member belongs to. This can be null for static members.</param>
		/// <param name="member">The <see cref="T:System.Reflection.MemberInfo" /> that describes the field or property to be accessed.</param>
		/// <returns>The <see cref="T:System.Linq.Expressions.MemberExpression" /> that results from calling the appropriate factory method.</returns>
		/// <exception cref="T:System.ArgumentNullException">
		///         <paramref name="member" /> is <see langword="null" />.</exception>
		/// <exception cref="T:System.ArgumentException">
		///         <paramref name="member" /> does not represent a field or property.</exception>
		public static MemberExpression MakeMemberAccess(Expression expression, MemberInfo member)
		{
			ContractUtils.RequiresNotNull(member, "member");
			FieldInfo fieldInfo = member as FieldInfo;
			if (fieldInfo != null)
			{
				return Expression.Field(expression, fieldInfo);
			}
			PropertyInfo propertyInfo = member as PropertyInfo;
			if (propertyInfo != null)
			{
				return Expression.Property(expression, propertyInfo);
			}
			throw Error.MemberNotFieldOrProperty(member, "member");
		}

		/// <summary>Creates a <see cref="T:System.Linq.Expressions.MemberInitExpression" />.</summary>
		/// <param name="newExpression">A <see cref="T:System.Linq.Expressions.NewExpression" /> to set the <see cref="P:System.Linq.Expressions.MemberInitExpression.NewExpression" /> property equal to.</param>
		/// <param name="bindings">An array of <see cref="T:System.Linq.Expressions.MemberBinding" /> objects to use to populate the <see cref="P:System.Linq.Expressions.MemberInitExpression.Bindings" /> collection.</param>
		/// <returns>A <see cref="T:System.Linq.Expressions.MemberInitExpression" /> that has the <see cref="P:System.Linq.Expressions.Expression.NodeType" /> property equal to <see cref="F:System.Linq.Expressions.ExpressionType.MemberInit" /> and the <see cref="P:System.Linq.Expressions.MemberInitExpression.NewExpression" /> and <see cref="P:System.Linq.Expressions.MemberInitExpression.Bindings" /> properties set to the specified values.</returns>
		/// <exception cref="T:System.ArgumentNullException">
		///         <paramref name="newExpression" /> or <paramref name="bindings" /> is <see langword="null" />.</exception>
		/// <exception cref="T:System.ArgumentException">The <see cref="P:System.Linq.Expressions.MemberBinding.Member" /> property of an element of <paramref name="bindings" /> does not represent a member of the type that <paramref name="newExpression" />.Type represents.</exception>
		public static MemberInitExpression MemberInit(NewExpression newExpression, params MemberBinding[] bindings)
		{
			return Expression.MemberInit(newExpression, bindings);
		}

		/// <summary>Represents an expression that creates a new object and initializes a property of the object.</summary>
		/// <param name="newExpression">A <see cref="T:System.Linq.Expressions.NewExpression" /> to set the <see cref="P:System.Linq.Expressions.MemberInitExpression.NewExpression" /> property equal to.</param>
		/// <param name="bindings">An <see cref="T:System.Collections.Generic.IEnumerable`1" /> that contains <see cref="T:System.Linq.Expressions.MemberBinding" /> objects to use to populate the <see cref="P:System.Linq.Expressions.MemberInitExpression.Bindings" /> collection.</param>
		/// <returns>A <see cref="T:System.Linq.Expressions.MemberInitExpression" /> that has the <see cref="P:System.Linq.Expressions.Expression.NodeType" /> property equal to <see cref="F:System.Linq.Expressions.ExpressionType.MemberInit" /> and the <see cref="P:System.Linq.Expressions.MemberInitExpression.NewExpression" /> and <see cref="P:System.Linq.Expressions.MemberInitExpression.Bindings" /> properties set to the specified values.</returns>
		/// <exception cref="T:System.ArgumentNullException">
		///         <paramref name="newExpression" /> or <paramref name="bindings" /> is <see langword="null" />.</exception>
		/// <exception cref="T:System.ArgumentException">The <see cref="P:System.Linq.Expressions.MemberBinding.Member" /> property of an element of <paramref name="bindings" /> does not represent a member of the type that <paramref name="newExpression" />.Type represents.</exception>
		public static MemberInitExpression MemberInit(NewExpression newExpression, IEnumerable<MemberBinding> bindings)
		{
			ContractUtils.RequiresNotNull(newExpression, "newExpression");
			ContractUtils.RequiresNotNull(bindings, "bindings");
			ReadOnlyCollection<MemberBinding> bindings2 = bindings.ToReadOnly<MemberBinding>();
			Expression.ValidateMemberInitArgs(newExpression.Type, bindings2);
			return new MemberInitExpression(newExpression, bindings2);
		}

		/// <summary>Creates a <see cref="T:System.Linq.Expressions.MemberListBinding" /> where the member is a field or property.</summary>
		/// <param name="member">A <see cref="T:System.Reflection.MemberInfo" /> that represents a field or property to set the <see cref="P:System.Linq.Expressions.MemberBinding.Member" /> property equal to.</param>
		/// <param name="initializers">An array of <see cref="T:System.Linq.Expressions.ElementInit" /> objects to use to populate the <see cref="P:System.Linq.Expressions.MemberListBinding.Initializers" /> collection.</param>
		/// <returns>A <see cref="T:System.Linq.Expressions.MemberListBinding" /> that has the <see cref="P:System.Linq.Expressions.MemberBinding.BindingType" /> property equal to <see cref="F:System.Linq.Expressions.MemberBindingType.ListBinding" /> and the <see cref="P:System.Linq.Expressions.MemberBinding.Member" /> and <see cref="P:System.Linq.Expressions.MemberListBinding.Initializers" /> properties set to the specified values.</returns>
		/// <exception cref="T:System.ArgumentNullException">
		///         <paramref name="member" /> is <see langword="null" />. -or-One or more elements of <paramref name="initializers" /> are <see langword="null" />.</exception>
		/// <exception cref="T:System.ArgumentException">
		///         <paramref name="member" /> does not represent a field or property.-or-The <see cref="P:System.Reflection.FieldInfo.FieldType" /> or <see cref="P:System.Reflection.PropertyInfo.PropertyType" /> of the field or property that <paramref name="member" /> represents does not implement <see cref="T:System.Collections.IEnumerable" />.</exception>
		public static MemberListBinding ListBind(MemberInfo member, params ElementInit[] initializers)
		{
			return Expression.ListBind(member, initializers);
		}

		/// <summary>Creates a <see cref="T:System.Linq.Expressions.MemberListBinding" /> where the member is a field or property.</summary>
		/// <param name="member">A <see cref="T:System.Reflection.MemberInfo" /> that represents a field or property to set the <see cref="P:System.Linq.Expressions.MemberBinding.Member" /> property equal to.</param>
		/// <param name="initializers">An <see cref="T:System.Collections.Generic.IEnumerable`1" /> that contains <see cref="T:System.Linq.Expressions.ElementInit" /> objects to use to populate the <see cref="P:System.Linq.Expressions.MemberListBinding.Initializers" /> collection.</param>
		/// <returns>A <see cref="T:System.Linq.Expressions.MemberListBinding" /> that has the <see cref="P:System.Linq.Expressions.MemberBinding.BindingType" /> property equal to <see cref="F:System.Linq.Expressions.MemberBindingType.ListBinding" /> and the <see cref="P:System.Linq.Expressions.MemberBinding.Member" /> and <see cref="P:System.Linq.Expressions.MemberListBinding.Initializers" /> properties set to the specified values.</returns>
		/// <exception cref="T:System.ArgumentNullException">
		///         <paramref name="member" /> is <see langword="null" />. -or-One or more elements of <paramref name="initializers" /> are <see langword="null" />.</exception>
		/// <exception cref="T:System.ArgumentException">
		///         <paramref name="member" /> does not represent a field or property.-or-The <see cref="P:System.Reflection.FieldInfo.FieldType" /> or <see cref="P:System.Reflection.PropertyInfo.PropertyType" /> of the field or property that <paramref name="member" /> represents does not implement <see cref="T:System.Collections.IEnumerable" />.</exception>
		public static MemberListBinding ListBind(MemberInfo member, IEnumerable<ElementInit> initializers)
		{
			ContractUtils.RequiresNotNull(member, "member");
			ContractUtils.RequiresNotNull(initializers, "initializers");
			Type listType;
			Expression.ValidateGettableFieldOrPropertyMember(member, out listType);
			ReadOnlyCollection<ElementInit> initializers2 = initializers.ToReadOnly<ElementInit>();
			Expression.ValidateListInitArgs(listType, initializers2, "member");
			return new MemberListBinding(member, initializers2);
		}

		/// <summary>Creates a <see cref="T:System.Linq.Expressions.MemberListBinding" /> object based on a specified property accessor method.</summary>
		/// <param name="propertyAccessor">A <see cref="T:System.Reflection.MethodInfo" /> that represents a property accessor method.</param>
		/// <param name="initializers">An array of <see cref="T:System.Linq.Expressions.ElementInit" /> objects to use to populate the <see cref="P:System.Linq.Expressions.MemberListBinding.Initializers" /> collection.</param>
		/// <returns>A <see cref="T:System.Linq.Expressions.MemberListBinding" /> that has the <see cref="P:System.Linq.Expressions.MemberBinding.BindingType" /> property equal to <see cref="F:System.Linq.Expressions.MemberBindingType.ListBinding" />, the <see cref="P:System.Linq.Expressions.MemberBinding.Member" /> property set to the <see cref="T:System.Reflection.MemberInfo" /> that represents the property accessed in <paramref name="propertyAccessor" />, and <see cref="P:System.Linq.Expressions.MemberListBinding.Initializers" /> populated with the elements of <paramref name="initializers" />.</returns>
		/// <exception cref="T:System.ArgumentNullException">
		///         <paramref name="propertyAccessor" /> is <see langword="null" />. -or-One or more elements of <paramref name="initializers" /> are <see langword="null" />.</exception>
		/// <exception cref="T:System.ArgumentException">
		///         <paramref name="propertyAccessor" /> does not represent a property accessor method.-or-The <see cref="P:System.Reflection.PropertyInfo.PropertyType" /> of the property that the method represented by <paramref name="propertyAccessor" /> accesses does not implement <see cref="T:System.Collections.IEnumerable" />.</exception>
		public static MemberListBinding ListBind(MethodInfo propertyAccessor, params ElementInit[] initializers)
		{
			return Expression.ListBind(propertyAccessor, initializers);
		}

		/// <summary>Creates a <see cref="T:System.Linq.Expressions.MemberListBinding" /> based on a specified property accessor method.</summary>
		/// <param name="propertyAccessor">A <see cref="T:System.Reflection.MethodInfo" /> that represents a property accessor method.</param>
		/// <param name="initializers">An <see cref="T:System.Collections.Generic.IEnumerable`1" /> that contains <see cref="T:System.Linq.Expressions.ElementInit" /> objects to use to populate the <see cref="P:System.Linq.Expressions.MemberListBinding.Initializers" /> collection.</param>
		/// <returns>A <see cref="T:System.Linq.Expressions.MemberListBinding" /> that has the <see cref="P:System.Linq.Expressions.MemberBinding.BindingType" /> property equal to <see cref="F:System.Linq.Expressions.MemberBindingType.ListBinding" />, the <see cref="P:System.Linq.Expressions.MemberBinding.Member" /> property set to the <see cref="T:System.Reflection.MemberInfo" /> that represents the property accessed in <paramref name="propertyAccessor" />, and <see cref="P:System.Linq.Expressions.MemberListBinding.Initializers" /> populated with the elements of <paramref name="initializers" />.</returns>
		/// <exception cref="T:System.ArgumentNullException">
		///         <paramref name="propertyAccessor" /> is <see langword="null" />. -or-One or more elements of <paramref name="initializers" /> are <see langword="null" />.</exception>
		/// <exception cref="T:System.ArgumentException">
		///         <paramref name="propertyAccessor" /> does not represent a property accessor method.-or-The <see cref="P:System.Reflection.PropertyInfo.PropertyType" /> of the property that the method represented by <paramref name="propertyAccessor" /> accesses does not implement <see cref="T:System.Collections.IEnumerable" />.</exception>
		public static MemberListBinding ListBind(MethodInfo propertyAccessor, IEnumerable<ElementInit> initializers)
		{
			ContractUtils.RequiresNotNull(propertyAccessor, "propertyAccessor");
			ContractUtils.RequiresNotNull(initializers, "initializers");
			return Expression.ListBind(Expression.GetProperty(propertyAccessor, "propertyAccessor", -1), initializers);
		}

		private static void ValidateListInitArgs(Type listType, ReadOnlyCollection<ElementInit> initializers, string listTypeParamName)
		{
			if (!typeof(IEnumerable).IsAssignableFrom(listType))
			{
				throw Error.TypeNotIEnumerable(listType, listTypeParamName);
			}
			int i = 0;
			int count = initializers.Count;
			while (i < count)
			{
				ElementInit elementInit = initializers[i];
				ContractUtils.RequiresNotNull(elementInit, "initializers", i);
				Expression.ValidateCallInstanceType(listType, elementInit.AddMethod);
				i++;
			}
		}

		/// <summary>Creates a <see cref="T:System.Linq.Expressions.MemberMemberBinding" /> that represents the recursive initialization of members of a field or property.</summary>
		/// <param name="member">The <see cref="T:System.Reflection.MemberInfo" /> to set the <see cref="P:System.Linq.Expressions.MemberBinding.Member" /> property equal to.</param>
		/// <param name="bindings">An array of <see cref="T:System.Linq.Expressions.MemberBinding" /> objects to use to populate the <see cref="P:System.Linq.Expressions.MemberMemberBinding.Bindings" /> collection.</param>
		/// <returns>A <see cref="T:System.Linq.Expressions.MemberMemberBinding" /> that has the <see cref="P:System.Linq.Expressions.MemberBinding.BindingType" /> property equal to <see cref="F:System.Linq.Expressions.MemberBindingType.MemberBinding" /> and the <see cref="P:System.Linq.Expressions.MemberBinding.Member" /> and <see cref="P:System.Linq.Expressions.MemberMemberBinding.Bindings" /> properties set to the specified values.</returns>
		/// <exception cref="T:System.ArgumentNullException">
		///         <paramref name="member" /> or <paramref name="bindings" /> is <see langword="null" />.</exception>
		/// <exception cref="T:System.ArgumentException">
		///         <paramref name="member" /> does not represent a field or property.-or-The <see cref="P:System.Linq.Expressions.MemberBinding.Member" /> property of an element of <paramref name="bindings" /> does not represent a member of the type of the field or property that <paramref name="member" /> represents.</exception>
		public static MemberMemberBinding MemberBind(MemberInfo member, params MemberBinding[] bindings)
		{
			return Expression.MemberBind(member, bindings);
		}

		/// <summary>Creates a <see cref="T:System.Linq.Expressions.MemberMemberBinding" /> that represents the recursive initialization of members of a field or property.</summary>
		/// <param name="member">The <see cref="T:System.Reflection.MemberInfo" /> to set the <see cref="P:System.Linq.Expressions.MemberBinding.Member" /> property equal to.</param>
		/// <param name="bindings">An <see cref="T:System.Collections.Generic.IEnumerable`1" /> that contains <see cref="T:System.Linq.Expressions.MemberBinding" /> objects to use to populate the <see cref="P:System.Linq.Expressions.MemberMemberBinding.Bindings" /> collection.</param>
		/// <returns>A <see cref="T:System.Linq.Expressions.MemberMemberBinding" /> that has the <see cref="P:System.Linq.Expressions.MemberBinding.BindingType" /> property equal to <see cref="F:System.Linq.Expressions.MemberBindingType.MemberBinding" /> and the <see cref="P:System.Linq.Expressions.MemberBinding.Member" /> and <see cref="P:System.Linq.Expressions.MemberMemberBinding.Bindings" /> properties set to the specified values.</returns>
		/// <exception cref="T:System.ArgumentNullException">
		///         <paramref name="member" /> or <paramref name="bindings" /> is <see langword="null" />.</exception>
		/// <exception cref="T:System.ArgumentException">
		///         <paramref name="member" /> does not represent a field or property.-or-The <see cref="P:System.Linq.Expressions.MemberBinding.Member" /> property of an element of <paramref name="bindings" /> does not represent a member of the type of the field or property that <paramref name="member" /> represents.</exception>
		public static MemberMemberBinding MemberBind(MemberInfo member, IEnumerable<MemberBinding> bindings)
		{
			ContractUtils.RequiresNotNull(member, "member");
			ContractUtils.RequiresNotNull(bindings, "bindings");
			ReadOnlyCollection<MemberBinding> bindings2 = bindings.ToReadOnly<MemberBinding>();
			Type type;
			Expression.ValidateGettableFieldOrPropertyMember(member, out type);
			Expression.ValidateMemberInitArgs(type, bindings2);
			return new MemberMemberBinding(member, bindings2);
		}

		/// <summary>Creates a <see cref="T:System.Linq.Expressions.MemberMemberBinding" /> that represents the recursive initialization of members of a member that is accessed by using a property accessor method.</summary>
		/// <param name="propertyAccessor">The <see cref="T:System.Reflection.MethodInfo" /> that represents a property accessor method.</param>
		/// <param name="bindings">An array of <see cref="T:System.Linq.Expressions.MemberBinding" /> objects to use to populate the <see cref="P:System.Linq.Expressions.MemberMemberBinding.Bindings" /> collection.</param>
		/// <returns>A <see cref="T:System.Linq.Expressions.MemberMemberBinding" /> that has the <see cref="P:System.Linq.Expressions.MemberBinding.BindingType" /> property equal to <see cref="F:System.Linq.Expressions.MemberBindingType.MemberBinding" />, the <see cref="P:System.Linq.Expressions.MemberBinding.Member" /> property set to the <see cref="T:System.Reflection.PropertyInfo" /> that represents the property accessed in <paramref name="propertyAccessor" />, and <see cref="P:System.Linq.Expressions.MemberMemberBinding.Bindings" /> properties set to the specified values.</returns>
		/// <exception cref="T:System.ArgumentNullException">
		///         <paramref name="propertyAccessor" /> or <paramref name="bindings" /> is <see langword="null" />.</exception>
		/// <exception cref="T:System.ArgumentException">
		///         <paramref name="propertyAccessor" /> does not represent a property accessor method.-or-The <see cref="P:System.Linq.Expressions.MemberBinding.Member" /> property of an element of <paramref name="bindings" /> does not represent a member of the type of the property accessed by the method that <paramref name="propertyAccessor" /> represents.</exception>
		public static MemberMemberBinding MemberBind(MethodInfo propertyAccessor, params MemberBinding[] bindings)
		{
			return Expression.MemberBind(propertyAccessor, bindings);
		}

		/// <summary>Creates a <see cref="T:System.Linq.Expressions.MemberMemberBinding" /> that represents the recursive initialization of members of a member that is accessed by using a property accessor method.</summary>
		/// <param name="propertyAccessor">The <see cref="T:System.Reflection.MethodInfo" /> that represents a property accessor method.</param>
		/// <param name="bindings">An <see cref="T:System.Collections.Generic.IEnumerable`1" /> that contains <see cref="T:System.Linq.Expressions.MemberBinding" /> objects to use to populate the <see cref="P:System.Linq.Expressions.MemberMemberBinding.Bindings" /> collection.</param>
		/// <returns>A <see cref="T:System.Linq.Expressions.MemberMemberBinding" /> that has the <see cref="P:System.Linq.Expressions.MemberBinding.BindingType" /> property equal to <see cref="F:System.Linq.Expressions.MemberBindingType.MemberBinding" />, the <see cref="P:System.Linq.Expressions.MemberBinding.Member" /> property set to the <see cref="T:System.Reflection.PropertyInfo" /> that represents the property accessed in <paramref name="propertyAccessor" />, and <see cref="P:System.Linq.Expressions.MemberMemberBinding.Bindings" /> properties set to the specified values.</returns>
		/// <exception cref="T:System.ArgumentNullException">
		///         <paramref name="propertyAccessor" /> or <paramref name="bindings" /> is <see langword="null" />.</exception>
		/// <exception cref="T:System.ArgumentException">
		///         <paramref name="propertyAccessor" /> does not represent a property accessor method.-or-The <see cref="P:System.Linq.Expressions.MemberBinding.Member" /> property of an element of <paramref name="bindings" /> does not represent a member of the type of the property accessed by the method that <paramref name="propertyAccessor" /> represents.</exception>
		public static MemberMemberBinding MemberBind(MethodInfo propertyAccessor, IEnumerable<MemberBinding> bindings)
		{
			ContractUtils.RequiresNotNull(propertyAccessor, "propertyAccessor");
			return Expression.MemberBind(Expression.GetProperty(propertyAccessor, "propertyAccessor", -1), bindings);
		}

		private static void ValidateGettableFieldOrPropertyMember(MemberInfo member, out Type memberType)
		{
			Type declaringType = member.DeclaringType;
			if (declaringType == null)
			{
				throw Error.NotAMemberOfAnyType(member, "member");
			}
			TypeUtils.ValidateType(declaringType, null, true, true);
			PropertyInfo propertyInfo = member as PropertyInfo;
			if (propertyInfo == null)
			{
				FieldInfo fieldInfo = member as FieldInfo;
				if (fieldInfo == null)
				{
					throw Error.ArgumentMustBeFieldInfoOrPropertyInfo("member");
				}
				memberType = fieldInfo.FieldType;
				return;
			}
			else
			{
				if (!propertyInfo.CanRead)
				{
					throw Error.PropertyDoesNotHaveGetter(propertyInfo, "member");
				}
				memberType = propertyInfo.PropertyType;
				return;
			}
		}

		private static void ValidateMemberInitArgs(Type type, ReadOnlyCollection<MemberBinding> bindings)
		{
			int i = 0;
			int count = bindings.Count;
			while (i < count)
			{
				MemberBinding memberBinding = bindings[i];
				ContractUtils.RequiresNotNull(memberBinding, "bindings");
				memberBinding.ValidateAsDefinedHere(i);
				if (!memberBinding.Member.DeclaringType.IsAssignableFrom(type))
				{
					throw Error.NotAMemberOfType(memberBinding.Member.Name, type, "bindings", i);
				}
				i++;
			}
		}

		internal static MethodCallExpression Call(MethodInfo method)
		{
			ContractUtils.RequiresNotNull(method, "method");
			ParameterInfo[] pis = Expression.ValidateMethodAndGetParameters(null, method);
			Expression.ValidateArgumentCount(method, ExpressionType.Call, 0, pis);
			return new MethodCallExpression0(method);
		}

		/// <summary>Creates a <see cref="T:System.Linq.Expressions.MethodCallExpression" /> that represents a call to a <see langword="static" /> (<see langword="Shared" /> in Visual Basic) method that takes one argument.</summary>
		/// <param name="method">A <see cref="T:System.Reflection.MethodInfo" /> to set the <see cref="P:System.Linq.Expressions.MethodCallExpression.Method" /> property equal to.</param>
		/// <param name="arg0">The <see cref="T:System.Linq.Expressions.Expression" /> that represents the first argument.</param>
		/// <returns>A <see cref="T:System.Linq.Expressions.MethodCallExpression" /> that has the <see cref="P:System.Linq.Expressions.Expression.NodeType" /> property equal to <see cref="F:System.Linq.Expressions.ExpressionType.Call" /> and the <see cref="P:System.Linq.Expressions.MethodCallExpression.Object" /> and <see cref="P:System.Linq.Expressions.MethodCallExpression.Method" /> properties set to the specified values.</returns>
		/// <exception cref="T:System.ArgumentNullException">
		///         <paramref name="method" /> is null.</exception>
		public static MethodCallExpression Call(MethodInfo method, Expression arg0)
		{
			ContractUtils.RequiresNotNull(method, "method");
			ContractUtils.RequiresNotNull(arg0, "arg0");
			ParameterInfo[] array = Expression.ValidateMethodAndGetParameters(null, method);
			Expression.ValidateArgumentCount(method, ExpressionType.Call, 1, array);
			arg0 = Expression.ValidateOneArgument(method, ExpressionType.Call, arg0, array[0], "method", "arg0");
			return new MethodCallExpression1(method, arg0);
		}

		/// <summary>Creates a <see cref="T:System.Linq.Expressions.MethodCallExpression" /> that represents a call to a static method that takes two arguments.</summary>
		/// <param name="method">A <see cref="T:System.Reflection.MethodInfo" /> to set the <see cref="P:System.Linq.Expressions.MethodCallExpression.Method" /> property equal to.</param>
		/// <param name="arg0">The <see cref="T:System.Linq.Expressions.Expression" /> that represents the first argument.</param>
		/// <param name="arg1">The <see cref="T:System.Linq.Expressions.Expression" /> that represents the second argument.</param>
		/// <returns>A <see cref="T:System.Linq.Expressions.MethodCallExpression" /> that has the <see cref="P:System.Linq.Expressions.Expression.NodeType" /> property equal to <see cref="F:System.Linq.Expressions.ExpressionType.Call" /> and the <see cref="P:System.Linq.Expressions.MethodCallExpression.Object" /> and <see cref="P:System.Linq.Expressions.MethodCallExpression.Method" /> properties set to the specified values.</returns>
		/// <exception cref="T:System.ArgumentNullException">
		///         <paramref name="method" /> is null.</exception>
		public static MethodCallExpression Call(MethodInfo method, Expression arg0, Expression arg1)
		{
			ContractUtils.RequiresNotNull(method, "method");
			ContractUtils.RequiresNotNull(arg0, "arg0");
			ContractUtils.RequiresNotNull(arg1, "arg1");
			ParameterInfo[] array = Expression.ValidateMethodAndGetParameters(null, method);
			Expression.ValidateArgumentCount(method, ExpressionType.Call, 2, array);
			arg0 = Expression.ValidateOneArgument(method, ExpressionType.Call, arg0, array[0], "method", "arg0");
			arg1 = Expression.ValidateOneArgument(method, ExpressionType.Call, arg1, array[1], "method", "arg1");
			return new MethodCallExpression2(method, arg0, arg1);
		}

		/// <summary>Creates a <see cref="T:System.Linq.Expressions.MethodCallExpression" /> that represents a call to a static method that takes three arguments.</summary>
		/// <param name="method">A <see cref="T:System.Reflection.MethodInfo" /> to set the <see cref="P:System.Linq.Expressions.MethodCallExpression.Method" /> property equal to.</param>
		/// <param name="arg0">The <see cref="T:System.Linq.Expressions.Expression" /> that represents the first argument.</param>
		/// <param name="arg1">The <see cref="T:System.Linq.Expressions.Expression" /> that represents the second argument.</param>
		/// <param name="arg2">The <see cref="T:System.Linq.Expressions.Expression" /> that represents the third argument.</param>
		/// <returns>A <see cref="T:System.Linq.Expressions.MethodCallExpression" /> that has the <see cref="P:System.Linq.Expressions.Expression.NodeType" /> property equal to <see cref="F:System.Linq.Expressions.ExpressionType.Call" /> and the <see cref="P:System.Linq.Expressions.MethodCallExpression.Object" /> and <see cref="P:System.Linq.Expressions.MethodCallExpression.Method" /> properties set to the specified values.</returns>
		/// <exception cref="T:System.ArgumentNullException">
		///         <paramref name="method" /> is null.</exception>
		public static MethodCallExpression Call(MethodInfo method, Expression arg0, Expression arg1, Expression arg2)
		{
			ContractUtils.RequiresNotNull(method, "method");
			ContractUtils.RequiresNotNull(arg0, "arg0");
			ContractUtils.RequiresNotNull(arg1, "arg1");
			ContractUtils.RequiresNotNull(arg2, "arg2");
			ParameterInfo[] array = Expression.ValidateMethodAndGetParameters(null, method);
			Expression.ValidateArgumentCount(method, ExpressionType.Call, 3, array);
			arg0 = Expression.ValidateOneArgument(method, ExpressionType.Call, arg0, array[0], "method", "arg0");
			arg1 = Expression.ValidateOneArgument(method, ExpressionType.Call, arg1, array[1], "method", "arg1");
			arg2 = Expression.ValidateOneArgument(method, ExpressionType.Call, arg2, array[2], "method", "arg2");
			return new MethodCallExpression3(method, arg0, arg1, arg2);
		}

		/// <summary>Creates a <see cref="T:System.Linq.Expressions.MethodCallExpression" /> that represents a call to a static method that takes four arguments.</summary>
		/// <param name="method">A <see cref="T:System.Reflection.MethodInfo" /> to set the <see cref="P:System.Linq.Expressions.MethodCallExpression.Method" /> property equal to.</param>
		/// <param name="arg0">The <see cref="T:System.Linq.Expressions.Expression" /> that represents the first argument.</param>
		/// <param name="arg1">The <see cref="T:System.Linq.Expressions.Expression" /> that represents the second argument.</param>
		/// <param name="arg2">The <see cref="T:System.Linq.Expressions.Expression" /> that represents the third argument.</param>
		/// <param name="arg3">The <see cref="T:System.Linq.Expressions.Expression" /> that represents the fourth argument.</param>
		/// <returns>A <see cref="T:System.Linq.Expressions.MethodCallExpression" /> that has the <see cref="P:System.Linq.Expressions.Expression.NodeType" /> property equal to <see cref="F:System.Linq.Expressions.ExpressionType.Call" /> and the <see cref="P:System.Linq.Expressions.MethodCallExpression.Object" /> and <see cref="P:System.Linq.Expressions.MethodCallExpression.Method" /> properties set to the specified values.</returns>
		/// <exception cref="T:System.ArgumentNullException">
		///         <paramref name="method" /> is null.</exception>
		public static MethodCallExpression Call(MethodInfo method, Expression arg0, Expression arg1, Expression arg2, Expression arg3)
		{
			ContractUtils.RequiresNotNull(method, "method");
			ContractUtils.RequiresNotNull(arg0, "arg0");
			ContractUtils.RequiresNotNull(arg1, "arg1");
			ContractUtils.RequiresNotNull(arg2, "arg2");
			ContractUtils.RequiresNotNull(arg3, "arg3");
			ParameterInfo[] array = Expression.ValidateMethodAndGetParameters(null, method);
			Expression.ValidateArgumentCount(method, ExpressionType.Call, 4, array);
			arg0 = Expression.ValidateOneArgument(method, ExpressionType.Call, arg0, array[0], "method", "arg0");
			arg1 = Expression.ValidateOneArgument(method, ExpressionType.Call, arg1, array[1], "method", "arg1");
			arg2 = Expression.ValidateOneArgument(method, ExpressionType.Call, arg2, array[2], "method", "arg2");
			arg3 = Expression.ValidateOneArgument(method, ExpressionType.Call, arg3, array[3], "method", "arg3");
			return new MethodCallExpression4(method, arg0, arg1, arg2, arg3);
		}

		/// <summary>Creates a <see cref="T:System.Linq.Expressions.MethodCallExpression" /> that represents a call to a static method that takes five arguments.</summary>
		/// <param name="method">A <see cref="T:System.Reflection.MethodInfo" /> to set the <see cref="P:System.Linq.Expressions.MethodCallExpression.Method" /> property equal to.</param>
		/// <param name="arg0">The <see cref="T:System.Linq.Expressions.Expression" /> that represents the first argument.</param>
		/// <param name="arg1">The <see cref="T:System.Linq.Expressions.Expression" /> that represents the second argument.</param>
		/// <param name="arg2">The <see cref="T:System.Linq.Expressions.Expression" /> that represents the third argument.</param>
		/// <param name="arg3">The <see cref="T:System.Linq.Expressions.Expression" /> that represents the fourth argument.</param>
		/// <param name="arg4">The <see cref="T:System.Linq.Expressions.Expression" /> that represents the fifth argument.</param>
		/// <returns>A <see cref="T:System.Linq.Expressions.MethodCallExpression" /> that has the <see cref="P:System.Linq.Expressions.Expression.NodeType" /> property equal to <see cref="F:System.Linq.Expressions.ExpressionType.Call" /> and the <see cref="P:System.Linq.Expressions.MethodCallExpression.Object" /> and <see cref="P:System.Linq.Expressions.MethodCallExpression.Method" /> properties set to the specified values.</returns>
		/// <exception cref="T:System.ArgumentNullException">
		///         <paramref name="method" /> is null.</exception>
		public static MethodCallExpression Call(MethodInfo method, Expression arg0, Expression arg1, Expression arg2, Expression arg3, Expression arg4)
		{
			ContractUtils.RequiresNotNull(method, "method");
			ContractUtils.RequiresNotNull(arg0, "arg0");
			ContractUtils.RequiresNotNull(arg1, "arg1");
			ContractUtils.RequiresNotNull(arg2, "arg2");
			ContractUtils.RequiresNotNull(arg3, "arg3");
			ContractUtils.RequiresNotNull(arg4, "arg4");
			ParameterInfo[] array = Expression.ValidateMethodAndGetParameters(null, method);
			Expression.ValidateArgumentCount(method, ExpressionType.Call, 5, array);
			arg0 = Expression.ValidateOneArgument(method, ExpressionType.Call, arg0, array[0], "method", "arg0");
			arg1 = Expression.ValidateOneArgument(method, ExpressionType.Call, arg1, array[1], "method", "arg1");
			arg2 = Expression.ValidateOneArgument(method, ExpressionType.Call, arg2, array[2], "method", "arg2");
			arg3 = Expression.ValidateOneArgument(method, ExpressionType.Call, arg3, array[3], "method", "arg3");
			arg4 = Expression.ValidateOneArgument(method, ExpressionType.Call, arg4, array[4], "method", "arg4");
			return new MethodCallExpression5(method, arg0, arg1, arg2, arg3, arg4);
		}

		/// <summary>Creates a <see cref="T:System.Linq.Expressions.MethodCallExpression" /> that represents a call to a <see langword="static" /> (<see langword="Shared" /> in Visual Basic) method that has arguments.</summary>
		/// <param name="method">A <see cref="T:System.Reflection.MethodInfo" /> that represents a <see langword="static" /> (<see langword="Shared" /> in Visual Basic) method to set the <see cref="P:System.Linq.Expressions.MethodCallExpression.Method" /> property equal to.</param>
		/// <param name="arguments">An array of <see cref="T:System.Linq.Expressions.Expression" /> objects to use to populate the <see cref="P:System.Linq.Expressions.MethodCallExpression.Arguments" /> collection.</param>
		/// <returns>A <see cref="T:System.Linq.Expressions.MethodCallExpression" /> that has the <see cref="P:System.Linq.Expressions.Expression.NodeType" /> property equal to <see cref="F:System.Linq.Expressions.ExpressionType.Call" /> and the <see cref="P:System.Linq.Expressions.MethodCallExpression.Method" /> and <see cref="P:System.Linq.Expressions.MethodCallExpression.Arguments" /> properties set to the specified values.</returns>
		/// <exception cref="T:System.ArgumentNullException">
		///         <paramref name="method" /> is <see langword="null" />.</exception>
		/// <exception cref="T:System.ArgumentException">The number of elements in <paramref name="arguments" /> does not equal the number of parameters for the method represented by <paramref name="method" />.-or-One or more of the elements of <paramref name="arguments" /> is not assignable to the corresponding parameter for the method represented by <paramref name="method" />.</exception>
		public static MethodCallExpression Call(MethodInfo method, params Expression[] arguments)
		{
			return Expression.Call(null, method, arguments);
		}

		/// <summary>Creates a <see cref="T:System.Linq.Expressions.MethodCallExpression" /> that represents a call to a static (Shared in Visual Basic) method.</summary>
		/// <param name="method">The <see cref="T:System.Reflection.MethodInfo" /> that represents the target method.</param>
		/// <param name="arguments">A collection of <see cref="T:System.Linq.Expressions.Expression" /> that represents the call arguments.</param>
		/// <returns>A <see cref="T:System.Linq.Expressions.MethodCallExpression" /> that has the <see cref="P:System.Linq.Expressions.Expression.NodeType" /> property equal to <see cref="F:System.Linq.Expressions.ExpressionType.Call" /> and the <see cref="P:System.Linq.Expressions.MethodCallExpression.Object" /> and <see cref="P:System.Linq.Expressions.MethodCallExpression.Method" /> properties set to the specified values.</returns>
		public static MethodCallExpression Call(MethodInfo method, IEnumerable<Expression> arguments)
		{
			return Expression.Call(null, method, arguments);
		}

		/// <summary>Creates a <see cref="T:System.Linq.Expressions.MethodCallExpression" /> that represents a call to a method that takes no arguments.</summary>
		/// <param name="instance">An <see cref="T:System.Linq.Expressions.Expression" /> that specifies the instance for an instance method call (pass <see langword="null" /> for a <see langword="static" /> (<see langword="Shared" /> in Visual Basic) method).</param>
		/// <param name="method">A <see cref="T:System.Reflection.MethodInfo" /> to set the <see cref="P:System.Linq.Expressions.MethodCallExpression.Method" /> property equal to.</param>
		/// <returns>A <see cref="T:System.Linq.Expressions.MethodCallExpression" /> that has the <see cref="P:System.Linq.Expressions.Expression.NodeType" /> property equal to <see cref="F:System.Linq.Expressions.ExpressionType.Call" /> and the <see cref="P:System.Linq.Expressions.MethodCallExpression.Object" /> and <see cref="P:System.Linq.Expressions.MethodCallExpression.Method" /> properties set to the specified values.</returns>
		/// <exception cref="T:System.ArgumentNullException">
		///         <paramref name="method" /> is <see langword="null" />.-or-
		///         <paramref name="instance" /> is <see langword="null" /> and <paramref name="method" /> represents an instance method.</exception>
		/// <exception cref="T:System.ArgumentException">
		///         <paramref name="instance" />.Type is not assignable to the declaring type of the method represented by <paramref name="method" />.</exception>
		public static MethodCallExpression Call(Expression instance, MethodInfo method)
		{
			ContractUtils.RequiresNotNull(method, "method");
			ParameterInfo[] pis = Expression.ValidateMethodAndGetParameters(instance, method);
			Expression.ValidateArgumentCount(method, ExpressionType.Call, 0, pis);
			if (instance != null)
			{
				return new InstanceMethodCallExpression0(method, instance);
			}
			return new MethodCallExpression0(method);
		}

		/// <summary>Creates a <see cref="T:System.Linq.Expressions.MethodCallExpression" /> that represents a call to a method that takes arguments.</summary>
		/// <param name="instance">An <see cref="T:System.Linq.Expressions.Expression" /> that specifies the instance for an instance method call (pass <see langword="null" /> for a <see langword="static" /> (<see langword="Shared" /> in Visual Basic) method).</param>
		/// <param name="method">A <see cref="T:System.Reflection.MethodInfo" /> to set the <see cref="P:System.Linq.Expressions.MethodCallExpression.Method" /> property equal to.</param>
		/// <param name="arguments">An array of <see cref="T:System.Linq.Expressions.Expression" /> objects to use to populate the <see cref="P:System.Linq.Expressions.MethodCallExpression.Arguments" /> collection.</param>
		/// <returns>A <see cref="T:System.Linq.Expressions.MethodCallExpression" /> that has the <see cref="P:System.Linq.Expressions.Expression.NodeType" /> property equal to <see cref="F:System.Linq.Expressions.ExpressionType.Call" /> and the <see cref="P:System.Linq.Expressions.MethodCallExpression.Object" />, <see cref="P:System.Linq.Expressions.MethodCallExpression.Method" />, and <see cref="P:System.Linq.Expressions.MethodCallExpression.Arguments" /> properties set to the specified values.</returns>
		/// <exception cref="T:System.ArgumentNullException">
		///         <paramref name="method" /> is <see langword="null" />.-or-
		///         <paramref name="instance" /> is <see langword="null" /> and <paramref name="method" /> represents an instance method.-or-
		///         <paramref name="arguments" /> is not <see langword="null" /> and one or more of its elements is <see langword="null" />.</exception>
		/// <exception cref="T:System.ArgumentException">
		///         <paramref name="instance" />.Type is not assignable to the declaring type of the method represented by <paramref name="method" />.-or-The number of elements in <paramref name="arguments" /> does not equal the number of parameters for the method represented by <paramref name="method" />.-or-One or more of the elements of <paramref name="arguments" /> is not assignable to the corresponding parameter for the method represented by <paramref name="method" />.</exception>
		public static MethodCallExpression Call(Expression instance, MethodInfo method, params Expression[] arguments)
		{
			return Expression.Call(instance, method, arguments);
		}

		internal static MethodCallExpression Call(Expression instance, MethodInfo method, Expression arg0)
		{
			ContractUtils.RequiresNotNull(method, "method");
			ContractUtils.RequiresNotNull(arg0, "arg0");
			ParameterInfo[] array = Expression.ValidateMethodAndGetParameters(instance, method);
			Expression.ValidateArgumentCount(method, ExpressionType.Call, 1, array);
			arg0 = Expression.ValidateOneArgument(method, ExpressionType.Call, arg0, array[0], "method", "arg0");
			if (instance != null)
			{
				return new InstanceMethodCallExpression1(method, instance, arg0);
			}
			return new MethodCallExpression1(method, arg0);
		}

		/// <summary>Creates a <see cref="T:System.Linq.Expressions.MethodCallExpression" /> that represents a call to a method that takes two arguments.</summary>
		/// <param name="instance">An <see cref="T:System.Linq.Expressions.Expression" /> that specifies the instance for an instance call. (pass null for a static (Shared in Visual Basic) method).</param>
		/// <param name="method">The <see cref="T:System.Reflection.MethodInfo" /> that represents the target method.</param>
		/// <param name="arg0">The <see cref="T:System.Linq.Expressions.Expression" /> that represents the first argument.</param>
		/// <param name="arg1">The <see cref="T:System.Linq.Expressions.Expression" /> that represents the second argument.</param>
		/// <returns>A <see cref="T:System.Linq.Expressions.MethodCallExpression" /> that has the <see cref="P:System.Linq.Expressions.Expression.NodeType" /> property equal to <see cref="F:System.Linq.Expressions.ExpressionType.Call" /> and the <see cref="P:System.Linq.Expressions.MethodCallExpression.Object" /> and <see cref="P:System.Linq.Expressions.MethodCallExpression.Method" /> properties set to the specified values.</returns>
		public static MethodCallExpression Call(Expression instance, MethodInfo method, Expression arg0, Expression arg1)
		{
			ContractUtils.RequiresNotNull(method, "method");
			ContractUtils.RequiresNotNull(arg0, "arg0");
			ContractUtils.RequiresNotNull(arg1, "arg1");
			ParameterInfo[] array = Expression.ValidateMethodAndGetParameters(instance, method);
			Expression.ValidateArgumentCount(method, ExpressionType.Call, 2, array);
			arg0 = Expression.ValidateOneArgument(method, ExpressionType.Call, arg0, array[0], "method", "arg0");
			arg1 = Expression.ValidateOneArgument(method, ExpressionType.Call, arg1, array[1], "method", "arg1");
			if (instance != null)
			{
				return new InstanceMethodCallExpression2(method, instance, arg0, arg1);
			}
			return new MethodCallExpression2(method, arg0, arg1);
		}

		/// <summary>Creates a <see cref="T:System.Linq.Expressions.MethodCallExpression" /> that represents a call to a method that takes three arguments.</summary>
		/// <param name="instance">An <see cref="T:System.Linq.Expressions.Expression" /> that specifies the instance for an instance call. (pass null for a static (Shared in Visual Basic) method).</param>
		/// <param name="method">The <see cref="T:System.Reflection.MethodInfo" /> that represents the target method.</param>
		/// <param name="arg0">The <see cref="T:System.Linq.Expressions.Expression" /> that represents the first argument.</param>
		/// <param name="arg1">The <see cref="T:System.Linq.Expressions.Expression" /> that represents the second argument.</param>
		/// <param name="arg2">The <see cref="T:System.Linq.Expressions.Expression" /> that represents the third argument.</param>
		/// <returns>A <see cref="T:System.Linq.Expressions.MethodCallExpression" /> that has the <see cref="P:System.Linq.Expressions.Expression.NodeType" /> property equal to <see cref="F:System.Linq.Expressions.ExpressionType.Call" /> and the <see cref="P:System.Linq.Expressions.MethodCallExpression.Object" /> and <see cref="P:System.Linq.Expressions.MethodCallExpression.Method" /> properties set to the specified values.</returns>
		public static MethodCallExpression Call(Expression instance, MethodInfo method, Expression arg0, Expression arg1, Expression arg2)
		{
			ContractUtils.RequiresNotNull(method, "method");
			ContractUtils.RequiresNotNull(arg0, "arg0");
			ContractUtils.RequiresNotNull(arg1, "arg1");
			ContractUtils.RequiresNotNull(arg2, "arg2");
			ParameterInfo[] array = Expression.ValidateMethodAndGetParameters(instance, method);
			Expression.ValidateArgumentCount(method, ExpressionType.Call, 3, array);
			arg0 = Expression.ValidateOneArgument(method, ExpressionType.Call, arg0, array[0], "method", "arg0");
			arg1 = Expression.ValidateOneArgument(method, ExpressionType.Call, arg1, array[1], "method", "arg1");
			arg2 = Expression.ValidateOneArgument(method, ExpressionType.Call, arg2, array[2], "method", "arg2");
			if (instance != null)
			{
				return new InstanceMethodCallExpression3(method, instance, arg0, arg1, arg2);
			}
			return new MethodCallExpression3(method, arg0, arg1, arg2);
		}

		/// <summary>Creates a <see cref="T:System.Linq.Expressions.MethodCallExpression" /> that represents a call to a method by calling the appropriate factory method.</summary>
		/// <param name="instance">An <see cref="T:System.Linq.Expressions.Expression" /> whose <see cref="P:System.Linq.Expressions.Expression.Type" /> property value will be searched for a specific method.</param>
		/// <param name="methodName">The name of the method.</param>
		/// <param name="typeArguments">An array of <see cref="T:System.Type" /> objects that specify the type parameters of the generic method. This argument should be null when methodName specifies a non-generic method.</param>
		/// <param name="arguments">An array of <see cref="T:System.Linq.Expressions.Expression" /> objects that represents the arguments to the method.</param>
		/// <returns>A <see cref="T:System.Linq.Expressions.MethodCallExpression" /> that has the <see cref="P:System.Linq.Expressions.Expression.NodeType" /> property equal to <see cref="F:System.Linq.Expressions.ExpressionType.Call" />, the <see cref="P:System.Linq.Expressions.MethodCallExpression.Object" /> property equal to <paramref name="instance" />, <see cref="P:System.Linq.Expressions.MethodCallExpression.Method" /> set to the <see cref="T:System.Reflection.MethodInfo" /> that represents the specified instance method, and <see cref="P:System.Linq.Expressions.MethodCallExpression.Arguments" /> set to the specified arguments.</returns>
		/// <exception cref="T:System.ArgumentNullException">
		///         <paramref name="instance" /> or <paramref name="methodName" /> is <see langword="null" />.</exception>
		/// <exception cref="T:System.InvalidOperationException">No method whose name is <paramref name="methodName" />, whose type parameters match <paramref name="typeArguments" />, and whose parameter types match <paramref name="arguments" /> is found in <paramref name="instance" />.Type or its base types.-or-More than one method whose name is <paramref name="methodName" />, whose type parameters match <paramref name="typeArguments" />, and whose parameter types match <paramref name="arguments" /> is found in <paramref name="instance" />.Type or its base types.</exception>
		public static MethodCallExpression Call(Expression instance, string methodName, Type[] typeArguments, params Expression[] arguments)
		{
			ContractUtils.RequiresNotNull(instance, "instance");
			ContractUtils.RequiresNotNull(methodName, "methodName");
			if (arguments == null)
			{
				arguments = Array.Empty<Expression>();
			}
			BindingFlags flags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.FlattenHierarchy;
			return Expression.Call(instance, Expression.FindMethod(instance.Type, methodName, typeArguments, arguments, flags), arguments);
		}

		/// <summary>Creates a <see cref="T:System.Linq.Expressions.MethodCallExpression" /> that represents a call to a <see langword="static" /> (<see langword="Shared" /> in Visual Basic) method by calling the appropriate factory method.</summary>
		/// <param name="type">The <see cref="T:System.Type" /> that specifies the type that contains the specified <see langword="static" /> (<see langword="Shared" /> in Visual Basic) method.</param>
		/// <param name="methodName">The name of the method.</param>
		/// <param name="typeArguments">An array of <see cref="T:System.Type" /> objects that specify the type parameters of the generic method. This argument should be null when methodName specifies a non-generic method.</param>
		/// <param name="arguments">An array of <see cref="T:System.Linq.Expressions.Expression" /> objects that represent the arguments to the method.</param>
		/// <returns>A <see cref="T:System.Linq.Expressions.MethodCallExpression" /> that has the <see cref="P:System.Linq.Expressions.Expression.NodeType" /> property equal to <see cref="F:System.Linq.Expressions.ExpressionType.Call" />, the <see cref="P:System.Linq.Expressions.MethodCallExpression.Method" /> property set to the <see cref="T:System.Reflection.MethodInfo" /> that represents the specified <see langword="static" /> (<see langword="Shared" /> in Visual Basic) method, and the <see cref="P:System.Linq.Expressions.MethodCallExpression.Arguments" /> property set to the specified arguments.</returns>
		/// <exception cref="T:System.ArgumentNullException">
		///         <paramref name="type" /> or <paramref name="methodName" /> is <see langword="null" />.</exception>
		/// <exception cref="T:System.InvalidOperationException">No method whose name is <paramref name="methodName" />, whose type parameters match <paramref name="typeArguments" />, and whose parameter types match <paramref name="arguments" /> is found in <paramref name="type" /> or its base types.-or-More than one method whose name is <paramref name="methodName" />, whose type parameters match <paramref name="typeArguments" />, and whose parameter types match <paramref name="arguments" /> is found in <paramref name="type" /> or its base types.</exception>
		public static MethodCallExpression Call(Type type, string methodName, Type[] typeArguments, params Expression[] arguments)
		{
			ContractUtils.RequiresNotNull(type, "type");
			ContractUtils.RequiresNotNull(methodName, "methodName");
			if (arguments == null)
			{
				arguments = Array.Empty<Expression>();
			}
			BindingFlags flags = BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.FlattenHierarchy;
			return Expression.Call(null, Expression.FindMethod(type, methodName, typeArguments, arguments, flags), arguments);
		}

		/// <summary>Creates a <see cref="T:System.Linq.Expressions.MethodCallExpression" /> that represents a call to a method that takes arguments.</summary>
		/// <param name="instance">An <see cref="T:System.Linq.Expressions.Expression" /> to set the <see cref="P:System.Linq.Expressions.MethodCallExpression.Object" /> property equal to (pass <see langword="null" /> for a <see langword="static" /> (<see langword="Shared" /> in Visual Basic) method).</param>
		/// <param name="method">A <see cref="T:System.Reflection.MethodInfo" /> to set the <see cref="P:System.Linq.Expressions.MethodCallExpression.Method" /> property equal to.</param>
		/// <param name="arguments">An <see cref="T:System.Collections.Generic.IEnumerable`1" /> that contains <see cref="T:System.Linq.Expressions.Expression" /> objects to use to populate the <see cref="P:System.Linq.Expressions.MethodCallExpression.Arguments" /> collection.</param>
		/// <returns>A <see cref="T:System.Linq.Expressions.MethodCallExpression" /> that has the <see cref="P:System.Linq.Expressions.Expression.NodeType" /> property equal to <see cref="F:System.Linq.Expressions.ExpressionType.Call" /> and the <see cref="P:System.Linq.Expressions.MethodCallExpression.Object" />, <see cref="P:System.Linq.Expressions.MethodCallExpression.Method" />, and <see cref="P:System.Linq.Expressions.MethodCallExpression.Arguments" /> properties set to the specified values.</returns>
		/// <exception cref="T:System.ArgumentNullException">
		///         <paramref name="method" /> is <see langword="null" />.-or-
		///         <paramref name="instance" /> is <see langword="null" /> and <paramref name="method" /> represents an instance method.</exception>
		/// <exception cref="T:System.ArgumentException">
		///         <paramref name="instance" />.Type is not assignable to the declaring type of the method represented by <paramref name="method" />.-or-The number of elements in <paramref name="arguments" /> does not equal the number of parameters for the method represented by <paramref name="method" />.-or-One or more of the elements of <paramref name="arguments" /> is not assignable to the corresponding parameter for the method represented by <paramref name="method" />.</exception>
		public static MethodCallExpression Call(Expression instance, MethodInfo method, IEnumerable<Expression> arguments)
		{
			IReadOnlyList<Expression> readOnlyList = (arguments as IReadOnlyList<Expression>) ?? arguments.ToReadOnly<Expression>();
			int count = readOnlyList.Count;
			switch (count)
			{
			case 0:
				return Expression.Call(instance, method);
			case 1:
				return Expression.Call(instance, method, readOnlyList[0]);
			case 2:
				return Expression.Call(instance, method, readOnlyList[0], readOnlyList[1]);
			case 3:
				return Expression.Call(instance, method, readOnlyList[0], readOnlyList[1], readOnlyList[2]);
			default:
			{
				if (instance == null)
				{
					if (count == 4)
					{
						return Expression.Call(method, readOnlyList[0], readOnlyList[1], readOnlyList[2], readOnlyList[3]);
					}
					if (count == 5)
					{
						return Expression.Call(method, readOnlyList[0], readOnlyList[1], readOnlyList[2], readOnlyList[3], readOnlyList[4]);
					}
				}
				ContractUtils.RequiresNotNull(method, "method");
				ReadOnlyCollection<Expression> args = readOnlyList.ToReadOnly<Expression>();
				Expression.ValidateMethodInfo(method, "method");
				Expression.ValidateStaticOrInstanceMethod(instance, method);
				Expression.ValidateArgumentTypes(method, ExpressionType.Call, ref args, "method");
				if (instance == null)
				{
					return new MethodCallExpressionN(method, args);
				}
				return new InstanceMethodCallExpressionN(method, instance, args);
			}
			}
		}

		private static ParameterInfo[] ValidateMethodAndGetParameters(Expression instance, MethodInfo method)
		{
			Expression.ValidateMethodInfo(method, "method");
			Expression.ValidateStaticOrInstanceMethod(instance, method);
			return Expression.GetParametersForValidation(method, ExpressionType.Call);
		}

		private static void ValidateStaticOrInstanceMethod(Expression instance, MethodInfo method)
		{
			if (method.IsStatic)
			{
				if (instance != null)
				{
					throw Error.OnlyStaticMethodsHaveNullInstance();
				}
			}
			else
			{
				if (instance == null)
				{
					throw Error.OnlyStaticMethodsHaveNullInstance();
				}
				ExpressionUtils.RequiresCanRead(instance, "instance");
				Expression.ValidateCallInstanceType(instance.Type, method);
			}
		}

		private static void ValidateCallInstanceType(Type instanceType, MethodInfo method)
		{
			if (!TypeUtils.IsValidInstanceType(method, instanceType))
			{
				throw Error.InstanceAndMethodTypeMismatch(method, method.DeclaringType, instanceType);
			}
		}

		private static void ValidateArgumentTypes(MethodBase method, ExpressionType nodeKind, ref ReadOnlyCollection<Expression> arguments, string methodParamName)
		{
			ExpressionUtils.ValidateArgumentTypes(method, nodeKind, ref arguments, methodParamName);
		}

		private static ParameterInfo[] GetParametersForValidation(MethodBase method, ExpressionType nodeKind)
		{
			return ExpressionUtils.GetParametersForValidation(method, nodeKind);
		}

		private static void ValidateArgumentCount(MethodBase method, ExpressionType nodeKind, int count, ParameterInfo[] pis)
		{
			ExpressionUtils.ValidateArgumentCount(method, nodeKind, count, pis);
		}

		private static Expression ValidateOneArgument(MethodBase method, ExpressionType nodeKind, Expression arg, ParameterInfo pi, string methodParamName, string argumentParamName)
		{
			return ExpressionUtils.ValidateOneArgument(method, nodeKind, arg, pi, methodParamName, argumentParamName, -1);
		}

		private static bool TryQuote(Type parameterType, ref Expression argument)
		{
			return ExpressionUtils.TryQuote(parameterType, ref argument);
		}

		private static MethodInfo FindMethod(Type type, string methodName, Type[] typeArgs, Expression[] args, BindingFlags flags)
		{
			int num = 0;
			MethodInfo methodInfo = null;
			foreach (MethodInfo methodInfo2 in type.GetMethods(flags))
			{
				if (methodInfo2.Name.Equals(methodName, StringComparison.OrdinalIgnoreCase))
				{
					MethodInfo methodInfo3 = Expression.ApplyTypeArgs(methodInfo2, typeArgs);
					if (methodInfo3 != null && Expression.IsCompatible(methodInfo3, args))
					{
						if (methodInfo == null || (!methodInfo.IsPublic && methodInfo3.IsPublic))
						{
							methodInfo = methodInfo3;
							num = 1;
						}
						else if (methodInfo.IsPublic == methodInfo3.IsPublic)
						{
							num++;
						}
					}
				}
			}
			if (num == 0)
			{
				if (typeArgs != null && typeArgs.Length != 0)
				{
					throw Error.GenericMethodWithArgsDoesNotExistOnType(methodName, type);
				}
				throw Error.MethodWithArgsDoesNotExistOnType(methodName, type);
			}
			else
			{
				if (num > 1)
				{
					throw Error.MethodWithMoreThanOneMatch(methodName, type);
				}
				return methodInfo;
			}
		}

		private static bool IsCompatible(MethodBase m, Expression[] arguments)
		{
			ParameterInfo[] parametersCached = m.GetParametersCached();
			if (parametersCached.Length != arguments.Length)
			{
				return false;
			}
			for (int i = 0; i < arguments.Length; i++)
			{
				Expression expression = arguments[i];
				ContractUtils.RequiresNotNull(expression, "arguments");
				Type type = expression.Type;
				Type type2 = parametersCached[i].ParameterType;
				if (type2.IsByRef)
				{
					type2 = type2.GetElementType();
				}
				if (!TypeUtils.AreReferenceAssignable(type2, type) && (!TypeUtils.IsSameOrSubclass(typeof(LambdaExpression), type2) || !type2.IsAssignableFrom(expression.GetType())))
				{
					return false;
				}
			}
			return true;
		}

		private static MethodInfo ApplyTypeArgs(MethodInfo m, Type[] typeArgs)
		{
			if (typeArgs == null || typeArgs.Length == 0)
			{
				if (!m.IsGenericMethodDefinition)
				{
					return m;
				}
			}
			else if (m.IsGenericMethodDefinition && m.GetGenericArguments().Length == typeArgs.Length)
			{
				return m.MakeGenericMethod(typeArgs);
			}
			return null;
		}

		/// <summary>Creates a <see cref="T:System.Linq.Expressions.MethodCallExpression" /> that represents applying an array index operator to a multidimensional array.</summary>
		/// <param name="array">An array of <see cref="T:System.Linq.Expressions.Expression" /> instances - indexes for the array index operation.</param>
		/// <param name="indexes">An array of <see cref="T:System.Linq.Expressions.Expression" /> objects to use to populate the <see cref="P:System.Linq.Expressions.MethodCallExpression.Arguments" /> collection.</param>
		/// <returns>A <see cref="T:System.Linq.Expressions.MethodCallExpression" /> that has the <see cref="P:System.Linq.Expressions.Expression.NodeType" /> property equal to <see cref="F:System.Linq.Expressions.ExpressionType.Call" /> and the <see cref="P:System.Linq.Expressions.MethodCallExpression.Object" /> and <see cref="P:System.Linq.Expressions.MethodCallExpression.Arguments" /> properties set to the specified values.</returns>
		/// <exception cref="T:System.ArgumentNullException">
		///         <paramref name="array" /> or <paramref name="indexes" /> is <see langword="null" />.</exception>
		/// <exception cref="T:System.ArgumentException">
		///         <paramref name="array" />.Type does not represent an array type.-or-The rank of <paramref name="array" />.Type does not match the number of elements in <paramref name="indexes" />.-or-The <see cref="P:System.Linq.Expressions.Expression.Type" /> property of one or more elements of <paramref name="indexes" /> does not represent the <see cref="T:System.Int32" /> type.</exception>
		public static MethodCallExpression ArrayIndex(Expression array, params Expression[] indexes)
		{
			return Expression.ArrayIndex(array, indexes);
		}

		/// <summary>Creates a <see cref="T:System.Linq.Expressions.MethodCallExpression" /> that represents applying an array index operator to an array of rank more than one.</summary>
		/// <param name="array">An <see cref="T:System.Linq.Expressions.Expression" /> to set the <see cref="P:System.Linq.Expressions.MethodCallExpression.Object" /> property equal to.</param>
		/// <param name="indexes">An <see cref="T:System.Collections.Generic.IEnumerable`1" /> that contains <see cref="T:System.Linq.Expressions.Expression" /> objects to use to populate the <see cref="P:System.Linq.Expressions.MethodCallExpression.Arguments" /> collection.</param>
		/// <returns>A <see cref="T:System.Linq.Expressions.MethodCallExpression" /> that has the <see cref="P:System.Linq.Expressions.Expression.NodeType" /> property equal to <see cref="F:System.Linq.Expressions.ExpressionType.Call" /> and the <see cref="P:System.Linq.Expressions.MethodCallExpression.Object" /> and <see cref="P:System.Linq.Expressions.MethodCallExpression.Arguments" /> properties set to the specified values.</returns>
		/// <exception cref="T:System.ArgumentNullException">
		///         <paramref name="array" /> or <paramref name="indexes" /> is <see langword="null" />.</exception>
		/// <exception cref="T:System.ArgumentException">
		///         <paramref name="array" />.Type does not represent an array type.-or-The rank of <paramref name="array" />.Type does not match the number of elements in <paramref name="indexes" />.-or-The <see cref="P:System.Linq.Expressions.Expression.Type" /> property of one or more elements of <paramref name="indexes" /> does not represent the <see cref="T:System.Int32" /> type.</exception>
		public static MethodCallExpression ArrayIndex(Expression array, IEnumerable<Expression> indexes)
		{
			ExpressionUtils.RequiresCanRead(array, "array", -1);
			ContractUtils.RequiresNotNull(indexes, "indexes");
			Type type = array.Type;
			if (!type.IsArray)
			{
				throw Error.ArgumentMustBeArray("array");
			}
			ReadOnlyCollection<Expression> readOnlyCollection = indexes.ToReadOnly<Expression>();
			if (type.GetArrayRank() != readOnlyCollection.Count)
			{
				throw Error.IncorrectNumberOfIndexes();
			}
			int i = 0;
			int count = readOnlyCollection.Count;
			while (i < count)
			{
				Expression expression = readOnlyCollection[i];
				ExpressionUtils.RequiresCanRead(expression, "indexes", i);
				if (expression.Type != typeof(int))
				{
					throw Error.ArgumentMustBeArrayIndexType("indexes", i);
				}
				i++;
			}
			MethodInfo method = array.Type.GetMethod("Get", BindingFlags.Instance | BindingFlags.Public);
			return Expression.Call(array, method, readOnlyCollection);
		}

		/// <summary>Creates a <see cref="T:System.Linq.Expressions.NewArrayExpression" /> that represents creating a one-dimensional array and initializing it from a list of elements.</summary>
		/// <param name="type">A <see cref="T:System.Type" /> that represents the element type of the array.</param>
		/// <param name="initializers">An array of <see cref="T:System.Linq.Expressions.Expression" /> objects to use to populate the <see cref="P:System.Linq.Expressions.NewArrayExpression.Expressions" /> collection.</param>
		/// <returns>A <see cref="T:System.Linq.Expressions.NewArrayExpression" /> that has the <see cref="P:System.Linq.Expressions.Expression.NodeType" /> property equal to <see cref="F:System.Linq.Expressions.ExpressionType.NewArrayInit" /> and the <see cref="P:System.Linq.Expressions.NewArrayExpression.Expressions" /> property set to the specified value.</returns>
		/// <exception cref="T:System.ArgumentNullException">
		///         <paramref name="type" /> or <paramref name="initializers" /> is <see langword="null" />.-or-An element of <paramref name="initializers" /> is <see langword="null" />.</exception>
		/// <exception cref="T:System.InvalidOperationException">The <see cref="P:System.Linq.Expressions.Expression.Type" /> property of an element of <paramref name="initializers" /> represents a type that is not assignable to the type <paramref name="type" />.</exception>
		public static NewArrayExpression NewArrayInit(Type type, params Expression[] initializers)
		{
			return Expression.NewArrayInit(type, initializers);
		}

		/// <summary>Creates a <see cref="T:System.Linq.Expressions.NewArrayExpression" /> that represents creating a one-dimensional array and initializing it from a list of elements.</summary>
		/// <param name="type">A <see cref="T:System.Type" /> that represents the element type of the array.</param>
		/// <param name="initializers">An <see cref="T:System.Collections.Generic.IEnumerable`1" /> that contains <see cref="T:System.Linq.Expressions.Expression" /> objects to use to populate the <see cref="P:System.Linq.Expressions.NewArrayExpression.Expressions" /> collection.</param>
		/// <returns>A <see cref="T:System.Linq.Expressions.NewArrayExpression" /> that has the <see cref="P:System.Linq.Expressions.Expression.NodeType" /> property equal to <see cref="F:System.Linq.Expressions.ExpressionType.NewArrayInit" /> and the <see cref="P:System.Linq.Expressions.NewArrayExpression.Expressions" /> property set to the specified value.</returns>
		/// <exception cref="T:System.ArgumentNullException">
		///         <paramref name="type" /> or <paramref name="initializers" /> is <see langword="null" />.-or-An element of <paramref name="initializers" /> is <see langword="null" />.</exception>
		/// <exception cref="T:System.InvalidOperationException">The <see cref="P:System.Linq.Expressions.Expression.Type" /> property of an element of <paramref name="initializers" /> represents a type that is not assignable to the type that <paramref name="type" /> represents.</exception>
		public static NewArrayExpression NewArrayInit(Type type, IEnumerable<Expression> initializers)
		{
			ContractUtils.RequiresNotNull(type, "type");
			ContractUtils.RequiresNotNull(initializers, "initializers");
			if (type == typeof(void))
			{
				throw Error.ArgumentCannotBeOfTypeVoid("type");
			}
			TypeUtils.ValidateType(type, "type");
			ReadOnlyCollection<Expression> readOnlyCollection = initializers.ToReadOnly<Expression>();
			Expression[] array = null;
			int i = 0;
			int count = readOnlyCollection.Count;
			while (i < count)
			{
				Expression expression = readOnlyCollection[i];
				ExpressionUtils.RequiresCanRead(expression, "initializers", i);
				if (!TypeUtils.AreReferenceAssignable(type, expression.Type))
				{
					if (!Expression.TryQuote(type, ref expression))
					{
						throw Error.ExpressionTypeCannotInitializeArrayType(expression.Type, type);
					}
					if (array == null)
					{
						array = new Expression[readOnlyCollection.Count];
						for (int j = 0; j < i; j++)
						{
							array[j] = readOnlyCollection[j];
						}
					}
				}
				if (array != null)
				{
					array[i] = expression;
				}
				i++;
			}
			if (array != null)
			{
				readOnlyCollection = new TrueReadOnlyCollection<Expression>(array);
			}
			return NewArrayExpression.Make(ExpressionType.NewArrayInit, type.MakeArrayType(), readOnlyCollection);
		}

		/// <summary>Creates a <see cref="T:System.Linq.Expressions.NewArrayExpression" /> that represents creating an array that has a specified rank.</summary>
		/// <param name="type">A <see cref="T:System.Type" /> that represents the element type of the array.</param>
		/// <param name="bounds">An array of <see cref="T:System.Linq.Expressions.Expression" /> objects to use to populate the <see cref="P:System.Linq.Expressions.NewArrayExpression.Expressions" /> collection.</param>
		/// <returns>A <see cref="T:System.Linq.Expressions.NewArrayExpression" /> that has the <see cref="P:System.Linq.Expressions.Expression.NodeType" /> property equal to <see cref="F:System.Linq.Expressions.ExpressionType.NewArrayBounds" /> and the <see cref="P:System.Linq.Expressions.NewArrayExpression.Expressions" /> property set to the specified value.</returns>
		/// <exception cref="T:System.ArgumentNullException">
		///         <paramref name="type" /> or <paramref name="bounds" /> is <see langword="null" />.-or-An element of <paramref name="bounds" /> is <see langword="null" />.</exception>
		/// <exception cref="T:System.ArgumentException">The <see cref="P:System.Linq.Expressions.Expression.Type" /> property of an element of <paramref name="bounds" /> does not represent an integral type.</exception>
		public static NewArrayExpression NewArrayBounds(Type type, params Expression[] bounds)
		{
			return Expression.NewArrayBounds(type, bounds);
		}

		/// <summary>Creates a <see cref="T:System.Linq.Expressions.NewArrayExpression" /> that represents creating an array that has a specified rank.</summary>
		/// <param name="type">A <see cref="T:System.Type" /> that represents the element type of the array.</param>
		/// <param name="bounds">An <see cref="T:System.Collections.Generic.IEnumerable`1" /> that contains <see cref="T:System.Linq.Expressions.Expression" /> objects to use to populate the <see cref="P:System.Linq.Expressions.NewArrayExpression.Expressions" /> collection.</param>
		/// <returns>A <see cref="T:System.Linq.Expressions.NewArrayExpression" /> that has the <see cref="P:System.Linq.Expressions.Expression.NodeType" /> property equal to <see cref="F:System.Linq.Expressions.ExpressionType.NewArrayBounds" /> and the <see cref="P:System.Linq.Expressions.NewArrayExpression.Expressions" /> property set to the specified value.</returns>
		/// <exception cref="T:System.ArgumentNullException">
		///         <paramref name="type" /> or <paramref name="bounds" /> is <see langword="null" />.-or-An element of <paramref name="bounds" /> is <see langword="null" />.</exception>
		/// <exception cref="T:System.ArgumentException">The <see cref="P:System.Linq.Expressions.Expression.Type" /> property of an element of <paramref name="bounds" /> does not represent an integral type.</exception>
		public static NewArrayExpression NewArrayBounds(Type type, IEnumerable<Expression> bounds)
		{
			ContractUtils.RequiresNotNull(type, "type");
			ContractUtils.RequiresNotNull(bounds, "bounds");
			if (type == typeof(void))
			{
				throw Error.ArgumentCannotBeOfTypeVoid("type");
			}
			TypeUtils.ValidateType(type, "type");
			ReadOnlyCollection<Expression> readOnlyCollection = bounds.ToReadOnly<Expression>();
			int count = readOnlyCollection.Count;
			if (count <= 0)
			{
				throw Error.BoundsCannotBeLessThanOne("bounds");
			}
			for (int i = 0; i < count; i++)
			{
				Expression expression = readOnlyCollection[i];
				ExpressionUtils.RequiresCanRead(expression, "bounds", i);
				if (!expression.Type.IsInteger())
				{
					throw Error.ArgumentMustBeInteger("bounds", i);
				}
			}
			Type type2;
			if (count == 1)
			{
				type2 = type.MakeArrayType();
			}
			else
			{
				type2 = type.MakeArrayType(count);
			}
			return NewArrayExpression.Make(ExpressionType.NewArrayBounds, type2, readOnlyCollection);
		}

		/// <summary>Creates a <see cref="T:System.Linq.Expressions.NewExpression" /> that represents calling the specified constructor that takes no arguments.</summary>
		/// <param name="constructor">The <see cref="T:System.Reflection.ConstructorInfo" /> to set the <see cref="P:System.Linq.Expressions.NewExpression.Constructor" /> property equal to.</param>
		/// <returns>A <see cref="T:System.Linq.Expressions.NewExpression" /> that has the <see cref="P:System.Linq.Expressions.Expression.NodeType" /> property equal to <see cref="F:System.Linq.Expressions.ExpressionType.New" /> and the <see cref="P:System.Linq.Expressions.NewExpression.Constructor" /> property set to the specified value.</returns>
		/// <exception cref="T:System.ArgumentNullException">
		///         <paramref name="constructor" /> is <see langword="null" />.</exception>
		/// <exception cref="T:System.ArgumentException">The constructor that <paramref name="constructor" /> represents has at least one parameter.</exception>
		public static NewExpression New(ConstructorInfo constructor)
		{
			return Expression.New(constructor, null);
		}

		/// <summary>Creates a <see cref="T:System.Linq.Expressions.NewExpression" /> that represents calling the specified constructor with the specified arguments.</summary>
		/// <param name="constructor">The <see cref="T:System.Reflection.ConstructorInfo" /> to set the <see cref="P:System.Linq.Expressions.NewExpression.Constructor" /> property equal to.</param>
		/// <param name="arguments">An array of <see cref="T:System.Linq.Expressions.Expression" /> objects to use to populate the <see cref="P:System.Linq.Expressions.NewExpression.Arguments" /> collection.</param>
		/// <returns>A <see cref="T:System.Linq.Expressions.NewExpression" /> that has the <see cref="P:System.Linq.Expressions.Expression.NodeType" /> property equal to <see cref="F:System.Linq.Expressions.ExpressionType.New" /> and the <see cref="P:System.Linq.Expressions.NewExpression.Constructor" /> and <see cref="P:System.Linq.Expressions.NewExpression.Arguments" /> properties set to the specified values.</returns>
		/// <exception cref="T:System.ArgumentNullException">
		///         <paramref name="constructor" /> is <see langword="null" />.-or-An element of <paramref name="arguments" /> is <see langword="null" />.</exception>
		/// <exception cref="T:System.ArgumentException">The length of <paramref name="arguments" /> does match the number of parameters for the constructor that <paramref name="constructor" /> represents.-or-The <see cref="P:System.Linq.Expressions.Expression.Type" /> property of an element of <paramref name="arguments" /> is not assignable to the type of the corresponding parameter of the constructor that <paramref name="constructor" /> represents.</exception>
		public static NewExpression New(ConstructorInfo constructor, params Expression[] arguments)
		{
			return Expression.New(constructor, arguments);
		}

		/// <summary>Creates a <see cref="T:System.Linq.Expressions.NewExpression" /> that represents calling the specified constructor with the specified arguments.</summary>
		/// <param name="constructor">The <see cref="T:System.Reflection.ConstructorInfo" /> to set the <see cref="P:System.Linq.Expressions.NewExpression.Constructor" /> property equal to.</param>
		/// <param name="arguments">An <see cref="T:System.Collections.Generic.IEnumerable`1" /> that contains <see cref="T:System.Linq.Expressions.Expression" /> objects to use to populate the <see cref="P:System.Linq.Expressions.NewExpression.Arguments" /> collection.</param>
		/// <returns>A <see cref="T:System.Linq.Expressions.NewExpression" /> that has the <see cref="P:System.Linq.Expressions.Expression.NodeType" /> property equal to <see cref="F:System.Linq.Expressions.ExpressionType.New" /> and the <see cref="P:System.Linq.Expressions.NewExpression.Constructor" /> and <see cref="P:System.Linq.Expressions.NewExpression.Arguments" /> properties set to the specified values.</returns>
		/// <exception cref="T:System.ArgumentNullException">
		///         <paramref name="constructor" /> is <see langword="null" />.-or-An element of <paramref name="arguments" /> is <see langword="null" />.</exception>
		/// <exception cref="T:System.ArgumentException">The <paramref name="arguments" /> parameter does not contain the same number of elements as the number of parameters for the constructor that <paramref name="constructor" /> represents.-or-The <see cref="P:System.Linq.Expressions.Expression.Type" /> property of an element of <paramref name="arguments" /> is not assignable to the type of the corresponding parameter of the constructor that <paramref name="constructor" /> represents.</exception>
		public static NewExpression New(ConstructorInfo constructor, IEnumerable<Expression> arguments)
		{
			ContractUtils.RequiresNotNull(constructor, "constructor");
			ContractUtils.RequiresNotNull(constructor.DeclaringType, "constructor.DeclaringType");
			TypeUtils.ValidateType(constructor.DeclaringType, "constructor", true, true);
			Expression.ValidateConstructor(constructor, "constructor");
			ReadOnlyCollection<Expression> arguments2 = arguments.ToReadOnly<Expression>();
			Expression.ValidateArgumentTypes(constructor, ExpressionType.New, ref arguments2, "constructor");
			return new NewExpression(constructor, arguments2, null);
		}

		/// <summary>Creates a <see cref="T:System.Linq.Expressions.NewExpression" /> that represents calling the specified constructor with the specified arguments. The members that access the constructor initialized fields are specified.</summary>
		/// <param name="constructor">The <see cref="T:System.Reflection.ConstructorInfo" /> to set the <see cref="P:System.Linq.Expressions.NewExpression.Constructor" /> property equal to.</param>
		/// <param name="arguments">An <see cref="T:System.Collections.Generic.IEnumerable`1" /> that contains <see cref="T:System.Linq.Expressions.Expression" /> objects to use to populate the <see cref="P:System.Linq.Expressions.NewExpression.Arguments" /> collection.</param>
		/// <param name="members">An <see cref="T:System.Collections.Generic.IEnumerable`1" /> that contains <see cref="T:System.Reflection.MemberInfo" /> objects to use to populate the <see cref="P:System.Linq.Expressions.NewExpression.Members" /> collection.</param>
		/// <returns>A <see cref="T:System.Linq.Expressions.NewExpression" /> that has the <see cref="P:System.Linq.Expressions.Expression.NodeType" /> property equal to <see cref="F:System.Linq.Expressions.ExpressionType.New" /> and the <see cref="P:System.Linq.Expressions.NewExpression.Constructor" />, <see cref="P:System.Linq.Expressions.NewExpression.Arguments" /> and <see cref="P:System.Linq.Expressions.NewExpression.Members" /> properties set to the specified values.</returns>
		/// <exception cref="T:System.ArgumentNullException">
		///         <paramref name="constructor" /> is <see langword="null" />.-or-An element of <paramref name="arguments" /> is <see langword="null" />.-or-An element of <paramref name="members" /> is <see langword="null" />.</exception>
		/// <exception cref="T:System.ArgumentException">The <paramref name="arguments" /> parameter does not contain the same number of elements as the number of parameters for the constructor that <paramref name="constructor" /> represents.-or-The <see cref="P:System.Linq.Expressions.Expression.Type" /> property of an element of <paramref name="arguments" /> is not assignable to the type of the corresponding parameter of the constructor that <paramref name="constructor" /> represents.-or-The <paramref name="members" /> parameter does not have the same number of elements as <paramref name="arguments" />.-or-An element of <paramref name="arguments" /> has a <see cref="P:System.Linq.Expressions.Expression.Type" /> property that represents a type that is not assignable to the type of the member that is represented by the corresponding element of <paramref name="members" />.</exception>
		public static NewExpression New(ConstructorInfo constructor, IEnumerable<Expression> arguments, IEnumerable<MemberInfo> members)
		{
			ContractUtils.RequiresNotNull(constructor, "constructor");
			ContractUtils.RequiresNotNull(constructor.DeclaringType, "constructor.DeclaringType");
			TypeUtils.ValidateType(constructor.DeclaringType, "constructor", true, true);
			Expression.ValidateConstructor(constructor, "constructor");
			ReadOnlyCollection<MemberInfo> members2 = members.ToReadOnly<MemberInfo>();
			ReadOnlyCollection<Expression> arguments2 = arguments.ToReadOnly<Expression>();
			Expression.ValidateNewArgs(constructor, ref arguments2, ref members2);
			return new NewExpression(constructor, arguments2, members2);
		}

		/// <summary>Creates a <see cref="T:System.Linq.Expressions.NewExpression" /> that represents calling the specified constructor with the specified arguments. The members that access the constructor initialized fields are specified as an array.</summary>
		/// <param name="constructor">The <see cref="T:System.Reflection.ConstructorInfo" /> to set the <see cref="P:System.Linq.Expressions.NewExpression.Constructor" /> property equal to.</param>
		/// <param name="arguments">An <see cref="T:System.Collections.Generic.IEnumerable`1" /> that contains <see cref="T:System.Linq.Expressions.Expression" /> objects to use to populate the <see cref="P:System.Linq.Expressions.NewExpression.Arguments" /> collection.</param>
		/// <param name="members">An array of <see cref="T:System.Reflection.MemberInfo" /> objects to use to populate the <see cref="P:System.Linq.Expressions.NewExpression.Members" /> collection.</param>
		/// <returns>A <see cref="T:System.Linq.Expressions.NewExpression" /> that has the <see cref="P:System.Linq.Expressions.Expression.NodeType" /> property equal to <see cref="F:System.Linq.Expressions.ExpressionType.New" /> and the <see cref="P:System.Linq.Expressions.NewExpression.Constructor" />, <see cref="P:System.Linq.Expressions.NewExpression.Arguments" /> and <see cref="P:System.Linq.Expressions.NewExpression.Members" /> properties set to the specified values.</returns>
		/// <exception cref="T:System.ArgumentNullException">
		///         <paramref name="constructor" /> is <see langword="null" />.-or-An element of <paramref name="arguments" /> is <see langword="null" />.-or-An element of <paramref name="members" /> is <see langword="null" />.</exception>
		/// <exception cref="T:System.ArgumentException">The <paramref name="arguments" /> parameter does not contain the same number of elements as the number of parameters for the constructor that <paramref name="constructor" /> represents.-or-The <see cref="P:System.Linq.Expressions.Expression.Type" /> property of an element of <paramref name="arguments" /> is not assignable to the type of the corresponding parameter of the constructor that <paramref name="constructor" /> represents.-or-The <paramref name="members" /> parameter does not have the same number of elements as <paramref name="arguments" />.-or-An element of <paramref name="arguments" /> has a <see cref="P:System.Linq.Expressions.Expression.Type" /> property that represents a type that is not assignable to the type of the member that is represented by the corresponding element of <paramref name="members" />.</exception>
		public static NewExpression New(ConstructorInfo constructor, IEnumerable<Expression> arguments, params MemberInfo[] members)
		{
			return Expression.New(constructor, arguments, members);
		}

		/// <summary>Creates a <see cref="T:System.Linq.Expressions.NewExpression" /> that represents calling the parameterless constructor of the specified type.</summary>
		/// <param name="type">A <see cref="T:System.Type" /> that has a constructor that takes no arguments.</param>
		/// <returns>A <see cref="T:System.Linq.Expressions.NewExpression" /> that has the <see cref="P:System.Linq.Expressions.Expression.NodeType" /> property equal to <see cref="F:System.Linq.Expressions.ExpressionType.New" /> and the <see cref="P:System.Linq.Expressions.NewExpression.Constructor" /> property set to the <see cref="T:System.Reflection.ConstructorInfo" /> that represents the constructor without parameters for the specified type.</returns>
		/// <exception cref="T:System.ArgumentNullException">
		///         <paramref name="type" /> is <see langword="null" />.</exception>
		/// <exception cref="T:System.ArgumentException">The type that <paramref name="type" /> represents does not have a constructor without parameters.</exception>
		public static NewExpression New(Type type)
		{
			ContractUtils.RequiresNotNull(type, "type");
			if (type == typeof(void))
			{
				throw Error.ArgumentCannotBeOfTypeVoid("type");
			}
			TypeUtils.ValidateType(type, "type");
			if (type.IsValueType)
			{
				return new NewValueTypeExpression(type, EmptyReadOnlyCollection<Expression>.Instance, null);
			}
			ConstructorInfo constructorInfo = type.GetConstructors(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic).SingleOrDefault((ConstructorInfo c) => c.GetParametersCached().Length == 0);
			if (constructorInfo == null)
			{
				throw Error.TypeMissingDefaultConstructor(type, "type");
			}
			return Expression.New(constructorInfo);
		}

		private static void ValidateNewArgs(ConstructorInfo constructor, ref ReadOnlyCollection<Expression> arguments, ref ReadOnlyCollection<MemberInfo> members)
		{
			ParameterInfo[] parametersCached;
			if ((parametersCached = constructor.GetParametersCached()).Length != 0)
			{
				if (arguments.Count != parametersCached.Length)
				{
					throw Error.IncorrectNumberOfConstructorArguments();
				}
				if (arguments.Count != members.Count)
				{
					throw Error.IncorrectNumberOfArgumentsForMembers();
				}
				Expression[] array = null;
				MemberInfo[] array2 = null;
				int i = 0;
				int count = arguments.Count;
				while (i < count)
				{
					Expression expression = arguments[i];
					ExpressionUtils.RequiresCanRead(expression, "arguments", i);
					MemberInfo memberInfo = members[i];
					ContractUtils.RequiresNotNull(memberInfo, "members", i);
					if (!TypeUtils.AreEquivalent(memberInfo.DeclaringType, constructor.DeclaringType))
					{
						throw Error.ArgumentMemberNotDeclOnType(memberInfo.Name, constructor.DeclaringType.Name, "members", i);
					}
					Type type;
					Expression.ValidateAnonymousTypeMember(ref memberInfo, out type, "members", i);
					if (!TypeUtils.AreReferenceAssignable(type, expression.Type) && !Expression.TryQuote(type, ref expression))
					{
						throw Error.ArgumentTypeDoesNotMatchMember(expression.Type, type, "arguments", i);
					}
					Type type2 = parametersCached[i].ParameterType;
					if (type2.IsByRef)
					{
						type2 = type2.GetElementType();
					}
					if (!TypeUtils.AreReferenceAssignable(type2, expression.Type) && !Expression.TryQuote(type2, ref expression))
					{
						throw Error.ExpressionTypeDoesNotMatchConstructorParameter(expression.Type, type2, "arguments", i);
					}
					if (array == null && expression != arguments[i])
					{
						array = new Expression[arguments.Count];
						for (int j = 0; j < i; j++)
						{
							array[j] = arguments[j];
						}
					}
					if (array != null)
					{
						array[i] = expression;
					}
					if (array2 == null && memberInfo != members[i])
					{
						array2 = new MemberInfo[members.Count];
						for (int k = 0; k < i; k++)
						{
							array2[k] = members[k];
						}
					}
					if (array2 != null)
					{
						array2[i] = memberInfo;
					}
					i++;
				}
				if (array != null)
				{
					arguments = new TrueReadOnlyCollection<Expression>(array);
				}
				if (array2 != null)
				{
					members = new TrueReadOnlyCollection<MemberInfo>(array2);
					return;
				}
			}
			else
			{
				if (arguments != null && arguments.Count > 0)
				{
					throw Error.IncorrectNumberOfConstructorArguments();
				}
				if (members != null && members.Count > 0)
				{
					throw Error.IncorrectNumberOfMembersForGivenConstructor();
				}
			}
		}

		private static void ValidateAnonymousTypeMember(ref MemberInfo member, out Type memberType, string paramName, int index)
		{
			FieldInfo fieldInfo = member as FieldInfo;
			if (fieldInfo != null)
			{
				if (fieldInfo.IsStatic)
				{
					throw Error.ArgumentMustBeInstanceMember(paramName, index);
				}
				memberType = fieldInfo.FieldType;
				return;
			}
			else
			{
				PropertyInfo propertyInfo = member as PropertyInfo;
				if (propertyInfo != null)
				{
					if (!propertyInfo.CanRead)
					{
						throw Error.PropertyDoesNotHaveGetter(propertyInfo, paramName, index);
					}
					if (propertyInfo.GetGetMethod().IsStatic)
					{
						throw Error.ArgumentMustBeInstanceMember(paramName, index);
					}
					memberType = propertyInfo.PropertyType;
					return;
				}
				else
				{
					MethodInfo methodInfo = member as MethodInfo;
					if (!(methodInfo != null))
					{
						throw Error.ArgumentMustBeFieldInfoOrPropertyInfoOrMethod(paramName, index);
					}
					if (methodInfo.IsStatic)
					{
						throw Error.ArgumentMustBeInstanceMember(paramName, index);
					}
					PropertyInfo property = Expression.GetProperty(methodInfo, paramName, index);
					member = property;
					memberType = property.PropertyType;
					return;
				}
			}
		}

		private static void ValidateConstructor(ConstructorInfo constructor, string paramName)
		{
			if (constructor.IsStatic)
			{
				throw Error.NonStaticConstructorRequired(paramName);
			}
		}

		/// <summary>Creates a <see cref="T:System.Linq.Expressions.ParameterExpression" /> node that can be used to identify a parameter or a variable in an expression tree.</summary>
		/// <param name="type">The type of the parameter or variable.</param>
		/// <returns>A <see cref="T:System.Linq.Expressions.ParameterExpression" /> node with the specified name and type.</returns>
		public static ParameterExpression Parameter(Type type)
		{
			return Expression.Parameter(type, null);
		}

		/// <summary>Creates a <see cref="T:System.Linq.Expressions.ParameterExpression" /> node that can be used to identify a parameter or a variable in an expression tree.</summary>
		/// <param name="type">The type of the parameter or variable.</param>
		/// <returns>A <see cref="T:System.Linq.Expressions.ParameterExpression" /> node with the specified name and type</returns>
		public static ParameterExpression Variable(Type type)
		{
			return Expression.Variable(type, null);
		}

		/// <summary>Creates a <see cref="T:System.Linq.Expressions.ParameterExpression" /> node that can be used to identify a parameter or a variable in an expression tree.</summary>
		/// <param name="type">The type of the parameter or variable.</param>
		/// <param name="name">The name of the parameter or variable, used for debugging or printing purpose only.</param>
		/// <returns>A <see cref="T:System.Linq.Expressions.ParameterExpression" /> that has the <see cref="P:System.Linq.Expressions.Expression.NodeType" /> property equal to <see cref="F:System.Linq.Expressions.ExpressionType.Parameter" /> and the <see cref="P:System.Linq.Expressions.Expression.Type" /> and <see cref="P:System.Linq.Expressions.ParameterExpression.Name" /> properties set to the specified values.</returns>
		/// <exception cref="T:System.ArgumentNullException">
		///         <paramref name="type" /> is <see langword="null" />.</exception>
		public static ParameterExpression Parameter(Type type, string name)
		{
			Expression.Validate(type, true);
			bool isByRef = type.IsByRef;
			if (isByRef)
			{
				type = type.GetElementType();
			}
			return ParameterExpression.Make(type, name, isByRef);
		}

		/// <summary>Creates a <see cref="T:System.Linq.Expressions.ParameterExpression" /> node that can be used to identify a parameter or a variable in an expression tree.</summary>
		/// <param name="type">The type of the parameter or variable.</param>
		/// <param name="name">The name of the parameter or variable. This name is used for debugging or printing purpose only.</param>
		/// <returns>A <see cref="T:System.Linq.Expressions.ParameterExpression" /> node with the specified name and type.</returns>
		public static ParameterExpression Variable(Type type, string name)
		{
			Expression.Validate(type, false);
			return ParameterExpression.Make(type, name, false);
		}

		private static void Validate(Type type, bool allowByRef)
		{
			ContractUtils.RequiresNotNull(type, "type");
			TypeUtils.ValidateType(type, "type", allowByRef, false);
			if (type == typeof(void))
			{
				throw Error.ArgumentCannotBeOfTypeVoid("type");
			}
		}

		/// <summary>Creates an instance of <see cref="T:System.Linq.Expressions.RuntimeVariablesExpression" />.</summary>
		/// <param name="variables">An array of <see cref="T:System.Linq.Expressions.ParameterExpression" /> objects to use to populate the <see cref="P:System.Linq.Expressions.RuntimeVariablesExpression.Variables" /> collection.</param>
		/// <returns>An instance of <see cref="T:System.Linq.Expressions.RuntimeVariablesExpression" /> that has the <see cref="P:System.Linq.Expressions.Expression.NodeType" /> property equal to <see cref="F:System.Linq.Expressions.ExpressionType.RuntimeVariables" /> and the <see cref="P:System.Linq.Expressions.RuntimeVariablesExpression.Variables" /> property set to the specified value.</returns>
		public static RuntimeVariablesExpression RuntimeVariables(params ParameterExpression[] variables)
		{
			return Expression.RuntimeVariables(variables);
		}

		/// <summary>Creates an instance of <see cref="T:System.Linq.Expressions.RuntimeVariablesExpression" />.</summary>
		/// <param name="variables">A collection of <see cref="T:System.Linq.Expressions.ParameterExpression" /> objects to use to populate the <see cref="P:System.Linq.Expressions.RuntimeVariablesExpression.Variables" /> collection.</param>
		/// <returns>An instance of <see cref="T:System.Linq.Expressions.RuntimeVariablesExpression" /> that has the <see cref="P:System.Linq.Expressions.Expression.NodeType" /> property equal to <see cref="F:System.Linq.Expressions.ExpressionType.RuntimeVariables" /> and the <see cref="P:System.Linq.Expressions.RuntimeVariablesExpression.Variables" /> property set to the specified value.</returns>
		public static RuntimeVariablesExpression RuntimeVariables(IEnumerable<ParameterExpression> variables)
		{
			ContractUtils.RequiresNotNull(variables, "variables");
			ReadOnlyCollection<ParameterExpression> readOnlyCollection = variables.ToReadOnly<ParameterExpression>();
			for (int i = 0; i < readOnlyCollection.Count; i++)
			{
				ContractUtils.RequiresNotNull(readOnlyCollection[i], "variables", i);
			}
			return new RuntimeVariablesExpression(readOnlyCollection);
		}

		/// <summary>Creates a <see cref="T:System.Linq.Expressions.SwitchCase" /> for use in a <see cref="T:System.Linq.Expressions.SwitchExpression" />.</summary>
		/// <param name="body">The body of the case.</param>
		/// <param name="testValues">The test values of the case.</param>
		/// <returns>The created <see cref="T:System.Linq.Expressions.SwitchCase" />.</returns>
		public static SwitchCase SwitchCase(Expression body, params Expression[] testValues)
		{
			return Expression.SwitchCase(body, testValues);
		}

		/// <summary>Creates a <see cref="T:System.Linq.Expressions.SwitchCase" /> object to be used in a <see cref="T:System.Linq.Expressions.SwitchExpression" /> object.</summary>
		/// <param name="body">The body of the case.</param>
		/// <param name="testValues">The test values of the case.</param>
		/// <returns>The created <see cref="T:System.Linq.Expressions.SwitchCase" />.</returns>
		public static SwitchCase SwitchCase(Expression body, IEnumerable<Expression> testValues)
		{
			ExpressionUtils.RequiresCanRead(body, "body");
			ReadOnlyCollection<Expression> readOnlyCollection = testValues.ToReadOnly<Expression>();
			ContractUtils.RequiresNotEmpty<Expression>(readOnlyCollection, "testValues");
			Expression.RequiresCanRead(readOnlyCollection, "testValues");
			return new SwitchCase(body, readOnlyCollection);
		}

		/// <summary>Creates a <see cref="T:System.Linq.Expressions.SwitchExpression" /> that represents a <see langword="switch" /> statement without a default case.</summary>
		/// <param name="switchValue">The value to be tested against each case.</param>
		/// <param name="cases">The set of cases for this switch expression.</param>
		/// <returns>The created <see cref="T:System.Linq.Expressions.SwitchExpression" />.</returns>
		public static SwitchExpression Switch(Expression switchValue, params SwitchCase[] cases)
		{
			return Expression.Switch(switchValue, null, null, cases);
		}

		/// <summary>Creates a <see cref="T:System.Linq.Expressions.SwitchExpression" /> that represents a <see langword="switch" /> statement that has a default case.</summary>
		/// <param name="switchValue">The value to be tested against each case.</param>
		/// <param name="defaultBody">The result of the switch if <paramref name="switchValue" /> does not match any of the cases.</param>
		/// <param name="cases">The set of cases for this switch expression.</param>
		/// <returns>The created <see cref="T:System.Linq.Expressions.SwitchExpression" />.</returns>
		public static SwitchExpression Switch(Expression switchValue, Expression defaultBody, params SwitchCase[] cases)
		{
			return Expression.Switch(switchValue, defaultBody, null, cases);
		}

		/// <summary>Creates a <see cref="T:System.Linq.Expressions.SwitchExpression" /> that represents a <see langword="switch" /> statement that has a default case.</summary>
		/// <param name="switchValue">The value to be tested against each case.</param>
		/// <param name="defaultBody">The result of the switch if <paramref name="switchValue" /> does not match any of the cases.</param>
		/// <param name="comparison">The equality comparison method to use.</param>
		/// <param name="cases">The set of cases for this switch expression.</param>
		/// <returns>The created <see cref="T:System.Linq.Expressions.SwitchExpression" />.</returns>
		public static SwitchExpression Switch(Expression switchValue, Expression defaultBody, MethodInfo comparison, params SwitchCase[] cases)
		{
			return Expression.Switch(switchValue, defaultBody, comparison, cases);
		}

		/// <summary>Creates a <see cref="T:System.Linq.Expressions.SwitchExpression" /> that represents a <see langword="switch" /> statement that has a default case..</summary>
		/// <param name="type">The result type of the switch.</param>
		/// <param name="switchValue">The value to be tested against each case.</param>
		/// <param name="defaultBody">The result of the switch if <paramref name="switchValue" /> does not match any of the cases.</param>
		/// <param name="comparison">The equality comparison method to use.</param>
		/// <param name="cases">The set of cases for this switch expression.</param>
		/// <returns>The created <see cref="T:System.Linq.Expressions.SwitchExpression" />.</returns>
		public static SwitchExpression Switch(Type type, Expression switchValue, Expression defaultBody, MethodInfo comparison, params SwitchCase[] cases)
		{
			return Expression.Switch(type, switchValue, defaultBody, comparison, cases);
		}

		/// <summary>Creates a <see cref="T:System.Linq.Expressions.SwitchExpression" /> that represents a <see langword="switch" /> statement that has a default case.</summary>
		/// <param name="switchValue">The value to be tested against each case.</param>
		/// <param name="defaultBody">The result of the switch if <paramref name="switchValue" /> does not match any of the cases.</param>
		/// <param name="comparison">The equality comparison method to use.</param>
		/// <param name="cases">The set of cases for this switch expression.</param>
		/// <returns>The created <see cref="T:System.Linq.Expressions.SwitchExpression" />.</returns>
		public static SwitchExpression Switch(Expression switchValue, Expression defaultBody, MethodInfo comparison, IEnumerable<SwitchCase> cases)
		{
			return Expression.Switch(null, switchValue, defaultBody, comparison, cases);
		}

		/// <summary>Creates a <see cref="T:System.Linq.Expressions.SwitchExpression" /> that represents a <see langword="switch" /> statement that has a default case.</summary>
		/// <param name="type">The result type of the switch.</param>
		/// <param name="switchValue">The value to be tested against each case.</param>
		/// <param name="defaultBody">The result of the switch if <paramref name="switchValue" /> does not match any of the cases.</param>
		/// <param name="comparison">The equality comparison method to use.</param>
		/// <param name="cases">The set of cases for this switch expression.</param>
		/// <returns>The created <see cref="T:System.Linq.Expressions.SwitchExpression" />.</returns>
		public static SwitchExpression Switch(Type type, Expression switchValue, Expression defaultBody, MethodInfo comparison, IEnumerable<SwitchCase> cases)
		{
			ExpressionUtils.RequiresCanRead(switchValue, "switchValue");
			if (switchValue.Type == typeof(void))
			{
				throw Error.ArgumentCannotBeOfTypeVoid("switchValue");
			}
			ReadOnlyCollection<SwitchCase> readOnlyCollection = cases.ToReadOnly<SwitchCase>();
			ContractUtils.RequiresNotNullItems<SwitchCase>(readOnlyCollection, "cases");
			Type type2;
			if (type != null)
			{
				type2 = type;
			}
			else if (readOnlyCollection.Count != 0)
			{
				type2 = readOnlyCollection[0].Body.Type;
			}
			else if (defaultBody != null)
			{
				type2 = defaultBody.Type;
			}
			else
			{
				type2 = typeof(void);
			}
			bool customType = type != null;
			if (comparison != null)
			{
				Expression.ValidateMethodInfo(comparison, "comparison");
				ParameterInfo[] parametersCached = comparison.GetParametersCached();
				if (parametersCached.Length != 2)
				{
					throw Error.IncorrectNumberOfMethodCallArguments(comparison, "comparison");
				}
				ParameterInfo parameterInfo = parametersCached[0];
				bool flag = false;
				if (!Expression.ParameterIsAssignable(parameterInfo, switchValue.Type))
				{
					flag = Expression.ParameterIsAssignable(parameterInfo, switchValue.Type.GetNonNullableType());
					if (!flag)
					{
						throw Error.SwitchValueTypeDoesNotMatchComparisonMethodParameter(switchValue.Type, parameterInfo.ParameterType);
					}
				}
				ParameterInfo parameterInfo2 = parametersCached[1];
				foreach (SwitchCase switchCase in readOnlyCollection)
				{
					ContractUtils.RequiresNotNull(switchCase, "cases");
					Expression.ValidateSwitchCaseType(switchCase.Body, customType, type2, "cases");
					int i = 0;
					int count = switchCase.TestValues.Count;
					while (i < count)
					{
						Type type3 = switchCase.TestValues[i].Type;
						if (flag)
						{
							if (!type3.IsNullableType())
							{
								throw Error.TestValueTypeDoesNotMatchComparisonMethodParameter(type3, parameterInfo2.ParameterType);
							}
							type3 = type3.GetNonNullableType();
						}
						if (!Expression.ParameterIsAssignable(parameterInfo2, type3))
						{
							throw Error.TestValueTypeDoesNotMatchComparisonMethodParameter(type3, parameterInfo2.ParameterType);
						}
						i++;
					}
				}
				if (comparison.ReturnType != typeof(bool))
				{
					throw Error.EqualityMustReturnBoolean(comparison, "comparison");
				}
			}
			else if (readOnlyCollection.Count != 0)
			{
				Expression expression = readOnlyCollection[0].TestValues[0];
				foreach (SwitchCase switchCase2 in readOnlyCollection)
				{
					ContractUtils.RequiresNotNull(switchCase2, "cases");
					Expression.ValidateSwitchCaseType(switchCase2.Body, customType, type2, "cases");
					int j = 0;
					int count2 = switchCase2.TestValues.Count;
					while (j < count2)
					{
						if (!TypeUtils.AreEquivalent(expression.Type, switchCase2.TestValues[j].Type))
						{
							throw Error.AllTestValuesMustHaveSameType("cases");
						}
						j++;
					}
				}
				comparison = Expression.Equal(switchValue, expression, false, comparison).Method;
			}
			if (defaultBody == null)
			{
				if (type2 != typeof(void))
				{
					throw Error.DefaultBodyMustBeSupplied("defaultBody");
				}
			}
			else
			{
				Expression.ValidateSwitchCaseType(defaultBody, customType, type2, "defaultBody");
			}
			return new SwitchExpression(type2, switchValue, defaultBody, comparison, readOnlyCollection);
		}

		private static void ValidateSwitchCaseType(Expression @case, bool customType, Type resultType, string parameterName)
		{
			if (customType)
			{
				if (resultType != typeof(void) && !TypeUtils.AreReferenceAssignable(resultType, @case.Type))
				{
					throw Error.ArgumentTypesMustMatch(parameterName);
				}
			}
			else if (!TypeUtils.AreEquivalent(resultType, @case.Type))
			{
				throw Error.AllCaseBodiesMustHaveSameType(parameterName);
			}
		}

		/// <summary>Creates an instance of <see cref="T:System.Linq.Expressions.SymbolDocumentInfo" />.</summary>
		/// <param name="fileName">A <see cref="T:System.String" /> to set the <see cref="P:System.Linq.Expressions.SymbolDocumentInfo.FileName" /> equal to.</param>
		/// <returns>A <see cref="T:System.Linq.Expressions.SymbolDocumentInfo" /> that has the <see cref="P:System.Linq.Expressions.SymbolDocumentInfo.FileName" /> property set to the specified value.</returns>
		public static SymbolDocumentInfo SymbolDocument(string fileName)
		{
			return new SymbolDocumentInfo(fileName);
		}

		/// <summary>Creates an instance of <see cref="T:System.Linq.Expressions.SymbolDocumentInfo" />.</summary>
		/// <param name="fileName">A <see cref="T:System.String" /> to set the <see cref="P:System.Linq.Expressions.SymbolDocumentInfo.FileName" /> equal to.</param>
		/// <param name="language">A <see cref="T:System.Guid" /> to set the <see cref="P:System.Linq.Expressions.SymbolDocumentInfo.Language" /> equal to.</param>
		/// <returns>A <see cref="T:System.Linq.Expressions.SymbolDocumentInfo" /> that has the <see cref="P:System.Linq.Expressions.SymbolDocumentInfo.FileName" /> and <see cref="P:System.Linq.Expressions.SymbolDocumentInfo.Language" /> properties set to the specified value.</returns>
		public static SymbolDocumentInfo SymbolDocument(string fileName, Guid language)
		{
			return new SymbolDocumentWithGuids(fileName, ref language);
		}

		/// <summary>Creates an instance of <see cref="T:System.Linq.Expressions.SymbolDocumentInfo" />.</summary>
		/// <param name="fileName">A <see cref="T:System.String" /> to set the <see cref="P:System.Linq.Expressions.SymbolDocumentInfo.FileName" /> equal to.</param>
		/// <param name="language">A <see cref="T:System.Guid" /> to set the <see cref="P:System.Linq.Expressions.SymbolDocumentInfo.Language" /> equal to.</param>
		/// <param name="languageVendor">A <see cref="T:System.Guid" /> to set the <see cref="P:System.Linq.Expressions.SymbolDocumentInfo.LanguageVendor" /> equal to.</param>
		/// <returns>A <see cref="T:System.Linq.Expressions.SymbolDocumentInfo" /> that has the <see cref="P:System.Linq.Expressions.SymbolDocumentInfo.FileName" /> and <see cref="P:System.Linq.Expressions.SymbolDocumentInfo.Language" /> and <see cref="P:System.Linq.Expressions.SymbolDocumentInfo.LanguageVendor" /> properties set to the specified value.</returns>
		public static SymbolDocumentInfo SymbolDocument(string fileName, Guid language, Guid languageVendor)
		{
			return new SymbolDocumentWithGuids(fileName, ref language, ref languageVendor);
		}

		/// <summary>Creates an instance of <see cref="T:System.Linq.Expressions.SymbolDocumentInfo" />.</summary>
		/// <param name="fileName">A <see cref="T:System.String" /> to set the <see cref="P:System.Linq.Expressions.SymbolDocumentInfo.FileName" /> equal to.</param>
		/// <param name="language">A <see cref="T:System.Guid" /> to set the <see cref="P:System.Linq.Expressions.SymbolDocumentInfo.Language" /> equal to.</param>
		/// <param name="languageVendor">A <see cref="T:System.Guid" /> to set the <see cref="P:System.Linq.Expressions.SymbolDocumentInfo.LanguageVendor" /> equal to.</param>
		/// <param name="documentType">A <see cref="T:System.Guid" /> to set the <see cref="P:System.Linq.Expressions.SymbolDocumentInfo.DocumentType" /> equal to.</param>
		/// <returns>A <see cref="T:System.Linq.Expressions.SymbolDocumentInfo" /> that has the <see cref="P:System.Linq.Expressions.SymbolDocumentInfo.FileName" /> and <see cref="P:System.Linq.Expressions.SymbolDocumentInfo.Language" /> and <see cref="P:System.Linq.Expressions.SymbolDocumentInfo.LanguageVendor" /> and <see cref="P:System.Linq.Expressions.SymbolDocumentInfo.DocumentType" /> properties set to the specified value.</returns>
		public static SymbolDocumentInfo SymbolDocument(string fileName, Guid language, Guid languageVendor, Guid documentType)
		{
			return new SymbolDocumentWithGuids(fileName, ref language, ref languageVendor, ref documentType);
		}

		/// <summary>Creates a <see cref="T:System.Linq.Expressions.TryExpression" /> representing a try block with a fault block and no catch statements.</summary>
		/// <param name="body">The body of the try block.</param>
		/// <param name="fault">The body of the fault block.</param>
		/// <returns>The created <see cref="T:System.Linq.Expressions.TryExpression" />.</returns>
		public static TryExpression TryFault(Expression body, Expression fault)
		{
			return Expression.MakeTry(null, body, null, fault, null);
		}

		/// <summary>Creates a <see cref="T:System.Linq.Expressions.TryExpression" /> representing a try block with a finally block and no catch statements.</summary>
		/// <param name="body">The body of the try block.</param>
		/// <param name="finally">The body of the finally block.</param>
		/// <returns>The created <see cref="T:System.Linq.Expressions.TryExpression" />.</returns>
		public static TryExpression TryFinally(Expression body, Expression @finally)
		{
			return Expression.MakeTry(null, body, @finally, null, null);
		}

		/// <summary>Creates a <see cref="T:System.Linq.Expressions.TryExpression" /> representing a try block with any number of catch statements and neither a fault nor finally block.</summary>
		/// <param name="body">The body of the try block.</param>
		/// <param name="handlers">The array of zero or more <see cref="T:System.Linq.Expressions.CatchBlock" /> expressions representing the catch statements to be associated with the try block.</param>
		/// <returns>The created <see cref="T:System.Linq.Expressions.TryExpression" />.</returns>
		public static TryExpression TryCatch(Expression body, params CatchBlock[] handlers)
		{
			return Expression.MakeTry(null, body, null, null, handlers);
		}

		/// <summary>Creates a <see cref="T:System.Linq.Expressions.TryExpression" /> representing a try block with any number of catch statements and a finally block.</summary>
		/// <param name="body">The body of the try block.</param>
		/// <param name="finally">The body of the finally block.</param>
		/// <param name="handlers">The array of zero or more <see cref="T:System.Linq.Expressions.CatchBlock" /> expressions representing the catch statements to be associated with the try block.</param>
		/// <returns>The created <see cref="T:System.Linq.Expressions.TryExpression" />.</returns>
		public static TryExpression TryCatchFinally(Expression body, Expression @finally, params CatchBlock[] handlers)
		{
			return Expression.MakeTry(null, body, @finally, null, handlers);
		}

		/// <summary>Creates a <see cref="T:System.Linq.Expressions.TryExpression" /> representing a try block with the specified elements.</summary>
		/// <param name="type">The result type of the try expression. If null, bodh and all handlers must have identical type.</param>
		/// <param name="body">The body of the try block.</param>
		/// <param name="finally">The body of the finally block. Pass null if the try block has no finally block associated with it.</param>
		/// <param name="fault">The body of the fault block. Pass null if the try block has no fault block associated with it.</param>
		/// <param name="handlers">A collection of <see cref="T:System.Linq.Expressions.CatchBlock" />s representing the catch statements to be associated with the try block.</param>
		/// <returns>The created <see cref="T:System.Linq.Expressions.TryExpression" />.</returns>
		public static TryExpression MakeTry(Type type, Expression body, Expression @finally, Expression fault, IEnumerable<CatchBlock> handlers)
		{
			ExpressionUtils.RequiresCanRead(body, "body");
			ReadOnlyCollection<CatchBlock> readOnlyCollection = handlers.ToReadOnly<CatchBlock>();
			ContractUtils.RequiresNotNullItems<CatchBlock>(readOnlyCollection, "handlers");
			Expression.ValidateTryAndCatchHaveSameType(type, body, readOnlyCollection);
			if (fault != null)
			{
				if (@finally != null || readOnlyCollection.Count > 0)
				{
					throw Error.FaultCannotHaveCatchOrFinally("fault");
				}
				ExpressionUtils.RequiresCanRead(fault, "fault");
			}
			else if (@finally != null)
			{
				ExpressionUtils.RequiresCanRead(@finally, "finally");
			}
			else if (readOnlyCollection.Count == 0)
			{
				throw Error.TryMustHaveCatchFinallyOrFault();
			}
			return new TryExpression(type ?? body.Type, body, @finally, fault, readOnlyCollection);
		}

		private static void ValidateTryAndCatchHaveSameType(Type type, Expression tryBody, ReadOnlyCollection<CatchBlock> handlers)
		{
			if (type != null)
			{
				if (!(type != typeof(void)))
				{
					return;
				}
				if (!TypeUtils.AreReferenceAssignable(type, tryBody.Type))
				{
					throw Error.ArgumentTypesMustMatch();
				}
				using (IEnumerator<CatchBlock> enumerator = handlers.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						CatchBlock catchBlock = enumerator.Current;
						if (!TypeUtils.AreReferenceAssignable(type, catchBlock.Body.Type))
						{
							throw Error.ArgumentTypesMustMatch();
						}
					}
					return;
				}
			}
			if (tryBody.Type == typeof(void))
			{
				using (IEnumerator<CatchBlock> enumerator = handlers.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						if (enumerator.Current.Body.Type != typeof(void))
						{
							throw Error.BodyOfCatchMustHaveSameTypeAsBodyOfTry();
						}
					}
					return;
				}
			}
			type = tryBody.Type;
			using (IEnumerator<CatchBlock> enumerator = handlers.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					if (!TypeUtils.AreEquivalent(enumerator.Current.Body.Type, type))
					{
						throw Error.BodyOfCatchMustHaveSameTypeAsBodyOfTry();
					}
				}
			}
		}

		/// <summary>Creates a <see cref="T:System.Linq.Expressions.TypeBinaryExpression" />.</summary>
		/// <param name="expression">An <see cref="T:System.Linq.Expressions.Expression" /> to set the <see cref="P:System.Linq.Expressions.TypeBinaryExpression.Expression" /> property equal to.</param>
		/// <param name="type">A <see cref="P:System.Linq.Expressions.Expression.Type" /> to set the <see cref="P:System.Linq.Expressions.TypeBinaryExpression.TypeOperand" /> property equal to.</param>
		/// <returns>A <see cref="T:System.Linq.Expressions.TypeBinaryExpression" /> for which the <see cref="P:System.Linq.Expressions.Expression.NodeType" /> property is equal to <see cref="F:System.Linq.Expressions.ExpressionType.TypeIs" /> and for which the <see cref="P:System.Linq.Expressions.TypeBinaryExpression.Expression" /> and <see cref="P:System.Linq.Expressions.TypeBinaryExpression.TypeOperand" /> properties are set to the specified values.</returns>
		/// <exception cref="T:System.ArgumentNullException">
		///         <paramref name="expression" /> or <paramref name="type" /> is <see langword="null" />.</exception>
		public static TypeBinaryExpression TypeIs(Expression expression, Type type)
		{
			ExpressionUtils.RequiresCanRead(expression, "expression");
			ContractUtils.RequiresNotNull(type, "type");
			if (type.IsByRef)
			{
				throw Error.TypeMustNotBeByRef("type");
			}
			return new TypeBinaryExpression(expression, type, ExpressionType.TypeIs);
		}

		/// <summary>Creates a <see cref="T:System.Linq.Expressions.TypeBinaryExpression" /> that compares run-time type identity.</summary>
		/// <param name="expression">An <see cref="T:System.Linq.Expressions.Expression" /> to set the <see cref="T:System.Linq.Expressions.Expression" /> property equal to.</param>
		/// <param name="type">A <see cref="P:System.Linq.Expressions.Expression.Type" /> to set the <see cref="P:System.Linq.Expressions.TypeBinaryExpression.TypeOperand" /> property equal to.</param>
		/// <returns>A <see cref="T:System.Linq.Expressions.TypeBinaryExpression" /> for which the <see cref="P:System.Linq.Expressions.Expression.NodeType" /> property is equal to <see cref="M:System.Linq.Expressions.Expression.TypeEqual(System.Linq.Expressions.Expression,System.Type)" /> and for which the <see cref="T:System.Linq.Expressions.Expression" /> and <see cref="P:System.Linq.Expressions.TypeBinaryExpression.TypeOperand" /> properties are set to the specified values.</returns>
		public static TypeBinaryExpression TypeEqual(Expression expression, Type type)
		{
			ExpressionUtils.RequiresCanRead(expression, "expression");
			ContractUtils.RequiresNotNull(type, "type");
			if (type.IsByRef)
			{
				throw Error.TypeMustNotBeByRef("type");
			}
			return new TypeBinaryExpression(expression, type, ExpressionType.TypeEqual);
		}

		/// <summary>Creates a <see cref="T:System.Linq.Expressions.UnaryExpression" />, given an operand, by calling the appropriate factory method.</summary>
		/// <param name="unaryType">The <see cref="T:System.Linq.Expressions.ExpressionType" /> that specifies the type of unary operation.</param>
		/// <param name="operand">An <see cref="T:System.Linq.Expressions.Expression" /> that represents the operand.</param>
		/// <param name="type">The <see cref="T:System.Type" /> that specifies the type to be converted to (pass <see langword="null" /> if not applicable).</param>
		/// <returns>The <see cref="T:System.Linq.Expressions.UnaryExpression" /> that results from calling the appropriate factory method.</returns>
		/// <exception cref="T:System.ArgumentNullException">
		///         <paramref name="operand" /> is <see langword="null" />.</exception>
		/// <exception cref="T:System.ArgumentException">
		///         <paramref name="unaryType" /> does not correspond to a unary expression node.</exception>
		public static UnaryExpression MakeUnary(ExpressionType unaryType, Expression operand, Type type)
		{
			return Expression.MakeUnary(unaryType, operand, type, null);
		}

		/// <summary>Creates a <see cref="T:System.Linq.Expressions.UnaryExpression" />, given an operand and implementing method, by calling the appropriate factory method.</summary>
		/// <param name="unaryType">The <see cref="T:System.Linq.Expressions.ExpressionType" /> that specifies the type of unary operation.</param>
		/// <param name="operand">An <see cref="T:System.Linq.Expressions.Expression" /> that represents the operand.</param>
		/// <param name="type">The <see cref="T:System.Type" /> that specifies the type to be converted to (pass <see langword="null" /> if not applicable).</param>
		/// <param name="method">The <see cref="T:System.Reflection.MethodInfo" /> that represents the implementing method.</param>
		/// <returns>The <see cref="T:System.Linq.Expressions.UnaryExpression" /> that results from calling the appropriate factory method.</returns>
		/// <exception cref="T:System.ArgumentNullException">
		///         <paramref name="operand" /> is <see langword="null" />.</exception>
		/// <exception cref="T:System.ArgumentException">
		///         <paramref name="unaryType" /> does not correspond to a unary expression node.</exception>
		public static UnaryExpression MakeUnary(ExpressionType unaryType, Expression operand, Type type, MethodInfo method)
		{
			if (unaryType <= ExpressionType.Quote)
			{
				if (unaryType <= ExpressionType.Convert)
				{
					if (unaryType == ExpressionType.ArrayLength)
					{
						return Expression.ArrayLength(operand);
					}
					if (unaryType == ExpressionType.Convert)
					{
						return Expression.Convert(operand, type, method);
					}
				}
				else
				{
					if (unaryType == ExpressionType.ConvertChecked)
					{
						return Expression.ConvertChecked(operand, type, method);
					}
					switch (unaryType)
					{
					case ExpressionType.Negate:
						return Expression.Negate(operand, method);
					case ExpressionType.UnaryPlus:
						return Expression.UnaryPlus(operand, method);
					case ExpressionType.NegateChecked:
						return Expression.NegateChecked(operand, method);
					case ExpressionType.New:
					case ExpressionType.NewArrayInit:
					case ExpressionType.NewArrayBounds:
						break;
					case ExpressionType.Not:
						return Expression.Not(operand, method);
					default:
						if (unaryType == ExpressionType.Quote)
						{
							return Expression.Quote(operand);
						}
						break;
					}
				}
			}
			else if (unaryType <= ExpressionType.Increment)
			{
				if (unaryType == ExpressionType.TypeAs)
				{
					return Expression.TypeAs(operand, type);
				}
				if (unaryType == ExpressionType.Decrement)
				{
					return Expression.Decrement(operand, method);
				}
				if (unaryType == ExpressionType.Increment)
				{
					return Expression.Increment(operand, method);
				}
			}
			else
			{
				if (unaryType == ExpressionType.Throw)
				{
					return Expression.Throw(operand, type);
				}
				if (unaryType == ExpressionType.Unbox)
				{
					return Expression.Unbox(operand, type);
				}
				switch (unaryType)
				{
				case ExpressionType.PreIncrementAssign:
					return Expression.PreIncrementAssign(operand, method);
				case ExpressionType.PreDecrementAssign:
					return Expression.PreDecrementAssign(operand, method);
				case ExpressionType.PostIncrementAssign:
					return Expression.PostIncrementAssign(operand, method);
				case ExpressionType.PostDecrementAssign:
					return Expression.PostDecrementAssign(operand, method);
				case ExpressionType.OnesComplement:
					return Expression.OnesComplement(operand, method);
				case ExpressionType.IsTrue:
					return Expression.IsTrue(operand, method);
				case ExpressionType.IsFalse:
					return Expression.IsFalse(operand, method);
				}
			}
			throw Error.UnhandledUnary(unaryType, "unaryType");
		}

		private static UnaryExpression GetUserDefinedUnaryOperatorOrThrow(ExpressionType unaryType, string name, Expression operand)
		{
			UnaryExpression userDefinedUnaryOperator = Expression.GetUserDefinedUnaryOperator(unaryType, name, operand);
			if (userDefinedUnaryOperator != null)
			{
				Expression.ValidateParamswithOperandsOrThrow(userDefinedUnaryOperator.Method.GetParametersCached()[0].ParameterType, operand.Type, unaryType, name);
				return userDefinedUnaryOperator;
			}
			throw Error.UnaryOperatorNotDefined(unaryType, operand.Type);
		}

		private static UnaryExpression GetUserDefinedUnaryOperator(ExpressionType unaryType, string name, Expression operand)
		{
			Type type = operand.Type;
			Type[] array = new Type[]
			{
				type
			};
			Type nonNullableType = type.GetNonNullableType();
			MethodInfo anyStaticMethodValidated = nonNullableType.GetAnyStaticMethodValidated(name, array);
			if (anyStaticMethodValidated != null)
			{
				return new UnaryExpression(unaryType, operand, anyStaticMethodValidated.ReturnType, anyStaticMethodValidated);
			}
			if (type.IsNullableType())
			{
				array[0] = nonNullableType;
				anyStaticMethodValidated = nonNullableType.GetAnyStaticMethodValidated(name, array);
				if (anyStaticMethodValidated != null && anyStaticMethodValidated.ReturnType.IsValueType && !anyStaticMethodValidated.ReturnType.IsNullableType())
				{
					return new UnaryExpression(unaryType, operand, anyStaticMethodValidated.ReturnType.GetNullableType(), anyStaticMethodValidated);
				}
			}
			return null;
		}

		private static UnaryExpression GetMethodBasedUnaryOperator(ExpressionType unaryType, Expression operand, MethodInfo method)
		{
			Expression.ValidateOperator(method);
			ParameterInfo[] parametersCached = method.GetParametersCached();
			if (parametersCached.Length != 1)
			{
				throw Error.IncorrectNumberOfMethodCallArguments(method, "method");
			}
			if (Expression.ParameterIsAssignable(parametersCached[0], operand.Type))
			{
				Expression.ValidateParamswithOperandsOrThrow(parametersCached[0].ParameterType, operand.Type, unaryType, method.Name);
				return new UnaryExpression(unaryType, operand, method.ReturnType, method);
			}
			if (operand.Type.IsNullableType() && Expression.ParameterIsAssignable(parametersCached[0], operand.Type.GetNonNullableType()) && method.ReturnType.IsValueType && !method.ReturnType.IsNullableType())
			{
				return new UnaryExpression(unaryType, operand, method.ReturnType.GetNullableType(), method);
			}
			throw Error.OperandTypesDoNotMatchParameters(unaryType, method.Name);
		}

		private static UnaryExpression GetUserDefinedCoercionOrThrow(ExpressionType coercionType, Expression expression, Type convertToType)
		{
			UnaryExpression userDefinedCoercion = Expression.GetUserDefinedCoercion(coercionType, expression, convertToType);
			if (userDefinedCoercion != null)
			{
				return userDefinedCoercion;
			}
			throw Error.CoercionOperatorNotDefined(expression.Type, convertToType);
		}

		private static UnaryExpression GetUserDefinedCoercion(ExpressionType coercionType, Expression expression, Type convertToType)
		{
			MethodInfo userDefinedCoercionMethod = TypeUtils.GetUserDefinedCoercionMethod(expression.Type, convertToType);
			if (userDefinedCoercionMethod != null)
			{
				return new UnaryExpression(coercionType, expression, convertToType, userDefinedCoercionMethod);
			}
			return null;
		}

		private static UnaryExpression GetMethodBasedCoercionOperator(ExpressionType unaryType, Expression operand, Type convertToType, MethodInfo method)
		{
			Expression.ValidateOperator(method);
			ParameterInfo[] parametersCached = method.GetParametersCached();
			if (parametersCached.Length != 1)
			{
				throw Error.IncorrectNumberOfMethodCallArguments(method, "method");
			}
			if (Expression.ParameterIsAssignable(parametersCached[0], operand.Type) && TypeUtils.AreEquivalent(method.ReturnType, convertToType))
			{
				return new UnaryExpression(unaryType, operand, method.ReturnType, method);
			}
			if ((operand.Type.IsNullableType() || convertToType.IsNullableType()) && Expression.ParameterIsAssignable(parametersCached[0], operand.Type.GetNonNullableType()) && (TypeUtils.AreEquivalent(method.ReturnType, convertToType.GetNonNullableType()) || TypeUtils.AreEquivalent(method.ReturnType, convertToType)))
			{
				return new UnaryExpression(unaryType, operand, convertToType, method);
			}
			throw Error.OperandTypesDoNotMatchParameters(unaryType, method.Name);
		}

		/// <summary>Creates a <see cref="T:System.Linq.Expressions.UnaryExpression" /> that represents an arithmetic negation operation.</summary>
		/// <param name="expression">An <see cref="T:System.Linq.Expressions.Expression" /> to set the <see cref="P:System.Linq.Expressions.UnaryExpression.Operand" /> property equal to.</param>
		/// <returns>A <see cref="T:System.Linq.Expressions.UnaryExpression" /> that has the <see cref="P:System.Linq.Expressions.Expression.NodeType" /> property equal to <see cref="F:System.Linq.Expressions.ExpressionType.Negate" /> and the <see cref="P:System.Linq.Expressions.UnaryExpression.Operand" /> property set to the specified value.</returns>
		/// <exception cref="T:System.ArgumentNullException">
		///         <paramref name="expression" /> is <see langword="null" />.</exception>
		/// <exception cref="T:System.InvalidOperationException">The unary minus operator is not defined for <paramref name="expression" />.Type.</exception>
		public static UnaryExpression Negate(Expression expression)
		{
			return Expression.Negate(expression, null);
		}

		/// <summary>Creates a <see cref="T:System.Linq.Expressions.UnaryExpression" /> that represents an arithmetic negation operation.</summary>
		/// <param name="expression">An <see cref="T:System.Linq.Expressions.Expression" /> to set the <see cref="P:System.Linq.Expressions.UnaryExpression.Operand" /> property equal to.</param>
		/// <param name="method">A <see cref="T:System.Reflection.MethodInfo" /> to set the <see cref="P:System.Linq.Expressions.UnaryExpression.Method" /> property equal to.</param>
		/// <returns>A <see cref="T:System.Linq.Expressions.UnaryExpression" /> that has the <see cref="P:System.Linq.Expressions.Expression.NodeType" /> property equal to <see cref="F:System.Linq.Expressions.ExpressionType.Negate" /> and the <see cref="P:System.Linq.Expressions.UnaryExpression.Operand" /> and <see cref="P:System.Linq.Expressions.UnaryExpression.Method" /> properties set to the specified values.</returns>
		/// <exception cref="T:System.ArgumentNullException">
		///         <paramref name="expression" /> is <see langword="null" />.</exception>
		/// <exception cref="T:System.ArgumentException">
		///         <paramref name="method" /> is not <see langword="null" /> and the method it represents returns <see langword="void" />, is not <see langword="static" /> (<see langword="Shared" /> in Visual Basic), or does not take exactly one argument.</exception>
		/// <exception cref="T:System.InvalidOperationException">
		///         <paramref name="method" /> is <see langword="null" /> and the unary minus operator is not defined for <paramref name="expression" />.Type.-or-
		///         <paramref name="expression" />.Type (or its corresponding non-nullable type if it is a nullable value type) is not assignable to the argument type of the method represented by <paramref name="method" />.</exception>
		public static UnaryExpression Negate(Expression expression, MethodInfo method)
		{
			ExpressionUtils.RequiresCanRead(expression, "expression");
			if (!(method == null))
			{
				return Expression.GetMethodBasedUnaryOperator(ExpressionType.Negate, expression, method);
			}
			if (expression.Type.IsArithmetic() && !expression.Type.IsUnsignedInt())
			{
				return new UnaryExpression(ExpressionType.Negate, expression, expression.Type, null);
			}
			return Expression.GetUserDefinedUnaryOperatorOrThrow(ExpressionType.Negate, "op_UnaryNegation", expression);
		}

		/// <summary>Creates a <see cref="T:System.Linq.Expressions.UnaryExpression" /> that represents a unary plus operation.</summary>
		/// <param name="expression">An <see cref="T:System.Linq.Expressions.Expression" /> to set the <see cref="P:System.Linq.Expressions.UnaryExpression.Operand" /> property equal to.</param>
		/// <returns>A <see cref="T:System.Linq.Expressions.UnaryExpression" /> that has the <see cref="P:System.Linq.Expressions.Expression.NodeType" /> property equal to <see cref="F:System.Linq.Expressions.ExpressionType.UnaryPlus" /> and the <see cref="P:System.Linq.Expressions.UnaryExpression.Operand" /> property set to the specified value.</returns>
		/// <exception cref="T:System.ArgumentNullException">
		///         <paramref name="expression" /> is <see langword="null" />.</exception>
		/// <exception cref="T:System.InvalidOperationException">The unary plus operator is not defined for <paramref name="expression" />.Type.</exception>
		public static UnaryExpression UnaryPlus(Expression expression)
		{
			return Expression.UnaryPlus(expression, null);
		}

		/// <summary>Creates a <see cref="T:System.Linq.Expressions.UnaryExpression" /> that represents a unary plus operation.</summary>
		/// <param name="expression">An <see cref="T:System.Linq.Expressions.Expression" /> to set the <see cref="P:System.Linq.Expressions.UnaryExpression.Operand" /> property equal to.</param>
		/// <param name="method">A <see cref="T:System.Reflection.MethodInfo" /> to set the <see cref="P:System.Linq.Expressions.UnaryExpression.Method" /> property equal to.</param>
		/// <returns>A <see cref="T:System.Linq.Expressions.UnaryExpression" /> that has the <see cref="P:System.Linq.Expressions.Expression.NodeType" /> property equal to <see cref="F:System.Linq.Expressions.ExpressionType.UnaryPlus" /> and the <see cref="P:System.Linq.Expressions.UnaryExpression.Operand" /> and <see cref="P:System.Linq.Expressions.UnaryExpression.Method" /> properties set to the specified values.</returns>
		/// <exception cref="T:System.ArgumentNullException">
		///         <paramref name="expression" /> is <see langword="null" />.</exception>
		/// <exception cref="T:System.ArgumentException">
		///         <paramref name="method" /> is not <see langword="null" /> and the method it represents returns <see langword="void" />, is not <see langword="static" /> (<see langword="Shared" /> in Visual Basic), or does not take exactly one argument.</exception>
		/// <exception cref="T:System.InvalidOperationException">
		///         <paramref name="method" /> is <see langword="null" /> and the unary plus operator is not defined for <paramref name="expression" />.Type.-or-
		///         <paramref name="expression" />.Type (or its corresponding non-nullable type if it is a nullable value type) is not assignable to the argument type of the method represented by <paramref name="method" />.</exception>
		public static UnaryExpression UnaryPlus(Expression expression, MethodInfo method)
		{
			ExpressionUtils.RequiresCanRead(expression, "expression");
			if (!(method == null))
			{
				return Expression.GetMethodBasedUnaryOperator(ExpressionType.UnaryPlus, expression, method);
			}
			if (expression.Type.IsArithmetic())
			{
				return new UnaryExpression(ExpressionType.UnaryPlus, expression, expression.Type, null);
			}
			return Expression.GetUserDefinedUnaryOperatorOrThrow(ExpressionType.UnaryPlus, "op_UnaryPlus", expression);
		}

		/// <summary>Creates a <see cref="T:System.Linq.Expressions.UnaryExpression" /> that represents an arithmetic negation operation that has overflow checking.</summary>
		/// <param name="expression">An <see cref="T:System.Linq.Expressions.Expression" /> to set the <see cref="P:System.Linq.Expressions.UnaryExpression.Operand" /> property equal to.</param>
		/// <returns>A <see cref="T:System.Linq.Expressions.UnaryExpression" /> that has the <see cref="P:System.Linq.Expressions.Expression.NodeType" /> property equal to <see cref="F:System.Linq.Expressions.ExpressionType.NegateChecked" /> and the <see cref="P:System.Linq.Expressions.UnaryExpression.Operand" /> property set to the specified value.</returns>
		/// <exception cref="T:System.ArgumentNullException">
		///         <paramref name="expression" /> is <see langword="null" />.</exception>
		/// <exception cref="T:System.InvalidOperationException">The unary minus operator is not defined for <paramref name="expression" />.Type.</exception>
		public static UnaryExpression NegateChecked(Expression expression)
		{
			return Expression.NegateChecked(expression, null);
		}

		/// <summary>Creates a <see cref="T:System.Linq.Expressions.UnaryExpression" /> that represents an arithmetic negation operation that has overflow checking. The implementing method can be specified.</summary>
		/// <param name="expression">An <see cref="T:System.Linq.Expressions.Expression" /> to set the <see cref="P:System.Linq.Expressions.UnaryExpression.Operand" /> property equal to.</param>
		/// <param name="method">A <see cref="T:System.Reflection.MethodInfo" /> to set the <see cref="P:System.Linq.Expressions.UnaryExpression.Method" /> property equal to.</param>
		/// <returns>A <see cref="T:System.Linq.Expressions.UnaryExpression" /> that has the <see cref="P:System.Linq.Expressions.Expression.NodeType" /> property equal to <see cref="F:System.Linq.Expressions.ExpressionType.NegateChecked" /> and the <see cref="P:System.Linq.Expressions.UnaryExpression.Operand" /> and <see cref="P:System.Linq.Expressions.UnaryExpression.Method" /> properties set to the specified values.</returns>
		/// <exception cref="T:System.ArgumentNullException">
		///         <paramref name="expression" /> is <see langword="null" />.</exception>
		/// <exception cref="T:System.ArgumentException">
		///         <paramref name="method" /> is not <see langword="null" /> and the method it represents returns <see langword="void" />, is not <see langword="static" /> (<see langword="Shared" /> in Visual Basic), or does not take exactly one argument.</exception>
		/// <exception cref="T:System.InvalidOperationException">
		///         <paramref name="method" /> is <see langword="null" /> and the unary minus operator is not defined for <paramref name="expression" />.Type.-or-
		///         <paramref name="expression" />.Type (or its corresponding non-nullable type if it is a nullable value type) is not assignable to the argument type of the method represented by <paramref name="method" />.</exception>
		public static UnaryExpression NegateChecked(Expression expression, MethodInfo method)
		{
			ExpressionUtils.RequiresCanRead(expression, "expression");
			if (!(method == null))
			{
				return Expression.GetMethodBasedUnaryOperator(ExpressionType.NegateChecked, expression, method);
			}
			if (expression.Type.IsArithmetic() && !expression.Type.IsUnsignedInt())
			{
				return new UnaryExpression(ExpressionType.NegateChecked, expression, expression.Type, null);
			}
			return Expression.GetUserDefinedUnaryOperatorOrThrow(ExpressionType.NegateChecked, "op_UnaryNegation", expression);
		}

		/// <summary>Creates a <see cref="T:System.Linq.Expressions.UnaryExpression" /> that represents a bitwise complement operation.</summary>
		/// <param name="expression">An <see cref="T:System.Linq.Expressions.Expression" /> to set the <see cref="P:System.Linq.Expressions.UnaryExpression.Operand" /> property equal to.</param>
		/// <returns>A <see cref="T:System.Linq.Expressions.UnaryExpression" /> that has the <see cref="P:System.Linq.Expressions.Expression.NodeType" /> property equal to <see cref="F:System.Linq.Expressions.ExpressionType.Not" /> and the <see cref="P:System.Linq.Expressions.UnaryExpression.Operand" /> property set to the specified value.</returns>
		/// <exception cref="T:System.ArgumentNullException">
		///         <paramref name="expression" /> is <see langword="null" />.</exception>
		/// <exception cref="T:System.InvalidOperationException">The unary not operator is not defined for <paramref name="expression" />.Type.</exception>
		public static UnaryExpression Not(Expression expression)
		{
			return Expression.Not(expression, null);
		}

		/// <summary>Creates a <see cref="T:System.Linq.Expressions.UnaryExpression" /> that represents a bitwise complement operation. The implementing method can be specified.</summary>
		/// <param name="expression">An <see cref="T:System.Linq.Expressions.Expression" /> to set the <see cref="P:System.Linq.Expressions.UnaryExpression.Operand" /> property equal to.</param>
		/// <param name="method">A <see cref="T:System.Reflection.MethodInfo" /> to set the <see cref="P:System.Linq.Expressions.UnaryExpression.Method" /> property equal to.</param>
		/// <returns>A <see cref="T:System.Linq.Expressions.UnaryExpression" /> that has the <see cref="P:System.Linq.Expressions.Expression.NodeType" /> property equal to <see cref="F:System.Linq.Expressions.ExpressionType.Not" /> and the <see cref="P:System.Linq.Expressions.UnaryExpression.Operand" /> and <see cref="P:System.Linq.Expressions.UnaryExpression.Method" /> properties set to the specified values.</returns>
		/// <exception cref="T:System.ArgumentNullException">
		///         <paramref name="expression" /> is <see langword="null" />.</exception>
		/// <exception cref="T:System.ArgumentException">
		///         <paramref name="method" /> is not <see langword="null" /> and the method it represents returns <see langword="void" />, is not <see langword="static" /> (<see langword="Shared" /> in Visual Basic), or does not take exactly one argument.</exception>
		/// <exception cref="T:System.InvalidOperationException">
		///         <paramref name="method" /> is <see langword="null" /> and the unary not operator is not defined for <paramref name="expression" />.Type.-or-
		///         <paramref name="expression" />.Type (or its corresponding non-nullable type if it is a nullable value type) is not assignable to the argument type of the method represented by <paramref name="method" />.</exception>
		public static UnaryExpression Not(Expression expression, MethodInfo method)
		{
			ExpressionUtils.RequiresCanRead(expression, "expression");
			if (!(method == null))
			{
				return Expression.GetMethodBasedUnaryOperator(ExpressionType.Not, expression, method);
			}
			if (expression.Type.IsIntegerOrBool())
			{
				return new UnaryExpression(ExpressionType.Not, expression, expression.Type, null);
			}
			UnaryExpression userDefinedUnaryOperator = Expression.GetUserDefinedUnaryOperator(ExpressionType.Not, "op_LogicalNot", expression);
			if (userDefinedUnaryOperator != null)
			{
				return userDefinedUnaryOperator;
			}
			return Expression.GetUserDefinedUnaryOperatorOrThrow(ExpressionType.Not, "op_OnesComplement", expression);
		}

		/// <summary>Returns whether the expression evaluates to false.</summary>
		/// <param name="expression">An <see cref="T:System.Linq.Expressions.Expression" /> to evaluate.</param>
		/// <returns>An instance of <see cref="T:System.Linq.Expressions.UnaryExpression" />.</returns>
		public static UnaryExpression IsFalse(Expression expression)
		{
			return Expression.IsFalse(expression, null);
		}

		/// <summary>Returns whether the expression evaluates to false.</summary>
		/// <param name="expression">An <see cref="T:System.Linq.Expressions.Expression" /> to evaluate.</param>
		/// <param name="method">A <see cref="T:System.Reflection.MethodInfo" /> that represents the implementing method.</param>
		/// <returns>An instance of <see cref="T:System.Linq.Expressions.UnaryExpression" />.</returns>
		public static UnaryExpression IsFalse(Expression expression, MethodInfo method)
		{
			ExpressionUtils.RequiresCanRead(expression, "expression");
			if (!(method == null))
			{
				return Expression.GetMethodBasedUnaryOperator(ExpressionType.IsFalse, expression, method);
			}
			if (expression.Type.IsBool())
			{
				return new UnaryExpression(ExpressionType.IsFalse, expression, expression.Type, null);
			}
			return Expression.GetUserDefinedUnaryOperatorOrThrow(ExpressionType.IsFalse, "op_False", expression);
		}

		/// <summary>Returns whether the expression evaluates to true.</summary>
		/// <param name="expression">An <see cref="T:System.Linq.Expressions.Expression" /> to evaluate.</param>
		/// <returns>An instance of <see cref="T:System.Linq.Expressions.UnaryExpression" />.</returns>
		public static UnaryExpression IsTrue(Expression expression)
		{
			return Expression.IsTrue(expression, null);
		}

		/// <summary>Returns whether the expression evaluates to true.</summary>
		/// <param name="expression">An <see cref="T:System.Linq.Expressions.Expression" /> to evaluate.</param>
		/// <param name="method">A <see cref="T:System.Reflection.MethodInfo" /> that represents the implementing method.</param>
		/// <returns>An instance of <see cref="T:System.Linq.Expressions.UnaryExpression" />.</returns>
		public static UnaryExpression IsTrue(Expression expression, MethodInfo method)
		{
			ExpressionUtils.RequiresCanRead(expression, "expression");
			if (!(method == null))
			{
				return Expression.GetMethodBasedUnaryOperator(ExpressionType.IsTrue, expression, method);
			}
			if (expression.Type.IsBool())
			{
				return new UnaryExpression(ExpressionType.IsTrue, expression, expression.Type, null);
			}
			return Expression.GetUserDefinedUnaryOperatorOrThrow(ExpressionType.IsTrue, "op_True", expression);
		}

		/// <summary>Returns the expression representing the ones complement.</summary>
		/// <param name="expression">An <see cref="T:System.Linq.Expressions.Expression" />.</param>
		/// <returns>An instance of <see cref="T:System.Linq.Expressions.UnaryExpression" />.</returns>
		public static UnaryExpression OnesComplement(Expression expression)
		{
			return Expression.OnesComplement(expression, null);
		}

		/// <summary>Returns the expression representing the ones complement.</summary>
		/// <param name="expression">An <see cref="T:System.Linq.Expressions.Expression" />.</param>
		/// <param name="method">A <see cref="T:System.Reflection.MethodInfo" /> that represents the implementing method.</param>
		/// <returns>An instance of <see cref="T:System.Linq.Expressions.UnaryExpression" />.</returns>
		public static UnaryExpression OnesComplement(Expression expression, MethodInfo method)
		{
			ExpressionUtils.RequiresCanRead(expression, "expression");
			if (!(method == null))
			{
				return Expression.GetMethodBasedUnaryOperator(ExpressionType.OnesComplement, expression, method);
			}
			if (expression.Type.IsInteger())
			{
				return new UnaryExpression(ExpressionType.OnesComplement, expression, expression.Type, null);
			}
			return Expression.GetUserDefinedUnaryOperatorOrThrow(ExpressionType.OnesComplement, "op_OnesComplement", expression);
		}

		/// <summary>Creates a <see cref="T:System.Linq.Expressions.UnaryExpression" /> that represents an explicit reference or boxing conversion where <see langword="null" /> is supplied if the conversion fails.</summary>
		/// <param name="expression">An <see cref="T:System.Linq.Expressions.Expression" /> to set the <see cref="P:System.Linq.Expressions.UnaryExpression.Operand" /> property equal to.</param>
		/// <param name="type">A <see cref="T:System.Type" /> to set the <see cref="P:System.Linq.Expressions.Expression.Type" /> property equal to.</param>
		/// <returns>A <see cref="T:System.Linq.Expressions.UnaryExpression" /> that has the <see cref="P:System.Linq.Expressions.Expression.NodeType" /> property equal to <see cref="F:System.Linq.Expressions.ExpressionType.TypeAs" /> and the <see cref="P:System.Linq.Expressions.UnaryExpression.Operand" /> and <see cref="P:System.Linq.Expressions.Expression.Type" /> properties set to the specified values.</returns>
		/// <exception cref="T:System.ArgumentNullException">
		///         <paramref name="expression" /> or <paramref name="type" /> is <see langword="null" />.</exception>
		public static UnaryExpression TypeAs(Expression expression, Type type)
		{
			ExpressionUtils.RequiresCanRead(expression, "expression");
			ContractUtils.RequiresNotNull(type, "type");
			TypeUtils.ValidateType(type, "type");
			if (type.IsValueType && !type.IsNullableType())
			{
				throw Error.IncorrectTypeForTypeAs(type, "type");
			}
			return new UnaryExpression(ExpressionType.TypeAs, expression, type, null);
		}

		/// <summary>Creates a <see cref="T:System.Linq.Expressions.UnaryExpression" /> that represents an explicit unboxing.</summary>
		/// <param name="expression">An <see cref="T:System.Linq.Expressions.Expression" /> to unbox.</param>
		/// <param name="type">The new <see cref="T:System.Type" /> of the expression.</param>
		/// <returns>An instance of <see cref="T:System.Linq.Expressions.UnaryExpression" />.</returns>
		public static UnaryExpression Unbox(Expression expression, Type type)
		{
			ExpressionUtils.RequiresCanRead(expression, "expression");
			ContractUtils.RequiresNotNull(type, "type");
			if (!expression.Type.IsInterface && expression.Type != typeof(object))
			{
				throw Error.InvalidUnboxType("expression");
			}
			if (!type.IsValueType)
			{
				throw Error.InvalidUnboxType("type");
			}
			TypeUtils.ValidateType(type, "type");
			return new UnaryExpression(ExpressionType.Unbox, expression, type, null);
		}

		/// <summary>Creates a <see cref="T:System.Linq.Expressions.UnaryExpression" /> that represents a type conversion operation.</summary>
		/// <param name="expression">An <see cref="T:System.Linq.Expressions.Expression" /> to set the <see cref="P:System.Linq.Expressions.UnaryExpression.Operand" /> property equal to.</param>
		/// <param name="type">A <see cref="T:System.Type" /> to set the <see cref="P:System.Linq.Expressions.Expression.Type" /> property equal to.</param>
		/// <returns>A <see cref="T:System.Linq.Expressions.UnaryExpression" /> that has the <see cref="P:System.Linq.Expressions.Expression.NodeType" /> property equal to <see cref="F:System.Linq.Expressions.ExpressionType.Convert" /> and the <see cref="P:System.Linq.Expressions.UnaryExpression.Operand" /> and <see cref="P:System.Linq.Expressions.Expression.Type" /> properties set to the specified values.</returns>
		/// <exception cref="T:System.ArgumentNullException">
		///         <paramref name="expression" /> or <paramref name="type" /> is <see langword="null" />.</exception>
		/// <exception cref="T:System.InvalidOperationException">No conversion operator is defined between <paramref name="expression" />.Type and <paramref name="type" />.</exception>
		public static UnaryExpression Convert(Expression expression, Type type)
		{
			return Expression.Convert(expression, type, null);
		}

		/// <summary>Creates a <see cref="T:System.Linq.Expressions.UnaryExpression" /> that represents a conversion operation for which the implementing method is specified.</summary>
		/// <param name="expression">An <see cref="T:System.Linq.Expressions.Expression" /> to set the <see cref="P:System.Linq.Expressions.UnaryExpression.Operand" /> property equal to.</param>
		/// <param name="type">A <see cref="T:System.Type" /> to set the <see cref="P:System.Linq.Expressions.Expression.Type" /> property equal to.</param>
		/// <param name="method">A <see cref="T:System.Reflection.MethodInfo" /> to set the <see cref="P:System.Linq.Expressions.UnaryExpression.Method" /> property equal to.</param>
		/// <returns>A <see cref="T:System.Linq.Expressions.UnaryExpression" /> that has the <see cref="P:System.Linq.Expressions.Expression.NodeType" /> property equal to <see cref="F:System.Linq.Expressions.ExpressionType.Convert" /> and the <see cref="P:System.Linq.Expressions.UnaryExpression.Operand" />, <see cref="P:System.Linq.Expressions.Expression.Type" />, and <see cref="P:System.Linq.Expressions.UnaryExpression.Method" /> properties set to the specified values.</returns>
		/// <exception cref="T:System.ArgumentNullException">
		///         <paramref name="expression" /> or <paramref name="type" /> is <see langword="null" />.</exception>
		/// <exception cref="T:System.ArgumentException">
		///         <paramref name="method" /> is not <see langword="null" /> and the method it represents returns <see langword="void" />, is not <see langword="static" /> (<see langword="Shared" /> in Visual Basic), or does not take exactly one argument.</exception>
		/// <exception cref="T:System.InvalidOperationException">No conversion operator is defined between <paramref name="expression" />.Type and <paramref name="type" />.-or-
		///         <paramref name="expression" />.Type is not assignable to the argument type of the method represented by <paramref name="method" />.-or-The return type of the method represented by <paramref name="method" /> is not assignable to <paramref name="type" />.-or-
		///         <paramref name="expression" />.Type or <paramref name="type" /> is a nullable value type and the corresponding non-nullable value type does not equal the argument type or the return type, respectively, of the method represented by <paramref name="method" />.</exception>
		/// <exception cref="T:System.Reflection.AmbiguousMatchException">More than one method that matches the <paramref name="method" /> description was found.</exception>
		public static UnaryExpression Convert(Expression expression, Type type, MethodInfo method)
		{
			ExpressionUtils.RequiresCanRead(expression, "expression");
			ContractUtils.RequiresNotNull(type, "type");
			TypeUtils.ValidateType(type, "type");
			if (!(method == null))
			{
				return Expression.GetMethodBasedCoercionOperator(ExpressionType.Convert, expression, type, method);
			}
			if (expression.Type.HasIdentityPrimitiveOrNullableConversionTo(type) || expression.Type.HasReferenceConversionTo(type))
			{
				return new UnaryExpression(ExpressionType.Convert, expression, type, null);
			}
			return Expression.GetUserDefinedCoercionOrThrow(ExpressionType.Convert, expression, type);
		}

		/// <summary>Creates a <see cref="T:System.Linq.Expressions.UnaryExpression" /> that represents a conversion operation that throws an exception if the target type is overflowed.</summary>
		/// <param name="expression">An <see cref="T:System.Linq.Expressions.Expression" /> to set the <see cref="P:System.Linq.Expressions.UnaryExpression.Operand" /> property equal to.</param>
		/// <param name="type">A <see cref="T:System.Type" /> to set the <see cref="P:System.Linq.Expressions.Expression.Type" /> property equal to.</param>
		/// <returns>A <see cref="T:System.Linq.Expressions.UnaryExpression" /> that has the <see cref="P:System.Linq.Expressions.Expression.NodeType" /> property equal to <see cref="F:System.Linq.Expressions.ExpressionType.ConvertChecked" /> and the <see cref="P:System.Linq.Expressions.UnaryExpression.Operand" /> and <see cref="P:System.Linq.Expressions.Expression.Type" /> properties set to the specified values.</returns>
		/// <exception cref="T:System.ArgumentNullException">
		///         <paramref name="expression" /> or <paramref name="type" /> is <see langword="null" />.</exception>
		/// <exception cref="T:System.InvalidOperationException">No conversion operator is defined between <paramref name="expression" />.Type and <paramref name="type" />.</exception>
		public static UnaryExpression ConvertChecked(Expression expression, Type type)
		{
			return Expression.ConvertChecked(expression, type, null);
		}

		/// <summary>Creates a <see cref="T:System.Linq.Expressions.UnaryExpression" /> that represents a conversion operation that throws an exception if the target type is overflowed and for which the implementing method is specified.</summary>
		/// <param name="expression">An <see cref="T:System.Linq.Expressions.Expression" /> to set the <see cref="P:System.Linq.Expressions.UnaryExpression.Operand" /> property equal to.</param>
		/// <param name="type">A <see cref="T:System.Type" /> to set the <see cref="P:System.Linq.Expressions.Expression.Type" /> property equal to.</param>
		/// <param name="method">A <see cref="T:System.Reflection.MethodInfo" /> to set the <see cref="P:System.Linq.Expressions.UnaryExpression.Method" /> property equal to.</param>
		/// <returns>A <see cref="T:System.Linq.Expressions.UnaryExpression" /> that has the <see cref="P:System.Linq.Expressions.Expression.NodeType" /> property equal to <see cref="F:System.Linq.Expressions.ExpressionType.ConvertChecked" /> and the <see cref="P:System.Linq.Expressions.UnaryExpression.Operand" />, <see cref="P:System.Linq.Expressions.Expression.Type" />, and <see cref="P:System.Linq.Expressions.UnaryExpression.Method" /> properties set to the specified values.</returns>
		/// <exception cref="T:System.ArgumentNullException">
		///         <paramref name="expression" /> or <paramref name="type" /> is <see langword="null" />.</exception>
		/// <exception cref="T:System.ArgumentException">
		///         <paramref name="method" /> is not <see langword="null" /> and the method it represents returns <see langword="void" />, is not <see langword="static" /> (<see langword="Shared" /> in Visual Basic), or does not take exactly one argument.</exception>
		/// <exception cref="T:System.InvalidOperationException">No conversion operator is defined between <paramref name="expression" />.Type and <paramref name="type" />.-or-
		///         <paramref name="expression" />.Type is not assignable to the argument type of the method represented by <paramref name="method" />.-or-The return type of the method represented by <paramref name="method" /> is not assignable to <paramref name="type" />.-or-
		///         <paramref name="expression" />.Type or <paramref name="type" /> is a nullable value type and the corresponding non-nullable value type does not equal the argument type or the return type, respectively, of the method represented by <paramref name="method" />.</exception>
		/// <exception cref="T:System.Reflection.AmbiguousMatchException">More than one method that matches the <paramref name="method" /> description was found.</exception>
		public static UnaryExpression ConvertChecked(Expression expression, Type type, MethodInfo method)
		{
			ExpressionUtils.RequiresCanRead(expression, "expression");
			ContractUtils.RequiresNotNull(type, "type");
			TypeUtils.ValidateType(type, "type");
			if (!(method == null))
			{
				return Expression.GetMethodBasedCoercionOperator(ExpressionType.ConvertChecked, expression, type, method);
			}
			if (expression.Type.HasIdentityPrimitiveOrNullableConversionTo(type))
			{
				return new UnaryExpression(ExpressionType.ConvertChecked, expression, type, null);
			}
			if (expression.Type.HasReferenceConversionTo(type))
			{
				return new UnaryExpression(ExpressionType.Convert, expression, type, null);
			}
			return Expression.GetUserDefinedCoercionOrThrow(ExpressionType.ConvertChecked, expression, type);
		}

		/// <summary>Creates a <see cref="T:System.Linq.Expressions.UnaryExpression" /> that represents an expression for obtaining the length of a one-dimensional array.</summary>
		/// <param name="array">An <see cref="T:System.Linq.Expressions.Expression" /> to set the <see cref="P:System.Linq.Expressions.UnaryExpression.Operand" /> property equal to.</param>
		/// <returns>A <see cref="T:System.Linq.Expressions.UnaryExpression" /> that has the <see cref="P:System.Linq.Expressions.Expression.NodeType" /> property equal to <see cref="F:System.Linq.Expressions.ExpressionType.ArrayLength" /> and the <see cref="P:System.Linq.Expressions.UnaryExpression.Operand" /> property equal to <paramref name="array" />.</returns>
		/// <exception cref="T:System.ArgumentNullException">
		///         <paramref name="array" /> is <see langword="null" />.</exception>
		/// <exception cref="T:System.ArgumentException">
		///         <paramref name="array" />.Type does not represent an array type.</exception>
		public static UnaryExpression ArrayLength(Expression array)
		{
			ExpressionUtils.RequiresCanRead(array, "array");
			if (array.Type.IsSZArray)
			{
				return new UnaryExpression(ExpressionType.ArrayLength, array, typeof(int), null);
			}
			if (!array.Type.IsArray || !typeof(Array).IsAssignableFrom(array.Type))
			{
				throw Error.ArgumentMustBeArray("array");
			}
			throw Error.ArgumentMustBeSingleDimensionalArrayType("array");
		}

		/// <summary>Creates a <see cref="T:System.Linq.Expressions.UnaryExpression" /> that represents an expression that has a constant value of type <see cref="T:System.Linq.Expressions.Expression" />.</summary>
		/// <param name="expression">An <see cref="T:System.Linq.Expressions.Expression" /> to set the <see cref="P:System.Linq.Expressions.UnaryExpression.Operand" /> property equal to.</param>
		/// <returns>A <see cref="T:System.Linq.Expressions.UnaryExpression" /> that has the <see cref="P:System.Linq.Expressions.Expression.NodeType" /> property equal to <see cref="F:System.Linq.Expressions.ExpressionType.Quote" /> and the <see cref="P:System.Linq.Expressions.UnaryExpression.Operand" /> property set to the specified value.</returns>
		/// <exception cref="T:System.ArgumentNullException">
		///         <paramref name="expression" /> is <see langword="null" />.</exception>
		public static UnaryExpression Quote(Expression expression)
		{
			ExpressionUtils.RequiresCanRead(expression, "expression");
			LambdaExpression lambdaExpression = expression as LambdaExpression;
			if (lambdaExpression == null)
			{
				throw Error.QuotedExpressionMustBeLambda("expression");
			}
			return new UnaryExpression(ExpressionType.Quote, lambdaExpression, lambdaExpression.PublicType, null);
		}

		/// <summary>Creates a <see cref="T:System.Linq.Expressions.UnaryExpression" /> that represents a rethrowing of an exception.</summary>
		/// <returns>A <see cref="T:System.Linq.Expressions.UnaryExpression" /> that represents a rethrowing of an exception.</returns>
		public static UnaryExpression Rethrow()
		{
			return Expression.Throw(null);
		}

		/// <summary>Creates a <see cref="T:System.Linq.Expressions.UnaryExpression" /> that represents a rethrowing of an exception with a given type.</summary>
		/// <param name="type">The new <see cref="T:System.Type" /> of the expression.</param>
		/// <returns>A <see cref="T:System.Linq.Expressions.UnaryExpression" /> that represents a rethrowing of an exception.</returns>
		public static UnaryExpression Rethrow(Type type)
		{
			return Expression.Throw(null, type);
		}

		/// <summary>Creates a <see cref="T:System.Linq.Expressions.UnaryExpression" /> that represents a throwing of an exception.</summary>
		/// <param name="value">An <see cref="T:System.Linq.Expressions.Expression" />.</param>
		/// <returns>A <see cref="T:System.Linq.Expressions.UnaryExpression" /> that represents the exception.</returns>
		public static UnaryExpression Throw(Expression value)
		{
			return Expression.Throw(value, typeof(void));
		}

		/// <summary>Creates a <see cref="T:System.Linq.Expressions.UnaryExpression" /> that represents a throwing of an exception with a given type.</summary>
		/// <param name="value">An <see cref="T:System.Linq.Expressions.Expression" />.</param>
		/// <param name="type">The new <see cref="T:System.Type" /> of the expression.</param>
		/// <returns>A <see cref="T:System.Linq.Expressions.UnaryExpression" /> that represents the exception.</returns>
		public static UnaryExpression Throw(Expression value, Type type)
		{
			ContractUtils.RequiresNotNull(type, "type");
			TypeUtils.ValidateType(type, "type");
			if (value != null)
			{
				ExpressionUtils.RequiresCanRead(value, "value");
				if (value.Type.IsValueType)
				{
					throw Error.ArgumentMustNotHaveValueType("value");
				}
			}
			return new UnaryExpression(ExpressionType.Throw, value, type, null);
		}

		/// <summary>Creates a <see cref="T:System.Linq.Expressions.UnaryExpression" /> that represents the incrementing of the expression value by 1.</summary>
		/// <param name="expression">An <see cref="T:System.Linq.Expressions.Expression" /> to increment.</param>
		/// <returns>A <see cref="T:System.Linq.Expressions.UnaryExpression" /> that represents the incremented expression.</returns>
		public static UnaryExpression Increment(Expression expression)
		{
			return Expression.Increment(expression, null);
		}

		/// <summary>Creates a <see cref="T:System.Linq.Expressions.UnaryExpression" /> that represents the incrementing of the expression by 1.</summary>
		/// <param name="expression">An <see cref="T:System.Linq.Expressions.Expression" /> to increment.</param>
		/// <param name="method">A <see cref="T:System.Reflection.MethodInfo" /> that represents the implementing method.</param>
		/// <returns>A <see cref="T:System.Linq.Expressions.UnaryExpression" /> that represents the incremented expression.</returns>
		public static UnaryExpression Increment(Expression expression, MethodInfo method)
		{
			ExpressionUtils.RequiresCanRead(expression, "expression");
			if (!(method == null))
			{
				return Expression.GetMethodBasedUnaryOperator(ExpressionType.Increment, expression, method);
			}
			if (expression.Type.IsArithmetic())
			{
				return new UnaryExpression(ExpressionType.Increment, expression, expression.Type, null);
			}
			return Expression.GetUserDefinedUnaryOperatorOrThrow(ExpressionType.Increment, "op_Increment", expression);
		}

		/// <summary>Creates a <see cref="T:System.Linq.Expressions.UnaryExpression" /> that represents the decrementing of the expression by 1.</summary>
		/// <param name="expression">An <see cref="T:System.Linq.Expressions.Expression" /> to decrement.</param>
		/// <returns>A <see cref="T:System.Linq.Expressions.UnaryExpression" /> that represents the decremented expression.</returns>
		public static UnaryExpression Decrement(Expression expression)
		{
			return Expression.Decrement(expression, null);
		}

		/// <summary>Creates a <see cref="T:System.Linq.Expressions.UnaryExpression" /> that represents the decrementing of the expression by 1.</summary>
		/// <param name="expression">An <see cref="T:System.Linq.Expressions.Expression" /> to decrement.</param>
		/// <param name="method">A <see cref="T:System.Reflection.MethodInfo" /> that represents the implementing method.</param>
		/// <returns>A <see cref="T:System.Linq.Expressions.UnaryExpression" /> that represents the decremented expression.</returns>
		public static UnaryExpression Decrement(Expression expression, MethodInfo method)
		{
			ExpressionUtils.RequiresCanRead(expression, "expression");
			if (!(method == null))
			{
				return Expression.GetMethodBasedUnaryOperator(ExpressionType.Decrement, expression, method);
			}
			if (expression.Type.IsArithmetic())
			{
				return new UnaryExpression(ExpressionType.Decrement, expression, expression.Type, null);
			}
			return Expression.GetUserDefinedUnaryOperatorOrThrow(ExpressionType.Decrement, "op_Decrement", expression);
		}

		/// <summary>Creates a <see cref="T:System.Linq.Expressions.UnaryExpression" /> that increments the expression by 1 and assigns the result back to the expression.</summary>
		/// <param name="expression">An <see cref="T:System.Linq.Expressions.Expression" /> to apply the operations on.</param>
		/// <returns>A <see cref="T:System.Linq.Expressions.UnaryExpression" /> that represents the resultant expression.</returns>
		public static UnaryExpression PreIncrementAssign(Expression expression)
		{
			return Expression.MakeOpAssignUnary(ExpressionType.PreIncrementAssign, expression, null);
		}

		/// <summary>Creates a <see cref="T:System.Linq.Expressions.UnaryExpression" /> that increments the expression by 1 and assigns the result back to the expression.</summary>
		/// <param name="expression">An <see cref="T:System.Linq.Expressions.Expression" /> to apply the operations on.</param>
		/// <param name="method">A <see cref="T:System.Reflection.MethodInfo" /> that represents the implementing method.</param>
		/// <returns>A <see cref="T:System.Linq.Expressions.UnaryExpression" /> that represents the resultant expression.</returns>
		public static UnaryExpression PreIncrementAssign(Expression expression, MethodInfo method)
		{
			return Expression.MakeOpAssignUnary(ExpressionType.PreIncrementAssign, expression, method);
		}

		/// <summary>Creates a <see cref="T:System.Linq.Expressions.UnaryExpression" /> that decrements the expression by 1 and assigns the result back to the expression.</summary>
		/// <param name="expression">An <see cref="T:System.Linq.Expressions.Expression" /> to apply the operations on.</param>
		/// <returns>A <see cref="T:System.Linq.Expressions.UnaryExpression" /> that represents the resultant expression.</returns>
		public static UnaryExpression PreDecrementAssign(Expression expression)
		{
			return Expression.MakeOpAssignUnary(ExpressionType.PreDecrementAssign, expression, null);
		}

		/// <summary>Creates a <see cref="T:System.Linq.Expressions.UnaryExpression" /> that decrements the expression by 1 and assigns the result back to the expression.</summary>
		/// <param name="expression">An <see cref="T:System.Linq.Expressions.Expression" /> to apply the operations on.</param>
		/// <param name="method">A <see cref="T:System.Reflection.MethodInfo" /> that represents the implementing method.</param>
		/// <returns>A <see cref="T:System.Linq.Expressions.UnaryExpression" /> that represents the resultant expression.</returns>
		public static UnaryExpression PreDecrementAssign(Expression expression, MethodInfo method)
		{
			return Expression.MakeOpAssignUnary(ExpressionType.PreDecrementAssign, expression, method);
		}

		/// <summary>Creates a <see cref="T:System.Linq.Expressions.UnaryExpression" /> that represents the assignment of the expression followed by a subsequent increment by 1 of the original expression.</summary>
		/// <param name="expression">An <see cref="T:System.Linq.Expressions.Expression" /> to apply the operations on.</param>
		/// <returns>A <see cref="T:System.Linq.Expressions.UnaryExpression" /> that represents the resultant expression.</returns>
		public static UnaryExpression PostIncrementAssign(Expression expression)
		{
			return Expression.MakeOpAssignUnary(ExpressionType.PostIncrementAssign, expression, null);
		}

		/// <summary>Creates a <see cref="T:System.Linq.Expressions.UnaryExpression" /> that represents the assignment of the expression followed by a subsequent increment by 1 of the original expression.</summary>
		/// <param name="expression">An <see cref="T:System.Linq.Expressions.Expression" /> to apply the operations on.</param>
		/// <param name="method">A <see cref="T:System.Reflection.MethodInfo" /> that represents the implementing method.</param>
		/// <returns>A <see cref="T:System.Linq.Expressions.UnaryExpression" /> that represents the resultant expression.</returns>
		public static UnaryExpression PostIncrementAssign(Expression expression, MethodInfo method)
		{
			return Expression.MakeOpAssignUnary(ExpressionType.PostIncrementAssign, expression, method);
		}

		/// <summary>Creates a <see cref="T:System.Linq.Expressions.UnaryExpression" /> that represents the assignment of the expression followed by a subsequent decrement by 1 of the original expression.</summary>
		/// <param name="expression">An <see cref="T:System.Linq.Expressions.Expression" /> to apply the operations on.</param>
		/// <returns>A <see cref="T:System.Linq.Expressions.UnaryExpression" /> that represents the resultant expression.</returns>
		public static UnaryExpression PostDecrementAssign(Expression expression)
		{
			return Expression.MakeOpAssignUnary(ExpressionType.PostDecrementAssign, expression, null);
		}

		/// <summary>Creates a <see cref="T:System.Linq.Expressions.UnaryExpression" /> that represents the assignment of the expression followed by a subsequent decrement by 1 of the original expression.</summary>
		/// <param name="expression">An <see cref="T:System.Linq.Expressions.Expression" /> to apply the operations on.</param>
		/// <param name="method">A <see cref="T:System.Reflection.MethodInfo" /> that represents the implementing method.</param>
		/// <returns>A <see cref="T:System.Linq.Expressions.UnaryExpression" /> that represents the resultant expression.</returns>
		public static UnaryExpression PostDecrementAssign(Expression expression, MethodInfo method)
		{
			return Expression.MakeOpAssignUnary(ExpressionType.PostDecrementAssign, expression, method);
		}

		private static UnaryExpression MakeOpAssignUnary(ExpressionType kind, Expression expression, MethodInfo method)
		{
			ExpressionUtils.RequiresCanRead(expression, "expression");
			Expression.RequiresCanWrite(expression, "expression");
			UnaryExpression unaryExpression;
			if (method == null)
			{
				if (expression.Type.IsArithmetic())
				{
					return new UnaryExpression(kind, expression, expression.Type, null);
				}
				string name;
				if (kind == ExpressionType.PreIncrementAssign || kind == ExpressionType.PostIncrementAssign)
				{
					name = "op_Increment";
				}
				else
				{
					name = "op_Decrement";
				}
				unaryExpression = Expression.GetUserDefinedUnaryOperatorOrThrow(kind, name, expression);
			}
			else
			{
				unaryExpression = Expression.GetMethodBasedUnaryOperator(kind, expression, method);
			}
			if (!TypeUtils.AreReferenceAssignable(expression.Type, unaryExpression.Type))
			{
				throw Error.UserDefinedOpMustHaveValidReturnType(kind, method.Name);
			}
			return unaryExpression;
		}

		private static readonly CacheDict<Type, MethodInfo> s_lambdaDelegateCache = new CacheDict<Type, MethodInfo>(40);

		private static volatile CacheDict<Type, Func<Expression, string, bool, ReadOnlyCollection<ParameterExpression>, LambdaExpression>> s_lambdaFactories;

		private static ConditionalWeakTable<Expression, Expression.ExtensionInfo> s_legacyCtorSupportTable;

		internal class BinaryExpressionProxy
		{
			public BinaryExpressionProxy(BinaryExpression node)
			{
				ContractUtils.RequiresNotNull(node, "node");
				this._node = node;
			}

			public bool CanReduce
			{
				get
				{
					return this._node.CanReduce;
				}
			}

			public LambdaExpression Conversion
			{
				get
				{
					return this._node.Conversion;
				}
			}

			public string DebugView
			{
				get
				{
					return this._node.DebugView;
				}
			}

			public bool IsLifted
			{
				get
				{
					return this._node.IsLifted;
				}
			}

			public bool IsLiftedToNull
			{
				get
				{
					return this._node.IsLiftedToNull;
				}
			}

			public Expression Left
			{
				get
				{
					return this._node.Left;
				}
			}

			public MethodInfo Method
			{
				get
				{
					return this._node.Method;
				}
			}

			public ExpressionType NodeType
			{
				get
				{
					return this._node.NodeType;
				}
			}

			public Expression Right
			{
				get
				{
					return this._node.Right;
				}
			}

			public Type Type
			{
				get
				{
					return this._node.Type;
				}
			}

			private readonly BinaryExpression _node;
		}

		internal class BlockExpressionProxy
		{
			public BlockExpressionProxy(BlockExpression node)
			{
				ContractUtils.RequiresNotNull(node, "node");
				this._node = node;
			}

			public bool CanReduce
			{
				get
				{
					return this._node.CanReduce;
				}
			}

			public string DebugView
			{
				get
				{
					return this._node.DebugView;
				}
			}

			public ReadOnlyCollection<Expression> Expressions
			{
				get
				{
					return this._node.Expressions;
				}
			}

			public ExpressionType NodeType
			{
				get
				{
					return this._node.NodeType;
				}
			}

			public Expression Result
			{
				get
				{
					return this._node.Result;
				}
			}

			public Type Type
			{
				get
				{
					return this._node.Type;
				}
			}

			public ReadOnlyCollection<ParameterExpression> Variables
			{
				get
				{
					return this._node.Variables;
				}
			}

			private readonly BlockExpression _node;
		}

		internal class CatchBlockProxy
		{
			public CatchBlockProxy(CatchBlock node)
			{
				ContractUtils.RequiresNotNull(node, "node");
				this._node = node;
			}

			public Expression Body
			{
				get
				{
					return this._node.Body;
				}
			}

			public Expression Filter
			{
				get
				{
					return this._node.Filter;
				}
			}

			public Type Test
			{
				get
				{
					return this._node.Test;
				}
			}

			public ParameterExpression Variable
			{
				get
				{
					return this._node.Variable;
				}
			}

			private readonly CatchBlock _node;
		}

		internal class ConditionalExpressionProxy
		{
			public ConditionalExpressionProxy(ConditionalExpression node)
			{
				ContractUtils.RequiresNotNull(node, "node");
				this._node = node;
			}

			public bool CanReduce
			{
				get
				{
					return this._node.CanReduce;
				}
			}

			public string DebugView
			{
				get
				{
					return this._node.DebugView;
				}
			}

			public Expression IfFalse
			{
				get
				{
					return this._node.IfFalse;
				}
			}

			public Expression IfTrue
			{
				get
				{
					return this._node.IfTrue;
				}
			}

			public ExpressionType NodeType
			{
				get
				{
					return this._node.NodeType;
				}
			}

			public Expression Test
			{
				get
				{
					return this._node.Test;
				}
			}

			public Type Type
			{
				get
				{
					return this._node.Type;
				}
			}

			private readonly ConditionalExpression _node;
		}

		internal class ConstantExpressionProxy
		{
			public ConstantExpressionProxy(ConstantExpression node)
			{
				ContractUtils.RequiresNotNull(node, "node");
				this._node = node;
			}

			public bool CanReduce
			{
				get
				{
					return this._node.CanReduce;
				}
			}

			public string DebugView
			{
				get
				{
					return this._node.DebugView;
				}
			}

			public ExpressionType NodeType
			{
				get
				{
					return this._node.NodeType;
				}
			}

			public Type Type
			{
				get
				{
					return this._node.Type;
				}
			}

			public object Value
			{
				get
				{
					return this._node.Value;
				}
			}

			private readonly ConstantExpression _node;
		}

		internal class DebugInfoExpressionProxy
		{
			public DebugInfoExpressionProxy(DebugInfoExpression node)
			{
				ContractUtils.RequiresNotNull(node, "node");
				this._node = node;
			}

			public bool CanReduce
			{
				get
				{
					return this._node.CanReduce;
				}
			}

			public string DebugView
			{
				get
				{
					return this._node.DebugView;
				}
			}

			public SymbolDocumentInfo Document
			{
				get
				{
					return this._node.Document;
				}
			}

			public int EndColumn
			{
				get
				{
					return this._node.EndColumn;
				}
			}

			public int EndLine
			{
				get
				{
					return this._node.EndLine;
				}
			}

			public bool IsClear
			{
				get
				{
					return this._node.IsClear;
				}
			}

			public ExpressionType NodeType
			{
				get
				{
					return this._node.NodeType;
				}
			}

			public int StartColumn
			{
				get
				{
					return this._node.StartColumn;
				}
			}

			public int StartLine
			{
				get
				{
					return this._node.StartLine;
				}
			}

			public Type Type
			{
				get
				{
					return this._node.Type;
				}
			}

			private readonly DebugInfoExpression _node;
		}

		internal class DefaultExpressionProxy
		{
			public DefaultExpressionProxy(DefaultExpression node)
			{
				ContractUtils.RequiresNotNull(node, "node");
				this._node = node;
			}

			public bool CanReduce
			{
				get
				{
					return this._node.CanReduce;
				}
			}

			public string DebugView
			{
				get
				{
					return this._node.DebugView;
				}
			}

			public ExpressionType NodeType
			{
				get
				{
					return this._node.NodeType;
				}
			}

			public Type Type
			{
				get
				{
					return this._node.Type;
				}
			}

			private readonly DefaultExpression _node;
		}

		internal class GotoExpressionProxy
		{
			public GotoExpressionProxy(GotoExpression node)
			{
				ContractUtils.RequiresNotNull(node, "node");
				this._node = node;
			}

			public bool CanReduce
			{
				get
				{
					return this._node.CanReduce;
				}
			}

			public string DebugView
			{
				get
				{
					return this._node.DebugView;
				}
			}

			public GotoExpressionKind Kind
			{
				get
				{
					return this._node.Kind;
				}
			}

			public ExpressionType NodeType
			{
				get
				{
					return this._node.NodeType;
				}
			}

			public LabelTarget Target
			{
				get
				{
					return this._node.Target;
				}
			}

			public Type Type
			{
				get
				{
					return this._node.Type;
				}
			}

			public Expression Value
			{
				get
				{
					return this._node.Value;
				}
			}

			private readonly GotoExpression _node;
		}

		internal class IndexExpressionProxy
		{
			public IndexExpressionProxy(IndexExpression node)
			{
				ContractUtils.RequiresNotNull(node, "node");
				this._node = node;
			}

			public ReadOnlyCollection<Expression> Arguments
			{
				get
				{
					return this._node.Arguments;
				}
			}

			public bool CanReduce
			{
				get
				{
					return this._node.CanReduce;
				}
			}

			public string DebugView
			{
				get
				{
					return this._node.DebugView;
				}
			}

			public PropertyInfo Indexer
			{
				get
				{
					return this._node.Indexer;
				}
			}

			public ExpressionType NodeType
			{
				get
				{
					return this._node.NodeType;
				}
			}

			public Expression Object
			{
				get
				{
					return this._node.Object;
				}
			}

			public Type Type
			{
				get
				{
					return this._node.Type;
				}
			}

			private readonly IndexExpression _node;
		}

		internal class InvocationExpressionProxy
		{
			public InvocationExpressionProxy(InvocationExpression node)
			{
				ContractUtils.RequiresNotNull(node, "node");
				this._node = node;
			}

			public ReadOnlyCollection<Expression> Arguments
			{
				get
				{
					return this._node.Arguments;
				}
			}

			public bool CanReduce
			{
				get
				{
					return this._node.CanReduce;
				}
			}

			public string DebugView
			{
				get
				{
					return this._node.DebugView;
				}
			}

			public Expression Expression
			{
				get
				{
					return this._node.Expression;
				}
			}

			public ExpressionType NodeType
			{
				get
				{
					return this._node.NodeType;
				}
			}

			public Type Type
			{
				get
				{
					return this._node.Type;
				}
			}

			private readonly InvocationExpression _node;
		}

		internal class LabelExpressionProxy
		{
			public LabelExpressionProxy(LabelExpression node)
			{
				ContractUtils.RequiresNotNull(node, "node");
				this._node = node;
			}

			public bool CanReduce
			{
				get
				{
					return this._node.CanReduce;
				}
			}

			public string DebugView
			{
				get
				{
					return this._node.DebugView;
				}
			}

			public Expression DefaultValue
			{
				get
				{
					return this._node.DefaultValue;
				}
			}

			public ExpressionType NodeType
			{
				get
				{
					return this._node.NodeType;
				}
			}

			public LabelTarget Target
			{
				get
				{
					return this._node.Target;
				}
			}

			public Type Type
			{
				get
				{
					return this._node.Type;
				}
			}

			private readonly LabelExpression _node;
		}

		internal class LambdaExpressionProxy
		{
			public LambdaExpressionProxy(LambdaExpression node)
			{
				ContractUtils.RequiresNotNull(node, "node");
				this._node = node;
			}

			public Expression Body
			{
				get
				{
					return this._node.Body;
				}
			}

			public bool CanReduce
			{
				get
				{
					return this._node.CanReduce;
				}
			}

			public string DebugView
			{
				get
				{
					return this._node.DebugView;
				}
			}

			public string Name
			{
				get
				{
					return this._node.Name;
				}
			}

			public ExpressionType NodeType
			{
				get
				{
					return this._node.NodeType;
				}
			}

			public ReadOnlyCollection<ParameterExpression> Parameters
			{
				get
				{
					return this._node.Parameters;
				}
			}

			public Type ReturnType
			{
				get
				{
					return this._node.ReturnType;
				}
			}

			public bool TailCall
			{
				get
				{
					return this._node.TailCall;
				}
			}

			public Type Type
			{
				get
				{
					return this._node.Type;
				}
			}

			private readonly LambdaExpression _node;
		}

		internal class ListInitExpressionProxy
		{
			public ListInitExpressionProxy(ListInitExpression node)
			{
				ContractUtils.RequiresNotNull(node, "node");
				this._node = node;
			}

			public bool CanReduce
			{
				get
				{
					return this._node.CanReduce;
				}
			}

			public string DebugView
			{
				get
				{
					return this._node.DebugView;
				}
			}

			public ReadOnlyCollection<ElementInit> Initializers
			{
				get
				{
					return this._node.Initializers;
				}
			}

			public NewExpression NewExpression
			{
				get
				{
					return this._node.NewExpression;
				}
			}

			public ExpressionType NodeType
			{
				get
				{
					return this._node.NodeType;
				}
			}

			public Type Type
			{
				get
				{
					return this._node.Type;
				}
			}

			private readonly ListInitExpression _node;
		}

		internal class LoopExpressionProxy
		{
			public LoopExpressionProxy(LoopExpression node)
			{
				ContractUtils.RequiresNotNull(node, "node");
				this._node = node;
			}

			public Expression Body
			{
				get
				{
					return this._node.Body;
				}
			}

			public LabelTarget BreakLabel
			{
				get
				{
					return this._node.BreakLabel;
				}
			}

			public bool CanReduce
			{
				get
				{
					return this._node.CanReduce;
				}
			}

			public LabelTarget ContinueLabel
			{
				get
				{
					return this._node.ContinueLabel;
				}
			}

			public string DebugView
			{
				get
				{
					return this._node.DebugView;
				}
			}

			public ExpressionType NodeType
			{
				get
				{
					return this._node.NodeType;
				}
			}

			public Type Type
			{
				get
				{
					return this._node.Type;
				}
			}

			private readonly LoopExpression _node;
		}

		internal class MemberExpressionProxy
		{
			public MemberExpressionProxy(MemberExpression node)
			{
				ContractUtils.RequiresNotNull(node, "node");
				this._node = node;
			}

			public bool CanReduce
			{
				get
				{
					return this._node.CanReduce;
				}
			}

			public string DebugView
			{
				get
				{
					return this._node.DebugView;
				}
			}

			public Expression Expression
			{
				get
				{
					return this._node.Expression;
				}
			}

			public MemberInfo Member
			{
				get
				{
					return this._node.Member;
				}
			}

			public ExpressionType NodeType
			{
				get
				{
					return this._node.NodeType;
				}
			}

			public Type Type
			{
				get
				{
					return this._node.Type;
				}
			}

			private readonly MemberExpression _node;
		}

		internal class MemberInitExpressionProxy
		{
			public MemberInitExpressionProxy(MemberInitExpression node)
			{
				ContractUtils.RequiresNotNull(node, "node");
				this._node = node;
			}

			public ReadOnlyCollection<MemberBinding> Bindings
			{
				get
				{
					return this._node.Bindings;
				}
			}

			public bool CanReduce
			{
				get
				{
					return this._node.CanReduce;
				}
			}

			public string DebugView
			{
				get
				{
					return this._node.DebugView;
				}
			}

			public NewExpression NewExpression
			{
				get
				{
					return this._node.NewExpression;
				}
			}

			public ExpressionType NodeType
			{
				get
				{
					return this._node.NodeType;
				}
			}

			public Type Type
			{
				get
				{
					return this._node.Type;
				}
			}

			private readonly MemberInitExpression _node;
		}

		internal class MethodCallExpressionProxy
		{
			public MethodCallExpressionProxy(MethodCallExpression node)
			{
				ContractUtils.RequiresNotNull(node, "node");
				this._node = node;
			}

			public ReadOnlyCollection<Expression> Arguments
			{
				get
				{
					return this._node.Arguments;
				}
			}

			public bool CanReduce
			{
				get
				{
					return this._node.CanReduce;
				}
			}

			public string DebugView
			{
				get
				{
					return this._node.DebugView;
				}
			}

			public MethodInfo Method
			{
				get
				{
					return this._node.Method;
				}
			}

			public ExpressionType NodeType
			{
				get
				{
					return this._node.NodeType;
				}
			}

			public Expression Object
			{
				get
				{
					return this._node.Object;
				}
			}

			public Type Type
			{
				get
				{
					return this._node.Type;
				}
			}

			private readonly MethodCallExpression _node;
		}

		internal class NewArrayExpressionProxy
		{
			public NewArrayExpressionProxy(NewArrayExpression node)
			{
				ContractUtils.RequiresNotNull(node, "node");
				this._node = node;
			}

			public bool CanReduce
			{
				get
				{
					return this._node.CanReduce;
				}
			}

			public string DebugView
			{
				get
				{
					return this._node.DebugView;
				}
			}

			public ReadOnlyCollection<Expression> Expressions
			{
				get
				{
					return this._node.Expressions;
				}
			}

			public ExpressionType NodeType
			{
				get
				{
					return this._node.NodeType;
				}
			}

			public Type Type
			{
				get
				{
					return this._node.Type;
				}
			}

			private readonly NewArrayExpression _node;
		}

		internal class NewExpressionProxy
		{
			public NewExpressionProxy(NewExpression node)
			{
				ContractUtils.RequiresNotNull(node, "node");
				this._node = node;
			}

			public ReadOnlyCollection<Expression> Arguments
			{
				get
				{
					return this._node.Arguments;
				}
			}

			public bool CanReduce
			{
				get
				{
					return this._node.CanReduce;
				}
			}

			public ConstructorInfo Constructor
			{
				get
				{
					return this._node.Constructor;
				}
			}

			public string DebugView
			{
				get
				{
					return this._node.DebugView;
				}
			}

			public ReadOnlyCollection<MemberInfo> Members
			{
				get
				{
					return this._node.Members;
				}
			}

			public ExpressionType NodeType
			{
				get
				{
					return this._node.NodeType;
				}
			}

			public Type Type
			{
				get
				{
					return this._node.Type;
				}
			}

			private readonly NewExpression _node;
		}

		internal class ParameterExpressionProxy
		{
			public ParameterExpressionProxy(ParameterExpression node)
			{
				ContractUtils.RequiresNotNull(node, "node");
				this._node = node;
			}

			public bool CanReduce
			{
				get
				{
					return this._node.CanReduce;
				}
			}

			public string DebugView
			{
				get
				{
					return this._node.DebugView;
				}
			}

			public bool IsByRef
			{
				get
				{
					return this._node.IsByRef;
				}
			}

			public string Name
			{
				get
				{
					return this._node.Name;
				}
			}

			public ExpressionType NodeType
			{
				get
				{
					return this._node.NodeType;
				}
			}

			public Type Type
			{
				get
				{
					return this._node.Type;
				}
			}

			private readonly ParameterExpression _node;
		}

		internal class RuntimeVariablesExpressionProxy
		{
			public RuntimeVariablesExpressionProxy(RuntimeVariablesExpression node)
			{
				ContractUtils.RequiresNotNull(node, "node");
				this._node = node;
			}

			public bool CanReduce
			{
				get
				{
					return this._node.CanReduce;
				}
			}

			public string DebugView
			{
				get
				{
					return this._node.DebugView;
				}
			}

			public ExpressionType NodeType
			{
				get
				{
					return this._node.NodeType;
				}
			}

			public Type Type
			{
				get
				{
					return this._node.Type;
				}
			}

			public ReadOnlyCollection<ParameterExpression> Variables
			{
				get
				{
					return this._node.Variables;
				}
			}

			private readonly RuntimeVariablesExpression _node;
		}

		internal class SwitchCaseProxy
		{
			public SwitchCaseProxy(SwitchCase node)
			{
				ContractUtils.RequiresNotNull(node, "node");
				this._node = node;
			}

			public Expression Body
			{
				get
				{
					return this._node.Body;
				}
			}

			public ReadOnlyCollection<Expression> TestValues
			{
				get
				{
					return this._node.TestValues;
				}
			}

			private readonly SwitchCase _node;
		}

		internal class SwitchExpressionProxy
		{
			public SwitchExpressionProxy(SwitchExpression node)
			{
				ContractUtils.RequiresNotNull(node, "node");
				this._node = node;
			}

			public bool CanReduce
			{
				get
				{
					return this._node.CanReduce;
				}
			}

			public ReadOnlyCollection<SwitchCase> Cases
			{
				get
				{
					return this._node.Cases;
				}
			}

			public MethodInfo Comparison
			{
				get
				{
					return this._node.Comparison;
				}
			}

			public string DebugView
			{
				get
				{
					return this._node.DebugView;
				}
			}

			public Expression DefaultBody
			{
				get
				{
					return this._node.DefaultBody;
				}
			}

			public ExpressionType NodeType
			{
				get
				{
					return this._node.NodeType;
				}
			}

			public Expression SwitchValue
			{
				get
				{
					return this._node.SwitchValue;
				}
			}

			public Type Type
			{
				get
				{
					return this._node.Type;
				}
			}

			private readonly SwitchExpression _node;
		}

		internal class TryExpressionProxy
		{
			public TryExpressionProxy(TryExpression node)
			{
				ContractUtils.RequiresNotNull(node, "node");
				this._node = node;
			}

			public Expression Body
			{
				get
				{
					return this._node.Body;
				}
			}

			public bool CanReduce
			{
				get
				{
					return this._node.CanReduce;
				}
			}

			public string DebugView
			{
				get
				{
					return this._node.DebugView;
				}
			}

			public Expression Fault
			{
				get
				{
					return this._node.Fault;
				}
			}

			public Expression Finally
			{
				get
				{
					return this._node.Finally;
				}
			}

			public ReadOnlyCollection<CatchBlock> Handlers
			{
				get
				{
					return this._node.Handlers;
				}
			}

			public ExpressionType NodeType
			{
				get
				{
					return this._node.NodeType;
				}
			}

			public Type Type
			{
				get
				{
					return this._node.Type;
				}
			}

			private readonly TryExpression _node;
		}

		internal class TypeBinaryExpressionProxy
		{
			public TypeBinaryExpressionProxy(TypeBinaryExpression node)
			{
				ContractUtils.RequiresNotNull(node, "node");
				this._node = node;
			}

			public bool CanReduce
			{
				get
				{
					return this._node.CanReduce;
				}
			}

			public string DebugView
			{
				get
				{
					return this._node.DebugView;
				}
			}

			public Expression Expression
			{
				get
				{
					return this._node.Expression;
				}
			}

			public ExpressionType NodeType
			{
				get
				{
					return this._node.NodeType;
				}
			}

			public Type Type
			{
				get
				{
					return this._node.Type;
				}
			}

			public Type TypeOperand
			{
				get
				{
					return this._node.TypeOperand;
				}
			}

			private readonly TypeBinaryExpression _node;
		}

		internal class UnaryExpressionProxy
		{
			public UnaryExpressionProxy(UnaryExpression node)
			{
				ContractUtils.RequiresNotNull(node, "node");
				this._node = node;
			}

			public bool CanReduce
			{
				get
				{
					return this._node.CanReduce;
				}
			}

			public string DebugView
			{
				get
				{
					return this._node.DebugView;
				}
			}

			public bool IsLifted
			{
				get
				{
					return this._node.IsLifted;
				}
			}

			public bool IsLiftedToNull
			{
				get
				{
					return this._node.IsLiftedToNull;
				}
			}

			public MethodInfo Method
			{
				get
				{
					return this._node.Method;
				}
			}

			public ExpressionType NodeType
			{
				get
				{
					return this._node.NodeType;
				}
			}

			public Expression Operand
			{
				get
				{
					return this._node.Operand;
				}
			}

			public Type Type
			{
				get
				{
					return this._node.Type;
				}
			}

			private readonly UnaryExpression _node;
		}

		private class ExtensionInfo
		{
			public ExtensionInfo(ExpressionType nodeType, Type type)
			{
				this.NodeType = nodeType;
				this.Type = type;
			}

			internal readonly ExpressionType NodeType;

			internal readonly Type Type;
		}

		private enum TryGetFuncActionArgsResult
		{
			Valid,
			ArgumentNull,
			ByRef,
			PointerOrVoid
		}
	}
}
