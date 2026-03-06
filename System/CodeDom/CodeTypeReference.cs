using System;
using System.Collections.Generic;
using System.Globalization;

namespace System.CodeDom
{
	/// <summary>Represents a reference to a type.</summary>
	[Serializable]
	public class CodeTypeReference : CodeObject
	{
		/// <summary>Initializes a new instance of the <see cref="T:System.CodeDom.CodeTypeReference" /> class.</summary>
		public CodeTypeReference()
		{
			this._baseType = string.Empty;
			this.ArrayRank = 0;
			this.ArrayElementType = null;
		}

		/// <summary>Initializes a new instance of the <see cref="T:System.CodeDom.CodeTypeReference" /> class using the specified type.</summary>
		/// <param name="type">The <see cref="T:System.Type" /> to reference.</param>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="type" /> is <see langword="null" />.</exception>
		public CodeTypeReference(Type type)
		{
			if (type == null)
			{
				throw new ArgumentNullException("type");
			}
			if (type.IsArray)
			{
				this.ArrayRank = type.GetArrayRank();
				this.ArrayElementType = new CodeTypeReference(type.GetElementType());
				this._baseType = null;
			}
			else
			{
				this.InitializeFromType(type);
				this.ArrayRank = 0;
				this.ArrayElementType = null;
			}
			this._isInterface = type.IsInterface;
		}

		/// <summary>Initializes a new instance of the <see cref="T:System.CodeDom.CodeTypeReference" /> class using the specified type and code type reference.</summary>
		/// <param name="type">The <see cref="T:System.Type" /> to reference.</param>
		/// <param name="codeTypeReferenceOption">The code type reference option, one of the <see cref="T:System.CodeDom.CodeTypeReferenceOptions" /> values.</param>
		public CodeTypeReference(Type type, CodeTypeReferenceOptions codeTypeReferenceOption) : this(type)
		{
			this.Options = codeTypeReferenceOption;
		}

		/// <summary>Initializes a new instance of the <see cref="T:System.CodeDom.CodeTypeReference" /> class using the specified type name and code type reference option.</summary>
		/// <param name="typeName">The name of the type to reference.</param>
		/// <param name="codeTypeReferenceOption">The code type reference option, one of the <see cref="T:System.CodeDom.CodeTypeReferenceOptions" /> values.</param>
		public CodeTypeReference(string typeName, CodeTypeReferenceOptions codeTypeReferenceOption)
		{
			this.Initialize(typeName, codeTypeReferenceOption);
		}

		/// <summary>Initializes a new instance of the <see cref="T:System.CodeDom.CodeTypeReference" /> class using the specified type name.</summary>
		/// <param name="typeName">The name of the type to reference.</param>
		public CodeTypeReference(string typeName)
		{
			this.Initialize(typeName);
		}

		private void InitializeFromType(Type type)
		{
			this._baseType = type.Name;
			if (!type.IsGenericParameter)
			{
				Type type2 = type;
				while (type2.IsNested)
				{
					type2 = type2.DeclaringType;
					this._baseType = type2.Name + "+" + this._baseType;
				}
				if (!string.IsNullOrEmpty(type.Namespace))
				{
					this._baseType = type.Namespace + "." + this._baseType;
				}
			}
			if (type.IsGenericType && !type.ContainsGenericParameters)
			{
				Type[] genericArguments = type.GetGenericArguments();
				for (int i = 0; i < genericArguments.Length; i++)
				{
					this.TypeArguments.Add(new CodeTypeReference(genericArguments[i]));
				}
				return;
			}
			if (!type.IsGenericTypeDefinition)
			{
				this._needsFixup = true;
			}
		}

		private void Initialize(string typeName)
		{
			this.Initialize(typeName, this.Options);
		}

		private void Initialize(string typeName, CodeTypeReferenceOptions options)
		{
			this.Options = options;
			if (string.IsNullOrEmpty(typeName))
			{
				typeName = typeof(void).FullName;
				this._baseType = typeName;
				this.ArrayRank = 0;
				this.ArrayElementType = null;
				return;
			}
			typeName = this.RipOffAssemblyInformationFromTypeName(typeName);
			int num = typeName.Length - 1;
			int i = num;
			this._needsFixup = true;
			Queue<int> queue = new Queue<int>();
			while (i >= 0)
			{
				int num2 = 1;
				if (typeName[i--] != ']')
				{
					break;
				}
				while (i >= 0 && typeName[i] == ',')
				{
					num2++;
					i--;
				}
				if (i < 0 || typeName[i] != '[')
				{
					break;
				}
				queue.Enqueue(num2);
				i--;
				num = i;
			}
			i = num;
			List<CodeTypeReference> list = new List<CodeTypeReference>();
			Stack<string> stack = new Stack<string>();
			if (i > 0 && typeName[i--] == ']')
			{
				this._needsFixup = false;
				int num3 = 1;
				int num4 = num;
				while (i >= 0)
				{
					if (typeName[i] == '[')
					{
						if (--num3 == 0)
						{
							break;
						}
					}
					else if (typeName[i] == ']')
					{
						num3++;
					}
					else if (typeName[i] == ',' && num3 == 1)
					{
						if (i + 1 < num4)
						{
							stack.Push(typeName.Substring(i + 1, num4 - i - 1));
						}
						num4 = i;
					}
					i--;
				}
				if (i > 0 && num - i - 1 > 0)
				{
					if (i + 1 < num4)
					{
						stack.Push(typeName.Substring(i + 1, num4 - i - 1));
					}
					while (stack.Count > 0)
					{
						string typeName2 = this.RipOffAssemblyInformationFromTypeName(stack.Pop());
						list.Add(new CodeTypeReference(typeName2));
					}
					num = i - 1;
				}
			}
			if (num < 0)
			{
				this._baseType = typeName;
				return;
			}
			if (queue.Count > 0)
			{
				CodeTypeReference codeTypeReference = new CodeTypeReference(typeName.Substring(0, num + 1), this.Options);
				for (int j = 0; j < list.Count; j++)
				{
					codeTypeReference.TypeArguments.Add(list[j]);
				}
				while (queue.Count > 1)
				{
					codeTypeReference = new CodeTypeReference(codeTypeReference, queue.Dequeue());
				}
				this._baseType = null;
				this.ArrayRank = queue.Dequeue();
				this.ArrayElementType = codeTypeReference;
			}
			else if (list.Count > 0)
			{
				for (int k = 0; k < list.Count; k++)
				{
					this.TypeArguments.Add(list[k]);
				}
				this._baseType = typeName.Substring(0, num + 1);
			}
			else
			{
				this._baseType = typeName;
			}
			if (this._baseType != null && this._baseType.IndexOf('`') != -1)
			{
				this._needsFixup = false;
			}
		}

		/// <summary>Initializes a new instance of the <see cref="T:System.CodeDom.CodeTypeReference" /> class using the specified type name and type arguments.</summary>
		/// <param name="typeName">The name of the type to reference.</param>
		/// <param name="typeArguments">An array of <see cref="T:System.CodeDom.CodeTypeReference" /> values.</param>
		public CodeTypeReference(string typeName, params CodeTypeReference[] typeArguments) : this(typeName)
		{
			if (typeArguments != null && typeArguments.Length != 0)
			{
				this.TypeArguments.AddRange(typeArguments);
			}
		}

		/// <summary>Initializes a new instance of the <see cref="T:System.CodeDom.CodeTypeReference" /> class using the specified code type parameter.</summary>
		/// <param name="typeParameter">A <see cref="T:System.CodeDom.CodeTypeParameter" /> that represents the type of the type parameter.</param>
		public CodeTypeReference(CodeTypeParameter typeParameter) : this((typeParameter != null) ? typeParameter.Name : null)
		{
			this.Options = CodeTypeReferenceOptions.GenericTypeParameter;
		}

		/// <summary>Initializes a new instance of the <see cref="T:System.CodeDom.CodeTypeReference" /> class using the specified array type name and rank.</summary>
		/// <param name="baseType">The name of the type of the elements of the array.</param>
		/// <param name="rank">The number of dimensions of the array.</param>
		public CodeTypeReference(string baseType, int rank)
		{
			this._baseType = null;
			this.ArrayRank = rank;
			this.ArrayElementType = new CodeTypeReference(baseType);
		}

		/// <summary>Initializes a new instance of the <see cref="T:System.CodeDom.CodeTypeReference" /> class using the specified array type and rank.</summary>
		/// <param name="arrayType">A <see cref="T:System.CodeDom.CodeTypeReference" /> that indicates the type of the array.</param>
		/// <param name="rank">The number of dimensions in the array.</param>
		public CodeTypeReference(CodeTypeReference arrayType, int rank)
		{
			this._baseType = null;
			this.ArrayRank = rank;
			this.ArrayElementType = arrayType;
		}

		/// <summary>Gets or sets the type of the elements in the array.</summary>
		/// <returns>A <see cref="T:System.CodeDom.CodeTypeReference" /> that indicates the type of the array elements.</returns>
		public CodeTypeReference ArrayElementType { get; set; }

		/// <summary>Gets or sets the array rank of the array.</summary>
		/// <returns>The number of dimensions of the array.</returns>
		public int ArrayRank { get; set; }

		internal int NestedArrayDepth
		{
			get
			{
				if (this.ArrayElementType != null)
				{
					return 1 + this.ArrayElementType.NestedArrayDepth;
				}
				return 0;
			}
		}

		/// <summary>Gets or sets the name of the type being referenced.</summary>
		/// <returns>The name of the type being referenced.</returns>
		public string BaseType
		{
			get
			{
				if (this.ArrayRank > 0 && this.ArrayElementType != null)
				{
					return this.ArrayElementType.BaseType;
				}
				if (string.IsNullOrEmpty(this._baseType))
				{
					return string.Empty;
				}
				string baseType = this._baseType;
				if (!this._needsFixup || this.TypeArguments.Count <= 0)
				{
					return baseType;
				}
				return baseType + "`" + this.TypeArguments.Count.ToString(CultureInfo.InvariantCulture);
			}
			set
			{
				this._baseType = value;
				this.Initialize(this._baseType);
			}
		}

		/// <summary>Gets or sets the code type reference option.</summary>
		/// <returns>A bitwise combination of the <see cref="T:System.CodeDom.CodeTypeReferenceOptions" /> values.</returns>
		public CodeTypeReferenceOptions Options { get; set; }

		/// <summary>Gets the type arguments for the current generic type reference.</summary>
		/// <returns>A <see cref="T:System.CodeDom.CodeTypeReferenceCollection" /> containing the type arguments for the current <see cref="T:System.CodeDom.CodeTypeReference" /> object.</returns>
		public CodeTypeReferenceCollection TypeArguments
		{
			get
			{
				if (this.ArrayRank > 0 && this.ArrayElementType != null)
				{
					return this.ArrayElementType.TypeArguments;
				}
				if (this._typeArguments == null)
				{
					this._typeArguments = new CodeTypeReferenceCollection();
				}
				return this._typeArguments;
			}
		}

		internal bool IsInterface
		{
			get
			{
				return this._isInterface;
			}
		}

		private string RipOffAssemblyInformationFromTypeName(string typeName)
		{
			int i = 0;
			int num = typeName.Length - 1;
			string result = typeName;
			while (i < typeName.Length)
			{
				if (!char.IsWhiteSpace(typeName[i]))
				{
					break;
				}
				i++;
			}
			while (num >= 0 && char.IsWhiteSpace(typeName[num]))
			{
				num--;
			}
			if (i < num)
			{
				if (typeName[i] == '[' && typeName[num] == ']')
				{
					i++;
					num--;
				}
				if (typeName[num] != ']')
				{
					int num2 = 0;
					for (int j = num; j >= i; j--)
					{
						if (typeName[j] == ',')
						{
							num2++;
							if (num2 == 4)
							{
								result = typeName.Substring(i, j - i);
								break;
							}
						}
					}
				}
			}
			return result;
		}

		private string _baseType;

		private readonly bool _isInterface;

		private CodeTypeReferenceCollection _typeArguments;

		private bool _needsFixup;
	}
}
