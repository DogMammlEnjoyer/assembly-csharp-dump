using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;

namespace System.Linq.Expressions
{
	internal sealed class ExpressionStringBuilder : ExpressionVisitor
	{
		private ExpressionStringBuilder()
		{
			this._out = new StringBuilder();
		}

		public override string ToString()
		{
			return this._out.ToString();
		}

		private int GetLabelId(LabelTarget label)
		{
			return this.GetId(label);
		}

		private int GetParamId(ParameterExpression p)
		{
			return this.GetId(p);
		}

		private int GetId(object o)
		{
			if (this._ids == null)
			{
				this._ids = new Dictionary<object, int>();
			}
			int count;
			if (!this._ids.TryGetValue(o, out count))
			{
				count = this._ids.Count;
				this._ids.Add(o, count);
			}
			return count;
		}

		private void Out(string s)
		{
			this._out.Append(s);
		}

		private void Out(char c)
		{
			this._out.Append(c);
		}

		internal static string ExpressionToString(Expression node)
		{
			ExpressionStringBuilder expressionStringBuilder = new ExpressionStringBuilder();
			expressionStringBuilder.Visit(node);
			return expressionStringBuilder.ToString();
		}

		internal static string CatchBlockToString(CatchBlock node)
		{
			ExpressionStringBuilder expressionStringBuilder = new ExpressionStringBuilder();
			expressionStringBuilder.VisitCatchBlock(node);
			return expressionStringBuilder.ToString();
		}

		internal static string SwitchCaseToString(SwitchCase node)
		{
			ExpressionStringBuilder expressionStringBuilder = new ExpressionStringBuilder();
			expressionStringBuilder.VisitSwitchCase(node);
			return expressionStringBuilder.ToString();
		}

		internal static string MemberBindingToString(MemberBinding node)
		{
			ExpressionStringBuilder expressionStringBuilder = new ExpressionStringBuilder();
			expressionStringBuilder.VisitMemberBinding(node);
			return expressionStringBuilder.ToString();
		}

		internal static string ElementInitBindingToString(ElementInit node)
		{
			ExpressionStringBuilder expressionStringBuilder = new ExpressionStringBuilder();
			expressionStringBuilder.VisitElementInit(node);
			return expressionStringBuilder.ToString();
		}

		private void VisitExpressions<T>(char open, ReadOnlyCollection<T> expressions, char close) where T : Expression
		{
			this.VisitExpressions<T>(open, expressions, close, ", ");
		}

		private void VisitExpressions<T>(char open, ReadOnlyCollection<T> expressions, char close, string seperator) where T : Expression
		{
			this.Out(open);
			if (expressions != null)
			{
				bool flag = true;
				foreach (T t in expressions)
				{
					if (flag)
					{
						flag = false;
					}
					else
					{
						this.Out(seperator);
					}
					this.Visit(t);
				}
			}
			this.Out(close);
		}

		protected internal override Expression VisitBinary(BinaryExpression node)
		{
			if (node.NodeType == ExpressionType.ArrayIndex)
			{
				this.Visit(node.Left);
				this.Out('[');
				this.Visit(node.Right);
				this.Out(']');
			}
			else
			{
				ExpressionType nodeType = node.NodeType;
				string s;
				switch (nodeType)
				{
				case ExpressionType.Add:
				case ExpressionType.AddChecked:
					s = "+";
					goto IL_2CE;
				case ExpressionType.And:
					s = (ExpressionStringBuilder.IsBool(node) ? "And" : "&");
					goto IL_2CE;
				case ExpressionType.AndAlso:
					s = "AndAlso";
					goto IL_2CE;
				case ExpressionType.ArrayLength:
				case ExpressionType.ArrayIndex:
				case ExpressionType.Call:
				case ExpressionType.Conditional:
				case ExpressionType.Constant:
				case ExpressionType.Convert:
				case ExpressionType.ConvertChecked:
				case ExpressionType.Invoke:
				case ExpressionType.Lambda:
				case ExpressionType.ListInit:
				case ExpressionType.MemberAccess:
				case ExpressionType.MemberInit:
				case ExpressionType.Negate:
				case ExpressionType.UnaryPlus:
				case ExpressionType.NegateChecked:
				case ExpressionType.New:
				case ExpressionType.NewArrayInit:
				case ExpressionType.NewArrayBounds:
				case ExpressionType.Not:
				case ExpressionType.Parameter:
				case ExpressionType.Quote:
				case ExpressionType.TypeAs:
				case ExpressionType.TypeIs:
					break;
				case ExpressionType.Coalesce:
					s = "??";
					goto IL_2CE;
				case ExpressionType.Divide:
					s = "/";
					goto IL_2CE;
				case ExpressionType.Equal:
					s = "==";
					goto IL_2CE;
				case ExpressionType.ExclusiveOr:
					s = "^";
					goto IL_2CE;
				case ExpressionType.GreaterThan:
					s = ">";
					goto IL_2CE;
				case ExpressionType.GreaterThanOrEqual:
					s = ">=";
					goto IL_2CE;
				case ExpressionType.LeftShift:
					s = "<<";
					goto IL_2CE;
				case ExpressionType.LessThan:
					s = "<";
					goto IL_2CE;
				case ExpressionType.LessThanOrEqual:
					s = "<=";
					goto IL_2CE;
				case ExpressionType.Modulo:
					s = "%";
					goto IL_2CE;
				case ExpressionType.Multiply:
				case ExpressionType.MultiplyChecked:
					s = "*";
					goto IL_2CE;
				case ExpressionType.NotEqual:
					s = "!=";
					goto IL_2CE;
				case ExpressionType.Or:
					s = (ExpressionStringBuilder.IsBool(node) ? "Or" : "|");
					goto IL_2CE;
				case ExpressionType.OrElse:
					s = "OrElse";
					goto IL_2CE;
				case ExpressionType.Power:
					s = "**";
					goto IL_2CE;
				case ExpressionType.RightShift:
					s = ">>";
					goto IL_2CE;
				case ExpressionType.Subtract:
				case ExpressionType.SubtractChecked:
					s = "-";
					goto IL_2CE;
				case ExpressionType.Assign:
					s = "=";
					goto IL_2CE;
				default:
					switch (nodeType)
					{
					case ExpressionType.AddAssign:
					case ExpressionType.AddAssignChecked:
						s = "+=";
						goto IL_2CE;
					case ExpressionType.AndAssign:
						s = (ExpressionStringBuilder.IsBool(node) ? "&&=" : "&=");
						goto IL_2CE;
					case ExpressionType.DivideAssign:
						s = "/=";
						goto IL_2CE;
					case ExpressionType.ExclusiveOrAssign:
						s = "^=";
						goto IL_2CE;
					case ExpressionType.LeftShiftAssign:
						s = "<<=";
						goto IL_2CE;
					case ExpressionType.ModuloAssign:
						s = "%=";
						goto IL_2CE;
					case ExpressionType.MultiplyAssign:
					case ExpressionType.MultiplyAssignChecked:
						s = "*=";
						goto IL_2CE;
					case ExpressionType.OrAssign:
						s = (ExpressionStringBuilder.IsBool(node) ? "||=" : "|=");
						goto IL_2CE;
					case ExpressionType.PowerAssign:
						s = "**=";
						goto IL_2CE;
					case ExpressionType.RightShiftAssign:
						s = ">>=";
						goto IL_2CE;
					case ExpressionType.SubtractAssign:
					case ExpressionType.SubtractAssignChecked:
						s = "-=";
						goto IL_2CE;
					}
					break;
				}
				throw new InvalidOperationException();
				IL_2CE:
				this.Out('(');
				this.Visit(node.Left);
				this.Out(' ');
				this.Out(s);
				this.Out(' ');
				this.Visit(node.Right);
				this.Out(')');
			}
			return node;
		}

		protected internal override Expression VisitParameter(ParameterExpression node)
		{
			if (node.IsByRef)
			{
				this.Out("ref ");
			}
			string name = node.Name;
			if (string.IsNullOrEmpty(name))
			{
				this.Out("Param_" + this.GetParamId(node).ToString());
			}
			else
			{
				this.Out(name);
			}
			return node;
		}

		protected internal override Expression VisitLambda<T>(Expression<T> node)
		{
			if (node.ParameterCount == 1)
			{
				this.Visit(node.GetParameter(0));
			}
			else
			{
				this.Out('(');
				string s = ", ";
				int i = 0;
				int parameterCount = node.ParameterCount;
				while (i < parameterCount)
				{
					if (i > 0)
					{
						this.Out(s);
					}
					this.Visit(node.GetParameter(i));
					i++;
				}
				this.Out(')');
			}
			this.Out(" => ");
			this.Visit(node.Body);
			return node;
		}

		protected internal override Expression VisitListInit(ListInitExpression node)
		{
			this.Visit(node.NewExpression);
			this.Out(" {");
			int i = 0;
			int count = node.Initializers.Count;
			while (i < count)
			{
				if (i > 0)
				{
					this.Out(", ");
				}
				this.VisitElementInit(node.Initializers[i]);
				i++;
			}
			this.Out('}');
			return node;
		}

		protected internal override Expression VisitConditional(ConditionalExpression node)
		{
			this.Out("IIF(");
			this.Visit(node.Test);
			this.Out(", ");
			this.Visit(node.IfTrue);
			this.Out(", ");
			this.Visit(node.IfFalse);
			this.Out(')');
			return node;
		}

		protected internal override Expression VisitConstant(ConstantExpression node)
		{
			if (node.Value != null)
			{
				string text = node.Value.ToString();
				if (node.Value is string)
				{
					this.Out('"');
					this.Out(text);
					this.Out('"');
				}
				else if (text == node.Value.GetType().ToString())
				{
					this.Out("value(");
					this.Out(text);
					this.Out(')');
				}
				else
				{
					this.Out(text);
				}
			}
			else
			{
				this.Out("null");
			}
			return node;
		}

		protected internal override Expression VisitDebugInfo(DebugInfoExpression node)
		{
			string s = string.Format(CultureInfo.CurrentCulture, "<DebugInfo({0}: {1}, {2}, {3}, {4})>", new object[]
			{
				node.Document.FileName,
				node.StartLine,
				node.StartColumn,
				node.EndLine,
				node.EndColumn
			});
			this.Out(s);
			return node;
		}

		protected internal override Expression VisitRuntimeVariables(RuntimeVariablesExpression node)
		{
			this.VisitExpressions<ParameterExpression>('(', node.Variables, ')');
			return node;
		}

		private void OutMember(Expression instance, MemberInfo member)
		{
			if (instance != null)
			{
				this.Visit(instance);
			}
			else
			{
				this.Out(member.DeclaringType.Name);
			}
			this.Out('.');
			this.Out(member.Name);
		}

		protected internal override Expression VisitMember(MemberExpression node)
		{
			this.OutMember(node.Expression, node.Member);
			return node;
		}

		protected internal override Expression VisitMemberInit(MemberInitExpression node)
		{
			if (node.NewExpression.ArgumentCount == 0 && node.NewExpression.Type.Name.Contains("<"))
			{
				this.Out("new");
			}
			else
			{
				this.Visit(node.NewExpression);
			}
			this.Out(" {");
			int i = 0;
			int count = node.Bindings.Count;
			while (i < count)
			{
				MemberBinding node2 = node.Bindings[i];
				if (i > 0)
				{
					this.Out(", ");
				}
				this.VisitMemberBinding(node2);
				i++;
			}
			this.Out('}');
			return node;
		}

		protected override MemberAssignment VisitMemberAssignment(MemberAssignment assignment)
		{
			this.Out(assignment.Member.Name);
			this.Out(" = ");
			this.Visit(assignment.Expression);
			return assignment;
		}

		protected override MemberListBinding VisitMemberListBinding(MemberListBinding binding)
		{
			this.Out(binding.Member.Name);
			this.Out(" = {");
			int i = 0;
			int count = binding.Initializers.Count;
			while (i < count)
			{
				if (i > 0)
				{
					this.Out(", ");
				}
				this.VisitElementInit(binding.Initializers[i]);
				i++;
			}
			this.Out('}');
			return binding;
		}

		protected override MemberMemberBinding VisitMemberMemberBinding(MemberMemberBinding binding)
		{
			this.Out(binding.Member.Name);
			this.Out(" = {");
			int i = 0;
			int count = binding.Bindings.Count;
			while (i < count)
			{
				if (i > 0)
				{
					this.Out(", ");
				}
				this.VisitMemberBinding(binding.Bindings[i]);
				i++;
			}
			this.Out('}');
			return binding;
		}

		protected override ElementInit VisitElementInit(ElementInit initializer)
		{
			this.Out(initializer.AddMethod.ToString());
			string s = ", ";
			this.Out('(');
			int i = 0;
			int argumentCount = initializer.ArgumentCount;
			while (i < argumentCount)
			{
				if (i > 0)
				{
					this.Out(s);
				}
				this.Visit(initializer.GetArgument(i));
				i++;
			}
			this.Out(')');
			return initializer;
		}

		protected internal override Expression VisitInvocation(InvocationExpression node)
		{
			this.Out("Invoke(");
			this.Visit(node.Expression);
			string s = ", ";
			int i = 0;
			int argumentCount = node.ArgumentCount;
			while (i < argumentCount)
			{
				this.Out(s);
				this.Visit(node.GetArgument(i));
				i++;
			}
			this.Out(')');
			return node;
		}

		protected internal override Expression VisitMethodCall(MethodCallExpression node)
		{
			int num = 0;
			Expression expression = node.Object;
			if (node.Method.GetCustomAttribute(typeof(ExtensionAttribute)) != null)
			{
				num = 1;
				expression = node.GetArgument(0);
			}
			if (expression != null)
			{
				this.Visit(expression);
				this.Out('.');
			}
			this.Out(node.Method.Name);
			this.Out('(');
			int i = num;
			int argumentCount = node.ArgumentCount;
			while (i < argumentCount)
			{
				if (i > num)
				{
					this.Out(", ");
				}
				this.Visit(node.GetArgument(i));
				i++;
			}
			this.Out(')');
			return node;
		}

		protected internal override Expression VisitNewArray(NewArrayExpression node)
		{
			ExpressionType nodeType = node.NodeType;
			if (nodeType != ExpressionType.NewArrayInit)
			{
				if (nodeType == ExpressionType.NewArrayBounds)
				{
					this.Out("new ");
					this.Out(node.Type.ToString());
					this.VisitExpressions<Expression>('(', node.Expressions, ')');
				}
			}
			else
			{
				this.Out("new [] ");
				this.VisitExpressions<Expression>('{', node.Expressions, '}');
			}
			return node;
		}

		protected internal override Expression VisitNew(NewExpression node)
		{
			this.Out("new ");
			this.Out(node.Type.Name);
			this.Out('(');
			ReadOnlyCollection<MemberInfo> members = node.Members;
			for (int i = 0; i < node.ArgumentCount; i++)
			{
				if (i > 0)
				{
					this.Out(", ");
				}
				if (members != null)
				{
					string name = members[i].Name;
					this.Out(name);
					this.Out(" = ");
				}
				this.Visit(node.GetArgument(i));
			}
			this.Out(')');
			return node;
		}

		protected internal override Expression VisitTypeBinary(TypeBinaryExpression node)
		{
			this.Out('(');
			this.Visit(node.Expression);
			ExpressionType nodeType = node.NodeType;
			if (nodeType != ExpressionType.TypeIs)
			{
				if (nodeType == ExpressionType.TypeEqual)
				{
					this.Out(" TypeEqual ");
				}
			}
			else
			{
				this.Out(" Is ");
			}
			this.Out(node.TypeOperand.Name);
			this.Out(')');
			return node;
		}

		protected internal override Expression VisitUnary(UnaryExpression node)
		{
			ExpressionType nodeType = node.NodeType;
			if (nodeType <= ExpressionType.Quote)
			{
				if (nodeType <= ExpressionType.Convert)
				{
					if (nodeType == ExpressionType.ArrayLength)
					{
						this.Out("ArrayLength(");
						goto IL_19E;
					}
					if (nodeType == ExpressionType.Convert)
					{
						this.Out("Convert(");
						goto IL_19E;
					}
				}
				else
				{
					if (nodeType == ExpressionType.ConvertChecked)
					{
						this.Out("ConvertChecked(");
						goto IL_19E;
					}
					switch (nodeType)
					{
					case ExpressionType.Negate:
					case ExpressionType.NegateChecked:
						this.Out('-');
						goto IL_19E;
					case ExpressionType.UnaryPlus:
						this.Out('+');
						goto IL_19E;
					case ExpressionType.New:
					case ExpressionType.NewArrayInit:
					case ExpressionType.NewArrayBounds:
						break;
					case ExpressionType.Not:
						this.Out("Not(");
						goto IL_19E;
					default:
						if (nodeType == ExpressionType.Quote)
						{
							goto IL_19E;
						}
						break;
					}
				}
			}
			else if (nodeType <= ExpressionType.Increment)
			{
				if (nodeType == ExpressionType.TypeAs)
				{
					this.Out('(');
					goto IL_19E;
				}
				if (nodeType == ExpressionType.Decrement)
				{
					this.Out("Decrement(");
					goto IL_19E;
				}
				if (nodeType == ExpressionType.Increment)
				{
					this.Out("Increment(");
					goto IL_19E;
				}
			}
			else
			{
				if (nodeType == ExpressionType.Throw)
				{
					this.Out("throw(");
					goto IL_19E;
				}
				if (nodeType == ExpressionType.Unbox)
				{
					this.Out("Unbox(");
					goto IL_19E;
				}
				switch (nodeType)
				{
				case ExpressionType.PreIncrementAssign:
					this.Out("++");
					goto IL_19E;
				case ExpressionType.PreDecrementAssign:
					this.Out("--");
					goto IL_19E;
				case ExpressionType.PostIncrementAssign:
				case ExpressionType.PostDecrementAssign:
					goto IL_19E;
				case ExpressionType.OnesComplement:
					this.Out("~(");
					goto IL_19E;
				case ExpressionType.IsTrue:
					this.Out("IsTrue(");
					goto IL_19E;
				case ExpressionType.IsFalse:
					this.Out("IsFalse(");
					goto IL_19E;
				}
			}
			throw new InvalidOperationException();
			IL_19E:
			this.Visit(node.Operand);
			nodeType = node.NodeType;
			if (nodeType <= ExpressionType.NegateChecked)
			{
				if (nodeType - ExpressionType.Convert <= 1)
				{
					this.Out(", ");
					this.Out(node.Type.Name);
					this.Out(')');
					return node;
				}
				if (nodeType - ExpressionType.Negate <= 2)
				{
					return node;
				}
			}
			else
			{
				if (nodeType == ExpressionType.Quote)
				{
					return node;
				}
				if (nodeType == ExpressionType.TypeAs)
				{
					this.Out(" As ");
					this.Out(node.Type.Name);
					this.Out(')');
					return node;
				}
				switch (nodeType)
				{
				case ExpressionType.PreIncrementAssign:
				case ExpressionType.PreDecrementAssign:
					return node;
				case ExpressionType.PostIncrementAssign:
					this.Out("++");
					return node;
				case ExpressionType.PostDecrementAssign:
					this.Out("--");
					return node;
				}
			}
			this.Out(')');
			return node;
		}

		protected internal override Expression VisitBlock(BlockExpression node)
		{
			this.Out('{');
			foreach (ParameterExpression node2 in node.Variables)
			{
				this.Out("var ");
				this.Visit(node2);
				this.Out(';');
			}
			this.Out(" ... }");
			return node;
		}

		protected internal override Expression VisitDefault(DefaultExpression node)
		{
			this.Out("default(");
			this.Out(node.Type.Name);
			this.Out(')');
			return node;
		}

		protected internal override Expression VisitLabel(LabelExpression node)
		{
			this.Out("{ ... } ");
			this.DumpLabel(node.Target);
			this.Out(':');
			return node;
		}

		protected internal override Expression VisitGoto(GotoExpression node)
		{
			string s;
			switch (node.Kind)
			{
			case GotoExpressionKind.Goto:
				s = "goto";
				break;
			case GotoExpressionKind.Return:
				s = "return";
				break;
			case GotoExpressionKind.Break:
				s = "break";
				break;
			case GotoExpressionKind.Continue:
				s = "continue";
				break;
			default:
				throw new InvalidOperationException();
			}
			this.Out(s);
			this.Out(' ');
			this.DumpLabel(node.Target);
			if (node.Value != null)
			{
				this.Out(" (");
				this.Visit(node.Value);
				this.Out(")");
			}
			return node;
		}

		protected internal override Expression VisitLoop(LoopExpression node)
		{
			this.Out("loop { ... }");
			return node;
		}

		protected override SwitchCase VisitSwitchCase(SwitchCase node)
		{
			this.Out("case ");
			this.VisitExpressions<Expression>('(', node.TestValues, ')');
			this.Out(": ...");
			return node;
		}

		protected internal override Expression VisitSwitch(SwitchExpression node)
		{
			this.Out("switch ");
			this.Out('(');
			this.Visit(node.SwitchValue);
			this.Out(") { ... }");
			return node;
		}

		protected override CatchBlock VisitCatchBlock(CatchBlock node)
		{
			this.Out("catch (");
			this.Out(node.Test.Name);
			ParameterExpression variable = node.Variable;
			if (!string.IsNullOrEmpty((variable != null) ? variable.Name : null))
			{
				this.Out(' ');
				this.Out(node.Variable.Name);
			}
			this.Out(") { ... }");
			return node;
		}

		protected internal override Expression VisitTry(TryExpression node)
		{
			this.Out("try { ... }");
			return node;
		}

		protected internal override Expression VisitIndex(IndexExpression node)
		{
			if (node.Object != null)
			{
				this.Visit(node.Object);
			}
			else
			{
				this.Out(node.Indexer.DeclaringType.Name);
			}
			if (node.Indexer != null)
			{
				this.Out('.');
				this.Out(node.Indexer.Name);
			}
			this.Out('[');
			int i = 0;
			int argumentCount = node.ArgumentCount;
			while (i < argumentCount)
			{
				if (i > 0)
				{
					this.Out(", ");
				}
				this.Visit(node.GetArgument(i));
				i++;
			}
			this.Out(']');
			return node;
		}

		protected internal override Expression VisitExtension(Expression node)
		{
			MethodInfo method = node.GetType().GetMethod("ToString", Type.EmptyTypes);
			if (method.DeclaringType != typeof(Expression) && !method.IsStatic)
			{
				this.Out(node.ToString());
				return node;
			}
			this.Out('[');
			this.Out((node.NodeType == ExpressionType.Extension) ? node.GetType().FullName : node.NodeType.ToString());
			this.Out(']');
			return node;
		}

		private void DumpLabel(LabelTarget target)
		{
			if (!string.IsNullOrEmpty(target.Name))
			{
				this.Out(target.Name);
				return;
			}
			this.Out("UnnamedLabel_" + this.GetLabelId(target).ToString());
		}

		private static bool IsBool(Expression node)
		{
			return node.Type == typeof(bool) || node.Type == typeof(bool?);
		}

		private readonly StringBuilder _out;

		private Dictionary<object, int> _ids;
	}
}
