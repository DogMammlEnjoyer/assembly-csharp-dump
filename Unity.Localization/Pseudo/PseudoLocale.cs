using System;
using System.Collections.Generic;

namespace UnityEngine.Localization.Pseudo
{
	[CreateAssetMenu(menuName = "Localization/Pseudo-Locale", fileName = "Pseudo-Locale(pseudo)")]
	public class PseudoLocale : Locale
	{
		public List<IPseudoLocalizationMethod> Methods
		{
			get
			{
				return this.m_Methods;
			}
		}

		public static PseudoLocale CreatePseudoLocale()
		{
			PseudoLocale pseudoLocale = ScriptableObject.CreateInstance<PseudoLocale>();
			pseudoLocale.name = "PseudoLocale";
			return pseudoLocale;
		}

		private PseudoLocale()
		{
			base.Identifier = new LocaleIdentifier("en");
		}

		internal void Reset()
		{
			foreach (IPseudoLocalizationMethod pseudoLocalizationMethod in this.Methods)
			{
				CharacterSubstitutor characterSubstitutor = pseudoLocalizationMethod as CharacterSubstitutor;
				if (characterSubstitutor != null)
				{
					characterSubstitutor.m_ReplacementsPosition = 0;
				}
			}
		}

		public virtual string GetPseudoString(string input)
		{
			Message message = Message.CreateMessage(input);
			foreach (IPseudoLocalizationMethod pseudoLocalizationMethod in this.Methods)
			{
				if (pseudoLocalizationMethod != null)
				{
					pseudoLocalizationMethod.Transform(message);
				}
			}
			string result = message.ToString();
			message.Release();
			return result;
		}

		public override string ToString()
		{
			return "Pseudo (" + base.ToString() + ")";
		}

		[SerializeReference]
		private List<IPseudoLocalizationMethod> m_Methods = new List<IPseudoLocalizationMethod>
		{
			new PreserveTags(),
			new Expander(),
			new Accenter(),
			new Encapsulator()
		};
	}
}
