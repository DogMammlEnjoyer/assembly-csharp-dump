using System;
using System.Collections;
using System.Collections.Specialized;

namespace System.CodeDom.Compiler
{
	/// <summary>Represents a set of options used by a code generator.</summary>
	public class CodeGeneratorOptions
	{
		/// <summary>Gets or sets the object at the specified index.</summary>
		/// <param name="index">The name associated with the object to retrieve.</param>
		/// <returns>The object associated with the specified name. If no object associated with the specified name exists in the collection, <see langword="null" />.</returns>
		public object this[string index]
		{
			get
			{
				return this._options[index];
			}
			set
			{
				this._options[index] = value;
			}
		}

		/// <summary>Gets or sets the string to use for indentations.</summary>
		/// <returns>A string containing the characters to use for indentations.</returns>
		public string IndentString
		{
			get
			{
				object obj = this._options["IndentString"];
				if (obj == null)
				{
					return "    ";
				}
				return (string)obj;
			}
			set
			{
				this._options["IndentString"] = value;
			}
		}

		/// <summary>Gets or sets the style to use for bracing.</summary>
		/// <returns>A string containing the bracing style to use.</returns>
		public string BracingStyle
		{
			get
			{
				object obj = this._options["BracingStyle"];
				if (obj == null)
				{
					return "Block";
				}
				return (string)obj;
			}
			set
			{
				this._options["BracingStyle"] = value;
			}
		}

		/// <summary>Gets or sets a value indicating whether to append an <see langword="else" />, <see langword="catch" />, or <see langword="finally" /> block, including brackets, at the closing line of each previous <see langword="if" /> or <see langword="try" /> block.</summary>
		/// <returns>
		///   <see langword="true" /> if an else should be appended; otherwise, <see langword="false" />. The default value of this property is <see langword="false" />.</returns>
		public bool ElseOnClosing
		{
			get
			{
				object obj = this._options["ElseOnClosing"];
				return obj != null && (bool)obj;
			}
			set
			{
				this._options["ElseOnClosing"] = value;
			}
		}

		/// <summary>Gets or sets a value indicating whether to insert blank lines between members.</summary>
		/// <returns>
		///   <see langword="true" /> if blank lines should be inserted; otherwise, <see langword="false" />. By default, the value of this property is <see langword="true" />.</returns>
		public bool BlankLinesBetweenMembers
		{
			get
			{
				object obj = this._options["BlankLinesBetweenMembers"];
				return obj == null || (bool)obj;
			}
			set
			{
				this._options["BlankLinesBetweenMembers"] = value;
			}
		}

		/// <summary>Gets or sets a value indicating whether to generate members in the order in which they occur in member collections.</summary>
		/// <returns>
		///   <see langword="true" /> to generate the members in the order in which they occur in the member collection; otherwise, <see langword="false" />. The default value of this property is <see langword="false" />.</returns>
		public bool VerbatimOrder
		{
			get
			{
				object obj = this._options["VerbatimOrder"];
				return obj != null && (bool)obj;
			}
			set
			{
				this._options["VerbatimOrder"] = value;
			}
		}

		private readonly IDictionary _options = new ListDictionary();
	}
}
