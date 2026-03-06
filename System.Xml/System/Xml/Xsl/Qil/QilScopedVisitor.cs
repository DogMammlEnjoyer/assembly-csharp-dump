using System;
using System.Collections.Generic;

namespace System.Xml.Xsl.Qil
{
	internal class QilScopedVisitor : QilVisitor
	{
		protected virtual void BeginScope(QilNode node)
		{
		}

		protected virtual void EndScope(QilNode node)
		{
		}

		protected virtual void BeforeVisit(QilNode node)
		{
			QilNodeType nodeType = node.NodeType;
			if (nodeType != QilNodeType.QilExpression)
			{
				if (nodeType - QilNodeType.Loop <= 2)
				{
					goto IL_EF;
				}
				if (nodeType != QilNodeType.Function)
				{
					return;
				}
			}
			else
			{
				QilExpression qilExpression = (QilExpression)node;
				foreach (QilNode node2 in qilExpression.GlobalParameterList)
				{
					this.BeginScope(node2);
				}
				foreach (QilNode node3 in qilExpression.GlobalVariableList)
				{
					this.BeginScope(node3);
				}
				using (IEnumerator<QilNode> enumerator = qilExpression.FunctionList.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						QilNode node4 = enumerator.Current;
						this.BeginScope(node4);
					}
					return;
				}
			}
			using (IEnumerator<QilNode> enumerator = ((QilFunction)node).Arguments.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					QilNode node5 = enumerator.Current;
					this.BeginScope(node5);
				}
				return;
			}
			IL_EF:
			this.BeginScope(((QilLoop)node).Variable);
		}

		protected virtual void AfterVisit(QilNode node)
		{
			QilNodeType nodeType = node.NodeType;
			if (nodeType != QilNodeType.QilExpression)
			{
				if (nodeType - QilNodeType.Loop <= 2)
				{
					goto IL_EF;
				}
				if (nodeType != QilNodeType.Function)
				{
					return;
				}
			}
			else
			{
				QilExpression qilExpression = (QilExpression)node;
				foreach (QilNode node2 in qilExpression.FunctionList)
				{
					this.EndScope(node2);
				}
				foreach (QilNode node3 in qilExpression.GlobalVariableList)
				{
					this.EndScope(node3);
				}
				using (IEnumerator<QilNode> enumerator = qilExpression.GlobalParameterList.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						QilNode node4 = enumerator.Current;
						this.EndScope(node4);
					}
					return;
				}
			}
			using (IEnumerator<QilNode> enumerator = ((QilFunction)node).Arguments.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					QilNode node5 = enumerator.Current;
					this.EndScope(node5);
				}
				return;
			}
			IL_EF:
			this.EndScope(((QilLoop)node).Variable);
		}

		protected override QilNode Visit(QilNode n)
		{
			this.BeforeVisit(n);
			QilNode result = base.Visit(n);
			this.AfterVisit(n);
			return result;
		}
	}
}
