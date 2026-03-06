using System;
using System.Configuration;

namespace System.CodeDom.Compiler
{
	internal sealed class CodeDomConfigurationHandler : ConfigurationSection
	{
		static CodeDomConfigurationHandler()
		{
			CodeDomConfigurationHandler.compilersProp = new ConfigurationProperty("compilers", typeof(CompilerCollection), CodeDomConfigurationHandler.default_compilers);
			CodeDomConfigurationHandler.properties = new ConfigurationPropertyCollection();
			CodeDomConfigurationHandler.properties.Add(CodeDomConfigurationHandler.compilersProp);
		}

		protected override void InitializeDefault()
		{
			CodeDomConfigurationHandler.compilersProp = new ConfigurationProperty("compilers", typeof(CompilerCollection), CodeDomConfigurationHandler.default_compilers);
		}

		[MonoTODO]
		protected override void PostDeserialize()
		{
			base.PostDeserialize();
		}

		protected override object GetRuntimeObject()
		{
			return this;
		}

		[ConfigurationProperty("compilers")]
		public CompilerCollection Compilers
		{
			get
			{
				return (CompilerCollection)base[CodeDomConfigurationHandler.compilersProp];
			}
		}

		public CompilerInfo[] CompilerInfos
		{
			get
			{
				CompilerCollection compilerCollection = (CompilerCollection)base[CodeDomConfigurationHandler.compilersProp];
				if (compilerCollection == null)
				{
					return null;
				}
				return compilerCollection.CompilerInfos;
			}
		}

		protected override ConfigurationPropertyCollection Properties
		{
			get
			{
				return CodeDomConfigurationHandler.properties;
			}
		}

		private static ConfigurationPropertyCollection properties;

		private static ConfigurationProperty compilersProp;

		private static CompilerCollection default_compilers = new CompilerCollection();
	}
}
