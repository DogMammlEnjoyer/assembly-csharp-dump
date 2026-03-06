using System;
using System.Collections.Generic;
using System.Xml.XmlConfiguration;
using System.Xml.XPath;
using System.Xml.Xsl.Qil;
using System.Xml.Xsl.XPath;

namespace System.Xml.Xsl.Xslt
{
	internal class XPathPatternParser
	{
		public QilNode Parse(XPathScanner scanner, XPathPatternParser.IPatternBuilder ptrnBuilder)
		{
			QilNode result = null;
			ptrnBuilder.StartBuild();
			try
			{
				this.scanner = scanner;
				this.ptrnBuilder = ptrnBuilder;
				result = this.ParsePattern();
				this.scanner.CheckToken(LexKind.Eof);
			}
			finally
			{
				result = ptrnBuilder.EndBuild(result);
			}
			return result;
		}

		private QilNode ParsePattern()
		{
			QilNode qilNode = this.ParseLocationPathPattern();
			while (this.scanner.Kind == LexKind.Union)
			{
				this.scanner.NextLex();
				qilNode = this.ptrnBuilder.Operator(XPathOperator.Union, qilNode, this.ParseLocationPathPattern());
			}
			return qilNode;
		}

		private QilNode ParseLocationPathPattern()
		{
			LexKind kind = this.scanner.Kind;
			if (kind != LexKind.SlashSlash)
			{
				if (kind != LexKind.Name)
				{
					if (kind == LexKind.Slash)
					{
						this.scanner.NextLex();
						QilNode qilNode = this.ptrnBuilder.Axis(XPathAxis.Root, XPathNodeType.All, null, null);
						if (XPathParser<QilNode>.IsStep(this.scanner.Kind))
						{
							qilNode = this.ptrnBuilder.JoinStep(qilNode, this.ParseRelativePathPattern());
						}
						return qilNode;
					}
				}
				else if (this.scanner.CanBeFunction && this.scanner.Prefix.Length == 0 && (this.scanner.Name == "id" || this.scanner.Name == "key"))
				{
					QilNode qilNode = this.ParseIdKeyPattern();
					LexKind kind2 = this.scanner.Kind;
					if (kind2 != LexKind.SlashSlash)
					{
						if (kind2 == LexKind.Slash)
						{
							this.scanner.NextLex();
							qilNode = this.ptrnBuilder.JoinStep(qilNode, this.ParseRelativePathPattern());
						}
					}
					else
					{
						this.scanner.NextLex();
						qilNode = this.ptrnBuilder.JoinStep(qilNode, this.ptrnBuilder.JoinStep(this.ptrnBuilder.Axis(XPathAxis.DescendantOrSelf, XPathNodeType.All, null, null), this.ParseRelativePathPattern()));
					}
					return qilNode;
				}
				return this.ParseRelativePathPattern();
			}
			this.scanner.NextLex();
			return this.ptrnBuilder.JoinStep(this.ptrnBuilder.Axis(XPathAxis.Root, XPathNodeType.All, null, null), this.ptrnBuilder.JoinStep(this.ptrnBuilder.Axis(XPathAxis.DescendantOrSelf, XPathNodeType.All, null, null), this.ParseRelativePathPattern()));
		}

		private QilNode ParseIdKeyPattern()
		{
			List<QilNode> list = new List<QilNode>(2);
			if (this.scanner.Name == "id")
			{
				this.scanner.NextLex();
				this.scanner.PassToken(LexKind.LParens);
				this.scanner.CheckToken(LexKind.String);
				list.Add(this.ptrnBuilder.String(this.scanner.StringValue));
				this.scanner.NextLex();
				this.scanner.PassToken(LexKind.RParens);
				return this.ptrnBuilder.Function("", "id", list);
			}
			this.scanner.NextLex();
			this.scanner.PassToken(LexKind.LParens);
			this.scanner.CheckToken(LexKind.String);
			list.Add(this.ptrnBuilder.String(this.scanner.StringValue));
			this.scanner.NextLex();
			this.scanner.PassToken(LexKind.Comma);
			this.scanner.CheckToken(LexKind.String);
			list.Add(this.ptrnBuilder.String(this.scanner.StringValue));
			this.scanner.NextLex();
			this.scanner.PassToken(LexKind.RParens);
			return this.ptrnBuilder.Function("", "key", list);
		}

		private QilNode ParseRelativePathPattern()
		{
			int num = this.parseRelativePath + 1;
			this.parseRelativePath = num;
			if (num > 1024 && XsltConfigSection.LimitXPathComplexity)
			{
				throw this.scanner.CreateException("The stylesheet is too complex.", Array.Empty<string>());
			}
			QilNode qilNode = this.ParseStepPattern();
			if (this.scanner.Kind == LexKind.Slash)
			{
				this.scanner.NextLex();
				qilNode = this.ptrnBuilder.JoinStep(qilNode, this.ParseRelativePathPattern());
			}
			else if (this.scanner.Kind == LexKind.SlashSlash)
			{
				this.scanner.NextLex();
				qilNode = this.ptrnBuilder.JoinStep(qilNode, this.ptrnBuilder.JoinStep(this.ptrnBuilder.Axis(XPathAxis.DescendantOrSelf, XPathNodeType.All, null, null), this.ParseRelativePathPattern()));
			}
			this.parseRelativePath--;
			return qilNode;
		}

		private QilNode ParseStepPattern()
		{
			LexKind kind = this.scanner.Kind;
			XPathAxis xpathAxis;
			if (kind <= LexKind.Name)
			{
				if (kind != LexKind.DotDot)
				{
					if (kind != LexKind.Axis)
					{
						if (kind != LexKind.Name)
						{
							goto IL_A6;
						}
						goto IL_A2;
					}
					else
					{
						xpathAxis = this.scanner.Axis;
						if (xpathAxis != XPathAxis.Child && xpathAxis != XPathAxis.Attribute)
						{
							throw this.scanner.CreateException("Only 'child' and 'attribute' axes are allowed in a pattern outside predicates.", Array.Empty<string>());
						}
						this.scanner.NextLex();
						this.scanner.NextLex();
						goto IL_CB;
					}
				}
			}
			else
			{
				if (kind == LexKind.Star)
				{
					goto IL_A2;
				}
				if (kind != LexKind.Dot)
				{
					if (kind != LexKind.At)
					{
						goto IL_A6;
					}
					xpathAxis = XPathAxis.Attribute;
					this.scanner.NextLex();
					goto IL_CB;
				}
			}
			throw this.scanner.CreateException("Only 'child' and 'attribute' axes are allowed in a pattern outside predicates.", Array.Empty<string>());
			IL_A2:
			xpathAxis = XPathAxis.Child;
			goto IL_CB;
			IL_A6:
			throw this.scanner.CreateException("Unexpected token '{0}' in the expression.", new string[]
			{
				this.scanner.RawValue
			});
			IL_CB:
			XPathNodeType nodeType;
			string prefix;
			string name;
			XPathParser<QilNode>.InternalParseNodeTest(this.scanner, xpathAxis, out nodeType, out prefix, out name);
			QilNode qilNode = this.ptrnBuilder.Axis(xpathAxis, nodeType, prefix, name);
			XPathPatternBuilder xpathPatternBuilder = this.ptrnBuilder as XPathPatternBuilder;
			if (xpathPatternBuilder != null)
			{
				List<QilNode> list = new List<QilNode>();
				while (this.scanner.Kind == LexKind.LBracket)
				{
					list.Add(this.ParsePredicate(qilNode));
				}
				if (list.Count > 0)
				{
					qilNode = xpathPatternBuilder.BuildPredicates(qilNode, list);
				}
			}
			else
			{
				while (this.scanner.Kind == LexKind.LBracket)
				{
					qilNode = this.ptrnBuilder.Predicate(qilNode, this.ParsePredicate(qilNode), false);
				}
			}
			return qilNode;
		}

		private QilNode ParsePredicate(QilNode context)
		{
			this.scanner.NextLex();
			QilNode result = this.predicateParser.Parse(this.scanner, this.ptrnBuilder.GetPredicateBuilder(context), LexKind.RBracket);
			this.scanner.NextLex();
			return result;
		}

		private XPathScanner scanner;

		private XPathPatternParser.IPatternBuilder ptrnBuilder;

		private XPathParser<QilNode> predicateParser = new XPathParser<QilNode>();

		private const int MaxParseRelativePathDepth = 1024;

		private int parseRelativePath;

		public interface IPatternBuilder : IXPathBuilder<QilNode>
		{
			IXPathBuilder<QilNode> GetPredicateBuilder(QilNode context);
		}
	}
}
