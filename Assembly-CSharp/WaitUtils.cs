using System;
using System.Linq.Expressions;
using System.Reflection;
using UnityEngine;

public static class WaitUtils
{
	public static WaitForSeconds WaitForSeconds(float seconds)
	{
		WaitUtils._waitForSecondsSetter(seconds);
		return WaitUtils._waitForSeconds;
	}

	private static WaitForSeconds _waitForSeconds = new WaitForSeconds(1f);

	private static ParameterExpression _param = Expression.Parameter(typeof(float));

	private static Action<float> _waitForSecondsSetter = Expression.Lambda<Action<float>>(Expression.Assign(Expression.Field(Expression.Constant(WaitUtils._waitForSeconds, typeof(WaitForSeconds)), typeof(WaitForSeconds).GetField("m_Seconds", BindingFlags.Instance | BindingFlags.NonPublic)), WaitUtils._param), new ParameterExpression[]
	{
		WaitUtils._param
	}).Compile();
}
