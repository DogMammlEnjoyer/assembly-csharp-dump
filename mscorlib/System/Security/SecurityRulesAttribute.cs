using System;

namespace System.Security
{
	/// <summary>Indicates the set of security rules the common language runtime should enforce for an assembly.</summary>
	[AttributeUsage(AttributeTargets.Assembly, AllowMultiple = false)]
	public sealed class SecurityRulesAttribute : Attribute
	{
		/// <summary>Initializes a new instance of the <see cref="T:System.Security.SecurityRulesAttribute" /> class using the specified rule set value.</summary>
		/// <param name="ruleSet">One of the enumeration values that specifies the transparency rules set.</param>
		public SecurityRulesAttribute(SecurityRuleSet ruleSet)
		{
			this.m_ruleSet = ruleSet;
		}

		/// <summary>Determines whether fully trusted transparent code should skip Microsoft intermediate language (MSIL) verification.</summary>
		/// <returns>
		///   <see langword="true" /> if MSIL verification should be skipped; otherwise, <see langword="false" />. The default is <see langword="false" />.</returns>
		public bool SkipVerificationInFullTrust
		{
			get
			{
				return this.m_skipVerificationInFullTrust;
			}
			set
			{
				this.m_skipVerificationInFullTrust = value;
			}
		}

		/// <summary>Gets the rule set to be applied.</summary>
		/// <returns>One of the enumeration values that specifies the transparency rules to be applied.</returns>
		public SecurityRuleSet RuleSet
		{
			get
			{
				return this.m_ruleSet;
			}
		}

		private SecurityRuleSet m_ruleSet;

		private bool m_skipVerificationInFullTrust;
	}
}
