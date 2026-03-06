using System;
using System.Collections.Generic;

namespace UnityEngine.Localization.SmartFormat.Core.Settings
{
	[Serializable]
	public class SmartSettings
	{
		internal SmartSettings()
		{
			this.CaseSensitivity = CaseSensitivityType.CaseSensitive;
			this.ConvertCharacterStringLiterals = true;
			this.FormatErrorAction = ErrorAction.ThrowError;
			this.ParseErrorAction = ErrorAction.ThrowError;
		}

		public ErrorAction FormatErrorAction
		{
			get
			{
				return this.m_FormatErrorAction;
			}
			set
			{
				this.m_FormatErrorAction = value;
			}
		}

		public ErrorAction ParseErrorAction
		{
			get
			{
				return this.m_ParseErrorAction;
			}
			set
			{
				this.m_ParseErrorAction = value;
			}
		}

		public CaseSensitivityType CaseSensitivity
		{
			get
			{
				return this.m_CaseSensitivity;
			}
			set
			{
				this.m_CaseSensitivity = value;
			}
		}

		public bool ConvertCharacterStringLiterals
		{
			get
			{
				return this.m_ConvertCharacterStringLiterals;
			}
			set
			{
				this.m_ConvertCharacterStringLiterals = value;
			}
		}

		internal IEqualityComparer<string> GetCaseSensitivityComparer()
		{
			CaseSensitivityType caseSensitivity = this.CaseSensitivity;
			if (caseSensitivity == CaseSensitivityType.CaseSensitive)
			{
				return StringComparer.Ordinal;
			}
			if (caseSensitivity != CaseSensitivityType.CaseInsensitive)
			{
				throw new InvalidOperationException(string.Format("The case sensitivity type [{0}] is unknown.", this.CaseSensitivity));
			}
			return StringComparer.OrdinalIgnoreCase;
		}

		internal StringComparison GetCaseSensitivityComparison()
		{
			CaseSensitivityType caseSensitivity = this.CaseSensitivity;
			if (caseSensitivity == CaseSensitivityType.CaseSensitive)
			{
				return StringComparison.Ordinal;
			}
			if (caseSensitivity != CaseSensitivityType.CaseInsensitive)
			{
				throw new InvalidOperationException(string.Format("The case sensitivity type [{0}] is unknown.", this.CaseSensitivity));
			}
			return StringComparison.OrdinalIgnoreCase;
		}

		[SerializeField]
		private ErrorAction m_FormatErrorAction;

		[SerializeField]
		private ErrorAction m_ParseErrorAction;

		[Tooltip("Determines whether placeholders are case-sensitive or not.")]
		[SerializeField]
		private CaseSensitivityType m_CaseSensitivity;

		[Tooltip("This setting is relevant for the 'Parsing.LiteralText', If true (the default), character string literals are treated like in normal string.Format: string.Format(\"\t\") will return a \"TAB\" character If false, character string literals are not converted, just like with this string.Format: string.Format(@\"\t\") will return the 2 characters \"\" and \"t\"")]
		[SerializeField]
		private bool m_ConvertCharacterStringLiterals = true;
	}
}
