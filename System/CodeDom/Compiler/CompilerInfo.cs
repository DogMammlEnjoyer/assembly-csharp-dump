using System;
using System.Collections.Generic;
using System.Reflection;

namespace System.CodeDom.Compiler
{
	/// <summary>Represents the configuration settings of a language provider. This class cannot be inherited.</summary>
	public sealed class CompilerInfo
	{
		private CompilerInfo()
		{
		}

		/// <summary>Gets the language names supported by the language provider.</summary>
		/// <returns>An array of language names supported by the language provider.</returns>
		public string[] GetLanguages()
		{
			return this.CloneCompilerLanguages();
		}

		/// <summary>Returns the file name extensions supported by the language provider.</summary>
		/// <returns>An array of file name extensions supported by the language provider.</returns>
		public string[] GetExtensions()
		{
			return this.CloneCompilerExtensions();
		}

		/// <summary>Gets the type of the configured <see cref="T:System.CodeDom.Compiler.CodeDomProvider" /> implementation.</summary>
		/// <returns>A read-only <see cref="T:System.Type" /> instance that represents the configured language provider type.</returns>
		/// <exception cref="T:System.Configuration.ConfigurationException">The language provider is not configured on this computer.</exception>
		/// <exception cref="T:System.Configuration.ConfigurationErrorsException">Cannot locate the type because it is a <see langword="null" /> or empty string.  
		///  -or-  
		///  Cannot locate the type because the name for the <see cref="T:System.CodeDom.Compiler.CodeDomProvider" /> cannot be found in the configuration file.</exception>
		public Type CodeDomProviderType
		{
			get
			{
				if (this._type == null)
				{
					lock (this)
					{
						if (this._type == null)
						{
							this._type = Type.GetType(this._codeDomProviderTypeName);
						}
					}
				}
				return this._type;
			}
		}

		/// <summary>Returns a value indicating whether the language provider implementation is configured on the computer.</summary>
		/// <returns>
		///   <see langword="true" /> if the language provider implementation type is configured on the computer; otherwise, <see langword="false" />.</returns>
		public bool IsCodeDomProviderTypeValid
		{
			get
			{
				return Type.GetType(this._codeDomProviderTypeName) != null;
			}
		}

		/// <summary>Returns a <see cref="T:System.CodeDom.Compiler.CodeDomProvider" /> instance for the current language provider settings.</summary>
		/// <returns>A CodeDOM provider associated with the language provider configuration.</returns>
		public CodeDomProvider CreateProvider()
		{
			if (this._providerOptions.Count > 0)
			{
				ConstructorInfo constructor = this.CodeDomProviderType.GetConstructor(new Type[]
				{
					typeof(IDictionary<string, string>)
				});
				if (constructor != null)
				{
					return (CodeDomProvider)constructor.Invoke(new object[]
					{
						this._providerOptions
					});
				}
			}
			return (CodeDomProvider)Activator.CreateInstance(this.CodeDomProviderType);
		}

		/// <summary>Returns a <see cref="T:System.CodeDom.Compiler.CodeDomProvider" /> instance for the current language provider settings and specified options.</summary>
		/// <param name="providerOptions">A collection of provider options from the configuration file.</param>
		/// <returns>A CodeDOM provider associated with the language provider configuration and specified options.</returns>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="providerOptions" /> is <see langword="null" />.</exception>
		/// <exception cref="T:System.InvalidOperationException">The provider does not support options.</exception>
		public CodeDomProvider CreateProvider(IDictionary<string, string> providerOptions)
		{
			if (providerOptions == null)
			{
				throw new ArgumentNullException("providerOptions");
			}
			ConstructorInfo constructor = this.CodeDomProviderType.GetConstructor(new Type[]
			{
				typeof(IDictionary<string, string>)
			});
			if (constructor != null)
			{
				return (CodeDomProvider)constructor.Invoke(new object[]
				{
					providerOptions
				});
			}
			throw new InvalidOperationException(SR.Format("This CodeDomProvider type does not have a constructor that takes providerOptions - \"{0}\"", this.CodeDomProviderType.ToString()));
		}

		/// <summary>Gets the configured compiler settings for the language provider implementation.</summary>
		/// <returns>A read-only <see cref="T:System.CodeDom.Compiler.CompilerParameters" /> instance that contains the compiler options and settings configured for the language provider.</returns>
		public CompilerParameters CreateDefaultCompilerParameters()
		{
			return this.CloneCompilerParameters();
		}

		internal CompilerInfo(CompilerParameters compilerParams, string codeDomProviderTypeName, string[] compilerLanguages, string[] compilerExtensions)
		{
			this._compilerLanguages = compilerLanguages;
			this._compilerExtensions = compilerExtensions;
			this._codeDomProviderTypeName = codeDomProviderTypeName;
			this._compilerParams = (compilerParams ?? new CompilerParameters());
		}

		internal CompilerInfo(CompilerParameters compilerParams, string codeDomProviderTypeName)
		{
			this._codeDomProviderTypeName = codeDomProviderTypeName;
			this._compilerParams = (compilerParams ?? new CompilerParameters());
		}

		/// <summary>Returns the hash code for the current instance.</summary>
		/// <returns>A 32-bit signed integer hash code for the current <see cref="T:System.CodeDom.Compiler.CompilerInfo" /> instance, suitable for use in hashing algorithms and data structures such as a hash table.</returns>
		public override int GetHashCode()
		{
			return this._codeDomProviderTypeName.GetHashCode();
		}

		/// <summary>Determines whether the specified object represents the same language provider and compiler settings as the current <see cref="T:System.CodeDom.Compiler.CompilerInfo" />.</summary>
		/// <param name="o">The object to compare with the current <see cref="T:System.CodeDom.Compiler.CompilerInfo" />.</param>
		/// <returns>
		///   <see langword="true" /> if <paramref name="o" /> is a <see cref="T:System.CodeDom.Compiler.CompilerInfo" /> object and its value is the same as this instance; otherwise, <see langword="false" />.</returns>
		public override bool Equals(object o)
		{
			CompilerInfo compilerInfo = o as CompilerInfo;
			return compilerInfo != null && (this.CodeDomProviderType == compilerInfo.CodeDomProviderType && this.CompilerParams.WarningLevel == compilerInfo.CompilerParams.WarningLevel && this.CompilerParams.IncludeDebugInformation == compilerInfo.CompilerParams.IncludeDebugInformation) && this.CompilerParams.CompilerOptions == compilerInfo.CompilerParams.CompilerOptions;
		}

		private CompilerParameters CloneCompilerParameters()
		{
			return new CompilerParameters
			{
				IncludeDebugInformation = this._compilerParams.IncludeDebugInformation,
				TreatWarningsAsErrors = this._compilerParams.TreatWarningsAsErrors,
				WarningLevel = this._compilerParams.WarningLevel,
				CompilerOptions = this._compilerParams.CompilerOptions
			};
		}

		private string[] CloneCompilerLanguages()
		{
			return (string[])this._compilerLanguages.Clone();
		}

		private string[] CloneCompilerExtensions()
		{
			return (string[])this._compilerExtensions.Clone();
		}

		internal CompilerParameters CompilerParams
		{
			get
			{
				return this._compilerParams;
			}
		}

		internal IDictionary<string, string> ProviderOptions
		{
			get
			{
				return this._providerOptions;
			}
		}

		internal readonly IDictionary<string, string> _providerOptions = new Dictionary<string, string>();

		internal string _codeDomProviderTypeName;

		internal CompilerParameters _compilerParams;

		internal string[] _compilerLanguages;

		internal string[] _compilerExtensions;

		private Type _type;
	}
}
