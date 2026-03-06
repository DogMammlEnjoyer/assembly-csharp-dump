using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Dynamic;
using System.Dynamic.Utils;
using System.Linq.Expressions;
using System.Linq.Expressions.Compiler;

namespace System.Runtime.CompilerServices
{
	/// <summary>Contains helper methods called from dynamically generated methods.</summary>
	[EditorBrowsable(EditorBrowsableState.Never)]
	[DebuggerStepThrough]
	public static class RuntimeOps
	{
		/// <summary>Gets the value of an item in an expando object.</summary>
		/// <param name="expando">The expando object.</param>
		/// <param name="indexClass">The class of the expando object.</param>
		/// <param name="index">The index of the member.</param>
		/// <param name="name">The name of the member.</param>
		/// <param name="ignoreCase">true if the name should be matched ignoring case; false otherwise.</param>
		/// <param name="value">The out parameter containing the value of the member.</param>
		/// <returns>True if the member exists in the expando object, otherwise false.</returns>
		[EditorBrowsable(EditorBrowsableState.Never)]
		[Obsolete("do not use this method", true)]
		public static bool ExpandoTryGetValue(ExpandoObject expando, object indexClass, int index, string name, bool ignoreCase, out object value)
		{
			return expando.TryGetValue(indexClass, index, name, ignoreCase, out value);
		}

		/// <summary>Sets the value of an item in an expando object.</summary>
		/// <param name="expando">The expando object.</param>
		/// <param name="indexClass">The class of the expando object.</param>
		/// <param name="index">The index of the member.</param>
		/// <param name="value">The value of the member.</param>
		/// <param name="name">The name of the member.</param>
		/// <param name="ignoreCase">true if the name should be matched ignoring case; false otherwise.</param>
		/// <returns>Returns the index for the set member.</returns>
		[EditorBrowsable(EditorBrowsableState.Never)]
		[Obsolete("do not use this method", true)]
		public static object ExpandoTrySetValue(ExpandoObject expando, object indexClass, int index, object value, string name, bool ignoreCase)
		{
			expando.TrySetValue(indexClass, index, value, name, ignoreCase, false);
			return value;
		}

		/// <summary>Deletes the value of an item in an expando object.</summary>
		/// <param name="expando">The expando object.</param>
		/// <param name="indexClass">The class of the expando object.</param>
		/// <param name="index">The index of the member.</param>
		/// <param name="name">The name of the member.</param>
		/// <param name="ignoreCase">true if the name should be matched ignoring case; false otherwise.</param>
		/// <returns>true if the item was successfully removed; otherwise, false.</returns>
		[EditorBrowsable(EditorBrowsableState.Never)]
		[Obsolete("do not use this method", true)]
		public static bool ExpandoTryDeleteValue(ExpandoObject expando, object indexClass, int index, string name, bool ignoreCase)
		{
			return expando.TryDeleteValue(indexClass, index, name, ignoreCase, ExpandoObject.Uninitialized);
		}

		/// <summary>Checks the version of the Expando object.</summary>
		/// <param name="expando">The Expando object.</param>
		/// <param name="version">The version to check.</param>
		/// <returns>Returns true if the version is equal; otherwise, false.</returns>
		[Obsolete("do not use this method", true)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		public static bool ExpandoCheckVersion(ExpandoObject expando, object version)
		{
			return expando.Class == version;
		}

		/// <summary>Promotes an Expando object from one class to a new class.</summary>
		/// <param name="expando">The Expando object.</param>
		/// <param name="oldClass">The old class of the Expando object.</param>
		/// <param name="newClass">The new class of the Expando object.</param>
		[Obsolete("do not use this method", true)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		public static void ExpandoPromoteClass(ExpandoObject expando, object oldClass, object newClass)
		{
			expando.PromoteClass(oldClass, newClass);
		}

		/// <summary>Quotes the provided expression tree.</summary>
		/// <param name="expression">The expression to quote.</param>
		/// <param name="hoistedLocals">The hoisted local state provided by the compiler.</param>
		/// <param name="locals">The actual hoisted local values.</param>
		/// <returns>The quoted expression.</returns>
		[EditorBrowsable(EditorBrowsableState.Never)]
		[Obsolete("do not use this method", true)]
		public static Expression Quote(Expression expression, object hoistedLocals, object[] locals)
		{
			return new RuntimeOps.ExpressionQuoter((HoistedLocals)hoistedLocals, locals).Visit(expression);
		}

		/// <summary>Combines two runtime variable lists and returns a new list.</summary>
		/// <param name="first">The first list.</param>
		/// <param name="second">The second list.</param>
		/// <param name="indexes">The index array indicating which list to get variables from.</param>
		/// <returns>The merged runtime variables.</returns>
		[EditorBrowsable(EditorBrowsableState.Never)]
		[Obsolete("do not use this method", true)]
		public static IRuntimeVariables MergeRuntimeVariables(IRuntimeVariables first, IRuntimeVariables second, int[] indexes)
		{
			return new RuntimeOps.MergedRuntimeVariables(first, second, indexes);
		}

		/// <summary>Creates an interface that can be used to modify closed over variables at runtime.</summary>
		/// <param name="data">The closure array.</param>
		/// <param name="indexes">An array of indicies into the closure array where variables are found.</param>
		/// <returns>An interface to access variables.</returns>
		[EditorBrowsable(EditorBrowsableState.Never)]
		[Obsolete("do not use this method", true)]
		public static IRuntimeVariables CreateRuntimeVariables(object[] data, long[] indexes)
		{
			return new RuntimeOps.RuntimeVariableList(data, indexes);
		}

		/// <summary>Creates an interface that can be used to modify closed over variables at runtime.</summary>
		/// <returns>An interface to access variables.</returns>
		[Obsolete("do not use this method", true)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		public static IRuntimeVariables CreateRuntimeVariables()
		{
			return new RuntimeOps.EmptyRuntimeVariables();
		}

		private sealed class ExpressionQuoter : ExpressionVisitor
		{
			internal ExpressionQuoter(HoistedLocals scope, object[] locals)
			{
				this._scope = scope;
				this._locals = locals;
			}

			protected internal override Expression VisitLambda<T>(Expression<T> node)
			{
				if (node.ParameterCount > 0)
				{
					HashSet<ParameterExpression> hashSet = new HashSet<ParameterExpression>();
					int i = 0;
					int parameterCount = node.ParameterCount;
					while (i < parameterCount)
					{
						hashSet.Add(node.GetParameter(i));
						i++;
					}
					this._shadowedVars.Push(hashSet);
				}
				Expression expression = this.Visit(node.Body);
				if (node.ParameterCount > 0)
				{
					this._shadowedVars.Pop();
				}
				if (expression == node.Body)
				{
					return node;
				}
				return node.Rewrite(expression, null);
			}

			protected internal override Expression VisitBlock(BlockExpression node)
			{
				if (node.Variables.Count > 0)
				{
					this._shadowedVars.Push(new HashSet<ParameterExpression>(node.Variables));
				}
				Expression[] array = ExpressionVisitorUtils.VisitBlockExpressions(this, node);
				if (node.Variables.Count > 0)
				{
					this._shadowedVars.Pop();
				}
				if (array == null)
				{
					return node;
				}
				return node.Rewrite(node.Variables, array);
			}

			protected override CatchBlock VisitCatchBlock(CatchBlock node)
			{
				if (node.Variable != null)
				{
					this._shadowedVars.Push(new HashSet<ParameterExpression>
					{
						node.Variable
					});
				}
				Expression expression = this.Visit(node.Body);
				Expression expression2 = this.Visit(node.Filter);
				if (node.Variable != null)
				{
					this._shadowedVars.Pop();
				}
				if (expression == node.Body && expression2 == node.Filter)
				{
					return node;
				}
				return Expression.MakeCatchBlock(node.Test, node.Variable, expression, expression2);
			}

			protected internal override Expression VisitRuntimeVariables(RuntimeVariablesExpression node)
			{
				int count = node.Variables.Count;
				List<IStrongBox> list = new List<IStrongBox>();
				List<ParameterExpression> list2 = new List<ParameterExpression>();
				int[] array = new int[count];
				for (int i = 0; i < array.Length; i++)
				{
					IStrongBox box = this.GetBox(node.Variables[i]);
					if (box == null)
					{
						array[i] = list2.Count;
						list2.Add(node.Variables[i]);
					}
					else
					{
						array[i] = -1 - list.Count;
						list.Add(box);
					}
				}
				if (list.Count == 0)
				{
					return node;
				}
				ConstantExpression constantExpression = Expression.Constant(new RuntimeOps.RuntimeVariables(list.ToArray()), typeof(IRuntimeVariables));
				if (list2.Count == 0)
				{
					return constantExpression;
				}
				return Expression.Call(CachedReflectionInfo.RuntimeOps_MergeRuntimeVariables, Expression.RuntimeVariables(new TrueReadOnlyCollection<ParameterExpression>(list2.ToArray())), constantExpression, Expression.Constant(array));
			}

			protected internal override Expression VisitParameter(ParameterExpression node)
			{
				IStrongBox box = this.GetBox(node);
				if (box == null)
				{
					return node;
				}
				return Expression.Field(Expression.Constant(box), "Value");
			}

			private IStrongBox GetBox(ParameterExpression variable)
			{
				using (Stack<HashSet<ParameterExpression>>.Enumerator enumerator = this._shadowedVars.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						if (enumerator.Current.Contains(variable))
						{
							return null;
						}
					}
				}
				HoistedLocals hoistedLocals = this._scope;
				object[] array = this._locals;
				int num;
				while (!hoistedLocals.Indexes.TryGetValue(variable, out num))
				{
					hoistedLocals = hoistedLocals.Parent;
					if (hoistedLocals == null)
					{
						throw ContractUtils.Unreachable;
					}
					array = HoistedLocals.GetParent(array);
				}
				return (IStrongBox)array[num];
			}

			private readonly HoistedLocals _scope;

			private readonly object[] _locals;

			private readonly Stack<HashSet<ParameterExpression>> _shadowedVars = new Stack<HashSet<ParameterExpression>>();
		}

		internal sealed class MergedRuntimeVariables : IRuntimeVariables
		{
			internal MergedRuntimeVariables(IRuntimeVariables first, IRuntimeVariables second, int[] indexes)
			{
				this._first = first;
				this._second = second;
				this._indexes = indexes;
			}

			public int Count
			{
				get
				{
					return this._indexes.Length;
				}
			}

			public object this[int index]
			{
				get
				{
					index = this._indexes[index];
					if (index < 0)
					{
						return this._second[-1 - index];
					}
					return this._first[index];
				}
				set
				{
					index = this._indexes[index];
					if (index >= 0)
					{
						this._first[index] = value;
						return;
					}
					this._second[-1 - index] = value;
				}
			}

			private readonly IRuntimeVariables _first;

			private readonly IRuntimeVariables _second;

			private readonly int[] _indexes;
		}

		private sealed class EmptyRuntimeVariables : IRuntimeVariables
		{
			int IRuntimeVariables.Count
			{
				get
				{
					return 0;
				}
			}

			object IRuntimeVariables.this[int index]
			{
				get
				{
					throw new IndexOutOfRangeException();
				}
				set
				{
					throw new IndexOutOfRangeException();
				}
			}
		}

		private sealed class RuntimeVariableList : IRuntimeVariables
		{
			internal RuntimeVariableList(object[] data, long[] indexes)
			{
				this._data = data;
				this._indexes = indexes;
			}

			public int Count
			{
				get
				{
					return this._indexes.Length;
				}
			}

			public object this[int index]
			{
				get
				{
					return this.GetStrongBox(index).Value;
				}
				set
				{
					this.GetStrongBox(index).Value = value;
				}
			}

			private IStrongBox GetStrongBox(int index)
			{
				long num = this._indexes[index];
				object[] array = this._data;
				for (int i = (int)(num >> 32); i > 0; i--)
				{
					array = HoistedLocals.GetParent(array);
				}
				return (IStrongBox)array[(int)num];
			}

			private readonly object[] _data;

			private readonly long[] _indexes;
		}

		internal sealed class RuntimeVariables : IRuntimeVariables
		{
			internal RuntimeVariables(IStrongBox[] boxes)
			{
				this._boxes = boxes;
			}

			int IRuntimeVariables.Count
			{
				get
				{
					return this._boxes.Length;
				}
			}

			object IRuntimeVariables.this[int index]
			{
				get
				{
					return this._boxes[index].Value;
				}
				set
				{
					this._boxes[index].Value = value;
				}
			}

			private readonly IStrongBox[] _boxes;
		}
	}
}
