using System;
using System.Collections.Generic;

namespace UnityEngine.Localization.Pseudo
{
	[Serializable]
	public class CharacterSubstitutor : IPseudoLocalizationMethod, ISerializationCallbackReceiver
	{
		public CharacterSubstitutor.SubstitutionMethod Method
		{
			get
			{
				return this.m_SubstitutionMethod;
			}
			set
			{
				this.m_SubstitutionMethod = value;
			}
		}

		public Dictionary<char, char> ReplacementMap { get; private set; } = new Dictionary<char, char>();

		public CharacterSubstitutor.ListSelectionMethod ListMode
		{
			get
			{
				return this.m_ListMode;
			}
			set
			{
				this.m_ListMode = value;
			}
		}

		public List<char> ReplacementList
		{
			get
			{
				return this.m_ReplacementList;
			}
		}

		private int GetRandomSeed(string input)
		{
			return input.GetHashCode();
		}

		internal char ReplaceCharFromMap(char value)
		{
			char result;
			if (this.ReplacementMap != null && this.ReplacementMap.TryGetValue(value, out result))
			{
				return result;
			}
			return value;
		}

		public void OnBeforeSerialize()
		{
			if (this.m_ReplacementsMap == null)
			{
				this.m_ReplacementsMap = new List<CharacterSubstitutor.CharReplacement>();
			}
			this.m_ReplacementsMap.Clear();
			foreach (KeyValuePair<char, char> keyValuePair in this.ReplacementMap)
			{
				this.m_ReplacementsMap.Add(new CharacterSubstitutor.CharReplacement
				{
					original = keyValuePair.Key,
					replacement = keyValuePair.Value
				});
			}
		}

		public void OnAfterDeserialize()
		{
			if (this.ReplacementMap == null)
			{
				this.ReplacementMap = new Dictionary<char, char>();
			}
			this.ReplacementMap.Clear();
			foreach (CharacterSubstitutor.CharReplacement charReplacement in this.m_ReplacementsMap)
			{
				this.ReplacementMap[charReplacement.original] = charReplacement.replacement;
			}
		}

		private void TransformFragment(WritableMessageFragment writableFragment)
		{
			switch (this.Method)
			{
			case CharacterSubstitutor.SubstitutionMethod.ToUpper:
				writableFragment.Text = writableFragment.Text.ToUpper();
				return;
			case CharacterSubstitutor.SubstitutionMethod.ToLower:
				writableFragment.Text = writableFragment.Text.ToLower();
				return;
			case CharacterSubstitutor.SubstitutionMethod.List:
				if (this.m_ReplacementList != null && this.m_ReplacementList.Count != 0)
				{
					if (this.m_ReplacementList.Count == 1)
					{
						writableFragment.Text = new string(this.m_ReplacementList[0], writableFragment.Length);
						return;
					}
					char[] array = new char[writableFragment.Length];
					if (this.ListMode == CharacterSubstitutor.ListSelectionMethod.Random)
					{
						Random.InitState(this.GetRandomSeed(writableFragment.Message.Original));
						for (int i = 0; i < array.Length; i++)
						{
							array[i] = this.m_ReplacementList[Random.Range(0, this.m_ReplacementList.Count)];
						}
					}
					else
					{
						if (this.ListMode == CharacterSubstitutor.ListSelectionMethod.LoopFromStart)
						{
							this.m_ReplacementsPosition = 0;
						}
						int j = 0;
						while (j < array.Length)
						{
							array[j] = this.m_ReplacementList[this.m_ReplacementsPosition % this.m_ReplacementList.Count];
							j++;
							this.m_ReplacementsPosition++;
						}
					}
					writableFragment.Text = new string(array);
				}
				return;
			case CharacterSubstitutor.SubstitutionMethod.Map:
			{
				char[] array2 = new char[writableFragment.Length];
				for (int k = 0; k < array2.Length; k++)
				{
					array2[k] = this.ReplaceCharFromMap(writableFragment[k]);
				}
				writableFragment.Text = new string(array2);
				return;
			}
			default:
				return;
			}
		}

		public void Transform(Message message)
		{
			foreach (MessageFragment messageFragment in message.Fragments)
			{
				WritableMessageFragment writableMessageFragment = messageFragment as WritableMessageFragment;
				if (writableMessageFragment != null)
				{
					this.TransformFragment(writableMessageFragment);
				}
			}
		}

		[SerializeField]
		private CharacterSubstitutor.SubstitutionMethod m_SubstitutionMethod;

		[SerializeField]
		private CharacterSubstitutor.ListSelectionMethod m_ListMode;

		[SerializeField]
		private List<CharacterSubstitutor.CharReplacement> m_ReplacementsMap;

		[SerializeField]
		private List<char> m_ReplacementList = new List<char>
		{
			'_'
		};

		internal int m_ReplacementsPosition;

		public enum SubstitutionMethod
		{
			ToUpper,
			ToLower,
			List,
			Map
		}

		[Serializable]
		private struct CharReplacement
		{
			public char original;

			public char replacement;
		}

		public enum ListSelectionMethod
		{
			Random,
			LoopFromPrevious,
			LoopFromStart
		}
	}
}
