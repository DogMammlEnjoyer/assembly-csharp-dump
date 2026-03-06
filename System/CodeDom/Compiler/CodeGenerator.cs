using System;
using System.Collections;
using System.Globalization;
using System.IO;
using System.Reflection;

namespace System.CodeDom.Compiler
{
	/// <summary>Provides an example implementation of the <see cref="T:System.CodeDom.Compiler.ICodeGenerator" /> interface. This class is abstract.</summary>
	public abstract class CodeGenerator : ICodeGenerator
	{
		/// <summary>Gets the code type declaration for the current class.</summary>
		/// <returns>The code type declaration for the current class.</returns>
		protected CodeTypeDeclaration CurrentClass
		{
			get
			{
				return this._currentClass;
			}
		}

		/// <summary>Gets the current class name.</summary>
		/// <returns>The current class name.</returns>
		protected string CurrentTypeName
		{
			get
			{
				if (this._currentClass == null)
				{
					return "<% unknown %>";
				}
				return this._currentClass.Name;
			}
		}

		/// <summary>Gets the current member of the class.</summary>
		/// <returns>The current member of the class.</returns>
		protected CodeTypeMember CurrentMember
		{
			get
			{
				return this._currentMember;
			}
		}

		/// <summary>Gets the current member name.</summary>
		/// <returns>The name of the current member.</returns>
		protected string CurrentMemberName
		{
			get
			{
				if (this._currentMember == null)
				{
					return "<% unknown %>";
				}
				return this._currentMember.Name;
			}
		}

		/// <summary>Gets a value indicating whether the current object being generated is an interface.</summary>
		/// <returns>
		///   <see langword="true" /> if the current object is an interface; otherwise, <see langword="false" />.</returns>
		protected bool IsCurrentInterface
		{
			get
			{
				return this._currentClass != null && !(this._currentClass is CodeTypeDelegate) && this._currentClass.IsInterface;
			}
		}

		/// <summary>Gets a value indicating whether the current object being generated is a class.</summary>
		/// <returns>
		///   <see langword="true" /> if the current object is a class; otherwise, <see langword="false" />.</returns>
		protected bool IsCurrentClass
		{
			get
			{
				return this._currentClass != null && !(this._currentClass is CodeTypeDelegate) && this._currentClass.IsClass;
			}
		}

		/// <summary>Gets a value indicating whether the current object being generated is a value type or struct.</summary>
		/// <returns>
		///   <see langword="true" /> if the current object is a value type or struct; otherwise, <see langword="false" />.</returns>
		protected bool IsCurrentStruct
		{
			get
			{
				return this._currentClass != null && !(this._currentClass is CodeTypeDelegate) && this._currentClass.IsStruct;
			}
		}

		/// <summary>Gets a value indicating whether the current object being generated is an enumeration.</summary>
		/// <returns>
		///   <see langword="true" /> if the current object is an enumeration; otherwise, <see langword="false" />.</returns>
		protected bool IsCurrentEnum
		{
			get
			{
				return this._currentClass != null && !(this._currentClass is CodeTypeDelegate) && this._currentClass.IsEnum;
			}
		}

		/// <summary>Gets a value indicating whether the current object being generated is a delegate.</summary>
		/// <returns>
		///   <see langword="true" /> if the current object is a delegate; otherwise, <see langword="false" />.</returns>
		protected bool IsCurrentDelegate
		{
			get
			{
				return this._currentClass != null && this._currentClass is CodeTypeDelegate;
			}
		}

		/// <summary>Gets or sets the amount of spaces to indent each indentation level.</summary>
		/// <returns>The number of spaces to indent for each indentation level.</returns>
		protected int Indent
		{
			get
			{
				return this._output.Indent;
			}
			set
			{
				this._output.Indent = value;
			}
		}

		/// <summary>Gets the token that represents <see langword="null" />.</summary>
		/// <returns>The token that represents <see langword="null" />.</returns>
		protected abstract string NullToken { get; }

		/// <summary>Gets the text writer to use for output.</summary>
		/// <returns>The text writer to use for output.</returns>
		protected TextWriter Output
		{
			get
			{
				return this._output;
			}
		}

		/// <summary>Gets the options to be used by the code generator.</summary>
		/// <returns>An object that indicates the options for the code generator to use.</returns>
		protected CodeGeneratorOptions Options
		{
			get
			{
				return this._options;
			}
		}

		private void GenerateType(CodeTypeDeclaration e)
		{
			this._currentClass = e;
			if (e.StartDirectives.Count > 0)
			{
				this.GenerateDirectives(e.StartDirectives);
			}
			this.GenerateCommentStatements(e.Comments);
			if (e.LinePragma != null)
			{
				this.GenerateLinePragmaStart(e.LinePragma);
			}
			this.GenerateTypeStart(e);
			if (this.Options.VerbatimOrder)
			{
				using (IEnumerator enumerator = e.Members.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						object obj = enumerator.Current;
						CodeTypeMember member = (CodeTypeMember)obj;
						this.GenerateTypeMember(member, e);
					}
					goto IL_CA;
				}
			}
			this.GenerateFields(e);
			this.GenerateSnippetMembers(e);
			this.GenerateTypeConstructors(e);
			this.GenerateConstructors(e);
			this.GenerateProperties(e);
			this.GenerateEvents(e);
			this.GenerateMethods(e);
			this.GenerateNestedTypes(e);
			IL_CA:
			this._currentClass = e;
			this.GenerateTypeEnd(e);
			if (e.LinePragma != null)
			{
				this.GenerateLinePragmaEnd(e.LinePragma);
			}
			if (e.EndDirectives.Count > 0)
			{
				this.GenerateDirectives(e.EndDirectives);
			}
		}

		/// <summary>Generates code for the specified code directives.</summary>
		/// <param name="directives">The code directives to generate code for.</param>
		protected virtual void GenerateDirectives(CodeDirectiveCollection directives)
		{
		}

		private void GenerateTypeMember(CodeTypeMember member, CodeTypeDeclaration declaredType)
		{
			if (this._options.BlankLinesBetweenMembers)
			{
				this.Output.WriteLine();
			}
			if (member is CodeTypeDeclaration)
			{
				((ICodeGenerator)this).GenerateCodeFromType((CodeTypeDeclaration)member, this._output.InnerWriter, this._options);
				this._currentClass = declaredType;
				return;
			}
			if (member.StartDirectives.Count > 0)
			{
				this.GenerateDirectives(member.StartDirectives);
			}
			this.GenerateCommentStatements(member.Comments);
			if (member.LinePragma != null)
			{
				this.GenerateLinePragmaStart(member.LinePragma);
			}
			if (member is CodeMemberField)
			{
				this.GenerateField((CodeMemberField)member);
			}
			else if (member is CodeMemberProperty)
			{
				this.GenerateProperty((CodeMemberProperty)member, declaredType);
			}
			else if (member is CodeMemberMethod)
			{
				if (member is CodeConstructor)
				{
					this.GenerateConstructor((CodeConstructor)member, declaredType);
				}
				else if (member is CodeTypeConstructor)
				{
					this.GenerateTypeConstructor((CodeTypeConstructor)member);
				}
				else if (member is CodeEntryPointMethod)
				{
					this.GenerateEntryPointMethod((CodeEntryPointMethod)member, declaredType);
				}
				else
				{
					this.GenerateMethod((CodeMemberMethod)member, declaredType);
				}
			}
			else if (member is CodeMemberEvent)
			{
				this.GenerateEvent((CodeMemberEvent)member, declaredType);
			}
			else if (member is CodeSnippetTypeMember)
			{
				int indent = this.Indent;
				this.Indent = 0;
				this.GenerateSnippetMember((CodeSnippetTypeMember)member);
				this.Indent = indent;
				this.Output.WriteLine();
			}
			if (member.LinePragma != null)
			{
				this.GenerateLinePragmaEnd(member.LinePragma);
			}
			if (member.EndDirectives.Count > 0)
			{
				this.GenerateDirectives(member.EndDirectives);
			}
		}

		private void GenerateTypeConstructors(CodeTypeDeclaration e)
		{
			foreach (object obj in e.Members)
			{
				CodeTypeMember codeTypeMember = (CodeTypeMember)obj;
				if (codeTypeMember is CodeTypeConstructor)
				{
					this._currentMember = codeTypeMember;
					if (this._options.BlankLinesBetweenMembers)
					{
						this.Output.WriteLine();
					}
					if (this._currentMember.StartDirectives.Count > 0)
					{
						this.GenerateDirectives(this._currentMember.StartDirectives);
					}
					this.GenerateCommentStatements(this._currentMember.Comments);
					CodeTypeConstructor codeTypeConstructor = (CodeTypeConstructor)codeTypeMember;
					if (codeTypeConstructor.LinePragma != null)
					{
						this.GenerateLinePragmaStart(codeTypeConstructor.LinePragma);
					}
					this.GenerateTypeConstructor(codeTypeConstructor);
					if (codeTypeConstructor.LinePragma != null)
					{
						this.GenerateLinePragmaEnd(codeTypeConstructor.LinePragma);
					}
					if (this._currentMember.EndDirectives.Count > 0)
					{
						this.GenerateDirectives(this._currentMember.EndDirectives);
					}
				}
			}
		}

		/// <summary>Generates code for the namespaces in the specified compile unit.</summary>
		/// <param name="e">The compile unit to generate namespaces for.</param>
		protected void GenerateNamespaces(CodeCompileUnit e)
		{
			foreach (object obj in e.Namespaces)
			{
				CodeNamespace e2 = (CodeNamespace)obj;
				((ICodeGenerator)this).GenerateCodeFromNamespace(e2, this._output.InnerWriter, this._options);
			}
		}

		/// <summary>Generates code for the specified namespace and the classes it contains.</summary>
		/// <param name="e">The namespace to generate classes for.</param>
		protected void GenerateTypes(CodeNamespace e)
		{
			foreach (object obj in e.Types)
			{
				CodeTypeDeclaration e2 = (CodeTypeDeclaration)obj;
				if (this._options.BlankLinesBetweenMembers)
				{
					this.Output.WriteLine();
				}
				((ICodeGenerator)this).GenerateCodeFromType(e2, this._output.InnerWriter, this._options);
			}
		}

		/// <summary>Gets a value indicating whether the generator provides support for the language features represented by the specified <see cref="T:System.CodeDom.Compiler.GeneratorSupport" /> object.</summary>
		/// <param name="support">The capabilities to test the generator for.</param>
		/// <returns>
		///   <see langword="true" /> if the specified capabilities are supported; otherwise, <see langword="false" />.</returns>
		bool ICodeGenerator.Supports(GeneratorSupport support)
		{
			return this.Supports(support);
		}

		/// <summary>Generates code for the specified Code Document Object Model (CodeDOM) type declaration and outputs it to the specified text writer using the specified options.</summary>
		/// <param name="e">The type to generate code for.</param>
		/// <param name="w">The text writer to output code to.</param>
		/// <param name="o">The options to use for generating code.</param>
		/// <exception cref="T:System.InvalidOperationException">
		///   <paramref name="w" /> is not available. <paramref name="w" /> may have been closed before the method call was made.</exception>
		void ICodeGenerator.GenerateCodeFromType(CodeTypeDeclaration e, TextWriter w, CodeGeneratorOptions o)
		{
			bool flag = false;
			if (this._output != null && w != this._output.InnerWriter)
			{
				throw new InvalidOperationException("The output writer for code generation and the writer supplied don't match and cannot be used. This is generally caused by a bad implementation of a CodeGenerator derived class.");
			}
			if (this._output == null)
			{
				flag = true;
				this._options = (o ?? new CodeGeneratorOptions());
				this._output = new ExposedTabStringIndentedTextWriter(w, this._options.IndentString);
			}
			try
			{
				this.GenerateType(e);
			}
			finally
			{
				if (flag)
				{
					this._output = null;
					this._options = null;
				}
			}
		}

		/// <summary>Generates code for the specified Code Document Object Model (CodeDOM) expression and outputs it to the specified text writer.</summary>
		/// <param name="e">The expression to generate code for.</param>
		/// <param name="w">The text writer to output code to.</param>
		/// <param name="o">The options to use for generating code.</param>
		/// <exception cref="T:System.InvalidOperationException">
		///   <paramref name="w" /> is not available. <paramref name="w" /> may have been closed before the method call was made.</exception>
		void ICodeGenerator.GenerateCodeFromExpression(CodeExpression e, TextWriter w, CodeGeneratorOptions o)
		{
			bool flag = false;
			if (this._output != null && w != this._output.InnerWriter)
			{
				throw new InvalidOperationException("The output writer for code generation and the writer supplied don't match and cannot be used. This is generally caused by a bad implementation of a CodeGenerator derived class.");
			}
			if (this._output == null)
			{
				flag = true;
				this._options = (o ?? new CodeGeneratorOptions());
				this._output = new ExposedTabStringIndentedTextWriter(w, this._options.IndentString);
			}
			try
			{
				this.GenerateExpression(e);
			}
			finally
			{
				if (flag)
				{
					this._output = null;
					this._options = null;
				}
			}
		}

		/// <summary>Generates code for the specified Code Document Object Model (CodeDOM) compilation unit and outputs it to the specified text writer using the specified options.</summary>
		/// <param name="e">The CodeDOM compilation unit to generate code for.</param>
		/// <param name="w">The text writer to output code to.</param>
		/// <param name="o">The options to use for generating code.</param>
		/// <exception cref="T:System.InvalidOperationException">
		///   <paramref name="w" /> is not available. <paramref name="w" /> may have been closed before the method call was made.</exception>
		void ICodeGenerator.GenerateCodeFromCompileUnit(CodeCompileUnit e, TextWriter w, CodeGeneratorOptions o)
		{
			bool flag = false;
			if (this._output != null && w != this._output.InnerWriter)
			{
				throw new InvalidOperationException("The output writer for code generation and the writer supplied don't match and cannot be used. This is generally caused by a bad implementation of a CodeGenerator derived class.");
			}
			if (this._output == null)
			{
				flag = true;
				this._options = (o ?? new CodeGeneratorOptions());
				this._output = new ExposedTabStringIndentedTextWriter(w, this._options.IndentString);
			}
			try
			{
				if (e is CodeSnippetCompileUnit)
				{
					this.GenerateSnippetCompileUnit((CodeSnippetCompileUnit)e);
				}
				else
				{
					this.GenerateCompileUnit(e);
				}
			}
			finally
			{
				if (flag)
				{
					this._output = null;
					this._options = null;
				}
			}
		}

		/// <summary>Generates code for the specified Code Document Object Model (CodeDOM) namespace and outputs it to the specified text writer using the specified options.</summary>
		/// <param name="e">The namespace to generate code for.</param>
		/// <param name="w">The text writer to output code to.</param>
		/// <param name="o">The options to use for generating code.</param>
		/// <exception cref="T:System.InvalidOperationException">
		///   <paramref name="w" /> is not available. <paramref name="w" /> may have been closed before the method call was made.</exception>
		void ICodeGenerator.GenerateCodeFromNamespace(CodeNamespace e, TextWriter w, CodeGeneratorOptions o)
		{
			bool flag = false;
			if (this._output != null && w != this._output.InnerWriter)
			{
				throw new InvalidOperationException("The output writer for code generation and the writer supplied don't match and cannot be used. This is generally caused by a bad implementation of a CodeGenerator derived class.");
			}
			if (this._output == null)
			{
				flag = true;
				this._options = (o ?? new CodeGeneratorOptions());
				this._output = new ExposedTabStringIndentedTextWriter(w, this._options.IndentString);
			}
			try
			{
				this.GenerateNamespace(e);
			}
			finally
			{
				if (flag)
				{
					this._output = null;
					this._options = null;
				}
			}
		}

		/// <summary>Generates code for the specified Code Document Object Model (CodeDOM) statement and outputs it to the specified text writer using the specified options.</summary>
		/// <param name="e">The statement that contains the CodeDOM elements to translate.</param>
		/// <param name="w">The text writer to output code to.</param>
		/// <param name="o">The options to use for generating code.</param>
		/// <exception cref="T:System.InvalidOperationException">
		///   <paramref name="w" /> is not available. <paramref name="w" /> may have been closed before the method call was made.</exception>
		void ICodeGenerator.GenerateCodeFromStatement(CodeStatement e, TextWriter w, CodeGeneratorOptions o)
		{
			bool flag = false;
			if (this._output != null && w != this._output.InnerWriter)
			{
				throw new InvalidOperationException("The output writer for code generation and the writer supplied don't match and cannot be used. This is generally caused by a bad implementation of a CodeGenerator derived class.");
			}
			if (this._output == null)
			{
				flag = true;
				this._options = (o ?? new CodeGeneratorOptions());
				this._output = new ExposedTabStringIndentedTextWriter(w, this._options.IndentString);
			}
			try
			{
				this.GenerateStatement(e);
			}
			finally
			{
				if (flag)
				{
					this._output = null;
					this._options = null;
				}
			}
		}

		/// <summary>Generates code for the specified class member using the specified text writer and code generator options.</summary>
		/// <param name="member">The class member to generate code for.</param>
		/// <param name="writer">The text writer to output code to.</param>
		/// <param name="options">The options to use when generating the code.</param>
		/// <exception cref="T:System.InvalidOperationException">The <see cref="P:System.CodeDom.Compiler.CodeGenerator.Output" /> property is not <see langword="null" />.</exception>
		public virtual void GenerateCodeFromMember(CodeTypeMember member, TextWriter writer, CodeGeneratorOptions options)
		{
			if (this._output != null)
			{
				throw new InvalidOperationException("This code generation API cannot be called while the generator is being used to generate something else.");
			}
			this._options = (options ?? new CodeGeneratorOptions());
			this._output = new ExposedTabStringIndentedTextWriter(writer, this._options.IndentString);
			try
			{
				CodeTypeDeclaration codeTypeDeclaration = new CodeTypeDeclaration();
				this._currentClass = codeTypeDeclaration;
				this.GenerateTypeMember(member, codeTypeDeclaration);
			}
			finally
			{
				this._currentClass = null;
				this._output = null;
				this._options = null;
			}
		}

		/// <summary>Gets a value that indicates whether the specified value is a valid identifier for the current language.</summary>
		/// <param name="value">The value to test.</param>
		/// <returns>
		///   <see langword="true" /> if the <paramref name="value" /> parameter is a valid identifier; otherwise, <see langword="false" />.</returns>
		bool ICodeGenerator.IsValidIdentifier(string value)
		{
			return this.IsValidIdentifier(value);
		}

		/// <summary>Throws an exception if the specified value is not a valid identifier.</summary>
		/// <param name="value">The identifier to validate.</param>
		void ICodeGenerator.ValidateIdentifier(string value)
		{
			this.ValidateIdentifier(value);
		}

		/// <summary>Creates an escaped identifier for the specified value.</summary>
		/// <param name="value">The string to create an escaped identifier for.</param>
		/// <returns>The escaped identifier for the value.</returns>
		string ICodeGenerator.CreateEscapedIdentifier(string value)
		{
			return this.CreateEscapedIdentifier(value);
		}

		/// <summary>Creates a valid identifier for the specified value.</summary>
		/// <param name="value">The string to generate a valid identifier for.</param>
		/// <returns>A valid identifier for the specified value.</returns>
		string ICodeGenerator.CreateValidIdentifier(string value)
		{
			return this.CreateValidIdentifier(value);
		}

		/// <summary>Gets the type indicated by the specified <see cref="T:System.CodeDom.CodeTypeReference" />.</summary>
		/// <param name="type">The type to return.</param>
		/// <returns>The name of the data type reference.</returns>
		string ICodeGenerator.GetTypeOutput(CodeTypeReference type)
		{
			return this.GetTypeOutput(type);
		}

		private void GenerateConstructors(CodeTypeDeclaration e)
		{
			foreach (object obj in e.Members)
			{
				CodeTypeMember codeTypeMember = (CodeTypeMember)obj;
				if (codeTypeMember is CodeConstructor)
				{
					this._currentMember = codeTypeMember;
					if (this._options.BlankLinesBetweenMembers)
					{
						this.Output.WriteLine();
					}
					if (this._currentMember.StartDirectives.Count > 0)
					{
						this.GenerateDirectives(this._currentMember.StartDirectives);
					}
					this.GenerateCommentStatements(this._currentMember.Comments);
					CodeConstructor codeConstructor = (CodeConstructor)codeTypeMember;
					if (codeConstructor.LinePragma != null)
					{
						this.GenerateLinePragmaStart(codeConstructor.LinePragma);
					}
					this.GenerateConstructor(codeConstructor, e);
					if (codeConstructor.LinePragma != null)
					{
						this.GenerateLinePragmaEnd(codeConstructor.LinePragma);
					}
					if (this._currentMember.EndDirectives.Count > 0)
					{
						this.GenerateDirectives(this._currentMember.EndDirectives);
					}
				}
			}
		}

		private void GenerateEvents(CodeTypeDeclaration e)
		{
			foreach (object obj in e.Members)
			{
				CodeTypeMember codeTypeMember = (CodeTypeMember)obj;
				if (codeTypeMember is CodeMemberEvent)
				{
					this._currentMember = codeTypeMember;
					if (this._options.BlankLinesBetweenMembers)
					{
						this.Output.WriteLine();
					}
					if (this._currentMember.StartDirectives.Count > 0)
					{
						this.GenerateDirectives(this._currentMember.StartDirectives);
					}
					this.GenerateCommentStatements(this._currentMember.Comments);
					CodeMemberEvent codeMemberEvent = (CodeMemberEvent)codeTypeMember;
					if (codeMemberEvent.LinePragma != null)
					{
						this.GenerateLinePragmaStart(codeMemberEvent.LinePragma);
					}
					this.GenerateEvent(codeMemberEvent, e);
					if (codeMemberEvent.LinePragma != null)
					{
						this.GenerateLinePragmaEnd(codeMemberEvent.LinePragma);
					}
					if (this._currentMember.EndDirectives.Count > 0)
					{
						this.GenerateDirectives(this._currentMember.EndDirectives);
					}
				}
			}
		}

		/// <summary>Generates code for the specified code expression.</summary>
		/// <param name="e">The code expression to generate code for.</param>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="e" /> is <see langword="null" />.</exception>
		/// <exception cref="T:System.ArgumentException">
		///   <paramref name="e" /> is not a valid <see cref="T:System.CodeDom.CodeStatement" />.</exception>
		protected void GenerateExpression(CodeExpression e)
		{
			if (e is CodeArrayCreateExpression)
			{
				this.GenerateArrayCreateExpression((CodeArrayCreateExpression)e);
				return;
			}
			if (e is CodeBaseReferenceExpression)
			{
				this.GenerateBaseReferenceExpression((CodeBaseReferenceExpression)e);
				return;
			}
			if (e is CodeBinaryOperatorExpression)
			{
				this.GenerateBinaryOperatorExpression((CodeBinaryOperatorExpression)e);
				return;
			}
			if (e is CodeCastExpression)
			{
				this.GenerateCastExpression((CodeCastExpression)e);
				return;
			}
			if (e is CodeDelegateCreateExpression)
			{
				this.GenerateDelegateCreateExpression((CodeDelegateCreateExpression)e);
				return;
			}
			if (e is CodeFieldReferenceExpression)
			{
				this.GenerateFieldReferenceExpression((CodeFieldReferenceExpression)e);
				return;
			}
			if (e is CodeArgumentReferenceExpression)
			{
				this.GenerateArgumentReferenceExpression((CodeArgumentReferenceExpression)e);
				return;
			}
			if (e is CodeVariableReferenceExpression)
			{
				this.GenerateVariableReferenceExpression((CodeVariableReferenceExpression)e);
				return;
			}
			if (e is CodeIndexerExpression)
			{
				this.GenerateIndexerExpression((CodeIndexerExpression)e);
				return;
			}
			if (e is CodeArrayIndexerExpression)
			{
				this.GenerateArrayIndexerExpression((CodeArrayIndexerExpression)e);
				return;
			}
			if (e is CodeSnippetExpression)
			{
				this.GenerateSnippetExpression((CodeSnippetExpression)e);
				return;
			}
			if (e is CodeMethodInvokeExpression)
			{
				this.GenerateMethodInvokeExpression((CodeMethodInvokeExpression)e);
				return;
			}
			if (e is CodeMethodReferenceExpression)
			{
				this.GenerateMethodReferenceExpression((CodeMethodReferenceExpression)e);
				return;
			}
			if (e is CodeEventReferenceExpression)
			{
				this.GenerateEventReferenceExpression((CodeEventReferenceExpression)e);
				return;
			}
			if (e is CodeDelegateInvokeExpression)
			{
				this.GenerateDelegateInvokeExpression((CodeDelegateInvokeExpression)e);
				return;
			}
			if (e is CodeObjectCreateExpression)
			{
				this.GenerateObjectCreateExpression((CodeObjectCreateExpression)e);
				return;
			}
			if (e is CodeParameterDeclarationExpression)
			{
				this.GenerateParameterDeclarationExpression((CodeParameterDeclarationExpression)e);
				return;
			}
			if (e is CodeDirectionExpression)
			{
				this.GenerateDirectionExpression((CodeDirectionExpression)e);
				return;
			}
			if (e is CodePrimitiveExpression)
			{
				this.GeneratePrimitiveExpression((CodePrimitiveExpression)e);
				return;
			}
			if (e is CodePropertyReferenceExpression)
			{
				this.GeneratePropertyReferenceExpression((CodePropertyReferenceExpression)e);
				return;
			}
			if (e is CodePropertySetValueReferenceExpression)
			{
				this.GeneratePropertySetValueReferenceExpression((CodePropertySetValueReferenceExpression)e);
				return;
			}
			if (e is CodeThisReferenceExpression)
			{
				this.GenerateThisReferenceExpression((CodeThisReferenceExpression)e);
				return;
			}
			if (e is CodeTypeReferenceExpression)
			{
				this.GenerateTypeReferenceExpression((CodeTypeReferenceExpression)e);
				return;
			}
			if (e is CodeTypeOfExpression)
			{
				this.GenerateTypeOfExpression((CodeTypeOfExpression)e);
				return;
			}
			if (e is CodeDefaultValueExpression)
			{
				this.GenerateDefaultValueExpression((CodeDefaultValueExpression)e);
				return;
			}
			if (e == null)
			{
				throw new ArgumentNullException("e");
			}
			throw new ArgumentException(SR.Format("Element type {0} is not supported.", e.GetType().FullName), "e");
		}

		private void GenerateFields(CodeTypeDeclaration e)
		{
			foreach (object obj in e.Members)
			{
				CodeTypeMember codeTypeMember = (CodeTypeMember)obj;
				if (codeTypeMember is CodeMemberField)
				{
					this._currentMember = codeTypeMember;
					if (this._options.BlankLinesBetweenMembers)
					{
						this.Output.WriteLine();
					}
					if (this._currentMember.StartDirectives.Count > 0)
					{
						this.GenerateDirectives(this._currentMember.StartDirectives);
					}
					this.GenerateCommentStatements(this._currentMember.Comments);
					CodeMemberField codeMemberField = (CodeMemberField)codeTypeMember;
					if (codeMemberField.LinePragma != null)
					{
						this.GenerateLinePragmaStart(codeMemberField.LinePragma);
					}
					this.GenerateField(codeMemberField);
					if (codeMemberField.LinePragma != null)
					{
						this.GenerateLinePragmaEnd(codeMemberField.LinePragma);
					}
					if (this._currentMember.EndDirectives.Count > 0)
					{
						this.GenerateDirectives(this._currentMember.EndDirectives);
					}
				}
			}
		}

		private void GenerateSnippetMembers(CodeTypeDeclaration e)
		{
			bool flag = false;
			foreach (object obj in e.Members)
			{
				CodeTypeMember codeTypeMember = (CodeTypeMember)obj;
				if (codeTypeMember is CodeSnippetTypeMember)
				{
					flag = true;
					this._currentMember = codeTypeMember;
					if (this._options.BlankLinesBetweenMembers)
					{
						this.Output.WriteLine();
					}
					if (this._currentMember.StartDirectives.Count > 0)
					{
						this.GenerateDirectives(this._currentMember.StartDirectives);
					}
					this.GenerateCommentStatements(this._currentMember.Comments);
					CodeSnippetTypeMember codeSnippetTypeMember = (CodeSnippetTypeMember)codeTypeMember;
					if (codeSnippetTypeMember.LinePragma != null)
					{
						this.GenerateLinePragmaStart(codeSnippetTypeMember.LinePragma);
					}
					int indent = this.Indent;
					this.Indent = 0;
					this.GenerateSnippetMember(codeSnippetTypeMember);
					this.Indent = indent;
					if (codeSnippetTypeMember.LinePragma != null)
					{
						this.GenerateLinePragmaEnd(codeSnippetTypeMember.LinePragma);
					}
					if (this._currentMember.EndDirectives.Count > 0)
					{
						this.GenerateDirectives(this._currentMember.EndDirectives);
					}
				}
			}
			if (flag)
			{
				this.Output.WriteLine();
			}
		}

		/// <summary>Outputs the code of the specified literal code fragment compile unit.</summary>
		/// <param name="e">The literal code fragment compile unit to generate code for.</param>
		protected virtual void GenerateSnippetCompileUnit(CodeSnippetCompileUnit e)
		{
			this.GenerateDirectives(e.StartDirectives);
			if (e.LinePragma != null)
			{
				this.GenerateLinePragmaStart(e.LinePragma);
			}
			this.Output.WriteLine(e.Value);
			if (e.LinePragma != null)
			{
				this.GenerateLinePragmaEnd(e.LinePragma);
			}
			if (e.EndDirectives.Count > 0)
			{
				this.GenerateDirectives(e.EndDirectives);
			}
		}

		private void GenerateMethods(CodeTypeDeclaration e)
		{
			foreach (object obj in e.Members)
			{
				CodeTypeMember codeTypeMember = (CodeTypeMember)obj;
				if (codeTypeMember is CodeMemberMethod && !(codeTypeMember is CodeTypeConstructor) && !(codeTypeMember is CodeConstructor))
				{
					this._currentMember = codeTypeMember;
					if (this._options.BlankLinesBetweenMembers)
					{
						this.Output.WriteLine();
					}
					if (this._currentMember.StartDirectives.Count > 0)
					{
						this.GenerateDirectives(this._currentMember.StartDirectives);
					}
					this.GenerateCommentStatements(this._currentMember.Comments);
					CodeMemberMethod codeMemberMethod = (CodeMemberMethod)codeTypeMember;
					if (codeMemberMethod.LinePragma != null)
					{
						this.GenerateLinePragmaStart(codeMemberMethod.LinePragma);
					}
					if (codeTypeMember is CodeEntryPointMethod)
					{
						this.GenerateEntryPointMethod((CodeEntryPointMethod)codeTypeMember, e);
					}
					else
					{
						this.GenerateMethod(codeMemberMethod, e);
					}
					if (codeMemberMethod.LinePragma != null)
					{
						this.GenerateLinePragmaEnd(codeMemberMethod.LinePragma);
					}
					if (this._currentMember.EndDirectives.Count > 0)
					{
						this.GenerateDirectives(this._currentMember.EndDirectives);
					}
				}
			}
		}

		private void GenerateNestedTypes(CodeTypeDeclaration e)
		{
			foreach (object obj in e.Members)
			{
				CodeTypeMember codeTypeMember = (CodeTypeMember)obj;
				if (codeTypeMember is CodeTypeDeclaration)
				{
					if (this._options.BlankLinesBetweenMembers)
					{
						this.Output.WriteLine();
					}
					CodeTypeDeclaration e2 = (CodeTypeDeclaration)codeTypeMember;
					((ICodeGenerator)this).GenerateCodeFromType(e2, this._output.InnerWriter, this._options);
				}
			}
		}

		/// <summary>Generates code for the specified compile unit.</summary>
		/// <param name="e">The compile unit to generate code for.</param>
		protected virtual void GenerateCompileUnit(CodeCompileUnit e)
		{
			this.GenerateCompileUnitStart(e);
			this.GenerateNamespaces(e);
			this.GenerateCompileUnitEnd(e);
		}

		/// <summary>Generates code for the specified namespace.</summary>
		/// <param name="e">The namespace to generate code for.</param>
		protected virtual void GenerateNamespace(CodeNamespace e)
		{
			this.GenerateCommentStatements(e.Comments);
			this.GenerateNamespaceStart(e);
			this.GenerateNamespaceImports(e);
			this.Output.WriteLine();
			this.GenerateTypes(e);
			this.GenerateNamespaceEnd(e);
		}

		/// <summary>Generates code for the specified namespace import.</summary>
		/// <param name="e">The namespace import to generate code for.</param>
		protected void GenerateNamespaceImports(CodeNamespace e)
		{
			foreach (object obj in e.Imports)
			{
				CodeNamespaceImport codeNamespaceImport = (CodeNamespaceImport)obj;
				if (codeNamespaceImport.LinePragma != null)
				{
					this.GenerateLinePragmaStart(codeNamespaceImport.LinePragma);
				}
				this.GenerateNamespaceImport(codeNamespaceImport);
				if (codeNamespaceImport.LinePragma != null)
				{
					this.GenerateLinePragmaEnd(codeNamespaceImport.LinePragma);
				}
			}
		}

		private void GenerateProperties(CodeTypeDeclaration e)
		{
			foreach (object obj in e.Members)
			{
				CodeTypeMember codeTypeMember = (CodeTypeMember)obj;
				if (codeTypeMember is CodeMemberProperty)
				{
					this._currentMember = codeTypeMember;
					if (this._options.BlankLinesBetweenMembers)
					{
						this.Output.WriteLine();
					}
					if (this._currentMember.StartDirectives.Count > 0)
					{
						this.GenerateDirectives(this._currentMember.StartDirectives);
					}
					this.GenerateCommentStatements(this._currentMember.Comments);
					CodeMemberProperty codeMemberProperty = (CodeMemberProperty)codeTypeMember;
					if (codeMemberProperty.LinePragma != null)
					{
						this.GenerateLinePragmaStart(codeMemberProperty.LinePragma);
					}
					this.GenerateProperty(codeMemberProperty, e);
					if (codeMemberProperty.LinePragma != null)
					{
						this.GenerateLinePragmaEnd(codeMemberProperty.LinePragma);
					}
					if (this._currentMember.EndDirectives.Count > 0)
					{
						this.GenerateDirectives(this._currentMember.EndDirectives);
					}
				}
			}
		}

		/// <summary>Generates code for the specified statement.</summary>
		/// <param name="e">The statement to generate code for.</param>
		/// <exception cref="T:System.ArgumentException">
		///   <paramref name="e" /> is not a valid <see cref="T:System.CodeDom.CodeStatement" />.</exception>
		protected void GenerateStatement(CodeStatement e)
		{
			if (e.StartDirectives.Count > 0)
			{
				this.GenerateDirectives(e.StartDirectives);
			}
			if (e.LinePragma != null)
			{
				this.GenerateLinePragmaStart(e.LinePragma);
			}
			if (e is CodeCommentStatement)
			{
				this.GenerateCommentStatement((CodeCommentStatement)e);
			}
			else if (e is CodeMethodReturnStatement)
			{
				this.GenerateMethodReturnStatement((CodeMethodReturnStatement)e);
			}
			else if (e is CodeConditionStatement)
			{
				this.GenerateConditionStatement((CodeConditionStatement)e);
			}
			else if (e is CodeTryCatchFinallyStatement)
			{
				this.GenerateTryCatchFinallyStatement((CodeTryCatchFinallyStatement)e);
			}
			else if (e is CodeAssignStatement)
			{
				this.GenerateAssignStatement((CodeAssignStatement)e);
			}
			else if (e is CodeExpressionStatement)
			{
				this.GenerateExpressionStatement((CodeExpressionStatement)e);
			}
			else if (e is CodeIterationStatement)
			{
				this.GenerateIterationStatement((CodeIterationStatement)e);
			}
			else if (e is CodeThrowExceptionStatement)
			{
				this.GenerateThrowExceptionStatement((CodeThrowExceptionStatement)e);
			}
			else if (e is CodeSnippetStatement)
			{
				int indent = this.Indent;
				this.Indent = 0;
				this.GenerateSnippetStatement((CodeSnippetStatement)e);
				this.Indent = indent;
			}
			else if (e is CodeVariableDeclarationStatement)
			{
				this.GenerateVariableDeclarationStatement((CodeVariableDeclarationStatement)e);
			}
			else if (e is CodeAttachEventStatement)
			{
				this.GenerateAttachEventStatement((CodeAttachEventStatement)e);
			}
			else if (e is CodeRemoveEventStatement)
			{
				this.GenerateRemoveEventStatement((CodeRemoveEventStatement)e);
			}
			else if (e is CodeGotoStatement)
			{
				this.GenerateGotoStatement((CodeGotoStatement)e);
			}
			else
			{
				if (!(e is CodeLabeledStatement))
				{
					throw new ArgumentException(SR.Format("Element type {0} is not supported.", e.GetType().FullName), "e");
				}
				this.GenerateLabeledStatement((CodeLabeledStatement)e);
			}
			if (e.LinePragma != null)
			{
				this.GenerateLinePragmaEnd(e.LinePragma);
			}
			if (e.EndDirectives.Count > 0)
			{
				this.GenerateDirectives(e.EndDirectives);
			}
		}

		/// <summary>Generates code for the specified statement collection.</summary>
		/// <param name="stms">The statements to generate code for.</param>
		protected void GenerateStatements(CodeStatementCollection stmts)
		{
			foreach (object obj in stmts)
			{
				CodeStatement e = (CodeStatement)obj;
				((ICodeGenerator)this).GenerateCodeFromStatement(e, this._output.InnerWriter, this._options);
			}
		}

		/// <summary>Generates code for the specified attribute declaration collection.</summary>
		/// <param name="attributes">The attributes to generate code for.</param>
		protected virtual void OutputAttributeDeclarations(CodeAttributeDeclarationCollection attributes)
		{
			if (attributes.Count == 0)
			{
				return;
			}
			this.GenerateAttributeDeclarationsStart(attributes);
			bool flag = true;
			foreach (object obj in attributes)
			{
				CodeAttributeDeclaration codeAttributeDeclaration = (CodeAttributeDeclaration)obj;
				if (flag)
				{
					flag = false;
				}
				else
				{
					this.ContinueOnNewLine(", ");
				}
				this.Output.Write(codeAttributeDeclaration.Name);
				this.Output.Write('(');
				bool flag2 = true;
				foreach (object obj2 in codeAttributeDeclaration.Arguments)
				{
					CodeAttributeArgument arg = (CodeAttributeArgument)obj2;
					if (flag2)
					{
						flag2 = false;
					}
					else
					{
						this.Output.Write(", ");
					}
					this.OutputAttributeArgument(arg);
				}
				this.Output.Write(')');
			}
			this.GenerateAttributeDeclarationsEnd(attributes);
		}

		/// <summary>Outputs an argument in an attribute block.</summary>
		/// <param name="arg">The attribute argument to generate code for.</param>
		protected virtual void OutputAttributeArgument(CodeAttributeArgument arg)
		{
			if (!string.IsNullOrEmpty(arg.Name))
			{
				this.OutputIdentifier(arg.Name);
				this.Output.Write('=');
			}
			((ICodeGenerator)this).GenerateCodeFromExpression(arg.Value, this._output.InnerWriter, this._options);
		}

		/// <summary>Generates code for the specified <see cref="T:System.CodeDom.FieldDirection" />.</summary>
		/// <param name="dir">One of the enumeration values that indicates the attribute of the field.</param>
		protected virtual void OutputDirection(FieldDirection dir)
		{
			switch (dir)
			{
			case FieldDirection.In:
				break;
			case FieldDirection.Out:
				this.Output.Write("out ");
				return;
			case FieldDirection.Ref:
				this.Output.Write("ref ");
				break;
			default:
				return;
			}
		}

		/// <summary>Outputs a field scope modifier that corresponds to the specified attributes.</summary>
		/// <param name="attributes">One of the enumeration values that specifies the attributes.</param>
		protected virtual void OutputFieldScopeModifier(MemberAttributes attributes)
		{
			if ((attributes & MemberAttributes.VTableMask) == MemberAttributes.New)
			{
				this.Output.Write("new ");
			}
			switch (attributes & MemberAttributes.ScopeMask)
			{
			case MemberAttributes.Final:
			case MemberAttributes.Override:
				break;
			case MemberAttributes.Static:
				this.Output.Write("static ");
				return;
			case MemberAttributes.Const:
				this.Output.Write("const ");
				break;
			default:
				return;
			}
		}

		/// <summary>Generates code for the specified member access modifier.</summary>
		/// <param name="attributes">One of the enumeration values that indicates the member access modifier to generate code for.</param>
		protected virtual void OutputMemberAccessModifier(MemberAttributes attributes)
		{
			MemberAttributes memberAttributes = attributes & MemberAttributes.AccessMask;
			if (memberAttributes <= MemberAttributes.Family)
			{
				if (memberAttributes == MemberAttributes.Assembly)
				{
					this.Output.Write("internal ");
					return;
				}
				if (memberAttributes == MemberAttributes.FamilyAndAssembly)
				{
					this.Output.Write("internal ");
					return;
				}
				if (memberAttributes != MemberAttributes.Family)
				{
					return;
				}
				this.Output.Write("protected ");
				return;
			}
			else
			{
				if (memberAttributes == MemberAttributes.FamilyOrAssembly)
				{
					this.Output.Write("protected internal ");
					return;
				}
				if (memberAttributes == MemberAttributes.Private)
				{
					this.Output.Write("private ");
					return;
				}
				if (memberAttributes != MemberAttributes.Public)
				{
					return;
				}
				this.Output.Write("public ");
				return;
			}
		}

		/// <summary>Generates code for the specified member scope modifier.</summary>
		/// <param name="attributes">One of the enumeration values that indicates the member scope modifier to generate code for.</param>
		protected virtual void OutputMemberScopeModifier(MemberAttributes attributes)
		{
			if ((attributes & MemberAttributes.VTableMask) == MemberAttributes.New)
			{
				this.Output.Write("new ");
			}
			switch (attributes & MemberAttributes.ScopeMask)
			{
			case MemberAttributes.Abstract:
				this.Output.Write("abstract ");
				return;
			case MemberAttributes.Final:
				this.Output.Write("");
				return;
			case MemberAttributes.Static:
				this.Output.Write("static ");
				return;
			case MemberAttributes.Override:
				this.Output.Write("override ");
				return;
			default:
			{
				MemberAttributes memberAttributes = attributes & MemberAttributes.AccessMask;
				if (memberAttributes == MemberAttributes.Family || memberAttributes == MemberAttributes.Public)
				{
					this.Output.Write("virtual ");
				}
				return;
			}
			}
		}

		/// <summary>Generates code for the specified type.</summary>
		/// <param name="typeRef">The type to generate code for.</param>
		protected abstract void OutputType(CodeTypeReference typeRef);

		/// <summary>Generates code for the specified type attributes.</summary>
		/// <param name="attributes">One of the enumeration values that indicates the type attributes to generate code for.</param>
		/// <param name="isStruct">
		///   <see langword="true" /> if the type is a struct; otherwise, <see langword="false" />.</param>
		/// <param name="isEnum">
		///   <see langword="true" /> if the type is an enum; otherwise, <see langword="false" />.</param>
		protected virtual void OutputTypeAttributes(TypeAttributes attributes, bool isStruct, bool isEnum)
		{
			TypeAttributes typeAttributes = attributes & TypeAttributes.VisibilityMask;
			if (typeAttributes - TypeAttributes.Public > 1)
			{
				if (typeAttributes == TypeAttributes.NestedPrivate)
				{
					this.Output.Write("private ");
				}
			}
			else
			{
				this.Output.Write("public ");
			}
			if (isStruct)
			{
				this.Output.Write("struct ");
				return;
			}
			if (isEnum)
			{
				this.Output.Write("enum ");
				return;
			}
			typeAttributes = (attributes & TypeAttributes.ClassSemanticsMask);
			if (typeAttributes == TypeAttributes.NotPublic)
			{
				if ((attributes & TypeAttributes.Sealed) == TypeAttributes.Sealed)
				{
					this.Output.Write("sealed ");
				}
				if ((attributes & TypeAttributes.Abstract) == TypeAttributes.Abstract)
				{
					this.Output.Write("abstract ");
				}
				this.Output.Write("class ");
				return;
			}
			if (typeAttributes != TypeAttributes.ClassSemanticsMask)
			{
				return;
			}
			this.Output.Write("interface ");
		}

		/// <summary>Generates code for the specified object type and name pair.</summary>
		/// <param name="typeRef">The type.</param>
		/// <param name="name">The name for the object.</param>
		protected virtual void OutputTypeNamePair(CodeTypeReference typeRef, string name)
		{
			this.OutputType(typeRef);
			this.Output.Write(' ');
			this.OutputIdentifier(name);
		}

		/// <summary>Outputs the specified identifier.</summary>
		/// <param name="ident">The identifier to output.</param>
		protected virtual void OutputIdentifier(string ident)
		{
			this.Output.Write(ident);
		}

		/// <summary>Generates code for the specified expression list.</summary>
		/// <param name="expressions">The expressions to generate code for.</param>
		protected virtual void OutputExpressionList(CodeExpressionCollection expressions)
		{
			this.OutputExpressionList(expressions, false);
		}

		/// <summary>Generates code for the specified expression list.</summary>
		/// <param name="expressions">The expressions to generate code for.</param>
		/// <param name="newlineBetweenItems">
		///   <see langword="true" /> to insert a new line after each item; otherwise, <see langword="false" />.</param>
		protected virtual void OutputExpressionList(CodeExpressionCollection expressions, bool newlineBetweenItems)
		{
			bool flag = true;
			int indent = this.Indent;
			this.Indent = indent + 1;
			foreach (object obj in expressions)
			{
				CodeExpression e = (CodeExpression)obj;
				if (flag)
				{
					flag = false;
				}
				else if (newlineBetweenItems)
				{
					this.ContinueOnNewLine(",");
				}
				else
				{
					this.Output.Write(", ");
				}
				((ICodeGenerator)this).GenerateCodeFromExpression(e, this._output.InnerWriter, this._options);
			}
			indent = this.Indent;
			this.Indent = indent - 1;
		}

		/// <summary>Generates code for the specified operator.</summary>
		/// <param name="op">The operator to generate code for.</param>
		protected virtual void OutputOperator(CodeBinaryOperatorType op)
		{
			switch (op)
			{
			case CodeBinaryOperatorType.Add:
				this.Output.Write('+');
				return;
			case CodeBinaryOperatorType.Subtract:
				this.Output.Write('-');
				return;
			case CodeBinaryOperatorType.Multiply:
				this.Output.Write('*');
				return;
			case CodeBinaryOperatorType.Divide:
				this.Output.Write('/');
				return;
			case CodeBinaryOperatorType.Modulus:
				this.Output.Write('%');
				return;
			case CodeBinaryOperatorType.Assign:
				this.Output.Write('=');
				return;
			case CodeBinaryOperatorType.IdentityInequality:
				this.Output.Write("!=");
				return;
			case CodeBinaryOperatorType.IdentityEquality:
				this.Output.Write("==");
				return;
			case CodeBinaryOperatorType.ValueEquality:
				this.Output.Write("==");
				return;
			case CodeBinaryOperatorType.BitwiseOr:
				this.Output.Write('|');
				return;
			case CodeBinaryOperatorType.BitwiseAnd:
				this.Output.Write('&');
				return;
			case CodeBinaryOperatorType.BooleanOr:
				this.Output.Write("||");
				return;
			case CodeBinaryOperatorType.BooleanAnd:
				this.Output.Write("&&");
				return;
			case CodeBinaryOperatorType.LessThan:
				this.Output.Write('<');
				return;
			case CodeBinaryOperatorType.LessThanOrEqual:
				this.Output.Write("<=");
				return;
			case CodeBinaryOperatorType.GreaterThan:
				this.Output.Write('>');
				return;
			case CodeBinaryOperatorType.GreaterThanOrEqual:
				this.Output.Write(">=");
				return;
			default:
				return;
			}
		}

		/// <summary>Generates code for the specified parameters.</summary>
		/// <param name="parameters">The parameter declaration expressions to generate code for.</param>
		protected virtual void OutputParameters(CodeParameterDeclarationExpressionCollection parameters)
		{
			bool flag = true;
			bool flag2 = parameters.Count > 15;
			if (flag2)
			{
				this.Indent += 3;
			}
			foreach (object obj in parameters)
			{
				CodeParameterDeclarationExpression e = (CodeParameterDeclarationExpression)obj;
				if (flag)
				{
					flag = false;
				}
				else
				{
					this.Output.Write(", ");
				}
				if (flag2)
				{
					this.ContinueOnNewLine("");
				}
				this.GenerateExpression(e);
			}
			if (flag2)
			{
				this.Indent -= 3;
			}
		}

		/// <summary>Generates code for the specified array creation expression.</summary>
		/// <param name="e">A <see cref="T:System.CodeDom.CodeArrayCreateExpression" /> that indicates the expression to generate code for.</param>
		protected abstract void GenerateArrayCreateExpression(CodeArrayCreateExpression e);

		/// <summary>Generates code for the specified base reference expression.</summary>
		/// <param name="e">A <see cref="T:System.CodeDom.CodeBaseReferenceExpression" /> that indicates the expression to generate code for.</param>
		protected abstract void GenerateBaseReferenceExpression(CodeBaseReferenceExpression e);

		/// <summary>Generates code for the specified binary operator expression.</summary>
		/// <param name="e">A <see cref="T:System.CodeDom.CodeBinaryOperatorExpression" /> that indicates the expression to generate code for.</param>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="e" /> is <see langword="null" />.</exception>
		protected virtual void GenerateBinaryOperatorExpression(CodeBinaryOperatorExpression e)
		{
			bool flag = false;
			this.Output.Write('(');
			this.GenerateExpression(e.Left);
			this.Output.Write(' ');
			if (e.Left is CodeBinaryOperatorExpression || e.Right is CodeBinaryOperatorExpression)
			{
				if (!this._inNestedBinary)
				{
					flag = true;
					this._inNestedBinary = true;
					this.Indent += 3;
				}
				this.ContinueOnNewLine("");
			}
			this.OutputOperator(e.Operator);
			this.Output.Write(' ');
			this.GenerateExpression(e.Right);
			this.Output.Write(')');
			if (flag)
			{
				this.Indent -= 3;
				this._inNestedBinary = false;
			}
		}

		/// <summary>Generates a line-continuation character and outputs the specified string on a new line.</summary>
		/// <param name="st">The string to write on the new line.</param>
		protected virtual void ContinueOnNewLine(string st)
		{
			this.Output.WriteLine(st);
		}

		/// <summary>Generates code for the specified cast expression.</summary>
		/// <param name="e">A <see cref="T:System.CodeDom.CodeCastExpression" /> that indicates the expression to generate code for.</param>
		protected abstract void GenerateCastExpression(CodeCastExpression e);

		/// <summary>Generates code for the specified delegate creation expression.</summary>
		/// <param name="e">The expression to generate code for.</param>
		protected abstract void GenerateDelegateCreateExpression(CodeDelegateCreateExpression e);

		/// <summary>Generates code for the specified field reference expression.</summary>
		/// <param name="e">The expression to generate code for.</param>
		protected abstract void GenerateFieldReferenceExpression(CodeFieldReferenceExpression e);

		/// <summary>Generates code for the specified argument reference expression.</summary>
		/// <param name="e">A <see cref="T:System.CodeDom.CodeArgumentReferenceExpression" /> that indicates the expression to generate code for.</param>
		protected abstract void GenerateArgumentReferenceExpression(CodeArgumentReferenceExpression e);

		/// <summary>Generates code for the specified variable reference expression.</summary>
		/// <param name="e">The expression to generate code for.</param>
		protected abstract void GenerateVariableReferenceExpression(CodeVariableReferenceExpression e);

		/// <summary>Generates code for the specified indexer expression.</summary>
		/// <param name="e">The expression to generate code for.</param>
		protected abstract void GenerateIndexerExpression(CodeIndexerExpression e);

		/// <summary>Generates code for the specified array indexer expression.</summary>
		/// <param name="e">A <see cref="T:System.CodeDom.CodeArrayIndexerExpression" /> that indicates the expression to generate code for.</param>
		protected abstract void GenerateArrayIndexerExpression(CodeArrayIndexerExpression e);

		/// <summary>Outputs the code of the specified literal code fragment expression.</summary>
		/// <param name="e">The expression to generate code for.</param>
		protected abstract void GenerateSnippetExpression(CodeSnippetExpression e);

		/// <summary>Generates code for the specified method invoke expression.</summary>
		/// <param name="e">The expression to generate code for.</param>
		protected abstract void GenerateMethodInvokeExpression(CodeMethodInvokeExpression e);

		/// <summary>Generates code for the specified method reference expression.</summary>
		/// <param name="e">The expression to generate code for.</param>
		protected abstract void GenerateMethodReferenceExpression(CodeMethodReferenceExpression e);

		/// <summary>Generates code for the specified event reference expression.</summary>
		/// <param name="e">The expression to generate code for.</param>
		protected abstract void GenerateEventReferenceExpression(CodeEventReferenceExpression e);

		/// <summary>Generates code for the specified delegate invoke expression.</summary>
		/// <param name="e">The expression to generate code for.</param>
		protected abstract void GenerateDelegateInvokeExpression(CodeDelegateInvokeExpression e);

		/// <summary>Generates code for the specified object creation expression.</summary>
		/// <param name="e">The expression to generate code for.</param>
		protected abstract void GenerateObjectCreateExpression(CodeObjectCreateExpression e);

		/// <summary>Generates code for the specified parameter declaration expression.</summary>
		/// <param name="e">The expression to generate code for.</param>
		protected virtual void GenerateParameterDeclarationExpression(CodeParameterDeclarationExpression e)
		{
			if (e.CustomAttributes.Count > 0)
			{
				this.OutputAttributeDeclarations(e.CustomAttributes);
				this.Output.Write(' ');
			}
			this.OutputDirection(e.Direction);
			this.OutputTypeNamePair(e.Type, e.Name);
		}

		/// <summary>Generates code for the specified direction expression.</summary>
		/// <param name="e">The expression to generate code for.</param>
		protected virtual void GenerateDirectionExpression(CodeDirectionExpression e)
		{
			this.OutputDirection(e.Direction);
			this.GenerateExpression(e.Expression);
		}

		/// <summary>Generates code for the specified primitive expression.</summary>
		/// <param name="e">The expression to generate code for.</param>
		/// <exception cref="T:System.ArgumentException">
		///   <paramref name="e" /> uses an invalid data type. Only the following data types are valid:  
		///
		/// string  
		///
		/// char  
		///
		/// byte  
		///
		/// Int16  
		///
		/// Int32  
		///
		/// Int64  
		///
		/// Single  
		///
		/// Double  
		///
		/// Decimal</exception>
		protected virtual void GeneratePrimitiveExpression(CodePrimitiveExpression e)
		{
			if (e.Value == null)
			{
				this.Output.Write(this.NullToken);
				return;
			}
			if (e.Value is string)
			{
				this.Output.Write(this.QuoteSnippetString((string)e.Value));
				return;
			}
			if (e.Value is char)
			{
				this.Output.Write("'" + e.Value.ToString() + "'");
				return;
			}
			if (e.Value is byte)
			{
				this.Output.Write(((byte)e.Value).ToString(CultureInfo.InvariantCulture));
				return;
			}
			if (e.Value is short)
			{
				this.Output.Write(((short)e.Value).ToString(CultureInfo.InvariantCulture));
				return;
			}
			if (e.Value is int)
			{
				this.Output.Write(((int)e.Value).ToString(CultureInfo.InvariantCulture));
				return;
			}
			if (e.Value is long)
			{
				this.Output.Write(((long)e.Value).ToString(CultureInfo.InvariantCulture));
				return;
			}
			if (e.Value is float)
			{
				this.GenerateSingleFloatValue((float)e.Value);
				return;
			}
			if (e.Value is double)
			{
				this.GenerateDoubleValue((double)e.Value);
				return;
			}
			if (e.Value is decimal)
			{
				this.GenerateDecimalValue((decimal)e.Value);
				return;
			}
			if (!(e.Value is bool))
			{
				throw new ArgumentException(SR.Format("Invalid Primitive Type: {0}. Consider using CodeObjectCreateExpression.", e.Value.GetType().ToString()));
			}
			if ((bool)e.Value)
			{
				this.Output.Write("true");
				return;
			}
			this.Output.Write("false");
		}

		/// <summary>Generates code for a single-precision floating point number.</summary>
		/// <param name="s">The value to generate code for.</param>
		protected virtual void GenerateSingleFloatValue(float s)
		{
			this.Output.Write(s.ToString("R", CultureInfo.InvariantCulture));
		}

		/// <summary>Generates code for a double-precision floating point number.</summary>
		/// <param name="d">The value to generate code for.</param>
		protected virtual void GenerateDoubleValue(double d)
		{
			this.Output.Write(d.ToString("R", CultureInfo.InvariantCulture));
		}

		/// <summary>Generates code for the specified decimal value.</summary>
		/// <param name="d">The decimal value to generate code for.</param>
		protected virtual void GenerateDecimalValue(decimal d)
		{
			this.Output.Write(d.ToString(CultureInfo.InvariantCulture));
		}

		/// <summary>Generates code for the specified reference to a default value.</summary>
		/// <param name="e">The reference to generate code for.</param>
		protected virtual void GenerateDefaultValueExpression(CodeDefaultValueExpression e)
		{
		}

		/// <summary>Generates code for the specified property reference expression.</summary>
		/// <param name="e">The expression to generate code for.</param>
		protected abstract void GeneratePropertyReferenceExpression(CodePropertyReferenceExpression e);

		/// <summary>Generates code for the specified property set value reference expression.</summary>
		/// <param name="e">The expression to generate code for.</param>
		protected abstract void GeneratePropertySetValueReferenceExpression(CodePropertySetValueReferenceExpression e);

		/// <summary>Generates code for the specified this reference expression.</summary>
		/// <param name="e">The expression to generate code for.</param>
		protected abstract void GenerateThisReferenceExpression(CodeThisReferenceExpression e);

		/// <summary>Generates code for the specified type reference expression.</summary>
		/// <param name="e">The expression to generate code for.</param>
		protected virtual void GenerateTypeReferenceExpression(CodeTypeReferenceExpression e)
		{
			this.OutputType(e.Type);
		}

		/// <summary>Generates code for the specified type of expression.</summary>
		/// <param name="e">The expression to generate code for.</param>
		protected virtual void GenerateTypeOfExpression(CodeTypeOfExpression e)
		{
			this.Output.Write("typeof(");
			this.OutputType(e.Type);
			this.Output.Write(')');
		}

		/// <summary>Generates code for the specified expression statement.</summary>
		/// <param name="e">The statement to generate code for.</param>
		protected abstract void GenerateExpressionStatement(CodeExpressionStatement e);

		/// <summary>Generates code for the specified iteration statement.</summary>
		/// <param name="e">The statement to generate code for.</param>
		protected abstract void GenerateIterationStatement(CodeIterationStatement e);

		/// <summary>Generates code for the specified throw exception statement.</summary>
		/// <param name="e">The statement to generate code for.</param>
		protected abstract void GenerateThrowExceptionStatement(CodeThrowExceptionStatement e);

		/// <summary>Generates code for the specified comment statement.</summary>
		/// <param name="e">The statement to generate code for.</param>
		/// <exception cref="T:System.ArgumentException">The <see cref="P:System.CodeDom.CodeCommentStatement.Comment" /> property of <paramref name="e" /> is not set.</exception>
		protected virtual void GenerateCommentStatement(CodeCommentStatement e)
		{
			if (e.Comment == null)
			{
				throw new ArgumentException(SR.Format("The 'Comment' property of the CodeCommentStatement '{0}' cannot be null.", "e"), "e");
			}
			this.GenerateComment(e.Comment);
		}

		/// <summary>Generates code for the specified comment statements.</summary>
		/// <param name="e">The expression to generate code for.</param>
		protected virtual void GenerateCommentStatements(CodeCommentStatementCollection e)
		{
			foreach (object obj in e)
			{
				CodeCommentStatement e2 = (CodeCommentStatement)obj;
				this.GenerateCommentStatement(e2);
			}
		}

		/// <summary>Generates code for the specified comment.</summary>
		/// <param name="e">A <see cref="T:System.CodeDom.CodeComment" /> to generate code for.</param>
		protected abstract void GenerateComment(CodeComment e);

		/// <summary>Generates code for the specified method return statement.</summary>
		/// <param name="e">The statement to generate code for.</param>
		protected abstract void GenerateMethodReturnStatement(CodeMethodReturnStatement e);

		/// <summary>Generates code for the specified conditional statement.</summary>
		/// <param name="e">The statement to generate code for.</param>
		protected abstract void GenerateConditionStatement(CodeConditionStatement e);

		/// <summary>Generates code for the specified <see langword="try...catch...finally" /> statement.</summary>
		/// <param name="e">The statement to generate code for.</param>
		protected abstract void GenerateTryCatchFinallyStatement(CodeTryCatchFinallyStatement e);

		/// <summary>Generates code for the specified assignment statement.</summary>
		/// <param name="e">A <see cref="T:System.CodeDom.CodeAssignStatement" /> that indicates the statement to generate code for.</param>
		protected abstract void GenerateAssignStatement(CodeAssignStatement e);

		/// <summary>Generates code for the specified attach event statement.</summary>
		/// <param name="e">A <see cref="T:System.CodeDom.CodeAttachEventStatement" /> that indicates the statement to generate code for.</param>
		protected abstract void GenerateAttachEventStatement(CodeAttachEventStatement e);

		/// <summary>Generates code for the specified remove event statement.</summary>
		/// <param name="e">The statement to generate code for.</param>
		protected abstract void GenerateRemoveEventStatement(CodeRemoveEventStatement e);

		/// <summary>Generates code for the specified <see langword="goto" /> statement.</summary>
		/// <param name="e">The expression to generate code for.</param>
		protected abstract void GenerateGotoStatement(CodeGotoStatement e);

		/// <summary>Generates code for the specified labeled statement.</summary>
		/// <param name="e">The statement to generate code for.</param>
		protected abstract void GenerateLabeledStatement(CodeLabeledStatement e);

		/// <summary>Outputs the code of the specified literal code fragment statement.</summary>
		/// <param name="e">The statement to generate code for.</param>
		protected virtual void GenerateSnippetStatement(CodeSnippetStatement e)
		{
			this.Output.WriteLine(e.Value);
		}

		/// <summary>Generates code for the specified variable declaration statement.</summary>
		/// <param name="e">The statement to generate code for.</param>
		protected abstract void GenerateVariableDeclarationStatement(CodeVariableDeclarationStatement e);

		/// <summary>Generates code for the specified line pragma start.</summary>
		/// <param name="e">The start of the line pragma to generate code for.</param>
		protected abstract void GenerateLinePragmaStart(CodeLinePragma e);

		/// <summary>Generates code for the specified line pragma end.</summary>
		/// <param name="e">The end of the line pragma to generate code for.</param>
		protected abstract void GenerateLinePragmaEnd(CodeLinePragma e);

		/// <summary>Generates code for the specified event.</summary>
		/// <param name="e">The member event to generate code for.</param>
		/// <param name="c">The type of the object that this event occurs on.</param>
		protected abstract void GenerateEvent(CodeMemberEvent e, CodeTypeDeclaration c);

		/// <summary>Generates code for the specified member field.</summary>
		/// <param name="e">The field to generate code for.</param>
		protected abstract void GenerateField(CodeMemberField e);

		/// <summary>Outputs the code of the specified literal code fragment class member.</summary>
		/// <param name="e">The member to generate code for.</param>
		protected abstract void GenerateSnippetMember(CodeSnippetTypeMember e);

		/// <summary>Generates code for the specified entry point method.</summary>
		/// <param name="e">The entry point for the code.</param>
		/// <param name="c">The code that declares the type.</param>
		protected abstract void GenerateEntryPointMethod(CodeEntryPointMethod e, CodeTypeDeclaration c);

		/// <summary>Generates code for the specified method.</summary>
		/// <param name="e">The member method to generate code for.</param>
		/// <param name="c">The type of the object that this method occurs on.</param>
		protected abstract void GenerateMethod(CodeMemberMethod e, CodeTypeDeclaration c);

		/// <summary>Generates code for the specified property.</summary>
		/// <param name="e">The property to generate code for.</param>
		/// <param name="c">The type of the object that this property occurs on.</param>
		protected abstract void GenerateProperty(CodeMemberProperty e, CodeTypeDeclaration c);

		/// <summary>Generates code for the specified constructor.</summary>
		/// <param name="e">The constructor to generate code for.</param>
		/// <param name="c">The type of the object that this constructor constructs.</param>
		protected abstract void GenerateConstructor(CodeConstructor e, CodeTypeDeclaration c);

		/// <summary>Generates code for the specified class constructor.</summary>
		/// <param name="e">The class constructor to generate code for.</param>
		protected abstract void GenerateTypeConstructor(CodeTypeConstructor e);

		/// <summary>Generates code for the specified start of the class.</summary>
		/// <param name="e">The start of the class to generate code for.</param>
		protected abstract void GenerateTypeStart(CodeTypeDeclaration e);

		/// <summary>Generates code for the specified end of the class.</summary>
		/// <param name="e">The end of the class to generate code for.</param>
		protected abstract void GenerateTypeEnd(CodeTypeDeclaration e);

		/// <summary>Generates code for the start of a compile unit.</summary>
		/// <param name="e">The compile unit to generate code for.</param>
		protected virtual void GenerateCompileUnitStart(CodeCompileUnit e)
		{
			if (e.StartDirectives.Count > 0)
			{
				this.GenerateDirectives(e.StartDirectives);
			}
		}

		/// <summary>Generates code for the end of a compile unit.</summary>
		/// <param name="e">The compile unit to generate code for.</param>
		protected virtual void GenerateCompileUnitEnd(CodeCompileUnit e)
		{
			if (e.EndDirectives.Count > 0)
			{
				this.GenerateDirectives(e.EndDirectives);
			}
		}

		/// <summary>Generates code for the start of a namespace.</summary>
		/// <param name="e">The namespace to generate code for.</param>
		protected abstract void GenerateNamespaceStart(CodeNamespace e);

		/// <summary>Generates code for the end of a namespace.</summary>
		/// <param name="e">The namespace to generate code for.</param>
		protected abstract void GenerateNamespaceEnd(CodeNamespace e);

		/// <summary>Generates code for the specified namespace import.</summary>
		/// <param name="e">The namespace import to generate code for.</param>
		protected abstract void GenerateNamespaceImport(CodeNamespaceImport e);

		/// <summary>Generates code for the specified attribute block start.</summary>
		/// <param name="attributes">A <see cref="T:System.CodeDom.CodeAttributeDeclarationCollection" /> that indicates the start of the attribute block to generate code for.</param>
		protected abstract void GenerateAttributeDeclarationsStart(CodeAttributeDeclarationCollection attributes);

		/// <summary>Generates code for the specified attribute block end.</summary>
		/// <param name="attributes">A <see cref="T:System.CodeDom.CodeAttributeDeclarationCollection" /> that indicates the end of the attribute block to generate code for.</param>
		protected abstract void GenerateAttributeDeclarationsEnd(CodeAttributeDeclarationCollection attributes);

		/// <summary>Gets a value indicating whether the specified code generation support is provided.</summary>
		/// <param name="support">The type of code generation support to test for.</param>
		/// <returns>
		///   <see langword="true" /> if the specified code generation support is provided; otherwise, <see langword="false" />.</returns>
		protected abstract bool Supports(GeneratorSupport support);

		/// <summary>Gets a value indicating whether the specified value is a valid identifier.</summary>
		/// <param name="value">The value to test for conflicts with valid identifiers.</param>
		/// <returns>
		///   <see langword="true" /> if the value is a valid identifier; otherwise, <see langword="false" />.</returns>
		protected abstract bool IsValidIdentifier(string value);

		/// <summary>Throws an exception if the specified string is not a valid identifier.</summary>
		/// <param name="value">The identifier to test for validity as an identifier.</param>
		/// <exception cref="T:System.ArgumentException">If the specified identifier is invalid or conflicts with reserved or language keywords.</exception>
		protected virtual void ValidateIdentifier(string value)
		{
			if (!this.IsValidIdentifier(value))
			{
				throw new ArgumentException(SR.Format("Identifier '{0}' is not valid.", value));
			}
		}

		/// <summary>Creates an escaped identifier for the specified value.</summary>
		/// <param name="value">The string to create an escaped identifier for.</param>
		/// <returns>The escaped identifier for the value.</returns>
		protected abstract string CreateEscapedIdentifier(string value);

		/// <summary>Creates a valid identifier for the specified value.</summary>
		/// <param name="value">A string to create a valid identifier for.</param>
		/// <returns>A valid identifier for the value.</returns>
		protected abstract string CreateValidIdentifier(string value);

		/// <summary>Gets the name of the specified data type.</summary>
		/// <param name="value">The type whose name will be returned.</param>
		/// <returns>The name of the data type reference.</returns>
		protected abstract string GetTypeOutput(CodeTypeReference value);

		/// <summary>Converts the specified string by formatting it with escape codes.</summary>
		/// <param name="value">The string to convert.</param>
		/// <returns>The converted string.</returns>
		protected abstract string QuoteSnippetString(string value);

		/// <summary>Gets a value indicating whether the specified string is a valid identifier.</summary>
		/// <param name="value">The string to test for validity.</param>
		/// <returns>
		///   <see langword="true" /> if the specified string is a valid identifier; otherwise, <see langword="false" />.</returns>
		public static bool IsValidLanguageIndependentIdentifier(string value)
		{
			return CSharpHelpers.IsValidTypeNameOrIdentifier(value, false);
		}

		internal static bool IsValidLanguageIndependentTypeName(string value)
		{
			return CSharpHelpers.IsValidTypeNameOrIdentifier(value, true);
		}

		/// <summary>Attempts to validate each identifier field contained in the specified <see cref="T:System.CodeDom.CodeObject" /> or <see cref="N:System.CodeDom" /> tree.</summary>
		/// <param name="e">An object to test for invalid identifiers.</param>
		/// <exception cref="T:System.ArgumentException">The specified <see cref="T:System.CodeDom.CodeObject" /> contains an invalid identifier.</exception>
		public static void ValidateIdentifiers(CodeObject e)
		{
			new CodeValidator().ValidateIdentifiers(e);
		}

		private const int ParameterMultilineThreshold = 15;

		private ExposedTabStringIndentedTextWriter _output;

		private CodeGeneratorOptions _options;

		private CodeTypeDeclaration _currentClass;

		private CodeTypeMember _currentMember;

		private bool _inNestedBinary;
	}
}
